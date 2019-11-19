using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using LSRetailPosis.Settings;
using System.IO;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using LSRetailPosis.DataAccess.DataUtil;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmBulkReceive : Form
    {
        SqlConnection conn = new SqlConnection();
        public IPosTransaction pos { get; set; }

        [Import]
        private IApplication application;
        DataTable dtTOR = new DataTable();
        bool bIsImport = false;
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();

        #region enum  TransactionType
        /// <summary>
        /// </summary>
        enum BulkTransactionType
        {
            None = 0,
            Receive = 1,
            Issue = 2,
            Sales = 3,
            SalesReturn = 4,
            OGPurchase = 5,
            OGExchange = 6,
            OGPurchaseReturn = 7,
            OGExchangeReturn = 8,
            OrderSampleReceive = 9,
            OrderStnDmdReceive = 10,
        }
        #endregion
        enum TransactionType
        {
            BulkItemTrans = 12
        }

        public frmBulkReceive()
        {
            InitializeComponent();
        }
        public frmBulkReceive(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
        }

        private void btnTransferSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetReceivedTransferId", sStoreId);

                    DataSet dsTransfer = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsTransfer.ReadXml(srTransDetail);
                    }

                    if (dsTransfer != null && dsTransfer.Tables[0].Rows.Count > 0)
                    {
                        string sSQl = "Select distinct TORNo from BulkItemTransHeader where TORNo!=''";// OR TORNo in(SELECT TransferOrder FROM TransferOrderTrans WHERE ReceivedOrShipped=1)";
                        DataTable dtRecTONo = objBlank.NIM_LoadCombo("", "", "", sSQl);

                        List<DataRow> rows_to_remove = new List<DataRow>();
                        foreach (DataRow row1 in dsTransfer.Tables[0].Rows)
                        {
                            foreach (DataRow row2 in dtRecTONo.Rows)
                            {
                                if (Convert.ToString(row1["TransferId"]) == Convert.ToString(row2["TORNo"]))
                                {
                                    rows_to_remove.Add(row1);
                                }
                            }
                        }
                        foreach (DataRow row in rows_to_remove)
                        {
                            dsTransfer.Tables[0].Rows.Remove(row);
                            dsTransfer.Tables[0].AcceptChanges();
                        }
                    }

                    if (dsTransfer != null && dsTransfer.Tables[0].Rows.Count > 0)
                    {
                        string sSQl = "SELECT distinct TransferOrder as TransferId FROM TransferOrderTrans WHERE ReceivedOrShipped=0 and isnull(TransferOrder,'')!=''";// OR TORNo in(SELECT TransferOrder FROM TransferOrderTrans WHERE ReceivedOrShipped=1)";
                        DataTable dtRecTONo = objBlank.NIM_LoadCombo(sSQl);

                        List<DataRow> rows_to_remove1 = new List<DataRow>();
                        foreach (DataRow row1 in dsTransfer.Tables[0].Rows)
                        {
                            int iChk = 0;
                            foreach (DataRow row2 in dtRecTONo.Rows)
                            {
                                if (Convert.ToString(row1["TransferId"]) != Convert.ToString(row2["TransferId"]))
                                    iChk = 1;
                                else
                                {
                                    iChk = 0;
                                    break;
                                }
                            }

                            if (iChk == 1)
                                rows_to_remove1.Add(row1);
                        }
                        foreach (DataRow row in rows_to_remove1)
                        {
                            dsTransfer.Tables[0].Rows.Remove(row);
                            dsTransfer.Tables[0].AcceptChanges();
                        }
                        DataView view = new DataView();

                        if (dtRecTONo != null && dtRecTONo.Rows.Count == 0)
                        {
                            view = new DataView(dtRecTONo);
                        }
                        else
                        {
                            view = new DataView(dsTransfer.Tables[0]);
                        }
                        DataTable distinctValues = view.ToTable(true, "TransferId");

                        //string sSQl1 = "SELECT TransferOrder FROM TransferOrderTrans WHERE ReceivedOrShipped=1";
                        //DataTable dtRecTONo1 = objBlank.NIM_LoadCombo("", "", "", sSQl);

                        //DataRow[] dr = dsTransfer.Tables[0].Select("TransferId='" + Convert.ToString(cmbCustomerOrder.SelectedValue) + "' and ORDERDETAILNUM=" + oCustOrder.lineId + "");// and LINENUM=" + oCustOrder.lineId + "


                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(distinctValues, null, "Transfer Order Id");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            txtTransferId.Text = Convert.ToString(dr["TransferId"]);

                            dtTOR = null;
                            grItems.DataSource = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnFetchItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransferId.Text))
            {
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sStoreId = ApplicationSettings.Terminal.StoreId;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetBulkItemListTR", txtTransferId.Text.Trim());

                        DataSet dsWH = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsWH.ReadXml(srTransDetail);
                        }
                        if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                        {
                            dtTOR = dsWH.Tables[0];
                            grItems.DataSource = dsWH.Tables[0];

                            CalTotal(dtTOR);
                        }
                    }
                }
                catch (Exception ex)
                {
                    lblTotPcs.Text = "0";
                    lblTotQty.Text = "0";
                }
            }
        }

        private void CalTotal(DataTable dtItem)
        {
            decimal iNoOfPcs = 0;
            decimal dTotQty = 0m;
            //Start : added on 10/05/2017
            foreach (DataRow dr1 in dtItem.Rows)
            {
                iNoOfPcs = iNoOfPcs + Convert.ToDecimal(dr1["PdsCWQtyReceived"]);
                dTotQty = dTotQty + Convert.ToDecimal(dr1["QtyReceived"]);
            }

            lblTotPcs.Text = Convert.ToString(iNoOfPcs);
            lblTotQty.Text = Convert.ToString(dTotQty);
            //End : added on 10/05/2017
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                SaveBulkItemTransfer();
            }
        }

        private bool ValidateControls()
        {
            if (string.IsNullOrEmpty(txtTransferId.Text))
            {
                ShowMessage("Transfer received order no is required");
                txtTransferId.Focus();
                return false;
            }
            if (dtTOR == null)
            {
                ShowMessage("Altleast one item should be transfer.");
                txtTransferId.Focus();
                return false;
            }
            if (dtTOR != null && dtTOR.Rows.Count == 0)
            {
                ShowMessage("Altleast one item should be transfer.");
                txtTransferId.Focus();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void ClearControls()
        {
            txtTransferId.Text = "";
            grItems.DataSource = null;
            lblTotPcs.Text = "0";
            lblTotQty.Text = "0";
        }

        public string ShowMessage(string _msg)
        {
            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(_msg, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                return _msg;
            }
        }

        private void transactionNumber(int transType, string funcProfile, out string mask)
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
                        conn.Open();

                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)TransactionType.BulkItemTrans;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(TransReceiptId,@VAL,LEN(TransReceiptId)),0) AS INTEGER)) + 1 from BulkItemTransHeader";

                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    Val = Convert.ToString(command.ExecuteScalar());

                    if (string.IsNullOrEmpty(Val))
                        Val = "1";

                    return Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        public string GetNextBulkItemTransferTransactionId()
        {
            try
            {
                TransactionType transType = TransactionType.BulkItemTrans;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                transactionNumber((int)transType, funcProfileId, out mask);
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

        public string GetBulkItemTransId()
        {
            string TransId = string.Empty;
            TransId = GetNextBulkItemTransferTransactionId();
            return TransId;
        }

        private void UpdateDataIntoLocal(DataTable dtItems)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                if (dtItems != null && dtItems.Rows.Count > 0)
                {
                    for (int ItemCount = 0; ItemCount < dtItems.Rows.Count; ItemCount++)
                    {

                        string sqlUpd = " IF EXISTS (SELECT Itemid FROM TransferOrderTrans WHERE" +
                                        " Itemid = '" + Convert.ToString(dtItems.Rows[ItemCount]["ITEMID"]) + "'" +
                                        " AND CONFIGID = '" + Convert.ToString(dtItems.Rows[ItemCount]["CONFIGID"]) + "'" +
                                        " AND INVENTCOLORID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTCOLORID"]) + "'" +
                                        " AND INVENTSIZEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSIZEID"]) + "'" +
                                        " AND INVENTSTYLEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSTYLEID"]) + "'" +
                                        " AND INVENTBATCHID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTBATCHID"]) + "'" +
                                        " and ReceivedOrShipped = 0 and TRANSTYPE=1 and TransferOrder='" + txtTransferId.Text.Trim() + "')" +
                                        " BEGIN " +
                                            " UPDATE TransferOrderTrans SET ReceivedOrShipped = 1" +
                                            " WHERE Itemid = '" + Convert.ToString(dtItems.Rows[ItemCount]["ITEMID"]) + "'" +
                                            " AND CONFIGID = '" + Convert.ToString(dtItems.Rows[ItemCount]["CONFIGID"]) + "'" +
                                            " AND INVENTCOLORID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTCOLORID"]) + "'" +
                                            " AND INVENTSIZEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSIZEID"]) + "'" +
                                            " AND INVENTSTYLEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSTYLEID"]) + "'" +
                                            " AND INVENTBATCHID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTBATCHID"]) + "'" +
                                            " and ReceivedOrShipped = 0 and TRANSTYPE=1 and TransferOrder='" + txtTransferId.Text.Trim() + "'" +
                                        " END";

                        SqlCommand command = new SqlCommand(sqlUpd, connection, transaction);
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                        command.Dispose();
                    }
                }
                transaction.Commit();
                transaction.Dispose();
            }
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
        }

        private void SaveBulkItemTransfer()
        {
            int iTransfer_Header = 0;
            int iTransfer_Details = 0;
            string TransferId = GetBulkItemTransId();
            SqlTransaction transaction = null;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " INSERT INTO [BulkItemTransHeader]([TORNo],[TransReceiptId]," +
                                 " [TransDate],[RetailStaffId],[RetailStoreId],[RetailTerminalId]," +
                                 " [DATAAREAID])" +
                                 " VALUES(@TORNo,@TransReceiptId,@TransDate,@RetailStaffId,@RetailStoreId," +
                                 " @RetailTerminalId, @DATAAREAID)";


            try
            {
                transaction = connection.BeginTransaction();

                #region Bulk Item receive HEADER
                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@TORNo", SqlDbType.NVarChar, 20).Value = txtTransferId.Text.Trim();
                command.Parameters.Add("@TransReceiptId", SqlDbType.NVarChar, 20).Value = TransferId.Trim();
                command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
                command.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                if (application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                #endregion

                command.CommandTimeout = 0;
                iTransfer_Header = command.ExecuteNonQuery();

                if (iTransfer_Header == 1)
                {
                    if (dtTOR != null && dtTOR.Rows.Count > 0)
                    {
                        #region  DETAILS & Transaction

                        string commandDetail = " INSERT INTO [BulkItemTransDetails]([TransReceiptId],[LineNumber],[ItemId],[ConfigId]," +
                                                         " [InventSizeId],[InventColorId],[InventStyleId],InventBatchId,[PdsCWQty],[Qty])" +
                                                         " VALUES(@TransReceiptId  ,@LineNumber , @ItemId," +
                                                         " @ConfigId,@InventSizeId,@InventColorId," +
                                                         " @InventStyleId,@InventBatchId,@PdsCWQty,@Qty) ";

                        commandDetail += " INSERT INTO [BulkItemTransTable]([TransReceiptId],[LineNumber]," +
                                                        " [TransDate],[TransType],[ItemId],[ConfigId]," +
                                                        " [InventSizeId],[InventColorId],[InventStyleId]," +
                                                        " InventBatchId,[PdsCWQty],[Qty]," +
                                                        " [RetailStaffId],[RetailStoreId]," +
                                                        " [RetailTerminalId],DATAAREAID,GrossWt,NetWt)" +
                                                        " VALUES(@TransReceiptId  ,@LineNumber ," +
                                                        " @TransDate,@TransType, @ItemId," +
                                                        " @ConfigId,@InventSizeId,@InventColorId,@InventStyleId," +
                                                        " @InventBatchId,@PdsCWQty,@Qty,@RetailStaffId," +
                                                        " @RetailStoreId,@RetailTerminalId, @DATAAREAID,@GrossWt,@NetWt) ";

                        for (int ItemCount = 0; ItemCount < dtTOR.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdDetail = new SqlCommand(commandDetail, connection, transaction);
                            cmdDetail.Parameters.Add("@TransReceiptId", SqlDbType.NVarChar, 20).Value = TransferId.Trim();
                            cmdDetail.Parameters.Add("@LineNumber", SqlDbType.Int, 10).Value = ItemCount + 1;
                            cmdDetail.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();

                            if (string.IsNullOrEmpty(Convert.ToString(dtTOR.Rows[ItemCount]["ItemId"])))
                                cmdDetail.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtTOR.Rows[ItemCount]["ItemId"]);

                            cmdDetail.Parameters.Add("@ConfigId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtTOR.Rows[ItemCount]["configId"]);
                            cmdDetail.Parameters.Add("@InventSizeId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtTOR.Rows[ItemCount]["InventSizeId"]);
                            cmdDetail.Parameters.Add("@InventColorId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtTOR.Rows[ItemCount]["InventColorId"]);
                            cmdDetail.Parameters.Add("@InventStyleId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtTOR.Rows[ItemCount]["InventStyleId"]);
                            cmdDetail.Parameters.Add("@InventBatchId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtTOR.Rows[ItemCount]["InventBatchId"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTOR.Rows[ItemCount]["PdsCWQtyReceived"])))
                                cmdDetail.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtTOR.Rows[ItemCount]["PdsCWQtyReceived"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTOR.Rows[ItemCount]["QtyReceived"])))
                                cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtTOR.Rows[ItemCount]["QtyReceived"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTOR.Rows[ItemCount]["QtyReceived"])))
                                cmdDetail.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dtTOR.Rows[ItemCount]["QtyReceived"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtTOR.Rows[ItemCount]["QtyReceived"])))
                                cmdDetail.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dtTOR.Rows[ItemCount]["QtyReceived"]);

                            cmdDetail.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)BulkTransactionType.Receive;
                            cmdDetail.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdDetail.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdDetail.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                            if (application != null)
                                cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                            else
                                cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            cmdDetail.CommandTimeout = 0;
                            iTransfer_Details = cmdDetail.ExecuteNonQuery();
                            cmdDetail.Dispose();
                        }

                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();

                if (iTransfer_Details != 0)
                {
                    if (dtTOR != null && dtTOR.Rows.Count > 0)
                    {
                        UpdateDataIntoLocal(dtTOR);
                    }

                    MessageBox.Show("Received successfully");

                    dtTOR = null;
                    grItems.DataSource = null;
                    txtTransferId.Text = "";
                    lblTotPcs.Text = "0";
                    lblTotQty.Text = "0";
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
    }
}
