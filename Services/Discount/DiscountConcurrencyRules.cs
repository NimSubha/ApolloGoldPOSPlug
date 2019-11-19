/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.Discount;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.MemoryTables;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using DE = Microsoft.Dynamics.Retail.Pos.DataEntity;
using Microsoft.Dynamics.Retail.Pos.DataEntity.Extensions;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.DiscountService
{
    /// <summary>
    /// This class knows how to take a transaction with possible periodic discounts set on 
    ///  its sales items, and choose which ones should be applied based on their concurrency settings.
    /// </summary>
    public static class DiscountConcurrencyRules
    {
        private struct OfferGroup
        {
            /// <summary>
            /// Offer Id for the Offer
            /// </summary>
            public string OfferId;

            /// <summary>
            /// String identifying unique set of sales lines which much be discounted together.
            ///   This is used by MixAndMatch to identify a particular application of the offer to a group of items.
            /// </summary>
            public string DiscountGroup;
        }

        /// <summary>
        /// Collects concurrency-related settings for an offer
        /// </summary>
        private struct ConcurrencySettings
        {
            /// <summary>
            /// What type of concurrency is set on the offer
            /// </summary>
            public ConcurrencyMode ConcurrencyMode;

            /// <summary>
            /// Can the offer type be compounded (e.g. Unit price/offer price can't be stacked, but discount percent can be)
            /// </summary>
            public bool IsCompoundable;
        }

        /// <summary>
        /// Gets the OfferGroup which is associated to this discount item
        /// </summary>
        /// <param name="discountItem"></param>
        /// <returns></returns>
        private static OfferGroup GetOfferGroup(PeriodicDiscountItem discountItem)
        {
            return new OfferGroup { OfferId = discountItem.OfferId, DiscountGroup = discountItem.PeriodicDiscGroupId };
        }

        /// <summary>
        /// Given a transaction, choose the discounts which should be active on each line
        ///   according to the concurrency rules and add them to the sales items
        /// </summary>
        /// <param name="retailTransaction">Transaction whose discounts and sales items will be processed</param>
        public static void ApplyConcurrencyRules(RetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            // In order to choose the best discount to apply when choosing from multiple we need to know
            //  the effect of the offer on the whole transaction. So we get a list of offers on the transaction
            //  for each type of concurrency, and sort them with largest discounts first.
            List<OfferGroup> sortedExclusiveOffers;
            List<OfferGroup> sortedBestPriceOffers;
            List<OfferGroup> sortedConcurrentOffers;
            GetSortedOfferLists(retailTransaction, out sortedExclusiveOffers, out sortedBestPriceOffers, out sortedConcurrentOffers);

            // Then, starting with exclusive offers, then best price, then compound offers, we find the best that apply.
            //   We keep track of applied offers to given them priority. This ensures that once a line in a discount group (like mix and match)
            //    chooses an offer, the rest of the members of the group will see the offer first.
            //   We keep track of rejected offers to exclude them from consideration. This ensures that once an exclusive offer or best price 
            //    offer is chosen, no other offers one that line are available for use. Example would be if an exclusive multibuy on an item
            //    is activated and a mix and match offer is also available for that item. The exclusive multibuy takes priority and no other
            //    items in that mix and match group should be able to have the mix and match discount.
            HashSet<OfferGroup> appliedOffers = new HashSet<OfferGroup>();
            HashSet<OfferGroup> rejectedOffers = new HashSet<OfferGroup>();
            ApplyPeriodicDiscounts(retailTransaction, ConcurrencyMode.Exclusive, sortedExclusiveOffers, appliedOffers, rejectedOffers);
            ApplyPeriodicDiscounts(retailTransaction, ConcurrencyMode.BestPrice, sortedBestPriceOffers, appliedOffers, rejectedOffers);
            ApplyPeriodicDiscounts(retailTransaction, ConcurrencyMode.Compounded, sortedConcurrentOffers, appliedOffers, rejectedOffers);
        }

        /// <summary>
        /// Given a transaction and some discount processing info, activate the given offers
        ///   to the transaction if applicable
        /// </summary>
        /// <param name="retailTransaction">Transaction to process</param>
        /// <param name="concurrencyMode">For this concurrency mode, try to activate the given offers</param>
        /// <param name="offers">List of offers to attempt to activate</param>
        /// <param name="appliedOffers">Set of offers which have already been activated</param>
        /// <param name="rejectedOffers">Set of offers which have already been rejected</param>
        private static void ApplyPeriodicDiscounts(RetailTransaction retailTransaction, ConcurrencyMode concurrencyMode, List<OfferGroup> offers, HashSet<OfferGroup> appliedOffers, HashSet<OfferGroup> rejectedOffers)
        {
            foreach (var salesItem in retailTransaction.CalculableSalesLines)
            {
                if (salesItem.PeriodicDiscountPossibilities.Count == 0)
                {
                    continue;
                }

                bool wasDiscountApplied = false;
                // if there are any discounts of the given concurrency mode
                if (salesItem.PeriodicDiscountPossibilities.Any(d => d.ConcurrencyMode == concurrencyMode))
                {
                    // apply the discount and return it if applied
                    PeriodicDiscountItem registeredDiscount = ApplyValidDiscountFromOffers(salesItem, offers, appliedOffers, rejectedOffers);
                    
                    wasDiscountApplied = (registeredDiscount != null);

                    // for exclusive and best price computations, if discount was put on item, mark the rest of possibilities as rejected
                    if (concurrencyMode != ConcurrencyMode.Compounded && registeredDiscount != null)
                    {
                        foreach (var discount in salesItem.PeriodicDiscountPossibilities)
                        {
                            if (discount != registeredDiscount)
                            {
                                rejectedOffers.Add(GetOfferGroup(discount));
                            }
                        }
                    }
                }

                // If processing compound discounts, add all non-rejected, non-compoundable, non-applied ones
                if (concurrencyMode == ConcurrencyMode.Compounded)
                {
                    foreach (PeriodicDiscountItem discount in salesItem.PeriodicDiscountPossibilities.Where(d => d.IsCompoundable && d.ConcurrencyMode == ConcurrencyMode.Compounded))
                    {
                        OfferGroup offerGroup = GetOfferGroup(discount);
                        if (!rejectedOffers.Contains(offerGroup) && !appliedOffers.Contains(offerGroup) && !salesItem.DiscountLines.Contains(discount))
                        {
                            salesItem.DiscountLines.AddLast(discount);
                            wasDiscountApplied = true;

                            NetTracer.Information("DiscountConcurrencyRules::ApplyPeriodicDiscounts: Applying periodic discount {0} for item {1} line {2}", discount.OfferId, salesItem.ItemId, salesItem.LineId);
                        }
                    }
                }

                // Set whole quantity of the row as discounted because final discount of this item has now been set
                if (wasDiscountApplied)
                {
                    salesItem.QuantityDiscounted = salesItem.Quantity;
                }
            }
        }

        /// <summary>
        /// Given sales item, and possible offers sorted by priority (biggest discount),
        ///  activate best, valid discount on the item
        /// </summary>
        /// <param name="saleItem">Item to process</param>
        /// <param name="offers">Sorted, possible discount offers</param>
        /// <param name="appliedOffers">Set of offers already applied</param>
        /// <param name="rejectedOffers">Set of offer already rejected</param>
        /// <returns>Discount which was activated or null if none were</returns>
        private static PeriodicDiscountItem ApplyValidDiscountFromOffers(SaleLineItem saleItem, List<OfferGroup> offers, HashSet<OfferGroup> appliedOffers, HashSet<OfferGroup> rejectedOffers)
        {
            PeriodicDiscountItem selectedPeriodicDiscount = null;
            var offersToPickFrom = offers;

            // If some of this item's discounts have already been applied, they get priority
            bool someOfferAlreadyApplied = saleItem.PeriodicDiscountPossibilities.Any(d => appliedOffers.Contains(GetOfferGroup(d)));
            offersToPickFrom = someOfferAlreadyApplied ? offersToPickFrom.Where(o => appliedOffers.Contains(o)).ToList() : offersToPickFrom;

            // If some of this item's discounts have already been rejected, they shouldn't be an option to choose from
            bool someOfferAlreadyRejected = saleItem.PeriodicDiscountPossibilities.Any(d => rejectedOffers.Contains(GetOfferGroup(d)));
            offersToPickFrom = someOfferAlreadyRejected ? offersToPickFrom.Where(o => !rejectedOffers.Contains(o)).ToList() : offersToPickFrom;

            if (offersToPickFrom.Count > 0)
            {
                // Since offersToPickFrom is sorted by discount amount and filtered for availability, 
                //  we need to find the first offer from the available offers which is also a possible item discount.
                selectedPeriodicDiscount = GetFirstPeriodicDiscountFromOffers(saleItem.PeriodicDiscountPossibilities, offersToPickFrom);
                if (selectedPeriodicDiscount != null && !saleItem.DiscountLines.Contains(selectedPeriodicDiscount))
                {
                    // Apply this discount to the item
                    saleItem.DiscountLines.AddLast(selectedPeriodicDiscount);
                    // Mark that we've applied this offer
                    appliedOffers.Add(GetOfferGroup(selectedPeriodicDiscount));

                    NetTracer.Information("DiscountConcurrencyRules::ApplyValidDiscountFromOffers: Applying periodic discount {0} for item {1} line {2}", selectedPeriodicDiscount.OfferId, saleItem.ItemId, saleItem.LineId);
                }
                // if not applied, make sure we return null
                else
                {
                    selectedPeriodicDiscount = null;
                }
            }

            return selectedPeriodicDiscount;
        }

        /// <summary>
        /// Returns the first periodic discount, as ordered by the given list of offers.
        /// Or null, if discount is not among the possible offers.
        /// </summary>
        /// <param name="periodicDiscounts">Possible discounts to apply</param>
        /// <param name="offerList">Sorted list of offers to consider</param>
        /// <returns>First periodic discount found in offers, or null if none found</returns>
        private static PeriodicDiscountItem GetFirstPeriodicDiscountFromOffers(Collection<PeriodicDiscountItem> periodicDiscounts, IEnumerable<OfferGroup> offerList)
        {
            var joinedDiscounts = (from offer in offerList
                                   join discount in periodicDiscounts
                                     on offer equals GetOfferGroup(discount)
                                   select discount).ToList();

            return joinedDiscounts.FirstOrDefault();
        }

        /// <summary>
        /// Initializes three lists of offers, one for each concurrency type,
        ///   sorted with largest discount offers first
        /// </summary>
        /// <param name="retailTransaction">Transaction to collect offers from</param>
        /// <param name="sortedExclusiveOffers">List of exclusive offers</param>
        /// <param name="sortedBestPriceOffers">List of best price offers</param>
        /// <param name="sortedConcurrentOffers">List of compound offers</param>
        private static void GetSortedOfferLists(RetailTransaction retailTransaction,
            out List<OfferGroup> sortedExclusiveOffers,
            out List<OfferGroup> sortedBestPriceOffers,
            out List<OfferGroup> sortedConcurrentOffers)
        {
            var discountedAmountForGroup = SavingsPerOfferGroup(retailTransaction);

            InitializeOfferLists(retailTransaction, out sortedExclusiveOffers, out sortedBestPriceOffers, out sortedConcurrentOffers);

            sortedExclusiveOffers = sortedExclusiveOffers.Distinct().OrderByDescending(g => discountedAmountForGroup[g]).ToList();
            sortedBestPriceOffers = sortedBestPriceOffers.Distinct().OrderByDescending(g => discountedAmountForGroup[g]).ToList();
            sortedConcurrentOffers = sortedConcurrentOffers.Distinct().OrderByDescending(g => discountedAmountForGroup[g]).ToList();
        }

        /// <summary>
        /// Creates three lists (one for each concurrency type) which contain all the offers
        ///  which have been applied to the given transaction.
        /// </summary>
        /// <param name="retailTransaction">Transaction to collect offers from</param>
        /// <param name="sortedExclusiveOffers">List of all exclusive offers</param>
        /// <param name="sortedBestPriceOffers">List of all best price offers</param>
        /// <param name="sortedConcurrentOffers">List of all compound offers</param>
        private static void InitializeOfferLists(RetailTransaction retailTransaction,
            out List<OfferGroup> sortedExclusiveOffers, 
            out List<OfferGroup> sortedBestPriceOffers, 
            out List<OfferGroup> sortedConcurrentOffers)
        {
            sortedExclusiveOffers = new List<OfferGroup>();
            sortedBestPriceOffers = new List<OfferGroup>();
            sortedConcurrentOffers = new List<OfferGroup>();
            List<OfferGroup> sortedAllConcurrentOffers = new List<OfferGroup>();

            var concurrencyForGroup = ConcurrencySettingsPerOfferGroup(retailTransaction);
            foreach (OfferGroup offerGroup in concurrencyForGroup.Keys)
            {
                ConcurrencySettings settings = concurrencyForGroup[offerGroup];
                switch (settings.ConcurrencyMode)
                {
                    case ConcurrencyMode.Exclusive:
                        sortedExclusiveOffers.Add(offerGroup);
                        break;

                    case ConcurrencyMode.BestPrice:
                        sortedBestPriceOffers.Add(offerGroup);
                        break;

                    case ConcurrencyMode.Compounded:
                        sortedAllConcurrentOffers.Add(offerGroup);
                        if (!concurrencyForGroup[offerGroup].IsCompoundable)
                        {
                            sortedConcurrentOffers.Add(offerGroup);
                        }
                        break;
                }
            }

            // If there are best price discounts, compounded discounts count as best price discounts
            if (sortedBestPriceOffers.Count > 0)
            {
                sortedBestPriceOffers.AddRange(sortedAllConcurrentOffers);
            }
        }

        /// <summary>
        /// Find amount saved on the total transaction for each offer group
        /// </summary>
        /// <param name="retailTransaction">Transaction to collect savings info from</param>
        /// <returns>Dictionary allowing lookup of savinvg per offer group</returns>
        private static Dictionary<OfferGroup, Decimal> SavingsPerOfferGroup(RetailTransaction retailTransaction)
        {
            Dictionary<OfferGroup, Decimal> savingsPerGroup = new Dictionary<OfferGroup, Decimal>();

            foreach (var saleItem in retailTransaction.SaleItems)
            {
                foreach (var discountItem in saleItem.PeriodicDiscountPossibilities)
                {
                    OfferGroup offerGroup = GetOfferGroup(discountItem);
                    if (!savingsPerGroup.ContainsKey(offerGroup))
                    {
                        savingsPerGroup[offerGroup] = 0;
                    }

                    savingsPerGroup[offerGroup] += discountItem.GetTotalDiscountAmountForSaleItem(saleItem.Price, saleItem.Quantity);
                }
            }

            return savingsPerGroup;
        }

        /// <summary>
        /// Return dictionary that, given offergroup, call tell the concurrency mode of the offer
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        private static Dictionary<OfferGroup, ConcurrencySettings> ConcurrencySettingsPerOfferGroup(RetailTransaction retailTransaction)
        {
            Dictionary<OfferGroup, ConcurrencySettings> concurrencySettingsPerGroup = new Dictionary<OfferGroup, ConcurrencySettings>();

            foreach (var saleItem in retailTransaction.SaleItems)
            {
                foreach (var discountItem in saleItem.PeriodicDiscountPossibilities)
                {
                    OfferGroup offerGroup = GetOfferGroup(discountItem);
                    if (!concurrencySettingsPerGroup.ContainsKey(offerGroup))
                    {
                        concurrencySettingsPerGroup.Add(offerGroup,
                            new ConcurrencySettings
                            {
                                ConcurrencyMode = discountItem.ConcurrencyMode,
                                IsCompoundable = discountItem.IsCompoundable
                            });
                    }
                }
            }

            return concurrencySettingsPerGroup;
        }
    }
}
