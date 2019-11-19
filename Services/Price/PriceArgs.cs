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
using System.Diagnostics.CodeAnalysis;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.PriceService
{
    public class PromotionInfo
    {
        public string PromoId { get; set; }
        public DiscountMethod DiscountMethod { get; set; }
        public string DiscValidationPeriod { get; set; }
        public decimal PriceInclTax { get; set; }
        public decimal Price { get; set; }
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pct", Justification = "Cannot change public API.")]
        public decimal MaxDiscPct { get; set; }
        public decimal MaxDiscAmount { get; set; }
        public int Priority { get; set; }
        public ConcurrencyMode Concurrency { get; set; }
        internal Price.DateValidationTypes ValidationType { get; set; }
        internal DateTime ValidFrom { get; set; }
        internal DateTime ValidTo { get; set; }
    }

    /// <summary>
    /// Enumeration to capture whether a customer or price-group price has specified an override value for Price-includes-tax
    /// </summary>
    internal enum PriceGroupIncludesTax
    {
        /// <summary>
        /// 0
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// 1
        /// </summary>
        PriceExcludesTax = 1,

        /// <summary>
        /// 2
        /// </summary>
        PriceIncludesTax = 2
    }

    /// <summary>
    /// Result from a price lookup, contains both the Price value and whether or not that price definition has specified an value for 'Price includes tax'
    /// </summary>
    internal struct PriceResult
    {
        /// <summary>
        /// Price value.
        /// </summary>
        public readonly decimal Price;

        /// <summary>
        /// Whether or not the price includes taxes.
        /// </summary>
        public readonly PriceGroupIncludesTax IncludesTax;

        /// <summary>
        /// Max. retail price.
        /// </summary>
        public decimal IndiaMRP;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="price"></param>
        /// <param name="includesTax"></param>
        public PriceResult(decimal price, PriceGroupIncludesTax includesTax)
        {
            Price = price;
            IncludesTax = includesTax;

            IndiaMRP = 0;
        }

        /// <summary>
        /// Construct a new PriceResult object by converting an existing PriceResult from one currency to another.
        /// </summary>
        /// <param name="result">existing Price Result instance to be copied</param>
        /// <param name="fromCurrencyCode">currency to convert from</param>
        /// <param name="toCurrencyCode">currency to convert to</param>
        public PriceResult(PriceResult result, string fromCurrencyCode, string toCurrencyCode)
            : this(PriceService.Price.InternalApplication.Services.Currency.CurrencyToCurrency(fromCurrencyCode, toCurrencyCode, result.Price), result.IncludesTax)
        {
        }
    }

    /// <summary>
    /// Encapsulates the trade agreement activation settings and how to read them
    /// </summary>
    internal struct PriceParameters
    {
        public readonly bool salesPriceAccountItem;
        public readonly bool salesPriceGroupItem;
        public readonly bool salesPriceAllItem;

        public PriceParameters(bool accountItem, bool groupItem, bool allItem)
        {
            this.salesPriceAccountItem = accountItem;
            this.salesPriceGroupItem = groupItem;
            this.salesPriceAllItem = allItem;
        }

        /// <summary>
        /// Returns if a certain relation is active for a price search.
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        public bool IsRelationActive(PriceDiscAccountCode accountCode, PriceDiscItemCode itemCode)
        {
            switch (accountCode)
            {
                case PriceDiscAccountCode.Table:
                    switch (itemCode)
                    {
                        case PriceDiscItemCode.Table: return this.salesPriceAccountItem;
                        case PriceDiscItemCode.GroupId: return false;
                        case PriceDiscItemCode.All: return false;
                    }
                    break;

                case PriceDiscAccountCode.GroupId:
                    switch (itemCode)
                    {
                        case PriceDiscItemCode.Table: return this.salesPriceGroupItem;
                        case PriceDiscItemCode.GroupId: return false;
                        case PriceDiscItemCode.All: return false;
                    }
                    break;

                case PriceDiscAccountCode.All:
                    switch (itemCode)
                    {
                        case PriceDiscItemCode.Table: return this.salesPriceAllItem;
                        case PriceDiscItemCode.GroupId: return false;
                        case PriceDiscItemCode.All: return false;
                    }
                    break;
            }

            return false;
        }
    }

    /// <summary>
    /// Arguments for a Retail price lookup operation
    /// </summary>
    internal struct RetailPriceArgs
    {
        public string CustomerId;
        public ReadOnlyCollection<string> PriceGroups;
        public string CurrencyCode;
        public decimal Quantity;
        public string ItemId;
        public string Barcode;
        public string InventUOM;
        public string SalesUOM;
        public UnitQtyConversion UnitQtyConversion;
        public Dimensions Dimensions;
        // Included SerialID as part of RetailPriceArgs
        public string SerialId;

        /// <summary>
        /// Convert to PriceAgreementArgs using the Sale UOM
        /// </summary>
        /// <returns></returns>
        public PriceAgreementArgs ArgreementArgsForSale()
        {
            return new PriceAgreementArgs()
            {
                CurrencyCode = this.CurrencyCode,
                CustomerId = this.CustomerId,
                Dimensions = this.Dimensions,
                ItemId = this.ItemId,
                PriceGroups = this.PriceGroups,
                Quantity = this.Quantity,
                UnitOfMeasure = this.SalesUOM,
                SerialId = this.SerialId
            };
        }

        /// <summary>
        /// Convert to PriceAgreementArgs using the Inventory UOM
        /// </summary>
        /// <returns></returns>
        public PriceAgreementArgs AgreementArgsForInventory()
        {
            return new PriceAgreementArgs()
            {
                CurrencyCode = this.CurrencyCode,
                CustomerId = this.CustomerId,
                Dimensions = this.Dimensions,
                ItemId = this.ItemId,
                PriceGroups = this.PriceGroups,
                Quantity = this.Quantity,
                UnitOfMeasure = this.InventUOM,
                SerialId = this.SerialId
            };
        }
    }

    /// <summary>
    /// Arguments for a Price Agreement lookup operation and methods for reading
    /// </summary>
    internal struct PriceAgreementArgs
    {
        public string CustomerId;
        public ReadOnlyCollection<string> PriceGroups;
        public string CurrencyCode;
        public decimal Quantity;
        public string ItemId;
        public string UnitOfMeasure;
        public Dimensions Dimensions;
        public string SerialId;

        /// <summary>
        /// Gets price agreement 'item relation' based on args and given item relation code
        /// </summary>
        /// <param name="args">Price agreement args (item, customer, etc.)</param>
        /// <param name="itemCode">Item relation code (item/group/all)</param>
        /// <returns>Returns item if 'item' relation code given, otherwise empty string</returns>
        public string GetItemRelation(PriceDiscItemCode itemCode)
        {
            string itemRelation = String.Empty;
            if (itemCode == PriceDiscItemCode.Table && !String.IsNullOrEmpty(this.ItemId))
            {
                itemRelation = this.ItemId;
            }
            return itemRelation;
        }

        /// <summary>
        /// Gets price agreement 'account relations' based on args and given account relation code
        /// </summary>
        /// <param name="args">Price agreement args (item, customer, etc.)</param>
        /// <param name="accountCode">Account relation code (customer/price group/all)</param>
        /// <returns>Returns customer if 'customer' code given, price groups if 'group' code given, otherwise empty</returns>
        public ReadOnlyCollection<string> GetAccountRelations(PriceDiscAccountCode accountCode)
        {
            ReadOnlyCollection<string> accountRelations = new ReadOnlyCollection<string>(new List<string> { String.Empty });
            if (accountCode == PriceDiscAccountCode.Table && !String.IsNullOrEmpty(this.CustomerId))
            {
                accountRelations = new ReadOnlyCollection<string>(new List<string> { this.CustomerId });
            }
            else if (accountCode == PriceDiscAccountCode.GroupId && this.PriceGroups.Count > 0)
            {
                accountRelations = this.PriceGroups;
            }
            return accountRelations;
        }

        /// <summary>
        /// Gets price agreement unit of measure based on args and given item relation code
        /// </summary>
        /// <param name="args">Price agreement args (item, customer, uom, etc.)</param>
        /// <param name="itemCode">Item relation code (item/group/all)</param>
        /// <returns>Return unit of measure id if 'item' code specified, otherwise empty</returns>
        public string GetUnitId(PriceDiscItemCode itemCode)
        {
            return (itemCode == PriceDiscItemCode.Table && !String.IsNullOrEmpty(this.UnitOfMeasure) ? this.UnitOfMeasure : String.Empty);
        }
    }
}