using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using System.IO;
using LSRetailPosis.Settings;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmRepairInHouseTransfer:frmTouchBase
    {
        string sInventLocationId = string.Empty;
        [Import]
        public IApplication application;

        DataTable dtItemInfo = new DataTable("dtItemInfo");
        DataTable dtTemp = new DataTable("dtTemp");
        string sCompanyName = "";
        string sTransferId = string.Empty;

        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations oBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();

        public frmRepairInHouseTransfer()
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if(string.IsNullOrEmpty(txtRepairId.Text))
                {
                    DataTable dtGridItems = new DataTable();

                    string commandText = " SELECT  d.REPAIRID FROM RetailRepairDetail d"+
                                        " left join RetailRepairHdr h on d.RepairId=h.RepairId"+
                                        "  WHERE d.ISINHOUSETRANSFER=0 ORDER BY h.OrderDate,d.REPAIRID";

                    SqlConnection connection = new SqlConnection();

                    if(application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;


                    if(connection.State == ConnectionState.Closed)
                        connection.Open();
                    SqlCommand command = new SqlCommand(commandText, connection);

                    command.CommandTimeout = 0;
                    SqlDataReader reader = command.ExecuteReader();
                    dtGridItems = new DataTable();
                    dtGridItems.Load(reader);
                    if(dtGridItems != null && dtGridItems.Rows.Count > 0)
                    {
                        DataRow selRow = null;
                        Dialog.Dialog objCustOrderSearch = new Dialog.Dialog();

                        objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "HO Repair Transfer");
                        if(selRow != null)
                            txtRepairId.Text = Convert.ToString(selRow["RepairId"]);
                        else
                            return;
                    }
                    else
                    {
                        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                }
            }
            catch(Exception ex)
            {
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtRepairId.Text = "";
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];

                    UpdateRepairLine(theRowToDelete["RepairId"].ToString(), Convert.ToDecimal(theRowToDelete["LineNum"]), 0);

                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }
            }
            if(DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtRepairId.Text))
            {
                dtTemp = GetRepairItemInfo(txtRepairId.Text);
                List<DataRow> rows_to_remove = new List<DataRow>();
                if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    foreach(DataRow DR in dtTemp.Rows)
                    {
                        foreach(DataRow row in dtItemInfo.Rows)
                        {
                            if(DR["RepairId"].ToString() == row["RepairId"].ToString()
                                && DR["LineNum"].ToString() == row["LineNum"].ToString())
                            {
                                rows_to_remove.Add(DR);
                            }
                        }
                    }


                    foreach(DataRow row1 in rows_to_remove)
                    {
                        dtTemp.Rows.Remove(row1);
                        dtTemp.AcceptChanges();
                    }
                }

                DataRow dr;
                if(dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    dtItemInfo.Columns.Add("RepairId", typeof(string));
                    dtItemInfo.Columns.Add("ItemId", typeof(string));
                    dtItemInfo.Columns.Add("LineNum", typeof(decimal));
                    dtItemInfo.Columns.Add("BATCHID", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGID", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("PdsCWQty", typeof(decimal));
                    dtItemInfo.Columns.Add("Qty", typeof(decimal));
                    dtItemInfo.Columns.Add("InventDimId", typeof(string));
                    dtItemInfo.Columns.Add("UNITID", typeof(string));

                    dtItemInfo = dtTemp;

                    grItems.DataSource = dtItemInfo.DefaultView;
                    txtRepairId.Text = "";

                    foreach(DataRow row in dtItemInfo.Rows)
                    {
                        UpdateRepairLine(row["RepairId"].ToString(), Convert.ToDecimal(row["LineNum"]), 1);
                    }
                }
                else
                {
                    dr = dtItemInfo.NewRow();

                    if(dtTemp != null)
                    {
                        foreach(DataRow dr1 in dtTemp.Rows)
                        {
                            dtItemInfo.ImportRow(dr1);
                        }
                    }

                    grItems.DataSource = dtItemInfo.DefaultView;
                    txtRepairId.Text = "";

                    foreach(DataRow row in dtItemInfo.Rows)
                    {
                        UpdateRepairLine(row["RepairId"].ToString(), Convert.ToDecimal(row["LineNum"]), 1);
                    }
                }
            }
        }

        private DataTable GetRepairItemInfo(string sRepairId)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " select RepairId,ItemId,LineNum,BATCHID,CONFIGID,CODE as COLOR," +
                                      " SIZEID as SIZE,STYLE,PCS as PdsCWQty,Qty,InventDimId,UNITID " +
                                      " from RetailRepairDetail where RepairId='" + sRepairId + "' and ISINHOUSETRANSFER=0";

                DataTable repDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(repDt);

                return repDt;

            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtWarehouse.Text))
            {
                if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    try
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sMsg = string.Empty;
                        string sRepair = string.Empty;

                        string sStaff = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                        string sStoreId = ApplicationSettings.Terminal.StoreId;
                        string sTerminalId = ApplicationSettings.Terminal.TerminalId;
                        string sToWH = txtWarehouse.Text.Trim();


                        if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                        {
                            dtItemInfo.TableName = "ItemInfo";

                            MemoryStream mstr = new MemoryStream();
                            dtItemInfo.WriteXml(mstr, true);
                            mstr.Seek(0, SeekOrigin.Begin);
                            StreamReader sr = new StreamReader(mstr);
                            sRepair = sr.ReadToEnd();

                            if(PosApplication.Instance.TransactionServices.CheckConnection())
                            {
                                bool bStatus = false;


                                containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("RepairInHouseTransferCreate", sStaff,
                                    sStoreId, sTerminalId, sInventLocationId, sRepair);

                                bStatus = Convert.ToBoolean(containerArray[1]);

                                if(bStatus)
                                {
                                    sMsg = Convert.ToString(containerArray[2]);
                                    MessageBox.Show(sMsg);
                                }
                                else
                                {
                                    foreach(DataRow row in dtItemInfo.Rows)
                                    {
                                        UpdateRepairLine(Convert.ToString(row["RepairId"]), Convert.ToDecimal(row["LineNum"]), 0);
                                    }

                                    sMsg = Convert.ToString(containerArray[2]);
                                    MessageBox.Show(sMsg);
                                }

                                #region for Voucher
                                if(bStatus)
                                {

                                    sTransferId = Convert.ToString(containerArray[3]);
                                    if(!string.IsNullOrEmpty(sTransferId))
                                    {
                                        try
                                        {
                                            PrintVoucher();
                                        }
                                        catch { }
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Repair In House transfer failed to create");
                                }

                                #endregion

                                ClearControls();
                            }
                        }
                    }

                    catch(Exception ex)
                    {
                        MessageBox.Show("Repair In House transfer failed to create");
                        ClearControls();

                    }
                }
                else
                {
                    MessageBox.Show("Please select alleast one repair id for transfer.");
                }
            }
            else
            {
                MessageBox.Show("Please select To ware house.");
            }
        }

        private void ClearControls()
        {
            txtRepairId.Text = "";
            txtWarehouse.Text = "";
            dtItemInfo.Clear();
            dtItemInfo = null;
            dtTemp.Clear();
            dtTemp = null;
        }

        private void UpdateRepairLine(string repairId, decimal dLineNum, int iUpdateWith)
        {
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE RetailRepairDetail SET ISINHOUSETRANSFER=" + iUpdateWith + " where RepairId= '" + repairId + "' and LINENUM='" + dLineNum + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();
            if(connection.State == ConnectionState.Open)
                connection.Close();
        }

        public void PrintVoucher()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            sCompanyName = oBlank.GetCompanyName(conn);

            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("REPAIRTRANS", dtItemInfo));

            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("FWH", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("TWH", string.IsNullOrEmpty(txtWarehouse.Text) ? " " : txtWarehouse.Text, true));
            rps.Add(new ReportParameter("Title", "REPAIR TRANSFER OUT", true));
            rps.Add(new ReportParameter("RETNO", sTransferId, true));
            rps.Add(new ReportParameter("TransDate", Convert.ToString(DateTime.Now), true));

            string reportName = @"rptRepairTransferOut";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("REPAIR TRANSFER OUT", reportPath, rds, rps, null);
            rptView.ShowDialog();
        }
    }
}
