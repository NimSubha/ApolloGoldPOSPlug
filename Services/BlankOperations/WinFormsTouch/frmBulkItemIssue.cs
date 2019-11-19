using System;
using System.Data;
using System.Windows.Forms;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using System.Text;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmBulkItemIssue : frmTouchBase
    {
        #region Variable Declaration
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;
        public DataTable dtItemInfo = new DataTable("dtItemInfo");
        DataTable dtTemp = new DataTable("dtTemp");

        bool IsEdit = false;
        int EditselectedIndex = 0;
        string sUnique = string.Empty;
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        string unitid = string.Empty;
        string metalType = "";
        Random randUnique = new Random();
        string sBaseItemUnit = string.Empty;
        int itransType = 0;
        string sSelectedItemConfigId = "";
        int iDecPre = 0;
        #endregion

        enum DisplayMetalType
        {
            None = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Stone = 7,
            LooseDmd = 12,
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
        }

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

        enum TransactionType
        {
            BulkItemIssue = 13
        }

        public frmBulkItemIssue()
        {
            InitializeComponent();
        }

        public frmBulkItemIssue(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;

            cmbMetalType.DataSource = Enum.GetValues(typeof(DisplayMetalType));
        }

        #region Event
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            //if(iDecPre == 0)
            //{
            //    if(e.KeyChar == '.')
            //    {
            //        e.Handled = true;
            //    }
            //}
        }

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
            if (iDecPre == 0)
            {
                if (e.KeyChar == '.')
                {
                    e.Handled = true;
                }
            }
        }

        private void txtItemId_Leave(object sender, EventArgs e)
        {
            bool isValidItem = false;

            //if (IsEdit)
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("You are in editing mode", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return;
            //    }
            //}
            if (!string.IsNullOrEmpty(txtItemId.Text))
            {
                isValidItem = ValidateItem(txtItemId.Text);


                if (isValidItem)
                {
                    string sItemName = NIM_ReturnExecuteScalar("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + txtItemId.Text + "'");

                    if (IsCatchWtItem(txtItemId.Text))
                    {
                        txtPCS.Text = "";
                        txtPCS.Enabled = true;
                    }
                    else
                    {
                        txtPCS.Text = "";
                        txtPCS.Enabled = false;
                    }

                    SqlConnection conn = new SqlConnection();
                    if (application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;

                    string sQty = string.Empty;
                    sQty = GetStandardQuantityFromDB(conn, Convert.ToString(txtItemId.Text));
                    if (!string.IsNullOrEmpty(sQty))
                    {
                        sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    }
                    txtItemId.Text = Convert.ToString(txtItemId.Text);
                    txtItemName.Text = Convert.ToString(sItemName);
                    unitid = NIM_ReturnExecuteScalar("select UNITID from INVENTTABLEMODULE WHERE ITEMID='" + txtItemId.Text + "'");
                    lblUnit.Text = unitid;

                    iDecPre = GetDecimalPrecison(unitid);

                    metalType = NIM_ReturnExecuteScalar("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + txtItemId.Text + "'");

                    //if(metalType == Convert.ToString((int)MetalType.Gold) || metalType == Convert.ToString((int)MetalType.Silver)
                    //    || metalType == Convert.ToString((int)MetalType.Platinum) || metalType == Convert.ToString((int)MetalType.Palladium))
                    //{
                    //    cmbConfig.Text = sSelectedItemConfigId;
                    //    cmbConfig.Enabled = false;
                    //}
                    //else
                    //{
                    //    cmbConfig.Text = "";
                    //    cmbConfig.Enabled = true;
                    //}

                    cmbConfig.Focus();
                }
                else
                {
                    MessageBox.Show("Invalid item.");
                    txtItemId.Text = "";
                }
            }
        }

        private int GetDecimalPrecison(string sUnit)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT top 1 DECIMALPRECISION FROM UNITOFMEASURE WHERE SYMBOL='" + sUnit + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }

        private void cmbSize_Leave(object sender, EventArgs e)
        {
            string strSql = "";
            string sSize = string.Empty;

            strSql = "select INVENTSIZEID" +
                   " FROM ASSORTEDINVENTDIMCOMBINATION IDC" +
                   " INNER JOIN ASSORTEDINVENTITEMS AIT ON AIT.ITEMID = IDC.ITEMID" +
                   " INNER JOIN INVENTDIM I ON I.INVENTDIMID = IDC.INVENTDIMID AND I.DATAAREAID = IDC.DATAAREAID" +
                   " LEFT OUTER JOIN ECORESCOLOR ON ECORESCOLOR.NAME = I.INVENTCOLORID" +
                   " LEFT OUTER JOIN ECORESPRODUCTMASTERCOLOR ON (ECORESPRODUCTMASTERCOLOR.COLOR = ECORESCOLOR.RECID)" +
                   " AND (ECORESPRODUCTMASTERCOLOR.COLORPRODUCTMASTER = AIT.PRODUCT)" +
                   " LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVC ON DVC.RECID = ECORESPRODUCTMASTERCOLOR.RECID" +
                   " where AIT.ITEMID='" + txtItemId.Text + "' and INVENTSIZEID='" + cmbSize.Text + "'";

            sSize = NIM_ReturnExecuteScalar(strSql);

            if (!string.IsNullOrEmpty(cmbSize.Text))
            {
                if (sSize.ToUpper() != cmbSize.Text.Trim().ToUpper())
                {
                    MessageBox.Show("Invalid size.");
                    cmbSize.Focus();
                }
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            inventDimId = GetInventDimId();
            if (ItemValidate())
            {
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("BATCH", typeof(string));
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));
                    dtItemInfo.Columns.Add("GrossWt", typeof(decimal));
                    dtItemInfo.Columns.Add("NetWt", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("UNITID", typeof(string));
                    dtItemInfo.Columns.Add("METALTYPE", typeof(string));
                    dtItemInfo.Columns.Add("ArticleCode", typeof(string));
                    dtItemInfo.Columns.Add("ProductTypeCode", typeof(string));
                    dtItemInfo.Columns.Add("ArticleDescription", typeof(string));
                    dtItemInfo.Columns.Add("RETAIL", typeof(int));


                    //DataColumn[] columns = new DataColumn[6];
                    //columns[0] = dtItemInfo.Columns["ITEMID"];
                    //columns[1] = dtItemInfo.Columns["CONFIGURATION"];
                    //columns[2] = dtItemInfo.Columns["COLOR"];
                    //columns[3] = dtItemInfo.Columns["SIZE"];
                    //columns[4] = dtItemInfo.Columns["STYLE"];
                    //columns[5] = dtItemInfo.Columns["BATCH"];

                    //dtItemInfo.PrimaryKey = columns;
                }

                if (IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUnique = Convert.ToString(randUnique.Next());
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    dr["STYLE"] = "";
                    dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    dr["BATCH"] = Convert.ToString(txtBatch.Text.Trim());
                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        dr["PCS"] = 0m;

                    string sArtCode = string.Empty;
                    string sProdType = string.Empty;
                    string sArtDesc = string.Empty;
                    int iRetail = 0;

                    GetItemDetails(ref sArtCode, ref sProdType, ref sArtDesc, ref iRetail, txtItemId.Text);

                    if (IsCatchWtItem(txtItemId.Text))
                    {
                        if (!string.IsNullOrEmpty(txtGrossWt.Text.Trim())) //txtQuantity
                            dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            dr["QUANTITY"] = DBNull.Value;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtQuantity.Text.Trim())) //txtQuantity
                            dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            dr["QUANTITY"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                        dr["GrossWt"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["GrossWt"] = DBNull.Value;
                    if (!string.IsNullOrEmpty(txtNetWt.Text.Trim()))
                        dr["NetWt"] = decimal.Round(Convert.ToDecimal(txtNetWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["NetWt"] = DBNull.Value;

                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;

                    dr["UNITID"] = string.IsNullOrEmpty(lblUnit.Text) ? string.Empty : lblUnit.Text;
                    dr["METALTYPE"] = string.IsNullOrEmpty(metalType) ? string.Empty : metalType;

                    dr["ArticleCode"] = string.IsNullOrEmpty(sArtCode) ? string.Empty : sArtCode;
                    dr["ProductTypeCode"] = string.IsNullOrEmpty(sProdType) ? string.Empty : sProdType;
                    dr["ArticleDescription"] = string.IsNullOrEmpty(sArtDesc) ? string.Empty : sArtDesc;
                    dr["RETAIL"] = iRetail;

                    dtItemInfo.Rows.Add(dr);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }

                if (IsEdit == true)
                {
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        EditRow["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        EditRow["PCS"] = 0m;

                    if (IsCatchWtItem(txtItemId.Text))
                    {
                        if (!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))//txtQuantity
                            EditRow["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            EditRow["QUANTITY"] = DBNull.Value;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))//txtQuantity
                            EditRow["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            EditRow["QUANTITY"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                        EditRow["GrossWt"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["GrossWt"] = DBNull.Value;

                    if (!string.IsNullOrEmpty(txtNetWt.Text.Trim()))
                        EditRow["NetWt"] = decimal.Round(Convert.ToDecimal(txtNetWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["NetWt"] = DBNull.Value;

                    EditRow["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    EditRow["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    EditRow["STYLE"] = "";
                    EditRow["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    EditRow["BATCH"] = Convert.ToString(txtBatch.Text.Trim());

                    EditRow["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;

                    EditRow["UNITID"] = string.IsNullOrEmpty(lblUnit.Text) ? string.Empty : lblUnit.Text;
                    EditRow["METALTYPE"] = string.IsNullOrEmpty(metalType) ? string.Empty : metalType;

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                    IsEdit = false;

                }
                ClearControls();
            }

            CalTotal();
        }

        private void cmbCode_Leave(object sender, EventArgs e)
        {
            string strSql = "";
            string sColor = string.Empty;

            strSql = "select INVENTCOLORID" +
                   " FROM ASSORTEDINVENTDIMCOMBINATION IDC" +
                   " INNER JOIN ASSORTEDINVENTITEMS AIT ON AIT.ITEMID = IDC.ITEMID" +
                   " INNER JOIN INVENTDIM I ON I.INVENTDIMID = IDC.INVENTDIMID AND I.DATAAREAID = IDC.DATAAREAID" +
                   " LEFT OUTER JOIN ECORESCOLOR ON ECORESCOLOR.NAME = I.INVENTCOLORID" +
                   " LEFT OUTER JOIN ECORESPRODUCTMASTERCOLOR ON (ECORESPRODUCTMASTERCOLOR.COLOR = ECORESCOLOR.RECID)" +
                   " AND (ECORESPRODUCTMASTERCOLOR.COLORPRODUCTMASTER = AIT.PRODUCT)" +
                   " LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVC ON DVC.RECID = ECORESPRODUCTMASTERCOLOR.RECID" +
                   " where AIT.ITEMID='" + txtItemId.Text + "' and INVENTCOLORID='" + cmbCode.Text + "'";


            sColor = NIM_ReturnExecuteScalar(strSql);

            if (!string.IsNullOrEmpty(cmbCode.Text))
            {
                if (sColor.ToUpper() != cmbCode.Text.Trim().ToUpper())
                {
                    MessageBox.Show("Invalid code.");
                    cmbCode.Focus();
                }
                else
                    cmbSize.Focus();
            }
        }

        private void cmbConfig_Leave(object sender, EventArgs e)
        {
            string strSql = "";
            string sConfig = string.Empty;

            strSql = "select CONFIGID" +
                    " FROM ASSORTEDINVENTDIMCOMBINATION IDC" +
                    " INNER JOIN ASSORTEDINVENTITEMS AIT ON AIT.ITEMID = IDC.ITEMID" +
                    " INNER JOIN INVENTDIM I ON I.INVENTDIMID = IDC.INVENTDIMID AND I.DATAAREAID = IDC.DATAAREAID" +
                    " LEFT OUTER JOIN ECORESCOLOR ON ECORESCOLOR.NAME = I.INVENTCOLORID" +
                    " LEFT OUTER JOIN ECORESPRODUCTMASTERCOLOR ON (ECORESPRODUCTMASTERCOLOR.COLOR = ECORESCOLOR.RECID)" +
                    " AND (ECORESPRODUCTMASTERCOLOR.COLORPRODUCTMASTER = AIT.PRODUCT)" +
                    " LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVC ON DVC.RECID = ECORESPRODUCTMASTERCOLOR.RECID" +
                    " where AIT.ITEMID='" + txtItemId.Text + "' and CONFIGID='" + cmbConfig.Text + "'";

            sConfig = NIM_ReturnExecuteScalar(strSql);

            if (!string.IsNullOrEmpty(cmbConfig.Text))
            {
                if (sConfig.ToUpper() != cmbConfig.Text.Trim().ToUpper())
                {
                    MessageBox.Show("Invalid configuration.");
                    cmbConfig.Focus();
                }
                else
                    cmbCode.Focus();
            }
        }

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
                    grItems.DataSource = dtItemInfo.DefaultView;
                }
            }


            if (DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;
            ClearControls();

            CalTotal();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;

            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    btnSubmit.Enabled = false;
                    btnDelete.Enabled = false;
                    btnFetchOD.Enabled = false;
                    btnOgFetch.Enabled = false;
                    IsEdit = true;
                    txtItemId.Enabled = false;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtItemInfo.Rows[EditselectedIndex];
                    cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                    txtBatch.Text = Convert.ToString(theRowToSelect["BATCH"]);
                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    lblUnit.Text = Convert.ToString(theRowToSelect["UNITID"]);
                    metalType = Convert.ToString(theRowToSelect["METALTYPE"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);//hidden qty=gross
                    txtGrossWt.Text = Convert.ToString(theRowToSelect["GrossWt"]);
                    txtNetWt.Text = Convert.ToString(theRowToSelect["NetWT"]);

                    txtQuantity.Tag = Convert.ToString(theRowToSelect["QUANTITY"]);
                    txtGrossWt.Tag = Convert.ToString(theRowToSelect["GrossWt"]);//keep the value for validation
                    txtNetWt.Tag = Convert.ToString(theRowToSelect["NetWT"]);//keep the value for validation
                    txtPCS.Tag = Convert.ToString(theRowToSelect["PCS"]);//keep the value for validation

                    //metalType = NIM_ReturnExecuteScalar("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + txtItemId.Text + "'");

                    // string sOgRateEdit = NIM_ReturnExecuteScalar("SELECT top 1 OGRateEdit FROM RETAILPARAMETERS ");

                    unitid = NIM_ReturnExecuteScalar("select UNITID from INVENTTABLEMODULE WHERE ITEMID='" + txtItemId.Text + "'");
                    // lblUnit.Text = unitid;

                    iDecPre = GetDecimalPrecison(unitid);


                    /* if (metalType == Convert.ToString((int)MetalType.Gold) || metalType == Convert.ToString((int)MetalType.Silver)
                         || metalType == Convert.ToString((int)MetalType.Platinum) || metalType == Convert.ToString((int)MetalType.Palladium))
                     {
                         if (string.IsNullOrEmpty(cmbConfig.Text))
                         {
                             cmbConfig.Text = sSelectedItemConfigId;
                         }
                         cmbConfig.Enabled = false;
                     }
                     else
                     {
                         cmbConfig.Text = "";
                         cmbConfig.Enabled = true;
                     }*/
                }
            }

            if (IsCatchWtItem(txtItemId.Text))
            {
                txtPCS.Enabled = true;
                txtQuantity.Enabled = false;
                // txtQuantity.Text = "";
            }
            else
            {
                txtPCS.Text = "";
                txtPCS.Enabled = false;
                txtQuantity.Enabled = true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearControls();
            grItems.DataSource = null;
            dtItemInfo.Clear();

            CalTotal();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateControls())
            {
                SaveBulkItemIssue();
                CalTotal();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Function
        public string NIM_ReturnExecuteScalar(string query)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection myCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            myCon.Open();

            try
            {
                if (myCon.State == ConnectionState.Closed)
                    myCon.Open();
                cmd = new SqlCommand(query, myCon);
                return Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (myCon.State == ConnectionState.Open)
                    myCon.Close();
            }
        }

        private bool ValidateItem(string item)
        {
            string sOutPut = string.Empty;

            string query = " DECLARE @ITEMID NVARCHAR(20) " +
                            " SELECT TOP(1) @ITEMID=ITEMID FROM INVENTTABLE WHERE ITEMID='" + item + "' AND RETAIL!=1" +
                            " IF ISNULL(@ITEMID,'')='' " +
                                " BEGIN " +
                                    " SELECT 'False' " +
                                " END " +
                            " ELSE " +
                                " BEGIN " +
                                    " SELECT 'True' " +
                                " END ";


            sOutPut = NIM_ReturnExecuteScalar(query);

            return Convert.ToBoolean(sOutPut);
        }

        public string GetStandardQuantityFromDB(SqlConnection conn, string itemid)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT STDQTY FROM INVENTTABLE WHERE ITEMID='" + itemid + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sQty = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sQty;

        }

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
            int iTransType = (int)TransactionType.BulkItemIssue;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(ReceiptId,@VAL,LEN(ReceiptId)),0) AS INTEGER)) + 1 from BulkItemIssueHeader";

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

        public string GetNextBulkItemIssueTransactionId()
        {
            try
            {
                TransactionType transType = TransactionType.BulkItemIssue;
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

        public string GetBulkItemIssueId()
        {
            string TransId = string.Empty;
            TransId = GetNextBulkItemIssueTransactionId();
            return TransId;
        }

        public string ShowMessage(string _msg)
        {
            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(_msg, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                return _msg;
            }
        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        private bool ValidateControls()
        {
            bool bResult = true;
            DataTable dtGroup = new DataTable();

            if (dtItemInfo == null || dtItemInfo.Rows.Count == 0)
            {
                ShowMessage("Altleast one item should be fetched.");
                txtItemId.Focus();
                bResult = false;
            }
            else
            {
                if (dtItemInfo.Rows.Count > 0 && dtItemInfo != null)
                {
                    var query = from r in dtItemInfo.AsEnumerable()
                                // group r by r.Field<string>(1) into groupedTable
                                group r by new
                                {
                                    ITEMID = r.Field<string>("ITEMID"),
                                    CONFIGURATION = r.Field<string>("CONFIGURATION"),
                                    SIZE = r.Field<string>("SIZE"),
                                    COLOR = r.Field<string>("COLOR"),
                                    STYLE = r.Field<string>("STYLE"),
                                    BATCH = r.Field<string>("BATCH")
                                } into groupedTable
                                select new
                                {
                                    ITEMID = groupedTable.Key.ITEMID,
                                    CONFIGURATION = groupedTable.Key.CONFIGURATION,
                                    SIZE = groupedTable.Key.SIZE,
                                    COLOR = groupedTable.Key.COLOR,
                                    STYLE = groupedTable.Key.STYLE,
                                    BATCH = groupedTable.Key.BATCH,
                                    QUANTITY = groupedTable.Sum(s => s.Field<decimal>("QUANTITY")),
                                    PCS = groupedTable.Sum(s => s.Field<decimal>("PCS")),
                                    GROSSWT = groupedTable.Sum(s => s.Field<decimal>("GROSSWT")),
                                    NETWT = groupedTable.Sum(s => s.Field<decimal>("NETWT"))
                                };
                    dtGroup = ConvertToDataTable(query);


                    if (dtGroup.Rows.Count > 0 && dtGroup != null)
                    {
                        if (dtGroup.Rows.Count != dtItemInfo.Rows.Count)
                        {
                            MessageBox.Show("Please check the duplicate item with same config.", "Please check duplicate item with same config", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return false;
                        }
                    }
                }

                if (dtGroup.Rows.Count > 0 && dtGroup != null)
                {
                    foreach (DataRow dr in dtGroup.Rows)
                    {
                        if (IsBulkItem(Convert.ToString(dr["ITEMID"])))
                        {
                            //saleLine.PartnerData.BulkItem = 1;
                            decimal dBulkPdsQty = 0m;
                            decimal dBulkQty = 0m;
                            decimal dBulkGrossQty = 0m;
                            decimal dBulkNetQty = 0m;
                            GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty, ref dBulkGrossQty, ref dBulkNetQty,
                                                                Convert.ToString(dr["ITEMID"]), Convert.ToString(dr["CONFIGURATION"]),
                                                                Convert.ToString(dr["SIZE"]), Convert.ToString(dr["COLOR"]),
                                                                Convert.ToString(dr["STYLE"]), Convert.ToString(dr["BATCH"]));

                            if (IsCatchWtItem(Convert.ToString(dr["ITEMID"])))
                            {
                                if (dBulkPdsQty <= 0 || dBulkQty <= 0 || dBulkGrossQty <= 0 || dBulkNetQty < 0)
                                {
                                    MessageBox.Show("Please check the inventory", "Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                //else if (dBulkPdsQty == Convert.ToDecimal(dr["PCS"]) && dBulkQty != Convert.ToDecimal(dr["QUANTITY"]))
                                //{
                                //    MessageBox.Show("Please check the inventory  for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //    return false;
                                //}
                                //else if (dBulkPdsQty != Convert.ToDecimal(dr["PCS"]) && dBulkQty == Convert.ToDecimal(dr["QUANTITY"]))
                                //{
                                //    MessageBox.Show("Please check the inventory  for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //    return false;
                                //}
                                else if (dBulkPdsQty < Convert.ToDecimal(dr["PCS"]) || dBulkGrossQty < Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkPdsQty == Convert.ToDecimal(dr["PCS"]) && dBulkGrossQty != Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkPdsQty != Convert.ToDecimal(dr["PCS"]) && dBulkGrossQty == Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkPdsQty < Convert.ToDecimal(dr["PCS"]) || dBulkNetQty < Convert.ToDecimal(dr["NetWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }

                                //else if (dBulkPdsQty == Convert.ToDecimal(dr["PCS"]) && dBulkNetQty != Convert.ToDecimal(dr["NetWt"]))
                                //{
                                //    MessageBox.Show("Please check the inventory  for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //    return false;
                                //}
                                //else if (dBulkPdsQty != Convert.ToDecimal(dr["PCS"]) && dBulkNetQty == Convert.ToDecimal(dr["NetWt"]))
                                //{
                                //    MessageBox.Show("Please check the inventory  for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //    return false;
                                //}
                            }
                            else
                            {
                                if (dBulkQty <= 0 || dBulkGrossQty <= 0)
                                {
                                    MessageBox.Show("Please check the inventory", "Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkQty < Convert.ToDecimal(dr["QUANTITY"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkGrossQty < Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkNetQty < Convert.ToDecimal(dr["NetWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkQty < Convert.ToDecimal(dr["QUANTITY"]) || dBulkGrossQty < Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkQty == Convert.ToDecimal(dr["QUANTITY"]) && dBulkGrossQty != Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkQty != Convert.ToDecimal(dr["QUANTITY"]) && dBulkGrossQty == Convert.ToDecimal(dr["GrossWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                                else if (dBulkQty < Convert.ToDecimal(dr["QUANTITY"]) || dBulkNetQty < Convert.ToDecimal(dr["NetWt"]))
                                {
                                    MessageBox.Show("Please check the inventory for " + Convert.ToString(dr["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    bResult = false;
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid item", "Invalid item", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return false;
                        }
                    }
                }
            }
            return bResult;
        }

        private void GetValidBulkItemPcsAndQtyForTrans(ref decimal dPdsCWQTY,
                                                    ref decimal dQty, ref decimal dGrossWt, ref decimal dNetQty, string sItemId,
                                                    string ConfigID,
                                                    string SizeID,
                                                    string ColorID,
                                                    string StyleID,
                                                    string sBatchId)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select sum(PdsCWQty) as PDSCWQTY,sum(Qty) as QTY,sum(GrossWt) as GrossWt ,sum(NetWt) as NetWt from BulkItemTransTable ");
            sbQuery.Append(" Where ItemId='" + sItemId + "'");
            sbQuery.Append(" and InventColorId='" + ColorID + "'");
            sbQuery.Append(" and ConfigId='" + ConfigID + "'");
            sbQuery.Append(" and InventSizeId='" + SizeID + "'");
            sbQuery.Append(" and InventStyleId='" + StyleID + "'");
            sbQuery.Append(" and InventBatchId='" + sBatchId + "'");
            sbQuery.Append(" group by ItemId,ConfigId,InventColorId,InventSizeId,InventStyleId,InventBatchId ");

            DataTable dtBI = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtBI);

            if (dtBI != null && dtBI.Rows.Count > 0)
            {
                dPdsCWQTY = Convert.ToDecimal(dtBI.Rows[0]["PDSCWQTY"]);
                dQty = Convert.ToDecimal(dtBI.Rows[0]["QTY"]);
                dGrossWt = Convert.ToDecimal(dtBI.Rows[0]["GrossWt"]);
                dNetQty = Convert.ToDecimal(dtBI.Rows[0]["NetWt"]);
            }

        }

        private void GetItemDetails(ref string sArtCode, ref string sProdTypeCode, ref string sArtDesc, ref int dRetail, string sItemId)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select isnull(I.ARTICLE_CODE,'') ARTICLE_CODE ,isnull(PRODUCTTYPECODE,'') PRODUCTTYPECODE,");
            sbQuery.Append(" RETAIL,isnull(A.DESCRIPTION,'') DESCRIPTION from INVENTTABLE I");
            sbQuery.Append(" left join ARTICLE_MASTER A on I.ARTICLE_CODE=A.ARTICLE_CODE");
            sbQuery.Append(" where itemid='" + sItemId + "'");

            DataTable dtBI = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtBI);

            if (dtBI != null && dtBI.Rows.Count > 0)
            {
                sArtCode = Convert.ToString(dtBI.Rows[0]["ARTICLE_CODE"]);
                sArtDesc = Convert.ToString(dtBI.Rows[0]["DESCRIPTION"]);
                sProdTypeCode = Convert.ToString(dtBI.Rows[0]["PRODUCTTYPECODE"]);
                dRetail = Convert.ToInt16(dtBI.Rows[0]["RETAIL"]);
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
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bBulkItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bBulkItem;
        }

        private void SaveBulkItemIssue()
        {
            int iTransfer_Header = 0;
            int iTransfer_Details = 0;
            string TransferId = GetBulkItemIssueId();
            SqlTransaction transaction = null;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " INSERT INTO [BulkItemIssueHeader]([ReceiptId]," +
                                 " [TransDate],[RetailStaffId],[RetailStoreId],[RetailTerminalId]," +
                                 " [DATAAREAID])" +
                                 " VALUES(@ReceiptId,@TransDate,@RetailStaffId,@RetailStoreId," +
                                 " @RetailTerminalId, @DATAAREAID)";


            try
            {
                transaction = connection.BeginTransaction();

                #region Bulk Item receive HEADER
                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@ReceiptId", SqlDbType.NVarChar, 20).Value = TransferId.Trim();
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
                    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        #region  DETAILS & Transaction

                        string commandDetail = " INSERT INTO [BulkItemIssueDetails]([ReceiptId],[LineNumber],[ItemId],[ConfigId]," +
                                                         " [InventSizeId],[InventColorId],[InventStyleId],InventBatchId,[PdsCWQty],[Qty],[NetWt])" +
                                                         " VALUES(@ReceiptId  ,@LineNumber , @ItemId," +
                                                         " @ConfigId,@InventSizeId,@InventColorId," +
                                                         " @InventStyleId,@InventBatchId,@PdsCWQty,@Qty,@NetWt) ";

                        commandDetail += " INSERT INTO [BulkItemTransTable]([TransReceiptId],[LineNumber]," +
                                                        " [TransDate],[TransType],[ItemId],[ConfigId]," +
                                                        " [InventSizeId],[InventColorId],[InventStyleId]," +
                                                        " InventBatchId,[PdsCWQty],[Qty]," +
                                                        " [RetailStaffId],[RetailStoreId]," +
                                                        " [RetailTerminalId],DATAAREAID,GrossWt,NetWt)" +
                                                        " VALUES(@ReceiptId  ,@LineNumber ," +
                                                        " @TransDate,@TransType, @ItemId," +
                                                        " @ConfigId,@InventSizeId,@InventColorId,@InventStyleId," +
                                                        " @InventBatchId,@PdsCWQty,@Qty,@RetailStaffId," +
                                                        " @RetailStoreId,@RetailTerminalId, @DATAAREAID,@GrossWt,@NetWt) ";

                        commandDetail += "INSERT INTO [TransferOrderTrans] " +
                                       "([ItemId],[LineNumber],[TransType]," +
                                       " [TransferOrder],[TransDate],[ConfigId]," +
                                       " [InventSizeId],[InventColorId],[InventStyleId]," +
                                       " [InventBatchId],[PdsCWQty],[Qty]," +
                                       " [CostPrice],[TransferCostPrice],[ReceivedOrShipped]," +
                                       " RetailStaffId,RetailStoreId,RetailTerminalId,DATAAREAID,NetWeight,GrossWeight," +
                                       " Name,MetalType,Article_code,ProductTypeCode,ArticleDescription,RETAIL) " +
                                       " VALUES" +
                                       "(@ItemId,@LineNumber,1," +//1 for bulk
                                       " @ReceiptId,@TransDate,@ConfigId," +
                                       " @InventSizeId,@InventColorId,@InventStyleId," +
                                       " @InventBatchId,@PdsCWQty,@Qty," +
                                       " 0,0,0," +
                                       " @RetailStaffId,@RetailStoreId,@RetailTerminalId," +
                                       " @DATAAREAID,@NetWt,@GrossWt,@Name,@MetalType,@Article_code," +
                                       " @ProductTypeCode,@ArticleDescription,@RETAIL)";

                        for (int ItemCount = 0; ItemCount < dtItemInfo.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdDetail = new SqlCommand(commandDetail, connection, transaction);
                            cmdDetail.Parameters.Add("@ReceiptId", SqlDbType.NVarChar, 20).Value = TransferId.Trim();
                            cmdDetail.Parameters.Add("@LineNumber", SqlDbType.Int, 10).Value = ItemCount + 1;
                            cmdDetail.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();// dtpTillDate.Value.ToShortDateString();

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["ItemId"])))
                                cmdDetail.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["ItemId"]);

                            cmdDetail.Parameters.Add("@ConfigId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["CONFIGURATION"]);
                            cmdDetail.Parameters.Add("@InventSizeId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["SIZE"]);
                            cmdDetail.Parameters.Add("@InventColorId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["COLOR"]);
                            cmdDetail.Parameters.Add("@InventStyleId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["STYLE"]);
                            cmdDetail.Parameters.Add("@InventBatchId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["BATCH"]);

                            if (Convert.ToInt16(dtItemInfo.Rows[ItemCount]["METALTYPE"]) != (int)MetalType.Jewellery)
                            {
                                if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["PCS"])))
                                    cmdDetail.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = DBNull.Value;
                                else
                                    cmdDetail.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItemInfo.Rows[ItemCount]["PCS"]) * -1;
                            }
                            else
                            {
                                cmdDetail.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = 0;
                            }

                            //if (Convert.ToInt16(dtItemInfo.Rows[ItemCount]["METALTYPE"]) != (int)MetalType.Jewellery)
                            //{
                            //    if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["QUANTITY"])))
                            //        cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                            //    else
                            //        cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItemInfo.Rows[ItemCount]["QUANTITY"]) * -1;
                            //}
                            //else
                            //{
                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["QUANTITY"])))//
                                cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItemInfo.Rows[ItemCount]["QUANTITY"]) * -1;
                            //}


                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["GrossWt"])))
                                cmdDetail.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItemInfo.Rows[ItemCount]["GrossWt"]) * -1;

                            if (string.IsNullOrEmpty(Convert.ToString(dtItemInfo.Rows[ItemCount]["NetWt"])))
                                cmdDetail.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdDetail.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dtItemInfo.Rows[ItemCount]["NetWt"]) * -1;


                            cmdDetail.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)BulkTransactionType.Issue;
                            cmdDetail.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdDetail.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdDetail.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                            if (application != null)
                                cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                            else
                                cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            cmdDetail.Parameters.Add("@Name", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["ITEMNAME"]);
                            cmdDetail.Parameters.Add("@MetalType", SqlDbType.Int).Value = Convert.ToInt16(dtItemInfo.Rows[ItemCount]["METALTYPE"]);
                            cmdDetail.Parameters.Add("@Article_code", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["ArticleCode"]);
                            cmdDetail.Parameters.Add("@ProductTypeCode", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["ProductTypeCode"]);
                            cmdDetail.Parameters.Add("@ArticleDescription", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtItemInfo.Rows[ItemCount]["ArticleDescription"]);
                            cmdDetail.Parameters.Add("@RETAIL", SqlDbType.Int).Value = Convert.ToInt16(dtItemInfo.Rows[ItemCount]["RETAIL"]);

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
                    MessageBox.Show("Issued successfully, Issue Id is " + TransferId + "");

                    dtItemInfo.Clear();

                    grItems.DataSource = null;
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

        private string GetInventDimId()
        {
            string sDimId = string.Empty;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT INVENTDIMID FROM INVENTDIM WHERE INVENTSIZEID = '" + cmbSize.Text.Trim() + "'" +
                                " and INVENTCOLORID = '" + cmbCode.Text.Trim() + "'" +
                                " and CONFIGID = '" + cmbConfig.Text.Trim() + "'" +
                                " and INVENTLOCATIONID = ''" +
                                " and INVENTBATCHID = ''" +
                                " and INVENTSITEID = ''";


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            sDimId = Convert.ToString(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sDimId;
        }

        void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            txtPCS.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            cmbCode.Text = string.Empty;
            cmbSize.Text = string.Empty;
            lblUnit.Text = string.Empty;
            IsEdit = false;
            cmbConfig.Text = string.Empty;
            cmbSize.Text = string.Empty;
            cmbCode.Text = string.Empty;
            txtBatch.Text = "";
            txtGrossWt.Text = "";
            txtNetWt.Text = "";
            txtGrossWt.Tag = "";
            txtNetWt.Tag = "";
            txtPCS.Tag = "";
            txtQuantity.Tag = "";
            txtItemId.Enabled = true;
            btnSubmit.Enabled = true;
            btnDelete.Enabled = true;
            btnFetchOD.Enabled = true;
            btnOgFetch.Enabled = true;
        }

        bool ItemValidate()
        {
            bool bResult = false;
            if (string.IsNullOrEmpty(txtItemId.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtItemId.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    bResult = false;
                    return bResult;
                }
            }
            if (string.IsNullOrEmpty(txtItemName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtItemId.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    bResult = false;
                    return bResult;
                }
            }
            if (string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                if (IsCatchWtItem(txtItemId.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtPCS.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        bResult = false;
                        return bResult;
                    }
                }
            }
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    bResult = false;
                    return bResult;
                }
            }
            if (string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Gross wt can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtGrossWt.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    bResult = false;
                    return bResult;
                }
            }
            if (string.IsNullOrEmpty(txtNetWt.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net wt can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtNetWt.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    bResult = false;
                    return bResult;
                }
            }
            else
            {

                //if (IsCatchWtItem(txtItemId.Text))
                //{
                //    decimal dQty = Convert.ToDecimal(txtQuantity.Text);
                //    decimal dPcs = Convert.ToDecimal(txtPCS.Text.Trim());

                //    string sMaxQty = NIM_ReturnExecuteScalar("SELECT isnull(PDSCWMAX,0) PDSCWMAX FROM PDSCATCHWEIGHTITEM  WHERE ITEMID ='" + txtItemId.Text + "'");
                //    string sMinQty = NIM_ReturnExecuteScalar("SELECT isnull(PDSCWMIN,0) PDSCWMIN FROM PDSCATCHWEIGHTITEM  WHERE ITEMID ='" + txtItemId.Text + "'");

                //    if (dQty > 0 && dPcs > 0)
                //    {
                //        if (dQty > (Convert.ToDecimal(sMaxQty) * dPcs) || dQty < (Convert.ToDecimal(sMinQty) * dPcs))
                //        {
                //            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity shuld be within " + (Convert.ToDecimal(sMaxQty) * dPcs) + " and " + (Convert.ToDecimal(sMinQty) * dPcs) + " ", MessageBoxButtons.OK, MessageBoxIcon.Information))
                //            {
                //                txtQuantity.Focus();
                //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //                return false;
                //            }
                //        }
                //        else
                //            return true;
                //    }
                //    else
                //        return false;
                //}
                //else
                if (IsEdit)
                {
                    decimal dPcs = 0;
                    decimal dOriPcs = 0;
                    decimal dGrossQty = Convert.ToDecimal(txtGrossWt.Text);
                    decimal dNetQty = Convert.ToDecimal(txtNetWt.Text);
                    decimal dOriGrossQty = Convert.ToDecimal(txtGrossWt.Tag);
                    decimal dOriNetQty = Convert.ToDecimal(txtNetWt.Tag);
                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        dPcs = Convert.ToDecimal(txtPCS.Text.Trim());
                        dOriPcs = Convert.ToDecimal(txtPCS.Tag);
                    }
                    if (dGrossQty < dNetQty)
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Gross wt can not be less than net wt", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            txtNetWt.Focus();
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            bResult = false;
                            return bResult;
                        }
                    }

                    if (dGrossQty > dOriGrossQty)
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Gross wt can not be greater than '" + dOriGrossQty + "'", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            txtGrossWt.Focus();
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            bResult = false;
                            return bResult;
                        }
                    }
                    if (dNetQty > dOriNetQty)
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net wt can not be greater than '" + dOriNetQty + "'", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            txtNetWt.Focus();
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            bResult = false;
                            return bResult;
                        }
                    }
                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        if (dPcs > dOriPcs)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Pcs can not be greater than '" + dOriPcs + "'", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                txtPCS.Focus();
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                bResult = false;
                                return bResult;
                            }
                        }
                    }

                    decimal dBulkPdsQty = 0m;
                    decimal dBulkQty = 0m;
                    decimal dGrossWt = 0m;
                    decimal dNetWt = 0m;
                    GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty, ref dGrossWt, ref dNetWt,
                                                        txtItemId.Text, cmbConfig.Text,
                                                        cmbSize.Text, cmbCode.Text,
                                                        "", txtBatch.Text);

                    if (IsCatchWtItem(txtItemId.Text))
                    {
                        if (dBulkPdsQty <= 0 || dBulkQty <= 0 || dGrossWt < 0 || dNetWt < 0)
                        {
                            MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkPdsQty < Convert.ToDecimal(txtPCS.Text) || dGrossWt < Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkPdsQty == Convert.ToDecimal(txtPCS.Text) && dGrossWt != Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkPdsQty != Convert.ToDecimal(txtPCS.Text) && dGrossWt == Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkPdsQty < Convert.ToDecimal(txtPCS.Text) || dNetWt < Convert.ToDecimal(txtNetWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }

                        //else if (dBulkPdsQty == Convert.ToDecimal(txtPCS.Text) && dNetWt != Convert.ToDecimal(txtNetWt.Text))
                        //{
                        //    MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //    bResult = false;
                        //    return bResult;
                        //}

                        //else if (dBulkPdsQty != Convert.ToDecimal(txtPCS.Text) && dNetWt == Convert.ToDecimal(txtNetWt.Text))
                        //{
                        //    MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //    bResult = false;
                        //    return bResult;
                        //}
                        else if ((Convert.ToDecimal(txtNetWt.Tag) - Convert.ToDecimal(txtNetWt.Text)) > (Convert.ToDecimal(txtGrossWt.Tag) - Convert.ToDecimal(txtGrossWt.Text)))
                        {
                            MessageBox.Show("Net wt balance should be less than Gross wt balance for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else
                            bResult = true;
                    }
                    else
                    {
                        if (dBulkQty <= 0 || dGrossWt < 0)
                        {
                            MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkQty < Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dGrossWt < Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dNetWt < Convert.ToDecimal(txtNetWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkQty < Convert.ToDecimal(txtQuantity.Text) || dGrossWt < Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkQty == Convert.ToDecimal(txtQuantity.Text) && dGrossWt != Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkQty != Convert.ToDecimal(txtQuantity.Text) && dGrossWt == Convert.ToDecimal(txtGrossWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if (dBulkQty < Convert.ToDecimal(txtQuantity.Text) || dNetWt < Convert.ToDecimal(txtNetWt.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else if ((Convert.ToDecimal(txtNetWt.Tag) - Convert.ToDecimal(txtNetWt.Text)) > (Convert.ToDecimal(txtGrossWt.Tag) - Convert.ToDecimal(txtGrossWt.Text)))
                        {
                            MessageBox.Show("Net wt balance should be less than Gross wt balance for " + txtItemId.Text + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            bResult = false;
                            return bResult;
                        }
                        else
                            bResult = true;
                    }
                }
                else
                    bResult = true;

            }
            return bResult;
        }
        #endregion

        private void btnOgFetch_Click(object sender, EventArgs e)
        {
            DataTable dtTrans = new DataTable();

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string sQuery = "GETOGDATAFORBULKISSUE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@FDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpFDate.Value).ToShortDateString();
            command.Parameters.Add("@TDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpToDate.Value).ToShortDateString();
            command.Parameters.Add("@ISOG", SqlDbType.Int).Value = 1;
            command.Parameters.Add("@METALTYPE", SqlDbType.Int).Value = Convert.ToInt16(cmbMetalType.SelectedValue);
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);

            dtItemInfo = dtTrans;
            grItems.DataSource = dtItemInfo.DefaultView;

            CalTotal();
        }

        private void CalTotal()
        {
            int iNoOfPcs = 0;
            decimal dTotQty = 0m;
            decimal dTotGrWt = 0m;
            decimal dTotNetWt = 0m;
            //Start : added on 10/05/2017
            foreach (DataRow dr1 in dtItemInfo.Rows)
            {
                iNoOfPcs = iNoOfPcs + Convert.ToInt32(dr1["PCS"]);
                dTotQty = dTotQty + Convert.ToDecimal(dr1["QUANTITY"]);
                dTotGrWt = dTotGrWt + Convert.ToDecimal(dr1["GrossWt"]);
                dTotNetWt = dTotNetWt + Convert.ToDecimal(dr1["NetWt"]);
            }

            lblTotPcs.Text = Convert.ToString(iNoOfPcs);
            lblTotQty.Text = Convert.ToString(dTotQty);
            lblTotGrWt.Text = Convert.ToString(dTotGrWt);
            lblTotNetWt.Text = Convert.ToString(dTotNetWt);
            //End : added on 10/05/2017
        }

        private void txtPCS_Leave(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtItemId.Text) && !string.IsNullOrEmpty(txtPCS.Text))
            //{
            //    string sQty = NIM_ReturnExecuteScalar("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + txtItemId.Text + "'");

            //}
        }

        private void btnFetchOD_Click(object sender, EventArgs e)
        {
            DataTable dtTrans = new DataTable();

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string sQuery = "GETOGDATAFORBULKISSUE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@FDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpFDate.Value).ToShortDateString();
            command.Parameters.Add("@TDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtpToDate.Value).ToShortDateString();
            command.Parameters.Add("@ISOG", SqlDbType.Int).Value = 0;//for dmd
            command.Parameters.Add("@METALTYPE", SqlDbType.Int).Value = Convert.ToInt16(cmbMetalType.SelectedValue);
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);

            dtItemInfo = dtTrans;
            grItems.DataSource = dtItemInfo.DefaultView;

            CalTotal();
        }

        private void txtGrossWt_Leave(object sender, EventArgs e)
        {
            if (IsCatchWtItem(txtItemId.Text))
            {
                txtQuantity.Text = txtGrossWt.Text;
            }
        }

        private void btnFetchSilver_Click(object sender, EventArgs e)
        {
            DataTable dtTrans = new DataTable();

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string sQuery = "GETOGDATAFORBULKISSUE";
            SqlCommand command = new SqlCommand(sQuery, connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@FDate", SqlDbType.DateTime).Value = dtpFDate.Value;
            command.Parameters.Add("@TDate", SqlDbType.DateTime).Value = dtpToDate.Value;
            command.Parameters.Add("@METALTYPE", SqlDbType.Int).Value = cmbMetalType.SelectedValue;
            command.Parameters.Add("@ISOG", SqlDbType.Int).Value = 2;//silver
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daTrans = new SqlDataAdapter(command);
            daTrans.Fill(dtTrans);

            dtItemInfo = dtTrans;
            grItems.DataSource = dtItemInfo.DefaultView;

            CalTotal();
        }

    }
}
