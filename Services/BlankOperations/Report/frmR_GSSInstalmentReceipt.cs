using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using LSRetailPosis.Settings;
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmR_GSSInstalmentReceipt : Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        // string sTransactionId = "";
        string sCustName = "-";
        string sCustId = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";

        string sInvNo = "";
        string sReceiptNo = "";
        string sReceiptVoucherNo = "";
        string sAmount = "";
        string sReceiptDate = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";
        string sDetailsLine = "";
        string sSchemeCode = "";
        string sInvoiceFooter = "-";
        string sTerminal = string.Empty;
        string sMaturityDate = string.Empty;
        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sGSSAccNumber = "";
        string sCurrencySymbol = "";

        string sStoreArabicName = string.Empty;
        string sPaidBy = string.Empty;
        string sBC = string.Empty;
        string sTime = "";
        string sInvDt = "";
        RetailTransaction retailTrans = null;
        int isMGDStore = 0;
        Int64 iTime_Ticks = 0;
        string sMetalRates = "";
        string sOperatorName = "";
        string sEmail = "";
        string sAccumulatedAmt = "";
        string sAccumulatedQty = "";
        int iNoOfEMI = 0;
        string stringNoOfEMI = "";
        decimal dAcQty = 0m;
        string sCurrentBookedAmtQty = "";
        string sFooterMOPName = "";
        string sFooterMOPValue = "";

        decimal dTaxPct = 0m;
        decimal dTaxAmt = 0m;
        decimal dExcludTaxAmt = 0m;
        decimal dBookedQty = 0m;
        bool bGSSTaxApplicable = false;
        string sCompanyTaxRegNo = "";
        string sGovtIdInfo = "";
        int iLang = 0;

        string sText1 = "";
        string sText2 = "";
        string sText3 = "";
        string sText4 = "";
        string sText5 = "";
        string sText6 = "";
        string sText7 = "";
        string sText8 = "";
        string sText9 = "";
        string sText10 = "";
        string sText11 = "";
        string sText12 = "";
        string sText13 = "";
        string sText14 = "";
        string sText15 = "";
        string sText16 = "";
        string sText17 = "";
        string sText18 = "";
        string sText19 = "";
        string sText20 = "";
        string sText21 = "";
        string sText22 = "";
        string sText23 = "";
        string sText24 = "";
        string sText25 = "";
        string sText26 = "";
        string sText27 = "";
        string sText28 = "";
        string sText29 = "";
        string sText30 = "";
        decimal dAmt = 0m;

        private enum IdentityProof
        {
            Govt_Id = 0,
            Driving_License = 1,
            Passport_No = 2
        }

        public frmR_GSSInstalmentReceipt(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmR_GSSInstalmentReceipt(IPosTransaction posTransaction, SqlConnection conn, string _sTransId, string _sAmt, string sGSSAccNo, string sTerminalId, int iLanguage)
        {
            InitializeComponent();
            sTerminal = sTerminalId;
            iLang = iLanguage;

            #region Language wise text

            sText1 = "Received with thanks from";//
            sText2 = "Net Value including";//
            sText3 = "Net value including tax in words";//
            sText4 = "Against Order no is";//
            sText5 = "Gold fixing done";//
            sText6 = "Paid by";//
            sText7 = "Tax Invoice";//
            sText8 = "Advance Receipt"; //
            sText9 = "Registration No";//
            sText10 = "Against order number is"; //
            sText11 = "Credit Note";//
            sText12 = "GIFT CARD RECEIPT";
            sText13 = "Against booking of";
            sText14 = "Ref Receipt No";
            sText15 = "and Order no is";
            sText16 = "Rate Freezed";//  
            sText17 = "Rate Freezed Done";//
            sText18 = "And Booked Quantity"; //
            sText19 = "Terms And Conditions Apply"; //
            sText20 = "Accumulated Advance Amount";
            sText21 = "Accumulated number of payments";
            sText22 = "Accumulated Gross Quantity";
            sText23 = "Booked Against this Voucher";
            sText24 = "Amount";
            sText25 = "With";
            sText26 = "TAX";
            sText27 = "Quantity";
            sText28 = "in words";
            sText29 = "Net value including";
            sText30 = "Net value in words";
            #endregion

            #region[Param Info]
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.CustomerId)))
                    sCustId = Convert.ToString(retailTrans.Customer.CustomerId);
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                //if(Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                //    sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);

                if (string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.Telephone)))
                {
                    sContactNo = getISD(sCustId) + " " + getMobilePriamry(sCustId);
                }
                //sCPanNo

                if (Convert.ToString(retailTrans.Customer.Email) != string.Empty)
                    sEmail = Convert.ToString(retailTrans.Customer.Email);

                if (string.IsNullOrEmpty(sEmail))
                {
                    sEmail = getEmail(sCustId);
                }


                //sCPanNo
                sCurrencySymbol = oBlank.GetCurrencySymbol();
                //-------
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                {
                    sReceiptNo = _sTransId;// Convert.ToString(retailTrans.TransactionId);
                    sReceiptVoucherNo = retailTrans.ReceiptId;
                }

                if (Convert.ToString(sReceiptVoucherNo) == string.Empty)
                    sReceiptVoucherNo = GetReciptVouNo(_sTransId, conn);
                string sBookedQty = "0.000";
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                {
                    sBookedQty = GetCurrentBookedQty(retailTrans.TransactionId, retailTrans.TerminalId, retailTrans.StoreId, conn);
                }

                //if(retailTrans.BeginDateTime != null)
                //    sReceiptDate = retailTrans.BeginDateTime.ToShortDateString();
                bGSSTaxApplicable = IsGSSEmiInclOfTax();

                dTaxPct = getGSSTaxPercentage();
                dAmt = decimal.Round(Convert.ToDecimal(_sAmt), 2, MidpointRounding.AwayFromZero);//Convert.ToDecimal(_sAmt);
                dTaxAmt = decimal.Round(Convert.ToDecimal(dAmt * dTaxPct / (100 + dTaxPct)), 2, MidpointRounding.AwayFromZero);
                dExcludTaxAmt = dAmt - dTaxAmt;
                decimal dAccumulatedAmt = 0m;
                dAccumulatedAmt = Convert.ToDecimal(getGSSAccumulatedAmt(sGSSAccNo, ref iNoOfEMI, ref dAcQty));

                string sTaxOrGST = "";

                if (ApplicationSettings.Terminal.StoreCountry == "SG" || ApplicationSettings.Terminal.StoreCountry == "MY")
                    sTaxOrGST = "GST";
                else
                    sTaxOrGST = sText26; //"TAX";//ضريبة

                sAmount = oBlank.Amtinwds(Convert.ToDouble(_sAmt));

                if (bGSSTaxApplicable)
                {
                    sCompanyTaxRegNo = "" + sTaxOrGST + " " + sText9 + " : " + "  " + oBlank.GetCompanyTaxRegNum(conn);

                    sDetailsLine = sCurrencySymbol + "  " + dExcludTaxAmt + "  " + "  " + sText25 + "  " + "" + dTaxPct + " %" + " " + sTaxOrGST + " : " +
                               " " + dTaxAmt + " " + sText1 + "  " + sCustName + " ;  " + Environment.NewLine + Environment.NewLine + " " + sText29 + " " + sTaxOrGST + "" + " " + sCurrencySymbol + " " + _sAmt;

                    if (string.IsNullOrEmpty(sBookedQty))
                        sBookedQty = "0";

                    //if (Convert.ToDecimal(dExcludTaxAmt) > 0 && Convert.ToDecimal(sBookedQty) > 0)
                    //    sCurrentBookedAmtQty = sText5 + " @" + " " + (decimal.Round(Convert.ToDecimal(dExcludTaxAmt) / Convert.ToDecimal(sBookedQty), 2, MidpointRounding.AwayFromZero)) + " / GM " + "" + sText18 + " = " + " " + (sBookedQty) + " Gms";

                    decimal dAccTaxAmt = decimal.Round(Convert.ToDecimal(dAccumulatedAmt * dTaxPct / (100 + dTaxPct)), 2, MidpointRounding.AwayFromZero);
                    decimal dAccExcludTaxAmt = dAccumulatedAmt - dAccTaxAmt;

                    sAccumulatedAmt = sText20 + " :  " + (dAccExcludTaxAmt) + "  " + sText25 + "  " + sTaxOrGST + " : " + dAccTaxAmt + " = " + sCurrencySymbol + " ";// + (dAccumulatedAmt) + "" + Environment.NewLine + Environment.NewLine + " " + sText22 + " :" + " " + (dAcQty) + " Gms";

                    sAmount = sText29 + "  " + sTaxOrGST + "  " + sText28 + "  : " + " " + sAmount;
                }
                else
                {
                    sDetailsLine = sCurrencySymbol + " " + _sAmt + " " + sText1 + "  " + "" + sCustName + " ";
                    sCurrentBookedAmtQty = sText23 + " " + ": " + sText24 + " : " + (_sAmt) + " " + sCurrencySymbol + "";// , " + sText27 + " :" + " " + (sBookedQty) + " g"
                    sAccumulatedAmt = sText20 + "  :" + " " + (dAccumulatedAmt) + " " + sCurrencySymbol + "";// ,  " + sText27 + " :" + " " + (dAcQty) + " g"
                    sAmount = " " + sText30 + " " + sAmount;
                }

                //----------store Info


                // sAccumulatedQty = "Accumulated Advance Qty :" + " " + (dAcQty) + " g";

                stringNoOfEMI = sText21 + " :" + " " + (iNoOfEMI);

                //getGSSAccumulatedAmount

                GetGSSMaturityDate(sGSSAccNo, conn); // added on 29/03/2014 req by Sailendra da. dev by R.Hossain
                //if(Convert.ToString(retailTrans.StoreName) != string.Empty)
                //    sStoreName = Convert.ToString(retailTrans.StoreName);
                //if(Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                //    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);
                ////if(Convert.ToString(retailTrans.StorePhone) != string.Empty)
                //if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.StorePhone)))
                //    sStorePhNo = Convert.ToString(retailTrans.StorePhone);

                sGovtIdInfo = GetCustomerGovtIdInfo(sCustId);

                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                //if(Convert.ToString(retailTrans.InventLocationId) != string.Empty)
                //    sInventLocationId = Convert.ToString(retailTrans.InventLocationId);
                sInventLocationId = ApplicationSettings.Terminal.InventLocationId;

                sOperatorName = getSalesManName(retailTrans.TransactionId, retailTrans.TerminalId, retailTrans.StoreId);

                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.BeginDateTime.ToShortDateString();

                if (retailTrans.EndDateTime != null)
                    sTime = retailTrans.BeginDateTime.ToShortTimeString(); //("HH:mm")

                TimeSpan t = TimeSpan.FromSeconds(iTime_Ticks);

                string answer = string.Format("{0:D2}h:{1:D2}m",//:{2:D2}s:{3:D3}ms
                                t.Hours,
                                t.Minutes); //

                sInvDt = getTransDateAndTime(_sTransId, retailTrans);


                iTime_Ticks = getTransTime(_sTransId, retailTrans);
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                {
                    sReceiptNo = _sTransId;
                    sInvNo = retailTrans.ReceiptId;// _sTransId;// Convert.ToString(retailTrans.TransactionId);
                    sReceiptVoucherNo = retailTrans.ReceiptId;
                }
            }
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo, ref sStoreArabicName, ref isMGDStore);
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            sGSSAccNumber = sGSSAccNo; // added on 18/04/2014

            #endregion
        }
        private decimal getGSSTaxPercentage()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            decimal sResult = 0m;

            string commandText = string.Empty;
            commandText = "select top 1 isnull(GSSTAXPERCENTAGE,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;

        }

        private bool IsGSSEmiInclOfTax()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select isnull(GSSEmiInclOfTax,0) from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) GSSADJUSTMENTITEMID FROM [RETAILPARAMETERS]");
            sbQuery.Append(" where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private string AdjustmentItemName(string sItemId)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select F.Name from INVENTTABLE D ");
            sbQuery.Append(" LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID ");
            sbQuery.Append(" LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F");
            sbQuery.Append(" ON E.RECID = F.PRODUCT and F.LANGUAGEID='en-us' where d.ItemId='" + sItemId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private void frmR_GSSInstalmentReceipt_Load(object sender, EventArgs e)
        {
            rptGSSViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = rptGSSViewer.LocalReport;
            localReport.ReportPath = "rptGSSInstalmentReceipt.rdlc";


            DataSet dsTender = new DataSet();
            DataSet dsTaxInfo = new DataSet();
            DataSet dsPaymentInfo = new DataSet();
            DataTable dtMatalRate = new DataTable();
            DataTable dtTaxGSS = new DataTable();

            GetMetalRates(ref dtMatalRate);

            GetTender(ref dsTender);
            GetTaxInfo(ref dsTaxInfo);
            GetPayInfo(ref dsPaymentInfo);

            if (bGSSTaxApplicable)
            {
                GetTaxGSSData(ref dtTaxGSS);
            }

            ReportDataSource dsGSSInstalmentReceipt = new ReportDataSource();

            dsGSSInstalmentReceipt.Name = "DataSet1";
            dsGSSInstalmentReceipt.Value = dsTender.Tables[0];

            this.rptGSSViewer.LocalReport.DataSources.Clear();
            this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dsTender.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "DataSet2";
            RDTaxInfo.Value = dsTaxInfo.Tables[0];
            this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet2", dsTaxInfo.Tables[0]));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));


            if (bGSSTaxApplicable)
            {
                ReportDataSource dsTaxGSS = new ReportDataSource();
                dsTaxGSS.Name = "GSSTAX";
                dsTaxGSS.Value = dtTaxGSS;
                this.rptGSSViewer.LocalReport.DataSources.Add(new ReportDataSource("GSSTAX", dtTaxGSS));
            }


            ReportParameter[] param = new ReportParameter[33];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
            //   param[4] = new ReportParameter("ReceiptNo", sReceiptVoucherNo);
            param[4] = new ReportParameter("ReceiptNo", sInvNo);
            param[5] = new ReportParameter("RecDate", sReceiptDate);
            if (!string.IsNullOrEmpty(sSchemeCode))
                param[6] = new ReportParameter("SchemeCode", sSchemeCode);
            else
                param[6] = new ReportParameter("SchemeCode", "-");

            param[7] = new ReportParameter("InstallNo", Convert.ToString(iNoOfEMI));

            param[8] = new ReportParameter("StoreAddress", sStoreAddress);
            param[9] = new ReportParameter("StorePhone", sStorePhNo);

            if (!string.IsNullOrEmpty(sAmount))
                param[10] = new ReportParameter("Amtinwds", sAmount);
            else
                param[10] = new ReportParameter("Amtinwds", "zero");

            param[11] = new ReportParameter("DetailsLine", sDetailsLine);
            param[12] = new ReportParameter("CId", sCustId);
            param[13] = new ReportParameter("MaturityDate", sMaturityDate);
            param[14] = new ReportParameter("VoucherNo", sReceiptVoucherNo);
            param[15] = new ReportParameter("CompName", sCompanyName); // added on 14/04/214 req from Sailendra Da
            param[16] = new ReportParameter("CIN", sCINNo); // added on 14/04/214 req from Sailendra Da
            param[17] = new ReportParameter("GSSAccNumber", sGSSAccNumber); // added on 14/04/214 req from Sailendra Da

            param[18] = new ReportParameter("MRATES", sMetalRates);
            param[19] = new ReportParameter("StrArabicName", sStoreArabicName);
            param[20] = new ReportParameter("StoreName", sStoreName);

            param[21] = new ReportParameter("OperatorName", sOperatorName);
            param[22] = new ReportParameter("PaidBy", sPaidBy);
            param[23] = new ReportParameter("Email", sEmail);
            param[24] = new ReportParameter("TransId", sReceiptNo);
            param[25] = new ReportParameter("AccumulatedAmt", sAccumulatedAmt);
            param[26] = new ReportParameter("NoOfEMI", stringNoOfEMI);
            param[27] = new ReportParameter("CurrentBookedAmtQty", sCurrentBookedAmtQty);
            param[28] = new ReportParameter("CompanyTaxRegNo", sCompanyTaxRegNo);
            param[29] = new ReportParameter("GovtId", sGovtIdInfo);

            param[30] = new ReportParameter("NetPayAmt", Convert.ToString(dAmt));
            param[31] = new ReportParameter("FooterMOPValues", sFooterMOPValue);
            param[32] = new ReportParameter("FooterMOPDetails", sFooterMOPName);

            this.rptGSSViewer.LocalReport.SetParameters(param);

            this.rptGSSViewer.RefreshReport();
        }

        private void GetTaxGSSData(ref DataTable dtCol)
        {
            DataTable tblTaxInfo = new DataTable("GSTTAX");

            string sAdJustItem = AdjustmentItemID();
            string sItemName = AdjustmentItemName(sAdJustItem);

            tblTaxInfo.Columns.Add("EXCLVATAMT", typeof(decimal));
            tblTaxInfo.Columns.Add("TAXPCT", typeof(decimal));
            tblTaxInfo.Columns.Add("TAXAMOUNT", typeof(decimal));
            tblTaxInfo.Columns.Add("TOTAMOUNT", typeof(decimal));
            tblTaxInfo.Columns.Add("BOOKEDQTY", typeof(decimal));
            tblTaxInfo.Columns.Add("NATUREOFADV", typeof(string));

            tblTaxInfo.Rows.Add(dExcludTaxAmt, dTaxPct, dTaxAmt, dExcludTaxAmt + dTaxAmt, dBookedQty, "");

            dtCol = tblTaxInfo;

            if (dtCol != null && dtCol.Rows.Count < 6 && dtCol.Rows.Count > 0)
            {
                for (int j = dtCol.Rows.Count; j < 6; j++)
                    dtCol.Rows.Add();
            }
        }

        private void GetTender(ref DataSet dsSTender)
        {
            //string sqlsubDtl = " SELECT DISTINCT N.NAME, M.TENDERTYPE,M.CARDORACCOUNT,"+
            //                    " CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT" +
            //                    " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
            //                    " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
            //                    " WHERE M.TRANSACTIONID = @TRANSACTIONID" +
            //                    " ORDER BY M.TENDERTYPE ";

            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME, M.TENDERTYPE,CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT," +
                                " (ISNULL(CARDORACCOUNT,'') + ISNULL(CREDITVOUCHERID,'') + ISNULL(stuff(ISNULL(GIFTCARDID,''),1,LEN(ISNULL(GIFTCARDID,''))-4,REPLICATE('x', LEN(ISNULL(GIFTCARDID,''))-4)),'')) AS CARDORACCOUNT" +
                                " ,CONVERT(VARCHAR,M.TRANSDATE,106) AS TRANSDATE" +
                                " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
                                " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
                                " WHERE M.TRANSACTIONID = @TRANSACTIONID AND N.CHANNEL = @CHANNEL AND M.TERMINAL = '" + sTerminal + "' AND M.TRANSACTIONSTATUS = 0" +
                                " ORDER BY M.TENDERTYPE ";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTender, "Tender");
            if (dsSTender.Tables[0].Rows.Count > 0)
                sReceiptDate = Convert.ToString(dsSTender.Tables[0].Rows[0]["TRANSDATE"]);

            if (dsSTender.Tables[0] != null && dsSTender.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dsSTender.Tables[0].Rows)
                {
                    string group = row["Name"].ToString();
                    decimal amt = Math.Abs(Convert.ToDecimal(row["AMOUNT"]));

                    if (string.IsNullOrEmpty(sPaidBy))
                        sPaidBy = sText6 + " - " + group + " : " + amt;
                    else
                        sPaidBy += "   " + group + " : " + amt;

                }

                for (int i = 0; i <= dsSTender.Tables[0].Rows.Count - 1; i++)
                {
                    if (string.IsNullOrEmpty(sFooterMOPName))
                        sFooterMOPName = Convert.ToString(dsSTender.Tables[0].Rows[i]["NAME"]);
                    else
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dsSTender.Tables[0].Rows[i]["NAME"])))
                            sFooterMOPName += " , " + Convert.ToString(dsSTender.Tables[0].Rows[i]["NAME"]);
                    }

                    if (string.IsNullOrEmpty(sFooterMOPValue))
                        sFooterMOPValue = Convert.ToString(Math.Abs(Convert.ToDecimal(dsSTender.Tables[0].Rows[i]["AMOUNT"])));
                    else
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dsSTender.Tables[0].Rows[i]["AMOUNT"])))
                            sFooterMOPValue += " , " + Convert.ToString(Math.Abs(Convert.ToDecimal(dsSTender.Tables[0].Rows[i]["AMOUNT"])));
                    }
                }
            }


        }

        private void GetTaxInfo(ref DataSet dsSTaxInfo)
        {
            string sqlTaxInfo = " DECLARE @TINCST AS NVARCHAR(20)  DECLARE @TINVAT AS NVARCHAR(20)  SELECT @TINCST = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'')" +
                                " FROM TAXREGISTRATIONNUMBERS_IN AS A, TAXINFORMATION_IN AS B, INVENTLOCATIONLOGISTICSLOCATION AS C," +
                                " INVENTLOCATION AS D,RETAILCHANNELTABLE AS E WHERE D.INVENTLOCATIONID=E.INVENTLOCATION AND C.INVENTLOCATION=D.RECID" +
                                " AND B.ISPRIMARY=1" + // ADDED ON 02/06/2014
                                " AND C.ISPRIMARY=1" + // ADDED ON 02/06/2014
                                " AND B.REGISTRATIONLOCATION=C.LOCATION AND B.SALESTAXREGISTRATIONNUMBER=A.RECID AND D.INVENTLOCATIONID = @INVENTLOCATIONID  " +
                                "  SELECT @TINVAT = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'') FROM TAXREGISTRATIONNUMBERS_IN AS A, TAXINFORMATION_IN AS B, INVENTLOCATIONLOGISTICSLOCATION AS C," +
                                " INVENTLOCATION AS D,RETAILCHANNELTABLE AS E WHERE D.INVENTLOCATIONID=E.INVENTLOCATION AND C.INVENTLOCATION=D.RECID" +
                                " AND B.ISPRIMARY=1" + // ADDED ON 02/06/2014
                                " AND C.ISPRIMARY=1" + // ADDED ON 02/06/2014
                                " AND B.REGISTRATIONLOCATION=C.LOCATION AND B.[TIN]=A.RECID" +
                                " AND D.INVENTLOCATIONID = @INVENTLOCATIONID  SELECT ISNULL(@TINCST,'') AS TINCST,ISNULL(@TINVAT,'') AS TINVAT";

            SqlCommand command = new SqlCommand(sqlTaxInfo, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;

            SqlDataAdapter daTax = new SqlDataAdapter(command);

            daTax.Fill(dsSTaxInfo, "TaxInfo");
        }

        private void GetPayInfo(ref DataSet dsSPaymentInfo)
        {
            DataSet dsTemp = new DataSet();

            string sqlsubDtl = " SELECT (ISNULL(B.DESCRIPTION,'') + ' : '+ ISNULL(A.INFORMATION,'')) AS DTLPAYINFO, A.TYPE,A.INFOCODEID,A.PARENTLINENUM,A.TRANSACTIONID" +
                                " ,CAST(ISNULL(A.AMOUNT,0) AS DECIMAL(28,2)) AS AMOUNT" + //R.AMOUNTCUR ->  A.AMOUNT ADDED RHossain on 21/04/2014
                                " FROM [dbo].[RETAILTRANSACTIONINFOCODETRANS] A" +
                                " INNER JOIN	 RETAILINFOCODETABLE B ON A.INFOCODEID = B.INFOCODEID" +
                                " INNER JOIN RETAILTRANSACTIONPAYMENTTRANS R ON A.TRANSACTIONID = R.TRANSACTIONID" +
                                " AND A.TERMINAL=R.TERMINAL AND A.PARENTLINENUM = R.LINENUM" + //AND A.TERMINAL=R.TERMINAL -->ADDED RHossain on 21/04/2014
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID  AND R.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;
            SqlDataAdapter daSTotal = new SqlDataAdapter(command);
            daSTotal.Fill(dsTemp, "PaymentInfo");
            DataTable dt = new DataTable();
            DataRow dr;
            if (dsTemp != null && dsTemp.Tables.Count > 0 && dsTemp.Tables[0].Rows.Count > 0)
            {
                string sPayInfo = string.Empty;
                dt.Columns.Add("PAYINFO", typeof(string));

                for (int i = 1; i <= dsTemp.Tables[0].Rows.Count; i++)
                {
                    if (i == 1)
                    {
                        sPayInfo = Convert.ToString(dsTemp.Tables[0].Rows[i - 1]["DTLPAYINFO"]);
                    }
                    else
                    {
                        if (Convert.ToInt32(dsTemp.Tables[0].Rows[i - 1]["PARENTLINENUM"])
                            != Convert.ToInt32(dsTemp.Tables[0].Rows[i - 2]["PARENTLINENUM"]))
                        {
                            //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[i - 2]["AMOUNT"]);//commented on 21/04/2014 Req by S.Sarma
                            dr = dt.NewRow();
                            dr["PAYINFO"] = sPayInfo;
                            dt.Rows.Add(dr);
                            dt.AcceptChanges();
                            sPayInfo = Convert.ToString(dsTemp.Tables[0].Rows[i - 1]["DTLPAYINFO"]);
                        }
                        else
                        {
                            sPayInfo = sPayInfo + "; " + Convert.ToString(dsTemp.Tables[0].Rows[i - 1]["DTLPAYINFO"]);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(sPayInfo))
                {
                    //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[dsTemp.Tables[0].Rows.Count - 1]["AMOUNT"]);//commented on 21/04/2014 Req by S.Sarma
                    dr = dt.NewRow();
                    dr["PAYINFO"] = sPayInfo;
                    dt.Rows.Add(dr);
                    dt.AcceptChanges();
                    sPayInfo = string.Empty;
                }
            }
            dsSPaymentInfo.Tables.Add(dt);
        }

        private void GetStoreInfo(ref string sStorePh, ref string sInvFooter, ref string sCINNo, ref string sArabicStoreName, ref int iMGD)
        {
            string sql = " SELECT ISNULL(STORECONTACT,'-') AS STORECONTACT," +
               " ISNULL(INVOICEFOOTERNOTE,'') AS INVOICEFOOTERNOTE," +
               " ISNULL(CINNO,'') as CINNO ,ISNULL(StoreArabicName,'') as StoreArabicName," +
               " ISNULL(MGD,0) as MGD From RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'";

            DataTable dtStoreInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtStoreInfo);

            if (dtStoreInfo != null && dtStoreInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtStoreInfo.Rows[0]["STORECONTACT"]) == string.Empty)
                    sStorePh = "...";
                else
                    sStorePh = Convert.ToString(dtStoreInfo.Rows[0]["STORECONTACT"]);
                if (Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTE"]) == string.Empty)
                    sInvFooter = "-";
                else
                    sInvFooter = Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTE"]);

                if (Convert.ToString(dtStoreInfo.Rows[0]["CINNO"]) == string.Empty)
                    sCINNo = "-";
                else
                    sCINNo = Convert.ToString(dtStoreInfo.Rows[0]["CINNO"]);

                if (Convert.ToString(dtStoreInfo.Rows[0]["StoreArabicName"]) == string.Empty)
                    sArabicStoreName = "-";
                else
                    sArabicStoreName = Convert.ToString(dtStoreInfo.Rows[0]["StoreArabicName"]);

                iMGD = Convert.ToInt16(dtStoreInfo.Rows[0]["MGD"]);
            }
        }

        private void GetGSSMaturityDate(string sGSSNo, SqlConnection conn)
        {
            string sql = "select CONVERT(VARCHAR(11),CLOSUREDATE,106) as CLOSUREDATE,SCHEMECODE  from GSSACCOUNTOPENINGPOSTED" +
                        " WHERE GSSACCOUNTNO ='" + sGSSNo + "'";

            DataTable dtGSSAccInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtGSSAccInfo);

            if (dtGSSAccInfo != null && dtGSSAccInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["CLOSUREDATE"]) == string.Empty)
                    sMaturityDate = "-";
                else
                    sMaturityDate = Convert.ToString(dtGSSAccInfo.Rows[0]["CLOSUREDATE"]);
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["SCHEMECODE"]) == string.Empty)
                    sSchemeCode = "-";
                else
                    sSchemeCode = Convert.ToString(dtGSSAccInfo.Rows[0]["SCHEMECODE"]);
            }
        }

        private string GetReciptVouNo(string sTransId, SqlConnection conn)
        {
            string invNo = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select RECEIPTID  from RETAILTRANSACTIONPAYMENTTRANS where TRANSACTIONID='" + sTransId + "'";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            conn.Open();
            invNo = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return invNo;
        }

        private string GetCurrentBookedQty(string sTransId, string sTerminalId, string sStore, SqlConnection conn)
        {
            string sBookedQty = string.Empty;
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select CAST(SUM(ISNULL(GOLDQUANTITY,0)) AS NUMERIC (28,3)) AS Quantity  from RETAILDEPOSITTABLE where TRANSACTIONID='" + sTransId + "' and TERMINALID='" + sTerminalId + "' AND STOREID ='" + sStore + "' and goldfixing=1";
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;

            conn.Open();
            sBookedQty = Convert.ToString(cmd.ExecuteScalar());
            conn.Close();

            return sBookedQty;
        }


        private string getTransDateAndTime(string sTransId, RetailTransaction retailTrans)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select CONVERT(VARCHAR(11),TRANSDATE,103) TRANSDATE" +
                                " from RETAILTRANSACTIONTABLE where TRANSACTIONID='" + sTransId + "'" +
                                " and STORE='" + retailTrans.StoreId + "'" +
                                " and TERMINAL='" + retailTrans.TerminalId + "'";

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

        private Int64 getTransTime(string sTransId, RetailTransaction retailTrans)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select isnull(TRANSTIME,0) TRANSTIME" +
                                " from RETAILTRANSACTIONTABLE where TRANSACTIONID='" + sTransId + "'" +
                                " and STORE='" + retailTrans.StoreId + "'" +
                                " and TERMINAL='" + retailTrans.TerminalId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            Int64 sResult = Convert.ToInt64(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;
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

        private void GetMetalRates(ref DataTable dTMetalRates) // added on 31/03/2014 RHossain for  show the tax detail in line
        {

            string sQuery = "GETMETALRATESFORINVOICE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(sInvDt);
            command.Parameters.Add("@TransTime", SqlDbType.BigInt).Value = iTime_Ticks;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dTMetalRates);


            DataTable dt = GetConfigListForRateShow();

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

        private decimal getGSSAccumulatedAmt(string sGSSAcc, ref int iNoOfEMI, ref decimal dQty)
        {
            decimal dAmt = 0m;

            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> cAGSS;
                    cAGSS = PosApplication.Instance.TransactionServices.InvokeExtension("getGSSAccumulatedAmount", sGSSAcc);//getGSSAccumulatedAmount

                    dAmt = Convert.ToDecimal(cAGSS[2]);
                    iNoOfEMI = Convert.ToInt16(cAGSS[3]);
                    dQty = Convert.ToDecimal(cAGSS[4]);
                }
            }
            catch (Exception ex)
            {

            }

            return dAmt;
        }

        private string getSalesManName(string sTransId, string sTerminal, string sStore)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select d.NAME as Name from RETAILSTAFFTABLE r" +
                                " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID" +
                                " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
                                " where r.STAFFID IN(select top 1 ADVANCESALESMANID from RETAILTRANSACTIONTABLE where TRANSACTIONID='" + sTransId + "'" +
                                " and TERMINAL='" + sTerminal + "' and STORE='" + sStore + "')";

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

        private string GetCustomerGovtIdInfo(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string sResult = "";
            int iGovtId = 0;
            string sGovtIdNo = "";
            string sql = "select GOVTIDENTITY,isnull(GOVTIDNO,'') GOVTIDNO from CUSTTABLE " +
                         " where ACCOUNTNUM='" + sCustAcc + "'";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dtCustInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtCustInfo);

            if (dtCustInfo != null && dtCustInfo.Rows.Count > 0)
            {
                iGovtId = Convert.ToInt16(dtCustInfo.Rows[0]["GOVTIDENTITY"]);

                if (Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO"]) == string.Empty)
                    sGovtIdNo = "";
                else
                    sGovtIdNo = Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO"]);

                if (!string.IsNullOrEmpty(sGovtIdNo))
                {
                    if (iGovtId == (int)IdentityProof.Driving_License)
                        sResult = "Driving License  : " + sGovtIdNo;
                    else if (iGovtId == (int)IdentityProof.Govt_Id)
                        sResult = "Govt. Id  : " + sGovtIdNo;
                    else if (iGovtId == (int)IdentityProof.Passport_No)
                        sResult = "Passport No  : " + sGovtIdNo;
                }
            }

            return sResult;
        }
    }
}
