

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Dynamics.Retail.Pos.Dimension;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using Microsoft.Dynamics.Retail.Pos.Item;
using Microsoft.Dynamics.Retail.Pos.RoundingService;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmRepairSubDetail : frmTouchBase
    {
        #region Variable Declaration
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        DataTable dtTemp = new DataTable("dtTemp");
        frmRepairDetail frmRepairDetail;
        bool IsEdit = false;
        int EditselectedIndex = 0;
        string sUnique = string.Empty;
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        string unitid = string.Empty;

        #endregion

        #region Initialization
        public frmRepairSubDetail(IPosTransaction posTransaction, IApplication Application, frmRepairDetail fOrderDetails, string UniqueID)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmRepairDetail = fOrderDetails;
            sUnique = UniqueID;
            //  BindRateTypeCombo();
            btnPOSItemSearch.Focus();
        }

        public frmRepairSubDetail(DataTable dtSubOrder, IPosTransaction posTransaction, IApplication Application, frmRepairDetail fOrderDetails, string UniqueID)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmRepairDetail = fOrderDetails;
            sUnique = UniqueID;
            //  BindRateTypeCombo();
            btnPOSItemSearch.Focus();
            dtItemInfo = dtSubOrder;
            dtTemp = dtSubOrder.Clone();
            DataRow[] drTemp = dtSubOrder.Select("UNIQUEID='" + UniqueID + "'");
            foreach (DataRow dr in drTemp)
            {
                dtTemp.ImportRow(dr);
            }

            grItems.DataSource = dtTemp;
            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dtTemp.Rows)
                {
                    dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                }
            }

        }

        public frmRepairSubDetail(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, frmRepairDetail fOrderDetails, string UniqueID)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmRepairDetail = fOrderDetails;
            sUnique = UniqueID;
            // BindRateTypeCombo();
            btnPOSItemSearch.Focus();
            DataTable dtSearchedOrdersTemp = new DataTable();
            dtSearchedOrdersTemp = dsSearchedDetails.Tables[2].Clone();
            
            DataRow[] drTemp = dsSearchedDetails.Tables[2].Select("ORDERDETAILNUM='" + UniqueID + "'");
            foreach (DataRow dr in drTemp)
            {
                dtSearchedOrdersTemp.ImportRow(dr);
            }
            dtSearchedOrdersTemp.Columns["CONFIGID"].ColumnName = "CONFIGURATION";
            
            grItems.DataSource = dtSearchedOrdersTemp;
            if (dtSearchedOrdersTemp != null && dtSearchedOrdersTemp.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dtSearchedOrdersTemp.Rows)
                {
                    dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                }
                //  txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                frmRepairDetail.sExtendedDetailsAmount = 0m;// Convert.ToDecimal(txtTotalAmount.Text);

            }
            btnPOSItemSearch.Enabled = false;
            btnAddItem.Enabled = false;
            btnSubmit.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnClear.Enabled = false;
        }

        #endregion


        #region Item Search Click
        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("You are in editing mode", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return;
                }
            }
            Dialog objdialog = new Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            objdialog.MyItemSearch(500, ref str, out  dsItem);

            saleLineItem = new SaleLineItem();

            if (dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                Item objItem = new Item();
                objItem.MYProcessItem(saleLineItem, application);
                Dimension objDim = new Dimension();
                DataTable dtDimension = new DataTable();
                dtDimension = objDim.GetDimensions(saleLineItem.ItemId);
                if (dtDimension != null && dtDimension.Rows.Count > 0)
                {
                    DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                    dimConfirmation.InventDimCombination = dtDimension;
                    dimConfirmation.DimensionData = saleLineItem.Dimension;

                    frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                    objfrmDim.ShowDialog();
                    if (objfrmDim.SelectDimCombination != null)
                    {
                        inventDimId = frmRepairDetail.GetInventID(Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[2]));
                        #region The following field(code,style,size and configuration are added to this form) @ 01/11/2014
                        txtCode.Text = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                        txtStyle.Text = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                        txtSize.Text = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                        txtConfig.Text = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                        #endregion
                    }

                }

                txtPCS.Focus();
                txtPCS.Text = "1";
                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                string sQty = string.Empty;
                sQty = frmRepairDetail.GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                if (!string.IsNullOrEmpty(sQty))
                {
                    sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                    txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                }
                txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                
                txtRate.Text = "0.00";// frmRepairDetail.getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                //  if (!string.IsNullOrEmpty(txtRate.Text))
                //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");
                //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();
                unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

            }
        }
        #endregion

        #region Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                frmRepairDetail.dtSubOrderDetails = dtItemInfo;
                frmRepairDetail.sExtendedDetailsAmount = 0m;// (string.IsNullOrEmpty(Convert.ToString(txtTotalAmount.Text))) ? 0 : Convert.ToDecimal(txtTotalAmount.Text);
            }
            this.Close();
        }
        #endregion

        #region PCS Key Pressed
        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }


        }
        #endregion

        #region ItemValidate()
        bool ItemValidate()
        {

            if (string.IsNullOrEmpty(txtItemId.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtItemName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPCS.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (Convert.ToDecimal(txtQuantity.Text.Trim()) == 0m)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && Convert.ToDecimal(txtRate.Text.Trim()) == 0m)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be Zero.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
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

        #region Edit Click
        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;

            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtTemp.Rows[EditselectedIndex];

                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                    #region The following field(code,style,size and configuration are added to this form to Edit) @ 01/11/2014
                    txtConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                    txtCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    txtStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                    txtSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    #endregion

                }
            }
            else
            {
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    if (grdView.RowCount > 0)
                    {
                        IsEdit = true;
                        EditselectedIndex = grdView.GetSelectedRows()[0];
                        DataRow theRowToSelect = dtItemInfo.Rows[EditselectedIndex];

                        txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                        txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                        txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                        txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                        txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                        #region The following field(code,style,size and configuration are added to this form to Edit) @ 01/11/2014
                        txtConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                        txtCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                        txtStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                        txtSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                        #endregion

                    }
                }
            }
        }
        #endregion

        #region DELETE CLICK
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            DataRow theRowToDelete = null;
            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    theRowToDelete = dtTemp.Rows[DeleteSelectedIndex];
                    string unique = Convert.ToString(theRowToDelete["UNIQUEID"]);
                    dtTemp.Rows.Remove(theRowToDelete);

                    foreach (DataRow dr in dtItemInfo.Select("UNIQUEID='" + unique.Trim() + "'"))
                    {
                        dtItemInfo.Rows.Remove(dr);
                    }
                    foreach (DataRow dr in dtTemp.Select("UNIQUEID='" + unique.Trim() + "'"))
                    {
                        dtItemInfo.ImportRow(dr);
                    }

                    grItems.DataSource = dtTemp.DefaultView;
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach (DataRow drTotal in dtTemp.Rows)
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                        }
                        //  txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }
                }
            }


            if (DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                // txtTotalAmount.Text = "0.00";
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;
            ClearControls();
        }
        #endregion

        #region ClearControls()
        void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            #region The following field(code,style,size and configuration are added to this form to Clear) @ 01/11/2014
            txtCode.Text = string.Empty;
            txtConfig.Text = string.Empty;
            txtSize.Text = string.Empty;
            txtStyle.Text = string.Empty;
            #endregion
            txtPCS.Text = string.Empty;
            txtRate.Text = string.Empty;
            txtQuantity.Text = string.Empty;

        }
        #endregion

        #region CLEAR CLICK
        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
            grItems.DataSource = null;
            dtItemInfo.Clear();
        }
        #endregion

        #region TEXT PCS TEXT CHANGED
        //private void txtPCS_TextChanged(object sender, EventArgs e)
        //{
        //    if (!string.IsNullOrEmpty(txtPCS.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
        //    {
        //        if (!string.IsNullOrEmpty(PreviousPcs))
        //            txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtQuantity.Text) / Convert.ToDecimal(PreviousPcs));
        //        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(txtQuantity.Text));
        //    }
        //    if (string.IsNullOrEmpty(txtPCS.Text))
        //        txtQuantity.Text = string.Empty;

        //    //if (string.IsNullOrEmpty(txtPCS.Text.Trim()) && (cmbRateType.SelectedIndex == 0 || cmbRateType.SelectedIndex == 1))
        //    //    txtAmount.Text = string.Empty;

        //   // cmbRateType_SelectedIndexChanged(sender, e);
        //    PreviousPcs = txtPCS.Text;
        //}
        #endregion


        #region Quantity Text Changed
        /*
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) && (cmbRateType.SelectedIndex == 0 || cmbRateType.SelectedIndex == 1))
            {
                txtAmount.Text = string.Empty;
            }
            txtRate.Text = "0.00";// frmRepairDetail.getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
           // cmbRateType_SelectedIndexChanged(sender, e);
        }
        
        */
        #endregion

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            // cmbRateType_SelectedIndexChanged(sender, e);
            if (ItemValidate())
            {
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(double));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    #region The following field(code,style,size and configuration are added to this form to Add columns to dtItemInfo) @ 01/11/2014
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));//Added by P.J @01/11/2014
                    dtItemInfo.Columns.Add("COLOR", typeof(string)); //Added by P.J @01/11/2014
                    dtItemInfo.Columns.Add("SIZE", typeof(string)); //Added by P.J @01/11/2014
                    dtItemInfo.Columns.Add("STYLE", typeof(string)); //Added by P.J @01/11/2014
                    #endregion
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));

                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));

                    dtItemInfo.Columns.Add("EXPECTEDQTY", typeof(decimal)); // na
                    dtItemInfo.Columns.Add("RATE", typeof(decimal));
                    //  dtItemInfo.Columns.Add("RATETYPE", typeof(string)); // na
                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("UNITID", typeof(string));
                    dtTemp = dtItemInfo.Clone();
                }

                if (IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUnique.Trim();
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    #region The following field(code,style,size and configuration are added to dr) @ 01/11/2014
                    dr["CONFIGURATION"] =  Convert.ToString(txtConfig.Text.Trim());//Added by P.J @01/11/2014
                    dr["COLOR"] = Convert.ToString(txtCode.Text.Trim());//Added by P.J @01/11/2014
                    dr["STYLE"] = Convert.ToString(txtStyle.Text.Trim());//Added by P.J @01/11/2014
                    dr["SIZE"] = Convert.ToString(txtSize.Text.Trim());//Added by P.J @01/11/2014
                    #endregion
                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        dr["PCS"] = DBNull.Value;

                    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dr["QUANTITY"] = Convert.ToDecimal(txtQuantity.Text.Trim());
                    else
                        dr["QUANTITY"] = DBNull.Value;

                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["RATE"] = Convert.ToDecimal(txtRate.Text.Trim());
                    else
                        dr["RATE"] = 0m;
                    //  dr["RATETYPE"] = "";// Convert.ToString(cmbRateType.Text.Trim());


                    //  //if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                    //  //    dr["AMOUNT"] = Convert.ToDecimal(txtAmount.Text.Trim());
                    ////  else
                    //  dr["AMOUNT"] = 0m;// DBNull.Value;

                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["AMOUNT"] = Convert.ToDecimal(txtRate.Text.Trim());
                    else
                        dr["AMOUNT"] = 0m;// DBNull.Value;


                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;

                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    dtItemInfo.Rows.Add(dr);
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        dtTemp.ImportRow(dr);
                        grItems.DataSource = dtTemp.DefaultView;
                        if (dtTemp != null && dtTemp.Rows.Count > 0)
                        {
                            Decimal dTotalAmount = 0m;
                            foreach (DataRow drTotal in dtTemp.Rows)
                            {
                                dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                            }
                            // txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                        }
                    }
                    else
                    {
                        dtTemp.ImportRow(dr);
                        grItems.DataSource = dtTemp.DefaultView;

                        if (dtTemp != null && dtTemp.Rows.Count > 0)
                        {
                            Decimal dTotalAmount = 0m;
                            foreach (DataRow drTotal in dtTemp.Rows)
                            {
                                dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                            }
                            // txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                        }
                    }
                }

                if (IsEdit == true)
                {
                    string unique = string.Empty;
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        DataRow EditTempRow = dtTemp.Rows[EditselectedIndex];
                        EditTempRow["PCS"] = txtPCS.Text.Trim();

                        EditTempRow["QUANTITY"] = txtQuantity.Text.Trim();
                        //  EditTempRow["RATETYPE"] = "";// cmbRateType.Text.Trim();
                        EditTempRow["RATE"] = txtRate.Text.Trim();
                        //  EditTempRow["AMOUNT"] = "0";// txtAmount.Text.Trim();
                        EditTempRow["AMOUNT"] = txtRate.Text.Trim(); // txtAmount.Text.Trim();
                        #region The following field(code,style,size and configuration are added to Edit) @ 01/11/2014
                        EditTempRow["CONFIGURATION"] = Convert.ToString(txtConfig.Text.Trim());//Added by P.J @01/11/2014
                        EditTempRow["COLOR"] = Convert.ToString(txtCode.Text.Trim());//Added by P.J @01/11/2014
                        EditTempRow["STYLE"] = Convert.ToString(txtStyle.Text.Trim());//Added by P.J @01/11/2014
                        EditTempRow["SIZE"] = Convert.ToString(txtSize.Text.Trim());//Added by P.J @01/11/2014
                        #endregion
                        unique = Convert.ToString(EditTempRow["UNIQUEID"]);
                        dtTemp.AcceptChanges();
                        grItems.DataSource = dtTemp.DefaultView;
                        IsEdit = false;
                        foreach (DataRow drNew in dtItemInfo.Select("UNIQUEID='" + unique.Trim() + "'"))
                        {
                            dtItemInfo.Rows.Remove(drNew);
                        }
                        foreach (DataRow drNew in dtTemp.Select("UNIQUEID='" + unique.Trim() + "'"))
                        {
                            dtItemInfo.ImportRow(drNew);
                        }
                        if (dtTemp != null && dtTemp.Rows.Count > 0)
                        {
                            Decimal dTotalAmount = 0m;
                            foreach (DataRow drTotal in dtTemp.Rows)
                            {
                                dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                            }
                            // txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                        }
                    }
                    else
                    {
                        DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                        EditRow["PCS"] = txtPCS.Text.Trim();

                        EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                        //    EditRow["RATETYPE"] = "";// cmbRateType.Text.Trim();
                        EditRow["RATE"] = txtRate.Text.Trim();
                        EditRow["AMOUNT"] = txtRate.Text.Trim();//"0";// txtAmount.Text.Trim();
                        dtItemInfo.AcceptChanges();
                        if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                        {
                            Decimal dTotalAmount = 0m;
                            foreach (DataRow drTotal in dtItemInfo.Rows)
                            {
                                dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                            }
                            // txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                        }
                        grItems.DataSource = dtItemInfo.DefaultView;
                        IsEdit = false;
                    }
                }
                ClearControls();
            }
        }
        #endregion

        #region Rate Text Changed
        /*
        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) && (cmbRateType.SelectedIndex == 0 || cmbRateType.SelectedIndex == 1))
            {
                txtAmount.Text = string.Empty;
            }

            cmbRateType_SelectedIndexChanged(sender, e);
        }
        
        */
        #endregion

        #region Submit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                frmRepairDetail.dtSubOrderDetails = dtItemInfo;
                frmRepairDetail.sExtendedDetailsAmount = 0m;// (string.IsNullOrEmpty(Convert.ToString(txtTotalAmount.Text))) ? 0 : Convert.ToDecimal(txtTotalAmount.Text);
                this.Close();
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select at least one item to submit.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }

        }
        #endregion

        #region Key Press Events
        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
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

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
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
        #endregion

        #region  NumPad()
        void NumPad()
        {
            if (obj != null)
            {
                obj.Dispose();
            }
            obj = new LSRetailPosis.POSProcesses.WinControls.NumPad();


            // 
            // numPad1
            // 

            obj.Anchor = System.Windows.Forms.AnchorStyles.None;
            obj.Appearance.BackColor = System.Drawing.Color.White;
            obj.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            obj.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            obj.Appearance.Options.UseBackColor = true;
            obj.Appearance.Options.UseFont = true;
            obj.Appearance.Options.UseForeColor = true;
            obj.AutoSize = true;
            obj.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            obj.EnteredValue = "";
            obj.EntryStartsInDecimals = false;
            obj.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.None;
            obj.Location = new System.Drawing.Point(598, 12);
            obj.LookAndFeel.SkinName = "The Asphalt World";
            obj.LookAndFeel.UseDefaultLookAndFeel = false;
            obj.MaskChar = null;
            obj.MaskInterval = 0;
            obj.MaxNumberOfDigits = 13;
            obj.MinimumSize = new System.Drawing.Size(180, 230);
            obj.Name = "numPad1";
            obj.NegativeMode = false;
            obj.NoOfTries = 0;
            obj.NumberOfDecimals = 3;
            obj.PromptText = null;
            obj.ShortcutKeysActive = false;
            obj.Size = new System.Drawing.Size(180, 230);
            obj.TabIndex = 14;
            obj.TimerEnabled = false;
            obj.Visible = true;

            numPad1.Visible = false;
            panelControl1.Controls.Add(obj);
            obj.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(obj_EnterButtonPressed);
        }
        #endregion

        #region numPad1_EnterButtonPressed
        private void numPad1_EnterButtonPressed_1()
        {
            //txtPCS.Text = numPad1.EnteredValue;

            //if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
            //{
            //    NumPad();
            //}
            if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                txtPCS.Text = numPad1.EnteredValue;

                numPad1.Refresh();
            }
            else if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                txtQuantity.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }
            else if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtRate.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }
        }
        #endregion

        #region obj_EnterButtonPressed
        private void obj_EnterButtonPressed()
        {
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                txtQuantity.Text = obj.EnteredValue;
                // NumPad();
                numPad1.Refresh();
            }
            else if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtRate.Text = obj.EnteredValue;
                //NumPad();
                numPad1.Refresh();
            }
        }
        #endregion

        

        


    }
}
