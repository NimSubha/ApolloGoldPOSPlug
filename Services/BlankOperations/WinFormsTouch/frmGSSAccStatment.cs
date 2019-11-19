using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
//using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;


namespace BlankOperations
{
    public partial class frmGSSAccStatment : frmTouchBase
    {

        public string CustAddress { get; set; }
        public string CustPhoneNo { get; set; }
        DateTime dtTransDate = DateTime.Now;
        string sInventLocationId = string.Empty;
        DataSet dsGSSAcInfo;
        DataSet dsGSSAcStaement;

        SqlConnection conn = new SqlConnection(); //DateDiff("d",Fields!DateOut.Value,Fields!DateIn.Value),
        [Import]
        private IApplication application;

        public frmGSSAccStatment()
        {
            InitializeComponent();
            btnSearchCustomer.Select();
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);
            obfrm.ShowDialog();
        }

        private void btnSearchGssAccNo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCustomerAccount.Text.Trim()))
            {
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sCustAcc = txtCustomerAccount.Text;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetGSSAccountInfo", sCustAcc);

                        dsGSSAcInfo = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsGSSAcInfo.ReadXml(srTransDetail);
                        }

                        if (dsGSSAcInfo != null && dsGSSAcInfo.Tables[0].Rows.Count > 0)
                        {
                            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch Osearch = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dsGSSAcInfo.Tables[0], null, "GSS Acc");
                            Osearch.ShowDialog();

                            DataRow dr = Osearch.SelectedDataRow;

                            if (dr != null)
                            {
                                txtGSSAccNo.Text = Convert.ToString(dr["GSSAccountNo"]);
                                txtOpDate.Text = Convert.ToString(dr["OpeningDate"]);
                                txtAmount.Text = Convert.ToString(dr["InstallmentAmount"]);
                            }
                        }
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtGSSAccNo.Text.Trim())) //isValiedSku(txtGSSAccNo.Text.Trim())
            {
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sGSSAcc = txtGSSAccNo.Text;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetGSSAccountStatement", sGSSAcc);

                        dsGSSAcStaement = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsGSSAcStaement.ReadXml(srTransDetail);
                        }
                        DataTable dtCT = new DataTable();
                        string sCustAcc = txtCustomerAccount.Text;
                        if (dsGSSAcStaement != null && dsGSSAcStaement.Tables.Count > 0)
                        {
                            if (dsGSSAcStaement != null && dsGSSAcStaement.Tables[0].Rows.Count > 0)
                            {
                                GetCurrentTrans(ref dtCT, sCustAcc, sGSSAcc);

                                if (dtCT != null && dtCT.Rows.Count > 0)
                                {
                                    foreach (DataRow drClone in dtCT.Rows)
                                    {
                                        dsGSSAcStaement.Tables[0].ImportRow(drClone);
                                        dsGSSAcStaement.Tables[0].AcceptChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            GetCurrentTrans(ref dtCT, sCustAcc, sGSSAcc);

                            if (dtCT != null && dtCT.Rows.Count > 0)
                            {
                                dsGSSAcStaement.Tables.Add(dtCT);
                            }
                        }

                        SqlConnection connection = new SqlConnection();
                        if (application != null)
                            connection = application.Settings.Database.Connection;
                        else
                            connection = ApplicationSettings.Database.LocalConnection;

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        if (dsGSSAcStaement != null && dsGSSAcStaement.Tables[0].Rows.Count > 0)
                        {
                            frmR_GSSAccStaement reportFrm = new frmR_GSSAccStaement(connection, dsGSSAcStaement, sGSSAcc, 
                                txtCustomerName.Text.Trim(), txtCustomerAddress.Text.Trim(), txtCustomerAccount.Text.Trim(), txtPhoneNumber.Text.Trim());
                            reportFrm.ShowDialog();
                        }
                        else
                        {
                            MessageBox.Show("No data found.");
                        }
                    }
                    txtAmount.Text = "";
                    txtGSSAccNo.Text = "";
                    txtOpDate.Text = "";
                }
                catch (Exception ex)
                {


                }
            }
            else
            {
                MessageBox.Show("Please select a valid GSS account.");
            }
        }

        //public bool isValiedSku(string sGssACNo)
        //{
        //    conn = application.Settings.Database.Connection;
        //    if (!string.IsNullOrEmpty(txtGSSAccNo.Text.Trim()))
        //    {
        //        if (conn.State == ConnectionState.Closed)
        //            conn.Open();
        //        Int16 iExist = 0;
        //        string commandText = " SELECT GSSACCOUNTNO FROM GSSACCOUNTOPENINGPOSTED WHERE GSSACCOUNTNO='" + sGssACNo + "'";

        //        SqlCommand command = new SqlCommand(commandText, conn);
        //        command.CommandTimeout = 0;

        //        iExist = (Int16)command.ExecuteScalar();

        //        if (conn.State == ConnectionState.Open)
        //            conn.Close();
        //        if (iExist != 0) 
        //            return true;
        //        else
        //            return false;
        //    }
        //    else
        //        return false;


        //}

        private void GetCurrentTrans(ref DataTable dtCurrentTrans, string sCustAcc, string sGSSAcc)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;//

            string sQuery = "GETCURRENTGSSTRANSACTION";
            SqlCommand command = new SqlCommand(sQuery, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtTransDate).ToShortDateString();
            command.Parameters.Add("@STORENUMBER", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command.Parameters.Add("@CUSTACC", SqlDbType.NVarChar).Value = sCustAcc;
            command.Parameters.Add("@GSSACC", SqlDbType.NVarChar).Value = sGSSAcc;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTD = new SqlDataAdapter(command);
            daTD.Fill(dtCurrentTrans);
        }

    }
}
