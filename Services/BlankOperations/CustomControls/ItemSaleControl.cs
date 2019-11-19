using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    sealed partial class ItemSaleControl : OrderDetailsPage
    {
        public string sPieces = string.Empty;
        public string sQuantity = string.Empty;
        public string sRate = string.Empty;
        public string sRateType = string.Empty;
        public string sMakingRate = string.Empty;
        public string sMakingType = string.Empty;
        public string sMakingDiscount = string.Empty;
        public string sMakingAmount = string.Empty;
        public string sTotalAmount = string.Empty;
        public DataTable dtIngredients = null;
        public DataTable dtIngredientsClone = null;
        public DataTable dtOrderNum = null;
        public string txtAmount = string.Empty;
        SqlConnection conn;
        bool isQtyEntered = false;
        Helper oHelper = new Helper();
        string sITEMID = string.Empty;
        public string sNumPadEntryType = string.Empty;
        public ItemSaleControl(string itemid, DataTable dtOrder)
        {

            InitializeComponent();
            sITEMID = itemid;
            lblTAmount.Text = string.Empty;
            sMakingAmount = lblMakingAmt.Text = string.Empty;
            sMakingType = txtMakingType.Text = Convert.ToString(MakingType.Weight);
            sRateType = txtRateType.Text = Convert.ToString(RateType.Weight);
            BindIngredientGrid(itemid);
            FillQtyPcsFromSKUTable(itemid);
            if (isQtyEntered)
                CheckRateFromDB();
            dtOrderNum = dtOrder;
            //  BindOrder(dtOrder);
            //    lblCustOrder.Visible = false;
            //   txtCustomerOrder.Visible = false;
            //    btnCustomerOrder.Visible = false;
            //    lblCustomerOrderDesc.Text = string.Empty;
            tableLayoutPanel1.Visible = false;

        }

        private void BindOrder(DataTable dtOrder)
        {
            dtOrder.PrimaryKey = new DataColumn[] { dtOrder.Columns["ORDERNUM"] };

            if (!dtOrder.Rows.Contains("NO SELECTION"))
            {
                dtOrder.Rows.Add("NO SELECTION");
            }
            string ss = oHelper.ConvertEnumToDataTable("ORDERNUM", "", txtCustomerOrder.Text, dtOrder);
            if (ss.ToUpper().Trim() == "NO SELECTION")
            {
                txtCustomerOrder.Text = string.Empty;
            }
            else
            {
                txtCustomerOrder.Text = ss;
            }

        }

        protected override void OnViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Transaction":
                    bindingSource.ResetBindings(false);
                    break;
                default:
                    break;
            }

        }

        #region Qty and Pcs Fill up
        private void FillQtyPcsFromSKUTable(string ItemID)
        {

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = "SELECT     TOP (1) SKULine_Posted.PDSCWQTY AS PCS , SKULine_Posted.QTY AS QTY " +
                                 " FROM         SKULine_Posted INNER JOIN SKUTable_Posted ON SKULine_Posted.SkuNumber = SKUTable_Posted.SkuNumber " +
                                 " WHERE     (SKULine_Posted.SkuNumber = '" + ItemID.Trim() + "') ";


            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sPieces = txtPCS.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(reader.GetValue(0)), 3, MidpointRounding.AwayFromZero));
                    isQtyEntered = false;
                    sQuantity = txtQuantity.Text = Convert.ToString(reader.GetValue(1));
                    isQtyEntered = true;

                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            reader.Close();
            reader.Dispose();

        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {


            sRateType = txtRateType.Text = oHelper.ConvertEnumToDataTable("RateType", "RATETYPE", txtRateType.Text);
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);

        }

        private void button2_Click(object sender, EventArgs e)
        {

            sMakingType = txtMakingType.Text = oHelper.ConvertEnumToDataTable("MakingType", "MAKINGTYPE", txtRateType.Text);
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);
        }

        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            sPieces = txtPCS.Text;
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            sQuantity = txtQuantity.Text;
            if (isQtyEntered)
                CheckRateFromDB();
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);
        }

        private void CheckRateFromDB()
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " DECLARE @INVENTLOCATION VARCHAR(20) " +
                                 " SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN " +
                                 " RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID " +
                                 " WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "' " +
                                 " SELECT  CAST(MK_RATE AS decimal (6,2)),MK_TYPE FROM RETAIL_SALES_AGREEMENT_DETAIL WHERE " +
                                 " (INVENTLOCATIONID=@INVENTLOCATION " +
                                 " AND ('" + txtQuantity.Text.Trim() + "' BETWEEN FROM_WEIGHT AND TO_WEIGHT) " +
                                 " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE)) " +
                                 " OR ITEMID= (SELECT ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + sITEMID.Trim() + "' )";

            // CHANGES TO BE DONE  : JOIN WITH INVENT TABLE TO PICK THE PARENT ITEM ID IN ABOVE QUERY 

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    txtMakingRate.Text = Convert.ToString(reader.GetValue(0));
                    if ((Convert.ToInt16(reader.GetValue(1)) == 0))
                        txtMakingType.Text = Convert.ToString(RateType.Weight);
                    else
                        txtMakingType.Text = Convert.ToString(Convert.ToInt16(reader.GetValue(1)) == 1 ? RateType.Pieces : RateType.Tot);
                    //  cmbMakingType.SelectedIndex = Convert.ToInt16(reader.GetValue(1));
                    //  cmbMakingType.SelectedIndex = Enum.GetValues(reader)
                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            reader.Close();
            reader.Dispose();


        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            sRate = txtRate.Text;
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);
        }

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pieces = 1,
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
            Jewellery = 14,

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
        }
        #endregion

        private void txtRateType_TextChanged(object sender, EventArgs e)
        {
            lblTAmount.Text = string.Empty;
            lblMakingAmt.Text = string.Empty;
            if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Weight).ToUpper().Trim() && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim());
                txtAmount = Convert.ToString(decimalAmount);
            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Weight).ToUpper().Trim() && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                txtAmount = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;

            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Pieces).ToUpper().Trim() && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                txtAmount = Convert.ToString(decimalAmount);
            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Pieces).ToUpper().Trim() && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                txtAmount = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;

            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Tot).ToUpper().Trim() && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtRate.Text.Trim());
                txtAmount = Convert.ToString(decimalAmount);
            }
            if (!string.IsNullOrEmpty(txtAmount.Trim()) && !string.IsNullOrEmpty(lblMakingAmt.Text.Trim()))
            {
                sTotalAmount = lblTAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Trim()) + Convert.ToDecimal(lblMakingAmt.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(txtMakingDisc.Text) && !string.IsNullOrEmpty(lblMakingAmt.Text)
                 && !string.IsNullOrEmpty(txtAmount))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = (Convert.ToDecimal(lblMakingAmt.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(lblMakingAmt.Text.Trim()))) + (Convert.ToDecimal(txtAmount));
                sTotalAmount = lblTAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                txtAmount = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;
            }
        }

        private void txtMakingRate_TextChanged(object sender, EventArgs e)
        {
            sMakingRate = txtMakingRate.Text;
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);
        }

        private void txtMakingType_TextChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Weight).ToUpper().Trim() && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim());
                sMakingAmount = lblMakingAmt.Text = Convert.ToString(decimalAmount);
            }
            if (txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Weight).ToUpper().Trim() && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
            {
                sMakingAmount = lblMakingAmt.Text = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;
            }
            if (txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Pieces).ToUpper().Trim() && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                sMakingAmount = lblMakingAmt.Text = Convert.ToString(decimalAmount);
            }
            if (txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Pieces).ToUpper().Trim() && (string.IsNullOrEmpty(txtMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                sMakingAmount = lblMakingAmt.Text = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;
            }
            if (txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Tot).ToUpper().Trim() && !string.IsNullOrEmpty(txtMakingRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtMakingRate.Text.Trim());
                sMakingAmount = lblMakingAmt.Text = Convert.ToString(decimalAmount);
            }
            if (txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Percentage).ToUpper().Trim() && !string.IsNullOrEmpty(txtMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = (Convert.ToDecimal(txtMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Trim()));
                sMakingAmount = lblMakingAmt.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if (txtMakingType.Text.ToUpper().Trim() == Convert.ToString(MakingType.Percentage).ToUpper().Trim() && string.IsNullOrEmpty(txtAmount.Trim()))
            {
                sMakingAmount = lblMakingAmt.Text = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;
            }

            if (!string.IsNullOrEmpty(txtAmount.Trim()) && !string.IsNullOrEmpty(lblMakingAmt.Text.Trim()))
            {
                sTotalAmount = lblTAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Trim()) + Convert.ToDecimal(lblMakingAmt.Text.Trim()));
            }
            if (!string.IsNullOrEmpty(txtMakingDisc.Text) && !string.IsNullOrEmpty(lblMakingAmt.Text) && !string.IsNullOrEmpty(txtAmount))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = (Convert.ToDecimal(lblMakingAmt.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(lblMakingAmt.Text.Trim()))) + (Convert.ToDecimal(txtAmount));
                sTotalAmount = lblTAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if (string.IsNullOrEmpty(txtMakingRate.Text.Trim()))
            {
                sMakingAmount = lblMakingAmt.Text = string.Empty;
                sTotalAmount = lblTAmount.Text = string.Empty;
            }
        }

        private void txtMakingDisc_TextChanged(object sender, EventArgs e)
        {
            sMakingDiscount = txtMakingDisc.Text;
            txtRateType_TextChanged(sender, e);
            txtMakingType_TextChanged(sender, e);
        }

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 0);
        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 0);
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }

        private void txtMakingRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }

        private void txtMakingDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }

        private void txtPCS_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "P";
            numpadEntry.PromptText = "Enter Pieces:";
        }

        private void txtQuantity_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "Q";
            numpadEntry.PromptText = "Enter Quantity:";
        }

        private void txtRate_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "R";
            numpadEntry.PromptText = "Enter Rate:";
        }

        private void txtMakingRate_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "MR";
            numpadEntry.PromptText = "Enter Making Rate:";
        }

        private void txtMakingDisc_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "MD";
            numpadEntry.PromptText = "Enter Making Discount:";
        }

        private void numpadEntry_EnterButtonPressed()
        {
            if (string.IsNullOrEmpty(txtPCS.Text) || sNumPadEntryType.ToUpper().Trim() == "P")
            {
                sPieces = txtPCS.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;

                txtQuantity.Focus();
                numpadEntry.PromptText = "Enter Quantity:";


                numpadEntry.Refresh();
                numpadEntry.Focus();

                return;
            }

            if (string.IsNullOrEmpty(txtQuantity.Text) || sNumPadEntryType.ToUpper().Trim() == "Q")
            {
                sQuantity = txtQuantity.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                txtRate.Focus();
                numpadEntry.PromptText = "Enter Rate:";
                numpadEntry.Refresh();
                numpadEntry.Focus();


            }

            else if (string.IsNullOrEmpty(txtRate.Text) || sNumPadEntryType.ToUpper().Trim() == "R")
            {
                sRate = txtRate.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                txtMakingRate.Focus();
                numpadEntry.PromptText = "Enter Making Rate:";
                numpadEntry.Refresh();
                numpadEntry.Focus();


            }

            else if (string.IsNullOrEmpty(txtMakingRate.Text) || sNumPadEntryType.ToUpper().Trim() == "MR")
            {
                sMakingRate = txtMakingRate.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                txtMakingDisc.Focus();
                numpadEntry.PromptText = "Enter Making Discount:";

                numpadEntry.Refresh();
                numpadEntry.Focus();

            }

            else if (string.IsNullOrEmpty(txtMakingDisc.Text) || sNumPadEntryType.ToUpper().Trim() == "MD")
            {
                sMakingDiscount = txtMakingDisc.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                numpadEntry.PromptText = "Enter Pieces:";
                sNumPadEntryType = "P";
                numpadEntry.Refresh();
                numpadEntry.Focus();


            }



            return;
        }

        private void BindIngredientGrid(string ItemID)
        {
            conn = ApplicationSettings.Database.LocalConnection;
            dtIngredients = new DataTable();

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            StringBuilder commandText = new StringBuilder();
            commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + ItemID.Trim() + "')");
            commandText.Append("  BEGIN  ");
            commandText.Append(" SELECT [SkuNumber] ,[SkuDate] ,[ItemID] ,[InventDimID] ,[InventSizeID] ,[InventColorID]   ");
            commandText.Append("   ,[ConfigID] ,[InventBatchID],[PDSCWQTY] AS PCS,[QTY] ,[CValue] ,[CRate] AS [Rate] ,[UnitID] FROM [SKULine_Posted] ");
            commandText.Append("  END ");

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            dtIngredients.Load(reader);



            if (dtIngredients != null && dtIngredients.Rows.Count > 0)
            {
                dtIngredientsClone = new DataTable();
                dtIngredientsClone = dtIngredients.Clone();

                foreach (DataRow drClone in dtIngredients.Rows)
                {
                    dtIngredientsClone.ImportRow(drClone);
                }


                foreach (DataRow dr in dtIngredientsClone.Rows)
                {
                    string sRate = string.Empty;
                    sRate = getRateFromMetalTable(Convert.ToString(dr["ItemID"]), Convert.ToString(dr["ConfigID"]), Convert.ToString(dr["QTY"]), Convert.ToString(dr["InventSizeID"]),
                                                                                    Convert.ToString(dr["InventColorID"]), Convert.ToString(dr["InventBatchID"]));
                    if (!string.IsNullOrEmpty(sRate))
                        dr["Rate"] = sRate;

                    dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                    sRate = txtRate.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));

                }
                dtIngredientsClone.AcceptChanges();

                txtRateType.Text = Convert.ToString(RateType.Tot);


            }
            else
            {

                // txtRate.Text = getRateFromMetalTable();

            }


            if (conn.State == ConnectionState.Open)
                conn.Close();
            reader.Close();
            reader.Dispose();
        }


        #region  GET RATE FROM METAL TABLE
        private string getRateFromMetalTable(string ItemID, string ConfigID, string GrWeight, string SizeID, string ColorID, string BatchID)
        {

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
                commandText.Append(" BEGIN ");
            }

            commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");

            commandText.Append(" AND METALTYPE=@METALTYPE ");


            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");


            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + ConfigID.Trim() + "' ");
            commandText.Append(" ORDER BY [TRANSDATE],[TIME] DESC ");

            if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                commandText.Append(" END ");
                commandText.Append(" ELSE ");
                commandText.Append(" BEGIN ");
                commandText.Append(" SELECT  CAST(UNIT_RATE AS decimal (6,2)) FROM RETAILCUSTOMERSTONEAGGREEMENTDETAIL WHERE  ");
                commandText.Append(" WAREHOUSE=@INVENTLOCATION AND ('" + GrWeight.Trim() + "' BETWEEN FROM_WEIGHT AND TO_WEIGHT)  ");
                commandText.Append(" AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE) ");
                commandText.Append(" AND ITEMID='" + ItemID.Trim() + "' AND INVENTBATCHID='" + BatchID.Trim() + "' ");
                commandText.Append(" AND INVENTSIZEID='" + SizeID.Trim() + "' AND INVENTCOLORID='" + ColorID.Trim() + "' ");
                commandText.Append(" END ");
            }


            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        private void btnCustomerOrder_Click(object sender, EventArgs e)
        {
            BindOrder(dtOrderNum);
        }

        private void btnSelectItems_Click(object sender, EventArgs e)
        {
            DataSet ds = SelectItems("");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {

                CustomerOrderDetails oCustOrder = new CustomerOrderDetails(ds, string.Empty);
                oCustOrder.ShowDialog();
                DataTable dtNew = new DataTable();
                dtNew = oCustOrder.dtCustOrderDetails;
                if (dtNew != null && dtNew.Rows.Count > 0)
                {
                    tableLayoutPanel1.Visible = true;
                    grItems.DataSource = dtNew.DefaultView;
                }
                else
                {
                    tableLayoutPanel1.Visible = false;
                }
            }
            else if (string.IsNullOrEmpty(txtCustomerOrder.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PLEASE SELECT THE CUSTOMER ORDER.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {

                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("NO ITEM DETAILS PRESENT.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {

                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
        }

        #region Select Items
        private DataSet SelectItems(string linenum)
        {
            if (!string.IsNullOrEmpty(txtCustomerOrder.Text))
            {
                DataSet dsCustOrder = new DataSet();
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string commandText = string.Empty;

                commandText = " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                     " ,[CONFIGID],[CODE],[SIZEID] ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE] " +
                                     " ,[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT] " +
                                     " ,[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[CREATEDON],[STAFFID] " +
                                     " FROM [CUSTORDER_DETAILS] " +
                                     " WHERE [ORDERNUM]='" + txtCustomerOrder.Text.Trim() + "' AND isDelivered='0';" +
                                     " SELECT  [ORDERNUM],[ORDERDETAILNUM],[LINENUM],[STOREID] " +
                                     " ,[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID] " +
                                     " ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE],[DATAAREAID] " +
                                     " ,[AMOUNT] FROM [CUSTORDER_SUBDETAILS] " +
                                     " WHERE [ORDERNUM]='" + txtCustomerOrder.Text.Trim() + "' ;";



                SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
                adapter.Fill(dsCustOrder);

                if (dsCustOrder == null && dsCustOrder.Tables.Count <= 0 && dsCustOrder.Tables[0].Rows.Count <= 0)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("NO ITEM DETAILS PRESENT.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {

                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                    }

                    return null;
                }



                if (conn.State == ConnectionState.Open)
                    conn.Close();
                return dsCustOrder;
            }
            return null;
        }
        #endregion

    }
}
