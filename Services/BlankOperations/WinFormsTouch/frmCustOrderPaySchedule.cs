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
    public partial class frmCustOrderPaySchedule : Form
    {
        public DataTable dtPaySched;
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
        public frmCustOrderPaySchedule()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public frmCustOrderPaySchedule(IPosTransaction posTransaction, IApplication Application, DataTable dtPaySch, decimal dTotAmount, int isView = 0)
        {
            InitializeComponent();

            #region
            pos = posTransaction;
            application = Application;
            this.dtPaySched = dtPaySch;
            frmCustOrd = new frmCustomerOrder(posTransaction, Application);
            sOrderNum = frmCustOrd.txtOrderNo.Text;
            DataTable dtTempShow = new DataTable();

            lblOrderNo.Text = sOrderNum;
            lblTotAmt.Text = Convert.ToString(dTotAmount);

            //foreach (DataRow dr in dtPaySch.Rows)
            //{
            //    dtTemp.ImportRow(dr);
            //}

            gridPaySchedule.DataSource = dtPaySch;
            if (dtPaySch != null && dtPaySch.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                Decimal dTotPer = 0m;
                foreach (DataRow drTotal in dtPaySch.Rows)
                {
                    dTotalAmount += Convert.ToDecimal(drTotal["Amount"]);
                    dTotPer += Convert.ToDecimal(drTotal["PerAmt"]);
                }
                lblGridTotAmt.Text = Convert.ToString(dTotalAmount);
                lblTotPer.Text = Convert.ToString(dTotPer);
            }
            //gridPaySchedule.DataSource = dtPaySch.DefaultView;

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

        //public frmCustOrderPaySchedule(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, DataTable dtPaySch, decimal dTotAmount)
        //{
        //    InitializeComponent();

        //    #region
        //    pos = posTransaction;
        //    application = Application;
        //    this.dtPaySched = dtPaySch;
        //    frmCustOrd = new frmCustomerOrder(posTransaction, Application);
        //    sOrderNum = frmCustOrd.txtOrderNo.Text;
        //    DataTable dtTempShow = new DataTable();

        //    lblOrderNo.Text = sOrderNum;
        //    lblTotAmt.Text = Convert.ToString(dTotAmount);

        //    foreach (DataRow dr in dtPaySch.Rows)
        //    {
        //        dtTemp.ImportRow(dr);
        //    }

        //    gridPaySchedule.DataSource = dtTemp;
        //    if (dtTemp != null && dtTemp.Rows.Count > 0)
        //    {
        //        Decimal dTotalAmount = 0m;
        //        Decimal dTotPer = 0m;
        //        foreach (DataRow drTotal in dtTemp.Rows)
        //        {
        //            dTotalAmount += Convert.ToDecimal(drTotal["Amount"]);
        //            dTotPer += Convert.ToDecimal(drTotal["PerAmt"]);
        //        }
        //        lblGridTotAmt.Text = Convert.ToString(dTotalAmount);
        //        lblTotPer.Text = Convert.ToString(dTotPer);
        //    }
        //    gridPaySchedule.DataSource = dtPaySch.DefaultView;

        //    btnCommit.Enabled = false;
        //    btnAdd.Enabled = false;
        //    btnEdit.Enabled = false;
        //    btnDelete.Enabled = false;
        //    btnClear.Enabled = false;
        //    #endregion
        //}


        #region Validate()
        bool isValidate()
        {

            if (string.IsNullOrEmpty(lblTotAmt.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Item selected in for the order", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnAdd.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtPer.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Percentage can not be Zero.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmt.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (Convert.ToDecimal(txtAmt.Text.Trim()) == 0m)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be Zero.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmt.Focus();
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

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            if (isValidate())
            {
                DataRow dr;
                if (IsEdit == false && dtPaySched != null && dtPaySched.Rows.Count == 0 && dtPaySched.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtPaySched.Columns.Add("PayDate", typeof(string));
                    dtPaySched.Columns.Add("PerAmt", typeof(decimal));

                    dtPaySched.Columns.Add("Amount", typeof(decimal));
                    dtTemp = dtPaySched.Clone();
                }

                if (IsEdit == false)
                {
                    dr = dtPaySched.NewRow();
                    dr["PayDate"] = Convert.ToString(dtPayDtae.Text.Trim());
                    if (!string.IsNullOrEmpty(txtAmt.Text.Trim()))
                        dr["PerAmt"] = Convert.ToDecimal(txtPer.Text.Trim());
                    else
                        dr["PerAmt"] = 0m;// DBNull.Value;

                    if (!string.IsNullOrEmpty(txtAmt.Text.Trim()))
                        dr["Amount"] = Convert.ToDecimal(txtAmt.Text.Trim());
                    else
                        dr["Amount"] = 0m;// DBNull.Value;

                    dtPaySched.Rows.Add(dr);
                    if (dtPaySched != null && dtPaySched.Rows.Count > 0)
                    {
                        //dtTemp.ImportRow(dr);
                        gridPaySchedule.DataSource = dtPaySched.DefaultView;
                        //if (dtTemp != null && dtTemp.Rows.Count > 0)
                        //{
                        Decimal dTotalAmount = 0m;
                        Decimal dTotPer = 0m;
                        foreach (DataRow drTotal in dtPaySched.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["Amount"]);
                            dTotPer += Convert.ToDecimal(drTotal["PerAmt"]);
                        }
                        lblGridTotAmt.Text = Convert.ToString(dTotalAmount);
                        lblTotPer.Text = Convert.ToString(dTotPer);
                        // }
                    }
                    //else
                    //{
                    //    dtTemp.ImportRow(dr);
                    //    gridPaySchedule.DataSource = dtTemp.DefaultView;

                    //    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    //    {
                    //        Decimal dTotalAmount = 0m;
                    //        Decimal dTotPer = 0m;
                    //        foreach (DataRow drTotal in dtTemp.Rows)
                    //        {
                    //            dTotalAmount += Convert.ToDecimal(drTotal["Amount"]);
                    //            dTotPer += Convert.ToDecimal(drTotal["PerAmt"]);
                    //        }
                    //        lblGridTotAmt.Text = Convert.ToString(dTotalAmount);
                    //        lblTotPer.Text = Convert.ToString(dTotPer);
                    //    }
                    //}
                }
                if (IsEdit == true)
                {
                    DataRow EditRow = dtPaySched.Rows[EditselectedIndex];
                    EditRow["PayDate"] = dtPayDtae.Text.Trim();
                    if (!string.IsNullOrEmpty(txtAmt.Text.Trim()))
                        EditRow["PerAmt"] = decimal.Round(Convert.ToDecimal(txtPer.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        EditRow["PerAmt"] = objRounding.Round(decimal.Zero, 2);
                    if (!string.IsNullOrEmpty(txtAmt.Text.Trim()))
                        EditRow["Amount"] = decimal.Round(Convert.ToDecimal(txtAmt.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        EditRow["Amount"] = objRounding.Round(decimal.Zero, 2);

                    dtPaySched.AcceptChanges();

                    gridPaySchedule.DataSource = dtPaySched.DefaultView;
                    if (dtPaySched != null && dtPaySched.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        Decimal dTotPer = 0m;
                        foreach (DataRow drTotal in dtPaySched.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["Amount"]);
                            dTotPer += Convert.ToDecimal(drTotal["PerAmt"]);
                        }
                        lblGridTotAmt.Text = Convert.ToString(dTotalAmount);
                        lblTotPer.Text = Convert.ToString(dTotPer);
                    }
                    IsEdit = false;
                }
                ClearControls();
                txtPer.Focus();
            }
        }

        #region ClearControls()
        void ClearControls()
        {
            txtPer.Text = string.Empty;
            txtAmt.Text = string.Empty;

        }
        #endregion

        private void txtPer_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPer.Text.Trim()) && !string.IsNullOrEmpty(lblTotAmt.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(lblTotAmt.Text.Trim()) * Convert.ToDecimal(txtPer.Text.Trim()) / 100, 2);
                txtAmt.Text = Convert.ToString(objRounding.Round(Convert.ToDecimal(decimalAmount), 2));
            }
        }

        private void txtPer_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnCommit_Click(object sender, EventArgs e)
        {
            Decimal MaxAmt = getToleranceAmt() + Convert.ToDecimal(lblTotAmt.Text.Trim());
            Decimal MinAmt = Convert.ToDecimal(lblTotAmt.Text.Trim()) - getToleranceAmt();

            if (Convert.ToDecimal(lblGridTotAmt.Text.Trim()) > MaxAmt)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Scheduled amount should be equal to total order amount.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPer.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            else if (Convert.ToDecimal(lblGridTotAmt.Text.Trim()) < MinAmt)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Scheduled amount should be equal to total order amount.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPer.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            {
                if (dtPaySched != null && dtPaySched.Rows.Count > 0)
                {
                    frmCustOrd.dtPaySchedule = dtPaySched;
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

                    gridPaySchedule.DataSource = dtTemp.DefaultView;
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        Decimal dTotPer = 0m;
                        foreach (DataRow drTotal in dtTemp.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["Amount"]);
                            dTotPer += Convert.ToDecimal(drTotal["PerAmt"]);
                        }
                        lblGridTotAmt.Text = Convert.ToString(dTotalAmount);
                        lblTotPer.Text = Convert.ToString(dTotPer);
                    }
                }
            }

            IsEdit = false;
            ClearControls();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
            dtPaySched.Clear();
            dtTemp.Clear();
            lblGridTotAmt.Text = string.Empty;
            lblTotPer.Text = string.Empty;
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

                    dtPayDtae.Text = Convert.ToString(theRowToSelect["PayDate"]);
                    txtPer.Text = Convert.ToString(theRowToSelect["PerAmt"]);
                    txtAmt.Text = Convert.ToString(theRowToSelect["Amount"]);
                }
            }
            else
            {
                if (dtPaySched != null && dtPaySched.Rows.Count > 0)
                {
                    if (grdView.RowCount > 0)
                    {
                        IsEdit = true;
                        EditselectedIndex = grdView.GetSelectedRows()[0];
                        DataRow theRowToSelect = dtPaySched.Rows[EditselectedIndex];

                        dtPayDtae.Text = Convert.ToString(theRowToSelect["PayDate"]);
                        txtPer.Text = Convert.ToString(theRowToSelect["PerAmt"]);
                        txtAmt.Text = Convert.ToString(theRowToSelect["Amount"]);

                    }
                }
            }
            txtPer.Focus();
        }

        private void txtPer_Leave(object sender, EventArgs e)
        {
            txtPer.Text = !string.IsNullOrEmpty(txtPer.Text) ? decimal.Round(Convert.ToDecimal(txtPer.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
        }
        //ORDERPAYSCHEDULETOLERANCE
        private Decimal getToleranceAmt()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ORDERPAYSCHEDULETOLERANCE FROM [RETAILPARAMETERS] ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }
    }
}
