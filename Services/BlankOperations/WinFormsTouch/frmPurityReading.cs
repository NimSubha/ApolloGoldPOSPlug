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

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmPurityReading : Form
    {

        public decimal dReading1 = 0;
        public decimal dReading2 = 0;
        public decimal dReading3 = 0;
        public string sCode = "";
        public string sName = "";
        decimal dMaxPurity = 0;
        decimal dMaxPurityDiff = 0;


        public frmPurityReading()
        {
            InitializeComponent();
            getPurityParamValue();
        }
        public frmPurityReading(decimal dR1, decimal dR2, decimal dR3, string sPCode, string sPName)
        {
            InitializeComponent();
            getPurityParamValue();

            txtR1.Text = Convert.ToString(dR1);
            txtR2.Text = Convert.ToString(dR2);
            txtR3.Text = Convert.ToString(dR3);
            sCode = sPCode;
            sName = sPName;

            txtCode.Text = sCode;
            txtName.Text = sName;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnSubmit_Click(sender, e);
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                dReading1 = Convert.ToDecimal(txtR1.Text);
                dReading2 = Convert.ToDecimal(txtR2.Text);
                dReading3 = Convert.ToDecimal(txtR3.Text);
                sCode = txtCode.Text.Trim();
                sName = txtName.Text.Trim();
                this.Close();
            }

        }

        private void txtR1_KeyPress(object sender, KeyPressEventArgs e)
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

            }
        }

        private void txtR2_KeyPress(object sender, KeyPressEventArgs e)
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

            }
        }

        private void txtR3_KeyPress(object sender, KeyPressEventArgs e)
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
                btnSubmit_Click(sender, e);
            }
        }

        #region Validate Controls
        private bool ValidateControls()
        {
            decimal dR1 = string.IsNullOrEmpty(Convert.ToString(txtR1.Text)) ? 0 : Convert.ToDecimal(txtR1.Text);
            decimal dR2 = string.IsNullOrEmpty(Convert.ToString(txtR2.Text)) ? 0 : Convert.ToDecimal(txtR2.Text);
            decimal dR3 = string.IsNullOrEmpty(Convert.ToString(txtR3.Text)) ? 0 : Convert.ToDecimal(txtR3.Text);


            if (dR1 != 0 && dR2 != 0 && dR3 != 0)
            {
                if (string.IsNullOrEmpty(txtR1.Text))
                {
                    txtR1.Focus();
                    MessageBox.Show("Reading 1 is Empty.", "Reading 1  is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (string.IsNullOrEmpty(txtR2.Text))
                {
                    txtR2.Focus();
                    MessageBox.Show("Reading 2 is Empty.", "Reading 2 is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (string.IsNullOrEmpty(txtR3.Text))
                {
                    txtR3.Focus();
                    MessageBox.Show("Reading 3 is Empty.", "Reading 3 is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                decimal result1 = Math.Abs(dR1 - dR2);
                decimal result2 = Math.Abs(dR1 - dR3);
                decimal result3 = Math.Abs(dR2 - dR3);

                if (result1 >= dMaxPurityDiff || result2 >= dMaxPurityDiff || result3 >= dMaxPurityDiff)
                {
                    txtR1.Focus();
                    MessageBox.Show("Different between these 3 reading should be less than'" + dMaxPurityDiff + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            else
                return false;

            return true;
        }
        #endregion

        private void btnFetch_Click(object sender, EventArgs e)
        {
            DataTable dtSP = new DataTable();
            DataRow drSP = null;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            conn.Open();

            string commandText = string.Empty;

            commandText = "select R.STAFFID as Code,R.NAMEONRECEIPT as Name from RETAILSTAFFTABLE r  " +
                             " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID " +
                             " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON ORDER BY R.STAFFID";


            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtSP);

            if (conn.State == ConnectionState.Open)
                conn.Close();


            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtSP,
                    drSP = null, "Sales Person Search");
            oSearch.ShowDialog();
            drSP = oSearch.SelectedDataRow;

            if (drSP != null)
            {
                txtCode.Text = Convert.ToString(drSP["code"]);
                txtName.Text = Convert.ToString(drSP["Name"]);
            }
        }

        private void getPurityParamValue()
        {
            DataTable dtPV = new DataTable();
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            conn.Open();

            string sQry = " SELECT isnull(MAXPURITYDIFF,0) MAXPURITYDIFF FROM RETAILPARAMETERS ";//MAXPURITYREADING

            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dtPV);
            if (dtPV != null && dtPV.Rows.Count > 0)
            {
                dMaxPurity = 1;
                dMaxPurityDiff = Convert.ToDecimal(dtPV.Rows[0]["MAXPURITYDIFF"]);
            }
        }

        private void txtR1_TextChanged(object sender, EventArgs e)
        {
            //decimal dR1 = string.IsNullOrEmpty(Convert.ToString(txtR1.Text)) ? 0 : Convert.ToDecimal(txtR1.Text);
            //decimal dR2 = string.IsNullOrEmpty(Convert.ToString(txtR2.Text)) ? 0 : Convert.ToDecimal(txtR2.Text);
            //decimal dR3 = string.IsNullOrEmpty(Convert.ToString(txtR3.Text)) ? 0 : Convert.ToDecimal(txtR3.Text);
            if (!string.IsNullOrEmpty(Convert.ToString(txtR1.Text)))
            {
                string value = Convert.ToString(txtR1.Text);
                decimal dR1;
                if (Decimal.TryParse(value, out dR1))
                {

                    if (dR1 != 0)
                    {
                        if (string.IsNullOrEmpty(txtR1.Text))
                        {
                            txtR1.Focus();
                            MessageBox.Show("Reading 1 is Empty.", "Reading 1  is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        if (dR1 > dMaxPurity)
                        {
                            txtR1.Focus();
                            MessageBox.Show("Reading should not greater than '" + dMaxPurity + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void txtR2_TextChanged(object sender, EventArgs e)
        {
            //decimal dR2 = string.IsNullOrEmpty(Convert.ToString(txtR2.Text)) ? 0 : Convert.ToDecimal(txtR2.Text);
            if (!string.IsNullOrEmpty(Convert.ToString(txtR2.Text)))
            {
                string value = Convert.ToString(txtR2.Text);
                decimal dR2;
                if (Decimal.TryParse(value, out dR2))
                {
                    if (dR2 != 0)
                    {
                        if (string.IsNullOrEmpty(txtR2.Text))
                        {
                            txtR2.Focus();
                            MessageBox.Show("Reading 2 is Empty.", "Reading 2 is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        if (dR2 > dMaxPurity)
                        {
                            txtR2.Focus();
                            MessageBox.Show("Reading should not greater than '" + dMaxPurity + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }

        private void txtR3_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Convert.ToString(txtR3.Text)))
            {
                string value = Convert.ToString(txtR3.Text);
                decimal dR3;
                if (Decimal.TryParse(value, out dR3))
                {
                    //decimal dR3 = string.IsNullOrEmpty(Convert.ToString(txtR3.Text)) ? 0 : Convert.ToDecimal(txtR3.Text);

                    if (dR3 != 0)
                    {
                        if (string.IsNullOrEmpty(txtR3.Text))
                        {
                            txtR3.Focus();
                            MessageBox.Show("Reading 3 is Empty.", "Reading 3 is Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        if (dR3 > dMaxPurity)
                        {
                            txtR3.Focus();
                            MessageBox.Show("Reading should not greater than '" + dMaxPurity + "'", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
        }
    }
}
