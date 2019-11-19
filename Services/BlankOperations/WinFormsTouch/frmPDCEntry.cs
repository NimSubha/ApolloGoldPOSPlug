using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using BlankOperations;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using BlankOperations.WinFormsTouch;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmPDCEntry : Form
    {
        public DataTable dtCheque;
        private IApplication application;

        int EditselectedIndex = 0;
        DataTable dtTemp = new DataTable("dtTemp");
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        Rounding objRounding = new Rounding();

        string sOrderNum = string.Empty;
        public IPosTransaction pos { get; set; }
        Random randUnique = new Random();
        bool IsEdit = false;


        enum CRWPDCType
        {
            None = 0,
            Advance = 1,
            CustOrder = 2,
            FPP = 3, 
        }

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

        public frmPDCEntry()
        {
            InitializeComponent();
        }

        public frmPDCEntry(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            txtPDCReceipt.Text = GetPDCEntryReceipt();
            cmbPDCType.Focus();
            cmbPDCType.DataSource = Enum.GetValues(typeof(CRWPDCType));
        }

        private void btnSearchCustomer_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);
            obfrm.ShowDialog();
        }

        public string GetPDCEntryReceipt()
        {
            string OrderNum = string.Empty;
            OrderNum = GetNextCustomerOrderID();
            return OrderNum;
        }
        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum ReceiptTransactionType
        {
            PDCEntry = 22
        }

        public string GetNextCustomerOrderID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.PDCEntry;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                orderNumber((int)transType, funcProfileId, out mask);
                if (string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }
        #endregion

        #region GetOrderNum()  - CHANGED BY NIMBUS
        private void orderNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        #endregion

        #region GetSeedVal() - CHANGED BY NIMBUS
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.PDCEntry;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                // string queryString = " SELECT  MAX(CAST(ISNULL(SUBSTRING(CUSTORDER_HEADER.ORDERNUM,3,LEN(CUSTORDER_HEADER.ORDERNUM)),0) AS INTEGER)) + 1 from CUSTORDER_HEADER ";

                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(PDCENTRYCODE,@VAL,LEN(PDCENTRYCODE)),0) AS INTEGER)) + 1 from PDCENTRYTABLE";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                    {
                        Val = "1";
                    }

                    return Val;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }



        }

        #endregion

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
                    gridCheque.DataSource = dtItemInfo.DefaultView;
                }
            }
            if (DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                gridCheque.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtPDCReceipt.Text = "";
            txtCustomerAccount.Text = "";
            txtCustomerName.Text = "";
            gridCheque.DataSource = null;
            dtItemInfo = new DataTable();
            txtPDCReceipt.Text = GetPDCEntryReceipt();
            cmbPDCType.Focus();
            cmbPDCType.DataSource = Enum.GetValues(typeof(CRWPDCType));
            clearItemControl();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if (isValiedItem())
            {
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("PDCENTRYCODE", typeof(string));
                    dtItemInfo.Columns.Add("CHEQUEAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("CHEQUEDATE", typeof(DateTime));
                    dtItemInfo.Columns.Add("CHEQUENO", typeof(string));
                    dtItemInfo.Columns.Add("ISSUEBANK", typeof(string));
                    dtItemInfo.Columns.Add("MICRCODE", typeof(string));
                    dtItemInfo.Columns.Add("TRANSDATE", typeof(DateTime));
                    dtItemInfo.Columns.Add("PDCREFERENCE", typeof(string));
                    dtItemInfo.Columns.Add("PDCTYPE", typeof(string));
                    dtItemInfo.Columns.Add("CONFIRM_", typeof(bool));
                }
                if (IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();

                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());
                    dr["PDCENTRYCODE"] = Convert.ToString(txtPDCReceipt.Text.Trim());
                    dr["CHEQUEAMOUNT"] = Convert.ToDecimal(txtAmount.Text.Trim());
                    dr["CHEQUEDATE"] = Convert.ToDateTime(dtpChqDtae.Text.Trim());
                    dr["CHEQUENO"] = Convert.ToString(txtChqNo.Text.Trim());
                    dr["ISSUEBANK"] = Convert.ToString(txtBank.Text.Trim());
                    dr["MICRCODE"] = Convert.ToString(txtMICR.Text.Trim());
                    dr["TRANSDATE"] = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    dr["PDCREFERENCE"] = Convert.ToString(txtPDCRef.Text.Trim());

                    if (!string.IsNullOrEmpty(cmbPDCType.Text.Trim()))
                        dr["PDCTYPE"] = Convert.ToString(cmbPDCType.Text.Trim());
                    else
                        dr["PDCTYPE"] = DBNull.Value;

                    dr["CONFIRM_"] = 1;

                    dtItemInfo.Rows.Add(dr);

                    gridCheque.DataSource = dtItemInfo.DefaultView;
                }

                if (IsEdit == true)
                {
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];

                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["PDCENTRYCODE"] = Convert.ToString(txtPDCReceipt.Text.Trim());
                    EditRow["CHEQUEAMOUNT"] = Convert.ToDecimal(txtAmount.Text.Trim());
                    EditRow["CHEQUEDATE"] = Convert.ToDateTime(dtpChqDtae.Text.Trim());
                    EditRow["CHEQUENO"] = Convert.ToString(txtChqNo.Text.Trim());
                    EditRow["ISSUEBANK"] = Convert.ToString(txtBank.Text.Trim());
                    EditRow["MICRCODE"] = Convert.ToString(txtMICR.Text.Trim());
                    EditRow["TRANSDATE"] = Convert.ToDateTime(DateTime.Now.ToShortDateString());
                    EditRow["PDCREFERENCE"] = Convert.ToString(txtPDCRef.Text.Trim());
                    EditRow["PDCTYPE"] = cmbPDCType.Text;

                    dtItemInfo.AcceptChanges();

                    gridCheque.DataSource = dtItemInfo.DefaultView;
                }

                clearItemControl();
            }
        }

        private bool isValiedItem()
        {
            if (cmbPDCType.SelectedIndex == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select pdc type", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    cmbPDCType.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            

            if (string.IsNullOrEmpty(txtChqNo.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Cheque no can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtChqNo.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (txtChqNo.Text.Length !=6)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter a valid cheque no", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtChqNo.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtBank.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Bank can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtBank.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (cmbPDCType.SelectedIndex == (int)CRWPDCType.CustOrder
                || cmbPDCType.SelectedIndex == (int)CRWPDCType.FPP)
            {
                if (string.IsNullOrEmpty(txtPDCRef.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PDC reference can not be blank or empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtPDCRef.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
            }

            if (string.IsNullOrEmpty(txtMICR.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("MICR no can not be blank or empty.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMICR.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private bool isValiedEntry()
        {
            if (string.IsNullOrEmpty(txtPDCReceipt.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PDC voucher format is not setup, please contact HO", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(cmbPDCType.SelectedIndex ==(int)CRWPDCType.CustOrder || cmbPDCType.SelectedIndex ==(int)CRWPDCType.FPP)
            {
                if (string.IsNullOrEmpty(txtPDCRef.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PDC reference is requried", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtPDCRef.Focus();
                        return false;
                    }
                }
                if (cmbPDCType.SelectedIndex == (int)CRWPDCType.CustOrder && !string.IsNullOrEmpty(txtPDCRef.Text.Trim()))
                {
                    if (!IsCustOrderConfirmed(txtPDCRef.Text.Trim()))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PDC reference is not valid", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            txtPDCRef.Focus();
                            return false;
                        }
                    }
                }
                else if (cmbPDCType.SelectedIndex == (int)CRWPDCType.FPP && !string.IsNullOrEmpty(txtPDCRef.Text.Trim()))
                {
                    if (!IsGSSConfirmed(txtPDCRef.Text.Trim(), txtCustomerAccount.Text.Trim()))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PDC reference is not valid", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            txtPDCRef.Focus();
                            return false;
                        }
                    }
                }
            }

            if (dtItemInfo == null)
            {
                //ShowMessage("Altleast one item should be transfer.");
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Altleast one cheque info should be enter.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
                return false;
            }
            if (dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Altleast one cheque info should be enter.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtCustomerAccount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Account can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtCustomerAccount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer name can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtCustomerName.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtSM.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Sales man can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnSM.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (gridCheque.DataSource == null || dtItemInfo.Rows.Count == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter the pdc form properly.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void clearItemControl()
        {
            txtAmount.Text = "";
            txtChqNo.Text = "";
            txtBank.Text = "";
            txtMICR.Text = "";
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                txtPDCRef.Enabled = false;
                cmbPDCType.Enabled = false;
            }
            else
            {
                txtPDCRef.Enabled = true;
                cmbPDCType.Enabled = true;
            }
        }

        private void SavePDC()
        {
            int iHeader = 0;
            int iDetails = 0;
            SqlTransaction transaction = null;


            //MODIFIED DATE :: 04/04/2019 ; MODIFIED BY : RIPAN HOSSAIN
            #region  HEADER
            string commandText = " INSERT INTO [PDCENTRYTABLE]([CONFIRM_],[CONFIRMDATE]," +
                                 " [CUSTACCOUNT],[CUSTOMERNAME],[PDCENTRYCODE],[STOREID],[TRANSDATE],[DATAAREAID]," +
                                 " [PDCTYPE],[PDCREFERENCE],SalesManID)" +
                                 " VALUES(@CONFIRM_,@CONFIRMDATE,@CUSTACCOUNT,@CUSTOMERNAME,@PDCENTRYCODE," +
                                 " @STOREID,@TRANSDATE,@DATAAREAID," +
                                 " @PDCTYPE,@PDCREFERENCE,@SalesManID)";

            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@CONFIRM_", SqlDbType.NVarChar).Value = 1;
                command.Parameters.Add("@CONFIRMDATE", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = txtCustomerAccount.Text;
                command.Parameters.Add("@CUSTOMERNAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtCustomerName.Text);
                command.Parameters.Add("@PDCENTRYCODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtPDCReceipt.Text);
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TRANSDATE", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
                //command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Database.TerminalID;
                if (application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@PDCTYPE", SqlDbType.Int).Value = cmbPDCType.SelectedIndex;
                command.Parameters.Add("@PDCREFERENCE", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtPDCRef.Text);
                command.Parameters.Add("@SalesManID", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtSM.Tag) == null ? string.Empty : Convert.ToString(txtSM.Tag);

                command.CommandTimeout = 0;
                iHeader = command.ExecuteNonQuery();

                if (iHeader == 1)
                {
                    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        #region ORDER DETAILS
                        //MODIFIED DATE :: 18/03/2013 ; MODIFIED BY : RIPAN HOSSAIN
                        string commandDetail = " INSERT INTO [PDCENTRYDETAILS]([CHEQUEAMOUNT],[CHEQUEDATE],[CHEQUENO]," +
                                                         " [ISSUEBANK],[LINENUM],[MICRCODE],[PDCENTRYCODE],[STOREID]," +
                                                         " [TRANSDATE],[DATAAREAID],[PDCREFERENCE],[PDCTYPE])" +
                                                         " VALUES(@CHEQUEAMOUNT,@CHEQUEDATE,@CHEQUENO," +
                                                         " @ISSUEBANK,@LINENUM,@MICRCODE,@PDCENTRYCODE,@STOREID," +
                                                         " @TRANSDATE,@DATAAREAID,@PDCREFERENCE,@PDCTYPE) ";

                        for (int ItemCount = 0; ItemCount < dtItemInfo.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdDetail = new SqlCommand(commandDetail, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["CHEQUEAMOUNT"])))
                                cmdDetail.Parameters.Add("@CHEQUEAMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@CHEQUEAMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItemInfo.Rows[ItemCount]["CHEQUEAMOUNT"]);

                            cmdDetail.Parameters.Add("@CHEQUEDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dtItemInfo.Rows[ItemCount]["CHEQUEDATE"]).ToShortDateString();

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["CHEQUENO"])))
                                cmdDetail.Parameters.Add("@CHEQUENO", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@CHEQUENO", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["CHEQUENO"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["ISSUEBANK"])))
                                cmdDetail.Parameters.Add("@ISSUEBANK", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@ISSUEBANK", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["ISSUEBANK"]);

                            cmdDetail.Parameters.Add("@LINENUM", SqlDbType.Int).Value = ItemCount + 1;

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["MICRCODE"])))
                                cmdDetail.Parameters.Add("@MICRCODE", SqlDbType.NVarChar, 9).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@MICRCODE", SqlDbType.NVarChar, 9).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["MICRCODE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCENTRYCODE"])))
                                cmdDetail.Parameters.Add("@PDCENTRYCODE", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@PDCENTRYCODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCENTRYCODE"]);

                            cmdDetail.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdDetail.Parameters.Add("@TRANSDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now.Date.ToShortDateString());
                            cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCREFERENCE"])))
                                cmdDetail.Parameters.Add("@PDCREFERENCE", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@PDCREFERENCE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCREFERENCE"]);

                            //cmdDetail.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            //cmdDetail.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;

                            //int pdcType = 0;
                            //if (Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCTYPE"]) == Convert.ToString(CRWPDCType.Advance))
                            //    pdcType =(int)CRWPDCType.Advance;
                            //else if (Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCTYPE"]) == Convert.ToString(CRWPDCType.CustOrder))
                            //    pdcType = (int)CRWPDCType.CustOrder;
                            //else if (Convert.ToString(dtItemInfo.Rows[ItemCount]["PDCTYPE"]) == Convert.ToString(CRWPDCType.GSS))
                            //    pdcType = (int)CRWPDCType.GSS;
                            //else
                            //    pdcType =(int)CRWPDCType.None;

                            cmdDetail.Parameters.Add("@PDCTYPE", SqlDbType.Int).Value = cmbPDCType.SelectedIndex;

                            cmdDetail.CommandTimeout = 0;
                            iDetails = cmdDetail.ExecuteNonQuery();
                            cmdDetail.Dispose();

                        }
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();
            }

            #endregion

            #region Exception
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
            #endregion
            if (iHeader == 1 && iDetails == 1)
            {
                MessageBox.Show("PDC entry done successfully.");

                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                
                frmPDCReceipt objRec = new frmPDCReceipt(txtPDCReceipt.Text, connection);
                objRec.ShowDialog();

               
                ReInitializeControl();
            }
        }

        private void ReInitializeControl()
        {
            cmbPDCType.DataSource = Enum.GetValues(typeof(CRWPDCType));
            clearItemControl();
            txtPDCRef.Text = "";
            txtPDCReceipt.Text = GetPDCEntryReceipt();
            dtItemInfo = new DataTable();
            gridCheque.DataSource = null;
            txtCustomerAccount.Text = "";
            txtCustomerName.Text = "";
            txtPDCRef.Enabled = true;
            txtSM.Tag = 0;
            txtSM.Text = "";
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            if (isValiedEntry())
            {
                SavePDC();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtItemInfo.Rows[EditselectedIndex];
                    txtPDCReceipt.Text = Convert.ToString(theRowToSelect["PDCENTRYCODE"]);
                    txtAmount.Text = Convert.ToString(theRowToSelect["CHEQUEAMOUNT"]);
                    dtpChqDtae.Value = Convert.ToDateTime(theRowToSelect["CHEQUEDATE"]);
                    txtChqNo.Text = Convert.ToString(theRowToSelect["CHEQUENO"]);

                    txtBank.Text = Convert.ToString(theRowToSelect["ISSUEBANK"]);
                    txtMICR.Text = Convert.ToString(theRowToSelect["MICRCODE"]);
                    txtPDCRef.Text = Convert.ToString(theRowToSelect["PDCREFERENCE"]);
                    cmbPDCType.SelectedIndex = cmbPDCType.FindString(Convert.ToString(theRowToSelect["PDCTYPE"]).Trim());

                    //if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    //{
                    //    Decimal dTotalAmount = 0m;
                    //    foreach (DataRow drTotal in dtItemInfo.Rows)
                    //    {
                    //        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                    //                        + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
                    //    }
                    //    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    //}

                }
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtChqNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private bool IsCustOrderConfirmed(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT IsConfirmed  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private bool IsGSSConfirmed(string sGSSAccNo,string sCustAcc)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select GSSCONFIRM from GSSACCOUNTOPENINGPOSTED WHERE GSSACCOUNTNO='" + sGSSAccNo + "' and CUSTACCOUNT='" + sCustAcc + "' and GSSMATURED=0");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private void btnBankS_Click(object sender, EventArgs e)
        {
            string sSQl = " select CODE,DESCRIPTION from  BankMaster";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Bank Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                txtBank.Text = string.Empty;
                txtBank.Text = Convert.ToString(drRes["CODE"]);
            }
        }

        private DataTable NIM_LoadCombo(string _tableName, string _fieldName, string _condition = null, string _sqlStr = null)
        {
            try
            {
                // Open Sql Connection  
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(_sqlStr))
                    SqlComm.CommandText = _sqlStr;
                else
                    SqlComm.CommandText = "select  " + _fieldName + " " +
                                            " FROM " + _tableName + " " +
                                            " " + _condition + " ";

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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataTable dtGridItems = new DataTable();

            string commandText = "Select CUSTACCOUNT,CUSTOMERNAME,"+
                " (CASE WHEN PDCTYPE = 1 THEN 'Advance'"+
		        " WHEN PDCTYPE = 2  THEN 'Customer Order'"+
                " WHEN PDCTYPE = 3  THEN 'FPP'" +
		        " end ) AS PDCTYPE,"+
                " PDCENTRYCODE,CONVERT(VARCHAR(15),TRANSDATE,103) AS TRANSDATE,PDCREFERENCE"+
                " FROM PDCENTRYTABLE"+
                " ORDER BY PDCENTRYCODE ";

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

                objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "PDC Entry list");
                if (selRow != null)
                {
                    string sPDCRec = Convert.ToString(selRow["PDCENTRYCODE"]);
                    frmPDCReceipt objRec = new frmPDCReceipt(sPDCRec, connection);
                    objRec.ShowDialog();
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No PDC entry exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                return;
            }
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
