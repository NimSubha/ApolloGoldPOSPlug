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

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmGoldStockSummery:Form
    {
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime(DateTime.Now);

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string   sMetal = "";


        public frmGoldStockSummery()
        {
            InitializeComponent();
        }

        public frmGoldStockSummery(string sMetalType, SqlConnection conn)
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

        private void frmGoldStockSummery_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dsGoldStockSummary.GetGoldStock' table. You can move, or remove it, as needed.
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptGoldStockSummary.rdlc";
            DataTable dtTransaction = new DataTable();
            DataTable dtScrapDetails = new DataTable();

            
            GetTransaction(ref dtTransaction);
            getSCRAPMETALSTOCKSUMMARY(ref dtScrapDetails);

            ReportDataSource rdsTrans = new ReportDataSource("GoldStockSummary", dtTransaction);
            ReportDataSource rdsSrcapTrans = new ReportDataSource("ScrapSummary", dtScrapDetails); 

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsTrans);
            reportViewer1.LocalReport.DataSources.Add(rdsSrcapTrans);
          
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
            string sQuery = "GETMETALSTOCKSUMMARY";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@METALTYPE", SqlDbType.NVarChar).Value = sMetal;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }

        private void getSCRAPMETALSTOCKSUMMARY(ref DataTable dtScrapTrans)  
        {
            string sQuery = "SCRAP_GETMETALSTOCKSUMMARY";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@METALTYPE", SqlDbType.NVarChar).Value = sMetal;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daScrapTrans = new SqlDataAdapter(command);
            daScrapTrans.Fill(dtScrapTrans);
        }
    }
}
