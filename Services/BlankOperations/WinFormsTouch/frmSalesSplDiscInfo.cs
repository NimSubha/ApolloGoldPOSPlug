using System;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmSalesSplDiscInfo : Form
    {
        private IApplication application;
        public string sCodeOrRemarks = string.Empty;
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

        public frmSalesSplDiscInfo()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRemarks.Text.Trim()))
            {
                sCodeOrRemarks = txtRemarks.Text.Trim();
                this.Close();
            }
            else
            {
                MessageBox.Show("Enter customer details or remarks.");
                txtRemarks.Focus();
            }
            
        }

        private void txtRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13) // this line added on 27/05/2014
            {
                btnOk_Click(sender, e);
            }
        }
    }
}
