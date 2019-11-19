/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Types of discounts
    /// </summary>
    public enum DiscountType
    {
        SummaryDiscount,
        LineDiscount,
        PeriodicDiscount,
        ReceiptDiscount,
        RoundingDiscount,
        LoyaltyDiscount
    }

    /// <summary>
    /// Helper class for discounts printing.
    /// </summary>
    public static class DiscountHelper
    {
        /// <summary>
        /// Gets discount amount of the selected type from the sales line.
        /// </summary>
        /// <param name="discountType">Discount type</param>
        /// <param name="salesLine">Sales line</param>
        /// <returns>Discount amount</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Design")]
        public static decimal GetSalesLineDiscountAmount(DiscountType discountType, ISaleLineItem salesLine)
        {
            if (salesLine == null)
            {
                throw new ArgumentNullException("salesLine");
            }

            decimal discountAmount;

            switch (discountType)
            {
                case DiscountType.SummaryDiscount:
                    discountAmount = salesLine.LineDiscount + salesLine.PeriodicDiscount + salesLine.TotalDiscount;
                    break;
                case DiscountType.LineDiscount:
                    discountAmount = salesLine.LineDiscount;
                    break;
                case DiscountType.PeriodicDiscount:
                    discountAmount = salesLine.PeriodicDiscount;
                    break;
                case DiscountType.ReceiptDiscount:
                    discountAmount = salesLine.TotalDiscount;
                    break;
                case DiscountType.RoundingDiscount:
                    discountAmount = decimal.Zero;
                    break;
                case DiscountType.LoyaltyDiscount:
                    discountAmount = salesLine.LoyaltyDiscount;
                    break;
                default:
                    throw new ArgumentException();
            }

            return discountAmount;
        }

        /// <summary>
        /// Gets discount percent of the selected type from the sales line
        /// </summary>
        /// <param name="discountType">Discount type</param>
        /// <param name="salesLine">Sales line</param>
        /// <returns>Discount percent</returns>
        public static decimal GetSalesLineDiscountPercent(DiscountType discountType, ISaleLineItem salesLine)
        {
            if (salesLine == null)
            {
                throw new ArgumentNullException("salesLine");
            }

            decimal discountPercent;

            switch (discountType)
            {
                case DiscountType.SummaryDiscount:
                    discountPercent = salesLine.GrossAmount != 0m ? GetSalesLineDiscountAmount(DiscountType.SummaryDiscount, salesLine) / salesLine.GrossAmount * 100m : 0m;
                    break;
                case DiscountType.LineDiscount:
                    discountPercent = salesLine.LinePctDiscount;
                    break;
                case DiscountType.PeriodicDiscount:
                    discountPercent = salesLine.PeriodicPctDiscount;
                    break;
                case DiscountType.ReceiptDiscount:
                    discountPercent = salesLine.TotalPctDiscount;
                    break;
                case DiscountType.RoundingDiscount:
                    discountPercent = decimal.Zero;
                    break;
                case DiscountType.LoyaltyDiscount:
                    discountPercent = salesLine.LoyaltyPercentageDiscount;
                    break;
                default:
                    throw new ArgumentException();
            }

            return discountPercent;
        }

        /// <summary>
        /// Gets discount amount of the selected type from the retail transaction
        /// </summary>
        /// <param name="discountType">Discount type</param>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <returns>Discount amount</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Design")]
        public static decimal GetRetailTransactionDiscountAmount(DiscountType discountType, IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            decimal discountAmount;

            switch (discountType)
            {
                case DiscountType.SummaryDiscount:
                    discountAmount = retailTransaction.LineDiscount + retailTransaction.PeriodicDiscountAmount + retailTransaction.TotalDiscount;
                    break;
                case DiscountType.LineDiscount:
                    discountAmount = retailTransaction.LineDiscount;
                    break;
                case DiscountType.PeriodicDiscount:
                    discountAmount = retailTransaction.PeriodicDiscountAmount;
                    break;
                case DiscountType.ReceiptDiscount:
                    discountAmount = retailTransaction.TotalDiscount;
                    break;
                case DiscountType.RoundingDiscount:
                    discountAmount = retailTransaction.RoundingSalePmtDiff;
                    break;
                case DiscountType.LoyaltyDiscount:
                    discountAmount = retailTransaction.LoyaltyDiscount;
                    break;
                default:
                    throw new ArgumentException();
            }

            return discountAmount;
        }

        /// <summary>
        /// Gets discount percent of selected type from retail transaction
        /// </summary>
        /// <param name="discountType">Discount type</param>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <returns>Discount percent</returns>
        public static decimal GetRetailTransactionDiscountPercent(DiscountType discountType, IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            decimal discountBase = 0m;

            switch (discountType)
            {
                case DiscountType.SummaryDiscount:
                    discountBase = retailTransaction.GrossAmount;
                    break;
                case DiscountType.LineDiscount:
                    discountBase = retailTransaction.GrossAmount - retailTransaction.PeriodicDiscountAmount;
                    break;
                case DiscountType.PeriodicDiscount:
                    discountBase = retailTransaction.GrossAmount;
                    break;
                case DiscountType.ReceiptDiscount:
                    discountBase = retailTransaction.GrossAmount - retailTransaction.PeriodicDiscountAmount - retailTransaction.LineDiscount;
                    break;
                case DiscountType.RoundingDiscount:
                    discountBase = retailTransaction.GrossAmount;
                    break;
                case DiscountType.LoyaltyDiscount:
                    discountBase = retailTransaction.GrossAmount - retailTransaction.PeriodicDiscountAmount - retailTransaction.LineDiscount - retailTransaction.TotalDiscount;
                    break;
                default:
                    throw new ArgumentException();
            }

            decimal discountPercent = discountBase != 0m ? GetRetailTransactionDiscountAmount(discountType, retailTransaction) / discountBase * 100m : 0m;

            return discountPercent;
        }

        /// <summary>
        /// Determines that sales line has some discounts
        /// </summary>
        /// <param name="salesLine">Sales line</param>
        /// <returns>true if sales line has any discount; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Design")]
        public static bool SalesLineHasDiscount(ISaleLineItem salesLine)
        {
            if (salesLine == null)
            {
                throw new ArgumentNullException("salesLine");
            }

            return salesLine.LineDiscount != 0m ||
                salesLine.PeriodicDiscount != 0m ||
                salesLine.TotalDiscount != 0m ||
                salesLine.LoyaltyDiscount != 0m;
        }

        /// <summary>
        /// Determines that retail transaction has some discounts
        /// </summary>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <returns>true if sales line has any discount; false otherwise.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Design")]
        public static bool RetailTransactionHasDiscount(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            return retailTransaction.LineDiscount != 0m ||
                retailTransaction.PeriodicDiscountAmount != 0m ||
                retailTransaction.TotalDiscount != 0m ||
                retailTransaction.LoyaltyDiscount != 0m;
        }

        /// <summary>
        /// Update tax lines with amounts from totals header discounts
        /// </summary>
        public static void DistributeTaxFromTotalsHeaderDiscounts()
        {
            var discountDataList = RussianFiscalPrinterDriver.FiscalPrinterDriver.GetTotalsHeaderDiscountLines();
            var discountSalesLines = RussianFiscalPrinterDriver.FiscalPrinterDriver.GetDiscountSalesLines();

            if (discountDataList == null || discountSalesLines == null)
            {
                return;
            }

            foreach (var discountData in discountDataList)
            {
                var taxAdjustments = discountData.DiscountTaxAmounts;
                var adjustedTaxValues = new Dictionary<string, decimal>();
                var maxTaxItems = new Dictionary<string, ITaxItem>();

                // distributing tax adjustments between sales lines related to current totals header discount
                foreach (var salesLineAndDiscount in (from salesLine in discountSalesLines[discountData.PrinterTaxGroup]
                                                      let discountAmount = DiscountHelper.GetSalesLineDiscountAmount(discountData.DiscountType, salesLine)
                                                      where discountAmount != 0m
                                                      select new { DiscountAmount = discountAmount, SalesLine = salesLine }))
                {
                    foreach (var tax in salesLineAndDiscount.SalesLine.TaxLines)
                    {
                        var printerTaxCodeId = GetPrinterTaxIdFromTaxCode(tax.TaxCode);

                        if (!maxTaxItems.ContainsKey(printerTaxCodeId))
                        {
                            maxTaxItems.Add(printerTaxCodeId, tax);
                        }

                        if (Math.Abs(maxTaxItems[printerTaxCodeId].Amount) < Math.Abs(tax.Amount))
                        {
                            maxTaxItems[printerTaxCodeId] = tax;
                        }

                        var taxAdjustment = (discountData.DiscountAmount != 0m) ? RoundAmount(taxAdjustments[printerTaxCodeId] * salesLineAndDiscount.DiscountAmount / discountData.DiscountAmount) : 0m;

                        tax.Amount += taxAdjustment;

                        if (!adjustedTaxValues.ContainsKey(printerTaxCodeId))
                        {
                            adjustedTaxValues.Add(printerTaxCodeId, 0m);
                        }

                        adjustedTaxValues[printerTaxCodeId] += taxAdjustment;
                    }
                }

                // adding pennydiff to tax with maximum tax amount value
                foreach (var taxItem in maxTaxItems)
                {
                    var taxAdjustment = taxAdjustments[taxItem.Key] - adjustedTaxValues[taxItem.Key];
                    taxItem.Value.Amount += taxAdjustment;
                }
            }
        }

        /// <summary>
        /// Gets printer taxId for POS tax code
        /// </summary>
        /// <param name="taxCode">Tax code</param>
        /// <returns>Printer taxId</returns>
        private static string GetPrinterTaxIdFromTaxCode(string taxCode)
        {
            return RussianFiscalPrinter.GetPrinterTaxIdFromTaxCode(taxCode);
        }

        /// <summary>
        /// Rounds amount through PosApplication rounding service
        /// </summary>
        /// <param name="amount">Amount</param>
        /// <returns>Rounded amount</returns>
        private static decimal RoundAmount(decimal amount)
        {
            return PosApplication.Instance.Services.Rounding.Round(amount);
        }
    }
}
