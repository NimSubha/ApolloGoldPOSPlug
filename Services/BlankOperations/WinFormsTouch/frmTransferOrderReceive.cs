using System;
using System.Data;
using System.Windows.Forms;
using LSRetailPosis.Settings;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmTransferOrderReceive : frmTouchBase
    {
        public IPosTransaction pos { get; set; }
        [Import]
        private IApplication application;

        enum CRWStockType
        {
            SKU = 0,
            Bulk = 1,
        }
        public enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
            GiftVoucher = 18,

        }

        DataTable dtItems = new DataTable();
        DataTable dtCustom = new DataTable();

        int iIngMetalType = 0;

        public frmTransferOrderReceive()
        {
            InitializeComponent();
        }

        private void btnTransferSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;

                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferId", sStoreId);

                    DataSet dsTransfer = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsTransfer.ReadXml(srTransDetail);
                    }
                    if (dsTransfer != null && dsTransfer.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsTransfer.Tables[0], null, "Transfer Order Id");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            txtTransferId.Text = Convert.ToString(dr["TransferId"]);

                            ReadOnlyCollection<object> containerArray1;

                            if (!string.IsNullOrEmpty(txtTransferId.Text))
                            {
                                containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("GetShippedAttribute", txtTransferId.Text.Trim());

                                int iNoOfPcs = 0;
                                decimal dTotQty = 0;

                                DataSet dsTransfer1 = new DataSet();
                                StringReader srTransDetail1 = new StringReader(Convert.ToString(containerArray1[3]));

                                if (Convert.ToString(containerArray1[3]).Trim().Length > 38)
                                {
                                    dsTransfer1.ReadXml(srTransDetail1);
                                }
                                if (dsTransfer1 != null && dsTransfer1.Tables[0].Rows.Count > 0)
                                {
                                    dtItems = dsTransfer1.Tables[0];
                                    grItems.DataSource = dtItems;// dsTransfer1.Tables[0];

                                    //Start : added on 10/05/2017
                                    foreach (DataRow dr1 in dtItems.Rows)
                                    {
                                        iNoOfPcs = iNoOfPcs + Convert.ToInt32(dr1["PdsCWQtyTransfer"]);
                                        dTotQty = dTotQty + Convert.ToDecimal(dr1["QtyTransfer"]);
                                    }

                                    lblTotPcs.Text = Convert.ToString(iNoOfPcs);
                                    lblTotQty.Text = Convert.ToString(dTotQty);
                                    //End : added on 10/05/2017
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblTotPcs.Text = "0";
                lblTotQty.Text = "0";
            }

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string sMsg = string.Empty;
            if (dtItems != null && dtItems.Rows.Count > 0)
            {
                try
                {
                    ReadOnlyCollection<object> containerArray;

                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        if (dtItems != null && dtItems.Rows.Count > 0)
                        {
                            SaveDataIntoLocal();
                        }

                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("TransferOrderReceipt", txtTransferId.Text.Trim(), dtItems.Rows.Count);
                        sMsg = Convert.ToString(containerArray[2]);
                        bool bSuccess = Convert.ToBoolean(containerArray[1]);

                        /*DataSet dsTransfer = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsTransfer.ReadXml(srTransDetail);
                        }
                        if (dsTransfer != null && dsTransfer.Tables[0].Rows.Count > 0)
                        {
                            dtItems = dsTransfer.Tables[0];
                        }*/


                        #region Commented on 150518
                        /* if (bSuccess)
                        {
                            if (dtItems != null && dtItems.Rows.Count > 0)
                            {
                                SaveDataIntoLocal();
                            }
                        }
                        else
                        {
                            #region rechecking TO received or not added on  09/10/17
                            if (PosApplication.Instance.TransactionServices.CheckConnection())
                            {
                                ReadOnlyCollection<object> containerArray1;

                                containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("checkTransferOrderReceived", txtTransferId.Text.Trim(), dtItems.Rows.Count);

                                bool bStatus = Convert.ToBoolean(containerArray1[1]);

                                if (bStatus)
                                {
                                    if (dtItems != null && dtItems.Rows.Count > 0)
                                    {
                                        SaveDataIntoLocal();
                                    }
                                }
                                else
                                {
                                    #region rechecking TO received or not added on   09/10/17
                                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                                    {
                                        ReadOnlyCollection<object> containerArray2;

                                        containerArray2 = PosApplication.Instance.TransactionServices.InvokeExtension("checkTransferOrderReceived", txtTransferId.Text.Trim(), dtItems.Rows.Count);

                                        bStatus = Convert.ToBoolean(containerArray2[1]);

                                        if (bStatus)
                                        {
                                            if (dtItems != null && dtItems.Rows.Count > 0)
                                            {
                                                SaveDataIntoLocal();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (dtItems != null && dtItems.Rows.Count > 0)
                                        {
                                            SaveDataIntoLocal();
                                        }
                                    }
                                    #endregion
                                }

                            }
                            else
                            {
                                if (dtItems != null && dtItems.Rows.Count > 0)
                                {
                                    SaveDataIntoLocal();
                                }
                            }

                            #endregion
                        }*/
                        #endregion

                        MessageBox.Show(sMsg);
                        txtTransferId.Text = string.Empty;
                        grItems.DataSource = null;
                        lblTotPcs.Text = "";
                        lblTotQty.Text = "";
                        dtItems.Clear();
                    }

                }
                catch (Exception ex)
                {
                    #region rechecking TO received or not added on   09/10/17
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray2;

                        containerArray2 = PosApplication.Instance.TransactionServices.InvokeExtension("checkTransferOrderReceived", txtTransferId.Text.Trim(), dtItems.Rows.Count);

                        bool bStatus = Convert.ToBoolean(containerArray2[1]);

                        if (bStatus)
                        {
                            if (dtItems != null && dtItems.Rows.Count > 0)
                            {
                                SaveDataIntoLocal();
                            }
                            MessageBox.Show(sMsg);
                            txtTransferId.Text = string.Empty;
                            grItems.DataSource = null;
                            lblTotPcs.Text = "";
                            lblTotQty.Text = "";
                            dtItems.Clear();
                        }
                    }
                    #endregion
                }
            }
            else
            {
                MessageBox.Show("Altleast one item should be fetched.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SaveDataIntoLocal()
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
                    string QRY = " IF NOT EXISTS(SELECT TOP 1 TransferOrder FROM TransferOrderTrans " +
                                   " WHERE TransferOrder=@TransferOrder and LineNumber=@LineNumber) BEGIN  " +//added on 060818
                                    "INSERT INTO [TransferOrderTrans] " +
                                            "([ItemId],[LineNumber],[TransType]," +
                                            " [TransferOrder],[TransDate],[ConfigId]," +
                                            " [InventSizeId],[InventColorId],[InventStyleId]," +
                                            " [InventBatchId],[PdsCWQty],[Qty],TotalCostPrice,GrossWeight,NetWeight," +
                                            " [CostPrice],[TransferCostPrice],[ReceivedOrShipped]," +
                                            " RetailStaffId,RetailStoreId,RetailTerminalId," +
                                            " DATAAREAID,Name,MetalType,Article_code,ProductTypeCode,"+
                                            " ArticleDescription,CRWSTONETRANSFERCOST) " +
                                            " VALUES" +
                                            "(@ItemId,@LineNumber,@TransType," +
                                            " @TransferOrder,@TransDate,@ConfigId," +
                                            " @InventSizeId,@InventColorId,@InventStyleId," +
                                            " @InventBatchId,@PdsCWQty,@Qty,@TotalCostPrice,@GrossWeight,@NetWeight," +
                                            " @CostPrice,@TransferCostPrice,@ReceivedOrShipped," +
                                            " @RetailStaffId,@RetailStoreId,@RetailTerminalId," +
                                            " @DATAAREAID,@Name,@MetalType,@Article_code,@ProductTypeCode," +
                                            " @ArticleDescription,@CRWSTONETRANSFERCOST) END ";

                    //added on 19-06-17 req by Mr.S.sharma

                    QRY = QRY + " IF EXISTS(SELECT TOP 1 SKUNUMBER FROM SKUTableTrans " +
                                           " WHERE SKUNUMBER=@ITEMID AND CounterIn=1 AND ISAVAILABLE=0) BEGIN" +
                                           " UPDATE SKUTableTrans SET CounterIn=0 WHERE SKUNUMBER=@ITEMID AND CounterIn=1 AND ISAVAILABLE=0 END";

                    for (int ItemCount = 0; ItemCount < dtItems.Rows.Count; ItemCount++)
                    {
                        using (SqlCommand cmdItem = new SqlCommand(QRY, connection, transaction))
                        {
                            cmdItem.CommandTimeout = 0;
                            cmdItem.Parameters.Add("@ItemId", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["ItemId"]);
                            cmdItem.Parameters.Add("@LineNumber", SqlDbType.Int).Value = ItemCount + 1;
                            cmdItem.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)((CRWStockType)Enum.Parse(typeof(CRWStockType), Convert.ToString(dtItems.Rows[ItemCount]["TransferType"])));
                            cmdItem.Parameters.Add("@TransferOrder", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["TransferId"]);
                            cmdItem.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Now.ToShortDateString();
                            cmdItem.Parameters.Add("@ConfigId", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["ConfigId"]);
                            cmdItem.Parameters.Add("@InventSizeId", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["InventSizeId"]);
                            cmdItem.Parameters.Add("@InventColorId", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["InventColorId"]);
                            cmdItem.Parameters.Add("@InventStyleId", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["InventStyleId"]);
                            cmdItem.Parameters.Add("@InventBatchId", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["InventBatchId"]);
                            cmdItem.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["PdsCWQtyTransfer"]);
                            cmdItem.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["QtyTransfer"]);
                            cmdItem.Parameters.Add("@CostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["CostPrice"]);
                            cmdItem.Parameters.Add("@TransferCostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["TransferCostPrice"]);

                            cmdItem.Parameters.Add("@TotalCostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["TotalCostPrice"]);

                            cmdItem.Parameters.Add("@GrossWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["GrossWt"]);
                            cmdItem.Parameters.Add("@NetWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["NetWt"]);

                            cmdItem.Parameters.Add("@ReceivedOrShipped", SqlDbType.Int).Value = 0;
                            cmdItem.Parameters.Add("@RetailStaffId", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                            cmdItem.Parameters.Add("@RetailStoreId", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                            cmdItem.Parameters.Add("@RetailTerminalId", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdItem.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            cmdItem.Parameters.Add("@Name", SqlDbType.VarChar, 60).Value = Convert.ToString(dtItems.Rows[ItemCount]["Name"]);
                            cmdItem.Parameters.Add("@MetalType", SqlDbType.Int).Value = Convert.ToInt16(dtItems.Rows[ItemCount]["MetalType"]);
                            cmdItem.Parameters.Add("@Article_code", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["Article_code"]);
                            cmdItem.Parameters.Add("@ProductTypeCode", SqlDbType.VarChar, 20).Value = Convert.ToString(dtItems.Rows[ItemCount]["ProductTypeCode"]);
                            cmdItem.Parameters.Add("@ArticleDescription", SqlDbType.VarChar, 60).Value = Convert.ToString(dtItems.Rows[ItemCount]["ArticleDescription"]);
                            cmdItem.Parameters.Add("@CRWSTONETRANSFERCOST", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItems.Rows[ItemCount]["CRWSTONETRANSFERCOST"]);

                            cmdItem.ExecuteNonQuery();
                            cmdItem.Dispose();
                        }
                    }
                }
                transaction.Commit();
                transaction.Dispose();


                DataTable dtH = getDTHeaderData(txtTransferId.Text.Trim());
                GetCustomData(dtItems);

                PrintTransferOrder(dtH, dtCustom, 0);

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

        private void GetCustomData(DataTable dtFromAx)
        {
            dtCustom = new DataTable("dtCustom");

            dtCustom.Columns.Add("ItemId", typeof(string));
            dtCustom.Columns.Add("ITEMNAME", typeof(string));
            dtCustom.Columns.Add("PdsCWQtyTransfer", typeof(decimal));
            dtCustom.Columns.Add("GrossWt", typeof(decimal));
            dtCustom.Columns.Add("StoneWt", typeof(decimal));
            dtCustom.Columns.Add("NettQty", typeof(decimal));
            dtCustom.Columns.Add("CONFIGID", typeof(string));
            dtCustom.Columns.Add("CRWTransferAmount", typeof(decimal));
            dtCustom.Columns.Add("HSNCode", typeof(string));
            dtCustom.Columns.Add("TotalBaseAmt", typeof(decimal));
            dtCustom.Columns.Add("DmdQty", typeof(decimal));
            dtCustom.Columns.Add("InventBatchId", typeof(string));
            dtCustom.Columns.Add("UnitPrice", typeof(decimal));
            dtCustom.Columns.Add("TaxValue", typeof(decimal));
            dtCustom.Columns.Add("TaxAmount", typeof(decimal));
            dtCustom.Columns.Add("SetOf", typeof(int));

            dtCustom.AcceptChanges();

            if (dtFromAx != null && dtFromAx.Rows.Count > 0)
            {
                for (int i = 0; i <= dtFromAx.Rows.Count - 1; i++)
                {
                    var row = dtCustom.NewRow();
                    bool isSKU = IsRetailItem(Convert.ToString(dtFromAx.Rows[i]["ItemId"]));
                    bool RepairItem = IsRepairItem(Convert.ToString(dtFromAx.Rows[i]["ItemId"]));
                    bool BulkItem = IsBulkItem(Convert.ToString(dtFromAx.Rows[i]["ItemId"]));

                    row["ItemId"] = Convert.ToString(dtFromAx.Rows[i]["ItemId"]);
                    row["ITEMNAME"] = Convert.ToString(dtFromAx.Rows[i]["NAME"]);
                    //=============================Soutik==========================================================14-10-2019
                    if (Convert.ToDecimal(dtFromAx.Rows[i]["PdsCWQtyTransfer"]) != 0)
                    {
                        row["PdsCWQtyTransfer"] = Convert.ToDecimal(dtFromAx.Rows[i]["PdsCWQtyTransfer"]);
                    }
                    else if (Convert.ToDecimal(dtFromAx.Rows[i]["QtyTransfer"]) != 0 && !RepairItem && !BulkItem)
                    {
                        row["PdsCWQtyTransfer"] = Convert.ToDecimal(dtFromAx.Rows[i]["QtyTransfer"]);
                    }
                    else
                        row["PdsCWQtyTransfer"] = 0;

                    if (RepairItem || BulkItem)
                        row["GrossWt"] = Convert.ToDecimal(dtFromAx.Rows[i]["QtyTransfer"]);
                    else
                        row["GrossWt"] = Convert.ToDecimal(dtFromAx.Rows[i]["GrossWt"]);

                    if (isSKU)
                    {
                        row["StoneWt"] = GetStoneWeight(Convert.ToString(dtFromAx.Rows[i]["ItemId"]));
                    }
                    else
                    {
                        row["StoneWt"] = Convert.ToDecimal(dtFromAx.Rows[i]["GrossWt"]) - Convert.ToDecimal(dtFromAx.Rows[i]["NetWt"]);
                    }
                    
                    row["NettQty"] = Convert.ToDecimal(dtFromAx.Rows[i]["NetWt"]);
                    row["CONFIGID"] = Convert.ToString(dtFromAx.Rows[i]["CONFIGID"]);
                    row["CRWTransferAmount"] = Convert.ToDecimal(dtFromAx.Rows[i]["TotalCostPrice"]);
                    row["HSNCode"] = Convert.ToString(dtFromAx.Rows[i]["HSNCode"]);
                    row["TotalBaseAmt"] = Convert.ToDecimal(dtFromAx.Rows[i]["TotalBaseAmt"]);
                    row["DmdQty"] = Convert.ToDecimal(dtFromAx.Rows[i]["DmdQty"]);
                    row["InventBatchId"] = Convert.ToString(dtFromAx.Rows[i]["InventBatchId"]);
                    row["UnitPrice"] = Convert.ToDecimal(dtFromAx.Rows[i]["UnitPrice"]);
                    row["TaxValue"] = Convert.ToDecimal(dtFromAx.Rows[i]["TaxValue"]);
                    row["TaxAmount"] = Convert.ToDecimal(dtFromAx.Rows[i]["TaxAmount"]);
                    //===========================Soutik===================================================
                    row["SetOf"] = GetSetOf(Convert.ToString(dtFromAx.Rows[i]["ItemId"]));
                    dtCustom.Rows.Add(row);
                }
            }

        }

        private bool IsBulkItem(string sItemId)
        {
            bool bBulkItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISBULK INT  SET @ISBULK = 0 IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN SET @ISBULK = 1 END SELECT @ISBULK";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bBulkItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();
            command.Dispose();

            return bBulkItem;
        }
        private int GetSetOf(string sItemId)
        {
            int iReturn = 0;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT top 1 isnull(SETOF,0) FROM INVENTTABLE WHERE ITEMID = '" + sItemId.Trim() + "'"; 

            SqlCommand command = new SqlCommand(commandText, connection);
            iReturn = Convert.ToInt16(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return iReturn;
        }

        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select RETAIL from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRetailItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bRetailItem;
        }

        private bool IsRepairItem(string sItemId)
        {
            bool bRepairItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select REPAIRITEM from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRepairItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bRepairItem;
        }

        private decimal GetStoneWeight(string sItemId)
        {
            decimal dTransStoneWt = 0m;
            DataTable dtTransferIngredients = new DataTable();

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" SELECT SKULine_Posted.SkuNumber,SKULine_Posted.ItemID,CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY, SKULine_Posted.UnitID,X.METALTYPE ");
            commandText.Append(" FROM  SKULine_Posted INNER JOIN "); 
            commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID  ");
            commandText.Append(" WHERE INVENTDIM.DATAAREAID= '" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" AND  [SkuNumber] = '" + sItemId.Trim() + "' ORDER BY X.METALTYPE ");

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                dtTransferIngredients.Load(reader);
            }
            if (dtTransferIngredients != null && dtTransferIngredients.Rows.Count > 0)
            {
                foreach (DataRow dr in dtTransferIngredients.Rows)
                {

                    if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone)
                    {
                        if (Convert.ToString(dr["UnitID"]) == "CT")
                            dTransStoneWt += decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(0.2), 3, MidpointRounding.AwayFromZero);
                        else
                            dTransStoneWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                    }

                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dTransStoneWt;
        }


        private DataTable getDTHeaderData(string sTransferID)
        {
            DataTable dtHeader = new DataTable("dtHeader");
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    DataSet dsHdr = new DataSet();
                    DataSet dsDtl = new DataSet();
                    ReadOnlyCollection<object> cTransReport;


                    cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherReceiptInfo", sTransferID);


                    StringReader srTransHdr = new StringReader(Convert.ToString(cTransReport[3]));

                    if (Convert.ToString(cTransReport[3]).Trim().Length > 38)
                    {
                        dsHdr.ReadXml(srTransHdr);
                    }

                    dtHeader = dsHdr.Tables[0];

                }
            }
            catch (Exception ex)
            {

            }

            return dtHeader;
        }

        private void PrintTransferOrder(DataTable dtHeader, DataTable dtItems, int iOption)
        {
            try
            {
                DataSet dsHdr = new DataSet();
                DataSet dsDtl = new DataSet();


                dsHdr.Tables.Add(dtHeader.Copy());
                dsDtl.Tables.Add(dtCustom.Copy());

                Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmTransOrderCreateRpt reportfrm
                    = new Report.frmTransOrderCreateRpt(dsHdr, dsDtl, iOption, "TRANSFER ORDER RECEIVE");

                reportfrm.ShowDialog();

            }
            catch (Exception ex)
            {


            }

        }
    }
}
