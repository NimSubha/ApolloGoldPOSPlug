using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmInputGoldBookingQty : Form
    {
        public decimal dQty = 0;
        decimal dMaxQty = 0m;

        public frmInputGoldBookingQty()
        {
            InitializeComponent();
        }

        public frmInputGoldBookingQty(decimal dMaxBookedQty)
        {
            InitializeComponent();
            dMaxQty = Convert.ToDecimal(decimal.Round(dMaxBookedQty, 3, MidpointRounding.AwayFromZero)); 
        }

        
        private void txtPct_KeyPress(object sender, KeyPressEventArgs e)
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

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQty.Text.Trim()))
            {
                dQty =Convert.ToDecimal(txtQty.Text.Trim());
                if (dQty > dMaxQty)
                {
                    MessageBox.Show("Exceeding the maximum booked quantity " + dMaxQty + " .");
                    txtQty.Focus();
                }
                else
                {
                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Please enter booked quantity.");
                txtQty.Focus();
            }
        }
    }
}
