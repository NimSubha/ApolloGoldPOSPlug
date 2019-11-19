using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using LSRetailPosis.Settings;
using System.IO;
using System.Data.SqlClient;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmBulkItemShipment:Form
    {
        string sInventLocationId = string.Empty;
        string sReceiptId = string.Empty;
        public IPosTransaction pos { get; set; }
        [Import]
        private IApplication application;

        DataTable dtSelectedBI = new DataTable("BI");

        public frmBulkItemShipment()
        {
            InitializeComponent();
        }

        private void btnWHSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if(PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetToWarehouse", sStoreId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if(Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if(dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], null, "Warehouse");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if(dr != null)
                        {
                            sInventLocationId = Convert.ToString(dr["InventLocationId"]);
                            txtWarehouse.Text = Convert.ToString(dr["Name"]);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void btnBIRFetch_Click(object sender, EventArgs e)
        {
            DataTable dt = GetBulkItemIssueReceipt();

            if(dt != null && dt.Rows.Count > 0)
            {
                Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Bulk Item Receipt List");
                Osearch.ShowDialog();

                DataRow dr = Osearch.SelectedDataRow;

                if(dr != null)
                {
                    sReceiptId = Convert.ToString(dr["ReceiptId"]);
                    txtIssueReceipt.Text = sReceiptId;
                }
            }

        }

        private DataTable GetBulkItemIssueReceipt()
        {
            string sSQL = "select ReceiptId,CONVERT(VARCHAR(11),TransDate,103) TransDate from BulkItemIssueHeader WHERE ISTRANSFERED=0";
            return GetDataTable(sSQL);
        }

        private static DataTable GetDataTable(string sSQL)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = sSQL;// "select ReceiptId,TransDate from BulkItemIssueHeader";

                DataTable dt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            dtSelectedBI = new DataTable("BI");
            if(!string.IsNullOrEmpty(txtIssueReceipt.Text))
            {
                //string sSQL = "select ITEMID,CONFIGID,INVENTCOLORID,INVENTSIZEID," +
                //             " INVENTSTYLEID,INVENTBATCHID,Abs(PDSCWQTY) as PCS,Abs(QTY) as QUANTITY,Abs(NetWt) as NetWt,'' STOREID," +
                //             " '' TWH, '' WAYBILL,'' AWBNUM" +
                //             " from BulkItemIssueDetails " +
                //             " where ReceiptId='" + txtIssueReceipt.Text.Trim() + "' ";
                //string sSQL =  "select BI.ITEMID,CONFIGID,INVENTCOLORID,INVENTSIZEID,"+
                //               " INVENTSTYLEID,BI.INVENTBATCHID,Abs(PDSCWQTY) as PCS,Abs(BI.QTY) as QUANTITY,Abs(NetWt) as NetWt,'' STOREID,"+
                //               " '' TWH, '' WAYBILL,'' AWBNUM,"+
                //               " (CASE WHEN I.OTHERDMD = 1 or I.OWNDMD=1 THEN "+
                //               " CAST(ISNULL((T.NETAMOUNT/t.QTY) * Abs(BI.QTY),0) AS DECIMAL(28,2))  else 0 end) AMOUNT "+
                //               " from BulkItemIssueDetails BI"+
                //               " left join RETAILTRANSACTIONSALESTRANS T on BI.InventBatchId=T.INVENTBATCHID"+
                //               " left join INVENTTABLE I on BI.ItemId =I.ITEMID" +
                //               " where BI.ReceiptId='" + txtIssueReceipt.Text.Trim() + "' ";

                
                string sSQL =  "IF NOT EXISTS(SELECT TOP 1 I.ItemId from BulkItemIssueDetails BI"+
	                           " left join RETAILTRANSACTIONSALESTRANS T on BI.InventBatchId=T.INVENTBATCHID "+
	                           " and t.TRANSACTIONSTATUS=0 and bi.ItemId=t.ITEMID"+
                               " left join INVENTTABLE I on BI.ItemId =I.ITEMID where BI.ReceiptId='" + txtIssueReceipt.Text.Trim() + "'" +
	                           " and (OTHERDMD = 1 or OWNDMD=1)) "+
	                           " BEGIN "+
	                           " select ITEMID,CONFIGID,INVENTCOLORID,INVENTSIZEID,"+
		                            " INVENTSTYLEID,INVENTBATCHID,Abs(PDSCWQTY) as PCS,Abs(QTY) as QUANTITY,Abs(NetWt) as NetWt,'' STOREID,"+
		                            " '' TWH, '' WAYBILL,'' AWBNUM"+
		                            " from BulkItemIssueDetails"+
                                    " where ReceiptId='" + txtIssueReceipt.Text.Trim() + "'" +
	                            " end"+
                            " else "+
	                            " begin "+
		                            " select BI.ITEMID,CONFIGID,INVENTCOLORID,INVENTSIZEID, INVENTSTYLEID,BI.INVENTBATCHID,"+
		                            " Abs(PDSCWQTY) as PCS,Abs(BI.QTY) as QUANTITY,Abs(NetWt) as NetWt,'' STOREID, '' TWH, '' WAYBILL,'' AWBNUM,"+
		                            " (CASE WHEN I.OTHERDMD = 1 or I.OWNDMD=1 THEN  CAST(ISNULL((T.NETAMOUNT/t.QTY) * Abs(BI.QTY),0) AS DECIMAL(28,2))"+
		                            "  else 0 end) AMOUNT  "+
		                            "  from BulkItemIssueDetails BI"+
		                            "   left join RETAILTRANSACTIONSALESTRANS T on BI.InventBatchId=T.INVENTBATCHID "+
		                            "   and t.TRANSACTIONSTATUS=0 and bi.ItemId=t.ITEMID"+
                                    " left join INVENTTABLE I on BI.ItemId =I.ITEMID where BI.ReceiptId='" + txtIssueReceipt.Text.Trim() + "'" +
	                            "end";


                

                dtSelectedBI = GetDataTable(sSQL);

                if(dtSelectedBI != null && dtSelectedBI.Rows.Count > 0)
                {
                    grItems.DataSource = dtSelectedBI;
                }

                CalTotal(dtSelectedBI);
            }
        }

        private void CalTotal(DataTable dtItem)
        {
            int iNoOfPcs = 0;
            decimal dTotQty = 0m;
            //Start : added on 10/05/2017
            foreach (DataRow dr1 in dtItem.Rows)
            {
                iNoOfPcs = iNoOfPcs + Convert.ToInt32(dr1["PCS"]);
                dTotQty = dTotQty + Convert.ToDecimal(dr1["QUANTITY"]);
            }

            lblTotPcs.Text = Convert.ToString(iNoOfPcs);
            lblTotQty.Text = Convert.ToString(dTotQty);
            //End : added on 10/05/2017
        }

        private void frmBulkItemShipment_Load(object sender, EventArgs e)
        {

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if(dtSelectedBI != null && dtSelectedBI.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtSelectedBI.Rows[DeleteSelectedIndex];

                    dtSelectedBI.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtSelectedBI.DefaultView;
                }

                CalTotal(dtSelectedBI);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(sInventLocationId))
            {
                if (dtSelectedBI != null && dtSelectedBI.Rows.Count > 0)
                {
                    try
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sMsg = string.Empty;
                        MemoryStream mstr = new MemoryStream();


                        foreach (DataRow dr in dtSelectedBI.Rows)
                        {
                            dr["STOREID"] = ApplicationSettings.Terminal.StoreId;
                            dr["TWH"] = sInventLocationId;
                            dr["WAYBILL"] = txtWayBillNo.Text.Trim();
                            dr["AWBNUM"] = txtAirwayBillNo.Text.Trim();
                        }
                        dtSelectedBI.AcceptChanges();

                        DataTable dtBI = new DataTable("BI");
                        dtBI.Columns.Add("ITEMID", typeof(string));
                        dtBI.Columns.Add("CONFIGID", typeof(string));
                        dtBI.Columns.Add("INVENTCOLORID", typeof(string));
                        dtBI.Columns.Add("INVENTSIZEID", typeof(string));
                        dtBI.Columns.Add("INVENTSTYLEID", typeof(string));
                        dtBI.Columns.Add("INVENTBATCHID", typeof(string));
                        dtBI.Columns.Add("PCS", typeof(decimal));
                        dtBI.Columns.Add("QUANTITY", typeof(decimal));
                        dtBI.Columns.Add("NETWT", typeof(decimal));
                        

                        dtBI.Columns.Add("STOREID", typeof(string));
                        dtBI.Columns.Add("TWH", typeof(string));
                        dtBI.Columns.Add("WAYBILL", typeof(string));
                        dtBI.Columns.Add("AWBNUM", typeof(string));
                        dtBI.Columns.Add("AMOUNT", typeof(decimal));

                        foreach (DataRow dr in dtSelectedBI.Rows)
                        {
                            dtBI.ImportRow(dr);
                        }
                        dtBI.AcceptChanges();

                        DataColumn[] columns = new DataColumn[6];
                        columns[0] = dtBI.Columns["ITEMID"];
                        columns[1] = dtBI.Columns["CONFIGID"];
                        columns[2] = dtBI.Columns["INVENTBATCHID"];
                        columns[3] = dtBI.Columns["INVENTCOLORID"];
                        columns[4] = dtBI.Columns["INVENTSIZEID"];
                        columns[5] = dtBI.Columns["INVENTSTYLEID"];

                        dtBI.PrimaryKey = columns;

                        dtBI.WriteXml(mstr, true);

                        mstr.Seek(0, SeekOrigin.Begin);
                        StreamReader sr = new StreamReader(mstr);
                        string sBulk = string.Empty;
                        sBulk = sr.ReadToEnd();
                        if (PosApplication.Instance.TransactionServices.CheckConnection())
                        {
                            bool bStatus = false;
                            string sTransferId = string.Empty;

                            string sRemarks = txtRemarks.Text;
                            string sOperatorID = ApplicationSettings.Terminal.TerminalOperator.OperatorId;// added on 061217
                            string sTerminalID = ApplicationSettings.Terminal.TerminalId;// added on 061217

                            containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("BulkTransferOrderCreate", sBulk, sRemarks, sOperatorID, sTerminalID);
                            bStatus = Convert.ToBoolean(containerArray[1]);
                            sMsg = Convert.ToString(containerArray[2]);

                            if (bStatus)
                            {
                                if (dtBI != null && dtBI.Rows.Count > 0)
                                {
                                    UpdateDataIntoLocal(dtBI);
                                }

                               
                                MessageBox.Show(sMsg);
                                sTransferId = Convert.ToString(containerArray[3]);
                                UpdateBulkItemIssueHeader(1, sTransferId);

                                if (!string.IsNullOrEmpty(sTransferId))
                                {
                                    DataSet dsHdr = new DataSet();
                                    DataSet dsDtl = new DataSet();
                                    ReadOnlyCollection<object> cTransReport;
                                    cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetBulkTransferVoucherInfo", sTransferId);
                                    StringReader srTransHdr = new StringReader(Convert.ToString(cTransReport[3]));
                                    if (Convert.ToString(cTransReport[3]).Trim().Length > 38)
                                    {
                                        dsHdr.ReadXml(srTransHdr);
                                    }
                                    StringReader srTransDetail = new StringReader(Convert.ToString(cTransReport[4]));
                                    if (Convert.ToString(cTransReport[4]).Trim().Length > 38)
                                    {
                                        dsDtl.ReadXml(srTransDetail);
                                    }

                                    Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmTransOrderCreateRpt reportfrm
                                        = new Report.frmTransOrderCreateRpt(dsHdr, dsDtl, 0, "STOCK TRANSFER");

                                    reportfrm.ShowDialog();
                                }
                            }
                            else
                            {
                                //UpdateBulkItemIssueHeader(0,sTransferId);
                               // MessageBox.Show("Transfer Order failed to create");
                                MessageBox.Show(sMsg);
                            }
                            ClearControls();
                        }
                    }

                    catch (Exception ex)
                    {
                        MessageBox.Show("Transfer Order failed to create");
                        ClearControls();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select to warehouse.");
                txtWarehouse.Focus();
            }
        }

        private void UpdateBulkItemIssueHeader(int iTransfered, string sTransVoucer)
        {
            SqlConnection connection = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE BulkItemIssueHeader SET ISTRANSFERED=" + iTransfered + "," +
                                " TRANSFEREDID='" + sTransVoucer + "' where RECEIPTID = '" + txtIssueReceipt.Text + "' "; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            if(connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void ClearControls()
        {
            txtIssueReceipt.Text = string.Empty;
            txtWarehouse.Text = string.Empty;
            txtWayBillNo.Text = string.Empty;
            txtAirwayBillNo.Text = string.Empty;
            grItems.DataSource = null;
            dtSelectedBI.Clear();
            lblTotPcs.Text = "0";
            lblTotQty.Text = "0";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateDataIntoLocal(DataTable dtItems)
        {
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlTransaction transaction = null;
            try
            {
                transaction = connection.BeginTransaction();

                if(dtItems != null && dtItems.Rows.Count > 0)
                {
                    for(int ItemCount = 0; ItemCount < dtItems.Rows.Count; ItemCount++)
                    {
                        string sqlUpd = " IF EXISTS (SELECT Itemid FROM TransferOrderTrans WHERE"+
                                        " Itemid = '" + Convert.ToString(dtItems.Rows[ItemCount]["ITEMID"]) + "'" +
                                        " AND CONFIGID = '" + Convert.ToString(dtItems.Rows[ItemCount]["CONFIGID"]) + "'" +
                                        " AND INVENTCOLORID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTCOLORID"]) + "'" +
                                        " AND INVENTSIZEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSIZEID"]) + "'" +
                                        " AND INVENTSTYLEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSTYLEID"]) + "'" +
                                        " AND INVENTBATCHID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTBATCHID"]) + "'" +
                                        " and ReceivedOrShipped = 0 and TRANSTYPE=1)" +
                                        " BEGIN "+
                                            " UPDATE TransferOrderTrans SET ReceivedOrShipped = 1"+
                                            " WHERE Itemid = '" + Convert.ToString(dtItems.Rows[ItemCount]["ITEMID"]) + "'" +
                                            " AND CONFIGID = '" + Convert.ToString(dtItems.Rows[ItemCount]["CONFIGID"]) + "'" +
                                            " AND INVENTCOLORID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTCOLORID"]) + "'" +
                                            " AND INVENTSIZEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSIZEID"]) + "'" +
                                            " AND INVENTSTYLEID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTSTYLEID"]) + "'" +
                                            " AND INVENTBATCHID = '" + Convert.ToString(dtItems.Rows[ItemCount]["INVENTBATCHID"]) + "'" +
                                            " and ReceivedOrShipped = 0 and TRANSTYPE=1"+
                                        " END";
                        SqlCommand command = new SqlCommand(sqlUpd, connection, transaction);
                        command.ExecuteNonQuery();
                    }
                }
                transaction.Commit();
                transaction.Dispose();
            }
            #region Exception
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
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
            #endregion
        }
    }
}
