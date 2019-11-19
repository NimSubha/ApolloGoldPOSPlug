using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.DataAccess.DataUtil;


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmTransferOrder : frmTouchBase
    {
        string sInventLocationId = string.Empty;
        DataTable dtSelectedSKU;
        DataTable dtSkuForShow;
        public IPosTransaction pos { get; set; }
        [Import]
        private IApplication application;

        DataSet dsSKU;

        SqlConnection connection;
        public frmTransferOrder(SqlConnection Conn)
        {
            InitializeComponent();

            connection = Conn;
            dtSelectedSKU = new DataTable("SKUINFO");

            dtSelectedSKU.Columns.Add("STOREID", typeof(string));
            dtSelectedSKU.Columns.Add("TWH", typeof(string));
            dtSelectedSKU.Columns.Add("WAYBILL", typeof(string));
            dtSelectedSKU.Columns.Add("AWBNUM", typeof(string));

            dtSelectedSKU.Columns.Add("SKUNumber", typeof(string));
            dtSelectedSKU.AcceptChanges();
            DataColumn[] columns = new DataColumn[1];
            columns[0] = dtSelectedSKU.Columns["SKUNumber"];
            dtSelectedSKU.PrimaryKey = columns;

            dtSkuForShow = new DataTable();
            dtSkuForShow.Columns.Add("SKUNumber", typeof(string));
            dtSkuForShow.Columns.Add("Name", typeof(string));
            dtSkuForShow.Columns.Add("PCS", typeof(decimal));
            dtSkuForShow.Columns.Add("Weight", typeof(decimal));

            dtSkuForShow.AcceptChanges();
            DataColumn[] columns1 = new DataColumn[1];
            columns[0] = dtSkuForShow.Columns["SKUNumber"];
            dtSkuForShow.PrimaryKey = columns1;

        }

        private void btnWHSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetToWarehouse", sStoreId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], null, "Warehouse");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            sInventLocationId = Convert.ToString(dr["InventLocationId"]);
                            txtWarehouse.Text = Convert.ToString(dr["Name"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void btnClearProduct_Click(object sender, EventArgs e)
        {
            txtSKUNo.Text = string.Empty;
            txtSKUNo.Focus();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            string sTransferId = string.Empty;
            string sStoreId = ApplicationSettings.Database.StoreID;
            string sSKUnumber = string.Empty;

            if (dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
            {
                try
                {
                    ReadOnlyCollection<object> containerArray;
                    string sMsg = string.Empty;
                    MemoryStream mstr = new MemoryStream();
                    foreach (DataRow dr in dtSelectedSKU.Rows)
                    {
                        dr["WAYBILL"] = txtWayBillNo.Text.Trim();
                        dr["AWBNUM"] = txtAirwayBillNo.Text.Trim();
                    }
                    dtSelectedSKU.AcceptChanges();

                    dtSelectedSKU.WriteXml(mstr, true);

                    mstr.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(mstr);
                    string sSKU = string.Empty;
                    sSKU = sr.ReadToEnd();

                    foreach (DataRow dr in dtSelectedSKU.Rows)
                    {
                        sSKUnumber = Convert.ToString(dr["SKUNumber"]);
                        break;
                    }

                    string sOperatorID = ApplicationSettings.Terminal.TerminalOperator.OperatorId;// added on 061217
                    string sTerminalID = ApplicationSettings.Terminal.TerminalId;// added on 061217


                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        bool bStatus = false;
                        string sRemarks = txtRemarks.Text;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("TransferOrderCreate", sSKU, sRemarks, sOperatorID, sTerminalID);
                        bStatus = Convert.ToBoolean(containerArray[1]);
                        sMsg = Convert.ToString(containerArray[2]);

                        if (bStatus)
                        {
                            #region if true
                            if (dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
                            {
                                UpdateDataIntoLocal(dtSelectedSKU);
                            }


                            MessageBox.Show(sMsg);
                            sTransferId = Convert.ToString(containerArray[3]);
                            if (!string.IsNullOrEmpty(sTransferId))
                            {
                                DataSet dsHdr = new DataSet();
                                DataSet dsDtl = new DataSet();
                                ReadOnlyCollection<object> cTransReport;
                                cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransferId);
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
                            #endregion
                        }
                        else
                        {
                            #region rechecking TO created or not added on  26/07/17
                            if (PosApplication.Instance.TransactionServices.CheckConnection())
                            {
                                ReadOnlyCollection<object> containerArray1;

                                containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("checkTransferOrderShip", sSKUnumber, sStoreId);

                                bStatus = Convert.ToBoolean(containerArray1[1]);

                                if (bStatus)
                                {
                                    if (dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
                                    {
                                        UpdateDataIntoLocal(dtSelectedSKU);
                                    }

                                    MessageBox.Show(sMsg);
                                    sTransferId = Convert.ToString(containerArray1[2]);
                                    if (!string.IsNullOrEmpty(sTransferId))
                                    {
                                        DataSet dsHdr = new DataSet();
                                        DataSet dsDtl = new DataSet();
                                        ReadOnlyCollection<object> cTransReport;
                                        cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransferId);
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
                                    #region rechecking TO created or not added on  25/08/17
                                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                                    {
                                        ReadOnlyCollection<object> containerArray2;

                                        containerArray2 = PosApplication.Instance.TransactionServices.InvokeExtension("checkTransferOrderShip", sSKUnumber, sStoreId);

                                        bStatus = Convert.ToBoolean(containerArray2[1]);

                                        if (bStatus)
                                        {
                                            if (dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
                                            {
                                                UpdateDataIntoLocal(dtSelectedSKU);
                                            }

                                            MessageBox.Show(sMsg);
                                            sTransferId = Convert.ToString(containerArray2[2]);
                                            if (!string.IsNullOrEmpty(sTransferId))
                                            {
                                                DataSet dsHdr = new DataSet();
                                                DataSet dsDtl = new DataSet();
                                                ReadOnlyCollection<object> cTransReport;
                                                cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransferId);
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
                                            MessageBox.Show(sMsg);

                                    }
                                    else
                                        MessageBox.Show(sMsg);
                                    #endregion
                                }

                            }
                            else
                                MessageBox.Show(sMsg);
                            #endregion
                        }
                        ClearControls();
                    }
                }

                catch (Exception ex)
                {

                    #region rechecking TO created or not added on  26/07/17

                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray1;

                        containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("checkTransferOrderShip", sSKUnumber, sStoreId);

                        bool bStatus = Convert.ToBoolean(containerArray1[1]);

                        if (bStatus)
                        {
                            if (dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
                            {
                                UpdateDataIntoLocal(dtSelectedSKU);
                            }


                            sTransferId = Convert.ToString(containerArray1[2]);
                            if (!string.IsNullOrEmpty(sTransferId))
                            {
                                DataSet dsHdr = new DataSet();
                                DataSet dsDtl = new DataSet();
                                ReadOnlyCollection<object> cTransReport;
                                cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransferId);
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
                            MessageBox.Show("Transfer Order failed to create");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Transfer Order failed to create");
                    }
                    #endregion

                    ClearControls();

                }
            }

        }

        private void btnSKUSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetSKUNumber");

                    dsSKU = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsSKU.ReadXml(srTransDetail);
                    }

                    int iNoOfPcs = 0;
                    decimal dTotQty = 0;

                    if (dsSKU != null && dsSKU.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsSKU.Tables[0], null, "SKU");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        DataRow drSKU;

                        if (dr != null)
                        {
                            txtSKUNo.Text = Convert.ToString(dr["itemid"]);

                            drSKU = dtSelectedSKU.NewRow();

                            drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
                            drSKU["TWH"] = sInventLocationId;
                            drSKU["SKUNumber"] = Convert.ToString(dr["itemid"]);
                            dtSelectedSKU.Rows.Add(drSKU);
                            dtSelectedSKU.AcceptChanges();
                            grItems.DataSource = dtSelectedSKU.DefaultView;

                            //Start : added on 10/05/2017
                            foreach (DataRow dr1 in dtSelectedSKU.Rows)
                            {
                                iNoOfPcs = iNoOfPcs + Convert.ToInt32(dr1[2]);
                                dTotQty = dTotQty + Convert.ToDecimal(dr1[3]);
                            }

                            lblTotPcs.Text = Convert.ToString(iNoOfPcs);
                            lblTotQty.Text = Convert.ToString(dTotQty);
                            //End : added on 10/05/2017

                        }
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearControls()
        {
            txtWarehouse.Text = string.Empty;
            txtSKUNo.Text = string.Empty;
            txtWayBillNo.Text = string.Empty;
            txtAirwayBillNo.Text = string.Empty;
            grItems.DataSource = null;
            dtSelectedSKU.Clear();
            dtSelectedSKU = new DataTable();
            lblTotPcs.Text = "0";
            lblTotQty.Text = "0";
            txtRemarks.Text = "";
            txtStockTransId.Text = "";
        }

        private void txtSKUNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                btnEnter_Click(sender, e);
            }
        }

        private bool IsValidSKU(string sSKUNo)
        {
            bool bStatus = false;

            string sQry = "DECLARE @ISVALID INT" +
                         " IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sSKUNo + "'" +
                         " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND RETAIL = 1) " +
                         " AND EXISTS (SELECT SkuNumber FROM SKUTableTrans WHERE SkuNumber = '" + sSKUNo + "'" +
                         " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND isAvailable  = 0) " +
                         " BEGIN SET @ISVALID = 1 END ELSE BEGIN SET @ISVALID = 0 END SELECT ISNULL(@ISVALID,0)";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            using (SqlCommand cmd = new SqlCommand(sQry, connection))
            {
                cmd.CommandTimeout = 0;
                bStatus = Convert.ToBoolean(cmd.ExecuteScalar());
            }

            return bStatus;
        }

        private void txtSKUNo_Leave(object sender, EventArgs e)
        {
            //if(!string.IsNullOrEmpty(txtSKUNo.Text.Trim()))
            //{
            //    if(IsValidSKU(txtSKUNo.Text.Trim()))
            //    {
            //        DataRow[] drArrEx = dtSelectedSKU.Select("SKUNumber = '" + txtSKUNo.Text.Trim() + "' ");
            //        if(drArrEx != null && drArrEx.Length == 0)
            //        {
            //            DataRow drSKU;
            //            drSKU = dtSelectedSKU.NewRow();
            //            drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
            //            drSKU["TWH"] = sInventLocationId;
            //            drSKU["SKUNumber"] = txtSKUNo.Text.Trim();
            //            dtSelectedSKU.Rows.Add(drSKU);
            //            dtSelectedSKU.AcceptChanges();
            //            grItems.DataSource = dtSelectedSKU.DefaultView;
            //        }
            //    }
            //    else
            //    {
            //        MessageBox.Show("Invalid Item Selected.");
            //    }
            //}
            btnEnter_Click(sender, e);
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtWarehouse.Text))
            {
                AddItemForTransfer(txtSKUNo.Text.Trim());
            }
            else
            {
                MessageBox.Show("Please select a warehouse");
                txtWarehouse.Focus();
            }
        }

        private void AddItemForTransfer(string sSKUNo)
        {
            if (!string.IsNullOrEmpty(sSKUNo))
            {
                if (IsValidSKU(sSKUNo))
                {
                    DataRow[] drArrEx = dtSelectedSKU.Select("SKUNumber = '" + sSKUNo + "' ");
                    if (drArrEx != null && drArrEx.Length == 0)
                    {
                        DataRow drSKU;
                        drSKU = dtSelectedSKU.NewRow();
                        drSKU["STOREID"] = ApplicationSettings.Terminal.StoreId;
                        drSKU["TWH"] = sInventLocationId;
                        drSKU["SKUNumber"] = sSKUNo;
                        dtSelectedSKU.Rows.Add(drSKU);
                        dtSelectedSKU.AcceptChanges();
                        // grItems.DataSource = dtSelectedSKU.DefaultView;
                    }

                    //for show in grid
                    DataTable dtSkuDetails = new DataTable();

                    dtSkuDetails = GetSKUInfo(sSKUNo);
                    //dtSkuForShow = dtSkuDetails.Clone();
                    if (drArrEx != null && drArrEx.Length == 0)
                    {
                        if (dtSkuDetails != null && dtSkuDetails.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtSkuDetails.Rows)
                            {
                                DataRow drSKU;
                                drSKU = dtSkuForShow.NewRow();
                                drSKU["SKUNumber"] = dr["SKUNumber"];
                                drSKU["Name"] = dr["Name"];
                                drSKU["PCS"] = dr["PCS"]; ;
                                drSKU["Weight"] = dr["Weight"];

                                dtSkuForShow.Rows.Add(drSKU);
                                dtSkuForShow.AcceptChanges();
                            }
                        }
                    }
                    grItems.DataSource = dtSkuForShow.DefaultView;
                    txtSKUNo.Text = "";
                    int iNoOfPcs = 0;
                    decimal dTotQty = 0m;
                    //Start : added on 10/05/2017
                    foreach (DataRow dr1 in dtSkuForShow.Rows)
                    {
                        iNoOfPcs = iNoOfPcs + Convert.ToInt32(dr1[2]);
                        dTotQty = dTotQty + Convert.ToDecimal(dr1[3]);
                    }

                    lblTotPcs.Text = Convert.ToString(iNoOfPcs);
                    lblTotQty.Text = Convert.ToString(dTotQty);
                    //End : added on 10/05/2017

                }
                else
                {
                    MessageBox.Show("Invalid Item Selected.");
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if (dtSelectedSKU != null && dtSelectedSKU.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtSelectedSKU.Rows[DeleteSelectedIndex];
                    DataRow theRowToDelete1 = dtSkuForShow.Rows[DeleteSelectedIndex];

                    dtSelectedSKU.Rows.Remove(theRowToDelete);
                    dtSkuForShow.Rows.Remove(theRowToDelete1);
                    //grItems.DataSource = dtSelectedSKU.DefaultView;

                    grItems.DataSource = dtSkuForShow.DefaultView;

                    int iNoOfPcs = 0;
                    decimal dTotQty = 0m;
                    //Start : added on 10/05/2017
                    foreach (DataRow dr1 in dtSkuForShow.Rows)
                    {
                        iNoOfPcs = iNoOfPcs + Convert.ToInt32(dr1[2]);
                        dTotQty = dTotQty + Convert.ToDecimal(dr1[3]);
                    }

                    lblTotPcs.Text = Convert.ToString(iNoOfPcs);
                    lblTotQty.Text = Convert.ToString(dTotQty);
                    //End : added on 10/05/2017
                }
            }
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
                        string sqlUpd = " IF EXISTS (SELECT Itemid FROM TransferOrderTrans WHERE Itemid = '" + Convert.ToString(dtItems.Rows[ItemCount]["SKUNUMBER"]) + "' and ReceivedOrShipped = 0)" +
                                        " BEGIN UPDATE TransferOrderTrans SET ReceivedOrShipped = 1 WHERE Itemid = '" + Convert.ToString(dtItems.Rows[ItemCount]["SKUNUMBER"]) + "' END";

                        SqlCommand command = new SqlCommand(sqlUpd, transaction.Connection, transaction);
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
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

        private DataTable GetSKUInfo(string sSkuNumber)  // SKU allow
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            DataTable dt = new DataTable();
            string commandText = "select SKUNumber,CASE WHEN d.METALTYPE =14 THEN " +
                                " CAST(ISNULL(sk.QTY,0) AS NUMERIC (28,0)) " +
                                " ELSE CAST(ISNULL(sk.PDSCWQTY,0) AS NUMERIC (28,3))  END AS PCS, " +
                                " CAST(ISNULL(SK.GrossWeight,0) AS DECIMAL(28,3)) as Weight," +
                                " (select top 1 f.NAME from ECORESPRODUCTTRANSLATION F" +
                                " left join  ECORESPRODUCT E ON E.RECID = F.PRODUCT" +
                                " where e.RECID =d.product) as Name" +
                                " from SKUTable_Posted sk" +
                                " LEFT JOIN INVENTTABLE D ON sk.SkuNumber =d.ITEMID where sk.SkuNumber='" + sSkuNumber + "'";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtWarehouse.Text))
            {
                if (!string.IsNullOrEmpty(txtStockTransId.Text))
                {
                    SqlConnection conn = new SqlConnection();
                    if (application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;

                    DataTable dtItems = new DataTable();
                    //string commandText = " select a.SKUNUMBER from SKUTRANSFER_DETAILS a " +
                    //                    " left join SKUTRANSFER_HEADER b on a.STOCKTRANSFERID=b.STOCKTRANSFERID" +
                    //                    " where b.STOCKTRANSFERID='" + txtStockTransId.Text.Trim() + "'";//commented on 230418

                    string commandText = " select Itemid SKUNUMBER from TransferOrderTrans where TransferOrder ='" + txtStockTransId.Text.Trim() + "' and ReceivedOrShipped = 0";
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    using (SqlCommand command = new SqlCommand(commandText, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(dtItems);
                    }
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                    if (dtItems != null && dtItems.Rows.Count > 0)
                    {
                        for (int ItemCount = 0; ItemCount < dtItems.Rows.Count; ItemCount++)
                        {
                            AddItemForTransfer(Convert.ToString(dtItems.Rows[ItemCount]["SKUNUMBER"]));
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a valid stock out number");
                    txtStockTransId.Focus();
                }
            }
            else
            {
                MessageBox.Show("Please select a warehouse");
                txtWarehouse.Focus();
            }
        }

        private void btnStockOutSearch_Click(object sender, EventArgs e)
        {
            try
            {

                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                DataTable dtItems = new DataTable();
                string commandText = " select distinct TransferOrder from TransferOrderTrans " +
                             " where  ReceivedOrShipped = 0 and TransferOrder " +
                             " in(select STOCKTRANSFERID from SKUTRANSFER_HEADER where STOCKTRANSFERTYPE='StockOut')";

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                using (SqlCommand command = new SqlCommand(commandText, conn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dtItems);
                }
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (dtItems != null && dtItems.Rows.Count > 0)
                {
                    Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dtItems, null, "Transfer Order");
                    Osearch.ShowDialog();

                    DataRow dr = Osearch.SelectedDataRow;

                    if (dr != null)
                    {
                        txtStockTransId.Text = Convert.ToString(dr["TransferOrder"]);
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
    }
}
