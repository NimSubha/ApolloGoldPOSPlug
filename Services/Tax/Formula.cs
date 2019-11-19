/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// AX Taxable Bases.  Only LineAmount and ExclAmount are applicable for POS
    /// </summary>
    public enum TaxableBases
    {
        LineAmount = 0,     // Tax applies to the line amount in addition to tax codes specified in formula
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Excl", Justification = "Cannot change public API.")]
        ExclAmount = 1,     // Tax only applies to the codes in the formula
        MRP = 2,            // Tax applies to the Manufacturer retail price
        Assessable = 3      // N/A for POS
    };



    /// <summary>
    /// Represents data from FormulaDesigner_IN table
    /// </summary>
    public sealed class Formula
    {
        // delimiter used in the formula calculation
        private const char delimiter = (char)164;

        /// <summary>
        /// Check whether current taxbasis is supported for normal taxable line.
        /// </summary>
        public bool SupportedTaxBasis
        {
            get
            {
                return
                    TaxableBasis == TaxableBases.LineAmount ||
                    TaxableBasis == TaxableBases.ExclAmount ||
                    TaxableBasis == TaxableBases.MRP;
            }
        }

        /// <summary>
        /// Check whether current taxbasis is supported for the taxable line if it is misc charge.
        /// </summary>
        public bool SupportedTaxBasisForMiscCharge
        {
            get
            {
                return
                    TaxableBasis == TaxableBases.LineAmount ||
                    TaxableBasis == TaxableBases.ExclAmount;
            }
        }

        /// <summary>
        /// Parses the expression based on the delimiter above.
        /// </summary>
        /// <returns></returns>
        public string[] ParseExpression()
        {
            return CalculationExpression.Split(delimiter);
        }

        /// <summary>
        /// Used to set priority for the tax code
        /// </summary>
        public int Id { get; set; } 

        /// <summary>
        ///  Described above
        /// </summary>
        public TaxableBases TaxableBasis { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool PriceIncludesTax { get; set; }

        /// <summary>
        /// Of the form: +[BCD]+[CVD]+[E-CESS_CVD]+[PE-C_CVD]+[SHE-C_CVD]
        /// where the brackets are replaced with the delimiter char(164)
        /// and BCD, CVD ... are tax codes.
        /// The operator may be + - / *.
        /// </summary>
        public string CalculationExpression { get; set; }   
    }
}
