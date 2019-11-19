
using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmRepairReturn : frmTouchBase
    {
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

        public DataTable dtRepairReturnCashAdvanceDataSku;
        public decimal dCashAdv = 0M;
        public decimal dMakinCharges = 0M;
        public decimal dRepairAmntDiff = 0M;
        public string sSkuNumber = string.Empty;
        public DataTable dtRepairReturnQtyData;
        public decimal dRepairNettWt = 0M;
        public decimal dRepairReturnNettWt = 0M;
        public decimal dRepairNettWtDiff = 0M;
        string sAdvAdjustmentId = string.Empty;
        string sStore = string.Empty;
        string sTerminal = string.Empty;
        public bool IsRepairReturn = false;
        public string sRepairRetId = string.Empty;
        private bool bInHouse = false;
        public string sInventBatchId = string.Empty;
        public decimal dRepairExpense = 0M;

        string sEmail = "";
        string sCompanyName = string.Empty;
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations oBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        public DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                 LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
                 LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);

        public frmRepairReturn()
        {
            InitializeComponent();
        }

        public frmRepairReturn(IPosTransaction posTransaction, IApplication Application, DataRow drSelect,int iInHouse=0)
        {
            InitializeComponent();

            application = Application;
            pos = posTransaction;
            bInHouse =Convert.ToBoolean(iInHouse);

            txtReturnNo.Text = GetOrderNum();
            txtOrderNo.Text = Convert.ToString(drSelect["REPAIRID"]);

            if (Convert.ToString(drSelect["OrderDate"]) != "")
            {
                dTPickerOrderDate.Text = Convert.ToString(drSelect["OrderDate"]);
            }

            txtCustomerAccount.Text = Convert.ToString(drSelect["CUSTACCOUNT"]);
            txtCustomerName.Text = Convert.ToString(drSelect["CustName"]);

            txtItemId.Text = Convert.ToString(drSelect["ITEMID"]);

            sAdvAdjustmentId = Convert.ToString(drSelect["TRANSACTIONID"]);
            sStore = Convert.ToString(drSelect["RETAILSTOREID"]);
            sTerminal = Convert.ToString(drSelect["RETAILTERMINALID"]);

            if(iInHouse == 1)
            {
                GetRepairReturnCashAdvSkuData(ref dtRepairReturnCashAdvanceDataSku, Convert.ToString(drSelect["REPAIRID"]));
                GetRepairReturnQtyData(Convert.ToString(drSelect["REPAIRID"]));
            }
            else
            {
                GetRepairReturnCashAdvOutSideData(ref dtRepairReturnCashAdvanceDataSku, Convert.ToString(drSelect["REPAIRID"]));
            }
        }

        private void GetRepairReturnQtyData(string repairId)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("  SELECT A.REPAIRID,A.QTY [REPAIRNETTWT],B.QTY [RETURNNETTWT],(A.QTY-B.QTY) [NETTWTDIFF] ");
            sbQuery.AppendLine(" FROM RETAILREPAIRDETAIL A ");
            sbQuery.AppendLine(" JOIN RETAILREPAIRRETURNDETAIL B ON A.REPAIRID=B.REPAIRID ");
            //sbQuery.AppendLine("  ");
            //sbQuery.AppendLine("  ");
            //sbQuery.AppendLine("  ");
            sbQuery.AppendLine("  WHERE B.REPAIRID='" + repairId + "' ");
            sbQuery.AppendLine(" AND B.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' ");
            sbQuery.AppendLine(" AND B.RETAILTERMINALID='" + ApplicationSettings.Database.TerminalID + "' ");
            sbQuery.AppendLine(" AND B.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            using (SqlCommand cmd = new SqlCommand { CommandText = sbQuery.ToString(), CommandTimeout = 0, Connection = connection })
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    if (dtRepairReturnQtyData == null)
                    {
                        dtRepairReturnQtyData = new DataTable();
                    }
                    da.Fill(dtRepairReturnQtyData);
                }
            }
            if (dtRepairReturnQtyData != null && dtRepairReturnQtyData.Rows.Count > 0)
            {
                DataRow dr = dtRepairReturnQtyData.Rows[0];
                dRepairNettWt = Convert.ToDecimal(dr["REPAIRNETTWT"]);
                dRepairReturnNettWt = Convert.ToDecimal(dr["RETURNNETTWT"]);
                dRepairNettWtDiff = Convert.ToDecimal(dr["NETTWTDIFF"]);
            }

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
               
        private void GetRepairReturnCashAdvSkuData(ref DataTable RepairReturnCashAdvanceDataSku, string repairId)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" SELECT X.REPAIRID ,D.CASHADVANCE  ");
            //sbQuery.AppendLine(" ,ISNULL(D.MAKINGCHARGES,0)  MAKINGCHARGES ");
            sbQuery.AppendLine(" ,ISNULL(D.TOTRepairCharges,0)  REPAIRCHARGES ");
            //sbQuery.AppendLine(" ,(D.CASHADVANCE - ISNULL(D.MAKINGCHARGES,0)) REPAIRAMNTDIFF ");
            sbQuery.AppendLine(" ,(D.CASHADVANCE - ISNULL(D.TOTRepairCharges,0)) REPAIRAMNTDIFF ");
            sbQuery.AppendLine(" ,ISNULL(E.SKUNUMBER,'') SKUNUMBER ");
            sbQuery.AppendLine(" FROM  RETAILREPAIRHDR X  ");
            //sbQuery.AppendLine(" JOIN RETAILDEPOSITTABLE A ON X.REPAIRID=A.REPAIRID ");
            //sbQuery.AppendLine(" JOIN RETAILADJUSTMENTTABLE B ON A.REPAIRID=B.REPAIRID AND A.REPAIRID<>'' AND A.DEPOSITTYPE=2 ");//for DEPOSITTYPE=2, see EnumClass>>DepositType enum
            //sbQuery.AppendLine(" AND A.TRANSACTIONID=B.TRANSACTIONID AND A.TERMINALID=B.RETAILTERMINALID ");
            //sbQuery.AppendLine(" AND A.STOREID=B.RETAILSTOREID AND A.DATAAREAID=B.DATAAREAID ");
            //sbQuery.AppendLine(" JOIN RETAILREPAIRDETAIL C ON A.REPAIRID=C.REPAIRID ");
            sbQuery.AppendLine(" LEFT JOIN RETAILREPAIRRETURNDETAIL D ON X.REPAIRID=D.REPAIRID ");
            sbQuery.AppendLine(" LEFT JOIN SKUTABLE_POSTED E ON X.REPAIRID=E.REPAIRID ");
            sbQuery.AppendLine("  WHERE X.REPAIRID='" + repairId + "' ");
            sbQuery.AppendLine(" AND X.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' ");
            //sbQuery.AppendLine(" AND X.RETAILTERMINALID='" + ApplicationSettings.Database.TerminalID + "' ");
            sbQuery.AppendLine(" AND X.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            //sbQuery.AppendLine("  ");
            //sbQuery.AppendLine("  ");
            using (SqlCommand cmd = new SqlCommand { CommandText = sbQuery.ToString(), CommandTimeout = 0, Connection = connection })
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    if (RepairReturnCashAdvanceDataSku == null)
                    {
                        RepairReturnCashAdvanceDataSku = new DataTable();
                    }
                    da.Fill(RepairReturnCashAdvanceDataSku);
                }
            }
            if (dtRepairReturnCashAdvanceDataSku != null && dtRepairReturnCashAdvanceDataSku.Rows.Count > 0)
            {
                DataRow dr = dtRepairReturnCashAdvanceDataSku.Rows[0];
                dCashAdv = Convert.ToDecimal(dr["CASHADVANCE"]);
                dMakinCharges = Convert.ToDecimal(dr["REPAIRCHARGES"]);
                dRepairAmntDiff = Convert.ToDecimal(dr["REPAIRAMNTDIFF"]);
                sSkuNumber = Convert.ToString(dr["SKUNUMBER"]);
            }

            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void GetRepairReturnCashAdvOutSideData(ref DataTable RepairReturnCashAdvanceDataSku, string repairId) 
        {
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" DECLARE @REPAIROGSERVICEITEM VARCHAR(20) ");
            sbQuery.AppendLine(" DECLARE @InventBatchId VARCHAR(25) ");
            sbQuery.AppendLine(" DECLARE @REPAIRCHARGES numeric(28, 2)");
            sbQuery.AppendLine(" DECLARE @CASHADVANCE numeric(28, 2)");
            sbQuery.AppendLine(" DECLARE @OGPurWt numeric(28, 3)");
           
            ////sbQuery.AppendLine(" select @REPAIROGSERVICEITEM=ISNULL(CRWREPAIRADJITEMFOROG,'') from RETAILPARAMETERS");
            ////sbQuery.AppendLine(" select @CASHADVANCE=sum(ISNULL(OGFINALAMT,0)),  @OGPurWt=sum(ISNULL(EXPECTEDQUANTITY,0)) from RETAIL_CUSTOMCALCULATIONS_TABLE A where REPAIRBATCHID='" + repairId + "'");
            ////sbQuery.AppendLine(" AND A.STOREID='" + ApplicationSettings.Database.StoreID + "' ");
            ////sbQuery.AppendLine(" AND A.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' group by REPAIRBATCHID");
            sbQuery.AppendLine(" select @REPAIRCHARGES=sum(ISNULL(AMOUNT,0)) from RETAILTRANSACTIONINCOMEEXPENSETRANS IE where REPAIRBATCHID='" + repairId + "'");
            sbQuery.AppendLine(" AND IE.STORE='" + ApplicationSettings.Database.StoreID + "' ");
            sbQuery.AppendLine(" AND IE.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'  group by REPAIRBATCHID ");

            //sbQuery.AppendLine(" select @InventBatchId= INVENTBATCHID from RETAILTRANSACTIONSALESTRANS a");
            //sbQuery.AppendLine(" left join RETAIL_CUSTOMCALCULATIONS_TABLE b on a.TRANSACTIONID=b.TRANSACTIONID");
            //sbQuery.AppendLine(" and a.STORE=b.STOREID and a.TERMINALID=b.TERMINALID ");
            //sbQuery.AppendLine(" where b.REPAIRBATCHID ='" + repairId + "'");

            sbQuery.AppendLine(" SELECT isnull(@REPAIRCHARGES,0) AS REPAIRCHARGES");
         

            using(SqlCommand cmd = new SqlCommand { CommandText = sbQuery.ToString(), CommandTimeout = 0, Connection = connection })
            {
                using(SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    if(RepairReturnCashAdvanceDataSku == null)
                    {
                        RepairReturnCashAdvanceDataSku = new DataTable();
                    }
                    da.Fill(RepairReturnCashAdvanceDataSku);
                }
            }
            if(dtRepairReturnCashAdvanceDataSku != null && dtRepairReturnCashAdvanceDataSku.Rows.Count > 0)
            {
                DataRow dr = dtRepairReturnCashAdvanceDataSku.Rows[0];
                dRepairExpense = Convert.ToDecimal(dr["REPAIRCHARGES"]); 
                //dMakinCharges = Convert.ToDecimal(dr["REPAIRCHARGES"]);
                //dRepairNettWtDiff = Convert.ToDecimal(dr["OGPurWt"]);
                //sSkuNumber = Convert.ToString(dr["SKUNUMBER"]);
               // sInventBatchId = Convert.ToString(dr["InventBatchId"]);//
            }

            if(connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }
        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
            if (ValidateControls())
            {
                SaveOrder();
            }
        }

        private void txtTotalAmount_KeyPress(object sender, KeyPressEventArgs e)
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

        #region ValidateControls()
        bool ValidateControls()
        {

            if ((string.IsNullOrEmpty(txtReturnNo.Text.Trim())))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Repair Return No can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtReturnNo.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            //repair no

            if ((string.IsNullOrEmpty(txtOrderNo.Text.Trim())))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Repair No. can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtReturnNo.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            //dtPickerDeliveryDate

            if ((string.IsNullOrEmpty(dtPickerDeliveryDate.Text.Trim())))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Repair Return Date can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    dtPickerDeliveryDate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            //dTPickerOrderDate

            if ((string.IsNullOrEmpty(dTPickerOrderDate.Text.Trim())))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Repair Order date can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    dTPickerOrderDate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            // dt compare

            if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dTPickerOrderDate.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Delivery Date cannot be less than Order Date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;
                    return false;
                }

            }

            //txtItemId 

            if ((string.IsNullOrEmpty(txtItemId.Text.Trim())))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtItemId.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            // txtCustomerAccount

            if ((string.IsNullOrEmpty(txtCustomerAccount.Text.Trim())))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Account can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtCustomerAccount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            else
            {
                return true;
            }
        }
        #endregion

        #region GetOrderNum()
        public string GetOrderNum()
        {
            string OrderNum = string.Empty;
            OrderNum = GetNextCustomerOrderID();
            return OrderNum;
        }
        #endregion

        #region -  GET THE ORDER ID

        enum ReceiptTransactionType
        {
            OrnamentRepairReturn = 10
        }

        public string GetNextCustomerOrderID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.OrnamentRepairReturn;
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

        #region GetOrderNum()
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

        #region GetSeedVal()
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.OrnamentRepairReturn;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {

                //string queryString = " SELECT  MAX(CAST(ISNULL(SUBSTRING(RETAILREPAIRRETURNTRANS.RepairReturnId,5,LEN(RETAILREPAIRRETURNTRANS.RepairReturnId)),0) AS INTEGER)) + 1 from RETAILREPAIRRETURNTRANS ";
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(RepairReturnId,@VAL,LEN(RepairReturnId)),0) AS INTEGER)) + 1 from RETAILREPAIRRETURNTRANS";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    
                    if(!string.IsNullOrEmpty(Val))
                        return Val;
                    else
                        return "1";

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

        #region Save
        private void SaveOrder()
        {
            int iRETAILREPAIRRETURNTRANS = 0;

            SqlTransaction transaction = null;

            #region Repair Return
            string commandText = " SET DATEFORMAT DMY;  BEGIN  INSERT INTO [RETAILREPAIRRETURNTRANS]([RepairReturnId],[DELIVERYDATE],[RepairId],[ORDERDATE]," +
                                 " [RetailStoreId],[RetailTerminalId],[RetailStaffId],[CUSTACCOUNT],[CUSTNAME],[CUSTADDRESS],[CUSTPHONE],[DATAAREAID] " +
                                 " ,REPAIRADVADJUSTMENTID,REPAIRRECVSTOREID,REPAIRRECVTERMINALID) " +
                                 " VALUES(@RepairReturnId,@DELIVERYDATE,@RepairId,@ORDERDATE," +
                                 " @RetailStoreId,@RetailTerminalId,@RetailStaffId,@CUSTACCOUNT,@CUSTNAME,@CUSTADDRESS,@CUSTPHONE,@DATAAREAID" +
                                 " ,@REPAIRADVADJUSTMENTID,@REPAIRRECVSTOREID,@REPAIRRECVTERMINALID)";

            if(bInHouse == true)
                commandText += "  UPDATE RetailRepairReturnHdr SET IsDelivered = 1, DeliveryDate = @DELIVERYDATE WHERE RepairId = @RepairId  END";
            else
                commandText += "  UPDATE RetailRepairDetail SET IsDelivered = 1, DeliveryDate = @DELIVERYDATE WHERE BatchId = @RepairId  END";
                                 
            //if(!string.IsNullOrEmpty(sAdvAdjustmentId))
            //{
            //    commandText += "  BEGIN UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED = 1 WHERE TRANSACTIONID = '" + sAdvAdjustmentId + "' AND REPAIRID = '" + txtOrderNo.Text.Trim() + "' END";
            //}

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

                command.Parameters.Add("@RepairReturnId", SqlDbType.NVarChar).Value = txtReturnNo.Text.Trim();
                command.Parameters.Add("@DELIVERYDATE", SqlDbType.DateTime).Value = dtPickerDeliveryDate.Value;
                command.Parameters.Add("@RepairId", SqlDbType.NVarChar).Value = txtOrderNo.Text.Trim();
                command.Parameters.Add("@ORDERDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                command.Parameters.Add("@RetailStoreId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RetailTerminalId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = txtCustomerAccount.Text.Trim();
                command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 60).Value = txtCustomerName.Text.Trim();
                command.Parameters.Add("@CUSTADDRESS", SqlDbType.NVarChar, 250).Value = txtCustomerAddress.Text.Trim();

                if (string.IsNullOrEmpty(txtPhoneNumber.Text))
                    command.Parameters.Add("@CUSTPHONE", SqlDbType.NVarChar, 20).Value = "";
                else
                    command.Parameters.Add(new SqlParameter("@CUSTPHONE", txtPhoneNumber.Text.Trim()));
                if (application != null)

                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@REPAIRADVADJUSTMENTID", SqlDbType.NVarChar, 20).Value = sAdvAdjustmentId;

                command.Parameters.Add("@REPAIRRECVSTOREID", SqlDbType.NVarChar, 20).Value = sStore;
                command.Parameters.Add("@REPAIRRECVTERMINALID", SqlDbType.NVarChar, 20).Value = sTerminal;

            #endregion

                command.CommandTimeout = 0;
                iRETAILREPAIRRETURNTRANS = command.ExecuteNonQuery();

                transaction.Commit();
                command.Dispose();
                transaction.Dispose();
                if (iRETAILREPAIRRETURNTRANS != -1)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Repair Return has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        RetailTransaction retailTrans = pos as RetailTransaction;

                        if (retailTrans != null)
                        {
                            IsRepairReturn = true;
                            sRepairRetId = txtReturnNo.Text.Trim();

                            retailTrans.PartnerData.REPAIRRETURNTRANS = true;
                            retailTrans.PartnerData.REPAIRID = txtOrderNo.Text.Trim();
                            retailTrans.PartnerData.REPAIRRETURNID = txtReturnNo.Text.Trim();
                        }


                        //try
                        //{
                        //    PrintVoucher();
                        //}
                        //catch { }

                        CLearControls();

                        this.Close();

                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
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
                    connection.Close();
            }
        }
        #endregion

        private void CLearControls()
        {
            txtReturnNo.Text = string.Empty;
            txtOrderNo.Text = string.Empty;

            dTPickerOrderDate.Text = DateTime.Now.ToString();
            dtPickerDeliveryDate.Text = DateTime.Now.ToString();

            txtItemId.Text = string.Empty;
            txtCustomerAccount.Text = string.Empty;

            txtCustomerName.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;

            txtPhoneNumber.Text = string.Empty;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region Print Voucher
        public void PrintVoucher()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            // PageSettings ps = new PageSettings { Landscape = false, PaperSize = new PaperSize { RawKind = (int)PaperKind.A4 }, Margins = new Margins { Top = 0, Right = 0, Bottom = 0, Left = 0 } };
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo()));
            rds.Add(new ReportDataSource("REPAIRDETAILINFO", (DataTable)GetDetailInfo()));
            // rds.Add(new ReportDataSource("BARCODEIMGTABLE", (DataTable)null));//GetBarcodeInfo(txtOrderNo.Text))
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", "Repair Receipt", true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));

            string reportName = @"rptRepairOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Repair Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();
        }

        public DataTable GetHeaderInfo()
        {
            DataTable dtHeader = new DataTable();
            dtHeader.Columns.Add("ORDERNO", typeof(string));
            dtHeader.Columns.Add("ORDERDATE", typeof(DateTime));
            dtHeader.Columns.Add("DELIVERYDATE", typeof(DateTime));
            dtHeader.Columns.Add("CUSTID", typeof(string));
            dtHeader.Columns.Add("CUSTNAME", typeof(string));
            dtHeader.Columns.Add("CUSTADD", typeof(string));
            dtHeader.Columns.Add("CUSTPHONE", typeof(string));
            dtHeader.Columns.Add("TOTALAMOUNT", typeof(decimal));
            dtHeader.Columns.Add("DELVERYDATEVISIBLE", typeof(bool));
            dtHeader.Columns.Add("RepairBagNo", typeof(string));
            dtHeader.Columns.Add("CustEmail", typeof(string));

            DataRow dr = dtHeader.NewRow();
            dr["ORDERNO"] = txtOrderNo.Text;
            dr["ORDERDATE"] = dTPickerOrderDate.Value;
            dr["DELIVERYDATE"] = dtPickerDeliveryDate.Value;
            dr["CUSTID"] = txtCustomerAccount.Text;

            #region Customer Address format Changed by Palas Jana @ 08-12-2014
            var SelectedCust = customerDataManager.GetTransactionalCustomer(txtCustomerAccount.Text);
            dr["CUSTNAME"] = txtCustomerName.Text;// Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.GetCustomerNameWithSalutation(SelectedCust);//txtCustomerName.Text;
            dr["CUSTADD"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.AddressLines(SelectedCust);//txtCustomerAddress.Text;
            sEmail = SelectedCust.Email;
            #endregion
            dr["CUSTPHONE"] = txtPhoneNumber.Text;
            dr["DELVERYDATEVISIBLE"] = true;
            dr["CustEmail"] = sEmail;

            dtHeader.Rows.Add(dr);

            return dtHeader;
        }

        public DataTable GetDetailInfo()
        {
            //string sArchivePath = GetArchivePathFromImage();
            //string sSaleItem = string.Empty;
            //string sItemParentId = string.Empty;

            DataTable dtDetails = new DataTable();
            //dtDetails.Columns.Add("ITEMID", typeof(string));
            //dtDetails.Columns.Add("PCS", typeof(decimal));
            //dtDetails.Columns.Add("QTY", typeof(decimal));
            //dtDetails.Columns.Add("NETTWT", typeof(decimal));
            //dtDetails.Columns.Add("APPXVALUE", typeof(decimal));
            //dtDetails.Columns.Add("JOBTYPE", typeof(string));
            //dtDetails.Columns.Add("JOBDETAILS", typeof(string));
            //dtDetails.Columns.Add("TOTALAMOUNT", typeof(decimal));
            //dtDetails.Columns.Add("ORDERLINEIMAGE", typeof(string));
            //dtDetails.Columns.Add("BATCHID", typeof(string));
            //dtDetails.Columns.Add("Article", typeof(string));


            //int i = 1;
            //foreach(DataRow item in dtOrderDetails.Rows)
            //{
            //    DataRow dr = dtDetails.NewRow();
            //    dr["ITEMID"] = item["ITEMID"];
            //    dr["PCS"] = Convert.ToDecimal(item["PCS"]);
            //    dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
            //    dr["NETTWT"] = Convert.ToDecimal(item["NETTWT"]);
            //    dr["APPXVALUE"] = Convert.ToDecimal(item["TOTALAMT"]);
            //    dr["JOBTYPE"] = Convert.ToString(item["JOBTYPE"]);
            //    dr["JOBDETAILS"] = Convert.ToString(item["REPAIRJOBDETAILS"]);
            //    dr["TOTALAMOUNT"] = Convert.ToDecimal(item["RATE"]);

            //    string path = sArchivePath + "" + txtOrderNo.Text + "_" + i + ".jpeg"; //

            //    if(File.Exists(path))
            //    {
            //        Image img = Image.FromFile(path);
            //        byte[] arr;
            //        using(MemoryStream ms1 = new MemoryStream())
            //        {
            //            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
            //            arr = ms1.ToArray();
            //        }

            //        dr["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
            //    }
            //    else
            //    {
            //        sSaleItem = Convert.ToString(dr["ITEMID"]);
            //        sItemParentId = GetItemParentId(Convert.ToString(dr["ITEMID"]));


            //        path = sArchivePath + "" + sItemParentId + "" + ".jpg"; //

            //        if(File.Exists(path))
            //        {
            //            Image img = Image.FromFile(path);
            //            byte[] arr;
            //            using(MemoryStream ms1 = new MemoryStream())
            //            {
            //                img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
            //                arr = ms1.ToArray();
            //            }

            //            dr["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
            //        }
            //        else
            //            dr["ORDERLINEIMAGE"] = "";

            //    }
            //    dr["BATCHID"] = txtOrderNo.Text + "" + i;
            //    dr["Article"] = Convert.ToString(item["Article"]);

            //    i++;
            //    dtDetails.Rows.Add(dr);
            //}

            return dtDetails;
        }

        public string GetItemParentId(string sSalesItem)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select ITEMIDPARENT  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        public string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) ARCHIVEPATH  from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }
       
        #endregion

    }
}
