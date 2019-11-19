/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Text;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// Tax code provider for Brazil
    /// </summary>
    public sealed class TaxCodeProviderBrazil : TaxCodeProvider
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application"></param>
        public TaxCodeProviderBrazil(IApplication application)
            : base(application)
        {
        }

        /// <summary>
        /// Returns the tax base amount for the given sales line item
        /// </summary>
        /// <param name="taxableItem"> </param>
        /// <param name="codes"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "codes", Justification = "Grandfather"),
         System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Grandfather")]
        public override decimal GetBasePriceForTaxIncluded(ITaxableItem taxableItem, ReadOnlyCollection<TaxCode> codes)
        {
            if (taxableItem == null)
            {
                throw new ArgumentNullException("taxableItem");
            }

            // Even though we use TaxIncludedInPrice, we have to adopt NetAmountPerUnit to mimic the Fiscal Printer behavior
            return taxableItem.NetAmountPerUnit;
        }

        /// <summary>
        /// SQL selection text to read item tax
        /// </summary>
        /// <remarks>This adds INCLUDEDTAX_BR field to the base method.</remarks>
        protected override string TaxSelectSqlText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("TAXONITEM.TAXITEMGROUP, ");
                sb.AppendLine("TAXONITEM.TAXCODE, ");
                sb.AppendLine("ISNULL(TAXDATA.TAXVALUE, 0.0) AS TAXVALUE, ");
                sb.AppendLine("ISNULL(TAXDATA.TAXLIMITMIN, 0.0) AS TAXLIMITMIN, ");
                sb.AppendLine("ISNULL(TAXDATA.TAXLIMITMAX, 0.0) AS TAXLIMITMAX, ");
                sb.AppendLine("TAXGROUPDATA.EXEMPTTAX, ");
                sb.AppendLine("TAXGROUPHEADING.TAXGROUPROUNDING, ");
                sb.AppendLine("TAXTABLE.TAXCURRENCYCODE, ");
                sb.AppendLine("TAXTABLE.TAXBASE, ");
                sb.AppendLine("TAXTABLE.TAXLIMITBASE, ");
                sb.AppendLine("TAXTABLE.TAXCALCMETHOD, ");
                sb.AppendLine("TAXTABLE.TAXONTAX, ");
                sb.AppendLine("TAXTABLE.TAXUNIT, ");
                sb.AppendLine("ISNULL(TAXCOLLECTLIMIT.TAXMAX,0) AS TAXMAX, ");
                sb.AppendLine("ISNULL(TAXCOLLECTLIMIT.TAXMIN,0) AS TAXMIN, ");
                sb.AppendLine("TAXTABLE.INCLUDEDTAX_BR ");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Gets the tax code.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="taxableItem">The taxable item.</param>
        /// <returns>The taxcode object</returns>
        protected override TaxCode GetTaxCode(SqlDataReader reader, ITaxableItem taxableItem)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            return new TaxCodeBrazil(
                reader["TAXCODE"] as string,
                taxableItem,
                reader["TAXITEMGROUP"] as string,
                reader["TAXCURRENCYCODE"] as string,
                (decimal)reader["TAXVALUE"],
                (decimal)reader["TAXLIMITMIN"],
                (decimal)reader["TAXLIMITMAX"],
                ((int)reader["EXEMPTTAX"] == 1),
                (TaxBase)reader["TAXBASE"],
                (TaxLimitBase)reader["TAXLIMITBASE"],
                (TaxCalculationMode)reader["TAXCALCMETHOD"],
                reader["TAXONTAX"] as string,
                reader["TAXUNIT"] as string,
                (decimal)reader["TAXMIN"],
                (decimal)reader["TAXMAX"],
                ((int)reader["TAXGROUPROUNDING"] == 1),
                ((int)reader["INCLUDEDTAX_BR"] == 1),
                this);
        }
    }
}
