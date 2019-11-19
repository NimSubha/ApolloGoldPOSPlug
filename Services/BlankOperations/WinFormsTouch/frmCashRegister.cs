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
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmCashRegister : frmTouchBase
    {
        private IApplication application;

        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
            }
        }


        public frmCashRegister()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            if (dtpTransDate != null && string.IsNullOrEmpty(txtTerminal.Text) && string.IsNullOrEmpty(Convert.ToString(txtSM.Tag)))
            {
                Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmCashRegisterrpt reportfrm
                     = new Report.frmCashRegisterrpt(dtpTransDate.Value.ToShortDateString(), connection);

                reportfrm.ShowDialog();
            }
            else
            {

                Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmCashRegTAndCWise reportfrm
                     = new Report.frmCashRegTAndCWise(dtpTransDate.Value.ToShortDateString(), connection, txtTerminal.Text, Convert.ToString(txtSM.Tag));

                reportfrm.ShowDialog();
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSM_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtSM.Tag = dialog6.SelectedEmployeeId;
                txtSM.Text = dialog6.SelectEmployeeName;
            }
        }

        private void btnTerminal_Click(object sender, EventArgs e)
        {
            DataTable dt = GetTerminalList();
            if (dt != null && dt.Rows.Count > 0)
            {
                Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Terminal List");
                Osearch.ShowDialog();

                DataRow dr = Osearch.SelectedDataRow;

                if (dr != null)
                {
                    string sTerminalId = Convert.ToString(dr["TERMINALID"]);
                    txtTerminal.Text = sTerminalId;
                }
            }
        }

        private DataTable GetTerminalList()
        {
            string sSQL = "select TERMINALID,Name from RETAILTERMINALTABLE ";
            return GetDataTable(sSQL);
        }

        private static DataTable GetDataTable(string sSQL)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = sSQL;// "select ReceiptId,TransDate from BulkItemIssueHeader";

                DataTable dt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnTerminalClear_Click(object sender, EventArgs e)
        {
            txtTerminal.Text = "";
        }

        private void btnCashierClear_Click(object sender, EventArgs e)
        {
            txtSM.Text = "";
        }
    }
}
