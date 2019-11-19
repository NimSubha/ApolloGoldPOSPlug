/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using LSRetailPosis.Transaction;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// Class for tag-a-long data for the SalesLineItem related to fiscal printer operations.
    /// </summary>
    [Serializable]
    public sealed class LineItemTagalong
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="LineItemTagalong"/> is voided on the printer.
        /// </summary>
        /// <value><c>true</c> if voided; otherwise, <c>false</c>.</value>
        public bool Voided { get; set; }

        /// <summary>
        /// Gets or sets the printer item number.
        /// </summary>
        /// <value>The printer item number.</value>
        public int PrinterItemNumber { get; set; }

        /// <summary>
        /// Gets or sets the posted price.
        /// </summary>
        /// <value>The posted price.</value>
        public decimal PostedPrice { get; set; }

        /// <summary>
        /// Gets or sets the posted discount.
        /// </summary>
        /// <value>The tax rate id.</value>
        public decimal PostedDiscount { get; set; }

        /// <summary>
        /// Gets or sets the posted quantity.
        /// </summary>
        /// <value>The posted quantity.</value>
        public decimal PostedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the tax rate id.
        /// </summary>
        /// <value>The tax rate id.</value>
        public string TaxRateId { get; set; }

        /// <summary>
        /// Gets or sets the previous line discount flag.
        /// </summary>
        /// <value>True if the item has a previous line discount; false otherwise</value>
        public bool HasPreviousLineDiscount { get; set; }

        /// <summary>
        /// Gets or sets the approximate tax value of the item.
        /// </summary>
        /// <value>The approximate tax value.</value>
        public decimal ApproximateTaxValue { get; set; }

        public LineItemTagalong(int printerItemNumber, decimal postedPrice, decimal postedQuantity, decimal postedDiscount, string taxRateId, decimal approximateTaxValue)
        {
            PrinterItemNumber = printerItemNumber;
            PostedPrice = postedPrice;
            PostedQuantity = postedQuantity;
            TaxRateId = taxRateId;
            PostedDiscount = postedDiscount;
            ApproximateTaxValue = approximateTaxValue;
        }

        /// <summary>
        /// Recovers the <see cref="LineItemTagalong"/> object associate with the given line ID.
        /// </summary>
        /// <param name="retailTransaction">The <see cref="RetailTransaction"/> that holds the transaction.</param>
        /// <param name="lineId">The line ID that indexes the information.</param>
        /// <returns>An instance of <see cref="LineItemTagalong"/> if it exists; otherwize, null.</returns>
        public static LineItemTagalong GetTagalong(RetailTransaction retailTransaction, int lineId)
        {
            LineItemTagalong tagalong = null;
            var extension = RetailTransactionExtension.GetExtension(retailTransaction);

            if (extension.SalesLineItemData.ContainsKey(lineId))
            {
                tagalong = extension.SalesLineItemData[lineId];
            }

            return tagalong;
        }

    }
}
