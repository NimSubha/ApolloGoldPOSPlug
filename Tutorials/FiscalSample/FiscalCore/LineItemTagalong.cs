using System;

namespace Microsoft.Dynamics.Retail.Pos.FiscalCore
{
    /// <summary>
    /// Class for tag-a-long data for the SalesLineItem related to fiscal printer operations.
    /// </summary>
    public class LineItemTagalong
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
        /// Gets or sets the posted quantity.
        /// </summary>
        /// <value>The posted quantity.</value>
        public decimal PostedQuantity { get; set; }

        /// <summary>
        /// Gets or sets the tax rate id.
        /// </summary>
        /// <value>The tax rate id.</value>
        public string TaxRateId { get; set; }

        public LineItemTagalong(int printerItemNumber, decimal postedPrice, decimal postedQuantity, string taxRateId)
        {
            this.PrinterItemNumber = printerItemNumber;
            this.PostedPrice = postedPrice;
            this.PostedQuantity = postedQuantity;
            this.TaxRateId = taxRateId;
        }
    }
}
