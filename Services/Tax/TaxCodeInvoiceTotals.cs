/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    using System.Collections.Generic;
    using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;

    /// <summary>
    /// Tax Code Marginal Base Support: Net amount of invoice balance.
    /// The class to keep track of the current rounded and non-rounded tax code totals for the invoice
    /// in order to use these values later to adjust tax code amount on the sales line
    /// if there is any discrepancy appears between these values on a given sales line tax iteration.
    /// </summary>
    public class TaxCodeInvoiceTotals
    {
        private Dictionary<string, Totals> currentTotals;

        internal TaxCodeInvoiceTotals()
        {
            this.currentTotals = new Dictionary<string, Totals>();
        }

        /// <summary>
        /// Keeps track of invoice totals aggregated by tax codes.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <param name="taxCodeAmountNonRounded">The non-rounded tax code amount.</param>
        /// <param name="taxCodeAmountRounded">The rounded tax code amount.</param>
        /// <returns>The new rounded value.</returns>
        internal decimal AddTaxCodeAmountToInvoiceAndAdjust(TaxCode taxCode, decimal taxCodeAmountNonRounded, decimal taxCodeAmountRounded)
        {
            if (taxCode.TaxLimitBase != TaxLimitBase.InvoiceWithoutVat)
            {
                // we need to keep track of totals for certain tax codes only.
                return taxCodeAmountRounded;
            }

            // a. Calculate current TaxCode.TotalAmount by summing up all non-rounded, [all previous and current] taxLines.taxCode.Amounts. 
            // That’s what we supposed to collect as a Tax for a given tax code.
            Totals codeInvoiceTotals;
            if (this.currentTotals.TryGetValue(taxCode.Code, out codeInvoiceTotals))
            {
                // previous invoice lines exists, add amount to them
                codeInvoiceTotals.TotalInvoiceNonRoundedValue += taxCodeAmountNonRounded;
            }
            else
            {
                codeInvoiceTotals = new Totals
                {
                    TotalInvoiceNonRoundedValue = taxCodeAmountNonRounded,
                    TotalInvoiceRoundedValue = taxCodeAmountRounded
                };

                this.currentTotals.Add(taxCode.Code, codeInvoiceTotals);

                // no further adjustments needed since there are no previous lines for the tax code.
                return taxCodeAmountRounded;
            }

            // b. If rounded Total Amount becomes more or less than Sum of Rounded tax amounts
            // then adjust current, rounded, taxLine.taxCode.AmountRounded by the difference.

            // Round new Total Amount.
            decimal roundedTotalInvoice = TaxService.Tax.InternalApplication.Services.Rounding.TaxRound(codeInvoiceTotals.TotalInvoiceNonRoundedValue, taxCode.Code);
            taxCodeAmountRounded = roundedTotalInvoice - codeInvoiceTotals.TotalInvoiceRoundedValue;

            // record the rounded and adjusted amount into invoice totals.
            // it will be used in next iteration.
            codeInvoiceTotals.TotalInvoiceRoundedValue += taxCodeAmountRounded;
            this.currentTotals[taxCode.Code] = codeInvoiceTotals;

            return taxCodeAmountRounded;
        }

        private struct Totals
        {
            /// <summary>
            ///  Gets or sets rounded value of the current total of the given tax code.
            /// </summary>
            public decimal TotalInvoiceRoundedValue { get; set; }

            /// <summary>
            ///  Gets or sets raw, non-rounded value of the current total of the given tax code.
            /// </summary>
            public decimal TotalInvoiceNonRoundedValue { get; set; }
        }
    }
}
