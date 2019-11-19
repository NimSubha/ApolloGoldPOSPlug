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

namespace BlankOperations.WinFormsTouch
{
    /// <summary>
    /// 
    /// </summary>
    public partial class frmCustOrderSampleStoneStone:frmTouchBase
    {
        #region Variable Declaration
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;

        DataTable dtItemInfo_stone = new DataTable("dtItemInfo_stone");
        decimal linenum = 0;
        Random randUnique = new Random();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataSet dsSearchedOrder = new DataSet();
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        public DataTable dtRecvStoneDetails = new DataTable();
        string unitid = string.Empty;
        string Previewdimensions = string.Empty;
        decimal dStoneWtRange = 0m;
        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;        //
        string sBookedSKUItem = string.Empty;
        string sRecvStoneID = string.Empty;
        string sOrderNum = string.Empty;
        frmCustomerOrder frmCustOrd;
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

        #region GetCustOrdStone_AutoID  Is Order saved or not
        private string ReturnRcvStoneIDByOrderNum(string OrdNum)
        {

            string commandText = "SELECT [ORDERNUM] from CUSTORDSTONE WHERE ORDERNUM='" + OrdNum.Trim() + "'";

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

        #region frmCustOrderSampleStone
        public frmCustOrderSampleStoneStone(IPosTransaction posTransaction, IApplication Application, decimal line)
        {
            InitializeComponent();
            // Enable_DisableAddViewBtn();
            pos = posTransaction;
            application = Application;
            linenum = line;
            btnPOSItemSearch.Focus();
        }

        public frmCustOrderSampleStoneStone(IPosTransaction posTransaction, IApplication Application, DataTable dt, decimal line, bool isView = false)
        {
            InitializeComponent();
            //Enable_DisableAddViewBtn();
            pos = posTransaction;
            application = Application;
            this.dtRecvStoneDetails = dt;
            frmCustOrd = new frmCustomerOrder(posTransaction, Application);
            sOrderNum = frmCustOrd.txtOrderNo.Text;
            DataTable dtTempShow = new DataTable();

            if(dt.Rows.Count > 0)
            {
                sOrderNum = dt.Rows[0]["OrderNum"].ToString();
                this.sRecvStoneID = ReturnRcvStoneIDByOrderNum(sOrderNum);
            }
            else
            {
                this.sRecvStoneID = ReturnRcvStoneIDByOrderNum(sOrderNum);
            }
            if(dtItemInfo_stone == null || dtItemInfo_stone.Columns.Count == 0)
            {
                dtItemInfo_stone = dtRecvStoneDetails.Clone();
                
                //dtRecvStoneDetails.Select("LINENUM=" + line).CopyToDataTable(dtItemInfo_stone, LoadOption.PreserveChanges);
            }
            if(dtRecvStoneDetails.Rows.Count > 0)
            {
                //foreach(DataRow dr in dtRecvStoneDetails.Rows)
                //{
                    //line = Convert.ToDecimal(dr["LINENUM"].ToString());
                dtRecvStoneDetails.Select("REFLINENUM=" + line).CopyToDataTable(dtItemInfo_stone, LoadOption.PreserveChanges);
                //}
            }


            if(!string.IsNullOrEmpty(sRecvStoneID.Trim()))
            {
                btnAddItem.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
                btnSubmit.Enabled = false;
            }
            else
            {
                btnAddItem.Enabled = true;
            }
            grItems.DataSource = dtItemInfo_stone.DefaultView; 
           // dtItemInfo_stone = dtRecvStoneDetails;
            if(isView)
            {
                btnSubmit.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
            linenum = line;
            btnPOSItemSearch.Focus();
        }
        #endregion

        #region Dimenssion info
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

            //if (!string.IsNullOrEmpty(cmbStyle.Text))
            //{
            //    if (colorSizeStyleConfig.Length > 0)
            //    {
            //        colorSizeStyleConfig.Append(dash);
            //    }
            //    colorSizeStyleConfig.Append(" Style : " + cmbStyle.Text);
            //}

            //if (!string.IsNullOrEmpty(cmbConfig.Text))
            //{
            //    if (colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
            //    colorSizeStyleConfig.Append(" Configuration : " + cmbConfig.Text);
            //}

            return colorSizeStyleConfig.ToString();
        }
        #endregion

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
            Dialog objdialog = new Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            //  objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) "); // blocked on 12.09.2013 // SKU allow

            // objdialog.MyItemSearch(500, ref str, out  dsItem, "");
            objdialog.MyItemSearch(5000, ref str, out  dsItem, " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE CustomerStn=1) ");// " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE CustomerStn=1)");// METALTYPE IN (" + (int)MetalType.Stone + " , " + (int)MetalType.LooseDmd + ") and

            saleLineItem = new SaleLineItem();
            string MetalID = string.Empty;
            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);

