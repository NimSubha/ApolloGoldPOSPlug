/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics365.Tax.Core;

namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>
    /// The proxy between generic tax engine and retail tax calculation
    /// </summary>
    public class TaxEngineServiceProxy
    {
        Customer defaultCustomer;
        string defaultAccountingCurrency;
        long ledgerRecordId;

        /// <summary>
        /// Determines whether tax calculation through GTE is required for specified transaction or not.
        /// </summary>
        /// <param name="retailTransaction">Retail transaction.</param>
        /// <returns>True if GTE calculation required; otherwise false.</returns>
        public static bool IsGTECalculationRequired(IRetailTransaction retailTransaction)
        {
            bool ret = false;

            if (Functions.CountryRegion == SupportedCountryRegion.IN)
            {
                IEnumerable<SaleLineItem> items = GetSalesLinesForTaxCalculation(retailTransaction);

                ret = items.Any() && !items.Any(s => !String.IsNullOrEmpty(s.TaxGroupId));
            }

            return ret;
        }

        /// <summary>
        /// Gets sales lines for tax calculation.
        /// </summary>
        /// <param name="retailTransaction">Retail transaction.</param>
        /// <returns>List of sales transactions for tax calculation.</returns>
        public static IEnumerable<SaleLineItem> GetSalesLinesForTaxCalculation(IRetailTransaction retailTransaction)
        {
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            IEnumerable<SaleLineItem> lines = null;

            if (transaction != null)
            {
                lines = transaction.SaleItems.Where(s => !s.Voided);
            }
            else
            {
                lines = new LinkedList<SaleLineItem>();
            }

            return lines;
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application"></param>
        public TaxEngineServiceProxy(IApplication application)
        {
            SqlConnection connection = application.Settings.Database.Connection;
            string dataAreaId = application.Settings.Database.DataAreaID;

            defaultCustomer = application.BusinessLogic.CustomerSystem.GetCustomerInfo(ApplicationSettings.Terminal.DefaultCustomer) as LSRetailPosis.Transaction.Customer;

            InitializeTaxSession(connection);
            InitializeDataFromLedger(connection, dataAreaId);
        }

        private void InitializeTaxSession(SqlConnection connection)
        {
            const string defaultSchema = "DBO";

            TaxContext taxContext = new TaxContext();

            taxContext.SessionId = (int)ApplicationSettings.Terminal.StorePrimaryId;
            taxContext.ConnectString = connection.ConnectionString;
            taxContext.CalculationThreadNumber = 1; // always use single thread calculation of GTE for Retail

            TaxSessionManager.InitSession(taxContext);
            TaxSessionManager.SetSessionMode(TaxSessionMode.Retail);
            TaxSessionManager.SetDefaultSchemaName(defaultSchema);
        }

        private void InitializeDataFromLedger(SqlConnection connection, string dataAreaId)
        {
            const string ledgerTable = "LEDGER";
            const string ledgerIdCol = "RECID";
            const string currencyCol = "ACCOUNTINGCURRENCY";
            const string nameCol = "NAME";

            DBUtil dbUtil = new DBUtil(connection);
            SqlSelect sqlSelect = new SqlSelect(ledgerTable);
            sqlSelect.Select(ledgerIdCol);
            sqlSelect.Select(currencyCol);
            sqlSelect.Where(nameCol, dataAreaId, false);
            var table = dbUtil.GetTable(sqlSelect);

            if (table.Rows.Count > 0)
            {
                ledgerRecordId = DBUtil.ToInt64(table.Rows[0][ledgerIdCol]);
                defaultAccountingCurrency = DBUtil.ToStr(table.Rows[0][ledgerIdCol]);
            }
        }

        private TaxEngineCalculationParameter GetTaxEngineCalculationParameter()
        {
            TaxEngineCalculationParameter taxEngineCalculationParameter = new TaxEngineCalculationParameter();

            taxEngineCalculationParameter.CurrentLedgerRecId = ledgerRecordId;
            taxEngineCalculationParameter.DefaultAccountingCurrency = defaultAccountingCurrency;
            taxEngineCalculationParameter.CurrentDateTime = DateTime.Now;
            taxEngineCalculationParameter.UserPreferredTimeZoneId = TimeZoneInfo.Local.Id;

            return taxEngineCalculationParameter;
        }

        /// <summary>
        /// Calculate taxes for the transaction
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void CalculateTax(IRetailTransaction retailTransaction)
        {
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            TaxableDocumentProvider documentProvider = new TaxableDocumentProvider(defaultCustomer, transaction);

            TaxableDocumentMapping documentMapping = documentProvider.GetTaxableDocumentWithMapping();

            var taxEngineCalculationParameter = GetTaxEngineCalculationParameter();

            try
            {       
                TaxEngineCalculationResult calculationResult = TaxEngineService.calculateAx(documentMapping.TaxableDocument, taxEngineCalculationParameter);
                if (calculationResult == null)
                {
                    throw new NullReferenceException("calculationResult");
                }

                if (calculationResult.isSuccessful())
                {
                    TaxDocumentParser.PopulateRetailTransactionWithTaxItems(calculationResult.TaxDocument, documentMapping);
                }
                else
                {
                    NetTracer.Error("Tax engine calculation was unsuccessful.");
                    string errorMessage = calculationResult.ProcessingLog.getFullErrorMessage();
                    if (!string.IsNullOrEmpty(errorMessage))
                    {
                        NetTracer.Error(errorMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "CalculateTax() failed in an Exception");
                LSRetailPosis.ApplicationExceptionHandler.HandleException("TaxEngineServiceProxy", ex);
            }
        }

    }
}
