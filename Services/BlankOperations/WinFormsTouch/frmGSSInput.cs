using System;
using System.Data;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmGSSInput : frmTouchBase
    {
        public bool GoldFixing = false;
        public double Amount = 0D;
        public int months = 0;
        public string GSSnumber = string.Empty;

        public bool bStatus = true;


        DataTable dtGoldSS = new DataTable();
        DataRow drGoldSS = null;

        public frmGSSInput()
        {
            InitializeComponent();
        }

        public frmGSSInput(DataTable dtGSS, decimal amount)
        {
            InitializeComponent();
            dtGoldSS = dtGSS;
            txtAmount.Text = !string.IsNullOrEmpty(Convert.ToString(amount)) ? Convert.ToString(amount) : string.Empty;
            txtMonths.Text = "1";
            //txtMonths.Enabled = false;
            txtAmount.Enabled = false;
        }

        private void btnGSSNo_Click(object sender, EventArgs e)
        {
            if (dtGoldSS != null && dtGoldSS.Rows.Count > 0)
            {
                Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtGoldSS, drGoldSS, "GSS Number");
                oSearch.ShowDialog();
                drGoldSS = oSearch.SelectedDataRow;
                if (drGoldSS != null)
                {
                    txtGSSNo.Text = string.Empty;
                    txtGSSNo.Text = Convert.ToString(drGoldSS["GSSACCOUNTNO"]);
                    txtGSSNo.ReadOnly = true;
                    if (Convert.ToString(drGoldSS["SCHEMETYPE"]).ToUpper().Trim() == "FIXED")
                    {
                        txtAmount.Text = Convert.ToString(drGoldSS["INSTALLMENTAMOUNT"]);// !string.IsNullOrEmpty(txtAmount.Text) ? txtAmount.Text : string.Empty;
                        //txtMonths.Text = string.Empty;
                        //txtMonths.Enabled = false;
                        txtAmount.ReadOnly = true;
                    }
                    else
                    {
                        txtAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? txtAmount.Text : string.Empty;
                        //txtMonths.Text = string.Empty;
                        //txtMonths.Enabled = false;
                        txtAmount.ReadOnly = true;
                    }
                    if (drGoldSS["SCHEMEDEPOSITTYPE"].ToString().ToUpper().Trim() == "GOLD")
                        GoldFixing = true;
                    else
                        GoldFixing = false;

                }
            }
            else
            {
                LSRetailPosis.POSControls.POSFormsManager.ShowPOSMessageDialog(999990);
                
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                Amount = Convert.ToDouble(txtAmount.Text);
                months = string.IsNullOrEmpty(Convert.ToString(txtMonths.Text)) ? 0 : Convert.ToInt32(txtMonths.Text);
                GSSnumber = Convert.ToString(txtGSSNo.Text);
                this.Close();
            }
        }
              
        private bool ValidateControls()
        {
            if (string.IsNullOrEmpty(txtGSSNo.Text))
            {
                txtGSSNo.Focus();
                MessageBox.Show("GSS Number is Empty.", "GSS Number is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (string.IsNullOrEmpty(txtAmount.Text))
            {
                txtGSSNo.Focus();
                MessageBox.Show("Amount cannot be Empty.", "Amount cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (drGoldSS != null && Convert.ToString(drGoldSS["SCHEMETYPE"]).ToUpper().Trim() == "FIXED" && string.IsNullOrEmpty(txtMonths.Text))
            {
                txtMonths.Focus();
                MessageBox.Show("No. of Months cannot be Empty.", "No. of Months cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }


            if (PosApplication.Instance.TransactionServices.CheckConnection())
            {
            
                bool bIsValid = false;
                ReadOnlyCollection<object> containerArray;

                containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("validateGSSTransaction", txtGSSNo.Text, txtMonths.Text, txtAmount.Text);

                bIsValid = Convert.ToBoolean(containerArray[1]);

                if (!bIsValid)
                {
                    string strMsg = Convert.ToString(containerArray[2]);
                    MessageBox.Show(strMsg, strMsg, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                return false;
            }

            return true;
        }
       
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (drGoldSS != null && txtMonths.Text != string.Empty)
            {
                if (Convert.ToString(drGoldSS["SCHEMETYPE"]).ToUpper().Trim() == "FIXED")
                {
                    txtAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(drGoldSS["INSTALLMENTAMOUNT"]) * Convert.ToDecimal(txtMonths.Text), 2, MidpointRounding.AwayFromZero));
                }
            }
            if (txtMonths.Text == string.Empty)
                txtAmount.Text = string.Empty;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnOK_Click(sender, e);
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
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnOK_Click(sender, e);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bStatus = false;
            this.Close();
        }

        private void txtGSSNo_Leave(object sender, EventArgs e)
        {
            if(dtGoldSS != null && dtGoldSS.Rows.Count > 0)
            {
                foreach(DataRow drGoldSS in dtGoldSS.Select("GSSACCOUNTNO='" + Convert.ToString(txtGSSNo.Text.Trim()) + "'"))
                {
                    //drGoldSS = dtGoldSS.Select("GSSACCOUNTNO='" + Convert.ToString(txtGSSNo.Text.Trim()) + "'");

                    if(drGoldSS != null)
                    {
                        txtGSSNo.Text = string.Empty;
                        txtGSSNo.Text = Convert.ToString(drGoldSS["GSSACCOUNTNO"]);
                        txtGSSNo.ReadOnly = true;
                        if(Convert.ToString(drGoldSS["SCHEMETYPE"]).ToUpper().Trim() == "FIXED")
                        {
                            txtAmount.Text = Convert.ToString(drGoldSS["INSTALLMENTAMOUNT"]);// !string.IsNullOrEmpty(txtAmount.Text) ? txtAmount.Text : string.Empty;
                            //txtMonths.Text = string.Empty;
                            //txtMonths.Enabled = false;
                            //txtAmount.ReadOnly = true;
                        }
                        else
                        {
                            txtAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? txtAmount.Text : string.Empty;
                            //txtMonths.Text = string.Empty;
                            //txtMonths.Enabled = false;
                            //txtAmount.ReadOnly = true;
                        }
                        if(drGoldSS["SCHEMEDEPOSITTYPE"].ToString().ToUpper().Trim() == "GOLD")
                            GoldFixing = true;
                        else
                            GoldFixing = false;

                    }
                }
            }
            else
            {
                LSRetailPosis.POSControls.POSFormsManager.ShowPOSMessageDialog(999990);
            }
           
        }
    }
}
