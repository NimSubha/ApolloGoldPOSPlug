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
    public partial class frmCashRegisterrpt : Form
    {
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime("01/01/1900");

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        public frmCashRegisterrpt()
        {
            InitializeComponent();
        }

        public frmCashRegisterrpt(string sTransactionDate, SqlConnection conn)
        {
            InitializeComponent();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            connection = conn;
            if (!string.IsNullOrEmpty(sTransactionDate))
                dtTransDate = Convert.ToDateTime(sTransactionDate);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void frmCashRegisterrpt_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "CashRegisterrpt .rdlc";
            DataTable dtTransaction = new DataTable();
            DataTable dtCollection = new DataTable();
            DataTable dtMOP = new DataTable();
            DataTable dtTD = new DataTable();
            DataTable dtTDeclare = new DataTable();
            DataTable dtFD = new DataTable();

            GetTransaction(ref dtTransaction);
            GetCollection(ref dtCollection);
            GetMOP(ref dtMOP);
            GetTD(ref dtTD);
            GetTDeclare(ref dtTDeclare);

            GetFD(ref dtFD);

            ReportDataSource rdsTrans = new ReportDataSource("dsCashRegister", dtTransaction);
            ReportDataSource rdsCollection = new ReportDataSource("dsCollection", dtCollection);
            ReportDataSource rdsMOP = new ReportDataSource("dsMOP", dtMOP);
            ReportDataSource rdsTD = new ReportDataSource("dsTD", dtTD);
            ReportDataSource rdsTDeclare = new ReportDataSource("dsTDeclare", dtTDeclare);
            ReportDataSource rdsFD = new ReportDataSource("dsFD", dtFD);

            //if ((dtTransaction != null)
            //    && (dtTransaction.Rows.Count > 0)
            //    && (Convert.ToString(dtTransaction.Rows[0]["TOTAMOUNT"]) != string.Empty)
            //    && (Convert.ToDecimal(dtTransaction.Rows[0]["TOTAMOUNT"]) > 0))
            //{
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsTrans);
            reportViewer1.LocalReport.DataSources.Add(rdsCollection);
            reportViewer1.LocalReport.DataSources.Add(rdsMOP);
            reportViewer1.LocalReport.DataSources.Add(rdsTD);
            reportViewer1.LocalReport.DataSources.Add(rdsTDeclare);
            reportViewer1.LocalReport.DataSources.Add(rdsFD);

            ReportParameter[] param = new ReportParameter[4];
            param[0] = new ReportParameter("TransDate", dtTransDate.ToShortDateString());
            param[1] = new ReportParameter("StoreAddress", sStoreAddress);
            param[2] = new ReportParameter("StorePhone", sStorePhNo);
            param[3] = new ReportParameter("StoreName", sStoreName);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
            //  }
            //else
            //{
            //    MessageBox.Show("No data found");
            //    return;
            //}
        }

        private void GetTransaction(ref DataTable dtTrans)
        {
            string sQuery = "GETCASHREGISTERTRANS";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }

        private void GetCollection(ref DataTable dtCol)//DataSet dsStock)
        {
            string sQuery = "GETCASHREGOTHCOLLECTION";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@STORENUMBER", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dtCol);
        }

        private void GetMOP(ref DataTable dtLess)
        {
            string sQuery = "GETCASHREGMOP";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@STORENUMBER", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar).Value = ApplicationSettings.Database.DATAAREAID;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daMOP = new SqlDataAdapter(command);
            daMOP.Fill(dtLess);
        }

        private void GetTD(ref DataTable dtTD)
        {
            string sQuery = "GETTENDERDENOMINATION";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = sReg;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtTD);
        }

        private void GetFD(ref DataTable dtFD)

        {
            string sQuery = "GETFDDETAILS";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = sReg;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtFD);
        }

        private void GetTDeclare(ref DataTable dtTDECLARE)
        {
            string sQuery = "GETTENDERDECLARE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = sReg;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtTDECLARE);
        }
    }
}
