using System;
using System.Data;
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
using LSRetailPosis.Settings.FunctionalityProfiles;
using System.Collections.ObjectModel;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmOrderDetails : frmTouchBase
    {
        #region Variable Declaration
        bool isMkAdded = false;
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;
        DataTable dtItemInfo = new DataTable("dtItemInfo");
        public DataTable dtSubOrderDetails = new DataTable("dtSubOrderDetails");
        public decimal sExtendedDetailsAmount = 0m;
        frmCustomerOrder frmCustOrder;
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
        decimal dStoneWtRange = 0m;

        decimal dIngrdMetalWtRange = 0m;
        decimal dIngrdTotalGoldQty = 0m;
        decimal dIngrdTotalGoldValue = 0m;
        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;
        Decimal dWMetalRate = 0m;
        decimal dWastQty = 0m; // Added for wastage
        //
        string sBookedSKUItem = string.Empty;
        public DataTable dtSketch = new DataTable();
        public DataTable dtSampleSketch = new DataTable();

        public DataTable dtStone = new DataTable();
        public DataTable dtPaySchedule = new DataTable();
        public DataTable dtCheque = new DataTable();

        private bool bIsGrossMetalCal = false;
        bool isMRPExists = false;
        int iDecPre = 0;
        bool bIsSKUItem = false;
        string sActualItemId = "";
        int iOrderStoreDeliveryDays = 0;
        int iChkSuvarnaVrudhi = 0;
        string sStorGrp = "";
        string sFestiveCode = "";
        decimal dDiscount = 0m;
        decimal dAdvancePct = 0m;
        int iRoundOff = 0;
        string sAgArticl = "";
        string sAgProd = "";
        string sAgItemId = "";
        string sAgCollection = "";

        DateTime dtDeliveryDate = new DateTime();
        decimal dMetalQty = 0m;
        decimal dGoldRate = 0m;
        decimal dSilverRate = 0m;
        decimal dPlatinumRate = 0m;

        int iGMA = 0;
        string sGMAPurity = "";
        string sGMAItem = "";
        decimal dGMAMinQty = 0m;

        enum CRWMakingCalcType
        {
            Net = 1,
            Gross = 2,
            NetChargeable = 3,
            Blank = 4,
        }

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
            // Inch = 5,
            Gross = 6,
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
        }

        #endregion

        #region Wastage Type

        enum WastageType
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }

        #endregion

        #endregion

        #region InventDimId
        public string GetInventID(string distinctProductVariantID)
        {
            string commandText = "select top(1)  INVENTDIMID from assortedinventdimcombination WHERE DISTINCTPRODUCTVARIANT='" + distinctProductVariantID + "'";

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            return Convert.ToString(command.ExecuteScalar());

        }
        #endregion

        #region frmOrderDetails
        public frmOrderDetails(IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();

            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            btnPOSItemSearch.Focus();
            //btnSample.Visible = true;
            iOrderStoreDeliveryDays = getOrderStoreDeliveryDays();
            iChkSuvarnaVrudhi = Convert.ToInt16(frmCustOrder.chkSUVARNAVRUDDHI.Checked);

            iGMA = Convert.ToInt16(frmCustOrder.chkGMA.Checked);
            //sGMAPurity = Convert.ToString(frmCustOrder.sGMAPurity);
            sGMAItem = Convert.ToString(frmCustOrder.sGMAItem);
            dGMAMinQty = Convert.ToDecimal(frmCustOrder.dGMAMinQty);


            if (iChkSuvarnaVrudhi == 1 || iGMA == 1)
            {
                ControlDisable();
            }

           
            //btnReBookSKU.Visible = false;
        }

        private void ControlDisable()
        {
            btnCodeS.Enabled = false;
            btnSizeS.Enabled = false;
            btnStyleS.Enabled = false;
            btnArticle.Enabled = false;
            btnCollection.Enabled = false;
            btnProduct.Enabled = false;
            btnConfig.Enabled = false;
            txtRate.Enabled = false;
            cmbRateType.Enabled = false;
            txtMakingRate.Enabled = false;
            cmbMakingType.Enabled = false;
        }

        public frmOrderDetails(DataTable dtSampleDetails, DataTable dtSubOrder, DataTable dtOrderDetails, DataTable dtPaySch, DataTable dtChqInfo, IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();

            dtItemInfo = dtOrderDetails;
            dtSubOrderDetails = dtSubOrder;
            dtSample = dtSampleDetails;
            dtPaySchedule = dtPaySch;
            dtCheque = dtChqInfo;

            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            btnPOSItemSearch.Focus();
            grItems.DataSource = dtItemInfo.DefaultView;
            iChkSuvarnaVrudhi = Convert.ToInt16(frmCustOrder.chkSUVARNAVRUDDHI.Checked);
            //btnReBookSKU.Visible = false;
            iGMA = Convert.ToInt16(frmCustOrder.chkGMA.Checked);
            // sGMAPurity = Convert.ToString(frmCustOrder.sGMAPurity);
            sGMAItem = Convert.ToString(frmCustOrder.sGMAItem);
            dGMAMinQty = Convert.ToDecimal(frmCustOrder.dGMAMinQty);


            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dtItemInfo.Rows)
                {
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) +  (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                    + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
                }
                txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
            }

            iOrderStoreDeliveryDays = getOrderStoreDeliveryDays();

            ControlDisable();
        }

        public frmOrderDetails(DataSet dsSearchedDetails, IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();

            dsSearchedOrder = dsSearchedDetails;
            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            btnPOSItemSearch.Focus();
            grItems.DataSource = dsSearchedDetails.Tables[1].DefaultView;

            if (dsSearchedDetails.Tables[1] != null && dsSearchedDetails.Tables[1].Rows.Count > 0)
            {
                Decimal dTotalAmount = 0m;
                foreach (DataRow drTotal in dsSearchedDetails.Tables[1].Rows)
                {
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0); //COs  Modification
                    dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                    + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0);
                }
                txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
            }
            panel1.Enabled = false;
            btnSubmit.Enabled = false;
            btnEdit.Enabled = false;
            btnClear.Enabled = false;
            btnDelete.Enabled = false;
            btnPOSItemSearch.Enabled = false;
            btnAddItem.Enabled = false;
            btnReBookSKU.Visible = true;

            iChkSuvarnaVrudhi = Convert.ToInt16(frmCustOrder.chkSUVARNAVRUDDHI.Checked);

            iGMA = Convert.ToInt16(frmCustOrder.chkGMA.Checked);
            // sGMAPurity = Convert.ToString(frmCustOrder.sGMAPurity);
            sGMAItem = Convert.ToString(frmCustOrder.sGMAItem);
            dGMAMinQty = Convert.ToDecimal(frmCustOrder.dGMAMinQty);

            if (dsSearchedDetails.Tables.Count > 4)
            {
                frmCustOrder.dtSampleDetails = dsSearchedDetails.Tables[3];
            }

            dtSample = frmCustOrder.dtSampleDetails;
            dtSampleSketch = frmCustOrder.dtSampleSketch;

            if (dtSample.Rows.Count > 0)
                btnSampleReturn.Enabled = true;

            iOrderStoreDeliveryDays = getOrderStoreDeliveryDays();
            ControlDisable();
        }
        #endregion

        #region Bind Rate Type and making Type Combo
        void BindRateTypeMakingTypeCombo()
        {
            cmbRateType.DataSource = Enum.GetValues(typeof(RateType));
            cmbMakingType.DataSource = Enum.GetValues(typeof(MakingType));
        }
        #endregion

        #region Bind Wastage Combo

        void BindWastage()
        {
            cmbWastage.DataSource = Enum.GetValues(typeof(WastageType));
        }

        #endregion

        private string ColorSizeStyleConfig()
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig;
            if (!string.IsNullOrEmpty(cmbCode.Text))
                colorSizeStyleConfig = new StringBuilder("Color : " + cmbCode.Text);
            else
                colorSizeStyleConfig = new StringBuilder(cmbCode.Text);

            if (!string.IsNullOrEmpty(cmbSize.Text))
            {
                if (colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Size : " + cmbSize.Text);
            }

            if (!string.IsNullOrEmpty(cmbStyle.Text))
            {
                if (colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Style : " + cmbStyle.Text);
            }

            if (!string.IsNullOrEmpty(cmbConfig.Text))
            {
                if (colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(" Configuration : " + cmbConfig.Text);
            }

            return colorSizeStyleConfig.ToString();
        }

        #region POS-ITEM-SEARCH-CLICK
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
            //  objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) "); // blocked on 12.09.2013 // SKU allow

            // objdialog.MyItemSearch(500, ref str, out  dsItem, "");
            //if (iChkSuvarnaVrudhi == 1)
            //    objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) ");
            //else
            //    objdialog.MyItemSearch(500, ref str, out  dsItem);

            if (iChkSuvarnaVrudhi == 1)
                objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) ");
            else if (iGMA == 1)
                objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID ='" + sGMAItem + "'");
            else
                objdialog.MyItemSearch(500, ref str, out  dsItem);


            saleLineItem = new SaleLineItem();

            if (dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);

                    Item objItem = new Item();
                    objItem.MYProcessItem(saleLineItem, application);

                    Dimension objDim = new Dimension();
                    DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);

                    if (dtDimension != null && dtDimension.Rows.Count > 0)
                    {
                        DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                        dimConfirmation.InventDimCombination = dtDimension;
                        dimConfirmation.DimensionData = saleLineItem.Dimension;

                        if (!IsSKUItem(saleLineItem.ItemId))
                        {
                            bIsSKUItem = false;
                            frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                            objfrmDim.ShowDialog();

                            if (objfrmDim.SelectDimCombination != null)
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

                                DimensionControll();

                                cmbConfig.Enabled = false;
                                cmbCode.Enabled = false;
                                cmbSize.Enabled = false;
                                cmbStyle.Enabled = false;

                                Previewdimensions = ColorSizeStyleConfig();
                            }
                        }
                        else
                        {
                            bIsSKUItem = true;
                            cmbSize.Text = dtDimension.Rows[0]["sizeid"].ToString();
                            cmbConfig.Text = dtDimension.Rows[0]["configid"].ToString();

                            DimensionControll();
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
                    SqlConnection conn = new SqlConnection();
                    if (application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;
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


                    string sQty = string.Empty;
                    sQty = GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                    if (!string.IsNullOrEmpty(sQty))
                    {
                        sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    }


                    txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);


                    if (!string.IsNullOrEmpty(txtRate.Text))
                        cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Weight");



                    unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                    lblUnit.Text = unitid;

                    iDecPre = GetDecimalPrecison(unitid);
                    //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();

                    if (Convert.ToString(txtItemId.Text.Trim()) == string.Empty
                        || Convert.ToString(cmbConfig.Text.Trim()) == string.Empty)
                    {
                        sBaseItemID = string.Empty;
                        sBaseConfigID = string.Empty;
                    }
                    else
                    {
                        sBaseItemID = txtItemId.Text.Trim();
                        sBaseConfigID = cmbConfig.Text;
                    }
                    if (IsSKUItem(txtItemId.Text.Trim()))
                    {
                        //chkBookedSKU.Enabled = true;
                        //txtPCS.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtRate.Enabled = false;
                        txtMakingRate.Enabled = false;
                        cmbMakingType.Enabled = false;
                        cmbRateType.Enabled = false;
                        sActualItemId = getActualItemId(txtItemId.Text.Trim());
                    }
                    else
                    {
                        //chkBookedSKU.Enabled = false;
                        //txtPCS.Enabled = true;
                        txtQuantity.Enabled = true;
                        txtRate.Enabled = false;
                        txtMakingRate.Enabled = true;
                        //cmbMakingType.Enabled = true;
                        //cmbRateType.Enabled = true;

                        if (iGMA == 0)
                        {
                            txtMakingRate.Enabled = true;
                            cmbMakingType.Enabled = true;
                        }

                        if (isCustOrderItem(txtItemId.Text.Trim()))
                        {
                            btnCodeS.Enabled = true;
                            btnStyleS.Enabled = true;
                            btnConfig.Enabled = true;
                            btnProduct.Enabled = true;
                            btnSizeS.Enabled = true;
                            sActualItemId = "";
                        }
                        else
                        {
                            sActualItemId = txtItemId.Text.Trim();
                            DimensionControll();
                        }
                    }

                    isItemExists = true;
                }
            }
            else
            {
                isItemExists = false;
            }

            if (IsMetalRateCalcTypeGross(sBaseItemID))
                bIsGrossMetalCal = true;
            else
                bIsGrossMetalCal = false;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            isMRPExists = isMRP(sBaseItemID, connection);

            string sProduct = "";
            string sArticle = "";
            string sCollection = "";

            GetItemInfo(sBaseItemID, ref sProduct, ref sArticle, ref sCollection);

            if (!string.IsNullOrEmpty(sCollection))
                cmbCollection.Text = sCollection;
            else
                cmbCollection.Text = "";

            if (!string.IsNullOrEmpty(sArticle))
                cmbArticle.Text = sArticle;
            else
                cmbArticle.Text = "";

            if (!string.IsNullOrEmpty(sProduct))
                cmbProductType.Text = sProduct;
            else
                cmbProductType.Text = "";

            if (iChkSuvarnaVrudhi == 1)
            {
                sStorGrp = getStoreFormatCode();

                GetSuvarnaVrudhiDiscount(frmCustOrder.cmbSVSchemeCode.Text.Trim(), ref dDiscount, ref dAdvancePct, ref iRoundOff, ref sAgArticl, ref sAgProd, ref sAgItemId, ref sAgCollection);

                if (!string.IsNullOrEmpty(sAgArticl))
                {
                    if (sArticle != sAgArticl)
                    {
                        MessageBox.Show("Invalid Item Selected");
                        ClearControls();
                    }
                }
                if (!string.IsNullOrEmpty(sAgProd))
                {
                    if (sProduct != sAgProd)
                    {
                        MessageBox.Show("Invalid Item Selected");
                        ClearControls();
                    }
                }
                if (!string.IsNullOrEmpty(sAgItemId))
                {
                    if (sBaseItemID != sAgItemId)
                    {
                        MessageBox.Show("Invalid Item Selected");
                        ClearControls();
                    }
                }
                if (!string.IsNullOrEmpty(sAgCollection))
                {
                    if (sCollection != sAgCollection)
                    {
                        MessageBox.Show("Invalid Item Selected");
                        ClearControls();
                    }
                }

                // GetSuvarnaVrudhiDiscount(txtItemId.Text.Trim(), sStorGrp, ref sFestiveCode, ref dDiscount, ref dAdvancePct, ref iRoundOff, ref dtDeliveryDate);
            }

            if (isMRPExists)
            {
                decimal dAmt = GetMRPPrice(saleLineItem.ItemId, connection);
                txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmt), 2, MidpointRounding.AwayFromZero)); //Convert.ToString(dAmt);
            }
            else if (iChkSuvarnaVrudhi == 1)
            {
                decimal dRate = 0m;
                if (!string.IsNullOrEmpty(txtRate.Text))
                    dRate = Convert.ToDecimal(txtRate.Text);

                if (dDiscount > 0)
                    dRate = dRate - (dRate * dDiscount / 100);

                txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dRate), iRoundOff, MidpointRounding.AwayFromZero));
            }

            if (!isMRPExists)
            {
                CheckMakingRateFromDB(connection, pos.StoreId, sBaseItemID, Convert.ToString(cmbProductType.Text)
                    , Convert.ToString(cmbCollection.Text), Convert.ToString(cmbArticle.Text));
            }
            else
                cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");

            // btnAddItem_Click(sender, e);

            if (bIsSKUItem)
            {
                if (IsBookedItem(sBaseItemID))
                {
                    chkBookedSKU.Checked = false;
                    chkBookedSKU.Enabled = false;
                }
                else
                    chkBookedSKU.Enabled = true;
            }

            if (iGMA == 1)
            {
                txtMakingRate.Text = "";
                txtMakingRate.Enabled = false;
                //if (sGMAPurity != cmbConfig.Text)
                //{
                //    MessageBox.Show("Invalid Configuration Selected");
                //    ClearControls();
                //}
            }

        }

        private decimal getConvertedGoldQty(string sTransConfigid, decimal dQtyToConvert)//, bool istranstofixing
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,10)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);
            //if(istranstofixing)
            //    commandText.Append("SET @istranstofixing = 'Y'");
            //else
            //    commandText.Append("SET @istranstofixing = 'N'");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Terminal.StoreId.Trim() + "'  "); //INVENTPARAMETERS
            commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            //commandText.Append(" IF(@FIXINGCONFIGRATIO >= @TRANSCONFIGRATIO) BEGIN  IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN  SELECT ISNULL((( @TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            ////commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");
            //commandText.Append(" ELSE BEGIN SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            //commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");

            commandText.Append(" IF(@FIXINGCONFIGRATIO >= @TRANSCONFIGRATIO) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL((( @TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY ");
            commandText.Append(" END  ");
            commandText.Append(" else");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            commandText.Append(" END ");
            commandText.Append(" END");
            commandText.Append(" ELSE");
            commandText.Append(" BEGIN	");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   ");
            commandText.Append(" END  ");
            commandText.Append(" ELSE");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            commandText.Append(" END ");
            commandText.Append(" END  ");

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

        }

        private string getActualItemId(string pItem)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT isnull(ITEMIDPARENT,'') ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + pItem + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private bool isCustOrderItem(string pItem)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) isnull(CRWCUSTORDITEM,0) FROM [INVENTTABLE] WHERE ITEMID='" + pItem + "' ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }
        #endregion

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

        private bool isMRP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private void GetItemInfo(string sItemId, ref string sProdTye, ref string sArtCode, ref string sCollectionCode)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT  TOP (1) isnull(PRODUCTTYPECODE,'') PRODUCTTYPE,isnull(ARTICLE_CODE,'') ARTICLE, " +
                            " isnull(COLLECTIONCODE,'') COLLECTIONCODE FROM   INVENTTABLE  " +
                            " WHERE ITEMID='" + sItemId + "'";


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sProdTye = Convert.ToString(reader.GetValue(0));
                    sArtCode = Convert.ToString(reader.GetValue(1));
                    sCollectionCode = Convert.ToString(reader.GetValue(2));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

        }

        #region GetIngredientDetails
        public DataTable GetIngredientDetails(string sUnique, bool IsSKUItem = false)
        {
            DataTable dt = new DataTable();
            DataTable dtClone = new DataTable();
            string commandText = string.Empty;

            decimal dPcs = 0;

            if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                dPcs = Convert.ToDecimal(txtPCS.Text.Trim());

            if (IsSKUItem)
            {
                commandText = "SELECT " + sUnique + " AS UNIQUEID, A.ItemID ITEMID," +
                              " (SELECT     ISNULL(TR.NAME, I.ITEMNAME) AS Expr1 FROM ASSORTEDINVENTITEMS AS I INNER JOIN " +
                              " ECORESPRODUCT AS PR ON PR.RECID = I.PRODUCT LEFT OUTER JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT  " +
                              "  AND TR.LANGUAGEID = '" + ApplicationSettings.Terminal.CultureName + "' WHERE (I.ITEMID = A.ITEMID) AND " +
                              " (I.DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') AND (I.STORERECID = '" + ApplicationSettings.Terminal.StorePrimaryId + "')) AS ITEMNAME, " +
                              " INVENTDIM.CONFIGID AS CONFIGURATION,INVENTDIM.INVENTCOLORID AS COLOR, INVENTDIM.INVENTSIZEID AS SIZE,INVENTDIM.INVENTSTYLEID AS STYLE, " +
                              " CAST(ISNULL(A.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(A.QTY,0) AS NUMERIC(16,3)) AS QUANTITY," +
                              " INVENTDIM.INVENTBATCHID, 0.00 AS RATE, 0 AS RATETYPE, 0.00 AS AMOUNT ,INVENTDIM.INVENTDIMID,A.UNITID,ISNULL(X.METALTYPE,0) AS METALTYPE " +
                              " FROM         SKULine_Posted A INNER JOIN " +
                              " INVENTDIM ON A.InventDimID = INVENTDIM.INVENTDIMID" +
                              " INNER JOIN INVENTTABLE X ON A.ITEMID = X.ITEMID" +
                              " WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'" +
                              " AND  A.[SkuNumber] = '" + txtItemId.Text.Trim() + "' ORDER BY X.METALTYPE ";
            }
            else
            {
                commandText = " DECLARE @BOMID VARCHAR(20) " +
                                    " DECLARE @QUANTITY NUMERIC(36,18) " +
                                    " DECLARE @PCS NUMERIC(36,18) " +
                                    " DECLARE @ITEMNAME VARCHAR(100) " +
                                    " SELECT @BOMID=BOMID FROM BOMVERSION WHERE ITEMID='" + txtItemId.Text.Trim() + "' AND" +
                                    " ACTIVE=1 AND APPROVED=1 AND DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' " +
                                    " and INVENTDIMID in(select INVENTDIMID FROM INVENTDIM WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'" +
                                    " and CONFIGID='" + cmbConfig.Text.Trim() + "' and INVENTSIZEID='" + cmbSize.Text.Trim() + "')" +
                                    " SET @PCS = " + dPcs + "; " +
                                    " SET @QUANTITY = " + Convert.ToDecimal(txtQuantity.Text.Trim()) + "; " +
                                    " SELECT " + sUnique + " AS UNIQUEID, BOM.ITEMID,  " +
                                    " (SELECT     ISNULL(TR.NAME, I.ITEMNAME) AS Expr1 FROM ASSORTEDINVENTITEMS AS I INNER JOIN  " +
                                    " ECORESPRODUCT AS PR ON PR.RECID = I.PRODUCT LEFT OUTER JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT  " +
                                    "  AND TR.LANGUAGEID = '" + ApplicationSettings.Terminal.CultureName + "' WHERE (I.ITEMID = BOM.ITEMID) AND " +
                                    " (I.DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') AND (I.STORERECID = '" + ApplicationSettings.Terminal.StorePrimaryId + "')) AS ITEMNAME, " +
                                    " INVENTDIM.CONFIGID AS CONFIGURATION,INVENTDIM.INVENTCOLORID AS COLOR, INVENTDIM.INVENTSIZEID AS SIZE,  " +
                                    " INVENTDIM.INVENTSTYLEID AS STYLE, CAST(ROUND(((BOM.PDSCWQTY/BOM.BOMQTYSERIE) * @QUANTITY),0) AS INT) AS PCS, CAST(((BOM.BOMQTY/BOM.BOMQTYSERIE) * @QUANTITY) AS NUMERIC(16,3)) AS QUANTITY, " +
                                    " INVENTDIM.INVENTBATCHID, 0.00 AS RATE, 0 AS RATETYPE, 0.00 AS AMOUNT ,INVENTDIM.INVENTDIMID " +
                                    " ,BOM.UNITID, ISNULL(X.METALTYPE,0) AS METALTYPE " + //COs  Modification
                                    " FROM  BOM INNER JOIN INVENTDIM ON BOM.INVENTDIMID = INVENTDIM.INVENTDIMID " +
                                    "  INNER JOIN INVENTTABLE X ON BOM.ITEMID = X.ITEMID" + //COs  Modification
                                    " WHERE     (BOM.BOMID = @BOMID) AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') " +
                                    " ORDER BY X.METALTYPE";
            }


            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                SqlCommand command = new SqlCommand(commandText, connection);
                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                    dtClone = dt.Clone();
                    dtClone.Columns["UNIQUEID"].DataType = typeof(double);
                    dtClone.Columns["RATETYPE"].DataType = typeof(string);
                    dtClone.Columns["PCS"].ReadOnly = false;
                    dtClone.Columns["QUANTITY"].ReadOnly = false;
                    dtClone.Columns["RATE"].ReadOnly = false;
                    dtClone.Columns["RATETYPE"].ReadOnly = false;

                    dtClone.Columns["AMOUNT"].ReadOnly = false;

                    foreach (DataRow dr in dt.Rows)
                    {
                        dtClone.ImportRow(dr);
                    }
                    foreach (DataRow dr in dtClone.Rows)
                    {
                        string sRate = string.Empty;
                        string sCalcType = "";

                        if (Convert.ToDecimal(dr["PCS"]) > 0)
                            dStoneWtRange = Convert.ToDecimal(dr["QUANTITY"]) / Convert.ToDecimal(dr["PCS"]);
                        else
                            dStoneWtRange = Convert.ToDecimal(dr["QUANTITY"]);
                        sRate = getRateFromMetalTable(Convert.ToString(dr["ITEMID"]), Convert.ToString(dr["CONFIGURATION"]), Convert.ToString(dr["INVENTBATCHID"]),
                                                      Convert.ToString(dr["COLOR"]), Convert.ToString(dr["SIZE"]), Convert.ToString(dr["QUANTITY"]), Convert.ToString(dr["PCS"]));

                        if (!string.IsNullOrEmpty(sRate))
                        {
                            sCalcType = GetIngredientCalcType(Convert.ToString(dr["ITEMID"]), Convert.ToString(dr["CONFIGURATION"]), Convert.ToString(dr["INVENTBATCHID"]),
                                                      Convert.ToString(dr["COLOR"]), Convert.ToString(dr["SIZE"]), Convert.ToString(dr["QUANTITY"]), Convert.ToString(dr["PCS"]));   // added 

                            if (iChkSuvarnaVrudhi == 1)
                            {
                                decimal dRate = 0m;
                                if (dDiscount > 0)
                                    dRate = Convert.ToDecimal(sRate) - (Convert.ToDecimal(sRate) * dDiscount / 100);

                                sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(dRate), iRoundOff, MidpointRounding.AwayFromZero));
                            }
                            dr["RATE"] = sRate;

                        }
                        if (!string.IsNullOrEmpty(sCalcType))
                        {
                            //  dr["RATETYPE"] = sCalcType; 
                            if (Convert.ToInt32(sCalcType) == (int)RateType.Weight)
                                dr["RATETYPE"] = Convert.ToString(RateType.Weight);
                            else if (Convert.ToInt32(sCalcType) == (int)RateType.Pcs)
                                dr["RATETYPE"] = Convert.ToString(RateType.Pcs);
                            else
                                dr["RATETYPE"] = Convert.ToString(RateType.Tot);

                            //
                        }
                        else
                        {
                            dr["RATETYPE"] = Convert.ToString(RateType.Weight);
                        }
                        if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone)
                        {

                            // Added 
                            if (!string.IsNullOrEmpty(sCalcType))
                            {

                                // if (Convert.ToInt32(dr["RATETYPE"]) == Convert.ToInt32(RateType.Weight)) //COs  Modification
                                if (Convert.ToString(dr["RATETYPE"]) == Convert.ToString(RateType.Weight)) //COs  Modification
                                    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                                // else if (Convert.ToInt32(dr["RATETYPE"]) == Convert.ToInt32(RateType.Pcs))
                                else if (Convert.ToString(dr["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                                else
                                    dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                            }
                            else
                            {
                                dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        else
                        {
                            // dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero); //COs  Modification
                            dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(dr["QUANTITY"]) * Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);

                        }

                        dStoneWtRange = 0m;

                    }
                    dtClone.Columns.Remove("INVENTBATCHID");
                    dtClone.AcceptChanges();
                }
                return dtClone;
            }
            catch (LSRetailPosis.PosisException ex)
            {
                throw ex;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        #endregion

        private bool IsMetalRateCalcTypeGross(string sSKUNo)
        {
            int bStatus = 0;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sQry = "SELECT CRWMetalRateCalcType FROM INVENTTABLE WHERE ITEMID = '" + sSKUNo + "'";


            using (SqlCommand cmd = new SqlCommand(sQry, connection))
            {
                cmd.CommandTimeout = 0;
                bStatus = Convert.ToInt16(cmd.ExecuteScalar());
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (bStatus == (int)CRWMakingCalcType.Gross)
                return true;
            else
                return false;

        }

        #region  GET RATE FROM METAL TABLE
        public string getRateFromMetalTable(string itemid, string configuration, string batchid, string colorid, string sizeid, string weight, string pcs = null)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @PARENTITEM  VARCHAR(20) DECLARE @ITEM VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + itemid.Trim() + "' ");

            commandText.Append(" SET @PARENTITEM = ''");
            commandText.Append(" IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + itemid.Trim() + "' AND RETAIL=1)");
            commandText.Append(" BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + itemid.Trim() + "' ");
            commandText.Append(" END  SET @ITEM='" + itemid.Trim() + "'");


            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");

            //   }
            commandText.Append(" SELECT TOP 1 CAST(RATES AS numeric(26,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + configuration.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            if (string.IsNullOrEmpty(weight) || weight ==".")
            {
                weight = "0";
            }
            string ldweight = string.Empty;
            ldweight = "0";
            if (pcs != null && Convert.ToDecimal(pcs) != Convert.ToDecimal(0))   // Convert.ToDecimal(pcs) != Convert.ToDecimal(0) added as per BOM.PDSCWQTY added in BOM on request of urvi and arunava 
            {
                ldweight = Convert.ToString(decimal.Round((Convert.ToDecimal(weight) / Convert.ToDecimal(pcs)), 3, MidpointRounding.AwayFromZero)); //Changes on 11/04/2014 R.Hossain
            }
            else
            {
                ldweight = Convert.ToString(Convert.ToDecimal(weight));
            }

            commandText.Append(" END ");
            commandText.Append(" ELSE IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION Or RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = '')");
            commandText.Append(" AND ('" + ldweight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID =@ITEM OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = @PARENTITEM");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID='') AND   ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') ");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            commandText.Append(" ELSE ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION Or RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = '')");
            commandText.Append(" AND ('" + weight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID =@ITEM OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = @PARENTITEM");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID='') AND (INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;


            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return Convert.ToString(objRounding.Round(Convert.ToDecimal(sResult.Trim())));
            else
                return string.Empty;

        }
        #endregion

        #region  Get Ingredient Calculation Type

        private string GetIngredientCalcType(string ItemID, string configuration, string batchid, string ColorID, string SizeID, string GrWeight, string pcs = null)  // added
        {
            StringBuilder commandText = new StringBuilder();

            string grweigh = string.Empty;
            if (string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            // commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
            commandText.Append(" SELECT     ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            // commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange); //COs  Modification
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");

            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') ");

            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1");

            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");
            // }

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if (isValied())
            {
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("PRODUCT", typeof(string));
                    dtItemInfo.Columns.Add("COLLECTION", typeof(string));
                    dtItemInfo.Columns.Add("ARTICLE", typeof(string));

                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));
                    dtItemInfo.Columns.Add("RATE", typeof(decimal));
                    dtItemInfo.Columns.Add("RATETYPE", typeof(string));
                    dtItemInfo.Columns.Add("MAKINGRATE", typeof(decimal));
                    dtItemInfo.Columns.Add("MAKINGTYPE", typeof(string));
                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("MAKINGAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("EXTENDEDDETAILS", typeof(decimal));
                    dtItemInfo.Columns.Add("TOTALAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("ROWTOTALAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("DIMENSIONS", typeof(string));

                    //Added on 15-01-2012
                    dtItemInfo.Columns.Add("UNITID", typeof(string));

                    dtItemInfo.Columns.Add("WastageRate", typeof(decimal));
                    dtItemInfo.Columns.Add("WastageType", typeof(string));
                    dtItemInfo.Columns.Add("WastageQty", typeof(decimal));
                    dtItemInfo.Columns.Add("WastagePercentage", typeof(decimal));
                    dtItemInfo.Columns.Add("WastageAmount", typeof(decimal));

                    dtItemInfo.Columns.Add("IsBookedSKU", typeof(bool));
                    dtItemInfo.Columns.Add("RemarksDtl", typeof(string)); // 
                    dtItemInfo.Columns.Add("IsCertReq", typeof(bool));
                    dtItemInfo.Columns.Add("CertAgentCode", typeof(string)); // 
                    dtItemInfo.Columns.Add("ACTUALITEMID", typeof(string)); // 
                    dtItemInfo.Columns.Add("CUSTORDSTOREDELIVERYDAYS", typeof(int)); // 
                    dtItemInfo.Columns.Add("FESTIVECODE", typeof(string)); // 
                    dtItemInfo.Columns.Add("ADVANCEAMT", typeof(decimal));
                    dtItemInfo.Columns.Add("MetalQty", typeof(decimal));
                    dtItemInfo.Columns.Add("GoldRate", typeof(decimal));
                    dtItemInfo.Columns.Add("SilverRate", typeof(decimal));
                    dtItemInfo.Columns.Add("PlatinumRate", typeof(decimal));
                    dtItemInfo.Columns.Add("SalesManId", typeof(string));
                }
                if (IsEdit == false)
                {

                    if (iGMA == 1 && dtItemInfo.Rows.Count > 0)
                    {
                        MessageBox.Show("Only one item can be added when GMA is checked");
                        return;
                    }
                    else
                    {

                        dr = dtItemInfo.NewRow();

                        dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());
                        dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                        dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                        dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                        dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                        dr["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                        dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                        if (!string.IsNullOrEmpty(cmbProductType.Text.Trim()))
                            dr["PRODUCT"] = Convert.ToString(cmbProductType.Text.Trim());
                        else
                            dr["PRODUCT"] = "";
                        if (!string.IsNullOrEmpty(cmbCollection.Text.Trim()))
                            dr["COLLECTION"] = Convert.ToString(cmbCollection.Text.Trim());
                        else
                            dr["COLLECTION"] = "";
                        if (!string.IsNullOrEmpty(cmbArticle.Text.Trim()))
                            dr["ARTICLE"] = Convert.ToString(cmbArticle.Text.Trim());
                        else
                            dr["ARTICLE"] = "";

                        if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                            dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                        else
                            dr["PCS"] = DBNull.Value;
                        if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                            dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            dr["QUANTITY"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                            dr["RATE"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            dr["RATE"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(cmbRateType.Text.Trim()))
                            dr["RATETYPE"] = Convert.ToString(cmbRateType.Text.Trim());
                        else
                            dr["RATETYPE"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
                            dr["MAKINGRATE"] = decimal.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        else
                            dr["MAKINGRATE"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(cmbMakingType.Text.Trim()))
                            dr["MAKINGTYPE"] = Convert.ToString(cmbMakingType.Text.Trim());
                        else
                            dr["MAKINGTYPE"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                            dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                        else
                            dr["AMOUNT"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                            dr["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                        else
                            dr["MAKINGAMOUNT"] = DBNull.Value;

                        if (!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                            dr["ROWTOTALAMOUNT"] = decimal.Round((Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim())), 2, MidpointRounding.AwayFromZero);
                        else
                            dr["ROWTOTALAMOUNT"] = decimal.Round((Convert.ToDecimal(txtAmount.Text.Trim())), 2, MidpointRounding.AwayFromZero);

                        if (!string.IsNullOrEmpty(txtTotalAmount.Text.Trim()))
                            dr["TOTALAMOUNT"] = decimal.Round(Convert.ToDecimal(txtTotalAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                        else
                            dr["TOTALAMOUNT"] = DBNull.Value;

                        dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;
                        dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                        dr["DIMENSIONS"] = Previewdimensions;

                        dr["IsBookedSKU"] = (chkBookedSKU.Checked) ? 1 : 0;
                        dr["RemarksDtl"] = txtRemarks.Text.Trim();

                        dr["IsCertReq"] = (chkCertReq.Checked) ? 1 : 0;
                        dr["CertAgentCode"] = cmbCertAgent.Text.Trim();

                        dr["CUSTORDSTOREDELIVERYDAYS"] = iOrderStoreDeliveryDays; // added on 250119
                        dr["ACTUALITEMID"] = sActualItemId;
                        dr["FESTIVECODE"] = frmCustOrder.cmbSVSchemeCode.Text.Trim();

                        // decimal dTotAmt = decimal.Round(Convert.ToDecimal(txtTotalAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);

                        dr["ADVANCEAMT"] = 0m;// decimal.Round(Convert.ToDecimal((dTotAmt * dAdvancePct / 100)), 2, MidpointRounding.AwayFromZero);

                        if (!string.IsNullOrEmpty(frmCustOrder.cmbSVSchemeCode.Text.Trim()))
                        {
                            dr["MetalQty"] = decimal.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                            dr["GoldRate"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            dr["MetalQty"] = 0m;
                            dr["GoldRate"] = 0m;
                        }
                        dr["SilverRate"] = 0m;
                        dr["PlatinumRate"] = 0m;
                        dr["SalesManId"] = Convert.ToString(txtSM.Tag) == null ? string.Empty : Convert.ToString(txtSM.Tag);

                        dtItemInfo.Rows.Add(dr);

                        grItems.DataSource = dtItemInfo.DefaultView;
                    }
                }

                if (IsEdit == true)
                {
                    decimal dWastageRowAmt = 0m;
                    Previewdimensions = ColorSizeStyleConfig();
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    EditRow["PCS"] = string.IsNullOrEmpty(txtPCS.Text.Trim()) ? "0" : txtPCS.Text.Trim();
                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                    EditRow["RATETYPE"] = cmbRateType.Text.Trim();
                    EditRow["RATE"] = txtRate.Text.Trim();
                    EditRow["AMOUNT"] = txtAmount.Text.Trim();
                    EditRow["RATETYPE"] = Convert.ToString(cmbRateType.Text.Trim());
                    EditRow["MAKINGRATE"] = decimal.Round(Convert.ToDecimal(string.IsNullOrEmpty(txtMakingRate.Text.Trim()) ? "0" : txtMakingRate.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    EditRow["MAKINGTYPE"] = Convert.ToString(cmbMakingType.Text.Trim());
                    EditRow["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(string.IsNullOrEmpty(txtMakingAmount.Text.Trim()) ? "0" : txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    EditRow["PRODUCT"] = cmbProductType.Text.Trim();
                    EditRow["COLLECTION"] = cmbCollection.Text.Trim();
                    EditRow["ARTICLE"] = cmbArticle.Text.Trim();
                    EditRow["RemarksDtl"] = txtRemarks.Text.Trim();
                    EditRow["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;
                    //if (!string.IsNullOrEmpty(Convert.ToString(txtWastageAmount.Text.Trim())))
                    //{
                    //    dWastageRowAmt = Convert.ToDecimal(txtWastageAmount.Text.Trim());
                    //}
                    EditRow["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    EditRow["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    EditRow["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                    EditRow["SIZE"] = Convert.ToString(cmbSize.Text.Trim());

                    EditRow["ROWTOTALAMOUNT"] = Convert.ToDecimal(string.IsNullOrEmpty(txtAmount.Text.Trim()) ? "0" : txtAmount.Text.Trim()) + Convert.ToDecimal(string.IsNullOrEmpty(txtMakingAmount.Text.Trim()) ? "0" : txtMakingAmount.Text.Trim()) + dWastageRowAmt;

                    EditRow["DIMENSIONS"] = Previewdimensions;

                    EditRow["IsCertReq"] = (chkCertReq.Checked) ? 1 : 0;
                    EditRow["CertAgentCode"] = cmbCertAgent.Text.Trim();

                    EditRow["CUSTORDSTOREDELIVERYDAYS"] = iOrderStoreDeliveryDays; // added on 250119
                    EditRow["ACTUALITEMID"] = sActualItemId;

                    EditRow["FESTIVECODE"] = sFestiveCode;

                    decimal dTotAmt = decimal.Round(Convert.ToDecimal(EditRow["ROWTOTALAMOUNT"]), 2, MidpointRounding.AwayFromZero);

                    decimal dSVTaxPct = frmCustOrder.dSVTaxPct;
                    EditRow["ADVANCEAMT"] = decimal.Round(Convert.ToDecimal((dTotAmt * dAdvancePct / 100) + (((dTotAmt * dAdvancePct / 100) * dSVTaxPct) / 100)), 2, MidpointRounding.AwayFromZero);

                    EditRow["SalesManID"] = Convert.ToString(txtSM.Tag) == null ? string.Empty : Convert.ToString(txtSM.Tag);

                    dtItemInfo.AcceptChanges();
                    grItems.DataSource = dtItemInfo.DefaultView;


                }

                #region FOR FILLING INGREDIENT DETAILS
                DataTable dt = new DataTable();
                if (!IsEdit)
                {
                    if (bIsSKUItem)//IsSKUItem(txtItemId.Text.Trim())
                        dt = GetIngredientDetails(sUniqueNo, true);
                    else
                        if (string.IsNullOrEmpty(frmCustOrder.cmbSVSchemeCode.Text.Trim()))
                        {
                            dt = GetIngredientDetails(sUniqueNo);
                        }
                }

                //if (IsEdit)
                //    dtSubOrderDetails = new DataTable("dtSubOrderDetails");

                decimal eAmt = 0;
                string UID = string.Empty;

                if (!isMRPExists)
                {
                    if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow drIngredients in dt.Rows)
                            {
                                UID = Convert.ToString(drIngredients["UNIQUEID"]);
                                eAmt += Convert.ToDecimal(drIngredients["AMOUNT"]);
                                dtSubOrderDetails.ImportRow(drIngredients);
                            }
                        }
                    }
                    else
                    {
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow drIngredients in dt.Rows)
                            {
                                UID = Convert.ToString(drIngredients["UNIQUEID"]);
                                eAmt += decimal.Round(Convert.ToDecimal(drIngredients["AMOUNT"]), 2, MidpointRounding.AwayFromZero);
                            }
                        }

                        dtSubOrderDetails = dt;
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow drIngrd in dt.Rows)
                        {
                            int iMetalType = 0;
                            iMetalType = Convert.ToInt32(drIngrd["METALTYPE"]);

                            if (iMetalType == (int)MetalType.Gold
                                || iMetalType == (int)MetalType.Silver
                                || iMetalType == (int)MetalType.Platinum)
                            {
                                dIngrdTotalGoldQty += Convert.ToDecimal(drIngrd["QUANTITY"]);
                                dIngrdTotalGoldValue += Convert.ToDecimal(drIngrd["AMOUNT"]);

                                dWMetalRate = getWastageMetalRate(pos.StoreId,
                                                Convert.ToString(drIngrd["ITEMID"]), Convert.ToString(drIngrd["CONFIGURATION"]));// Added for wastage // not consider -- Multi Ingredient gold
                            }

                            if (bIsGrossMetalCal)
                            {
                                dIngrdMetalWtRange += Convert.ToDecimal(drIngrd["QUANTITY"]);
                            }
                        }
                        if (!bIsGrossMetalCal)
                            dIngrdMetalWtRange = Convert.ToDecimal(dt.Rows[0]["QUANTITY"]);
                    }
                }
                else
                    dtSubOrderDetails = dt;

                #endregion
                // if (dIngrdTotalGoldQty > 0) // blocked on 05.08.2013

                if (iGMA == 0)
                {
                    SqlConnection conn = new SqlConnection();
                    if (application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;

                    CheckMakingRateFromDB(conn, pos.StoreId, sBaseItemID, Convert.ToString(cmbProductType.SelectedValue)
                    , Convert.ToString(cmbCollection.SelectedValue), Convert.ToString(cmbArticle.SelectedValue));

                    if (eAmt != 0)
                    {
                        foreach (DataRow dr1 in dtItemInfo.Select("UNIQUEID='" + UID.Trim() + "'"))
                        {
                            //if (!IsEdit) //COs  Modification
                            //  {
                            // if (dt != null && dt.Rows.Count > 0) // --check   

                            if (!string.IsNullOrEmpty(cmbMakingType.Text.Trim()))
                            {
                                dr1["MAKINGTYPE"] = Convert.ToString(cmbMakingType.Text.Trim());
                            }
                            if (!string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
                                dr1["MAKINGRATE"] = decimal.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                            else
                                dr1["MAKINGRATE"] = 0m;

                            if (!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                                dr1["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(txtMakingAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                            else
                                dr1["MAKINGAMOUNT"] = 0m;


                            dr1["EXTENDEDDETAILS"] = eAmt;

                            dr1["ROWTOTALAMOUNT"] = eAmt + Convert.ToDecimal(dr1["MAKINGAMOUNT"]);// +Convert.ToDecimal(dr1["WastageAmount"]);

                            dr1["AMOUNT"] = eAmt;
                            dr1["RATE"] = eAmt;

                            dr1["FESTIVECODE"] = sFestiveCode;

                            decimal dTotAmt = decimal.Round(Convert.ToDecimal(dr1["ROWTOTALAMOUNT"]), 2, MidpointRounding.AwayFromZero);

                            decimal dSVTaxPct = frmCustOrder.dSVTaxPct;

                            dr1["ADVANCEAMT"] = decimal.Round(Convert.ToDecimal((dTotAmt * dAdvancePct / 100) + (((dTotAmt * dAdvancePct / 100) * dSVTaxPct) / 100)), 2, MidpointRounding.AwayFromZero);

                            if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                            {
                                cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");
                                dr1["RATETYPE"] = cmbRateType.Text; //cmbRateType.FindStringExact("Tot");
                            }

                            dtItemInfo.AcceptChanges();
                            break;
                        }
                        grItems.DataSource = dtItemInfo.DefaultView;
                    }
                }

                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    Decimal dTotalAmount = 0m;
                    foreach (DataRow drTotal in dtItemInfo.Rows)
                    {
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);

                        decimal dTotAmt = decimal.Round((!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0), 2, MidpointRounding.AwayFromZero);
                        //drTotal["ADVANCEAMT"] = decimal.Round(Convert.ToDecimal((dTotAmt * dAdvancePct / 100)), 2, MidpointRounding.AwayFromZero);

                        decimal dSVTaxPct = frmCustOrder.dSVTaxPct;

                        drTotal["ADVANCEAMT"] = decimal.Round(Convert.ToDecimal((dTotAmt * dAdvancePct / 100) + (((dTotAmt * dAdvancePct / 100) * dSVTaxPct) / 100)), 2, MidpointRounding.AwayFromZero);


                        dtItemInfo.AcceptChanges();
                    }
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                if (IsEdit == true || dtItemInfo.Rows.Count > 1)
                {
                    ExtendedDetailsCalAtEditTime(UID, 1);
                }
                else
                {
                    ExtendedDetailsCalAtEditTime(UID);
                }

                ClearControls();

                dIngrdMetalWtRange = 0m;
                dIngrdTotalGoldQty = 0m;
                dIngrdTotalGoldValue = 0m;

                dWastQty = 0m;
                dWMetalRate = 0m;

                frmCustOrder.chkSUVARNAVRUDDHI.Enabled = false;
                frmCustOrder.btnSVSchemeSearch.Enabled = false;

                if (IsEdit == true)
                    IsEdit = false;
            }
        }
        #endregion

        private void GetSuvarnaVrudhiDiscount(string sItemId, string sStoreGrp, ref string sFestiveCode, ref decimal dDisc, ref decimal dAdv, ref int iRoundOff, ref DateTime dtDeliveryDate)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @Temp TABLE (FESTIVECODE nvarchar(20) ,DISCOUNT numeric(20,2)," +
                                 " ADVANCE numeric(20,2) ,ROUNDINGTOLEREANCE numeric(10,2),DELIVERYDATE [datetime]);" +
                                 "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVE FROM CRWSUVARNA_VRUDDHI SAD )" +
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +

                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END ";


            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE" + //1
                " FROM   CRWSUVARNA_VRUDDHI  SAD" +
                " WHERE (SAD.ITEMID=@PARENTITEM) " +
                 " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                     " BEGIN " +
                     " INSERT INTO @Temp (FESTIVECODE,DISCOUNT,ADVANCE,ROUNDINGTOLEREANCE,DELIVERYDATE)  " +
                     "     SELECT  TOP (1) FESTIVECODE,CAST(SAD.DISCOUNT AS decimal(18, 2))" +
                    "     ,CAST(SAD.ADVANCE AS decimal(18, 2)),CAST(SAD.ROUNDINGTOLEREANCE AS decimal(10, 2)),DELIVERYDATE FROM CRWSUVARNA_VRUDDHI SAD  " +
                     " WHERE     " +
                     "     (SAD.ITEMID=@PARENTITEM ) " +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                     " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                     "       AND SAD.ACTIVE = 1" +
                     " ORDER BY SAD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE" + //2
               " FROM    CRWSUVARNA_VRUDDHI SAD  " +
               " WHERE (SAD.ITEMID=@PARENTITEM) " +

                 " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                    " BEGIN " +
                    " INSERT INTO @Temp (FESTIVECODE,DISCOUNT,ADVANCE,ROUNDINGTOLEREANCE,DELIVERYDATE)  " +
                    "     SELECT  TOP (1)  FESTIVECODE,CAST(SAD.DISCOUNT AS decimal(18, 2))" +
                    "     ,CAST(SAD.ADVANCE AS decimal(18, 2)),CAST(SAD.ROUNDINGTOLEREANCE AS decimal(10, 2)),DELIVERYDATE FROM CRWSUVARNA_VRUDDHI SAD  " +
                    "     WHERE     " +
                    "     (SAD.ITEMID=@PARENTITEM ) " +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                     " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)" +
                    "       AND SAD.ACTIVE = 1" +
                    " ORDER BY SAD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE" + //3
                " FROM    CRWSUVARNA_VRUDDHI SAD  " +
                " WHERE (SAD.ITEMID='') " +

                " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +

                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                     " BEGIN " +
                      " INSERT INTO @Temp (FESTIVECODE,DISCOUNT,ADVANCE,ROUNDINGTOLEREANCE,DELIVERYDATE)  " +
                     "     SELECT   FESTIVECODE,CAST(SAD.DISCOUNT AS decimal(18, 2))" +
                    "     ,CAST(SAD.ADVANCE AS decimal(18, 2)),CAST(SAD.ROUNDINGTOLEREANCE AS decimal(10, 2)),DELIVERYDATE FROM CRWSUVARNA_VRUDDHI SAD  " +
                     "     WHERE     " +
                     "     (SAD.ITEMID='' )" +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +

                      " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                     "       AND SAD.ACTIVE = 1" +
                     " ORDER BY SAD.ITEMID DESC END";
            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE" + //4
               " FROM   CRWSUVARNA_VRUDDHI SAD " +
               " WHERE (SAD.ITEMID='') " +
               " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                    " BEGIN " +
                     " INSERT INTO @Temp (FESTIVECODE,DISCOUNT,ADVANCE,ROUNDINGTOLEREANCE,DELIVERYDATE)  " +
                    "     SELECT  TOP (1) FESTIVECODE,CAST(SAD.DISCOUNT AS decimal(18, 2))" +
                    "     ,CAST(SAD.ADVANCE AS decimal(18, 2)),CAST(SAD.ROUNDINGTOLEREANCE AS decimal(10, 2)),DELIVERYDATE FROM CRWSUVARNA_VRUDDHI SAD  " +
                    "     WHERE     " +
                    "     (SAD.ITEMID='' )  " +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +

                    " AND (SAD.STORE = CASE WHEN SAD.STORETYPE=1 THEN '" + sStoreGrp + "' WHEN SAD.STORETYPE=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.STORETYPE=2 THEN '' END)" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                    "       AND SAD.ACTIVE = 1" +
                    " ORDER BY SAD.ITEMID DESC END";
            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    sFestiveCode = Convert.ToString(mkDiscRd.GetValue(0));
                    dDisc = Convert.ToDecimal(mkDiscRd.GetValue(1));
                    dAdv = Convert.ToDecimal(mkDiscRd.GetValue(2));
                    iRoundOff = Convert.ToInt16(mkDiscRd.GetValue(3));
                    dtDeliveryDate = Convert.ToDateTime(mkDiscRd.GetValue(4));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void GetSuvarnaVrudhiDiscount(string sSchemeCode, ref decimal dDisc, ref decimal dAdv, ref int iRoundOff,
            ref string sArticle, ref string sProd, ref string sItemId, ref string sColl)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT  TOP (1) CAST(DISCOUNT AS decimal(18, 2))" +
                        " ,CAST(ADVANCE AS decimal(18, 2)),CAST(ROUNDINGTOLEREANCE AS decimal(10, 2))," +
                        " isnull(ARTICLE,'') ARTICLE,isnull(PRODUCTTYPECODE,'') PRODUCTTYPECODE," +
                        " isnull(ITEMID,'') ITEMID,isnull(COLLECTION,'') COLLECTION FROM CRWSUVARNA_VRUDDHI " +
                        " WHERE    FestiveCode='" + sSchemeCode + "'  and GETDATE() between FROMDATE and TODATE";


            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dDisc = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    dAdv = Convert.ToDecimal(mkDiscRd.GetValue(1));
                    iRoundOff = Convert.ToInt16(mkDiscRd.GetValue(2));
                    sArticle = Convert.ToString(mkDiscRd.GetValue(3));
                    sProd = Convert.ToString(mkDiscRd.GetValue(4));
                    sItemId = Convert.ToString(mkDiscRd.GetValue(5));
                    sColl = Convert.ToString(mkDiscRd.GetValue(6));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        #region Extended Details CLick
        private void btnExtendedDetails_Click(object sender, EventArgs e)
        {
            BlankOperations.WinFormsTouch.frmSubOrderDetails objSubOrderdetails = null;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                objSubOrderdetails = ExtendedDetailsCal(objSubOrderdetails);
            }
            else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[2].Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];
                    sExtendedDetailsAmount = 0m;
                    objSubOrderdetails = new BlankOperations.WinFormsTouch.frmSubOrderDetails(dsSearchedOrder, pos, application, this, Convert.ToString(drRowToUpdate["LINENUM"]));
                    objSubOrderdetails.ShowDialog();
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the details or No Ingredients Exists.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }

        private BlankOperations.WinFormsTouch.frmSubOrderDetails ExtendedDetailsCal(BlankOperations.WinFormsTouch.frmSubOrderDetails objSubOrderdetails)
        {
            if (grdView.RowCount > 0)
            {
                int selectedRow = grdView.GetSelectedRows()[0];
                DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                sExtendedDetailsAmount = 0m;

                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                bool bMRP = isMRP(Convert.ToString(drRowToUpdate["ITEMID"]), conn);

                string UID = Convert.ToString(drRowToUpdate["UNIQUEID"]);

                DataTable dtSubOrderDetailsClone = new DataTable();
                dtSubOrderDetailsClone = dtSubOrderDetails.Clone();

                if (bMRP)
                {
                    foreach (DataRow drClone in dtSubOrderDetails.Rows)
                    {
                        drClone["Amount"] = 0;
                        drClone["Rate"] = 0;

                        dtSubOrderDetailsClone.ImportRow(drClone);
                    }
                }

                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    objSubOrderdetails = new BlankOperations.WinFormsTouch.frmSubOrderDetails(dtSubOrderDetails, pos, application, this, Convert.ToString(drRowToUpdate["UNIQUEID"]), bMRP);
                else
                {
                    dtSubOrderDetails = new DataTable();
                    objSubOrderdetails = new BlankOperations.WinFormsTouch.frmSubOrderDetails(pos, application, this, Convert.ToString(drRowToUpdate["UNIQUEID"]));
                }
                objSubOrderdetails.ShowDialog();

                decimal dMkAmt = 0;
                decimal dWastAmt = 0;
                decimal dQty = 0;
                decimal dConvertedMetalQty = 0m;
                decimal dConvertedGrossQty = 0m;
                decimal dMkRate = 0M;

                if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["MAKINGAMOUNT"])))
                    dMkAmt = Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);

                if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["ItemId"])))
                    sBaseItemID = Convert.ToString(drRowToUpdate["ItemId"]);

                int iMainItemMetalType = GetMetalType(sBaseItemID);

                if (sExtendedDetailsAmount != 0)
                {
                    drRowToUpdate["AMOUNT"] = sExtendedDetailsAmount;
                    drRowToUpdate["RATE"] = sExtendedDetailsAmount;

                    if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                    {
                        decimal dItemQty = 0m;
                        foreach (DataRow drSubTotal in dtSubOrderDetails.Select("UNIQUEID='" + UID.Trim() + "'"))
                        {
                            if (Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Gold
                                || Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Platinum
                                || Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Silver
                                || Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Palladium)
                            {
                                int iIngItemMetalType = GetMetalType(Convert.ToString(drSubTotal["ItemId"]));

                                if (iMainItemMetalType == iIngItemMetalType)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["PCS"])))
                                        dItemQty = Convert.ToDecimal(drSubTotal["Quantity"]) / Convert.ToDecimal(drRowToUpdate["PCS"]);
                                    else
                                        dItemQty = Convert.ToDecimal(drSubTotal["Quantity"]);

                                    dConvertedMetalQty += Convert.ToDecimal(NIM_GetConvertionValue(Convert.ToString(drSubTotal["UNITID"]), unitid, dItemQty, Convert.ToInt16(drSubTotal["METALTYPE"])));
                                }
                            }

                            dConvertedGrossQty += Convert.ToDecimal(NIM_GetConvertionValue(Convert.ToString(drSubTotal["UNITID"]), unitid, Convert.ToDecimal(drSubTotal["Quantity"]), Convert.ToInt16(drSubTotal["METALTYPE"])));
                        }

                        int iMkType = 0;
                        dMkRate = getMakingRateFromDB(pos.StoreId, sBaseItemID, Convert.ToString(drRowToUpdate["PRODUCT"])
                                    , Convert.ToString(drRowToUpdate["COLLECTION"]), Convert.ToString(drRowToUpdate["ARTICLE"]), dConvertedMetalQty, ref iMkType);


                        dMkAmt = 0m;
                        foreach (DataRow drSubTotal in dtSubOrderDetails.Select("UNIQUEID='" + UID.Trim() + "'"))
                        {
                            if (Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Gold
                                || Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Platinum
                                || Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Silver
                                || Convert.ToInt32(drSubTotal["METALTYPE"]) == (int)MetalType.Palladium)
                            {

                                decimal dPcs = 0m;
                                if (!string.IsNullOrEmpty(Convert.ToString(drSubTotal["PCS"])))
                                {
                                    dPcs = Convert.ToDecimal(drSubTotal["PCS"]);
                                }

                                switch (Convert.ToString(iMkType))
                                {
                                    case "0":
                                        // cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces));
                                        dMkAmt += decimal.Round(dPcs * dMkRate, 2, MidpointRounding.AwayFromZero);
                                        break;
                                    case "2":
                                        //cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight));
                                        dMkAmt += decimal.Round(Convert.ToDecimal(drSubTotal["QUANTITY"]) * dMkRate, 2, MidpointRounding.AwayFromZero);
                                        break;
                                    case "3":
                                        //cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot));
                                        dMkAmt += decimal.Round(dMkRate, 2, MidpointRounding.AwayFromZero);
                                        break;
                                    case "4":
                                        //cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));
                                        dMkAmt += decimal.Round((Convert.ToDecimal(drSubTotal["AMOUNT"]) * dMkRate) / 100, 2, MidpointRounding.AwayFromZero);
                                        break;
                                    case "6":
                                        //cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Gross));
                                        dMkAmt += decimal.Round(Convert.ToDecimal(drSubTotal["QUANTITY"]) * dMkRate, 2, MidpointRounding.AwayFromZero);
                                        break;
                                    default:
                                        //cmbMakingType.SelectedIndex = 2;
                                        dMkAmt += decimal.Round(Convert.ToDecimal(drSubTotal["QUANTITY"]) * dMkRate, 2, MidpointRounding.AwayFromZero);
                                        break;
                                }

                                //if (Convert.ToString(drSubTotal["RATETYPE"]) == Convert.ToString(RateType.Weight))
                                //    dMkAmt += decimal.Round(Convert.ToDecimal(drSubTotal["QUANTITY"]) * dMkRate, 2, MidpointRounding.AwayFromZero);
                                //else if (Convert.ToString(drSubTotal["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                //    dMkAmt += decimal.Round(Convert.ToDecimal(drSubTotal["PCS"]) * dMkRate, 2, MidpointRounding.AwayFromZero);
                                //else
                                //    dMkAmt += decimal.Round(dMkRate, 2, MidpointRounding.AwayFromZero);
                            }
                        }
                    }
                    drRowToUpdate["MAKINGRATE"] = Convert.ToString(dMkRate);
                    drRowToUpdate["MAKINGAMOUNT"] = dMkAmt;
                    drRowToUpdate["Quantity"] = dConvertedGrossQty;
                    drRowToUpdate["ROWTOTALAMOUNT"] = sExtendedDetailsAmount + dMkAmt + dWastAmt;
                }
                else
                {
                    drRowToUpdate["ROWTOTALAMOUNT"] = Convert.ToDecimal(drRowToUpdate["AMOUNT"]) + dMkAmt + dWastAmt;
                }
                drRowToUpdate["EXTENDEDDETAILS"] = sExtendedDetailsAmount;

                dtItemInfo.AcceptChanges();
                grItems.DataSource = null;
                grItems.DataSource = dtItemInfo;
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    Decimal dTotalAmount = 0m;
                    foreach (DataRow drTotal in dtItemInfo.Rows)
                    {
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);
                    }
                    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                }

            }
            return objSubOrderdetails;
        }

        #region NIM_GetConvertionValue //24/09/2014
        public string NIM_GetConvertionValue(string _frmUnit, string _toUnit, decimal _frmValue, int iMetalType)
        {
            string sqlString = string.Empty;
            string frmUnit = string.Empty;
            string toUnit = string.Empty;
            string convValue = "1";
            string defUnit = string.Empty;

            defUnit = _toUnit;

            sqlString = " DECLARE @frmUnit varchar(10)" + //24K
                   " DECLARE @toUnit varchar(10)" + //14K-58.3
                   " set @frmUnit='" + _frmUnit + "'" +
                   " set @toUnit='" + defUnit + "'" +

                   "if exists ( SELECT RECID FROM UNITOFMEASURECONVERSION WHERE TOUNITOFMEASURE " +
                               " =(SELECT RECID FROM UNITOFMEASURE WHERE SYMBOL=@toUnit)" +
                               " and FROMUNITOFMEASURE =(select RECID FROM UNITOFMEASURE WHERE SYMBOL=@frmUnit)" +
                               " and PRODUCT=0" +
                               " )" +
                        " begin" +
                            " SELECT 'FROM'=@frmUnit,'TO'=@toUnit, " +
                //" CONVERT(VARCHAR,CONVERT(DECIMAL(12,3),UOMC.FACTOR * (UOMC.NUMERATOR/UOMC.DENOMINATOR ))) AS QNT,'','', '' " +
                             " CONVERT(VARCHAR,CONVERT(DECIMAL(12,3),UOMC.FACTOR)) AS QNT,'','','' " +
                            " FROM UNITOFMEASURECONVERSION AS UOMC" +
                            " WHERE UOMC.PRODUCT=0" +
                            " AND UOMC.TOUNITOFMEASURE =(SELECT RECID FROM UNITOFMEASURE WHERE SYMBOL= @toUnit)" +
                            " AND UOMC.FROMUNITOFMEASURE =(SELECT RECID FROM UNITOFMEASURE WHERE SYMBOL= @frmUnit) " +
                        " end " +

                    "else " +
                        " begin" +
                            " SELECT 'FROM'=@toUnit,'TO'=@frmUnit," +
                //" CONVERT(VARCHAR,CONVERT(DECIMAL(12,3),UOMC.FACTOR * (UOMC.NUMERATOR/UOMC.DENOMINATOR ))) AS QNT,'','','' " +
                            " CONVERT(VARCHAR,CONVERT(DECIMAL(12,3),UOMC.FACTOR)) AS QNT,'','','' " +
                            " FROM UNITOFMEASURECONVERSION AS UOMC" +
                            " WHERE UOMC.PRODUCT=0" +
                            " AND UOMC.TOUNITOFMEASURE =(SELECT RECID FROM UNITOFMEASURE WHERE SYMBOL=@frmUnit)" +
                            " AND UOMC.FROMUNITOFMEASURE =(SELECT RECID FROM UNITOFMEASURE WHERE SYMBOL=@toUnit)" +
                        " end ";

            NIM_ReturnMoreValues(Convert.ToString(sqlString), out frmUnit, out toUnit, out convValue);

            if (!string.IsNullOrEmpty(defUnit) && !string.IsNullOrEmpty(_frmUnit))
            {
                if (defUnit.ToUpper() == _frmUnit.ToUpper())
                    return Convert.ToString(_frmValue);
                else
                {
                    if (Convert.ToDecimal(convValue) > 0)
                        return !string.IsNullOrEmpty(Convert.ToString(_frmValue * Convert.ToDecimal(convValue))) ? decimal.Round(Convert.ToDecimal(Convert.ToString(_frmValue * Convert.ToDecimal(convValue))), 3, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                    //Convert.ToString(_frmValue * Convert.ToDecimal(convValue));
                    else
                        return "0";
                }
            }
            else
                return Convert.ToString(_frmValue);
        }
        public void NIM_ReturnMoreValues(string query, out string val1, out string val2, out string val3)
        {
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            try
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                cmd = new SqlCommand(query, connection);
                reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    string ReturnVal1 = "0";
                    string ReturnVal2 = "0";
                    string ReturnVal3 = "0";
                    while (reader.Read())
                    {
                        ReturnVal1 = Convert.ToString(reader.GetValue(0));
                        ReturnVal2 = Convert.ToString(reader.GetValue(1));
                        ReturnVal3 = Convert.ToString(reader.GetValue(2));
                    }
                    reader.Close();
                    val1 = Convert.ToString(ReturnVal1);
                    val2 = Convert.ToString(ReturnVal2);
                    val3 = Convert.ToString(ReturnVal3);

                }
                else
                {
                    reader.Close();
                    val1 = "0";
                    val2 = "0";
                    val3 = "0";
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
                reader.Close();
                reader.Dispose();

            }

        }
        #endregion

        private void ExtendedDetailsCalAtEditTime(string UID, int iEdit)
        {
            if (grdView.RowCount > 0)
            {
                int selectedRow = grdView.GetSelectedRows()[0];
                if (iEdit == 1 && string.IsNullOrEmpty(UID))
                {
                    DataRow drR = dtItemInfo.Rows[selectedRow];
                    UID = Convert.ToString(drR["UNIQUEID"]);
                }

                DataRow[] drRowToUpdate = dtItemInfo.Select("UNIQUEID='" + UID.Trim() + "'");
                sExtendedDetailsAmount = 0m;

                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                bool bMRP = false;//= isMRP(Convert.ToString(drRowToUpdate["ITEMID"]), conn);

                foreach (DataRow row in drRowToUpdate)
                {
                    bMRP = isMRP(Convert.ToString(drRowToUpdate[0]["ITEMID"]), conn);
                }


                DataTable dtSubOrderDetailsClone = new DataTable();
                dtSubOrderDetailsClone = dtSubOrderDetails.Clone();
                Decimal dTotalAmount = 0m;

                DataTable dtTEMP = dtSubOrderDetails;

                if (dtTEMP != null && dtTEMP.Rows.Count > 0)
                {
                    if (!string.IsNullOrEmpty(UID))
                    {
                        foreach (DataRow drNew in dtTEMP.Select("UNIQUEID='" + UID.Trim() + "'"))
                        {
                            if (!string.IsNullOrEmpty(PreviousPcs) && (IsEdit == true))
                            {
                                drNew["PCS"] = Convert.ToInt16(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["PCS"])) / Convert.ToDecimal(PreviousPcs), 0));
                                drNew["QUANTITY"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["QUANTITY"])) / Convert.ToDecimal(PreviousPcs), 3));
                                drNew["AMOUNT"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(drNew["RATE"]) * Convert.ToDecimal(drNew["QUANTITY"])), 2));
                            }
                            //else
                            //{
                            //    drNew["PCS"] = Convert.ToInt16(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["PCS"])), 0));
                            //    drNew["QUANTITY"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["QUANTITY"])), 3));
                            //    drNew["AMOUNT"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(drNew["RATE"]) * Convert.ToDecimal(drNew["QUANTITY"])), 2));
                            //}

                            dtTEMP.AcceptChanges();
                        }
                    }
                }

                dtSubOrderDetails = dtTEMP;


                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                {
                    if (!bMRP && !string.IsNullOrEmpty(UID))
                    {
                        foreach (DataRow drTotal in dtSubOrderDetails.Select("UNIQUEID='" + UID.Trim() + "'"))//in dtItemInfo.Select("UNIQUEID='" + UID.Trim() + "'")
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                        }
                        txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));// Convert.ToString(dTotalAmount);
                    }
                }

                if (bMRP && !string.IsNullOrEmpty(UID))
                {
                    foreach (DataRow drClone in dtSubOrderDetails.Select("UNIQUEID='" + UID.Trim() + "'"))
                    {
                        drClone["Amount"] = 0;
                        drClone["Rate"] = 0;

                        dtSubOrderDetailsClone.ImportRow(drClone);
                    }
                }

                decimal dMkAmt = 0;
                decimal dWastAmt = 0;
                if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate[0]["MAKINGAMOUNT"])))
                    dMkAmt = Convert.ToDecimal(drRowToUpdate[0]["MAKINGAMOUNT"]);

                if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate[0]["WastageAmount"])))
                    dWastAmt = Convert.ToDecimal(drRowToUpdate[0]["WastageAmount"]);

                if (dTotalAmount != 0)
                {
                    drRowToUpdate[0]["AMOUNT"] = dTotalAmount;
                    drRowToUpdate[0]["RATE"] = dTotalAmount;

                    drRowToUpdate[0]["ROWTOTALAMOUNT"] = dTotalAmount + dMkAmt + dWastAmt;
                }

                drRowToUpdate[0]["EXTENDEDDETAILS"] = Convert.ToString(objRounding.Round(dTotalAmount + dMkAmt, 2));

                dtItemInfo.AcceptChanges();
                grItems.DataSource = null;
                grItems.DataSource = dtItemInfo;

                dTotalAmount = 0;
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    foreach (DataRow drTotal in dtItemInfo.Rows)
                    {
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);
                    }
                    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                }
            }
        }

        private void ExtendedDetailsCalAtEditTime(string UID)
        {
            if (grdView.RowCount > 0)
            {
                int selectedRow = grdView.GetSelectedRows()[0];
                DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                sExtendedDetailsAmount = 0m;

                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                bool bMRP = isMRP(Convert.ToString(drRowToUpdate["ITEMID"]), conn);

                DataTable dtSubOrderDetailsClone = new DataTable();
                dtSubOrderDetailsClone = dtSubOrderDetails.Clone();
                Decimal dTotalAmount = 0m;

                DataTable dtTEMP = dtSubOrderDetails;

                if (dtTEMP != null && dtTEMP.Rows.Count > 0)
                {
                    foreach (DataRow drNew in dtTEMP.Rows)
                    {
                        if (!string.IsNullOrEmpty(PreviousPcs) && (IsEdit == true))
                        {
                            drNew["PCS"] = Convert.ToInt16(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["PCS"])) / Convert.ToDecimal(PreviousPcs), 0));
                            drNew["QUANTITY"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["QUANTITY"])) / Convert.ToDecimal(PreviousPcs), 3));
                            drNew["AMOUNT"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(drNew["RATE"]) * Convert.ToDecimal(drNew["QUANTITY"])), 2));
                        }
                        //else
                        //{
                        //    drNew["PCS"] = Convert.ToInt16(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["PCS"])), 0));
                        //    drNew["QUANTITY"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["QUANTITY"])), 3));
                        //    drNew["AMOUNT"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(drNew["RATE"]) * Convert.ToDecimal(drNew["QUANTITY"])), 2));
                        //}

                        dtTEMP.AcceptChanges();
                    }
                }

                dtSubOrderDetails = dtTEMP;


                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                {
                    if (!bMRP && !string.IsNullOrEmpty(UID))
                    {
                        foreach (DataRow drTotal in dtSubOrderDetails.Select("UNIQUEID='" + UID.Trim() + "'"))//in dtItemInfo.Select("UNIQUEID='" + UID.Trim() + "'")
                        {
                            dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                        }
                        txtTotalAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotalAmount), 2, MidpointRounding.AwayFromZero));// Convert.ToString(dTotalAmount);
                    }
                }

                if (bMRP && !string.IsNullOrEmpty(UID))
                {
                    foreach (DataRow drClone in dtSubOrderDetails.Select("UNIQUEID='" + UID.Trim() + "'"))
                    {
                        drClone["Amount"] = 0;
                        drClone["Rate"] = 0;

                        dtSubOrderDetailsClone.ImportRow(drClone);
                    }
                }

                decimal dMkAmt = 0;
                decimal dWastAmt = 0;
                if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["MAKINGAMOUNT"])))
                    dMkAmt = Convert.ToDecimal(drRowToUpdate["MAKINGAMOUNT"]);

                if (!string.IsNullOrEmpty(Convert.ToString(drRowToUpdate["WastageAmount"])))
                    dWastAmt = Convert.ToDecimal(drRowToUpdate["WastageAmount"]);

                if (dTotalAmount != 0)
                {
                    drRowToUpdate["AMOUNT"] = dTotalAmount;
                    drRowToUpdate["RATE"] = dTotalAmount;

                    drRowToUpdate["ROWTOTALAMOUNT"] = dTotalAmount + dMkAmt + dWastAmt;
                }

                drRowToUpdate["EXTENDEDDETAILS"] = Convert.ToString(objRounding.Round(dTotalAmount + dMkAmt, 2));

                dtItemInfo.AcceptChanges();
                grItems.DataSource = null;
                grItems.DataSource = dtItemInfo;

                dTotalAmount = 0;
                if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                {
                    foreach (DataRow drTotal in dtItemInfo.Rows)
                    {
                        dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["ROWTOTALAMOUNT"])) ? Convert.ToDecimal(drTotal["ROWTOTALAMOUNT"]) : 0);
                    }
                    txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                }
            }
        }
        #endregion

        #region PCS TEXT CHANGED
        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            if (isItemExists)
            {
                if (!string.IsNullOrEmpty(txtPCS.Text) && !string.IsNullOrEmpty(txtQuantity.Text))
                {
                    decimal dPreQty = 0m;
                    if (!string.IsNullOrEmpty(PreviousPcs))
                        dPreQty = Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text) / Convert.ToDecimal(PreviousPcs));
                    else
                        dPreQty = Convert.ToDecimal(Convert.ToDecimal(txtQuantity.Text));
                    if (!string.IsNullOrEmpty(txtPCS.Text))
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(txtPCS.Text) * dPreQty);


                    DataTable dtTEMP = dtSubOrderDetails;

                    if (dtTEMP != null && dtTEMP.Rows.Count > 0 & !string.IsNullOrEmpty(PreviousPcs))
                    {
                        foreach (DataRow drNew in dtTEMP.Rows)
                        {
                            drNew["PCS"] = Convert.ToInt16(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["PCS"])) / Convert.ToDecimal(PreviousPcs), 0));
                            drNew["QUANTITY"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(drNew["QUANTITY"])) / Convert.ToDecimal(PreviousPcs), 2));
                            drNew["AMOUNT"] = Convert.ToString(objRounding.Round(Convert.ToDecimal(Convert.ToDecimal(drNew["RATE"]) * Convert.ToDecimal(drNew["QUANTITY"])), 2));

                            dtTEMP.AcceptChanges();
                        }
                    }

                    dtSubOrderDetails = dtTEMP;

                    decimal dTotalAmount = 0;
                    foreach (DataRow drTotal in dtSubOrderDetails.Rows)
                    {
                        dTotalAmount += Convert.ToDecimal(drTotal["AMOUNT"]);
                    }

                    sExtendedDetailsAmount = dTotalAmount;

                    //if (!string.IsNullOrEmpty(txtMakingRate.Text))
                    //{
                    //    if (!string.IsNullOrEmpty(PreviousPcs))
                    //        txtMakingRate.Text = Convert.ToString(Convert.ToDecimal(txtMakingRate.Text) / Convert.ToDecimal(PreviousPcs));
                    //    txtMakingRate.Text = Convert.ToString(Convert.ToDecimal(txtPCS.Text) * Convert.ToDecimal(txtMakingRate.Text));
                    //}
                }
                //if (string.IsNullOrEmpty(txtPCS.Text))
                //{
                //    txtQuantity.Text = string.Empty;
                //    txtMakingRate.Text = string.Empty;
                //}
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
                PreviousPcs = txtPCS.Text;
            }
        }
        #endregion

        #region QUANTITY TEXT CHANGED
        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            if (isItemExists)
            {
                SqlConnection conn = new SqlConnection();
                if (application != null)
                    conn = application.Settings.Database.Connection;
                else
                    conn = ApplicationSettings.Database.LocalConnection;

                if (iChkSuvarnaVrudhi == 0)
                {
                    txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                }

                CheckMakingRateFromDB(conn, pos.StoreId, txtItemId.Text, Convert.ToString(cmbProductType.Text)
                , Convert.ToString(cmbCollection.Text), Convert.ToString(cmbArticle.Text));
                if (string.IsNullOrEmpty(txtQuantity.Text))
                    txtMakingRate.Text = string.Empty;
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);

                if (iGMA == 1)
                {
                    txtMakingRate.Text = "";
                    txtMakingRate.Enabled = false;
                }
            }
        }
        #endregion

        #region RATE TEXT CHANGED
        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region RATE TYPE SELECTED INDEX CHANGED
        private void cmbRateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()) && txtQuantity.Text!=".")
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3);
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

        #region MAKING RATE TEXT CHANGED
        private void txtMakingRate_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region MAKING TYPE SELECTED INDEX CHANGED
        private void cmbMakingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            decimal dMakingQty = 0m;
            if (!string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && (cmbMakingType.SelectedIndex == 1
                 || cmbMakingType.SelectedIndex == 4)
                 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                Decimal decimalAmount = 0m;

                //if (dIngrdTotalGoldQty > 0)
                //{
                //    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * dIngrdTotalGoldQty;
                //}
                //else
                //{
                //    decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim());
                //}
                if (cmbMakingType.SelectedIndex == 4)
                {
                    dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                }
                else
                {
                    if (dIngrdTotalGoldQty > 0) // if ingr are there
                    {
                        string dNetWt = "";
                        if (bIsSKUItem)//IsSKUItem(txtItemId.Text.Trim())
                        {
                            string sSqr = "SELECT isnull(NetWeight,0) FROM SKUTable_Posted WHERE SkuNumber='" + sBaseItemID + "'";
                            dNetWt = NIM_ReturnExecuteScalar(sSqr);
                        }
                        else
                            dNetWt = txtQuantity.Text;

                        dMakingQty = Convert.ToDecimal(dNetWt);
                    }
                    else
                        dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                }

                //DEV ON 17/05/17 instructed by S.SHARMA
                if (!IsBulkItem(txtItemId.Text.Trim()))
                {
                    if (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    {

                        if (Convert.ToDecimal(dMakingQty) <= 6 && Convert.ToDecimal(dMakingQty) > 0)
                        {
                            decimal dActMkRate = 0;
                            if (!string.IsNullOrEmpty(txtMakingRate.Text))
                                dActMkRate = Convert.ToDecimal(txtMakingRate.Text);
                            if (!isMkAdded && dActMkRate > 0)
                            {
                                isMkAdded = true;
                                txtMakingRate.Text = Convert.ToString(decimal.Round(dActMkRate + (30 / Convert.ToDecimal(dMakingQty)), 2, MidpointRounding.AwayFromZero));
                            }
                        }
                    }
                }

                decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * dMakingQty;

                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }

            if (cmbMakingType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                txtMakingAmount.Text = string.Empty;
            }
            if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2) * objRounding.Round(Convert.ToDecimal(txtPCS.Text.Trim()), 0);
                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if (cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                txtMakingAmount.Text = string.Empty;
            }
            if (cmbMakingType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = objRounding.Round(Convert.ToDecimal(txtMakingRate.Text.Trim()), 2);
                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                Decimal decimalAmount = 0m;

                if (dIngrdTotalGoldValue > 0)
                {
                    decimalAmount = objRounding.Round((Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * dIngrdTotalGoldValue, 2);
                }
                else
                {
                    decimalAmount = objRounding.Round((Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim())), 2);
                }

                txtMakingAmount.Text = Convert.ToString(Math.Round(decimalAmount, 2));
            }
            if (cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                txtMakingAmount.Text = string.Empty;
            }
            if (string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            {
                txtMakingAmount.Text = string.Empty;
            }
        }
        #endregion

        #region Submmit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            bool bValidIngredients = false;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                {
                    foreach (DataRow drMainLine in dtItemInfo.Rows)
                    {
                        string sMainLineConfig = Convert.ToString(drMainLine["CONFIGURATION"]);
                        int iMainLineMetalType = GetMetalType(Convert.ToString(drMainLine["ITEMID"]));

                        if (iMainLineMetalType == (int)MetalType.Gold)
                            dGoldRate = Convert.ToDecimal(drMainLine["RATE"]);
                        else if (iMainLineMetalType == (int)MetalType.Silver)
                            dSilverRate = Convert.ToDecimal(drMainLine["RATE"]);
                        else if (iMainLineMetalType == (int)MetalType.Platinum)
                            dPlatinumRate = Convert.ToDecimal(drMainLine["RATE"]);


                        dMetalQty = Convert.ToDecimal(drMainLine["QUANTITY"]);

                        decimal dMQty = 0;

                        foreach (DataRow drSubLine in dtSubOrderDetails.Select("UNIQUEID='" + Convert.ToString(drMainLine["UNIQUEID"]) + "'"))
                        {
                            string sSubLineConfig = Convert.ToString(drSubLine["CONFIGURATION"]);
                            int iSubLineMetalType = Convert.ToInt16(drSubLine["MetalType"]);
                            if (iMainLineMetalType == iSubLineMetalType)
                            {
                                if (sSubLineConfig == sMainLineConfig)
                                {
                                    dMQty += Convert.ToDecimal(drSubLine["QUANTITY"]);
                                }
                            }
                        }
                        if (dMQty > 0)
                            dMetalQty = dMQty;

                        foreach (DataRow drSubLine in dtSubOrderDetails.Select("UNIQUEID='" + Convert.ToString(drMainLine["UNIQUEID"]) + "'"))
                        {
                            string sSubLineConfig = Convert.ToString(drSubLine["CONFIGURATION"]);
                            int iSubLineMetalType = Convert.ToInt16(drSubLine["MetalType"]);

                            if (iSubLineMetalType == (int)MetalType.Gold)
                            {
                                // if (dGoldRate == 0)
                                dGoldRate = Convert.ToDecimal(drSubLine["RATE"]);
                            }
                            else if (iSubLineMetalType == (int)MetalType.Silver)
                            {
                                // if (dSilverRate == 0)
                                dSilverRate = Convert.ToDecimal(drSubLine["RATE"]);
                            }
                            else if (iSubLineMetalType == (int)MetalType.Platinum)
                            {
                                // if (dPlatinumRate == 0)
                                dPlatinumRate = Convert.ToDecimal(drSubLine["RATE"]);
                            }
                        }

                        foreach (DataRow drSubLine in dtSubOrderDetails.Select("UNIQUEID='" + Convert.ToString(drMainLine["UNIQUEID"]) + "'"))
                        {
                            string sSubLineConfig = Convert.ToString(drSubLine["CONFIGURATION"]);
                            int iSubLineMetalType = Convert.ToInt16(drSubLine["MetalType"]);

                            if (iMainLineMetalType == iSubLineMetalType)
                            {
                                if (sSubLineConfig == sMainLineConfig)
                                {
                                    bValidIngredients = true;
                                    break;
                                }
                            }
                        }

                        foreach (DataRow drSubLine in dtSubOrderDetails.Select("UNIQUEID='" + Convert.ToString(drMainLine["UNIQUEID"]) + "'"))
                        {
                            string sSubLineConfig = Convert.ToString(drSubLine["CONFIGURATION"]);
                            int iSubLineMetalType = Convert.ToInt16(drSubLine["MetalType"]);

                            if (iMainLineMetalType == iSubLineMetalType)
                            {
                                if (sSubLineConfig == sMainLineConfig)
                                {
                                    drMainLine["MetalQty"] = decimal.Round(dMetalQty, 3, MidpointRounding.AwayFromZero);
                                }
                            }
                        }

                        drMainLine["GoldRate"] = decimal.Round(dGoldRate, 2, MidpointRounding.AwayFromZero);
                        drMainLine["SilverRate"] = decimal.Round(dSilverRate, 2, MidpointRounding.AwayFromZero);
                        drMainLine["PlatinumRate"] = decimal.Round(dPlatinumRate, 2, MidpointRounding.AwayFromZero);
                        dtItemInfo.AcceptChanges();

                    }
                    if (bValidIngredients == true)
                    {
                        frmCustOrder.dtOrderDetails = dtItemInfo;
                        frmCustOrder.dtSubOrderDetails = dtSubOrderDetails;
                        frmCustOrder.dtSampleDetails = dtSample;
                        frmCustOrder.sOrderDetailsAmount = txtTotalAmount.Text.Trim();
                        frmCustOrder.sSubOrderDetailsAmount = Convert.ToString(sExtendedDetailsAmount);
                        frmCustOrder.dtSketchDetails = dtSketch;
                        frmCustOrder.dtSampleSketch = dtSampleSketch;
                        frmCustOrder.dtRecvStoneDetails = dtStone;
                        frmCustOrder.dtPaySchedule = dtPaySchedule;
                        frmCustOrder.dtChequeInfo = dtCheque;
                        frmCustOrder.dtSVDeliveryDate = dtDeliveryDate;

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Please check line and ingredients metaltype or configuration");
                    }
                }
                else
                {

                    frmCustOrder.dtOrderDetails = dtItemInfo;
                    frmCustOrder.dtSubOrderDetails = dtSubOrderDetails;
                    frmCustOrder.dtSampleDetails = dtSample;
                    frmCustOrder.sOrderDetailsAmount = txtTotalAmount.Text.Trim();
                    frmCustOrder.sSubOrderDetailsAmount = Convert.ToString(sExtendedDetailsAmount);
                    frmCustOrder.dtSketchDetails = dtSketch;
                    frmCustOrder.dtSampleSketch = dtSampleSketch;
                    frmCustOrder.dtRecvStoneDetails = dtStone;
                    frmCustOrder.dtPaySchedule = dtPaySchedule;
                    frmCustOrder.dtChequeInfo = dtCheque;

                    this.Close();
                }
            }
            else
            {
                MessageBox.Show("Please add atleast one item into grid");
            }
        }
        #endregion

        #region MAKING RATE TEXT CHANGED
        private void txtMakingRate_TextChanged_1(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }
        #endregion

        #region EDIT CLICK
        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
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

                    cmbProductType.Text = Convert.ToString(theRowToSelect["PRODUCT"]);
                    cmbCollection.Text = Convert.ToString(theRowToSelect["COLLECTION"]);
                    cmbArticle.Text = Convert.ToString(theRowToSelect["ARTICLE"]);

                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    sBaseItemID = Convert.ToString(theRowToSelect["ITEMID"]);

                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATE"]);
                    cmbRateType.SelectedIndex = cmbRateType.FindString(Convert.ToString(theRowToSelect["RATETYPE"]).Trim());
                    txtMakingRate.Text = Convert.ToString(theRowToSelect["MAKINGRATE"]);
                    cmbMakingType.SelectedIndex = cmbMakingType.FindString(Convert.ToString(theRowToSelect["MAKINGTYPE"]).Trim());
                    txtAmount.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                    txtMakingAmount.Text = Convert.ToString(theRowToSelect["MAKINGAMOUNT"]);

                    cmbWastage.SelectedIndex = cmbWastage.FindString(Convert.ToString(theRowToSelect["WastageType"]).Trim());
                    txtWastagePercentage.Text = Convert.ToString(theRowToSelect["WastagePercentage"]);
                    txtWastageQty.Text = Convert.ToString(theRowToSelect["WastageQty"]);
                    txtWastageAmount.Text = Convert.ToString(theRowToSelect["WastageAmount"]);

                    chkBookedSKU.Checked = Convert.ToBoolean(theRowToSelect["IsBookedSKU"]);
                    chkBookedSKU.Enabled = false;

                    chkCertReq.Checked = Convert.ToBoolean(theRowToSelect["IsCertReq"]);
                    cmbCertAgent.Text = Convert.ToString(theRowToSelect["CertAgentCode"]);

                    txtRemarks.Text = Convert.ToString(theRowToSelect["RemarksDtl"]);
                    lblUnit.Text = Convert.ToString(theRowToSelect["UNITID"]);
                    txtSM.Tag = Convert.ToString(theRowToSelect["SalesManId"]);
                    txtSM.Text = getSalesManName(Convert.ToString(theRowToSelect["SalesManId"]));

                    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach (DataRow drTotal in dtItemInfo.Rows)
                        {
                            dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                            + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
                        }
                        txtTotalAmount.Text = Convert.ToString(objRounding.Round(dTotalAmount, 2));
                    }

                }
            }
        }
        #endregion

        #region DELETE CLICK
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];

                    if (IsSKUItem(Convert.ToString(theRowToDelete["ITEMID"])))
                    {
                        if (Convert.ToBoolean(theRowToDelete["IsBookedSKU"]))
                        {
                            SKUInfo(Convert.ToString(theRowToDelete["ITEMID"]), false);
                        }
                    }

                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        Decimal dTotalAmount = 0m;
                        foreach (DataRow drTotal in dtItemInfo.Rows)
                        {
                            // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["EXTENDEDDETAILS"])) ? Convert.ToDecimal(drTotal["EXTENDEDDETAILS"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                            // dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) +  (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0);
                            dTotalAmount += (!string.IsNullOrEmpty(Convert.ToString(drTotal["AMOUNT"])) ? Convert.ToDecimal(drTotal["AMOUNT"]) : 0) + (!string.IsNullOrEmpty(Convert.ToString(drTotal["MAKINGAMOUNT"])) ? Convert.ToDecimal(drTotal["MAKINGAMOUNT"]) : 0)
                                                + (!string.IsNullOrEmpty(Convert.ToString(drTotal["WastageAmount"])) ? Convert.ToDecimal(drTotal["WastageAmount"]) : 0); // Added for wastage 
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

        }
        #endregion

        #region Sample Click
        private void btnSample_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    frmCustOrderSample frm = new frmCustOrderSample(pos, application, dtSample, selectedRow + 1, dtSampleSketch);

                    frm.ShowDialog();
                    dtSample = frm.dtSample;
                    dtSampleSketch = frm.dtSketch;
                }
            }
            else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[3].Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    frmCustOrderSample frm = new frmCustOrderSample(pos, application, dsSearchedOrder.Tables[3], selectedRow + 1, dtSampleSketch, true);
                    frm.ShowDialog();
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }
        #endregion

        private string getSalesManName(string sStaffId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select d.NAME as Name from RETAILSTAFFTABLE r" +
                                " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID" +
                                " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
                                " where r.STAFFID='" + sStaffId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        #region CLear Click
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                foreach (DataRow dr in dtItemInfo.Rows)
                {
                    if (IsSKUItem(Convert.ToString(dr["ITEMID"])))
                    {
                        if (Convert.ToBoolean(dr["IsBookedSKU"]))
                        {
                            SKUInfo(Convert.ToString(dr["ITEMID"]), false);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(txtItemId.Text))
            {
                if (IsSKUItem(Convert.ToString(txtItemId.Text)))
                {
                    if (Convert.ToBoolean(chkBookedSKU.Checked))
                    {
                        SKUInfo(Convert.ToString(txtItemId.Text), false);
                    }
                }
            }
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
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                foreach (DataRow dr in dtItemInfo.Rows)
                {
                    if (IsSKUItem(Convert.ToString(dr["ITEMID"])))
                    {
                        if (Convert.ToBoolean(dr["IsBookedSKU"]))
                        {
                            SKUInfo(Convert.ToString(dr["ITEMID"]), false);
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(txtItemId.Text))
            {
                if (IsSKUItem(Convert.ToString(txtItemId.Text)))
                {
                    if (Convert.ToBoolean(chkBookedSKU.Checked))
                    {
                        SKUInfo(Convert.ToString(txtItemId.Text), false);
                    }
                }
            }
            this.Close();
        }
        #endregion

        bool isValied() // added on 05/06/2014 for after validate all req field chk the isbook or not chkbox , not before
        {
            if (ValidateControls())
            {
                if (IsSKUItem(txtItemId.Text.Trim()))
                {
                    if (chkBookedSKU.Checked)
                    {
                        if (!SKUInfo(txtItemId.Text.Trim(), true))
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is already booked / not available in counter", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            UpdateSKUInfo(txtItemId.Text.Trim());

                            chkBookedSKU.Enabled = false;
                            chkBookedSKU.Checked = false;
                            btnPOSItemSearch.Focus();
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                    return true;
            }
            else
            {
                return false;
            }
        }

        private bool SKUInfo(string sItemId, bool bMode)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if (bMode)
            {
                commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "'");
                commandText.Append(" AND isAvailable='True' AND isLocked='False' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "')");


                commandText.Append(" BEGIN SET @IsSKU = 1  UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' END SELECT @IsSKU");
            }
            else
            {
                commandText.Append("DECLARE @IsSKU AS INT; SET @IsSKU = 1; UPDATE SKUTableTrans SET isAvailable='True',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  SELECT @IsSKU");
            }


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private void UpdateSKUInfo(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("UPDATE SKUTableTrans SET isAvailable='True',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        #region ValidateControls()
        bool ValidateControls()
        {

            if (txtItemId.Text.ToUpper().Trim() == "ITEM ID" || string.IsNullOrEmpty(txtItemId.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            //if (bIsSKUItem && chkBookedSKU.Checked == false)//IsSKUItem(txtItemId.Text)
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU should be booked", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        btnPOSItemSearch.Focus();
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}

            //if (iChkSuvarnaVrudhi == 1)
            //{
            //    if (sFestiveCode != frmCustOrder.cmbSVSchemeCode.Text)
            //    {
            //        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Invalid scheme code.", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //        {
            //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //            return false;
            //        }
            //    }
            //}

            if (iChkSuvarnaVrudhi == 1 && dtItemInfo != null && dtItemInfo.Rows.Count == 1)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Only one item can be taken for Suvarna Vruddhi.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (bIsSKUItem && IsTransitItem(txtItemId.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("This SKU is in transit location.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }


            if (txtItemName.Text.ToUpper().Trim() == "ITEM NAME" || string.IsNullOrEmpty(txtItemName.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (IsCatchWtItem(txtItemId.Text.Trim()))
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
                if (Convert.ToDecimal(txtPCS.Text) <= 0)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be zero", MessageBoxButtons.OK, MessageBoxIcon.Information))
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
            if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && Convert.ToDecimal(txtRate.Text.Trim()) == 0m && !saleLineItem.ZeroPriceValid)
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
                    txtAmount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (!string.IsNullOrEmpty(txtAmount.Text.Trim()) && Convert.ToDecimal(txtAmount.Text.Trim()) == 0m && !saleLineItem.ZeroPriceValid)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can not be Zero", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtAmount.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtSM.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Sales man can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtSM.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            //if (string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            //{

            //    txtMakingRate.Text = "0";
            //}
            //if (string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Making Amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        txtMakingAmount.Focus();
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}
            else
            {
                return true;
            }
        }
        #endregion

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
            txtQuantity.Text = string.Empty;
            txtRate.Text = string.Empty;
            cmbRateType.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtMakingRate.Text = string.Empty;
            cmbMakingType.SelectedIndex = 0;
            txtMakingAmount.Text = string.Empty;

            cmbWastage.SelectedIndex = 0;
            txtWastageQty.Text = string.Empty;
            txtWastagePercentage.Text = string.Empty;
            txtWastageAmount.Text = string.Empty;
            //
            chkBookedSKU.Checked = false;
            txtRemarks.Text = string.Empty;
            lblUnit.Text = "--";
            cmbArticle.Text = "";
            cmbProductType.Text = "";
            cmbCollection.Text = "";
            chkCertReq.Checked = false;
            cmbCertAgent.Text = "";
            txtSM.Text = "";
            txtSM.Tag = "";
        }
        #endregion

        #region Key Press Events

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtQuantity.Text == string.Empty && e.KeyChar == '.')
            //{
            //    e.Handled = true;
            //}
            //else
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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtMakingRate_KeyPress(object sender, KeyPressEventArgs e)
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
        //void NumPad()
        //{
        //    if (obj != null)
        //    {
        //        obj.Dispose();
        //    }
        //    obj = new LSRetailPosis.POSProcesses.WinControls.NumPad();


        //    obj.Anchor = System.Windows.Forms.AnchorStyles.None;
        //    obj.Appearance.BackColor = System.Drawing.Color.White;
        //    obj.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        //    obj.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
        //    obj.Appearance.Options.UseBackColor = true;
        //    obj.Appearance.Options.UseFont = true;
        //    obj.Appearance.Options.UseForeColor = true;
        //    obj.AutoSize = true;
        //    obj.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        //    obj.EnteredValue = "";
        //    obj.EntryStartsInDecimals = false;
        //    obj.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.None;
        //    obj.Location = new System.Drawing.Point(738, 65);
        //    obj.LookAndFeel.SkinName = "The Asphalt World";
        //    obj.LookAndFeel.UseDefaultLookAndFeel = false;
        //    obj.MaskChar = null;
        //    obj.MaskInterval = 0;
        //    obj.MaxNumberOfDigits = 13;
        //    obj.MinimumSize = new System.Drawing.Size(170, 242);
        //    obj.Name = "numPad1";
        //    obj.NegativeMode = false;
        //    obj.NoOfTries = 0;
        //    obj.NumberOfDecimals = 3;
        //    obj.PromptText = "Enter Details: ";
        //    obj.ShortcutKeysActive = false;
        //    obj.Size = new System.Drawing.Size(170, 242);
        //    obj.TabIndex = 14;
        //    obj.TimerEnabled = false;
        //    obj.Visible = true;

        //    //numPad1.Visible = false;

        //    //obj.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(obj_EnterButtonPressed);
        //}
        #endregion

        #region NUMPAD ENTER PRESSED
        //private void numPad1_EnterButtonPressed()
        //{
        //if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
        //{
        //    txtPCS.Text = numPad1.EnteredValue;

        //    numPad1.Refresh();
        //}
        //else if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
        //{
        //    txtQuantity.Text = numPad1.EnteredValue;
        //    numPad1.Refresh();
        //}
        //else if (string.IsNullOrEmpty(txtRate.Text.Trim()))
        //{
        //    txtRate.Text = numPad1.EnteredValue;
        //    numPad1.Refresh();
        //}
        //else if (string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
        //{
        //    txtMakingRate.Text = numPad1.EnteredValue;
        //    numPad1.Refresh();
        //}
        //}
        #endregion

        #region obj_EnterButtonPressed
        //private void obj_EnterButtonPressed()
        //{
        //    if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
        //    {
        //        txtQuantity.Text = obj.EnteredValue;
        //        numPad1.Refresh();
        //    }
        //    else if (string.IsNullOrEmpty(txtRate.Text.Trim()))
        //    {
        //        txtRate.Text = obj.EnteredValue;
        //        numPad1.Refresh();
        //    }
        //    else if (string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
        //    {
        //        txtMakingRate.Text = obj.EnteredValue;
        //        numPad1.Refresh();
        //    }

        //}
        #endregion

        #region Making Rate
        private void CheckMakingRateFromDB(SqlConnection conn, string StoreID, string ItemID, string sProduct, string sCollection, string sArticle)
        {
            decimal dWtRange = 0m;
            decimal dPcs = 0;


            if (dIngrdMetalWtRange != 0)
                dWtRange = dIngrdMetalWtRange;
            else if (!string.IsNullOrEmpty(Convert.ToString(txtQuantity.Text.Trim())) && txtQuantity.Text !=".")
                dWtRange = Convert.ToDecimal(txtQuantity.Text.Trim());

            if (!string.IsNullOrEmpty(Convert.ToString(txtPCS.Text.Trim())))
                dPcs = Convert.ToInt16(txtPCS.Text.Trim());

            //if(bIsGrossMetalCal)
            //    dWtRange = Convert.ToDecimal(txtQuantity.Text.Trim());

            if (dPcs > 0)
                dWtRange = dWtRange / dPcs;


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            StoreID = pos.StoreId;

            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)" +
                                " DECLARE @ITEM VARCHAR(20)  DECLARE @PARENTITEM VARCHAR(20)   DECLARE @MFGCODE VARCHAR(20)" +  // CHANGED
                                "  DECLARE @CUSTCLASSCODE VARCHAR(20) " +
                                "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
                                "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                                "  WHERE STORENUMBER='" + StoreID + "' " +
                                "  SELECT @MFGCODE = MFG_CODE FROM INVENTTABLE WHERE ITEMID='" + ItemID + "'" +
                                "  IF EXISTS(SELECT TOP(1) * FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
                                "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +

                                " SET @PARENTITEM = ''" +
                                " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "' AND RETAIL=1)" + //ADDED
                                " BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +  // ADDED
                                " END  SET @ITEM='" + ItemID + "'" +
                //added on 09/02/16
                                "  SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + frmCustOrder.sCustAcc.Trim() + "'";

            commandText += " SELECT  TOP (1) CAST(RETAIL_SALES_AGREEMENT_DETAIL.MK_RATE AS decimal(18, 2)) , RETAIL_SALES_AGREEMENT_DETAIL.MK_TYPE " +
                           " ,WAST_TYPE,CAST(WAST_QTY AS decimal(18,2))" + // added for wastage
                           " FROM         RETAIL_SALES_AGREEMENT_DETAIL  " +

                           //" INNER JOIN (SELECT MFG_CODE FROM INVENTTABLE WHERE  ITEMID=  '" + ItemID.Trim() + "') T ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = T.MFG_CODE " +
                           " WHERE    " +

                           " (RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID = @PARENTITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID='')" +
                           " AND (RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID=@LOCATION  or RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID='')" +

                           //" AND RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = @MFGCODE" +

                           //changes on 09/02/16
                           " AND  (RETAIL_SALES_AGREEMENT_DETAIL.COLLECTIONCODE=(SELECT COLLECTIONCODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.COLLECTIONCODE='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.PRODUCTCODE=(SELECT PRODUCTTYPECODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.PRODUCTCODE='')  AND " +
                           " " +

                           " (RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='" + frmCustOrder.sCustAcc.Trim() + "' " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId=@CUSTCLASSCODE " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId='') AND " +

                           " ('" + dWtRange + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.ACTIVATE = 1 " + // ADDED ON 14.03.13 FOR ACTIVATE filter 

                           " ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC," +
                           " RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
                           " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC,RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID desc";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                txtMakingRate.Text = "0";
                isMkAdded = false;
                while (reader.Read())
                {
                    // txtMakingRate.Text = Convert.ToString(objRounding.Round(Convert.ToDecimal(reader.GetValue(0)), 2));
                    switch (Convert.ToString(reader.GetValue(1)))
                    {
                        case "0":
                            cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces));
                            break;
                        case "2":
                            cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight));
                            break;
                        case "3":
                            cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot));
                            break;
                        case "4":
                            cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));
                            break;
                        case "6":
                            cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Gross));
                            break;
                        default:
                            cmbMakingType.SelectedIndex = 2;
                            break;
                    }
                    txtMakingRate.Text = Convert.ToString(objRounding.Round(Convert.ToDecimal(reader.GetValue(0)), 2));

                    dWastQty = Convert.ToDecimal(reader.GetValue(3));

                    switch (Convert.ToString(reader.GetValue(2)))
                    {
                        case "0":
                            cmbWastage.SelectedIndex = cmbWastage.FindStringExact(Convert.ToString(WastageType.Weight));
                            break;
                        case "1":
                            cmbWastage.SelectedIndex = cmbWastage.FindStringExact(Convert.ToString(WastageType.Percentage));
                            break;
                        default:
                            cmbWastage.SelectedIndex = 0;
                            break;
                    }

                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();


        }

        private decimal getMakingRateFromDB(string StoreID, string ItemID, string sProduct, string sCollection, string sArticle, decimal dWtRange, ref int iMkRateType)
        {

            decimal dResult = 0m;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            StoreID = pos.StoreId;

            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)" +
                                " DECLARE @ITEM VARCHAR(20)  DECLARE @PARENTITEM VARCHAR(20)   DECLARE @MFGCODE VARCHAR(20)" +  // CHANGED
                                "  DECLARE @CUSTCLASSCODE VARCHAR(20) " +
                                "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
                                "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                                "  WHERE STORENUMBER='" + StoreID + "' " +
                                "  SELECT @MFGCODE = MFG_CODE FROM INVENTTABLE WHERE ITEMID='" + ItemID + "'" +
                                "  IF EXISTS(SELECT TOP(1) * FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
                                "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +

                                " SET @PARENTITEM = ''" +
                                " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "' AND RETAIL=1)" + //ADDED
                                " BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +  // ADDED
                                " END  SET @ITEM='" + ItemID + "'" +
                //added on 09/02/16
                                "  SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + frmCustOrder.sCustAcc.Trim() + "'";

            commandText += " SELECT  TOP (1) CAST(RETAIL_SALES_AGREEMENT_DETAIL.MK_RATE AS decimal(18, 2)) , RETAIL_SALES_AGREEMENT_DETAIL.MK_TYPE " +
                           " ,WAST_TYPE,CAST(WAST_QTY AS decimal(18,2))" + // added for wastage
                           " FROM         RETAIL_SALES_AGREEMENT_DETAIL  " +

                           //" INNER JOIN (SELECT MFG_CODE FROM INVENTTABLE WHERE  ITEMID=  '" + ItemID.Trim() + "') T ON RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = T.MFG_CODE " +
                           " WHERE    " +

                           " (RETAIL_SALES_AGREEMENT_DETAIL.ITEMID=@ITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID = @PARENTITEM OR RETAIL_SALES_AGREEMENT_DETAIL.ITEMID='')" +
                           " AND (RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID=@LOCATION  or RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID='')" +

                           //" AND RETAIL_SALES_AGREEMENT_DETAIL.MFG_CODE = @MFGCODE" +

                           //changes on 09/02/16
                           " AND  (RETAIL_SALES_AGREEMENT_DETAIL.COLLECTIONCODE=(SELECT COLLECTIONCODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.COLLECTIONCODE='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.PRODUCTCODE=(SELECT PRODUCTTYPECODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.PRODUCTCODE='')  AND " +
                           " " +

                           " (RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='" + frmCustOrder.sCustAcc.Trim() + "' " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId=@CUSTCLASSCODE " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId='') AND " +

                           " ('" + dWtRange + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.ACTIVATE = 1 " + // ADDED ON 14.03.13 FOR ACTIVATE filter 

                           " ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC," +
                           " RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
                           " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC,RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID desc";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                txtMakingRate.Text = "0";
                isMkAdded = false;
                while (reader.Read())
                {

                    iMkRateType = Convert.ToInt16(reader.GetValue(1));
                    dResult = Convert.ToDecimal(objRounding.Round(Convert.ToDecimal(reader.GetValue(0)), 2));

                    dWastQty = Convert.ToDecimal(reader.GetValue(3));
                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return dResult;

        }
        #endregion

        #region Get Quantity
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
        #endregion

        #region  GET Wastage Metal rate
        private decimal getWastageMetalRate(string StoreID, string sItemId, string sConfigId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            // chkOwn.Enabled = true;
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");
            //    commandText.Append(" ORDER BY [TRANSDATE] ");
            //  commandText.Append(" ,[TIME] ");
            //   commandText.Append(" DESC ");

            //   SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //   CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            //string sResult = Convert.ToString(command.ExecuteScalar());

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

        }

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
        #endregion

        private bool IsSKUItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + sItemId + "') ");
            commandText.Append(" BEGIN SET @IsSKU = 1 END SELECT @IsSKU");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private bool IsBookedItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS(select TOP(1) [SkuNumber] from SKUTable_Posted where SkuNumber='" + sItemId + "' and CUSTORDERNUM!=''");
            commandText.Append(" union ");
            commandText.Append(" select top 1 SKUNUMBER FROM RETAILCUSTOMERDEPOSITSKUDETAILS ");
            commandText.Append(" WHERE SKUNUMBER='" + sItemId + "' AND DELIVERED=0 AND RELEASED = 0)");

            commandText.Append(" BEGIN SET @IsSKU = 1 END SELECT @IsSKU");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }


        private bool IsTransitItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsTransitSKU AS INT; SET @IsTransitSKU = 0; IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "' and ToCounter='Transit') ");
            commandText.Append(" BEGIN SET @IsTransitSKU = 1 END SELECT @IsTransitSKU");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private void chkBookedSKU_CheckedChanged(object sender, EventArgs e)
        {
            //int i = 0;
            //if (chkBookedSKU.Checked)
            //{
            //    int iSkuStatus = SKUInfo(txtItemId.Text.Trim(), true);
            //    if (iSkuStatus == 2 && IsEdit == false)
            //    {
            //        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is already booked", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //        {
            //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        }
            //        chkBookedSKU.Enabled = false;
            //        chkBookedSKU.Checked = false;
            //        return;
            //    }
            //    if (iSkuStatus == 3)
            //    {
            //        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU is not in counter", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //        {
            //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        }
            //        chkBookedSKU.Enabled = false;
            //        chkBookedSKU.Checked = false;
            //        return;
            //    }

            //    // i = 1;
            //    chkBookedSKU.Enabled = false;
            //}
            //else if(chkBookedSKU.Checked == false)
            //{
            //    SKUInfo(txtItemId.Text.Trim(), false);
            //}
        }

        //private int SKUInfo(string sItemId, bool bMode)
        //{
        //    SqlConnection conn = new SqlConnection();
        //    if (application != null)
        //        conn = application.Settings.Database.Connection;
        //    else
        //        conn = ApplicationSettings.Database.LocalConnection;

        //    StringBuilder commandText = new StringBuilder();
        //    if (bMode)
        //    {
        //        //commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "'");
        //        //commandText.Append(" AND isAvailable='True' AND isLocked='False' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "') ");
        //        //commandText.Append(" BEGIN SET @IsSKU = 1  UPDATE SKUTableTrans SET isLocked='True' WHERE SkuNumber = '" + sItemId + "'");
        //        //commandText.Append(" AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' END  else SET @IsSKU = 2 SELECT @IsSKU");

        //        commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0;");
        //        commandText.Append(" IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] ");
        //        commandText.Append(" WHERE  [SkuNumber] = '" + sItemId + "')");
        //        commandText.Append(" BEGIN  ");
        //        commandText.Append("IF EXISTS   (SELECT TOP(1) [SkuNumber] FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "' ");
        //        commandText.Append("AND isLocked='False' AND isAvailable='True' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "')  ");
        //        commandText.Append("BEGIN  SET @IsSKU = 1  ");
        //        commandText.Append("UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'");
        //        commandText.Append("END  ");
        //        commandText.Append("else ");
        //        commandText.Append("SET @IsSKU = 2 ");
        //        commandText.Append("end ");
        //        commandText.Append("ELSE ");
        //        commandText.Append("SET @IsSKU = 3 SELECT @IsSKU");
        //    }
        //    else
        //    {
        //        commandText.Append("DECLARE @IsSKU AS INT; SET @IsSKU = 1; UPDATE SKUTableTrans SET isAvailable='True',isLocked='False'");
        //        commandText.Append(" WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  SELECT @IsSKU");
        //    }


        //    if (conn.State == ConnectionState.Closed)
        //        conn.Open();
        //    SqlCommand command = new SqlCommand(commandText.ToString(), conn);
        //    command.CommandTimeout = 0;

        //    int bVal = Convert.ToInt16(command.ExecuteScalar());

        //    if (conn.State == ConnectionState.Open)
        //        conn.Close();

        //    return bVal;

        //}

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

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                DataTable dt = new DataTable();

                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    DataTable dtSketchDownload = new DataTable();
                    if (dtSketch != null && dtSketch.Rows.Count > 0)
                    {
                        foreach (DataRow dtDown in dtSketch.Rows)
                        {
                            if (Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(dtDown["UNIQUEID"]))
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
                    if (oSketch.dtUploadSketch != null && oSketch.dtUploadSketch.Rows.Count > 0)
                    {
                        if (dtSketch != null && dtSketch.Rows.Count > 0)
                        {
                            foreach (DataRow dr in oSketch.dtUploadSketch.Rows)
                            {
                                foreach (DataRow drSk in dtSketch.Rows)
                                {
                                    if (Convert.ToString(dr["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
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
                        foreach (DataRow drSk in dtSketch.Rows)
                        {
                            if (Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                            {
                                drSk.Delete();
                                dtSketch.AcceptChanges();
                                break;
                            }
                        }
                    }
                }
            }
            else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[1].Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
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

        public void UpdatelocalSampleTable(DataTable dt)
        {
            frmCustOrder.dtSampleDetails = dt;
            dtSample = dt;
        }

        private void btnSampleReturn_Click(object sender, EventArgs e)
        {
            frmCustOrderSampleReturn frmSamRet = new frmCustOrderSampleReturn(pos, application, dtSample, this);
            frmSamRet.ShowDialog();
        }

        private void btnRcvStone_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    if (dtStone.Rows.Count == 0)
                    {
                        dtStone = frmCustOrder.dtRecvStoneDetails;
                    }
                    frmCustOrderSampleStoneStone frm = new frmCustOrderSampleStoneStone(pos, application, dtStone, selectedRow + 1);

                    frm.ShowDialog();
                    dtStone = frm.dtRecvStoneDetails;
                }
            }
            else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[4].Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    frmCustOrderSampleStoneStone frm = new frmCustOrderSampleStoneStone(pos, application, dsSearchedOrder.Tables[4], selectedRow + 1);

                    frm.ShowDialog();
                }

            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
        }

        private void btnPaySchedule_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    //frmCustOrderSample frm = new frmCustOrderSample(pos, application, dtSample, selectedRow + 1, dtSampleSketch);
                    frmCustOrderPaySchedule frmPay = new frmCustOrderPaySchedule(pos, application, dtPaySchedule, Convert.ToDecimal(txtTotalAmount.Text));

                    frmPay.ShowDialog();
                    dtPaySchedule = frmPay.dtPaySched;
                }
            }
            else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[5].Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    frmCustOrderPaySchedule frmPay = new frmCustOrderPaySchedule(pos, application, dsSearchedOrder.Tables[5], Convert.ToDecimal(txtTotalAmount.Text), 1);
                    frmPay.ShowDialog();
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }

        private decimal GetMRPPrice(string sItemId, SqlConnection connection)
        {

            //List<DE.PriceDiscTable> tradeAgreements = new List<DE.PriceDiscTable>();
            //string itemRelation = args.GetItemRelation(itemCode);
            //IList<string> accountRelations = args.GetAccountRelations(accountCode);
            //string unitId = args.GetUnitId(itemCode);
            DateTime today = DateTime.Now.Date;

            decimal dAmount = 0;
            string dataAreaId = ApplicationSettings.Database.DATAAREAID;

            bool isIndia = Functions.CountryRegion == SupportedCountryRegion.IN;
            try
            {
                // convert account relations list to data-table for use as TVP in the query.
                string queryString = @"
                        SELECT 
                            ta.PRICEUNIT,
                            ta.ALLOCATEMARKUP,
                            ta.AMOUNT,
                            ta.SEARCHAGAIN"
                            + (isIndia ? ", ta.MAXIMUMRETAILPRICE_IN" : string.Empty)
                            + @"
                        FROM 
                            PRICEDISCTABLE ta LEFT JOIN 
                                INVENTDIM invdim ON ta.INVENTDIMID = invdim.INVENTDIMID AND ta.DATAAREAID = invdim.DATAAREAID
                        WHERE
                            ta.RELATION = 4
                            AND ta.ITEMCODE = @ITEMCODE
                            AND ta.ITEMRELATION = @ITEMRELATION
                            AND ta.ACCOUNTCODE = @ACCOUNTCODE
                    
                            -- USES Tvp: CREATE TYPE FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE AS TABLE(ACCOUNTRELATION nvarchar(20) NOT NULL);
                            --AND (ta.ACCOUNTRELATION) IN (SELECT ar.ACCOUNTRELATION FROM @ACCOUNTRELATIONS ar)

                            AND ta.CURRENCY = @CURRENCYCODE
                            AND ta.UNITID = @UNITID
                            AND ta.QUANTITYAMOUNTFROM <= abs(@QUANTITY)
                            AND (ta.QUANTITYAMOUNTTO >= abs(@QUANTITY) OR ta.QUANTITYAMOUNTTO = 0)
                            AND ta.DATAAREAID = @DATAAREAID
                            AND ((ta.FROMDATE <= @TODAY OR ta.FROMDATE <= @NODATE)
                                    AND (ta.TODATE >= @TODAY OR ta.TODATE <= @NODATE))
                            AND (invdim.INVENTCOLORID in (@COLORID, ''))
                            AND (invdim.INVENTSIZEID in (@SIZEID,''))
                            AND (invdim.INVENTSTYLEID in (@STYLEID, ''))
                            AND (invdim.CONFIGID in (@CONFIGID, ''))

                            --// ORDERBY CLAUSE MUST MATCH THAT IN AX TO ENSURE COMPATIBLE PRICING BEHAVIOR.
                            --// SEE THE CLASS PRICEDISC.FINDPRICEAGREEMENT() AND TABLE PRICEDISCTABLE.PRICEDISCIDX
                            order by ta.AMOUNT, ta.QUANTITYAMOUNTFROM, ta.QUANTITYAMOUNTTO, ta.FROMDATE";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);
                    command.Parameters.AddWithValue("@ITEMCODE", 0);
                    command.Parameters.AddWithValue("@ITEMRELATION", sItemId);
                    command.Parameters.AddWithValue("@ACCOUNTCODE", 2);
                    command.Parameters.AddWithValue("@UNITID", unitid);
                    command.Parameters.AddWithValue("@CURRENCYCODE", ApplicationSettings.Terminal.CompanyCurrency);
                    command.Parameters.AddWithValue("@QUANTITY", 1);
                    command.Parameters.AddWithValue("@COLORID", ("") ?? string.Empty);
                    command.Parameters.AddWithValue("@SIZEID", (cmbSize.Text) ?? string.Empty);
                    command.Parameters.AddWithValue("@STYLEID", ("") ?? string.Empty);
                    command.Parameters.AddWithValue("@CONFIGID", (cmbConfig.Text) ?? string.Empty);
                    command.Parameters.AddWithValue("@TODAY", today);
                    command.Parameters.AddWithValue("@NODATE", DateTime.Parse("1900-01-01"));

                    // Fill out TVP for account relations list
                    using (DataTable accountRelationsTable = new DataTable())
                    {
                        accountRelationsTable.Columns.Add("ACCOUNTRELATION", typeof(string));
                        //foreach(string relation in accountRelations)
                        //{
                        //    accountRelationsTable.Rows.Add(relation); 
                        //}

                        SqlParameter param = command.Parameters.Add("@ACCOUNTRELATIONS", SqlDbType.Structured);
                        param.Direction = ParameterDirection.Input;
                        param.TypeName = "FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE";
                        param.Value = accountRelationsTable;

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        SqlDataReader reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dAmount = reader.GetDecimal(reader.GetOrdinal("AMOUNT"));
                            }
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return dAmount;
        }

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

        private bool IsBulkItem(string sItemId)
        {
            bool bBulkItem = false;

            string commandText = "DECLARE @ISBULK INT  SET @ISBULK = 0 IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN SET @ISBULK = 1 END SELECT @ISBULK";

            SqlCommand cmd = new SqlCommand();
            SqlConnection myCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            myCon.Open();

            try
            {
                if (myCon.State == ConnectionState.Closed)
                    myCon.Open();
                cmd = new SqlCommand(commandText, myCon);
                bBulkItem = Convert.ToBoolean(cmd.ExecuteScalar());
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

            return bBulkItem;
        }

        private int getOrderStoreDeliveryDays()
        {
            int iResult = 0;

            string commandText = "SELECT CUSTORDSTOREDELIVERYDAYS from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand cmd = new SqlCommand();
            SqlConnection myCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            myCon.Open();

            try
            {
                if (myCon.State == ConnectionState.Closed)
                    myCon.Open();
                cmd = new SqlCommand(commandText, myCon);
                iResult = Convert.ToInt16(cmd.ExecuteScalar());
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

            return iResult;
        }


        private void cmbArticle_SelectedIndexChanged(object sender, EventArgs e)
        {
            //cmbArticle.Text = cmbArticle.ValueMember;
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            string sSQl = " SELECT distinct PRODUCTCODE CODE, DESCRIPTION from PRODUCTTYPEMASTER";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Product type Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbProductType.Text = string.Empty;
                cmbProductType.Text = Convert.ToString(drRes["CODE"]);
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

        private void btnCollection_Click(object sender, EventArgs e)
        {
            string sSQl = "SELECT distinct COLLECTIONCODE CODE , COLLECTIONDESC DESCRIPTION from COLLECTIONMASTER";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Collection Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbCollection.Text = string.Empty;
                cmbCollection.Text = Convert.ToString(drRes["CODE"]);
            }
        }

        private void btnArticle_Click(object sender, EventArgs e)
        {
            string sbQuery = " SELECT distinct ARTICLE_CODE CODE, [DESCRIPTION] from ARTICLE_MASTER";

            DataTable dtResult = NIM_LoadCombo("", "", "", sbQuery);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Article Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbArticle.Text = string.Empty;
                cmbArticle.Text = Convert.ToString(drRes["CODE"]);
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

                txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
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
                txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
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
                txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
            }
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
                txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
            }
        }

        private void chkCertReq_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCertReq.Checked)
                btnAgent.Enabled = true;
            else
            {
                btnAgent.Enabled = false;
                cmbCertAgent.Text = "";
            }
        }

        private void btnAgent_Click(object sender, EventArgs e)
        {
            string sSQl = " select CERTIFICATIONCODE CODE,DESCRIPTION  FROM CRWCERTIFICATIONAGENCY ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Agent Search");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbCertAgent.Text = string.Empty;
                cmbCertAgent.Text = Convert.ToString(drRes["CODE"]);
            }
        }

        private void btnChequeInfo_Click(object sender, EventArgs e)
        {
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    frmCustomerOrderChequeInfo frmChk = new frmCustomerOrderChequeInfo(pos, application, dtCheque, Convert.ToDecimal(txtTotalAmount.Text));

                    frmChk.ShowDialog();
                    dtCheque = frmChk.dtCheque;
                }
            }
            else if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[5].Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];

                    frmCustomerOrderChequeInfo frmChk = new frmCustomerOrderChequeInfo(pos, application, dsSearchedOrder.Tables[6], Convert.ToDecimal(txtTotalAmount.Text), 1);
                    frmChk.ShowDialog();
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the sample details or there are no sample present for the selected item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }

        private void btnStockSample_Click(object sender, EventArgs e)
        {

        }

        private string getStoreFormatCode()
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(STOREFORMAT,'') STOREFORMAT FROM RETAILSTORETABLE WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            string sStoreFormat = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sStoreFormat;

        }

        private void btnSM_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtSM.Tag = dialog6.SelectedEmployeeId;
                txtSM.Text = dialog6.SelectEmployeeName;
            }
        }

        private void btnReBookSKU_Click(object sender, EventArgs e)
        {
            if (dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0)// && dsSearchedOrder.Tables[2].Rows.Count > 0
            {
                if (grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];

                    string sRowItem = Convert.ToString(drRowToUpdate["ItemId"]);
                    int iRowItemMetalType = GetMetalType(Convert.ToString(drRowToUpdate["ItemId"]));
                    int iRowOrderLine = Convert.ToInt16(drRowToUpdate["LineNum"]);
                    string sRowItemConfigId = Convert.ToString(drRowToUpdate["ConfigId"]);
                    bool bRowItemBooked = Convert.ToBoolean(drRowToUpdate["IsBookedSKU"]);

                    #region Item Search
                    Dialog objdialog = new Dialog();
                    string str = string.Empty;
                    DataSet dsItem = new DataSet();

                    objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) ");

                    saleLineItem = new SaleLineItem();

                    if (dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                        {
                            saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);

                            Item objItem = new Item();
                            objItem.MYProcessItem(saleLineItem, application);

                            Dimension objDim = new Dimension();
                            DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);

                            if (dtDimension != null && dtDimension.Rows.Count > 0)
                            {
                                DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                                dimConfirmation.InventDimCombination = dtDimension;
                                dimConfirmation.DimensionData = saleLineItem.Dimension;

                                if (IsSKUItem(saleLineItem.ItemId))
                                {
                                    int iSelectedItemMetalType = GetMetalType(saleLineItem.ItemId);
                                    string sSelectItemConfigId = Convert.ToString(dtDimension.Rows[0]["configid"].ToString());

                                    if (iRowItemMetalType == iSelectedItemMetalType && sRowItemConfigId == sSelectItemConfigId)
                                    {
                                        bool isAvailable = isSKUAvailableForBook(saleLineItem.ItemId);
                                        bool isReleased = isSKUReleased(sRowItem, frmCustOrder.sCustOrderSearchNumber);
                                        bool isReBooked = isAlreadyReBooked(frmCustOrder.sCustOrderSearchNumber, iRowOrderLine);

                                        if (bRowItemBooked)
                                        {
                                            if (isAvailable && isReleased && !isReBooked)
                                            {
                                                ReBookSKUinHO(saleLineItem.ItemId, frmCustOrder.sCustOrderSearchNumber, iRowOrderLine);
                                            }
                                            else if (!isAvailable)
                                                MessageBox.Show("" + saleLineItem.ItemId + " is not in stock");
                                            else if (!isReleased)
                                                MessageBox.Show("There is already a booked sku against the order " + frmCustOrder.sCustOrderSearchNumber + "");
                                            else if (isReBooked)
                                                MessageBox.Show("There is already a re-booked sku against the order " + frmCustOrder.sCustOrderSearchNumber + " and Line " + iRowOrderLine + "");
                                        }
                                        else
                                        {
                                            if (isAvailable && !isReBooked)
                                            {
                                                ReBookSKUinHO(saleLineItem.ItemId, frmCustOrder.sCustOrderSearchNumber, iRowOrderLine);
                                            }
                                            else if (isReBooked)
                                                MessageBox.Show("There is already a re-booked sku against the order " + frmCustOrder.sCustOrderSearchNumber + " and Line " + iRowOrderLine + "");
                                            else
                                                MessageBox.Show("" + saleLineItem.ItemId + " is not in stock");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Different config / Metal item is not allowed.");
                                    }
                                }

                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please enter at least one row to enter the details or No Ingredients Exists.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
        }

        private void ReBookSKUinHO(string sSKU, string sOrderNo, int iOrderLine)
        {
            bool bResult = false;
            try
            {
                if (this.application.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = this.application.TransactionServices.InvokeExtension("ReBookSKU", sSKU, sOrderNo, iOrderLine);

                    bResult = Convert.ToBoolean(containerArray[1]);
                    if (bResult)
                    {
                        ReBookSKU(sSKU, sOrderNo, iOrderLine);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format("Failed to release SKU from HO due to RTS issue."));
            }
            if (bResult == false)
            {
                MessageBox.Show(string.Format("Failed to release SKU from HO."));
            }
        }

        private void ReBookSKU(string sSKU, string sOrderNo, int iOrderLine)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "  UPDATE SKUTABLE_POSTED SET CUSTORDERNUM ='" + sOrderNo + "'," +
                                    " CUSTORDERLINENUM=" + iOrderLine + " WHERE SKUNUMBER='" + sSKU + "'";


                SqlComm.ExecuteNonQuery();
                SqlComm.CommandText = "";
                SqlComm.CommandTimeout = 0;
                SqlComm.Dispose();

                MessageBox.Show("SKU Re-Booking successful");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool isSKUAvailableForBook(string sSKU)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select A.isAvailable from SKUTableTrans A");
            sbQuery.Append(" left join SKUTable_Posted B on A.SkuNumber =B.SkuNumber");
            sbQuery.Append(" where b.CUSTORDERLINENUM =0 and b.CUSTORDERNUM='' and A.SkuNumber='" + sSKU + "' and A.TOCOUNTER!='Transit'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public bool isSKUReleased(string sSKU, string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string CommandText = "select RELEASED FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE" +
                                     " SKUNUMBER='" + sSKU + "' AND DELIVERED=0 and ORDERNUMBER='" + sOrderNo + "'";

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(CommandText.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public bool isAlreadyReBooked(string sOrderNo, int iLine)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string CommandText = "select isnull(CUSTORDERNUM,'') from SKUTable_Posted where CUSTORDERNUM='" + sOrderNo + "' and CUSTORDERLINENUM=" + iLine + "";

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(CommandText.ToString(), connection);
            string sCustOrder = Convert.ToString(cmd.ExecuteScalar());

            if (!string.IsNullOrEmpty(sCustOrder))
                return true;
            else
                return false;
        }

        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            //txtQuantity.Text = string.Format("{0:0.000}", txtQuantity.Text);
            //txtQuantity.Text = string.Format("0.000", txtQuantity.Text);
            //txtQuantity.Text = String.Format(Convert.ToDecimal(txtQuantity.Text) % 1 == 0 ? "{0:0}" : "{0:0.00}", txtQuantity.Text);
        }

        private void grItems_Click(object sender, EventArgs e)
        {

        }

    }

}
