using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.SimulatedFiscalPrinter
{
    /// <summary>
    /// Class for basic tax information related to fiscal printer operations
    /// </summary>
    public sealed class TaxInfo
    {
        public string TaxIdentifier { get; private set; }
        public decimal TaxRate { get; private set; }
        public bool IsTaxInclusive { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxInfo"/> class.
        /// <remarks>Only tax inclusive is supported at this time.</remarks>
        /// </summary>
        /// <param name="taxIdentifier">The tax identifier.</param>
        /// <param name="taxRate">The tax rate.</param>
        public TaxInfo(string taxIdentifier, decimal taxRate)
            : this(taxIdentifier, taxRate, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaxInfo"/> class.
        /// </summary>
        /// <param name="taxIdentifier">The tax identifier.</param>
        /// <param name="taxRate">The tax rate.</param>
        /// <param name="taxInclusive">if set to <c>true</c> [tax inclusive].</param>
        public TaxInfo(string taxIdentifier, decimal taxRate, bool taxInclusive)
        {
            this.TaxIdentifier = taxIdentifier;
            this.TaxRate = taxRate;
            this.IsTaxInclusive = taxInclusive;
        }
    }
}
