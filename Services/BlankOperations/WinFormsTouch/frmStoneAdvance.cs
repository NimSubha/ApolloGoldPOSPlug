using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using BlankOperations;


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmStoneAdvance:frmTouchBase
    {
        #region Variable Declaration
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        Random randUnique = new Random();
        frmCustomerOrder frmCustOrder;
        [Import]
        private IApplication application;
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        DataTable dtTemp = new DataTable("dtTemp");

        bool IsEdit = false;
        int EditselectedIndex = 0;
        string sUnique = string.Empty;
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        string unitid = string.Empty;
        string Previewdimensions = string.Empty;
        #endregion


        public frmStoneAdvance(IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
        }

        public frmStoneAdvance(DataTable dtOrderStnAdv, IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            dtItemInfo = dtOrderStnAdv;
            grItems.DataSource = dtItemInfo.DefaultView;
        }
        public frmStoneAdvance(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            grItems.DataSource = dsSearchedDetails.Tables[4].DefaultView;
            btnSubmit.Enabled = false;
            btnClear.Enabled = false;
            btnAddItem.Enabled = false;
            btnPOSItemSearch.Enabled = false;
            btnDelete.Enabled = false;
            btnEdit.Enabled = false;
        }
        
        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            if(IsEdit)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("You are in editing mode", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return;
                }
            }
            Microsoft.Dynamics.Retail.Pos.Dialog.Dialog objdialog = new Dialog.Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            objdialog.MyItemSearch(500, ref str, out  dsItem);

            saleLineItem = new SaleLineItem();

            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                Microsoft.Dynamics.Retail.Pos.Item.Item objItem = new Item.Item();
                objItem.MYProcessItem(saleLineItem, application);
                Microsoft.Dynamics.Retail.Pos.Dimension.Dimension objDim = new Dimension.Dimension();
                DataTable dtDimension = new DataTable();
                dtDimension = objDim.GetDimensions(saleLineItem.ItemId);
                if(dtDimension != null && dtDimension.Rows.Count > 0)
                {
                    DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                    dimConfirmation.InventDimCombination = dtDimension;
                    dimConfirmation.DimensionData = saleLineItem.Dimension;

                    frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                    objfrmDim.ShowDialog();
                    if(objfrmDim.SelectDimCombination != null)
                    {
                        inventDimId = GetInventID(Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[2]));
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
                        cmbConfig.Enabled = false;
                        cmbCode.Enabled = false;
                        cmbSize.Enabled = false;
                        cmbStyle.Enabled = false;

                        Previewdimensions = ColorSizeStyleConfig();
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
                txtPCS.Text = "";
                SqlConnection conn = new SqlConnection();
                if(application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);


                unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if(isValied())
            {
                DataRow dr;
                if(IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("DIMENSIONS", typeof(string));
                    dtItemInfo.Columns.Add("UNITID", typeof(string));
                    dtItemInfo.Columns.Add("REMARKS", typeof(string)); 

                }
                if(IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    dr["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                    dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        dr["PCS"] = DBNull.Value;
                    if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["QUANTITY"] = DBNull.Value;

                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;
                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    dr["DIMENSIONS"] = Previewdimensions;
                    dr["REMARKS"] = txtRemarks.Text.Trim();

                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                if(IsEdit == true)
                {
                    
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    EditRow["PCS"] = txtPCS.Text.Trim();
                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                    EditRow["REMARKS"] = txtRemarks.Text.Trim();

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                ClearControls();
            }
        }
       
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
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
                    
                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }
            }
            if(DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            frmCustOrder.dtOrderStnAdv = dtItemInfo;
            this.Close();
        }

        bool isValied()
        {
            if(ValidateControls())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region ValidateControls()
        bool ValidateControls()
        {

            if(txtItemId.Text.ToUpper().Trim() == "ITEM ID" || string.IsNullOrEmpty(txtItemId.Text))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(txtItemName.Text.ToUpper().Trim() == "ITEM NAME" || string.IsNullOrEmpty(txtItemName.Text))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            //if(IsCatchWtItem(txtItemId.Text.Trim()))
            //{
            //    if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
            //    {
            //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //        {
            //            txtPCS.Focus();
            //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //            return false;
            //        }
            //    }
            //}
            //else
            //{
            //    txtPCS.Enabled = false;
            //}

            if((string.IsNullOrEmpty(txtPCS.Text.Trim()))) //&& (Convert.ToDecimal(txtPCS.Text) > 1)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPCS.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
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

        private string ColorSizeStyleConfig()
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig;
            if(!string.IsNullOrEmpty(cmbCode.Text))
                colorSizeStyleConfig = new StringBuilder("Color : " + cmbCode.Text);
            else
                colorSizeStyleConfig = new StringBuilder(cmbCode.Text);

            if(!string.IsNullOrEmpty(cmbSize.Text))
            {
                if(colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Size : " + cmbSize.Text);
            }

            if(!string.IsNullOrEmpty(cmbStyle.Text))
            {
                if(colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Style : " + cmbStyle.Text);
            }

            if(!string.IsNullOrEmpty(cmbConfig.Text))
            {
                if(colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(" Configuration : " + cmbConfig.Text);
            }

            return colorSizeStyleConfig.ToString();
        }

        private bool IsCatchWtItem(string sItemId)
        {
            bool bCatchWtItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISCATCHWT INT  SET @ISCATCHWT = 0 IF EXISTS (SELECT ITEMID FROM pdscatchweightitem WHERE ITEMID = '" + sItemId + "')" +
                                 " BEGIN SET @ISCATCHWT = 1 END SELECT @ISCATCHWT";


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bCatchWtItem;
        }

        #region InventDimId
        public string GetInventID(string distinctProductVariantID)
        {
            string commandText = "select top(1)  INVENTDIMID from assortedinventdimcombination WHERE DISTINCTPRODUCTVARIANT='" + distinctProductVariantID + "'";

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            return Convert.ToString(command.ExecuteScalar());

        }
        #endregion

        #region Clear Controls
        private void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            cmbCode.Text = string.Empty;
            cmbStyle.Text = string.Empty;
            cmbSize.Text = string.Empty;
            cmbConfig.Text = string.Empty;
            txtPCS.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtRemarks.Text = string.Empty;
        }
        #endregion

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
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
                    txtRemarks.Text = Convert.ToString(theRowToSelect["REMARKS"]);
                }
            }
        }

    }
}
