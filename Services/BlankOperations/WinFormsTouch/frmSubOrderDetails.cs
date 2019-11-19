using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Item;
using Microsoft.Dynamics.Retail.Pos.Dimension;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmSubOrderDetails : frmTouchBase
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
        frmOrderDetails frmOrderDetails;
        bool IsEdit = false;
        int EditselectedIndex = 0;
        string sUnique = string.Empty;
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        string unitid = string.Empty;
        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pcs = 1,
            Tot = 2,
        }
        #endregion



        #endregion

        #region Initialization
        public frmSubOrderDetails(IPosTransaction posTransaction, IApplication Application, frmOrderDetails fOrderDetails, string UniqueID)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmOrderDetails = fOrderDetails;
            sUnique = UniqueID;
            BindRateTypeCombo();
            btnPOSItemSearch.Focus();
        }

        public frmSubOrderDetails(DataTable dtSubOrder, IPosTransaction posTransaction, IApplication Application, frmOrderDetails fOrderDetails, string UniqueID, bool isMrp = false)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmOrderDetails = fOrderDetails;
            sUnique = UniqueID;
            BindRateTypeCombo();
            btnPOSItemSearch.Focus();
            dtItemInfo = dtSubOrder;
            dtTemp = dtSubOrder.Clone();
            DataRow[] drTemp = dtSubOrder.Select("UNIQUEID='" + UniqueID + "'");
            foreach (DataRow dr in drTemp)
            {
                dr["AMOUNT"] = Convert.ToString(decimal.Round(Convert.ToDecimal(dr["AMOUNT"]), 2, MidpointRounding.AwayFromZero));

                dtTemp.ImportRow(dr);
            }

            grItems.DataSource = dtTemp;
            if (dtTemp != null && dtTemp.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                if (!isMrp)
                {
                    foreach (DataRow drTotal in dtTemp.Rows)
                    {
                        dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                    }
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));// Convert.ToString(dTotalAmount);
                    btnEdit.Enabled = true;
                    btnDelete.Enabled = true;
                }
                else
                {
                    btnEdit.Enabled = false;
                    btnDelete.Enabled = false;
                }
            }


        }

        public frmSubOrderDetails(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, frmOrderDetails fOrderDetails, string UniqueID)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmOrderDetails = fOrderDetails;
            sUnique = UniqueID;
            BindRateTypeCombo();
            btnPOSItemSearch.Focus();
            DataTable dtSearchedOrdersTemp = new DataTable();
            dtSearchedOrdersTemp = dsSearchedDetails.Tables[2].Clone();
            DataRow[] drTemp = dsSearchedDetails.Tables[2].Select("ORDERDETAILNUM='" + UniqueID + "'");
            foreach (DataRow dr in drTemp)
            {
                dtSearchedOrdersTemp.ImportRow(dr);
            }

            grItems.DataSource = dtSearchedOrdersTemp;
            if (dtSearchedOrdersTemp != null && dtSearchedOrdersTemp.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dtSearchedOrdersTemp.Rows)
                {
                    dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                }
                txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));// Convert.ToString(dTotalAmount);
                frmOrderDetails.sExtendedDetailsAmount = Convert.ToDecimal(txtTotalAmount.Text);

            }
            btnPOSItemSearch.Enabled = false;
            btnAddItem.Enabled = false;
            btnSubmit.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            btnClear.Enabled = false;
        }

        #endregion

        #region Bind Rate  Type Combo
        void BindRateTypeCombo()
        {
            cmbRateType.DataSource = Enum.GetValues(typeof(RateType));

        }
        #endregion

        private bool isMRP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }


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
                        inventDimId = frmOrderDetails.GetInventID(Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[2]));
                        DataTable dtcmbCode = new DataTable();
                        dtcmbCode.Columns.Add("CodeID", typeof(string));
                        dtcmbCode.Columns.Add("CodeValue", typeof(string));
                        DataRow drCode;
                        drCode = dtcmbCode.NewRow();
                        drCode["CodeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                        drCode["CodeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                        dtcmbCode.Rows.Add(drCode);
                        cmbCode.DataSource = dtcmbCode;
                        cmbCode.DisplayMember = "CodeValue";
                        cmbCode.ValueMember = "CodeID";

                        DataTable dtSize = new DataTable();
                        dtSize.Columns.Add("SizeID", typeof(string));
                        dtSize.Columns.Add("SizeValue", typeof(string));
                        DataRow drSize;
                        drSize = dtSize.NewRow();
                        drSize["SizeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                        drSize["SizeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                        dtSize.Rows.Add(drSize);
                        cmbSize.DataSource = dtSize;
                        cmbSize.DisplayMember = "SizeID";
                        cmbSize.ValueMember = "SizeValue";

                        DataTable dtConfig = new DataTable();
                        dtConfig.Columns.Add("ConfigID", typeof(string));
                        dtConfig.Columns.Add("ConfigValue", typeof(string));
                        DataRow drConfig;
                        drConfig = dtConfig.NewRow();
                        drConfig["ConfigID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                        drConfig["ConfigValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                        dtConfig.Rows.Add(drConfig);
                        cmbConfig.DataSource = dtConfig;
                        cmbConfig.DisplayMember = "ConfigID";
                        cmbConfig.ValueMember = "ConfigValue";

                        DataTable dtStyle = new DataTable();
                        dtStyle.Columns.Add("StyleID", typeof(string));
                        dtStyle.Columns.Add("StyleValue", typeof(string));
                        DataRow drStyle;
                        drStyle = dtStyle.NewRow();
                        drStyle["StyleID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                        drStyle["StyleValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                        dtStyle.Rows.Add(drStyle);
                        cmbStyle.DataSource = dtStyle;
                        cmbStyle.DisplayMember = "StyleID";
                        cmbStyle.ValueMember = "StyleValue";

                        DimensionControll();

                        cmbConfig.Enabled = false;
                        cmbCode.Enabled = false;
                        cmbSize.Enabled = false;
                        cmbStyle.Enabled = false;
                    }

                }

                else
                {
                    cmbStyle.Text = string.Empty;
                    cmbConfig.Text = string.Empty;
                    cmbCode.Text = string.Empty;
                    cmbSize.Text = string.Empty;
                    cmbConfig.Enabled = false;
                    cmbCode.Enabled = false;
                    cmbSize.Enabled = false;
                    cmbStyle.Enabled = false;
                }
                txtPCS.Focus();

                if (IsCatchWtItem(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    txtPCS.Enabled = true;
                    txtPCS.Focus();
                    txtPCS.Text = "1";
                }
                else
                {
                    txtPCS.Text = "";
                    txtQuantity.Focus();
                    txtPCS.Enabled = false;
                }

                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                string sQty = string.Empty;
                sQty = frmOrderDetails.GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                if (!string.IsNullOrEmpty(sQty))
                {
                    sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                    txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                }
                txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                txtRate.Text = frmOrderDetails.getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                //  if (!string.IsNullOrEmpty(txtRate.Text))
                //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");
                //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();
                unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                lblUnit.Text = unitid;

            }
        }
        #endregion

        #region Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                frmOrderDetails.dtSubOrderDetails = dtItemInfo;
                frmOrderDetails.sExtendedDetailsAmount = (string.IsNullOrEmpty(Convert.ToString(txtTotalAmount.Text))) ? 0 : Convert.ToDecimal(txtTotalAmount.Text);
            }
            this.Close();
        }
        #endregion

        #region Rate Type Selected Index Changed
        private void cmbRateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()) && txtQuantity.Text !="." )
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 2);
                txtAmount.Text = Convert.ToString(objRounding.Round(decimalAmount, 2));
            }
            if (cmbRateType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                txtAmount.Text = string.Empty;
            }
            if (cmbRateType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtPCS.Text.Trim()), 2);
                txtAmount.Text = Convert.ToString(objRounding.Round(decimalAmount, 2));
            }
            if (cmbRateType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                txtAmount.Text = string.Empty;
            }
            if (cmbRateType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2);
                txtAmount.Text = Convert.ToString(objRounding.Round(decimalAmount, 2));
            }
            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtAmount.Text = string.Empty;
            }
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

            if (IsCatchWtItem(Convert.ToString(txtItemId.Text.Trim())))
            {
                if (string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtPCS.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
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

            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Rate can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && Convert.ToDecimal(txtRate.Text.Trim()) == 0m)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Zero Rate is not allowed.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(txtAmount.Text.Trim()) && Convert.ToDecimal(txtAmount.Text.Trim()) == 0m && !saleLineItem.ZeroPriceValid)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be Zero", MessageBoxButtons.OK, MessageBoxIcon.Information))
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
        #endregion


        private bool IsCatchWtItem(string sItemId)
        {
            bool bCatchWtItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISCATCHWT INT  SET @ISCATCHWT = 0 IF EXISTS (SELECT ITEMID FROM pdscatchweightitem WHERE ITEMID = '" + sItemId + "')" +
                                 " BEGIN SET @ISCATCHWT = 1 END SELECT @ISCATCHWT";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bCatchWtItem;
        }

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
                    cmbStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                    cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                    cmbRateType.SelectedIndex = cmbRateType.FindString(Convert.ToString(theRowToSelect["RATETYPE"]).Trim());
                    txtAmount.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                    lblUnit.Text = Convert.ToString(theRowToSelect["UNITID"]);

                    DimensionControll();
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
                        cmbStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                        cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                        cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                        cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                        txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                        txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                        txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                        txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                        txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                        cmbRateType.SelectedIndex = cmbRateType.FindString(Convert.ToString(theRowToSelect["RATETYPE"]).Trim());
                        txtAmount.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                        lblUnit.Text = Convert.ToString(theRowToSelect["UNITID"]);
                        DimensionControll();
                    }
                }
            }
        }

        private void DimensionControll()
        {
            if (!string.IsNullOrEmpty(cmbCode.Text))
                btnCodeS.Enabled = true;
            else
                btnCodeS.Enabled = false;

            if (!string.IsNullOrEmpty(cmbConfig.Text))
                btnConfig.Enabled = true;
            else
                btnConfig.Enabled = false;

            if (!string.IsNullOrEmpty(cmbSize.Text))
                btnSizeS.Enabled = true;
            else
                btnSizeS.Enabled = false;

            if (!string.IsNullOrEmpty(cmbStyle.Text))
                btnStyleS.Enabled = true;
            else
                btnStyleS.Enabled = false;
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
                        txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }
                }
            }


            if (DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                txtTotalAmount.Text = "0.00";
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
            txtPCS.Text = string.Empty;
            txtRate.Text = string.Empty;
            cmbRateType.SelectedIndex = 0;
            txtQuantity.Text = string.Empty;
            txtAmount.Text = string.Empty;
            cmbCode.Text = string.Empty;
            cmbSize.Text = string.Empty;
            cmbConfig.Text = string.Empty;
            cmbStyle.Text = string.Empty;
            lblUnit.Text = "--";
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
        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPCS.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
            {
                if (!string.IsNullOrEmpty(PreviousPcs))
                {
                    if (Convert.ToDecimal(PreviousPcs) > 0)
                    {
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtQuantity.Text) / Convert.ToDecimal(PreviousPcs));
                    }
                }


                txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(txtQuantity.Text));
            }
            if (string.IsNullOrEmpty(txtPCS.Text))
                txtQuantity.Text = string.Empty;

            if (string.IsNullOrEmpty(txtPCS.Text.Trim()) && (cmbRateType.SelectedIndex == 0 || cmbRateType.SelectedIndex == 1))
                txtAmount.Text = string.Empty;

            cmbRateType_SelectedIndexChanged(sender, e);
            PreviousPcs = txtPCS.Text;
        }
        #endregion

        #region Quantity Text Changed
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) && (cmbRateType.SelectedIndex == 0 || cmbRateType.SelectedIndex == 1))
            {
                txtAmount.Text = string.Empty;
            }
            txtRate.Text = frmOrderDetails.getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
            cmbRateType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            if (ItemValidate())
            {
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(double));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));

                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));

                    dtItemInfo.Columns.Add("EXPECTEDQTY", typeof(decimal));
                    dtItemInfo.Columns.Add("RATE", typeof(decimal));
                    dtItemInfo.Columns.Add("RATETYPE", typeof(string));
                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("UNITID", typeof(string));
                    dtItemInfo.Columns.Add("METALTYPE", typeof(int)); //ADDED ON 17/02/2015
                    dtTemp = dtItemInfo.Clone();
                }

                if (IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUnique.Trim();
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    dr["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                    dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        dr["PCS"] = 0;

                    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dr["QUANTITY"] = Convert.ToDecimal(txtQuantity.Text.Trim());
                    else
                        dr["QUANTITY"] = 0;



                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["RATE"] = Convert.ToDecimal(txtRate.Text.Trim());
                    else
                        dr["RATE"] = 0;
                    dr["RATETYPE"] = Convert.ToString(cmbRateType.Text.Trim());


                    if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                        dr["AMOUNT"] = Convert.ToDecimal(txtAmount.Text.Trim());
                    else
                        dr["AMOUNT"] = 0;

                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;

                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    dr["METALTYPE"] = GetMetalType(Convert.ToString(txtItemId.Text.Trim()));//ADDED ON 17/02/2015

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
                            txtTotalAmount.Text = Convert.ToString(dTotalAmount);
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
                            txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                        }
                    }
                }

                if (IsEdit == true)
                {
                    string unique = string.Empty;
                    if (dtTemp != null && dtTemp.Rows.Count > 0)
                    {
                        DataRow EditTempRow = dtTemp.Rows[EditselectedIndex];
                        EditTempRow["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                        EditTempRow["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                        EditTempRow["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                        EditTempRow["SIZE"] = Convert.ToString(cmbSize.Text.Trim());

                        EditTempRow["PCS"] = txtPCS.Text.Trim();

                        EditTempRow["QUANTITY"] = txtQuantity.Text.Trim();
                        EditTempRow["RATETYPE"] = cmbRateType.Text.Trim();
                        EditTempRow["RATE"] = txtRate.Text.Trim();
                        EditTempRow["AMOUNT"] = txtAmount.Text.Trim();
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
                            txtTotalAmount.Text = Convert.ToString(dTotalAmount);
                        }
                    }
                    else
                    {
                        DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                        EditRow["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                        EditRow["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                        EditRow["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                        EditRow["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                        EditRow["PCS"] = txtPCS.Text.Trim();

                        EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                        EditRow["RATETYPE"] = cmbRateType.Text.Trim();
                        EditRow["RATE"] = txtRate.Text.Trim();
                        EditRow["AMOUNT"] = txtAmount.Text.Trim();
                        dtItemInfo.AcceptChanges();
                        if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                        {
                            Decimal dTotalAmount = 0m;
                            foreach (DataRow drTotal in dtItemInfo.Rows)
                            {
                                dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                            }
                            txtTotalAmount.Text = Convert.ToString(dTotalAmount);
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
        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) && (cmbRateType.SelectedIndex == 0 || cmbRateType.SelectedIndex == 1))
            {
                txtAmount.Text = string.Empty;
            }

            cmbRateType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region Submit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                frmOrderDetails.dtSubOrderDetails = dtItemInfo;
                frmOrderDetails.sExtendedDetailsAmount = (string.IsNullOrEmpty(Convert.ToString(txtTotalAmount.Text))) ? 0 : Convert.ToDecimal(txtTotalAmount.Text);
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

        protected int GetMetalType(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return iMetalType;

        }

        private void btnSizeS_Click(object sender, EventArgs e)
        {
            string sSQl = " select isnull(Name,'') Size from ECORESSIZE ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Size Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbSize.Text = string.Empty;
                cmbSize.Text = Convert.ToString(drRes["Size"]);
            }
        }

        private void btnStyleS_Click(object sender, EventArgs e)
        {
            string sSQl = " select isnull(Name,'') Style from ECORESSTYLE ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Style Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbStyle.Text = string.Empty;
                cmbStyle.Text = Convert.ToString(drRes["Style"]);
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

        private void btnConfig_Click(object sender, EventArgs e)
        {
            string sSQl = " select isnull(Name,'') Configuration from ECORESCONFIGURATION ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Configuration Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbConfig.Text = string.Empty;
                cmbConfig.Text = Convert.ToString(drRes["Configuration"]);
            }
        }

        private void btnCodeS_Click(object sender, EventArgs e)
        {
            string sSQl = " select isnull(Name,'') Code from ECORESCOLOR ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Code Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbCode.Text = string.Empty;
                cmbCode.Text = Convert.ToString(drRes["Code"]);
            }
        }

    }
}
