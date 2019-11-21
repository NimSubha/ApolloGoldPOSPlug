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
using System.Text.RegularExpressions;
using LSRetailPosis.Transaction;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmAddLocalCustomer : frmTouchBase
    {
        RetailTransaction RetailTrans;
        public frmAddLocalCustomer()
        {
            InitializeComponent();
        }

        public frmAddLocalCustomer(RetailTransaction RTrans)
        {
            InitializeComponent();
            RetailTrans = RTrans;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            int a=0;
            a = a + 1;
            int ab = 0;
            ab = ab + 1;
        }

        private void txtCustomerName_KeyPress(object sender, KeyPressEventArgs e)
        {
           // if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z]$")))
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"^[a-zA-Z ]*$")))
            {
                e.Handled = true;
            }
        }

        private void txtContactNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9+]$")))
            {
                e.Handled = true;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                RetailTrans.PartnerData.LCCustomerName = txtCustomerName.Text.Trim();
                RetailTrans.PartnerData.LCCustomerAddress = txtaddress.Text.Trim();
                RetailTrans.PartnerData.LCCustomerContactNo = txtContactNo.Text.Trim();

                this.Close();
            }
        }

        #region isValid()
        private bool isValid()
        {
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtCustomerName.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtaddress.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Address can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtaddress.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtContactNo.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Contact No cannot be less than Order Date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtContactNo.Focus();
                    return false;
                }

            }

            else
            {
                return true;
            }
        }
        #endregion
    }
}
