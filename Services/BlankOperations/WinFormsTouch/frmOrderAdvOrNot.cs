using System;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOrderAdvOrNot : Form
    {
        private IApplication application;
        public int iWithAdv = 0;
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

        public frmOrderAdvOrNot()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            iWithAdv = 1;
            this.Close();
        }

        private void btnWithOutAdv_Click(object sender, EventArgs e)
        {
            iWithAdv = 0;
            this.Close();
        }
        
    }
}
