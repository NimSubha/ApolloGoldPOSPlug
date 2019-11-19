/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.DiscountService
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using LSRetailPosis.Transaction;
    using LSRetailPosis.Transaction.Line.Discount;
    using LSRetailPosis.Transaction.Line.SaleItem;
    using Microsoft.Dynamics.Retail.Pos.Contracts;
    using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

    /// <summary>
    /// This class is used to find and populate multiline trade agreement discounts on a transaction
    /// </summary>
    internal sealed class MultilineDiscountCalculator
    {
        /// <summary>
        /// Create new instance of <see cref="MultilineDiscountCalculator"/> class with given configurations
        /// </summary>
        /// <param name="discountParameters">Configuration dictating which trade agreement discount combinations are active</param>
        /// <param name="application">Reference to application to call other services and utilities</param>
        /// <param name="discountService">Reference to discount service with cached trade agreement lookup</param>
        /// <param name="channelCurrency">Currency code assigned to this store</param>
        /// <param name="companyCurrency">Currency code assigned to this legal entity</param>
        public MultilineDiscountCalculator(DiscountParameters discountParameters, IApplication application, Discount discountService, string channelCurrency, string companyCurrency)
        {
            this.DiscountParameters = discountParameters;
            this.ChannelCurrency = channelCurrency;
            this.CompanyCurrency = companyCurrency;
            this.Application = application;
            this.DiscountService = discountService;
        }

        /// <summary>
        /// Hide the default constructor
        /// </summary>
        private MultilineDiscountCalculator() { }

        private string ChannelCurrency { get; set; }
        private string CompanyCurrency { get; set; }
        private DiscountParameters DiscountParameters { get; set; }
        private IApplication Application;
        private Discount DiscountService;

        /// <summary>
        /// The calculation of a customer multiline discount.
        /// </summary>
        /// <remarks>
        /// Calculation of multiline discount is done as follows:
        ///   1. Create working table for calculation
        ///   2. Populate working table with total quantities for all the multiline groups encountered on the sales lines
        ///   3. For all rows (and therefore multiline groups) found, search for trade agreements in the database
        ///      a. The search is first for customer-specific, then customer multiline discount group, then all customers
        ///      b. The search stops when a trade agreement is encountered with "Find next" unmarked
        ///      c. If nothing is found for the store currency the search is attempted again with the company accounting currency
        ///   4. All found agreements are summed in the working table and applied to each sales line which matches the multiline groups
        ///   5. If there are sales lines which weren't discounted with a multiline discount
        ///      a. Find their total quantity and search for any multiline trade agreements marked for "All items"
        ///      b. If any agreements were found apply them to any lines that weren't already discounted with a multiline discount
        /// </remarks>
        /// <param name="transaction">The transaction which will have multiline discount populated on it</param>
        public RetailTransaction CalcMultiLineDiscount(RetailTransaction transaction)
        {
            using (DataTable multiLineDiscTable = MakeMultiLineDiscTable())
            {
                // clear working table for multiline discount and repopulate with sums for each multiline discount group
                InitializeMultilineDiscTable(multiLineDiscTable, transaction);

                // collection of salesLine not discounted by multiline discount group
                var nondiscountedSalesLines = new List<SaleLineItem>(transaction.CalculableSalesLines);

                //Find discounts for the different multiline discount groups
                foreach (DataRow nextRow in multiLineDiscTable.Rows)
                {
                    // we've found some multiline discount groups, so clear non-discounted lines from the default of "all"
                    nondiscountedSalesLines.Clear();

                    // find multiline discounts for this multiline discount row
                    DataRow mlRow = GetMultilineDiscountLineForCurrencies(PriceDiscItemCode.GroupId, transaction, nextRow, this.ChannelCurrency, this.CompanyCurrency);

                    //Update the sale items.
                    foreach (var saleItem in transaction.CalculableSalesLines)
                    {
                        string discountGroupId = saleItem.MultiLineDiscountGroup;
                        if (discountGroupId == mlRow.Field<string>("MULTILINEGROUP"))
                        {
                            // if line is part of discounted item group, apply the discount
                            ApplyMultilineDiscount(saleItem, mlRow);
                        }
                        else
                        {
                            // otherwise, add to non-discounted lines
                            nondiscountedSalesLines.Add(saleItem);
                        }
                    }
                }

                // find total quantity of items on lines still eligible for multiline discount
                decimal lineSum = nondiscountedSalesLines.Aggregate(0M, (acc, sl) => acc + sl.Quantity);

                // find any multiline discounts to apply to "all items"
                DataRow emptyRow = InitializeMultiLineDiscTableRow(multiLineDiscTable, String.Empty, lineSum);
                DataRow discountRow = GetMultilineDiscountLineForCurrencies(PriceDiscItemCode.All, transaction, emptyRow, this.ChannelCurrency, this.CompanyCurrency);

                //Update the sale items.
                foreach (var saleItem in nondiscountedSalesLines)
                {
                    ApplyMultilineDiscount(saleItem, discountRow);
                }
            }

            return transaction;
        }

        /// <summary>
        /// Create the MultiLineDiscTable in memory. 
        /// This is the working data for collecting quantities and discounts per multiline item discount group Id.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification= "Disposed by caller")]
        private static DataTable MakeMultiLineDiscTable()
        {
            // Create a new DataTable.
            DataTable discountTable = new DataTable("MULTILINEDISCTABLE");
            DataColumn column;

            //Adding MultiLineGrup
            column = discountTable.Columns.Add("MULTILINEGROUP", typeof(System.String));
            column.AutoIncrement = false;
            column.Caption = "MultiLineGroup";
            column.ReadOnly = false;
            column.Unique = true;

            //Adding Quantity
            column = discountTable.Columns.Add("QUANTITY", typeof(System.Decimal));
            column.AutoIncrement = false;
            column.Caption = "Quantity";
            column.ReadOnly = false;
            column.Unique = false;

            //Adding Percent1
            column = discountTable.Columns.Add("PERCENT1", typeof(System.Decimal));
            column.AutoIncrement = false;
            column.Caption = "Percent1";
            column.ReadOnly = false;
            column.Unique = false;

            //Adding Percent2
            column = discountTable.Columns.Add("PERCENT2", typeof(System.Decimal));
            column.AutoIncrement = false;
            column.Caption = "Percent2";
            column.ReadOnly = false;
            column.Unique = false;

            //Adding Amount
            column = discountTable.Columns.Add("AMOUNT", typeof(System.Decimal));
            column.AutoIncrement = false;
            column.Caption = "Amount";
            column.ReadOnly = false;
            column.Unique = false;

            //Adding min Quantity for activation
            column = discountTable.Columns.Add("MINQTY", typeof(System.Decimal));
            column.AutoIncrement = false;
            column.Caption = "MinQty";
            column.ReadOnly = false;
            column.Unique = false;

            return discountTable;
        }

        /// <summary>
        /// Find and total all multiline discount trade agreements that match the given relations and quantity
        /// </summary>
        /// <param name="itemCode">The item code to search by (item group or all)</param>
        /// <param name="retailTransaction">The transaction context with Id and customer Id</param>
        /// <param name="discountRow">Current row in multiline discount working table. Will be populated and returned.</param>
        /// <param name="currencyCode">The currency code to filter by</param>
        /// <returns>Discount row populated with sums for all discounts found</returns>
        private DataRow GetMultiLineDiscountLine(PriceDiscItemCode itemCode, RetailTransaction retailTransaction, DataRow discountRow, string currencyCode)
        {
            PriceDiscType relation = PriceDiscType.MultiLineDiscSales; //Sales multiline discount - 6
            Dimensions dimension = (Dimensions)this.Application.BusinessLogic.Utility.CreateDimension();

            bool searchAgain = true;
            var codes = new PriceDiscAccountCode[] { PriceDiscAccountCode.Table, PriceDiscAccountCode.GroupId, PriceDiscAccountCode.All };
            foreach (var accountCode in codes)
            {
                // skip to next configuration if this one isn't enabled
                if (!DiscountParameters.Activation(relation, accountCode, itemCode))
                {
                    continue;
                }

                // get item relation based on item code
                string itemRelation = (itemCode == PriceDiscItemCode.GroupId) ? discountRow.Field<string>("MULTILINEGROUP") : string.Empty;
                itemRelation = itemRelation ?? String.Empty;

                // get customer relation based on account code
                string accountRelation = String.Empty;
                if (accountCode == PriceDiscAccountCode.Table)
                {
                    accountRelation = retailTransaction.Customer.CustomerId;
                }
                else if (accountCode == PriceDiscAccountCode.GroupId)
                {
                    accountRelation = retailTransaction.Customer.MultiLineDiscountGroup;
                }
                accountRelation = accountRelation ?? String.Empty;

                // if both relations are valid for the given item and account codes, look for trade agreements matching these relations
                if ((Discount.ValidRelation(accountCode, accountRelation)) &&
                    (Discount.ValidRelation(itemCode, itemRelation)))
                {
                    // get any active multiline discount trade agreement matching relations and quantity
                    decimal quantityAmount = discountRow.Field<decimal>("QUANTITY");
                    var priceDiscTable = this.DiscountService.GetPriceDiscDataCached(
                        retailTransaction.TransactionId, relation, itemRelation, accountRelation, (int)itemCode, (int)accountCode, quantityAmount, currencyCode, dimension, false);

                    // compute running sum of discount values found
                    foreach (Discount.DiscountAgreementArgs row in priceDiscTable)
                    {
                        discountRow["PERCENT1"] = discountRow.Field<decimal>("PERCENT1");
                        discountRow["PERCENT1"] = discountRow.Field<decimal>("PERCENT1") + row.Percent1;
                        discountRow["PERCENT2"] = discountRow.Field<decimal>("PERCENT2") + row.Percent2;
                        discountRow["AMOUNT"] = discountRow.Field<decimal>("AMOUNT") + row.Amount;
                        discountRow["MINQTY"] = discountRow.Field<decimal>("MINQTY") + row.QuantityAmountFrom;

                        // stop search when we find a trade agreement set to not find next trade agreement
                        if (!row.SearchAgain)
                        {
                            searchAgain = false;
                        }
                    }
                }

                // stop search if we found a discount without "find next" marked
                if (!searchAgain)
                {
                    break;
                }
            }

            return discountRow;
        }

        /// <summary>
        /// Retrieve all multiline discount amounts for given item code and transaction.
        /// If nothing is found for channel currency, fall back to company currency to search
        /// </summary>
        /// <param name="itemCode">The item code to search by (item group or all)</param>
        /// <param name="transaction">The transaction context containing Id and customer Id</param>
        /// <param name="discountRow">The working multiline discount row to populate</param>
        /// <param name="channelCurrency">The channel currency of the current channel</param>
        /// <param name="companyCurrency">The company currency of the channel's company</param>
        /// <returns>Discount row populated with total discount amounts found</returns>
        private DataRow GetMultilineDiscountLineForCurrencies(
            PriceDiscItemCode itemCode, 
            RetailTransaction transaction, 
            DataRow discountRow, 
            string channelCurrency, 
            string companyCurrency)
        {
            DataRow mlRow = this.GetMultiLineDiscountLine(itemCode, transaction, discountRow, channelCurrency);

            if (mlRow.Field<decimal>("PERCENT1") == 0M
                && mlRow.Field<decimal>("PERCENT2") == 0M
                && mlRow.Field<decimal>("AMOUNT") == 0M
                && (!channelCurrency.Equals(companyCurrency, StringComparison.OrdinalIgnoreCase)))
            {
                mlRow = this.GetMultiLineDiscountLine(itemCode, transaction, discountRow, companyCurrency);
                mlRow["AMOUNT"] = this.Application.Services.Currency.CurrencyToCurrency(this.CompanyCurrency, this.ChannelCurrency, mlRow.Field<decimal>("AMOUNT"));
            }

            return mlRow;
        }

        /// <summary>
        /// Initializes the given multiline discount working table. It is filled with 
        /// the quantities of items on the transaction which belong to each multiline discount group.
        /// </summary>
        /// <param name="multiLineDiscTable">The working table which will be initialized</param>
        /// <param name="transaction">The transaction to initialize from</param>
        private static void InitializeMultilineDiscTable(DataTable multiLineDiscTable, RetailTransaction transaction)
        {
            // start with empty working table
            multiLineDiscTable.Clear();

            //Sum upp all the linegroup discount lines in the same group
            foreach (var saleItem in transaction.CalculableSalesLines)
            {
                if (!saleItem.Voided)
                {
                    // collect quantities for multi-line discount groups
                    if (!string.IsNullOrEmpty(saleItem.MultiLineDiscountGroup))
                    {
                        UpdateMultiLineDiscTable(multiLineDiscTable, saleItem.MultiLineDiscountGroup, saleItem.Quantity);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the multilineGroup memory table, that is used in the customer multiline calculation.
        /// </summary>
        /// <param name="multiLineDiscTable">Working table containing totals for multiline discount groups on the transaction</param>
        /// <param name="multiLineGroup">The multiline group.</param>
        /// <param name="quantity">The item quantity.</param>
        private static void UpdateMultiLineDiscTable(DataTable multiLineDiscTable, string multiLineGroup, decimal quantity)
        {
            bool rowfound = false;

            foreach (DataRow tableRow in multiLineDiscTable.Rows)
            {
                if (multiLineGroup == tableRow.Field<string>("MULTILINEGROUP"))
                {
                    tableRow["QUANTITY"] = tableRow.Field<decimal>("QUANTITY") + quantity;
                    rowfound = true;
                }
            }
            //If multiline group is not found then add a new row.
            if (!rowfound)
            {
                var row = InitializeMultiLineDiscTableRow(multiLineDiscTable, multiLineGroup, quantity);
                multiLineDiscTable.Rows.Add(row);
            }
        }

        private static DataRow InitializeMultiLineDiscTableRow(DataTable multiLineDiscTable, string multiLineGroup, decimal quantity)
        {
            DataRow row;
            row = multiLineDiscTable.NewRow();
            row["MULTILINEGROUP"] = multiLineGroup;
            row["QUANTITY"] = quantity;
            row["PERCENT1"] = 0M;
            row["PERCENT2"] = 0M;
            row["AMOUNT"] = 0M;
            row["MINQTY"] = 0M;

            return row;
        }

        /// <summary>
        /// Apply the given multiline discount row to the given sales line if discount amounts have been specified
        /// </summary>
        /// <param name="salesLine">The sales line which will recieve the discount</param>
        /// <param name="discountRow">The discount row from which to create the line discount</param>
        private void ApplyMultilineDiscount(SaleLineItem salesLine, DataRow discountRow)
        {
            if (discountRow.Field<decimal>("PERCENT1") > 0M || discountRow.Field<decimal>("PERCENT2") > 0M || discountRow.Field<decimal>("AMOUNT") > 0M)
            {
                CustomerDiscountItem discountItem = (CustomerDiscountItem)this.Application.BusinessLogic.Utility.CreateCustomerDiscountItem();
                discountItem.LineDiscountType = LineDiscountItem.DiscountTypes.Customer;
                discountItem.CustomerDiscountType = CustomerDiscountItem.CustomerDiscountTypes.MultiLineDiscount;
                discountItem.Percentage = (1 - (1 - (discountRow.Field<decimal>("PERCENT1") / 100)) * (1 - (discountRow.Field<decimal>("PERCENT2") / 100))) * 100;
                discountItem.Amount = discountRow.Field<decimal>("AMOUNT");

                Discount.UpdateDiscountLines(salesLine, discountItem);
            }
        }
    }
}

