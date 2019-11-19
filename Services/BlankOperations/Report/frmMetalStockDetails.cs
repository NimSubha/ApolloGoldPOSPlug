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
    public partial class frmMetalStockDetails:Form
    {
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime(DateTime.Now);

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sMetal = "";

        public frmMetalStockDetails()
        {
            InitializeComponent();
        }

        public frmMetalStockDetails(string sMetalType, SqlConnection conn)
        {
            InitializeComponent();
            if(Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if(Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if(!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            connection = conn;
            sMetal = sMetalType;

            if(connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void frmMetalStockDetails_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptStockDetails.rdlc";
            DataTable dtTransaction = new DataTable();

            GetTransaction(ref dtTransaction);

            ReportDataSource rdsTrans = new ReportDataSource("STOCKDETAILS", dtTransaction);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsTrans);

            ReportParameter[] param = new ReportParameter[6];
            param[0] = new ReportParameter("TransDate", dtTransDate.ToShortDateString());
            param[1] = new ReportParameter("StoreAddress", sStoreAddress);
            param[2] = new ReportParameter("StorePhone", sStorePhNo);
            param[3] = new ReportParameter("StoreName", sStoreName);
            param[4] = new ReportParameter("MetalType", sMetal);
            param[5] = new ReportParameter("StoreCurrency", ApplicationSettings.Terminal.CompanyCurrency);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetTransaction(ref DataTable dtTrans)
        {
            string sQuery = "GETMETALSTOCKDETAILS";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@METALTYPE", SqlDbType.NVarChar).Value = sMetal;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }
    }
}
