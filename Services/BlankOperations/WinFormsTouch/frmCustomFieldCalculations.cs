

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using LSRetailPosis.Settings;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using LSRetailPosis.Transaction.Line.Discount;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.TaxService;


namespace BlankOperations.WinFormsTouch
{
    public partial class frmCustomFieldCalculations : frmTouchBase
    {
        private List<Microsoft.Dynamics.Retail.Pos.Contracts.Services.ITaxProvider> Providers { get; set; }
        string preTransType = string.Empty; ////Start : Changes on 08/04/2014 RHossain   
        bool bValidSKU = false;
        #region Variable Declaration
        bool isBulkItem = false;
        bool isMkAdded = false;
        bool isValidMkDisc = true;
        string sOGBatchIdForRepirRet = string.Empty;
        decimal dTransferCost = 0m;
        Rounding oRounding = new Rounding();
        string sEnteredNum = string.Empty;
        public string RadioChecked { get; set; }
        bool isViewing = false;
        public bool isCancelClick = false;
        public List<KeyValuePair<string, string>> saleList;
        public List<KeyValuePair<string, string>> purchaseList;
        SqlConnection conn;
        string ItemID = string.Empty;
        string StoreID = string.Empty;
        string ConfigID = string.Empty;
        string ColorID = string.Empty;
        string BatchID = string.Empty;
        string SizeID = string.Empty;
        string StyleID = string.Empty;
        string GrWeight = string.Empty;
        decimal dStoneWtRange = 0m;
        int OlineNum = 0;
        string index = string.Empty;
        public DataTable dtIngredients = null;
        public DataTable dtIngredientsClone = null;
        ArrayList alist = new ArrayList();
        DataSet dsCustOrder = new DataSet();
        string LineNum = string.Empty;
        string sNumPadEntryType = string.Empty;
        string MRPUCP = string.Empty;
        bool isMRPUCP = false;

        // GSS Maturity
        bool IsGSSTransaction = false;
        decimal dGSSMaturityQty = 0m;
        decimal dGSSAvgRate = 0m;

        decimal dPrevGSSSaleWt = 0m;
        decimal dPrevRunningQtyGSS = 0m;
        decimal dActualGSSSaleWt = 0m;

        string sGSSAdjustmentGoldRate = string.Empty;
        RetailTransaction retailTrans;

        // Avg Gold Rate Adjustment
        bool IsSaleAdjustment = false;
        decimal dSaleAdjustmentGoldAmt = 0m;
        decimal dSaleAdjustmentGoldQty = 0m;
        decimal dSaleAdjustmentMetalRate = 0m;
        decimal dSaleAdjustmentAvgGoldRate = 0m;
        decimal dSaleAdjustmentOrderMetalRate = 0m;
        string sAdjuOrderNo = "";
        decimal dCAdvAdjustmentAvgGoldRate = 0m;

        //=====================Soutik=======================
        decimal dAdjustmentCommitedQty = 0m;
        int iSaleAdjustmentCommitedForDays = 0;
        DateTime CustAdvDepositDate;
        decimal dSaleAdjustCommitedQty = 0;
        bool IsAdjustmentCommitedQtyDone = false;

        decimal dPrevTransAdjGoldQty = 0m;
        decimal dPrevRunningTransAdjGoldQty = 0m;
        decimal dActualTransAdjGoldQty = 0m;

        string sAdvAdjustmentGoldRate = string.Empty;

        // Added for wastage
        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;
        Decimal dWMetalRate = 0m;
        //
        // Making Discount Type  
        decimal dMakingDiscDbAmt = 0m;

        decimal dCustomerOrderFreezedRate = 0m;

        int iCallfrombase = 0;

        public DataTable dtExtndPurchExchng = null;
        string sBaseUnitId;
        string sMkPromoCode = "";
        string sCustomerId = string.Empty;
        int iIsMkDiscFlat = 0;
        int iOWNDMD = 0; int iOWNOG = 0; int iOTHERDMD = 0; int iOTHEROG = 0; int iMkDisType = 0;
        string sPromoCode = string.Empty; // add on 19/11/2014

        int iIsDiscFlat = 0;
        int iDecPre = 0;
        bool bIsGrossMetalCal = false;
        string sFGPromoCode = string.Empty;
        SaleLineItem _saleLineItem;
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objB = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();

        int iTotAmtChanged = 0;
        int iMkRateChanged = 0;
        decimal dViewModeCalAdjRateforMetal = 0m;

        public decimal dPurityReading1 = 0m;
        public decimal dPurityReading2 = 0m;
        public decimal dPurityReading3 = 0m;
        public string sPurityPerson = "";
        public string sPurityPersonName = "";

        string sGRate = "0";
        string sSRate = "0";
        string sPRate = "0";
        string sMQty = "0";
        string sOrderLineConfig = "";

        #endregion

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
            //Pieces = 0,
            //Weight = 2,
            //Tot = 3,
            //Percentage = 4,
            //Inch = 5,
            //Gross = 6,
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
            // Inch = 5,
            Gross = 6,
        }

        enum CRWMakingCalcType
        {
            Net = 1,
            Gross = 2,
            NetChargeable = 3,
            Blank = 4,
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
        #region // Making Discount Type

        enum MakingDiscType
        {
            PerPCS = 0,
            PerGram = 1,
            Percent = 2,
        }
        #endregion

        #region enum CRWRetailDiscPermission
        private enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }

        enum AdvAgainst
        {
            None = 0,
            OGPurchase = 1,
            OGExchange = 2,
            SaleReturn = 3,
        }

        enum CRWGiftType
        {
            Gold = 0,
            Diamond = 1,
            Both = 2,
        }

        enum TableGroupAll
        {
            Table = 0,
            GroupId = 1,
            All = 2,
        }
        #endregion

