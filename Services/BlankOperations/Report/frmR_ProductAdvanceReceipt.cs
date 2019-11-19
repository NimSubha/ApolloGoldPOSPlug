
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
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Reporting.WinForms;
using BarcodeLib.Barcode;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmR_ProductAdvanceReceipt : Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        // string sTransactionId = "";
        string sCustName = "-";
        string sCustId = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";
        string sEmail = "";
        Int64 iTime_Ticks = 0;

        string sReceiptNo = "";
        string sInvNo = "";

        string sReceiptVoucherNo = "";
        string sAmount = "";
        string sReceiptDate = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";
        string sDetailsLine = "";
        string sBookedSkuList = "";
        string sFixedRate = "";
        string sInvoiceFooter = "-";

        string sTerminal = string.Empty;
        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sRTitle = string.Empty;
        string sFooterText = string.Empty;
        string sCurrencySymbol = "";
        string sRefRceiptNo = string.Empty;
        string sRateFreezedText = string.Empty;
        string sOrderNo = "";
        string sMetalRates = "";
        string sOperatorName = "";
        DataTable dtGroupTotal = new DataTable();
        string sStoreArabicName = string.Empty;
        string sPaidBy = string.Empty;
        string sBC = string.Empty;
        string sTime = "";
        string sInvDt = "";
        RetailTransaction retailTrans = null;
        int isMGDStore = 0;
        string sCompanyTaxRegNo = "";
        string sNetValueInWord = "";
        bool bTaxApplicable = false;
        DateTime taxCutOffDate = new DateTime();
        decimal dTaxPct = 0m;
        decimal dTaxAmt = 0m;
        decimal dExcludTaxAmt = 0m;
        decimal dBookedQty = 0m;
        bool bIsConvToAd = false;
        bool bWithTaxPrint = false;
        int iLang = 0;
        string sFooterMOPName = "";
        string sFooterMOPValue = "";
        decimal dBookingRate = 0m;
        string sFixConfigId = string.Empty;

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
        decimal dAmt = 0;
        decimal pCurrentRate = 0m;

        public frmR_ProductAdvanceReceipt(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmR_ProductAdvanceReceipt(IPosTransaction posTransaction, SqlConnection conn, string _sTransId, string _sAmt, string sTerminalId, string sGiftCardItemName = "", string sGiftCardNo = "", int iIsAdvRefund = 0, bool iIsConvToAd = false, int iLanguage = 0, int iFromOfflineGMA = 0)
        {
            InitializeComponent();
            retailTrans = posTransaction as RetailTransaction;
            bIsConvToAd = iIsConvToAd;
            sTerminal = sTerminalId;
            iLang = iLanguage;

            #region Language wise text

            sText1 = "Received with thanks from";//
            sText2 = "Net Value including tax";//
            sText3 = "Net value including tax in words";//
            sText4 = "Against Order no is";//
            sText5 = "Gold fixing done";//
            sText6 = "Paid by";//
            sText7 = "Tax Invoice";//
            sText8 = "ADVANCE RECEIPT"; //
            sText9 = "Tax Registration No";//
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


            #endregion

            #region[Param Info]
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

                //if (Convert.ToString(retailTrans.OperatorName) != string.Empty)
                //    sOperatorName = Convert.ToString(retailTrans.OperatorName);
                //else
                sOperatorName = getSalesManName(retailTrans.TransactionId, retailTrans.TerminalId, retailTrans.StoreId);
                bTaxApplicable = IsTaxApplicable();
                bWithTaxPrint = IsWithTaxPrint();

                taxCutOffDate = getTaxCutOffDate();

                if (string.IsNullOrEmpty(sOperatorName))
                    sOperatorName = getSalesManName(retailTrans.TransactionId, retailTrans.TerminalId);


                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.BeginDateTime.ToShortDateString();

                if (retailTrans.EndDateTime != null)
                    sTime = retailTrans.BeginDateTime.ToShortTimeString(); //("HH:mm")

                TimeSpan t = TimeSpan.FromSeconds(iTime_Ticks);

                string answer = string.Format("{0:D2}h:{1:D2}m",//:{2:D2}s:{3:D3}ms
                                t.Hours,
                                t.Minutes); //

                sInvDt = getTransDateAndTime(_sTransId, retailTrans);


                iTime_Ticks = getTransTime(_sTransId, retailTrans);// Convert.ToInt64(retailTrans.BeginDateTime.TimeOfDay.TotalSeconds);



                //t.Seconds,        t.Milliseconds
                //-------
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                {
                    sReceiptNo = _sTransId;
                    sInvNo = retailTrans.ReceiptId;// _sTransId;// Convert.ToString(retailTrans.TransactionId);
                    sReceiptVoucherNo = retailTrans.ReceiptId;
                }

                sBC = sInvNo;

                //if(retailTrans.BeginDateTime != null)
                //    sReceiptDate = retailTrans.BeginDateTime.ToShortDateString();
                getFreezedMetalRate(retailTrans);

                //if (iLang == 1)
                sAmount = oBlank.Amtinwds(Convert.ToDouble(_sAmt));
                //else if (iLang == 2)
                //    sAmount = oBlank.AmtinwdsInArabic(Convert.ToDouble(_sAmt));
                //else if (iLang == 3)
                //    sAmount = oBlank.Amtinwds(Convert.ToDouble(_sAmt)) + " " + oBlank.AmtinwdsInArabic(Convert.ToDouble(_sAmt));

                sCurrencySymbol = oBlank.GetCurrencySymbol();

                if (string.IsNullOrEmpty(sCurrencySymbol))
                    sCurrencySymbol = ApplicationSettings.Terminal.CompanyCurrency;
                dTaxPct = getItemTaxPercentage();
                dAmt = decimal.Round(Convert.ToDecimal(_sAmt), 2, MidpointRounding.AwayFromZero);//Convert.ToDecimal(_sAmt);
                dTaxAmt = decimal.Round(Convert.ToDecimal(dAmt * dTaxPct / (100 + dTaxPct)), 2, MidpointRounding.AwayFromZero);
                dExcludTaxAmt = dAmt - dTaxAmt;



                if (bTaxApplicable)
                {
                    if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                    {
                        if (string.IsNullOrEmpty(sCurrencySymbol))
                            sCurrencySymbol = ApplicationSettings.Terminal.CompanyCurrency;


                        sNetValueInWord = "Net value in words";


                        if (string.IsNullOrEmpty(sGiftCardItemName))
                        {
                            sFooterText = "Request to handover Advance Receipt at the time of billing";
                            sRTitle = sText8;
                            sDetailsLine = sCurrencySymbol + " " + _sAmt + "  " + sText1 + "" + sCustName + " ";
                        }
                        else if (iIsAdvRefund == 1)
                        {
                            sFooterText = "";
                            sRTitle = "ADVANCE REFUND";
                            sDetailsLine = "Advance Refund" + "                                                                                                                " + sCurrencySymbol + " " + _sAmt;
                        }
                        else
                        {
                            sFooterText = "";
                            sRTitle = sText12; //  "GIFT CARD RECEIPT";
                            sDetailsLine = sGiftCardItemName + "            " + sGiftCardNo + "                                                                         " + sCurrencySymbol + " " + _sAmt;
                        }
                    }
                    else
                    {
                        if (iIsConvToAd)//added on 21/12/2017
                        {
                            sFooterText = "Request to handover Advance Receipt at the time of billing";
                            sRTitle = "Credit Note";
                            sNetValueInWord = "Net value in words";
                            sDetailsLine = sCurrencySymbol + "  " + _sAmt + "  " +
                                sText1 + "  " + sCustName;
                        }
                        else if (string.IsNullOrEmpty(sGiftCardItemName))
                        {
                            sFooterText = "Request to handover Advance Receipt at the time of billing";
                            sRTitle = sText7 + "( " + sText8 + " )";// "Tax Invoice ( Advance Receipt )";
                            sNetValueInWord = sText3;// "Net value including tax in words";
                            sDetailsLine = sCurrencySymbol + "  " + dExcludTaxAmt + "  " + " With " + "" + dTaxPct + " %" + " Tax : " +
                                " " + dTaxAmt + "  " + sText1 + "  " + sCustName + " ;  " + sText2 + " " + sCurrencySymbol + " " + _sAmt;
                        }
                        else if (iIsAdvRefund == 1)
                        {
                            sFooterText = "";
                            sNetValueInWord = sText3;// "Net value including tax in words";
                            sRTitle = "ADVANCE REFUND";
                            sDetailsLine = "Advance Refund" + "                                                                                                                " + sCurrencySymbol + " " + _sAmt;
                        }

                        else
                        {
                            sFooterText = "";
                            sNetValueInWord = sText3;// "Net value including tax in words";
                            sRTitle = sText12; // "GIFT CARD RECEIPT";
                            sDetailsLine = sGiftCardItemName + "            " + sGiftCardNo + "                                                                         " + sCurrencySymbol + " " + _sAmt;
                        }
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(sCurrencySymbol))
                        sCurrencySymbol = ApplicationSettings.Terminal.CompanyCurrency;


                    sNetValueInWord = "Net value in words";


                    if (string.IsNullOrEmpty(sGiftCardItemName))
                    {
                        sFooterText = "Request to handover Advance Receipt at the time of billing";
                        sRTitle = sText8;// "ADVANCE RECEIPT";
                        sDetailsLine = sCurrencySymbol + " " + _sAmt + "  " + sText1 + " " + sCustName + " ";
                    }
                    else if (iIsAdvRefund == 1)
                    {
                        sFooterText = "";
                        sRTitle = "ADVANCE REFUND";
                        sDetailsLine = "Advance Refund" + "                                                                                                                " + sCurrencySymbol + " " + _sAmt;
                    }
                    else
                    {
                        sFooterText = "";
                        sRTitle = sText12;// "GIFT CARD RECEIPT"; //كرت هديةإيصال
                        sDetailsLine = sGiftCardItemName + "            " + sGiftCardNo + "                                                                         " + sCurrencySymbol + " " + _sAmt;
                    }
                }
                //----------store Info

                //if(Convert.ToString(retailTrans.StoreName) != string.Empty)
                //    sStoreName = Convert.ToString(retailTrans.StoreName);
                //if(Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                //    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);

                //if (! string.IsNullOrEmpty(Convert.ToString(retailTrans.StorePhone)))
                //    sStorePhNo = Convert.ToString(retailTrans.StorePhone);

                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                // if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)

                sInventLocationId = ApplicationSettings.Terminal.InventLocationId; //Convert.ToString(retailTrans.InventLocationId);

            }
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo, ref sStoreArabicName, ref isMGDStore);

            //sCINNo = oBlank.getValue("select CINNO  from RETAILSTORETABLE where STORENUMBER ='" + Convert.ToString(ApplicationSettings.Terminal.StoreId) + "'");
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            sCompanyTaxRegNo = oBlank.GetCompanyTaxRegNum(conn);

            if (sRTitle == sText7 + "( " + sText8 + " )" || iIsAdvRefund == 1)
            {
                if (bTaxApplicable)
                {
                    if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                        sCompanyTaxRegNo = "";
                    else
                        sCompanyTaxRegNo = sText9 + " " + sCompanyTaxRegNo; //"Tax Registration No " 
                }
                else
                    sCompanyTaxRegNo = "";
            }
            else
                sCompanyTaxRegNo = "";

            if (iIsAdvRefund == 1)
            {
                sRefRceiptNo = GetRefReciptId(retailTrans.TransactionId);
            }

            #endregion
        }


        private void frmR_ProductAdvanceReceipt_Load(object sender, EventArgs e)
        {

            #region BarcodeDatatable
            BarcodeLib.Barcode.Linear codabar = new BarcodeLib.Barcode.Linear();
            codabar.Type = BarcodeType.CODE39;//CODABAR;
            codabar.Data = sBC;

            codabar.UOM = UnitOfMeasure.PIXEL;
            codabar.BarColor = System.Drawing.Color.Black;
            codabar.BarWidth = 2;
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

            //sInvDt

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (string.IsNullOrEmpty(sRefRceiptNo))
                localReport.ReportPath = "rptProductAdvanceReceipt.rdlc";
            else
                localReport.ReportPath = "rptAdvanceRefundReceipt.rdlc";

            DataSet dsTender = new DataSet();
            DataSet dsTaxInfo = new DataSet();
            DataSet dsBookedSku = new DataSet();
            DataSet dsPaymentInfo = new DataSet();
            DataTable dtMatalRate = new DataTable();
            DataTable dtTaxAdv = new DataTable();


            GetMetalRates(ref dtMatalRate);

            GetTender(ref dsTender);
            GetTaxInfo(ref dsTaxInfo);
            sOrderNo = GetOrderNo(sReceiptNo);

            GetBookedSku(ref dsBookedSku);
            // getFixedMetalRate();
            GetPayInfo(ref dsPaymentInfo);

            GetTaxAdvanceData(ref dtTaxAdv);

            pCurrentRate = Convert.ToDecimal(GetMetalRate());

            ReportDataSource dsGSSInstalmentReceipt = new ReportDataSource();

            dsGSSInstalmentReceipt.Name = "Tender";
            dsGSSInstalmentReceipt.Value = dsTender.Tables[0];

            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dsTender.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "TaxInfo";
            RDTaxInfo.Value = dsTaxInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInfo", dsTaxInfo.Tables[0]));

            //ReportDataSource RDBookedSku = new ReportDataSource();
            //RDBookedSku.Name = "DataSet3";
            //RDBookedSku.Value = dsBookedSku.Tables[0];
            //this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet3", dsBookedSku.Tables[0]));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));

            ReportDataSource dsBCI = new ReportDataSource();
            dsBCI.Name = "BCI";
            dsBCI.Value = dtBarcode;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("BCI", dtBarcode));

            ReportDataSource dsTaxAdv = new ReportDataSource();
            dsTaxAdv.Name = "TAXADV";
            dsTaxAdv.Value = dtTaxAdv;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TAXADV", dtTaxAdv));

            ReportParameter[] param;

            //if (string.IsNullOrEmpty(sRefRceiptNo))
            //    param = new ReportParameter[27];
            //else
            param = new ReportParameter[34];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);

            param[4] = new ReportParameter("ReceiptNo", sInvNo);
            param[5] = new ReportParameter("RecDate", sInvDt);//+ "   " + sTime

            param[6] = new ReportParameter("StoreAddress", sStoreAddress);
            param[7] = new ReportParameter("StorePhone", sStorePhNo);

            if (!string.IsNullOrEmpty(sAmount))
                param[8] = new ReportParameter("Amtinwds", sAmount);
            else
                param[8] = new ReportParameter("Amtinwds", "zero");

            param[9] = new ReportParameter("DetailsLine", sDetailsLine);

            if (!string.IsNullOrEmpty(sBookedSkuList))
                param[10] = new ReportParameter("BookedSkuList", sBookedSkuList);
            else
                param[10] = new ReportParameter("BookedSkuList", " ");


            // sRateFreezedText = GetFreezedRateList();
            //getFreezedMetalRate(retailTrans);

            if (!string.IsNullOrEmpty(sFixedRate))
                param[11] = new ReportParameter("FixedRate", sFixedRate);
            else
                param[11] = new ReportParameter("FixedRate", " ");

            param[12] = new ReportParameter("CId", sCustId);
            param[13] = new ReportParameter("VoucherNo", sReceiptVoucherNo); // added RHossain on 01/04/14
            param[14] = new ReportParameter("CompName", sCompanyName); // added on 14/04/2014 req from Sailendra Da
            param[15] = new ReportParameter("CIN", sCINNo); // added on 14/04/214 req from Sailendra Da
            param[16] = new ReportParameter("RTitle", sRTitle); // added on 02/07/2014 req from Sailendra Da
            param[17] = new ReportParameter("FooterText", sFooterText);// added on 02/07/2014 req from Sailendra Da


            param[18] = new ReportParameter("MRATES", sMetalRates);
            param[19] = new ReportParameter("StrArabicName", sStoreArabicName);
            param[20] = new ReportParameter("StoreName", sStoreName);

            param[21] = new ReportParameter("OperatorName", sOperatorName);
            param[22] = new ReportParameter("PaidBy", sPaidBy);
            param[23] = new ReportParameter("Email", sEmail);
            param[24] = new ReportParameter("TransId", sReceiptNo);
            param[25] = new ReportParameter("NetValueInWord", sNetValueInWord);


            //if (!string.IsNullOrEmpty(sRefRceiptNo))
            param[26] = new ReportParameter("RefReceiptNo", sRefRceiptNo);// added on 02/07/2014 req from Sailendra Da
            param[27] = new ReportParameter("CompanyTaxRegNo", sCompanyTaxRegNo);
            param[28] = new ReportParameter("OrderNo", sOrderNo);
            param[29] = new ReportParameter("AdvAmt", Convert.ToString(dAmt));
            param[30] = new ReportParameter("CCode", sCustId);
            param[31] = new ReportParameter("FooterMOPValues", sFooterMOPValue);
            param[32] = new ReportParameter("FooterMOPDetails", sFooterMOPName);
            //param[33] = new ReportParameter("MetalRate", Convert.ToString(pCurrentRate) + "/22kt");
            param[33] = new ReportParameter("MetalRate", Convert.ToString(dBookingRate)+"/"+ sFixConfigId );

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

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
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");//[INVENTPARAMETERS] 

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 and Retail=1 "); // RATETYPE -- > GSS->Sale 4->3 on 10/06/2016 req by S.Sharma
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return Convert.ToString(sResult.Trim());
        }

        private bool IsTaxApplicable()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select TAXAPPLICABLE from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        private bool IsWithTaxPrint()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select WITHTAXPRINT from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        private DateTime getTaxCutOffDate()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) TAX_CUTOFFDATE FROM [RETAILPARAMETERS] where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToDateTime(cmd.ExecuteScalar());
        }

        private void GetTender(ref DataSet dsSTender)
        {
            //string sqlsubDtl = " SELECT DISTINCT N.NAME, M.TENDERTYPE,M.CARDORACCOUNT," +
            //                    " CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT" +
            //                    " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
            //                    " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
            //                    " WHERE M.TRANSACTIONID = @TRANSACTIONID" +
            //                    " ORDER BY M.TENDERTYPE ";

            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME,N.SRTOADVANCE, M.TENDERTYPE,CAST(ISNULL(ABS(M.AMOUNTCUR),0) AS DECIMAL(28,2)) AS AMOUNT," +
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
                DataTable dtAdv = new DataTable();
                DataRow[] dr = dsSTender.Tables[0].Select("SRTOADVANCE=1");

                int isConvToAdv = 0;
                if (dr.Length > 0)
                    isConvToAdv = 1;

                dtAdv = dsSTender.Tables[0].Clone();
                if (isConvToAdv == 0)
                {
                    dtAdv = dsSTender.Tables[0];
                }
                else
                {
                    foreach (DataRow row in dr)
                    {
                        dtAdv.ImportRow(row);
                    }
                }

                foreach (DataRow row in dtAdv.Rows)
                {
                    string group = row["Name"].ToString();
                    decimal amt = Math.Abs(Convert.ToDecimal(row["AMOUNT"]));

                    if (!string.IsNullOrEmpty(sRefRceiptNo))
                        amt = amt * -1;

                    if (string.IsNullOrEmpty(sPaidBy))
                        sPaidBy = sText6 + " - " + group + " : " + amt;//Paid by
                    else
                        sPaidBy += "   " + group + " : " + amt;
                }

                for (int i = 0; i <= dtAdv.Rows.Count - 1; i++)
                {
                    if (string.IsNullOrEmpty(sFooterMOPName))
                        sFooterMOPName = Convert.ToString(dtAdv.Rows[i]["NAME"]);
                    else
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dtAdv.Rows[i]["NAME"])))
                            sFooterMOPName += " , " + Convert.ToString(dtAdv.Rows[i]["NAME"]);
                    }

                    if (string.IsNullOrEmpty(sFooterMOPValue))
                    {
                        if (!string.IsNullOrEmpty(sRefRceiptNo))
                            sFooterMOPValue = Convert.ToString(Math.Abs(Convert.ToDecimal(dtAdv.Rows[i]["AMOUNT"])) * -1);
                        else
                            sFooterMOPValue = Convert.ToString(Math.Abs(Convert.ToDecimal(dtAdv.Rows[i]["AMOUNT"])));
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dtAdv.Rows[i]["AMOUNT"])))
                        {
                            if (!string.IsNullOrEmpty(sRefRceiptNo))
                                sFooterMOPValue += " , " + Convert.ToString(Math.Abs(Convert.ToDecimal(dtAdv.Rows[i]["AMOUNT"])) * -1);
                            else
                                sFooterMOPValue += " , " + Convert.ToString(Math.Abs(Convert.ToDecimal(dtAdv.Rows[i]["AMOUNT"])));
                        }
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

        private void GetBookedSku(ref DataSet dsBookedSku)
        {
            // string sOrderNo = "";
            string sqlBookedSku = " DECLARE @ORDERNUM VARCHAR(20)" +
                                 " SELECT @ORDERNUM = ORDERNUM from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID = @TRANSACTIONID AND TERMINALID = '" + sTerminal + "' " +
                //" select SKUNUMBER from retailcustomerdepositskudetails WHERE ORDERNUMBER = @ORDERNUM and ORDERNUMBER!=''";
                                  " select SKUNUMBER,ORDERNUMBER from retailcustomerdepositskudetails WHERE TRANSID = @TRANSACTIONID AND TERMINALID = '" + sTerminal + "' "; // changes on req by S.Shrma on 23/02/2015

            SqlCommand command = new SqlCommand(sqlBookedSku, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;

            SqlDataAdapter daBookedSku = new SqlDataAdapter(command);

            daBookedSku.Fill(dsBookedSku, "BookedSkuInfo");

            if (dsBookedSku.Tables[0] != null && dsBookedSku.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow drNew in dsBookedSku.Tables[0].Rows)
                {
                    if (string.IsNullOrEmpty(sBookedSkuList))
                        sBookedSkuList = sText13 + " " + Convert.ToString(drNew["SKUNUMBER"]);
                    else
                        sBookedSkuList = sBookedSkuList + ", " + Convert.ToString(drNew["SKUNUMBER"]);
                }
            }

            if (!string.IsNullOrEmpty(sBookedSkuList) && !string.IsNullOrEmpty(sOrderNo))
                sBookedSkuList = sBookedSkuList + " " + sText15 + "  : " + sOrderNo;
            if (string.IsNullOrEmpty(sBookedSkuList) && !string.IsNullOrEmpty(sOrderNo))
                sBookedSkuList = sText4 + " : " + sOrderNo; //Against Order no is

        }

        private void getFixedMetalRate()
        {

            decimal dBookedQty = 0m;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" declare @ORDERNUM VARCHAR(20) ");
            commandText.Append(" declare @IsFixedQty int ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @ORDERNUM = ORDERNUM from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID =@TRANSACTIONID");
            commandText.Append(" select @IsFixedQty=ISFIXEDQTY from CUSTORDER_HEADER WHERE ORDERNUM = @ORDERNUM and ORDERNUM!='' ");
            commandText.Append(" IF(@IsFixedQty=1)");
            commandText.Append(" BEGIN");
            commandText.Append(" select FIXEDMETALRATE from CUSTORDER_HEADER WHERE ORDERNUM = @ORDERNUM and ORDERNUM!=''");
            commandText.Append(" END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    //dBookedQty = Convert.ToDecimal(reader.GetValue(0));
                    sFixedRate = sText5 + " @ " + Convert.ToString(reader.GetValue(0)) + "/gm"; //blocked on 25/04/2014 req by S.Sharma "Gold fixing done" 
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void getFreezedMetalRate(RetailTransaction retailTrans)
        {
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            string sStoreNo = retailTrans.StoreId;// ApplicationSettings.Terminal.StoreId;
            string sTerminal = retailTrans.TerminalId;// ApplicationSettings.Terminal.TerminalId;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" SELECT GOLDFIXING,(CASE WHEN CAST(ISNULL(ManualBookedQty,0) AS DECIMAL(28,3)) = 0 THEN CAST(ISNULL(GOLDQUANTITY,0) AS DECIMAL(28,3)) ELSE CAST(ISNULL(ManualBookedQty,0) AS DECIMAL(28,3)) END) AS GOLDQUANTITY ,");
            commandText.Append(" CAST(ISNULL(RATE,0) AS DECIMAL(28,2)) AS RATE,FIXEDCONFIGID from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID ='" + sReceiptNo + "'");
            commandText.Append("AND STOREID ='" + sStoreNo + "'");
            commandText.Append("AND TERMINALID ='" + sTerminal + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.Parameters.Clear();
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dBookedQty = Convert.ToDecimal(reader.GetValue(1));
                    dBookingRate = Convert.ToDecimal(reader.GetValue(2));
                    sFixConfigId = Convert.ToString(reader.GetValue(3));

                    if (Convert.ToInt16(reader.GetValue(0)) > 0)
                    {
                        sFixedRate = sText5 + " @ " + Convert.ToString(reader.GetValue(2))//"Gold Fixed done "
                            + "/gm " + " " + sText18 + " = " + Convert.ToString(reader.GetValue(1)) + "";
                    }
                    else
                    {
                        sFixedRate = sText17 + " @ " + Convert.ToString(reader.GetValue(2))
                            + "/gm " + " " + sText18 + " = " + Convert.ToString(reader.GetValue(1)) + "";//"Rate Freezed done "
                    }
                }
            }


            if (conn.State == ConnectionState.Open)
                conn.Close();

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
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID AND R.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
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
                            // sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[i - 2]["AMOUNT"]);  //commented on 21/04/2014 Req by S.Sarma
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
                    //sPayInfo = sPayInfo + "; Amount : " + Convert.ToString(dsTemp.Tables[0].Rows[dsTemp.Tables[0].Rows.Count - 1]["AMOUNT"]);
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

        private string GetOrderNo(string sTransId)
        {
            string sRecNo = string.Empty;


            string sql = " SELECT ORDERNUM from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID ='" + sTransId + "' AND TERMINALID = '" + sTerminal + "' ";

            DataTable dtStoreInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtStoreInfo);

            if (dtStoreInfo != null && dtStoreInfo.Rows.Count > 0)
            {
                for (int i = 1; i <= dtStoreInfo.Rows.Count; i++)
                {
                    if (i > 1)
                        sRecNo = sRecNo + ",";
                    if (Convert.ToString(dtStoreInfo.Rows[0]["ORDERNUM"]) == string.Empty)
                        sRecNo = "...";
                    else
                        sRecNo += Convert.ToString(dtStoreInfo.Rows[0]["ORDERNUM"]);
                }
            }

            return sRecNo;

        }

        private string GetRefReciptId(string sTransId)
        {
            string sRecNo = string.Empty;


            string sql = " select RECEIPTID from RETAILTRANSACTIONPAYMENTTRANS " +
                 " where TRANSACTIONID in (select ADVANCEADJUSTMENTID from RETAIL_CUSTOMCALCULATIONS_TABLE where transactionid ='" + sTransId + "')";

            DataTable dtStoreInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, connection);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtStoreInfo);

            if (dtStoreInfo != null && dtStoreInfo.Rows.Count > 0)
            {
                for (int i = 1; i <= dtStoreInfo.Rows.Count; i++)
                {
                    if (i > 1)
                        sRecNo = sRecNo + ",";
                    if (Convert.ToString(dtStoreInfo.Rows[0]["RECEIPTID"]) == string.Empty)
                        sRecNo = "...";
                    else
                        sRecNo += Convert.ToString(dtStoreInfo.Rows[0]["RECEIPTID"]);
                }
            }

            return sText14 + " : " + "" + sRecNo;

        }

        private string GetFreezedRateList()
        {
            DataTable dtRate = new DataTable();
            string sFreezedRateList = string.Empty;
            string sqlBookedSku = " DECLARE @ORDERNUM VARCHAR(20) declare @IsFreezedRate int " +
                                  " SELECT @ORDERNUM = ORDERNUM from [RETAILDEPOSITTABLE] WHERE TRANSACTIONID = @TRANSACTIONID AND TERMINALID = '" + sTerminal + "' " +
                                  " select @IsFreezedRate=ISMETALRATEFREEZE from CUSTORDER_HEADER WHERE ORDERNUM = @ORDERNUM and ORDERNUM!=''" +
                                  " IF(@IsFreezedRate=1)" +
                                  " BEGIN" +
                                  " select LineFixRate,CONFIGID from CUSTORDER_DETAILS WHERE ORDERNUM = @ORDERNUM and ORDERNUM!='' and LineFixRate>0" +
                                  " END";


            SqlCommand command = new SqlCommand(sqlBookedSku, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sReceiptNo;

            SqlDataAdapter daBookedSku = new SqlDataAdapter(command);

            daBookedSku.Fill(dtRate);

            if (dtRate != null && dtRate.Rows.Count > 0)// Convert.ToString(decimal.Round(Convert.ToDecimal(Convert.ToString(drNew["LineFixRate"]), 2, MidpointRounding.AwayFromZero));
            {
                foreach (DataRow drNew in dtRate.Rows)
                {
                    if (string.IsNullOrEmpty(sFreezedRateList))
                        sFreezedRateList = Convert.ToString(drNew["CONFIGID"]) + " : " + Convert.ToString(decimal.Round(Convert.ToDecimal(Convert.ToString(drNew["LineFixRate"])), 2, MidpointRounding.AwayFromZero));
                    else
                        sFreezedRateList = sFreezedRateList + ", " + Convert.ToString(drNew["CONFIGID"]) + " : " + Convert.ToString(decimal.Round(Convert.ToDecimal(Convert.ToString(drNew["LineFixRate"])), 2, MidpointRounding.AwayFromZero));
                }
                sFreezedRateList = sText16 + " @ " + "" + sFreezedRateList;
            }


            return sFreezedRateList;

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

        private string getSalesManName(string sTransId, string sTerminalId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select top 1 d.NAME as Name from RETAILTRANSACTIONSALESTRANS A" +
                      " LEFT JOIN dbo.RETAILSTAFFTABLE AS T11 on A.STAFF =T11.STAFFID " +
                      " LEFT JOIN dbo.HCMWORKER AS T22 ON T11.STAFFID = T22.PERSONNELNUMBER" +
                      " left join dbo.DIRPARTYTABLE as d on d.RECID = T22.PERSON " +
                      " Where TRANSACTIONID='" + sTransId + "' and a.TERMINALID='" + sTerminalId + "' and isnull(A.STAFF,'')!=''";

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
                return "";
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

        private decimal getItemTaxPercentage()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            decimal sResult = 0m;

            string commandText = string.Empty;
            commandText = "select top 1 isnull(ADVANCETAXPERCENTAGE,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;

        }

        private void GetTaxAdvanceData(ref DataTable dtCol)
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

            if (bIsConvToAd)
                tblTaxInfo.Rows.Add(dExcludTaxAmt + dTaxAmt, 0, 0, dExcludTaxAmt + dTaxAmt, dBookedQty, "");
            else
                tblTaxInfo.Rows.Add(dExcludTaxAmt, dTaxPct, dTaxAmt, dExcludTaxAmt + dTaxAmt, dBookedQty, "");

            dtCol = tblTaxInfo;

            if (dtCol != null && dtCol.Rows.Count < 6 && dtCol.Rows.Count > 0)
            {
                for (int j = dtCol.Rows.Count; j < 6; j++)
                    dtCol.Rows.Add();
            }
        }

        #region Adjustment Item Name
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS]");
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

        private decimal getTaxPctValue(string sTaxCodeGrp)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select CAST(ISNULL(TAXVALUE,0)AS DECIMAL(28,2)) TAXVALUE from TAXDATA");
            sbQuery.Append(" where  TAXCODE='" + sTaxCodeGrp + "' and DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }
        #endregion
    }
}
