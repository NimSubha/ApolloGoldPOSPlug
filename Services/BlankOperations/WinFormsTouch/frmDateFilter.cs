using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using System.Data.SqlClient;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmDateFilter : Form
    {
        DateTime dtFDate = new DateTime();
        DateTime dtTDate = new DateTime();
        SqlConnection connection;
        int iMOPWiseTrans = 0;

        public frmDateFilter()
        {
            InitializeComponent();
        }

        public frmDateFilter(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
        }

        public frmDateFilter(SqlConnection conn, int iMOPWiseTransaction = 0)
        {
            InitializeComponent();
            connection = conn;
            iMOPWiseTrans = iMOPWiseTransaction;

            if (iMOPWiseTrans == 1)
            {
                label2.Visible = false;
                dateTimePicker2.Visible = false;
                label1.Text = "Trans Date";
            }
            else
            {
                label2.Visible = true;
                dateTimePicker2.Visible = true;
                label1.Text = "From Date";
            }
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            dtFDate = dateTimePicker1.Value;
            dtTDate = dateTimePicker2.Value;

            frmDashBoard objDB = new frmDashBoard(dtFDate, dtTDate, connection, 1);//MOP wise summary
            objDB.ShowDialog();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            dtFDate = dateTimePicker1.Value;
            dtTDate = dateTimePicker2.Value;

            frmDashBoard objDB = new frmDashBoard(dtFDate, dtTDate, connection, 0);//MOP wise details
            objDB.ShowDialog();
        }
    }
}
