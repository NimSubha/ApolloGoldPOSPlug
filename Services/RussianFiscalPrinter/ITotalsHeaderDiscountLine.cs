/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.Collections.Generic;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Interface of discount data structure for totals header discount data transfer
    /// </summary>
    public interface ITotalsHeaderDiscountLine
    {
        /// <summary>
        /// Discount type
        /// </summary>
        DiscountType DiscountType { get; set; }

        /// <summary>
        /// Set of printer tax codes (printer tax group)
        /// </summary>
        string PrinterTaxGroup { get; set; }

        /// <summary>
        /// Discount amount
        /// </summary>
        decimal DiscountAmount { get; set; }

        /// <summary>
        /// Discount tax amounts dictionary.
        /// </summary>
        /// <remarks>
        /// dictionary of discount tax amounts calculated for this discount line
        /// where key - printer tax Id, value - printer calculated tax amount.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Design")]
        IDictionary<string, decimal> DiscountTaxAmounts { get; set; }
    }
}
