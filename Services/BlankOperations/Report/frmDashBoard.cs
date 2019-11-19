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
    public partial class frmDashBoard : Form
    {
        SqlConnection connection;
        DateTime dtFromDate = Convert.ToDateTime(DateTime.Now);
        DateTime dtToDate = Convert.ToDateTime(DateTime.Now); 

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        int iSummery = 0;

        public frmDashBoard()
        {
            InitializeComponent();
        }
        public frmDashBoard(DateTime dtFDate, DateTime dtTTime, SqlConnection conn,int iMOPSummery=0) 
        {
            InitializeComponent();

            dtFromDate = dtFDate;
            dtToDate = dtTTime;
            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            iSummery = iMOPSummery;
            connection = conn;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void frmDashBoard_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if(iSummery==1)
                localReport.ReportPath = "rptMOPWiseSummary.rdlc";
            else
                localReport.ReportPath = "rptMOPWiseDetails.rdlc";

            DataTable dtMOPWiseSummary = new DataTable();
            DataTable dtMOPWiseDetails = new DataTable();

            GetMOPWiseSummary(ref dtMOPWiseSummary);
            GetMOPWiseDetails(ref dtMOPWiseDetails);

            ReportDataSource rdsMOPWiseSummary = new ReportDataSource("MOPWiseSummary", dtMOPWiseSummary);
            ReportDataSource rdsMOPWiseDetails = new ReportDataSource("MOPWiseDetails", dtMOPWiseDetails);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsMOPWiseSummary);
            reportViewer1.LocalReport.DataSources.Add(rdsMOPWiseDetails);

            ReportParameter[] param = new ReportParameter[6];
            param[0] = new ReportParameter("FromDate",Convert.ToDateTime(dtFromDate).ToString("dd-MMM-yyyy"));
            param[1] = new ReportParameter("ToDate",Convert.ToDateTime(dtToDate).ToString("dd-MMM-yyyy"));
            param[2] = new ReportParameter("StoreAddress", sStoreAddress);
            param[3] = new ReportParameter("StorePhone", sStorePhNo);
            param[4] = new ReportParameter("StoreName", sStoreName);
            param[5] = new ReportParameter("StoreCurrency", ApplicationSettings.Terminal.CompanyCurrency);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetMOPWiseSummary(ref DataTable dtMOPWiseSummary) 
        {
            string sQuery = "MOPWiseSummary";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtFromDate).ToString("dd-MMM-yyyy") ;
           // command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = dtToDate;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtMOPWiseSummary);
        }

        private void GetMOPWiseDetails(ref DataTable dtMOPWiseDetails) 
        {
            string sQuery = "MOPWiseDetails";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtFromDate).ToString("dd-MMM-yyyy");
            //command.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = dtToDate;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtMOPWiseDetails);
        }
    }
}
