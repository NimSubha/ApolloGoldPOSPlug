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
    public partial class FrmIncomeExpenseRegisterRpt : Form
    {
        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        DateTime dtTransDate = Convert.ToDateTime("01/01/1900");

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";
        string sTerminal = "";
        string sStaff = "";

        public FrmIncomeExpenseRegisterRpt(string sTransactionDate, SqlConnection conn, string sTerminalId, string sStaffId)
        {
            InitializeComponent();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            sTerminal = sTerminalId;
            sStaff = sStaffId;

            connection = conn;
            if (!string.IsNullOrEmpty(sTransactionDate))
                dtTransDate = Convert.ToDateTime(sTransactionDate);
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void FrmIncomeExpenseRegisterRpt_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "Rpt_IncomeExpenseRegister.rdlc";
            DataTable dtTransaction = new DataTable();

            GetTransaction(ref dtTransaction);

            ReportDataSource rdsTrans = new ReportDataSource("dsIncomeExpenseRegister", dtTransaction);

            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsTrans);

            ReportParameter[] param = new ReportParameter[2];

            param[0] = new ReportParameter("CompanyName", oBlank.GetCompanyName(connection));
            param[1] = new ReportParameter("SDate", dtTransDate.ToShortDateString());

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();
            this.reportViewer1.RefreshReport();
        }

         private void GetTransaction(ref DataTable dtTrans)
        {
            string sQuery = "spReports_IncomeExpenseReg";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@pvTransDate", SqlDbType.DateTime).Value = dtTransDate;
            command.Parameters.Add("@pvTerminal", SqlDbType.NVarChar).Value = sTerminal;
            command.Parameters.Add("@pvStaff", SqlDbType.NVarChar).Value = sStaff;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);
        }
    }
}
