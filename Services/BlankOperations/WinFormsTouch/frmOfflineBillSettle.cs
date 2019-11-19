using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOfflineBillSettle : frmTouchBase
    {

        SqlConnection conn = new SqlConnection();
        public IPosTransaction pos { get; set; }

        [Import]
        private IApplication application;
        Random randUnique = new Random();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        DateTime dateSettled = new DateTime();
        DateTime dateOfflineBill = new DateTime();

        public frmOfflineBillSettle()
        {
            InitializeComponent();
        }

        public frmOfflineBillSettle(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if (isValiedItem())
            {
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ReceiptId", typeof(string));
                    dtItemInfo.Columns.Add("CustAccount", typeof(string));
                    dtItemInfo.Columns.Add("CustName", typeof(string));
                    dtItemInfo.Columns.Add("CustPAN", typeof(string));
                    dtItemInfo.Columns.Add("CustAadhar", typeof(string));
                    dtItemInfo.Columns.Add("CustMobile", typeof(string));
                    dtItemInfo.Columns.Add("CustLoyaltyNo", typeof(string));
                    dtItemInfo.Columns.Add("CustGSTIN", typeof(string));
                    dtItemInfo.Columns.Add("CustAddress", typeof(string));
                    dtItemInfo.Columns.Add("ISSETTLED", typeof(bool));
                    dtItemInfo.Columns.Add("SettledInvNo", typeof(string));
                    dtItemInfo.Columns.Add("SettledDate", typeof(string));
                    dtItemInfo.Columns.Add("STAFFID", typeof(string));
                }
                if (IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();

                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());

                    dr["ReceiptId"] = Convert.ToString(txtRefInvoiceNo.Text.Trim());
                    dr["CustAccount"] = Convert.ToString(txtCustAcc.Text.Trim());
                    dr["CustName"] = Convert.ToString(txtCustName.Text.Trim());
                    dr["CustPAN"] = Convert.ToString(txtCustPAN.Text.Trim());
                    dr["CustAadhar"] = Convert.ToString(txtCustAadhar.Text.Trim());
                    dr["CustMobile"] = Convert.ToString(txtCustMobile.Text.Trim());
                    dr["CustLoyaltyNo"] = Convert.ToString(txtCustLoyalty.Text.Trim());
                    dr["CustGSTIN"] = Convert.ToString(txtCustGSTIN.Text.Trim());
                    dr["CustAddress"] = Convert.ToString(txtAddress.Text.Trim());
                    dr["ISSETTLED"] = (string.IsNullOrEmpty(txtSettleBill.Text)) ? 0 : 1;
                    dr["SettledInvNo"] = Convert.ToString(txtSettleBill.Text.Trim());
                    dr["SettledDate"] = Convert.ToString(dateSettled.ToShortDateString());
                    dr["STAFFID"] = ApplicationSettings.Terminal.TerminalOperator.OperatorId;

                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                }

                clearItemControl();
            }
        }

        private bool isValiedItem()
        {
            bool bResult = true;
            if (string.IsNullOrEmpty(txtRefInvoiceNo.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select a offline bill", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    btnSearchBill.Focus();
                    bResult= false;
                }
            }
            if (string.IsNullOrEmpty(txtSettleBill.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter a proper settlement bill", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtSettleBill.Focus();
                    bResult = false;
                }
            }

            if (IsExist(txtSettleBill.Text.Trim()))
            {
                dateSettled = GetSettlementDate(txtSettleBill.Text.Trim());

                dateOfflineBill = GetOfflineBillDate(txtRefInvoiceNo.Text.Trim());

                if (string.IsNullOrEmpty(Convert.ToString(dateSettled.ToShortDateString())))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter a proper settlement bill", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtSettleBill.Focus();
                        bResult = false;
                    }
                }
                else if (Convert.ToDateTime(dateSettled.ToShortDateString()) < Convert.ToDateTime(dateOfflineBill.ToShortDateString()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Offline bill date can not be greater than settlement bill date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtSettleBill.Focus();
                        bResult = false;
                    }
                }
                else
                    bResult = true;
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Invalid settlement bill no. entered", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtSettleBill.Focus();
                    bResult = false;
                }
            }


            return bResult;
           
        }

        private void clearItemControl()
        {
            txtRefInvoiceNo.Text = "";
            txtCustAcc.Text = "";
            txtCustName.Text = "";
            txtCustPAN.Text = "";
            txtCustAadhar.Text = "";
            txtCustMobile.Text = "";
            txtCustLoyalty.Text = "";
            txtCustGSTIN.Text = "";
            txtAddress.Text = "";
            txtSettleBill.Text = "";
            txtSettleBill.Text = "";

        }

        private void btnSearchBill_Click(object sender, EventArgs e)
        {
            DataTable dtGridItems = new DataTable();

            string commandText = "Select ReceiptId,CONVERT(VARCHAR(15),TransDate,103) AS TransDate ,CustAccount";
            commandText += " ,CustName ,CustMobile ,CustPAN,CustAadhar,CustGSTIN,CustLoyaltyNo ";
            commandText += " ,CustAddress FROM CRWOfflineBillHeader where ISSETTLED=0";
            commandText += " ORDER BY ReceiptId ";

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);

            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            dtGridItems = new DataTable();
            dtGridItems.Load(reader);
            if (dtGridItems != null && dtGridItems.Rows.Count > 0)
            {
                DataRow selRow = null;
                Dialog.Dialog objCustOrderSearch = new Dialog.Dialog();

                objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Offline Bill");
                if (selRow != null)
                {
                    txtRefInvoiceNo.Text = Convert.ToString(selRow["ReceiptId"]);
                    txtCustAcc.Text = Convert.ToString(selRow["CustAccount"]);
                    txtCustName.Text = Convert.ToString(selRow["CustName"]);
                    txtCustPAN.Text = Convert.ToString(selRow["CustPAN"]);
                    txtCustMobile.Text = Convert.ToString(selRow["CustMobile"]);
                    txtCustLoyalty.Text = Convert.ToString(selRow["CustLoyaltyNo"]);
                    txtCustGSTIN.Text = Convert.ToString(selRow["CustGSTIN"]);
                    txtCustAadhar.Text = Convert.ToString(selRow["CustAadhar"]);
                    txtAddress.Text = Convert.ToString(selRow["CustAddress"]);
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No offline bill exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                return;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];
                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }
            }
            if (DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsExist(string sReceiptId)
        {
            bool bResult = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISXIST INT  SET @ISXIST = 0 IF EXISTS (SELECT TOP 1 TRANSDATE from RETAILTRANSACTIONTABLE where RECEIPTID='" + sReceiptId + "' and ENTRYSTATUS=0)" +
                                 " BEGIN SET @ISXIST = 1 END SELECT @ISXIST";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bResult = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();
            command.Dispose();

            return bResult;
        }

        private static DateTime GetSettlementDate(string sReceiptId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT TOP 1 TRANSDATE from RETAILTRANSACTIONTABLE where RECEIPTID='" + sReceiptId + "' and ENTRYSTATUS=0");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            DateTime dateResult = Convert.ToDateTime(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dateResult;
        }

        private static DateTime GetOfflineBillDate(string sOfflineReceipt)  
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT TOP 1 TRANSDATE from CRWOfflineBillHeader where RECEIPTID='" + sOfflineReceipt + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            DateTime dateResult = Convert.ToDateTime(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dateResult;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (isValied())
            {
                SaveOfflineBillSettlement();
            }
        }

        private void SaveOfflineBillSettlement()
        {
            int iInsert = 0;

            SqlTransaction transaction = null;

            #region Offline Bill update

            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                SqlCommand command = new SqlCommand();
            #endregion

                #region Bill
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    int iL = 1;
                    foreach (DataRow dr in dtItemInfo.Rows)
                    {
                        string commandText = " INSERT INTO [CRWOfflineBillSettlement](ReceiptId,TransDate," +
                                " [STOREID],[TERMINALID],[DATAAREAID],[CustAccount]," +
                                " [CustName],[CustPAN],[CustAadhar],[CustMobile], " +
                                " [CustLoyaltyNo],[CustGSTIN],[CustAddress],[StaffId]," +
                                " [Remarks],ISSETTLED,SETTLEDINVOICE,SETTLEDINVOICEDATE)" +
                                " VALUES(@ReceiptId,@TransDate,@STOREID,@TERMINALID,@DATAAREAID," +
                                " @CustAccount,@CustName,@CustPAN,@CustAadhar,@CustMobile,@CustLoyaltyNo,@CustGSTIN," +
                                " @CustAddress,@StaffId,@Remarks,@ISSETTLED,@SETTLEDINVOICE,@SETTLEDINVOICEDATE)";

                        commandText += " UPDATE CRWOfflineBillHeader SET ISSETTLED=@ISSETTLED,SETTLEDINVOICE=@SETTLEDINVOICE,SETTLEDINVOICEDATE=@SETTLEDINVOICEDATE WHERE ReceiptId=@ReceiptId";

                        transaction = connection.BeginTransaction();

                        using (command = new SqlCommand(commandText, connection, transaction))
                        {
                            command.CommandTimeout = 0;
                            command.Parameters.Add("@ReceiptId", SqlDbType.NVarChar).Value = Convert.ToString(dr["ReceiptId"].ToString());
                            command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now).ToShortDateString();
                            command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            if (application != null)
                                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                            else
                                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            command.Parameters.Add("@CustAccount", SqlDbType.NVarChar, 20).Value = Convert.ToString(dr["CustAccount"].ToString());
                            command.Parameters.Add("@CustName", SqlDbType.NVarChar, 60).Value = Convert.ToString(dr["CustName"].ToString());
                            command.Parameters.Add("@CustPAN", SqlDbType.NVarChar, 10).Value = Convert.ToString(dr["CustPAN"].ToString());
                            command.Parameters.Add("@CustAadhar", SqlDbType.NVarChar, 20).Value = Convert.ToString(dr["CustAadhar"].ToString());
                            command.Parameters.Add("@CustMobile", SqlDbType.NVarChar, 13).Value = Convert.ToString(dr["CustMobile"].ToString());
                            command.Parameters.Add("@CustLoyaltyNo", SqlDbType.NVarChar, 20).Value = Convert.ToString(dr["CustLoyaltyNo"].ToString());
                            command.Parameters.Add("@CustGSTIN", SqlDbType.NVarChar, 20).Value = Convert.ToString(dr["CustGSTIN"].ToString());
                            command.Parameters.Add("@CustAddress", SqlDbType.NVarChar, 250).Value = Convert.ToString(dr["CustAddress"].ToString());
                            command.Parameters.Add("@StaffId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dr["StaffId"].ToString());
                            command.Parameters.Add("@Remarks", SqlDbType.NVarChar, 250).Value = "";

                            command.Parameters.Add("@ISSETTLED", SqlDbType.Bit).Value = Convert.ToBoolean(dr["ISSETTLED"].ToString());
                            command.Parameters.Add("@SETTLEDINVOICE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dr["SettledInvNo"].ToString());
                            command.Parameters.Add("@SETTLEDINVOICEDATE", SqlDbType.DateTime).Value = Convert.ToString(dr["SettledDate"].ToString());

                            command.ExecuteNonQuery();

                            command.Dispose();
                            iL++;
                        }
                    }
                }
                #endregion
                command.CommandTimeout = 0;
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();

                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Offline Bill has been settled successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                    clearItemControl();
                    grItems.DataSource = null;
                    dtItemInfo.Clear();
                    dtItemInfo = new DataTable();
                }

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private bool isValied()
        {
            if (dtItemInfo == null || dtItemInfo.Rows.Count == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Items are there in settlment line", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else
                return true;
        }
    }
}
