
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
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Reflection;
//using GenCode128;


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmGSTSalesInv : Form
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

        string sTerminal = string.Empty;
        string sTitle = string.Empty;

        // string sStorePh = "-";
        string sInvoiceFooter = "-";
        string sTime = "";

        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sDuplicateCopy = string.Empty;//aded on 14/04/2014 R.Hossain

        Double dTotAmt = 0;

        string sRcardNo = string.Empty;
        string sCompPAN = string.Empty;

        string sEMIEmpCode = string.Empty;
        string sEMIOrderNo = string.Empty;

        string sExcNo = string.Empty;
        string sCTHNo = string.Empty;
        string sGSTNo = string.Empty;
        string sCreditNoteNo = "";
        int isCreditNote = 0;
        string sStoreTaxState = "";
        string CustTaxState = "";
        string PrincplePlaceOfBuss = "";
        string PlaceOfSupply = "";
        string CustOrdAndDate = "";
        string RefNoAndDate = "";
        string sCustGST = "";
        string sCustDelAddress = "";
        string sCustDelStateCode = "";
        string sCustDelGSTNO = "";
        string sCustomTransType = string.Empty;
        RetailTransaction retailTrans;//= posTransaction as RetailTransaction;
        string sUTLabel = "";
        string sIGSTCalCode = "";
        string sGovtIdNo1 = "";
        string sGovtIdNo2 = "";
        string sGovtId1Type = "";
        string sGovtId2Type = "";
        decimal dTotOgGrossWt = 0;
        decimal dTotSalableWt = 0;
        decimal dTotSalableAmt = 0;
        int iOWNDMD = 0; int iOWNOG = 0; int iOTHERDMD = 0; int iOTHEROG = 0;
        string sFooterMOPName = "";
        string sFooterMOPValue = "";
        string sCustLoyaltyNo = "";
        string sGVNo = "";
        decimal dGVAmt = 0m;
        int isFromShowJournal = 0;
        string sLineCertNo = "";
        //=======Soutik==========
        string pSalesManName = "";
        decimal pCurrentRate = 0m;

        private enum IdentityProof
        {
            Aadhar = 0,
            PAN = 1,
            Driving_License = 2,
            Passport_No = 3,
            Voter_Id = 4,
            Emp_Id = 5,
        }

        public frmGSTSalesInv()
        {
            InitializeComponent();
        }

        public frmGSTSalesInv(string sTransactionId, SqlConnection conn)
        {
            InitializeComponent();

            sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }


        public frmGSTSalesInv(SqlConnection conn)
        {
            InitializeComponent();

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        public frmGSTSalesInv(IPosTransaction posTransaction, SqlConnection conn, bool bDuplicate, int iCN = 0, string sCNNo = "", int iPrintFromShowJournal = 0)
        {
            InitializeComponent();

            retailTrans = posTransaction as RetailTransaction;
            #region[Param Info]


            int IsReturnSale = 0;

            if (retailTrans != null)
            {
                sTerminal = retailTrans.TerminalId;
                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)  //PrimaryAddress
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                //if (Convert.ToString(retailTrans.Customer.MobilePhone) != string.Empty)
                // sContactNo = Convert.ToString(retailTrans.Customer.MobilePhone);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);

                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);

                // string sCustShippingAddress = Convert.ToString(retailTrans.Customer.); ;

                if (sContactNo == "-")
                {
                    sContactNo = GetCustomerMobilePrimary(retailTrans.Customer.CustomerId);// "select retailmobileprimary from custtable";
                }


                //sIGSTCalCode = GetIGSTCalStateCode(retailTrans.TransactionId, retailTrans.TerminalId, retailTrans.Customer.CustomerId);

                //if (!string.IsNullOrEmpty(sIGSTCalCode))
                //{
                //    sCustDelAddress = GetValueBySqlQry("select top 1 DeliveryAddress from CustDeliveryAddressMaster where CustId='" + retailTrans.Customer.CustomerId + "'");
                //    sCustDelStateCode = GetValueBySqlQry("select top 1 DeliveryStateCode from CustDeliveryAddressMaster where CustId='" + retailTrans.Customer.CustomerId + "'");
                //    sCustDelGSTNO = GetValueBySqlQry("select top 1 DeliveryGSTNo from CustDeliveryAddressMaster where CustId='" + retailTrans.Customer.CustomerId + "'");
                //}

                //if (string.IsNullOrEmpty(retailTrans.Customer.Name))
                //{
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName)))
                {
                    sCustName = Convert.ToString(retailTrans.PartnerData.LCCustomerName);
                    //sCustAddress = Convert.ToString(retailTrans.PartnerData.LCCustomerAddress);
                    //sContactNo = Convert.ToString(retailTrans.PartnerData.LCCustomerContactNo);
                    //sCPinCode = "-";
                }
                //else //  added on 12/04/2014 for print local cust later ( from show journal)
                //{
                //    GetLocalCustomerInfo(sInvoiceNo);
                //}
                //}
                //-------

                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.EndDateTime.ToString("dd-MM-yyyy");

                if (retailTrans.EndDateTime != null)
                    sTime = retailTrans.EndDateTime.ToShortTimeString(); //("HH:mm")


                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                    sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber) != string.Empty)
                    sGSTNo = Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber);

                //if (!string.IsNullOrEmpty(retailTrans.Customer.State))
                //    CustTaxState = GetStateCodeNameById(Convert.ToString(retailTrans.Customer.State));

                //PrincplePlaceOfBuss = GetPPOBAddress(sStoreTaxState);
                //PlaceOfSupply = GetStateCodeNameById(sStoreTaxState) + " , " + GetStateNameById(sStoreTaxState);

                //  sStoreTaxState = GetStateCodeNameById(sStoreTaxState);

                // Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState)
                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                isCreditNote = iCN;

                if (Convert.ToString(retailTrans.InventLocationId) != string.Empty)
                    sInventLocationId = Convert.ToString(retailTrans.InventLocationId);
                if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.Customer.CustomerId)))
                    sCustCode = Convert.ToString(retailTrans.Customer.CustomerId);
                if (Convert.ToString(retailTrans.ReceiptId) != string.Empty)
                    sReceiptNo = Convert.ToString(retailTrans.ReceiptId);

                sCustomTransType = string.Empty; //will open RHossain 

                sCustomTransType = GetCustomTransType(sInvoiceNo, retailTrans.TerminalId);
                sCustGST = GetGSTNoByCustId(sCustCode);

                GetCustomerIdentityDetails(sCustCode);
                sCustLoyaltyNo = GetLoyaltyCardNoByCustAcc(sCustCode);

                if (string.IsNullOrEmpty(sCustGST))
                    sCustGST = "Unregistered";

                //sCPanNo = GetPANofCustomer(Convert.ToString(retailTrans.TransactionId), retailTrans.TerminalId);
                //sCompPAN = GetPANofCompany();

                // sRcardNo = GetRCardNo(retailTrans.TransactionId, retailTrans.TerminalId);

                string sIssueGV = "";

                if (retailTrans != null)
                {
                    foreach (var item in retailTrans.SaleItems)
                    {
                        if (item.Description == "Issue gift card")
                        {
                            sIssueGV = "Issue gift card";

                            sGVNo = item.Comment;
                            dGVAmt = item.NetAmount;
                            break;
                        }
                    }
                }


                if (retailTrans.SaleIsReturnSale)
                {
                    sCreditNoteNo = sCNNo;
                    sTitle = "SALES RETURN  " + " - " + sCreditNoteNo + "   " + "( " + sReceiptNo + " )";
                    IsReturnSale = 1;
                    RefNoAndDate = "Original Tax Invoice No and Date";
                    CustOrdAndDate = GetOriginalTaxInvAndDate(retailTrans.TransactionId);
                }
                else if (sCustomTransType == "1" || sCustomTransType == "2" || sCustomTransType == "3" || sCustomTransType == "4")
                {
                    sTitle = "URD Purchase";
                    sExcNo = "";
                    sCTHNo = "";
                    sCreditNoteNo = "";
                }
                else if (sIssueGV == "Issue gift card")
                {
                    sTitle = "GIFT VOUCHER";
                }
                else
                {
                    sTitle = "TAX INVOICE";

                    RefNoAndDate = "Customer Order";// Ref. and Date
                    CustOrdAndDate = GetOrderNoAndDate(retailTrans.TransactionId);
                    sCreditNoteNo = "";
                }
            }
            #endregion
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo);


            //sInvoiceFooter = GetGSTInvoiceFooter();

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            if (bDuplicate)
                sDuplicateCopy = ""; // "DUPLICATE"; It will open later RHossain 21/04/2014

            isFromShowJournal = iPrintFromShowJournal;

        }

        private void frmGSTSalesInv_Load(object sender, EventArgs e)
        {
            //// TODO: This line of code loads data into the 'DSSaleInv.Detail' table. You can move, or remove it, as needed.
            //this.DetailTableAdapter.Fill(this.DSSaleInv.Detail);
            //// TODO: This line of code loads data into the 'DSSaleInv.SubTotal' table. You can move, or remove it, as needed.
            //this.SubTotalTableAdapter.Fill(this.DSSaleInv.SubTotal);
            //// TODO: This line of code loads data into the 'DSSaleInv.Tender' table. You can move, or remove it, as needed.
            //this.TenderTableAdapter.Fill(this.DSSaleInv.Tender);
            //// TODO: This line of code loads data into the 'DSSaleInv.StdRate' table. You can move, or remove it, as needed.
            //this.StdRateTableAdapter.Fill(this.DSSaleInv.StdRate);
            //// TODO: This line of code loads data into the 'DSSaleInv.TaxInfo' table. You can move, or remove it, as needed.
            //this.TaxInfoTableAdapter.Fill(this.DSSaleInv.TaxInfo);
            //string sUrl = GetURL();

            //int isUT = IsUnionTerriroryStore();
            //if (isUT == 1)
            //    sUTLabel = "UGST";
            //else
            //    sUTLabel = "State Tax";

            // DataTable dtQRCode = GetBarcodeInfo(sUrl);

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "GSTRptSaleInv.rdlc";

            //if (!string.IsNullOrEmpty(sIGSTCalCode))
            //{
            //    localReport.ReportPath = "igstGSTRptSaleInv.rdlc";
            //}

            // localReport.ReportPath = "SPCASE_GSTRptSaleInv.rdlc";
            DataTable dtSalesData = new DataTable();
            DataSet dsSubTotal = new DataSet();
            DataSet dsTender = new DataSet();
            DataTable dtTender = new DataTable();
            DataSet dsStdRate = new DataSet();
            DataTable dtTaxInfo = new DataTable();
            DataSet dsPaymentInfo = new DataSet();

            DataSet dsTaxDetail = new DataSet(); // added on 31/03/2014 RHossain

            GetSalesInvData(ref dtSalesData);
            GetSubTotal(ref dsSubTotal);
            GetTender(ref dtTender);
            GetStdRate(ref dsStdRate);
            GetTaxInfo(ref dtTaxInfo);
            GetPayInfo(ref dsPaymentInfo);

            //GetTaxDetail(ref dsTaxDetail); // added on 31/03/2014 RHossain

            sAmtinwds = Amtinwds(Math.Abs(dTotAmt)); // added on 28/04/2014 RHossain               

            if (dtTaxInfo != null && dtTaxInfo.Rows.Count > 0)
            {
                if (sTitle == "URD Purchase")
                {
                    sTitle = "TAX INVOICE";
                }
            }

            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "Detail";
            dsSalesOrder.Value = dtSalesData;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtSalesData));


            ReportDataSource RDSubTotal = new ReportDataSource();
            //dsSalesOrder.Name = "DataSet1";
            RDSubTotal.Name = "SubTotal";
            RDSubTotal.Value = dsSubTotal.Tables[0];
            //   localReport.DataSources.Add(dsSalesOrder);
            //  this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SubTotal", dsSubTotal.Tables[0]));


            ReportDataSource RDTender = new ReportDataSource();
            //dsSalesOrder.Name = "DataSet1";
            RDTender.Name = "Tender";
            RDTender.Value = dtTender;
            //   localReport.DataSources.Add(dsSalesOrder);
            //  this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dtTender));

            ReportDataSource RDStdRate = new ReportDataSource();
            RDStdRate.Name = "StdRate";
            RDStdRate.Value = dsStdRate.Tables[0];
            //   localReport.DataSources.Add(dsSalesOrder);
            //  this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("StdRate", dsStdRate.Tables[0]));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "GSTTAX";
            RDTaxInfo.Value = dtTaxInfo;
            //   localReport.DataSources.Add(dsSalesOrder);
            //  this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("GSTTAX", dtTaxInfo));

            ReportDataSource RDPaymentInfo = new ReportDataSource();
            //dsSalesOrder.Name = "DataSet1";
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dsPaymentInfo.Tables[0];
            //   localReport.DataSources.Add(dsSalesOrder);
            //  this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dsPaymentInfo.Tables[0]));

            //this.reportViewer1.LocalReport.EnableExternalImages = true;
            //this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("QRCode", dtQRCode));

            //ReportDataSource RDTaxInDetail = new ReportDataSource();
            //RDTaxInDetail.Name = "TaxInDetails";
            //RDTaxInDetail.Value = dsTaxDetail.Tables[0];
            //this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("TaxInDetails", dsTaxDetail.Tables[0]));

            ReportParameter[] param = new ReportParameter[50];

            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
            param[4] = new ReportParameter("CPanNo", sCPanNo);
            param[5] = new ReportParameter("InvoiceNo", sInvoiceNo);
            param[6] = new ReportParameter("InvDate", sInvDt);

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
            param[18] = new ReportParameter("DuplicateCopy", sDuplicateCopy); // added on 14/04/214 req from Sailendra Da
            if (!string.IsNullOrEmpty(sEMIEmpCode))
                param[19] = new ReportParameter("RCARDNO", sRcardNo + " EMI  Emp code :" + sEMIEmpCode + " and Order no :" + sEMIOrderNo); // added on 14/04/214 req from Sailendra Da
            else
                param[19] = new ReportParameter("RCARDNO", sRcardNo);

            param[20] = new ReportParameter("CompPAN", sCompPAN);
            param[21] = new ReportParameter("ExcNo", sExcNo);
            param[22] = new ReportParameter("CTHNo", sCTHNo);
            param[23] = new ReportParameter("GSTNo", sGSTNo);
            param[24] = new ReportParameter("StoreTaxState", sStoreTaxState);
            param[25] = new ReportParameter("CustTaxState", CustTaxState);
            param[26] = new ReportParameter("PrincplePlaceOfBuss", PrincplePlaceOfBuss);
            param[27] = new ReportParameter("PlaceOfSupply", PlaceOfSupply);
            param[28] = new ReportParameter("RefNoAndDate", RefNoAndDate);
            param[29] = new ReportParameter("CustOrdAndDate", CustOrdAndDate);
            param[30] = new ReportParameter("CustomerGSTNo", sCustGST);
            param[31] = new ReportParameter("CustomTransType", sCustomTransType);
            param[32] = new ReportParameter("UTLabel", sUTLabel);
            param[33] = new ReportParameter("DelAddress", sCustDelAddress);
            param[34] = new ReportParameter("DelStateCode", sCustDelStateCode);
            param[35] = new ReportParameter("DelGSTNO", sCustDelGSTNO);

            param[36] = new ReportParameter("capGovtId1", sGovtId1Type);
            param[37] = new ReportParameter("valGovtId1", sGovtIdNo1);
            param[38] = new ReportParameter("capGovtId2", sGovtId2Type);
            param[39] = new ReportParameter("valGovtId2", sGovtIdNo2);
            param[40] = new ReportParameter("totSalableWt", Convert.ToString(dTotSalableWt));
            param[41] = new ReportParameter("totPayableAmt", Convert.ToString(dTotSalableAmt));
            param[42] = new ReportParameter("totOGGrossWt", Convert.ToString(dTotOgGrossWt));
            param[43] = new ReportParameter("FooterMOPDetails", sFooterMOPName);
            param[44] = new ReportParameter("FooterMOPValues", sFooterMOPValue);
            param[45] = new ReportParameter("CustLoyaltyNo", sCustLoyaltyNo);
            param[46] = new ReportParameter("LineCertNo", sLineCertNo);
            param[47] = new ReportParameter("ORGDATE", "");
            //===============Soutik
            pCurrentRate = Convert.ToDecimal(GetMetalRate());
            param[48] = new ReportParameter("MetalRate", Convert.ToString(pCurrentRate)+"/22kt");
            pSalesManName =  GetSalesManName(sInvoiceNo, sTerminal);
            param[49] = new ReportParameter("SM", pSalesManName);
            //==================================================================================
           


            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();

            //using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Auto print.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //{
            //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
            //    {
            //        oBlank.Export(reportViewer1.LocalReport);
            //        oBlank.Print_Invoice(reportViewer1.LocalReport,1);
            //    }
            //}
            //if (isFromShowJournal == 0)
            //{
            //    oBlank.Export(reportViewer1.LocalReport);
            //    oBlank.Print_Invoice(reportViewer1.LocalReport, 1);
            //    this.Close();
            //}
        }

        //private DataTable GetBarcodeInfo(string sURL)
        //{
        //    Byte[] bitmap01 = null;
        //    bitmap01 = Code128Rendering.GetQrBarcode(sURL);

        //    DataTable dtBarcode = new DataTable();
        //    dtBarcode.Columns.Add("ID", typeof(int));
        //    dtBarcode.Columns.Add("URLCODE", typeof(byte[]));
        //    DataRow dr = dtBarcode.NewRow();
        //    dr["ID"] = 1;
        //    dr["URLCODE"] = bitmap01;

        //    dtBarcode.Rows.Add(dr);

        //    return dtBarcode;
        //}
       //================================================soutik

        private string GetSalesManName(string sTransId, string sTerminal)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select top 1 d.NAME as Name from RETAILTRANSACTIONSALESTRANS A" +
                        " LEFT JOIN dbo.RETAILSTAFFTABLE AS T11 on A.STAFF =T11.STAFFID " +
                        " LEFT JOIN dbo.HCMWORKER AS T22 ON T11.STAFFID = T22.PERSONNELNUMBER" +
                        " left join dbo.DIRPARTYTABLE as d on d.RECID = T22.PERSON " +
                        " Where TRANSACTIONID='" + sTransId + "' and a.TERMINALID='" + sTerminal + "' and isnull(A.STAFF,'')!=''";

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
       //=========================================================================================
        private int ISGSV(string sGSSNo)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT isnull(SCHEMETYPE,0) SCHEMETYPE from GSSACCOUNTOPENINGPOSTED where  GSSACCOUNTNO ='" + sGSSNo + "' and  SCHEMEDEPOSITTYPE =0";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            int iResult = Convert.ToInt16(command.ExecuteScalar());

            return iResult;
        }

        private void GetSalesInvData(ref DataTable dtSalesOrder)
        {
            string sQuery = "GSTGetSalesInvData";//
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            // command.Parameters.Add("@Store", SqlDbType.NVarChar).Value = sStoreNo;
            command.Parameters.Add("@Terminal", SqlDbType.NVarChar).Value = retailTrans.TerminalId;
            command.Parameters.Add("@TransactionId", SqlDbType.NVarChar).Value = retailTrans.TransactionId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtSalesOrder);

            if (dtSalesOrder.Rows.Count > 0)
            {
                for (int i = 0; i <= dtSalesOrder.Rows.Count - 1; i++)
                {
                    dTotAmt = dTotAmt + Convert.ToDouble(dtSalesOrder.Rows[i]["AMOUNT"]);
                    //CertificationNo
                    if (string.IsNullOrEmpty(sLineCertNo))
                        sLineCertNo = Convert.ToString(dtSalesOrder.Rows[i]["CertificationNo"]);
                    else
                        sLineCertNo = sLineCertNo + "," + Convert.ToString(dtSalesOrder.Rows[i]["CertificationNo"]);

                    GetItemType(Convert.ToString(dtSalesOrder.Rows[i]["SKUNUMBER"]));
                    if (iOWNDMD == 1 || iOWNOG == 1 || iOTHERDMD == 1 || iOTHEROG == 1)
                    {
                        dTotOgGrossWt += Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["GROSSWT"]));
                    }
                    else
                    {
                        dTotSalableAmt += Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["AMOUNT"])) + Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["TAX"]));
                        dTotSalableWt += Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["GROSSWT"]));
                    }
                }
            }
            else
            {

                string sQuery1 = "GSTGetSalesOrderInvData";//added on 080319
                SqlCommand command1 = new SqlCommand(sQuery1, connection);
                command1.CommandType = CommandType.StoredProcedure;
                command1.CommandTimeout = 0;
                command1.Parameters.Clear();
                command1.Parameters.Add("@Terminal", SqlDbType.NVarChar).Value = retailTrans.TerminalId;
                command1.Parameters.Add("@TransactionId", SqlDbType.NVarChar).Value = retailTrans.TransactionId;
                command1.Parameters.Add("@GVNo", SqlDbType.NVarChar).Value = sGVNo;
                command1.Parameters.Add("@GVAmt", SqlDbType.Decimal).Value = dGVAmt;

                command1.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
                SqlDataAdapter daTrans1 = new SqlDataAdapter(command1);
                daTrans1.Fill(dtSalesOrder);

            }


        }

        private void GetItemType(string item) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
        {
            SqlConnection conn = new SqlConnection();
            try
            {
                conn = ApplicationSettings.Database.LocalConnection;

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";


                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iOWNDMD = Convert.ToInt16(reader.GetValue(0));
                        iOWNOG = Convert.ToInt16(reader.GetValue(1));
                        iOTHERDMD = Convert.ToInt16(reader.GetValue(2));
                        iOTHEROG = Convert.ToInt16(reader.GetValue(3));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
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

        private void GetTender(ref DataTable dtTender)
        {
            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sGSVNo = "";

            //if (Convert.ToString(retailTrans.PartnerData.GSSTotAmt) != "0")
            //{
            //    sGSVNo = Convert.ToString(retailTrans.PartnerData.GSSMaturityNo);
            //}

            //ISGSV
            string sQuery = "";
            //if (ISGSV(sGSVNo) == 1)
            sQuery = "GetInvoiceTender";
            //else
            //    sQuery = "GetInvoiceTenderGSS";


            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@Store", SqlDbType.NVarChar).Value = sStoreNo;
            command.Parameters.Add("@Terminal", SqlDbType.NVarChar).Value = retailTrans.TerminalId;
            command.Parameters.Add("@TransactionId", SqlDbType.NVarChar).Value = retailTrans.TransactionId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTender);

            for (int i = 0; i <= dtTender.Rows.Count - 1; i++)
            {
                if (dTotAmt == 0)
                    dTotAmt = Convert.ToDouble(dtTender.Rows[i]["AMOUNT"]);

                if (string.IsNullOrEmpty(sFooterMOPName))
                    sFooterMOPName = Convert.ToString(dtTender.Rows[i]["NAME"]);
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dtTender.Rows[i]["NAME"])))
                        sFooterMOPName += " , " + Convert.ToString(dtTender.Rows[i]["NAME"]);
                }

                if (string.IsNullOrEmpty(sFooterMOPValue))
                    sFooterMOPValue = Convert.ToString(Convert.ToDecimal(dtTender.Rows[i]["AMOUNT"]));
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dtTender.Rows[i]["AMOUNT"])))
                        sFooterMOPValue += " , " + Convert.ToString(Convert.ToDecimal(dtTender.Rows[i]["AMOUNT"]));
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

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter daSTotal = new SqlDataAdapter(command);

            daSTotal.Fill(dsSStdRate, "STDRate");
        }

        private void GetTaxInfo(ref DataTable dtSTaxInfo)
        {

            DataTable tblTaxInfo = new DataTable("GSTTAX");
            DataTable dtCategoryWiseGroup = new DataTable();

            DataTable dtCategoryWiseGroupForWtRange = new DataTable();

            if (retailTrans != null)
            {
                tblTaxInfo.Columns.Add("HSNCODE", typeof(string));
                tblTaxInfo.Columns.Add("CTAXCODE", typeof(string));
                tblTaxInfo.Columns.Add("CTRATE", typeof(decimal));
                tblTaxInfo.Columns.Add("CTAMT", typeof(decimal));
                tblTaxInfo.Columns.Add("STAXCODE", typeof(string));
                tblTaxInfo.Columns.Add("STRATE", typeof(decimal));
                tblTaxInfo.Columns.Add("STAMT", typeof(decimal));
                tblTaxInfo.Columns.Add("ITAXCODE", typeof(string));
                tblTaxInfo.Columns.Add("ITRATE", typeof(decimal));
                tblTaxInfo.Columns.Add("ITAMT", typeof(decimal));
                //==================Soutik======================
                tblTaxInfo.Columns.Add("CESSCODE", typeof(string));
                tblTaxInfo.Columns.Add("CESSRATE", typeof(decimal));
                //==============================================
                tblTaxInfo.Columns.Add("CESS", typeof(decimal));
                tblTaxInfo.Columns.Add("TOTTAX", typeof(decimal));
            }

            string sPreItem = "";
            int iLine = 0;
            foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
            {
                //if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                //{
                //if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                //{
                if (!saleLineItem.Voided && saleLineItem.Description != "Issue gift card" && retailTrans.SalesInvoiceAmounts == 0)
                {
                    if (saleLineItem.PartnerData.TransactionType == "0")
                    {
                        string HSNCODE = "";
                        string CTAXCODE = "";
                        decimal CTRATE = 0m;
                        decimal CTAMT = 0m;
                        string STAXCODE = "";
                        decimal STRATE = 0m;
                        decimal STAMT = 0m;
                        string ITAXCODE = "";
                        decimal ITRATE = 0m;
                        decimal ITAMT = 0m;
                        //=========Soutik
                        string CESSCODE = "";
                        decimal CESSRATE = 0m;
                        //==================
                        decimal CESS = 0m;

                        HSNCODE = saleLineItem.HSNCode;

                        if (string.IsNullOrEmpty(HSNCODE))
                        {
                            HSNCODE = saleLineItem.ServiceAccountingCode;
                        }


                        //LSRetailPosis.Transaction.Line.TaxItems.TaxItemGTE objnn=new LSRetailPosis.Transaction.Line.TaxItems.TaxItemGTE();
                        int i = 0;


                        foreach (LSRetailPosis.Transaction.Line.TaxItems.TaxItemGTE taxLine in saleLineItem.TaxLines)
                        {
                            if (taxLine.Percentage != decimal.Zero)
                            {
                                //CTAXCODE = taxLine.TaxComponent;
                                //CTRATE = taxLine.Percentage;
                                //CTAMT = taxLine.Amount;
                                CESS = 0;

                                if (tblTaxInfo != null && tblTaxInfo.Rows.Count > 0 && sPreItem == saleLineItem.ItemId && iLine == saleLineItem.LineId)
                                {
                                    if (taxLine.TaxComponent == "SGST")
                                    {
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["STAXCODE"] = taxLine.TaxComponent;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["STRATE"] = taxLine.Percentage;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["STAMT"] = taxLine.Amount;
                                        tblTaxInfo.AcceptChanges();
                                    }
                                    if (taxLine.TaxComponent == "IGST")
                                    {
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["ITAXCODE"] = taxLine.TaxComponent;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["ITRATE"] = taxLine.Percentage;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["ITAMT"] = taxLine.Amount;
                                        tblTaxInfo.AcceptChanges();
                                    }
                                    if (taxLine.TaxComponent == "CGST")
                                    {
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["CTAXCODE"] = taxLine.TaxComponent;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["CTRATE"] = taxLine.Percentage;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["CTAMT"] = taxLine.Amount;
                                        tblTaxInfo.AcceptChanges();
                                    }
                                    //=======================Soutik
                                    if (taxLine.TaxComponent == "CESS")
                                    {
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["CESSCODE"] = taxLine.TaxComponent;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["CESSRATE"] = taxLine.Percentage;
                                        tblTaxInfo.Rows[tblTaxInfo.Rows.Count - 1]["CESS"] = taxLine.Amount;
                                        tblTaxInfo.AcceptChanges();
                                    }
                                    //====================================
                                }
                                else
                                {
                                    if (taxLine.TaxComponent == "SGST")
                                    {
                                        //tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, ITAXCODE, ITRATE, ITAMT, CESS);
                                        tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, ITAXCODE, ITRATE, ITAMT, CESSCODE, CESSRATE, CESS);
                                    }
                                    if (taxLine.TaxComponent == "IGST")
                                    {
                                        //tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, STAXCODE, STRATE, STAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, CESS);
                                        tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, STAXCODE, STRATE, STAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, CESSCODE, CESSRATE, CESS);
                                    }
                                    if (taxLine.TaxComponent == "CGST")
                                    {
                                        //tblTaxInfo.Rows.Add(HSNCODE, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, STAXCODE, STRATE, STAMT, ITAXCODE, ITRATE, ITAMT, CESS);
                                        tblTaxInfo.Rows.Add(HSNCODE, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, STAXCODE, STRATE, STAMT, ITAXCODE, ITRATE, ITAMT, CESSCODE, CESSRATE, CESS);
                                    }
                                    //======================Soutik
                                    if (taxLine.TaxComponent == "CESS")
                                    {
                                        tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, STAXCODE, STRATE, STAMT, ITAXCODE, ITRATE, ITAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount);
                                    }
                                    //============================

                                }
                                sPreItem = saleLineItem.ItemId;
                                iLine = saleLineItem.LineId;
                                i++;
                            }
                        }
                    }

                }
            }
            // }
            //}

            Dictionary<string, decimal> dicSum = new Dictionary<string, decimal>();
            foreach (DataRow row in tblTaxInfo.Rows)
            {
                string group = row["HSNCODE"].ToString();
                string CTAXCODE = row["CTAXCODE"].ToString();
                decimal CTRATE = Math.Abs(Convert.ToDecimal(row["CTRATE"]));
                decimal CTAMT = Convert.ToDecimal(row["CTAMT"]);
                string STAXCODE = row["STAXCODE"].ToString();
                decimal STRATE = Math.Abs(Convert.ToDecimal(row["STRATE"]));
                decimal STAMT = Convert.ToDecimal(row["STAMT"]);
                string ITAXCODE = row["ITAXCODE"].ToString();
                decimal ITRATE = Math.Abs(Convert.ToDecimal(row["ITRATE"]));
                decimal ITAMT = Convert.ToDecimal(row["ITAMT"]);
                //======================Soutik
                string CESSCODE = row["CESSCODE"].ToString();
                decimal CESSRATE = Math.Abs(Convert.ToDecimal(row["CESSRATE"]));
                //========================================================
                decimal CESS = Math.Abs(Convert.ToDecimal(row["CESS"]));

                //if (dicSum.ContainsKey(group))
                //{
                //    dicSum[group] += TAMT;
                //}
                //else
                //{
                //    dicSum.Add(group, TAMT);

                //}
            }

            var invoiceSum =
                        tblTaxInfo.AsEnumerable()
                        .Select(x =>
                            new
                            {
                                HSNCODE = x["HSNCODE"],
                                CTAXCODE = x["CTAXCODE"],
                                CTRATE = x["CTRATE"],
                                CTAMT = x["CTAMT"],

                                STAXCODE = x["STAXCODE"],
                                STRATE = x["STRATE"],
                                STAMT = x["STAMT"],

                                ITAXCODE = x["ITAXCODE"],
                                ITRATE = x["ITRATE"],
                                ITAMT = x["ITAMT"],

                                //===========Soutik
                                CESSCODE = x["CESSCODE"],
                                CESSRATE = x["CESSRATE"],
                                //==================
                                CESS = x["CESS"],
                            }
                         )
                         .GroupBy(s => new { s.HSNCODE, s.CTAXCODE, s.STAXCODE, s.ITAXCODE,s.CESSCODE, s.CTRATE, s.STRATE, s.ITRATE,s.CESSRATE })
                         .Select(g =>
                                new
                                {
                                    HSNCODE = g.Key.HSNCODE,
                                    CTAXCODE = g.Key.CTAXCODE,
                                    CTRATE = g.Key.CTRATE,
                                    CTAMT = g.Sum(x => Math.Round(Convert.ToDecimal(x.CTAMT), 2)),
                                    STAXCODE = g.Key.STAXCODE,
                                    STRATE = g.Key.STRATE,
                                    STAMT = g.Sum(x => Math.Round(Convert.ToDecimal(x.STAMT), 2)),
                                    ITAXCODE = g.Key.ITAXCODE,
                                    ITRATE = g.Key.ITRATE,
                                    ITAMT = g.Sum(x => Math.Round(Convert.ToDecimal(x.ITAMT), 2)),
                                    //===================Soutik
                                    CESSCODE = g.Key.CESSCODE,
                                    CESSRATE = g.Key.CESSRATE,
                                    ////=========================
                                    CESS = g.Sum(x => Math.Round(Convert.ToDecimal(x.CESS), 2)),
                                }
                         );

            DataTable dtGroupTotal = new DataTable();

            dtGroupTotal.Columns.Add("HSNCODE", typeof(string));
            dtGroupTotal.Columns.Add("CTAXCODE", typeof(string));
            dtGroupTotal.Columns.Add("CTRATE", typeof(decimal));
            dtGroupTotal.Columns.Add("CTAMT", typeof(decimal));

            dtGroupTotal.Columns.Add("STAXCODE", typeof(string));
            dtGroupTotal.Columns.Add("STRATE", typeof(decimal));
            dtGroupTotal.Columns.Add("STAMT", typeof(decimal));

            dtGroupTotal.Columns.Add("ITAXCODE", typeof(string));
            dtGroupTotal.Columns.Add("ITRATE", typeof(decimal));
            dtGroupTotal.Columns.Add("ITAMT", typeof(decimal));

            //Soutik==========================================
            dtGroupTotal.Columns.Add("CESSCODE", typeof(string));
            dtGroupTotal.Columns.Add("CESSRATE", typeof(decimal));
            //===================================================
            dtGroupTotal.Columns.Add("CESS", typeof(decimal));

            foreach (var element in invoiceSum)
            {
                var row = dtGroupTotal.NewRow();
                row["HSNCODE"] = element.HSNCODE;
                row["CTAXCODE"] = element.CTAXCODE;
                row["CTRATE"] = element.CTRATE;
                row["CTAMT"] = element.CTAMT;

                row["STAXCODE"] = element.STAXCODE;
                row["STRATE"] = element.STRATE;
                row["STAMT"] = element.STAMT;

                row["ITAXCODE"] = element.ITAXCODE;
                row["ITRATE"] = element.ITRATE;
                row["ITAMT"] = element.ITAMT;

                //===============Soutik
                row["CESSCODE"] = element.CESSCODE;
                row["CESSRATE"] = element.CESSRATE;
                //==============================
                row["CESS"] = element.CESS;
                dtGroupTotal.Rows.Add(row);
            }

            dtSTaxInfo = dtGroupTotal;

            for (int i = 0; i <= dtSTaxInfo.Rows.Count - 1; i++)
            {
                //dTotAmt = dTotAmt + Convert.ToDouble(dtSTaxInfo.Rows[i]["CTAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["STAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["ITAMT"]);
                //===============================================Soutik===========================================================================================================
                dTotAmt = dTotAmt + Convert.ToDouble(dtSTaxInfo.Rows[i]["CTAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["STAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["ITAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["CESS"]);
                //=================================================================================================================================================================
            }

        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        private void GetPayInfo(ref DataSet dsSPaymentInfo)
        {
            DataSet dsTemp = new DataSet();

            string sqlsubDtl = " SELECT (ISNULL(B.DESCRIPTION,'') + ' : '+ ISNULL(A.INFORMATION,'')) AS DTLPAYINFO, A.TYPE,A.INFOCODEID,A.PARENTLINENUM,A.TRANSACTIONID" +
                                " ,CAST(ISNULL(A.AMOUNT,0) AS DECIMAL(28,2)) AS AMOUNT" + //R.AMOUNTCUR ->  A.AMOUNT ADDED RHossain on 21/04/2014
                                " FROM [dbo].[RETAILTRANSACTIONINFOCODETRANS] A" +
                                " INNER JOIN	 RETAILINFOCODETABLE B ON A.INFOCODEID = B.INFOCODEID" +
                                " WHERE A.TRANSACTIONID = @TRANSACTIONID and B.PRINTINPUTONRECEIPT=1 AND A.TERMINAL = '" + sTerminal + "' ORDER BY A.PARENTLINENUM,A.INFOCODEID";

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
                y = y + "Paise ";
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

        private void GetStoreInfo(ref string sStorePh, ref string sInvFooter, ref string sCINNo)
        {
            string sql = " SELECT ISNULL(STORECONTACT,'-') AS STORECONTACT, ISNULL(INVOICEFOOTERNOTE,'') AS INVOICEFOOTERNOTE," +
                " ISNULL(CINNO,'') as CINNO FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'";

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
            }
        }

        private void GetTaxDetail(ref DataSet dsTaxDetail) // added on 31/03/2014 RHossain for  show the tax detail in line
        {

            string sqlStdRate = " DECLARE @TINCST AS NVARCHAR(20)  DECLARE @TINVAT AS NVARCHAR(20)  SELECT @TINCST = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'')" +
                               " FROM TAXREGISTRATIONNUMBERS_IN AS A, TAXINFORMATION_IN AS B, INVENTLOCATIONLOGISTICSLOCATION AS C," +
                               " INVENTLOCATION AS D,RETAILCHANNELTABLE AS E WHERE D.INVENTLOCATIONID=E.INVENTLOCATION AND C.INVENTLOCATION=D.RECID" +
                               " AND B.ISPRIMARY=1" +
                               " AND C.ISPRIMARY=1" +
                               " AND B.REGISTRATIONLOCATION=C.LOCATION AND B.SALESTAXREGISTRATIONNUMBER=A.RECID AND D.INVENTLOCATIONID = @INVENTLOCATIONID  " +
                               "  SELECT @TINVAT = ISNULL(A.REGISTRATIONNUMBER,'') + ISNULL(A.TTYPE,'') FROM TAXREGISTRATIONNUMBERS_IN AS A, TAXINFORMATION_IN AS B, INVENTLOCATIONLOGISTICSLOCATION AS C," +
                               " INVENTLOCATION AS D,RETAILCHANNELTABLE AS E WHERE D.INVENTLOCATIONID=E.INVENTLOCATION AND C.INVENTLOCATION=D.RECID" +
                               " AND B.ISPRIMARY=1" +
                               " AND C.ISPRIMARY=1" +
                               " AND B.REGISTRATIONLOCATION=C.LOCATION AND B.[GSTIN]=A.RECID" +
                               " AND D.INVENTLOCATIONID = @INVENTLOCATIONID  SELECT ISNULL(@TINCST,'') AS TINCST,ISNULL(@TINVAT,'') AS TINVAT";

            SqlCommand command = new SqlCommand(sqlStdRate, connection);
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar).Value = sInventLocationId;

            SqlDataAdapter daTax = new SqlDataAdapter(command);

            daTax.Fill(dsTaxDetail, "TaxInfo");

            if (dsTaxDetail.Tables[0] != null && dsTaxDetail.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToString(dsTaxDetail.Tables[0].Rows[0]["TINVAT"]) == string.Empty)
                    sGSTNo = "-";
                else
                    sGSTNo = Convert.ToString(dsTaxDetail.Tables[0].Rows[0]["TINVAT"]);
            }

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

        /// <summary>
        /// Created on 12/05/2014
        /// Purppose: After craeting sales invoice, print SalesInvoice from sjow journal with locul customer
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

        private string GetCustomerMobilePrimary(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT RETAILMOBILEPRIMARY FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustAcc + "'");

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

        private string GetIGSTCalStateCode(string sTransactionId, string sTerminalId, string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select top 1 IGSTCALSTATECODE from RETAILTRANSACTIONTABLE where ");
            commandText.Append(" TRANSACTIONID='" + sTransactionId + "' and CUSTACCOUNT='" + sCustAcc + "' and TERMINAL='" + sTerminalId + "'");

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
                return "";
            }
        }

        private string GetRCardNo(string stRansId, string sTerminalId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT  ISNULL(stuff(ISNULL(RCARDNO,''),1,LEN(ISNULL(RCARDNO,''))-4,REPLICATE('x', LEN(ISNULL(RCARDNO,''))-4)),'') AS RCARDNO from RETAILTRANSACTIONTABLE WHERE TRANSACTIONID='" + stRansId + "' and STORE='" + ApplicationSettings.Terminal.StoreId + "' and TERMINAL='" + sTerminalId + "'");

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

        private string GetPANofCustomer(string stRansId, string sTerminalId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string sResult = string.Empty;
            StringBuilder commandText = new StringBuilder();


            commandText.Append("SELECT  ISNULL(PANNO,'') as PANNO,ISNULL(FORM60,'') as FORM60, ");
            commandText.Append(" ISNULL(EMIEMPCODE,'') as EMIEMPCODE,ISNULL(EMIORDERNO,'') as EMIORDERNO ");
            commandText.Append("from RETAILTRANSACTIONTABLE WHERE TRANSACTIONID='" + stRansId + "' ");
            commandText.Append("and STORE='" + ApplicationSettings.Terminal.StoreId + "' and TERMINAL='" + sTerminalId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dtCustPAN = new DataTable();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtCustPAN);

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (dtCustPAN != null && dtCustPAN.Rows.Count > 0)
            {
                if (Convert.ToString(dtCustPAN.Rows[0]["PANNO"]) == string.Empty)
                    sResult = "-";
                else
                    sResult = Convert.ToString(dtCustPAN.Rows[0]["PANNO"]);
                if (Convert.ToString(dtCustPAN.Rows[0]["PANNO"]) == string.Empty)
                {
                    if (Convert.ToString(dtCustPAN.Rows[0]["FORM60"]) == string.Empty)
                        sResult = "-";
                    else
                        sResult = Convert.ToString(dtCustPAN.Rows[0]["FORM60"]);
                }

                if (!string.IsNullOrEmpty(Convert.ToString(dtCustPAN.Rows[0]["EMIEMPCODE"])))
                    sEMIEmpCode = Convert.ToString(dtCustPAN.Rows[0]["EMIEMPCODE"]);

                if (!string.IsNullOrEmpty(Convert.ToString(dtCustPAN.Rows[0]["EMIORDERNO"])))
                    sEMIOrderNo = Convert.ToString(dtCustPAN.Rows[0]["EMIORDERNO"]);
            }
            return sResult;
        }

        private string GetPANofCompany()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT TOP 1 ISNULL(PANNUMBER,'') as PANNO from TaxInformationLegalEntity_IN ");

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

        private string GetOriginalTaxInvAndDate(string sTransId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select RECEIPTID + ';  Date  '+ CONVERT(VARCHAR(10), TRANSDATE, 103) from RETAILTRANSACTIONTABLE where TRANSACTIONID");
            commandText.Append(" in (select RETURNTRANSACTIONID from RETAILTRANSACTIONSALESTRANS where TRANSACTIONID='" + sTransId + "')");

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

        private string GetGSTInvoiceFooter()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select top 1 isnull(RegisterAddress,'') as RegisterAddress from RETAILPARAMETERS");

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

        private string GetOrderNoAndDate(string sTransId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select CUSTOMERORDER + ';  Date  '+ CONVERT(VARCHAR(10), TRANSDATE, 103) from RETAILTRANSACTIONTABLE where TRANSACTIONID");
            commandText.Append(" in (select GSVGSSADVREF from RETAILTRANSACTIONSALESTRANS where TRANSACTIONID='" + sTransId + "' and isnull(GSVGSSADVREF,'')!='')");

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

        private string GetStateNameById(string sStateId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select top 1 name from LOGISTICSADDRESSSTATE where STATEID='" + sStateId + "'");

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

        private string GetLoyaltyCardNoByCustAcc(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT A.CARDNUMBER FROM  RETAILLOYALTYMSRCARDTABLE A");//,a.LOYALTYSCHEMEID,B.ACCOUNTNUM
            commandText.Append(" LEFT JOIN RETAILLOYALTYCUSTTABLE B");
            commandText.Append(" ON A.LOYALTYCUSTID=B.LOYALTYCUSTID ");
            commandText.Append(" WHERE");
            commandText.Append(" ACCOUNTNUM ='" + sCustAcc + "'");

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



        private string GetURL()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select top 1 URLCODE from retailparameters");

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

        private string GetStateCodeNameById(string sStateId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("select top 1 statecode_in from LOGISTICSADDRESSSTATE where STATEID='" + sStateId + "'");

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

        private string GetValueBySqlQry(string sQry)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            //StringBuilder commandText = new StringBuilder();

            //commandText.Append("select top 1 statecode_in from CustDeliveryAddressMaster where STATEID='" + sCustId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(sQry, conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private string GetGSTNoByCustId(string sCustId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            string sQry = " select top 1 TAXREGISTRATIONNUMBERS_IN.REGISTRATIONNUMBER" +
                         " from" +
                         " TAXREGISTRATIONNUMBERS_IN inner join TAXINFORMATION_IN" +
                         " on TAXREGISTRATIONNUMBERS_IN.RECID = TAXINFORMATION_IN.GSTIN" +
                         " inner join LOGISTICSLOCATION " +
                         " on TAXINFORMATION_IN.REGISTRATIONLOCATION = LOGISTICSLOCATION.RECID" +
                         " and TAXINFORMATION_IN.ISPRIMARY = 1" +
                         " inner join DIRPARTYLOCATION" +
                         " on DIRPARTYLOCATION.LOCATION = LOGISTICSLOCATION.RECID" +
                         " and DIRPARTYLOCATION.ISPRIMARY = 1" +
                         " inner join CUSTTABLE on CustTable.party = DIRPARTYLOCATION.PARTY" +
                         " and CUSTTABLE.ACCOUNTNUM = '" + sCustId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(sQry.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }

        private bool isSISStore()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT isnull(SISSTORE,0) FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "' ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        private int IsUnionTerriroryStore()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT isnull(UNIONTERRITORY,0) FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "' ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            int sResult = Convert.ToInt16(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        private void GetCustomerIdentityDetails(string sCustACc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string sResult = string.Empty;
            StringBuilder commandText = new StringBuilder();

            commandText.Append("select isnull(GOVTIDENTITY,0) GOVTIDENTITY,");
            commandText.Append("isnull(GOVTIDNO,'') GOVTIDNO,");
            commandText.Append("isnull(GOVTIDENTITY2,0) GOVTIDENTITY2,");
            commandText.Append("isnull(GOVTIDNO2,'') GOVTIDNO2");
            commandText.Append(" from CUSTTABLE WHERE ACCOUNTNUM='" + sCustACc + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dtCustInfo = new DataTable();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtCustInfo);

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (dtCustInfo != null && dtCustInfo.Rows.Count > 0)
            {
                int iGovtId1 = Convert.ToInt16(dtCustInfo.Rows[0]["GOVTIDENTITY"]);
                int iGovtId2 = Convert.ToInt16(dtCustInfo.Rows[0]["GOVTIDENTITY2"]);

                if (Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO"]) == string.Empty)
                    sGovtIdNo1 = "";
                else
                    sGovtIdNo1 = Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO"]);

                if (Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO2"]) == string.Empty)
                    sGovtIdNo2 = "";
                else
                    sGovtIdNo2 = Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO2"]);

                if (!string.IsNullOrEmpty(sGovtIdNo1))
                {
                    if (iGovtId1 == (int)IdentityProof.Aadhar)
                        sGovtId1Type = "AADHAR No.";
                    else if (iGovtId1 == (int)IdentityProof.Driving_License)
                        sGovtId1Type = "Driving License";
                    else if (iGovtId1 == (int)IdentityProof.Emp_Id)
                        sGovtId1Type = "Emp. Id";
                    else if (iGovtId1 == (int)IdentityProof.PAN)
                        sGovtId1Type = "PAN No.";
                    else if (iGovtId1 == (int)IdentityProof.Passport_No)
                        sGovtId1Type = "Passport No";
                    else if (iGovtId1 == (int)IdentityProof.Voter_Id)
                        sGovtId1Type = "Voter Id";
                    else
                        sGovtId1Type = "PAN No.";
                }
                else
                    sGovtId1Type = "PAN No.";

                if (!string.IsNullOrEmpty(sGovtIdNo2))
                {
                    if (iGovtId2 == (int)IdentityProof.Aadhar)
                        sGovtId2Type = "AADHAR No.";
                    else if (iGovtId2 == (int)IdentityProof.Driving_License)
                        sGovtId2Type = "Driving License";
                    else if (iGovtId2 == (int)IdentityProof.Emp_Id)
                        sGovtId2Type = "Emp. Id";
                    else if (iGovtId2 == (int)IdentityProof.PAN)
                        sGovtId2Type = "PAN No.";
                    else if (iGovtId2 == (int)IdentityProof.Passport_No)
                        sGovtId2Type = "Passport No";
                    else if (iGovtId2 == (int)IdentityProof.Voter_Id)
                        sGovtId2Type = "Voter Id";
                    else
                        sGovtId2Type = "AADHAR No.";
                }
                else
                    sGovtId2Type = "AADHAR No.";
            }
        }
    }
}
