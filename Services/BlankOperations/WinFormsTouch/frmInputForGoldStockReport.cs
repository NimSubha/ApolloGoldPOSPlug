using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmInputForGoldStockReport:Form
    {

        public IPosTransaction pos { get; set; }
        [Import]
        private IApplication application;


        enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
            GiftVoucher = 18,
        }
        SqlConnection sqlConn;
        int isSammary = 0;
        public frmInputForGoldStockReport()
        {
            InitializeComponent();
        }

        public frmInputForGoldStockReport(SqlConnection conn, int iStockSummaryOrDetails=0)
        {
            InitializeComponent();
           // cCmbMetalType.Properties.DataSource = GetProductCode();// Enum.GetValues(typeof(MetalType));
            DataTable dtPC = GetProductCode();

            if (dtPC.Rows.Count > 0)
            {
                cCmbMetalType.Properties.DataSource = dtPC;
                cCmbMetalType.Properties.DisplayMember = "PRODUCTTYPECODE";
                cCmbMetalType.Properties.ValueMember = "PRODUCTTYPECODE";
            }

           
            sqlConn = conn;
            isSammary = iStockSummaryOrDetails;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string displayArticleValues = cCmbMetalType.Properties.GetDisplayText(cCmbMetalType.EditValue);
            List<string> lMatlType = displayArticleValues.Split(cCmbMetalType.Properties.SeparatorChar).ToList();
            string sMetalType = string.Format("{0}", string.Join(",", lMatlType.Select(i => i.Replace(" ", "")).ToArray()));

            if(!string.IsNullOrEmpty(sMetalType) && isSammary==0)
            {
                frmGoldStockSummery objGSR = new frmGoldStockSummery(sMetalType, sqlConn);
                objGSR.ShowDialog();
            }
            else if(!string.IsNullOrEmpty(sMetalType) && isSammary == 1)
            {
                frmMetalStockDetails objGSR = new frmMetalStockDetails(sMetalType, sqlConn);
                objGSR.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select accounted under.");
                cCmbMetalType.Focus();
            }
        }

        private DataTable GetProductCode()   
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            DataTable dt = new DataTable();
            string commandText = "select distinct PRODUCTTYPECODE from INVENTTABLE";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;
        }

    }
}
