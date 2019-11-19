using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using BlankOperations;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using BlankOperations.WinFormsTouch;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmCustomerOrderChequeInfo : Form
    {
        public DataTable dtCheque;
        private IApplication application;
        frmCustomerOrder frmCustOrd;
        frmOrderDetails frmOrderDet;
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataTable dtTemp = new DataTable("dtTemp");
        Rounding objRounding = new Rounding();

        string sOrderNum = string.Empty;
        public IPosTransaction pos { get; set; }

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

        public frmCustomerOrderChequeInfo()
        {
            InitializeComponent();
        }

        public frmCustomerOrderChequeInfo(IPosTransaction posTransaction, IApplication Application, DataTable dtChq, decimal dTotAmount, int isView = 0)
        {
            InitializeComponent();

            #region
            pos = posTransaction;
            application = Application;
            this.dtCheque = dtChq;
            frmCustOrd = new frmCustomerOrder(posTransaction, Application);
            sOrderNum = frmCustOrd.txtOrderNo.Text;
            DataTable dtTempShow = new DataTable();

            lblOrderNo.Text = sOrderNum;
            lblTotAmt.Text = Convert.ToString(dTotAmount);

            gridCheque.DataSource = dtChq;
            if (dtChq != null && dtChq.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dtChq.Rows)
                {
                    dTotalAmount += Convert.ToDecimal(drTotal["ChqAmt"]);
                }
                lblGridTotAmt.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));// Convert.ToString(dTotalAmount);
            }

            if (isView == 1)
            {
                btnCommit.Enabled = false;
                btnAdd.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnClear.Enabled = false;
            }
            #endregion
        }

        #region Validate()
        bool isValidate()
        {
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtBank.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Bank can not be blank or empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtBank.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtBranch.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Branch can not be blank or empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtBranch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtIFSC.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("IFSC code can not be blank or empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtIFSC.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtMICR.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("MICR no can not be blank or empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMICR.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            else
            {
                return true;
            }
        }
        #endregion

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (isValidate())
            {
                DataRow dr;
                if (IsEdit == false && dtCheque != null && dtCheque.Rows.Count == 0 && dtCheque.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtCheque.Columns.Add("ChequeNo", typeof(string));
                    dtCheque.Columns.Add("ChqDate", typeof(string));
                    dtCheque.Columns.Add("ChqAmt", typeof(decimal));
                    dtCheque.Columns.Add("Bank", typeof(string));
                    dtCheque.Columns.Add("Branch", typeof(string));
                    dtCheque.Columns.Add("IFSCCODE", typeof(string));
                    dtCheque.Columns.Add("MICRNO", typeof(string));
                    dtTemp = dtCheque.Clone();
                }

                if (IsEdit == false)
                {
                    dr = dtCheque.NewRow();

                    if (!string.IsNullOrEmpty(txtChqNo.Text.Trim()))
                        dr["ChequeNo"] = Convert.ToString(txtChqNo.Text.Trim());
                    else
                        dr["ChequeNo"] = "";

                    dr["ChqDate"] = Convert.ToString(dtpChqDtae.Text.Trim());

                    if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                        dr["ChqAmt"] = Convert.ToDecimal(txtAmount.Text.Trim());
                    else
                        dr["ChqAmt"] = 0m;// DBNull.Value;

                    if (!string.IsNullOrEmpty(txtBank.Text.Trim()))
                        dr["Bank"] = Convert.ToString(txtBank.Text.Trim());
                    else
                        dr["Bank"] = "";

                    if (!string.IsNullOrEmpty(txtBranch.Text.Trim()))
                        dr["Branch"] = Convert.ToString(txtBranch.Text.Trim());
                    else
                        dr["Branch"] = "";

                    if (!string.IsNullOrEmpty(txtIFSC.Text.Trim()))
                        dr["IFSCCODE"] = Convert.ToString(txtIFSC.Text.Trim());
                    else
                        dr["IFSCCODE"] = "";
                    if (!string.IsNullOrEmpty(txtMICR.Text.Trim()))
                        dr["MICRNO"] = Convert.ToString(txtMICR.Text.Trim());
                    else
                        dr["MICRNO"] = "";

                    dtCheque.Rows.Add(dr);
                    if (dtCheque != null && dtCheque.Rows.Count > 0)
                    {
                        gridCheque.DataSource = dtCheque.DefaultView;
                        Decimal dTotalAmount = 0m;
                        foreach (DataRow drTotal in dtCheque.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["ChqAmt"]);
                        }
                        lblGridTotAmt.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));// Convert.ToString(dTotalAmount);
                    }
                }
                if (IsEdit == true)
                {
                    DataRow EditRow = dtCheque.Rows[EditselectedIndex];

                    if (!string.IsNullOrEmpty(txtChqNo.Text.Trim()))
                        EditRow["ChequeNo"] = Convert.ToString(txtChqNo.Text.Trim());
                    else
                        EditRow["ChequeNo"] = "";

                    EditRow["ChqDate"] = dtpChqDtae.Text.Trim();
                    if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                        EditRow["ChqAmt"] = decimal.Round(Convert.ToDecimal(txtAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        EditRow["ChqAmt"] = objRounding.Round(decimal.Zero, 2);
                    if (!string.IsNullOrEmpty(txtBank.Text.Trim()))
                        EditRow["Bank"] = txtBank.Text;
                    else
                        EditRow["Bank"] = "";

                    if (!string.IsNullOrEmpty(txtBranch.Text.Trim()))
                        EditRow["Branch"] = txtBranch.Text;
                    else
                        EditRow["Branch"] = "";

                    if (!string.IsNullOrEmpty(txtIFSC.Text.Trim()))
                        EditRow["IFSCCODE"] = txtIFSC.Text;
                    else
                        EditRow["IFSCCODE"] = "";

                    if (!string.IsNullOrEmpty(txtMICR.Text.Trim()))
                        EditRow["MICRNo"] = txtMICR.Text;
                    else
                        EditRow["MICRNo"] = "";

                    dtCheque.AcceptChanges();

                    gridCheque.DataSource = dtCheque.DefaultView;
                    if (dtCheque != null && dtCheque.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach (DataRow drTotal in dtCheque.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["ChqAmt"]);
                        }
                        lblGridTotAmt.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));// Convert.ToString(dTotalAmount);
                    }
                    IsEdit = false;
                }
                ClearControls();
            }
        }

        private void ClearControls()
        {
            txtBank.Text = "";
            txtAmount.Text = "0.00";
            txtBranch.Text = "";
            txtChqNo.Text = "";
            txtIFSC.Text = "";
            txtMICR.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
            dtCheque.Clear();
            dtTemp.Clear();
            lblGridTotAmt.Text = string.Empty;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;

            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtTemp.Rows[EditselectedIndex];

                    txtChqNo.Text = Convert.ToString(theRowToSelect["ChequeNo"]);
                    dtpChqDtae.Text = Convert.ToString(theRowToSelect["ChqDate"]);
                    txtAmount.Text = Convert.ToString(theRowToSelect["ChqAmt"]);
                    txtBank.Text = Convert.ToString(theRowToSelect["Bank"]);
                    txtBranch.Text = Convert.ToString(theRowToSelect["Branch"]);
                    txtIFSC.Text = Convert.ToString(theRowToSelect["IFSCCODE"]);
                    txtMICR.Text = Convert.ToString(theRowToSelect["MICRNo"]);
                }
            }
            else
            {
                if (dtCheque != null && dtCheque.Rows.Count > 0)
                {
                    if (grdView.RowCount > 0)
                    {
                        IsEdit = true;
                        EditselectedIndex = grdView.GetSelectedRows()[0];
                        DataRow theRowToSelect = dtCheque.Rows[EditselectedIndex];
                        txtChqNo.Text = Convert.ToString(theRowToSelect["ChequeNo"]);
                        dtpChqDtae.Text = Convert.ToString(theRowToSelect["ChqDate"]);
                        txtAmount.Text = Convert.ToString(theRowToSelect["ChqAmt"]);
                        txtBank.Text = Convert.ToString(theRowToSelect["Bank"]);
                        txtBranch.Text = Convert.ToString(theRowToSelect["Branch"]);
                        txtIFSC.Text = Convert.ToString(theRowToSelect["IFSCCODE"]);
                        txtMICR.Text = Convert.ToString(theRowToSelect["MICRNo"]);
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            DataRow theRowToDelete = null;
            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    theRowToDelete = dtTemp.Rows[DeleteSelectedIndex];
                    dtTemp.Rows.Remove(theRowToDelete);

                    gridCheque.DataSource = dtTemp.DefaultView;
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach (DataRow drTotal in dtTemp.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["ChqAmt"]);
                        }
                        lblGridTotAmt.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));// Convert.ToString(dTotalAmount);
                    }
                }
            }

            IsEdit = false;
            ClearControls();
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
           /* SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            decimal dCustOrderTotalAmt = 0m;
            if (!string.IsNullOrEmpty(lblTotAmt.Text))
                dCustOrderTotalAmt = Convert.ToDecimal(lblTotAmt.Text);

            string sMaxAmount = string.Empty;
            string sTerminalID = ApplicationSettings.Terminal.TerminalId;
            string sMinAmt = Convert.ToString(ValidateMinDeposit(connection, out sMaxAmount, sTerminalID, dCustOrderTotalAmt));

            string sSms = "INFORMATION : MINIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sMinAmt), 2, MidpointRounding.AwayFromZero) + " " +
                          "AND MAXIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sMaxAmount), 2, MidpointRounding.AwayFromZero);



            if (Convert.ToDecimal(sMinAmt) != 0 && Convert.ToDecimal(sMaxAmount) != 0 && !string.IsNullOrEmpty(lblGridTotAmt.Text))
            {
                if (Convert.ToDecimal(sMinAmt) > Convert.ToDecimal(lblGridTotAmt.Text)
                    || Convert.ToDecimal(sMaxAmount) < Convert.ToDecimal(lblGridTotAmt.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sSms, MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        btnAdd.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }
                else
                {
                    if (dtCheque != null && dtCheque.Rows.Count > 0)
                    {
                        frmCustOrd.dtChequeInfo = dtCheque;
                        this.Close();
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select at least one item to submit.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            btnAdd.Focus();
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                }
            }*/

            if (dtCheque != null && dtCheque.Rows.Count > 0)
            {
                frmCustOrd.dtChequeInfo = dtCheque;
                this.Close();
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select at least one item to submit.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnAdd.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }

        private string ValidateMinDeposit(SqlConnection connection, out string MaxAmount, string sTerminalId, decimal dOrdAmt)// RETAILTEMPTABLE
        {
            string commandText = " DECLARE @MINDEPOSITPCT NUMERIC(28,16) " +
                            " SELECT @MINDEPOSITPCT=ISNULL(MINIMUMDEPOSITFORCUSTORDER,0) from RETAILPARAMETERS " +
                            " SELECT " + dOrdAmt / 100 + " * @MINDEPOSITPCT ";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = null;
            reader = command.ExecuteReader();
            string sMinAmount = string.Empty;
            string sMaxAmount = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sMinAmount = Convert.ToString(reader.GetValue(0));
                    sMaxAmount = Convert.ToString(dOrdAmt);
                    MaxAmount = sMaxAmount;
                }
                reader.Close();

            }
            else
            {
                reader.Close();
                sMinAmount = "0";
                sMaxAmount = "0";
            }
            connection.Close();
            MaxAmount = sMaxAmount;
            return sMinAmount;
        }

        private void txtMICR_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtChqNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
    }
}
