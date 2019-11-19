using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using LSRetailPosis.Settings;
using System.IO;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.Text.RegularExpressions;
using LSRetailPosis.DataAccess;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmChangePassword : Form
    {
        [Import]
        private IApplication application;
        public IPosTransaction pos { get; set; }

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

        public frmChangePassword()
        {
            InitializeComponent();
            lblStaffId.Text = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
        }

        public frmChangePassword(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            lblStaffId.Text = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
            pos = posTransaction;
            application = Application;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateControls()
        {
            //var regexItem = new Regex("^[a-zA-Z0-9 ]*$");
            if (string.IsNullOrEmpty(txtOldPass.Text))
            {
                txtOldPass.Focus();
                MessageBox.Show("Old password is required.", "Old password is required.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOldPass.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtPassword.Text))
            {
                txtPassword.Focus();
                MessageBox.Show("New password is required.", "New password is required.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtPassword.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(txtConPass.Text))
            {
                txtConPass.Focus();
                MessageBox.Show("Confirm password is requird.", "Confirm password is requird.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtConPass.Focus();
                return false;
            }

            if (txtPassword.Text != txtConPass.Text)
            {
                txtPassword.Focus();
                MessageBox.Show("Password mismatch.", "Password mismatch.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            bool userHasAccess = false;
            string operatorId = lblStaffId.Text;
            string oldPassword = txtOldPass.Text; 
            LogonData logonData = new LogonData(PosApplication.Instance.Settings.Database.Connection, PosApplication.Instance.Settings.Database.DataAreaID);
            userHasAccess = logonData.ValidatePasswordHash(ApplicationSettings.Terminal.StoreId, operatorId, LogonData.ComputePasswordHash(operatorId, oldPassword, ApplicationSettings.Terminal.StaffPasswordHashName));

            if (!userHasAccess)
            {
                MessageBox.Show("Invalid old password.", "Invalid old password.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtOldPass.Focus();
                return false;
            }
            else
                return true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            if (ValidateControls())
            {
                string sStaffId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Do you want to change password", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                    {
                        try
                        {
                            if (PosApplication.Instance.TransactionServices.CheckConnection())
                            {
                                DataSet dsPass = new DataSet();
                                DataTable dtPass = new DataTable();

                                ReadOnlyCollection<object> containerArray;
                                containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("ChangeRetailStaffPassword", sStaffId, txtPassword.Text);
                                string sMsg = Convert.ToString(containerArray[2]);

                                if (Convert.ToBoolean(containerArray[1]) == true)
                                {
                                    StringReader srPass = new StringReader(Convert.ToString(containerArray[3]));

                                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                                    {
                                        dsPass.ReadXml(srPass);
                                    }
                                    if (dsPass.Tables.Count > 0)
                                    {
                                        if (dsPass != null && dsPass.Tables[0].Rows.Count > 0)
                                        {
                                            dtPass = dsPass.Tables[0];

                                            UpdatePass(dtPass, sStaffId);
                                        }
                                    }
                                }
                                else
                                    MessageBox.Show(sMsg);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Please contact to your admin for check real time service.");
                            txtPassword.Focus();
                        }
                    }
                }
            }
        }

        private void UpdatePass(DataTable dtPass, string sStaffId)
        {
            if (dtPass != null && dtPass.Rows.Count > 0)
            {
                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                string commandChangePass = " UPDATE RETAILSTAFFTABLE SET [PASSWORD]=@PASSWORD,PASSWORDDATA=@PASSWORDDATA,CHANGEPASSWORD=@CHANGEPASSWORD" +
                                        " WHERE STAFFID = '" + sStaffId + "'";

                SqlCommand cmdPass = new SqlCommand(commandChangePass, connection);

                if (string.IsNullOrEmpty(Convert.ToString(dtPass.Rows[0]["PASSWORD"])))
                    cmdPass.Parameters.Add("@PASSWORD", SqlDbType.NVarChar, 32).Value = "";
                else
                    cmdPass.Parameters.Add("@PASSWORD", SqlDbType.NVarChar, 32).Value = Convert.ToString(dtPass.Rows[0]["PASSWORD"]);
                if (string.IsNullOrEmpty(Convert.ToString(dtPass.Rows[0]["PASSWORDDATA"])))
                    cmdPass.Parameters.Add("@PASSWORDDATA", SqlDbType.NVarChar, 128).Value = "";
                else
                    cmdPass.Parameters.Add("@PASSWORDDATA", SqlDbType.NVarChar, 128).Value = Convert.ToString(dtPass.Rows[0]["PASSWORDDATA"]);

                cmdPass.Parameters.Add("@CHANGEPASSWORD", SqlDbType.Int).Value = Convert.ToInt16(dtPass.Rows[0]["CHANGEPASSWORD"]);

                cmdPass.CommandTimeout = 0;
                int isSuccess = cmdPass.ExecuteNonQuery();
                cmdPass.Dispose();

                if (isSuccess == 1)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Password successfully changed,Login again", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        Application.RunOperation(PosisOperations.LogOff, sStaffId);
                        this.Close();
                    }
                }
            }
        }
    }
}