                    Item objItem = new Item();
                    objItem.MYProcessItem(saleLineItem, application);

                    Dimension objDim = new Dimension();
                    DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);


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
                            //cmbConfig.DataSource = dtConfig;
                            //  cmbConfig.DisplayMember = "ConfigID";
                            //cmbConfig.ValueMember = "ConfigValue";

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
                            //cmbConfig.Enabled = false;
                            cmbCode.Enabled = false;
                            cmbSize.Enabled = false;
                            cmbStyle.Enabled = false;

                            Previewdimensions = ColorSizeStyleConfig();
                        }
                    }

                    else
                    {
                        cmbStyle.Text = string.Empty;
                        //cmbConfig.Text = string.Empty;
                        cmbCode.Text = string.Empty;
                        cmbSize.Text = string.Empty;
                        // cmbConfig.Enabled = false;
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
                    txtPCS.Text = "1";
                    //string sQty = string.Empty;
                    //sQty = GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                    //if (!string.IsNullOrEmpty(sQty))
                    //{
                    //    sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                    //    txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    //}
                    txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    //txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                    //if (!string.IsNullOrEmpty(txtRate.Text))
                    //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");

                    unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                    //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();
                    lblUnit.Text = unitid;

                    if(Convert.ToString(txtItemId.Text.Trim()) == string.Empty)    //  || Convert.ToString(cmbConfig.Text.Trim()) == string.Empty
                    {
                        sBaseItemID = string.Empty;
                        sBaseConfigID = string.Empty;
                    }
                    else
                    {
                        sBaseItemID = txtItemId.Text.Trim();
                        //sBaseConfigID = cmbConfig.Text;
                    }
                }
            }
            txtNetWt.Enabled = true;
            txtGrossWt.Enabled = true;
            //string Metal_Type = getMetalType_InventTable(sBaseItemID);
            //if (Metal_Type ==((int) MetalType.Stone).ToString())
            //{
            //    txtDiaAmt.Enabled = false;
            //    txtGrossWt.Enabled = false;
            //    txtStnAmt.Enabled = true;
            //    txtNetWt.Enabled = true;
            //}
            //else
            //{
            //    txtDiaAmt.Enabled = true; ;
            //    txtGrossWt.Enabled = true;
            //    txtStnAmt.Enabled = false;
            //    txtNetWt.Enabled = false;
            //}

        }
        #endregion

        #region GET METAL_TYPE FROM INVENTTABLE
        private string getMetalType_InventTable(string ItemID)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;


            if(conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            string commandText = "select MetalType from InventTable where ItemID='" + ItemID + "'";
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            return sResult;
        }
        #endregion

        #region  GET RATE FROM METAL TABLE
        public string getRateFromMetalTable(string itemid, string configuration, string batchid, string colorid, string sizeid, string weight, string pcs = null)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + itemid.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //   }
            commandText.Append(" SELECT TOP 1 CAST(RATES AS numeric(26,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + configuration.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            if(string.IsNullOrEmpty(weight))
            {
                weight = "0";
            }
            string ldweight = string.Empty;
            ldweight = "0";
            if(pcs != null && Convert.ToDecimal(pcs) != Convert.ToDecimal(0))   // Convert.ToDecimal(pcs) != Convert.ToDecimal(0) added as per BOM.PDSCWQTY added in BOM on request of urvi and arunava 
            {
                ldweight = Convert.ToString(decimal.Round((Convert.ToDecimal(weight) / Convert.ToDecimal(pcs)), 3, MidpointRounding.AwayFromZero)); //Changes on 11/04/2014 R.Hossain
                //dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
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
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + ldweight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + itemid.Trim() + "') AND   "); //(INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND
            //commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid.Trim() + "') ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            // commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            commandText.Append(" ELSE ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + weight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + itemid.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND  ");
            //commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid.Trim() + "') ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            //   }

            //    SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //    CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;


            if(conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
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
            if(string.IsNullOrEmpty(GrWeight))
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
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        #region Enable or disable Add Item to View Button
        private void Enable_DisableAddViewBtn()
        {

            if(ValidateControls())
            {
                btnAddItem.Enabled = true;
            }
            else
            {
                btnAddItem.Enabled = false;
            }


        }
        #endregion

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            //decimal dStnAmt = 0m;
            //decimal dDiaAmt = 0m;
            if(ValidateControls())
            {
                DataRow dr;
                if(IsEdit == false && dtItemInfo_stone != null && dtItemInfo_stone.Rows.Count == 0 && dtItemInfo_stone.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo_stone.Columns.Add("LINENUM", typeof(decimal));
                    dtItemInfo_stone.Columns.Add("REFLINENUM", typeof(decimal));                    
                    dtItemInfo_stone.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo_stone.Columns.Add("ITEMNAME", typeof(string));
                    //dtItemInfo_stone.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo_stone.Columns.Add("COLOR", typeof(string));
                    dtItemInfo_stone.Columns.Add("SIZE", typeof(string));
                    dtItemInfo_stone.Columns.Add("STYLE", typeof(string));
                    dtItemInfo_stone.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo_stone.Columns.Add("QTY", typeof(decimal));

                    dtItemInfo_stone.Columns.Add("NETWT", typeof(decimal));
                    dtItemInfo_stone.Columns.Add("UNITID", typeof(string));
                    dtItemInfo_stone.Columns.Add("OrderNum", typeof(string));

                    dtItemInfo_stone.Columns.Add("REMARKSDTL", typeof(string));
                    dtItemInfo_stone.Columns.Add("ISRETURNED", typeof(string));
                    dtItemInfo_stone.Columns.Add("RecvDate", typeof(DateTime));
                    dtItemInfo_stone.Columns.Add("STONEBATCHID", typeof(string));

                }
                if(IsEdit == false)
                {
                    int lastRowIndex = dtItemInfo_stone.Rows.Count;
                    dr = dtItemInfo_stone.NewRow();
                    dr["LINENUM"] = lastRowIndex + 1; //dr["LINENUM"] = linenum;
                    dr["REFLINENUM"] = linenum;
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    // dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    dr["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                    dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = objRounding.Round(Convert.ToDecimal(txtPCS.Text.Trim()), 0);
                    else
                        dr["PCS"] = objRounding.Round(decimal.Zero, 0);

                    if(!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                        dr["QTY"] = objRounding.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3);
                    else
                        dr["QTY"] = objRounding.Round(decimal.Zero, 3);


                    if(!string.IsNullOrEmpty(txtNetWt.Text.Trim()))
                        dr["NETWT"] = objRounding.Round(Convert.ToDecimal(txtNetWt.Text.Trim()), 3);
                    else
                        dr["NETWT"] = objRounding.Round(decimal.Zero, 3);

                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;

                    dr["OrderNum"] = sOrderNum;

                    dr["REMARKSDTL"] = txtRemarks.Text.Trim();
                    dr["ISRETURNED"] = "False";
                    dr["STONEBATCHID"] = sOrderNum + "/" + linenum + "/" + (lastRowIndex + 1);

                    dtItemInfo_stone.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo_stone.DefaultView;
                }
                if(IsEdit == true)
                {
                    DataRow EditRow = dtItemInfo_stone.Rows[EditselectedIndex];
                    EditRow["PCS"] = txtPCS.Text.Trim();
                    if(!string.IsNullOrEmpty(txtNetWt.Text.Trim()))
                        EditRow["NETWT"] = decimal.Round(Convert.ToDecimal(txtNetWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["NETWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                        EditRow["QTY"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["QTY"] = objRounding.Round(decimal.Zero, 3);

                    EditRow["REMARKSDTL"] = txtRemarks.Text.Trim();
                    EditRow["ISRETURNED"] = "False";
                    dtItemInfo_stone.AcceptChanges();

                    grItems.DataSource = dtItemInfo_stone.DefaultView;
                    IsEdit = false;
                }

                ClearControls();
            }
            //if (dtItemInfo.Rows.Count == 0)
            //{
            btnAddItem.Enabled = true;
            //}
            //else if (dtItemInfo.Rows.Count == 1)
            //{
            //    btnAddItem.Enabled = false;
            //}
            //btnEdit.Enabled = false;
        }
        #endregion

        #region Submmit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            //frmCustOrder.dtOrderDetails = dtItemInfo;
            //frmCustOrder.dtSubOrderDetails = dtSubOrderDetails;
            //frmCustOrder.dtRecvStoneDetailsDetails = dtRecvStoneDetails;
            ////frmCustOrder.sOrderDetailsAmount = txtTotalAmount.Text.Trim();
            //frmCustOrder.sSubOrderDetailsAmount = Convert.ToString(sExtendedDetailsAmount);
            //dtRecvStoneDetails = dtItemInfo;
            if(dtRecvStoneDetails.Columns.Count == 0)
            {
                dtRecvStoneDetails = dtItemInfo_stone.Clone();
            }
            foreach(DataRow dr in dtItemInfo_stone.Rows)
            {

                if(dtRecvStoneDetails.Rows.Count > 0)
                {
                    DataRow[] drSam = dtRecvStoneDetails.Select("REFLINENUM=" + dr["REFLINENUM"] + " AND LINENUM=" + dr["LINENUM"]);
                    if(drSam.Count() > 0)
                    {
                        //drSam[0]["PCS"] = dr["PCS"];
                        //drSam[0]["QTY"] = dr["QTY"];
                        //drSam[0]["NETWT"] = dr["NETWT"];
                        ////drSam[0]["DIAWT"] = dr["DIAWT"];
                        ////drSam[0]["DIAAMT"] = dr["DIAAMT"];
                        ////drSam[0]["STNWT"] = dr["STNWT"];
                        ////drSam[0]["STNAMT"] = dr["STNAMT"];
                        ////drSam[0]["TOTALAMT"] = dr["TOTALAMT"];
                        //drSam[0]["RemarksDtl"] = dr["RemarksDtl"];


                        foreach(var r in drSam)
                            dtRecvStoneDetails.Rows.Remove(r);

                        dtRecvStoneDetails.ImportRow(dr);
                    }
                    else
                    {
                        dtRecvStoneDetails.ImportRow(dr);
                    }
                }
                else
                {
                    dtRecvStoneDetails.ImportRow(dr);
                }

            }
            btnAddItem.Enabled = false;
            IsEdit = false;
            this.Close();

        }
        #endregion

        #region EDIT CLICK
        private void btnEdit_Click(object sender, EventArgs e)
        {

            // string MetalType = getMetalType_InventTable(txtItemId.Text);
            dtItemInfo_stone = dtRecvStoneDetails;
            if(dtItemInfo_stone != null && dtItemInfo_stone.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtItemInfo_stone.Rows[EditselectedIndex];
                    //cmbStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                    cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    // cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    //  txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);

                    //txtNettWt.Text = Convert.ToString(theRowToSelect["NETTWT"]);
                    txtGrossWt.Text = Convert.ToString(theRowToSelect["QTY"]);
                    //txtDiaAmt.Text =Convert.ToString(decimal.Round(Convert.ToDecimal(Convert.ToString(theRowToSelect["DIAAMT"])), 2, MidpointRounding.AwayFromZero));
                    //txtDiaAmt.Text = Convert.ToString(theRowToSelect["DIAAMT"]);
                    txtNetWt.Text = Convert.ToString(theRowToSelect["NETWT"]);
                    //txtStnAmt.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(Convert.ToString(theRowToSelect["STNAMT"])), 2, MidpointRounding.AwayFromZero)); //Convert.ToString(theRowToSelect["STNAMT"]);
                    //txtStnAmt.Text = Convert.ToString(theRowToSelect["STNAMT"]);
                    //txtTotalAmt.Text = Convert.ToString(theRowToSelect["TOTALAMT"]);
                    //string Metal_Type = getMetalType_InventTable(txtItemId.Text);
                    //if(Metal_Type == ((int)MetalType.Stone).ToString())
                    //{
                    //    txtDiaAmt.Enabled = false;
                    //    txtGrossWt.Enabled = false;
                    //    txtStnAmt.Enabled = true;
                    //    txtNetWt.Enabled = true;
                    //}
                    //else
                    //{
                    //    txtDiaAmt.Enabled = true; ;
                    //    txtGrossWt.Enabled = true;
                    //    txtStnAmt.Enabled = false;
                    //    txtNetWt.Enabled = false;
                    //}

                    txtRemarks.Text = Convert.ToString(theRowToSelect["RemarksDtl"]);

                    btnAddItem.Enabled = true;
                }
            }
        }
        #endregion

        #region DELETE CLICK
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            dtItemInfo_stone = dtRecvStoneDetails;
            if(dtItemInfo_stone != null && dtItemInfo_stone.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo_stone.Rows[DeleteSelectedIndex];


                    dtItemInfo_stone.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo_stone.DefaultView;
                }

            }
            if(DeleteSelectedIndex == 0 && dtItemInfo_stone != null && dtItemInfo_stone.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo_stone.Clear();
            }
            IsEdit = false;

            if(dtItemInfo_stone.Rows.Count == 0)
            {
                btnAddItem.Enabled = true;
            }
            else if(dtItemInfo_stone.Rows.Count == 1)
            {
                btnAddItem.Enabled = false;
            }
        }
        #endregion

        #region CLear Click
        private void btnClear_Click(object sender, EventArgs e)
        {
            saleLineItem = new SaleLineItem();
            objRounding = new Rounding();
            dtItemInfo_stone = new DataTable();
            randUnique = new Random();
            IsEdit = false;
            EditselectedIndex = 0;
            dsSearchedOrder = new DataSet();
            grItems.DataSource = null;
            ClearControls();
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
            sBaseItemID = txtItemId.Text;
            string Metal_Type = getMetalType_InventTable(sBaseItemID);

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
            if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPCS.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if((!string.IsNullOrEmpty(txtPCS.Text.Trim())) && (Convert.ToInt32(txtPCS.Text) < 1))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank or less than 1", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPCS.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
           
            if((string.IsNullOrEmpty(txtGrossWt.Text.Trim())))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity should be non-zero positive quantity.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtGrossWt.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if((!string.IsNullOrEmpty(txtNetWt.Text.Trim())) && (Convert.ToDecimal(txtNetWt.Text) <= 0))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Net Weight should be non-zero positive quantity.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtNetWt.Focus();
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

        #region ClearControls
        private void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            cmbCode.Text = string.Empty;
            //cmbStyle.Text = string.Empty;
            cmbSize.Text = string.Empty;
            // cmbConfig.Text = string.Empty;
            txtPCS.Text = string.Empty;
            //txtQuantity.Text = string.Empty;
            // txtNettWt.Text = string.Empty;
            txtGrossWt.Text = string.Empty;
            //txtDiaAmt.Text = string.Empty;
            txtNetWt.Text = string.Empty;
            //txtStnAmt.Text = string.Empty;
            //txtTotalAmt.Text = string.Empty;
            txtRemarks.Text = string.Empty;
            lblUnit.Text = "";

            if(dtItemInfo_stone.Rows.Count == 0)
            {
                btnAddItem.Enabled = true;
            }
            else if(dtItemInfo_stone.Rows.Count >= 1)
            {
                btnAddItem.Enabled = false;
            }
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

        //private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtNettWt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtDiaWt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}
        //private void txtDiaAmt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtStnWt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}
        //private void txtStnAmt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtTotalAmt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}
        private void GetAfterDecimalFixValue(TextBox txtBox, Int16 iVal)
        {
            string word = txtBox.Text.Trim();
            string[] wordArr = word.Split('.');
            if(wordArr.Length > 1)
            {
                string afterDot = wordArr[1];
                if(afterDot.Length > iVal)
                {
                    txtBox.Text = wordArr[0] + "." + afterDot.Substring(0, iVal);
                    txtBox.Select(txtBox.Text.Length, 0);
                }
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
            obj.Location = new System.Drawing.Point(738, 65);
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

            numPad1.Visible = false; // CHANGES THE NAME OF nUMBER PAD.

            obj.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(obj_EnterButtonPressed);
        }
        #endregion

        #region NUMPAD ENTER PRESSED
        private void numPad1_EnterButtonPressed()
        {
            if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                txtPCS.Text = numPad1.EnteredValue;

                numPad1.Refresh();
            }
            //else if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            //{
            //    txtQuantity.Text = numPad1.EnteredValue;
            //    numPad1.Refresh();
            //}

        }
        #endregion

        #region obj_EnterButtonPressed
        private void obj_EnterButtonPressed()
        {
            //if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            //{
            //    txtQuantity.Text = obj.EnteredValue;
            //    numPad1.Refresh();
            //}

        }
        #endregion

        #region Get Quantity
        public string GetStandardQuantityFromDB(SqlConnection conn, string itemid)
        {
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT STDQTY FROM INVENTTABLE WHERE ITEMID='" + itemid + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sQty = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return sQty;

        }
        #endregion

        #region SKU item Checking
        private bool IsSKUItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKUTable_Posted] WHERE  [SkuNumber] = '" + sItemId + "') ");
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
        #endregion

        private void txtDiaWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            GetAfterDecimalFixValue(txtGrossWt, 2);
        }
                

        private void txtStnWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            GetAfterDecimalFixValue(txtNetWt, 2);
        }
    }

}
