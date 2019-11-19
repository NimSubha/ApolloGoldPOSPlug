using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmReportInput : Form
    {
        private IApplication application;
        public string sCounter = string.Empty;
        int iCWise = 0;
        SqlConnection connection;


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
        public frmReportInput(SqlConnection sConn)
        {
            InitializeComponent();
            connection = sConn;
            loadComboValue();
        }

        public frmReportInput(SqlConnection sConn, int iCounerWise)
        {
            InitializeComponent();
            connection = sConn;
            loadComboValue();
            iCWise = iCounerWise;
        }

        bool ValidateControls()
        {

            if (string.IsNullOrEmpty(cmbCounter.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter counter", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    cmbCounter.Focus();
                    return false;
                }
            }
            else
            {
                if (isValiedCounter(connection, cmbCounter.Text))
                    return true;
                else
                {
                    MessageBox.Show("Invalid counter.");
                    return false;
                }
            }

        }

        private void btnOk_Click(object sender, EventArgs e)
        {

            if (ValidateControls())
            {
                sCounter = cmbCounter.Text.Trim();
                if (iCWise == 0)
                {
                    frmCounterWiseStkReport objStkReport = new frmCounterWiseStkReport(connection, 1, sCounter);
                    objStkReport.ShowDialog();
                }
                else if (iCWise == 1)
                {
                    frmCounterWiseStkReport objStkReport = new frmCounterWiseStkReport(connection, 0, sCounter);
                    objStkReport.ShowDialog();
                }
                else if (iCWise == 2)
                {
                    frmCounterWiseStkDtlReport objStkReportDtl = new frmCounterWiseStkDtlReport(connection, sCounter);
                    objStkReportDtl.ShowDialog();
                }
            }


        }
        private DataTable getDataTable(string _sSql)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = _sSql;

                DataTable dtComboField = new DataTable();
                DataRow row = dtComboField.NewRow();
                dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #region loadComboValue
        private void loadComboValue()
        {
            DataTable dtSchemeCode = getDataTable("select isnull(COUNTERCODE,'') as COUNTERCODE from COUNTERMASTER");
            cmbCounter.DataSource = dtSchemeCode;
            cmbCounter.DisplayMember = "COUNTERCODE";
        }


        #endregion

        public bool isValiedCounter(SqlConnection conn, string _Counter)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = string.Empty;

            commandText = "select isnull(COUNTERCODE,'') from COUNTERMASTER where COUNTERCODE = '" + _Counter + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sCont = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (sCont != string.Empty)
                return true;
            else
                return false;

        }

        private void btnAllCounter_Click(object sender, EventArgs e)
        {
            sCounter = cmbCounter.Text.Trim();
            if (iCWise == 0)
            {
                frmCounterWiseStkReport objStkReport = new frmCounterWiseStkReport(connection, 1);
                objStkReport.ShowDialog();
            }
            else if (iCWise == 1)
            {
                frmCounterWiseStkReport objStkReport = new frmCounterWiseStkReport(connection);
                objStkReport.ShowDialog();
            }
            else if (iCWise == 2)
            {
                frmCounterWiseStkDtlReport objStkReportDtl = new frmCounterWiseStkDtlReport(connection, sCounter);
                objStkReportDtl.ShowDialog();
            }
        }
    }
}
