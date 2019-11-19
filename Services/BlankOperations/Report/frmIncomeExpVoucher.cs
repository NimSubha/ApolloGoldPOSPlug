using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.Transaction;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Reporting.WinForms;
using BarcodeLib.Barcode;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmIncomeExpVoucher : Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        string sBC = string.Empty;

        string sInvoiceNo = "";
        string sInvDt = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";

        string sAmtinwds = "";
        string sReceiptNo = "-";

        string sTerminal = string.Empty;
        string sTitle = string.Empty;
        string sType = "";
        string sVouType = "";

        // string sStorePh = "-";       
        string sTime = "";

        string sCompanyName = string.Empty;
        RetailTransaction retailTrans;

        bool IsRepairRet = false; //IsRepairRetTrans
        string sCurrencySymbol = "";
        decimal dTotBillAmt = 0;
        int iVoucherType = 0;

        public frmIncomeExpVoucher()
        {
            InitializeComponent();
        }

        public frmIncomeExpVoucher(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmIncomeExpVoucher(IPosTransaction posTransaction, SqlConnection conn, int isSafeDropBankDropStartAmt = 0)
        {
            InitializeComponent();
            connection = conn;
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            iVoucherType = isSafeDropBankDropStartAmt;

            #region[Param Info]
            retailTrans = posTransaction as RetailTransaction;
            StartingAmountTransaction startingAmountTransaction = null;
            SafeDropTransaction safeDropTrans = null;
            BankDropTransaction bankDropTrans = null;

            if (iVoucherType == 3)
                startingAmountTransaction = (StartingAmountTransaction)posTransaction;
            else if (iVoucherType == 2)
                bankDropTrans = (BankDropTransaction)posTransaction;
            else if (iVoucherType == 1)
                safeDropTrans = (SafeDropTransaction)posTransaction;


            if (retailTrans != null)
            {
                #region retailTrans
                sTerminal = retailTrans.TerminalId;

                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);

                if (retailTrans.BeginDateTime != null)
                    sInvDt = retailTrans.BeginDateTime.ToShortDateString();

                if (retailTrans.BeginDateTime != null)
                    sTime = retailTrans.BeginDateTime.ToShortTimeString(); //("HH:mm")


                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)
                    sInventLocationId = Convert.ToString(retailTrans.InventLocationId);

                if (Convert.ToString(retailTrans.ReceiptId) != string.Empty)
                    sReceiptNo = Convert.ToString(retailTrans.ReceiptId);

                sBC = sReceiptNo;

                //if (iVoucherType == 3)
                //{
                //    sTitle = "DECLARE START AMOUNT VOUCHER";
                //    sType = retailTrans.TerminalId;
                //    sVouType = "TERMINAL";
                //}
                //else
                //{
                if (retailTrans.IncomeExpenseAmounts > 0)
                {
                    sTitle = "EXPENSE ACCOUNT VOUCHER";
                    sType = "EXPENSE - EXPENSE ACCOUNTS";
                    sVouType = "Type";
                }
                else
                {
                    sTitle = "INCOME ACCOUNT VOUCHER";
                    sType = "INCOME - INCOME ACCOUNTS";
                    sVouType = "Type";
                }
                //  }


                dTotBillAmt = Math.Abs(retailTrans.IncomeExpenseAmounts);
                sAmtinwds = oBlank.Amtinwds(Math.Abs(Convert.ToDouble(dTotBillAmt)));

                //decimal dAmt = decimal.Round((Math.Abs(Convert.ToDecimal(dTotAmt))), 2, MidpointRounding.AwayFromZero);
                //sAmtinwds = oBlank.Amtinwds(Convert.ToDouble(dAmt)); // added on 28/04/2014 RHossain               
                //sAmtinwdsArabic = oBlank.AmtinwdsInArabic(Math.Abs(dTotAmt));

                //if (iInvLang == 2)
                //    sAmtinwds = sAmtinwdsArabic;
                //else if (iInvLang == 3)
                //    sAmtinwds = sAmtinwds + System.Environment.NewLine + "" + sAmtinwdsArabic;

                #endregion
            }
            else if (iVoucherType == 3)
            {
                #region iVoucherType == 3
                sTerminal = startingAmountTransaction.TerminalId;

                if (Convert.ToString(startingAmountTransaction.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(startingAmountTransaction.TransactionId);

                if (startingAmountTransaction.BeginDateTime != null)
                    sInvDt = startingAmountTransaction.BeginDateTime.ToShortDateString();

                if (startingAmountTransaction.BeginDateTime != null)
                    sTime = startingAmountTransaction.BeginDateTime.ToShortTimeString(); //("HH:mm")


                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                if (Convert.ToString(startingAmountTransaction.StoreId) != string.Empty)
                    sInventLocationId = Convert.ToString(startingAmountTransaction.StoreId);


                sReceiptNo = Convert.ToString(startingAmountTransaction.TransactionId);
                sBC = sReceiptNo;

                sTitle = "DECLARE START AMOUNT VOUCHER";
                sType = startingAmountTransaction.TerminalId;
                sVouType = "TERMINAL";

                dTotBillAmt = Math.Abs(startingAmountTransaction.Amount);
                sAmtinwds = oBlank.Amtinwds(Math.Abs(Convert.ToDouble(dTotBillAmt)));
                #endregion
            }
            else if (iVoucherType == 2)
            {
                #region bankDropTrans
                sTerminal = bankDropTrans.TerminalId;

                if (Convert.ToString(bankDropTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(bankDropTrans.TransactionId);

                if (bankDropTrans.BeginDateTime != null)
                    sInvDt = bankDropTrans.BeginDateTime.ToShortDateString();

                if (bankDropTrans.BeginDateTime != null)
                    sTime = bankDropTrans.BeginDateTime.ToShortTimeString(); //("HH:mm")


                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                if (Convert.ToString(bankDropTrans.StoreId) != string.Empty)
                    sInventLocationId = Convert.ToString(bankDropTrans.StoreId);


                sReceiptNo = Convert.ToString(bankDropTrans.TransactionId);

                sBC = sReceiptNo;


                sTitle = "BANK DROP VOUCHER";
                sType = bankDropTrans.TerminalId;
                sVouType = "TERMINAL";
                foreach (ITenderLineItem tenderLineItem in bankDropTrans.TenderLines)
                {
                    if (tenderLineItem.Voided == false)
                    {
                        dTotBillAmt += Math.Abs(tenderLineItem.Amount);
                    }
                }
                sAmtinwds = oBlank.Amtinwds(Math.Abs(Convert.ToDouble(dTotBillAmt)));
                #endregion
            }
            else if (iVoucherType == 1)
            {
                #region safeDropTrans
                sTerminal = safeDropTrans.TerminalId;

                if (Convert.ToString(safeDropTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(safeDropTrans.TransactionId);

                if (safeDropTrans.BeginDateTime != null)
                    sInvDt = safeDropTrans.BeginDateTime.ToShortDateString();

                if (safeDropTrans.BeginDateTime != null)
                    sTime = safeDropTrans.BeginDateTime.ToShortTimeString(); //("HH:mm")


                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                if (Convert.ToString(safeDropTrans.StoreId) != string.Empty)
                    sInventLocationId = Convert.ToString(safeDropTrans.StoreId);


                sReceiptNo = Convert.ToString(safeDropTrans.TransactionId);

                sBC = sReceiptNo;


                sTitle = "SAFE DROP VOUCHER";
                sType = safeDropTrans.TerminalId;
                sVouType = "TERMINAL";
                foreach (ITenderLineItem tenderLineItem in safeDropTrans.TenderLines)
                {
                    if (tenderLineItem.Voided == false)
                    {
                        dTotBillAmt += Math.Abs(tenderLineItem.Amount);
                    }
                }

                sAmtinwds = oBlank.Amtinwds(Math.Abs(Convert.ToDouble(dTotBillAmt)));
                #endregion
            }
            #endregion

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo); //, ref sInvoiceFooter

        }


        public frmIncomeExpVoucher(SqlConnection conn, string sT, DateTime dtTransDate, string sTi,
            string sStore, string sTransId, string sReceiptId, decimal dAmt, int isSafeDropBankDropStartAmt = 0)
        {
            InitializeComponent();
            connection = conn;
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            iVoucherType = isSafeDropBankDropStartAmt;

            #region[Param Info]

            sTerminal = sT;
            sInvoiceNo = sTransId;
            sInvDt = dtTransDate.ToShortDateString();
            sTime = sTi; //("HH:mm")
            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);
            sInventLocationId = sStore;
            sReceiptNo = sTransId;
            sBC = sReceiptNo;


            if (iVoucherType == 1)
            {
                sTitle = "SAFE DROP VOUCHER";
                sType = sTerminal;
                sVouType = "TERMINAL";
            }
            else if (iVoucherType == 2)
            {
                sTitle = "BANK DROP VOUCHER";
                sType = sTerminal;
                sVouType = "TERMINAL";
            }
            else if (iVoucherType == 3)
            {
                sTitle = "DECLARE START AMOUNT VOUCHER";
                sType = sTerminal;
                sVouType = "TERMINAL";
            }
            else if (iVoucherType == 4)
            {
                sTitle = "TENDER DECLARATION VOUCHER";
                sType = sTerminal;
                sVouType = "TERMINAL";
            }
            dTotBillAmt = dAmt;

            sAmtinwds = oBlank.Amtinwds(Math.Abs(Convert.ToDouble(dTotBillAmt)));


            #endregion

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo); //, ref sInvoiceFooter

        }

        private void GetStoreInfo(ref string sStorePh) //, ref string sInvFooter
        {
            string sql = " SELECT ISNULL(STORECONTACT,'-') AS STORECONTACT, ISNULL(INVOICEFOOTERNOTE,'') AS INVOICEFOOTERNOTE FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'";

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
                //if(Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTE"]) == string.Empty)
                //    sInvFooter = "-";
                //else
                //    sInvFooter = Convert.ToString(dtStoreInfo.Rows[0]["INVOICEFOOTERNOTE"]);
            }
        }

        public string Amtinwds(double amt)
        {
            object[] words = new object[28];
            string Awds = null;
            string x = null;
            string y = null;
            string a = null;
            string t = null;
            string cror = null;
            string lakh = null;
            string lak2 = null;
            string thou = null;
            string tho2 = null;
            string hund = null;
            string rupe = null;
            string rup2 = null;
            string pais = null;
            string pai2 = null;

            words[1] = "One ";
            words[2] = "Two ";
            words[3] = "Three ";
            words[4] = "Four ";
            words[5] = "Five ";
            words[6] = "Six ";
            words[7] = "Seven ";
            words[8] = "Eight ";
            words[9] = "Nine ";
            words[10] = "Ten ";
            words[11] = "Eleven ";
            words[12] = "Twelve ";
            words[13] = "Thirteen ";
            words[14] = "Fourteen ";
            words[15] = "Fifteen ";
            words[16] = "Sixteen ";
            words[17] = "Seventeen ";
            words[18] = "Eighteen ";
            words[19] = "Ninteen ";
            words[20] = "Twenty ";
            words[21] = "Thirty ";
            words[22] = "Forty ";
            words[23] = "Fifty ";
            words[24] = "Sixty ";
            words[25] = "Seventy ";
            words[26] = "Eighty ";
            words[27] = "Ninety ";

            if (amt >= 1)
            {
                Awds = "Rupees ";
            }
            else
            {
                Awds = "Rupee ";
            }
            x = (amt.ToString("0.00")).PadLeft(12, Convert.ToChar("0"));
            cror = x.Substring(1, 1);
            lakh = x.Substring(2, 2);
            lak2 = x.Substring(3, 1);
            thou = x.Substring(4, 2);
            tho2 = x.Substring(5, 1);
            hund = x.Substring(6, 1);
            rupe = x.Substring(7, 2);
            rup2 = x.Substring(8, 1);
            pais = x.Substring(10, 2);
            pai2 = x.Substring(11, 1);
            y = "";
            if (Convert.ToInt32(cror) > 0)
            {
                y = words[Convert.ToInt32(cror)].ToString() + "crores ";
            }
            t = Convert.ToString(lakh);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = lakh.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(lak2) != 0)
                        y = y + words[Convert.ToInt32(lak2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
                y = y + "lakhs ";
            }
            t = Convert.ToString(thou);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = thou.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(tho2) != 0)
                        y = y + words[Convert.ToInt32(tho2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
                y = y + "thousand ";
            }
            if (Convert.ToInt32(hund) > 0)
            {
                y = y + words[Convert.ToInt32(hund)] + "hundred ";
            }
            t = Convert.ToString(rupe);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = rupe.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(rup2) != 0)
                        y = y + words[Convert.ToInt32(rup2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
            }
            t = Convert.ToString(pais);
            if (Convert.ToInt32(t) > 0)
            {
                y = y + "paise ";
                if (Convert.ToInt32(t) > 20)
                {
                    a = pais.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(pai2) != 0)
                        y = y + words[Convert.ToInt32(pai2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
            }
            string amtwrd = "";
            if (y.Length > 0)
            {
                amtwrd = Awds + y + "only ";
            }
            return amtwrd;
        }

        private void frmIncomeExpVoucher_Load(object sender, EventArgs e)
        {
            #region BarcodeDatatable
            BarcodeLib.Barcode.Linear codabar = new BarcodeLib.Barcode.Linear();
            codabar.Type = BarcodeType.CODE39;//CODABAR;
            codabar.Data = sBC;
            codabar.UOM = UnitOfMeasure.PIXEL;
            codabar.BarColor = System.Drawing.Color.Black;
            codabar.BarWidth = 2;
            //codabar.LeftMargin = 8;
            //codabar.RightMargin = 8;
            codabar.CodabarStartChar = CodabarStartStopChar.C;
            codabar.ImageFormat = System.Drawing.Imaging.ImageFormat.Gif;

            MemoryStream ms = new MemoryStream();
            codabar.drawBarcode(ms);
            Byte[] bitmap01 = null;
            bitmap01 = ms.GetBuffer();

            DataTable dtBarcode = new DataTable();
            dtBarcode.Columns.Add("ID", typeof(int));
            dtBarcode.Columns.Add("BARCODEIMG", typeof(byte[]));
            DataRow dr = dtBarcode.NewRow();
            dr["ID"] = 1;
            dr["BARCODEIMG"] = bitmap01;
            dtBarcode.Rows.Add(dr);
            dr = null;
            // 
            #endregion

            DataTable dtStoreLogo = new DataTable();

            dtStoreLogo = GetStoreLogo();

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (iVoucherType == 4)
                localReport.ReportPath = "rptTenderDeclaration.rdlc";
            else
                localReport.ReportPath = "rptIncExpVoucher.rdlc";

            sCurrencySymbol = oBlank.GetCurrencySymbol();
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DataTable dt = new DataTable();
            DataTable dtTD = new DataTable();
            DataTable dtTDeclare = new DataTable();
            DataTable dtFD = new DataTable();

            GetData(ref dt);

            sCompanyName = oBlank.GetCompanyName(connection);
            if (iVoucherType == 4)
            {
                GetTD(ref dtTD);
                GetTDeclare(ref dtTDeclare);
                GetFD(ref dtFD);

                ReportDataSource rdsTD = new ReportDataSource("dsTD", dtTD);
                ReportDataSource rdsTDeclare = new ReportDataSource("dsTDeclare", dtTDeclare);
                ReportDataSource rdsFD = new ReportDataSource("dsFD", dtFD);
                
                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(rdsTD);
                this.reportViewer1.LocalReport.DataSources.Add(rdsTDeclare);
                this.reportViewer1.LocalReport.DataSources.Add(rdsFD);

                this.reportViewer1.LocalReport.EnableExternalImages = true;
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("BARCODEIMGTABLE", dtBarcode));//dsBarcode.Tables[0]));

                this.reportViewer1.LocalReport.EnableExternalImages = true;
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("STORELOGO", dtStoreLogo));
            }
            else
            {
                ReportDataSource dsSalesOrder = new ReportDataSource();
                dsSalesOrder.Name = "GETINCOMEEXPENSEVOU";
                dsSalesOrder.Value = dt;
                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("GETINCOMEEXPENSEVOU", dt));

                this.reportViewer1.LocalReport.EnableExternalImages = true;
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("BARCODEIMGTABLE", dtBarcode));//dsBarcode.Tables[0]));

                this.reportViewer1.LocalReport.EnableExternalImages = true;
                this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("STORELOGO", dtStoreLogo));
            }

            ReportParameter[] param = new ReportParameter[13];

            param[0] = new ReportParameter("InvDate", sInvDt);
            //param[7] = new ReportParameter("StoreName", sStoreName);
            param[1] = new ReportParameter("StoreAddress", sStoreAddress);
            param[2] = new ReportParameter("StorePhone", sStorePhNo);

            if (!string.IsNullOrEmpty(sAmtinwds))
                param[3] = new ReportParameter("Amtinwds", sAmtinwds);
            else
                param[3] = new ReportParameter("Amtinwds", "zero");

            param[4] = new ReportParameter("ReceiptNo", sReceiptNo);
            param[5] = new ReportParameter("Title", sTitle);
            param[6] = new ReportParameter("CompName", string.IsNullOrEmpty(sCompanyName) ? "" : sCompanyName, true); //string.IsNullOrEmpty(sRateCType) ? " " : sRateCType, true
            param[7] = new ReportParameter("cs", sCurrencySymbol);
            param[8] = new ReportParameter("Type", sType); //dTotBillAmt added on 26/12/2014
            if (iVoucherType != 0)
                param[9] = new ReportParameter("WH", ApplicationSettings.Terminal.StoreId + " - " + ApplicationSettings.Terminal.StoreName);
            else
                param[9] = new ReportParameter("WH", retailTrans.StoreId + " - " + retailTrans.StoreName);
            param[10] = new ReportParameter("VouTime", sTime);
            if (iVoucherType != 0)
                param[11] = new ReportParameter("SalesPerson", ApplicationSettings.Terminal.TerminalOperator.OperatorId + " - " + ApplicationSettings.Terminal.TerminalOperator.Name);
            else
                param[11] = new ReportParameter("SalesPerson", retailTrans.OperatorId + " - " + retailTrans.OperatorName);
            param[12] = new ReportParameter("VouType", sVouType);
            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();

            //oBlank.Export(reportViewer1.LocalReport);
            //oBlank.Print_Invoice(1);
            //this.reportViewer1.RefreshReport();
        }

        private DataTable GetStoreLogo()  // SKU allow
        {
            DataTable dt = new DataTable();
            string commandText = "SELECT 1 AS ID, STOREIMAGE AS LOGOIMG from CRWRETAILSTOREIMG where storenumber='" + ApplicationSettings.Terminal.StoreId + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (SqlCommand command = new SqlCommand(commandText, connection))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dt;
        }
        private void GetData(ref DataTable dtCol)
        {
            string sQuery = "";
            if (iVoucherType == 1)
                sQuery = "GETSAFEDROPVOU";
            else if (iVoucherType == 2)
                sQuery = "GETBANKDROPVOU";
            else if (iVoucherType == 3)
                sQuery = "GETDECLARESTARTAMTVOU";
            else
                sQuery = "GETINCOMEEXPENSEVOU";//GETSALESORPURCHASEINVOICE,GETDECLARESTARTAMTVOU


            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();

            command.Parameters.Add("@TRANSID", SqlDbType.NVarChar).Value = sInvoiceNo;

            if (!string.IsNullOrEmpty(sTerminal) && retailTrans != null)
            {
                if (iVoucherType == 0)
                    command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = retailTrans.TerminalId;// ApplicationSettings.Terminal.TerminalId;
                else
                    command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.TerminalId;
            }
            else
            {
                command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = sTerminal;
            }

            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dtCol);
        }

        private void GetTD(ref DataTable dtTD)
        {
            string sQuery = "GETTENDERDENOMINATION";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = sInvDt;
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = sReg;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtTD);
        }

        private void GetFD(ref DataTable dtFD)
        {
            string sQuery = "GETFDDETAILS";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = sInvDt;
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = sReg;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtFD);
        }

        private void GetTDeclare(ref DataTable dtTDECLARE)
        {
            string sQuery = "GETTENDERDECLARE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = sInvDt;
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = sReg;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtTDECLARE);
        }
    }
}
