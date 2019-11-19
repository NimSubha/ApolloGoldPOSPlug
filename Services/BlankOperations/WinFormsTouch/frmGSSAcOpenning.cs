using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
//using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Customer;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Data;

namespace BlankOperations
{
    public partial class frmGSSAcOpenning : frmTouchBase
    {
        public string CustAddress { get; set; }
        public string CustPhoneNo { get; set; }

        [Import]
        private IApplication application;


        int iNoOfMonth = 0;

        enum SchemeType
        {
            Fixed = 0,
            Flexible = 1,
        }
        enum SchemeDepositType
        {
            Gold = 0,
            Amount = 1,
        }

        int iSchDepositeType = 0;
        decimal dMinAMt = 0m;
        int iSchType = 0;

        #region Constractor frmGSSAcOpenning
        public frmGSSAcOpenning()
        {
            InitializeComponent();
            cmbIdType.DataSource = Enum.GetValues(typeof(IdentityProof));
            txtGuardianName.Enabled = false;
            txtRelationship.Enabled = false;

            loadComboValue();
            btnSearchCustomer.Select();
        }
        #endregion

        #region loadComboValue
        private void loadComboValue()
        {
            DataTable dtSchemeCode = getDataTable("select SchemeCode FROM  GSSSchemeMaster_Posted" +
                                                    " Where ActiveScheme=1 and CloseScheme=0");
            cmbSchemeCode.DataSource = dtSchemeCode;
            cmbSchemeCode.DisplayMember = "SchemeCode";
        }
        #endregion

