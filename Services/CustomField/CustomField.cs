/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.Collections.Generic;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using LSRetailPosis.Transaction.Line.SaleItem;
using System;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Settings;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.CustomField
{
    /// <summary>
    /// Implementation of ICustomField service
    /// Provides run-time values for custom UI layout fields in the POS
    /// </summary>
    [Export(typeof(ICustomField))]
    public class CustomField : ICustomField
    {
        //Start:Nim
        private IApplication application;

        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
            }
        }
        //End:Nim

        #region ICustomFieldtV1 Members

        /// <summary>
        /// Return values for custom ItemReceipt fields
        /// </summary>
        /// <param name="fields">collection of the custom fields that exist</param>
        /// <param name="saleLine">current sale line</param>
        /// <param name="posTransaction">current transaction</param>
        /// <returns>dictionary of [Field Name, Field Value] pairs</returns>
        public IDictionary<string, string> PopulateItemReceiptFields(IEnumerable<CustomFieldValue> fields, ISaleLineItem saleLineItem, IPosTransaction posTransaction)
        {
#if DEBUG
                                                                                                                                            if (saleLineItem != null)
                {
                    Dictionary<string, string> fieldResults = new Dictionary<string, string>();
                    fieldResults.Add("ITEM_1", saleLineItem.Price.ToString());
                    fieldResults.Add("ITEM_2", saleLineItem.LineId.ToString());
                    fieldResults.Add("ITEM_3", saleLineItem.TaxRatePct.ToString());
                    return fieldResults;
                }
#endif

            # region Nim
            //#region Changed By Nimbus
            Dictionary<string, string> fieldResults = new Dictionary<string, string>();

            if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
            {

                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }


                //  string commandText = " SELECT CUSTORDER,GOLDFIXING FROM RETAILTEMPTABLE WHERE ID=1 ";
                string commandText = " SELECT CUSTORDER,GOLDFIXING FROM RETAILTEMPTABLE WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                SqlCommand command = new SqlCommand(commandText, connection);
                SqlDataReader reader = command.ExecuteReader();

                string strCustOrder = null;
                string sGoldFixing = null;

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        strCustOrder = Convert.ToString(reader.GetValue(0));
                        sGoldFixing = Convert.ToString(reader.GetValue(1));

                    }
                }
                reader.Dispose();


                if (string.IsNullOrEmpty(strCustOrder) && string.IsNullOrEmpty(sGoldFixing))
                {
                    if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Payment == 0m)
                    {

                        if (((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).LastRunOperation == PosisOperations.CustomerAccountDeposit)
                        {
                            string custOrder = string.Empty;
                            string custGoldFixing = string.Empty;

                            #region Commented
                            //frmOptionSelectionGSSorCustomerOrder optionSelection = new frmOptionSelectionGSSorCustomerOrder();
                            //// this.Application.ApplicationFramework.POSShowForm(optionSelection);
                            //optionSelection.ShowDialog();

                            //if (optionSelection.isGSS)
                            //{
                            //    commandText = string.Empty;
                            //    commandText = " SELECT   GSSACCOUNTOPENING.GSSACCOUNTNO AS [GSS ACCOUNT NO.], " +
                            //                  " DIRPARTYTABLE.NAMEALIAS AS [CUSTOMER NAME],  " +
                            //                  "  CAST(GSSACCOUNTOPENING.INSTALLMENTAMOUNT AS NUMERIC(28,2)) AS [INSTALLMENT AMOUNT], " +
                            //                  " ( CASE  WHEN  GSSACCOUNTOPENING.SCHEMETYPE=0 THEN 'FIXED' ELSE 'FLEXIBLE' END) AS [SCHEME TYPE], " +
                            //                  " GSSACCOUNTOPENING.SCHEMECODE AS [SCHEME CODE], " +
                            //                  " ( CASE WHEN GSSACCOUNTOPENING.SCHEMEDEPOSITTYPE=0 THEN 'GOLD' ELSE 'AMOUNT' END) AS [DEPOSIT TYPE],   " +
                            //                  " GSSACCOUNTOPENING.GSSConfirm AS [GSS CONFIRM] " +
                            //                  " FROM         DIRPARTYTABLE INNER JOIN " +
                            //                  " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                            //                  " GSSACCOUNTOPENING ON CUSTTABLE.ACCOUNTNUM = GSSACCOUNTOPENING.CUSTACCOUNT ";

                            //    if (connection.State == ConnectionState.Closed)
                            //        connection.Open();


                            //    SqlCommand cmd = new SqlCommand(commandText, connection);
                            //    SqlDataReader rdr = cmd.ExecuteReader();
                            //    DataTable dtGSS = new DataTable();
                            //    dtGSS.Load(rdr);

                            //    //if (dtGSS != null && dtGSS.Rows.Count > 0)
                            //    //{

                            //    BlankOperations.WinFormsTouch.frmGSSInput oGSS = new frmGSSInput(dtGSS);
                            //    oGSS.ShowDialog();

                            //    //   }
                            //    //else
                            //    //{
                            //    //    LSRetailPosis.POSControls.POSFormsManager.ShowPOSMessageDialog(999990);
                            //    //    //LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(4544);
                            //    //}
                            //    rdr.Dispose();
                            //    custOrder = string.Empty;
                            //    custGoldFixing = Convert.ToString(oGSS.GoldFixing);
                            //    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount = Convert.ToDecimal(oGSS.Amount);
                            //    (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount = Convert.ToDecimal(oGSS.Amount);
                            //    (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).TransSalePmtDiff = Convert.ToDecimal(oGSS.Amount);

                            // }
                            #endregion
                            if (Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone))
                            {
                                custOrder = string.Empty;
                                custGoldFixing = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing);
                            }
                            else
                            {
                                frmSearchOrder oSearch = new frmSearchOrder(posTransaction, application, " WHERE isConfirmed=1 AND isDelivered=0 AND CUSTACCOUNT='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "' ");
                                oSearch.ShowDialog();

                                string sGoldRate = GetMetalRate();

                                bool bIsGoldFixing = CheckGoldFixing();

                                //if(bIsGoldFixing)
                                //{
                                //    //  using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select your option for Gold Fixing.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select your option for Gold Fixing...Gold Rate is : " + sGoldRate, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                //    {
                                //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                //        custGoldFixing = Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES" ? "True" : "False";
                                //    }
                                //}
                                //else
                                //{
                                //    custGoldFixing = "False";
                                //}

                                custGoldFixing = "False";

                                custOrder = oSearch.sOrderNo;
                            }

                            commandText = string.Empty;
                            //   commandText = " UPDATE RETAILTEMPTABLE SET TRANSID=@TRANSID,CUSTID=@CUSTID,CUSTORDER=@CUSTORDER,GOLDFIXING=@GOLDFIXING WHERE ID=1";
                            commandText = " UPDATE RETAILTEMPTABLE SET TRANSID=@TRANSID,CUSTID=@CUSTID,CUSTORDER=@CUSTORDER,GOLDFIXING=@GOLDFIXING WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

                            fieldResults.Add("Customer Order", custOrder);
                            fieldResults.Add("Gold Fixing", custGoldFixing);
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo = custOrder;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing = custGoldFixing;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                            ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;

                            command = new SqlCommand(commandText, connection);
                            command.Parameters.Clear();
                            command.Parameters.Add("@TRANSID", SqlDbType.NVarChar).Value = posTransaction.TransactionId;
                            command.Parameters.Add("@CUSTID", SqlDbType.NVarChar, 20).Value = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId;
                            command.Parameters.Add("@CUSTORDER", SqlDbType.NVarChar, 20).Value = (string.IsNullOrEmpty(custOrder)) ? string.Empty : Convert.ToString(custOrder);
                            command.Parameters.Add("@GOLDFIXING", SqlDbType.SmallInt).Value = Convert.ToBoolean(custGoldFixing) == true ? 1 : 0;

                            if (connection.State == ConnectionState.Closed)
                                connection.Open();

                            command.ExecuteNonQuery();
                        }
                    }
                }
                else
                {
                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo = strCustOrder;
                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing = sGoldFixing;
                    fieldResults.Add("Customer Order", strCustOrder);
                    fieldResults.Add("Gold Fixing", sGoldFixing);

                }
                return fieldResults;
            }
            #endregion

            return null;
        }

        /// <summary>
        /// Return values for custom Payment fields
        /// </summary>
        /// <param name="fields">collection of the custom fields that exist</param>
        /// <param name="saleLine">current sale line</param>
        /// <param name="posTransaction">current transaction</param>
        /// <returns>dictionary of [Field Name, Field Value] pairs</returns>
        public IDictionary<string, string> PopulatePaymentFields(IEnumerable<CustomFieldValue> fields, ITenderLineItem tenderLineItem, IPosTransaction posTransaction)
        {
#if DEBUG
            if (tenderLineItem != null)
            {
                Dictionary<string, string> fieldResults = new Dictionary<string, string>();
                fieldResults.Add("PAY_1", tenderLineItem.ExchangeRate.ToString());
                fieldResults.Add("PAY_2", tenderLineItem.LineId.ToString());
                return fieldResults;
            }
#endif
            return null;
        }

        /// <summary>
        /// Return values for custom Total fields
        /// </summary>
        /// <param name="fields">collection of the custom fields that exist</param>
        /// <param name="posTransaction">current transaction</param>
        /// <returns>dictionary of [Field Name, Field Value] pairs</returns>
        public IDictionary<string, string> PopulateTotalFields(IEnumerable<CustomFieldValue> fields, IPosTransaction posTransaction)
        {
#if DEBUG
            if (posTransaction != null)
            {
                Dictionary<string, string> fieldResults = new Dictionary<string, string>();
                fieldResults.Add("TOTALS_1", posTransaction.TransactionId.ToString());
                return fieldResults;
            }
#endif
            return null;
        }

        #endregion

        #region ICustomFieldtV2 Members

        const string itemPreviewTextDiscountField = "Discount";
        const string itemPreviewTextQuantityField = "Quantity";
        const string itemPreviewTextPriceOverriddenField = "PriceOverridden";
        const string itemPreviewTextDimensionField = "Dimension";
        const string itemPreviewTextSalesPersonField = "SalesPerson";
        const string itemPreviewTextSerialIdField = "SerialId";

        /// <summary>
        /// Returns the ItemReceipt Preview Text.
        /// </summary>
        /// <param name="previewComments">The preview Comments.</param>
        /// <param name="posTransaction">The pos transaction.</param>
        /// <param name="saleLineItem">The sale line item.</param>
        /// <returns>List[comments]</returns>
        public IDictionary<string, string> PopulateItemReceiptPreviewText(IDictionary<string, string> previewComments, IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
#if DEBUG
            if (previewComments != null && posTransaction != null)
            {
                SaleLineItem saleLine = (SaleLineItem)saleLineItem;

                // Customization of the comments can be done as below.            
                //previewComments[itemPreviewTextDiscountField] = "Your discount is " + saleLine.PartnerData.CustomPercentDiscountField.Tostring() + " Percent.";               

                // previewComments[itemPreviewTextQuantityField] = "User customized quantity";
                // if(saleLine.PriceOverridden)
                // {
                // previewComments[itemPreviewTextPriceOverriddenField] = "User customized text";
                // }
                // previewComments[itemPreviewTextDimensionField] = "User customized Dimension";                 
                // if (saleLine.SalesPersonId)
                // {                     
                // previewComments[itemPreviewTextSalesPersonField] = "User Custom Text";
                // }
                // if (saleLine.SerialId)
                // {
                // previewComments[itemPreviewTextSerialIdField] = "User Custom Text";
                // }   
            }
#endif
            return previewComments;
        }

        #endregion

        private string GetMetalRate()
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");// [INVENTPARAMETERS]

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 "); // RATETYPE -- > GSS->Sale 4->3 on 10/06/2016 req by S.Sharma
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");


            //  enum RateTypeNew
            //   {
            //       Purchase = 0,
            //       OGP = 1,
            //       OGOP = 2,
            //       Sale = 3,
            //       GSS = 4,
            //       Exchange = 6,
            //   }

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return Convert.ToString(sResult.Trim());
        }

        private bool CheckGoldFixing() // ADDED ON 09/04/2015
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT top 1 isnull(GOLDFIXING,0) FROM RETAILPARAMETERS"; // RETAILTEMPTABLE


            SqlCommand command = new SqlCommand(commandText, connection);

            int sResult = Convert.ToInt16(command.ExecuteScalar());
            connection.Close();
            if (sResult > 0)
            {
                bReturn = true;
            }
            return bReturn;
        }
    }
}
