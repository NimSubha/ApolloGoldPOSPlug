
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using LSRetailPosis.Settings;
using System.IO;
using BarcodeLib.Barcode;


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmSaleInv : Form
    {
        BlankOperations oBlank = new BlankOperations();
        SqlConnection connection;
        // string sTransactionId = "";
        string sCustName = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";
        string sCPanNo = "-";
        long iTime_Ticks = 0;
        string sInvoiceNo = "";
        string sInvDt = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";

        string sAmtinwds = "";
        string sAmtinwdsArabic = "";
        string sCustCode = "-";
        string sReceiptNo = "-";

        string sTerminal = string.Empty;
        string sTitle = string.Empty;

        // string sStorePh = "-";
        string sInvoiceFooter = "-";
        string sTime = "";

        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sDuplicateCopy = string.Empty;//aded on 14/04/2014 R.Hossain
        byte[] bCompImage = null;
        string sCurrencySymbol = "";
        Double dTotAmt = 0;
        int iGiftPrint = 0;
        int iInvLang = 0;
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
        DataTable dtGroupTotal = new DataTable();
        string sStoreArabicName = string.Empty;
        string sPaidBy = string.Empty;
        int isItFromShowJournal = 0;
        int isMGDStore = 0;
        string sLanguage = string.Empty;
        int isBothInv = 0;
        decimal dNonTaxableAmt = 0;
        decimal dNonTaxableAmt1 = 0;
        decimal dTaxableAmt = 0;
        decimal dTaxableAmt1 = 0;

        decimal dTaxAmt = 0;
        decimal dTaxAmt1 = 0;
        string sTxtAdj = "";
        string sCompanyTaxRegNo = string.Empty;
        DateTime taxCutOffDate = new DateTime();
        string sGovtIdInfo = "";
        bool bTaxPrint = false;

        public frmSaleInv()
        {
            InitializeComponent();
        }

        public frmSaleInv(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmSaleInv(IPosTransaction posTransaction, SqlConnection conn, bool bDuplicate, int iIsGift, int iLanForInv, int isItFromShowJour)
        {
            InitializeComponent();

            #region[Param Info]
            retailTrans = posTransaction as RetailTransaction;
            string sCustomTransType = string.Empty; //will open RHossain 

            iInvLang = iLanForInv;

            if (retailTrans != null)
            {
                sSM = getSalesManName(retailTrans.TransactionId, retailTrans.TerminalId);

                isItFromShowJournal = isItFromShowJour;

                sTerminal = retailTrans.TerminalId;
                string sCustId = "";

                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                {
                    // sCustName = Convert.ToString(retailTrans.Customer.Name);
                    sCustId = retailTrans.Customer.CustomerId;
                }
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                //if (Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                //    sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);


                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = getISD(sCustId) + " " + Convert.ToString(retailTrans.Customer.Telephone);

                if (string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.Telephone)))
                {
                    sContactNo = getISD(sCustId) + " " + getMobilePriamry(sCustId);
                }

                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);

                if (Convert.ToString(retailTrans.Customer.Email) != string.Empty)
                    sEmail = Convert.ToString(retailTrans.Customer.Email);


                if (string.IsNullOrEmpty(sEmail))
                {
                    sEmail = getEmail(sCustId);
                }

                if (Convert.ToString(retailTrans.OperatorName) != string.Empty)
                    sOperatorName = Convert.ToString(retailTrans.OperatorName);


                //sCPanNo
                if (string.IsNullOrEmpty(retailTrans.Customer.Name))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName)))
                    {
                        // sCustName = Convert.ToString(retailTrans.PartnerData.LCCustomerName);
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


                //iTime_Ticks = getTransTime(sInvoiceNo, sTerminal);
                iTime_Ticks = Convert.ToInt64(retailTrans.EndDateTime.TimeOfDay.TotalSeconds);
                //----------store Info
                //if (Convert.ToString(retailTrans.StoreName) != string.Empty)
                //    sStoreName = Convert.ToString(retailTrans.StoreName);
                //if (Convert.ToString(retailTrans.StoreAddress) != string.Empty)
                //    sStoreAddress = Convert.ToString(retailTrans.StoreAddress);
                //if (Convert.ToString(retailTrans.StorePhone) != string.Empty)
                //    sStorePhNo = Convert.ToString(retailTrans.StorePhone);

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

                sBC = sReceiptNo;

                sCustomTransType = GetCustomTransType(sInvoiceNo, retailTrans.TerminalId);

                sCustName = GetCustomerNameFromDirPartyTable(sCustCode);
                sGovtIdInfo = GetCustomerGovtIdInfo(sCustCode);
                sCurrencySymbol = oBlank.GetCurrencySymbol();

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

                // sCINNo = oBlank.getValue("select CINNO  from RETAILSTORETABLE where STORENUMBER ='" + Convert.ToString(ApplicationSettings.Terminal.StoreId) + "'");

            }
            #endregion
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo, ref sStoreArabicName, ref isMGDStore);

            taxCutOffDate = getTaxCutOffDate();

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain

            sCompanyTaxRegNo = oBlank.GetCompanyTaxRegNum(conn);
            if (bDuplicate)
                sDuplicateCopy = ""; // "DUPLICATE"; It will open later RHossain 21/04/2014


            iGiftPrint = iIsGift;
            bool bTaxApplicable = IsTaxApplicable();
            bTaxPrint = IsWithTaxPrint();
            sLanguage = "en-us";

            if (retailTrans.SaleIsReturnSale)
            {
                //if (iLanForInv == 1)
                //{
                //    sLanguage = "en-us";
                //}
                //else if (iLanForInv == 2)
                //{
                //    sLanguage = "ar";
                //}
                //else if (iLanForInv == 3)
                //{
                //    sLanguage = "en-us";
                //    isBothInv = 1;
                //}

                if (sCustomTransType == "0")
                {
                    if (bTaxPrint)
                    {
                        if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                            sTitle = "INVOICE";
                        else
                            sTitle = "TAX INVOICE";
                    }
                    else
                        sTitle = "INVOICE";
                }
                else
                {
                    if (bTaxPrint)
                    {
                        if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                            sTitle = "RETURN INVOICE";
                        else
                            sTitle = "TAX RETURN INVOICE";
                    }
                    else
                        sTitle = "RETURN INVOICE";
                }
            }
            else if (sCustomTransType == "1" || sCustomTransType == "2" || sCustomTransType == "3" || sCustomTransType == "4")
            {
                //sTitle = "PURCHASE";
                if (iLanForInv == 1)
                {
                    sTitle = "PURCHASE";
                    sLanguage = "en-us";
                }
                else if (iLanForInv == 2)
                {
                    sTitle = "فاتورة بيـع";
                    sLanguage = "ar";
                }
                else if (iLanForInv == 3)
                {
                    sTitle = "فاتورة بيـع" + Environment.NewLine + "PURCHASE";
                    sLanguage = "en-us";
                    isBothInv = 1;
                }
            }
            else
            {
                if (iGiftPrint == 1)
                {
                    sLanguage = "en-us";
                    sTitle = "GIFT INVOICE";
                }
                if (iLanForInv == 1)
                {
                    sLanguage = "en-us";
                    if (bTaxPrint)
                    {
                        if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                            sTitle = "INVOICE";
                        else
                            sTitle = "TAX INVOICE";
                    }
                    else
                        sTitle = "INVOICE";
                }
                else if (iLanForInv == 2)
                {
                    sLanguage = "ar";
                    if (bTaxPrint)
                    {
                        if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                            sTitle = "فاتورة بيـع";
                        else
                            sTitle = "الضريبة فاتورة بيـع";
                    }
                    else
                        sTitle = "فاتورة بيـع";

                }
                else if (iLanForInv == 3)
                {
                    sLanguage = "en-us";

                    if (bTaxPrint)
                    {
                        if (Convert.ToDateTime(sInvDt) < taxCutOffDate)
                            sTitle = "فاتورة بيـع" + Environment.NewLine + "INVOICE";
                        else
                            sTitle = "الضريبة فاتورة بيـع" + Environment.NewLine + "TAX INVOICE";
                    }
                    else
                        sTitle = "فاتورة بيـع" + Environment.NewLine + "INVOICE";

                    isBothInv = 1;
                }
            }

            if (bTaxPrint && !string.IsNullOrEmpty(sCompanyTaxRegNo))
            {
                if (iLanForInv == 3)
                    sCompanyTaxRegNo = " TAX REGISTRATION NO " + "رقم التسجيل الضريبي" + " : " + sCompanyTaxRegNo;
                else if (iLanForInv == 2)
                    sCompanyTaxRegNo = "رقم التسجيل الضريبي" + " : " + sCompanyTaxRegNo;
                else
                {
                    if (ApplicationSettings.Terminal.StoreCountry == "SG" || ApplicationSettings.Terminal.StoreCountry == "MY")
                        sCompanyTaxRegNo = " GST REG NO :" + sCompanyTaxRegNo;
                    else
                        sCompanyTaxRegNo = " TAX REGISTRATION NO :" + sCompanyTaxRegNo;
                }
            }
            else
                sCompanyTaxRegNo = "";

        }

        public frmSaleInv(string sTransactionId, SqlConnection conn)
        {
            InitializeComponent();

            sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void frmSaleInv_Load(object sender, EventArgs e)
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

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;

            bool bTaxApplicable = IsTaxApplicable();

            taxCutOffDate = getTaxCutOffDate();

            localReport.ReportPath = "GSTRptSaleInv.rdlc";

            DataTable dtSalesData = new DataTable();
            DataTable dtTaxAdjData = new DataTable();
            //DataSet dataset = new DataSet();
            DataSet dsSubTotal = new DataSet();
            DataSet dsTender = new DataSet();

            DataSet dsStdRate = new DataSet();
            DataSet dsTaxInfo = new DataSet();
            DataSet dsPaymentInfo = new DataSet();

            DataSet dsTaxDetail = new DataSet(); // added on 31/03/2014 RHossain
            DataSet dsCompanyLogo = new DataSet(); // added on 27/08/2014 RHossain

            DataTable dtMatalRate = new DataTable();


            GetMetalRates(ref dtMatalRate);
            GetSalesInvData(ref dtSalesData);
            GetSubTotal(ref dsSubTotal);
            GetTender(ref dsTender, bTaxPrint);
            GetStdRate(ref dsStdRate);
            GetTaxInfo(ref dsTaxInfo);
            GetPayInfo(ref dsPaymentInfo);

            GetTaxDetail(ref dsTaxDetail); // added on 31/03/2014 RHossain

            GetCompanyLogo(ref dsCompanyLogo);// added on 26/08/2014 RHossain

            if (bTaxPrint)
                GetTaxAdjustData(ref dtTaxAdjData, dtSalesData);

            //decimal dShippingCharge = GetShipingCharges();

            decimal dAmt = decimal.Round((Math.Abs(Convert.ToDecimal(dTotAmt))), 2, MidpointRounding.AwayFromZero);
            sAmtinwds = oBlank.Amtinwds(Convert.ToDouble(dAmt)); // added on 28/04/2014 RHossain               
            sAmtinwdsArabic = oBlank.AmtinwdsInArabic(Math.Abs(dTotAmt));

            if (iInvLang == 2)
                sAmtinwds = sAmtinwdsArabic;
            else if (iInvLang == 3)
                sAmtinwds = sAmtinwds + System.Environment.NewLine + "" + sAmtinwdsArabic;

            bCompImage = oBlank.GetCompLogo(Convert.ToString(ApplicationSettings.Database.DATAAREAID));// added on 26/08/2014 for MME

            sCurrencySymbol = oBlank.GetCurrencySymbol();

            if (dtTaxAdjData != null && dtTaxAdjData.Rows.Count > 0 && dtSalesData != null && dtSalesData.Rows.Count > 0)
                sTxtAdj = "Adjustment";

            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "Detail";
            dsSalesOrder.Value = dtSalesData;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtSalesData));


            ReportDataSource RDSubTotal = new ReportDataSource();
            RDSubTotal.Name = "SubTotal";
            RDSubTotal.Value = dsSubTotal.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SubTotal", dsSubTotal.Tables[0]));


            ReportDataSource RDTender = new ReportDataSource();
            RDTender.Name = "Tender";
            RDTender.Value = dsTender.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dsTender.Tables[0]));

            ReportDataSource RDStdRate = new ReportDataSource();
            RDStdRate.Name = "StdRate";
            RDStdRate.Value = dsStdRate.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("StdRate", dsStdRate.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "TaxInfo";
            RDTaxInfo.Value = dsTaxInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInfo", dsTaxInfo.Tables[0]));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));

            ReportDataSource RDTaxInDetail = new ReportDataSource();
            RDTaxInDetail.Name = "TaxInDetails";
            RDTaxInDetail.Value = dsTaxDetail.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInDetails", dsTaxDetail.Tables[0]));


            ReportDataSource RDCompanyLogo = new ReportDataSource();
            RDCompanyLogo.Name = "CompanyLogo";
            RDCompanyLogo.Value = dsCompanyLogo.Tables[0];
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("CompanyLogo", dsCompanyLogo.Tables[0]));// added on 28/08/2014

            ReportDataSource dsBCI = new ReportDataSource();
            dsBCI.Name = "BCI";
            dsBCI.Value = dtBarcode;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("BCI", dtBarcode));

            ReportDataSource dsGT = new ReportDataSource();// group total trans type wise
            dsBCI.Name = "dsGT";
            dsBCI.Value = dtGroupTotal;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsGT", dtGroupTotal));

            ReportDataSource dsMR = new ReportDataSource();
            dsBCI.Name = "MR";
            dsBCI.Value = dtBarcode;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("MR", dtMatalRate));

            ReportDataSource dsTaxAdjData = new ReportDataSource();
            dsTaxAdjData.Name = "TaxAdjust";
            dsTaxAdjData.Value = dtTaxAdjData;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxAdjust", dtTaxAdjData));



            ReportParameter[] param = new ReportParameter[33];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
            param[4] = new ReportParameter("CPanNo", sCPanNo);
            param[5] = new ReportParameter("InvoiceNo", sInvoiceNo);
            param[6] = new ReportParameter("InvDate", sInvDt + " " + sTime);

            param[7] = new ReportParameter("StoreName", sStoreName);
            param[8] = new ReportParameter("StoreAddress", sStoreAddress);
            param[9] = new ReportParameter("StorePhone", sStorePhNo);

            if (!string.IsNullOrEmpty(sAmtinwds))
                param[10] = new ReportParameter("Amtinwds", sAmtinwds);
            else
                param[10] = new ReportParameter("Amtinwds", "zero");

            param[11] = new ReportParameter("CCode", sCustCode);
            param[12] = new ReportParameter("ReceiptNo", sReceiptNo);
            param[13] = new ReportParameter("Title", sTitle);
            param[14] = new ReportParameter("InvoiceFooter", sInvoiceFooter);
            param[15] = new ReportParameter("InvoiceTime", sTime); // added on 29/03/214 req from Sailendra Da
            param[16] = new ReportParameter("CompName", sCompanyName); // added on 14/04/214 req from Sailendra Da
            param[17] = new ReportParameter("CIN", sCINNo); // added on 14/04/214 req from Sailendra Da
            param[18] = new ReportParameter("DuplicateCopy", sDuplicateCopy); // added on 14/04/2014 req from Sailendra Da
            param[19] = new ReportParameter("cs", sCurrencySymbol); //Currencysymbol added on 02/09/2014 req from Sailendra Da

            param[20] = new ReportParameter("Email", sEmail); // added on 14/04/2014 req from Sailendra Da
            param[21] = new ReportParameter("SM", sSM); //Currencysymbol added on 02/09/2014 req from Sailendra Da
            param[22] = new ReportParameter("MRATES", sMetalRates);
            param[23] = new ReportParameter("StrArabicName", sStoreArabicName);
            param[24] = new ReportParameter("OperatorName", sOperatorName);
            param[25] = new ReportParameter("PaidBy", sPaidBy);
            param[26] = new ReportParameter("ShippingCharge", "0");
            param[27] = new ReportParameter("TotNonTaxableAmt", Convert.ToString(dNonTaxableAmt + dNonTaxableAmt1));
            param[28] = new ReportParameter("TotTaxAmt", Convert.ToString(dTaxAmt + dTaxAmt1));
            param[29] = new ReportParameter("txtAdj", sTxtAdj);
            param[30] = new ReportParameter("CompanyTaxRegNo", sCompanyTaxRegNo);
            param[31] = new ReportParameter("TotTaxableAmt", Convert.ToString(dTaxableAmt + dTaxableAmt1));
            param[32] = new ReportParameter("GovtId", sGovtIdInfo);


            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();

            if (isItFromShowJournal == 0)
            {
                oBlank.Export(reportViewer1.LocalReport);
                oBlank.Print_Invoice(reportViewer1.LocalReport, 2);
                this.Close();
            }
        }

        private void GetTaxAdjustData(ref DataTable dtCol, DataTable dtSalesData)
        {

            string sIngrDetails = string.Empty;
            string sQuery = "";
            sQuery = "TAX_ADJUSTMENT_INVOICE";


            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSID", SqlDbType.NVarChar).Value = sInvoiceNo;
            command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = retailTrans.TerminalId;// ApplicationSettings.Terminal.TerminalId;
            command.Parameters.Add("@LANGUAGEID", SqlDbType.NVarChar).Value = sLanguage;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dtCol);

            Dictionary<string, decimal> dicSum = new Dictionary<string, decimal>();
            foreach (DataRow row in dtCol.Rows)
            {
                dTotAmt = dTotAmt + Convert.ToDouble(decimal.Round(Convert.ToDecimal(row["AMOUNT"]), 2, MidpointRounding.AwayFromZero));

                string group = row["TRANSACTIONTYPE"].ToString();
                decimal price = (Convert.ToDecimal(row["PRICE"]));
                decimal amt = (Convert.ToDecimal(row["AMOUNT"]));
                decimal pcs = Math.Abs(Convert.ToDecimal(row["QTY"]));
                decimal gwt = Math.Abs(Convert.ToDecimal(row["GROSSWT"]));
                decimal netwt = Math.Abs(Convert.ToDecimal(row["NETWT"]));
                decimal gsale = Math.Abs(Convert.ToDecimal(row["AMOUNT"])) - Math.Abs(Convert.ToDecimal(row["Tax"]));
                decimal tax = (Convert.ToDecimal(row["Tax"]));

                if (dicSum.ContainsKey(group))
                    dicSum[group] += amt;
                else
                    dicSum.Add(group, amt);

                if (tax == 0)
                    dNonTaxableAmt += amt;
                else
                {
                    dTaxAmt += tax;
                    dTaxableAmt -= gsale;
                }
            }

            var invoiceSum =
                        dtCol.AsEnumerable()
                        .Select(x =>
                            new
                            {
                                TRANSACTIONTYPE = x["TRANSACTIONTYPE"],
                                AMOUNT = x["AMOUNT"],
                                QTY = x["QTY"],
                                GROSSWT = x["GROSSWT"],
                                NETWT = x["NETWT"],
                                GSALE = Convert.ToDecimal(x["AMOUNT"]) - Convert.ToDecimal(x["TAX"]),
                                TAX = x["TAX"],
                            }
                         )
                         .GroupBy(s => new { s.TRANSACTIONTYPE })
                         .Select(g =>
                                new
                                {
                                    TRANSACTIONTYPE = g.Key.TRANSACTIONTYPE,
                                    AMOUNT = g.Sum(x => Math.Round(Convert.ToDecimal(x.AMOUNT), 2)),
                                    QTY = g.Sum(x => Math.Round(Convert.ToDecimal(x.QTY), 3)),
                                    GROSSWT = g.Sum(x => Math.Round(Convert.ToDecimal(x.GROSSWT), 3)),
                                    NETWT = g.Sum(x => Math.Round(Convert.ToDecimal(x.NETWT), 3)),
                                    GSALE = g.Sum(x => Math.Round(Convert.ToDecimal(x.AMOUNT) - Convert.ToDecimal(x.TAX), 2)),
                                    TAX = g.Sum(x => Math.Round(Convert.ToDecimal(x.TAX), 2)),
                                }
                         );

            if (dtGroupTotal != null && dtGroupTotal.Rows.Count > 0)
            {
                foreach (var element in invoiceSum)
                {
                    var row = dtGroupTotal.NewRow();
                    row["NAME"] = element.TRANSACTIONTYPE;
                    row["AMOUNT"] = element.AMOUNT;
                    row["TotQTY"] = element.QTY;
                    row["TotGROSSWT"] = element.GROSSWT;
                    row["TotNETWT"] = element.NETWT;
                    row["TotGSale"] = element.AMOUNT - element.TAX;
                    row["TotTax"] = element.TAX;
                    dtGroupTotal.Rows.Add(row);
                }
            }
            else
            {
                dtGroupTotal = new DataTable();

                dtGroupTotal.Columns.Add("NAME", typeof(string));
                dtGroupTotal.Columns.Add("AMOUNT", typeof(decimal));
                dtGroupTotal.Columns.Add("TotQTY", typeof(decimal));
                dtGroupTotal.Columns.Add("TotGROSSWT", typeof(decimal));
                dtGroupTotal.Columns.Add("TotNETWT", typeof(decimal));
                dtGroupTotal.Columns.Add("TotGSale", typeof(decimal));
                dtGroupTotal.Columns.Add("TotTax", typeof(decimal));
                foreach (var element in invoiceSum)
                {
                    var row = dtGroupTotal.NewRow();
                    row["NAME"] = element.TRANSACTIONTYPE;
                    row["AMOUNT"] = element.AMOUNT;
                    row["TotQTY"] = element.QTY;
                    row["TotGROSSWT"] = element.GROSSWT;
                    row["TotNETWT"] = element.NETWT;
                    row["TotGSale"] = element.AMOUNT - element.TAX;
                    row["TotTax"] = element.TAX;
                    dtGroupTotal.Rows.Add(row);
                }
            }

            if (dtSalesData != null && dtSalesData.Rows.Count == 0)
            {
                if (dtCol != null && dtCol.Rows.Count < 6 && dtCol.Rows.Count > 0)
                {
                    for (int j = dtCol.Rows.Count; j < 6; j++)
                        dtCol.Rows.Add();
                }
            }
        }

        private void GetSalesInvData(ref DataTable dtCol)
        {
            bool bTaxApplicable = IsTaxApplicable();

            string sIngrDetails = string.Empty;
            string sQuery = "";
            if (bTaxPrint)
            {
                if (Convert.ToDateTime(sInvDt) >= taxCutOffDate)
                    sQuery = "TAX_GETSALESORPURCHASEINVOICE";
                else
                    sQuery = "GETSALESORPURCHASEINVOICE";
            }
            else
                sQuery = "GETSALESORPURCHASEINVOICE";

            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSID", SqlDbType.NVarChar).Value = sInvoiceNo;
            command.Parameters.Add("@TerminalID", SqlDbType.NVarChar).Value = retailTrans.TerminalId;// ApplicationSettings.Terminal.TerminalId;
            command.Parameters.Add("@LANGUAGEID", SqlDbType.NVarChar).Value = sLanguage;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dtCol);


            #region Ingredients Item
            DataTable dtIng = new DataTable();
            for (int i = 0; i <= dtCol.Rows.Count - 1; i++) //.Select("SomeIntColumn > 0")
            {
                bool bIsMRP = isMRP(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));

                dTotAmt = dTotAmt + Convert.ToDouble(decimal.Round(Convert.ToDecimal(dtCol.Rows[i]["AMOUNT"]), 2, MidpointRounding.AwayFromZero));
                dtIng = GetIngredientItemData(sInvoiceNo, retailTrans.TerminalId, Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));
                string sArabicDesc = GetArabicDesc(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));

                string sItemDesc = Convert.ToString(dtCol.Rows[i]["ITEMDESC"]);

                if (!string.IsNullOrEmpty(Convert.ToString(dtCol.Rows[i]["ITEMDESC"])) && string.IsNullOrEmpty(Convert.ToString(dtCol.Rows[i]["INVENTBATCHID"])))
                    sIngrDetails = Convert.ToString(dtCol.Rows[i]["ITEMDESC"]) + "" + System.Environment.NewLine;
                else
                    sIngrDetails = Convert.ToString(dtCol.Rows[i]["ITEMDESC"]);

                decimal dTotIngStnWt = 0;
                decimal dTotIngStnWt1 = 0;
                decimal dTotIngDmdWt = 0;
                decimal dTotIngStnPcs = 0;
                decimal dTotIngDmdPcs = 0;
                string sStoneCCC = string.Empty;
                string sDmdCCC = string.Empty;
                string sSaleRate = "";
                decimal dRate = 0m;
                decimal dGoldQty = 0m;
                decimal dStnAmt = 0m;

                for (int j = 0; j <= dtIng.Rows.Count - 1; j++)
                {
                    int iMetalType = GetMetalType(Convert.ToString(dtIng.Rows[j]["ITEMID"]));

                    if (iMetalType == (int)MetalType.Gold)
                    {
                        dGoldQty += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);

                        //if (!string.IsNullOrEmpty(sSaleRate))
                        //{
                        dRate += Convert.ToDecimal(dtIng.Rows[j]["CRate"]) * Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                        // }
                    }

                    if (iMetalType == (int)MetalType.Stone)
                    {
                        if (Convert.ToString(dtIng.Rows[j]["UNITID"]).ToUpper() == "CT")
                            dTotIngStnWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]) / 5;
                        else
                            dTotIngStnWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);

                        dTotIngStnWt1 += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                        dTotIngStnPcs += Convert.ToDecimal(dtIng.Rows[j]["PCS"]);
                        sStoneCCC = Convert.ToString(dtIng.Rows[j]["INVENTCOLORID"]);
                        dStnAmt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]) * Convert.ToDecimal(dtIng.Rows[j]["CRate"]);
                    }
                    else if (iMetalType == (int)MetalType.LooseDmd)
                    {
                        dTotIngDmdWt += Convert.ToDecimal(dtIng.Rows[j]["QTY"]);
                        dTotIngDmdPcs += Convert.ToDecimal(dtIng.Rows[j]["PCS"]);
                        sDmdCCC = Convert.ToString(dtIng.Rows[j]["INVENTCOLORID"]);
                    }
                }

                decimal dRatePct = 0m;
                bool bMetalRateIncTax = IsMetalRateInclOfTax(ref dRatePct);

                if (dRate > 0 && dGoldQty > 0
                    && bMetalRateIncTax
                    && dRatePct > 0)
                {
                    sSaleRate = Convert.ToString(decimal.Round((Convert.ToDecimal(dRate)
                        + Math.Abs(Convert.ToDecimal(dRate / dGoldQty) * dRatePct / 100)), 2, MidpointRounding.AwayFromZero));
                }
                else if (dRate > 0 && dGoldQty > 0)
                {
                    sSaleRate = Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dRate / dGoldQty)), 2, MidpointRounding.AwayFromZero));
                }

                //if (dTotIngStnWt > 0)
                //{
                //    sIngrDetails += "C- " + Convert.ToString(dTotIngStnWt) + "--" + dTotIngStnPcs + "--"+ sStoneCCC;    // D- 1.250--12, C- 1.250--
                //    if (dTotIngDmdWt > 0)
                //    {
                //        if (!string.IsNullOrEmpty(sIngrDetails))
                //            sIngrDetails = sIngrDetails + ", D- " + Convert.ToString(dTotIngDmdWt) + "--" + dTotIngDmdPcs + "--" + sDmdCCC;
                //    }
                //}
                //else if (dTotIngDmdWt > 0)
                //{
                //    if (!string.IsNullOrEmpty(sIngrDetails))
                //        sIngrDetails = sIngrDetails + ", D- " + Convert.ToString(dTotIngDmdWt) + "--" + dTotIngDmdPcs + "--" + sDmdCCC;
                //    else
                //        sIngrDetails += "D- " + Convert.ToString(dTotIngDmdWt) + "--" + dTotIngDmdPcs + "--" + sDmdCCC;
                //}
                if (dTotIngDmdWt > 0)
                {
                    sIngrDetails += "D- " + Convert.ToString(dTotIngDmdWt) + "--" + dTotIngDmdPcs + "--" + sDmdCCC;
                    if (dTotIngStnWt > 0)
                    {
                        if (!string.IsNullOrEmpty(sIngrDetails))
                            sIngrDetails = sIngrDetails + ", C- " + Convert.ToString(dTotIngStnWt) + "--" + dTotIngStnPcs;  //+ "--" + sStoneCCC
                    }
                }
                else if (dTotIngStnWt > 0)
                {
                    if (!string.IsNullOrEmpty(sIngrDetails))
                        sIngrDetails = sIngrDetails + ", C- " + Convert.ToString(dTotIngStnWt) + "--" + dTotIngStnPcs; /// + "--" + sStoneCCC
                    else
                        sIngrDetails += "C- " + Convert.ToString(dTotIngStnWt) + "--" + dTotIngStnPcs; // + "--" + sStoneCCC
                }
                else if (!string.IsNullOrEmpty(sIngrDetails))
                    sIngrDetails = sIngrDetails.Replace(System.Environment.NewLine, "");

                if (isBothInv == 1)
                {
                    //if (msg from admin) {
                    //    richTextBox.AppendText(Environment.NewLine + msg);
                    //    richTextBox.SelectionAlignment = HorizontalAlignment.Right;
                    //} else {
                    //    richTextBox.AppendText(Environment.NewLine + msg);
                    //    richTextBox.SelectionAlignment = HorizontalAlignment.Left;
                    //}

                    //if (!string.IsNullOrEmpty(sArabicDesc))
                    //    dtCol.Rows[i]["ITEMDESC"] = sArabicDesc + "" + System.Environment.NewLine + " " + sIngrDetails + "  " + GetParentItemIdAndCertificateNo(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));
                    //else
                    //    dtCol.Rows[i]["ITEMDESC"] = sIngrDetails + "  " + GetParentItemIdAndCertificateNo(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));

                    if (!string.IsNullOrEmpty(sArabicDesc))
                        dtCol.Rows[i]["ITEMDESC"] = Convert.ToString(sArabicDesc) + "" + System.Environment.NewLine + " " + sIngrDetails + "  " + GetParentItemIdAndCertificateNo(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));
                    else
                        dtCol.Rows[i]["ITEMDESC"] = sIngrDetails + "  " + GetParentItemIdAndCertificateNo(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));
                }
                else
                {
                    dtCol.Rows[i]["ITEMDESC"] = sIngrDetails + "  " + GetParentItemIdAndCertificateNo(Convert.ToString(dtCol.Rows[i]["SKUNUMBER"]));
                }

                //if (!string.IsNullOrEmpty(sSaleRate))
                //    dtCol.Rows[i]["ITEMDESC"] = sIngrDetails + "  @ " + sSaleRate;

                if (!string.IsNullOrEmpty(sSaleRate))
                {
                    if (isBothInv == 1)
                    {
                        if (!string.IsNullOrEmpty(sArabicDesc))
                            dtCol.Rows[i]["ITEMDESC"] = sArabicDesc + "" + System.Environment.NewLine + " " + sIngrDetails + "  @ " + sSaleRate;
                        else
                            dtCol.Rows[i]["ITEMDESC"] = sIngrDetails + "  @ " + sSaleRate;
                    }
                }

                if (!bIsMRP)
                {
                    if (dStnAmt > 0 && dTotIngStnWt > 0)
                    {
                        dtCol.Rows[i]["STNRATE"] = Convert.ToDecimal(decimal.Round(dStnAmt / dTotIngStnWt, 2, MidpointRounding.AwayFromZero));
                        dtCol.Rows[i]["STNWT"] = Convert.ToDecimal(decimal.Round(dTotIngStnWt, 3, MidpointRounding.AwayFromZero));
                    }
                }


                dtCol.AcceptChanges();
            }
            #endregion

            #region Group total
            Dictionary<string, decimal> dicSum = new Dictionary<string, decimal>();
            foreach (DataRow row in dtCol.Rows)
            {
                string group = row["TRANSACTIONTYPE"].ToString();
                decimal price = (Convert.ToDecimal(row["PRICE"]));
                decimal amt = Math.Abs(Convert.ToDecimal(row["AMOUNT"]));
                decimal pcs = Math.Abs(Convert.ToDecimal(row["QTY"]));
                decimal gwt = Math.Abs(Convert.ToDecimal(row["GROSSWT"]));
                decimal netwt = Math.Abs(Convert.ToDecimal(row["NETWT"]));
                decimal gsale = Math.Abs(Convert.ToDecimal(row["AMOUNT"])) - Math.Abs(Convert.ToDecimal(row["Tax"]));
                decimal tax = Math.Abs(Convert.ToDecimal(row["Tax"]));
                decimal stnwt = Math.Abs(Convert.ToDecimal(row["STNWT"]));
                decimal mkamt = Math.Abs(Convert.ToDecimal(row["MAKINGAMOUNT"]));

                if (dicSum.ContainsKey(group))
                {
                    dicSum[group] += amt;
                }
                else
                {
                    dicSum.Add(group, amt);
                }

                if (tax == 0)
                    dNonTaxableAmt1 += amt;
                else
                {
                    dTaxAmt1 += tax;
                    dTaxableAmt += gsale;
                }
                //dtCol.Compute("Sum(Convert(AMOUNT, 'decimal'))", "TRANSACTIONTYPE = '" + group + "'");
            }



            var invoiceSum =
                        dtCol.AsEnumerable()
                        .Select(x =>
                            new
                            {
                                TRANSACTIONTYPE = x["TRANSACTIONTYPE"],
                                AMOUNT = x["AMOUNT"],
                                QTY = x["QTY"],
                                GROSSWT = x["GROSSWT"],
                                NETWT = x["NETWT"],
                                GSALE = Convert.ToDecimal(x["AMOUNT"]) - Convert.ToDecimal(x["TAX"]),
                                TAX = x["TAX"],
                                STNWT = x["STNWT"],
                                MKAMT = x["MAKINGAMOUNT"]
                            }
                         )
                         .GroupBy(s => new { s.TRANSACTIONTYPE })
                         .Select(g =>
                                new
                                {
                                    TRANSACTIONTYPE = g.Key.TRANSACTIONTYPE,
                                    AMOUNT = g.Sum(x => Math.Round(Convert.ToDecimal(x.AMOUNT), 2)),
                                    QTY = g.Sum(x => Math.Round(Convert.ToDecimal(x.QTY), 3)),
                                    GROSSWT = g.Sum(x => Math.Round(Convert.ToDecimal(x.GROSSWT), 3)),
                                    NETWT = g.Sum(x => Math.Round(Convert.ToDecimal(x.NETWT), 3)),
                                    GSALE = g.Sum(x => Math.Round(Convert.ToDecimal(x.AMOUNT) - Convert.ToDecimal(x.TAX), 2)),
                                    TAX = g.Sum(x => Math.Round(Convert.ToDecimal(x.TAX), 2)),
                                    STNWT = g.Sum(x => Math.Round(Convert.ToDecimal(x.STNWT), 3)),
                                    MKAMT = g.Sum(x => Math.Round(Convert.ToDecimal(x.MKAMT), 2)),
                                }
                         );

            dtGroupTotal = new DataTable();

            dtGroupTotal.Columns.Add("NAME", typeof(string));
            dtGroupTotal.Columns.Add("AMOUNT", typeof(decimal));
            dtGroupTotal.Columns.Add("TotQTY", typeof(decimal));
            dtGroupTotal.Columns.Add("TotGROSSWT", typeof(decimal));
            dtGroupTotal.Columns.Add("TotNETWT", typeof(decimal));
            dtGroupTotal.Columns.Add("TotGSale", typeof(decimal));
            dtGroupTotal.Columns.Add("TotTax", typeof(decimal));
            dtGroupTotal.Columns.Add("TotStnWt", typeof(decimal));
            dtGroupTotal.Columns.Add("TotMkAmt", typeof(decimal));

            foreach (var element in invoiceSum)
            {
                var row = dtGroupTotal.NewRow();
                row["NAME"] = element.TRANSACTIONTYPE;
                row["AMOUNT"] = Math.Abs(element.AMOUNT);
                row["TotQTY"] = element.QTY;
                row["TotGROSSWT"] = element.GROSSWT;
                row["TotNETWT"] = element.NETWT;
                row["TotGSale"] = element.AMOUNT - element.TAX;
                row["TotTax"] = element.TAX;
                row["TotStnWt"] = element.STNWT;
                row["TotMkAmt"] = element.MKAMT;
                dtGroupTotal.Rows.Add(row);
            }
            #endregion


            if (dtCol != null && dtCol.Rows.Count < 6 && dtCol.Rows.Count > 0)
            {
                for (int j = dtCol.Rows.Count; j < 6; j++)
                    dtCol.Rows.Add();
            }
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

        public bool isMRP(string itemid)
        {
            int iR = 0;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT top 1 MRP FROM INVENTTABLE WHERE ITEMID='" + itemid + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iR = (int)reader.GetValue(0);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            if (iR == 0)
                return false;
            else
                return true;

        }

        public DataTable GetIngredientItemData(string sTransId, string sTerminalId, string sSKUNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                //SqlComm.CommandText = "SELECT ITEMID,ABS(CAST(ISNULL(QTY,0)AS DECIMAL(28,3))) QTY,"+
                //    " ABS(CAST(ISNULL(PCS,0)AS DECIMAL(28,0))) PCS  FROM  RETAIL_SALE_INGREDIENTS_DETAILS"+
                //    " WHERE TRANSACTIONID = '" + sTransId + "'  and SKUNUMBER ='" + sSKUNo + "'";

                SqlComm.CommandText = "  SELECT a.ITEMID,ABS(CAST(sum(ISNULL(a.QTY,0)) AS DECIMAL(28,3))) QTY," +
                    " ABS(CAST(sum(ISNULL(a.PCS,0)) AS DECIMAL(28,0))) PCS, isnull(a.INVENTCOLORID,'') INVENTCOLORID, " +
                    " ABS(CAST(sum(ISNULL(a.CRate,0)) AS DECIMAL(28,2))) CRate,a.UNITID    FROM  RETAIL_SALE_INGREDIENTS_DETAILS a" +
                    " WHERE a.TRANSACTIONID = '" + sTransId + "' " +
                    " and a.SKUNUMBER ='" + sSKUNo + "'" +
                    " and a.REFLINENUM in (select a.LINENUM " +
                    " from RETAILTRANSACTIONSALESTRANS a" +
                    " WHERE a.TRANSACTIONID = '" + sTransId + "'  and TERMINALID='" + sTerminalId + "' and a.TRANSACTIONSTATUS=0)" +
                    " group by  a.ITEMID,a.INVENTCOLORID,a.UNITID ";


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

        private void GetSubTotal(ref DataSet dsSTotal)
        {
            string sqlsubDtl = "   SELECT ABS(CAST(ISNULL(A.NETAMOUNT,0) AS DECIMAL(28,2))) AS NETAMOUNT" +
            " ,ABS(CAST(ISNULL(A.GROSSAMOUNT,0) AS DECIMAL(28,2))) AS GROSSAMOUNT" +
            " ,ABS(CAST(ISNULL(A.PAYMENTAMOUNT,0) AS DECIMAL(28,2))) AS PAYMENTAMOUNT " +
            " ,(ABS(CAST(ISNULL(A.DISCAMOUNT,0) AS DECIMAL(18,2))) " +
            "  + ABS(CAST(ISNULL(A.TOTALDISCAMOUNT,0) AS DECIMAL(18,2)))" +
             " )AS DISCAMOUNT," +
                //" ABS(CAST(ISNULL(A.SALESPAYMENTDIFFERENCE,0) AS DECIMAL(28,2)))AS PaymentDiffAmt" +
            " (CAST(ISNULL(A.SALESPAYMENTDIFFERENCE,0) AS DECIMAL(28,2))* (-1)) AS PaymentDiffAmt" +
                //" ,( SELECT "+
                //" (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2))) + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2)))) AS SHIPPINGCHARGE "+
                //"  FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B "+
                //" ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM AND A.TERMINALID = B.TERMINALID "+
                //" INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID "+
                //"  WHERE A.transactionid = @TRANSACTIONID AND A.TERMINALID = '" + sTerminal + "' AND A.TransactionStatus = 0  " +
                //"  AND D.ITEMTYPE = 2  and d.METALTYPE=16 ) SHIPPINGCHARGE" +
            " FROM RETAILTRANSACTIONTABLE A where transactionid = @TRANSACTIONID AND A.TERMINAL = '" + sTerminal + "' AND A.TYPE <> 1";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTotal, "SubTotal");

            if (dsSTotal != null && dsSTotal.Tables[0].Rows.Count > 0)
            {
                //dTotAmt = dTotAmt+  Convert.ToDouble(dsSTotal.Tables[0].Rows[0]["PAYMENTAMOUNT"]);
                dTotAmt = dTotAmt + Convert.ToDouble(dsSTotal.Tables[0].Rows[0]["PaymentDiffAmt"]);
                //sAmtinwds = Amtinwds(dPayAmt);

            }
        }

        protected decimal GetShipingCharges()
        {
            decimal dAmt = 0m;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" SELECT (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2))) + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2)))) AS SHIPPINGCHARGE ");
            commandText.Append(" FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B");
            commandText.Append(" ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM AND A.TERMINALID = B.TERMINALID");
            commandText.Append(" INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID");
            commandText.Append(" WHERE A.transactionid ='" + sInvoiceNo + "'  AND A.TERMINALID = '" + sTerminal + "' AND A.TransactionStatus = 0 ");
            commandText.Append(" AND D.ITEMTYPE = 2  and d.METALTYPE=16 ");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dAmt = (decimal)reader.GetValue(0);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return dAmt;

        }

        private void GetTender(ref DataSet dsSTender, bool bTaxApplicable)
        {
            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sqlsubDtl = " DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreNo + "'" +
                                "  SELECT N.NAME, M.TENDERTYPE,CAST(ISNULL(M.AMOUNTCUR,0) AS DECIMAL(28,2)) AS AMOUNT," +
                                " (ISNULL(CARDORACCOUNT,'') + ISNULL(CREDITVOUCHERID,'') + ISNULL(stuff(ISNULL(GIFTCARDID,''),1,LEN(ISNULL(GIFTCARDID,''))-4,REPLICATE('x', LEN(ISNULL(GIFTCARDID,''))-4)),'')) AS CARDNO" + //+ ISNULL(GIFTCARDID,'') ADDED ON 16/07/2014
                                " FROM RETAILTRANSACTIONPAYMENTTRANS M" +
                                " LEFT JOIN RETAILSTORETENDERTYPETABLE N ON M.TENDERTYPE = N.TENDERTYPEID" +
                                " WHERE M.TRANSACTIONID = @TRANSACTIONID AND N.CHANNEL = @CHANNEL  AND M.TERMINAL = '" + sTerminal + "' AND M.TRANSACTIONSTATUS = 0 ";

            if (!bTaxApplicable)
            {
                sqlsubDtl += //" UNION SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
                            " UNION ALL SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
                            " + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))))AS AMOUNT, '' AS CARDNO" +
                            " FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM AND A.TERMINALID = B.TERMINALID" +
                            " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID  LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                            " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
                            " WHERE A.transactionid = @TRANSACTIONID AND A.TERMINALID = '" + sTerminal + "'" +
                            " AND A.TransactionStatus = 0 AND D.ITEMTYPE = 2 and d.METALTYPE!=16" + //AND D.ITEMTYPE != 2 and d.METALTYPE!=16 added on 30/08/2017 //  
                            " ORDER BY M.TENDERTYPE ";
            }


            //" UNION SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
            //" UNION ALL SELECT F.NAME AS NAME,100 AS TENDERTYPE, (ABS(CAST(ISNULL(A.TAXAMOUNT,0)AS DECIMAL(18,2)))" +
            //" + ABS(CAST(ISNULL(A.NETAMOUNT,0)AS DECIMAL(28,2))))AS AMOUNT, '' AS CARDNO" +
            //" FROM RETAILTRANSACTIONSALESTRANS A INNER JOIN RETAIL_CUSTOMCALCULATIONS_TABLE B ON A.TRANSACTIONID = B.TRANSACTIONID AND A.LINENUM = B.LINENUM AND A.TERMINALID = B.TERMINALID" +
            //" INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID  LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
            //" LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
            //" WHERE A.transactionid = @TRANSACTIONID AND A.TERMINALID = '" + sTerminal + "'" +
            //" AND A.TransactionStatus = 0 AND D.ITEMTYPE != 2 and d.METALTYPE!=16" + //and d.METALTYPE!=16 added on 30/08/2017 //  
            //" ORDER BY M.TENDERTYPE ";


            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSTender, "Tender");



            if (dsSTender.Tables[0] != null && dsSTender.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in dsSTender.Tables[0].Rows)
                {
                    string group = row["Name"].ToString();
                    decimal amt = (Convert.ToDecimal(row["AMOUNT"]));//Math.Abs

                    if (iInvLang == 2 || iInvLang == 3)
                    {
                        if (string.IsNullOrEmpty(sPaidBy))
                            sPaidBy = "طريق التسوية : " + group + "  :   " + amt;
                        else
                            sPaidBy += "   " + group + " :   " + amt;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(sPaidBy))
                            sPaidBy = "Paid by : " + group + " :   " + amt;
                        else
                            sPaidBy += "   " + group + " :   " + amt;
                    }
                }
            }
        }

        private void GetStdRate(ref DataSet dsSStdRate)
        {
            string sqlStdRate = " SELECT TOP 1 CAST(RATES AS decimal (16,2)) AS STDGOLDRATE," +
                                " (SELECT DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID = @DATAAREAID) AS BASECONFIG " +
                                " FROM METALRATES WHERE INVENTLOCATIONID= @INVENTLOCATIONID AND METALTYPE=1 AND RETAIL=1 AND RATETYPE = 3" +
                                " AND ACTIVE=1 AND CONFIGIDSTANDARD = (SELECT DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID = @DATAAREAID)" +
                                " ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC";

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;
            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = sDataAreaId;

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSStdRate, "STDRate");
        }

        private void GetTaxInfo(ref DataSet dsSTaxInfo)
        {
            string sqlStdRate = " DECLARE @TINCST AS NVARCHAR(20)  DECLARE @TINVAT AS NVARCHAR(20)  SELECT @TINCST = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'')" +
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

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;
            //command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = sDataAreaId;

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

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
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID AND R.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID";

            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;
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

        private void GetTaxDetail(ref DataSet dsTaxDetail) // added on 31/03/2014 RHossain for  show the tax detail in line
        {
            //string sqlsubDtl = " select TAXCODE,ABS(CAST(ISNULL(sum(Amount),0)AS DECIMAL(18,2)))AS TAX from RETAILTRANSACTIONTAXTRANS " +
            //                   " where TRANSACTIONID = @TRANSACTIONID" +
            //                   " group by TRANSACTIONID,TERMINALID,STOREID ,TAXCODE";


            string sqlsubDtl = "select TAXCODE,ABS(CAST(ISNULL(sum(Amount),0)AS DECIMAL(18,2)))AS TAX" +
                              "  from RETAILTRANSACTIONTAXTRANS A " +
                              " join RETAILTRANSACTIONSALESTRANS B " +
                              "  on A.TRANSACTIONID =B.TRANSACTIONID and A.SALELINENUM =B.LINENUM " +
                              "  and B.TRANSACTIONSTATUS =0" +
                              " and A.TERMINALID=b.TERMINALID and A.STOREID =B.STORE " +
                              "  where A.TRANSACTIONID =@TRANSACTIONID and B.TERMINALID='" + sTerminal + "' " +
                              " group by A.TRANSACTIONID,A.TERMINALID,STOREID ,taxcode ";


            SqlCommand command = new SqlCommand(sqlsubDtl, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar).Value = sInvoiceNo;

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter daSTaxDetail = new SqlDataAdapter(command);

            daSTaxDetail.Fill(dsTaxDetail, "Tender");

            //for (int i = 0; i <= dsTaxDetail.Tables[0].Rows.Count - 1; i++)
            //{
            //    dTotAmt = dTotAmt + Convert.ToDouble(dsTaxDetail.Tables[0].Rows[i]["TAX"]);
            //}
        }


        /// <summary>
        /// Purpose : Invoice title change whether it is sales invoice or others.
        /// Created Date : 07/04/2014
        /// Created By : RHossain
        /// </summary>
        /// <param name="sTransId"></param>
        /// <returns></returns>
        private string GetCustomTransType(string sTransId, string sTerminalId) // Sales/Purchase/Exchange.....
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TRANSACTIONTYPE FROM RETAIL_CUSTOMCALCULATIONS_TABLE");
            commandText.Append(" WHERE TRANSACTIONID='" + sTransId + "' and STOREID='" + ApplicationSettings.Terminal.StoreId + "'");
            commandText.Append(" and TERMINALID='" + sTerminalId + "' order by TRANSACTIONTYPE asc");

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

        private string GetCustomerNameFromDirPartyTable(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("  select top 1 isnull(NAME ,'') FROM DIRPARTYTABLE ");
            commandText.Append(" left JOIN CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY");
            commandText.Append(" where ACCOUNTNUM='" + sCustAcc + "'");

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

        private enum IdentityProof
        {
            Govt_Id = 0,
            Driving_License = 1,
            Passport_No = 2
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
                    if (ApplicationSettings.Terminal.StoreCountry == "KW")
                    {
                        var lefttoright = ((Char)0x200E).ToString();

                        /* if (iInvLang == 1)
                             sResult = "CIVIL ID NO. : " + sGovtIdNo;
                         else if (iInvLang == 2)
                             sResult = "الرقم المدني  : " + sGovtIdNo;
                         else if (iInvLang == 3)
                             sResult = abbreviation + lefttoright + sGovtIdNo;*/


                        if (iGovtId == (int)IdentityProof.Driving_License || iGovtId == (int)IdentityProof.Govt_Id)
                        {
                            string followUpFormula = "Civil ID NO.";
                            string renewAbbreviation = "الرقم المدني  : ";
                            string abbreviation = followUpFormula + "  " + renewAbbreviation;

                            if (iInvLang == 1)
                                sResult = followUpFormula + sGovtIdNo;
                            else if (iInvLang == 2)
                                sResult = renewAbbreviation + sGovtIdNo;
                            else if (iInvLang == 3)
                                sResult = abbreviation + lefttoright + sGovtIdNo;
                        }
                        else if (iGovtId == (int)IdentityProof.Passport_No)
                        {
                            string followUpFormula1 = "Passport No : ";
                            string renewAbbreviation1 = "رقم الجواز  : ";
                            string abbreviation1 = followUpFormula1 + "  " + renewAbbreviation1;

                            if (iInvLang == 1)
                                sResult = followUpFormula1 + sGovtIdNo;
                            else if (iInvLang == 2)
                                sResult = renewAbbreviation1 + sGovtIdNo;
                            else if (iInvLang == 3)
                                sResult = abbreviation1 + lefttoright + sGovtIdNo;
                        }
                    }
                    else
                    {
                        if (iGovtId == (int)IdentityProof.Driving_License)
                            sResult = "Driving License  : " + sGovtIdNo;
                        else if (iGovtId == (int)IdentityProof.Govt_Id)
                            sResult = "Govt. Id  : " + sGovtIdNo;
                        else if (iGovtId == (int)IdentityProof.Passport_No)
                            sResult = "Passport No  : " + sGovtIdNo;
                    }
                }
            }

            return sResult;
        }


        /// <summary>
        /// Created on 12/05/2014
        /// Purppose: After craeting sales invoice, print SalesInvoice from sjow journal with local customer
        /// </summary>
        /// <param name="sTransId"></param>
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

        private void GetCompanyLogo(ref DataSet dsCompanyLogo)
        {
            string sqlStdRate = "SELECT [IMAGE] as COMPLOGO FROM CompanyImage WHERE DATAAREAID = @DATAAREAID";

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = ApplicationSettings.Database.DATAAREAID;

            SqlDataAdapter daCL = new SqlDataAdapter(command);

            daCL.Fill(dsCompanyLogo, "CompanyLogo");
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

        private long getTransTime(string sTransId, string sTerminalId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = " select isnull(TRANSTIME,0) from RETAILTRANSACTIONTABLE " +
                      " Where TRANSACTIONID='" + sTransId + "'" +
                      " and store= '" + ApplicationSettings.Database.StoreID + "'" +
                      " and TERMINAL= '" + sTerminalId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            long sResult = Convert.ToInt64(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;
        }

        private string GetParentItemIdAndCertificateNo(string sSKU)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select sp.CertificationNo ");//d.ITEMIDPARENT + '  ' +
            commandText.Append("from INVENTTABLE D ");
            commandText.Append("left join SKUTable_Posted sp ");
            commandText.Append("on d.ITEMID=sp.SkuNumber where d.itemid='" + sSKU + "'");

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

        private string GetArabicDesc(string sSKU)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select F.Description FROM INVENTTABLE D");
            commandText.Append(" LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID ");
            commandText.Append(" LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT");
            commandText.Append(" WHERE d.ITEMID='" + sSKU + "' and f.LANGUAGEID='ar'");

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

        protected bool IsMetalRateInclOfTax(ref decimal dRatePct)
        {
            bool bResult = false;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select MetalRateInclOfTax,isnull(METALRATETAXPERCENTAGE,0) METALRATETAXPERCENTAGE from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    bResult = Convert.ToBoolean((int)reader.GetValue(0));
                    dRatePct = (decimal)reader.GetValue(1);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return bResult;

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
    }
}
