using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmStockVerificationReport : Form
    {
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime(DateTime.Now);

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";
        string sProduct = "";
        string sArticle = "";

        string sVoucher = "";
        int iSummaryOrDetails = 0;


        public frmStockVerificationReport()
        {
            InitializeComponent();
        }

        public frmStockVerificationReport(int isSummary, string sVou)
        {
            InitializeComponent();
            iSummaryOrDetails = isSummary;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            connection = SqlCon;
            sVoucher = sVou;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            GetReportParam(sVoucher, connection);
        }


        private void frmStockVerificationReport_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dsGoldStockSummary.GetGoldStock' table. You can move, or remove it, as needed.
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (iSummaryOrDetails == 2)
                localReport.ReportPath = "rptStockVerification.rdlc";
            else
                localReport.ReportPath = "rptStockVerificationSummary.rdlc";

            DataTable dtTransaction = new DataTable();
            DataTable dtSummery = new DataTable();


            GetTransaction(ref dtTransaction);
            getStockVerificationSummery(ref dtSummery);

            ReportDataSource rdsTrans = new ReportDataSource("StockVerification", dtTransaction);
            ReportDataSource rdsSummery = new ReportDataSource("SVSummery", dtSummery);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsTrans);
            reportViewer1.LocalReport.DataSources.Add(rdsSummery);

            ReportParameter[] param = new ReportParameter[8];
            param[0] = new ReportParameter("TransDate", dtTransDate.ToShortDateString());
            param[1] = new ReportParameter("StoreAddress", sStoreAddress);
            param[2] = new ReportParameter("StorePhone", sStorePhNo);
            param[3] = new ReportParameter("StoreName", sStoreName);
            param[4] = new ReportParameter("StoreCurrency", ApplicationSettings.Terminal.CompanyCurrency);
            param[5] = new ReportParameter("DocNo", sVoucher);
            param[6] = new ReportParameter("ProductType", sProduct);
            param[7] = new ReportParameter("Article", sArticle);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetTransaction(ref DataTable dtTrans)
        {
            string sQuery = "";
            if (iSummaryOrDetails == 2)
                sQuery = "GetStockVerificationDetails";
            else
                sQuery = "GetStockVerificationSummary";

            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@Voucher", SqlDbType.NVarChar).Value = sVoucher;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }


        private void getStockVerificationSummery(ref DataTable dtSummeryTrans)
        {
            string sQuery = "";
            if (iSummaryOrDetails == 2)
                sQuery = "GetStockVerificationDetailsAtGlance";
            else
                sQuery = "GetStockVerificationSummaryAtGlance";

            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@Voucher", SqlDbType.NVarChar).Value = sVoucher;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daScrapTrans = new SqlDataAdapter(command);
            daScrapTrans.Fill(dtSummeryTrans);
        }

        private void GetReportParam(string sVou, SqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandString = " select  isnull(PRODUCTTYPE,'') PRODUCTTYPE ,isnull(ARTICLE,'') ARTICLE from RETAILSTOCKCOUNTHEADER where VOUCHERNUM='" + sVou + "'";

            SqlCommand command = new SqlCommand(commandString, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sProduct = Convert.ToString(reader.GetValue(0));
                    sArticle = Convert.ToString(reader.GetValue(1));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

    }
}
