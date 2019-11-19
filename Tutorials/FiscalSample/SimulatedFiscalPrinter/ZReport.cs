using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Dynamics.Retail.Pos.SimulatedFiscalPrinter
{
    /// <summary>
    /// Z-Report Class keeps track of transaction data required for the Z-report.
    /// </summary>
    public sealed class ZReport
    {
        /// <summary>
        /// Gets or sets the opening grand total.
        /// </summary>
        /// <value>The opening grand total.</value>
        public decimal OpeningGrandTotal { get; private set; }

        /// <summary>
        /// Gets or sets the open date time.
        /// </summary>
        /// <value>The open date time.</value>
        public DateTime OpenDateTime { get; private set; }
        
        /// <summary>
        /// Gets or sets the closing grand total.
        /// </summary>
        /// <value>The closing grand total.</value>
        public decimal ClosingGrandTotal { get; private set; }
        
        /// <summary>
        /// Gets or sets the closing date time.
        /// </summary>
        /// <value>The closing date time.</value>
        public DateTime ClosingDateTime { get; private set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed { get; private set; }

        private Dictionary<string, decimal> _collectedTaxes;
        private Dictionary<string, decimal> _tenders;

        /// <summary>
        /// Initializes a new instance of the <see cref="ZReport"/> class.
        /// </summary>
        public ZReport()
            : this(0m)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZReport"/> class.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        public ZReport(decimal grandTotal)
        {
            IsClosed = false;
            OpeningGrandTotal = grandTotal;
            OpenDateTime = DateTime.Now;
            _collectedTaxes = new Dictionary<string, decimal>();
            _tenders = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZReport"/> class.
        /// </summary>
        /// <param name="lastZReport">The last Z report.</param>
        public ZReport(ZReport lastZReport)
            : this(0m)
        {
            if (lastZReport != null)
            {
                if (lastZReport.IsClosed)
                {
                    OpeningGrandTotal = lastZReport.ClosingGrandTotal;
                    OpenDateTime = lastZReport.ClosingDateTime;
                }
                else
                {   // Just copy lastZReport into this
                    OpeningGrandTotal = lastZReport.OpeningGrandTotal;
                    OpenDateTime = lastZReport.OpenDateTime;
                    foreach (string taxId in lastZReport._collectedTaxes.Keys)
                    {
                        _collectedTaxes.Add(taxId, lastZReport._collectedTaxes[taxId]);
                    }

                    foreach (string tender in lastZReport._tenders.Keys)
                    {
                        _tenders.Add(tender, lastZReport._tenders[tender]);
                    }

                }
            }
        }

        /// <summary>
        /// Adds the tender.
        /// </summary>
        /// <param name="tenderId">The tender id.</param>
        /// <param name="amount">The amount.</param>
        public void AddTender(string tenderId, decimal amount)
        {
            Debug.Assert(!IsClosed);

            if (!IsClosed)
            {
                if (_tenders.ContainsKey(tenderId))
                {   // Adjust existing tender
                    _tenders[tenderId] += amount;
                }
                else
                {   // Add new tender
                    _tenders.Add(tenderId, amount);
                }
            }
        }

        /// <summary>
        /// Adds the tax.
        /// </summary>
        /// <param name="taxId">The tax id.</param>
        /// <param name="collectedTax">The collected tax.</param>
        public void AddTax(string taxId, decimal collectedTax)
        {
            Debug.Assert(!IsClosed);

            if (!IsClosed)
            {
                if (_collectedTaxes.ContainsKey(taxId))
                {   // Adjust existing tax
                    _collectedTaxes[taxId] += collectedTax;
                }
                else
                {   // Add new tax
                    _collectedTaxes.Add(taxId, collectedTax);
                }
            }
        }

        /// <summary>
        /// Closes the z-report using the specified grand total.
        /// </summary>
        /// <param name="grandTotal">The grand total.</param>
        public void Close(decimal grandTotal)
        {
            Debug.Assert(!IsClosed);

            if (!IsClosed)
            {
                this.ClosingGrandTotal = grandTotal;
                this.ClosingDateTime = DateTime.Now;
                this.IsClosed = true;
            }
        }

        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetReport()
        {
            StringBuilder result = new StringBuilder();

            result.AppendLine("Z-Report --->");
            result.AppendLine(string.Format(CultureInfo.CurrentCulture, "Opening Date: {0}", this.OpenDateTime.ToString(CultureInfo.CurrentCulture)));
            result.AppendLine(string.Format(CultureInfo.CurrentCulture, "Open Total: {0:c}", this.OpeningGrandTotal));
            result.AppendLine();

            if (IsClosed)
            {
                result.AppendLine(string.Format(CultureInfo.CurrentCulture, "Closing Date: {0}", this.ClosingDateTime.ToString(CultureInfo.CurrentCulture)));
                result.AppendLine(string.Format(CultureInfo.CurrentCulture, "Close Total: {0:c}", this.ClosingGrandTotal));

                result.AppendLine("Tenders");
                foreach (string tenderId in this._tenders.Keys)
                {
                    result.AppendLine(string.Format(CultureInfo.CurrentCulture, "\t{0,-15}: {1:c}", tenderId, _tenders[tenderId]));
                }

                result.AppendLine("Taxes");
                foreach (string taxId in this._collectedTaxes.Keys)
                {
                    result.AppendLine(string.Format(CultureInfo.CurrentCulture, "\t{0,-15}: {1:c}", taxId, _collectedTaxes[taxId]));
                }

                result.AppendLine();
                result.AppendLine("End Z-Report <---");
            }
            else
            {
                result.AppendLine("*** Z-Report has not yet been closed <---");
            }

            return result.ToString();
        }
    }
}
