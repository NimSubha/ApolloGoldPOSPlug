
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

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmOfflineBillPrint : Form
    {
        BlankOperations oBlank = new BlankOperations();
        SqlConnection connection;
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

        string sInvoiceFooter = "-";
        string sTime = "";

        string sCompanyName = string.Empty;
        string sCINNo = string.Empty;
        string sDuplicateCopy = string.Empty;

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
        string sOfflineBill = "";
        string sLineCertNo = "";
        int iOffLineBillType = 0;
        string sORGDate = "";
        decimal dGMADISCPCT = 0m;

        DataTable dtSalesData = new DataTable();
        DataTable dtSubTotal = new DataTable();
        DataTable dtTender = new DataTable();
        DataTable dtStdRate = new DataTable();
        DataTable dtTaxInfo = new DataTable();
        DataSet dsPaymentInfo = new DataSet();

        DataSet dsTaxDetail = new DataSet();
        int iDiscVoucher = 0;
        int iOrderTrans = 0;

        enum OfflineTransactionType
        {
            None = 0,
            Sales = 1,
            Purchase = 2,
            Exchange = 3,
            PurchaseReturn = 4,
            ExchangeReturn = 5,
            Advance = 6,
            FPP = 7,
            GMA = 8,
        }

        private enum IdentityProof
        {
            Aadhar = 0,
            PAN = 1,
            Driving_License = 2,
            Passport_No = 3,
            Voter_Id = 4,
            Emp_Id = 5,
        }

        public frmOfflineBillPrint()
        {
            InitializeComponent();
        }

        public frmOfflineBillPrint(IPosTransaction posTransaction, string sOfflineBillNo, int iDVoucher = 0, int iFromOrder = 0)
        {
            InitializeComponent();

            retailTrans = posTransaction as RetailTransaction;
            #region[Param Info]

            sOfflineBill = sOfflineBillNo;
            iDiscVoucher = iDVoucher;
            iOrderTrans = iFromOrder;

            if (retailTrans != null)
            {
                sTerminal = retailTrans.TerminalId;
                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                if (Convert.ToString(retailTrans.Customer.Address) != string.Empty)
                    sCustAddress = Convert.ToString(retailTrans.Customer.Address);
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);

                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);


                if (sContactNo == "-")
                {
                    // sContactNo = GetCustomerMobilePrimary(retailTrans.Customer.CustomerId);
                }

                if (string.IsNullOrEmpty(retailTrans.Customer.Name))
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName)))
                    {
                        sCustName = Convert.ToString(retailTrans.PartnerData.LCCustomerName);
                        sCustAddress = Convert.ToString(retailTrans.PartnerData.LCCustomerAddress);
                        sContactNo = Convert.ToString(retailTrans.PartnerData.LCCustomerContactNo);
                        sCPinCode = "-";
                    }
                }

                if (retailTrans.EndDateTime != null)
                    sInvDt = retailTrans.EndDateTime.ToString("dd-MMM-yyyy");

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

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                sCustomTransType = string.Empty; //will open RHossain 

                //sCustomTransType = GetCustomTransType(sInvoiceNo, retailTrans.TerminalId);
                //sCustGST = GetGSTNoByCustId(sCustCode);

                //GetCustomerIdentityDetails(sCustCode);
                //sCustLoyaltyNo = GetLoyaltyCardNoByCustAcc(sCustCode);

                if (string.IsNullOrEmpty(sCustGST))
                    sCustGST = "Unregistered";

                if (sCustomTransType == "1" || sCustomTransType == "2" || sCustomTransType == "3" || sCustomTransType == "4")
                {
                    sTitle = "OG Purchase ";
                    sExcNo = "";
                    sCTHNo = "";
                    sCreditNoteNo = "";
                }
                else
                {
                    sTitle = "TAX INVOICE";

                    RefNoAndDate = "Customer Order";
                    // CustOrdAndDate = GetOrderNoAndDate(retailTrans.TransactionId);
                    sCreditNoteNo = "";
                }
            }
            else
            {
                string commandText = "";
                if (iOrderTrans == 0)
                {
                    commandText = "Select ReceiptId,CONVERT(VARCHAR(15),TransDate,103) AS TransDate ," +
                                    " isnull(CustAccount,'') CustAccount ,isnull(CustName,'') CustName ,isnull(CustMobile,'') CustMobile," +
                                    " isnull(TERMINALID,'') TERMINALID,isnull(CustAddress,'') CustAddress" +
                                    " ,isnull(CustPAN,'') CustPAN,isnull(CustAadhar,'') CustAadhar," +
                                    " isnull(CustLoyaltyNo,'') CustLoyaltyNo,isnull(CustGSTIN,'') CustGSTIN,isnull(RefOrdNo,'')" +
                                    " RefOrdNo,OffLineBillType,0 GMADISCPER FROM CRWOfflineBillHeader" +
                                    " Where ReceiptId ='" + sOfflineBill + "'";
                }
                else
                {
                    commandText = "Select ORDERNUM ReceiptId,CONVERT(VARCHAR(15), ORDERDATE,103) AS TransDate ," +
                                   " isnull(CustAccount,'') CustAccount ,isnull(CustName,'') CustName ,isnull(CUSTPHONE,'') CustMobile," +
                                   " isnull(TERMINALID,'') TERMINALID,isnull(CustAddress,'') CustAddress" +
                                   " ,'' CustPAN,'' CustAadhar," +
                                   " '' CustLoyaltyNo,'' CustGSTIN,''" +
                                   " RefOrdNo,8 OffLineBillType, isnull(GMADISCPER,0) GMADISCPER FROM CUSTORDER_HEADER" +
                                   " Where ORDERNUM = '" + sOfflineBill + "'";


                }

                connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                SqlCommand command = new SqlCommand(commandText, connection);

                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                DataTable dtBill = new DataTable();
                dtBill.Load(reader);
                if (dtBill != null && dtBill.Rows.Count > 0)
                {
                    sTerminal = Convert.ToString(dtBill.Rows[0]["TERMINALID"]);
                    sCustCode = Convert.ToString(dtBill.Rows[0]["CustAccount"]);
                    sCustName = Convert.ToString(dtBill.Rows[0]["CustName"]);
                    sCustAddress = Convert.ToString(dtBill.Rows[0]["CustAddress"]);
                    sContactNo = Convert.ToString(dtBill.Rows[0]["CustMobile"]);
                    sInvoiceNo = sOfflineBill;
                    sInvDt = Convert.ToDateTime(dtBill.Rows[0]["TransDate"]).ToString("dd-MMM-yyyy");
                    sTime = Convert.ToDateTime(dtBill.Rows[0]["TransDate"]).ToShortTimeString();

                    sGovtIdNo1 = Convert.ToString(dtBill.Rows[0]["CustPAN"]);
                    sGovtIdNo2 = Convert.ToString(dtBill.Rows[0]["CustAadhar"]);

                    sCustGST = Convert.ToString(dtBill.Rows[0]["CustGSTIN"]);
                    sCustLoyaltyNo = Convert.ToString(dtBill.Rows[0]["CustLoyaltyNo"]);
                    CustOrdAndDate = Convert.ToString(dtBill.Rows[0]["RefOrdNo"]);//
                    iOffLineBillType = Convert.ToInt16(dtBill.Rows[0]["OffLineBillType"]);//
                    dGMADISCPCT = Convert.ToDecimal(dtBill.Rows[0]["GMADISCPER"]);
                }


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

                sDataAreaId = Convert.ToString(ApplicationSettings.Database.DATAAREAID);

                sCustomTransType = string.Empty;

                if (string.IsNullOrEmpty(sCustGST))
                    sCustGST = "Unregistered";

                if (iOffLineBillType == (int)OfflineTransactionType.GMA)
                    sTitle = "GOLD METAL ADVANCE RECEIPT";
                else
                    sTitle = "TAX INVOICE";
            }

            #endregion

            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            conn.Open();

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            GetStoreInfo(ref sStorePhNo, ref sInvoiceFooter, ref sCINNo);

            sCompanyName = oBlank.GetCompanyName(conn);
        }

        public string GetORGDate(string sOffLineBillNo)
        {
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            conn.Open();
            string sResult = "";

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sQry = "select CONVERT(VARCHAR(11),ORGDATE,106) from CRWOfflineBillHeader where ReceiptId='" + sOffLineBillNo + "' and ORGDATE>'01-Jan-1900'";

            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            sResult = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;
        }

        private void GetGMAFromToDays(ref int iFDas, ref int iToDays)
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            // string commandText = "SELECT isnull(GMAFROMDAYS,0), isnull(GMATODAYS,0) FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            string commandText = " select isnull(GMAFROMDAYS,0), isnull(GMATODAYS,0) from CRWGOLDMETALADVANCE where " +
                " GMASCHEMECODE =( select GMASCHEMECODE from CUSTORDER_HEADER where ORDERNUM ='" + sOfflineBill + "')";
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iFDas = Convert.ToInt16(reader.GetValue(0));
                    iToDays = Convert.ToInt16(reader.GetValue(1));
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
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

        private void GetSalesInvData(ref DataTable dtSalesOrder)
        {
            string sQuery = "GetOfflineBillInvData";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@ReceiptId", SqlDbType.NVarChar).Value = sOfflineBill;

            if (iOrderTrans == 1)
                command.Parameters.Add("@GMAFROMORDER", SqlDbType.Int).Value = 1;
            else
                command.Parameters.Add("@GMAFROMORDER", SqlDbType.Int).Value = 0;

            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtSalesOrder);

            for (int i = 0; i <= dtSalesOrder.Rows.Count - 1; i++)
            {
                dTotAmt = dTotAmt + Convert.ToDouble(dtSalesOrder.Rows[i]["AMOUNT"]);

                if (string.IsNullOrEmpty(sLineCertNo))
                    sLineCertNo = Convert.ToString(dtSalesOrder.Rows[i]["CertificationNo"]);
                else
                    sLineCertNo = sLineCertNo + "," + Convert.ToString(dtSalesOrder.Rows[i]["CertificationNo"]);

                if (iOWNDMD == 1 || iOWNOG == 1 || iOTHERDMD == 1 || iOTHEROG == 1)
                {
                    dTotOgGrossWt += Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["GROSSWT"]));
                }
                else
                {
                    dTotSalableAmt += Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["AMOUNT"]));
                    dTotSalableWt += Math.Abs(Convert.ToDecimal(dtSalesOrder.Rows[i]["GROSSWT"]));
                }
            }
        }

        private void GetTender(ref DataTable dtTender)
        {
            string sStoreNo = ApplicationSettings.Terminal.StoreId;

            string sGSVNo = "";
            string sQuery = "";
            sQuery = "GetOfflineInvoiceTender";

            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@ReceiptId", SqlDbType.NVarChar).Value = sOfflineBill;
            command.Parameters.Add("@Store", SqlDbType.NVarChar).Value = sStoreNo;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTender);

            for (int i = 0; i <= dtTender.Rows.Count - 1; i++)
            {
                if (string.IsNullOrEmpty(sFooterMOPName))
                    sFooterMOPName = Convert.ToString(dtTender.Rows[i]["NAME"]);
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dtTender.Rows[i]["NAME"])))
                        sFooterMOPName += " , " + Convert.ToString(dtTender.Rows[i]["NAME"]);
                }

                if (string.IsNullOrEmpty(sFooterMOPValue))
                    sFooterMOPValue = Convert.ToString(Math.Abs(Convert.ToDecimal(dtTender.Rows[i]["AMOUNT"])));
                else
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(dtTender.Rows[i]["AMOUNT"])))
                        sFooterMOPValue += " , " + Convert.ToString(Math.Abs(Convert.ToDecimal(dtTender.Rows[i]["AMOUNT"])));
                }
            }
        }

        private void frmOfflineBillPrint_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (iOffLineBillType == (int)OfflineTransactionType.GMA)
            {
                if (iOrderTrans == 1)
                {
                    if (iDiscVoucher == 0)
                        localReport.ReportPath = "OrderOfflineAdvanceReceipt.rdlc";
                    else
                        localReport.ReportPath = "OrderOfflineAdvanceDiscountVoucher.rdlc";
                }
                else
                {
                    sORGDate = GetORGDate(sOfflineBill);
                    if (iDiscVoucher == 0)
                        localReport.ReportPath = "OfflineAdvanceReceipt.rdlc";
                    else
                        localReport.ReportPath = "OfflineAdvanceDiscountVoucher.rdlc";
                }
            }
            else
            {
                localReport.ReportPath = "GSTRptSaleInv.rdlc";
            }

            GetSalesInvData(ref dtSalesData);
            GetTender(ref dtTender);
            GetTaxInfo(ref dtTaxInfo);

            int iFromDays = 0;
            int iToDays = 0;


            GetGMAFromToDays(ref iFromDays, ref iToDays);

            sAmtinwds = Amtinwds(Math.Abs(dTotAmt));

            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "Detail";
            dsSalesOrder.Value = dtSalesData;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtSalesData));

            ReportDataSource RDTender = new ReportDataSource();
            RDTender.Name = "Tender";
            RDTender.Value = dtTender;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Tender", dtTender));

            ReportDataSource RDSubTotal = new ReportDataSource();
            RDSubTotal.Name = "SubTotal";
            RDSubTotal.Value = dtSubTotal;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("SubTotal", dtSubTotal));

            ReportDataSource RDTaxInfo = new ReportDataSource();
            RDTaxInfo.Name = "GSTTAX";
            RDTaxInfo.Value = dtTaxInfo;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("GSTTAX", dtTaxInfo));

            ReportDataSource RDStdRate = new ReportDataSource();
            RDStdRate.Name = "StdRate";
            RDStdRate.Value = dtStdRate;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("StdRate", dtStdRate));


            ReportDataSource RDPaymentInfo = new ReportDataSource();
            RDPaymentInfo.Name = "PaymentInfo";
            RDPaymentInfo.Value = dtSalesData;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("PaymentInfo", dtSalesData));
            ReportParameter[] param;

            if (iOrderTrans == 1)
                param = new ReportParameter[51];
            else
                param = new ReportParameter[48];


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
            param[12] = new ReportParameter("ReceiptNo", sInvoiceNo);
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
            param[28] = new ReportParameter("RefNoAndDate", "Ref Order No");
            param[29] = new ReportParameter("CustOrdAndDate", CustOrdAndDate);
            param[30] = new ReportParameter("CustomerGSTNo", sCustGST);
            param[31] = new ReportParameter("CustomTransType", sCustomTransType);
            param[32] = new ReportParameter("UTLabel", sUTLabel);
            param[33] = new ReportParameter("DelAddress", sCustDelAddress);
            param[34] = new ReportParameter("DelStateCode", sCustDelStateCode);
            param[35] = new ReportParameter("DelGSTNO", sCustDelGSTNO);

            param[36] = new ReportParameter("capGovtId1", "PAN");
            param[37] = new ReportParameter("valGovtId1", sGovtIdNo1);
            param[38] = new ReportParameter("capGovtId2", "Aadhar");
            param[39] = new ReportParameter("valGovtId2", sGovtIdNo2);
            param[40] = new ReportParameter("totSalableWt", Convert.ToString(dTotSalableWt));
            param[41] = new ReportParameter("totPayableAmt", Convert.ToString(dTotSalableAmt));
            param[42] = new ReportParameter("totOGGrossWt", Convert.ToString(dTotOgGrossWt));
            param[43] = new ReportParameter("FooterMOPDetails", sFooterMOPName);
            param[44] = new ReportParameter("FooterMOPValues", sFooterMOPValue);
            param[45] = new ReportParameter("CustLoyaltyNo", sCustLoyaltyNo);
            param[46] = new ReportParameter("LineCertNo", sLineCertNo);
            param[47] = new ReportParameter("ORGDATE", sORGDate);

            if (iOrderTrans == 1)
            {
                param[48] = new ReportParameter("GMAFROMDAYS", Convert.ToString(iFromDays));
                param[49] = new ReportParameter("GMATODAYS", Convert.ToString(iToDays));
                param[50] = new ReportParameter("GMADISCPCT", Convert.ToString(dGMADISCPCT));
            }


            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetTaxInfo(ref DataTable dtSTaxInfo)
        {
            #region Table Create
            DataTable tblTaxInfo = new DataTable("GSTTAX");

            dtSTaxInfo = tblTaxInfo;
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
                tblTaxInfo.Columns.Add("CESS", typeof(decimal));
                tblTaxInfo.Columns.Add("TOTTAX", typeof(decimal));
            }
            #endregion

            #region Create table data
            /*
            string sPreItem = "";
            int iLine = 0;
            for (int i = 0; i <= dtSalesData.Rows.Count - 1; i++)
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
                decimal CESS = 0m;

                HSNCODE = "";

                if (string.IsNullOrEmpty(HSNCODE))
                {
                    HSNCODE = "";
                }


                if (taxLine.Percentage != decimal.Zero)
                {
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
                    }
                    else
                    {
                        if (taxLine.TaxComponent == "SGST")
                        {
                            tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, ITAXCODE, ITRATE, ITAMT, CESS);
                        }
                        if (taxLine.TaxComponent == "IGST")
                        {
                            tblTaxInfo.Rows.Add(HSNCODE, CTAXCODE, CTRATE, CTAMT, STAXCODE, STRATE, STAMT, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, CESS);
                        }
                        if (taxLine.TaxComponent == "CGST")
                        {
                            tblTaxInfo.Rows.Add(HSNCODE, taxLine.TaxComponent, taxLine.Percentage, taxLine.Amount, STAXCODE, STRATE, STAMT, ITAXCODE, ITRATE, ITAMT, CESS);
                        }

                    }
                    sPreItem = saleLineItem.ItemId;
                    iLine = saleLineItem.LineId;
                    i++;
                }
            }
            */
            #endregion

            #region Tax Group

            /*Dictionary<string, decimal> dicSum = new Dictionary<string, decimal>();
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
                decimal CESS = Math.Abs(Convert.ToDecimal(row["CESS"]));
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

                                CESS = x["CESS"],
                            }
                         )
                         .GroupBy(s => new { s.HSNCODE, s.CTAXCODE, s.STAXCODE, s.ITAXCODE, s.CTRATE, s.STRATE, s.ITRATE })
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

                row["CESS"] = element.CESS;
                dtGroupTotal.Rows.Add(row);
            }

            dtSTaxInfo = dtGroupTotal;

            for (int i = 0; i <= dtSTaxInfo.Rows.Count - 1; i++)
            {
                dTotAmt = dTotAmt + Convert.ToDouble(dtSTaxInfo.Rows[i]["CTAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["STAMT"]) + Convert.ToDouble(dtSTaxInfo.Rows[i]["ITAMT"]);
            }*/
            #endregion
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

    }
}
