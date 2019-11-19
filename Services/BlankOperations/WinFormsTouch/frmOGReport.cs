using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOGReport : frmTouchBase
    {

        private IApplication application;

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

        private int  iReportType = 0; 
        public frmOGReport()
        {
            InitializeComponent();
        }
        public frmOGReport(int  iTrans) // OGReport =1, SalesAndSalesReturnReport=2 , SKU Counter Movement=3
        {
            InitializeComponent();
            iReportType = iTrans;

            lblSKUNo.Visible = false;
            txtSKUNo.Visible = false;

            if (iReportType == 1)           
                lblTitle.Text = "OG Report";
            else if (iReportType == 2)
                lblTitle.Text = "Sales and Sales Return Report";
            else if (iReportType == 3)
            {
                lblSKUNo.Visible = true ;
                txtSKUNo.Visible = true;
                lblTitle.Text = "Counter Movement Report";
            }
            
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dtpTransDate != null)
            {
                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                if (iReportType == 1)
                {
                    Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmOGReportrpt reportfrm
                         = new Report.frmOGReportrpt(dtpTransDate.Value.ToShortDateString(), connection);
                    reportfrm.ShowDialog();
                }
                else if (iReportType == 2)
                {
                    Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmR_SalesAndSalesReturn reportfrm
                         = new Report.frmR_SalesAndSalesReturn(dtpTransDate.Value.ToShortDateString(), connection);
                    reportfrm.ShowDialog();
                }
                else if (iReportType == 3)
                {
                    Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmR_SKUCounterMovement reportfrm
                         = new Report.frmR_SKUCounterMovement(txtSKUNo.Text.Trim(),dtpTransDate.Value.ToShortDateString(), connection);
                    reportfrm.ShowDialog();
                }
               
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
