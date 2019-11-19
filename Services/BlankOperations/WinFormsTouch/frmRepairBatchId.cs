using System;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmRepairBatchId:Form
    {
        private IApplication application;
        public string sRepairBatchId = string.Empty;
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

        public frmRepairBatchId()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtRepairBatchId.Text.Trim()))
            {
                sRepairBatchId = txtRepairBatchId.Text.Trim();
                this.Close();
            } 
        }
    }
}
