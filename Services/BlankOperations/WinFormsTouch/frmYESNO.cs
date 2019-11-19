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
    public partial class frmYESNO:frmTouchBase
    {
        public bool isY = false;
        public bool isN = false;

        public frmYESNO()
        {
            InitializeComponent();
        }

        public frmYESNO(string sYN,string sNN)
        {
            InitializeComponent();
            btnYes.Text = sYN;
            btnNo.Text = sNN;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            isY = true;
            this.Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            isN = true;
            this.Close();
        }
    }
}
