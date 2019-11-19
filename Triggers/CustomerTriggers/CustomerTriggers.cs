//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Data.SqlClient;
using System.Data;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Transaction;
using System;
using System.Text;
using System.Windows.Forms;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.CustomerTriggers
{
    [Export(typeof(ICustomerTrigger))]
    public class CustomerTriggers : ICustomerTrigger
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

        #region Constructor - Destructor

        public CustomerTriggers()
        {
            
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for CustomerTriggers are reserved at 65000 - 65999
        }

        #endregion

        #region ICustomerTriggersV1 Members

        public void PreCustomerClear(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Start:Nim
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                //bool isServiceItemExists = false;
                System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                foreach (var sale in saleline)
                {
                    if (saleline.Count > 0)
                    {
                        preTriggerResult.ContinueOperation = false;
                        preTriggerResult.MessageId = 65001; //Finish the current Transaction before clear the selected customer.
                        break;
                    }
                }
            }
            //End:Nim

            //Base
            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV1.PreCustomerClear", "Prior to clearing a customer from the transaction.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostCustomerClear(IPosTransaction posTransaction)
        {
            //Start: Nim
            RetailTransaction retailtrans = posTransaction as RetailTransaction;

            if (retailtrans != null)
            {
                string sStoreId = ApplicationSettings.Terminal.StoreId;
                string sTerminalId = ApplicationSettings.Terminal.TerminalId;

                #region //Nimbus by MIAM @ 08Jul14 : delete record from Offline Customer info table
                using (SqlConnection conn = (Application != null) ? Application.Settings.Database.Connection : ApplicationSettings.Database.LocalConnection)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(retailtrans.PartnerData.OFFLINECUSTID))
                        {
                            StringBuilder commandText = new StringBuilder();
                            commandText.AppendLine(" DELETE FROM NIM_OFFLINECUSTOMERINFO WHERE DATAAREAID='" + Application.Settings.Database.DataAreaID
                                                        + "' AND STOREID='" + sStoreId + "' AND TERMINALID='" + sTerminalId + "' AND OFFLINECUSID='" + retailtrans.PartnerData.OFFLINECUSTID + "'; ");


                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            using (SqlCommand cmd = new SqlCommand(commandText.ToString(), conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception ex1) { LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV1.PostCustomerClear : Offline Customer Info", ex1.Message, LSRetailPosis.LogTraceLevel.Error); }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
                #endregion
            }
            //end:Nim

            //Base
            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV1.PostCustomerClear", "After clearing a customer from the transaction.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region ICustomerTriggersV2 Members
        /// <summary>
        /// Triggered at the beginning
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreCustomer(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Base
            //LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV2.PreCustomer", "Triggered at the beginning.", LSRetailPosis.LogTraceLevel.Trace);

            //Start Nim
            LSRetailPosis.Transaction.RetailTransaction retailTrans = posTransaction as LSRetailPosis.Transaction.RetailTransaction;

            if (retailTrans != null)
            {
                if (retailTrans.PartnerData.SearchCustomer == false)
                {
                    MessageBox.Show("Sorry ! customer add / search is not possible here");
                    preTriggerResult.ContinueOperation = false;
                    retailTrans.PartnerData.SearchCustomer = true;
                }

               
            }
            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV2.PreCustomer", "Triggered at the beginning.", LSRetailPosis.LogTraceLevel.Trace);
            //End Nim
        }

        /// <summary>
        /// Triggered at the end
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostCustomer(IPosTransaction posTransaction)
        {
            //nimbus on 29/06/17
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if ((retailTrans != null) && (retailTrans != null))
            {

                string sStoreTaxState = "";
                string sStoreTaxCountry = "";

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                    sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry) != string.Empty)
                    sStoreTaxCountry = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry);

                string sCustTaxState = Convert.ToString(retailTrans.Customer.State);
                string sCustTaxCountry = Convert.ToString(retailTrans.Customer.Country);

                string sCustGSTNo = getCustomerGSTNumber(retailTrans.Customer.CustomerId);// according to mail of Shrikanta

                if (string.IsNullOrEmpty(sCustGSTNo))//iRes == 0
                {
                    if (sStoreTaxState != sCustTaxState) // for cal CGST and SGST only @PAN INDIA
                        retailTrans.Customer.State = sStoreTaxState;

                    if (sStoreTaxCountry != sCustTaxCountry) // for cal CGST and SGST only @PAN INDIA
                        retailTrans.Customer.Country = sStoreTaxCountry;
                }
                else //else part is added on 050918 
                {
                    if (string.IsNullOrEmpty(sCustTaxState))
                    {
                        return;
                    }
                }


                #region For auto loyalty add for PNG
                if (retailTrans.Customer != null)
                {
                    Loyalty.Loyalty objL = new Loyalty.Loyalty();
                    DE.ICardInfo cardInfo = null;
                    if (application != null)
                        objL.Application = application;
                    else
                        objL.Application = this.Application;
                    RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                    objL.AddLoyaltyRequest(retailTransaction, cardInfo);
                }
                #endregion

            }
            //end

            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV2.PostCustomer", "Triggered at the end.", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered just before the customer is set
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="customerId"></param>
        public void PreCustomerSet(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, string customerId)
        {
            //nimbus on 29/06/17
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if ((retailTrans != null) && (retailTrans != null))
            {

                string sStoreTaxState = "";
                string sStoreTaxCountry = "";

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                    sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry) != string.Empty)
                    sStoreTaxCountry = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry);

                string sCustTaxState = Convert.ToString(retailTrans.Customer.State);
                string sCustTaxCountry = Convert.ToString(retailTrans.Customer.Country);

                string sCustGSTNo = getCustomerGSTNumber(retailTrans.Customer.CustomerId);// according to mail of Shrikanta

                if (string.IsNullOrEmpty(sCustGSTNo))//iRes == 0
                {
                    if (sStoreTaxState != sCustTaxState) // for cal CGST and SGST only @PAN INDIA
                        retailTrans.Customer.State = sStoreTaxState;

                    if (sStoreTaxCountry != sCustTaxCountry) // for cal CGST and SGST only @PAN INDIA
                        retailTrans.Customer.Country = sStoreTaxCountry;
                }
                else //else part is added on 050918 
                {
                    if (string.IsNullOrEmpty(sCustTaxState))
                    {
                        return;
                    }
                }
            }
            //end

            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV2.PreCustomerSet", "Triggered just before the customer is set.", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered before customer search
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreCustomerSearch(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Start:Nim
            LSRetailPosis.Transaction.RetailTransaction retailTrans = posTransaction as LSRetailPosis.Transaction.RetailTransaction;
            bool isServiceItemExists = false;
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
            foreach (var sale in saleline)
            {
                if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                {
                    isServiceItemExists = true;
                    break;
                }
            }
            if (isServiceItemExists)
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 65999;
            }
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV2.PreCustomerSearch", "Triggered before customer search.", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered after customer search
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostCustomerSearch(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV2.PostCustomerSearch", "Triggered after customer search.", LSRetailPosis.LogTraceLevel.Trace);
        }


        #endregion

        //Start:Nim
        #region [Nimbus]

        private string getCustomerGSTNumber(string sCustId)
        {
            string sResult = "";

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select isnull(tri.REGISTRATIONNUMBER,'') from ");
            commandText.Append(" custtable c1 left join DIRPARTYTABLE dpt on dpt.recid=c1.PARTY");
            commandText.Append(" left join dirpartylocation dpl on dpl.PARTY=c1.PARTY");
            commandText.Append(" left join taxinformation_in tii on tii.REGISTRATIONLOCATION=dpl.LOCATION");
            commandText.Append(" left join TaxRegistrationNumbers_IN tri on tri.RECID=tii.GSTIN where");
            commandText.Append(" dpl.ISPRIMARY=1 and tii.ISPRIMARY=1 and c1.accountnum = '" + sCustId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        private void updateCustomerAdvanceAdjustment(string transactionid,
                                                    string sStoreId, string sTerminalId, int adjustment)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = string.Empty;

            commandText += " UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED = '" + adjustment + "' WHERE TRANSACTIONID ='" + transactionid.Trim() + "' AND RETAILSTOREID = '" + sStoreId + "' AND RETAILTERMINALID ='" + sTerminalId + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion
        //End:Nim
    }
}
