using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Printing;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmGetEstimateion:frmTouchBase
    {
        public DataTable dtIngredients = null;
        public DataTable dtIngredientsClone = null;
        SqlConnection connection;
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;

        string unitid = string.Empty;
        string Previewdimensions = string.Empty;
        bool isItemExists = false;


        decimal dIngrdMetalWtRange = 0m;
        decimal dIngrdTotalGoldQty = 0m;
        decimal dIngrdTotalGoldValue = 0m;
        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;
        Decimal dWMetalRate = 0m;
        decimal dWastQty = 0m; // Added for wastage
        decimal dMakingDiscDbAmt = 0m;
        string ItemID = string.Empty;
        string StoreID = string.Empty;
        string ConfigID = string.Empty;
        string ColorID = string.Empty;
        string BatchID = string.Empty;
        string SizeID = string.Empty;
        string GrWeight = string.Empty;

        string MRPUCP = string.Empty;
        bool isMRPUCP = false;
        string sBaseUnitId = string.Empty;

        decimal dCustomerOrderFixedRate = 0m;
        int iCallfrombase = 0;
        decimal dStoneWtRange = 0m;
        bool bIsGrossMetalCal = false;

        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                // InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        //internal static IApplication InternalApplication { get; private set; }


        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        string sCustomerId = string.Empty;

        RetailTransaction retailTrans;
        [Import]
        private IApplication application;


        public frmGetEstimateion(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            retailTrans = posTransaction as RetailTransaction;
            application = Application;

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if(retailTrans != null)
            {
                if((string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                    && (string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName))))
                {
                    MessageBox.Show("Customer not selected");
                }
                else
                    sCustomerId = retailTrans.Customer.CustomerId;
            }
        }
        enum WastageType
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }

        enum CRWMakingCalcType
        {
            Net = 1,
            Gross = 2,
            NetChargeable = 3,
            Blank = 4,
        }

        enum MakingDiscType
        {
            Percentage = 0,
            Weight = 1,
            Amount = 2,

        }

        enum RateType
        {
            Weight = 0,
            Pieces = 1,
            Tot = 2,
        }

        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }


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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            saleLineItem = new SaleLineItem();
            saleLineItem.ItemId = "";
            saleLineItem.ItemId = Convert.ToString(txtSKU.Text);

            if(IsMetalRateCalcTypeGross(sBaseItemID))
                bIsGrossMetalCal = true;
            else
                bIsGrossMetalCal = false;

            if(GetValidItem(saleLineItem.ItemId))
            {
                GetItemForEstimation();

                RetailTransaction retailTrans = pos as RetailTransaction;
                decimal dNetWt = 0;

                DataTable dtDetail = new DataTable("Detail");

                DataTable dtIngredient = new DataTable("Ingredient");
                DataRow drDtl;
                DataRow drIngrd;

                dtDetail.Columns.Add("ITEMID", typeof(string));
                dtDetail.Columns.Add("LINENUM", typeof(int));
                dtDetail.Columns.Add("MAKINGAMOUNT", typeof(decimal));
                dtDetail.Columns.Add("MakingDisc", typeof(decimal));
                dtDetail.Columns.Add("WastageAmount", typeof(decimal));
                dtDetail.AcceptChanges();

                dtIngredient.Columns.Add("SKUNUMBER", typeof(string));
                dtIngredient.Columns.Add("ITEMID", typeof(string));
                dtIngredient.Columns.Add("LINENUM", typeof(int));
                dtIngredient.Columns.Add("REFLINENUM", typeof(int));

                dtIngredient.Columns.Add("InventSizeID", typeof(string));
                dtIngredient.Columns.Add("InventColorID", typeof(string));
                dtIngredient.Columns.Add("ConfigID", typeof(string));

                dtIngredient.Columns.Add("UnitID", typeof(string));
                dtIngredient.Columns.Add("METALTYPE", typeof(int));

                dtIngredient.Columns.Add("QTY", typeof(decimal));
                dtIngredient.Columns.Add("PCS", typeof(decimal));
                dtIngredient.Columns.Add("CRATE", typeof(decimal));
                dtIngredient.Columns.Add("CVALUE", typeof(decimal));
                dtIngredient.Columns.Add("INGRDDISCAMT", typeof(decimal));
                dtIngredient.AcceptChanges();

                int i = 1;

                drDtl = dtDetail.NewRow();

                string sMkAmt = "0";
                string sWasAmt = "0";
                string sMkDiscTot = "0";

                if(!string.IsNullOrEmpty(txtMakingAmount.Text))
                    sMkAmt = txtMakingAmount.Text;

                if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                    sMkDiscTot = txtMakingDiscTotAmt.Text;

                if(!string.IsNullOrEmpty(txtWastageAmount.Text))
                    sWasAmt = txtWastageAmount.Text;

                drDtl["ITEMID"] = saleLineItem.ItemId;
                drDtl["LINENUM"] = i;
                drDtl["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(sMkAmt), 2, MidpointRounding.AwayFromZero);
                drDtl["MakingDisc"] = decimal.Round(Convert.ToDecimal(sMkDiscTot), 2, MidpointRounding.AwayFromZero);
                drDtl["WastageAmount"] = decimal.Round(Convert.ToDecimal(sWasAmt), 2, MidpointRounding.AwayFromZero);

                dtDetail.Rows.Add(drDtl);
                dtDetail.AcceptChanges();

                if(dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
                {
                    int iGrd = 1;

                    foreach(DataRow dr in dtIngredientsClone.Rows)
                    {
                        drIngrd = dtIngredient.NewRow();

                        drIngrd["SKUNUMBER"] = Convert.ToString(dr["SKUNUMBER"]);
                        drIngrd["ITEMID"] = Convert.ToString(dr["ITEMID"]);
                        drIngrd["LINENUM"] = iGrd;

                        drIngrd["REFLINENUM"] = i;
                        drIngrd["InventSizeID"] = Convert.ToString(dr["InventSizeID"]);
                        drIngrd["InventColorID"] = Convert.ToString(dr["InventColorID"]);
                        drIngrd["ConfigID"] = Convert.ToString(dr["ConfigID"]);

                        drIngrd["UnitID"] = Convert.ToString(dr["UnitID"]);
                        drIngrd["METALTYPE"] = Convert.ToInt32(dr["METALTYPE"]);

                        if(iGrd == 1)
                            dNetWt = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain

                        drIngrd["QTY"] = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                        drIngrd["PCS"] = decimal.Round(Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                        drIngrd["CRATE"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                        if(isMRPUCP == true)
                        {// added on 31/03/2014 and mod on 25/04/14
                            if(iGrd == 1) // added on 08/07/2014-- for esteemate of mrp item-- bom value should not come
                                //drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(retailTrans.NetAmountWithNoTax), 2, MidpointRounding.AwayFromZero);
                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(lblRate.Text), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(dr["CVALUE"]), 2, MidpointRounding.AwayFromZero);
                        }

                        drIngrd["INGRDDISCAMT"] = (!string.IsNullOrEmpty(Convert.ToString(dr["IngrdDiscTotAmt"])) ? Convert.ToDecimal(dr["IngrdDiscTotAmt"]) : 0);
                        //decimal.Round(Convert.ToDecimal(dr["IngrdDiscTotAmt"]), 2, MidpointRounding.AwayFromZero);

                        dtIngredient.Rows.Add(drIngrd);
                        dtIngredient.AcceptChanges();

                        iGrd++;
                    }

                }

                i++;

                if((dtDetail != null && dtDetail.Rows.Count > 0)
                && (dtIngredient != null && dtIngredient.Rows.Count > 0))
                {
                    DataRow drLIngrd;

                    foreach(DataRow dr in dtDetail.Rows)
                    {
                        drLIngrd = dtIngredient.NewRow();
                        drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                        drLIngrd["ITEMID"] = "Labour";
                        drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                        drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]);
                        drLIngrd["InventSizeID"] = string.Empty;
                        drLIngrd["InventColorID"] = string.Empty;
                        drLIngrd["ConfigID"] = string.Empty;

                        drLIngrd["UnitID"] = string.Empty;
                        drLIngrd["METALTYPE"] = 0;

                        drLIngrd["QTY"] = 0;
                        drLIngrd["PCS"] = 0;
                        drLIngrd["CRATE"] = Convert.ToString(decimal.Round((Convert.ToDecimal(sMkAmt) + Convert.ToDecimal(sWasAmt)) / Convert.ToDecimal(dNetWt), 2, MidpointRounding.AwayFromZero));
                        drLIngrd["CVALUE"] = Convert.ToDecimal(sMkAmt) + Convert.ToDecimal(sWasAmt);
                        drLIngrd["INGRDDISCAMT"] = Convert.ToDecimal(sMkDiscTot);
                        dtIngredient.Rows.Add(drLIngrd);
                        dtIngredient.AcceptChanges();
                    }
                }
                if((dtDetail != null && dtDetail.Rows.Count > 0)
                   && (dtIngredient != null && dtIngredient.Rows.Count > 0))
                {

                    frmEstimationReport objEstimationReport = new frmEstimationReport(pos, dtDetail, dtIngredient);
                    objEstimationReport.ShowDialog();
                }
                else
                {
                    #region Nimbus
                    FormModulation formMod = new FormModulation(ApplicationSettings.Database.LocalConnection);
                    RetailTransaction retailTransaction = pos as RetailTransaction;
                    if(retailTransaction != null)
                    {
                        ICollection<Point> signaturePoints = null;
                        if(retailTransaction.TenderLines != null
                            && retailTransaction.TenderLines.Count > 0
                            && retailTransaction.TenderLines.First.Value != null)
                        {
                            signaturePoints = retailTransaction.TenderLines.First.Value.SignatureData;
                        }
                        FormInfo formInfo = formMod.GetInfoForForm(Microsoft.Dynamics.Retail.Pos.Contracts.Services.FormType.QuotationReceipt, false, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);
                        formMod.GetTransformedTransaction(formInfo, retailTransaction);

                        string textForPreview = formInfo.Header;
                        textForPreview += formInfo.Details;
                        textForPreview += formInfo.Footer;
                        textForPreview = textForPreview.Replace("|1C", string.Empty);
                        textForPreview = textForPreview.Replace("|2C", string.Empty);
                        frmReportList preview = new frmReportList(textForPreview, signaturePoints);
                        LSRetailPosis.POSControls.POSFormsManager.ShowPOSForm(preview);
                    }
                    #endregion
                }
            }
        }

        public bool isMRP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        private void frmGetEstimateion_Load(object sender, EventArgs e)
        {

        }

        private bool IsMetalRateCalcTypeGross(string sSKUNo)
        {
            int bStatus = 0;
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            string sQry = "SELECT CRWMetalRateCalcType FROM INVENTTABLE WHERE ITEMID = '" + sSKUNo + "'";


            using(SqlCommand cmd = new SqlCommand(sQry, connection))
            {
                cmd.CommandTimeout = 0;
                bStatus = Convert.ToInt16(cmd.ExecuteScalar());
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(bStatus == (int)CRWMakingCalcType.Gross)
                return true;
            else
                return false;

        }

        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            Dialog.Dialog objdialog = new Dialog.Dialog();

            string str = string.Empty;
            DataSet dsItem = new DataSet();
            objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) "); // blocked on 12.09.2013 // SKU allow

            saleLineItem = new SaleLineItem();

            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    GetItemForEstimation();
                }

            }
            else
            {
                isItemExists = false;
            }
        }

        private void GetItemForEstimation()
        {
            if(GetValidItem(saleLineItem.ItemId))
            {
                //frmGetEstimateion.InternalApplication.Services.Item.ProcessItem(saleLineItem, true);
                Item.Item objItem = new Item.Item();

                objItem.MYProcessItem(saleLineItem, application);
                //=====================
                Dimension.Dimension objDim = new Dimension.Dimension();
                DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);

                if(dtDimension != null && dtDimension.Rows.Count > 0)
                {
                    DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                    dimConfirmation.InventDimCombination = dtDimension;
                    dimConfirmation.DimensionData = saleLineItem.Dimension;

                    if(!IsSKUItem(saleLineItem.ItemId))
                    {
                        MessageBox.Show("Invalid SKU.");
                    }
                    else
                    {
                        cmbSize.Text = dtDimension.Rows[0]["sizeid"].ToString();
                        cmbConfig.Text = dtDimension.Rows[0]["configid"].ToString();
                    }
                }
                //===============
                bool isMRPExists = isMRP(saleLineItem.ItemId, connection);
                string sMRPRate = string.Empty;
                BindRateTypeMakingTypeCombo();
                BindWastage();
                BindMakingDiscount();

                ItemID = saleLineItem.ItemId;
                StoreID = ApplicationSettings.Database.StoreID;
                ConfigID = string.IsNullOrEmpty(Convert.ToString(cmbConfig.Text)) ? "" : Convert.ToString(cmbConfig.Text);
                MRPUCP = sMRPRate;
                isMRPUCP = isMRPExists;


                sBaseItemID = saleLineItem.ItemId;
                sBaseConfigID = ConfigID;
                sBaseUnitId = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                SaleLineItem saleLine = (SaleLineItem)saleLineItem;
                if(isMRPExists)
                {
                    //RetailTransaction posClone = retailTrans as RetailTransaction;

                    //RetailTransaction pos = retailTrans.CloneTransaction() as RetailTransaction;
                    //pos.Add(saleLine);
                    //application.Services.Price.GetPrice(pos);
                    //SaleLineItem saleItemforMrp = pos.SaleItems.Last.Value;
                    decimal dAmt = GetMRPPrice(saleLineItem.ItemId);
                    sMRPRate = Convert.ToString(dAmt);
                }


                BindIngredientGrid();

                FillQtyPcsFromSKUTable();

                iCallfrombase = 1;

                iCallfrombase = 0;


                if(isMRPUCP)
                {
                    lblRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(sMRPRate), 2, MidpointRounding.AwayFromZero));
                    cmbRateType.SelectedIndex = (int)RateType.Tot;   // On Request of Urvi
                }


                txtQuantity.Text = !string.IsNullOrEmpty(txtQuantity.Text) ? decimal.Round(Convert.ToDecimal(txtQuantity.Text), 3, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                lblRate.Text = !string.IsNullOrEmpty(lblRate.Text) ? decimal.Round(Convert.ToDecimal(lblRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtPCS.Text = !string.IsNullOrEmpty(txtPCS.Text) ? decimal.Round(Convert.ToDecimal(txtPCS.Text), 0, MidpointRounding.AwayFromZero).ToString() : string.Empty;

                if(!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                {
                    CheckRateFromDB();
                    CheckMakingDiscountFromDB();
                }

                txtAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;

                // Added  -- wastage
                if((!string.IsNullOrEmpty(lblRate.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                {
                    if((Convert.ToDecimal(lblRate.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                    {
                        lblRate.Text = Convert.ToString(Convert.ToDecimal(lblRate.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                    }
                }
                //if(!string.IsNullOrEmpty(lblRate.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                //{
                //    lblRate.Text = Convert.ToString(Convert.ToDecimal(lblRate.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));
                //}
                if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text)
                && !string.IsNullOrEmpty(lblRate.Text))
                {
                    Decimal decimalAmount = 0m;
                    //decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                    decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(lblRate.Text));
                    lblRate.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
            }
        }

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

        void BindRateTypeMakingTypeCombo()
        {
            cmbRateType.DataSource = Enum.GetValues(typeof(RateType));
            cmbMakingType.DataSource = Enum.GetValues(typeof(MakingType));
        }

        #region Qty and Pcs Fill up
        private void FillQtyPcsFromSKUTable()
        {
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = " SELECT     TOP (1) SKUTableTrans.PDSCWQTY AS PCS , SKUTableTrans.QTY AS QTY " + //SKU Table New
                                    " FROM         SKUTableTrans " +
                                    " WHERE     (SKUTableTrans.SkuNumber = '" + ItemID.Trim() + "') ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    txtPCS.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(reader.GetValue(0)), 3, MidpointRounding.AwayFromZero));
                    txtQuantity.Text = Convert.ToString(reader.GetValue(1));

                }
            }
            if(connection.State == ConnectionState.Open)
                connection.Close();
        }
        #endregion

        private void BindIngredientGrid()
        {
            dtIngredients = new DataTable();

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder commandText = new StringBuilder();
            commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + ItemID.Trim() + "')");
            commandText.Append("  BEGIN  ");

            commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
            // commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(SKULine_Posted.PDSCWQTY AS INT) AS PCS, CAST(SKULine_Posted.QTY AS NUMERIC(16,3)) AS QTY,  ");
            commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
            commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
            //  commandText.Append(" ,0 AS IngrdDiscType,0 AS IngrdDiscAmt,0 AS IngrdDiscTotAmt "); 
            commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
            commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
            commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' ORDER BY X.METALTYPE END ");


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            dtIngredients.Load(reader);

            if(dtIngredients != null && dtIngredients.Rows.Count > 0)
            {
                #region // Stone Discount

                dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                dtIngredients.Columns.Add("CTYPE", typeof(int));

                #endregion

                lblRate.Text = string.Empty;
                dtIngredientsClone = new DataTable();
                dtIngredientsClone = dtIngredients.Clone();

                foreach(DataRow drClone in dtIngredients.Rows)
                {
                    if(isMRPUCP)
                    {
                        drClone["CValue"] = 0;
                        drClone["Rate"] = 0;
                        drClone["CTYPE"] = 0;
                        drClone["IngrdDiscType"] = 0;
                        drClone["IngrdDiscAmt"] = 0;
                        drClone["IngrdDiscTotAmt"] = 0;
                    }
                    dtIngredientsClone.ImportRow(drClone);
                }

                if(!isMRPUCP)
                {
                    foreach(DataRow dr in dtIngredientsClone.Rows)
                    {
                        string item = ItemID;
                        string config = ConfigID;
                        ConfigID = string.Empty;
                        ItemID = string.Empty;
                        ConfigID = Convert.ToString(dr["ConfigID"]);
                        ItemID = Convert.ToString(dr["ItemID"]);
                        BatchID = Convert.ToString(dr["InventBatchID"]);
                        ColorID = Convert.ToString(dr["InventColorID"]);
                        SizeID = Convert.ToString(dr["InventSizeID"]);
                        GrWeight = Convert.ToString(dr["QTY"]);
                        string sRate = string.Empty;
                        string sCalcType = "";

                        if(Convert.ToDecimal(dr["PCS"]) > 0)
                            dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                        else
                            dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);

                        // Stone Discount
                        int iStoneDiscType = 0;
                        decimal dStoneDiscAmt = 0;
                        decimal dStoneDiscTotAmt = 0m;

                        if(retailTrans == null)
                            sRate = getRateFromMetalTable();
                        else
                            sRate = getRateFromMetalTable(retailTrans.Customer.CustomerId);


                        if(!string.IsNullOrEmpty(sRate))
                        {
                            sCalcType = GetIngredientCalcType(ref iStoneDiscType, ref dStoneDiscAmt);
                            if(!string.IsNullOrEmpty(sCalcType))
                                dr["CTYPE"] = Convert.ToInt32(sCalcType);
                            else
                                dr["CTYPE"] = 0;

                            dr["Rate"] = sRate;
                            ItemID = item;
                            ConfigID = config;
                            BatchID = string.Empty;
                            ColorID = string.Empty;
                            SizeID = string.Empty;
                            GrWeight = string.Empty;
                        }
                        else
                        {
                            dr["Rate"] = "0"; // Added on 08.08.2013 -- Instructed by Urminavo Das 
                            // if not rate found make it 0 -- validation related issues will be raised by RGJL in CR
                            dr["CTYPE"] = 0;

                            ItemID = item;
                            ConfigID = config;
                            BatchID = string.Empty;
                            ColorID = string.Empty;
                            SizeID = string.Empty;
                            GrWeight = string.Empty;
                        }

                        // If Ingredient item is LooseDmd or Stone and stone rate is 0 -- cancel the operation 

                        if((Convert.ToDecimal(dr["Rate"]) <= 0)
                            && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd
                                || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                        {
                            using(LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("0 Stone Rate is not valid for this item ", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                            }
                            this.Close();
                            return;
                        }

                        StringBuilder commandText1 = new StringBuilder();
                        commandText1.Append("select metaltype from inventtable where itemid='" + Convert.ToString(dr["ItemID"]) + "'");

                        if(connection.State == ConnectionState.Closed)
                            connection.Open();

                        SqlCommand command1 = new SqlCommand(commandText1.ToString(), connection);
                        command1.CommandTimeout = 0;
                        SqlDataReader reader1 = command1.ExecuteReader();
                        if(reader1.HasRows)
                        {
                            while(reader1.Read())
                            {
                                //---- add code --  -- for CalcType
                                if(((int)reader1.GetValue(0) == (int)MetalType.LooseDmd) || ((int)reader1.GetValue(0) == (int)MetalType.Stone))
                                {

                                    if(!string.IsNullOrEmpty(sCalcType))
                                    {
                                        #region // Stone Discount Calculation


                                        #endregion

                                        if(Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Weight))
                                        {
                                            if(dStoneDiscAmt > 0)
                                            {
                                                dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                            }
                                            dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }

                                        else if(Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Pieces))
                                        {
                                            if(dStoneDiscAmt > 0)
                                            {
                                                dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["PCS"]), Convert.ToDecimal(dr["Rate"]));
                                            }
                                            dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }

                                        else // Tot
                                        {
                                            if(dStoneDiscAmt > 0)
                                            {
                                                dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                            }
                                            dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    else
                                    {
                                        if(dStoneDiscAmt > 0)
                                        {
                                            dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                        }
                                        dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                    }
                                }
                                else
                                {
                                    dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                }

                                if((int)reader1.GetValue(0) == (int)MetalType.Gold)
                                {
                                    txtgval.Text = (string.IsNullOrEmpty(lblRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtgval.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                }
                            }
                        }
                        if(connection.State == ConnectionState.Open)
                            connection.Close();

                        // Stone Discount
                        if(dStoneDiscAmt > 0)
                        {
                            dr["IngrdDiscType"] = iStoneDiscType;
                            dr["IngrdDiscAmt"] = dStoneDiscAmt;
                            dr["IngrdDiscTotAmt"] = decimal.Round(dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            dr["IngrdDiscType"] = 0;
                            dr["IngrdDiscAmt"] = 0;
                            dr["IngrdDiscTotAmt"] = 0;
                        }
                        lblRate.Text = (string.IsNullOrEmpty(lblRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(lblRate.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                    }

                    dtIngredientsClone.AcceptChanges();

                    cmbRateType.SelectedIndex = (int)RateType.Tot;
                    cmbRateType.Enabled = false;

                    // Added on 07.06.2013
                    txtPCS.Enabled = false;
                    txtQuantity.Enabled = false;
                    lblRate.Enabled = false;
                    txtMakingRate.Enabled = false;
                    txtMakingDisc.Enabled = false;
                    cmbMakingType.Enabled = false;
                }
                else
                {
                    lblRate.Text = MRPUCP;
                }
            }
            else
            {
                if(!isMRPUCP)
                {
                    if(retailTrans == null)
                        lblRate.Text = getRateFromMetalTable();
                    else
                        lblRate.Text = getRateFromMetalTable(retailTrans.Customer.CustomerId);
                }
                else
                    lblRate.Text = MRPUCP;
            }
            if(connection.State == ConnectionState.Open)
                connection.Close();
        }

        void BindWastage()
        {
            cmbWastage.DataSource = Enum.GetValues(typeof(WastageType));
        }

        void BindMakingDiscount()
        {
            cmbMakingDiscType.DataSource = Enum.GetValues(typeof(MakingDiscType));
        }

        private decimal CalcStoneDiscount(decimal dStnDiscAmt, int iStnDiscType, decimal dStnDiscWt, decimal dStnRate)
        {
            decimal dStnDiscTotAmt = 0m;

            if(iStnDiscType == 0) // Percentage
            {
                dStnDiscTotAmt = (dStnDiscAmt / 100) * (dStnDiscWt * dStnRate);
            }
            else if(iStnDiscType == 1) // Weight
            {
                dStnDiscTotAmt = dStnDiscAmt * dStnDiscWt;
            }
            else if(iStnDiscType == 2)
            {
                dStnDiscTotAmt = dStnDiscAmt;
            }

            return dStnDiscTotAmt;
        }

        private string getRateFromMetalTable(string sCustAcc = null)
        {

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            if(dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
                commandText.Append(" BEGIN ");
            }

            if(string.IsNullOrEmpty(sCustAcc))
            {
                commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
                commandText.Append(" AND METALTYPE=@METALTYPE ");
            }
            else
            {
                //if(IsSpecialCustomer(sCustAcc))
                //{
                //    commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM CUSTMETALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
                //    commandText.Append(" AND METALTYPE=@METALTYPE AND CUSTOMERACC='" + sCustAcc + "'");
                //}
                //else
                //{
                commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
                commandText.Append(" AND METALTYPE=@METALTYPE ");
                //}
            }



            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "'");

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + ConfigID.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            string grweigh = string.Empty;
            if(string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            if(dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                commandText.Append(" END ");
                commandText.Append(" ELSE ");
                commandText.Append(" BEGIN ");

                commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
                commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
                commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
                commandText.Append(dStoneWtRange);
                commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
                commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
                commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
                commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
                commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

                commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // added on 02.09.2013
                commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
                commandText.Append(" END ");
            }

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return Convert.ToString(sResult.Trim());

        }

        private bool GetValidItem(string sItemId)
        {
            bool bValidItem = true;

            int? isLocked = null;
            int? isAvailable = null;
            int? isReturnLocked = null;
            int? isReturnAvailable = null;
            CheckForReturnSKUExistence(sItemId, connection, out isReturnLocked, out isReturnAvailable);

            if(isReturnAvailable == null && isReturnLocked == null)
            {
                CheckForSKUExistence(sItemId, connection, out isLocked, out isAvailable);
                if(isLocked != null && isAvailable != null)
                {
                    if(Convert.ToBoolean(isLocked))
                    {
                        bValidItem = false;
                        MessageBox.Show("Item is currently locked");
                        string query = string.Empty;
                        query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                        isItemAvailableORReturn(query);
                    }
                    else if(!Convert.ToBoolean(isLocked) && !Convert.ToBoolean(isAvailable))
                    {
                        bValidItem = false;

                        MessageBox.Show("Item is not available for sale at this point of time.");
                        string query = string.Empty;
                        query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                        isItemAvailableORReturn(query);
                    }
                    else
                    {
                        bValidItem = true;
                    }

                }
                else
                {
                    bValidItem = false;
                }

            }
            else if(isReturnAvailable == 1 && isReturnLocked == 1)
            {
                bValidItem = false;
                MessageBox.Show("Item is not available for sale at this point of time.");
                string query = string.Empty;
                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                isItemAvailableORReturn(query);

            }
            else if(isReturnAvailable == 1 && isReturnLocked == 0)
            {
                bValidItem = false;
            }
            else if((isReturnAvailable == 0 && isReturnLocked == 0))
            {
                bValidItem = false;
            }
            else
                bValidItem = true;

            return bValidItem;

        }

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
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
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
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");

            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();
            if(!string.IsNullOrEmpty(sResult))
                return Convert.ToString(objRounding.Round(Convert.ToDecimal(sResult.Trim())));
            else
                return string.Empty;

        }
        #endregion

        public string GetInventID(string distinctProductVariantID)
        {
            string commandText = "select top(1)  INVENTDIMID from assortedinventdimcombination WHERE DISTINCTPRODUCTVARIANT='" + distinctProductVariantID + "'";

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            return Convert.ToString(command.ExecuteScalar());

        }

        #region Get Quantity
        public string GetStandardQuantityFromDB(SqlConnection connection, string itemid)
        {
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT STDQTY FROM INVENTTABLE WHERE ITEMID='" + itemid + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string sQty = Convert.ToString(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return sQty;

        }
        #endregion

        private void CheckForSKUExistence(string itemid, SqlConnection connection, out int? isLock, out int? isAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SkuNumber='" + itemid + "') ");
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" SELECT isAvailable,isLocked FROM SKUTableTrans WHERE SkuNumber='" + itemid + "' ");
            sbQuery.Append(" END ");
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();


            if(reader.HasRows)
            {
                bool isAvail = false;
                bool isLocked = false;
                while(reader.Read())
                {
                    isAvail = Convert.ToBoolean(reader.GetValue(0));
                    isLocked = Convert.ToBoolean(reader.GetValue(1));
                }
                reader.Close();
                isLock = Convert.ToInt16(isLocked);
                isAvailable = Convert.ToInt16(isAvail);

            }
            else
            {
                reader.Close();
                isLock = null;
                isAvailable = null;
            }

        }

        private void CheckForReturnSKUExistence(string itemid, SqlConnection connection, out int? isRetunLock, out int? isReturnAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            sbQuery.Append(" DECLARE @RETURN BIT ");
            sbQuery.Append(" SELECT @RETURN=ITEMRETURN FROM RETAILTEMPTABLE WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "); // RETAILTEMPTABLE
            sbQuery.Append(" IF(@RETURN=1) ");
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "')  ");
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" SELECT isLocked,isAvailable FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "'  ");
            sbQuery.Append(" END  ");
            sbQuery.Append(" END  ");


            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                bool isAvail = false;
                bool isLocked = false;
                while(reader.Read())
                {
                    isLocked = Convert.ToBoolean(reader.GetValue(0));
                    isAvail = Convert.ToBoolean(reader.GetValue(1));
                }
                reader.Close();
                isRetunLock = Convert.ToInt16(isLocked);
                isReturnAvailable = Convert.ToInt16(isAvail);
            }
            else
            {
                reader.Close();
                isRetunLock = null;
                isReturnAvailable = null;
            }

        }

        private void isItemAvailableORReturn(string query)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }

        private string GetIngredientCalcType(ref int iSDisctype, ref decimal dSDiscAmt)  // modified on 30.04.2013 // Stone Discount
        {
            string sResult = string.Empty;
            StringBuilder commandText = new StringBuilder();

            string grweigh = string.Empty;
            if(string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            commandText.Append(" SELECT   TOP 1  ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE,ISNULL(DISCTYPE,0) AS DISCTYPE, ISNULL(DISCAMT,0) AS DISCAMT ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    sResult = Convert.ToString(reader.GetValue(0));
                    iSDisctype = Convert.ToInt32(reader.GetValue(1));
                    dSDiscAmt = Convert.ToDecimal(reader.GetValue(2));
                }
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return Convert.ToString(sResult.Trim());

        }



        private void CheckRateFromDB(bool IsFreezedRate = false)
        {
            string sWtRange = string.Empty;
            decimal dTotWt = 0m;
            if(connection.State == ConnectionState.Open)
                connection.Close();

            // Quantity calculation //// NOT Consider Multi Metal in Ingredient [Order by Metaltype in Ingredient]
            if(dtIngredients != null && dtIngredients.Rows.Count > 0)
            {
                int iIngMetalType;

                if(bIsGrossMetalCal)
                {
                    foreach(DataRow dr in dtIngredients.Rows)
                    {
                        dTotWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                    }
                    sWtRange = Convert.ToString(dTotWt);
                }
                else
                {
                    iIngMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    if((iIngMetalType == (int)MetalType.Gold) || (iIngMetalType == (int)MetalType.Silver)
                        || (iIngMetalType == (int)MetalType.Platinum) || (iIngMetalType == (int)MetalType.Palladium))
                    {
                        sWtRange = Convert.ToString(dtIngredients.Rows[0]["Qty"]);
                    }
                    else
                    {
                        sWtRange = txtQuantity.Text.Trim();
                    }
                }


            }
            else
            {
                sWtRange = txtQuantity.Text.Trim();
            }


            #region New for Samara changes
            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)   DECLARE @ITEM VARCHAR(20)  DECLARE @PARENTITEM VARCHAR(20)   DECLARE @MFGCODE VARCHAR(20)" +  // CHANGED
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
                                "  SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "'";

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

                           " (RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='" + sCustomerId.Trim() + "' " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.AccountNum='') AND " +
                           " (RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId=@CUSTCLASSCODE " +
                           " OR RETAIL_SALES_AGREEMENT_DETAIL.CustClassificationId='') AND " +

                           " ('" + sWtRange + "' BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROM_WEIGHT AND RETAIL_SALES_AGREEMENT_DETAIL.TO_WEIGHT)  " +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE AND RETAIL_SALES_AGREEMENT_DETAIL.TODATE)  " +
                           " AND RETAIL_SALES_AGREEMENT_DETAIL.ACTIVATE = 1 " + // ADDED ON 14.03.13 FOR ACTIVATE filter 

                           "  ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC,RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
                           " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC";
            #endregion

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal dWastQty = 0m;
            using(SqlDataReader reader = command.ExecuteReader())
            {
                if(reader.HasRows)
                {
                    while(reader.Read())
                    {
                        txtChangedMakingRate.Text = Convert.ToString(reader.GetValue(0));
                        txtActMakingRate.Text = Convert.ToString(reader.GetValue(0));

                        // txtManualAmount.Enabled = true;
                        switch(Convert.ToString(reader.GetValue(1)))
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
                            default:
                                cmbMakingType.SelectedIndex = 2;
                                break;
                        }

                        dWastQty = Convert.ToDecimal(reader.GetValue(3));

                        switch(Convert.ToString(reader.GetValue(2)))
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

                        //
                    }
                }

            }
            if(connection.State == ConnectionState.Open)
                connection.Close();


            // --------- check MFG_CODE AND SALE_COMPLEXITYCODE  against the item if Making Rate is 0 // 30.08.2013

            if(string.IsNullOrEmpty(txtChangedMakingRate.Text))
            {
                txtChangedMakingRate.Text = "0";
                txtActMakingRate.Text = "0";
            }

            if((Convert.ToDecimal(txtChangedMakingRate.Text) <= 0) && (dWastQty <= 0))
            {
                // get sku -- Metal Type
                int iSKUMetaltype = GetMetalType(sBaseItemID);

                if(iSKUMetaltype == (int)MetalType.Gold
                    || iSKUMetaltype == (int)MetalType.Silver
                    || iSKUMetaltype == (int)MetalType.Platinum
                    || iSKUMetaltype == (int)MetalType.Palladium)
                {

                    if(!IsValidMakingRate(ItemID.Trim()))
                    {
                        using(LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("0 Making Rate is not valid for this item", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                        }
                        this.Close();
                        return;
                    }
                }

            }

            //----------------------


            #region [ Wastage Amount Calculation] // for wastage  // NOT Consider Multi Metal in Ingredient
            // if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbWastage.SelectedIndex == 0 && !string.IsNullOrEmpty(txtWastageQty.Text.Trim()))
            if(cmbWastage.SelectedIndex == 0 && dWastQty > 0)
            {
                // if (Convert.ToDecimal(txtWastageQty.Text) > 0)
                //{
                Decimal dAmount = 0m;
                int iMetalType = 0;
                string sRate = "";

                // enum MetalType  ---> Gold = 1,

                //  iMetalType = GetMetalType(ItemID);
                //  iMetalType = GetMetalType(sBaseItemID);

                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                    iMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                else
                    iMetalType = GetMetalType(sBaseItemID);

                if(iMetalType == (int)MetalType.Gold
                    || iMetalType == (int)MetalType.Silver
                    || iMetalType == (int)MetalType.Palladium
                    || iMetalType == (int)MetalType.Platinum
                    )
                {
                    if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                    {
                        //if(IsFreezedRate)
                        //    sRate = Convert.ToString(dCustomerOrderFreezedRate);
                        //else
                        sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                                                    , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                    }
                    else
                    {
                        sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                    }

                    if(!string.IsNullOrEmpty(sRate))
                        dWMetalRate = Convert.ToDecimal(sRate);

                    if(dWMetalRate > 0)
                    {
                        if(bIsGrossMetalCal)
                        {
                            txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dTotWt), 3, MidpointRounding.AwayFromZero)); ;//Convert.ToString(dWastQty);
                            dAmount = dWMetalRate * dTotWt;
                            txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmount), 2, MidpointRounding.AwayFromZero));
                        }
                        else
                        {
                            txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastQty), 3, MidpointRounding.AwayFromZero)); ;//Convert.ToString(dWastQty);
                            dAmount = dWMetalRate * dWastQty;
                            txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dAmount), 2, MidpointRounding.AwayFromZero));
                        }
                    }
                }
            }

            else if(cmbWastage.SelectedIndex == 1 && dWastQty > 0)
            {
                // Calculate wastage Qty  -- consider Ingredient

                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    Decimal dIngrdTotQty = 0m;
                    Decimal dWastageQty = 0m;
                    Decimal decimalAmount = 0m;

                    int iMetalType = 100;

                    int iIngrdMType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    // iMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    if(iIngrdMType == (int)MetalType.Gold
                        || iIngrdMType == (int)MetalType.Silver
                        || iIngrdMType == (int)MetalType.Palladium
                        || iIngrdMType == (int)MetalType.Platinum)
                    {
                        iMetalType = iIngrdMType;
                    }

                    foreach(DataRow dr in dtIngredients.Rows)
                    {
                        if(iMetalType == Convert.ToInt32(dr["METALTYPE"]))
                        {
                            if(Convert.ToString(dr["Qty"]) != "")
                            {
                                dIngrdTotQty += Convert.ToDecimal(dr["Qty"]);
                            }
                        }
                    }
                    if(dIngrdTotQty > 0) //dWastQty
                    {
                        string sRate = "";
                        //if(IsFreezedRate)
                        //    sRate = Convert.ToString(dCustomerOrderFreezedRate);
                        //else
                        //{
                        //    if((IsSaleAdjustment) && (!string.IsNullOrEmpty(sAdvAdjustmentGoldRate))) //14.01.2014
                        //        sRate = sAdvAdjustmentGoldRate;
                        //    else if((IsGSSTransaction) && (!string.IsNullOrEmpty(sGSSAdjustmentGoldRate))) //14.01.2014
                        //        sRate = sGSSAdjustmentGoldRate;
                        //    else
                        sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                                                    , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                        //}

                        if(!string.IsNullOrEmpty(sRate))
                            dWMetalRate = Convert.ToDecimal(sRate);

                        if(dWMetalRate > 0)
                        {
                            // dWastageQty = dIngrdTotQty * (dWastQty / 100);

                            dWastageQty = decimal.Round(Convert.ToDecimal(dIngrdTotQty * (dWastQty / 100)), 3, MidpointRounding.AwayFromZero);

                            //decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * dWastageQty;

                            decimalAmount = dWMetalRate * dWastageQty;

                            // txtWastageAmount.Text = Convert.ToString(decimalAmount);

                            txtWastagePercentage.Text = Convert.ToString(dWastQty);

                            txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastageQty), 3, MidpointRounding.AwayFromZero));//Convert.ToString(dWastageQty);

                            txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(decimalAmount), 2, MidpointRounding.AwayFromZero));
                        }
                    }

                }
                else
                {
                    if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        if(Convert.ToDecimal(txtQuantity.Text) > 0)
                        {
                            Decimal dWastageQty = 0m;
                            Decimal decimalAmount = 0m;
                            int iMetalType;
                            string sRate = "";

                            //    iMetalType = GetMetalType(ItemID);
                            iMetalType = GetMetalType(sBaseItemID);

                            if(iMetalType == (int)MetalType.Gold)
                            {
                                sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                                if(!string.IsNullOrEmpty(sRate))
                                    dWMetalRate = Convert.ToDecimal(sRate);

                                if(dWMetalRate > 0)
                                {

                                    // dWastageQty = Convert.ToDecimal(txtQuantity.Text) * (Convert.ToDecimal(txtWastageQty.Text) / 100);
                                    dWastageQty = Convert.ToDecimal(txtQuantity.Text) * (dWastQty / 100);

                                    //decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * dWastageQty;

                                    decimalAmount = dWMetalRate * dWastageQty;

                                    txtWastagePercentage.Text = Convert.ToString(dWastQty);
                                    txtWastageQty.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWastageQty), 3, MidpointRounding.AwayFromZero));//Convert.ToString(dWastageQty);

                                    // txtWastageAmount.Text = Convert.ToString(decimalAmount);
                                    txtWastageAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(decimalAmount), 2, MidpointRounding.AwayFromZero));
                                }
                            }
                        }
                    }
                }


                //}
            }
            //if (cmbWastage.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtWastageQty.Text.Trim())))
            else
            {
                txtWastagePercentage.Text = string.Empty;
                txtWastageQty.Text = string.Empty;
                txtWastageAmount.Text = string.Empty;
            }


            #endregion


        }
        private bool IsValidMakingRate(string sMKItemId)
        {
            bool bValid = true;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @MFG_CODE AS NVARCHAR(20)  DECLARE @COMPLEXITY_CODE AS NVARCHAR(20) " +
                                 " DECLARE @RESULT AS INT  SELECT @MFG_CODE = ISNULL(MFG_CODE,'') FROM INVENTTABLE WHERE ITEMID = '" + sMKItemId + "'" +
                                 " SELECT @COMPLEXITY_CODE = ISNULL(COMPLEXITY_CODE,'') FROM INVENTTABLE WHERE ITEMID = '" + sMKItemId + "'" +
                                 " IF(@MFG_CODE <> '' AND @COMPLEXITY_CODE <> '') BEGIN SET @RESULT = 0 END ELSE" +
                                 " BEGIN SET @RESULT = 1 END  SELECT @RESULT";

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bValid = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bValid;
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
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            return bCatchWtItem;
        }

        private void CheckMakingDiscountFromDB()
        {
            string sWtRange = string.Empty;
            decimal dDiscAmt = 0m;

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            // Quantity calculation //// NOT Consider Multi Metal in Ingredient [Order by Metailtype in Ingredient]
            if(dtIngredients != null && dtIngredients.Rows.Count > 0)
            {
                int iIngMetalType;
                iIngMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                if((iIngMetalType == (int)MetalType.Gold) || (iIngMetalType == (int)MetalType.Silver)
                    || (iIngMetalType == (int)MetalType.Platinum) || (iIngMetalType == (int)MetalType.Palladium))
                {
                    sWtRange = Convert.ToString(dtIngredients.Rows[0]["Qty"]);
                }
                else
                    sWtRange = txtQuantity.Text.Trim();
            }
            else
                sWtRange = txtQuantity.Text.Trim();

            string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)  " +
                                 "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
                                 "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
                                 "  WHERE STORENUMBER='" + StoreID + "' " +
                                 "  IF EXISTS(SELECT TOP(1) * FROM RETAILMAKINGDISCTABLE WHERE INVENTLOCATIONID=@INVENTLOCATION) " +
                                 "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +

                                " SELECT  TOP (1) CAST(ISNULL(DISCAMT,0) AS decimal(18, 2)) , DISCTYPE " +
                                " FROM  RETAILMAKINGDISCTABLE WHERE RETAILMAKINGDISCTABLE.INVENTLOCATIONID=@LOCATION " +
                                " AND  (RETAILMAKINGDISCTABLE.ARTICLE =(SELECT ARTICLE_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                                " OR RETAILMAKINGDISCTABLE.ARTICLE ='') AND " +
                                "  (RETAILMAKINGDISCTABLE.COMPLEXITY =(SELECT COMPLEXITY_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID.Trim() + "') " +
                                "  OR RETAILMAKINGDISCTABLE.COMPLEXITY ='')  AND " +
                                "  ('" + sWtRange + "' BETWEEN RETAILMAKINGDISCTABLE.FROMWT AND RETAILMAKINGDISCTABLE.TOWT)  " +
                                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN RETAILMAKINGDISCTABLE.FROMDATE AND RETAILMAKINGDISCTABLE.TODATE)  " +
                                " AND RETAILMAKINGDISCTABLE.ACTIVATE = 1 " + //  ACTIVATE filter 
                                "  ORDER BY RETAILMAKINGDISCTABLE.ARTICLE DESC,RETAILMAKINGDISCTABLE.COMPLEXITY DESC,RETAILMAKINGDISCTABLE.FROMDATE DESC"; // Changed order sequence on 03.06.2013 as per u.das

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();


            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    dDiscAmt = Convert.ToDecimal(reader.GetValue(0));
                    switch(Convert.ToString(reader.GetValue(1)))
                    {
                        case "0":
                            cmbMakingDiscType.SelectedIndex = cmbMakingDiscType.FindStringExact(Convert.ToString(MakingDiscType.Percentage));
                            break;
                        case "1":
                            cmbMakingDiscType.SelectedIndex = cmbMakingDiscType.FindStringExact(Convert.ToString(MakingDiscType.Weight));
                            break;
                        case "2":
                            cmbMakingDiscType.SelectedIndex = cmbMakingDiscType.FindStringExact(Convert.ToString(MakingDiscType.Amount));
                            break;
                        default:
                            cmbMakingType.SelectedIndex = 0;
                            break;
                    }
                }
            }
            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(cmbMakingDiscType.SelectedIndex == 0)
            {
                txtMakingDisc.Text = Convert.ToString(dDiscAmt);
            }
            if(dDiscAmt > 0)
            {
                dMakingDiscDbAmt = dDiscAmt;
                CalcMakingDiscount(dDiscAmt);
            }
        }

        private void CalcMakingDiscount(decimal dMkDiscAmt)
        {
            #region Making Discount Calculation

            if(cmbMakingDiscType.SelectedIndex == 0)   // Percentage  dDiscAmt
            {
                if(!string.IsNullOrEmpty(txtMakingAmount.Text))
                {
                    decimal dTotMkDiscAmt = 0m;
                    decimal dWastageAmt = 0m;
                    if(string.IsNullOrEmpty(txtWastageAmount.Text))//Added txtWastageAmount on 29/05/2014 by S.Sharma
                        dWastageAmt = 0;
                    else
                        dWastageAmt = Convert.ToDecimal(txtWastageAmount.Text.Trim());

                    dTotMkDiscAmt = ((dMkDiscAmt / 100) * ((Convert.ToDecimal(txtMakingAmount.Text.Trim())) + dWastageAmt));//Added txtWastageAmount on 29/05/2014 by S.Sharma
                    txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dTotMkDiscAmt, 2, MidpointRounding.AwayFromZero));
                }
            }
            else if(cmbMakingDiscType.SelectedIndex == 1)   // Weight
            {
                #region [Gold wt calc]
                decimal dGoldWt = 0m;

                if(dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    foreach(DataRow drDisc in dtIngredients.Rows)
                    {
                        if(Convert.ToString(drDisc["METALTYPE"]) != "")
                        {
                            if(Convert.ToInt32(drDisc["METALTYPE"]) == (int)MetalType.Gold)
                            {
                                dGoldWt += Convert.ToDecimal(drDisc["Qty"]);
                            }
                        }
                    }

                }
                else if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                {
                    dGoldWt = Convert.ToDecimal(txtQuantity.Text);
                }
                #endregion
                if(dGoldWt > 0)
                {
                    txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dGoldWt * dMkDiscAmt, 2, MidpointRounding.AwayFromZero));
                }
            }
            else if(cmbMakingDiscType.SelectedIndex == 2)   // Amount
            {
                txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dMkDiscAmt, 2, MidpointRounding.AwayFromZero));
            }

            #endregion
        }

        private decimal getPDSCWQty(string sItemId)
        {
            string commandText = " select PDSCWQTY from SKUTable_Posted  WHERE SKUNUMBER='" + sItemId + "'";

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;

        }

        protected int GetMetalType(string sItemId)
        {
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if(connection.State == ConnectionState.Open)
                connection.Close();
            return iMetalType;

        }

        private string getWastageMetalRate(string sItemId, string sConfigId, string sCustAcc = null)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");

            if(string.IsNullOrEmpty(sCustAcc))
            {
                commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
                commandText.Append(" AND METALTYPE=@METALTYPE ");
            }
            else
            {
                //if(IsSpecialCustomer(sCustAcc))
                //{
                //    commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM CUSTMETALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
                //    commandText.Append(" AND METALTYPE=@METALTYPE AND CUSTOMERACC='" + sCustAcc + "'");
                //}
                //else
                //{
                commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
                commandText.Append(" AND METALTYPE=@METALTYPE ");
                //}
            }

            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return Convert.ToString(sResult.Trim());

        }

        private bool IsSpecialCustomer(string _sCustAcc)
        {
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "select ISNULL(CUSTOMERACC,'') CUSTOMERACC" +
                          " from CUSTMETALRATES where CUSTOMERACC ='" + _sCustAcc + "'";

            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private decimal GetTaxPercentage(string sItemId, int iModuleType)
        {
            string sqlsubDtl = "select top 1 ABS(CAST(ISNULL(TAXVALUE,0)AS DECIMAL(18,2)))AS TAX" +
                              "  from TAXDATA  where TAXCODE in (select a.TAXCODE from (" +
                              " select taxcode from TAXONITEM " +
                              "  where TAXITEMGROUP in (select TAXITEMGROUPID  from INVENTTABLEMODULE" +
                              " where ITEMID='" + sItemId + "' and moduletype=" + iModuleType + ")) a " +
                              "  inner join" +
                              " (select taxcode from TAXGROUPDATA where TAXGROUP " +
                              " in(select TAXGROUP  from RETAILSTORETABLE where" +
                              " STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "')) b " +
                              " on a.TAXCODE=b.TAXCODE )";

            SqlCommand command = new SqlCommand(sqlsubDtl.ToString(), connection);
            command.CommandTimeout = 0;
            decimal sResult = Convert.ToDecimal(command.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();

            return sResult;
        }

        private void txtSKU_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == 13)
            {
                saleLineItem = new SaleLineItem();

                saleLineItem.ItemId = Convert.ToString(txtSKU.Text);
                GetItemForEstimation();
            }
        }

        private decimal GetMRPPrice(string sItemId)
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

                using(SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);
                    command.Parameters.AddWithValue("@ITEMCODE", 0);
                    command.Parameters.AddWithValue("@ITEMRELATION", sItemId);
                    command.Parameters.AddWithValue("@ACCOUNTCODE", 2);
                    command.Parameters.AddWithValue("@UNITID", sBaseUnitId);
                    command.Parameters.AddWithValue("@CURRENCYCODE", ApplicationSettings.Terminal.CompanyCurrency);
                    command.Parameters.AddWithValue("@QUANTITY", 1);
                    command.Parameters.AddWithValue("@COLORID", ("") ?? string.Empty);
                    command.Parameters.AddWithValue("@SIZEID", (cmbSize.Text) ?? string.Empty);
                    command.Parameters.AddWithValue("@STYLEID", ("") ?? string.Empty);
                    command.Parameters.AddWithValue("@CONFIGID", (cmbConfig.Text) ?? string.Empty);
                    command.Parameters.AddWithValue("@TODAY", today);
                    command.Parameters.AddWithValue("@NODATE", DateTime.Parse("1900-01-01"));

                    // Fill out TVP for account relations list
                    using(DataTable accountRelationsTable = new DataTable())
                    {
                        accountRelationsTable.Columns.Add("ACCOUNTRELATION", typeof(string));

                        SqlParameter param = command.Parameters.Add("@ACCOUNTRELATIONS", SqlDbType.Structured);
                        param.Direction = ParameterDirection.Input;
                        param.TypeName = "FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE";
                        param.Value = accountRelationsTable;

                        if(connection.State != ConnectionState.Open)
                            connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            {
                                dAmount = reader.GetDecimal(reader.GetOrdinal("AMOUNT"));
                            }
                        }
                    }
                }
            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close();
            }
            return dAmount;
        }

        private void cmbMakingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Decimal dMakingQty = 0;

            if(!string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && cmbMakingType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimal dMkRate1 = 0m;
                decimal dMkRate = 0m;

                if(!string.IsNullOrEmpty(txtActMakingRate.Text))
                    dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                else
                    dMkRate1 = 0;

                if(!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                    dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                else
                    dMkRate = 0;

                if(dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                {
                    if(bIsGrossMetalCal)
                    {
                        foreach(DataRow dr in dtIngredients.Rows)
                        {
                            dMakingQty += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                        }
                    }
                    else
                        dMakingQty = Convert.ToDecimal(dtIngredients.Rows[0]["QTY"]);
                }
                else
                    dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());


                decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) * dMakingQty;

                //for malabar
                //if((dMkRate1 - dMkRate) > 0)
                //{
                txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - decimalAmount, 2, MidpointRounding.AwayFromZero));
                // }
                decimal dTotAmt = 0m;
                decimal dActTotAmt = 0m;
                decimal dManualChangesAmt = 0m;
                decimal dMkDiscTotAmt = 0m;
                decimal dQty = 0m;


                if(!string.IsNullOrEmpty(txtQuantity.Text))
                    dQty = Convert.ToDecimal(txtQuantity.Text);
                else
                    dQty = 0;

                if(!string.IsNullOrEmpty(txtActMakingRate.Text))
                    dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                else
                    dMkRate1 = 0;

                if(!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                    dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                else
                    dMkRate = 0;

                if(!string.IsNullOrEmpty(txtTotalAmount.Text))
                    dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                else
                    dTotAmt = 0;
                if(!string.IsNullOrEmpty(txtActToAmount.Text))
                    dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                else
                    dActTotAmt = 0;
                if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                    dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                else
                    dMkDiscTotAmt = 0;

                if(!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                    dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);
                else
                    dManualChangesAmt = 0;

                decimal dMkAmt = 0;
                if(dActTotAmt > 0 && dManualChangesAmt > 0)
                {
                    dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                    // txtChangedMakingRate.Text = txtActMakingRate.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                    txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                }

                //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));


                if(!string.IsNullOrEmpty(txtMakingAmount.Text))
                    dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                else
                    dMkAmt = 0;

                if(dMkAmt > 0)
                    txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt / dMakingQty, 2, MidpointRounding.AwayFromZero));
                else
                    txtChangedMakingRate.Text = txtActMakingRate.Text;
            }
            if(cmbMakingType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                txtMakingAmount.Text = string.Empty;
                // txtTotalAmount.Text = string.Empty;
            }
            if(cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if(cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                txtMakingAmount.Text = string.Empty;
                //txtTotalAmount.Text = string.Empty;
            }
            if(cmbMakingType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim());
                // txtMakingAmount.Text = Convert.ToString(decimalAmount);
                txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if(cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                if(!string.IsNullOrEmpty(txtgval.Text))
                {
                    if(Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()));
                    else
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                }
                else
                {
                    decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                }

                txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if(cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                txtMakingAmount.Text = string.Empty;
                // txtTotalAmount.Text = string.Empty;
            }

            if(!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
            {
                txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));
                // txtActToAmount.Text = txtTotalAmount.Text;
            }


            if(!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text) && !string.IsNullOrEmpty(txtAmount.Text))
            {
                Decimal decimalAmount = 0m;
                Decimal decimalAmount1 = 0m;
                decimalAmount1 = Convert.ToDecimal(txtActMakingRate.Text.Trim()) * dMakingQty;
                // decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                decimalAmount = (decimalAmount1) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                //txtActToAmount.Text = txtTotalAmount.Text;
            }

            //
            if(string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()))
            {
                txtMakingAmount.Text = string.Empty;
                //txtTotalAmount.Text = string.Empty;
            }

            // Added  -- wastage
            if((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
            {
                if((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                {
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                    //txtActToAmount.Text = txtTotalAmount.Text;
                }
            }
        }
    }


}
