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
    public partial class frmAWCSReport:Form
    {
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime("01/01/1900");
        BlankOperations oBlank = new BlankOperations();

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";
        string sCompanyName = string.Empty;

        public frmAWCSReport()
        {
            InitializeComponent();
        }

        public frmAWCSReport(string sTransactionDate, SqlConnection conn)
        {
            InitializeComponent();
            if(Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if(Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if(!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            connection = conn;
            if(!string.IsNullOrEmpty(sTransactionDate))
                dtTransDate = Convert.ToDateTime(sTransactionDate);
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
        }

        private void frmAWCSReport_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptAWCSReport.rdlc";
            DataTable dtTransaction = new DataTable();
            GetTransaction(ref dtTransaction);
            ReportDataSource dsAWCS = new ReportDataSource("dsAWCS", dtTransaction);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(dsAWCS);

            ReportParameter[] param = new ReportParameter[4];
            param[0] = new ReportParameter("TransDate", dtTransDate.ToShortDateString());
            param[1] = new ReportParameter("StoreAddress", sStoreAddress);
            param[2] = new ReportParameter("StoreName", sStoreName);
            param[3] = new ReportParameter("CompName", sCompanyName);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetTransaction(ref DataTable dtTrans)
        {
            string sQuery = "GetArticleWiseCounterStockSummery";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
           // command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }
    }
}
