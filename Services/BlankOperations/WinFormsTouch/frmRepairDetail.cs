
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Item;
using Microsoft.Dynamics.Retail.Pos.Dimension;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmRepairDetail:frmTouchBase
    {
        #region Variable Declaration
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        public DataTable dtSubOrderDetails = new DataTable("dtSubOrderDetails");
        public decimal sExtendedDetailsAmount = 0m;
        frmRepair frmRepairOrder;
        Random randUnique = new Random();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataSet dsSearchedOrder = new DataSet();
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        public DataTable dtSample = new DataTable();
        string unitid = string.Empty;
        string Previewdimensions = string.Empty;
        bool isItemExists = false;
        public DataTable dtSketch = new DataTable();
        int iDecPre = 0;

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pcs = 1,
            Tot = 2,
        }
        #endregion

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion

        #region enum  RateTypeNew
        enum RateTypeNew
        {
            Purchase = 0,
            OGP = 1,
            OGOP = 2,
            Sale = 3,
            GSS = 4,
            Exchange = 6,
            OtherExchange = 8,
        }
        #endregion

        #region enum MetalType
        enum MetalType
        {
            Gold = 0,
            Silver = 1,
            Platinum = 2,
            Alloy = 3,
            Diamond = 4,
            Pearl = 5,
            Stone = 6,
            Consumables = 7,
            Copper = 8,
            GoldSilver = 9,
            Other = 10,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,

        }
        #endregion

        #endregion

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

        #region frmRepairDetail
        public frmRepairDetail(IPosTransaction posTransaction, IApplication Application, frmRepair fRepairOrder)
        {
            InitializeComponent();
            grdView.OptionsView.ColumnAutoWidth = false;
            grdView.ScrollStyle = ScrollStyleFlags.LiveHorzScroll;
            grdView.HorzScrollVisibility = ScrollVisibility.Always;

            pos = posTransaction;
            application = Application;
            frmRepairOrder = fRepairOrder;
            LoadJobType();
            LoadArticle();

            btnPOSItemSearch.Focus();
            btnSample.Visible = false;
        }

        public frmRepairDetail(DataTable dtSubOrder, DataTable dtOrderDetails, IPosTransaction posTransaction, IApplication Application, frmRepair fRepairOrder, DataTable dtSketchDetails)
        {
            InitializeComponent();
            grdView.OptionsView.ColumnAutoWidth = false;
            grdView.ScrollStyle = ScrollStyleFlags.LiveHorzScroll;
            grdView.HorzScrollVisibility = ScrollVisibility.Always;

            btnSample.Visible = false;
            dtItemInfo = dtOrderDetails;
            dtSubOrderDetails = dtSubOrder;
            pos = posTransaction;
            application = Application;
            frmRepairOrder = fRepairOrder;
            dtSketch = dtSketchDetails;

            //  BindRateTypeMakingTypeCombo();
            btnPOSItemSearch.Focus();
            grItems.DataSource = dtItemInfo.DefaultView;

            //if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            //{
            //    Decimal dTotalAmount = 0m;
            //    foreach (DataRow drTotal in dtItemInfo.Rows)
            //    {
            //        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
            //    }
            //    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
            //}
            LoadJobType();//Nimbus by MIAM @ 03Jul14 : added
            LoadArticle();

        }

        public frmRepairDetail(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, frmRepair fRepairOrder)
        {
            InitializeComponent();
            grdView.OptionsView.ColumnAutoWidth = false;
            grdView.ScrollStyle = ScrollStyleFlags.LiveHorzScroll;
            grdView.HorzScrollVisibility = ScrollVisibility.Always;

            btnSample.Visible = false;
            dsSearchedOrder = dsSearchedDetails;
            pos = posTransaction;
            application = Application;
            frmRepairOrder = fRepairOrder;
            //BindRateTypeMakingTypeCombo();
            btnPOSItemSearch.Focus();
            grItems.DataSource = dsSearchedDetails.Tables[1].DefaultView;

            /* 
            if (dsSearchedDetails.Tables[1] != null && dsSearchedDetails.Tables[1].Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dsSearchedDetails.Tables[1].Rows)
                {
                    dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                }
                txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
            }
             */
            LoadJobType();//Nimbus by MIAM @ 03Jul14 : added
            LoadArticle();

            panel1.Enabled = false;
            btnSubmit.Enabled = false;
            btnEdit.Enabled = false;
            btnClear.Enabled = false;
            btnDelete.Enabled = false;
            btnPOSItemSearch.Enabled = false;
            btnAddItem.Enabled = false;

        }
        #endregion

        #region Load JobType Combo //Nimbus by MIAM @ 03Jul14 : added
        private void LoadJobType()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" SELECT CODE='0',[DESCRIPTION]='-Select-',CHARGE=0 ");
            sbQuery.AppendLine(" UNION ");
            sbQuery.AppendFormat("SELECT CODE,DESCRIPTION,convert(decimal(18,2),CHARGE) CHARGE from CRWJOBTYPEMST WHERE DATAAREAID='{0}'", ApplicationSettings.Database.DATAAREAID);
            DataTable dtJobType = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            using(SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        dtJobType.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if(connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            if(dtJobType.Rows.Count > 0)
            {
                cmbRate.DataSource = dtJobType;
                cmbRate.DisplayMember = "DESCRIPTION";
                cmbRate.ValueMember = "CODE";
            }

        }
        #endregion

        private void LoadArticle()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.AppendLine(" SELECT CODE='0',[DESCRIPTION]='-Select-' ");
            sbQuery.AppendLine(" UNION ");
            sbQuery.AppendLine(" SELECT distinct ARTICLE_CODE CODE,[DESCRIPTION] from ARTICLE_MASTER");// WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            DataTable dtArticle = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            using(SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        dtArticle.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(dtArticle.Rows.Count > 0)
            {
                cmbArticle.DataSource = dtArticle;
                cmbArticle.DisplayMember = "DESCRIPTION";
                cmbArticle.ValueMember = "CODE";
                //lblArticleDesc.Text = cmbArticle.DisplayMember;
            }
        }

        private string ColorSizeStyleConfig()
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig;
            colorSizeStyleConfig = new StringBuilder("");
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

        private bool IsSKUItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + sItemId + "') ");
            commandText.Append(" BEGIN SET @IsSKU = 1 END SELECT @IsSKU");


            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        #region POS-ITEM-SEARCH-CLICK
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
            //if(!grdView.IsEmpty)
            //{

            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Only one item can be added at time", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return;
            //    }
            //}
            Dialog objdialog = new Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            objdialog.MyItemSearch(50000, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE REPAIRITEM=1) ");

            saleLineItem = new SaleLineItem();

            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);

                    Item objItem = new Item();
                    objItem.MYProcessItem(saleLineItem, application);

                    Dimension objDim = new Dimension();
                    DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);

                    #region testing
                    //DataRow dr = null;
                    //dr = dtDimension.NewRow();
                    //dr[0] = "";
                    //dr[1] = "NSPL-00016";
                    //dr[2] = "5637144869";
                    //dr[3] = "42";
                    //dr[4] = "01";
                    //dr[5] = "1";
                    //dr[6] = "HD";
                    //dr[7] = "Black";
                    //dr[8] = "42";
                    //dr[9] = "";
                    //dr[10] = "High Definition";
                    //dr[11] = "1";
                    //dr[12] = "1";
                    //dr[13] = "0.0000000000000000";
                    //dr[14] = "0.0000000000000000";
                    //dr[15] = "0.0000000000000000";
                    //dtDimension.Rows.Add(dr);
                    #endregion


                    if(dtDimension != null && dtDimension.Rows.Count > 0)
                    {
                        DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                        dimConfirmation.InventDimCombination = dtDimension;
                        dimConfirmation.DimensionData = saleLineItem.Dimension;
                        if(!IsSKUItem(saleLineItem.ItemId))
                        {
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
                            cmbSize.Text = dtDimension.Rows[0]["sizeid"].ToString();
                            cmbConfig.Text = dtDimension.Rows[0]["configid"].ToString();
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
                    SqlConnection conn = new SqlConnection();
                    if(application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;
                    txtPCS.Focus();
                   // 
                    //string sQty = string.Empty;
                    //sQty = GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])); // // CHECK -- S
                    //if (!string.IsNullOrEmpty(sQty))
                    //{
                    //    sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                    //    txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    //}
                    txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    txtRate.Text = "0.00";// // CHECK -- S // getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);

                    // CheckMakingRateFromDB(conn, pos.StoreId, txtItemId.Text);

                    if (IsCatchWtItem(txtItemId.Text.Trim()))
                    {
                        txtPCS.Text = "1";
                        txtPCS.Enabled = true;
                    }
                    else
                    {
                        txtPCS.Text = "1";
                        txtPCS.Enabled = false;
                    }

                    unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                    iDecPre = GetDecimalPrecison(unitid);
                    //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();
                   
                    txtGrossWt.Text = "0.000";
                    txtQuantity.Text = "0.000";
                    txtNettWt.Text = "0.000";
                    txtDiaWt.Text = "0.000";
                    txtStnWt.Text = "0.000";
                    txtDiaAmt.Text = "0.00";
                    txtStnAmt.Text = "0.00";
                    txtTotalAmt.Text = "0.00";

                    isItemExists = true;
                }
            }
            else
            {
                isItemExists = false;
            }

        }
        #endregion

        private int GetDecimalPrecison(string sUnit)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT top 1 DECIMALPRECISION FROM UNITOFMEASURE WHERE SYMBOL='" + sUnit + "'");

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if(ValidateControls())
            {
                DataRow dr;
                if(IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string)); //Nimbus by MIAM @ 03Jul14 : ENABLED
                    dtItemInfo.Columns.Add("COLOR", typeof(string)); //Nimbus by MIAM @ 03Jul14 : ENABLED
                    dtItemInfo.Columns.Add("SIZE", typeof(string)); //Nimbus by MIAM @ 03Jul14 : ENABLED
                    dtItemInfo.Columns.Add("STYLE", typeof(string)); //Nimbus by MIAM @ 03Jul14 : ENABLED
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("GROSSQTY", typeof(decimal));
                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));
                    dtItemInfo.Columns.Add("RATE", typeof(decimal));
                    //dtItemInfo.Columns.Add("RATETYPE", typeof(string));
                    //dtItemInfo.Columns.Add("MAKINGRATE", typeof(decimal));
                    //dtItemInfo.Columns.Add("MAKINGTYPE", typeof(string));
                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                    // dtItemInfo.Columns.Add("MAKINGAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("EXTENDEDDETAILS", typeof(decimal));
                    dtItemInfo.Columns.Add("TOTALAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("ROWTOTALAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("DIMENSIONS", typeof(string));

                    //Added on 15-01-2012
                    dtItemInfo.Columns.Add("UNITID", typeof(string));

                    dtItemInfo.Columns.Add("REPAIRJOBDETAILS", typeof(string)); // added on 19.06.2013

                    dtItemInfo.Columns.Add("NETTWT", typeof(decimal)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("DIAWT", typeof(decimal)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("DIAAMT", typeof(decimal)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("STNWT", typeof(decimal)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("STNAMT", typeof(decimal)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("TOTALAMT", typeof(decimal)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("JOBTYPE", typeof(string)); //Nimbus by MIAM @ 03Jul14 : ADDED
                    dtItemInfo.Columns.Add("ARTICLE", typeof(string));

                }
                if(IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next()); // CHECK -- S
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

                     if(!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                         dr["GROSSQTY"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                         dr["GROSSQTY"] = DBNull.Value;

                    if(!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["RATE"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["RATE"] = 0m;

                    //dr["RATETYPE"] = DBNull.Value;
                    //dr["MAKINGRATE"] = DBNull.Value;
                    //dr["MAKINGTYPE"] = DBNull.Value;

                    //if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                    //    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    //else
                    //    dr["AMOUNT"] = 0m;


                    if(!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["AMOUNT"] = 0m;

                    if(!string.IsNullOrEmpty(txtJobDetails.Text.Trim()))
                        dr["REPAIRJOBDETAILS"] = txtJobDetails.Text.Trim();

                    //  dr["MAKINGAMOUNT"] = 0m;
                    dr["ROWTOTALAMOUNT"] = 0m;// decimal.Round((Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim())), 2, MidpointRounding.AwayFromZero);

                    //if (!string.IsNullOrEmpty(txtTotalAmount.Text.Trim()))
                    //    dr["TOTALAMOUNT"] = decimal.Round(Convert.ToDecimal(txtTotalAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    //else
                    dr["TOTALAMOUNT"] = DBNull.Value;

                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;
                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    dr["DIMENSIONS"] = Previewdimensions;

                    if(!string.IsNullOrEmpty(txtNettWt.Text.Trim()))
                        dr["NETTWT"] = objRounding.Round(Convert.ToDecimal(txtNettWt.Text.Trim()), 3);
                    else
                        dr["NETTWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaWt.Text.Trim()))
                        dr["DIAWT"] = objRounding.Round(Convert.ToDecimal(txtDiaWt.Text.Trim()), 3);
                    else
                        dr["DIAWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaAmt.Text.Trim()))
                        dr["DIAAMT"] = objRounding.Round(Convert.ToDecimal(txtDiaAmt.Text.Trim()), 2);
                    else
                        dr["DIAAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtStnWt.Text.Trim()))
                        dr["STNWT"] = objRounding.Round(Convert.ToDecimal(txtStnWt.Text.Trim()), 3);
                    else
                        dr["STNWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtStnAmt.Text.Trim()))
                        dr["STNAMT"] = objRounding.Round(Convert.ToDecimal(txtStnAmt.Text.Trim()), 2);
                    else
                        dr["STNAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtTotalAmt.Text.Trim()))
                        dr["TOTALAMT"] = objRounding.Round(Convert.ToDecimal(txtTotalAmt.Text.Trim()), 2);
                    else
                        dr["TOTALAMT"] = objRounding.Round(decimal.Zero, 2);

                    dr["JOBTYPE"] = cmbRate.SelectedValue;
                    dr["ARTICLE"] = cmbArticle.Text;

                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                if(IsEdit == true)
                {

                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    EditRow["PCS"] = txtPCS.Text.Trim();
                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);  // CHECK -- S
                    EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                    EditRow["GROSSQTY"] = txtGrossWt.Text.Trim();
                    
                    //  EditRow["RATETYPE"] = "";// cmbRateType.Text.Trim();
                    EditRow["RATE"] = txtRate.Text.Trim();
                    //EditRow["AMOUNT"] = txtAmount.Text.Trim();

                    EditRow["AMOUNT"] = txtRate.Text.Trim();
                    EditRow["REPAIRJOBDETAILS"] = txtJobDetails.Text.Trim();
                    EditRow["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    //EditRow["RATETYPE"] = "";// Convert.ToString(cmbRateType.Text.Trim());
                    //EditRow["MAKINGRATE"] = 0m;// decimal.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    //EditRow["MAKINGTYPE"] = "";// Convert.ToString(cmbMakingType.Text.Trim());
                    //EditRow["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);

                    //EditRow["ROWTOTALAMOUNT"] = Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()) + (string.IsNullOrEmpty(Convert.ToString(EditRow["EXTENDEDDETAILS"])) ? 0 : Convert.ToDecimal(EditRow["EXTENDEDDETAILS"]));
                    EditRow["ROWTOTALAMOUNT"] = Convert.ToDecimal(txtAmount.Text.Trim()) + (string.IsNullOrEmpty(Convert.ToString(EditRow["EXTENDEDDETAILS"])) ? 0 : Convert.ToDecimal(EditRow["EXTENDEDDETAILS"]));

                    if(!string.IsNullOrEmpty(txtNettWt.Text.Trim()))
                        EditRow["NETTWT"] = objRounding.Round(Convert.ToDecimal(txtNettWt.Text.Trim()), 3);
                    else
                        EditRow["NETTWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaWt.Text.Trim()))
                        EditRow["DIAWT"] = objRounding.Round(Convert.ToDecimal(txtDiaWt.Text.Trim()), 3);
                    else
                        EditRow["DIAWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaAmt.Text.Trim()))
                        EditRow["DIAAMT"] = objRounding.Round(Convert.ToDecimal(txtDiaAmt.Text.Trim()), 2);
                    else
                        EditRow["DIAAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtStnWt.Text.Trim()))
                        EditRow["STNWT"] = objRounding.Round(Convert.ToDecimal(txtStnWt.Text.Trim()), 3);
                    else
                        EditRow["STNWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtStnAmt.Text.Trim()))
                        EditRow["STNAMT"] = objRounding.Round(Convert.ToDecimal(txtStnAmt.Text.Trim()), 2);
                    else
                        EditRow["STNAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtTotalAmt.Text.Trim()))
                        EditRow["TOTALAMT"] = objRounding.Round(Convert.ToDecimal(txtTotalAmt.Text.Trim()), 2);
                    else
                        EditRow["TOTALAMT"] = objRounding.Round(decimal.Zero, 2);

                    EditRow["JOBTYPE"] = cmbRate.SelectedValue;
                    EditRow["ARTICLE"] = cmbArticle.Text;

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                    //  IsEdit = false;
                }

                if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)  // CHECK -- S
                {
                    Decimal dTotalAmount = 0m;
                    foreach(DataRow drTotal in dtItemInfo.Rows)
                    {
                        //  dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["RATE"])) ? Convert.ToDecimal(drTotal["RATE"]) : 0);
                    }
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));
                }

                #region FOR FILLING INGREDIENT DETAILS
                //if (!IsEdit)
                //{
                if(IsEdit)
                {
                    // if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    if(dtSubOrderDetails != null)
                        if(dtSubOrderDetails.Rows.Count > 0)
                        {
                            {
                                //foreach (DataRow dr1 in dtSubOrderDetails.Rows)
                                //{
                                //    if (dr1["UNIQUEID"].ToString().Trim() == sUniqueNo.Trim())
                                //        dr1.Delete();
                                //}
                                foreach(DataRow dr1 in dtSubOrderDetails.Rows)
                                {
                                    if(dr1["UNIQUEID"].ToString().Trim() == sUniqueNo.Trim())
                                    {
                                        dr1.Delete();
                                        dtSubOrderDetails.AcceptChanges();
                                        if(dtSubOrderDetails.Rows.Count == 0)
                                        {
                                            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                                            {
                                                foreach(DataRow drTotal in dtItemInfo.Rows)
                                                {
                                                    //  drTotal["ROWTOTALAMOUNT"] = Math.Round((Convert.ToDecimal(drTotal["AMOUNT"]) + Convert.ToDecimal(drTotal["MAKINGAMOUNT"])), 2);
                                                    drTotal["ROWTOTALAMOUNT"] = Math.Round((Convert.ToDecimal(drTotal["AMOUNT"])), 2);
                                                }
                                                dtItemInfo.AcceptChanges();
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                            dtSubOrderDetails.AcceptChanges();
                        }
                    IsEdit = false;
                }

                //  DataTable dt = GetIngredientDetails(sUniqueNo); // CHECK -- S
                // decimal eAmt = 0;
                // string UID = string.Empty;
                //if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                //{
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        foreach (DataRow drIngredients in dt.Rows)
                //        {
                //            UID = Convert.ToString(drIngredients["UNIQUEID"]);
                //            eAmt += Convert.ToDecimal(drIngredients["AMOUNT"]);
                //            dtSubOrderDetails.ImportRow(drIngredients);
                //        }
                //    }
                //}
                //else
                //{
                //    dtSubOrderDetails = dt;
                //    if (dt != null && dt.Rows.Count > 0)
                //    {
                //        foreach (DataRow drIngredients in dt.Rows)
                //        {
                //            UID = Convert.ToString(drIngredients["UNIQUEID"]);
                //            eAmt += decimal.Round(Convert.ToDecimal(drIngredients["AMOUNT"]), 2, MidpointRounding.AwayFromZero);
                //        }
                //    }
                //}

                #endregion

                //if (eAmt != 0)
                //{
                //    foreach (DataRow dr1 in dtItemInfo.Select("UNIQUEID='" + UID.Trim() + "'"))
                //    {
                //        dr1["EXTENDEDDETAILS"] = eAmt;
                //        dr1["ROWTOTALAMOUNT"] = eAmt + Convert.ToDecimal(dr1["MAKINGAMOUNT"]);
                //        //  dr1["AMOUNT"] = Convert.ToString(dt.Rows[0]["AMOUNT"]);
                //        //  dr1["RATE"] = Convert.ToDecimal(Convert.ToString(dt.Rows[0]["AMOUNT"]));
                //        dr1["AMOUNT"] = eAmt;
                //        dr1["RATE"] = eAmt;
                //        dtItemInfo.AcceptChanges();
                //        break;
                //    }
                //    grItems.DataSource = dtItemInfo.DefaultView;
                //}

                if(dtItemInfo != null && dtItemInfo.Rows.Count > 0) // CHECK -- S
                {
                    Decimal dTotalAmount = 0m;
                    foreach(DataRow drTotal in dtItemInfo.Rows)
                    {
                        //  dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["RATE"])) ? Convert.ToDecimal(drTotal["RATE"]) : 0);
                    }
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));
                }
                //}
                //else
                //{
                //    IsEdit = false;
                //}
                ClearControls();
            }


        }
        #endregion

        #region Extended Details CLick
        private void btnExtendedDetails_Click(object sender, EventArgs e)  // CHECK -- S
        {
            BlankOperations.WinFormsTouch.frmRepairSubDetail objSubOrderdetails = null;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    sExtendedDetailsAmount = 0m;


                    if(dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    {
                        objSubOrderdetails = new BlankOperations.WinFormsTouch.frmRepairSubDetail(dtSubOrderDetails, pos, application, this, Convert.ToString(drRowToUpdate["UNIQUEID"]));
                    }
                    else
                    {
                        dtSubOrderDetails = new DataTable();
                        objSubOrderdetails = new BlankOperations.WinFormsTouch.frmRepairSubDetail(pos, application, this, Convert.ToString(drRowToUpdate["UNIQUEID"]));
                    }
                    objSubOrderdetails.ShowDialog();

                    //  DataTable dtSubRepDtl = new DataTable();

                    #region -- detail amount
                    //  --- applicable for single row in Repair Detail Grid
                    Decimal dDetailRowAmt = 0m;
                    Decimal dTotalAmountRep = 0m;

                    if(dtItemInfo.Rows.Count > 0)
                    {

                        dDetailRowAmt = Convert.ToDecimal(dtItemInfo.Rows[0]["Rate"]);
                    }


                    if(dtSubOrderDetails.Rows.Count > 0)
                    {
                        //Start  : Commented on 22/09/2014 By asking Mr. A.Mitra
                        //foreach (DataRow drTotal in dtSubOrderDetails.Rows)
                        //{
                        //    dTotalAmountRep += Convert.ToDecimal(drTotal["AMOUNT"]);
                        //}

                        //if (dtItemInfo.Rows.Count > 0)
                        //{
                        //    dtItemInfo.Rows[0]["AMOUNT"] = dTotalAmountRep;
                        //    dtItemInfo.Rows[0]["Rate"] = dTotalAmountRep;
                        //    dtItemInfo.AcceptChanges();
                        //}
                        //End :Commented on 22/09/2014 By asking Mr. A.Mitra
                    }
                    else
                    {
                        if(dtItemInfo.Rows.Count > 0)
                        {

                            dtItemInfo.Rows[0]["AMOUNT"] = dDetailRowAmt; // decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                            dtItemInfo.Rows[0]["Rate"] = dDetailRowAmt; // decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);

                            dtItemInfo.AcceptChanges();
                        }

                    }

                    #endregion


                    //if (string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["EXTENDEDDETAILS"])))
                    //{
                    //    drRowToUpdate["EXTENDEDDETAILS"] = sExtendedDetailsAmount;

                    //    drRowToUpdate["ROWTOTALAMOUNT"] = Convert.ToDecimal(drRowToUpdate["EXTENDEDDETAILS"]) + Convert.ToDecimal(drRowToUpdate["AMOUNT"]) + Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);
                    //}
                    //else
                    //{
                    //if (sExtendedDetailsAmount != 0)
                    //{
                    //    drRowToUpdate["AMOUNT"] = sExtendedDetailsAmount;
                    //    drRowToUpdate["RATE"] = sExtendedDetailsAmount;
                    //    drRowToUpdate["ROWTOTALAMOUNT"] = sExtendedDetailsAmount + Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);
                    //}
                    //else
                    //{
                    //    drRowToUpdate["ROWTOTALAMOUNT"] = Convert.ToDecimal(drRowToUpdate["AMOUNT"]) + sExtendedDetailsAmount + Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);
                    //}
                    //drRowToUpdate["EXTENDEDDETAILS"] = sExtendedDetailsAmount;
                    ////  drRowToUpdate["ROWTOTALAMOUNT"] = Convert.ToDecimal(drRowToUpdate["EXTENDEDDETAILS"]) + Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);

                    ////   }
                    dtItemInfo.AcceptChanges();
                    grItems.DataSource = null;
                    grItems.DataSource = dtItemInfo;
                    //if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    //{
                    //    Decimal dTotalAmount = 0m;
                    //    foreach (DataRow drTotal in dtItemInfo.Rows)
                    //    {
                    //        //  dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                    //        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);
                    //    }
                    //    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    //}

                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[2].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];
                    sExtendedDetailsAmount = 0m;
                    objSubOrderdetails = new BlankOperations.WinFormsTouch.frmRepairSubDetail(dsSearchedOrder, pos, application, this, Convert.ToString(drRowToUpdate["LINENUM"]));
                    objSubOrderdetails.ShowDialog();

                }

            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the details or No Ingredients Exists.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
        }
        #endregion

        #region Submmit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //  --- applicable for single row in Repair Detail Grid
            Decimal dDetailRowAmt = 0m;
            Decimal dTotalAmount = 0m;

            if(dtItemInfo.Rows.Count > 0)
            {
                dDetailRowAmt = Convert.ToDecimal(dtItemInfo.Rows[0]["Rate"]);
            }


            if(dtSubOrderDetails.Rows.Count > 0)
            {
                //foreach (DataRow drTotal in dtSubOrderDetails.Rows)
                //{
                //    dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                //}

                //if (dtItemInfo.Rows.Count > 0)
                //{
                //    dtItemInfo.Rows[0]["AMOUNT"] = dTotalAmount;
                //    dtItemInfo.Rows[0]["Rate"] = dTotalAmount;
                //    dtItemInfo.AcceptChanges();
                //}
            }
            else
            {
                if(dtItemInfo.Rows.Count > 0)
                {
                    dtItemInfo.Rows[0]["AMOUNT"] = dDetailRowAmt; // decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    dtItemInfo.Rows[0]["Rate"] = dDetailRowAmt; // decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);

                    dtItemInfo.AcceptChanges();
                }

            }
            //

            frmRepairOrder.dtOrderDetails = dtItemInfo;
            frmRepairOrder.dtSubOrderDetails = dtSubOrderDetails;
            //   frmCustOrder.dtSampleDetails = dtSample; // CHECK -- S
            frmRepairOrder.sOrderDetailsAmount = txtTotalAmount.Text.Trim();
            frmRepairOrder.sSubOrderDetailsAmount = Convert.ToString(sExtendedDetailsAmount);
            frmRepairOrder.dtSketchDetails = dtSketch;
            this.Close();

        }
        #endregion

        #region EDIT CLICK
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

                    cmbRate.SelectedValue = theRowToSelect["JOBTYPE"];

                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                    txtAmount.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                    txtJobDetails.Text = Convert.ToString(theRowToSelect["REPAIRJOBDETAILS"]);
                    //   txtMakingAmount.Text = Convert.ToString(theRowToSelect["MAKINGAMOUNT"]);


                    //if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    //{
                    //    Decimal dTotalAmount = 0m;
                    //    foreach (DataRow drTotal in dtItemInfo.Rows)
                    //    {
                    //        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                    //    }
                    //    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    //}
                    cmbStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                    cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);

                    txtNettWt.Text = Convert.ToString(theRowToSelect["NETTWT"]);
                    txtDiaWt.Text = Convert.ToString(theRowToSelect["DIAWT"]);
                    txtDiaAmt.Text = Convert.ToString(theRowToSelect["DIAAMT"]);
                    txtStnWt.Text = Convert.ToString(theRowToSelect["STNWT"]);
                    txtStnAmt.Text = Convert.ToString(theRowToSelect["STNAMT"]);
                    txtTotalAmt.Text = Convert.ToString(theRowToSelect["TOTALAMT"]);
                }
            }
        }
        #endregion

        #region DELETE CLICK
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
                    if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach(DataRow drTotal in dtItemInfo.Rows)
                        {
                            dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["RATE"])) ? Convert.ToDecimal(drTotal["RATE"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0);
                        }
                        txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }
                }

            }
            if(DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                txtTotalAmount.Text = "0.00";
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;

        }
        #endregion

        #region Sample Click
        //private void btnSample_Click(object sender, EventArgs e)
        //{
        //    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
        //    {
        //        DataTable dt = new DataTable();
        //        SqlConnection conn = new SqlConnection();
        //        conn = application.Settings.Database.Connection;
        //        if (conn.State == ConnectionState.Closed)
        //            conn.Open();
        //        string commandText = string.Empty;
        //        commandText = " select ARTICLE_CODE,[DESCRIPTION] from article_master ";

        //        SqlCommand command = new SqlCommand(commandText, conn);
        //        command.CommandTimeout = 0;
        //        SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
        //        adapter.Fill(dt);
        //        if (conn.State == ConnectionState.Open)
        //            conn.Close();

        //        if (grdView.RowCount > 0)
        //        {
        //            int selectedRow = grdView.GetSelectedRows()[0];
        //            DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
        //            DataTable dtSampleDownload = new DataTable();
        //            if (dtSample != null && dtSample.Rows.Count > 0)
        //            {
        //                foreach (DataRow dtDown in dtSample.Rows)
        //                {
        //                    if (Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(dtDown["UNIQUEID"]))
        //                    {
        //                        dtSampleDownload = dtSample.Clone();
        //                        dtSampleDownload.ImportRow(dtDown);
        //                        dtSampleDownload.AcceptChanges();
        //                        break;
        //                    }
        //                }
        //            }
        //            frmOrderSample oSample = new frmOrderSample(dt, Convert.ToString(drRowToUpdate["UNIQUEID"]), dtSampleDownload, selectedRow + 1);
        //            oSample.ShowDialog();
        //            if (oSample.dtUploadSample != null && oSample.dtUploadSample.Rows.Count > 0)
        //            {
        //                if (dtSample != null && dtSample.Rows.Count > 0)
        //                {
        //                    foreach (DataRow dr in oSample.dtUploadSample.Rows)
        //                    {
        //                        foreach (DataRow drSam in dtSample.Rows)
        //                        {
        //                            if (Convert.ToString(dr["UNIQUEID"]) == Convert.ToString(drSam["UNIQUEID"]))
        //                            {
        //                                drSam.Delete();
        //                                dtSample.AcceptChanges();
        //                                break;
        //                            }
        //                        }
        //                        dtSample.ImportRow(dr);
        //                    }
        //                    dtSample.AcceptChanges();
        //                    oSample.dtUploadSample = new DataTable();
        //                }
        //                else
        //                {
        //                    dtSample = oSample.dtUploadSample;
        //                }
        //            }
        //            else
        //            {
        //                foreach (DataRow drSam in dtSample.Rows)
        //                {
        //                    if (Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(drSam["UNIQUEID"]))
        //                    {
        //                        drSam.Delete();
        //                        dtSample.AcceptChanges();
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[3].Rows.Count > 0)
        //    {
        //        if (grdView.RowCount > 0)
        //        {
        //            int selectedRow = grdView.GetSelectedRows()[0];
        //            DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];
        //            sExtendedDetailsAmount = 0m;
        //            frmOrderSample objSample = new frmOrderSample(dsSearchedOrder.Tables[3], Convert.ToDecimal(drRowToUpdate["LINENUM"]));
        //            //objSample.ShowDialog();

        //        }

        //    }
        //    else
        //    {
        //        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
        //        {
        //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

        //        }
        //    }
        //}
        #endregion

        #region CLear Click
        private void btnClear_Click(object sender, EventArgs e)
        {
            saleLineItem = new SaleLineItem();
            objRounding = new Rounding();
            dtItemInfo = new DataTable();
            dtSubOrderDetails = new DataTable();
            sExtendedDetailsAmount = 0m;
            randUnique = new Random();
            IsEdit = false;
            EditselectedIndex = 0;
            dsSearchedOrder = new DataSet();
            grItems.DataSource = null;
            ClearControls();
            txtTotalAmount.Text = string.Empty;
        }
        #endregion

        #region Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ValidateControls()
        bool ValidateControls()
        {

            if((txtItemId.Text.ToUpper().Trim() == "ITEM ID") || (string.IsNullOrEmpty(txtItemId.Text.Trim())))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if((txtItemName.Text.ToUpper().Trim() == "ITEM NAME") || (string.IsNullOrEmpty(txtItemName.Text.Trim())))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(IsCatchWtItem(txtItemId.Text.Trim()))
            {
                if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtPCS.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
                if(Convert.ToDecimal(txtPCS.Text) <= 0)
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be zero", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        txtPCS.Focus();
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
            }
            else
            {
                txtPCS.Enabled = false;
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
            else if(Convert.ToDecimal(txtQuantity.Text.Trim()) == 0m)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(string.IsNullOrEmpty(txtNettWt.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net Wt can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if(Convert.ToDecimal(txtNettWt.Text.Trim()) == 0m)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net Wt can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if(Convert.ToDecimal(txtNettWt.Text.Trim()) < 0m)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net Wt can not be negetive", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(Convert.ToDecimal(txtNettWt.Text.Trim()) > Convert.ToDecimal(txtQuantity.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net Wt can not be greater than quantity", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtNettWt.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(cmbRate.SelectedValue.ToString() == "0")
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Select job type.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    cmbRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Rate can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(!string.IsNullOrEmpty(txtRate.Text.Trim()) && Convert.ToDecimal(txtRate.Text.Trim()) == 0m)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be Zero.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtRate.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(cmbArticle.SelectedIndex == 0)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select article.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    cmbArticle.Focus();
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

        #region ClearControls
        private void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            cmbCode.Text = string.Empty;
            cmbStyle.Text = string.Empty;
            cmbSize.Text = string.Empty;
            cmbConfig.Text = string.Empty;
            txtPCS.Text = string.Empty;
            txtGrossWt.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtRate.Text = string.Empty;
            //  cmbRateType.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtJobDetails.Text = string.Empty;

            txtNettWt.Text = string.Empty;
            txtDiaWt.Text = string.Empty;
            txtDiaAmt.Text = string.Empty;
            txtStnWt.Text = string.Empty;
            txtStnAmt.Text = string.Empty;
            txtTotalAmt.Text = string.Empty;
            lblArticleDesc.Text = "";
            cmbArticle.SelectedIndex = 0;
            cmbRate.SelectedIndex = 0;
        }
        #endregion

        #region Key Press Events

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        #endregion

        #region  NumPad()
        void NumPad()
        {
            if(obj != null)
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
            obj.Location = new System.Drawing.Point(660, 67);
            obj.LookAndFeel.SkinName = "The Asphalt World";
            obj.LookAndFeel.UseDefaultLookAndFeel = false;
            obj.MaskChar = null;
            obj.MaskInterval = 0;
            obj.MaxNumberOfDigits = 13;
            obj.MinimumSize = new System.Drawing.Size(170, 242);
            obj.Name = "numPad1";
            obj.NegativeMode = false;
            obj.NoOfTries = 0;
            obj.NumberOfDecimals = 3;
            obj.PromptText = "Enter Details: ";
            obj.ShortcutKeysActive = false;
            obj.Size = new System.Drawing.Size(170, 242);
            obj.TabIndex = 14;
            obj.TimerEnabled = false;
            obj.Visible = true;

            numPad1.Visible = false;

            obj.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(obj_EnterButtonPressed);
        }
        #endregion

        #region NUMPAD ENTER PRESSED
        private void numPad1_EnterButtonPressed()
        {
            //  txtPCS.Text = numPad1.EnteredValue;

            if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                txtPCS.Text = numPad1.EnteredValue;

                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                txtQuantity.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtRate.Text = numPad1.EnteredValue;
                numPad1.Refresh();
            }

        }
        #endregion

        #region obj_EnterButtonPressed
        private void obj_EnterButtonPressed()
        {
            if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                txtQuantity.Text = obj.EnteredValue;
                numPad1.Refresh();
            }
            else if(string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtRate.Text = obj.EnteredValue;
                numPad1.Refresh();
            }


        }
        #endregion

        #region Get Quantity
        public string GetStandardQuantityFromDB(SqlConnection conn, string itemid)
        {
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT STDQTY FROM INVENTTABLE WHERE ITEMID='" + itemid + "'";

            // CHANGES TO BE DONE  : JOIN WITH INVENT TABLE TO PICK THE PARENT ITEM ID IN ABOVE QUERY 

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sQty = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return sQty;

        }
        #endregion

        #region //Nimbus by MIAM @ 03Jul14 : added
        private void cmbRate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbRate.SelectedIndex > 0)
            {
                DataTable dtJobType = (DataTable)cmbRate.DataSource;
                DataRow[] row = dtJobType.Select(string.Format("CODE='{0}'", cmbRate.SelectedValue));
                if(row.Count() > 0)
                {
                    txtRate.Text = row[0]["CHARGE"].ToString();
                }
            }
            else
            {
                txtRate.Text = "0.00";
            }
        }
        #endregion

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                DataTable dt = new DataTable();

                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    DataTable dtSketchDownload = new DataTable();
                    if(dtSketch != null && dtSketch.Rows.Count > 0)
                    {
                        foreach(DataRow dtDown in dtSketch.Rows)
                        {
                            if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(dtDown["UNIQUEID"]))
                            {
                                dtSketchDownload = dtSketch.Clone();
                                dtSketchDownload.ImportRow(dtDown);
                                dtSketchDownload.AcceptChanges();
                                break;
                            }
                        }
                    }
                    frmOrderSketch oSketch = new frmOrderSketch(dt, Convert.ToString(drRowToUpdate["UNIQUEID"]), dtSketchDownload, selectedRow + 1);

                    //oSketch.lblBreadCrumbs.Text = lblBreadCrumbs.Text + ">" + " line no:" + " " + Convert.ToInt16(selectedRow + 1);
                    oSketch.ShowDialog();
                    if(oSketch.dtUploadSketch != null && oSketch.dtUploadSketch.Rows.Count > 0)
                    {
                        if(dtSketch != null && dtSketch.Rows.Count > 0)
                        {
                            foreach(DataRow dr in oSketch.dtUploadSketch.Rows)
                            {
                                foreach(DataRow drSk in dtSketch.Rows)
                                {
                                    if(Convert.ToString(dr["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                                    {
                                        drSk.Delete();
                                        dtSketch.AcceptChanges();
                                        break;
                                    }
                                }
                                dtSketch.ImportRow(dr);
                            }
                            dtSketch.AcceptChanges();
                            oSketch.dtUploadSketch = new DataTable();
                        }
                        else
                        {
                            dtSketch = oSketch.dtUploadSketch;
                        }
                    }
                    else
                    {
                        foreach(DataRow drSk in dtSketch.Rows)
                        {
                            if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                            {
                                drSk.Delete();
                                dtSketch.AcceptChanges();
                                break;
                            }
                        }
                    }
                }
            }
            else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[1].Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];

                    frmOrderSketch objSketch = new frmOrderSketch(dsSearchedOrder.Tables[1], Convert.ToDecimal(drRowToUpdate["LINENUM"]), "");
                }
            }
            else
            {
                MessageBox.Show("Please enter at least one row to upload the sketch details or " +
                "there are no sketch present for the selected item");
            }
        }

        private void cmbArticle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbArticle.SelectedIndex > 0)
            {
                DataTable dtArticle = (DataTable)cmbArticle.DataSource;
                DataRow[] row = dtArticle.Select(string.Format("CODE='{0}'", cmbArticle.SelectedValue));
                if(row.Count() > 0)
                {
                    lblArticleDesc.Text = row[0]["CODE"].ToString();
                }
            }
            else
            {
                lblArticleDesc.Text = "";
            }
        }

        private void getNetWt()
        {
            decimal dNetWt = 0m;
            decimal dGrossQty = 0m;
            decimal dDmdWt = 0m;
            decimal dStnWt = 0m;

            if(!string.IsNullOrEmpty(txtGrossWt.Text))
            {
                dGrossQty = Convert.ToDecimal(txtGrossWt.Text);
            }
            if(!string.IsNullOrEmpty(txtDiaWt.Text))
            {
                dDmdWt = Convert.ToDecimal(txtDiaWt.Text);
            }
            if(!string.IsNullOrEmpty(txtStnWt.Text))
            {
                dStnWt = Convert.ToDecimal(txtStnWt.Text);
            }

            dNetWt = dGrossQty - ((dDmdWt/5) + dStnWt);
            if(dNetWt < 0)
            {
                MessageBox.Show("Invaild net wt.");
            }
            txtNettWt.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dNetWt), 3, MidpointRounding.AwayFromZero));
              

        }

        internal bool IsNumeric(string ValueInNumeric)
        {
            bool IsFine = true;
            foreach (char ch in ValueInNumeric)
            {
                if (!(ch >= '0' && ch <= '9'))
                {
                    IsFine = false;
                }
            }
            return IsFine;
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                string s = txtQuantity.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtQuantity.Focus();
                    }
                    else
                    {
                        getNetWt();
                    }
                }
                else
                {
                    getNetWt();
                }
            }

            
        }

        private void txtDiaWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDiaWt.Text))
            {
                string s = txtDiaWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtDiaWt.Focus();
                    }
                    else
                    {
                        getNetWt();
                    }
                }
                else
                {
                    getNetWt();
                }
            }
        }

        private void txtStnWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStnWt.Text))
            {
                string s = txtStnWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtStnWt.Focus();
                    }
                    else
                    {
                        getNetWt();
                    }
                }
                else
                {
                    getNetWt();
                }
            }
        }

        private void txtStnAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtRate_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRate.Text))
            {
                string s = txtRate.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 2)
                    {
                        MessageBox.Show("Maximum decimal places is two.");
                        txtRate.Focus();
                    }
                }
            }
        }

        private void txtStnAmt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStnAmt.Text))
            {
                string s = txtStnAmt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 2)
                    {
                        MessageBox.Show("Maximum decimal places is two.");
                        txtStnAmt.Focus();
                    }
                }
            }
        }

        private void txtDiaAmt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDiaAmt.Text))
            {
                string s = txtDiaAmt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 2)
                    {
                        MessageBox.Show("Maximum decimal places is two.");
                        txtDiaAmt.Focus();
                    }
                }
            }
        }

        private void txtNettWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNettWt.Text))
            {
                string s = txtNettWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtNettWt.Focus();
                    }
                }
            }
        }

        private void txtTotalAmt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTotalAmt.Text))
            {
                string s = txtTotalAmt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 2)
                    {
                        MessageBox.Show("Maximum decimal places is two.");
                        txtTotalAmt.Focus();
                    }
                }
            }
        }

        private void txtGrossWt_KeyPress(object sender, KeyPressEventArgs e)
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
    }
}
