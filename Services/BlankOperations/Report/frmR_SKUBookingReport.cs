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
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using LSRetailPosis.Settings;
namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmR_SKUBookingReport : Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";
        string sCompanyName = string.Empty;

        public frmR_SKUBookingReport(SqlConnection conn)
        {
            InitializeComponent();

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
        }

        private void frmR_SKUBookingReport_Load(object sender, EventArgs e)
        {

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptSKUBooking.rdlc";
            DataTable dtTransaction = new DataTable();
            GetTransaction(ref dtTransaction);
            ReportDataSource rdsOGReport = new ReportDataSource("GetSKUBookingReportData", dtTransaction);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsOGReport);

            ReportParameter[] param = new ReportParameter[3];           
            param[0] = new ReportParameter("StoreAddress", sStoreAddress);
            param[1] = new ReportParameter("StoreName", sStoreName);
            param[2] = new ReportParameter("CompName", sCompanyName);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetTransaction(ref DataTable dtTrans)
        {
            string sQuery = "GetSKUBookingReportData";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }
    }
}
