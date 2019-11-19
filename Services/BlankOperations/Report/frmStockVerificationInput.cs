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

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmStockVerificationInput : Form
    {
        public frmStockVerificationInput()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dtSchemeCode = getDataTable("select isnull(VOUCHERNUM,'') VOUCHERNUM from RETAILSTOCKCOUNTHEADER");

            Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dtSchemeCode, null, "Search Stock Verification Voucher");
            Osearch.ShowDialog();

            DataRow dr = Osearch.SelectedDataRow;

            if (dr != null)
            {
                txtVou.Text = Convert.ToString(dr["VOUCHERNUM"]);
            }
        }

        private DataTable getDataTable(string _sSql)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = _sSql;

                DataTable dtComboField = new DataTable();
                //DataRow row = dtComboField.NewRow();
                //dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtVou.Text))
            {
                frmStockVerificationReport objRep = new frmStockVerificationReport(1, txtVou.Text);//Summary
                objRep.Show();
            }
            else
            {
                MessageBox.Show("Please select a valid voucher");
                btnSearch.Focus();
            }
        }

        private void btnDetails_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtVou.Text))
            {
                frmStockVerificationReport objRep = new frmStockVerificationReport(2, txtVou.Text);//Details
                objRep.Show();
            }
            else
            {
                MessageBox.Show("Please select a valid voucher");
                btnSearch.Focus();
            }
        }
    }
}
