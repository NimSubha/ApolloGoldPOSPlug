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
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmPrintGV : Form
    {
        string sGVNumber = "";
        string sGiftVDate = "";
        string sGVAmount = "";
        
        string sGMAVoucher = "";
        int iGMAV = 0;
        public IPosTransaction pos { get; set; }
        [Import]
        private IApplication application;



        public frmPrintGV()
        {
            InitializeComponent();
        }

        public frmPrintGV(int iGMA, IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();

            iGMAV = iGMA;
            pos = posTransaction;
            application = Application;
            this.Text = "Print GMA Discount Voucher";
            lblVendorAccount.Text = "GMA Invoice No";
        }

        public frmPrintGV(string sGVNo, string sGVDate, string sGVAmt)
        {
            InitializeComponent();

            sGVNumber = sGVNo;
            sGiftVDate = sGVDate;
            sGVAmount = sGVAmt;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGVNumber.Text) && iGMAV == 0)
            {
                getGVInfo(txtGVNumber.Text);

                if (string.IsNullOrEmpty(sGVAmount))
                {
                    MessageBox.Show("Please enter a valid gift voucher");
                }
                else
                {
                    frmGiftVoucher objGV = new frmGiftVoucher(txtGVNumber.Text, sGiftVDate, sGVAmount);
                    objGV.ShowDialog();
                }
            }
            else if (!string.IsNullOrEmpty(txtGVNumber.Text) && iGMAV == 1)
            {
                if (IsGMA(txtGVNumber.Text))
                {
                    frmOfflineBillPrint objVoucher1 = new frmOfflineBillPrint(pos, txtGVNumber.Text, 1, 1);
                    objVoucher1.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Please enter a valid GMA voucher");
                }
            }
        }

        private bool IsGMA(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GMA  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private void getGVInfo(string sGVNo) 
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select ABS(CAST(ISNULL(NETAMOUNT,0)AS DECIMAL(28,2))),CONVERT(VARCHAR(11),TRANSDATE,103) TRANSDATE from RETAILTRANSACTIONSALESTRANS where COMMENT='" + sGVNo + "' and itemid=''");
            SqlCommand cmd = new SqlCommand(commandText.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sGVAmount = Convert.ToString(reader.GetValue(0));
                    sGiftVDate = Convert.ToString(reader.GetValue(1));
                }
            }
            else
            {
                sGVAmount = "";
            }

            reader.Close();
            reader.Dispose();
        }
    }
}
