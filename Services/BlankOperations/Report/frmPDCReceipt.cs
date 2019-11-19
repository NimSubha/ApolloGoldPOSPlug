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
    public partial class frmPDCReceipt : Form
    {
        SqlConnection connection;
        DateTime dtFromDate = Convert.ToDateTime(DateTime.Now);
        DateTime dtToDate = Convert.ToDateTime(DateTime.Now);

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sPDCReceiptId = "";

        public frmPDCReceipt()
        {
            InitializeComponent();
        }

        public frmPDCReceipt(string sReceiptId, SqlConnection conn)
        {
            InitializeComponent();
            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            connection = conn;
            sPDCReceiptId = sReceiptId;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void frmPDCReceipt_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;

            localReport.ReportPath = "rptPDCReceipt.rdlc";
          

            DataTable dtPDCH = new DataTable();
            DataTable dtPDCD = new DataTable();

            GetPDCHeader(ref dtPDCH);
            GetPDCDetails(ref dtPDCD);

            ReportDataSource rdsPDCH = new ReportDataSource("getPDCH", dtPDCH);
            ReportDataSource rdsPDCD= new ReportDataSource("getPDCD", dtPDCD);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsPDCH);
            reportViewer1.LocalReport.DataSources.Add(rdsPDCD);

            ReportParameter[] param = new ReportParameter[6];
            param[0] = new ReportParameter("FromDate", Convert.ToDateTime(dtFromDate).ToString("dd-MMM-yyyy"));
            param[1] = new ReportParameter("ToDate", Convert.ToDateTime(dtToDate).ToString("dd-MMM-yyyy"));
            param[2] = new ReportParameter("StoreAddress", sStoreAddress);
            param[3] = new ReportParameter("StorePhone", sStorePhNo);
            param[4] = new ReportParameter("StoreName", sStoreName);
            param[5] = new ReportParameter("StoreCurrency", ApplicationSettings.Terminal.CompanyCurrency);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetPDCHeader(ref DataTable dtPDCH)
        {
            string sQuery = "GetPDCEntryHeader";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@PDCENTRYCODE", SqlDbType.NVarChar).Value = sPDCReceiptId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtPDCH);
        }

        private void GetPDCDetails(ref DataTable dtPDCD)
        {
            string sQuery = "GetPDCEntryDetails";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@PDCENTRYCODE", SqlDbType.NVarChar).Value = sPDCReceiptId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtPDCD);
        }
    }
}
