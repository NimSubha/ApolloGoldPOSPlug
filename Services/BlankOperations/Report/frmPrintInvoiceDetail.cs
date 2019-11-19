using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using LSRetailPosis.Settings;
using System.Data.SqlClient;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmPrintInvoiceDetail : Form
    {
        BlankOperations oBlank = new BlankOperations();
        SqlConnection connection;
        // string sTransactionId = "";
        string sCustName = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";
        string sCPanNo = "-";

        string sInvoiceNo = "";
        string sInvDt = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";

        string sAmtinwds = "";
        string sCustCode = "-";
        string sReceiptNo = "-";
        string sCustNationality = "";
        string sCustState = "";

        string sTerminal = string.Empty;
        string sTitle = string.Empty;

        string sInvoiceFooter = "-";
        string sTime = "";

        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sDuplicateCopy = string.Empty;//aded on 14/04/2014 R.Hossain
        byte[] bCompImage = null;
        string sCurrencySymbol = "";
        Double dTotAmt = 0;

        #region enum MetalType
        enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
        }
        #endregion
        string sBC = string.Empty;
        string sSM = "";
        RetailTransaction retailTrans;
        string sEmail = string.Empty;
        string sMetalRates = "";
        string sOperatorName = "";
        Int64 iTime_Ticks = 0;
        string sCustomTransType = "";

        public frmPrintInvoiceDetail()
        {
            InitializeComponent();
        }

        public frmPrintInvoiceDetail(IPosTransaction transaction)
        {
            InitializeComponent();

            DE.ICustomer transactionCustomer = null;

            if (transaction is IRetailTransaction)
            {
                RetailTransaction retailTransaction = (RetailTransaction)transaction;
                transactionCustomer = retailTransaction.Customer;

            }
            else if (transaction is CustomerPaymentTransaction)
            {
                CustomerPaymentTransaction customerPaymentTransaction = (CustomerPaymentTransaction)transaction;
                transactionCustomer = customerPaymentTransaction.Customer;
            }

            if (transactionCustomer != null)
            {
                string sCustId = transactionCustomer.CustomerId;
                sCustName = transactionCustomer.Name;
                sCustAddress = transactionCustomer.Address;
                sContactNo = transactionCustomer.Telephone;
                sEmail = transactionCustomer.Email;

                if (!string.IsNullOrEmpty(sContactNo))
                    sContactNo = getISD(sCustId) + " " + sContactNo;

                if (string.IsNullOrEmpty(sContactNo))
                {
                    sContactNo = getISD(sCustId) + " " + getMobilePriamry(sCustId);
                }

                if (string.IsNullOrEmpty(sEmail))
                {
                    sEmail = getEmail(sCustId);
                }
            }



            retailTrans = transaction as RetailTransaction;

            sInvoiceNo = transaction.TransactionId;
            sTerminal = transaction.TerminalId;

            if (retailTrans != null)
            {
                sSM = getSalesManName(sInvoiceNo, sTerminal);

                sTerminal = retailTrans.TerminalId;

                iTime_Ticks = Convert.ToInt64(retailTrans.EndDateTime.TimeOfDay.TotalSeconds);

                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);

                if (Convert.ToString(retailTrans.OperatorName) != string.Empty)
                    sOperatorName = Convert.ToString(retailTrans.OperatorName);

                sCustomTransType = GetCustomTransType(sInvoiceNo, retailTrans.TerminalId);

                if (retailTrans.SaleIsReturnSale)
                    sTitle = "RETURN INVOICE";
                else if (sCustomTransType == "1")
                    sTitle = "PURCHASE";
                else if (sCustomTransType == "3")
                    sTitle = "EXCHANGE";
                else if (sCustomTransType == "2")
                    sTitle = "PURCHASE RETURN";
                else if (sCustomTransType == "4")
                    sTitle = "EXCHANGE RETURN";
                else
                    sTitle = "INVOICE";

                if (string.IsNullOrEmpty(retailTrans.Customer.Name))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName)))
                    {
                        sCustName = Convert.ToString(retailTrans.PartnerData.LCCustomerName);
                        sCustAddress = Convert.ToString(retailTrans.PartnerData.LCCustomerAddress);
                        sContactNo = Convert.ToString(retailTrans.PartnerData.LCCustomerContactNo);
                        sCPinCode = "-";
                    }
                    else //  added on 12/04/2014 for print local cust later ( from show journal)
                    {
                        GetLocalCustomerInfo(sInvoiceNo);
                    }

                }
                //-------

                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.EndDateTime.ToShortDateString();

                if (retailTrans.EndDateTime != null)
                    sTime = retailTrans.EndDateTime.ToShortTimeString(); //("HH:mm")

                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)
                    sInventLocationId = Convert.ToString(retailTrans.InventLocationId);
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.CustomerId)))
                    sCustCode = Convert.ToString(retailTrans.Customer.CustomerId);
                if (Convert.ToString(retailTrans.ReceiptId) != string.Empty)
                    sReceiptNo = Convert.ToString(retailTrans.ReceiptId);

                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.Country)))
                    sCustNationality = Convert.ToString(retailTrans.Customer.Country);

                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.State)))
                     sCustState = Convert.ToString(retailTrans.Customer.State);

                sBC = sReceiptNo;
            }
        }
        private string getISD(string sCustNo)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ISNULL(RETAILSTDCODE,'') RETAILSTDCODE FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustNo + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }

        private string getMobilePriamry(string sCustNo)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ISNULL(RETAILMOBILEPRIMARY,'') RETAILMOBILEPRIMARY FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustNo + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }

        private string getEmail(string sCustNo)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ISNULL(CRWRECEIPTEMAIL,'') CRWRECEIPTEMAIL FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustNo + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }

        private string GetCustomTransType(string sTransId, string sTerminalId) // Sales/Purchase/Exchange.....
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TRANSACTIONTYPE FROM RETAIL_CUSTOMCALCULATIONS_TABLE WHERE TRANSACTIONID='" + sTransId + "' and STOREID='" + ApplicationSettings.Terminal.StoreId + "' and TERMINALID='" + sTerminalId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }
        private void GetLocalCustomerInfo(string sTransId)
        {

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;


            string sql = "select ISNULL(LocalCustomerName,'') AS LocalCustomerName," +
                         " ISNULL(LocalCustomerAddress,'') AS LocalCustomerAddress," +
                         " ISNULL(LocalCustomerContactNo,'') AS LocalCustomerContactNo " +
                         " FROM RETAILTRANSACTIONTABLE " +
                         " WHERE TRANSACTIONID='" + sTransId + "'";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dtCustInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtCustInfo);

            if (dtCustInfo != null && dtCustInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerName"]) == string.Empty)
                    sCustName = "-";
                else
                    sCustName = Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerName"]);
                if (Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerAddress"]) == string.Empty)
                    sCustAddress = "-";
                else
                    sCustAddress = Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerAddress"]);

                if (Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerContactNo"]) == string.Empty)
                    sContactNo = "-";
                else
                    sContactNo = Convert.ToString(dtCustInfo.Rows[0]["LocalCustomerContactNo"]);
            }
        }

        private string getSalesManName(string sTransId, string sTerminal)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select top 1 d.NAME as Name from RETAILTRANSACTIONSALESTRANS A" +
                        " LEFT JOIN dbo.RETAILSTAFFTABLE AS T11 on A.STAFF =T11.STAFFID " +
                        " LEFT JOIN dbo.HCMWORKER AS T22 ON T11.STAFFID = T22.PERSONNELNUMBER" +
                        " left join dbo.DIRPARTYTABLE as d on d.RECID = T22.PERSON " +
                        " Where TRANSACTIONID='" + sTransId + "' and a.TERMINALID='" + sTerminal + "' and isnull(A.STAFF,'')!=''";

            //string commandText = "select d.NAME as Name from RETAILSTAFFTABLE r" +
            //                  " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID" +
            //                  " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
            //                  " where r.STAFFID='" + sStaffId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private void frmPrintInvoiceDetail_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;

            localReport.ReportPath = "rptInvoiceDetails.rdlc";

            DataTable dtSalesData = new DataTable();
            DataSet dsTender = new DataSet();

            DataTable dtMatalRate = new DataTable();

            GetMetalRates(ref dtMatalRate);
            GetSalesInvData(ref dtSalesData);
            GetTender(ref dsTender);

            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "Detail";
            dsSalesOrder.Value = dtSalesData;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtSalesData));

            ReportDataSource RDTender = new ReportDataSource();
            RDTender.Name = "Tender";
            RDTender.Value = dsTender.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dsTender.Tables[0]));

            ReportParameter[] param = new ReportParameter[14];
            param[0] = new ReportParameter("CustId", sCustCode);
            param[1] = new ReportParameter("CName", sCustName);
            param[2] = new ReportParameter("CContactNo", sContactNo);
            param[3] = new ReportParameter("InvDate", sInvDt + "   " + sTime);

            param[4] = new ReportParameter("StoreName", sStoreName);
            param[5] = new ReportParameter("Nationality", sCustNationality);
            param[6] = new ReportParameter("Region", sStorePhNo);
            param[7] = new ReportParameter("ReceiptNo", sReceiptNo);
            param[8] = new ReportParameter("EMail", sEmail); // added on 14/04/2014 req from Sailendra Da
            param[9] = new ReportParameter("SM", sSM); //Currencysymbol added on 02/09/2014 req from Sailendra Da
            param[10] = new ReportParameter("MRates", sMetalRates);
            param[11] = new ReportParameter("VType", sTitle);
            param[12] = new ReportParameter("POSRate", sMetalRates);
            param[13] = new ReportParameter("CustState", sCustState);
            

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetSalesInvData(ref DataTable dtCol)
        {
            string sIngrDetails = string.Empty;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            string sQuery = "GETTRANSACTIONDETAILS";
            SqlCommand command = new SqlCommand(sQuery, SqlCon);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSID", SqlDbType.NVarChar).Value = sInvoiceNo;
            command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = sTerminal;// ApplicationSettings.Terminal.TerminalId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dtCol);

            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            #region Ingredients Item
           /* DataTable dtIng = new DataTable();
            for (int i = 0; i <= dtCol.Rows.Count - 1; i++) //.Select("SomeIntColumn > 0")
            {
                dTotAmt = dTotAmt + Convert.ToDouble(decimal.Round(Convert.ToDecimal(dtCol.Rows[i]["AMOUNT"]), 2, MidpointRounding.AwayFromZero));
                dtIng = GetIngredientItemData(sInvoiceNo, Convert.ToString(dtCol.Rows[i]["ITEMDESC"]));

                sIngrDetails = "";
                decimal dTotIngStnWt = 0;
                decimal dTotIngDmdWt = 0;

                for (int j = 0; j <= dtIng.Rows.Count - 1; j++)
                {
                    int iMetalType = GetMetalType(Convert.ToString(dtIng.Rows[j]["ITEMID"]));

                    if (iMetalType == (int)MetalType.Stone)
                        dTotIngStnWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                    else if (iMetalType == (int)MetalType.LooseDmd)
                        dTotIngDmdWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                }
                if (dTotIngStnWt > 0)
                {
                    sIngrDetails = "Stone" + " : " + Convert.ToString(dTotIngStnWt);
                    if (dTotIngDmdWt > 0)
                    {
                        if (!string.IsNullOrEmpty(sIngrDetails))
                            sIngrDetails = sIngrDetails + ", Diamond" + " : " + Convert.ToString(dTotIngDmdWt);
                    }
                }
                else if (dTotIngDmdWt > 0)
                {
                    if (!string.IsNullOrEmpty(sIngrDetails))
                        sIngrDetails = sIngrDetails + ", Diamond" + " : " + Convert.ToString(dTotIngDmdWt);
                    else
                        sIngrDetails = "Diamond" + " : " + Convert.ToString(dTotIngDmdWt);
                }

                dtCol.Rows[i]["ITEMDESC"] = sIngrDetails;

                dtCol.AcceptChanges();
            }*/
            #endregion
        }

        private void GetTender(ref DataSet dsSTender)
        {
            string sStoreNo = ApplicationSettings.Terminal.StoreId;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME, M.TENDERTYPE,CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT," +
                                " (ISNULL(CARDORACCOUNT,'') + ISNULL(CREDITVOUCHERID,'') + ISNULL(stuff(ISNULL(GIFTCARDID,''),1,LEN(ISNULL(GIFTCARDID,''))-4,REPLICATE('x', LEN(ISNULL(GIFTCARDID,''))-4)),'')) AS CARDNO" + //+ ISNULL(GIFTCARDID,'') ADDED ON 16/07/2014
                                " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
                                " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
                                " WHERE M.TRANSACTIONID = @TRANSACTIONID AND N.CHANNEL = @CHANNEL  AND M.TERMINAL = '" + sTerminal + "' AND M.TRANSACTIONSTATUS = 0 " +
                //" UNION SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
                                " UNION ALL SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
                                " + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))))AS AMOUNT, '' AS CARDNO" +
                                " FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM AND A.TERMINALID = B.TERMINALID" +
                                " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID  LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
                                " WHERE A.transactionid = @TRANSACTIONID AND A.TERMINALID = '" + sTerminal + "'" +
                                " AND A.TransactionStatus = 0  AND D.ITEMTYPE = 2" +
                                " ORDER BY M.TENDERTYPE ";


            SqlCommand command = new SqlCommand(sqlsubDtl, SqlCon);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTender, "Tender");

            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
        }

        protected int GetMetalType(string sItemId)
        {
            int iMetalType = 100;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + sItemId + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return iMetalType;

        }

        public DataTable GetIngredientItemData(string sTransId, string sSKUNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT ITEMID,QTY  FROM  RETAIL_SALE_INGREDIENTS_DETAILS WHERE TRANSACTIONID = '" + sTransId + "'  and SKUNUMBER ='" + sSKUNo + "'";

                DataTable dt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DataTable GetConfigListForRateShow()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT top 4 isnull(SEQUENCE,0) SEQUENCE ,isnull(CONFIG,'') CONFIG FROM RATESHOWCONFIGURATION order by SEQUENCE asc";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                if (SqlCon.State == ConnectionState.Closed)
                    SqlCon.Open();

                return CustBalDt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void GetMetalRates(ref DataTable dTMetalRates)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            SqlCon.Open();

            string sQuery = "GETMETALRATESFORINVOICE";
            SqlCommand command = new SqlCommand(sQuery, SqlCon);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(sInvDt);
            command.Parameters.Add("@TransTime", SqlDbType.BigInt).Value = iTime_Ticks;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dTMetalRates);
            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            //if(dTMetalRates != null && dTMetalRates.Rows.Count > 0)
            //{
            //    for(int i = 1; i <= dTMetalRates.Rows.Count; i++)
            //    {
            //        if(i == 1)
            //            sMetalRates = Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
            //        else
            //            sMetalRates = sMetalRates + Environment.NewLine + Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
            //    }
            //}

            DataTable dt = GetConfigListForRateShow();
            sMetalRates = "Metal Rate :  ";
            if (dt != null && dt.Rows.Count > 0)
            {
                for (int i = 1; i <= dt.Rows.Count; i++)
                {

                    if (dTMetalRates != null && dTMetalRates.Rows.Count > 0)
                    {
                        foreach (DataRow row in dTMetalRates.Rows)
                        {
                            if (Convert.ToString(dt.Rows[i - 1]["CONFIG"]) == Convert.ToString(row["CONFIGIDSTANDARD"]))
                            {
                                if (Convert.ToInt16(dt.Rows[i - 1]["SEQUENCE"]) == 1)
                                {
                                    sMetalRates = sMetalRates + Convert.ToString(dt.Rows[i - 1]["CONFIG"]) + " : " + Convert.ToString(row["RATES"]) + "  ";
                                }

                                if (Convert.ToInt16(dt.Rows[i - 1]["SEQUENCE"]) == 2)
                                {
                                    sMetalRates = sMetalRates + Convert.ToString(dt.Rows[i - 1]["CONFIG"]) + " : " + Convert.ToString(row["RATES"]) + "  ";
                                }

                                if (Convert.ToInt16(dt.Rows[i - 1]["SEQUENCE"]) == 3)
                                {
                                    sMetalRates = sMetalRates + Convert.ToString(dt.Rows[i - 1]["CONFIG"]) + " : " + Convert.ToString(row["RATES"]) + "  ";
                                }
                                if (Convert.ToInt16(dt.Rows[i - 1]["SEQUENCE"]) == 4)
                                {
                                    sMetalRates = sMetalRates + Convert.ToString(dt.Rows[i - 1]["CONFIG"]) + " : " + Convert.ToString(row["RATES"]) + "  ";
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
