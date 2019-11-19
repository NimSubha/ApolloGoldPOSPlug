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
    public partial class frmSKUDetails : frmTouchBase
    {
        public frmSKUDetails()
        {
            InitializeComponent();
        }

        public frmSKUDetails(DataTable dtIngredients)
        {
            InitializeComponent();

            grItems.DataSource = dtIngredients;
        }

        private void frmSKUDetails_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Escape)
                this.Close();
        }
    }
}
