using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using LSRetailPosis.Settings;
using System.IO;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmFetchCustomerFromHO : frmTouchBase
    {

        [Import]
        private IApplication application;
        public IPosTransaction pos { get; set; }

        DataTable dtCustTable = new DataTable();
        DataTable dtDPTable = new DataTable();
        DataTable dtLPATable = new DataTable();
        DataTable dtRCTable = new DataTable();
        DataTable dtLEATable = new DataTable();
        DataTable dtDPLTable = new DataTable();
        DataTable dtLLTable = new DataTable();
        DataTable dtDPLRTable = new DataTable();
        DataTable dtDABPTable = new DataTable();
        DataTable dtRLoyaltyCustTable = new DataTable();
        DataTable dtRLoyaltyMSRCardTable = new DataTable();
        DataTable dtTaxinfo_in = new DataTable();
        DataTable dtTaxReg_IN = new DataTable();

        public frmFetchCustomerFromHO()
        {
            InitializeComponent();
        }
        public frmFetchCustomerFromHO(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();

            pos = posTransaction;
            application = Application;
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(mobileNumber.Text) || !string.IsNullOrEmpty(txtCustAcc.Text))
            {
                //if (!IsExistInLocal(mobileNumber.Text, txtCustAcc.Text))
                //{
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetCustomerDetail", mobileNumber.Text, txtCustAcc.Text);

                        if (Convert.ToBoolean(containerArray[1]) == true)
                        {
                            DataSet dsCD = new DataSet();
                            StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                            if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                            {
                                dsCD.ReadXml(srTransDetail);
                            }
                            if (dsCD != null && dsCD.Tables[0].Rows.Count > 0)
                            {
                                dtCustTable = dsCD.Tables[0];
                                dtDPTable = dsCD.Tables[1];
                                if (dsCD.Tables.Contains("LogisticsPostalAddress"))
                                {
                                    dtLPATable = dsCD.Tables["LogisticsPostalAddress"];
                                }
                                if (dsCD.Tables.Contains("RetailCustTable"))
                                {
                                    dtRCTable = dsCD.Tables["RetailCustTable"];
                                }
                                if (dsCD.Tables.Contains("LogisticsElectronicAddress"))
                                {
                                    dtLEATable = dsCD.Tables["LogisticsElectronicAddress"];
                                }

                                if (dsCD.Tables.Contains("DirPartyLocation"))
                                {
                                    dtDPLTable = dsCD.Tables["DirPartyLocation"];
                                }
                                if (dsCD.Tables.Contains("LogisticsLocation"))
                                {
                                    dtLLTable = dsCD.Tables["LogisticsLocation"];
                                }

                                if (dsCD.Tables.Contains("DirPartyLocationRole"))
                                {
                                    dtDPLRTable = dsCD.Tables["DirPartyLocationRole"];
                                }

                                if (dsCD.Tables.Contains("DirAddressBookParty"))
                                {
                                    dtDABPTable = dsCD.Tables["DirAddressBookParty"];
                                }

                                if (dsCD.Tables.Contains("RETAILLOYALTYCUSTTABLE"))
                                {
                                    dtRLoyaltyCustTable = dsCD.Tables["RETAILLOYALTYCUSTTABLE"];
                                }

                                if (dsCD.Tables.Contains("RETAILLOYALTYMSRCARDTABLE"))
                                {
                                    dtRLoyaltyMSRCardTable = dsCD.Tables["RETAILLOYALTYMSRCARDTABLE"];
                                }

                                if (dsCD.Tables.Contains("taxinformation_in"))
                                {
                                    dtTaxinfo_in = dsCD.Tables["taxinformation_in"];
                                }

                                if (dsCD.Tables.Contains("TaxRegistrationNumbers_IN"))
                                {
                                    dtTaxReg_IN = dsCD.Tables["TaxRegistrationNumbers_IN"];
                                }

                                SaveOrder();
                            }
                        }
                        else
                        {
                            MessageBox.Show("No record found in HO");
                            mobileNumber.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please contact to your admin for check real time service.");
                    mobileNumber.Focus();
                }
                //}
                //else
                //{
                //    MessageBox.Show("With this mobile no/customer account ,a customer already in your local system.");
                //    mobileNumber.Focus();
                //}
            }
            else
            {
                MessageBox.Show("Please enter a mobile no.");
                mobileNumber.Focus();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsExistInLocal(string sMobileNo, string sAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string commandText = "";

            if (!string.IsNullOrEmpty(sMobileNo))
            {
                commandText = "SELECT top 1 isnull(RECID,'') RECID" +
                                 " from CUSTTABLE where  RETAILMOBILEPRIMARY ='" + sMobileNo + "'";
            }
            else
            {
                commandText = " SELECT top 1 isnull(RECID,'') RECID" +
                                " from CUSTTABLE where  ACCOUNTNUM ='" + sAcc + "'";
            }

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private void SaveOrder()
        {
            int iCustTable = 0;
            int iDIRPARTYTABLE = 0;
            int iLOGISTICSPOSTALADDRESS = 0;
            int iRETAILCUSTTABLE = 0;
            int iLOGISTICSELECTRICADDRESS = 0;
            int iDirPartyLocation = 0;
            int iLogisticsLocation = 0;
            int iDirPartyLocationRole = 0;
            string sCustAcc = string.Empty;

            SqlTransaction transaction = null;

            #region CUSTTABLE
            if (dtCustTable != null && dtCustTable.Rows.Count > 0)
            {

                //string commandText = " IF NOT EXISTS(SELECT TOP 1 RECID FROM CUSTTABLE " +
                //                   " WHERE ACCOUNTNUM=@ACCOUNTNUM ) BEGIN" +
                //                   " INSERT INTO [CUSTTABLE]([ACCOUNTNUM],[CUSTGROUP],[CURRENCY],[TAXGROUP]," +
                //                   " [PARTY],[OCCUPATION],[RELIGION], " + //,[RETAILDOB],[RETAILMARRIAGEDATE]
                //                   " [RETAILMOBILEPRIMARY],[RETAILMOBILESECONDARY],[RETAILSALUTATION]," +
                //                   " [RETAILSTDCODE],[RECID],[DATAAREAID],[LanguageId])" +
                //                   " VALUES(@ACCOUNTNUM,@CUSTGROUP,@CURRENCY,@TAXGROUP,@PARTY,@OCCUPATION," +
                //                   " @RELIGION,@RETAILMOBILEPRIMARY,@RETAILMOBILESECONDARY," + //,@RETAILDOB,@RETAILMARRIAGEDATE
                //                   " @RETAILSALUTATION,@RETAILSTDCODE,@RECID,@DATAAREAID,@LanguageId) END";

                string commandText = " IF NOT EXISTS(SELECT TOP 1 RECID FROM CUSTTABLE " +
                                   " WHERE ACCOUNTNUM=@ACCOUNTNUM ) BEGIN" +
                                    " INSERT INTO [CUSTTABLE]([ACCOUNTNUM],[CUSTGROUP],[CURRENCY],[TAXGROUP]," +
                                     " [PARTY],[OCCUPATION],[RELIGION], " + //,[RETAILDOB],[RETAILMARRIAGEDATE]
                                     " [RETAILMOBILEPRIMARY],[RETAILMOBILESECONDARY],[RETAILSALUTATION]," +
                                     " [RETAILSTDCODE],[RECID],[DATAAREAID],GOVTIDENTITY,GOVTIDNO," +
                                     " ISKARIGAR,CustClassificationId,LanguageId,CRWRECEIPTEMAIL," +
                                     " BANKACCOUNT,IFSCCode,BankName,GOVTIDENTITY2,GOVTIDNO2)" +
                                     " VALUES(@ACCOUNTNUM,@CUSTGROUP,@CURRENCY,@TAXGROUP,@PARTY,@OCCUPATION," +
                                     " @RELIGION,@RETAILMOBILEPRIMARY,@RETAILMOBILESECONDARY," + //,@RETAILDOB,@RETAILMARRIAGEDATE
                                     " @RETAILSALUTATION,@RETAILSTDCODE,@RECID,@DATAAREAID," +
                                     " @GOVTIDENTITY,@GOVTIDNO,@ISKARIGAR,@CustClassificationId,@LanguageId,@CRWRECEIPTEMAIL," +
                                     " @BANKACCOUNT,@IFSCCode,@BankName,@GOVTIDENTITY2,@GOVTIDNO2) END";

                SqlConnection connection = new SqlConnection();
                try
                {
                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;


                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    transaction = connection.BeginTransaction();

                    SqlCommand command = new SqlCommand(commandText, connection, transaction);
                    command.Parameters.Clear();
                    //if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["ACCOUNTNUM"])))
                    sCustAcc = Convert.ToString(dtCustTable.Rows[0]["ACCOUNTNUM"]);
                    command.Parameters.Add("@ACCOUNTNUM", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["ACCOUNTNUM"]);
                    command.Parameters.Add("@CUSTGROUP", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtCustTable.Rows[0]["CUSTGROUP"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["CURRENCY"])))
                        command.Parameters.Add("@CURRENCY", SqlDbType.NVarChar, 3).Value = "";
                    else
                        command.Parameters.Add("@CURRENCY", SqlDbType.NVarChar, 3).Value = Convert.ToString(dtCustTable.Rows[0]["CURRENCY"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["TAXGROUP"])))
                        command.Parameters.Add("@TAXGROUP", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@TAXGROUP", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtCustTable.Rows[0]["TAXGROUP"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["PARTY"])))
                        command.Parameters.Add("@PARTY", SqlDbType.BigInt).Value = 0;
                    else
                        command.Parameters.Add("@PARTY", SqlDbType.BigInt).Value = Convert.ToInt64(dtCustTable.Rows[0]["PARTY"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["OCCUPATION"])))
                        command.Parameters.Add("@OCCUPATION", SqlDbType.NVarChar, 60).Value = "";
                    else
                        command.Parameters.Add("@OCCUPATION", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtCustTable.Rows[0]["OCCUPATION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RELIGION"])))
                        command.Parameters.Add("@RELIGION", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@RELIGION", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["RELIGION"]);

                    //if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RETAILDOB"])))
                    //    command.Parameters.Add("@RETAILDOB", SqlDbType.DateTime).Value = Convert.ToDateTime("1900-01-01").ToString("dd-MMM-yyyy");
                    //else
                    //    command.Parameters.Add("@RETAILDOB", SqlDbType.DateTime).Value = Convert.ToDateTime(dtCustTable.Rows[0]["RETAILDOB"]).ToString("dd-MMM-yyyy");
                    //if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RETAILMARRIAGEDATE"])))
                    //    command.Parameters.Add("@RETAILMARRIAGEDATE", SqlDbType.DateTime).Value = Convert.ToDateTime("1900-01-01").ToString("dd-MMM-yyyy");
                    //else
                    //    command.Parameters.Add("@RETAILMARRIAGEDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dtCustTable.Rows[0]["RETAILMARRIAGEDATE"]).ToString("dd-MMM-yyyy");

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RETAILMOBILEPRIMARY"])))
                        command.Parameters.Add("@RETAILMOBILEPRIMARY", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@RETAILMOBILEPRIMARY", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["RETAILMOBILEPRIMARY"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RETAILMOBILESECONDARY"])))
                        command.Parameters.Add("@RETAILMOBILESECONDARY", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@RETAILMOBILESECONDARY", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["RETAILMOBILESECONDARY"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RETAILSALUTATION"])))
                        command.Parameters.Add("@RETAILSALUTATION", SqlDbType.BigInt).Value = 0;
                    else
                        command.Parameters.Add("@RETAILSALUTATION", SqlDbType.BigInt).Value = Convert.ToInt64(dtCustTable.Rows[0]["RETAILSALUTATION"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["RETAILSTDCODE"])))
                        command.Parameters.Add("@RETAILSTDCODE", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@RETAILSTDCODE", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtCustTable.Rows[0]["RETAILSTDCODE"]);

                    command.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtCustTable.Rows[0]["RECID"]);
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtCustTable.Rows[0]["DATAAREAID"]);

                    command.Parameters.Add("@GOVTIDENTITY", SqlDbType.Int).Value = Convert.ToInt16(dtCustTable.Rows[0]["GOVTIDENTITY"]);
                    command.Parameters.Add("@GOVTIDNO", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["GOVTIDNO"]);
                    command.Parameters.Add("@ISKARIGAR", SqlDbType.Int).Value = Convert.ToInt16(dtCustTable.Rows[0]["ISKARIGAR"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["CustClassificationId"])))
                        command.Parameters.Add("@CustClassificationId", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@CustClassificationId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["CustClassificationId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["LanguageId"])))
                        command.Parameters.Add("@LanguageId", SqlDbType.NVarChar, 7).Value = "";
                    else
                        command.Parameters.Add("@LanguageId", SqlDbType.NVarChar, 7).Value = Convert.ToString(dtCustTable.Rows[0]["LanguageId"]);


                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["CRWRECEIPTEMAIL"])))
                        command.Parameters.Add("@CRWRECEIPTEMAIL", SqlDbType.NVarChar, 80).Value = "";
                    else
                        command.Parameters.Add("@CRWRECEIPTEMAIL", SqlDbType.NVarChar, 80).Value = Convert.ToString(dtCustTable.Rows[0]["CRWRECEIPTEMAIL"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["BankAccount"])))
                        command.Parameters.Add("@BankAccount", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@BankAccount", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["BankAccount"]);


                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["IFSCCode"])))
                        command.Parameters.Add("@IFSCCode", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@IFSCCode", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["IFSCCode"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtCustTable.Rows[0]["BankName"])))
                        command.Parameters.Add("@BankName", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@BankName", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["BankName"]);

                    command.Parameters.Add("@GovtIdentity2", SqlDbType.Int).Value = Convert.ToInt16(dtCustTable.Rows[0]["GovtIdentity2"]);
                    command.Parameters.Add("@GovtIdNo2", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["GovtIdNo2"]);



            #endregion

                    command.CommandTimeout = 0;
                    iCustTable = command.ExecuteNonQuery();


                    if (dtDPTable != null && dtDPTable.Rows.Count > 0)
                    {
                        #region DIRPARTYTABLE
                        string commandDIRPARTYTABLE = " IF NOT EXISTS(SELECT TOP 1 RECID FROM DIRPARTYTABLE " +
                                                    " WHERE RECID=@RECID ) BEGIN" +
                                                    " INSERT INTO [DIRPARTYTABLE](NAME,LANGUAGEID,NAMEALIAS,PARTYNUMBER,INSTANCERELATIONTYPE,RECID)" +
                                                    " VALUES(@NAME,@LANGUAGEID,@NAMEALIAS,@PARTYNUMBER,@INSTANCERELATIONTYPE,@RECID) END";

                        SqlCommand cmdDirParty = new SqlCommand(commandDIRPARTYTABLE, connection, transaction);
                        if (string.IsNullOrEmpty(Convert.ToString(dtDPTable.Rows[0]["NAME"])))
                            cmdDirParty.Parameters.Add("@NAME", SqlDbType.NVarChar, 100).Value = "";
                        else
                            cmdDirParty.Parameters.Add("@NAME", SqlDbType.NVarChar, 100).Value = Convert.ToString(dtDPTable.Rows[0]["NAME"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtDPTable.Rows[0]["LANGUAGEID"])))
                            cmdDirParty.Parameters.Add("@LANGUAGEID", SqlDbType.NVarChar, 7).Value = "";
                        else
                            cmdDirParty.Parameters.Add("@LANGUAGEID", SqlDbType.NVarChar, 7).Value = Convert.ToString(dtDPTable.Rows[0]["LANGUAGEID"]);
                        if (string.IsNullOrEmpty(Convert.ToString(dtDPTable.Rows[0]["NAMEALIAS"])))
                            cmdDirParty.Parameters.Add("@NAMEALIAS", SqlDbType.NVarChar, 20).Value = "";
                        else
                            cmdDirParty.Parameters.Add("@NAMEALIAS", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtDPTable.Rows[0]["NAMEALIAS"]);
                        if (string.IsNullOrEmpty(Convert.ToString(dtDPTable.Rows[0]["PARTYNUMBER"])))
                            cmdDirParty.Parameters.Add("@PARTYNUMBER", SqlDbType.NVarChar, 40).Value = "";
                        else
                            cmdDirParty.Parameters.Add("@PARTYNUMBER", SqlDbType.NVarChar, 40).Value = Convert.ToString(dtDPTable.Rows[0]["PARTYNUMBER"]);
                        if (string.IsNullOrEmpty(Convert.ToString(dtDPTable.Rows[0]["INSTANCERELATIONTYPE"])))
                            cmdDirParty.Parameters.Add("@INSTANCERELATIONTYPE", SqlDbType.BigInt).Value = 0;
                        else
                            cmdDirParty.Parameters.Add("@INSTANCERELATIONTYPE", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPTable.Rows[0]["INSTANCERELATIONTYPE"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtDPTable.Rows[0]["RECID"])))
                            cmdDirParty.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                        else
                            cmdDirParty.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPTable.Rows[0]["RECID"]);
                        cmdDirParty.CommandTimeout = 0;
                        iDIRPARTYTABLE = cmdDirParty.ExecuteNonQuery();
                        cmdDirParty.Dispose();

                        #endregion
                    }

                    if (dtLPATable != null && dtLPATable.Rows.Count > 0)
                    {
                        #region LOGISTICSPOSTALADDRESS
                        //if (iDIRPARTYTABLE == 1)
                        //{
                        //string commandLPA = " IF NOT EXISTS(SELECT TOP 1 RECID FROM LOGISTICSPOSTALADDRESS " +
                        //                    " WHERE RECID=@RECID ) BEGIN" +
                        //                    " INSERT INTO [LOGISTICSPOSTALADDRESS](LOCATION,VALIDFROM,VALIDTO,ISPRIVATE," +//VALIDFROMTZID,VALIDTOTZID,
                        //                    " ZIPCODE,[STATE],COUNTY,[ADDRESS],COUNTRYREGIONID,CITY,STREET,RECID)" +
                        //                    " VALUES(@LOCATION,@VALIDFROM,@VALIDTO," +//,@VALIDFROMTZID,@VALIDTOTZID
                        //                    " @ISPRIVATE,@ZIPCODE,@STATE,@COUNTY,@ADDRESS,@COUNTRYREGIONID,@CITY,@STREET,@RECID) END";

                        string commandLPA = " DELETE FROM LOGISTICSPOSTALADDRESS " +
                                " WHERE LOCATION=@LOCATION; " +
                                " BEGIN INSERT INTO [LOGISTICSPOSTALADDRESS](LOCATION,ISPRIVATE," +//,VALIDFROM,VALIDTO,VALIDFROMTZID,VALIDTOTZID,
                                " ZIPCODE,[STATE],COUNTY,[ADDRESS],COUNTRYREGIONID,CITY,STREET,RECID)" +
                                " VALUES(@LOCATION,@ISPRIVATE,@ZIPCODE,@STATE,@COUNTY,@ADDRESS," +//,@VALIDFROM,@VALIDTO,@VALIDFROMTZID,@VALIDTOTZID
                                " @COUNTRYREGIONID,@CITY,@STREET,@RECID) END ";

                        for (int ItemCount = 0; ItemCount < dtLPATable.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdLPA = new SqlCommand(commandLPA, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["LOCATION"])))
                                cmdLPA.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdLPA.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["LOCATION"]);
                            //if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["VALIDFROM"])))
                            //    cmdLPA.Parameters.Add("@VALIDFROM", SqlDbType.DateTime).Value = Convert.ToDateTime("1900-01-01");
                            //else
                            //    cmdLPA.Parameters.Add("@VALIDFROM", SqlDbType.DateTime).Value = Convert.ToDateTime(dtLPATable.Rows[ItemCount]["VALIDFROM"]);
                            //if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["VALIDTO"])))
                            //    cmdLPA.Parameters.Add("@VALIDTO", SqlDbType.DateTime).Value = Convert.ToDateTime("1900-01-01");
                            //else
                            //    cmdLPA.Parameters.Add("@VALIDTO", SqlDbType.DateTime).Value = Convert.ToDateTime(dtLPATable.Rows[ItemCount]["VALIDTO"]);

                            //cmdLPA.Parameters.Add("@VALIDFROMTZID", SqlDbType.Int).Value = ApplicationSettings.Terminal.TerminalId;
                            //cmdLPA.Parameters.Add("@VALIDTOTZID", SqlDbType.Int).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["ISPRIVATE"])))
                                cmdLPA.Parameters.Add("@ISPRIVATE", SqlDbType.Int).Value = 0;
                            else
                                cmdLPA.Parameters.Add("@ISPRIVATE", SqlDbType.Int).Value = Convert.ToInt16(dtLPATable.Rows[ItemCount]["ISPRIVATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["ZIPCODE"])))
                                cmdLPA.Parameters.Add("@ZIPCODE", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdLPA.Parameters.Add("@ZIPCODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["ZIPCODE"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["STATE"])))
                                cmdLPA.Parameters.Add("@STATE", SqlDbType.NVarChar, 10).Value = "";
                            else
                                cmdLPA.Parameters.Add("@STATE", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["STATE"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["COUNTY"])))
                                cmdLPA.Parameters.Add("@COUNTY", SqlDbType.NVarChar, 10).Value = "";
                            else
                                cmdLPA.Parameters.Add("@COUNTY", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["COUNTY"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["ADDRESS"])))
                                cmdLPA.Parameters.Add("@ADDRESS", SqlDbType.NVarChar, 250).Value = "";
                            else
                                cmdLPA.Parameters.Add("@ADDRESS", SqlDbType.NVarChar, 250).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["ADDRESS"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["COUNTRYREGIONID"])))
                                cmdLPA.Parameters.Add("@COUNTRYREGIONID", SqlDbType.NVarChar, 10).Value = "";
                            else
                                cmdLPA.Parameters.Add("@COUNTRYREGIONID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["COUNTRYREGIONID"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["CITY"])))
                                cmdLPA.Parameters.Add("@CITY", SqlDbType.NVarChar, 60).Value = "";
                            else
                                cmdLPA.Parameters.Add("@CITY", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["CITY"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["STREET"])))
                                cmdLPA.Parameters.Add("@STREET", SqlDbType.NVarChar, 250).Value = "";
                            else
                                cmdLPA.Parameters.Add("@STREET", SqlDbType.NVarChar, 250).Value = Convert.ToString(dtLPATable.Rows[ItemCount]["STREET"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLPATable.Rows[ItemCount]["RECID"])))
                                cmdLPA.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdLPA.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtLPATable.Rows[ItemCount]["RECID"]);

                            cmdLPA.CommandTimeout = 0;
                            iLOGISTICSPOSTALADDRESS = cmdLPA.ExecuteNonQuery();
                            cmdLPA.Dispose();
                        }
                        //}
                        #endregion
                    }

                    if (dtRCTable != null && dtRCTable.Rows.Count > 0)
                    {
                        #region RETAILCUSTTABLE
                        //if (iCustTable == 1)
                        //{
                        //string commandRCT = " IF NOT EXISTS(SELECT TOP 1 RECID FROM RETAILCUSTTABLE " +
                        //                    " WHERE RECID=@RECID ) BEGIN" +
                        //                    " INSERT INTO [RETAILCUSTTABLE](AccountNum,RECEIPTEMAIL,NONCHARGABLEACCOUNT," +
                        //                    " REQUIRESAPPROVAL,RECID,DATAAREAID)" +
                        //                    " VALUES(@AccountNum,@RECEIPTEMAIL,@NONCHARGABLEACCOUNT,@REQUIRESAPPROVAL,@RECID,@DATAAREAID) END";

                        string commandRCT = " IF NOT EXISTS(SELECT TOP 1 RECID FROM RETAILCUSTTABLE " +
                                  " WHERE ACCOUNTNUM=@ACCOUNTNUM and DATAAREAID=@DATAAREAID ) BEGIN" +
                                  " INSERT INTO [RETAILCUSTTABLE](ACCOUNTNUM,RECEIPTEMAIL,NONCHARGABLEACCOUNT,REQUIRESAPPROVAL,RECID,DATAAREAID)" +
                                       " VALUES(@ACCOUNTNUM,@RECEIPTEMAIL,@NONCHARGABLEACCOUNT,@REQUIRESAPPROVAL,@RECID,@DATAAREAID) END";

                        SqlCommand cmdRCT = new SqlCommand(commandRCT, connection, transaction);
                        if (string.IsNullOrEmpty(Convert.ToString(dtRCTable.Rows[0]["AccountNum"])))
                            cmdRCT.Parameters.Add("@AccountNum", SqlDbType.NVarChar, 20).Value = "";
                        else
                            cmdRCT.Parameters.Add("@AccountNum", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtRCTable.Rows[0]["AccountNum"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtRCTable.Rows[0]["RECEIPTEMAIL"])))
                            cmdRCT.Parameters.Add("@RECEIPTEMAIL", SqlDbType.NVarChar, 20).Value = "";
                        else
                            cmdRCT.Parameters.Add("@RECEIPTEMAIL", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtRCTable.Rows[0]["RECEIPTEMAIL"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtRCTable.Rows[0]["NONCHARGABLEACCOUNT"])))
                            cmdRCT.Parameters.Add("@NONCHARGABLEACCOUNT", SqlDbType.Int).Value = 0;
                        else
                            cmdRCT.Parameters.Add("@NONCHARGABLEACCOUNT", SqlDbType.Int).Value = Convert.ToInt16(dtRCTable.Rows[0]["NONCHARGABLEACCOUNT"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtRCTable.Rows[0]["REQUIRESAPPROVAL"])))
                            cmdRCT.Parameters.Add("@REQUIRESAPPROVAL", SqlDbType.Int).Value = 0;
                        else
                            cmdRCT.Parameters.Add("@REQUIRESAPPROVAL", SqlDbType.Int).Value = Convert.ToInt16(dtRCTable.Rows[0]["REQUIRESAPPROVAL"]);
                        if (string.IsNullOrEmpty(Convert.ToString(dtRCTable.Rows[0]["RECID"])))
                            cmdRCT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                        else
                            cmdRCT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRCTable.Rows[0]["RECID"]);

                        cmdRCT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtRCTable.Rows[0]["DATAAREAID"]);
                        cmdRCT.CommandTimeout = 0;
                        iRETAILCUSTTABLE = cmdRCT.ExecuteNonQuery();
                        cmdRCT.Dispose();
                        //}
                        #endregion
                    }

                    if (dtLEATable != null && dtLEATable.Rows.Count > 0)
                    {
                        #region LogisticsElectronicAddress
                        //if (iCustTable == 1)
                        //{
                        string commandLEA = " IF NOT EXISTS(SELECT TOP 1 RECID FROM LogisticsElectronicAddress " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [LogisticsElectronicAddress](LOCATION," +
                                            " ISPRIVATE," +
                                            " LOCATOR,[TYPE],RECVERSION,DESCRIPTION,ISPRIMARY,RECID)" +
                                            " VALUES(@LOCATION, @ISPRIVATE,@LOCATOR,@TYPE,@RECVERSION," +
                                            " @DESCRIPTION,@ISPRIMARY,@RECID) END";

                        for (int ItemCount = 0; ItemCount < dtLEATable.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdLEA = new SqlCommand(commandLEA, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["LOCATION"])))
                                cmdLEA.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdLEA.Parameters.Add("@LOCATION", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtLEATable.Rows[ItemCount]["LOCATION"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["ISPRIVATE"])))
                                cmdLEA.Parameters.Add("@ISPRIVATE", SqlDbType.Int).Value = 0;
                            else
                                cmdLEA.Parameters.Add("@ISPRIVATE", SqlDbType.Int).Value = Convert.ToInt16(dtLEATable.Rows[ItemCount]["ISPRIVATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["LOCATOR"])))
                                cmdLEA.Parameters.Add("@LOCATOR", SqlDbType.NVarChar, 255).Value = "";
                            else
                                cmdLEA.Parameters.Add("@LOCATOR", SqlDbType.NVarChar, 255).Value = Convert.ToString(dtLEATable.Rows[ItemCount]["LOCATOR"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["TYPE"])))
                                cmdLEA.Parameters.Add("@TYPE", SqlDbType.Int).Value = 0;
                            else
                                cmdLEA.Parameters.Add("@TYPE", SqlDbType.Int).Value = Convert.ToInt16(dtLEATable.Rows[ItemCount]["TYPE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["RECVERSION"])))
                                cmdLEA.Parameters.Add("@RECVERSION", SqlDbType.BigInt).Value = 0;
                            else
                                cmdLEA.Parameters.Add("@RECVERSION", SqlDbType.BigInt).Value = Convert.ToInt64(dtLEATable.Rows[ItemCount]["RECVERSION"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["DESCRIPTION"])))
                                cmdLEA.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 60).Value = "";
                            else
                                cmdLEA.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtLEATable.Rows[ItemCount]["DESCRIPTION"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["ISPRIMARY"])))
                                cmdLEA.Parameters.Add("@ISPRIMARY", SqlDbType.Int).Value = 0;
                            else
                                cmdLEA.Parameters.Add("@ISPRIMARY", SqlDbType.Int).Value = Convert.ToInt16(dtLEATable.Rows[ItemCount]["ISPRIMARY"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLEATable.Rows[ItemCount]["RECID"])))
                                cmdLEA.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdLEA.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtLEATable.Rows[ItemCount]["RECID"]);

                            cmdLEA.CommandTimeout = 0;
                            iLOGISTICSELECTRICADDRESS = cmdLEA.ExecuteNonQuery();
                            cmdLEA.Dispose();
                        }
                        //}
                        #endregion
                    }

                    if (dtDPLTable != null && dtDPLTable.Rows.Count > 0)
                    {
                        #region DIRPARTYLOCATION
                        //if (iCustTable == 1)
                        //{
                        string commandDPL = " IF NOT EXISTS(SELECT TOP 1 RECID FROM DirPartyLocation " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [DirPartyLocation](IsLocationOwner " +
                                                                      ",IsPostalAddress" +
                                                                      ",ISPRIMARY" +
                                                                      ",ISPRIVATE" +
                                                                      ",Location" +
                                                                      ",Party" +
                                                                      ",RECID)" +
                                            " VALUES(@IsLocationOwner " +
                                                    ",@IsPostalAddress" +
                                                    ",@ISPRIMARY" +
                                                    ",@ISPRIVATE" +
                                                    ",@Location" +
                                                    ",@Party" +
                                                    ",@RECID) END";

                        for (int ItemCount = 0; ItemCount < dtDPLTable.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdDPL = new SqlCommand(commandDPL, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["IsLocationOwner"])))
                                cmdDPL.Parameters.Add("@IsLocationOwner", SqlDbType.Int).Value = 0;
                            else
                                cmdDPL.Parameters.Add("@IsLocationOwner", SqlDbType.Int).Value = Convert.ToInt16(dtDPLTable.Rows[ItemCount]["IsLocationOwner"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["IsPostalAddress"])))
                                cmdDPL.Parameters.Add("@IsPostalAddress", SqlDbType.Int).Value = 0;
                            else
                                cmdDPL.Parameters.Add("@IsPostalAddress", SqlDbType.Int).Value = Convert.ToInt16(dtDPLTable.Rows[ItemCount]["IsPostalAddress"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["ISPRIMARY"])))
                                cmdDPL.Parameters.Add("@ISPRIMARY", SqlDbType.Int).Value = 0;
                            else
                                cmdDPL.Parameters.Add("@ISPRIMARY", SqlDbType.Int).Value = Convert.ToInt16(dtDPLTable.Rows[ItemCount]["ISPRIMARY"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["ISPRIVATE"])))
                                cmdDPL.Parameters.Add("@ISPRIVATE", SqlDbType.Int).Value = 0;
                            else
                                cmdDPL.Parameters.Add("@ISPRIVATE", SqlDbType.Int).Value = Convert.ToInt16(dtDPLTable.Rows[ItemCount]["ISPRIVATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["LOCATION"])))
                                cmdDPL.Parameters.Add("@LOCATION", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDPL.Parameters.Add("@LOCATION", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPLTable.Rows[ItemCount]["LOCATION"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["Party"])))
                                cmdDPL.Parameters.Add("@Party", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdDPL.Parameters.Add("@Party", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtDPLTable.Rows[ItemCount]["Party"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLTable.Rows[ItemCount]["RECID"])))
                                cmdDPL.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDPL.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPLTable.Rows[ItemCount]["RECID"]);

                            cmdDPL.CommandTimeout = 0;
                            iDirPartyLocation = cmdDPL.ExecuteNonQuery();
                            cmdDPL.Dispose();
                        }
                        //}
                        #endregion
                    }

                    if (dtLLTable != null && dtLLTable.Rows.Count > 0)
                    {
                        #region LOGISTICSLOCATION
                        //if (iCustTable == 1)
                        //{
                        string commandLL = " IF NOT EXISTS(SELECT TOP 1 RECID FROM LOGISTICSLOCATION " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [LOGISTICSLOCATION](locationid,description,IsPostalAddress,RECID )" +
                                            " VALUES(@locationid,@description,@IsPostalAddress,@RECID ) END";

                        for (int ItemCount = 0; ItemCount < dtLLTable.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdLL = new SqlCommand(commandLL, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtLLTable.Rows[ItemCount]["locationid"])))
                                cmdLL.Parameters.Add("@locationid", SqlDbType.NVarChar, 30).Value = 0;
                            else
                                cmdLL.Parameters.Add("@locationid", SqlDbType.NVarChar, 30).Value = Convert.ToString(dtLLTable.Rows[ItemCount]["locationid"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLLTable.Rows[ItemCount]["IsPostalAddress"])))
                                cmdLL.Parameters.Add("@IsPostalAddress", SqlDbType.Int).Value = 0;
                            else
                                cmdLL.Parameters.Add("@IsPostalAddress", SqlDbType.Int).Value = Convert.ToInt16(dtLLTable.Rows[ItemCount]["IsPostalAddress"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtLLTable.Rows[ItemCount]["description"])))
                                cmdLL.Parameters.Add("@description", SqlDbType.NVarChar, 60).Value = "";
                            else
                                cmdLL.Parameters.Add("@description", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtLLTable.Rows[ItemCount]["description"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtLLTable.Rows[ItemCount]["RECID"])))
                                cmdLL.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdLL.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtLLTable.Rows[ItemCount]["RECID"]);

                            cmdLL.CommandTimeout = 0;
                            iLogisticsLocation = cmdLL.ExecuteNonQuery();
                            cmdLL.Dispose();
                        }
                        // }
                        #endregion
                    }

                    if (dtDPLRTable != null && dtDPLRTable.Rows.Count > 0)
                    {
                        #region DirPartyLocationRole
                        //if (iCustTable == 1)
                        //{
                        string commandDIPLR = " IF NOT EXISTS(SELECT TOP 1 RECID FROM DirPartyLocationRole " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [DirPartyLocationRole](PARTYLOCATION,LOCATIONROLE,RECID )" +
                                            " VALUES(@PARTYLOCATION,@LOCATIONROLE,@RECID ) END";

                        for (int ItemCount = 0; ItemCount < dtDPLRTable.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdDPLR = new SqlCommand(commandDIPLR, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLRTable.Rows[ItemCount]["PARTYLOCATION"])))
                                cmdDPLR.Parameters.Add("@PARTYLOCATION", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDPLR.Parameters.Add("@PARTYLOCATION", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPLRTable.Rows[ItemCount]["PARTYLOCATION"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLRTable.Rows[ItemCount]["LOCATIONROLE"])))
                                cmdDPLR.Parameters.Add("@LOCATIONROLE", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDPLR.Parameters.Add("@LOCATIONROLE", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPLRTable.Rows[ItemCount]["LOCATIONROLE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDPLRTable.Rows[ItemCount]["RECID"])))
                                cmdDPLR.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDPLR.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtDPLRTable.Rows[ItemCount]["RECID"]);

                            cmdDPLR.CommandTimeout = 0;
                            iDirPartyLocationRole = cmdDPLR.ExecuteNonQuery();
                            cmdDPLR.Dispose();
                        }
                        //}
                        #endregion
                    }

                    if (dtDABPTable != null && dtDABPTable.Rows.Count > 0)
                    {
                        #region DirAddressBookParty
                        //if (iCustTable == 1)
                        //{
                        string commandDABP = " IF NOT EXISTS(SELECT TOP 1 RECID FROM DirAddressBookParty " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [DirAddressBookParty](PARTY,ADDRESSBOOK,RECID )" +
                                            " VALUES(@PARTY,@ADDRESSBOOK,@RECID ) END";

                        for (int ItemCount = 0; ItemCount < dtDABPTable.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdDABP = new SqlCommand(commandDABP, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtDABPTable.Rows[ItemCount]["PARTY"])))
                                cmdDABP.Parameters.Add("@PARTY", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDABP.Parameters.Add("@PARTY", SqlDbType.BigInt).Value = Convert.ToInt64(dtDABPTable.Rows[ItemCount]["PARTY"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDABPTable.Rows[ItemCount]["ADDRESSBOOK"])))
                                cmdDABP.Parameters.Add("@ADDRESSBOOK", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDABP.Parameters.Add("@ADDRESSBOOK", SqlDbType.BigInt).Value = Convert.ToInt64(dtDABPTable.Rows[ItemCount]["ADDRESSBOOK"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtDABPTable.Rows[ItemCount]["RECID"])))
                                cmdDABP.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDABP.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtDABPTable.Rows[ItemCount]["RECID"]);

                            cmdDABP.CommandTimeout = 0;
                            cmdDABP.ExecuteNonQuery();
                            cmdDABP.Dispose();
                        }
                        //}
                        #endregion
                    }

                    if (dtRLoyaltyCustTable != null && dtRLoyaltyCustTable.Rows.Count > 0)
                    {
                        #region RETAILLOYALTYCUSTTABLE
                        if (iCustTable == 1)
                        {
                            string commandDIPLR = " IF NOT EXISTS(SELECT TOP 1 RECID FROM RETAILLOYALTYCUSTTABLE " +
                                                " WHERE RECID=@RECID ) BEGIN" +
                                                " INSERT INTO [RETAILLOYALTYCUSTTABLE](LOYALTYCUSTID,CUSTNAME,ACCOUNTNUM,DATAAREAID,RECID )" +
                                                " VALUES(@LOYALTYCUSTID,@CUSTNAME,@ACCOUNTNUM,@DATAAREAID,@RECID ) END";

                            for (int ItemCount = 0; ItemCount < dtRLoyaltyCustTable.Rows.Count; ItemCount++)
                            {
                                SqlCommand cmdRLCustT = new SqlCommand(commandDIPLR, connection, transaction);
                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["LOYALTYCUSTID"])))
                                    cmdRLCustT.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLCustT.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["LOYALTYCUSTID"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["CUSTNAME"])))
                                    cmdRLCustT.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLCustT.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["CUSTNAME"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["ACCOUNTNUM"])))
                                    cmdRLCustT.Parameters.Add("@ACCOUNTNUM", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLCustT.Parameters.Add("@ACCOUNTNUM", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["ACCOUNTNUM"]);

                                cmdRLCustT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["DATAAREAID"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyCustTable.Rows[ItemCount]["RECID"])))
                                    cmdRLCustT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                                else
                                    cmdRLCustT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRLoyaltyCustTable.Rows[ItemCount]["RECID"]);

                                cmdRLCustT.CommandTimeout = 0;
                                cmdRLCustT.ExecuteNonQuery();
                                cmdRLCustT.Dispose();
                            }
                        }
                        #endregion
                    }

                    if (dtRLoyaltyMSRCardTable != null && dtRLoyaltyMSRCardTable.Rows.Count > 0)
                    {
                        #region RETAILLOYALTYMSRCARDTABLE
                        if (iCustTable == 1)
                        {
                            string commandDIPLR = " IF NOT EXISTS(SELECT TOP 1 RECID FROM RETAILLOYALTYMSRCARDTABLE " +
                                                " WHERE RECID=@RECID ) BEGIN" +
                                                " INSERT INTO [RETAILLOYALTYMSRCARDTABLE](CARDNUMBER,LINKTYPE,LINKID,LOYALTYSCHEMEID,LOYALTYTENDER,LOYALTYCUSTID,DATAAREAID,RECID )" +
                                                " VALUES(@CARDNUMBER,@LINKTYPE,@LINKID,@LOYALTYSCHEMEID,@LOYALTYTENDER,@LOYALTYCUSTID,@DATAAREAID,@RECID ) END";

                            for (int ItemCount = 0; ItemCount < dtRLoyaltyMSRCardTable.Rows.Count; ItemCount++)
                            {
                                SqlCommand cmdRLMSRCT = new SqlCommand(commandDIPLR, connection, transaction);
                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["CARDNUMBER"])))
                                    cmdRLMSRCT.Parameters.Add("@CARDNUMBER", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLMSRCT.Parameters.Add("@CARDNUMBER", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["CARDNUMBER"]);

                                cmdRLMSRCT.Parameters.Add("@LINKTYPE", SqlDbType.Int).Value = Convert.ToInt16(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LINKTYPE"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LINKID"])))
                                    cmdRLMSRCT.Parameters.Add("@LINKID", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLMSRCT.Parameters.Add("@LINKID", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LINKID"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LOYALTYSCHEMEID"])))
                                    cmdRLMSRCT.Parameters.Add("@LOYALTYSCHEMEID", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLMSRCT.Parameters.Add("@LOYALTYSCHEMEID", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LOYALTYSCHEMEID"]);

                                cmdRLMSRCT.Parameters.Add("@LOYALTYTENDER", SqlDbType.Int).Value = Convert.ToInt16(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LOYALTYTENDER"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LOYALTYCUSTID"])))
                                    cmdRLMSRCT.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar).Value = "";
                                else
                                    cmdRLMSRCT.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["LOYALTYCUSTID"]);

                                cmdRLMSRCT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["DATAAREAID"]);

                                if (string.IsNullOrEmpty(Convert.ToString(dtRLoyaltyMSRCardTable.Rows[ItemCount]["RECID"])))
                                    cmdRLMSRCT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                                else
                                    cmdRLMSRCT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRLoyaltyMSRCardTable.Rows[ItemCount]["RECID"]);

                                cmdRLMSRCT.CommandTimeout = 0;
                                cmdRLMSRCT.ExecuteNonQuery();
                                cmdRLMSRCT.Dispose();
                            }
                        }
                        #endregion
                    }

                    if (dtTaxinfo_in != null && dtTaxinfo_in.Rows.Count > 0)//added on 210918 (if GST no added later then fetch and get the gst details
                    {
                        #region taxinformation_in
                        string commandTaxInfo = " IF NOT EXISTS(SELECT TOP 1 RECID FROM taxinformation_in " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [taxinformation_in](TIN,REGISTRATIONLOCATION,STCREGISTRATIONNUMBERTABLE,ECCNUMBER," +
                                            " ISPRIMARY,RECID,SALESTAXREGISTRATIONNUMBER,GSTIN )" +
                                            " VALUES(@TIN,@REGISTRATIONLOCATION,@STCREGISTRATIONNUMBERTABLE," +
                                            " @ECCNUMBER,@ISPRIMARY,@RECID,@SALESTAXREGISTRATIONNUMBER,@GSTIN ) END";

                        for (int ItemCount = 0; ItemCount < dtTaxinfo_in.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdTaxInfo = new SqlCommand(commandTaxInfo, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["TIN"])))
                                cmdTaxInfo.Parameters.Add("@TIN", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@TIN", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["TIN"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["REGISTRATIONLOCATION"])))
                                cmdTaxInfo.Parameters.Add("@REGISTRATIONLOCATION", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@REGISTRATIONLOCATION", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["REGISTRATIONLOCATION"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["STCREGISTRATIONNUMBERTABLE"])))
                                cmdTaxInfo.Parameters.Add("@STCREGISTRATIONNUMBERTABLE", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@STCREGISTRATIONNUMBERTABLE", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["STCREGISTRATIONNUMBERTABLE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["ECCNUMBER"])))
                                cmdTaxInfo.Parameters.Add("@ECCNUMBER", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@ECCNUMBER", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["ECCNUMBER"]);

                            cmdTaxInfo.Parameters.Add("@ISPRIMARY", SqlDbType.Int).Value = Convert.ToInt16(dtTaxinfo_in.Rows[ItemCount]["ISPRIMARY"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["RECID"])))
                                cmdTaxInfo.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["RECID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["SALESTAXREGISTRATIONNUMBER"])))
                                cmdTaxInfo.Parameters.Add("@SALESTAXREGISTRATIONNUMBER", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@SALESTAXREGISTRATIONNUMBER", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["SALESTAXREGISTRATIONNUMBER"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxinfo_in.Rows[ItemCount]["GSTIN"])))
                                cmdTaxInfo.Parameters.Add("@GSTIN", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxInfo.Parameters.Add("@GSTIN", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxinfo_in.Rows[ItemCount]["GSTIN"]);

                            cmdTaxInfo.CommandTimeout = 0;
                            cmdTaxInfo.ExecuteNonQuery();
                            cmdTaxInfo.Dispose();
                        }
                        #endregion
                    }

                    if (dtTaxReg_IN != null && dtTaxReg_IN.Rows.Count > 0)//added on 210918 (if GST no added later then fetch and get the gst details
                    {
                        #region TaxRegistrationNumbers_IN
                        string commandTaxReg = " IF NOT EXISTS(SELECT TOP 1 RECID FROM TaxRegistrationNumbers_IN " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [TaxRegistrationNumbers_IN](REGISTRATIONNUMBER,RECID,TTYPE )" +
                                            " VALUES(@REGISTRATIONNUMBER,@RECID,@TTYPE ) END";

                        for (int ItemCount = 0; ItemCount < dtTaxReg_IN.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdTaxReg = new SqlCommand(commandTaxReg, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxReg_IN.Rows[ItemCount]["REGISTRATIONNUMBER"])))
                                cmdTaxReg.Parameters.Add("@REGISTRATIONNUMBER", SqlDbType.NVarChar, 15).Value = "";
                            else
                                cmdTaxReg.Parameters.Add("@REGISTRATIONNUMBER", SqlDbType.NVarChar, 15).Value = Convert.ToString(dtTaxReg_IN.Rows[ItemCount]["REGISTRATIONNUMBER"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxReg_IN.Rows[ItemCount]["RECID"])))
                                cmdTaxReg.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdTaxReg.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtTaxReg_IN.Rows[ItemCount]["RECID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTaxReg_IN.Rows[ItemCount]["TTYPE"])))
                                cmdTaxReg.Parameters.Add("@TTYPE", SqlDbType.NVarChar, 1).Value = "";
                            else
                                cmdTaxReg.Parameters.Add("@TTYPE", SqlDbType.NVarChar, 1).Value = Convert.ToString(dtTaxReg_IN.Rows[ItemCount]["TTYPE"]);


                            cmdTaxReg.CommandTimeout = 0;
                            cmdTaxReg.ExecuteNonQuery();
                            cmdTaxReg.Dispose();
                        }
                        #endregion
                    }


                    transaction.Commit();
                    command.Dispose();
                    transaction.Dispose();

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer  " + sCustAcc + "   fetched successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        this.Close();
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                    }

                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }

                }
            }
        }
    }
}
