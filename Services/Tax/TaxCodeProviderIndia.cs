/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using LSRetailPosis.Settings;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// Provides India specific functionality
    /// </summary>
    public sealed class TaxCodeProviderIndia : TaxCodeProvider
    {
        private bool taxParameterVatIn;
        private bool taxParameterServiceIn;
        private bool taxParameterSalesIn;
        private Int64 salesTaxFormTypesIn;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public TaxCodeProviderIndia(IApplication application)
            : base(application)
        {
           // SqlConnection connection = application.Settings.Database.Connection;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            try
            {
                // Read parameters from the Tax parameters table
                DBUtil dbUtil = new DBUtil(connection);
                DataTable tbTaxParameters = dbUtil.GetTable("SELECT VAT_IN, SERVICETAX_IN, SALESTAX_IN FROM TAXPARAMETERS");
                taxParameterServiceIn = (int)tbTaxParameters.Rows[0]["SERVICETAX_IN"] == 1;
                taxParameterVatIn = (int)tbTaxParameters.Rows[0]["VAT_IN"] == 1;
                taxParameterSalesIn = (int)tbTaxParameters.Rows[0]["SALESTAX_IN"] == 1;

                if (taxParameterSalesIn)
                {
                    salesTaxFormTypesIn = GetSalesTaxFormTypes();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Warning(ex, "TaxCodeProviderIndia(): handled an exception: {0}", ex.Message);
                LSRetailPosis.ApplicationExceptionHandler.HandleException("TaxCodeProviderIndia", ex);
                // Not necessary to rethrow because tax parameters are set to default state.
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        protected override string TaxSelectSqlText
        {
            get
            {
                return "TAXONITEM.TAXITEMGROUP, TAXONITEM.TAXCODE, TAXONITEM.ABATEMENTPERCENT_IN, " +
                "ISNULL(TAXDATA.TAXVALUE, 0.0) AS TAXVALUE, ISNULL(TAXDATA.TAXLIMITMIN, 0.0) AS TAXLIMITMIN, ISNULL(TAXDATA.TAXLIMITMAX, 0.0) AS TAXLIMITMAX, " +
                "TAXGROUPDATA.EXEMPTTAX, TAXGROUPHEADING.TAXGROUPROUNDING, " +
                "TAXTABLE.TAXCURRENCYCODE, TAXTABLE.TAXTYPE_IN, TAXBASE, TAXLIMITBASE, TAXCALCMETHOD, TAXONTAX, TAXUNIT, " +
                "ISNULL(TAXMAX,0) AS TAXMAX, ISNULL(TAXMIN,0) AS TAXMIN, " +
                "ISNULL(TAXDATA.SALESTAXFORMTYPES_IN, 0) AS SALESTAXFORMTYPES_IN ";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        protected override System.Collections.ObjectModel.ReadOnlyCollection<TaxCode> SortCodes(IEnumerable<TaxCode> codes)
        {
            // Return codes to be processed in the following order:
            // Non-India codes
            // India codes ordered by Id
            return new ReadOnlyCollection<TaxCode>(
                codes.OrderBy(code =>
                {
                    TaxCodeIndia codeIndia = code as TaxCodeIndia;
                    if (codeIndia != null)
                    {
                        return codeIndia.Formula.Id + MaxPriorityTaxCode + 1;
                    }
                    else
                    {
                        return TaxCodeProvider.TaxCodePriority(code);
                    }
                }).ToList());
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
                throw new ArgumentNullException("reader");

            if ((TaxTypes)reader["TAXTYPE_IN"] == TaxTypes.None)
            {
                return base.GetTaxCode(reader, taxableItem);
            }
            else
            {
                return new TaxCodeIndia(
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
                    (decimal)reader["ABATEMENTPERCENT_IN"],
                    (TaxTypes)reader["TAXTYPE_IN"],
                    this);
            }
        }

        /// <summary>
        /// Add Extendtion sql condition to select data
        /// </summary>
        /// <param name="command"></param>
        protected override string AddTaxSelectSqlCondition(SqlCommand command)
        {
            if (command != null)
            {
                command.Parameters.AddWithValue("@TAXTYPE", TaxTypes.SalesTax);
                command.Parameters.AddWithValue("@SALESTAXFORMTYPES", salesTaxFormTypesIn);

                return @"AND 
                ((TAXTABLE.TAXTYPE_IN = @TAXTYPE AND TAXDATA.SALESTAXFORMTYPES_IN = @SALESTAXFORMTYPES) 
                OR TAXTABLE.TAXTYPE_IN != @TAXTYPE) ";

            }
            return string.Empty;
        }

        /// <summary>
        /// Gets sales tax form types from India Retail store table.
        /// </summary>
        private static Int64 GetSalesTaxFormTypes()
        {
            Int64 salesTaxFormTypes = 0;

            SqlConnection connection = ApplicationSettings.Database.LocalConnection;

            try
            {
                // Read sales tax form type record id from the India Retail store table
                DBUtil dbUtil = new DBUtil(connection);
                SqlParameter[] sqlParameter = { new SqlParameter("@STOREID", ApplicationSettings.Terminal.StoreId) };

                DataTable table = dbUtil.GetTable(@"SELECT ISNULL(SALESTAXFORMTYPES, 0) AS SALESTAXFORMTYPES FROM RETAILSTORETABLE_IN 
                                                            INNER JOIN RETAILSTORETABLE ON 
                                                            RETAILSTORETABLE_IN.RETAILSTORETABLE = RETAILSTORETABLE.RECID 
                                                            AND  RETAILSTORETABLE.STORENUMBER = @STOREID", sqlParameter);

                salesTaxFormTypes = table.Rows.Count > 0 ? (Int64)table.Rows[0]["SALESTAXFORMTYPES"] : 0;

                return salesTaxFormTypes;
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "GetSalesTaxFormTypes() failed in an Exception");
                LSRetailPosis.ApplicationExceptionHandler.HandleException("TaxCodeProviderIndia", ex);
                return 0;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Only support:
        /// codes that are of the supported tax basis.
        /// and codes that are defined in the formula (Id > 0)
        /// and codes that are of either ServiceTax or VAT or SalesTax
        /// </summary>
        /// <param name="codeIndia"></param>
        /// <returns></returns>
        private bool SupportedTax(ITaxableItem taxableItem, TaxCodeIndia codeIndia)
        {
            if (taxableItem is IMiscellaneousCharge)
            {
                return codeIndia.Formula.SupportedTaxBasisForMiscCharge && (codeIndia.Formula.Id > 0) &&
                    (((codeIndia.TaxType == TaxTypes.ServiceTax) && taxParameterServiceIn));
            }
            else
            {
                return codeIndia.Formula.SupportedTaxBasis && (codeIndia.Formula.Id > 0) &&
                    (((codeIndia.TaxType == TaxTypes.ServiceTax) && taxParameterServiceIn) ||
                    ((codeIndia.TaxType == TaxTypes.VAT) && taxParameterVatIn) ||
                    ((codeIndia.TaxType == TaxTypes.SalesTax) && taxParameterSalesIn));
            }
        }

        protected override ReadOnlyCollection<TaxCode> GetTaxCodes(ITaxableItem taxableItem)
        {
            // Only use codes that are not processed as India tax codes, or those that are and are
            // supported.
            return new ReadOnlyCollection<TaxCode>(base.GetTaxCodes(taxableItem).Where(c =>
                {
                    var codeIndia = c as TaxCodeIndia;

                    return codeIndia == null || SupportedTax(taxableItem, codeIndia);
                }).ToList<TaxCode>());
        }
    }
}