        private IApplication application;

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
                InternalApplication = value;
            }
        }

        internal static IApplication InternalApplication { get; private set; }

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
            GiftVoucher = 18,

        }
        #endregion

        #region enum LType
        enum LType
        {
            Main = 0,
            Sub = 1,

        }
        #endregion

        #region TransactionType
        public enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }
        #endregion

        #region Initialization

        public frmCustomFieldCalculations()
        {
            InitializeComponent();
            btnCLose.Visible = false;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            BindMakingDiscount(); // Making Discount Type
            EnableSaleButtons();
            RadioChecked = Convert.ToString((int)TransactionType.Sale);
            chkOwn.Visible = false;
            lblBatchNo.Visible = false;
            txtBatchNo.Visible = false;
            //txtGoldTax.Text = "0";

            // ManagerPermissionEnabled();
        }

        private void ManagerPermissionEnabled()
        {
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();
            if (Convert.ToInt16(iSPId) != (int)CRWRetailDiscPermission.Manager2)
            {
                rdbPurchReturn.Visible = false;
                rdbExchangeReturn.Visible = false;
            }
            else
            {
                rdbPurchReturn.Visible = true;
                rdbExchangeReturn.Visible = true;
            }
        }

        private bool ValidateItem(string item)
        {
            bool ValidItem = true;

            SqlCommand cmd = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " DECLARE @ITEMID NVARCHAR(20) " +
                               " SELECT TOP(1) @ITEMID=ITEMID FROM INVENTTABLE WHERE ITEMID='" + item + "' AND RETAIL=1" +
                               " IF ISNULL(@ITEMID,'')='' " +
                               " BEGIN " +
                               " SELECT 'True' " +
                               " END " +
                    // " ELSE IF EXISTS(SELECT * FROM SKUTable_Posted WHERE SKUNUMBER=@ITEMID) " +
                               " ELSE IF EXISTS(SELECT * FROM SKUTableTrans WHERE SKUNUMBER=@ITEMID) " +
                               " BEGIN " +
                               " SELECT 'True' " +
                               " END " +
                               " ELSE " +
                               " BEGIN " +
                               " SELECT 'False' " +
                               " END ";
                cmd = new SqlCommand(query, conn);
                ValidItem = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

            }
            return ValidItem;
        }

        private void GetItemType(string item) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";


                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iOWNDMD = Convert.ToInt16(reader.GetValue(0));
                        iOWNOG = Convert.ToInt16(reader.GetValue(1));
                        iOTHERDMD = Convert.ToInt16(reader.GetValue(2));
                        iOTHEROG = Convert.ToInt16(reader.GetValue(3));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private bool ValidateServiceItem(string item)
        {
            bool ValidItem = true;

            SqlCommand cmd = null;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " DECLARE @status INT IF EXISTS" +
                    " (SELECT ADJUSTMENTITEMID+GSSADJUSTMENTITEMID+GSSDISCOUNTITEMID+GSSAMOUNTADJUSTMENTITEMID+GSSAMOUNTDISCOUNTITEMID " +
                    " FROM [RETAILPARAMETERS] WHERE  (ADJUSTMENTITEMID = '" + item + "' OR GSSADJUSTMENTITEMID = '" + item + "' " +
                    " OR GSSAMOUNTADJUSTMENTITEMID = '" + item + "' OR GSSAMOUNTDISCOUNTITEMID = '" + item + "' OR GSSDISCOUNTITEMID = '" + item + "')" +
                    " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "')" +
                    " BEGIN SET @status = 0 END ELSE BEGIN SET @status = 1 END SELECT @status";

                cmd = new SqlCommand(query, conn);
                ValidItem = Convert.ToBoolean(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

            }
            return ValidItem;

        }

        public frmCustomFieldCalculations(SqlConnection connection, string itemId, string storeId,
                                            string configId, string sizeId, string colorId, string styleId,
                                            ArrayList al, string MRP,
                                            bool isMRP, IPosTransaction posTransaction = null,
                                            string UnitId = null, decimal dFGQty = 0,
                                            decimal dFGPCS = 0, string sFreeGPromoCode = null) // GSS Maturity
        {
            InitializeComponent();
            lblBatchNo.Visible = false;
            txtBatchNo.Visible = false;
            txtMakingDisc.Enabled = false;
            retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                IsGSSTransaction = retailTrans.PartnerData.IsGSSMaturity;

                IsSaleAdjustment = retailTrans.PartnerData.SaleAdjustment;

                if (!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                {
                    sCustomerId = retailTrans.Customer.CustomerId;
                }

                if (!string.IsNullOrEmpty(retailTrans.PartnerData.FREEGIFTCON))
                {
                    txtQuantity.Text = Convert.ToString(decimal.Round(dFGQty, 3, MidpointRounding.AwayFromZero));
                    txtPCS.Text = Convert.ToString(decimal.Round(dFGPCS, 0, MidpointRounding.AwayFromZero));

                    sFGPromoCode = sFreeGPromoCode;
                }
            }

            btnCLose.Visible = false;
            conn = connection;
            if (!ValidateItem(itemId) || !ValidateServiceItem(itemId))
            {
                using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("This item is not properly defined on other modules.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                }
                isCancelClick = true;
                this.Close();
            }
            else if (retailTrans.PartnerData.SKUBookedItems == true && lblCustOrder.Text == "NO SELECTION")
            {
                using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("This item is booked against order no " + lblCustOrder.Text + ".", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                }
                isCancelClick = true;
                this.Close();
            }
            else
            {
                #region
                dTransferCost = getTransferCostPrice(itemId);
                GetItemType(itemId);

                BindRateTypeMakingTypeCombo();
                BindWastage();
                BindMakingDiscount();

                if (Convert.ToDecimal(retailTrans.PartnerData.FlatMkDiscPct) > 0)
                {
                    cmbMakingType.SelectedIndex = (int)MakingType.Percentage;
                }
                else
                {
                    cmbMakingType.SelectedIndex = (int)MakingType.Weight;
                }


                ItemID = itemId;
                StoreID = storeId;
                ConfigID = configId;
                SizeID = sizeId;
                ColorID = colorId;
                StyleID = styleId;

                MRPUCP = MRP;
                isMRPUCP = isMRP;
                if (!IsRetailItem(ItemID)) // for samra gift item should allow price override in sales screen
                    EnableSaleButtons();
                else
                {
                    if (IsGiftItem(ItemID))
                    {
                        EnableSaleButtons();
                    }
                    else
                    {
                        txtPCS.Enabled = false;
                        txtRate.Enabled = false;
                        txtTotalWeight.Enabled = false;
                        txtPurity.Enabled = false;
                        txtLossPct.Enabled = false;
                    }
                }

                string sItemDesc = "";
                if (retailTrans != null)
                {
                    string sSqr = "select top 1 f.DESCRIPTION from ECORESPRODUCTTRANSLATION F" +
                                   " left join  ECORESPRODUCT E ON E.RECID = F.PRODUCT " +
                                   " left join INVENTTABLE as d on e.RECID =d.product " +
                                   "  where d.ITEMID='" + itemId + "' and f.LANGUAGEID ='" + CultureInfo.CurrentUICulture.IetfLanguageTag + "'";
                    sItemDesc = NIM_ReturnExecuteScalar(sSqr);
                }

                lblSelectedSKU.Text = itemId + " - " + sItemDesc;// added on 25/02/2015
                //txtTotalWeight.Enabled = false;
                //txtPurity.Enabled = false; 

                sBaseItemID = itemId;
                sBaseConfigID = configId;
                sBaseUnitId = UnitId;
                //
                if (!IsCatchWtItem(sBaseItemID))
                {
                    txtPCS.Enabled = false;
                    txtPCS.Text = "";
                }
                else
                {
                    txtPCS.Enabled = true;
                    txtPCS.Text = "1";
                }

                iDecPre = GetDecimalPrecison(sBaseUnitId);

                //  RadioChecked = "Sale";
                chkOwn.Visible = false;

                if (IsMetalRateCalcTypeGross(sBaseItemID))
                    bIsGrossMetalCal = true;
                else
                    bIsGrossMetalCal = false;
                if (retailTrans.PartnerData.IsRepairReturn)
                {
                    RadioChecked = Convert.ToString((int)TransactionType.Sale);
                    rdbSale.Visible = true;
                    rdbPurchase.Visible = false;
                    rdbExchangeReturn.Visible = false;
                    rdbExchange.Visible = false;
                    rdbPurchReturn.Visible = false;
                    txtChangedMakingRate.Enabled = true;
                    txtChangedMakingRate.BackColor = SystemColors.Window;
                    txtChangedTotAmount.Enabled = true;
                    txtChangedTotAmount.BackColor = SystemColors.Window;
                }
                else
                {
                    if (iOWNDMD == 1 || iOWNOG == 1 || iOTHERDMD == 1 || iOTHEROG == 1)
                    {
                        //rdbPurchase.Checked = true;

                        RadioChecked = Convert.ToString((int)TransactionType.Purchase);
                        rdbSale.Visible = false;
                        rdbSale.Checked = false;
                        rdbPurchase.Checked = true;
                        txtQuantity.Enabled = false;
                        txtRate.Enabled = false;
                        txtChangedMakingRate.Enabled = false;
                        txtChangedMakingRate.BackColor = SystemColors.Control;
                        txtChangedTotAmount.Enabled = false;
                        txtChangedTotAmount.BackColor = SystemColors.Control;
                        panel1.Enabled = false;
                        cmbMakingType.Enabled = true;//added on 17/07/2017
                        cmbRateType.Enabled = true;//added on 17/07/2017
                        ManagerPermissionEnabled();
                    }
                    else
                    {
                        RadioChecked = Convert.ToString((int)TransactionType.Sale);
                        cmbMakingType.Enabled = false;//added on 17/07/2017
                        cmbRateType.Enabled = false;//added on 17/07/2017
                        rdbSale.Visible = true;
                        rdbSale.Checked = true;
                        rdbPurchase.Visible = false;
                        rdbExchangeReturn.Visible = false;
                        rdbExchange.Visible = false;
                        rdbPurchReturn.Visible = false;
                        txtChangedMakingRate.Enabled = true;
                        txtChangedMakingRate.BackColor = SystemColors.Window;
                        txtChangedTotAmount.Enabled = true;
                        txtChangedTotAmount.BackColor = SystemColors.Window;
                        panel1.Enabled = true;
                    }
                }

                if (IsGSSTransaction)
                {
                    dGSSMaturityQty = Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);
                    dGSSAvgRate = Convert.ToDecimal(retailTrans.PartnerData.GSSAvgRate);
                    dPrevGSSSaleWt = Convert.ToDecimal(retailTrans.PartnerData.GSSSaleWt);
                }

                decimal dAdjAmt = 0;
                decimal dCtoAdvAdjAmt = 0;
                decimal dAdjQty = 0;

                foreach (SaleLineItem SLItem in retailTrans.SaleItems)
                {
                    #region Adv Adjustment
                    if (IsSaleAdjustment && SLItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                    {

                        dAdjustmentCommitedQty = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentCommitedQty);

                        //if (SLItem.ItemId == AdjustmentItemID()) //if clause added for GSS and Adv @ a time
                        //{

                        dSaleAdjustmentGoldAmt = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentGoldAmt);
                        dSaleAdjustmentGoldQty = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentGoldQty);
                        dSaleAdjustmentMetalRate = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentMetalRate);
                        sAdjuOrderNo = Convert.ToString(SLItem.PartnerData.SaleAdjustmentOrderNo);

                        string sCurrentRate = GetPureMetalRate();
                        // decimal dPureVal = dSaleAdjustmentGoldQty * Convert.ToDecimal(sCurrentRate);
                        string sTransId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentTransactionID);
                        string sStoreId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentStoreId);
                        string sTerminalId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentTerminalId);

                        if (dAdjustmentCommitedQty == 0)
                        {
                            dAdjAmt += dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty;
                            dCtoAdvAdjAmt += dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty; //dCAdvAdjustmentAvgGoldRate
                            dAdjQty += dSaleAdjustmentGoldQty;
                        }
                        else
                        {
                            //========================Soutik==============================================================
                            iSaleAdjustmentCommitedForDays = Convert.ToInt32(SLItem.PartnerData.SaleAdjustmentCommitedForDays);
                            CustAdvDepositDate = Convert.ToDateTime(SLItem.PartnerData.SaleAdjustmentAdvanceDepositDate);

                            if (dSaleAdjustCommitedQty == 0 && !IsAdjustmentCommitedQtyDone)
                            {
                                dSaleAdjustCommitedQty = Convert.ToDecimal(dAdjustmentCommitedQty);
                                IsAdjustmentCommitedQtyDone = true;

                                retailTrans.PartnerData.CustAdvCommitedQty = dSaleAdjustCommitedQty;
                                retailTrans.PartnerData.CustDepositDate = Convert.ToString(SLItem.PartnerData.SaleAdjustmentAdvanceDepositDate);
                                dAdjAmt += dSaleAdjustmentMetalRate * dAdjustmentCommitedQty;
                                dCtoAdvAdjAmt += dSaleAdjustmentMetalRate * dAdjustmentCommitedQty;
                                dAdjQty += dAdjustmentCommitedQty;
                            }
                        }

                        //============================================================================================
                    }
                    #endregion

                    #region Sales Exchange Adjust dev on 12012018
                    if (IsSaleAdjustment && SLItem.ReturnLineId != 0)
                    {
                        dSaleAdjustmentGoldQty = Convert.ToDecimal(SLItem.PartnerData.CalExchangeQty);

                        if (dSaleAdjustmentGoldQty > 0)
                        {
                            dAdjAmt += Math.Abs(Convert.ToDecimal(SLItem.NetAmountWithNoTax));

                            dCtoAdvAdjAmt += Math.Abs(Convert.ToDecimal(SLItem.NetAmountWithNoTax));

                            dAdjQty += dSaleAdjustmentGoldQty;
                        }
                    }
                    #endregion
                }

                //if (retailTrans != null)
                //{
                //    string orderAdj = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);
                //    int iLineNum = 1;

                //    if (!string.IsNullOrEmpty(orderAdj))
                //    {
                //        getOrderInfoAtMultiAdj(orderAdj, iLineNum);
                //    }
                //}

                if (IsSaleAdjustment)
                {
                    if (dAdjQty > 0)
                    {
                        dSaleAdjustmentGoldQty = dAdjQty;
                        dSaleAdjustmentAvgGoldRate = (dAdjAmt / dAdjQty);
                        dCAdvAdjustmentAvgGoldRate = (dCtoAdvAdjAmt / dAdjQty);//dCtoAdvAdjAmt replace with dAdjAmt
                    }
                    else
                    {
                        dSaleAdjustmentGoldQty = 0;
                        dSaleAdjustmentAvgGoldRate = 0;
                        dCAdvAdjustmentAvgGoldRate = 0;
                    }

                    retailTrans.PartnerData.dSaleAdjustmentAvgGoldRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                    retailTrans.PartnerData.dCAdvAdjustmentAvgGoldRate = Convert.ToString(dCAdvAdjustmentAvgGoldRate);
                    dPrevTransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty);
                }

                #region old logic
                //if (IsSaleAdjustment)
                //{
                //    dSaleAdjustmentGoldAmt = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldAmt);
                //    dSaleAdjustmentGoldQty = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldQty);

                //    if (dSaleAdjustmentGoldQty > 0)
                //    {
                //        dSaleAdjustmentAvgGoldRate = (dSaleAdjustmentGoldAmt / dSaleAdjustmentGoldQty);
                //    }
                //    else
                //    {
                //        dSaleAdjustmentAvgGoldRate = 0;
                //    }

                //    dPrevTransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty);
                //}

                #endregion



                BindIngredientGrid();

                FillQtyPcsFromSKUTable();

                iCallfrombase = 1;

                alist = al;
                FnArrayList();

                iCallfrombase = 0;

                if (isMRPUCP)
                {
                    txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text), 3, MidpointRounding.AwayFromZero));
                    txtChangedMakingRate.Text = string.Empty;
                    txtActMakingRate.Text = string.Empty;
                    cmbRateType.SelectedIndex = (int)RateType.Tot;
                    txtRate.Enabled = false;
                    txtQuantity.Enabled = false;
                    txtQuantity.BackColor = SystemColors.Control;
                    txtChangedMakingRate.Enabled = false;
                    txtChangedMakingRate.BackColor = SystemColors.Control;
                    txtLineDisc.Enabled = true;
                    txtLineDisc.BackColor = SystemColors.Window;
                }
                else
                {
                    txtLineDisc.Enabled = false;
                    txtLineDisc.BackColor = SystemColors.Control;
                    txtRate.Enabled = false;
                    txtRate.BackColor = SystemColors.Control;

                    if (IsValidSKU(sBaseItemID))
                    {
                        bValidSKU = true;
                        txtQuantity.Enabled = false;
                        txtQuantity.BackColor = SystemColors.Control;
                        txtChangedMakingRate.Enabled = true;
                        txtWtDiffDiscQty.Enabled = true;
                        LockSKU(sBaseItemID, 1);
                    }
                    else
                    {
                        bValidSKU = false;
                        txtQuantity.Enabled = true;
                        txtQuantity.BackColor = SystemColors.Window;
                        txtChangedMakingRate.Enabled = true;
                        lblMetalRatesShow.Text = txtRate.Text;
                        txtWtDiffDiscQty.Enabled = false;
                    }

                    if (retailTrans.PartnerData.IsRepairReturn)
                    {
                        DataTable dtOGRep = GetOGrepairInfo();

                        DropOGrepairInfo();
                        if (dtOGRep != null && dtOGRep.Rows.Count > 0)
                        {
                            sOGBatchIdForRepirRet = Convert.ToString(dtOGRep.Rows[0]["INVENTBATCHID"]);
                            txtQuantity.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(Convert.ToString(dtOGRep.Rows[0]["QTY"])), 3, MidpointRounding.AwayFromZero));
                        }
                    }
                }


                txtQuantity.Text = !string.IsNullOrEmpty(txtQuantity.Text) ? decimal.Round(Convert.ToDecimal(txtQuantity.Text), 3, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtRate.Text = !string.IsNullOrEmpty(txtRate.Text) ? decimal.Round(Convert.ToDecimal(txtRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtPCS.Text = !string.IsNullOrEmpty(txtPCS.Text) ? decimal.Round(Convert.ToDecimal(txtPCS.Text), 0, MidpointRounding.AwayFromZero).ToString() : string.Empty;

                #endregion

                if (!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                {
                    if (!isViewing)
                        CheckRateFromDB();

                    #region for totmaking type

                    CalMakingForTot();

                    #endregion

                    #region new Making disc
                    //string sFieldName = string.Empty;
                    //int iSPId = 0;
                    //iSPId = getUserDiscountPermissionId();
                    decimal dinitDiscValue = 0;

                    decimal dQty = 0;
                    if (!string.IsNullOrEmpty(txtQuantity.Text))
                        dQty = Convert.ToDecimal(txtQuantity.Text);
                    else
                        dQty = 0;
                    if (!isMRPUCP)
                        dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "OPENINGDISCPCT");// get OPENINGDISCPCT field value FOR THE OPENING
                    //else
                    //    MessageBox.Show("Invalid discount policy for this item");

                    decimal dMkPerDisc = 0;
                    if (!string.IsNullOrEmpty(txtMakingDisc.Text))
                        dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
                    else
                        dMkPerDisc = 0;


                    int iPcs = 0;

                    if (!string.IsNullOrEmpty(txtPCS.Text))
                        iPcs = Convert.ToInt16(txtPCS.Text);
                    else
                        iPcs = 0;

                    //if(dinitDiscValue > 0)
                    //    txtMakingDisc.Enabled = false;
                    //else
                    //    txtMakingDisc.Enabled = true;
                    decimal dDiscValue = 0m;

                    if (dinitDiscValue >= 0 && !string.IsNullOrEmpty(txtMakingAmount.Text))
                    {
                        if ((dMkPerDisc > dinitDiscValue))
                        {
                            MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                            txtMakingDisc.Focus();
                        }
                        else
                        {
                            if (iMkDisType == (int)MakingDiscType.Percent)
                                dDiscValue = (Convert.ToDecimal(txtMakingAmount.Text) * dinitDiscValue) / 100;
                            else if (iMkDisType == (int)MakingDiscType.PerGram)
                                dDiscValue = (dQty * dinitDiscValue);
                            else
                                dDiscValue = dinitDiscValue * iPcs;

                            txtMakingDisc.Text = Convert.ToString(dDiscValue);
                            txtMakingDisc.Tag = iIsMkDiscFlat;
                            //iIsMkDiscFlat 
                            //iMkDisType 
                        }
                    }

                    //else
                    //{
                    //    MessageBox.Show("Not allowed for this item");
                    //}
                    CheckMakingDiscountFromDB(dinitDiscValue);
                    #endregion
                }

                #region value assign in control
                if (IsBulkItem(sBaseItemID))
                {
                    isBulkItem = true;
                    txtRate.Enabled = false;
                    txtQuantity.Enabled = true;
                    txtQuantity.BackColor = SystemColors.Window;
                }
                else
                {
                    isBulkItem = false;
                    txtRate.Enabled = false;
                    txtQuantity.Enabled = false;
                    txtQuantity.BackColor = SystemColors.Control;
                }

                txtAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;




                if (isMRPUCP)
                {
                    txtTotalAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                    txtActToAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                }
                else
                {
                    txtActToAmount.Text = txtTotalAmount.Text;
                }

                if (string.IsNullOrEmpty(txtChangedMakingRate.Text))
                {
                    txtChangedMakingRate.Text = "0";
                    txtActMakingRate.Text = "0";
                }
                #endregion

                #region Auot cal for CO rate freezed line wise Multi Adjustment
                if (retailTrans != null)
                {
                    string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;
                    string order = "";
                    string cust = "";
                    //int iLineNum = 0;
                    getMultiAdjOrderNo(sTableName, ref order, ref cust, ref OlineNum);

                    string orderAdj1 = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);


                    //if (!string.IsNullOrEmpty(orderAdj1))
                    //{
                    //    order = GetBookedOrderNo(sBaseItemID, sCustomerId, orderAdj1, ref iLineNum);
                    //}

                    SelectItems("", order);
                    lblCustOrder.Text = "NO SELECTION";
                    lblCustOrder.Text = order;

                    if (!string.IsNullOrEmpty(order))
                    {
                        cmbCustomerOrder.Enabled = false;
                        // OlineNum = GetCustOrderLineNoBookedItemWise(order, sBaseItemID);
                        btnSelectItems.Enabled = false;
                    }
                    else
                    {
                        OlineNum = 0;
                        cmbCustomerOrder.Enabled = false;
                        btnSelectItems.Enabled = false;
                    }

                    if (OlineNum > 0)
                    {
                        lblItemSelected.Text = "ORDER : " + order + " SELECTED LINE NO. : " + OlineNum;
                    }

                    dropMultiAdjOrderNo(sTableName);
                }
                #endregion

                // ManagerPermissionEnabled();

                #region Gold Tax Cal
                /* if (RadioChecked == Convert.ToString((int)TransactionType.Sale))
                {
                    decimal dGVal = 0m;
                    decimal dTaxPerc = 0m;
                    //txtGoldTax.Visible = true;
                    //label13.Visible = true;
                    int iMetalType = GetMetalType(sBaseItemID);
                    if (!string.IsNullOrEmpty(txtgval.Text))
                    {
                        dGVal = Convert.ToDecimal(txtgval.Text);
                        dTaxPerc = getItemTaxPercentage(sBaseItemID);

                        if (dTaxPerc > 0 && dGVal > 0)
                            txtGoldTax.Text = decimal.Round(dGVal * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero).ToString();// Convert.ToString(dGVal * (dTaxPerc / 100));
                        else
                            txtGoldTax.Text = "0";
                    }
                    else if (isBulkItem == true && iMetalType == (int)MetalType.Gold)
                    {
                        if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        {
                            dGVal = Convert.ToDecimal(txtTotalAmount.Text);
                            dTaxPerc = getItemTaxPercentage(sBaseItemID);

                            if (dTaxPerc > 0 && dGVal > 0)
                                txtGoldTax.Text = decimal.Round(dGVal * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero).ToString();// Convert.ToString(dGVal * (dTaxPerc / 100));
                            else
                                txtGoldTax.Text = "0";
                        }
                    }
                    else
                        txtGoldTax.Text = "0";
                }
                else
                {
                    txtGoldTax.Text = "0";
                    txtGoldTax.Visible = false;
                    label13.Visible = false;
                }*/
                #endregion
                if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text));//+ Convert.ToDecimal(txtGoldTax.Text)

                if (!string.IsNullOrEmpty(txtRate.Text))
                {
                    decimal dRate = Convert.ToDecimal(txtRate.Text);
                    string sRate = "0";

                    if (dRate == 0)
                        sRate = getRateFromMetalTable(0);
                    else
                        sRate = Convert.ToString(dRate);

                    txtRate.Text = sRate;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text) && !string.IsNullOrEmpty(txtActToAmount.Text))
                    {
                        decimal dActAmt = 0;
                        dActAmt = Convert.ToDecimal(txtActToAmount.Text);

                        if (dActAmt == 0)
                            txtActToAmount.Text = txtTotalAmount.Text;
                    }
                }
                else
                {
                    if (!bValidSKU && !string.IsNullOrEmpty(lblCustOrder.Text)
                       && lblCustOrder.Text != "NO CUSTOMER ORDER"
                       && !string.IsNullOrEmpty(sBaseItemID))
                    {
                        string sBIRate = "0";
                        string sBIQty = "";
                        string sBlank = "";
                        string sqlString = "SELECT CRate,QTY,ORDERNUM,0,0 FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + lblCustOrder.Text + "'" +
                          " AND ITEMID='" + sBaseItemID + "' AND LINENUM=" + OlineNum + "";

                        NIM_ReturnMoreValues(sqlString, out sBIRate, out sBIQty, out sBlank, out sBlank, out sBlank);
                        txtRate.Text = sBIRate;
                        txtQuantity.Text = sBIQty;
                        lblMetalRatesShow.Text = sBIRate;
                        //txtQuantity.Focus();
                        txtWtDiffDiscQty.Focus();
                    }
                    if (lblCustOrder.Text == "NO CUSTOMER ORDER")
                    {
                        lblItemSelected.Text = "";
                    }
                }
            }

            if (isMRPUCP)
                txtWtDiffDiscQty.Enabled = false;
            else
                txtWtDiffDiscQty.Enabled = true;


        }

        private void getOrderInfoAtMultiAdj(string sOrd, int iLine = 0)
        {
            string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;
            string order = "";
            string cust = "";
            getMultiAdjOrderNo(sTableName, ref order, ref cust, ref OlineNum);

            //if (string.IsNullOrEmpty(order))
            //{
            //order = GetBookedOrderNo(sBaseItemID, sCustomerId, sOrd, ref OlineNum);
            //}
            //if (!string.IsNullOrEmpty(order))
            //{
            //    OlineNum = GetCustOrderLineNoBookedItemWise(order, sBaseItemID);
            //}

            if (!string.IsNullOrEmpty(order))
            {
                sAdjuOrderNo = order;
                lblCustOrder.Text = sAdjuOrderNo;
            }

            #region for PNG Multiple adj added on 140319
            if (!string.IsNullOrEmpty(order) && OlineNum > 0)
            {
                string sqlString = "SELECT GoldRate,SilverRate,PlatinumRate,MetalQty,CONFIGID FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + order + "'" +
                     " AND LINENUM=" + OlineNum + "";

                NIM_ReturnMoreValues(sqlString, out sGRate, out sSRate, out sPRate, out sMQty, out sOrderLineConfig);
            }
            #endregion
        }



        private void CalMakingForTot()
        {
            decimal dMakingQty = 0;
            decimal decimalAmount = 0m;
            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;
            decimal dTotAmt = 0m;
            decimal dActTotAmt = 0m;
            decimal dManualChangesAmt = 0m;
            decimal dMkDiscTotAmt = 0m;
            decimal dQty = 0m;
            if (cmbMakingType.SelectedIndex == 2)
            {
                if (!string.IsNullOrEmpty(txtAmount.Text))
                    decimalAmount = Convert.ToDecimal(txtAmount.Text);
                else
                    decimalAmount = 0;

                if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                    dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                else
                    dMkRate1 = 0;

                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                    dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                else
                    dMkRate = 0;

                if (dMkRate1 > 0 && decimalAmount > 0)
                {
                    txtTotalAmount.Text = Convert.ToString(decimal.Round((decimalAmount + dMkRate1), 2, MidpointRounding.AwayFromZero));
                    txtActToAmount.Text = Convert.ToString(decimal.Round((decimalAmount + dMkRate1), 2, MidpointRounding.AwayFromZero));
                    txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate1), 2, MidpointRounding.AwayFromZero));
                }

                if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                    dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                else
                    dTotAmt = 0;
                if (!string.IsNullOrEmpty(txtActToAmount.Text))
                    dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                else
                    dActTotAmt = 0;
                if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                    dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                else
                    dMkDiscTotAmt = 0;

                if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                    dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);
                else
                    dManualChangesAmt = 0;

                decimal dMkAmt = 0;
                if (dActTotAmt > 0 && dManualChangesAmt > 0)
                {
                    dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                    // txtChangedMakingRate.Text = txtActMakingRate.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                    txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                }
                else if (dMkRate1 > 0 && dMkRate > 0)
                {
                    dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                    // txtChangedMakingRate.Text = txtActMakingRate.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                    txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                }

                //txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                //decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());

                //txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                //// txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                //if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                //    dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                //else
                //    dMkAmt = 0;


                if (dMkAmt > 0)
                {
                    if (dMkDiscTotAmt == 0)
                        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                    else
                        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                }
                else
                    txtChangedMakingRate.Text = txtActMakingRate.Text;
            }
            //else if (cmbMakingType.SelectedIndex == 3)
            //{
            //    if (!string.IsNullOrEmpty(txtgval.Text))
            //    {
            //        if (Convert.ToDecimal(txtgval.Text.Trim()) != 0)
            //            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()));
            //        else
            //            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
            //    }
            //    else
            //    {
            //        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
            //    }

            //    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));

            //    if (decimalAmount > 0 && !string.IsNullOrEmpty(txtAmount.Text))
            //    {
            //        decimal dAmt = Convert.ToDecimal((txtAmount.Text));
            //        txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount + dAmt, 2, MidpointRounding.AwayFromZero));
            //        txtActToAmount.Text = txtTotalAmount.Text;
            //    }
            //}
        }

        #region Qty and Pcs Fill up
        private void FillQtyPcsFromSKUTable()
        {
            if (!isViewing)
            {
                if (rdbSale.Checked)
                {
                    if (conn.State == ConnectionState.Closed)
                        conn.Open();

                    string commandText = " SELECT     TOP (1) SKUTableTrans.PDSCWQTY AS PCS , SKUTableTrans.QTY AS QTY " + //SKU Table New
                                         " FROM         SKUTableTrans " +
                                         " WHERE     (SKUTableTrans.SkuNumber = '" + ItemID.Trim() + "') ";

                    SqlCommand command = new SqlCommand(commandText, conn);
                    command.CommandTimeout = 0;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                txtPCS.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(reader.GetValue(0)), 3, MidpointRounding.AwayFromZero));
                                txtQuantity.Text = Convert.ToString(reader.GetValue(1));

                            }
                        }
                    }
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                }
            }

        }
        #endregion

        private void FnArrayList()
        {
            if (alist.Count == 0)
            {
                alist.Add("NO CUSTOMER ORDER");
                cmbCustomerOrder.DataSource = alist;
                cmbCustomerOrder.Enabled = false;

                btnSelectItems.Visible = false;
            }
            else if (alist.Count == 1 && alist.Contains(Convert.ToString("NO CUSTOMER ORDER")))
            {

                cmbCustomerOrder.Enabled = false;

                btnSelectItems.Visible = false;
            }
            else
            {
                if (!alist.Contains(Convert.ToString("NO SELECTION")))
                {

                    alist.Add("NO SELECTION");
                    cmbCustomerOrder.DataSource = alist;
                    cmbCustomerOrder.Enabled = false;

                    btnSelectItems.Visible = false;
                }
                else if (alist.Count > 1)
                {
                    cmbCustomerOrder.DataSource = alist;
                    cmbCustomerOrder.Enabled = false;

                    btnSelectItems.Visible = false;
                }
            }
        }

        private void BindIngredientGrid()
        {
            if (rdbSale.Checked)
            {
                dtIngredients = new DataTable();

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                StringBuilder commandText = new StringBuilder();
                commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + ItemID.Trim() + "')");
                commandText.Append("  BEGIN  ");
                commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
                commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
                commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
                commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
                commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
                commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' ORDER BY X.METALTYPE END ");


                SqlCommand command = new SqlCommand(commandText.ToString(), conn);
                command.CommandTimeout = 0;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dtIngredients.Load(reader);
                }

                if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    #region // Stone Discount

                    dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                    dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                    dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                    dtIngredients.Columns.Add("CTYPE", typeof(int));

                    #endregion

                    txtRate.Text = string.Empty;
                    dtIngredientsClone = new DataTable();
                    dtIngredientsClone = dtIngredients.Clone();

                    foreach (DataRow drClone in dtIngredients.Rows)
                    {
                        if (isMRPUCP)
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

                    if (!isMRPUCP)
                    {
                        decimal dTotWt = 0m;
                        if (bIsGrossMetalCal)
                        {
                            foreach (DataRow dr in dtIngredientsClone.Rows)
                            {
                                dTotWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                            }
                        }

                        foreach (DataRow dr in dtIngredientsClone.Rows)
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


                            if (Convert.ToDecimal(dr["PCS"]) > 0)
                                dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                            else
                                dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);

                            // Stone Discount
                            int iStoneDiscType = 0;
                            decimal dStoneDiscAmt = 0;
                            decimal dStoneDiscTotAmt = 0m;

                            if ((IsGSSTransaction) && (dGSSMaturityQty > 0) && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                            {
                                #region GSS New
                                //sRate = Convert.ToString(getGSSRate(GrWeight));
                                sRate = Convert.ToString(getGSSRate(GrWeight, ConfigID, dTotWt));
                                sGSSAdjustmentGoldRate = sRate;
                                #endregion
                            }
                            else if (IsSaleAdjustment   // Avg Gold Rate Adjustment
                                    && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum)
                                    )//&& (dSaleAdjustmentAvgGoldRate > 0)//commented on 080419
                            {
                                if (isViewing)
                                    sRate = Convert.ToString(dViewModeCalAdjRateforMetal);// Convert.ToString(dr["RATE"]);
                                else
                                    sRate = Convert.ToString(getAdjustmentRate(GrWeight, ConfigID, dTotWt));//3


                                if (Convert.ToDecimal(sRate) == 0)
                                    sRate = getRateFromMetalTable();

                                sAdvAdjustmentGoldRate = sRate;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(sAdjuOrderNo))
                                {
                                    if (!bIsGrossMetalCal)
                                        sRate = getRateFromMetalTable();
                                    else if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold)
                                        sRate = getRateFromMetalTable();
                                }
                                else
                                {
                                    if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum)
                                        sRate = Convert.ToString(getAdjustmentRate(GrWeight, ConfigID, dTotWt));
                                    else
                                        sRate = getRateFromMetalTable();
                                }
                            }

                            if (!string.IsNullOrEmpty(sRate))
                            {
                                sCalcType = GetIngredientCalcType(ref iStoneDiscType, ref dStoneDiscAmt);
                                if (!string.IsNullOrEmpty(sCalcType))
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
                            if (!bIsGrossMetalCal && IsNoPriceRequird(Convert.ToString(dr["ItemID"])) == 0)
                            {
                                if ((Convert.ToDecimal(dr["Rate"]) <= 0)
                                    && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("0 Stone Rate is not valid for this item ", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                                    }
                                    isCancelClick = true;
                                    this.Close();
                                    return;
                                }
                            }

                            //INVENTTABLE ADD NOPRICEREQUIRED

                            StringBuilder commandText1 = new StringBuilder();
                            commandText1.Append("select metaltype,ISNULL(NOPRICEREQUIRED,0) from inventtable where itemid='" + Convert.ToString(dr["ItemID"]) + "'");

                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

                            SqlCommand command1 = new SqlCommand(commandText1.ToString(), conn);
                            command1.CommandTimeout = 0;
                            SqlDataReader reader1 = command1.ExecuteReader();
                            if (reader1.HasRows)
                            {
                                #region Reader
                                while (reader1.Read())
                                {
                                    //---- add code --  -- for CalcType
                                    if ((int)reader1.GetValue(0) == (int)MetalType.LooseDmd || (int)reader1.GetValue(0) == (int)MetalType.Stone)
                                    {
                                        //if((int)reader1.GetValue(1) == 0)
                                        //{
                                        if (!string.IsNullOrEmpty(sCalcType))
                                        {
                                            if (Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Weight))
                                            {
                                                if (dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }

                                            else if (Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Pieces))
                                            {
                                                if (dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["PCS"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }

                                            else // Tot
                                            {
                                                if (dStoneDiscAmt > 0)
                                                {
                                                    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                }
                                                //dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        else
                                        {
                                            if (dStoneDiscAmt > 0)
                                            {
                                                dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                            }
                                            // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                            dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }
                                        //} // commented on 28/03/17 By S.Sharma
                                        //else
                                        //{
                                        //    dr["CValue"] = 0;
                                        //}
                                    }
                                    else
                                    {
                                        //  if(!bIsGrossMetalCal)
                                        dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                        //else
                                        //    dr["CValue"] = decimal.Round(dTotWt * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                    }

                                    // --- end

                                    if ((int)reader1.GetValue(0) == (int)MetalType.Gold)
                                    {
                                        decimal dgValue = 0;
                                        if (!string.IsNullOrEmpty(txtgval.Text))
                                            dgValue = Convert.ToDecimal(txtgval.Text);

                                        txtgval.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(dgValue + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                    }

                                    if ((int)reader1.GetValue(0) == (int)MetalType.Silver
                                            || (int)reader1.GetValue(0) == (int)MetalType.Platinum)
                                    {
                                        decimal dOMValue = 0;
                                        if (!string.IsNullOrEmpty(txtOMValue.Text))
                                            dOMValue = Convert.ToDecimal(txtOMValue.Text);

                                        txtOMValue.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(dOMValue + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                    }
                                }
                                #endregion
                            }
                            if (conn.State == ConnectionState.Open)
                                conn.Close();

                            // Stone Discount
                            if (dStoneDiscAmt > 0)
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

                            //

                            txtRate.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));

                        }

                        dtIngredientsClone.AcceptChanges();

                        cmbRateType.SelectedIndex = (int)RateType.Tot;
                        cmbRateType.Enabled = false;

                        // Added on 07.06.2013
                        txtPCS.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtRate.Enabled = false;
                        //txtChangedMakingRate.Enabled = false;
                        txtMakingDisc.Enabled = false;
                        cmbMakingType.Enabled = false;
                        // txtPurity.Enabled = false;
                        // btnAdd.Focus();
                        //
                    }
                    else
                    {
                        txtRate.Text = MRPUCP;
                        lblMetalRatesShow.Text = MRPUCP;
                    }
                    btnIngrdientsDetails.Visible = true;

                    #region  lblMetalRatesShow shwo purpose
                    decimal dRate = 0m;
                    decimal dGoldQty = 0m;
                    foreach (DataRow dr in dtIngredientsClone.Rows)
                    {
                        if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold
                            || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver
                            || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum
                            || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["Rate"])) && !string.IsNullOrEmpty(Convert.ToString(dr["QTY"])))
                            {
                                dGoldQty += Convert.ToDecimal(dr["QTY"]);
                                dRate += (Convert.ToDecimal(dr["Rate"]) * Convert.ToDecimal(dr["QTY"]));
                            }
                        }
                    }
                    if (dRate > 0 && dGoldQty > 0)
                        lblMetalRatesShow.Text = Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dRate / dGoldQty)), 2, MidpointRounding.AwayFromZero));

                    #endregion
                }
                else
                {
                    if (!isMRPUCP)
                    {
                        #region [ GSS Transaction]
                        if (IsGSSTransaction)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if (iMetalType == (int)MetalType.Gold)
                            {
                                if (!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getGSSRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));
                                    if (dMetalRate > 0)
                                    {
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                        lblMetalRatesShow.Text = txtRate.Text;
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                        lblMetalRatesShow.Text = txtRate.Text;
                                    }

                                }
                                else
                                {
                                    decimal dSKUQty = 0;

                                    dSKUQty = GetSKUQty(ItemID);

                                    if (dSKUQty > 0)
                                    {
                                        decimal dMetalRate = 0;
                                        if (string.IsNullOrEmpty(txtQuantity.Text.Trim()))//added on 091117
                                        {
                                            txtQuantity.Text = Convert.ToString(dSKUQty);
                                        }

                                        dMetalRate = getGSSRate(Convert.ToString(dSKUQty), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));

                                        if (dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                            lblMetalRatesShow.Text = txtRate.Text;
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                            lblMetalRatesShow.Text = txtRate.Text;
                                        }
                                    }
                                    else
                                    {
                                        txtRate.Text = getRateFromMetalTable();
                                        lblMetalRatesShow.Text = txtRate.Text;
                                    }

                                }
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                                lblMetalRatesShow.Text = txtRate.Text;
                            }

                        }
                        #endregion

                        #region [Sale Adjustment] // Avg Gold Rate Adjustment
                        else if (IsSaleAdjustment)
                        {
                            if (IsCustOrderWithAdv(sAdjuOrderNo))
                            {
                                getNewAdjustmentRate();
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                                lblMetalRatesShow.Text = txtRate.Text;
                            }
                        }
                        #endregion
                        else if (!string.IsNullOrEmpty(sAdjuOrderNo))//if with out adv order
                        {
                            if (IsCustOrderWithAdv(sAdjuOrderNo))
                            {
                                getNewAdjustmentRate();
                            }
                            else
                            {
                                txtRate.Text = getRateFromMetalTable();
                                lblMetalRatesShow.Text = txtRate.Text;
                            }
                        }
                        else
                        {
                            txtRate.Text = getRateFromMetalTable();
                            lblMetalRatesShow.Text = txtRate.Text;
                        }
                    }
                    else
                    {
                        txtRate.Text = MRPUCP;
                        lblMetalRatesShow.Text = txtRate.Text;
                    }
                    btnIngrdientsDetails.Visible = false;
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
            else
            {
                txtRate.Text = getRateFromMetalTable();
                lblMetalRatesShow.Text = txtRate.Text;
            }
        }

        private bool IsCustOrderWithAdv(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT WITHADVANCE  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private void getNewAdjustmentRate()
        {
            int iMetalType = GetMetalType(ItemID);
            //getMetalRate(string sItemId, string sConfigId)
            if (iMetalType == (int)MetalType.Gold
                || iMetalType == (int)MetalType.Silver
                || iMetalType == (int)MetalType.Platinum)
            {
                if (!string.IsNullOrEmpty(txtQuantity.Text))
                {
                    decimal dMetalRate = 0;

                    dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));//4
                    if (dMetalRate > 0)
                    {
                        txtRate.Text = Convert.ToString(dMetalRate);
                        lblMetalRatesShow.Text = txtRate.Text;
                    }
                    else
                    {
                        txtRate.Text = getRateFromMetalTable();
                        lblMetalRatesShow.Text = txtRate.Text;
                    }
                }
                else
                {
                    decimal dSKUQty = 0;

                    dSKUQty = GetSKUQty(ItemID);
                    decimal dMetalRate = 0;
                    if (dSKUQty > 0)
                    {
                        dMetalRate = getAdjustmentRate(Convert.ToString(dSKUQty), ConfigID, Convert.ToDecimal(dSKUQty));//5
                        if (dMetalRate > 0)
                        {
                            txtRate.Text = Convert.ToString(dMetalRate);
                            lblMetalRatesShow.Text = txtRate.Text;
                        }
                        else
                        {
                            txtRate.Text = getRateFromMetalTable();
                            lblMetalRatesShow.Text = txtRate.Text;
                        }
                    }
                    else if (!IsRetailItem(ItemID))
                    {
                        dMetalRate = getAdjustmentRate(Convert.ToString(dSKUQty), ConfigID, Convert.ToDecimal(dSKUQty));//5
                        if (dMetalRate > 0)
                        {
                            txtRate.Text = Convert.ToString(dMetalRate);
                            lblMetalRatesShow.Text = txtRate.Text;
                        }
                        else
                        {
                            txtRate.Text = getRateFromMetalTable();
                            lblMetalRatesShow.Text = txtRate.Text;
                        }
                    }
                    else
                    {
                        txtRate.Text = getRateFromMetalTable();
                        lblMetalRatesShow.Text = txtRate.Text;
                    }
                }
            }
            else
            {
                txtRate.Text = getRateFromMetalTable();
                lblMetalRatesShow.Text = txtRate.Text;
            }
        }

        public frmCustomFieldCalculations(string sPcs, string sQty, string sRate, string sRateType, string sMRate, string sMType, string sAmt,
                                      string sMDisc, string sMAmt, string sTAmt, string sTWeight, string sLossPct, string sLossWt,
                                      string sEQty, string sTType, string sOChecked, DataSet dsIng, string ordernum, string orderlinenum,
                                      string sWastageType, string sWastagePercentage, string sWastageQty, string sWastageAmount,
                                      string sMkDiscType, string sTotMkDicAmt, string sPurity, string sRateShow, string sActTotAmt,
                                      string sActMakingRate, string sItemId, string sLineDisc, SaleLineItem saleLineItem, RetailTransaction retTrans,
                                      SqlConnection connection, IApplication _application) // changed for wastage // Making Discount
        {
            InitializeComponent();
            application = _application;
            btnCLose.Visible = false;
            txtMakingDisc.Enabled = false;
            BindRateTypeMakingTypeCombo();
            BindWastage();
            BindMakingDiscount();
            isViewing = true;
            retailTrans = retTrans;
            conn = connection;
            // panel1.Enabled = false;
            sBaseItemID = sItemId;
            int iMetalType = GetMetalType(sBaseItemID);

            sBaseUnitId = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

            if (IsBulkItem(sBaseItemID))
                isBulkItem = true;
            else
                isBulkItem = false;

            if (IsValidSKU(sBaseItemID))
                bValidSKU = true;
            else
                bValidSKU = false;

            _saleLineItem = saleLineItem;
            panel3.Enabled = false;
            // btnAdd.Enabled = false;

            if (!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
            {
                sCustomerId = retailTrans.Customer.CustomerId;
            }

            ItemID = sBaseItemID;
            panel3.Enabled = false;

            StoreID = ApplicationSettings.Database.StoreID;

            ConfigID = saleLineItem.Dimension.ConfigId;
            SizeID = saleLineItem.Dimension.SizeId;
            ColorID = saleLineItem.Dimension.ColorId;
            StyleID = saleLineItem.Dimension.StyleId;




            if (_saleLineItem.PartnerData.isMRP == true)
            {
                txtLineDisc.Enabled = true;
                isMRPUCP = true;
            }
            else
            {
                txtLineDisc.Enabled = false;
                isMRPUCP = false;
            }

            int isServiceItem = 0;
            decimal dGTaxV = 0m;
            decimal dGValue = 0m;
            decimal dTotAmt = 0m;
            decimal dMkAmt = 0m;
            decimal dOMValue = 0m;

            //if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.LINEGOLDTAX))
            //    dGTaxV = Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(_saleLineItem.PartnerData.LINEGOLDTAX), 2, MidpointRounding.AwayFromZero)));

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.LINEGOLDVALUE))
                dGValue = Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(_saleLineItem.PartnerData.LINEGOLDVALUE), 2, MidpointRounding.AwayFromZero)));

            txtgval.Text = Convert.ToString(dGValue);

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.LINEOMVALUE))
                dOMValue = Convert.ToDecimal(Convert.ToString(decimal.Round(Convert.ToDecimal(_saleLineItem.PartnerData.LINEOMVALUE), 2, MidpointRounding.AwayFromZero)));

            txtOMValue.Text = Convert.ToString(dOMValue);
            //_saleLineItem.PartnerData.LINEGOLDVALUE;
            //txtGoldTax.Text = Convert.ToString(dGTaxV);
            //_saleLineItem.PartnerData.LINEGOLDTAX;


            if (!string.IsNullOrEmpty(sTAmt))
                dTotAmt = Convert.ToDecimal(sTAmt);

            if (!string.IsNullOrEmpty(sMAmt))
                dMkAmt = Convert.ToDecimal(sMAmt);

            IsGSSTransaction = retTrans.PartnerData.IsGSSMaturity;

            IsSaleAdjustment = retTrans.PartnerData.SaleAdjustment;

            #region Adj
            decimal dAdjAmt = 0;
            decimal dCtoAdvAdjAmt = 0;
            decimal dAdjQty = 0;
            dPrevTransAdjGoldQty = 0;
            foreach (SaleLineItem SLItem in retailTrans.SaleItems)
            {
                if (IsSaleAdjustment && SLItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                {
                    #region Adv Adjust
                    dSaleAdjustmentGoldAmt = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentGoldAmt);
                    dSaleAdjustmentGoldQty = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentGoldQty);
                    dSaleAdjustmentMetalRate = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentMetalRate);
                    sAdjuOrderNo = Convert.ToString(SLItem.PartnerData.SaleAdjustmentOrderNo);

                    if (!string.IsNullOrEmpty(sAdjuOrderNo))
                        break;

                    //decimal dConvertedToFixingQty= getConvertedRate();
                    string sCurrentRate = GetPureMetalRate();
                    // decimal dPureVal = dSaleAdjustmentGoldQty * Convert.ToDecimal(sCurrentRate);
                    string sTransId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentTransactionID);
                    string sStoreId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentStoreId);
                    string sTerminalId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentTerminalId);

                    if (IsAdvanceAgainstNone(sTransId, sStoreId, sTerminalId))
                    {
                        if (dSaleAdjustmentMetalRate > Convert.ToDecimal(sCurrentRate))
                            dAdjAmt += Convert.ToDecimal(sCurrentRate) * dSaleAdjustmentGoldQty;
                        else
                            dAdjAmt += dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty;
                    }
                    else
                        dAdjAmt += Convert.ToDecimal(dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty);//dSaleAdjustmentGoldAmt

                    dCtoAdvAdjAmt += dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty; //dCAdvAdjustmentAvgGoldRate

                    dAdjQty += dSaleAdjustmentGoldQty;
                    #endregion
                }

                #region Sales Exchange Adjust dev on 12012018
                if (IsSaleAdjustment && SLItem.ReturnLineId != 0)
                {
                    dSaleAdjustmentGoldQty = Convert.ToDecimal(SLItem.PartnerData.CalExchangeQty);

                    if (dSaleAdjustmentGoldQty > 0)
                    {
                        dAdjAmt += Math.Abs(Convert.ToDecimal(SLItem.NetAmountWithNoTax));

                        dCtoAdvAdjAmt += Math.Abs(Convert.ToDecimal(SLItem.NetAmountWithNoTax));

                        dAdjQty += dSaleAdjustmentGoldQty;
                    }
                }
                #endregion

                if (SLItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                {
                    isServiceItem = 1;
                }
            }

            if (IsSaleAdjustment)
            {
                if (string.IsNullOrEmpty(sAdjuOrderNo))
                {
                    if (dAdjQty > 0)
                    {
                        dSaleAdjustmentGoldQty = dAdjQty;
                        dSaleAdjustmentAvgGoldRate = (dAdjAmt / dAdjQty);
                        dCAdvAdjustmentAvgGoldRate = (dCtoAdvAdjAmt / dAdjQty);
                    }
                    else
                    {
                        dSaleAdjustmentGoldQty = 0;
                        dSaleAdjustmentAvgGoldRate = 0;
                        dCAdvAdjustmentAvgGoldRate = 0;
                    }

                    retailTrans.PartnerData.dSaleAdjustmentAvgGoldRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                    retailTrans.PartnerData.dCAdvAdjustmentAvgGoldRate = Convert.ToString(dCAdvAdjustmentAvgGoldRate);
                }
                else
                {
                    dSaleAdjustmentAvgGoldRate = dSaleAdjustmentMetalRate;
                    dCAdvAdjustmentAvgGoldRate = dSaleAdjustmentMetalRate;
                    retailTrans.PartnerData.dSaleAdjustmentAvgGoldRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                    retailTrans.PartnerData.dCAdvAdjustmentAvgGoldRate = Convert.ToString(dCAdvAdjustmentAvgGoldRate);
                }

                if (!isViewing)
                    dPrevTransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty);
                else
                {
                    DataSet dsIngredients = new DataSet(); // commented on 16/01/17
                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                    dsIngredients.ReadXml(reader);
                    foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                        {
                            dViewModeCalAdjRateforMetal = Convert.ToDecimal(drIng["Rate"]);
                            break;
                        }
                    }
                }
            }
            #endregion

            txtPCS.Enabled = false;
            txtQuantity.Enabled = false;
            txtMakingAmount.Enabled = false;
            txtRate.Enabled = false;
            txtAmount.Enabled = false;
            txtTotalAmount.Enabled = false;
            txtTotalWeight.Enabled = false;
            txtLossPct.Enabled = false;
            txtLossWeight.Enabled = false;
            txtPurity.Enabled = false;
            txtExpectedQuantity.Enabled = false;
            txtActToAmount.Enabled = false;
            txtActMakingRate.Enabled = false;
            //txtChangedMakingRate.Enabled = true;
            txtChangedTotAmount.Enabled = true;

            if (!string.IsNullOrEmpty(sPcs))
                txtPCS.Text = sPcs;// Convert.ToString(decimal.Round(Convert.ToDecimal(sPcs), 3, MidpointRounding.AwayFromZero));
            else
                txtPCS.Text = sPcs;
            if (!string.IsNullOrEmpty(sQty))
                txtQuantity.Text = sQty;// Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
            else
                txtQuantity.Text = sQty;

            if (!string.IsNullOrEmpty(sRate))
                txtRate.Text = sRate;//Convert.ToString(decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero));
            else
                txtRate.Text = sRate;

            lblMetalRatesShow.Text = txtRate.Text;


            txtWtDiffDiscQty.Text = Convert.ToString(saleLineItem.PartnerData.WTDIFFDISCQTY);
            //saleLineItem.PartnerData.LINEOMVALUE
            txtWtDiffDiscAmt.Text = saleLineItem.PartnerData.WTDIFFDISCAMT;



            if (!string.IsNullOrEmpty(sMAmt))
                txtMakingAmount.Text = Convert.ToString(dMkAmt - dGTaxV);// Convert.ToString(decimal.Round(Convert.ToDecimal(sMAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtMakingAmount.Text = Convert.ToString(dMkAmt - dGTaxV);

            cmbRateType.SelectedIndex = Convert.ToInt16(sRateType);

            switch (Convert.ToInt16(sMType))
            {
                case 0:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces)); //0
                    break;
                case 2:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight)); //1
                    break;
                case 3:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot)); //2
                    break;
                case 4:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));  //3
                    break;
                //case 5:
                //    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Inch));
                //    break;
                case 6:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Gross));////4
                    break;
                default:
                    cmbMakingType.SelectedIndex = 0;
                    break;
            }


            txtChangedMakingRate.Text = sMRate;// Convert.ToString(oRounding.Round(Convert.ToDecimal(sMRate), 2));
            txtActMakingRate.Text = sMRate;// Convert.ToString(oRounding.Round(Convert.ToDecimal(sMRate), 2));

            if (!string.IsNullOrEmpty(sAmt))
                txtAmount.Text = (sAmt);// Convert.ToString(decimal.Round(Convert.ToDecimal(sAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtAmount.Text = (sAmt);

            txtMakingDisc.Text = sMDisc;

            if (!string.IsNullOrEmpty(sTAmt))
                txtTotalAmount.Text = Convert.ToString(dTotAmt);// Convert.ToString(decimal.Round(Convert.ToDecimal(sTAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtTotalAmount.Text = Convert.ToString(dTotAmt);

            if (iMetalType == (int)MetalType.Jewellery)
            {
                if (!string.IsNullOrEmpty(sTAmt))
                    txtChangedTotAmount.Text = Convert.ToString(dTotAmt);// added on 270818
                else
                    txtChangedTotAmount.Text = Convert.ToString(dTotAmt);
            }


            if (!string.IsNullOrEmpty(sTWeight))
                txtTotalWeight.Text = sTWeight;// Convert.ToString(decimal.Round(Convert.ToDecimal(sTWeight), 3, MidpointRounding.AwayFromZero));
            else
                txtTotalWeight.Text = sTWeight;

            if (!string.IsNullOrEmpty(sLossPct))
                txtLossPct.Text = sLossPct;// Convert.ToString(decimal.Round(Convert.ToDecimal(sLossPct), 2, MidpointRounding.AwayFromZero));
            else
                txtLossPct.Text = sLossPct;

            if (!string.IsNullOrEmpty(sLossWt))
                txtLossWeight.Text = sLossWt;// Convert.ToString(decimal.Round(Convert.ToDecimal(sLossWt), 3, MidpointRounding.AwayFromZero));
            else
                txtLossWeight.Text = sLossWt;

            if (!string.IsNullOrEmpty(sPurity)) // -- 
                txtPurity.Text = sPurity;// Convert.ToString(decimal.Round(Convert.ToDecimal(sPurity), 4, MidpointRounding.AwayFromZero));
            else
                txtPurity.Text = sPurity;

            if (!string.IsNullOrEmpty(sEQty))
                txtExpectedQuantity.Text = sEQty;// Convert.ToString(decimal.Round(Convert.ToDecimal(sEQty), 3, MidpointRounding.AwayFromZero));
            else
                txtExpectedQuantity.Text = sEQty;

            if (!string.IsNullOrEmpty(sActTotAmt))
                txtActToAmount.Text = sActTotAmt;// Convert.ToString(decimal.Round(Convert.ToDecimal(sActTotAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtActToAmount.Text = sActTotAmt;

            if (!string.IsNullOrEmpty(sActMakingRate))
                txtActMakingRate.Text = sActMakingRate;// Convert.ToString(decimal.Round(Convert.ToDecimal(sActMakingRate), 2, MidpointRounding.AwayFromZero));
            else
                txtActMakingRate.Text = sActMakingRate;

            if (!string.IsNullOrEmpty(sLineDisc))
                txtLineDisc.Text = sLineDisc;//Convert.ToString(decimal.Round(Convert.ToDecimal(sLineDisc), 2, MidpointRounding.AwayFromZero));
            else
                txtLineDisc.Text = sLineDisc;

            if (!string.IsNullOrEmpty(sTotMkDicAmt))
                txtMakingDiscTotAmt.Text = sTotMkDicAmt;// Convert.ToString(decimal.Round(Convert.ToDecimal(sTotMkAmt), 2, MidpointRounding.AwayFromZero));
            else
                txtMakingDiscTotAmt.Text = "0";

            lblMetalRatesShow.Text = sRateShow;

            if (Convert.ToInt16(sTType) == (int)TransactionType.Sale)
            {
                rdbSale.Checked = true;
                chkOwn.Visible = false;
                lblBatchNo.Visible = false;
                txtBatchNo.Visible = false;
                if (dsIng != null && dsIng.Tables.Count > 0 && dsIng.Tables[0].Rows.Count > 0)
                {
                    dtIngredientsClone = dsIng.Tables[0];
                    btnIngrdientsDetails.Visible = true;
                }
                else
                {
                    dtIngredientsClone = new DataTable();
                    btnIngrdientsDetails.Visible = false;
                }

                dtIngredients = dtIngredientsClone;

                cmbCustomerOrder.Enabled = false;
                alist = new ArrayList();
                if (!string.IsNullOrEmpty(ordernum))
                {
                    alist.Add(ordernum);
                    cmbCustomerOrder.DataSource = alist;
                }
                else
                {
                    alist.Add("NO CUSTOMER ORDER");
                    cmbCustomerOrder.DataSource = alist;
                    btnSelectItems.Visible = false;
                }

                if (string.IsNullOrEmpty(orderlinenum) || orderlinenum == "0")
                {
                    lblItemSelected.Text = "NO ITEM SELECTED";
                    btnSelectItems.Enabled = false; ;
                }
                else if (!string.IsNullOrEmpty(ordernum))
                {
                    lblItemSelected.Text = "ORDER :" + ordernum + " SELECTED LINE NO. : " + orderlinenum;
                    btnSelectItems.Visible = false;
                    btnSelectItems.Enabled = false;
                    LineNum = orderlinenum;

                }

                if (!string.IsNullOrEmpty(sWastageType))
                    cmbWastage.SelectedIndex = Convert.ToInt32(sWastageType);
                else
                    cmbWastage.SelectedIndex = 0;

                if (!string.IsNullOrEmpty(sWastageQty))
                    txtWastageQty.Text = sWastageQty;
                else
                    txtWastageQty.Text = "0";

                if (!string.IsNullOrEmpty(sWastageAmount))
                    txtWastageAmount.Text = sWastageAmount;// Convert.ToString(decimal.Round(Convert.ToDecimal(sWastageAmount), 2, MidpointRounding.AwayFromZero));
                else
                    txtWastageAmount.Text = "0";

                if (!string.IsNullOrEmpty(sWastagePercentage) && (cmbWastage.SelectedIndex == 1))
                    txtWastagePercentage.Text = sWastagePercentage;// Convert.ToString(decimal.Round(Convert.ToDecimal(sWastagePercentage), 2, MidpointRounding.AwayFromZero));
                else
                    txtWastagePercentage.Text = string.Empty;

                // Making Discount Type
                if (!string.IsNullOrEmpty(sMkDiscType))
                    cmbMakingDiscType.SelectedIndex = Convert.ToInt32(sMkDiscType);
                else
                    cmbMakingDiscType.SelectedIndex = 0;



                //if(!string.IsNullOrEmpty(sTotMkDicAmt))
                //{
                //    if(!string.IsNullOrEmpty(txtMakingAmount.Text))
                //    {
                //        txtMakingAmount.Text = Convert.ToString(Convert.ToDecimal(txtMakingAmount.Text) - Convert.ToDecimal(sTotMkDicAmt));// Convert.ToString(decimal.Round(Convert.ToDecimal(sTotMkAmt), 2, MidpointRounding.AwayFromZero));
                //        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text) - Convert.ToDecimal(sTotMkDicAmt));
                //    }
                //}
                //else
                //    txtMakingDiscTotAmt.Text = "0";

                //if(Convert.ToDecimal(sMDisc) > 0 && Convert.ToDecimal(sTotMkDicAmt) > 0 && Convert.ToDecimal(sMDisc) == Convert.ToDecimal(sTotMkDicAmt))
                //{
                //    txtMakingAmount.Text = "0"; // Convert.ToString(decimal.Round(Convert.ToDecimal(sTotMkAmt), 2, MidpointRounding.AwayFromZero));
                //    txtTotalAmount.Text = txtAmount.Text;
                //}
                //else r
                //{
                //    txtActToAmount.Text = txtTotalAmount.Text;
                //}

                if (saleLineItem.LinePctDiscount > 0)
                {
                    txtLineDisc.Text = "0";
                    txtLineDisc.Text = Convert.ToString(saleLineItem.LinePctDiscount);
                    txtLineDisc.Focus();
                }
                //txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtGoldTax.Text.Trim()));
                //
            }
            else
            {

                if (Convert.ToInt16(sTType) == (int)TransactionType.Purchase)
                    rdbPurchase.Checked = true;
                else if (Convert.ToInt16(sTType) == (int)TransactionType.Exchange)
                    rdbExchange.Checked = true;
                else if (Convert.ToInt16(sTType) == (int)TransactionType.ExchangeReturn)
                {
                    rdbExchangeReturn.Checked = true;
                    lblBatchNo.Visible = true;
                    txtBatchNo.Visible = true;
                    txtBatchNo.Enabled = false;
                }
                else if (Convert.ToInt16(sTType) == (int)TransactionType.PurchaseReturn)
                {
                    lblBatchNo.Visible = true;
                    txtBatchNo.Visible = true;
                    txtBatchNo.Enabled = false;
                    rdbPurchReturn.Checked = true;
                }
            }

            txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text));// + Convert.ToDecimal(txtGoldTax.Text)

            if (isServiceItem == 1)
                txtChangedTotAmount.Enabled = false;
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

        #region Bind Making Discount Combo

        void BindMakingDiscount()
        {
            cmbMakingDiscType.DataSource = Enum.GetValues(typeof(MakingDiscType));
        }

        #endregion


        #region Btn Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCancelClick = true;
            dPrevRunningTransAdjGoldQty = 0;
            LockSKU(sBaseItemID, 0);
            this.Close();
        }
        #endregion

        #region Submit Button Click
        private void btnAdd_Click(object sender, EventArgs e)
        {
            txtMakingRate_Leave(sender, e);//added on140518
            if (ValidateControls())
            {
                if (retailTrans != null)
                {
                    if ((string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                        && (string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.LCCustomerName))))
                    {
                        MessageBox.Show("Customer not selected");
                    }
                    else
                        sCustomerId = retailTrans.Customer.CustomerId;

                    if (isViewing)
                    {
                        decimal dGTaxValue = 0;// Convert.ToDecimal(_saleLineItem.PartnerData.LINEGOLDTAX);
                        decimal dMkAmt = 0;
                        if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                            dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);

                        _saleLineItem.PriceOverridden = true;
                        _saleLineItem.NetAmount = Convert.ToDecimal(txtTotalAmount.Text);//+ dGTaxValue
                        _saleLineItem.GrossAmount = Convert.ToDecimal(txtTotalAmount.Text);//+ dGTaxValue
                        _saleLineItem.PartnerData.MakingRate = Convert.ToString(txtChangedMakingRate.Text);
                        _saleLineItem.PartnerData.WTDIFFDISCQTY = Convert.ToString(txtWtDiffDiscQty.Text);
                        _saleLineItem.PartnerData.WTDIFFDISCAMT = Convert.ToString(txtWtDiffDiscAmt.Text);


                        _saleLineItem.PartnerData.MakingTotalDiscount = Convert.ToString(txtMakingDiscTotAmt.Text);
                        _saleLineItem.PartnerData.TotalAmount = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text));// + dGTaxValue
                        _saleLineItem.PartnerData.MakingAmount = Convert.ToString(dMkAmt + dGTaxValue);// + dGTaxValue

                        _saleLineItem.Price = _saleLineItem.NetAmount / _saleLineItem.Quantity;

                        #region added for after sales details amt change or after disc
                        /*  decimal dLineDiscAmt = 0m;
                        decimal dItemLastTaxAmt = 0m;
                        decimal dNewTaxPct = 0m;
                        decimal dTaxAmtOfDiscAmt = 0m;

                        if (retailTrans != null)
                        {
                            if (!string.IsNullOrEmpty(txtLineDisc.Text))
                            {
                                _saleLineItem.PartnerData.LINEDISC = Convert.ToString(txtLineDisc.Text);
                            }

                            if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                            {
                                dLineDiscAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                            }
                        }


                        dItemLastTaxAmt = _saleLineItem.TaxAmount;
                        decimal dItemTaxPerc = getItemTaxPercentage(_saleLineItem.ItemId);

                        if (dLineDiscAmt > 0 && dItemTaxPerc > 0)
                        {
                            dTaxAmtOfDiscAmt = (dLineDiscAmt * dItemTaxPerc) / (100 + dItemTaxPerc);
                        }

                        if (dItemLastTaxAmt > 0 && dTaxAmtOfDiscAmt > 0)
                        {
                            dNewTaxPct = ((dItemLastTaxAmt - dTaxAmtOfDiscAmt) / (_saleLineItem.NetAmount - (dItemLastTaxAmt - dTaxAmtOfDiscAmt))) * 100;
                        }*/
                        #endregion

                        #region Recalculate Tax

                        _saleLineItem.TaxRatePct = _saleLineItem.PartnerData.NewTaxPerc;// dNewTaxPct;
                        _saleLineItem.TaxLines.Clear();
                        _saleLineItem.CalculateLine();


                        Tax objTax = new Tax();
                        if (application != null)
                            objTax.Application = application;
                        else
                            objTax.Application = Application;

                        objTax.Initialize();
                        objTax.CalculateTax(_saleLineItem, retailTrans);
                        #endregion
                    }
                }

                if (rdbSale.Checked)
                {
                    if (string.IsNullOrEmpty(txtMakingDisc.Text))
                    {
                        txtMakingDisc.Text = "0";
                    }
                    BuildSaleList();

                    purchaseList = new List<KeyValuePair<string, string>>();

                    if (IsGSSTransaction && !isViewing)
                    {
                        retailTrans.PartnerData.RunningQtyGSS = dPrevRunningQtyGSS;
                        retailTrans.PartnerData.GSSSaleWt = dActualGSSSaleWt;
                    }

                    if (!isMRPUCP)// added on 15/05/17 after asking Mr.S.Sharma
                    {
                        if (IsSaleAdjustment && !isViewing)
                        {
                            retailTrans.PartnerData.RunningQtyAdjustment = dPrevRunningTransAdjGoldQty;
                            retailTrans.PartnerData.TransAdjGoldQty = dActualTransAdjGoldQty;
                        }
                    }
                }
                else
                {

                    BuildPurchaseList();
                    saleList = new List<KeyValuePair<string, string>>();
                }
                this.Close();
            }
            else
            {
                if (!string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftCWQTY))
                {
                    retailTrans.PartnerData.FreeGiftQTY = "";
                    retailTrans.PartnerData.FreeGiftCWQTY = "";
                    this.Close();
                }
            }


            decimal dMkAmt1 = 0m;
            if (!String.IsNullOrEmpty(txtMakingAmount.Text))
                dMkAmt1 = Convert.ToDecimal(txtMakingAmount.Text);

            string sTblName = "MAKINGINFO" + ApplicationSettings.Terminal.TerminalId;

            if (retailTrans != null
               && retailTrans.SaleItems != null
               && retailTrans.SaleItems.Count > 0)
            {
                UpdateMakingInfo(sTblName, retailTrans.TransactionId, dMkAmt1);
            }
            else
            {
                InsertMakingInfo(sTblName, retailTrans.TransactionId, dMkAmt1);
            }
        }
        #endregion

        #region Key Press Events
        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnAdd_Click(sender, e);
            }
        }
        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtQuantity.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
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

                if (Regex.IsMatch(txtQuantity.Text, @"\.\d\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtRate.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtMakingRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtChangedMakingRate.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    //btnAdd_Click(sender, e);
                }
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnAdd_Click(sender, e);
            }
        }

        private void txtMakingDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtMakingDisc.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtMakingAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtMakingAmount.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void txtTotalAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnAdd_Click(sender, e);
            }
        }

        private void txtTotalWeight_KeyPress(object sender, KeyPressEventArgs e)
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

            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnAdd_Click(sender, e);
            }
        }

        private void txtPurity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
                btnAdd_Click(sender, e);
            }

        }

        private void txtLossPct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtLossPct.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                    btnAdd_Click(sender, e);
                }
            }
        }
        #endregion

        #region Pruchase List
        private void BuildPurchaseList()
        {
            purchaseList = new List<KeyValuePair<string, string>>();
            if (rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Pieces", !string.IsNullOrEmpty(txtPCS.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtPCS.Text.Trim())) : txtPCS.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("Pieces", txtPCS.Text.Trim()));

            if (rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Quantity", !string.IsNullOrEmpty(txtQuantity.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtQuantity.Text.Trim())) : txtQuantity.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("Quantity", txtQuantity.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("Rate", txtRate.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("RateType", Convert.ToString(cmbRateType.SelectedIndex)));
            purchaseList.Add(new KeyValuePair<string, string>("MakingRate", txtChangedMakingRate.Text.Trim()));

            int makingtype = 0;
            switch (cmbMakingType.SelectedIndex)
            {

                case 0:
                    makingtype = (int)MakingType.Pieces;
                    break;
                case 1:
                    makingtype = (int)MakingType.Weight;
                    break;
                case 2:
                    makingtype = (int)MakingType.Tot;
                    break;
                case 3:
                    makingtype = (int)MakingType.Percentage;
                    break;
                case 4:
                    makingtype = (int)MakingType.Gross;
                    break;
            }
            decimal dGTaxV = 0m;
            decimal dTotAmt = 0m;
            decimal dMkAmt = 0m;
            //if (!string.IsNullOrEmpty(txtGoldTax.Text))
            //    dGTaxV = Convert.ToDecimal(txtGoldTax.Text);

            if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);

            if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);

            purchaseList.Add(new KeyValuePair<string, string>("MakingType", Convert.ToString(makingtype)));


            // purchaseList.Add(new KeyValuePair<string, string>("TotalAmount", Convert.ToString(dTotAmt)));// + dGTaxV

            //purchaseList.Add(new KeyValuePair<string, string>("RateType", Convert.ToString(cmbRateType.SelectedIndex)));
            //purchaseList.Add(new KeyValuePair<string, string>("MakingRate", "0"));
            //purchaseList.Add(new KeyValuePair<string, string>("MakingType", "0"));

            if (rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("Amount", !string.IsNullOrEmpty(txtAmount.Text.Trim()) ? Convert.ToString((-1) * Convert.ToDecimal(txtAmount.Text.Trim())) : txtAmount.Text.Trim()));
            else
                purchaseList.Add(new KeyValuePair<string, string>("Amount", txtAmount.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("MakingDiscount", txtMakingDisc.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("MakingAmount", Convert.ToString(dMkAmt + dGTaxV)));// 
            //purchaseList.Add(new KeyValuePair<string, string>("MakingDiscount", "0"));
            //purchaseList.Add(new KeyValuePair<string, string>("MakingAmount", "0"));

            if (rdbPurchase.Checked || rdbExchange.Checked)
                purchaseList.Add(new KeyValuePair<string, string>("TotalAmount", !string.IsNullOrEmpty(Convert.ToString(dTotAmt)) ? Convert.ToString((-1) * dTotAmt) : Convert.ToString(dTotAmt)));
            else
                purchaseList.Add(new KeyValuePair<string, string>("TotalAmount", Convert.ToString(dTotAmt)));

            purchaseList.Add(new KeyValuePair<string, string>("TotalWeight", txtTotalWeight.Text.Trim()));
            // purchaseList.Add(new KeyValuePair<string, string>("Purity", txtPurity.Text.Trim())); // 

            purchaseList.Add(new KeyValuePair<string, string>("LossPct", txtLossPct.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("LossWeight", txtLossWeight.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("ExpectedQuantity", txtExpectedQuantity.Text.Trim()));
            if (iOWNDMD == 1 || iOWNOG == 1)
                purchaseList.Add(new KeyValuePair<string, string>("OwnCheckBox", Convert.ToString(true)));
            else
                purchaseList.Add(new KeyValuePair<string, string>("OwnCheckBox", Convert.ToString(false)));

            purchaseList.Add(new KeyValuePair<string, string>("OrderNum", string.Empty));
            purchaseList.Add(new KeyValuePair<string, string>("OrderLineNum", string.Empty));
            purchaseList.Add(new KeyValuePair<string, string>("SampleReturn", string.Empty));
            // Added for wastage 
            purchaseList.Add(new KeyValuePair<string, string>("WastageType", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastageQty", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastageAmount", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastagePercentage", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("WastageRate", "0"));
            //
            // Making Discount Type  
            purchaseList.Add(new KeyValuePair<string, string>("MakingDiscountType", "0"));
            purchaseList.Add(new KeyValuePair<string, string>("MakingTotalDiscount", "0"));
            //
            purchaseList.Add(new KeyValuePair<string, string>("ConfigId", sBaseConfigID));
            purchaseList.Add(new KeyValuePair<string, string>("Purity", string.IsNullOrEmpty(txtPurity.Text.Trim()) ? "0" : txtPurity.Text.Trim()));

            //  if(iOWNDMD == 1 || iOWNOG == 1)
            //    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
            //else if(iOTHERDMD == 1 || iOTHEROG == 1)
            //    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");

            if ((dtExtndPurchExchng.Rows.Count > 0)) //(iOWNDMD == 1 || iOWNOG == 1) &&
            {
                purchaseList.Add(new KeyValuePair<string, string>("GROSSWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("GROSSUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDPCS", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDPCS"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("DMDAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEPCS", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEPCS"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("STONEAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETRATE", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETRATE"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETUNIT"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETPURITY", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETPURITY"])));
                purchaseList.Add(new KeyValuePair<string, string>("NETAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["NETAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("REFINVOICENO", Convert.ToString(dtExtndPurchExchng.Rows[0]["REFINVOICENO"])));

                purchaseList.Add(new KeyValuePair<string, string>("FLAT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("PROMOCODE", ""));

            }
            else
            {
                purchaseList.Add(new KeyValuePair<string, string>("GROSSWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("GROSSUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDPCS", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("DMDAMOUNT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEPCS", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("STONEAMOUNT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETRATE", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETUNIT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETPURITY", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("NETAMOUNT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("REFINVOICENO", string.Empty));

                purchaseList.Add(new KeyValuePair<string, string>("FLAT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("PROMOCODE", string.Empty));


            }
            purchaseList.Add(new KeyValuePair<string, string>("RETAILBATCHNO", string.IsNullOrEmpty(txtBatchNo.Text.Trim()) ? " " : txtBatchNo.Text.Trim()));

            purchaseList.Add(new KeyValuePair<string, string>("ACTMKRATE", string.IsNullOrEmpty(txtActMakingRate.Text.Trim()) ? "0" : txtActMakingRate.Text.Trim()));//txtActMakingRate.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("ACTTOTAMT", string.IsNullOrEmpty(txtActToAmount.Text.Trim()) ? "0" : txtActToAmount.Text.Trim()));//txtActToAmount.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("CHANGEDTOTAMT", string.IsNullOrEmpty(txtChangedTotAmount.Text.Trim()) ? "0" : txtChangedTotAmount.Text.Trim()));// txtChangedTotAmount.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("LINEDISC", string.IsNullOrEmpty(txtLineDisc.Text.Trim()) ? "0" : txtLineDisc.Text.Trim()));// txtLineDisc.Text.Trim()));


            //For OG changes 
            if ((dtExtndPurchExchng.Rows.Count > 0))
            {
                purchaseList.Add(new KeyValuePair<string, string>("OGREFBATCHNO", Convert.ToString(dtExtndPurchExchng.Rows[0]["REFBATCHNO"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGCHANGEDGROSSWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["CHANGEDGROSSWT"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGDMDRATE", Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDRATE"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGSTONEATE", Convert.ToString(dtExtndPurchExchng.Rows[0]["STONERATE"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGGROSSAMT", Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSAMT"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGACTAMT", Convert.ToString(dtExtndPurchExchng.Rows[0]["ACTAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGCHANGEDAMT", Convert.ToString(dtExtndPurchExchng.Rows[0]["CHANGEDAMOUNT"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGFINALAMT", Convert.ToString(dtExtndPurchExchng.Rows[0]["FINALAMT"])));
            }
            else
            {
                purchaseList.Add(new KeyValuePair<string, string>("OGREFBATCHNO", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGCHANGEDGROSSWT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGDMDRATE", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGSTONEATE", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGGROSSAMT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGACTAMT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGCHANGEDAMT", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGFINALAMT", "0"));
            }

            purchaseList.Add(new KeyValuePair<string, string>("FGPROMOCODE", string.Empty));

            if ((dtExtndPurchExchng.Rows.Count > 0))
            {
                purchaseList.Add(new KeyValuePair<string, string>("ISREPAIR", Convert.ToString(dtExtndPurchExchng.Rows[0]["ISREPAIR"])));
                purchaseList.Add(new KeyValuePair<string, string>("REPAIRBATCHID", Convert.ToString(dtExtndPurchExchng.Rows[0]["REPAIRBATCHID"])));
            }
            else
            {
                purchaseList.Add(new KeyValuePair<string, string>("ISREPAIR", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("REPAIRBATCHID", string.Empty));
            }
            purchaseList.Add(new KeyValuePair<string, string>("REPAIRINVENTBATCHID", string.Empty));
            purchaseList.Add(new KeyValuePair<string, string>("RATESHOW", string.IsNullOrEmpty(lblMetalRatesShow.Text.Trim()) ? "0" : lblMetalRatesShow.Text.Trim()));
            purchaseList.Add(new KeyValuePair<string, string>("TRANSFERCOSTPRICE", "0"));

            if ((dtExtndPurchExchng.Rows.Count > 0))
            {
                purchaseList.Add(new KeyValuePair<string, string>("OGLINENUM", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGLINENUM"])));
                purchaseList.Add(new KeyValuePair<string, string>("OGRECEIPTNO", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGRECEIPTNO"])));
            }
            else
            {
                purchaseList.Add(new KeyValuePair<string, string>("OGLINENUM", "0"));
                purchaseList.Add(new KeyValuePair<string, string>("OGRECEIPTNO", string.Empty));
            }
            purchaseList.Add(new KeyValuePair<string, string>("BULKINVENTBATCHID", string.Empty));
            purchaseList.Add(new KeyValuePair<string, string>("LINEGOLDVALUE", txtgval.Text));
            purchaseList.Add(new KeyValuePair<string, string>("LINEGOLDTAX", txtGoldTax.Text));
            purchaseList.Add(new KeyValuePair<string, string>("WTDIFFDISCQTY", txtWtDiffDiscQty.Text));

            //added on 070119
            purchaseList.Add(new KeyValuePair<string, string>("PURITYREADING1", Convert.ToString(dPurityReading1)));
            purchaseList.Add(new KeyValuePair<string, string>("PURITYREADING2", Convert.ToString(dPurityReading2)));
            purchaseList.Add(new KeyValuePair<string, string>("PURITYREADING3", Convert.ToString(dPurityReading3)));
            purchaseList.Add(new KeyValuePair<string, string>("PURITYPERSON", Convert.ToString(sPurityPerson)));
            purchaseList.Add(new KeyValuePair<string, string>("PURITYPERSONNAME", Convert.ToString(sPurityPersonName)));
            purchaseList.Add(new KeyValuePair<string, string>("LINEOMVALUE", txtOMValue.Text));
            purchaseList.Add(new KeyValuePair<string, string>("WTDIFFDISCAMT", txtWtDiffDiscAmt.Text));

            purchaseList.Add(new KeyValuePair<string, string>("OGPSTONEWT", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEWT"])));
            purchaseList.Add(new KeyValuePair<string, string>("OGPSTONEPCS", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEPCS"])));
            purchaseList.Add(new KeyValuePair<string, string>("OGPSTONEUNIT", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEUNIT"])));
            purchaseList.Add(new KeyValuePair<string, string>("OGPSTONEATE", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONERATE"])));
            purchaseList.Add(new KeyValuePair<string, string>("OGPSTONEAMOUNT", Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEAMOUNT"])));
        }
        #endregion

        #region SaleList
        private void BuildSaleList()
        {
            saleList = new List<KeyValuePair<string, string>>();

            saleList.Add(new KeyValuePair<string, string>("Pieces", txtPCS.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("Quantity", txtQuantity.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("Rate", txtRate.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("RateType", Convert.ToString(cmbRateType.SelectedIndex)));
            saleList.Add(new KeyValuePair<string, string>("MakingRate", txtChangedMakingRate.Text.Trim()));
            int makingtype = 0;
            switch (cmbMakingType.SelectedIndex)
            {

                case 0:
                    makingtype = (int)MakingType.Pieces;
                    break;
                case 1:
                    makingtype = (int)MakingType.Weight;
                    break;
                case 2:
                    makingtype = (int)MakingType.Tot;
                    break;
                case 3:
                    makingtype = (int)MakingType.Percentage;
                    break;
                case 4:
                    makingtype = (int)MakingType.Gross;
                    break;
            }
            decimal dGTaxV = 0m;
            decimal dTotAmt = 0m;
            decimal dMkAmt = 0m;
            //if (!string.IsNullOrEmpty(txtGoldTax.Text))
            //    dGTaxV = Convert.ToDecimal(txtGoldTax.Text);

            if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);

            if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);

            saleList.Add(new KeyValuePair<string, string>("MakingType", Convert.ToString(makingtype)));
            saleList.Add(new KeyValuePair<string, string>("Amount", txtAmount.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("MakingDiscount", txtMakingDisc.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("MakingAmount", Convert.ToString(dMkAmt + dGTaxV)));// 
            saleList.Add(new KeyValuePair<string, string>("TotalAmount", Convert.ToString(dTotAmt)));// + dGTaxV
            saleList.Add(new KeyValuePair<string, string>("TotalWeight", "0"));
            saleList.Add(new KeyValuePair<string, string>("LossPct", "0"));
            saleList.Add(new KeyValuePair<string, string>("LossWeight", "0"));
            saleList.Add(new KeyValuePair<string, string>("ExpectedQuantity", "0"));
            saleList.Add(new KeyValuePair<string, string>("OwnCheckBox", Convert.ToString(false)));
            saleList.Add(new KeyValuePair<string, string>("OrderNum", Convert.ToString(lblCustOrder.Text)));
            //saleList.Add(new KeyValuePair<string, string>("OrderNum", (Convert.ToString(cmbCustomerOrder.SelectedValue).ToUpper().Trim() == "NO CUSTOMER ORDER"
            //                                            || (Convert.ToString(cmbCustomerOrder.SelectedValue).ToUpper().Trim() == "NO SELECTION")) ? string.Empty : Convert.ToString(cmbCustomerOrder.SelectedValue)));

            saleList.Add(new KeyValuePair<string, string>("OrderLineNum", Convert.ToString(OlineNum)));

            saleList.Add(new KeyValuePair<string, string>("SampleReturn", Convert.ToString(chkSampleReturn.Checked)));

            // Added for wastage 
            saleList.Add(new KeyValuePair<string, string>("WastageType", Convert.ToString(cmbWastage.SelectedIndex)));

            if (!string.IsNullOrEmpty(txtWastageQty.Text))
                saleList.Add(new KeyValuePair<string, string>("WastageQty", Convert.ToString(txtWastageQty.Text)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastageQty", "0"));

            if (!string.IsNullOrEmpty(txtWastageAmount.Text))
                saleList.Add(new KeyValuePair<string, string>("WastageAmount", Convert.ToString(txtWastageAmount.Text)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastageAmount", "0"));
            //--------
            if (!string.IsNullOrEmpty(txtWastagePercentage.Text))
                saleList.Add(new KeyValuePair<string, string>("WastagePercentage", Convert.ToString(txtWastagePercentage.Text)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastagePercentage", "0"));
            //-------------

            //--------
            // if (!string.IsNullOrEmpty(txtRate.Text)) 
            if (dWMetalRate > 0)
                saleList.Add(new KeyValuePair<string, string>("WastageRate", Convert.ToString(dWMetalRate)));
            else
                saleList.Add(new KeyValuePair<string, string>("WastageRate", "0"));
            //-------------

            //--- end

            // Making Discount Type 

            saleList.Add(new KeyValuePair<string, string>("MakingDiscountType", Convert.ToString(cmbMakingDiscType.SelectedIndex)));
            if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                saleList.Add(new KeyValuePair<string, string>("MakingTotalDiscount", txtMakingDiscTotAmt.Text.Trim()));
            else
                saleList.Add(new KeyValuePair<string, string>("MakingTotalDiscount", "0"));
            //

            saleList.Add(new KeyValuePair<string, string>("ConfigId", sBaseConfigID));

            saleList.Add(new KeyValuePair<string, string>("Purity", "0"));

            saleList.Add(new KeyValuePair<string, string>("GROSSWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("GROSSUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDPCS", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("DMDAMOUNT", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEPCS", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("STONEAMOUNT", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETRATE", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETUNIT", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETPURITY", "0"));
            saleList.Add(new KeyValuePair<string, string>("NETAMOUNT", "0"));
            saleList.Add(new KeyValuePair<string, string>("REFINVOICENO", string.Empty));

            saleList.Add(new KeyValuePair<string, string>("FLAT", Convert.ToString(iIsMkDiscFlat)));
            saleList.Add(new KeyValuePair<string, string>("PROMOCODE", Convert.ToString(sMkPromoCode)));
            saleList.Add(new KeyValuePair<string, string>("RETAILBATCHNO", string.Empty));

            saleList.Add(new KeyValuePair<string, string>("ACTMKRATE", string.IsNullOrEmpty(txtActMakingRate.Text.Trim()) ? "0" : txtActMakingRate.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("ACTTOTAMT", string.IsNullOrEmpty(txtActToAmount.Text.Trim()) ? "0" : txtActToAmount.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("CHANGEDTOTAMT", string.IsNullOrEmpty(txtChangedTotAmount.Text.Trim()) ? "0" : txtChangedTotAmount.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("LINEDISC", string.IsNullOrEmpty(txtLineDisc.Text.Trim()) ? "0" : txtLineDisc.Text.Trim()));

            //For OG changes 
            saleList.Add(new KeyValuePair<string, string>("OGREFBATCHNO", string.Empty));
            saleList.Add(new KeyValuePair<string, string>("OGCHANGEDGROSSWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGDMDRATE", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGSTONEATE", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGGROSSAMT", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGACTAMT", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGCHANGEDAMT", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGFINALAMT", "0"));

            saleList.Add(new KeyValuePair<string, string>("FGPROMOCODE", sFGPromoCode));
            saleList.Add(new KeyValuePair<string, string>("ISREPAIR", "0"));
            saleList.Add(new KeyValuePair<string, string>("REPAIRBATCHID", string.Empty));
            saleList.Add(new KeyValuePair<string, string>("REPAIRINVENTBATCHID", sOGBatchIdForRepirRet));
            saleList.Add(new KeyValuePair<string, string>("RATESHOW", string.IsNullOrEmpty(lblMetalRatesShow.Text.Trim()) ? "0" : lblMetalRatesShow.Text.Trim()));
            saleList.Add(new KeyValuePair<string, string>("TRANSFERCOSTPRICE", Convert.ToString(dTransferCost)));
            saleList.Add(new KeyValuePair<string, string>("OGLINENUM", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGRECEIPTNO", ""));
            saleList.Add(new KeyValuePair<string, string>("BULKINVENTBATCHID", BatchID));
            saleList.Add(new KeyValuePair<string, string>("LINEGOLDVALUE", txtgval.Text));
            saleList.Add(new KeyValuePair<string, string>("LINEGOLDTAX", txtGoldTax.Text));
            saleList.Add(new KeyValuePair<string, string>("WTDIFFDISCQTY", txtWtDiffDiscQty.Text));
            //added on 07/01/19
            saleList.Add(new KeyValuePair<string, string>("PURITYREADING1", "0"));
            saleList.Add(new KeyValuePair<string, string>("PURITYREADING2", "0"));
            saleList.Add(new KeyValuePair<string, string>("PURITYREADING3", "0"));
            saleList.Add(new KeyValuePair<string, string>("PURITYPERSON", Convert.ToString(sPurityPerson)));
            saleList.Add(new KeyValuePair<string, string>("PURITYPERSONNAME", Convert.ToString(sPurityPersonName)));
            saleList.Add(new KeyValuePair<string, string>("LINEOMVALUE", txtOMValue.Text));//other metal sum like silver and platinum
            saleList.Add(new KeyValuePair<string, string>("WTDIFFDISCAMT", txtWtDiffDiscAmt.Text));

            saleList.Add(new KeyValuePair<string, string>("OGPSTONEWT", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGPSTONEPCS", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGPSTONEUNIT", ""));
            saleList.Add(new KeyValuePair<string, string>("OGPSTONEATE", "0"));
            saleList.Add(new KeyValuePair<string, string>("OGPSTONEAMOUNT", "0"));
        }
        #endregion

        #region Text Changed
        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            if (txtRate.Enabled == false)
            {
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                //txtActToAmount.Text = txtTotalAmount.Text;
            }
        }

        private void txtMakingRate_TextChanged(object sender, EventArgs e)
        {

            //cmbRateType_SelectedIndexChanged(sender, e);
            //cmbMakingType_SelectedIndexChanged(sender, e);
            //txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? txtTotalAmount.Text : string.Empty;
            //txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? txtMakingAmount.Text : string.Empty;

        }

        private void txtMakingDisc_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtMakingDiscTotAmt_TextChanged(object sender, EventArgs e) // Making Discount Type
        {
            //cmbRateType_SelectedIndexChanged(sender, e);
            //cmbMakingType_SelectedIndexChanged(sender, e);
        }
        private void txtWastageAmount_TextChanged(object sender, EventArgs e)  //wastage
        {
            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
        }

        private void txtTotalWeight_TextChanged(object sender, EventArgs e)
        {


            if (!isViewing)  // BLOCKED ON 19.11.2013
            {
                //txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                //txtLossPct_TextChanged(sender, e);
            }
        }

        private void txtTotalWeight_Leave(object sender, EventArgs e)
        {
            //if (!isViewing)
            if ((!isViewing) && (!rdbSale.Checked))
            {
                //if(iOWNDMD == 1 || iOWNOG == 1)
                //    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
                //else if(iOTHERDMD == 1 || iOTHEROG == 1)
                //    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");

                if (iOWNDMD == 1 || iOWNOG == 1)
                {
                    txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                    txtLossPct_TextChanged(sender, e);
                }
                else
                {
                    GetPurcExchngGoldQty();
                    txtLossPct_TextChanged(sender, e);
                }
            }

        }

        private void txtPurity_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPurity.Text.Trim()))
            {
                if (Convert.ToDecimal(txtPurity.Text.Trim()) > 1)
                {
                    MessageBox.Show("Purity value cannot greter than 1");
                    txtPurity.Text = string.Empty;
                    txtPurity.Focus();
                }
                else
                {
                    txtPurity.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(txtPurity.Text.Trim()), 4, MidpointRounding.AwayFromZero));
                    // if (!isViewing)
                    if (!isViewing && (!rdbSale.Checked))
                    {
                        if (iOWNDMD == 1 || iOWNOG == 1)
                        {
                            txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                            txtPurity.Text = string.Empty;
                            txtLossPct_TextChanged(sender, e);
                        }
                        else
                        {
                            GetPurcExchngGoldQty();
                            txtLossPct_TextChanged(sender, e);
                        }
                    }
                }
            }

        }

        private void txtLossPct_TextChanged(object sender, EventArgs e)
        {
            //if (!isViewing && rdbSale.Checked)
            //{
            //    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()) && !string.IsNullOrEmpty(txtLossPct.Text.Trim()))
            //    {
            //        decimal CalPerLossWt = Convert.ToDecimal(txtTotalWeight.Text.Trim()) * Convert.ToDecimal(txtLossPct.Text.Trim()) / 100;//txtQuantity
            //        decimal CalLossWt = Convert.ToDecimal(txtTotalWeight.Text.Trim()) - Convert.ToDecimal(CalPerLossWt);//txtQuantity
            //        //txtExpectedQuantity.Text = Convert.ToString(CalLossWt);
            //        txtExpectedQuantity.Text = Convert.ToString(decimal.Round(CalLossWt, 3, MidpointRounding.AwayFromZero));
            //        txtQuantity.Text = Convert.ToString(decimal.Round(CalLossWt, 3, MidpointRounding.AwayFromZero));
            //        //txtLossWeight.Text = Convert.ToString(CalPerLossWt);
            //        txtLossWeight.Text = Convert.ToString(decimal.Round(CalPerLossWt, 3, MidpointRounding.AwayFromZero));

            //        cmbRateType_SelectedIndexChanged(sender, e);
            //    }
            //    if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) || string.IsNullOrEmpty(txtLossPct.Text.Trim()))
            //    {
            //        txtLossWeight.Text = string.Empty;
            //        txtExpectedQuantity.Text = string.Empty;
            //        cmbRateType_SelectedIndexChanged(sender, e);
            //    }
            //    txtLossWeight_Leave(sender, e);
            //    cmbRateType_SelectedIndexChanged(sender, e);
            //}
        }
        #endregion

        #region Selected Index Changed
        private void cmbRateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isViewing)
            {
                if (rdbSale.Checked)
                {
                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        Decimal decimalAmount = 0;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtQuantity.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                        txtAmount.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero));
                    }
                    if (cmbRateType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                        txtTotalAmount.Text = string.Empty;
                        txtActToAmount.Text = txtTotalAmount.Text;
                    }
                    if (cmbRateType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimalAmount);
                    }
                    if (cmbRateType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                        txtTotalAmount.Text = string.Empty;
                        //txtActToAmount.Text = txtTotalAmount.Text;

                    }
                    if (cmbRateType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimalAmount);
                    }

                    if (!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                    {
                        if (!string.IsNullOrEmpty(txtWtDiffDiscAmt.Text))
                            txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()) + Convert.ToDecimal(txtWtDiffDiscAmt.Text.Trim()));
                        else
                            txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));
                        //txtActToAmount.Text = txtTotalAmount.Text;
                    }


                    // Making Discount Type -- 29.04.2013
                    //if (!string.IsNullOrEmpty(txtMakingDisc.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text)
                    //     && !string.IsNullOrEmpty(txtAmount.Text))
                    //{
                    //    Decimal decimalAmount = 0m;
                    //    decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                    //    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    //}

                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text)
                        && !string.IsNullOrEmpty(txtAmount.Text))
                    {
                        Decimal decimalAmount = 0m;
                        //decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                        decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                        txtTotalAmount.Text = Convert.ToString(decimalAmount);
                        //txtActToAmount.Text = txtTotalAmount.Text;
                    }


                    //
                    if (string.IsNullOrEmpty(txtRate.Text.Trim()))
                    {
                        txtAmount.Text = string.Empty;
                        txtTotalAmount.Text = string.Empty;
                        //txtActToAmount.Text = txtTotalAmount.Text;
                    }

                    // Added  -- wastage
                    if ((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                    {
                        if ((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                        {
                            txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                            //txtActToAmount.Text = txtTotalAmount.Text;
                        }
                    }

                    //
                }
                else
                {

                    if (cmbRateType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtLossWeight.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtExpectedQuantity.Text.Trim());
                        //  txtAmount.Text = Convert.ToString(decimalAmount);
                        txtAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }
                    if (cmbRateType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtLossWeight.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                    }
                    if (cmbRateType.SelectedIndex == 1 && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                        //txtAmount.Text = Convert.ToString(decimalAmount);
                        txtAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }
                    if (cmbRateType.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                    {
                        txtAmount.Text = string.Empty;
                    }
                    if (cmbRateType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
                    {
                        Decimal decimalAmount = 0m;
                        decimalAmount = Convert.ToDecimal(txtRate.Text.Trim());
                        txtAmount.Text = Convert.ToString(decimalAmount);
                    }
                    if (string.IsNullOrEmpty(txtRate.Text.Trim()))
                    {
                        txtAmount.Text = string.Empty;
                    }
                }
            }
        }

        private void cmbMakingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalcMakingAtSelectedIndexChange();
        }

        //New

        private void CalcMakingAtSelectedIndexChange()
        {
            Decimal dMakingQty = 0;
            Decimal decimalAmount = 0m;
            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;
            decimal dTotAmt = 0m;
            decimal dActTotAmt = 0m;
            decimal dManualChangesAmt = 0m;
            decimal dMkDiscTotAmt = 0m;
            decimal dQty = 0m;
            decimal dWtDiffDisc = 0m;
            decimal dWtDiffDiscVal = 0m;
            decimal dActMkRate = 0m;

            decimal dChangeMkrate = 0m;


            if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                dActMkRate = Convert.ToDecimal(txtActMakingRate.Text);
            else
                dActMkRate = 0;

            if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                dChangeMkrate = Convert.ToDecimal(txtChangedMakingRate.Text);
            else
                dChangeMkrate = 0;


            //if (dActMkRate > 0 && dChangeMkrate > 0)
            //{
            //    if (dChangeMkrate > dActMkRate)
            //        dActMkRate = dChangeMkrate;
            //}

            if (!string.IsNullOrEmpty(txtWtDiffDiscQty.Text) && txtWtDiffDiscQty.Text != ".")
                dWtDiffDisc = Convert.ToDecimal(txtWtDiffDiscQty.Text);
            else
                dWtDiffDisc = 0;

            if (dActMkRate < dChangeMkrate)
            {
                if (dWtDiffDisc > 0)
                {
                    if (cmbMakingType.SelectedIndex == 3)//(int)MakingType.Percentage
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text) * dChangeMkrate) / 100), 2, MidpointRounding.AwayFromZero);
                    else if (cmbMakingType.SelectedIndex == 0)//(int)MakingType.Pieces
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);
                    else if (cmbMakingType.SelectedIndex == 2)//(int)MakingType.Tot
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);
                    else if (cmbMakingType.SelectedIndex == 1)//(int)MakingType.Weight
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dChangeMkrate)), 2, MidpointRounding.AwayFromZero);
                    else if (cmbMakingType.SelectedIndex == 5)//(int)MakingType.Gross
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dChangeMkrate)), 2, MidpointRounding.AwayFromZero);
                }
            }
            else
            {

                if (dWtDiffDisc > 0)
                {
                    if (cmbMakingType.SelectedIndex == 3)//(int)MakingType.Percentage
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text) * dActMkRate) / 100), 2, MidpointRounding.AwayFromZero);//(dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text) * dActMkRate) / 100);
                    else if (cmbMakingType.SelectedIndex == 0)//(int)MakingType.Pieces
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);// (dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text));
                    else if (cmbMakingType.SelectedIndex == 2)//(int)MakingType.Tot
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);//(dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text));
                    else if (cmbMakingType.SelectedIndex == 1)//(int)MakingType.Weight
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate)), 2, MidpointRounding.AwayFromZero);//(dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate));
                    else if (cmbMakingType.SelectedIndex == 5)//(int)MakingType.Gross
                        dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate)), 2, MidpointRounding.AwayFromZero);// (dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate));
                }
            }

            if (dWtDiffDiscVal > 0)
                txtWtDiffDiscAmt.Text = Convert.ToString(decimal.Round(dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero));
            else
                txtWtDiffDiscAmt.Text = "";

            if (!isViewing)
            {
                #region !isView mode
                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                {
                    #region when mking type weight and gross
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;
                    if (cmbMakingType.SelectedIndex == 4)
                    {
                        dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                    }
                    else
                    {
                        if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                        {
                            decimal dQty1 = 0m;
                            string dNetWt = "";
                            if (bValidSKU)
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


                    decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) * dMakingQty;

                    //if(dMkRate<dMkRate1)
                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtQuantity.Text))
                        dQty = Convert.ToDecimal(txtQuantity.Text);
                    else
                        dQty = 0;

                    //DEV ON 20/0417 REQ S.SHARMA
                    decimal dAddMk = 0;
                    decimal dMinMkQty = 0m;

                    string sSqr1 = "select isnull(NOEXTRAMKADDITION,0) from INVENTTABLE where itemid='" + sBaseItemID + "'";
                    string sResultForNOEXTRAMKADDITION = NIM_ReturnExecuteScalar(sSqr1);

                    string sSqr2 = "select isnull(ADDITIONALMAKING,0) from RETAILPARAMETERS  where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                    string sAddMk = NIM_ReturnExecuteScalar(sSqr2);

                    string sSqr3 = "select isnull(MINMAKINGQTY,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                    string sMinMkQty = NIM_ReturnExecuteScalar(sSqr3);


                    dAddMk = Convert.ToDecimal(sAddMk);
                    dMinMkQty = Convert.ToDecimal(sMinMkQty);

                    if (sResultForNOEXTRAMKADDITION == "0")
                    {
                        if (!isBulkItem)
                        {
                            if (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                            {

                                if (Convert.ToDecimal(dMakingQty) <= dMinMkQty && Convert.ToDecimal(dMakingQty) > 0) //dMinMkQty==6(pre hard coded value) changes on 260418
                                {
                                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                                        dActMkRate = Convert.ToDecimal(txtActMakingRate.Text);
                                    if (!isMkAdded)
                                    {
                                        txtActMakingRate.Text = Convert.ToString(decimal.Round(dActMkRate + (dAddMk / Convert.ToDecimal(dMakingQty)), 2, MidpointRounding.AwayFromZero));
                                        isMkAdded = true;
                                    }
                                    //txtChangedMakingRate.Text = txtActMakingRate.Text;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;


                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;



                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am; - Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;

                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }

                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    if (dMkRate > dMkRate1)
                        txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate * dMakingQty) - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    else
                        txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));


                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;

                    if (dMkAmt > 0)
                    {
                        decimal dMkCh = 0m;
                        if (dManualChangesAmt > 0)
                        {
                            if (!string.IsNullOrEmpty(txtgval.Text))
                            {
                                dMkCh = ((dMkAmt * 100) / Convert.ToDecimal(txtgval.Text.Trim()));
                            }
                        }

                        if (dMkRate1 > dMkRate)
                        {
                            if (dMkDiscTotAmt == 0)
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                            else
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkCh, 2, MidpointRounding.AwayFromZero));
                        }
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;

                    if (dMkRate1 == dMkRate)
                    {
                        dMkDiscTotAmt = dMkRate1 - dMkAmt;
                    }

                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtActToAmount.Text) - dMkDiscTotAmt;
                    else
                        dTotAmt = 0;

                    if (dTotAmt > 0)
                        txtTotalAmount.Text = Convert.ToString(decimal.Round(dTotAmt, 2, MidpointRounding.AwayFromZero));

                    //if (dMkAmt > 0)
                    //{
                    //    if (dMkDiscTotAmt == 0 && dMkRate < dMkRate1)
                    //        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                    //    else
                    //        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt / dMakingQty, 2, MidpointRounding.AwayFromZero));
                    //}

                    //else if (dMkAmt != 0)
                    //{
                    //    txtChangedMakingRate.Text = txtActMakingRate.Text;
                    //}


                    CalWtDiffAndChangeMakingDisc(dMkRate1, dMkRate, dWtDiffDiscVal);
                    #endregion

                }
                if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                    // txtTotalAmount.Text = string.Empty;
                }
                //if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtPCS.Text.Trim()))//&& !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    #region when making type pcs
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am- Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;



                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }
                    else if (dMkRate1 > 0 && dMkRate >= 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }

                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;

                    //if (dMkAmt >= 0)
                    //{
                    //    if (dMkDiscTotAmt == 0)
                    //        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                    //    else
                    //        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    //}
                    //else
                    //    txtChangedMakingRate.Text = txtActMakingRate.Text;
                    if (dMkAmt > 0)
                    {
                        decimal dMkCh = 0m;
                        if (dManualChangesAmt > 0)
                        {
                            if (!string.IsNullOrEmpty(txtgval.Text))
                            {
                                dMkCh = ((dMkAmt * 100) / Convert.ToDecimal(txtgval.Text.Trim()));
                            }
                        }

                        if (dMkRate1 > dMkRate)
                        {
                            if (dMkDiscTotAmt == 0)
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                            else
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkCh, 2, MidpointRounding.AwayFromZero));
                        }
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;

                    CalWtDiffAndChangeMakingDisc(dMkRate1, dMkRate, dWtDiffDiscVal);

                    #endregion

                }
                if (cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                    //txtTotalAmount.Text = string.Empty;
                }


                if (cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                    // txtTotalAmount.Text = string.Empty;
                }

                decimal dMakingkAmt = 0m;
                decimal dActMakingAmt = 0m;


                if (!string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                    dMakingkAmt = Convert.ToDecimal(txtMakingAmount.Text.Trim());
                if (!string.IsNullOrEmpty(txtActMakingRate.Text.Trim()))
                    dActMakingAmt = Convert.ToDecimal(txtActMakingRate.Text.Trim());

                if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + dMakingkAmt);

                    if (string.IsNullOrEmpty(txtActToAmount.Text))
                    {
                        txtActToAmount.Text = txtTotalAmount.Text;
                    }
                    else
                    {
                        if (txtRate.Enabled)//added on 10/01/2018
                        {
                            txtActToAmount.Text = txtTotalAmount.Text;
                        }
                        if (dMkRate > dMkRate1)
                        {
                            txtActToAmount.Text = txtTotalAmount.Text;
                        }
                    }
                }

                //
                if (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                    //txtTotalAmount.Text = string.Empty;
                }

                // Added  -- wastage
                if ((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                {
                    if ((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                    {
                        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                        //txtActToAmount.Text = txtTotalAmount.Text;
                    }
                }
                if (cmbMakingType.SelectedIndex == 2 && !isMRPUCP)
                {
                    #region for making type tot
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am;; - Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;



                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dManualChangesAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }
                    else if (dMkRate1 > 0 && dMkRate >= 0)
                    {
                        if (rdbSale.Checked)
                        {
                            txtTotalAmount.Text = Convert.ToString(decimal.Round((dActTotAmt - dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            if (!isMRPUCP)
                            {
                                dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));


                                txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                                txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                            }
                        }
                    }

                    //txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    //decimalAmount = Convert.ToDecimal(dMkRate1);

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(dMkRate1 - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    //if (dMkAmt > 0)
                    //{
                    //    if (dMkDiscTotAmt == 0)
                    //        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                    //    else
                    //        txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    //}
                    //else if (dMkRate1 == dMkDiscTotAmt)
                    //    txtChangedMakingRate.Text = "0";
                    //else
                    //    txtChangedMakingRate.Text = txtActMakingRate.Text;

                    if (dMkAmt > 0)
                    {
                        decimal dMkCh = 0m;
                        if (dManualChangesAmt > 0)
                        {
                            if (!string.IsNullOrEmpty(txtgval.Text))
                            {
                                dMkCh = ((dMkAmt * 100) / Convert.ToDecimal(txtgval.Text.Trim()));
                            }
                        }

                        if (dMkRate1 > dMkRate)
                        {
                            if (dMkDiscTotAmt == 0)
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                            else
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkCh, 2, MidpointRounding.AwayFromZero));
                        }
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;

                    CalWtDiffAndChangeMakingDisc(dMkRate1, dMkRate, dWtDiffDiscVal);

                    #endregion
                }


                if (isMRPUCP)
                {
                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am;; - Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;

                    if (dManualChangesAmt > 0)//dActTotAmt > 0 &&
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dManualChangesAmt), 2, MidpointRounding.AwayFromZero));
                    }
                }

                if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && !string.IsNullOrEmpty(txtAmount.Text.Trim()) && !isMRPUCP)
                {
                    #region when making type percentage
                    if (!string.IsNullOrEmpty(txtgval.Text))
                    {
                        decimal dOMVal = 0m;
                        if (!string.IsNullOrEmpty(txtOMValue.Text))
                        {
                            dOMVal = Convert.ToDecimal(txtOMValue.Text.Trim());
                        }

                        if (Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()) + dOMVal);
                        else
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }
                    else
                    {
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am;; - Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;

                    if (dManualChangesAmt > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round(dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                    {
                        if (decimalAmount > 0 && !string.IsNullOrEmpty(txtAmount.Text))
                        {
                            decimal dAmt = Convert.ToDecimal((txtAmount.Text));
                            txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount + dAmt, 2, MidpointRounding.AwayFromZero));
                            txtActToAmount.Text = txtTotalAmount.Text;
                        }
                    }


                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;


                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        // txtChangedMakingRate.Text = txtActMakingRate.Text;
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        // txtChangedMakingRate.Text = txtActMakingRate.Text;
                        if (!isMRPUCP)
                        {
                            if (dMkRate1 > dMkRate)
                            {
                                dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                                txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                                txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                            }
                        }

                    }



                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    // decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        decimal dMkCh = 0m;
                        if (dManualChangesAmt > 0)
                        {
                            if (!string.IsNullOrEmpty(txtgval.Text))
                            {
                                dMkCh = ((dMkAmt * 100) / Convert.ToDecimal(txtgval.Text.Trim()));
                            }
                        }

                        if (dMkRate1 > dMkRate)
                        {
                            if (dMkDiscTotAmt == 0)
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                            else
                                txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkCh, 2, MidpointRounding.AwayFromZero));
                        }
                        //else
                        //{
                        //    txtChangedMakingRate.Enabled = false;
                        //}
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;


                    CalWtDiffAndChangeMakingDisc(dMkRate1, dMkRate, dWtDiffDiscVal);

                    #endregion
                }
                #endregion
            }
            else
            {
                #region View mode
                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                {
                    #region when mking type weight and gross
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (cmbMakingType.SelectedIndex == 4)
                    {
                        dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                    }
                    else
                    {
                        if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                        {
                            string dNetWt = "";
                            if (bValidSKU)
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

                    if (iMkRateChanged == 1)//; iMkRateChanged = 0;iTotAmtChanged
                    {
                        decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) * dMakingQty;
                        txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }

                    if (!string.IsNullOrEmpty(txtQuantity.Text))
                        dQty = Convert.ToDecimal(txtQuantity.Text);
                    else
                        dQty = 0;

                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;


                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am;;- Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;

                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));


                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;

                    if (iTotAmtChanged == 1)//; iMkRateChanged = 0;iTotAmtChanged
                    {
                        if (dMkAmt > 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt / dMakingQty, 2, MidpointRounding.AwayFromZero));

                        else if (dMkAmt != 0)
                        {
                            txtChangedMakingRate.Text = txtActMakingRate.Text;

                        }
                        //txtChangedMakingRate.Text = txtActMakingRate.Text;
                    }
                    #endregion
                }
                if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    #region Pcs
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am;; - Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;

                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round((dActTotAmt) - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));// + Convert.ToDecimal(txtGoldTax.Text)
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }

                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;



                    if (dMkAmt >= 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;
                    #endregion
                }
                if (cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                if ((cmbMakingType.SelectedIndex == 2)
                    && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim().Trim()))
                {
                    decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim());
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }

                if (isMRPUCP)
                {
                    if (dManualChangesAmt > 0)//dActTotAmt > 0 &&
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dManualChangesAmt), 2, MidpointRounding.AwayFromZero));
                    }
                }

                if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    decimal dOMVal = 0m;
                    if (!string.IsNullOrEmpty(txtOMValue.Text))
                    {
                        dOMVal = Convert.ToDecimal(txtOMValue.Text.Trim());
                    }

                    if (!string.IsNullOrEmpty(txtgval.Text))
                    {
                        if (Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()) + dOMVal);
                        else
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }
                    else
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if (cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                decimal dLinDisc = 0m;
                if (string.IsNullOrEmpty(txtLineDisc.Text))
                    dLinDisc = 0;
                else
                    dLinDisc = Convert.ToDecimal(txtLineDisc.Text);


                if (!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()) && dLinDisc == 0)
                {
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));// + Convert.ToDecimal(txtGoldTax.Text.Trim())
                }
                //if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text) && !string.IsNullOrEmpty(txtAmount.Text) && dLinDisc == 0)
                //{
                //    Decimal decimalAmount1 = 0m;
                //    decimalAmount1 = Convert.ToDecimal(txtActMakingRate.Text.Trim()) * dMakingQty;
                //    decimalAmount = (decimalAmount1) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                //    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                //}
                if (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                if ((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                {
                    if ((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                    {
                        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                    }
                }

                if (cmbMakingType.SelectedIndex == 2)
                {
                    #region for making type tot
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);//am;;- Convert.ToDecimal(txtGoldTax.Text)
                    else
                        dManualChangesAmt = 0;



                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dManualChangesAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dActTotAmt - dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));

                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }

                    }

                    //txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    //decimalAmount = Convert.ToDecimal(dMkRate1);

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(dMkRate1 - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;
                    #endregion
                }

                decimal dFAmt = 0m;
                decimal dMkDiscAmt = 0m;
                decimal dMkingAmt = 0m;
                if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                    dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                else
                    dMkRate1 = 0;

                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                    dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                else
                    dMkRate = 0;


                if (dMkRate1 > 0 && dMkRate > 0)
                {
                    // txtChangedMakingRate.Text = txtActMakingRate.Text;
                    if (!isMRPUCP)
                    {
                        dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                        txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                        txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                    }
                }

                if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                    dFAmt = Convert.ToDecimal((txtTotalAmount.Text));

                if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                    dMkDiscAmt = Convert.ToDecimal((txtMakingDiscTotAmt.Text));

                if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                    dMkingAmt = Convert.ToDecimal((txtMakingAmount.Text));


                //txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dMkDiscAmt + dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero));
                //txtTotalAmount.Text = Convert.ToString(decimal.Round(dFAmt - dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero));
                //txtMakingAmount.Text = Convert.ToString(decimal.Round(dMkingAmt - dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero));

                decimal dMkDiscTotAmt1 = decimal.Round(dMkDiscAmt + dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);
                decimal dTotAmt1 = decimal.Round(dFAmt - dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);
                decimal dMKAmt1 = decimal.Round(dMkingAmt - dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);
                if (dMkingAmt != (dMKAmt1 + dMkDiscTotAmt1))
                {
                    dMkDiscTotAmt1 = dMkDiscTotAmt1 + (dMkingAmt - (dMKAmt1 + dMkDiscTotAmt1));
                }
                if (dMkDiscAmt == 0)
                {
                    txtWtDiffDiscAmt.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt1, 2, MidpointRounding.AwayFromZero));
                }
                else
                {
                    txtWtDiffDiscAmt.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt1 - dMkDiscAmt, 2, MidpointRounding.AwayFromZero));
                }

                txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt1, 2, MidpointRounding.AwayFromZero));
                txtTotalAmount.Text = Convert.ToString(decimal.Round(dTotAmt1, 2, MidpointRounding.AwayFromZero));
                txtMakingAmount.Text = Convert.ToString(decimal.Round(dMKAmt1, 2, MidpointRounding.AwayFromZero));


                iTotAmtChanged = 0;
                iMkRateChanged = 0;
                #endregion
            }
        }

        private void CalWtDiffAndChangeMakingDisc(decimal dMkRate1, decimal dMkRate, decimal dWtDiffDiscVal)
        {
            decimal dMkDiscAmt = 0m;
            decimal dFAmt = 0m;
            decimal dMkingAmt = 0m;
            decimal dMkDiscTotAmt1 = 0m;

            if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                dFAmt = Convert.ToDecimal((txtTotalAmount.Text));
            if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                dMkDiscAmt = Convert.ToDecimal((txtMakingDiscTotAmt.Text));
            if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                dMkingAmt = Convert.ToDecimal((txtMakingAmount.Text));

            if (dMkRate1 > dMkRate)
                dMkDiscTotAmt1 = decimal.Round(dMkDiscAmt + dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);
            else
                dMkDiscTotAmt1 = decimal.Round(dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);

            decimal dTotAmt1 = decimal.Round(dFAmt - dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);
            decimal dMKAmt1 = decimal.Round(dMkingAmt - dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero);

            if (dMkingAmt != (dMKAmt1 + dMkDiscTotAmt1))
            {
                dMkDiscTotAmt1 = dMkDiscTotAmt1 + (dMkingAmt - (dMKAmt1 + dMkDiscTotAmt1));
            }
            if (dMkDiscAmt == 0)
                txtWtDiffDiscAmt.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt1, 2, MidpointRounding.AwayFromZero));
            else if (dWtDiffDiscVal == dMkDiscAmt)
                txtWtDiffDiscAmt.Text = Convert.ToString(decimal.Round(dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero));
            else
                txtWtDiffDiscAmt.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt1 - dMkDiscAmt, 2, MidpointRounding.AwayFromZero));

            txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt1, 2, MidpointRounding.AwayFromZero));
            txtTotalAmount.Text = Convert.ToString(decimal.Round(dTotAmt1, 2, MidpointRounding.AwayFromZero));

            if (dWtDiffDiscVal == dMkDiscAmt)
                txtMakingAmount.Text = Convert.ToString(decimal.Round(dMkingAmt, 2, MidpointRounding.AwayFromZero));
            else
                txtMakingAmount.Text = Convert.ToString(decimal.Round(dMKAmt1, 2, MidpointRounding.AwayFromZero));
        }

        //08/05/18
        /*
        private void CalcMakingAtSelectedIndexChange()
        {
            Decimal dMakingQty = 0;
            Decimal decimalAmount = 0m;
            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;
            decimal dTotAmt = 0m;
            decimal dActTotAmt = 0m;
            decimal dManualChangesAmt = 0m;
            decimal dMkDiscTotAmt = 0m;
            decimal dQty = 0m;

            if (!isViewing)
            {
                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                {
                    #region when mking type weight and gross
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;
                    if (cmbMakingType.SelectedIndex == 4)
                    {
                        dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                    }
                    else
                    {
                        if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                        {
                            decimal dQty1 = 0m;
                            string dNetWt = "";
                            if (bValidSKU)
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


                    decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) * dMakingQty;

                    txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtQuantity.Text))
                        dQty = Convert.ToDecimal(txtQuantity.Text);
                    else
                        dQty = 0;

                    //DEV ON 20/0417 REQ S.SHARMA
                    decimal dAddMk = 0;
                    decimal dMinMkQty = 0m;

                    string sSqr1 = "select isnull(NOEXTRAMKADDITION,0) from INVENTTABLE where itemid='" + sBaseItemID + "'";
                    string sResultForNOEXTRAMKADDITION = NIM_ReturnExecuteScalar(sSqr1);

                    string sSqr2 = "select isnull(ADDITIONALMAKING,0) from RETAILPARAMETERS  where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                    string sAddMk = NIM_ReturnExecuteScalar(sSqr2);

                    string sSqr3 = "select isnull(MINMAKINGQTY,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                    string sMinMkQty = NIM_ReturnExecuteScalar(sSqr3);


                    dAddMk = Convert.ToDecimal(sAddMk);
                    dMinMkQty = Convert.ToDecimal(sMinMkQty);

                    if (sResultForNOEXTRAMKADDITION == "0")
                    {
                        if (!isBulkItem)
                        {
                            if (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                            {

                                if (Convert.ToDecimal(dMakingQty) <= dMinMkQty && Convert.ToDecimal(dMakingQty) > 0) //dMinMkQty==6(pre hard coded value) changes on 260418
                                {
                                    decimal dActMkRate = 0;
                                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                                        dActMkRate = Convert.ToDecimal(txtActMakingRate.Text);
                                    if (!isMkAdded)
                                    {
                                        txtActMakingRate.Text = Convert.ToString(decimal.Round(dActMkRate + (dAddMk / Convert.ToDecimal(dMakingQty)), 2, MidpointRounding.AwayFromZero));
                                        isMkAdded = true;
                                    }
                                    //txtChangedMakingRate.Text = txtActMakingRate.Text;
                                }
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am;
                    else
                        dManualChangesAmt = 0;

                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }

                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));


                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;



                    if (dMkAmt > 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt / dMakingQty, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;
                    #endregion

                }
                if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                    // txtTotalAmount.Text = string.Empty;
                }
                if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    #region when making type pcs
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am
                    else
                        dManualChangesAmt = 0;



                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }

                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;

                    #endregion

                }
                if (cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                    //txtTotalAmount.Text = string.Empty;
                }


                if (cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                    // txtTotalAmount.Text = string.Empty;
                }


                if (!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()))
                {
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));

                    if (string.IsNullOrEmpty(txtActToAmount.Text))
                    {
                        txtActToAmount.Text = txtTotalAmount.Text;
                    }
                    else
                    {
                        if (txtRate.Enabled)//added on 10/01/2018
                        {
                            txtActToAmount.Text = txtTotalAmount.Text;
                        }
                    }
                }



                //if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text) && !string.IsNullOrEmpty(txtAmount.Text))
                //{

                //    Decimal decimalAmount1 = 0m;
                //    decimalAmount1 = Convert.ToDecimal(txtMakingAmount.Text.Trim());//* dMakingQty
                //    // decimalAmount = (Convert.ToDecimal(txtMakingAmount.Text.Trim())) - ((Convert.ToDecimal(txtMakingDisc.Text.Trim()) / 100) * (Convert.ToDecimal(txtMakingAmount.Text.Trim()))) + (Convert.ToDecimal(txtAmount.Text));
                //    decimalAmount = (decimalAmount1) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                //    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                //    //txtActToAmount.Text = txtTotalAmount.Text;
                //}


                //
                if (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                    //txtTotalAmount.Text = string.Empty;
                }

                // Added  -- wastage
                if ((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                {
                    if ((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                    {
                        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                        //txtActToAmount.Text = txtTotalAmount.Text;
                    }
                }
                if (cmbMakingType.SelectedIndex == 2)
                {
                    #region for making type tot
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am;;
                    else
                        dManualChangesAmt = 0;



                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dManualChangesAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dActTotAmt - dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }

                    //txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    //decimalAmount = Convert.ToDecimal(dMkRate1);

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(dMkRate1 - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else if (dMkRate1 == dMkDiscTotAmt)
                        txtChangedMakingRate.Text = "0";
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;
                    #endregion
                }

                if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    #region when making type percentage
                    if (!string.IsNullOrEmpty(txtgval.Text))
                    {
                        if (Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()));
                        else
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }
                    else
                    {
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am;;
                    else
                        dManualChangesAmt = 0;

                    if (dManualChangesAmt > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round(dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                    {
                        if (decimalAmount > 0 && !string.IsNullOrEmpty(txtAmount.Text))
                        {
                            decimal dAmt = Convert.ToDecimal((txtAmount.Text));
                            txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount + dAmt, 2, MidpointRounding.AwayFromZero));
                            txtActToAmount.Text = txtTotalAmount.Text;
                        }
                    }


                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;


                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        
                        // txtChangedMakingRate.Text = txtActMakingRate.Text;
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        // txtChangedMakingRate.Text = txtActMakingRate.Text;
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }



                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    // decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        decimal dMkCh = 0m;
                        if (dManualChangesAmt > 0)
                        {
                            if (!string.IsNullOrEmpty(txtgval.Text))
                            {
                                dMkCh = ((dMkAmt * 100) / Convert.ToDecimal(txtgval.Text.Trim()));
                            }
                        }


                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkCh, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;

                    #endregion
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim())
                    && (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                {
                    #region when mking type weight and gross
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (cmbMakingType.SelectedIndex == 4)
                    {
                        dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                    }
                    else
                    {
                        if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                        {
                            string dNetWt = "";
                            if (bValidSKU)
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

                    if (iMkRateChanged == 1)//; iMkRateChanged = 0;iTotAmtChanged
                    {
                        decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) * dMakingQty;
                        txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - decimalAmount, 2, MidpointRounding.AwayFromZero));
                    }

                    if (!string.IsNullOrEmpty(txtQuantity.Text))
                        dQty = Convert.ToDecimal(txtQuantity.Text);
                    else
                        dQty = 0;

                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;


                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am;;
                    else
                        dManualChangesAmt = 0;

                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    txtMakingAmount.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));


                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;

                    if (iTotAmtChanged == 1)//; iMkRateChanged = 0;iTotAmtChanged
                    {
                        if (dMkAmt > 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt / dMakingQty, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = txtActMakingRate.Text;
                        //txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * dMakingQty) - (dMkRate * dMakingQty), 2, MidpointRounding.AwayFromZero));
                    }
                    #endregion
                }
                if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                    && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtQuantity.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
                {
                    #region Pcs
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am;;
                    else
                        dManualChangesAmt = 0;

                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {

                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round((dActTotAmt) - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));// + Convert.ToDecimal(txtGoldTax.Text)
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            // txtChangedMakingRate.Text = txtActMakingRate.Text;
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }

                    //txtMakingAmount.Text = txtMakingDiscTotAmt.Text;
                    txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    decimalAmount = Convert.ToDecimal(dMkRate1) * Convert.ToDecimal(txtPCS.Text.Trim());
                    //  txtMakingAmount.Text = Convert.ToString(decimalAmount);
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;
                    #endregion
                }
                if (cmbMakingType.SelectedIndex == 0 && (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                if ((cmbMakingType.SelectedIndex == 2)
                    && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim().Trim()))
                {
                    decimalAmount = Convert.ToDecimal(txtChangedMakingRate.Text.Trim());
                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()) && !string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    if (!string.IsNullOrEmpty(txtgval.Text))
                    {
                        if (Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()));
                        else
                            decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                    }
                    else
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if (cmbMakingType.SelectedIndex == 3 && string.IsNullOrEmpty(txtAmount.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                decimal dLinDisc = 0m;
                if (string.IsNullOrEmpty(txtLineDisc.Text))
                    dLinDisc = 0;
                else
                    dLinDisc = Convert.ToDecimal(txtLineDisc.Text);


                if (!string.IsNullOrEmpty(txtAmount.Text.Trim()) && !string.IsNullOrEmpty(txtMakingAmount.Text.Trim()) && dLinDisc == 0)
                {
                    txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtAmount.Text.Trim()) + Convert.ToDecimal(txtMakingAmount.Text.Trim()));// + Convert.ToDecimal(txtGoldTax.Text.Trim())
                }
                //if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text) && !string.IsNullOrEmpty(txtMakingAmount.Text) && !string.IsNullOrEmpty(txtAmount.Text) && dLinDisc == 0)
                //{
                //    Decimal decimalAmount1 = 0m;
                //    decimalAmount1 = Convert.ToDecimal(txtActMakingRate.Text.Trim()) * dMakingQty;
                //    decimalAmount = (decimalAmount1) - (Convert.ToDecimal(txtMakingDiscTotAmt.Text)) + (Convert.ToDecimal(txtAmount.Text));
                //    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                //}
                if (string.IsNullOrEmpty(txtChangedMakingRate.Text.Trim()))
                {
                    txtMakingAmount.Text = string.Empty;
                }
                if ((!string.IsNullOrEmpty(txtTotalAmount.Text.Trim())) && (!string.IsNullOrEmpty(txtWastageAmount.Text.Trim())))
                {
                    if ((Convert.ToDecimal(txtTotalAmount.Text) > 0) && (Convert.ToDecimal(txtWastageAmount.Text) > 0))
                    {
                        txtTotalAmount.Text = Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text.Trim()) + Convert.ToDecimal(txtWastageAmount.Text.Trim()));
                    }
                }

                if (cmbMakingType.SelectedIndex == 2)
                {
                    #region for making type tot
                    if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                        dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
                    else
                        dMkRate1 = 0;

                    if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                        dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                    else
                        dMkRate = 0;

                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                        dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    else
                        dTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtActToAmount.Text))
                        dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
                    else
                        dActTotAmt = 0;
                    if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                        dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
                    else
                        dMkDiscTotAmt = 0;

                    if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                        dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text) - Convert.ToDecimal(txtGoldTax.Text);//am;;
                    else
                        dManualChangesAmt = 0;



                    decimal dMkAmt = 0;
                    if (dActTotAmt > 0 && dManualChangesAmt > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dManualChangesAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dActTotAmt - dManualChangesAmt, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }
                    else if (dMkRate1 > 0 && dMkRate > 0)
                    {
                        txtTotalAmount.Text = Convert.ToString(decimal.Round((dActTotAmt - dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                        if (!isMRPUCP)
                        {
                            dMkDiscTotAmt = Convert.ToDecimal(decimal.Round(dMkRate1 - dMkRate, 2, MidpointRounding.AwayFromZero));
                            txtMakingDisc.Text = Convert.ToString(decimal.Round((dMkDiscTotAmt), 2, MidpointRounding.AwayFromZero));
                            txtMakingDiscTotAmt.Text = txtMakingDisc.Text;
                        }
                    }

                    //txtMakingDisc.Text = Convert.ToString(decimal.Round(dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));
                    //decimalAmount = Convert.ToDecimal(dMkRate1);

                    txtMakingAmount.Text = Convert.ToString(decimal.Round(dMkRate1 - dMkDiscTotAmt, 2, MidpointRounding.AwayFromZero));

                    // txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round((dMkRate1 * Convert.ToDecimal(txtPCS.Text.Trim())) - decimalAmount, 2, MidpointRounding.AwayFromZero));

                    if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                        dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    else
                        dMkAmt = 0;


                    if (dMkAmt > 0)
                    {
                        if (dMkDiscTotAmt == 0)
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkRate1, 2, MidpointRounding.AwayFromZero));
                        else
                            txtChangedMakingRate.Text = Convert.ToString(decimal.Round(dMkAmt, 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        txtChangedMakingRate.Text = txtActMakingRate.Text;
                    #endregion
                }

                iTotAmtChanged = 0;
                iMkRateChanged = 0;
            }
        }
        */


        #endregion

        #region Validate Controls
        private bool ValidateControls()
        {
            DataTable dtBookedSKU = BookedSKU(Convert.ToString(lblCustOrder.Text), sCustomerId);
            if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
            {
                bool bBookedSku = false;

                foreach (DataRow drNew in dtBookedSKU.Rows)
                {
                    //if (sBaseItemID == Convert.ToString(drNew["SKUNUMBER"]))
                    //{
                    //    if (!IsBulkItem(sBaseItemID))
                    //    {
                    //        bBookedSku = true;
                    //        break;
                    //    }
                    //}
                    //else
                    //{
                    //    bBookedSku = false;
                    //    break;
                    //}

                    bBookedSku = true;

                    if (IsRetailItem(sBaseItemID))
                    {
                        break;
                    }
                }
                return bBookedSku;
            }

            #region new Making disc
            decimal dinitDiscValue = 0;
            decimal dDiscValue = 0m;
            decimal dQty = 0;

            int iPcs = 0;

            if (!string.IsNullOrEmpty(txtQuantity.Text))
                dQty = Convert.ToDecimal(txtQuantity.Text);
            else
                dQty = 0;
            if (!isMRPUCP)
                dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "OPENINGDISCPCT");// get OPENINGDISCPCT field value FOR THE OPENING

            decimal dMkPerDisc = 0;
            if (!string.IsNullOrEmpty(txtMakingDisc.Text))
                dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
            else
                dMkPerDisc = 0;

            if (!string.IsNullOrEmpty(txtPCS.Text))
                iPcs = Convert.ToInt16(txtPCS.Text);
            else
                iPcs = 0;

            //if (dinitDiscValue >= 0 && !string.IsNullOrEmpty(txtMakingAmount.Text))
            //{
            //    if ((dMkPerDisc > dinitDiscValue))
            //    {
            //        MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
            //        txtMakingDisc.Focus();
            //        return false;
            //    }
            //    else
            //    {
            //        if (iMkDisType == (int)MakingDiscType.Percent)
            //            dDiscValue = (Convert.ToDecimal(txtMakingAmount.Text) * dinitDiscValue) / 100;
            //        else if (iMkDisType == (int)MakingDiscType.PerGram)
            //            dDiscValue = (dQty * dinitDiscValue);
            //        else
            //            dDiscValue = dinitDiscValue * iPcs;

            //        txtMakingDisc.Text = Convert.ToString(dDiscValue);
            //        txtMakingDisc.Tag = iIsMkDiscFlat;
            //    }
            //}
            #endregion

            if (IsCatchWtItem(sBaseItemID))
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
                else if (Convert.ToDecimal(txtPCS.Text) == 0)
                {
                    txtPCS.Focus();
                    MessageBox.Show("Pieces cannot be zero for catch weight product.", "Pieces cannot be zero catch weight product.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                else
                {

                    dQty = Convert.ToDecimal(txtQuantity.Text);
                    decimal dPcs = Convert.ToDecimal(txtPCS.Text.Trim());

                    string sMaxQty = NIM_ReturnExecuteScalar("SELECT isnull(PDSCWMAX,0) PDSCWMAX FROM PDSCATCHWEIGHTITEM  WHERE ITEMID ='" + sBaseItemID + "'");
                    string sMinQty = NIM_ReturnExecuteScalar("SELECT isnull(PDSCWMIN,0) PDSCWMIN FROM PDSCATCHWEIGHTITEM  WHERE ITEMID ='" + sBaseItemID + "'");

                    if (dQty > 0 && dPcs > 0)
                    {
                        if (dQty > (Convert.ToDecimal(sMaxQty) * dPcs) || dQty < (Convert.ToDecimal(sMinQty) * dPcs))
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Quantity shuld be within " + (Convert.ToDecimal(sMaxQty) * dPcs) + " and " + (Convert.ToDecimal(sMinQty) * dPcs) + " ", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                txtQuantity.Focus();
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                return false;
                            }
                        }
                    }
                    else
                        return false;

                }
            }
            else
            {
                txtPCS.Enabled = false;
            }

            //if(string.IsNullOrEmpty(txtPCS.Text))
            //{
            //    txtPCS.Focus();
            //    MessageBox.Show("Pieces cannot be Empty.", "Pieces cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return false;
            //}


            if (txtBatchNo.Visible == true && IsBatchItem(sBaseItemID))
            {
                if (string.IsNullOrEmpty(txtBatchNo.Text))
                {
                    MessageBox.Show("Batch No cannot be Empty.", "Batch No cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                if (!IsValidBatchId(txtBatchNo.Text.Trim()))
                {
                    MessageBox.Show("Invalid Batch No.", "Invalid Batch No.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                if ((dtExtndPurchExchng.Rows.Count > 0))
                {
                    int iOgLineNo = Convert.ToInt16(dtExtndPurchExchng.Rows[0]["OGLINENUM"]);
                    string sOgBatchId = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGRECEIPTNO"]);

                    decimal dRetQty = getReturnQty(sOgBatchId, iOgLineNo);
                    decimal dRetPcs = getReturnPcs(sOgBatchId, iOgLineNo);

                    //commented on 050517 req by S.Sharma for partial return of og
                    //if (!IsReturnQtyValid(sOgBatchId, iOgLineNo, Convert.ToDecimal(txtQuantity.Text)))
                    //{
                    //    MessageBox.Show("Return quantity is not matching.", "Return quantity is not matching.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return false;
                    //}
                    //if (!IsReturnPcsValid(sOgBatchId, iOgLineNo, Convert.ToDecimal(txtPCS.Text)))
                    //{
                    //    MessageBox.Show("Return Pcs is not matching.", "Return Pcs is not matching.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    return false;
                    //}
                    if (IsCatchWtItem(sBaseItemID))
                    {
                        if (dRetQty <= 0 || dRetPcs <= 0)
                        {
                            MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dRetPcs < Convert.ToDecimal(txtPCS.Text) || dRetQty < Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dRetPcs == Convert.ToDecimal(txtPCS.Text) && dRetQty != Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dRetPcs != Convert.ToDecimal(txtPCS.Text) && dRetQty == Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                    else
                    {
                        if (dRetQty <= 0)
                        {
                            MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dRetQty < Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
            }

            if (IsRetailItem(sBaseItemID) && (rdbSale.Checked == false)) // added on 17/04/2014 for retail=1 item can be only sale trans
            {
                MessageBox.Show("Item is not valid for this transaction.", "Item is not valid for this transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (string.IsNullOrEmpty(txtQuantity.Text))
            {
                txtQuantity.Focus();
                MessageBox.Show("Quantity cannot be Empty.", "Quantity cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (string.IsNullOrEmpty(txtRate.Text))
            {
                txtRate.Focus();
                MessageBox.Show("Rate cannot be Empty.", "Rate cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (string.IsNullOrEmpty(txtAmount.Text))
            {
                MessageBox.Show("Amount cannot be Empty.", "Amount cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (IsRetailItem(sBaseItemID))
            {
                if (!IsExistinSKUTABLE_Posted(sBaseItemID))
                {
                    MessageBox.Show("SKU is not present in SKU table, Please contact to head office.", "SKU is not present in SKU table.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            if (IsIngredient(sBaseItemID) && btnIngrdientsDetails.Visible == false)
            {
                MessageBox.Show("Item has no ingredients, Please contact to head office.", "Item has no ingredients.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (rdbSale.Checked)
            {
                decimal dSalesValue = 0m;
                int iMetalType = GetMetalType(sBaseItemID);

                string sSqr = "select isnull(NOCOSTVALIDATERETAIL,0) from INVENTTABLE where itemid='" + sBaseItemID + "'";
                string sResultForNOCOSTVALIDATERETAIL = NIM_ReturnExecuteScalar(sSqr);

                if (IsIngredient(sBaseItemID) && iMetalType == (int)MetalType.Gold)
                {
                    decimal convertedValue = 0;
                    foreach (DataRow dr in dtIngredientsClone.Rows)
                    {
                        convertedValue += Convert.ToDecimal(NIM_GetConvertionValue(Convert.ToString(dr["UNITID"]), sBaseUnitId, Convert.ToDecimal(dr["QTY"]), Convert.ToInt16(dr["METALTYPE"])));
                    }
                    convertedValue = decimal.Round(Convert.ToDecimal(convertedValue), 3, MidpointRounding.AwayFromZero);

                    if (convertedValue != Convert.ToDecimal(txtQuantity.Text))
                    {
                        MessageBox.Show("Quantity mismatch.", "Quantity mismatch.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }

                if (sResultForNOCOSTVALIDATERETAIL == "0")
                {
                    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                    {
                        dSalesValue = Convert.ToDecimal(txtTotalAmount.Text);// - metal value if is metal=gold
                        if (IsBulkItem(sBaseItemID))
                        {
                            if (!getCostPrice(sBaseItemID, dSalesValue))
                            {
                                MessageBox.Show("Sales price can not be less than cost price.", "Sales price can not be less than cost price.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                        else
                        {
                            if (iMetalType == (int)MetalType.Gold && !string.IsNullOrEmpty(txtgval.Text))
                            {
                                decimal dGoldValue = Convert.ToDecimal(txtgval.Text);
                                dSalesValue = dSalesValue - dGoldValue;

                                if (!getCostPrice(sBaseItemID, dSalesValue))
                                {
                                    MessageBox.Show("Sales price can not be less than cost price.", "Sales price can not be less than cost price.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    return false;
                                }
                            }
                        }
                    }
                }

                if (iMetalType == (int)MetalType.PackingMaterial)
                {
                    return true;
                }

                /* decimal dActMkRate = 0m;
                 decimal dChangedMkRate = 0m;
                 if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                 {
                     dChangedMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
                 }
                 if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                 {
                     dActMkRate = Convert.ToDecimal(txtActMakingRate.Text);
                 }*/

                if (string.IsNullOrEmpty(txtChangedMakingRate.Text))
                {
                    txtChangedMakingRate.Focus();
                    txtChangedMakingRate.Text = "0";
                    //txtActMakingRate.Text = dActMkRate;
                }
                if (!isMRPUCP)
                {
                    if (!getValidMakingCalValue())
                    {
                        txtChangedTotAmount.Focus();
                        return false;
                    }
                }
                if (isMRPUCP)
                {
                    if (!getValidMRPItemLineDiscount())
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(txtMakingAmount.Text))
                {
                    decimal dMkAmt = Convert.ToDecimal(txtMakingAmount.Text);
                    if (dMkAmt < 0)
                    {
                        txtTotalAmount.Focus();
                        MessageBox.Show("Making amount can not be negetive.", "Making amount can not be negetive.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }

                if (!string.IsNullOrEmpty(txtActToAmount.Text) && !string.IsNullOrEmpty(txtTotalAmount.Text))
                {
                    decimal dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);//rh//+ Convert.ToDecimal(txtGoldTax.Text)
                    decimal dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
                    if (dActTotAmt < dTotAmt)
                    {
                        txtTotalAmount.Focus();
                        MessageBox.Show("Final amount can not be greater than actual total amount.", "Final amount can not be greater than actual total amount.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }

                if (IsBulkItem(sBaseItemID))
                {
                    //saleLine.PartnerData.BulkItem = 1;
                    if (IsBatchItem(sBaseItemID))
                    {
                        frmSalesInfoCode objBatch = new frmSalesInfoCode(true);
                        objBatch.ShowDialog();
                        BatchID = objBatch.sCodeOrRemarks;
                    }

                    decimal dBulkPdsQty = 0m;
                    decimal dBulkQty = 0m;
                    GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty,
                                                        sBaseItemID, ConfigID,
                                                        SizeID, ColorID,
                                                        StyleID, BatchID);

                    if (IsCatchWtItem(sBaseItemID))
                    {
                        if (dBulkPdsQty <= 0 || dBulkQty <= 0)
                        {
                            MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dBulkPdsQty < Convert.ToDecimal(txtPCS.Text) || dBulkQty < Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dBulkPdsQty == Convert.ToDecimal(txtPCS.Text) && dBulkQty != Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dBulkPdsQty != Convert.ToDecimal(txtPCS.Text) && dBulkQty == Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                    else
                    {
                        if (dBulkQty <= 0)
                        {
                            MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                        else if (dBulkQty < Convert.ToDecimal(txtQuantity.Text))
                        {
                            MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }
            }
            else
            {
                if (RadioChecked == Convert.ToString((int)TransactionType.Purchase)
                   || RadioChecked == Convert.ToString((int)TransactionType.Exchange))
                {
                    if (retailTrans != null && retailTrans.AmountDue < 0) //&& retailTrans.AmountDue < 0
                    {
                        if (retailTrans.SaleIsReturnSale == true)
                        {
                            MessageBox.Show("OG transaction can not done if sales return are in sales line");
                            return false;
                        }
                    }
                }
                else
                {
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleIsReturnSale == true)
                        {
                            MessageBox.Show("OG transaction can not done if sales return are in sales line");
                            return false;
                        }
                    }
                }

                if (IsSaleAdjustment)
                {
                    if (retailTrans != null)
                    {
                        if (retailTrans.AmountDue < 0)
                        {
                            MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;
                        }
                    }
                }

                if (string.IsNullOrEmpty(txtTotalWeight.Text))
                {
                    txtChangedMakingRate.Focus();
                    MessageBox.Show("Total Weight can not be Empty.", "Total Weight can not be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                #region commented for Samra
                //if((string.IsNullOrEmpty(txtPurity.Text)) && (!chkOwn.Checked))
                //{
                //    txtPurity.Focus();
                //    MessageBox.Show("Purity cannot be Empty.", "Purity cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return false;
                //}

                //if(string.IsNullOrEmpty(txtLossPct.Text))
                //{
                //    txtMakingDisc.Focus();
                //    MessageBox.Show("Loss Percentage cannot be Empty.", "Loss Percentage cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return false;
                //}
                //if(string.IsNullOrEmpty(txtLossWeight.Text))
                //{
                //    MessageBox.Show("Loss Weight cannot be Empty.", "Loss Weight cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return false;
                //}
                #endregion

                if (string.IsNullOrEmpty(txtExpectedQuantity.Text))
                {
                    MessageBox.Show("Expected Quantity cannot be Empty.", "Expected Quantity cannot be Empty.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }

                int iMetalType = GetMetalType(sBaseItemID);
                if (iMetalType == (int)MetalType.PackingMaterial)
                {
                    return true;
                }

                if (RadioChecked == Convert.ToString((int)TransactionType.PurchaseReturn)
                    || RadioChecked == Convert.ToString((int)TransactionType.ExchangeReturn))
                {
                    if (IsBulkItem(sBaseItemID))
                    {
                        //saleLine.PartnerData.BulkItem = 1;
                        decimal dBulkPdsQty = 0m;
                        decimal dBulkQty = 0m;

                        if (string.IsNullOrEmpty(BatchID))
                            BatchID = txtBatchNo.Text;

                        GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty,
                                                            sBaseItemID, ConfigID,
                                                            SizeID, ColorID,
                                                            StyleID, BatchID);

                        if (IsCatchWtItem(sBaseItemID))
                        {
                            if (dBulkPdsQty <= 0 || dBulkQty <= 0)
                            {
                                MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                            else if (dBulkPdsQty < Convert.ToDecimal(txtPCS.Text) || dBulkQty < Convert.ToDecimal(txtQuantity.Text))
                            {
                                MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                            else if (dBulkPdsQty == Convert.ToDecimal(txtPCS.Text) && dBulkQty != Convert.ToDecimal(txtQuantity.Text))
                            {
                                MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                            else if (dBulkPdsQty != Convert.ToDecimal(txtPCS.Text) && dBulkQty == Convert.ToDecimal(txtQuantity.Text))
                            {
                                MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                        else
                        {
                            if (dBulkQty <= 0)
                            {
                                MessageBox.Show("Please check the inventory.", "Please check the inventory.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                            else if (dBulkQty < Convert.ToDecimal(txtQuantity.Text))
                            {
                                MessageBox.Show("Please check the inventory  for " + sBaseItemID + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        }
                    }
                }

            }
            #region MyRegion //Start : Changes on 08/04/2014 RHossain
            //RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null
            && retailTrans.SaleItems != null
            && retailTrans.SaleItems.Count > 0)
            {
                SaleLineItem saleItem = retailTrans.SaleItems.Last.Value;
                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    // if (saleLineItem.ItemId == saleItem.ItemId)
                    preTransType = saleItem.PartnerData.TransactionType;

                    switch (Convert.ToInt16(preTransType))
                    {
                        case 0:
                            if (rdbSale.Checked || rdbExchange.Checked || rdbPurchase.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 1:
                            if (rdbPurchase.Checked || rdbSale.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 2:
                            if (rdbPurchReturn.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 3:
                            if (rdbSale.Checked || rdbExchange.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        case 4:
                            if (rdbExchangeReturn.Checked)
                                return true;
                            else
                            {
                                MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return false;
                            }
                        default:
                            MessageBox.Show("Invalid transaction.", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return false;

                    }

                }
            }
            #endregion //End : Changes on 08/04/2014 RHossain
            return true;
        }
        #endregion


        private void GetValidBulkItemPcsAndQtyForTrans(ref decimal dPdsCWQTY,
                                                    ref decimal dQty, string sItemId,
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

            sbQuery.Append("select sum(PdsCWQty) as PDSCWQTY,sum(Qty) as QTY from BulkItemTransTable ");
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
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        #region Close Click
        private void btnCLose_Click(object sender, EventArgs e)
        {
            ClearControls();
            if (rdbSale.Checked)
            {
                BuildSaleList();
                purchaseList = new List<KeyValuePair<string, string>>();
            }
            else
            {
                BuildPurchaseList();
                saleList = new List<KeyValuePair<string, string>>();
            }
            dtIngredients = new DataTable();
            dtIngredientsClone = new DataTable();
            this.Close();
        }
        #endregion

        #region Radio Buttons Checked Changed
        private void rdbSale_CheckedChanged(object sender, EventArgs e)
        {
            lblBatchNo.Visible = false;
            txtBatchNo.Visible = false;
            if (!isViewing)
            {
                //  RadioChecked = "Sale";
                RadioChecked = Convert.ToString((int)TransactionType.Sale);

                dTransferCost = getTransferCostPrice(ItemID);
                if (!IsRetailItem(ItemID))
                    EnableSaleButtons();
                else // On Request of S.Sharma on 09/07/2014
                {
                    txtPCS.Enabled = false;
                    txtRate.Enabled = false;
                    txtTotalWeight.Enabled = false;
                    txtPurity.Enabled = false;
                    txtLossPct.Enabled = false;
                }

                if (saleList != null && saleList.Count > 0)
                {
                    BindSaleControls();
                }
                else
                {
                    BuildSaleList();
                    ClearControls();
                }
                //chkOwn.Checked = false;
                //chkOwn.Visible = false;
                cmbRateType.SelectedIndex = 0;
                txtRate.Text = string.Empty;
                //txtRate.Text = getRateFromMetalTable();
                BindIngredientGrid();
                panel4.Enabled = true;
                FnArrayList();
                //chkSampleReturn.Visible = true;

                if (isMRPUCP)// On Request of S.Sharma on 09/07/2014
                {
                    cmbRateType.SelectedIndex = (int)RateType.Tot;
                    cmbRateType.Enabled = false;
                }


            }
        }

        private void InsertMakingInfo(string sTableName, string sTransactionId, decimal dAmt)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" TRANSACTIONID NVARCHAR (20),MakingAmt numeric(20,2)) END");
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTransactionId + "', " + dAmt + ") END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }
        private void UpdateMakingInfo(string sTableName, string sTransactionId, decimal dAmt)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  Update " + sTableName + " set  MakingAmt = MakingAmt + " + dAmt + " Where TRANSACTIONID = '" + sTransactionId + "' END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void rdbPurchase_CheckedChanged(object sender, EventArgs e)
        {
            lblBatchNo.Visible = false;
            txtBatchNo.Visible = false;
            //chkOwn.Enabled = true;
            if (!isViewing)
            {

                // RadioChecked = "Purchase";
                RadioChecked = Convert.ToString((int)TransactionType.Purchase);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BuildPurchaseList();
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls();

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0";
                //chkOwn_CheckedChanged(sender, e);
            }
        }

        private void rdbExchange_CheckedChanged(object sender, EventArgs e)
        {
            lblBatchNo.Visible = false;
            txtBatchNo.Visible = false;
            if (!isViewing)
            {
                txtLossPct.Text = "0";
                //   RadioChecked = "Exchange";
                RadioChecked = Convert.ToString((int)TransactionType.Exchange);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls(); //
                // 
                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0"; //
                //chkOwn_CheckedChanged(sender, e);
            }
        }

        private void rdbPurchReturn_CheckedChanged(object sender, EventArgs e)
        {
            lblBatchNo.Visible = true;
            txtBatchNo.Visible = true;
            txtBatchNo.Enabled = false;
            chkOwn.Enabled = true;
            if (!isViewing)
            {

                //  RadioChecked = "PurchaseReturn";
                RadioChecked = Convert.ToString((int)TransactionType.PurchaseReturn);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls(); //

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0";
                //chkOwn_CheckedChanged(sender, e);
            }
        }

        private void rdbExchangeReturn_CheckedChanged(object sender, EventArgs e)
        {
            lblBatchNo.Visible = true;
            txtBatchNo.Visible = true;
            txtBatchNo.Enabled = false;
            if (!isViewing)
            {
                txtLossPct.Text = "0";
                //  RadioChecked = "ExchangeReturn";
                RadioChecked = Convert.ToString((int)TransactionType.ExchangeReturn);
                EnablePurchaseExchangeButtons();
                //if (purchaseList != null && purchaseList.Count > 0)
                //{
                //    BindPurchaseControls();
                //}
                //else
                //{
                //    BuildPurchaseList();
                //    ClearControls();
                //}

                ClearControls(); //

                CommonHelper();
                chkSampleReturn.Visible = false;
                txtLossPct.Text = "0";
                //chkOwn_CheckedChanged(sender, e);
            }
        }
        #endregion

        #region Bind Sale Purchase COntrols
        private void BindSaleControls()
        {
            txtPCS.Text = saleList[0].Value;
            txtQuantity.Text = saleList[1].Value;
            txtRate.Text = saleList[2].Value;
            //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact(Convert.ToString(saleList[3].Value));
            cmbRateType.SelectedIndex = Convert.ToInt16(saleList[3].Value);
            txtChangedMakingRate.Text = saleList[4].Value;
            txtActMakingRate.Text = saleList[4].Value;
            //    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(saleList[5].Value));
            switch (Convert.ToInt16(saleList[5].Value))
            {
                case 0:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Pieces));
                    break;
                case 2:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Weight));
                    break;
                case 3:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Tot));
                    break;
                case 4:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Percentage));
                    break;
                //case 5:
                //    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Inch));
                //    break;
                case 6:
                    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Gross));
                    break;
            }

            txtAmount.Text = saleList[6].Value;
            txtMakingDisc.Text = saleList[7].Value;
            txtMakingAmount.Text = saleList[8].Value;
            txtTotalAmount.Text = saleList[9].Value;

            txtTotalWeight.Text = string.Empty;
            txtPurity.Text = string.Empty;
            txtLossPct.Text = string.Empty;
            txtLossWeight.Text = string.Empty;
            txtExpectedQuantity.Text = string.Empty;

            // Added for wastage
            cmbWastage.SelectedIndex = Convert.ToInt32(saleList[18].Value);
            txtWastageQty.Text = saleList[19].Value;
            txtWastageAmount.Text = saleList[20].Value;

            txtWastagePercentage.Text = saleList[21].Value;
            // wastage rate
            //
            // Making Discount Type
            cmbMakingDiscType.SelectedIndex = Convert.ToInt32(saleList[23].Value);
            txtMakingDiscTotAmt.Text = saleList[24].Value;

            //

        }

        private void BindPurchaseControls()
        {
            txtPCS.Text = purchaseList[0].Value;
            txtQuantity.Text = purchaseList[1].Value;
            txtRate.Text = purchaseList[2].Value;
            //   cmbRateType.SelectedIndex = cmbRateType.FindStringExact(Convert.ToString(purchaseList[3].Value));
            cmbRateType.SelectedIndex = Convert.ToInt16(purchaseList[3].Value);
            txtTotalWeight.Text = purchaseList[10].Value;
            txtLossPct.Text = Convert.ToString(purchaseList[11].Value);
            txtAmount.Text = purchaseList[6].Value;
            txtLossWeight.Text = purchaseList[12].Value;
            txtExpectedQuantity.Text = purchaseList[13].Value;
            txtPurity.Text = purchaseList[26].Value;

            txtChangedMakingRate.Text = string.Empty;
            txtActMakingRate.Text = string.Empty;
            cmbMakingType.SelectedIndex = 0;
            txtMakingDisc.Text = string.Empty;
            txtMakingAmount.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            txtActToAmount.Text = txtTotalAmount.Text;
            // Added for wastage 
            cmbWastage.SelectedIndex = 0;
            txtWastageQty.Text = "0";
            txtWastageAmount.Text = "0";
            txtWastagePercentage.Text = "";
            //

            // Making Discount Type
            cmbMakingDiscType.SelectedIndex = 0;
            txtMakingDiscTotAmt.Text = "0";

            //
        }
        #endregion

        #region Enable Disable Buttons
        private void EnablePurchaseExchangeButtons()
        {
            txtPCS.Enabled = false;
            if (iOWNDMD == 1 || iOWNOG == 1)  //if(chkOwn.Checked)
            {
                txtTotalWeight.Enabled = false;
                txtPurity.Enabled = false;
                txtLossPct.Enabled = false;
            }
            else
            {
                txtTotalWeight.Enabled = true;
                txtPurity.Enabled = true;
                txtLossPct.Enabled = true;
            }

            txtLossPct.Enabled = false;
            txtRate.Enabled = false;
            cmbRateType.Enabled = false;

            txtPCS.Enabled = false;
            txtQuantity.Enabled = false;
            txtChangedMakingRate.Enabled = false;
            txtMakingDisc.Enabled = false;
            cmbMakingType.Enabled = false;

        }

        private void EnableSaleButtons() //if (IsRetailItem(sBaseItemID)
        {
            if (IsCatchWtItem(ItemID))
            {
                txtPCS.Enabled = true;
                txtPCS.Text = "1";
                txtPCS.Focus();
            }
            else
            {
                txtPCS.Text = "";
                txtQuantity.Focus();
                txtPCS.Enabled = false;
            }
            //txtPCS.Enabled = true;
            txtQuantity.Enabled = true;
            txtRate.Enabled = true;
            txtChangedMakingRate.Enabled = true;
            //txtMakingDisc.Enabled = true;
            cmbRateType.Enabled = true;
            cmbMakingType.Enabled = true;

            txtTotalWeight.Enabled = false;
            txtPurity.Enabled = false;

            txtLossPct.Enabled = false;
        }
        #endregion

        #region Clear Controls
        private void ClearControls()
        {
            //rdbSale.Checked = true;
            txtPCS.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtRate.Text = string.Empty;
            cmbRateType.SelectedIndex = 0;
            txtChangedMakingRate.Text = string.Empty;
            txtActMakingRate.Text = string.Empty;
            cmbMakingType.SelectedIndex = 0;
            txtAmount.Text = string.Empty;
            txtMakingDisc.Text = string.Empty; // Making Discount
            txtTotalWeight.Text = string.Empty;
            txtPurity.Text = string.Empty;

            txtLossPct.Text = string.Empty;
            txtLossWeight.Text = string.Empty;
            txtExpectedQuantity.Text = string.Empty;
            txtMakingAmount.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            txtActToAmount.Text = txtTotalAmount.Text;
            // Making Discount Type
            txtMakingDiscTotAmt.Text = string.Empty;
            cmbMakingDiscType.SelectedIndex = 0;

            // Added for wastage
            cmbWastage.SelectedIndex = 0;
            txtWastageQty.Text = "0";
            txtWastageAmount.Text = "0";
            txtWastagePercentage.Text = "";
            //
        }
        #endregion

        #region Text Loss Weight Leave
        private void txtLossWeight_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()) && !string.IsNullOrEmpty(txtLossWeight.Text.Trim()))
            {
                decimal dLossWt = 0m;
                dLossWt = Convert.ToDecimal(txtTotalWeight.Text.Trim()) - Convert.ToDecimal(txtExpectedQuantity.Text.Trim());//txtQuantity
                txtLossWeight.Text = Convert.ToString(dLossWt);
                cmbRateType_SelectedIndexChanged(sender, e);
            }
        }
        #endregion

        #region txtRate_Leave

        #endregion

        #region txtQuantity_Leave
        private void txtQuantity_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text))
            {
                if (IsBulkItem(sBaseItemID))
                {
                    decimal dAdjAmt = 0;
                    decimal dCtoAdvAdjAmt = 0;
                    decimal dAdjQty = 0;

                    foreach (SaleLineItem SLItem in retailTrans.SaleItems)
                    {
                        //if (SLItem.ItemId == AdjustmentItemID())//if clause added for GSS and Adv @ a time
                        //{
                        #region Adv Adjst
                        if (IsSaleAdjustment && SLItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                        {
                            dPrevRunningTransAdjGoldQty = 0;
                            dSaleAdjustmentGoldAmt = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentGoldAmt);
                            dSaleAdjustmentGoldQty = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentGoldQty);
                            dSaleAdjustmentMetalRate = Convert.ToDecimal(SLItem.PartnerData.SaleAdjustmentMetalRate);

                            sAdjuOrderNo = Convert.ToString(SLItem.PartnerData.SaleAdjustmentOrderNo);

                            //if (!string.IsNullOrEmpty(sAdjuOrderNo))   // Ask Ripan Da 12-08-2019
                            //    break;

                            //decimal dConvertedToFixingQty= getConvertedRate();
                            string sCurrentRate = GetPureMetalRate();
                            // decimal dPureVal = dSaleAdjustmentGoldQty * Convert.ToDecimal(sCurrentRate);
                            string sTransId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentTransactionID);
                            string sStoreId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentStoreId);
                            string sTerminalId = Convert.ToString(SLItem.PartnerData.ServiceItemCashAdjustmentTerminalId);

                            if (IsAdvanceAgainstNone(sTransId, sStoreId, sTerminalId))
                            {
                                if (dSaleAdjustmentMetalRate > Convert.ToDecimal(sCurrentRate))
                                    dAdjAmt += Convert.ToDecimal(sCurrentRate) * dSaleAdjustmentGoldQty;
                                else
                                    dAdjAmt += dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty;
                            }
                            else
                                //dAdjAmt += dAdjAmt += Convert.ToDecimal(dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty);//dSaleAdjustmentGoldAmt  \\Ask Ripan Da

                               //==================================Soutik======================================================
                               dAdjAmt += decimal.Round(Convert.ToDecimal(dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty), 2, MidpointRounding.AwayFromZero);

                            dCtoAdvAdjAmt += dSaleAdjustmentMetalRate * dSaleAdjustmentGoldQty; //dCAdvAdjustmentAvgGoldRate

                            dAdjQty += dSaleAdjustmentGoldQty;
                        }
                        #endregion

                        #region Sales Exchange Adjust dev on 12012018
                        if (IsSaleAdjustment && SLItem.ReturnLineId != 0)
                        {
                            dSaleAdjustmentGoldQty = Convert.ToDecimal(SLItem.PartnerData.CalExchangeQty);

                            if (dSaleAdjustmentGoldQty > 0)
                            {
                                dAdjAmt += Math.Abs(Convert.ToDecimal(SLItem.NetAmountWithNoTax));

                                dCtoAdvAdjAmt += Math.Abs(Convert.ToDecimal(SLItem.NetAmountWithNoTax));

                                dAdjQty += dSaleAdjustmentGoldQty;
                            }
                        }
                        #endregion
                        //}
                    }

                    if (IsSaleAdjustment)
                    {
                        if ((string.IsNullOrEmpty(sAdjuOrderNo)) || (!string.IsNullOrEmpty(sAdjuOrderNo)))
                        {
                            if (dAdjQty > 0)
                            {
                                dSaleAdjustmentGoldQty = dAdjQty;
                                dSaleAdjustmentAvgGoldRate = (dAdjAmt / dAdjQty);
                                dCAdvAdjustmentAvgGoldRate = (dCtoAdvAdjAmt / dAdjQty);
                            }
                            else
                            {
                                dSaleAdjustmentGoldQty = 0;
                                dSaleAdjustmentAvgGoldRate = 0;
                                dCAdvAdjustmentAvgGoldRate = 0;
                            }

                            retailTrans.PartnerData.dSaleAdjustmentAvgGoldRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                            retailTrans.PartnerData.dCAdvAdjustmentAvgGoldRate = Convert.ToString(dCAdvAdjustmentAvgGoldRate);
                            dPrevTransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty);
                        }
                        else
                        {
                            dSaleAdjustmentAvgGoldRate = dSaleAdjustmentMetalRate;
                            dCAdvAdjustmentAvgGoldRate = dSaleAdjustmentMetalRate;
                            retailTrans.PartnerData.dSaleAdjustmentAvgGoldRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                            retailTrans.PartnerData.dCAdvAdjustmentAvgGoldRate = Convert.ToString(dCAdvAdjustmentAvgGoldRate);
                        }
                    }
                }

                if (rdbSale.Checked)
                {
                    if (!bValidSKU && !string.IsNullOrEmpty(lblCustOrder.Text) && !string.IsNullOrEmpty(sBaseItemID))
                    {
                        //if (IsBulkItem(sBaseItemID) && !string.IsNullOrEmpty(lblCustOrder.Text))
                        //{
                        //    if (!bValidSKU && !string.IsNullOrEmpty(lblCustOrder.Text) && !string.IsNullOrEmpty(sBaseItemID))
                        //    {
                        //        string sBIRate = "0";
                        //        string sBIQty = "";
                        //        string sBlank = "";
                        //        string sqlString = "SELECT CRate,QTY,ORDERNUM FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + lblCustOrder.Text + "'" +
                        //          " AND ITEMID='" + sBaseItemID + "' AND LINENUM=" + OlineNum + "";

                        //        NIM_ReturnMoreValues(sqlString, out sBIRate, out sBIQty, out sBlank);
                        //        txtRate.Text = sBIRate;
                        //        //txtQuantity.Text = sBIQty;
                        //        lblMetalRatesShow.Text = sBIRate;
                        //        txtQuantity.Focus();
                        //        txtWtDiffDiscQty.Focus();
                        //    }
                        //}
                        txtQuantity.Enabled = false;
                        CheckRateFromDB();

                        NewMakingDiscCal();
                    }
                    else
                    {
                        txtQuantity.Enabled = true;
                        CheckRateFromDB();

                        NewMakingDiscCal();
                    }
                }
                int iMType = GetMetalType(sBaseItemID);

                if (IsBulkItem(sBaseItemID) && iMType != (int)MetalType.Gold)
                {
                    txtRate.Enabled = true;
                    txtRate.BackColor = SystemColors.Window;
                    txtRate.Focus();
                    txtQuantity.Enabled = true;
                    txtQuantity.BackColor = SystemColors.Window;
                }
                else
                {
                    txtRate.Enabled = false;
                    txtQuantity.Enabled = false;
                    txtQuantity.BackColor = SystemColors.Control;
                }

                txtAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;


                if (isMRPUCP)
                    txtActToAmount.Text = !string.IsNullOrEmpty(txtAmount.Text) ? decimal.Round(Convert.ToDecimal(txtAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                else
                    txtActToAmount.Text = txtTotalAmount.Text;

                if (string.IsNullOrEmpty(txtChangedMakingRate.Text))
                {
                    txtChangedMakingRate.Text = "0";
                    txtActMakingRate.Text = "0";
                }

                cmbMakingType.Enabled = false;
                cmbRateType.Enabled = false;
                cmbMakingDiscType.Enabled = false;
            }
        }

        private void NewMakingDiscCal()
        {
            decimal dinitDiscValue = 0;

            decimal dQty = 0;
            if (!string.IsNullOrEmpty(txtQuantity.Text))
                dQty = Convert.ToDecimal(txtQuantity.Text);
            else
                dQty = 0;

            if (!isMRPUCP)
                dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "OPENINGDISCPCT");// get OPENINGDISCPCT field value FOR THE OPENING

            decimal dMkPerDisc = 0;
            if (!string.IsNullOrEmpty(txtMakingDisc.Text))
                dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
            else
                dMkPerDisc = 0;

            if (dinitDiscValue >= 0)
            {
                if ((dMkPerDisc > dinitDiscValue))
                {
                    MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                }
                else
                    CheckMakingDiscountFromDB(dinitDiscValue);
            }
        }

        private void CheckRateFromDB(bool IsFreezedRate = false)
        {
            string sWtRange = string.Empty;
            decimal dTotWt = 0m;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            //if(IsSaleAdjustment   // Avg Gold Rate Adjustment
            //                        && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold)
            //                        && (dSaleAdjustmentAvgGoldRate > 0))
            //{
            //    sRate = Convert.ToString(getAdjustmentRate(GrWeight, ConfigID, dTotWt));

            //    sAdvAdjustmentGoldRate = sRate;
            //}

            // Quantity calculation //// NOT Consider Multi Metal in Ingredient [Order by Metaltype in Ingredient]
            if (dtIngredients != null && dtIngredients.Rows.Count > 0)
            {
                int iIngMetalType;

                if (bIsGrossMetalCal)
                {
                    foreach (DataRow dr in dtIngredients.Rows)
                    {
                        dTotWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                    }
                    sWtRange = Convert.ToString(dTotWt);
                }
                else
                {
                    iIngMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    if ((iIngMetalType == (int)MetalType.Gold) || (iIngMetalType == (int)MetalType.Silver)
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

                           " ORDER BY RETAIL_SALES_AGREEMENT_DETAIL.ITEMID DESC,RETAIL_SALES_AGREEMENT_DETAIL.COMPLEXITY_CODE DESC," +
                           " RETAIL_SALES_AGREEMENT_DETAIL.ARTICLE_CODE DESC" +
                           " ,RETAIL_SALES_AGREEMENT_DETAIL.FROMDATE DESC,RETAIL_SALES_AGREEMENT_DETAIL.INVENTLOCATIONID desc";
            #endregion

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sMkType = "0";
            decimal dWastQty = 0m;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        isMkAdded = false;

                        if (Convert.ToDecimal(retailTrans.PartnerData.FlatMkDiscPct) > 0)
                        {
                            // txtActMakingRate.Text = Convert.ToString(retailTrans.PartnerData.FlatMkDiscPct);
                            txtChangedMakingRate.Text = Convert.ToString(retailTrans.PartnerData.FlatMkDiscPct);
                            txtActMakingRate.Text = Convert.ToString(retailTrans.PartnerData.FlatMkDiscPct);

                            sMkType = "4";//Percentage
                        }
                        else
                        {
                            txtChangedMakingRate.Text = Convert.ToString(reader.GetValue(0));
                            txtActMakingRate.Text = Convert.ToString(reader.GetValue(0));
                            sMkType = Convert.ToString(reader.GetValue(1));
                        }
                        // txtManualAmount.Enabled = true;
                        switch (sMkType)
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
                            //case "5":
                            //    cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Inch));
                            //    break;
                            case "6":
                                cmbMakingType.SelectedIndex = cmbMakingType.FindStringExact(Convert.ToString(MakingType.Gross));
                                break;
                            default:
                                cmbMakingType.SelectedIndex = 2;
                                break;
                        }

                        CalcMakingAtSelectedIndexChange();

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

                        //
                    }
                }
                else
                {
                    reader.Close();
                    reader.Dispose();

                    if (!isViewing)
                        txtChangedTotAmount.Enabled = false;
                }

                reader.Close();
                reader.Dispose();

            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

            // --------- check MFG_CODE AND SALE_COMPLEXITYCODE  against the item if Making Rate is 0 // 30.08.2013

            if (string.IsNullOrEmpty(txtChangedMakingRate.Text))
            {
                txtChangedMakingRate.Text = "0";
                txtActMakingRate.Text = "0";
            }

            if ((Convert.ToDecimal(txtChangedMakingRate.Text) <= 0) && (dWastQty <= 0))
            {
                // get sku -- Metal Type
                int iSKUMetaltype = GetMetalType(sBaseItemID);

                if (iSKUMetaltype == (int)MetalType.Gold
                    || iSKUMetaltype == (int)MetalType.Silver
                    || iSKUMetaltype == (int)MetalType.Platinum
                    || iSKUMetaltype == (int)MetalType.Palladium)
                {

                    if (!IsValidMakingRate(ItemID.Trim()))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("0 Making Rate is not valid for this item", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                        }
                        isCancelClick = true;
                        this.Close();
                        return;
                    }
                }

            }

            //----------------------
            #region [ Wastage Amount Calculation] // for wastage  // NOT Consider Multi Metal in Ingredient
            // if (!string.IsNullOrEmpty(txtRate.Text.Trim()) && cmbWastage.SelectedIndex == 0 && !string.IsNullOrEmpty(txtWastageQty.Text.Trim()))
            if (cmbWastage.SelectedIndex == 0 && dWastQty > 0)
            {
                // if (Convert.ToDecimal(txtWastageQty.Text) > 0)
                //{
                Decimal dAmount = 0m;
                int iMetalType = 0;
                string sRate = "";

                // enum MetalType  ---> Gold = 1,

                //  iMetalType = GetMetalType(ItemID);
                //  iMetalType = GetMetalType(sBaseItemID);

                if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    iMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);

                }
                else
                {
                    iMetalType = GetMetalType(sBaseItemID);
                }

                if (iMetalType == (int)MetalType.Gold
                    || iMetalType == (int)MetalType.Silver
                    || iMetalType == (int)MetalType.Palladium
                    || iMetalType == (int)MetalType.Platinum
                    )
                {
                    if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                    {
                        if (IsFreezedRate)
                            sRate = Convert.ToString(dCustomerOrderFreezedRate);
                        else
                            sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                                                        , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                    }
                    else
                    {
                        sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                    }

                    if (!string.IsNullOrEmpty(sRate))
                        dWMetalRate = Convert.ToDecimal(sRate);

                    if (dWMetalRate > 0)
                    {
                        if (bIsGrossMetalCal)
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

            else if (cmbWastage.SelectedIndex == 1 && dWastQty > 0)
            {
                // Calculate wastage Qty  -- consider Ingredient

                if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    Decimal dIngrdTotQty = 0m;
                    Decimal dWastageQty = 0m;
                    Decimal decimalAmount = 0m;

                    int iMetalType = 100;

                    int iIngrdMType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    // iMetalType = Convert.ToInt32(dtIngredients.Rows[0]["METALTYPE"]);
                    if (iIngrdMType == (int)MetalType.Gold
                        || iIngrdMType == (int)MetalType.Silver
                        || iIngrdMType == (int)MetalType.Palladium
                        || iIngrdMType == (int)MetalType.Platinum)
                    {
                        iMetalType = iIngrdMType;
                    }

                    foreach (DataRow dr in dtIngredients.Rows)
                    {
                        if (iMetalType == Convert.ToInt32(dr["METALTYPE"]))
                        {
                            if (Convert.ToString(dr["Qty"]) != "")
                            {
                                dIngrdTotQty += Convert.ToDecimal(dr["Qty"]);
                            }
                        }
                    }
                    if (dIngrdTotQty > 0) //dWastQty
                    {
                        string sRate = "";
                        if (IsFreezedRate)
                            sRate = Convert.ToString(dCustomerOrderFreezedRate);
                        else
                        {
                            if ((IsSaleAdjustment) && (!string.IsNullOrEmpty(sAdvAdjustmentGoldRate))) //14.01.2014
                                sRate = sAdvAdjustmentGoldRate;
                            else if ((IsGSSTransaction) && (!string.IsNullOrEmpty(sGSSAdjustmentGoldRate))) //14.01.2014
                                sRate = sGSSAdjustmentGoldRate;
                            else
                                sRate = getWastageMetalRate(Convert.ToString(dtIngredients.Rows[0]["ITEMID"])
                                                            , Convert.ToString(dtIngredients.Rows[0]["ConfigID"]));
                        }

                        if (!string.IsNullOrEmpty(sRate))
                            dWMetalRate = Convert.ToDecimal(sRate);

                        if (dWMetalRate > 0)
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
                    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        if (Convert.ToDecimal(txtQuantity.Text) > 0)
                        {
                            Decimal dWastageQty = 0m;
                            Decimal decimalAmount = 0m;
                            int iMetalType;
                            string sRate = "";

                            //    iMetalType = GetMetalType(ItemID);
                            iMetalType = GetMetalType(sBaseItemID);

                            if (iMetalType == (int)MetalType.Gold)
                            {
                                if ((IsSaleAdjustment) && (!string.IsNullOrEmpty(sAdvAdjustmentGoldRate))) //14.01.2014
                                    sRate = sAdvAdjustmentGoldRate;
                                else if ((IsGSSTransaction) && (!string.IsNullOrEmpty(sGSSAdjustmentGoldRate))) //14.01.2014
                                    sRate = sGSSAdjustmentGoldRate;
                                else
                                    sRate = getWastageMetalRate(sBaseItemID, sBaseConfigID);
                                if (!string.IsNullOrEmpty(sRate))
                                    dWMetalRate = Convert.ToDecimal(sRate);

                                if (dWMetalRate > 0)
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
            }
            //if (cmbWastage.SelectedIndex == 1 && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtWastageQty.Text.Trim())))
            else
            {
                if (IsBulkItem(sBaseItemID))
                {
                    if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        if (Convert.ToDecimal(txtQuantity.Text) > 0)
                        {
                            Decimal dWastageQty = 0m;
                            Decimal decimalAmount = 0m;
                            int iMetalType;
                            string sRate = "";

                            //    iMetalType = GetMetalType(ItemID);
                            iMetalType = GetMetalType(sBaseItemID);

                            if (iMetalType == (int)MetalType.Gold
                                || iMetalType == (int)MetalType.Silver
                                || iMetalType == (int)MetalType.Platinum)
                            {
                                if (IsSaleAdjustment)
                                {
                                    if (!bValidSKU && !string.IsNullOrEmpty(lblCustOrder.Text) && !string.IsNullOrEmpty(sBaseItemID))
                                    {
                                        if (!string.IsNullOrEmpty(txtQuantity.Text))
                                        {
                                            decimal dMetalRate = 0;

                                            dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));//6
                                            if (dMetalRate > 0)
                                            {
                                                txtRate.Text = Convert.ToString(dMetalRate);
                                                lblMetalRatesShow.Text = txtRate.Text;
                                            }
                                            else
                                            {
                                                txtRate.Text = getRateFromMetalTable();
                                                lblMetalRatesShow.Text = txtRate.Text;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(txtQuantity.Text))
                                        {
                                            decimal dMetalRate = 0;

                                            dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));//6
                                            if (dMetalRate > 0)
                                            {
                                                txtRate.Text = Convert.ToString(dMetalRate);
                                                lblMetalRatesShow.Text = txtRate.Text;
                                            }
                                            else
                                            {
                                                txtRate.Text = getRateFromMetalTable();
                                                lblMetalRatesShow.Text = txtRate.Text;
                                            }
                                        }
                                    }
                                }
                                else if (!string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum)))
                                {
                                    if (!string.IsNullOrEmpty(txtQuantity.Text))
                                    {
                                        decimal dMetalRate = 0;

                                        dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));//6
                                        if (dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                            lblMetalRatesShow.Text = txtRate.Text;
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                            lblMetalRatesShow.Text = txtRate.Text;
                                        }
                                    }

                                }
                                if (dWMetalRate > 0)
                                {
                                    txtRate.Text = Convert.ToString(decimal.Round(Convert.ToDecimal(dWMetalRate), 2, MidpointRounding.AwayFromZero));
                                    lblMetalRatesShow.Text = txtRate.Text;
                                }
                            }
                        }
                    }
                }

                txtWastagePercentage.Text = string.Empty;
                txtWastageQty.Text = string.Empty;
                txtWastageAmount.Text = string.Empty;
            }
            #endregion
        }
        #endregion

        #region Check Box Checked Changed
        private void chkOwn_CheckedChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region  GET RATE FROM METAL TABLE
        private string getRateFromMetalTable()
        {

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @CUSTCLASSCODE VARCHAR(20)");
            commandText.Append(" DECLARE @METALTYPE INT  DECLARE @PARENTITEM VARCHAR(20)");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");
            //added on 09/02/16
            commandText.Append(" SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' ");

            if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
                commandText.Append(" BEGIN ");
            }

            commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            if (rdbSale.Checked)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
                chkOwn.Enabled = true;

            }
            else if (rdbExchange.Checked || rdbExchangeReturn.Checked)
            {
                if (iOWNDMD == 1 || iOWNOG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.Exchange + "' ");
                else if (iOTHERDMD == 1 || iOTHEROG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OtherExchange + "' ");
            }
            else
            {

                if (iOWNDMD == 1 || iOWNOG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
                else if (iOTHERDMD == 1 || iOTHEROG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");
            }

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + ConfigID.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");



            string grweigh = string.Empty;
            if (string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            {
                //commandText.Append(" END ");
                //commandText.Append(" ELSE ");
                //commandText.Append(" BEGIN ");
                //commandText.Append(" SELECT  CAST(UNIT_RATE AS decimal (6,2)) FROM RETAILCUSTOMERSTONEAGGREEMENTDETAIL WHERE  ");
                //commandText.Append(" WAREHOUSE=@INVENTLOCATION AND ('" + grweigh + "' BETWEEN FROM_WEIGHT AND TO_WEIGHT)  ");
                //commandText.Append(" AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE) ");
                //commandText.Append(" AND ITEMID='" + ItemID.Trim() + "' AND INVENTBATCHID='" + BatchID.Trim() + "' ");
                //commandText.Append(" AND INVENTSIZEID='" + SizeID.Trim() + "' AND INVENTCOLORID='" + ColorID.Trim() + "' ");
                //commandText.Append(" END ");

                commandText.Append(" END ");
                commandText.Append(" ELSE ");
                commandText.Append(" BEGIN ");
                commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
                commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
                commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
                //commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
                commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION or RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE='') AND (  ");
                commandText.Append(dStoneWtRange);
                commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
                commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
                commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
                //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
                commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
                //    commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') "); // 21.01.2014 // REMOVE TRIM
                commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

                commandText.Append(" AND(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='" + sCustomerId.Trim() + "' ");
                commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='') AND ");
                commandText.Append(" (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId=@CUSTCLASSCODE ");
                commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId='')");


                //commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
                commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // added on 02.09.2013
                // commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC"); // Added on 29.05.2013
                commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
                commandText.Append(" END ");
            }

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (string.IsNullOrEmpty(sResult))
                sResult = "0";

            return Convert.ToString(sResult.Trim());

        }

        private string getRateFromMetalTable(int iForNonIngredientsItem = 0)
        {

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @CUSTCLASSCODE VARCHAR(20)");
            commandText.Append(" DECLARE @METALTYPE INT  DECLARE @PARENTITEM VARCHAR(20)");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");
            //added on 09/02/16
            commandText.Append(" SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' ");

            string grweigh = string.Empty;
            if (string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            if (!string.IsNullOrEmpty(txtQuantity.Text))
                dStoneWtRange = Convert.ToDecimal(txtQuantity.Text);


            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Stone + "','" + (int)MetalType.LooseDmd + "')) ");
            commandText.Append(" BEGIN ");

            commandText.Append("  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + ItemID + "')" + // AND RETAIL=1 req by S.Sharma on 15/02/17
                                      "  BEGIN " +
                                          " SELECT @PARENTITEM = ITEMIDPARENT" +
                                          " FROM [INVENTTABLE] WHERE ITEMID = '" + ItemID + "' " +
                                      " END " +
                                 " ELSE" +
                                      " SET @PARENTITEM='" + ItemID + "' ");

            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2)) ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            //commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION or RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE='') AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");

            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID =@PARENTITEM)  AND  ");

            //    commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') "); // 21.01.2014 // REMOVE TRIM
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

            commandText.Append(" AND(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='" + sCustomerId.Trim() + "' ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='') AND ");
            commandText.Append(" (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId=@CUSTCLASSCODE ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId='')");


            //commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // added on 02.09.2013
            // commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC"); // Added on 29.05.2013
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (string.IsNullOrEmpty(sResult))
                sResult = "0";

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        #region  Get Ingredient Calculation Type

        // private string GetIngredientCalcType()  
        private string GetIngredientCalcType(ref int iSDisctype, ref decimal dSDiscAmt)  // modified on 30.04.2013 // Stone Discount
        {
            string sResult = string.Empty;
            StringBuilder commandText = new StringBuilder();

            string grweigh = string.Empty;
            if (string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            // if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            // {
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @CUSTCLASSCODE VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");
            //added on 09/02/16
            commandText.Append(" SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            // commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
            commandText.Append(" SELECT   TOP 1  ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE,ISNULL(DISCTYPE,0) AS DISCTYPE, ISNULL(DISCAMT,0) AS DISCAMT ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            //  commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION or RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE='') AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            // commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') "); //21.01.2014
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");

            commandText.Append(" AND(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='" + sCustomerId.Trim() + "' ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='') AND ");
            commandText.Append(" (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId=@CUSTCLASSCODE ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId='')");

            //commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')"); 
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013
            // commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC"); // Added on 29.05.2013
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");
            // }



            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sResult = Convert.ToString(reader.GetValue(0));
                        iSDisctype = Convert.ToInt32(reader.GetValue(1));
                        dSDiscAmt = Convert.ToDecimal(reader.GetValue(2));
                    }
                }
            }
            //string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        #region Ingredient Details Click
        private void btnIngrdientsDetails_Click(object sender, EventArgs e)
        {
            frmSKUDetails oSKUDetails = new frmSKUDetails(dtIngredientsClone);
            oSKUDetails.ShowDialog();
        }
        #endregion

        #region Select Item
        private void btnSelectItems_Click(object sender, EventArgs e)
        {
            if (isViewing)
            {
                SelectItems(LineNum, Convert.ToString(cmbCustomerOrder.SelectedValue));

                CustomerOrderDetails oCustOrder = new CustomerOrderDetails(dsCustOrder, string.Empty);
                oCustOrder.ShowDialog();
            }
            else
            {
                SelectItems("", Convert.ToString(cmbCustomerOrder.SelectedValue));

                CustomerOrderDetails oCustOrder = new CustomerOrderDetails(dsCustOrder, index);
                oCustOrder.ShowDialog();
                index = oCustOrder.index;
                if (oCustOrder.lineId == 0)
                {
                    OlineNum = oCustOrder.lineId;
                    // lblItemSelected.Text = "NO ITEM SELECTED";
                    // Freezed Metal Rate New

                    BindIngredientGrid();
                    if (!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                    {
                        CheckRateFromDB();
                        //CheckMakingDiscountFromDB();
                    }
                }
                else
                {
                    OlineNum = oCustOrder.lineId;
                    lblItemSelected.Text = " SELECTED LINE NO. : " + oCustOrder.lineId;
                    // Fixed Metal Rate New
                    decimal dFreezedMetalRateVal = 0m;
                    string sFreezedMetalRateConfigId = string.Empty;
                    decimal dQty = 0m;



                    if (!IsGSSTransaction)
                    {
                        //if (Convert.ToBoolean(GetCustOrderRateFreezeInfo(Convert.ToString(cmbCustomerOrder.SelectedValue))))// changed parameter @same function(Overloading for new requirement ) 01/10/2015 Req by S.Sharma
                        //{
                        // -- give pop up window for freezed rate yes / no -- if yes --> then
                        // -- give metal rate value in Message Box
                        int iAdvAgainst = GetCustOrderRateFreezeAdvAgainstInfo(Convert.ToString(cmbCustomerOrder.SelectedValue));

                        DataRow[] dr = dsCustOrder.Tables[1].Select("ORDERNUM='" + Convert.ToString(cmbCustomerOrder.SelectedValue) + "' and ORDERDETAILNUM=" + oCustOrder.lineId + "");// and LINENUM=" + oCustOrder.lineId + "
                        DataRow[] dr1 = dsCustOrder.Tables[0].Select("ORDERNUM='" + Convert.ToString(cmbCustomerOrder.SelectedValue) + "' and  LINENUM=" + oCustOrder.lineId + "");// and LINENUM=" + oCustOrder.lineId + "

                        int isIngre = 0;
                        if (dr.Length > 0)
                            isIngre = 1;


                        if (dr1.Length > 0)
                            dr = dr1;

                        if (dr.Length > 0)
                        {
                            DataTable dt = new DataTable();
                            dt = dsCustOrder.Tables[1].Clone();
                            foreach (DataRow row in dr)
                            {
                                dt.ImportRow(row);
                            }
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                foreach (DataRow row in dt.Rows)
                                {
                                    dFreezedMetalRateVal = Convert.ToDecimal(row["RATE"]);
                                    sFreezedMetalRateConfigId = Convert.ToString(row["CONFIGID"]);
                                    if (!string.IsNullOrEmpty(sFreezedMetalRateConfigId))
                                    {
                                        if (ConfigID == sFreezedMetalRateConfigId)
                                        {
                                            if (!string.IsNullOrEmpty(sFreezedMetalRateConfigId))
                                            {
                                                if (isIngre == 1)
                                                {
                                                    BindIngredientGrid(Convert.ToString(dFreezedMetalRateVal), sFreezedMetalRateConfigId, iAdvAgainst);

                                                    if (!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                                                    {
                                                        CheckRateFromDB(true);
                                                    }
                                                }
                                                else
                                                {
                                                    txtQuantity.Text = Convert.ToString(row["QTY"]);
                                                    txtQuantity.Focus();
                                                    txtWtDiffDiscQty.Focus();
                                                    txtRate.Text = Convert.ToString(row["RATE"]);
                                                    lblMetalRatesShow.Text = Convert.ToString(row["RATE"]);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Invalid Configuration");
                                        }
                                    }
                                }
                            }

                        }
                        //}
                    }

                    //--- End
                }
            }
        }
        #endregion

        #region Common Helper
        private void CommonHelper()
        {
            //chkOwn.Visible = true;
            txtRate.Text = string.Empty;
            lblMetalRatesShow.Text = txtRate.Text;
            txtRate.Text = getRateFromMetalTable();

            btnIngrdientsDetails.Visible = false;
            dtIngredients = new DataTable();
            dtIngredientsClone = new DataTable();
            panel4.Enabled = false;
            cmbCustomerOrder.DataSource = null;
            cmbCustomerOrder.Items.Add("NO CUSTOMER ORDER");
            cmbCustomerOrder.SelectedIndex = 0;
            btnSelectItems.Visible = false;

            cmbCustomerOrder.Enabled = false;
            lblItemSelected.Text = string.Empty;
        }
        #endregion

        #region Selected Index Change
        private void cmbCustomerOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (iCallfrombase == 0)
            {
                if (!string.IsNullOrEmpty(txtQuantity.Text) && !isMRPUCP)
                {
                    if (!isViewing)
                        CheckRateFromDB();
                }
            }

            if (Convert.ToString(cmbCustomerOrder.SelectedValue).ToUpper().Trim() == "NO SELECTION")
            {

                lblItemSelected.Text = string.Empty;
                btnSelectItems.Visible = false;

                return;
            }
            else
            {
                lblItemSelected.Text = string.Empty;
                btnSelectItems.Visible = false;
            }
        }
        #endregion

        #region Select Items
        private void SelectItems(string linenum, string sOrderNo = null)
        {
            dsCustOrder = new DataSet();
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            string commandText = string.Empty;
            if (isViewing)
            {
                commandText = " SELECT [ORDERNUM],CAST(LINENUM AS INT) LINENUM,[STOREID],[TERMINALID],[ITEMID] " +
                                      " ,[CONFIGID],[CODE],[SIZEID] ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE] " +
                                      " ,[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT] " +
                                      " ,[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[CREATEDON],[STAFFID] " +
                                      " FROM [CUSTORDER_DETAILS] " +
                                      " WHERE [ORDERNUM]='" + sOrderNo + "' AND  [LINENUM]=" + linenum + ";" +
                                      " SELECT  [ORDERNUM],CAST(ORDERDETAILNUM AS INT) [ORDERDETAILNUM],CAST(LINENUM AS INT) [LINENUM],[STOREID] " +
                                      " ,[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID] " +
                                      " ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE],[DATAAREAID] " +
                                      " ,[AMOUNT] FROM [CUSTORDER_SUBDETAILS] " +
                                      " WHERE [ORDERNUM]='" + sOrderNo + "' ;";
            }
            else
            {
                commandText = " SELECT [ORDERNUM], CAST(LINENUM AS INT) LINENUM,[STOREID],[TERMINALID],[ITEMID] " +
                                     " ,[CONFIGID],[CODE],[SIZEID] ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE] " +
                                     " ,[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT] " +
                                     " ,[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[CREATEDON],[STAFFID] " +
                                     " FROM [CUSTORDER_DETAILS] " +
                                     " WHERE [ORDERNUM]='" + sOrderNo + "' AND isDelivered='0';" +
                                     " SELECT  [ORDERNUM],CAST(ORDERDETAILNUM AS INT) [ORDERDETAILNUM],CAST(LINENUM AS INT) [LINENUM],[STOREID] " +
                                     " ,[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID] " +
                                     " ,[STYLE],[PCS],[QTY],[CRATE] AS [RATE],[RATETYPE],[DATAAREAID] " +
                                     " ,[AMOUNT] FROM [CUSTORDER_SUBDETAILS] " +
                                     " WHERE [ORDERNUM]='" + sOrderNo + "' ;";// cmbCustomerOrder.SelectedValue
            }

            //  SqlCommand command = new SqlCommand(commandText, conn);
            //  command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dsCustOrder);

            if (dsCustOrder == null && dsCustOrder.Tables.Count <= 0 && dsCustOrder.Tables[0].Rows.Count <= 0)
            {
                MessageBox.Show("NO ITEM DETAILS PRESENT.", "WARNING ! ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }
        #endregion

        #region Numpad Enter Pressed
        private void numAmount_EnterButtonPressed()
        {
            if (string.IsNullOrEmpty(txtPCS.Text) || sNumPadEntryType.ToUpper().Trim() == "P")
            {
                txtPCS.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                if (rdbSale.Checked)
                {
                    txtQuantity.Focus();
                    numpadEntry.PromptText = "Enter Quantity:";
                }
                else
                {
                    txtRate.Focus();
                    numpadEntry.PromptText = "Enter Rate:";
                }

                numpadEntry.Refresh();
                numpadEntry.Focus();

                return;
            }
            if (rdbSale.Checked)
            {

                if (string.IsNullOrEmpty(txtQuantity.Text) || sNumPadEntryType.ToUpper().Trim() == "Q")
                {
                    txtQuantity.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    txtRate.Focus();
                    numpadEntry.PromptText = "Enter Rate:";
                    numpadEntry.Refresh();
                    numpadEntry.Focus();


                }

                else if (string.IsNullOrEmpty(txtRate.Text) || sNumPadEntryType.ToUpper().Trim() == "R")
                {
                    txtRate.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    txtChangedMakingRate.Focus();
                    numpadEntry.PromptText = "Enter Making Rate:";
                    numpadEntry.Refresh();
                    numpadEntry.Focus();


                }

                else if (string.IsNullOrEmpty(txtChangedMakingRate.Text) || sNumPadEntryType.ToUpper().Trim() == "MR")
                {
                    txtChangedMakingRate.Text = numpadEntry.EnteredValue;
                    txtActMakingRate.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    txtMakingDisc.Focus();
                    numpadEntry.PromptText = "Enter Making Discount:";

                    numpadEntry.Refresh();
                    numpadEntry.Focus();

                }

                else if (string.IsNullOrEmpty(txtMakingDisc.Text) || sNumPadEntryType.ToUpper().Trim() == "MD")
                {
                    txtMakingDisc.Text = numpadEntry.EnteredValue;
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    numpadEntry.PromptText = "Enter Pieces:";
                    sNumPadEntryType = "P";
                    numpadEntry.Refresh();
                    numpadEntry.Focus();
                }

                return;
            }
            else
            {
                if (string.IsNullOrEmpty(txtRate.Text) || sNumPadEntryType.ToUpper().Trim() == "R")
                {
                    txtRate.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "Enter Total Weight:";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    txtTotalWeight.Focus();
                }

                else if (string.IsNullOrEmpty(txtTotalWeight.Text) || sNumPadEntryType.ToUpper().Trim() == "TW")
                {
                    txtTotalWeight.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    txtLossPct.Focus();
                }

                else if (string.IsNullOrEmpty(txtPurity.Text) || sNumPadEntryType.ToUpper().Trim() == "PUR") //
                {
                    txtPurity.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                    txtLossPct.Focus();
                }

                else if (string.IsNullOrEmpty(txtLossPct.Text) || sNumPadEntryType.ToUpper().Trim() == "LP")
                {
                    txtLossPct.Text = numpadEntry.EnteredValue;
                    numpadEntry.PromptText = "Enter Pieces:";
                    numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                    sNumPadEntryType = "P";
                }

                numpadEntry.Refresh();
                numpadEntry.Focus();
                numpadEntry.EnteredValue = string.Empty;
                return;
            }
        }
        #endregion

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

        private void txtTotalWeight_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "TW";
            numpadEntry.PromptText = "Enter Total Weight:";
        }

        private void txtPurity_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "PUR";
            numpadEntry.PromptText = "Enter Purity:";
        }

        private void txtLossPct_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "LP";
            numpadEntry.PromptText = "Enter Loss Percentage:";

        }

        protected int GetMetalType(string sItemId)
        {
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iMetalType = (int)reader.GetValue(0);
                    }
                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return iMetalType;

        }


        private string GetInventDimId(string sSize, string sCode, string sConfig, string sItemId)
        {
            string sDimId = string.Empty;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT INVENTDIMID FROM INVENTDIM WHERE INVENTSIZEID = '" + sSize + "'" +
                                " and INVENTCOLORID = '" + sCode + "'" +
                                " and CONFIGID = '" + sConfig + "'" +
                                " and INVENTLOCATIONID = ''" +
                                " and INVENTBATCHID = ''" +
                                " and INVENTSITEID = ''" +
                                " AND INVENTDIMID IN(SELECT INVENTDIMID FROM INVENTDIMCOMBINATION WHERE ITEMID='" + sItemId + "')";


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            sDimId = Convert.ToString(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sDimId;
        }

        protected int IsNoPriceRequird(string sItemId)
        {
            int iResult = 0;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select isnull(NOPRICEREQUIRED,0) from inventtable where itemid='" + sItemId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iResult = (int)reader.GetValue(0);
                    }
                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return iResult;

        }

        #region  GET Wastage Metal rate
        private string getWastageMetalRate(string sItemId, string sConfigId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");

            //if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            //{
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //}

            commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            if (rdbSale.Checked)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
                // chkOwn.Enabled = true;

            }
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

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

        #region GSS Maturity

        private decimal getMetalRate(string sItemId, string sConfigId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");

            //if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
            //{
            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //}

            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            //  commandText.Append(" AND DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0)<=TRANSDATE AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            if (rdbSale.Checked)
            {
                commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
                // chkOwn.Enabled = true;
            }

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return Convert.ToDecimal(sResult.Trim());
            else
                return 0;
        }

        private decimal GetSKUQty(string sItemId)
        {
            decimal dSKUQty = 0m;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT top 1 QTY FROM SKUTableTrans " + //SKU Table New
                                    " WHERE SKUTableTrans.SkuNumber = '" + ItemID.Trim() + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            dSKUQty = decimal.Round(Convert.ToDecimal(command.ExecuteScalar()), 3, MidpointRounding.AwayFromZero);
            //using(SqlDataReader reader = command.ExecuteReader())
            //{
            //    if(reader.HasRows)
            //    {
            //        dSKUQty = decimal.Round(Convert.ToDecimal(reader.GetValue(0)), 3, MidpointRounding.AwayFromZero);
            //    }
            //}
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dSKUQty;

        }

        private decimal getGSSRate(string GrWeight, string sTransItemConfigID, decimal dTotWt) // TransItem config id
        {
            decimal dConvertedToFixingQty = 0m;
            string sRate = string.Empty;
            string sDefConfigId = GetDefaultConfigId();

            //if (sTransItemConfigID == sDefConfigId)
            //{
            #region GSS
            //decimal dActualSaleQty = 0;
            //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

            // if(!string.IsNullOrEmpty(txtQuantity.Text))
            if (!string.IsNullOrEmpty(GrWeight))
            {

                if (bIsGrossMetalCal)
                    dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(dTotWt)), 3, MidpointRounding.AwayFromZero);
                else
                    dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                if (dPrevRunningQtyGSS == 0)
                {
                    //  dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight);
                    dActualGSSSaleWt = dPrevGSSSaleWt + dConvertedToFixingQty;
                }
                else
                {
                    // dActualGSSSaleWt = dPrevGSSSaleWt + Convert.ToDecimal(GrWeight) + dPrevRunningQtyGSS;
                    dActualGSSSaleWt = dPrevGSSSaleWt + dConvertedToFixingQty + dPrevRunningQtyGSS;
                }
            }

            if (dActualGSSSaleWt <= dGSSMaturityQty)
            {
                //  sRate = Convert.ToString(dGSSAvgRate);
                // decimal dPureVal = dActualGSSSaleWt * dGSSAvgRate;
                decimal dPureVal = dConvertedToFixingQty * dGSSAvgRate;
                sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
            }
            else  //if (dActualGSSSaleWt > dGSSMaturityQty)
            {
                if (dPrevGSSSaleWt >= dGSSMaturityQty)
                {
                    sRate = getRateFromMetalTable();

                    if (bIsGrossMetalCal)
                    {
                        sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate) * dTotWt / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                        // (Convert.ToDecimal(sRate) * dTotWt) / Convert.ToDecimal(GrWeight));
                    }
                }
                else
                {
                    //  if ((dPrevGSSSaleWt + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
                    if (dPrevGSSSaleWt + dPrevRunningQtyGSS >= dGSSMaturityQty)
                    {
                        sRate = getRateFromMetalTable();
                        if (bIsGrossMetalCal)
                        {
                            sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate) * dTotWt / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                            // (Convert.ToDecimal(sRate) * dTotWt) / Convert.ToDecimal(GrWeight));
                        }
                    }
                    else
                    {
                        decimal dAvgRateQty = 0m;
                        decimal dCurrentRateQty = 0m;
                        decimal dCurrentRateConvertedtoTransQty = 0m;
                        decimal dCurrentRate = 0m;

                        // dCurrentRateQty = (dActualGSSSaleWt - dGSSMaturityQty - dPrevRunningQtyGSS);

                        dCurrentRateQty = (dActualGSSSaleWt - dGSSMaturityQty);


                        //dCurrentRateConvertedtoTransQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, dCurrentRateQty), 3, MidpointRounding.AwayFromZero);  // new


                        //  dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

                        dAvgRateQty = dConvertedToFixingQty - dCurrentRateQty;


                        dCurrentRate = getMetalRate(ItemID, ConfigID);

                        if (dGSSAvgRate > 0 && dCurrentRate > 0)
                        {
                            // sRate = Convert.ToString(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight));
                            //sRate = Convert.ToString(decimal.Round(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateConvertedtoTransQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                            sRate = Convert.ToString(decimal.Round(((dGSSAvgRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                        }
                    }
                }

            }

            if (!string.IsNullOrEmpty(sRate))
            {
                //  dPrevRunningQtyGSS += Convert.ToDecimal(GrWeight); 
                dPrevRunningQtyGSS += dConvertedToFixingQty;
                return decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                return 0;
            }

            #endregion
            //}
            //else
            //    return getMetalRate(ItemID, ConfigID);

        }
        #endregion
        private decimal GetConvertedGoldQty_Commited(string sTransConfigid, decimal dQtyToConvert)//, bool istranstofixing
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,10)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);

            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  "); //INVENTPARAMETERS
            commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            commandText.Append(" BEGIN  ");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL((( @FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY ");
            commandText.Append(" END  ");
            commandText.Append(" END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

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
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  "); //INVENTPARAMETERS
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

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

        }

        #region  // Avg Gold Rate Adjustment  calc
        private decimal getAdjustmentRate(string GrWeight, string sTransItemConfigID, decimal dTotWt)
        {
            decimal dConvertedToFixingQty = 0m;
            string sRate = string.Empty;
            string sDefConfigId = GetDefaultConfigId();

            //if (sTransItemConfigID == sDefConfigId)
            //{
            if (IsCustOrderWithAdv(sAdjuOrderNo))
            {
                #region AdjustmentRate
                //decimal dActualSaleQty = 0;
                //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

                // if(!string.IsNullOrEmpty(txtQuantity.Text))
                if (!string.IsNullOrEmpty(GrWeight))
                {
                    //dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                    if (bIsGrossMetalCal)
                        dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(dTotWt)), 3, MidpointRounding.AwayFromZero);
                    else
                        dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                    if (dPrevRunningTransAdjGoldQty == 0)
                    {
                        // dActualTransAdjGoldQty = dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight);
                        dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty;
                    }
                    else
                    {
                        // dActualTransAdjGoldQty = dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight) + dPrevRunningTransAdjGoldQty;
                        dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty + dPrevRunningTransAdjGoldQty;
                    }
                }

                if (dActualTransAdjGoldQty <= dSaleAdjustmentGoldQty)
                {
                    //sRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                    //decimal dPureVal = dActualTransAdjGoldQty * dSaleAdjustmentAvgGoldRate;

                    string sCurrentRate = getRateFromMetalTable();

                    decimal dPureVal = dConvertedToFixingQty * dSaleAdjustmentAvgGoldRate;
                    // sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));

                    if (Convert.ToDecimal(GrWeight) > 0)
                        sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                    else
                        sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sCurrentRate), 2, MidpointRounding.AwayFromZero));
                }
                else //if (dActualTransAdjGoldQty > dSaleAdjustmentGoldQty)
                {
                    if (dPrevTransAdjGoldQty >= dSaleAdjustmentGoldQty)
                    {
                        sRate = getRateFromMetalTable();
                        if (bIsGrossMetalCal)
                        {
                            if (Convert.ToDecimal(GrWeight) > 0)
                                sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate) * dTotWt / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                            // (Convert.ToDecimal(sRate) * dTotWt) / Convert.ToDecimal(GrWeight));
                        }
                    }
                    else
                    {
                        //  if ((dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
                        if (dPrevTransAdjGoldQty + dPrevRunningTransAdjGoldQty >= dSaleAdjustmentGoldQty)
                        {
                            sRate = getRateFromMetalTable();
                            if (bIsGrossMetalCal)
                            {
                                if (Convert.ToDecimal(GrWeight) > 0)
                                    sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate) * dTotWt / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                                // (Convert.ToDecimal(sRate) * dTotWt) / Convert.ToDecimal(GrWeight));
                            }
                        }
                        else
                        {
                            decimal dAvgRateQty = 0m;
                            decimal dCurrentRateQty = 0m;
                            decimal dCurrentRateConvertedtoTransQty = 0m;
                            decimal dCurrentRate = 0m;

                            // dCurrentRateQty = (dActualTransAdjGoldQty - dSaleAdjustmentGoldQty - dPrevRunningTransAdjGoldQty);
                            dCurrentRateQty = (dActualTransAdjGoldQty - dSaleAdjustmentGoldQty);
                            // dCurrentRateConvertedtoTransQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, dCurrentRateQty), 3, MidpointRounding.AwayFromZero);  // new

                            // dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

                            dAvgRateQty = dConvertedToFixingQty - dCurrentRateQty;

                            dCurrentRate = getMetalRate(ItemID, ConfigID);

                            if (dSaleAdjustmentAvgGoldRate > 0 && dCurrentRate > 0)
                            {
                                if (Convert.ToDecimal(GrWeight) > 0)
                                    sRate = Convert.ToString(decimal.Round(((dSaleAdjustmentAvgGoldRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                                else
                                    sRate = Convert.ToString(dCurrentRate);
                            }
                        }
                    }

                }

                if (!string.IsNullOrEmpty(sRate))
                {
                    // dPrevRunningTransAdjGoldQty += Convert.ToDecimal(GrWeight);
                    dPrevRunningTransAdjGoldQty += dConvertedToFixingQty;
                    return decimal.Round(Convert.ToDecimal(sRate), 3, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return 0;
                }


                #endregion
            }
            //==================================================================================================================================
            else if (dSaleAdjustCommitedQty > 0)  //=========Soutik
            {
                #region get CommitedQty/ForDays Wise rate
                int NoOfDaysDiff = 0;
                decimal dCalculaterate = 0;
                decimal dCurrentGoldrate = 0;
                //====================Get Date difference beyween Deposit Date to Today===============================
                TimeSpan t = DateTime.Now - CustAdvDepositDate;
                NoOfDaysDiff = Convert.ToInt16((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - CustAdvDepositDate).TotalDays); //Store Date Difference

                dCurrentGoldrate = getMetalRate(ItemID, ConfigID);  //Store Current Gold rate

                if (dCurrentGoldrate > dSaleAdjustmentMetalRate)  // Get Lowest Rate 
                    dCalculaterate = dSaleAdjustmentMetalRate;
                else
                    dCalculaterate = dCurrentGoldrate;

                if (!string.IsNullOrEmpty(GrWeight))
                {
                    if (bIsGrossMetalCal)
                        dConvertedToFixingQty = decimal.Round(GetConvertedGoldQty_Commited(sTransItemConfigID, Convert.ToDecimal(dTotWt)), 3, MidpointRounding.AwayFromZero);
                    else
                        dConvertedToFixingQty = decimal.Round(GetConvertedGoldQty_Commited(sTransItemConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                    if (dPrevRunningTransAdjGoldQty == 0)
                    {
                        dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty;
                    }
                    else
                    {
                        dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty + dPrevRunningTransAdjGoldQty;
                    }
                }

                if (iSaleAdjustmentCommitedForDays >= NoOfDaysDiff)
                {
                    //sRate = Convert.ToString(GetRateCommitedQtyAndDayWise(Convert.ToDecimal(dConvertedToFixingQty), dSaleAdjustCommitedQty, dCalculaterate, dCurrentGoldrate, Convert.ToString(dConvertedToFixingQty),));
                    sRate = Convert.ToString(GetRateCommitedQtyAndDayWise(Convert.ToDecimal(GrWeight), dSaleAdjustCommitedQty, dCalculaterate, dCurrentGoldrate, Convert.ToString(dConvertedToFixingQty)));

                   //private decimal GetRateCommitedQtyAndDayWise(decimal dSaleAdjustmentGoldQty, decimal dAdjustmentCommitedQty, decimal dLowestMetalRate, decimal dCurrentMetalRate, string GrossWeight)
                }
                else if (iSaleAdjustmentCommitedForDays < NoOfDaysDiff)
                {
                    int iSPId = 0;
                    iSPId = getUserDiscountPermissionId();
                    if ((Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager) || (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2))
                    {
                        sRate = Convert.ToString(GetRateCommitedQtyAndDayWise(Convert.ToDecimal(dConvertedToFixingQty), dSaleAdjustCommitedQty, dCalculaterate, dCurrentGoldrate, Convert.ToString(dConvertedToFixingQty)));
                    }
                    else
                    {
                        sRate = Convert.ToString(dCurrentGoldrate);
                    }
                }

                if (!string.IsNullOrEmpty(sRate))
                {
                    //dSaleAdjustCommitedQty -= dSaleAdjustCommitedQty;
                    // dSaleAdjustCommitedQty -= dSaleAdjustCommitedQty ;
                    dPrevRunningTransAdjGoldQty += dConvertedToFixingQty;
                    return decimal.Round(Convert.ToDecimal(sRate), 3, MidpointRounding.AwayFromZero);
                }
                else
                {
                    return 0;
                }
                #endregion
            }
            else
            {
                return getMetalRate(ItemID, ConfigID);
            }
            //}
            //else
            //    return getMetalRate(ItemID, ConfigID);

            #region commented
            /*if (string.IsNullOrEmpty(sAdjuOrderNo))
            {
                if (sTransItemConfigID == sDefConfigId)
                {
                    #region AdjustmentRate
                    //decimal dActualSaleQty = 0;
                    //decimal dGSSMaturityQty = //Convert.ToDecimal(retailTrans.PartnerData.GSSTotQty);

                    // if(!string.IsNullOrEmpty(txtQuantity.Text))
                    if (!string.IsNullOrEmpty(GrWeight))
                    {
                        //dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                        if (bIsGrossMetalCal)
                            dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(dTotWt)), 3, MidpointRounding.AwayFromZero);
                        else
                            dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                        if (dPrevRunningTransAdjGoldQty == 0)
                        {
                            // dActualTransAdjGoldQty = dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight);
                            dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty;
                        }
                        else
                        {
                            // dActualTransAdjGoldQty = dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight) + dPrevRunningTransAdjGoldQty;
                            dActualTransAdjGoldQty = dPrevTransAdjGoldQty + dConvertedToFixingQty + dPrevRunningTransAdjGoldQty;
                        }
                    }

                    if (dActualTransAdjGoldQty <= dSaleAdjustmentGoldQty)
                    {
                        //sRate = Convert.ToString(dSaleAdjustmentAvgGoldRate);
                        //  decimal dPureVal = dActualTransAdjGoldQty * dSaleAdjustmentAvgGoldRate;

                        string sCurrentRate = getRateFromMetalTable();

                        decimal dPureVal = dConvertedToFixingQty * dSaleAdjustmentAvgGoldRate;
                        // sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));

                        if (Convert.ToDecimal(GrWeight) > 0)
                            sRate = Convert.ToString(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                        else
                            sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sCurrentRate), 2, MidpointRounding.AwayFromZero));
                    }
                    else //if (dActualTransAdjGoldQty > dSaleAdjustmentGoldQty)
                    {
                        if (dPrevTransAdjGoldQty >= dSaleAdjustmentGoldQty)
                        {
                            sRate = getRateFromMetalTable();
                            if (bIsGrossMetalCal)
                            {
                                if (Convert.ToDecimal(GrWeight) > 0)
                                    sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate) * dTotWt / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                                // (Convert.ToDecimal(sRate) * dTotWt) / Convert.ToDecimal(GrWeight));
                            }
                        }
                        else
                        {
                            //  if ((dPrevTransAdjGoldQty + Convert.ToDecimal(GrWeight)+ dPrevRunningQtyGSS ) == dGSSMaturityQty)
                            if (dPrevTransAdjGoldQty + dPrevRunningTransAdjGoldQty >= dSaleAdjustmentGoldQty)
                            {
                                sRate = getRateFromMetalTable();
                                if (bIsGrossMetalCal)
                                {
                                    if (Convert.ToDecimal(GrWeight) > 0)
                                        sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(sRate) * dTotWt / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                                    // (Convert.ToDecimal(sRate) * dTotWt) / Convert.ToDecimal(GrWeight));
                                }
                            }
                            else
                            {
                                decimal dAvgRateQty = 0m;
                                decimal dCurrentRateQty = 0m;
                                decimal dCurrentRateConvertedtoTransQty = 0m;
                                decimal dCurrentRate = 0m;

                                // dCurrentRateQty = (dActualTransAdjGoldQty - dSaleAdjustmentGoldQty - dPrevRunningTransAdjGoldQty);
                                dCurrentRateQty = (dActualTransAdjGoldQty - dSaleAdjustmentGoldQty);
                                // dCurrentRateConvertedtoTransQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, dCurrentRateQty), 3, MidpointRounding.AwayFromZero);  // new

                                // dAvgRateQty = Convert.ToDecimal(GrWeight) - dCurrentRateQty;

                                dAvgRateQty = dConvertedToFixingQty - dCurrentRateQty;

                                dCurrentRate = getMetalRate(ItemID, ConfigID);

                                if (dSaleAdjustmentAvgGoldRate > 0 && dCurrentRate > 0)
                                {
                                    if (Convert.ToDecimal(GrWeight) > 0)
                                        sRate = Convert.ToString(decimal.Round(((dSaleAdjustmentAvgGoldRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                                    else
                                        sRate = Convert.ToString(dCurrentRate);
                                }
                            }
                        }

                    }

                    if (!string.IsNullOrEmpty(sRate))
                    {
                        // dPrevRunningTransAdjGoldQty += Convert.ToDecimal(GrWeight);
                        dPrevRunningTransAdjGoldQty += dConvertedToFixingQty;
                        return decimal.Round(Convert.ToDecimal(sRate), 3, MidpointRounding.AwayFromZero);
                    }
                    else
                    {
                        return 0;
                    }


                    #endregion
                }
                else
                    return getMetalRate(ItemID, ConfigID);
            }
            else
            {
                #region sAdjuOrderNo
                int iMetalType = GetMetalType(ItemID);
                if (sTransItemConfigID == sOrderLineConfig)
                {
                    decimal dAvgRateQty = 0m;
                    decimal dCurrentRateQty = 0m;
                    decimal dCurrentRate = 0m;

                    if (!string.IsNullOrEmpty(GrWeight))
                    {
                        if (bIsGrossMetalCal)
                            dConvertedToFixingQty = decimal.Round(Convert.ToDecimal(dTotWt), 3, MidpointRounding.AwayFromZero);
                        else
                            dConvertedToFixingQty = decimal.Round(Convert.ToDecimal(GrWeight), 3, MidpointRounding.AwayFromZero);

                        dActualTransAdjGoldQty = dConvertedToFixingQty;
                    }

                    dCurrentRateQty = (dActualTransAdjGoldQty - Convert.ToDecimal(sMQty));

                    if (dCurrentRateQty > 0)
                        dAvgRateQty = dActualTransAdjGoldQty - dCurrentRateQty;
                    else
                    {
                        dCurrentRateQty = 0;
                        dAvgRateQty = dActualTransAdjGoldQty;
                    }

                    dCurrentRate = getMetalRate(ItemID, ConfigID);

                    decimal dCommRate = 0m;

                    if (iMetalType == (int)MetalType.Gold)
                        dCommRate = Convert.ToDecimal(sGRate);
                    else if (iMetalType == (int)MetalType.Silver)
                        dCommRate = Convert.ToDecimal(sSRate);
                    else if (iMetalType == (int)MetalType.Platinum)
                        dCommRate = Convert.ToDecimal(sPRate);

                    if (dCommRate > 0 && dCurrentRate > 0 && Convert.ToDecimal(GrWeight) > 0)
                        sRate = Convert.ToString(decimal.Round(((dCommRate * dAvgRateQty) + (dCurrentRateQty * dCurrentRate)) / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));
                    else
                        sRate = Convert.ToString(decimal.Round(Convert.ToDecimal(dCommRate), 2, MidpointRounding.AwayFromZero));

                    if (string.IsNullOrEmpty(sRate))
                        return 0;
                    else
                        return decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero);
                    // return decimal.Round(Convert.ToDecimal(sRate), 2, MidpointRounding.AwayFromZero);
                }
                else
                {
                    //if (iMetalType == (int)MetalType.Gold)
                    //    return Convert.ToDecimal(sGRate);
                    //else if (iMetalType == (int)MetalType.Silver)
                    //    return Convert.ToDecimal(sSRate);
                    //else if (iMetalType == (int)MetalType.Platinum)
                    //    return Convert.ToDecimal(sPRate);
                    //else
                    // return getMetalRate(ItemID, ConfigID);

                    // MessageBox.Show("Configuration mismatch");
                    return 0;
                }

                #endregion
            }*/
            #endregion
        }
        #endregion

        #region Get Adjustment Wise Metal rate
        private decimal GetRateCommitedQtyAndDayWise(decimal dSaleAdjustmentGoldQty, decimal dAdjustmentCommitedQty, decimal dLowestMetalRate, decimal dCurrentMetalRate, string ConvertFixingQty)
        {
            decimal dExtraGold = 0;
            string pRate = string.Empty;
            decimal dCalculaterate = 0m;
            decimal dCurrentRateQty = (dAdjustmentCommitedQty - dPrevTransAdjGoldQty);

            decimal dPureVal = Convert.ToDecimal(ConvertFixingQty) * dSaleAdjustmentAvgGoldRate;
            dCalculaterate = Convert.ToDecimal(decimal.Round(dPureVal /dSaleAdjustmentGoldQty, 5, MidpointRounding.AwayFromZero)); 

            //=========================Check For Lowest Metal Rate========================================
            if (dCurrentMetalRate > dCalculaterate)
                dLowestMetalRate = Convert.ToDecimal(dCalculaterate);
            else
                dLowestMetalRate = Convert.ToDecimal(dCurrentMetalRate);
            //===========================================================================================
            if (dSaleAdjustmentGoldQty <= dCurrentRateQty && dCurrentRateQty > 0)
            {
                pRate = Convert.ToString(dLowestMetalRate);
            }
            else if (dSaleAdjustmentGoldQty > dCurrentRateQty && dCurrentRateQty >= 0)
            {
                dExtraGold = dSaleAdjustmentGoldQty - (dAdjustmentCommitedQty - dPrevTransAdjGoldQty);
                if (Convert.ToDecimal(dSaleAdjustmentGoldQty) > 0)
                    //pRate = Convert.ToString(decimal.Round(((dLowestMetalRate * (dAdjustmentCommitedQty - dPrevTransAdjGoldQty)) + (dExtraGold * dCurrentMetalRate)) / Convert.ToDecimal(dSaleAdjustmentGoldQty), 2, MidpointRounding.AwayFromZero));
                    pRate = Convert.ToString(decimal.Round(((dLowestMetalRate * (dAdjustmentCommitedQty - dPrevTransAdjGoldQty)) + (dExtraGold * dCurrentMetalRate)) / Convert.ToDecimal(dSaleAdjustmentGoldQty), 2, MidpointRounding.AwayFromZero));
                else
                    pRate = Convert.ToString(dCurrentMetalRate);
            }
            else
            {
                pRate = Convert.ToString(dCurrentMetalRate);                
            }

            if (!string.IsNullOrEmpty(pRate))
            {
                return decimal.Round(Convert.ToDecimal(pRate), 3, MidpointRounding.AwayFromZero);
            }
            else
            {
                return 0;
            }

        }
        #endregion

        #region // Making Discount Type

        private void CheckMakingDiscountFromDB(decimal dDiscValue)
        {
            string sWtRange = string.Empty;
            decimal dDiscAmt = 0m;
            dDiscAmt = dDiscValue;
            if (cmbMakingDiscType.SelectedIndex == 0)
                txtMakingDisc.Text = Convert.ToString(dDiscAmt);
            if (dDiscAmt >= 0)
            {
                dMakingDiscDbAmt = dDiscAmt;
                CalcMakingDiscount(dDiscAmt);
            }
        }
        private int getUserDiscountPermissionId()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select RETAILDISCPERMISSION from RETAILSTAFFTABLE where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);

            int iResult = 0;
            iResult = Convert.ToInt16(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iResult;
        }
        private decimal GetMkDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet)
        {
            decimal dResult = 0;


            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string sFlatQry = "";
            if (sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWMAKINGDISCOUNT.FLAT =1";
            else
                sFlatQry = "";

            string commandText = " DECLARE @Temp TABLE (MAXSALESPERSONSDISCPCT numeric(20,2), PROMOTIONCODE nvarchar(20),FLAT int,DiscountType int);" +
                                "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWMAKINGDISCOUNT )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" + // AND RETAIL=1 req by S.Sharma on 15/02/17
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +

                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" + // AND RETAIL=0 req by S.Sharma on 22/05/17
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END " +

                                 "  SELECT @CUSTCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "'";
            #region
            //commandText += " SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2)),ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT " +
            //                " FROM   CRWMAKINGDISCOUNT  " +
            //                " WHERE    " +
            //                " (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM  or CRWMAKINGDISCOUNT.ITEMID='')" +
            //                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE  or CRWMAKINGDISCOUNT.CODE='')" +
            //    //" AND (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM  OR CRWMAKINGDISCOUNT.ITEMID='')" +
            //                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' or CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
            //    //" AND  (CRWMAKINGDISCOUNT.CATEGORY=(SELECT CATEGORY FROM [ECORESPRODUCTCATEGORY] WHERE PRODUCT = @PRODUCT) " +
            //    //" OR CRWMAKINGDISCOUNT.CATEGORY=0) AND " +
            //                " and ('" + dItemMainValue + "' BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
            //                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)  " +
            //                " AND CRWMAKINGDISCOUNT.ACTIVATE = 1 " +
            //                " " + sFlatQry + "" +
            //                " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWMAKINGDISCOUNT.FLAT ASC";
            //else
            //    commandText += ", CRWMAKINGDISCOUNT.FLAT DESC";
            #endregion
            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //1
                " FROM   CRWMAKINGDISCOUNT  " +
                " WHERE (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM) " +
                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE) " +
                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                     " BEGIN " +
                     " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(DiscountType,0) as DiscountType " +
                     "       FROM   CRWMAKINGDISCOUNT  WHERE     " +
                     "     (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM ) AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE ) " +

                     "  AND   (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                     "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                     "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //2
               " FROM   CRWMAKINGDISCOUNT  " +
               " WHERE (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM) " +
               " AND (CRWMAKINGDISCOUNT.CODE='') " +
               " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                    " BEGIN " +
                    " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(DiscountType,0) as DiscountType  FROM   CRWMAKINGDISCOUNT   " +
                    "     WHERE     " +
                    "     (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM ) AND (CRWMAKINGDISCOUNT.CODE='' ) " +

                     "  AND   (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                    "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)" +
                    "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //3
                " FROM   CRWMAKINGDISCOUNT  " +
                " WHERE (CRWMAKINGDISCOUNT.ITEMID='') " +
                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE) " +
                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                     " BEGIN " +
                      " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT ,ISNULL(DiscountType,0) as DiscountType FROM   CRWMAKINGDISCOUNT   " +
                     "     WHERE     " +
                     "     (CRWMAKINGDISCOUNT.ITEMID='' ) AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE ) " +

                     "   AND  (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                     "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                     "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //4
               " FROM   CRWMAKINGDISCOUNT  " +
               " WHERE (CRWMAKINGDISCOUNT.ITEMID='') " +
               " AND (CRWMAKINGDISCOUNT.CODE='') " +
               " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                    " BEGIN " +
                     " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(DiscountType,0) as DiscountType   FROM   CRWMAKINGDISCOUNT   " +
                    "     WHERE     " +
                    "     (CRWMAKINGDISCOUNT.ITEMID='' ) AND (CRWMAKINGDISCOUNT.CODE='' ) " +

                     "   AND (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                    "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                    "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;
            //SqlDataReader mkDiscRd = commandMk.ExecuteReader();

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    sMkPromoCode = Convert.ToString(mkDiscRd.GetValue(1));
                    iIsMkDiscFlat = Convert.ToInt16(mkDiscRd.GetValue(2));
                    iMkDisType = Convert.ToInt16(mkDiscRd.GetValue(3));
                }
            }

            cmbMakingDiscType.SelectedIndex = Convert.ToInt16(iMkDisType);

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }
        private void CalcMakingDiscount(decimal dMkDiscAmt)
        {
            #region Making Discount Calculation

            if (!string.IsNullOrEmpty(txtMakingAmount.Text))
            {
                decimal dTotAmt = 0m;

                if (string.IsNullOrEmpty(txtTotalAmount.Text))
                    dTotAmt = 0;
                else
                    dTotAmt = Convert.ToDecimal(txtTotalAmount.Text.Trim());

                ////if(string.IsNullOrEmpty(txtActToAmount.Text))r
                ////    txtActToAmount.Text = Convert.ToString(decimal.Round(dTotAmt, 2, MidpointRounding.AwayFromZero));
            }

            #endregion
        }
        #endregion

        #region [ // Stone Discount Calculation]

        private decimal CalcStoneDiscount(decimal dStnDiscAmt, int iStnDiscType, decimal dStnDiscWt, decimal dStnRate)
        {
            decimal dStnDiscTotAmt = 0m;

            if (iStnDiscType == 0) // Percentage
            {
                dStnDiscTotAmt = (dStnDiscAmt / 100) * (dStnDiscWt * dStnRate);
            }
            else if (iStnDiscType == 1) // Weight
            {
                dStnDiscTotAmt = dStnDiscAmt * dStnDiscWt;
            }
            else if (iStnDiscType == 2)
            {
                dStnDiscTotAmt = dStnDiscAmt;
            }

            return dStnDiscTotAmt;
        }

        #endregion

        #region // Fixed Metal Rate New
        private void BindIngredientGrid(string sCustOrdrFreezedMetalRate, string sFreezedRateConfigId, int isAdvAgainstNone)
        {
            if (rdbSale.Checked)
            {
                dtIngredients = new DataTable();

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

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


                SqlCommand command = new SqlCommand(commandText.ToString(), conn);
                command.CommandTimeout = 0;
                using (SqlDataReader readerFixRateIngr = command.ExecuteReader())
                {
                    dtIngredients.Load(readerFixRateIngr);
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    #region // Stone Discount

                    dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                    dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                    dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                    dtIngredients.Columns.Add("CTYPE", typeof(int));

                    #endregion

                    txtRate.Text = string.Empty;
                    lblMetalRatesShow.Text = txtRate.Text;
                    dtIngredientsClone = new DataTable();
                    dtIngredientsClone = dtIngredients.Clone();
                    // dtIngredientsClone.Columns["LType"].DataType = typeof(string);
                    //   dtIngredientsClone.Columns["CType"].DataType = typeof(string);
                    decimal dGQty = 0m;
                    decimal dGRate = 0m;

                    foreach (DataRow drClone in dtIngredients.Rows)
                    {
                        if (isMRPUCP)
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

                    if (!isMRPUCP)
                    {
                        foreach (DataRow dr in dtIngredientsClone.Rows)
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

                            if (Convert.ToDecimal(dr["PCS"]) > 0)
                                dStoneWtRange = decimal.Round((Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"])), 3, MidpointRounding.AwayFromZero);//Changes on 11/04/2014 R.Hossain
                            else
                                dStoneWtRange = decimal.Round((Convert.ToDecimal(dr["QTY"])), 3, MidpointRounding.AwayFromZero); //Changes on 11/04/2014 R.Hossain

                            // Stone Discount
                            int iStoneDiscType = 0;
                            decimal dStoneDiscAmt = 0;
                            decimal dStoneDiscTotAmt = 0m;


                            if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold
                                || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver
                                || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum)
                            {
                                //sRate = sCustOrdrFixedMetalRate;  // ConfigID

                                decimal dFreezedRateConfigRatio = 0m;
                                decimal dTransConfigRatio = 0m;
                                decimal dTransIncrSalePct = 0m;
                                decimal dTransIncrSaleAmt = 0m;
                                decimal dActualMetalRate = 0m;

                                dFreezedRateConfigRatio = GetConfigRatioInfo(sFreezedRateConfigId,
                                                                           ConfigID, ref dTransConfigRatio, ref dTransIncrSalePct);

                                if (dTransIncrSalePct > 0)
                                {
                                    dTransIncrSaleAmt = Convert.ToDecimal(sCustOrdrFreezedMetalRate) * (dTransIncrSalePct * 0.01m);
                                }

                                // calculate metal rate based on configuration
                                if (Convert.ToInt32(sFreezedRateConfigId.Substring(0, 2)) == Convert.ToInt32(ConfigID.Substring(0, 2)))
                                {
                                    string sDBMetalRate = getRateFromMetalTable();
                                    sRate = sCustOrdrFreezedMetalRate;

                                    if (isAdvAgainstNone == (int)AdvAgainst.None)
                                    {
                                        if (Convert.ToDecimal(sDBMetalRate) < Convert.ToDecimal(sRate))
                                        {
                                            sRate = sDBMetalRate;
                                        }
                                    }

                                }
                                else if (Convert.ToInt32(sFreezedRateConfigId.Substring(0, 2)) > Convert.ToInt32(ConfigID.Substring(0, 2)))
                                {
                                    dActualMetalRate = (Convert.ToDecimal(sCustOrdrFreezedMetalRate) + dTransIncrSaleAmt) * (dFreezedRateConfigRatio / dTransConfigRatio);
                                    sRate = Convert.ToString(decimal.Round(dActualMetalRate, MidpointRounding.AwayFromZero));

                                    string sDBMetalRate = getRateFromMetalTable();
                                    if (isAdvAgainstNone == (int)AdvAgainst.None)
                                    {
                                        if (Convert.ToDecimal(sDBMetalRate) < Convert.ToDecimal(sRate))
                                        {
                                            sRate = sDBMetalRate;
                                        }
                                    }
                                }
                                else
                                {
                                    dActualMetalRate = (Convert.ToDecimal(sCustOrdrFreezedMetalRate) + dTransIncrSaleAmt) * (dTransConfigRatio / dFreezedRateConfigRatio);
                                    sRate = Convert.ToString(decimal.Round(dActualMetalRate, MidpointRounding.AwayFromZero));

                                    string sDBMetalRate = getRateFromMetalTable();

                                    if (isAdvAgainstNone == (int)AdvAgainst.None)
                                    {
                                        if (Convert.ToDecimal(sDBMetalRate) < Convert.ToDecimal(sRate))
                                        {
                                            sRate = sDBMetalRate;
                                        }
                                    }
                                }

                                // -- end
                                dCustomerOrderFreezedRate = Convert.ToDecimal(sRate);


                                //dGRate += decimal.Round((Convert.ToDecimal(sRate)), 2, MidpointRounding.AwayFromZero);
                                //dGQty += decimal.Round((Convert.ToDecimal(dr["QTY"])), 3, MidpointRounding.AwayFromZero);

                                //lblMetalRatesShow.Text = sRate;
                            }
                            else
                            {
                                sRate = getRateFromMetalTable();
                            }


                            // ------*********

                            if (!string.IsNullOrEmpty(sRate))
                            {
                                sCalcType = GetIngredientCalcType(ref iStoneDiscType, ref dStoneDiscAmt);

                                if (!string.IsNullOrEmpty(sCalcType))
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


                            StringBuilder commandText1 = new StringBuilder();
                            commandText1.Append("select metaltype,ISNULL(NOPRICEREQUIRED,0) from inventtable where itemid='" + Convert.ToString(dr["ItemID"]) + "'");

                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

                            SqlCommand command1 = new SqlCommand(commandText1.ToString(), conn);
                            command1.CommandTimeout = 0;

                            using (SqlDataReader reader1 = command1.ExecuteReader())
                            {
                                if (reader1.HasRows)
                                {
                                    #region Reader
                                    while (reader1.Read())
                                    {
                                        //---- add code --  for CalcType

                                        if (((int)reader1.GetValue(0) == (int)MetalType.LooseDmd) || ((int)reader1.GetValue(0) == (int)MetalType.Stone))
                                        {
                                            if ((int)reader1.GetValue(1) == 0)
                                            {
                                                if (!string.IsNullOrEmpty(sCalcType))
                                                {
                                                    #region // Stone Discount Calculation


                                                    #endregion

                                                    if (Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Weight))
                                                    {
                                                        if (dStoneDiscAmt > 0)
                                                        {
                                                            dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                        }
                                                        // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                        dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                    }

                                                    else if (Convert.ToInt32(sCalcType) == Convert.ToInt32(RateType.Pieces))
                                                    {
                                                        if (dStoneDiscAmt > 0)
                                                        {
                                                            dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["PCS"]), Convert.ToDecimal(dr["Rate"]));
                                                        }
                                                        // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                        dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["PCS"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                    }

                                                    else // Tot
                                                    {
                                                        if (dStoneDiscAmt > 0)
                                                        {
                                                            dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                        }
                                                        //dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                        dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["Rate"]) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                    }
                                                }
                                                else
                                                {
                                                    if (dStoneDiscAmt > 0)
                                                    {
                                                        dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                                    }
                                                    // dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                                    dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                                }
                                            }
                                            else
                                            {
                                                dr["CValue"] = 0;
                                            }
                                        }
                                        else
                                        {
                                            //if (dStoneDiscAmt > 0)
                                            //{
                                            //    dStoneDiscTotAmt = CalcStoneDiscount(dStoneDiscAmt, iStoneDiscType, Convert.ToDecimal(dr["QTY"]), Convert.ToDecimal(dr["Rate"]));
                                            //}

                                            dr["CValue"] = decimal.Round(Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"]), 2, MidpointRounding.AwayFromZero);
                                            // dr["CValue"] = decimal.Round((Convert.ToDecimal(dr["QTY"]) * Convert.ToDecimal(dr["Rate"])) - dStoneDiscTotAmt, 2, MidpointRounding.AwayFromZero);
                                        }

                                        // --- end

                                        if ((int)reader1.GetValue(0) == (int)MetalType.Gold)
                                        {
                                            decimal dgValue = 0;
                                            if (!string.IsNullOrEmpty(txtgval.Text))
                                                dgValue = Convert.ToDecimal(txtgval.Text);

                                            txtgval.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(dgValue + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                        }

                                        if ((int)reader1.GetValue(0) == (int)MetalType.Silver
                                            || (int)reader1.GetValue(0) == (int)MetalType.Platinum)
                                        {
                                            decimal dOMValue = 0;
                                            if (!string.IsNullOrEmpty(txtOMValue.Text))
                                                dOMValue = Convert.ToDecimal(txtOMValue.Text);

                                            txtOMValue.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(dOMValue + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                                        }
                                    }
                                    #endregion
                                }

                                if (reader1.IsClosed == false)
                                    reader1.Close();
                            }

                            if (conn.State == ConnectionState.Open)
                                conn.Close();

                            if (dStoneDiscAmt > 0)
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
                            txtRate.Text = (string.IsNullOrEmpty(txtRate.Text)) ? Convert.ToString(dr["CValue"]) : Convert.ToString(decimal.Round(Convert.ToDecimal(txtRate.Text) + Convert.ToDecimal(dr["CValue"]), 2, MidpointRounding.AwayFromZero));
                        }
                        dtIngredientsClone.AcceptChanges();

                        //gold
                        //if (dGQty > 0 && dGRate > 0)
                        //    lblMetalRatesShow.Text = Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dGRate / dGQty)), 2, MidpointRounding.AwayFromZero));


                        cmbRateType.SelectedIndex = (int)RateType.Tot;
                        cmbRateType.Enabled = false;

                        // Added on 07.06.2013
                        txtPCS.Enabled = false;
                        txtQuantity.Enabled = false;
                        txtRate.Enabled = false;
                        //txtChangedMakingRate.Enabled = false;
                        txtMakingDisc.Enabled = false;
                        cmbMakingType.Enabled = false;
                        //
                    }
                    else
                    {
                        txtRate.Text = MRPUCP;
                        //decimal dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sBaseConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);
                        //dPrevRunningTransAdjGoldQty += dConvertedToFixingQty;
                    }
                    btnIngrdientsDetails.Visible = true;
                }
                else
                {
                    if (!isMRPUCP)
                    {
                        #region [ GSS Transaction]
                        if (IsGSSTransaction)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if (iMetalType == (int)MetalType.Gold)
                            {
                                if (!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getGSSRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));
                                    if (dMetalRate > 0)
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                    else
                                        txtRate.Text = getRateFromMetalTable();
                                }
                                else
                                {
                                    // Get Qty
                                    decimal dSKUQty = 0;

                                    dSKUQty = GetSKUQty(ItemID);

                                    if (dSKUQty > 0)
                                    {
                                        decimal dMetalRate = 0;

                                        dMetalRate = getGSSRate(Convert.ToString(dSKUQty), ConfigID, Convert.ToDecimal(dSKUQty));
                                        if (dMetalRate > 0)
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                        else
                                            txtRate.Text = getRateFromMetalTable();
                                    }
                                    else
                                        txtRate.Text = getRateFromMetalTable();
                                }
                            }
                            else
                                txtRate.Text = getRateFromMetalTable();
                        }
                        #endregion

                        #region [Sale Adjustment]
                        else if (IsSaleAdjustment)
                        {
                            int iMetalType = GetMetalType(ItemID);
                            //getMetalRate(string sItemId, string sConfigId)
                            if (iMetalType == (int)MetalType.Gold
                                || iMetalType == (int)MetalType.Silver
                                || iMetalType == (int)MetalType.Platinum)
                            {
                                if (!string.IsNullOrEmpty(txtQuantity.Text))
                                {
                                    decimal dMetalRate = 0;

                                    dMetalRate = getAdjustmentRate(txtQuantity.Text.Trim(), ConfigID, Convert.ToDecimal(txtQuantity.Text.Trim()));//1
                                    if (dMetalRate > 0)
                                        txtRate.Text = Convert.ToString(dMetalRate);
                                    else
                                        txtRate.Text = getRateFromMetalTable();
                                }
                                else
                                {
                                    decimal dSKUQty = 0;
                                    dSKUQty = GetSKUQty(ItemID);
                                    decimal dMetalRate = 0;
                                    if (dSKUQty > 0)
                                    {
                                        dMetalRate = getAdjustmentRate(Convert.ToString(dSKUQty), ConfigID, Convert.ToDecimal(dSKUQty));//2
                                        if (dMetalRate > 0)
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                        else
                                            txtRate.Text = getRateFromMetalTable();
                                    }
                                    else if (!IsRetailItem(ItemID))
                                    {
                                        dMetalRate = getAdjustmentRate(Convert.ToString(dSKUQty), ConfigID, Convert.ToDecimal(dSKUQty));//5
                                        if (dMetalRate > 0)
                                        {
                                            txtRate.Text = Convert.ToString(dMetalRate);
                                            lblMetalRatesShow.Text = txtRate.Text;
                                        }
                                        else
                                        {
                                            txtRate.Text = getRateFromMetalTable();
                                            lblMetalRatesShow.Text = txtRate.Text;
                                        }
                                    }
                                    else
                                        txtRate.Text = getRateFromMetalTable();
                                }
                            }
                            else
                                txtRate.Text = getRateFromMetalTable();
                        }
                        #endregion

                        else
                            txtRate.Text = getRateFromMetalTable();
                    }
                    else
                    {
                        txtRate.Text = MRPUCP;
                        // decimal dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sBaseConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                    }
                    btnIngrdientsDetails.Visible = false;
                }

                #region  lblMetalRatesShow shwo purpose
                decimal dRate = 0m;
                decimal dGoldQty = 0m;
                if (dtIngredientsClone != null && dtIngredientsClone.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtIngredientsClone.Rows)
                    {
                        if (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold
                            || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver
                            || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum
                            || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Palladium)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["Rate"])) && !string.IsNullOrEmpty(Convert.ToString(dr["QTY"])))
                            {
                                dGoldQty += Convert.ToDecimal(dr["QTY"]);
                                dRate += (Convert.ToDecimal(dr["Rate"]) * Convert.ToDecimal(dr["QTY"]));
                            }
                        }
                    }
                }
                if (dRate > 0 && dGoldQty > 0)
                    lblMetalRatesShow.Text = Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dRate / dGoldQty)), 2, MidpointRounding.AwayFromZero));

                #endregion


                if (conn.State == ConnectionState.Open)
                    conn.Close();


            }
        }
        private int GetCustOrderFixedRateInfo(string sOrderNo, ref decimal dFixedMetalRateVal, ref string sConfigId)  // 
        {
            int IsFixedMetalRate = 0;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT ISNULL(ISFIXEDQTY,0), ISNULL(FIXEDMETALRATE,0),FIXEDMETALRATECONFIGID FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sOrderNo + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        IsFixedMetalRate = Convert.ToInt32(reader.GetValue(0));
                        dFixedMetalRateVal = Convert.ToDecimal(reader.GetValue(1));
                        sConfigId = Convert.ToString(reader.GetValue(2));
                    }
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return IsFixedMetalRate;
        }

        private int GetCustOrderRateFreezeInfo(string sOrderNo)  // 
        {
            int IsFreezedMetalRate = 0;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT ISNULL(ISMETALRATEFREEZE,0) FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sOrderNo + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    IsFreezedMetalRate = Convert.ToInt32(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return IsFreezedMetalRate;
        }

        private int GetCustOrderRateFreezeAdvAgainstInfo(string sOrderNo)  //  
        {
            int IsRateWhichEverisLessWillWork = 0;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT ISNULL(ADVAGAINST,0) FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sOrderNo + "' "; // if none=0 then true

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    IsRateWhichEverisLessWillWork = Convert.ToInt32(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return IsRateWhichEverisLessWillWork;
        }

        private decimal GetConfigRatioInfo(string sFixedConfigId, string sTransConfigId, ref decimal dTransConfigRatio, ref decimal dTransIncrSalePct)  // // Fixed Metal Rate New
        {
            decimal dFixedRateConfigRatio = 0m;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @INVENTLOCATION VARCHAR(20) " +
                                " DECLARE @FIXEDCONFRATIO NUMERIC(32,10)  SELECT @INVENTLOCATION = RETAILCHANNELTABLE.INVENTLOCATION" +
                                " FROM   RETAILCHANNELTABLE INNER JOIN RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID" +
                                " WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                                " SELECT @FIXEDCONFRATIO = ISNULL(CONFRATIO,0) FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = @FIXEDCONFIGID" +
                                " SELECT @FIXEDCONFRATIO AS FIXEDCONFRATIO,ISNULL(CONFRATIO,0) AS TRANSCONFRATIO ,ISNULL(INCRSALEPERCENT,0) AS TRANSINCRSALEPCT" +
                                " FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = @TRANSCONFIGID";

            if (connection.State == ConnectionState.Closed)
                connection.Open();


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            command.Parameters.Add("@FIXEDCONFIGID", SqlDbType.NVarChar, 10).Value = sFixedConfigId;
            command.Parameters.Add("@TRANSCONFIGID", SqlDbType.NVarChar, 10).Value = sTransConfigId;


            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dFixedRateConfigRatio = Convert.ToDecimal(reader.GetValue(0));
                        dTransConfigRatio = Convert.ToDecimal(reader.GetValue(1));
                        dTransIncrSalePct = Convert.ToDecimal(reader.GetValue(2));
                    }
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dFixedRateConfigRatio;
        }

        private void GetPurcExchngConfigRatio(string sConfigId, ref decimal dTransConfigRatio, ref decimal dParamConfigRatio)  // OG Retagging
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @PARAMCONFIGID NVARCHAR(10)  DECLARE @TransCONFRATIO NUMERIC(32,10)  DECLARE @ParamCONFRATIO NUMERIC(32,10) " + //DECLARE @FIXEDCONFRATIO NUMERIC(32,10)
                                " SELECT @INVENTLOCATION = RETAILCHANNELTABLE.INVENTLOCATION" +
                                " FROM   RETAILCHANNELTABLE INNER JOIN RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID" +
                                " WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                                " SELECT  @PARAMCONFIGID = ISNULL(DEFAULTCONFIGIDGOLD,'') FROM RETAILPARAMETERS WHERE DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'" +//INVENTPARAMETERS
                                " SELECT @TransCONFRATIO = ISNULL(CONFRATIO,0) FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = '" + sConfigId + "'" +
                                " SELECT @ParamCONFRATIO = ISNULL(CONFRATIO,0) FROM CONFIGRATIO WHERE METALTYPE = 1 AND INVENTLOCATIONID = @INVENTLOCATION AND CONFIGIDSTANDARD = @PARAMCONFIGID" +
                                " SELECT @TransCONFRATIO, @ParamCONFRATIO";

            if (connection.State == ConnectionState.Closed)
                connection.Open();


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            //command.Parameters.Add("@FIXEDCONFIGID", SqlDbType.NVarChar, 10).Value = sFixedConfigId;
            // command.Parameters.Add("@TRANSCONFIGID", SqlDbType.NVarChar, 10).Value = sTransConfigId;


            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        dTransConfigRatio = Convert.ToDecimal(reader.GetValue(0));
                        dParamConfigRatio = Convert.ToDecimal(reader.GetValue(1));
                    }
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

        }

        private void GetPurcExchngGoldQty()
        {
            decimal dGoldQty = 0m;

            int iPEMetalType = 0;
            iPEMetalType = GetMetalType(sBaseItemID);

            if (iPEMetalType == (int)MetalType.Gold)
            {
                decimal dTransCR = 0;
                decimal dParamCR = 0;

                #region commented for samra
                //GetPurcExchngConfigRatio(sBaseConfigID, ref dTransCR, ref dParamCR);

                //if(dTransCR > 0 && dParamCR > 0)
                //{

                //    if(!string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) && !string.IsNullOrEmpty(txtPurity.Text.Trim()))
                //    {
                //        dGoldQty = (Convert.ToDecimal(txtTotalWeight.Text.Trim()) * Convert.ToDecimal(txtPurity.Text.Trim()))
                //                        * (dTransCR / dParamCR);
                //    }

                //    if(dGoldQty > 0)
                //        txtQuantity.Text = Convert.ToString(decimal.Round(dGoldQty, 3, MidpointRounding.AwayFromZero));
                //    else
                //        txtQuantity.Text = string.Empty;
                //}
                #endregion
                if (!string.IsNullOrEmpty(txtTotalWeight.Text.Trim()))
                {
                    dGoldQty = Convert.ToDecimal(txtTotalWeight.Text.Trim());
                }

                if (dGoldQty > 0)
                    txtQuantity.Text = Convert.ToString(decimal.Round(dGoldQty, 3, MidpointRounding.AwayFromZero));
                else
                    txtQuantity.Text = string.Empty;

            }

        }

        private bool IsValidMakingRate(string sMKItemId)
        {
            bool bValid = true;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @MFG_CODE AS NVARCHAR(20)  DECLARE @COMPLEXITY_CODE AS NVARCHAR(20) " +
                                 " DECLARE @RESULT AS INT  SELECT @MFG_CODE = ISNULL(MFG_CODE,'') FROM INVENTTABLE WHERE ITEMID = '" + sMKItemId + "'" +
                                 " SELECT @COMPLEXITY_CODE = ISNULL(COMPLEXITY_CODE,'') FROM INVENTTABLE WHERE ITEMID = '" + sMKItemId + "'" +
                                 " IF(@MFG_CODE <> '' AND @COMPLEXITY_CODE <> '') BEGIN SET @RESULT = 0 END ELSE" +
                                 " BEGIN SET @RESULT = 1 END  SELECT @RESULT";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bValid = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bValid;
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
            command.Dispose();

            return bBulkItem;
        }
        #endregion

        /// <summary>
        /// retail =1 in inventtable , item can only sale
        /// Dev on  : 17/04/2014 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select RETAIL from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRetailItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bRetailItem;
        }

        private bool IsIngredient(string sItemId)
        {
            bool bIsIngredient = false;
            int iRes = 0;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select top 1 isnull(INGREDIENT,0) from SKUTable_Posted where SkuNumber = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            iRes = Convert.ToInt16(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (iRes == 1)
                bIsIngredient = true;

            return bIsIngredient;
        }

        private bool IsExistinSKUTABLE_Posted(string sItemId)
        {
            bool bIsExist = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISEXIST INT  SET @ISEXIST = 0 IF EXISTS (SELECT top 1 SkuNumber from SKUTable_Posted  WHERE SkuNumber = '" + sItemId + "')" +
                                 " BEGIN SET @ISEXIST = 1 END SELECT @ISEXIST";

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bIsExist = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bIsExist;
        }

        /// <summary>
        /// GIFT =1 in inventtable 
        /// Dev on  : 14/08/2015 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsGiftItem(string sItemId)
        {
            bool bGiftItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select GIFT from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bGiftItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bGiftItem;
        }

        private void txtMakingDisc_Leave(object sender, EventArgs e)
        {

        }

        private bool IsValidBatchId(string sBatchId)
        {
            bool bValidBatchId = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select INVENTBATCHID from RETAILTRANSACTIONSALESTRANS WHERE INVENTBATCHID = '" + sBatchId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string sINVENTBATCHID = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sINVENTBATCHID))
                bValidBatchId = true;
            else
                bValidBatchId = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bValidBatchId;
        }

        private bool IsBatchItem(string sItemId)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "select ISNULL(TRACKINGDIMENSIONGROUP,'') TRACKINGDIMENSIONGROUP from ECORESTRACKINGDIMENSIONGROUPFLDSETUP " +
                        " where TRACKINGDIMENSIONGROUP = (select TRACKINGDIMENSIONGROUP  from ECORESTRACKINGDIMENSIONGROUPITEM where ITEMID ='" + sItemId + "'" +
                        " and ITEMDATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') and DIMENSIONFIELDID=2 and ISACTIVE=1";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private decimal getItemTaxPercentage(string sItemId)
        {
            SqlConnection connection = new SqlConnection();
            decimal sResult = 0m;

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            //commandText = "select ABS(CAST(ISNULL(TD.TAXVALUE,0)AS DECIMAL(28,2))) TAXVALUE from TAXDATA td," +
            //               " INVENTTABLEMODULE im,taxonitem it where it.TAXITEMGROUP=im.TAXITEMGROUPID and td.TAXCODE=it.taxcode and  im.moduletype=2 and im.ITEMID='" + sItemId + "'";


            commandText = "select (CAST(ISNULL(TD.TAXVALUE,0)AS DECIMAL(28,2))) TAXVALUE from TAXDATA td," +
                  " INVENTTABLEMODULE im,taxonitem it," +
                  " RETAILSTORETABLE rs, TAXGROUPDATA tg" +
                  " where it.TAXITEMGROUP=im.TAXITEMGROUPID " +
                  " and td.TAXCODE=it.taxcode and  im.moduletype=2 and im.ITEMID='" + sItemId + "'" +
                  " and rs.STORENUMBER='" + ApplicationSettings.Database.StoreID + "'" +
                  " and rs.TAXGROUP=tg.TAXGROUP" +
                  " and tg.TAXCODE=td.TAXCODE and tg.EXEMPTTAX=0";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;

        }


        private bool IsReturnQtyValid(string sReceiptIdId, int iLine, decimal _dQty)
        {
            bool bReturnQtyValid = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select QTY from RETAILTRANSACTIONSALESTRANS WHERE RECEIPTID = '" + sReceiptIdId + "' AND LINENUM = " + iLine + "";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal dQty = Convert.ToDecimal(command.ExecuteScalar());

            if (dQty == _dQty)
                bReturnQtyValid = true;
            else
                bReturnQtyValid = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturnQtyValid;
        }

        private decimal getReturnQty(string sReceiptIdId, int iLine)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select QTY from RETAILTRANSACTIONSALESTRANS WHERE RECEIPTID = '" + sReceiptIdId + "' AND LINENUM = " + iLine + "";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal dQty = Convert.ToDecimal(command.ExecuteScalar());


            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dQty;
        }

        private bool getCostPrice(string sItemId, decimal dSalesValue)
        {
            bool bReturn = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            //added CRWSTONETRANSFERCOST on 060818
            string commandText = "select ABS(CAST(ISNULL(CostPrice + TRANSFERCOSTPRICE + CRWSTONETRANSFERCOST,0)AS DECIMAL(28,2)))  from SKUTABLE_POSTED WHERE skunumber = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal _dSalesValue = Convert.ToDecimal(command.ExecuteScalar());

            if (_dSalesValue <= dSalesValue)
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }


        private decimal getTransferCostPrice(string sItemId)
        {
            decimal bReturn = 0m;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ABS(CAST(ISNULL(TRANSFERCOSTPRICE,0)AS DECIMAL(28,2)))  from SKUTABLE_POSTED WHERE skunumber = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bReturn = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }

        private bool IsReturnPcsValid(string sReceiptId, int iLine, decimal _dPcs)
        {
            bool bReturnPcsValid = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select PIECES  from RETAIL_CUSTOMCALCULATIONS_TABLE  a" +
                                " left join RETAILTRANSACTIONSALESTRANS b on a.TRANSACTIONID=b.TRANSACTIONID " +
                                " and a.LINENUM=b.LINENUM and a.TERMINALID=b.TERMINALID " +
                                " WHERE b.RECEIPTID = '" + sReceiptId + "' AND B.LINENUM=" + iLine + "";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal dPcs = Convert.ToDecimal(command.ExecuteScalar());

            if (Math.Abs(_dPcs) == Math.Abs(dPcs))
                bReturnPcsValid = true;
            else
                bReturnPcsValid = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturnPcsValid;
        }

        private decimal getReturnPcs(string sReceiptId, int iLine)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select PIECES  from RETAIL_CUSTOMCALCULATIONS_TABLE  a" +
                                " left join RETAILTRANSACTIONSALESTRANS b on a.TRANSACTIONID=b.TRANSACTIONID " +
                                " and a.LINENUM=b.LINENUM and a.TERMINALID=b.TERMINALID " +
                                " WHERE b.RECEIPTID = '" + sReceiptId + "' AND B.LINENUM=" + iLine + "";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal dPcs = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dPcs;
        }

        private void frmCustomFieldCalculations_Load(object sender, EventArgs e)
        {
            if (retailTrans != null)
            {
                if (!string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftCWQTY))
                {
                    btnAdd_Click(sender, e);
                }
                if (retailTrans.PartnerData.IsRepairReturn)
                {
                    btnAdd_Click(sender, e);
                }
            }
        }

        private void rdbPurchase_Click(object sender, EventArgs e)
        {
            //GetExtendedPurchase(sender, e);
        }

        private void rdbExchange_Click(object sender, EventArgs e)
        {
            //GetExtendedPurchase(sender, e);
        }

        private void GetExtendedPurchase(object sender, EventArgs e)
        {
            if (!isViewing)
            {
                if (!isMRPUCP)
                {
                    txtRate.Text = getRateFromMetalTable();

                    lblMetalRatesShow.Text = txtRate.Text;
                    //

                    if (iOWNDMD == 1 || iOWNOG == 1 || iOTHERDMD == 1 || iOTHEROG == 1)
                    {
                        // string sBaseUnit = string.Empty;
                        cmbRateType.SelectedIndex = (int)RateType.Tot;

                        txtQuantity.Text = string.Empty;
                        txtLossPct.Text = "0";
                        txtLossPct.Enabled = false;
                        //  txtLossWeight.Text = "0";

                        txtTotalWeight.Text = string.Empty;
                        txtTotalWeight.Enabled = false;

                        txtPurity.Text = string.Empty;
                        txtPurity.Enabled = false;
                        bool bIsOGReturn = false;
                        if (rdbExchangeReturn.Checked || rdbPurchReturn.Checked)
                        {
                            bIsOGReturn = true;
                        }


                        int iRType = 0;
                        int iCurrentTransType = 0;

                        if (rdbExchangeReturn.Checked || rdbExchange.Checked)
                        {
                            iRType = (int)RateTypeNew.Exchange;
                        }
                        else if (rdbPurchReturn.Checked || rdbPurchase.Checked)
                        {
                            iRType = (int)RateTypeNew.Purchase;
                        }

                        if (rdbPurchase.Checked)
                            iCurrentTransType = (int)TransactionType.Purchase;
                        else if (rdbExchange.Checked)
                            iCurrentTransType = (int)TransactionType.Exchange;
                        else if (rdbPurchReturn.Checked)
                            iCurrentTransType = (int)TransactionType.PurchaseReturn;
                        else if (rdbExchangeReturn.Checked)
                            iCurrentTransType = (int)TransactionType.ExchangeReturn;




                        frmExtendedPurchExch ObjExtendedPurchExch = new frmExtendedPurchExch(sBaseItemID, sBaseUnitId, sBaseConfigID, txtRate.Text,
                                                                    this, dtExtndPurchExchng, bIsOGReturn, sCustomerId, iRType, iCurrentTransType);


                        ObjExtendedPurchExch.ShowDialog();

                        // txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
                        txtLossPct_TextChanged(sender, e);
                        BindOGIngredient();
                    }
                    else
                    {
                        if (!rdbSale.Checked)
                        {
                            txtTotalWeight.Enabled = true;
                            txtPurity.Enabled = true;
                            txtLossPct.Enabled = true;

                            txtQuantity.Text = string.Empty;
                            GetPurcExchngGoldQty();
                            txtLossPct_TextChanged(sender, e);
                        }
                    }
                }
            }
        }

        private void rdbPurchReturn_Click(object sender, EventArgs e)
        {
            //GetExtendedPurchase(sender, e);
        }

        private void rdbExchangeReturn_Click(object sender, EventArgs e)
        {

        }

        private void rdbPurchase_MouseClick(object sender, MouseEventArgs e)
        {
            GetExtendedPurchase(sender, e);
        }

        private void rdbExchange_MouseClick(object sender, MouseEventArgs e)
        {
            GetExtendedPurchase(sender, e);
        }

        private void rdbPurchReturn_MouseClick(object sender, MouseEventArgs e)
        {
            GetExtendedPurchase(sender, e);
        }

        private void rdbExchangeReturn_MouseClick(object sender, MouseEventArgs e)
        {
            GetExtendedPurchase(sender, e);
        }

        private void txtManualAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtChangedTotAmount.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtManualAmount_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtManualAmount_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtChangedTotAmount.Text))
            {
                // txtTotalAmount.Text = txtActToAmount.Text;
                txtChangedMakingRate.Text = txtActMakingRate.Text;
            }
            else
            {
                if (Convert.ToDecimal(txtChangedTotAmount.Text) == 0)
                {
                    // txtTotalAmount.Text = txtActToAmount.Text;
                    txtChangedMakingRate.Text = txtActMakingRate.Text;
                }
                else
                    iTotAmtChanged = 1;
            }


            if (!isMRPUCP)
            {
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;// + Convert.ToDecimal(txtGoldTax.Text)
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                //txtActToAmount.Text = txtTotalAmount.Text;
            }
            else
                txtLineDisc_Leave(sender, e);
        }

        private bool getValidMakingCalValue()
        {
            string sFieldName = string.Empty;
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();

            bool bIsValid = false;
            decimal dinitDiscValue = 0;
            decimal dQty = 0;
            decimal dMkPerDisc = 0;
            int iPcs = 0;
            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;
            decimal dTotAmt = 0m;
            decimal dActTotAmt = 0m;
            decimal dManualChangesAmt = 0m;
            decimal dMkDiscTotAmt = 0m;
            decimal dChangedTotAmt = 0m;

            decimal dPcs = 0m;


            if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                sFieldName = "MAXSALESPERSONSDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                sFieldName = "MAXMANAGERLEVELDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                sFieldName = "MAXMANAGERLEVEL2DISCPCT";
            else
                sFieldName = "OPENINGDISCPCT";


            if (!string.IsNullOrEmpty(txtPCS.Text))
                dPcs = Convert.ToDecimal(txtPCS.Text);
            else
                dPcs = 0;
            if (!string.IsNullOrEmpty(txtQuantity.Text))
                dQty = Convert.ToDecimal(txtQuantity.Text);
            else
                dQty = 0;

            if (!string.IsNullOrEmpty(txtPCS.Text))
                iPcs = Convert.ToInt16(txtPCS.Text);
            else
                iPcs = 0;

            if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
            else
                dMkRate1 = 0;

            if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
            else
                dMkRate = 0;

            if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
            else
                dTotAmt = 0;
            if (!string.IsNullOrEmpty(txtActToAmount.Text))
                dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
            else
                dActTotAmt = 0;
            if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
                dMkDiscTotAmt = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
            else
                dMkDiscTotAmt = 0;

            if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                dManualChangesAmt = Convert.ToDecimal(txtChangedTotAmount.Text);
            else
                dManualChangesAmt = 0;

            if (!isMRPUCP)
                dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, sFieldName);// get OPENINGDISCPCT field value FOR THE OPENING

            if (!string.IsNullOrEmpty(txtMakingDisc.Text))
                dMkPerDisc = Convert.ToDecimal(txtMakingDisc.Text);
            else
                dMkPerDisc = 0;

            if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                dChangedTotAmt = Convert.ToDecimal(txtChangedTotAmount.Text);
            else
                dChangedTotAmt = 0;
            Decimal decimalAmount = 0m;
            decimal dMakingQty = 0m;

            if (cmbMakingType.SelectedIndex == 4)
            {
                //foreach (DataRow dr in dtIngredients.Rows)
                //{
                //    dMakingQty += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                //}
                dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
            }
            else
            {
                if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                {
                    string dNetWt = "";
                    if (bValidSKU)
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

            #region added for making disc when qty<6 on 31/07/17
            if (bValidSKU)
            {
                if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4))
                {
                    string sSqr1 = "select isnull(NOEXTRAMKADDITION,0) from INVENTTABLE where itemid='" + sBaseItemID + "'";
                    string sResultForNOEXTRAMKADDITION = NIM_ReturnExecuteScalar(sSqr1);
                    decimal dAddMk = 0;
                    decimal dMinMkQty = 0m;

                    string sSqr3 = "select isnull(ADDITIONALMAKING,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                    string sAddMk = NIM_ReturnExecuteScalar(sSqr3);

                    string sSqr4 = "select isnull(MINMAKINGQTY,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                    string sMinMkQty = NIM_ReturnExecuteScalar(sSqr4);


                    dMinMkQty = Convert.ToDecimal(sMinMkQty);

                    dAddMk = Convert.ToDecimal(sAddMk);

                    if (sResultForNOEXTRAMKADDITION == "0")
                    {
                        if (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                        {
                            if (Convert.ToDecimal(dMakingQty) <= dMinMkQty && Convert.ToDecimal(dMakingQty) > 0 && dinitDiscValue > 0) // 6 replace with  dMinMkQty on 260418
                            {
                                dinitDiscValue = ((dinitDiscValue * dMakingQty) + dAddMk) / dMakingQty;
                            }
                        }
                    }
                }
            }
            #endregion

            if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                decimalAmount = dinitDiscValue * dMakingQty;
            else if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                decimalAmount = dinitDiscValue * dPcs;
            else if (cmbMakingType.SelectedIndex == 2)
                decimalAmount = dinitDiscValue;
            else if (cmbMakingType.SelectedIndex == 3)
            {
                if (!string.IsNullOrEmpty(txtgval.Text))
                {
                    if (Convert.ToDecimal(txtgval.Text.Trim()) != 0)
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtgval.Text.Trim()));
                    else
                        decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
                }
                else
                    decimalAmount = (Convert.ToDecimal(txtChangedMakingRate.Text.Trim()) / 100) * (Convert.ToDecimal(txtAmount.Text.Trim()));
            }


            if (dinitDiscValue >= 0)
            {
                if ((dChangedTotAmt) > dTotAmt) //am- Convert.ToDecimal(txtGoldTax.Text)
                {
                    MessageBox.Show("Changed total amount should not greater than final amount.");
                    //txtChangedTotAmount.Focus();
                    bIsValid = false;
                }
                else if ((dMkPerDisc < 0))
                {
                    MessageBox.Show("Discount should not negetive value");
                    //txtChangedTotAmount.Focus();
                    bIsValid = false;
                }
                else if ((dMkDiscTotAmt > decimalAmount))
                {
                    if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                        && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero)) + "' as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' per Gms.");
                    else if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero)) + "' as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' per Pcs");
                    else if (cmbMakingType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero)) + "' as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' as Total");
                    else if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero)) + "' as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' as Percentage");

                    txtChangedTotAmount.Text = "";
                    //txtChangedTotAmount.Focus();
                    bIsValid = false;
                }
                else if (((dMkRate1 * dQty) < (dinitDiscValue * dQty)) && dMkPerDisc > 0 && (dMkRate1 * dQty) < dMkPerDisc)
                {
                    if ((cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                        && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round((dMkRate1 * dQty), 2, MidpointRounding.AwayFromZero)) + "'  as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' per Gms");
                    else if (cmbMakingType.SelectedIndex == 0 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round((dMkRate1 * dQty), 2, MidpointRounding.AwayFromZero)) + "'  as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' per Pcs");
                    else if (cmbMakingType.SelectedIndex == 2 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero)) + "' as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' as Total");
                    else if (cmbMakingType.SelectedIndex == 3 && !string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        MessageBox.Show("Discount should not more than '" + Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero)) + "' as discount is '" + Convert.ToString(decimal.Round(dinitDiscValue, 2, MidpointRounding.AwayFromZero)) + "' as Percentage");

                    txtChangedTotAmount.Text = "";
                    //txtChangedTotAmount.Focus();
                    bIsValid = false;
                }
                else
                {
                    CheckMakingDiscountFromDB(dMkPerDisc);
                    bIsValid = true;
                }
            }

            return bIsValid;
        }

        private void txtMakingRate_Leave(object sender, EventArgs e)
        {
            iMkRateChanged = 1;
            if (string.IsNullOrEmpty(txtChangedMakingRate.Text))
                txtChangedMakingRate.Text = "0";


            cmbRateType_SelectedIndexChanged(sender, e);
            cmbMakingType_SelectedIndexChanged(sender, e);
            txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? txtTotalAmount.Text : string.Empty;
            txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? txtMakingAmount.Text : string.Empty;

            if (rdbSale.Visible == true && isCancelClick == false)
            {
                //bool bResult = getValidMakingCalValue();
                //if (!bResult)
                //{
                isValidMkDisc = false;
                // CheckRateFromDB();
                iMkRateChanged = 0;
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
                decimal dMakingQty = 0;

                /*  if (cmbMakingType.SelectedIndex == 4)
                  {
                      dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                  }
                  else
                  {
                      if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                      {
                          string dNetWt = "";
                          if (bValidSKU)
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
                  /* if (cmbMakingType.SelectedIndex == 4)
                   {
                       decimal dQty1 = 0m;
                       foreach (DataRow dr in dtIngredients.Rows)
                       {
                           if (Convert.ToString(dr["UnitId"]) == "CT" || Convert.ToString(dr["UnitId"]) == "Ct")
                               dQty1 = Convert.ToDecimal(dr["QTY"]) / 5;
                           else
                               dQty1 = Convert.ToDecimal(dr["QTY"]);

                           dMakingQty += decimal.Round(dQty1, 3, MidpointRounding.AwayFromZero);
                       }
                   }
                   else
                   {
                       if (dtIngredients != null && dtIngredients.Rows.Count > 0) // not considered Multi Metal
                       {
                           decimal dQty1 = 0m;
                           if (bIsGrossMetalCal)
                           {
                               foreach (DataRow dr in dtIngredients.Rows)
                               {

                                   if (Convert.ToString(dr["UnitId"]) == "CT" || Convert.ToString(dr["UnitId"]) == "Ct")
                                       dQty1 = Convert.ToDecimal(dr["QTY"]) / 5;
                                   else
                                       dQty1 = Convert.ToDecimal(dr["QTY"]);

                                   dMakingQty += decimal.Round(dQty1, 3, MidpointRounding.AwayFromZero);
                               }
                           }
                           else
                               dMakingQty = Convert.ToDecimal(dtIngredients.Rows[0]["QTY"]);
                       }
                       else
                       {
                           if (!string.IsNullOrEmpty(txtQuantity.Text))
                               dMakingQty = Convert.ToDecimal(txtQuantity.Text.Trim());
                       }
                   }*/

                string sSqr = "select isnull(NOEXTRAMKADDITION,0) from INVENTTABLE where itemid='" + sBaseItemID + "'";
                string sResultForNOEXTRAMKADDITION = NIM_ReturnExecuteScalar(sSqr);


                decimal dAddMk = 0;
                decimal dMinMkQty = 0m;

                string sSqr2 = "select isnull(ADDITIONALMAKING,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                string sAddMk = NIM_ReturnExecuteScalar(sSqr2);

                string sSqr3 = "select isnull(MINMAKINGQTY,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
                string sMinMkQty = NIM_ReturnExecuteScalar(sSqr3);


                dMinMkQty = Convert.ToDecimal(sMinMkQty);
                dAddMk = Convert.ToDecimal(sAddMk);

                if (bValidSKU)
                {
                    if (sResultForNOEXTRAMKADDITION == "0")
                    {
                        if (cmbMakingType.SelectedIndex == 1 || cmbMakingType.SelectedIndex == 4)
                        {
                            if (Convert.ToDecimal(dMakingQty) <= dMinMkQty && Convert.ToDecimal(dMakingQty) > 0) // 6 replace with  dMinMkQty on 260418
                            {
                                decimal dActMkRate = 0;
                                if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                                    dActMkRate = Convert.ToDecimal(txtActMakingRate.Text);

                                if (!isMkAdded)
                                {
                                    txtActMakingRate.Text = Convert.ToString(decimal.Round(dActMkRate + (dAddMk / Convert.ToDecimal(dMakingQty)), 2, MidpointRounding.AwayFromZero));
                                    isMkAdded = true;
                                }
                            }
                        }
                    }
                }

                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? Convert.ToString(Convert.ToDecimal(txtTotalAmount.Text)) : string.Empty; //need to stop later by AM + Convert.ToDecimal(txtGoldTax.Text)
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? txtMakingAmount.Text : string.Empty;

                //txtChangedMakingRate.Focus();
                // }*/
            }

        }

        private void txtLineDisc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtLineDisc.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private bool getValidMRPItemLineDiscount()
        {
            // Start : RH 01/09/2014
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();
            decimal dinitDiscValue = 0;
            string sFieldName = string.Empty;
            decimal dLineDisc = 0m;
            bool bIsValid = false;
            decimal dChangedTotAmt = 0m;
            decimal dTotAmt = 0m;

            if (!string.IsNullOrEmpty(txtLineDisc.Text))
                dLineDisc = Convert.ToDecimal(txtLineDisc.Text);
            else
                dLineDisc = 0;

            if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                dChangedTotAmt = Convert.ToDecimal(txtChangedTotAmount.Text);
            else
                dChangedTotAmt = 0;

            if (!string.IsNullOrEmpty(txtTotalAmount.Text))
                dTotAmt = Convert.ToDecimal(txtTotalAmount.Text);
            else
                dTotAmt = 0;


            if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                sFieldName = "MAXSALESPERSONSDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                sFieldName = "MAXMANAGERLEVELDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                sFieldName = "MAXMANAGERLEVEL2DISCPCT";
            else
                sFieldName = "OPENINGDISCPCT";
            if (dTotAmt != Convert.ToDecimal(txtActToAmount.Text))
            {
                if (IsRetailItem(sBaseItemID))
                {
                    if (isMRPUCP)
                        dinitDiscValue = GetDiscountFromDiscPolicy(sBaseItemID, Convert.ToDecimal(txtActToAmount.Text), sFieldName, 0);

                    if (dinitDiscValue > 0)
                    {
                        if (dChangedTotAmt > dTotAmt)
                        {
                            MessageBox.Show("Changed total amount should not greater than final amount.");
                            txtChangedTotAmount.Focus();
                            bIsValid = false;
                        }
                        else if ((dLineDisc > dinitDiscValue))
                        {
                            MessageBox.Show("Line discount percentage should not more than '" + dinitDiscValue + "'");
                            bIsValid = false;
                        }
                        else if (dLineDisc < 0)
                        {
                            MessageBox.Show("Line discount percentage should not negetive value");
                            bIsValid = false;
                        }
                        else
                            bIsValid = true;
                    }
                    else if (dLineDisc > 0)
                    {
                        MessageBox.Show("Not allowed for this item");
                        txtTotalAmount.Text = txtActToAmount.Text;
                        sMkPromoCode = "";
                        iIsMkDiscFlat = 0;
                        bIsValid = false;
                    }
                }
                else
                    bIsValid = true;
            }
            else
            {
                bIsValid = true;
            }

            return bIsValid;

        }

        private decimal GetDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet, int iFlat)
        {
            decimal dResult = 0;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sFlatQry = "";
            if (sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWDISCOUNTPOLICY.FLAT =" + iFlat + "";
            else
                sFlatQry = "";

            string commandText = "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +
                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWDISCOUNTPOLICY )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=1)" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT ,@PRODUCTCODE=PRODUCTTYPECODE" + // FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE,@COLLECTIONCODE =CollectionCode" +
                                 "  FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                //"  SELECT @PRODUCT = ITEMID FROM [INVENTTABLE] WHERE ITEMID = @PARENTITEM " +
                                 "  SELECT @CUSTCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' " +
                                 "  END ";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //1
                " FROM   CRWDISCOUNTPOLICY  " +
                " WHERE (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM) " +
                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE) " +
                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                     "     WHERE     " +
                     "     (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM ) AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE ) " +

                     "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                     "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                     "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //2
               " FROM   CRWDISCOUNTPOLICY  " +
               " WHERE (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM) " +
               " AND (CRWDISCOUNTPOLICY.CODE='') " +
               " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                    "     WHERE     " +
                    "     (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM ) AND (CRWDISCOUNTPOLICY.CODE='' ) " +

                      "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                    "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                    "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //3
                " FROM   CRWDISCOUNTPOLICY  " +
                " WHERE (CRWDISCOUNTPOLICY.ITEMID='') " +
                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE) " +
                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                     "     WHERE     " +
                     "     (CRWDISCOUNTPOLICY.ITEMID='' ) AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE ) " +

                       "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                     "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                     "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //4
               " FROM   CRWDISCOUNTPOLICY  " +
               " WHERE (CRWDISCOUNTPOLICY.ITEMID='') " +
               " AND (CRWDISCOUNTPOLICY.CODE='') " +
               " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                    "     WHERE     " +
                    "     (CRWDISCOUNTPOLICY.ITEMID='' ) AND (CRWDISCOUNTPOLICY.CODE='' ) " +

                      "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                    "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                    "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dResult = Convert.ToDecimal(reader.GetValue(0));
                    //sPromoCode = Convert.ToString(reader.GetValue(1));
                    //iIsDiscFlat = Convert.ToInt16(reader.GetValue(2));
                    sMkPromoCode = Convert.ToString(reader.GetValue(1));
                    iIsMkDiscFlat = Convert.ToInt16(reader.GetValue(2));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;
        }

        private void txtLineDisc_Leave(object sender, EventArgs e)
        {

            decimal dActTotAmt = 0m;
            decimal dLineDisc = 0m;
            decimal decimalAmount = 0m;
            decimal dManualAmt = 0m;

            if (!string.IsNullOrEmpty(txtActToAmount.Text))
                dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
            else
                dActTotAmt = 0;

            if (!string.IsNullOrEmpty(txtChangedTotAmount.Text))
                dManualAmt = Convert.ToDecimal(txtChangedTotAmount.Text); //am- Convert.ToDecimal(txtGoldTax.Text)
            else
                dManualAmt = 0;

            if (!string.IsNullOrEmpty(txtLineDisc.Text))
                dLineDisc = Convert.ToDecimal(txtLineDisc.Text);
            else
                dLineDisc = 0;

            if (dManualAmt > 0)
            {
                dLineDisc = (100 * (dActTotAmt - dManualAmt)) / dActTotAmt;
                txtLineDisc.Text = Convert.ToString(decimal.Round(dLineDisc, 2, MidpointRounding.AwayFromZero));
            }

            if (getValidMRPItemLineDiscount())
            {
                if (dActTotAmt > 0 && dLineDisc > 0)
                {
                    decimalAmount = dActTotAmt - (dActTotAmt * dLineDisc / 100);
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
                }
                if (dLineDisc == 0 && dActTotAmt > 0)
                {
                    txtTotalAmount.Text = Convert.ToString(decimal.Round(dActTotAmt, 2, MidpointRounding.AwayFromZero));
                }
            }
            //else
            //{
            //    // MessageBox.Show("Invalid line discount %.");
            //    txtLineDisc.Focus();
            //}
        }

        private bool IsValidSKU(string sSKUNo)
        {
            bool bStatus = false;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sQry = "DECLARE @ISVALID INT" +
                         " IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sSKUNo + "'" +
                         " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND RETAIL = 1) " +
                         " AND EXISTS (SELECT SkuNumber FROM SKUTableTrans WHERE SkuNumber = '" + sSKUNo + "'" +
                         " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') " +
                         " BEGIN SET @ISVALID = 1 END ELSE BEGIN SET @ISVALID = 0 END SELECT ISNULL(@ISVALID,0)";


            using (SqlCommand cmd = new SqlCommand(sQry, connection))
            {
                cmd.CommandTimeout = 0;
                bStatus = Convert.ToBoolean(cmd.ExecuteScalar());
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bStatus;

        }

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

        private bool IsAdvanceAgainstNone(string sTransId, string sStore, string sTerminal)
        {
            bool bStatus = false;

            #region block locally
            ////SqlConnection connection = new SqlConnection();
            ////if(application != null)
            ////    connection = application.Settings.Database.Connection;
            ////else
            ////    connection = ApplicationSettings.Database.LocalConnection;
            ////if(connection.State == ConnectionState.Closed)
            ////    connection.Open();

            ////string sQry = "SELECT isnull(ADVAGAINST,0) from RETAILADJUSTMENTTABLE  where TRANSACTIONID = '" + sTransId + "'";


            ////using(SqlCommand cmd = new SqlCommand(sQry, connection))
            ////{
            ////    cmd.CommandTimeout = 0;
            ////    bStatus = Convert.ToInt16(cmd.ExecuteScalar());
            ////}

            ////if(connection.State == ConnectionState.Open)
            ////    connection.Close();
            #endregion

            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;

                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("IsAdvanceAgainstNone", sTransId, sStore, sTerminal);

                    bStatus = Convert.ToBoolean(containerArray[1]);
                }
            }
            catch (Exception ex)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }

            return bStatus;

        }

        private string GetPureMetalRate()
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");//INVENTPARAMETERS

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 AND RETAIL=1"); // RATETYPE -- > GSS->Sale 4->3 on 10/06/2016 req by S.Sharma
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return Convert.ToString(sResult.Trim());
        }

        private string GetDefaultConfigId()
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();

            commandText.Append(" SELECT isnull(DEFAULTCONFIGIDGOLD,'') FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());
        }

        private decimal getConvertedRate()
        {
            decimal sRate = 0;
            if (rdbSale.Checked)
            {
                dtIngredients = new DataTable();

                if (conn.State == ConnectionState.Closed)
                    conn.Open();

                StringBuilder commandText = new StringBuilder();
                commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + ItemID.Trim() + "')");
                commandText.Append("  BEGIN  ");
                commandText.Append(" SELECT     SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID, INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
                commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID, CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
                commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
                commandText.Append(" FROM         SKULine_Posted INNER JOIN ");
                commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
                commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
                commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
                commandText.Append("  AND  [SkuNumber] = '" + ItemID.Trim() + "' ORDER BY X.METALTYPE END ");


                SqlCommand command = new SqlCommand(commandText.ToString(), conn);
                command.CommandTimeout = 0;

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    dtIngredients.Load(reader);
                }

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                {
                    #region // Stone Discount

                    dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                    dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                    dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                    dtIngredients.Columns.Add("CTYPE", typeof(int));

                    #endregion

                    txtRate.Text = string.Empty;
                    dtIngredientsClone = new DataTable();
                    dtIngredientsClone = dtIngredients.Clone();

                    foreach (DataRow drClone in dtIngredients.Rows)
                    {
                        if (isMRPUCP)
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

                    if (!isMRPUCP)
                    {
                        decimal dTotWt = 0m;
                        if (bIsGrossMetalCal)
                        {
                            foreach (DataRow dr in dtIngredientsClone.Rows)
                            {
                                dTotWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                            }
                        }

                        foreach (DataRow dr in dtIngredientsClone.Rows)
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
                            string sCalcType = "";

                            if (Convert.ToDecimal(dr["PCS"]) > 0)
                                dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                            else
                                dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);


                            if ((IsGSSTransaction) && (dGSSMaturityQty > 0) && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                            {
                                sRate = Convert.ToDecimal(getGSSRate(GrWeight, ConfigID, Convert.ToDecimal(GrWeight)));
                            }
                            else if (IsSaleAdjustment   // Avg Gold Rate Adjustment
                                    && (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver
                                        || Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum))
                            {
                                //string sCurrentRate = getRateFromMetalTable();
                                decimal dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(ConfigID, Convert.ToDecimal(GrWeight)), 3, MidpointRounding.AwayFromZero);

                                //decimal dPureVal = dConvertedToFixingQty * dSaleAdjustmentAvgGoldRate;
                                //sRate = Convert.ToDecimal(decimal.Round(dPureVal / Convert.ToDecimal(GrWeight), 2, MidpointRounding.AwayFromZero));

                                //sRate = Convert.ToDecimal(getAdjustmentRate(GrWeight, ConfigID));
                                return dConvertedToFixingQty;
                            }
                        }
                    }
                }
            }
            return sRate;
        }

        private DataTable GetOGrepairInfo()
        {
            string sTableName = "REPOG" + "" + ApplicationSettings.Database.TerminalID;

            DataTable dtCard = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");

            commandText.Append(" BEGIN  SELECT ISNULL(QTY,0) AS QTY, ");
            commandText.Append(" ISNULL(INVENTBATCHID,'') AS INVENTBATCHID FROM " + sTableName + " END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText.ToString(), conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtCard);
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dtCard;
        }

        private void DropOGrepairInfo()
        {
            string sTableName = "REPOG" + "" + ApplicationSettings.Database.TerminalID;

            DataTable dtCard = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            string sQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')" +
                          " BEGIN  DROP TABLE " + sTableName + " END ";

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(sQry.ToString(), conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtCard);
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        private int GetCustOrderLineNoBookedItemWise(string sOrdNum, string sItemId)
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            string commandText = string.Empty;

            commandText = "select LINENUM from CUSTORDER_DETAILS " +
                        " where ORDERNUM='" + sOrdNum + "' and itemid='" + sItemId + "'" +
                        " union " +
                        " select CUSTORDERLINENUM  LINENUM from SKUTable_Posted where CUSTORDERNUM ='" + sOrdNum + "' and SkuNumber='" + sItemId + "'";


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            int sResult = 0;
            sResult = Convert.ToInt16(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;
        }

        private void getMultiAdjOrderNo(string sTableName, ref string sOrder, ref string sCust, ref int OlineNum)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            sbQuery.Append(" SELECT ISNULL(ORDERNO,'') AS ORDERNO,ISNULL(CUSTACC,'') AS CUSTACC,isnull(LINENUM,0) LINENUM FROM " + sTableName + "");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sOrder = Convert.ToString(reader.GetValue(0));
                    sCust = Convert.ToString(reader.GetValue(1));
                    OlineNum = Convert.ToInt16(reader.GetValue(2));
                }
            }
            reader.Close();
            reader.Dispose();
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void dropMultiAdjOrderNo(string sTableName)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            sbQuery.Append(" drop table " + sTableName + "");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(sbQuery.ToString(), conn);
            command.CommandTimeout = 0;
            command.ExecuteScalar();

            if (connection.State == ConnectionState.Open)
                connection.Close();
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

            int iResult = 0;
            iResult = Convert.ToInt16(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iResult;
        }

        private string GetBookedOrderNo(string sSKU, string sCustId, string sOrderNo, ref int iLineNum)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string sResult = "";

            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                StringBuilder sbQuery = new StringBuilder();

                //=======================================Subha==========================================
                sbQuery.Append("select distinct ORDERNUMBER,LineNum FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE SKUNUMBER='" + sSKU + "' AND CUSTOMERID='" + sCustId + "' AND DELIVERED=0 AND RELEASED = 0 and ORDERNUMBER='" + sOrderNo + "'");
                sbQuery.Append("Union select CUSTORDERNUM ORDERNUMBER, CUSTORDERLINENUM  LineNum from SKUTable_Posted where SKUNUMBER='" + sSKU + "' and CUSTORDERNUM='" + sOrderNo + "'");


                SqlCommand cmd = new SqlCommand(sbQuery.ToString(), conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sResult = Convert.ToString(reader.GetValue(0));
                        iLineNum = Convert.ToInt16(reader.GetValue(1));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return sResult;
        }

        private void txtLineDisc_TextChanged(object sender, EventArgs e)
        {
            txtLineDisc_Leave(sender, e);
        }

        private DataTable BookedSKU(string orderNum, string account)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select distinct SKUNUMBER,LineNum FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE" +
                                      " ORDERNUMBER='" + orderNum + "' AND CUSTOMERID='" + account + "' AND DELIVERED=0 AND RELEASED = 0";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                if (SqlCon.State == ConnectionState.Open)
                    SqlCon.Close();

                return CustBalDt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void txtChangedMakingRate_KeyDown(object sender, KeyEventArgs e)
        {
            txtChangedTotAmount.Text = "";
        }

        private void BindOGIngredient()
        {
            if (rdbSale.Checked == false)
            {
                dtIngredients = new DataTable();

                if (dtExtndPurchExchng != null)
                {
                    if ((dtExtndPurchExchng.Rows.Count > 0))
                    {
                        #region getNew table data
                        decimal dDmdPcs = 0m;
                        string sDmdUnit = "";
                        decimal dDmdWt = 0m;
                        decimal dDmdRate = 0m;
                        decimal dDmdAmt = 0m;
                        decimal dStnPcs = 0m;
                        string sStnUnit = "";
                        decimal dStnAmt = 0m;
                        decimal dStnRate = 0m;
                        decimal dStnWt = 0m;
                        decimal dNetRate = 0m;
                        decimal dNetWt = 0m;
                        decimal dNetAmt = 0m;
                        string sNetUnit = "";
                        decimal dNetPcs = 0m;

                        decimal dpStnPcs = 0m;
                        string spStnUnit = "";
                        decimal dpStnAmt = 0m;
                        decimal dpStnRate = 0m;
                        decimal dpStnWt = 0m;


                        bool bValidOgIng = false;

                        if (!string.IsNullOrEmpty(Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDPCS"])))
                        {
                            dDmdPcs = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["DMDPCS"]);
                            sDmdUnit = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDUNIT"]);
                            dDmdWt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["DMDWT"]);
                            dDmdRate = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["DMDRATE"]);
                            dDmdAmt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["DMDAMOUNT"]);
                        }
                        if (!string.IsNullOrEmpty(Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEPCS"])))
                        {
                            dStnPcs = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["STONEPCS"]);
                            sStnUnit = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEUNIT"]);
                            dStnAmt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"]);
                            dStnRate = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["STONERATE"]);
                            dStnWt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["STONEWT"]);
                        }

                        dNetPcs = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["PCS"]);
                        dNetRate = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["NETRATE"]);
                        dNetWt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["NETWT"]);
                        dNetAmt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["NETAMOUNT"]);
                        sNetUnit = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETUNIT"]);


                        dpStnPcs = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["OGPSTONEPCS"]);
                        spStnUnit = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEUNIT"]);
                        dpStnAmt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["OGPSTONEAMOUNT"]);
                        dpStnRate = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["OGPSTONERATE"]);
                        dpStnWt = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["OGPSTONEWT"]);

                        if (conn.State == ConnectionState.Closed)
                            conn.Open();

                        StringBuilder commandText = new StringBuilder();
                        commandText.Append("select '' SkuNumber, ItemID,InventSizeID,");
                        commandText.Append(" InventColorID,ConfigID,InventBatchID,METALTYPE,PRECIOUS");
                        commandText.Append(" from OGIngredientMaster");


                        SqlCommand command = new SqlCommand(commandText.ToString(), conn);
                        command.CommandTimeout = 0;
                        using (SqlDataReader readerFixRateIngr = command.ExecuteReader())
                        {
                            dtIngredients.Load(readerFixRateIngr);
                        }

                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                        #endregion

                        if (dtIngredients != null && dtIngredients.Rows.Count > 0)
                        {
                            int iMetalType = 0;
                            iMetalType = GetMetalType(sBaseItemID);

                            #region
                            dtIngredients.Columns.Add("IngrdDiscType", typeof(int));
                            dtIngredients.Columns.Add("IngrdDiscAmt", typeof(decimal));
                            dtIngredients.Columns.Add("IngrdDiscTotAmt", typeof(decimal));
                            dtIngredients.Columns.Add("CTYPE", typeof(int));
                            dtIngredients.Columns.Add("UnitID", typeof(string));
                            dtIngredients.Columns.Add("PCS", typeof(decimal));
                            dtIngredients.Columns.Add("QTY", typeof(decimal));
                            dtIngredients.Columns.Add("CValue", typeof(decimal));
                            dtIngredients.Columns.Add("Rate", typeof(decimal));
                            dtIngredients.Columns.Add("InventDimID", typeof(string));

                            dtIngredientsClone = new DataTable();
                            dtIngredientsClone = dtIngredients.Clone();

                            foreach (DataRow drClone in dtIngredients.Rows)
                            {
                                if (iMetalType == Convert.ToInt16(drClone["METALTYPE"]))
                                {
                                    bValidOgIng = true;
                                    break;
                                }
                            }

                            if (bValidOgIng)
                            {
                                foreach (DataRow drClone in dtIngredients.Rows)
                                {
                                    int iIngMetalType = Convert.ToInt16(drClone["METALTYPE"]);

                                    if (iIngMetalType == iMetalType)
                                    {
                                        string sInventDimId = GetInventDimId("", "", sBaseConfigID, Convert.ToString(drClone["ItemID"]));
                                        if (dNetWt > 0)
                                        {
                                            dtIngredients.Columns["Rate"].ReadOnly = false;
                                            dtIngredients.Columns["PCS"].ReadOnly = false;
                                            dtIngredients.Columns["QTY"].ReadOnly = false;
                                            dtIngredients.Columns["CValue"].ReadOnly = false;
                                            dtIngredients.Columns["UnitID"].ReadOnly = false;
                                            dtIngredients.Columns["ConfigID"].ReadOnly = false;
                                            dtIngredients.Columns["InventDimID"].ReadOnly = false;

                                            drClone["InventDimID"] = sInventDimId;
                                            drClone["ConfigID"] = sBaseConfigID;
                                            drClone["Rate"] = dNetRate;
                                            drClone["PCS"] = 0;// dNetPcs;
                                            drClone["QTY"] = dNetWt;
                                            drClone["CValue"] = dNetAmt;
                                            drClone["CTYPE"] = 0;
                                            drClone["UnitID"] = sNetUnit;
                                            drClone["IngrdDiscType"] = 0;
                                            drClone["IngrdDiscAmt"] = 0;
                                            drClone["IngrdDiscTotAmt"] = 0;

                                            dtIngredientsClone.ImportRow(drClone);
                                        }
                                    }
                                    if (iIngMetalType == (int)MetalType.LooseDmd)
                                    {
                                        string sInventDimId = GetInventDimId(Convert.ToString(drClone["InventSizeID"]), Convert.ToString(drClone["InventColorID"]), "", Convert.ToString(drClone["ItemID"]));
                                        if (dDmdPcs > 0)
                                        {
                                            drClone["InventDimID"] = sInventDimId;
                                            drClone["Rate"] = dDmdRate;
                                            drClone["PCS"] = dDmdPcs;
                                            drClone["QTY"] = dDmdWt;
                                            drClone["CValue"] = dDmdAmt;
                                            drClone["CTYPE"] = 0;
                                            drClone["UnitID"] = sDmdUnit;
                                            drClone["IngrdDiscType"] = 0;
                                            drClone["IngrdDiscAmt"] = 0;
                                            drClone["IngrdDiscTotAmt"] = 0;

                                            dtIngredientsClone.ImportRow(drClone);
                                        }
                                    }
                                    if (iIngMetalType == (int)MetalType.Stone && Convert.ToInt16(drClone["PRECIOUS"]) == 0)
                                    {
                                        string sInventDimId = GetInventDimId(Convert.ToString(drClone["InventSizeID"]), Convert.ToString(drClone["InventColorID"]), "", Convert.ToString(drClone["ItemID"]));
                                        if (dStnPcs > 0)
                                        {
                                            drClone["InventDimID"] = sInventDimId;
                                            drClone["Rate"] = dStnRate;
                                            drClone["PCS"] = dStnPcs;
                                            drClone["QTY"] = dStnWt;
                                            drClone["CValue"] = dStnAmt;
                                            drClone["CTYPE"] = 0;
                                            drClone["UnitID"] = sStnUnit;
                                            drClone["IngrdDiscType"] = 0;
                                            drClone["IngrdDiscAmt"] = 0;
                                            drClone["IngrdDiscTotAmt"] = 0;

                                            dtIngredientsClone.ImportRow(drClone);
                                        }
                                    }

                                    if (iIngMetalType == (int)MetalType.Stone && Convert.ToInt16(drClone["PRECIOUS"]) == 1)
                                    {
                                        string sInventDimId = GetInventDimId(Convert.ToString(drClone["InventSizeID"]), Convert.ToString(drClone["InventColorID"]), "", Convert.ToString(drClone["ItemID"]));
                                        if (dStnPcs > 0)
                                        {
                                            drClone["InventDimID"] = sInventDimId;
                                            drClone["Rate"] = dpStnRate;
                                            drClone["PCS"] = dpStnPcs;
                                            drClone["QTY"] = dpStnWt;
                                            drClone["CValue"] = dpStnAmt;
                                            drClone["CTYPE"] = 0;
                                            drClone["UnitID"] = spStnUnit;
                                            drClone["IngrdDiscType"] = 0;
                                            drClone["IngrdDiscAmt"] = 0;
                                            drClone["IngrdDiscTotAmt"] = 0;

                                            dtIngredientsClone.ImportRow(drClone);
                                        }
                                    }

                                    dtIngredientsClone.AcceptChanges();

                                }
                            }
                            #endregion
                        }

                        if (conn.State == ConnectionState.Open)
                            conn.Close();

                    }
                }
            }
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

        private void txtPCS_Leave(object sender, EventArgs e)
        {
            //txtRate_TextChanged(sender, e);
        }

        private void txtRate_Leave(object sender, EventArgs e)
        {
            if (txtRate.Enabled == true)
            {
                cmbRateType_SelectedIndexChanged(sender, e);
                cmbMakingType_SelectedIndexChanged(sender, e);
                txtTotalAmount.Text = !string.IsNullOrEmpty(txtTotalAmount.Text) ? decimal.Round(Convert.ToDecimal(txtTotalAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                txtMakingAmount.Text = !string.IsNullOrEmpty(txtMakingAmount.Text) ? decimal.Round(Convert.ToDecimal(txtMakingAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                //txtActToAmount.Text = txtTotalAmount.Text;
            }
        }

        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        #region NIM_GetConvertionValue //24/09/2014
        public string NIM_GetConvertionValue(string _frmUnit, string _toUnit, decimal _frmValue, int iMetalType)
        {
            string sqlString = string.Empty;
            string frmUnit = string.Empty;
            string toUnit = string.Empty;
            string convValue = "1";
            string defUnit = string.Empty;
            string sBlank = "";
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

            NIM_ReturnMoreValues(Convert.ToString(sqlString), out frmUnit, out toUnit, out convValue, out sBlank, out sBlank);

            if (!string.IsNullOrEmpty(defUnit) && !string.IsNullOrEmpty(_frmUnit))
            {
                if (defUnit.ToUpper() == _frmUnit.ToUpper())
                    return Convert.ToString(_frmValue);
                else
                {
                    if (Convert.ToDecimal(convValue) > 0)
                        return !string.IsNullOrEmpty(Convert.ToString(_frmValue * Convert.ToDecimal(convValue))) ? decimal.Round(Convert.ToDecimal(Convert.ToString(_frmValue * Convert.ToDecimal(convValue))),5, MidpointRounding.AwayFromZero).ToString() : string.Empty;
                    //Convert.ToString(_frmValue * Convert.ToDecimal(convValue));
                    else
                        return "0";
                }
            }
            else
                return Convert.ToString(_frmValue);
        }
        #endregion
        public void NIM_ReturnMoreValues(string query, out string val1, out string val2, out string val3, out string val4, out string val5)
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
                    string ReturnVal4 = "0";
                    string ReturnVal5 = "";
                    while (reader.Read())
                    {
                        ReturnVal1 = Convert.ToString(reader.GetValue(0));
                        ReturnVal2 = Convert.ToString(reader.GetValue(1));
                        ReturnVal3 = Convert.ToString(reader.GetValue(2));
                        ReturnVal4 = Convert.ToString(reader.GetValue(3));
                        ReturnVal5 = Convert.ToString(reader.GetValue(4));
                    }
                    reader.Close();
                    val1 = Convert.ToString(ReturnVal1);
                    val2 = Convert.ToString(ReturnVal2);
                    val3 = Convert.ToString(ReturnVal3);
                    val4 = Convert.ToString(ReturnVal4);
                    val5 = Convert.ToString(ReturnVal5);
                }
                else
                {
                    reader.Close();
                    val1 = "0";
                    val2 = "0";
                    val3 = "0";
                    val4 = "0";
                    val5 = "";
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

        private void txtWtDiffDiscQty_TextChanged(object sender, EventArgs e)
        {
            decimal dWtDiffDisc = 0m;
            decimal dWtDiffDiscVal = 0m;
            if (!string.IsNullOrEmpty(txtWtDiffDiscQty.Text))
                dWtDiffDisc = Convert.ToDecimal(txtWtDiffDiscQty.Text);
            else
                dWtDiffDisc = 0;

            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;
            decimal dActMkRate = 0m;
            decimal dChangeMkrate = 0m;

            if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
            else
                dMkRate1 = 0;

            if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
            else
                dMkRate = 0;

            if (dWtDiffDisc > 0)
            {
                if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                    dActMkRate = Convert.ToDecimal(txtActMakingRate.Text);
                else
                    dActMkRate = 0;

                if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                    dChangeMkrate = Convert.ToDecimal(txtChangedMakingRate.Text);
                else
                    dChangeMkrate = 0;

                if (!string.IsNullOrEmpty(txtWtDiffDiscQty.Text) && txtWtDiffDiscQty.Text != ".")
                    dWtDiffDisc = Convert.ToDecimal(txtWtDiffDiscQty.Text);
                else
                    dWtDiffDisc = 0;

                if (dActMkRate < dChangeMkrate)
                {
                    if (dWtDiffDisc > 0)
                    {
                        if (cmbMakingType.SelectedIndex == 3)//(int)MakingType.Percentage
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text) * dChangeMkrate) / 100), 2, MidpointRounding.AwayFromZero);
                        else if (cmbMakingType.SelectedIndex == 0)//(int)MakingType.Pieces
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);
                        else if (cmbMakingType.SelectedIndex == 2)//(int)MakingType.Tot
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);
                        else if (cmbMakingType.SelectedIndex == 1)//(int)MakingType.Weight
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dChangeMkrate)), 2, MidpointRounding.AwayFromZero);
                        else if (cmbMakingType.SelectedIndex == 5)//(int)MakingType.Gross
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dChangeMkrate)), 2, MidpointRounding.AwayFromZero);
                    }
                }
                else
                {

                    if (dWtDiffDisc > 0)
                    {
                        if (cmbMakingType.SelectedIndex == 3)//(int)MakingType.Percentage
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text) * dActMkRate) / 100), 2, MidpointRounding.AwayFromZero);//(dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text) * dActMkRate) / 100);
                        else if (cmbMakingType.SelectedIndex == 0)//(int)MakingType.Pieces
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);// (dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text));
                        else if (cmbMakingType.SelectedIndex == 2)//(int)MakingType.Tot
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)), 2, MidpointRounding.AwayFromZero);//(dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text));
                        else if (cmbMakingType.SelectedIndex == 1)//(int)MakingType.Weight
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate)), 2, MidpointRounding.AwayFromZero);//(dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate));
                        else if (cmbMakingType.SelectedIndex == 5)//(int)MakingType.Gross
                            dWtDiffDiscVal = decimal.Round((dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate)), 2, MidpointRounding.AwayFromZero);// (dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)) + ((dWtDiffDisc * dActMkRate));
                    }
                }
            }
            else
            {
                // txtChangedMakingRate.Enabled = true;
                txtWtDiffDiscAmt.Text = "0";
            }

            cmbRateType_SelectedIndexChanged(sender, e);
            CalWtDiffAndChangeMakingDisc(dMkRate1, dMkRate, dWtDiffDiscVal);
            CalcMakingAtSelectedIndexChange();


            //decimal dActTotAmt = 0m;
            //decimal dWtDiffDisc = 0m;
            //decimal dWtDiffDiscVal = 0m;
            //decimal dExistingMkDisc = 0m;


            //if (!string.IsNullOrEmpty(txtMakingDiscTotAmt.Text))
            //    dExistingMkDisc = Convert.ToDecimal(txtMakingDiscTotAmt.Text);
            //else
            //    dExistingMkDisc = 0;

            //if (!string.IsNullOrEmpty(txtActToAmount.Text))
            //    dActTotAmt = Convert.ToDecimal(txtActToAmount.Text);
            //else
            //    dActTotAmt = 0;

            //if (!string.IsNullOrEmpty(txtWtDiffDiscQty.Text))
            //    dWtDiffDisc = Convert.ToDecimal(txtWtDiffDiscQty.Text);
            //else
            //    dWtDiffDisc = 0;

            //if (dWtDiffDisc > 0)
            //{
            //    dWtDiffDiscVal = (100 * (dActTotAmt - (dWtDiffDisc * Convert.ToDecimal(lblMetalRatesShow.Text)))) / dActTotAmt;
            //}
            //txtMakingDiscTotAmt.Text = Convert.ToString(decimal.Round(dExistingMkDisc + dWtDiffDiscVal, 2, MidpointRounding.AwayFromZero)); 

        }

        private void txtWtDiffDiscQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtWtDiffDiscQty.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtWtDiffDiscQty_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string sSqr3 = "select isnull(QtyDiffDiscTolerance,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
            string sQtyDiffDiscTollerence = NIM_ReturnExecuteScalar(sSqr3);
            string sEnterdValue = "0";
            sEnterdValue = txtWtDiffDiscQty.Text;

            if (!string.IsNullOrEmpty(sQtyDiffDiscTollerence) && !string.IsNullOrEmpty(sEnterdValue))
            {
                if (Convert.ToDecimal(sQtyDiffDiscTollerence) > 0 && Convert.ToDecimal(sEnterdValue) > 0)
                {
                    if (Convert.ToDecimal(sQtyDiffDiscTollerence) < Convert.ToDecimal(sEnterdValue))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage("Weight differents can not beyond " + sQtyDiffDiscTollerence + " limit.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(message);
                            e.Cancel = true;
                            return;
                        }
                    }
                }
            }
        }

        private void LockSKU(string sSkuNum, int iLock)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE SKUTABLETRANS SET ISLOCKED=" + iLock + " WHERE SKUNUMBER='" + sSkuNum + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Clear();
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void lblMetalRatesShow_Click(object sender, EventArgs e)
        {

        }

        private void txtChangedMakingRate_Leave(object sender, EventArgs e)
        {
            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;

            if (!string.IsNullOrEmpty(txtActMakingRate.Text))
                dMkRate1 = Convert.ToDecimal(txtActMakingRate.Text);
            else
                dMkRate1 = 0;

            if (!string.IsNullOrEmpty(txtChangedMakingRate.Text))
                dMkRate = Convert.ToDecimal(txtChangedMakingRate.Text);
            else
                dMkRate = 0;

            txtChangedMakingRate.Enabled = false;

            if (dMkRate > dMkRate1)
            {
                CalcMakingAtSelectedIndexChange();
            }
            else if (dMkRate < dMkRate1)
            {
                MessageBox.Show("Changed making rate cannot be less tahn actual making rate.");
                txtChangedMakingRate.Text = txtActMakingRate.Text;
            }
        }

        private void txtChangedMakingRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtChangedMakingRate.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtWtDiffDiscQty_Leave(object sender, EventArgs e)
        {
            CalcMakingAtSelectedIndexChange();
        }
    }
}
