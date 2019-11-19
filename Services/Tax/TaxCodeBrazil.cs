/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// Provides specific logic for the Brazilian tax engine
    /// </summary>
    public sealed class TaxCodeBrazil : TaxCode
    {
        /// <summary>
        /// Flags if this specific tax code is included in the sales price
        /// </summary>
        public bool IncludedTax { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">TaxCode</param>
        /// <param name="lineItem">The line item which the tax is being applied to</param>
        /// <param name="taxGroup">TaxGroup that this Tax Code belongs to</param>
        /// <param name="currency">Currency that this tax is calculated in</param>
        /// <param name="value">Value/Rate of the tax</param>
        /// <param name="limitMin">Minimum amount required to calculate this tax.</param>
        /// <param name="limitMax">Maximum amount required to calculate this tax</param>
        /// <param name="exempt">Whether or not this tax is exempt</param>
        /// <param name="taxBase">Origin from which sales tax is calculated</param>
        /// <param name="limitBase">Basis of sales tax limits</param>
        /// <param name="method">Whether tax is calculated for entire amounts or for intervals</param>
        /// <param name="taxOnTax">TaxCode of the other sales tax that this tax is based on.</param>
        /// <param name="unit">Unit for calculating per-unit amounts</param>
        /// <param name="collectMin">Collection limits, the minimum tax that can be collected</param>
        /// <param name="collectMax">Collection limits, the maximum tax that can be collected</param>
        /// <param name="groupRounding">Whether or not this code should be rounded at the Tax Group scope.</param>
        /// <param name="includedTax">Whether or not this tax is included in sales price</param>
        /// <param name="provider">The tax code provider that created this instance.</param>
        public TaxCodeBrazil(
            string code,
            ITaxableItem lineItem,
            string taxGroup,
            string currency,
            decimal value,
            decimal limitMin,
            decimal limitMax,
            bool exempt,
            TaxBase taxBase,
            TaxLimitBase limitBase,
            TaxCalculationMode method,
            string taxOnTax,
            string unit,
            decimal collectMin,
            decimal collectMax,
            bool groupRounding,
            bool includedTax,
            TaxCodeProvider provider)
            : base(
                code, lineItem, taxGroup, currency, value, limitMin, limitMax, exempt, taxBase, limitBase, method,
                taxOnTax, unit, false, collectMin, collectMax, groupRounding, provider)
        {
            this.IncludedTax = includedTax;
        }

        /// <summary>
        /// Flags if the tax amount should be included in the sales price
        /// </summary>
        public override bool TaxIncludedInPrice
        {
            get { return this.IncludedTax; }
        }
    }
}
