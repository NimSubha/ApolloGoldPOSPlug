using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using LSRetailPosis.Settings;
using System.IO;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmCustomerBalance : Form
    {
        [Import]
        private IApplication application;
        public IPosTransaction pos { get; set; }

        public frmCustomerBalance()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustAcc.Text) || !string.IsNullOrEmpty(txtMobile.Text))
            {
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        string sStoreId = ApplicationSettings.Database.StoreID;
                        ReadOnlyCollection<object> containerArray;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetCustomerAccountBalance", txtCustAcc.Text.Trim(), txtMobile.Text.Trim());

                        if (Convert.ToBoolean(containerArray[1]) == true)
                        {
                            lblCustBalance.Text = Convert.ToString(Convert.ToDecimal(containerArray[3]));
                        }
                        else
                        {
                            MessageBox.Show("No record found in HO");
                            txtCustAcc.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please contact to your admin for check real time service.");
                    txtCustAcc.Focus();
                }
            }
            else
            {
                MessageBox.Show("Please enter a customer account or mobile.");
                txtCustAcc.Focus();
            }
        }
    }
}
