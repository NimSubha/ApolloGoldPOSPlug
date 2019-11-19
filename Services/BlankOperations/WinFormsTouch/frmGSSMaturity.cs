using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch;
namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmGSSMaturity : frmTouchBase
    {
        DataRow drGSS = null;
        DataRow drGSSCust = null;
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

        public frmGSSMaturity()
        {
            InitializeComponent();
            btnSearchCustomer.Select();
        }

        //public frmGSSMaturity()
        //{
        //    InitializeComponent();
        //}

        private void btnGSSSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustAcc.Text.Trim()))
            {
                try
                {
                    SqlConnection connection = new SqlConnection();

                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;

                    string sQry = "SELECT  GSSACCOUNTNO AS [GSSNumber]  FROM GSSACCOUNTOPENINGPOSTED" +
                                  " WHERE GSSMATURED = 0 and CustAccount='" + txtCustAcc.Text.Trim() + "'"; // should be online search to be done

                    SqlCommand cmd = new SqlCommand(sQry, connection);
                    cmd.CommandTimeout = 0;

                    DataTable dtGSSNo = new DataTable();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dtGSSNo);

                    if (dtGSSNo != null && dtGSSNo.Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtGSSNo, drGSS, "FPP Number");
                        oSearch.ShowDialog();
                        drGSS = oSearch.SelectedDataRow;
                        if (drGSS != null)
                            txtGSSNumber.Text = Convert.ToString(drGSS[0]);
                    }
                }
                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("Please select customer account.");
            }

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGSSNumber.Text.Trim()))
            {
                try
                {
                    if (drGSS != null)
                    {
                        string sGSSNo = Convert.ToString(drGSS[0]);
                        string sSalesManId = Convert.ToString(txtSM.Tag);

                        if (PosApplication.Instance.TransactionServices.CheckConnection())
                        {
                            ReadOnlyCollection<object> containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GSSMaturiry", sGSSNo, sSalesManId);
                            string sMsg = Convert.ToString(containerArray[2]);
                            MessageBox.Show(sMsg);
                            txtGSSNumber.Text = string.Empty;
                            txtSM.Tag = 0;
                            txtSM.Text = "";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to mature");
                }
            }
            else
            {
                MessageBox.Show("Please select GSS number.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e) // added on 01/05/2014 on req of S.Sharma
        {
            frmCustomerSearch obfrm = new frmCustomerSearch(this);
            obfrm.ShowDialog();

            #region old
            /*try
            {
                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                string sQry = "SELECT distinct CUSTACCOUNT,CUSTNAME FROM GSSACCOUNTOPENINGPOSTED WHERE GSSMATURED = 0 and isnull(CUSTACCOUNT,'')<>''";

                SqlCommand cmd = new SqlCommand(sQry, connection);
                cmd.CommandTimeout = 0;

                DataTable dtGSSCust = new DataTable();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtGSSCust);

                if (dtGSSCust != null && dtGSSCust.Rows.Count > 0)
                {
                    Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtGSSCust, drGSSCust, "Customer Search");
                    oSearch.ShowDialog();
                    drGSSCust = oSearch.SelectedDataRow;
                    if (drGSSCust != null)
                    {
                        txtCustAcc.Text = Convert.ToString(drGSSCust[0]);
                        txtCustName.Text = Convert.ToString(drGSSCust[1]);
                    }
                }
            }
            catch (Exception ex)
            {

            }*/
            #endregion
        }

        private void btnSM_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtSM.Tag = dialog6.SelectedEmployeeId;
                txtSM.Text = dialog6.SelectEmployeeName;
            }
        }
    }
}