        #region btnSearchCustomer_Click
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 16/09/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : Searching existing customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);
            obfrm.ShowDialog();
        }
        #endregion

        #region btnNewCustomer_Click
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 16/09/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : new customer entry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNewCustomer_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.Customer obj = new Customer();
            string strCustId = string.Empty;
            string strCustName = string.Empty;
            string strCustCurrency = string.Empty;

            obj.AddNew(out strCustId, out  strCustName, out strCustCurrency);
            {
                txtCustomerAccount.Text = strCustId;
                txtCustomerName.Text = strCustName;
            }
        }
        #endregion

        #region Enum IdProof
        /// <summary>
        /// added on 16/09/2013 by RHossain
        /// </summary>
        private enum IdentityProof
        {
            Aadhar = 0,
            PAN = 1,
            Driving_License = 2,
            Passport_No = 3,
            Voter_Id = 4,
            Emp_Id = 5,
        }
        #endregion

        #region chkMinor_CheckedChanged
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 16/09/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : if nominee is minor, enter his/her Guardian name and relationship with Guardian
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkMinor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkMinor.Checked)
            {
                txtGuardianName.Enabled = true;
                txtRelationship.Enabled = true;
            }
            else
            {
                txtGuardianName.Text = string.Empty;
                txtRelationship.Text = string.Empty;
                txtGuardianName.Enabled = false;
                txtRelationship.Enabled = false;
            }

        }
        #endregion

        #region btnClose_Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            //using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Are you sure wants to close.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //{
            //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //    if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
            //    {
            this.Close();
            //}
            // }
        }
        #endregion

        #region getDataTable
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 18/09/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : passing sql, get datatable for load combo with first value empty;
        /// </summary>
        /// <param name="_sSql"></param>
        /// <returns></returns>
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
                DataRow row = dtComboField.NewRow();
                dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region btnSubmit_Click
        //private void btnSubmit_Click(object sender, EventArgs e)
        //{
        //    if(IsValid())
        //    {
        //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Do you want to save.", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
        //        {
        //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
        //            if(Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
        //            {
        //                SaveData();
        //            }
        //        }
        //    }
        //}
        #endregion

        #region SaveData

        #region Blocked
        /*
        private void SaveData()
        {
            int iIsLocalDataSaveSuccess = 0;
            int iIsRealTimeDataSaveSuccess = 0;

            SqlTransaction transaction = null;

            #region Loacl Data Save
            string commandText = " INSERT INTO [GSSACCOUNTOPENINGPOSTED]([GSSACCOUNTNO],[SCHEMECODE],[CUSTACCOUNT],[CUSTNAME]," + 
                                 " [DATAAREAID],[CREATEDBY],[IDENTITYPROOFTYPE],[IDENTITYPROOFNO],[NOMINEENAME], " +
                                 " [MOBILE],[NOMINEEPHONE],[NOMINEEEMAIL],[MINOR],[GUARDIANNAME],[RELATIONWITHMINOR],[SALESRESPONSIBLE],[RECID])" +
                                 " VALUES(@GSSACCOUNTNO,@SCHEMECODE,@CUSTACCOUNT,@CUSTNAME,@DATAAREAID,@STAFFID," +
                                 " @IDENTITYPROOFTYPE,@IDENTITYPROOFNO,@NOMINEENAME,@MOBILE,@NOMINEEPHONE,@NOMINEEEMAIL,"+
                                 " @MINOR,@GUARDIANNAME,@RELATIONWITHMINOR,@SALESRESPONSIBLE,@RECID)";
            SqlConnection connection = new SqlConnection();
            try
            {
                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();

                command.Parameters.Add("@GSSACCOUNTNO", SqlDbType.NVarChar, 20).Value = "TempAcc001";// temp acc, later it will come from ax
                command.Parameters.Add("@SCHEMECODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(cmbSchemeCode.Text.Trim());
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = txtCustomerAccount.Text.Trim();
                command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 100).Value = txtCustomerName.Text.Trim();
               
                if(application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                
                command.Parameters.Add("@IDENTITYPROOFTYPE", SqlDbType.Int).Value = Convert.ToInt16(cmbIdType.SelectedIndex);
                command.Parameters.Add("@IDENTITYPROOFNO", SqlDbType.NVarChar, 60).Value = txtIdNo.Text.Trim();
                command.Parameters.Add("@NOMINEENAME", SqlDbType.NVarChar, 60).Value = txtNName.Text.Trim();
                command.Parameters.Add("@MOBILE", SqlDbType.NVarChar, 20).Value = txtNMobile.Text.Trim();
                command.Parameters.Add("@NOMINEEPHONE", SqlDbType.NVarChar, 20).Value = txtNPhone.Text.Trim();
                command.Parameters.Add("@NOMINEEEMAIL", SqlDbType.NVarChar, 80).Value = txtNEmail.Text.Trim();
                command.Parameters.Add("@MINOR", SqlDbType.Int).Value = Convert.ToInt16(chkMinor.Checked);
                command.Parameters.Add("@GUARDIANNAME", SqlDbType.NVarChar, 60).Value = txtGuardianName.Text.Trim();
                command.Parameters.Add("@RELATIONWITHMINOR", SqlDbType.NVarChar, 20).Value = txtRelationship.Text.Trim();
                command.Parameters.Add("@SALESRESPONSIBLE", SqlDbType.NVarChar, 25).Value = cmbSalesPerson.Text.Trim();
                command.Parameters.Add("@RECID", SqlDbType.Int).Value = 1;  // temp recid, later it will come from ax
            #endregion

                command.CommandTimeout = 0;
                iIsLocalDataSaveSuccess = command.ExecuteNonQuery();

                if(iIsLocalDataSaveSuccess == 1)
                {
                    #region Real Time Data Save     
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;

                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GSSAccountCreate", txtCustomerAccount.Text.Trim()
                                                                                    , txtGuardianName.Text.Trim(), txtIdNo.Text.Trim()
                                                                                    , Convert.ToInt16(cmbIdType.SelectedIndex)
                                                                                    , ApplicationSettings.Terminal.StoreId
                                                                                    , Convert.ToInt16(chkMinor.Checked)
                                                                                    , txtNEmail.Text.Trim()
                                                                                    , txtNName.Text.Trim(), txtNPhone.Text.Trim()
                                                                                    , txtNMobile.Text.Trim(), txtRelationship.Text.Trim()
                                                                                    , ApplicationSettings.Terminal.TerminalOperator.OperatorId
                                                                                    , Convert.ToString(cmbSchemeCode.Text.Trim()));
                    }
                    #endregion
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();

                if(iIsLocalDataSaveSuccess == 1 )//|| iIsRealTimeDataSaveSuccess == 1)
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("GSS A/C openning has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);                        
                        ClearControls();                        
                        this.Close();
                    }
                }
                else
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }
        */
        #endregion

        private bool bISValidPAN(string sPANNo)
        {
            string regex = @"^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$"; //@"[A-Z]{5}[0-9]{4}[A-Z]{1}";^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$

            return Regex.IsMatch(sPANNo, regex);
        }

        private void SaveData()
        {
            int iIsLocalDataSaveSuccess = 0;

            ReadOnlyCollection<object> containerArray;

            SqlTransaction transaction = null;
            string sGSSAcNo = string.Empty;
            decimal dInstlAmt = 0m;
            int iSchemeType = 0;
            int iSchemeDepositType = 0;


            dInstlAmt = (string.IsNullOrEmpty(txtEMIAmount.Text.Trim())) ? 0m : Convert.ToDecimal(txtEMIAmount.Text.Trim());
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GSSAccountCreate", txtCustomerAccount.Text.Trim()
                                                                                 , txtGuardianName.Text.Trim(), txtIdNo.Text.Trim()
                                                                                 , Convert.ToInt16(cmbIdType.SelectedIndex)
                                                                                 , ApplicationSettings.Terminal.StoreId
                                                                                 , Convert.ToInt16(chkMinor.Checked)
                                                                                 , txtNEmail.Text.Trim()
                                                                                 , txtNName.Text.Trim(), txtNPhone.Text.Trim()
                                                                                 , txtNMobile.Text.Trim(), txtRelationship.Text.Trim()
                                                                                 , txtSalesPerson.Text.Trim()
                                                                                 , Convert.ToString(cmbSchemeCode.Text.Trim())
                                                                                 , dInstlAmt
                                                                                 , Convert.ToString(txtPassBook.Text.Trim())
                                                                                 , Convert.ToString(txtPAN.Text)
                                                                                 );

                    if (Convert.ToBoolean(containerArray[1]))
                    {
                        sGSSAcNo = Convert.ToString(containerArray[2]);
                    }

                    if (!string.IsNullOrEmpty(sGSSAcNo))
                    {
                        if (Convert.ToString(containerArray[3]) != string.Empty)
                        {
                            iSchemeType = Convert.ToInt32(containerArray[3]);
                        }

                        if (Convert.ToString(containerArray[4]) != string.Empty)
                        {
                            iSchemeDepositType = Convert.ToInt32(containerArray[4]);
                        }

                        #region Local Data Save


                        string commandText = " INSERT INTO [GSSACCOUNTOPENINGPOSTED]([GSSACCOUNTNO],[SCHEMECODE],[CUSTACCOUNT],[CUSTNAME]," +
                                             " [DATAAREAID],[CREATEDBY],[IDENTITYPROOFTYPE],[IDENTITYPROOFNO],[NOMINEENAME], " +
                                             " [MOBILE],[NOMINEEPHONE],[NOMINEEEMAIL],[MINOR],[GUARDIANNAME],[RELATIONWITHMINOR],[SALESRESPONSIBLE],[RECID]," +
                                             " [INSTALLMENTAMOUNT],[SCHEMETYPE],[SCHEMEDEPOSITTYPE],[GSSConfirm],[CLOSUREDATE],[OPENINGDATE],[MANUALPASSBOOK])" +
                                             " VALUES(@GSSACCOUNTNO,@SCHEMECODE,@CUSTACCOUNT,@CUSTNAME,@DATAAREAID,@STAFFID," +
                                             " @IDENTITYPROOFTYPE,@IDENTITYPROOFNO,@NOMINEENAME,@MOBILE,@NOMINEEPHONE,@NOMINEEEMAIL," +
                                             " @MINOR,@GUARDIANNAME,@RELATIONWITHMINOR,@SALESRESPONSIBLE,@RECID," +
                                             " @INSTALLMENTAMOUNT,@SCHEMETYPE,@SCHEMEDEPOSITTYPE,@GSSConfirm,@CLOSUREDATE,@OPENINGDATE,@MANUALPASSBOOK)"
                                             ;
                        SqlConnection connection = new SqlConnection();
                        //try
                        //{
                        if (application != null)
                            connection = application.Settings.Database.Connection;
                        else
                            connection = ApplicationSettings.Database.LocalConnection;


                        if (connection.State == ConnectionState.Closed)
                        {
                            connection.Open();
                        }

                        SqlCommand command = new SqlCommand(commandText, connection, transaction);
                        command.Parameters.Clear();

                        command.Parameters.Add("@GSSACCOUNTNO", SqlDbType.NVarChar, 20).Value = sGSSAcNo;// temp acc, later it will come from ax
                        command.Parameters.Add("@SCHEMECODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(cmbSchemeCode.Text.Trim());
                        command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = txtCustomerAccount.Text.Trim();
                        command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 100).Value = txtCustomerName.Text.Trim();

                        if (application != null)
                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                        else
                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                        command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalOperator.OperatorId;

                        command.Parameters.Add("@IDENTITYPROOFTYPE", SqlDbType.Int).Value = Convert.ToInt16(cmbIdType.SelectedIndex);
                        command.Parameters.Add("@IDENTITYPROOFNO", SqlDbType.NVarChar, 60).Value = txtIdNo.Text.Trim();
                        command.Parameters.Add("@NOMINEENAME", SqlDbType.NVarChar, 60).Value = txtNName.Text.Trim();
                        command.Parameters.Add("@MOBILE", SqlDbType.NVarChar, 20).Value = txtNMobile.Text.Trim();
                        command.Parameters.Add("@NOMINEEPHONE", SqlDbType.NVarChar, 20).Value = txtNPhone.Text.Trim();
                        command.Parameters.Add("@NOMINEEEMAIL", SqlDbType.NVarChar, 80).Value = txtNEmail.Text.Trim();
                        command.Parameters.Add("@MINOR", SqlDbType.Int).Value = Convert.ToInt16(chkMinor.Checked);
                        command.Parameters.Add("@GUARDIANNAME", SqlDbType.NVarChar, 60).Value = txtGuardianName.Text.Trim();
                        command.Parameters.Add("@RELATIONWITHMINOR", SqlDbType.NVarChar, 20).Value = txtRelationship.Text.Trim();
                        command.Parameters.Add("@SALESRESPONSIBLE", SqlDbType.NVarChar, 25).Value = txtSalesPerson.Text.Trim();
                        command.Parameters.Add("@RECID", SqlDbType.Int).Value = 1;
                        command.Parameters.Add("@INSTALLMENTAMOUNT", SqlDbType.Decimal).Value = dInstlAmt;
                        command.Parameters.Add("@SCHEMETYPE", SqlDbType.Int).Value = iSchemeType;
                        command.Parameters.Add("@SCHEMEDEPOSITTYPE", SqlDbType.Int).Value = iSchemeDepositType;
                        command.Parameters.Add("@GSSConfirm", SqlDbType.Int).Value = 1;
                        command.Parameters.Add("@CLOSUREDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(GetMaturityDate(Convert.ToString(cmbSchemeCode.Text.Trim())));
                        command.Parameters.Add("@OPENINGDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now); // added on 03/06/2014
                        // command.Parameters.Add("@NoOfMonth", SqlDbType.Int).Value = iNoOfMonth; // added on 03/06/2014
                        command.Parameters.Add("@MANUALPASSBOOK", SqlDbType.NVarChar, 20).Value = txtPassBook.Text.Trim();

                        command.CommandTimeout = 0;
                        iIsLocalDataSaveSuccess = command.ExecuteNonQuery();
                        #endregion

                    }

                    if (!string.IsNullOrEmpty(sGSSAcNo))
                    {
                        if (iIsLocalDataSaveSuccess == 1)
                        {
                            MessageBox.Show("GSS A/C(" + sGSSAcNo + ") openning has been created successfully.");
                        }
                        else
                        {
                            MessageBox.Show("GSS A/C(" + sGSSAcNo + ") openning not created locally, please execute the job from HO.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("GSS A/C openning creation failed.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(sGSSAcNo))
                {
                    MessageBox.Show("GSS A/C(" + sGSSAcNo + ") openning not created locally, please execute the job from HO.");
                }
                else
                {
                    MessageBox.Show("GSS A/C openning creation failed.");
                }
                ClearControls();
            }

        }
        #endregion

        #region IsValid
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 18/09/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose : data validation
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            decimal dSchemeAmt = 0m;
            decimal dEMIAmt = 0m;

            if (!string.IsNullOrEmpty(txtSchAmt.Text))
                dSchemeAmt = Convert.ToDecimal(txtSchAmt.Text);

            if (!string.IsNullOrEmpty(txtEMIAmount.Text))
                dEMIAmt = Convert.ToDecimal(txtEMIAmount.Text);

            //if((dEMIAmt/dSchemeAmt)

            int remainder = Convert.ToInt16(dEMIAmt % dSchemeAmt);
            if (remainder != 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("EMI amount should be in multiples of scheme amount.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtCustomerAccount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Account can not be blank and empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Name can not be blank and empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            //if(string.IsNullOrEmpty(cmbSalesPerson.Text.Trim()))
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Sales person should not be blank.", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}
            if (string.IsNullOrEmpty(cmbSchemeCode.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Scheme code can not be blank and empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(cmbIdType.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Id type can not be blank and empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtIdNo.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Id no should not be blank.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (!string.IsNullOrEmpty(txtPAN.Text))
            {
                if (!bISValidPAN(txtPAN.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Invalid PAN no.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
            }

            if (chkMinor.Checked)
            {
                if (string.IsNullOrEmpty(txtGuardianName.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Guardian name should not be blank.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(txtRelationship.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Relationship should not be blank.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
            }
            //if(string.IsNullOrEmpty(txtNName.Text.Trim()))
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Nominee name should not be blank.", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}
            //else
            //{
            //    return true;
            //}
            return true;

        }
        #endregion

        #region ClearControls
        private void ClearControls()
        {
            txtIdNo.Text = "";
            txtCustomerAccount.Text = "";
            txtCustomerAddress.Text = "";
            txtCustomerName.Text = "";
            txtPhoneNumber.Text = "";
            txtGuardianName.Text = "";
            txtNName.Text = "";
            txtNMobile.Text = "";
            txtNPhone.Text = "";
            txtNEmail.Text = "";
            txtRelationship.Text = "";
            txtSalesPerson.Text = "";
            txtEMIAmount.Text = "";
            txtPAN.Text = "";
        }
        #endregion

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                SaveData();
                ClearControls();
            }
        }

        private void txtEMIAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtEMIAmount_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEMIAmount.Text))
            {
                txtEMIAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(txtEMIAmount.Text), 2, MidpointRounding.AwayFromZero));
            }
        }

        private void btnSalesPerson_Click(object sender, EventArgs e)
        {
            DataTable dtSP = new DataTable();
            DataRow drSP = null;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            conn.Open();

            string commandText = string.Empty;

            commandText = "select R.STAFFID as Code,R.NAMEONRECEIPT as Name from RETAILSTAFFTABLE r  " +
                             " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID " +
                             " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON ORDER BY R.STAFFID";


            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtSP);

            if (conn.State == ConnectionState.Open)
                conn.Close();


            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtSP,
                    drSP = null, "Sales Person Search");
            oSearch.ShowDialog();
            drSP = oSearch.SelectedDataRow;

            if (drSP != null)
            {
                txtSalesPerson.Text = Convert.ToString(drSP["code"]);
            }
        }

        //Start : on 01/04/2014 RHossain 
        private string GetMaturityDate(string sGSSSchemeCode)
        {
            int ExMnth = 0;
            int i = 0;
            string closureDate = DateTime.Now.ToShortDateString();
            DataTable dtGSSNo = new DataTable();
            try
            {
                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                string sQry = " select ExtndMonthOfMature,NoOfMonth from GSSSchemeMaster_Posted where SchemeCode='" + sGSSSchemeCode.Trim() + "'";

                SqlCommand cmd = new SqlCommand(sQry, connection);
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtGSSNo);
                if (dtGSSNo != null && dtGSSNo.Rows.Count > 0)
                {
                    ExMnth = Convert.ToInt16(dtGSSNo.Rows[0]["ExtndMonthOfMature"]);
                    iNoOfMonth = Convert.ToInt16(dtGSSNo.Rows[0]["NoOfMonth"]);
                }
                DateTime t1 = DateTime.Parse(closureDate);

                while (i < ExMnth)
                {
                    t1 = t1.AddMonths(1);
                    i++;
                }
                closureDate = t1.AddMonths(-1).ToShortDateString();

            }
            catch (Exception ex)
            {

            }
            return closureDate;
        }

        private void cmbSchemeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetGSSSchemeInfo(cmbSchemeCode.Text.Trim());
        }

        private void GetGSSSchemeInfo(string sGSSSchemeCode)
        {
            int i = 0;
            DataTable dtGSSNo = new DataTable();
            try
            {
                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                string sQry = " select SCHEMEDEPOSITTYPE,SCHEMETYPE,isnull(MINIMUMEMIAMOUNT,0) as MINIMUMEMIAMOUNT" +
                              " from GSSSchemeMaster_Posted" +
                              " where SchemeCode='" + sGSSSchemeCode.Trim() + "'";

                SqlCommand cmd = new SqlCommand(sQry, connection);
                cmd.CommandTimeout = 0;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtGSSNo);
                if (dtGSSNo != null && dtGSSNo.Rows.Count > 0)
                {
                    iSchDepositeType = Convert.ToInt16(dtGSSNo.Rows[0]["SCHEMEDEPOSITTYPE"]);
                    iSchType = Convert.ToInt16(dtGSSNo.Rows[0]["SCHEMETYPE"]);
                    dMinAMt = Convert.ToInt16(dtGSSNo.Rows[0]["MINIMUMEMIAMOUNT"]);

                    SchemeType mySchType = (SchemeType)iSchType;
                    SchemeDepositType mySchDepT = (SchemeDepositType)iSchDepositeType;

                    txtSchType.Text = mySchType.ToString();
                    txtDepositeType.Text = mySchDepT.ToString();
                    txtSchAmt.Text = Convert.ToString(dMinAMt);
                }
            }
            catch (Exception ex)
            {

            }

        }


        // End: on 01/04/2014 RHossain 

    }
}
