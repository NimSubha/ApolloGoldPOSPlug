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
    public partial class CustomerSubOrderDetails : Form
    {
        public CustomerSubOrderDetails(DataTable dt)
        {
            InitializeComponent();
            dgvCustomerOrderSubDetails.AutoGenerateColumns = false;
            dgvCustomerOrderSubDetails.DataSource = dt;
        }
    }
}
