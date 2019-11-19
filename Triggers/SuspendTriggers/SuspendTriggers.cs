//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Data;
using System;
using LSRetailPosis.Transaction;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Microsoft.Dynamics.Retail.Pos.SuspendTriggers
{
    [Export(typeof(ISuspendTrigger))]
    public class SuspendTriggers : ISuspendTrigger
    {
        //Start: Nim
        #region Nim
        string sDesignNo = "-";
        string sComplexity = "-";
        string sGrossWt = "0";
        string sProdDesc = "-";
        string sSKUNo = "-";
        decimal dWastagePer = 0;
        decimal dMkDisc = 0;
        int sMkType = 0;

        #region enum MetalType
        enum MetalType
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

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion

        private void UpdateSKUWithSuspned(int iSuspended, string sSKU, string sCustAcc)
        {
            SqlConnection connection = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE SKUTableTrans SET ISSUSPENDED=" + iSuspended + ", SuspendCustAccount='" + sCustAcc + "' where skunumber= '" + sSKU + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private string getValue(string sSqlString) // passing sql query string return  one string value
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = sSqlString;

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private void GetDefaulParamtUnit(ref string sDefaulUnitId, ref string sDefaultDmdUnit)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;


            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT  ISNULL(DEFAULTUNITID,'')  DEFAULTUNITID ,isnull(CRWDefaultDiamondUnitId,'') CRWDefaultDiamondUnitId" +
                                " FROM INVENTPARAMETERS WHERE DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'";


            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sDefaulUnitId = Convert.ToString(reader.GetValue(0));
                        sDefaultDmdUnit = Convert.ToString(reader.GetValue(1));
                    }
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }
        #endregion
        //ENd:Nim

        #region Constructor - Destructor

        public SuspendTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for SuspendTriggers are reserved at 62000 - 62999

        }

        #endregion

        #region ISuspendTriggers Members

        public void OnSuspendTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.OnSuspendTransaction", "On the suspension of a transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void OnRecallTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.OnRecallTransaction", "On the recall of a suspended transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region ISuspendTriggers Members

        public void PreSuspendTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Start:Nim
            //System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline
            //    = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
            //bool isServiceItem = false;
            //foreach(var sale in saleline)
            //{
            //    if(sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
            //    {
            //        isServiceItem = true;
            //        break;
            //    }
            //}
            //if(isServiceItem)
            //{
            //    preTriggerResult.MessageId = 62999;
            //    preTriggerResult.ContinueOperation = false;
            //    return;
            //}

            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans.PartnerData.SingaporeTaxCal == "1")
            {
                MessageBox.Show("If OG tax calculation is applied, transaction cannot be suspended", "Transaction cannot be suspended.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                preTriggerResult.ContinueOperation = false;
                return;
            }
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PreSuspendTransaction", "Prior to the suspension of a transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostSuspendTransaction(IPosTransaction posTransaction)
        {
            //Start:Nim
            string sDmdUnit = "";
            string sStnUnit = "";
            string sMetalUnit = "";

            int iOGP = 0;
            decimal dGoldQty = 0;
            decimal dDmdQty = 0;
            decimal dStnQty = 0;
            decimal dPltQty = 0;
            decimal dPaladQty = 0;
            decimal dSilQty = 0;
            bool bIsBulkItem = false;
            string sTaxCode = string.Empty;
            decimal dTaxRatePct = 0;
            decimal dTaxAmount = 0;


            #region
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans != null)
            {
                #region Table making
                DataTable dtDetail = new DataTable("Detail");
                DataTable dtIngredient = new DataTable("Ingredient");
                DataTable dtMetalTotal = new DataTable("MetalTotal");
                DataRow drDtl;
                DataRow drIngrd;
                DataRow drMetalTotal;


                dtDetail.Columns.Add("ITEMID", typeof(string));
                dtDetail.Columns.Add("LINENUM", typeof(int));
                dtDetail.Columns.Add("MAKINGAMOUNT", typeof(decimal));
                dtDetail.Columns.Add("MakingDisc", typeof(decimal));
                dtDetail.Columns.Add("WastageAmount", typeof(decimal));
                dtDetail.Columns.Add("TAXAMT", typeof(decimal));
                dtDetail.Columns.Add("TAXPER", typeof(decimal));
                dtDetail.Columns.Add("TAXCODE", typeof(string));
                dtDetail.Columns.Add("MKRATE", typeof(decimal));
                dtDetail.Columns.Add("WASTPER", typeof(decimal));
                dtDetail.Columns.Add("WASTPERWT", typeof(decimal));
                dtDetail.Columns.Add("MKTYPE", typeof(Int16));
                dtDetail.Columns.Add("TCSAMT", typeof(decimal));
                dtDetail.Columns.Add("TCSPER", typeof(decimal));
                dtDetail.Columns.Add("TCSCODE", typeof(string));
                dtDetail.Columns.Add("NETCHARGEVALUE", typeof(decimal));

                dtDetail.AcceptChanges();

                dtIngredient.Columns.Add("SKUHEADER", typeof(string));
                dtIngredient.Columns.Add("SKUNUMBER", typeof(string));
                dtIngredient.Columns.Add("ITEMID", typeof(string));
                dtIngredient.Columns.Add("LINENUM", typeof(int));
                dtIngredient.Columns.Add("REFLINENUM", typeof(int));

                dtIngredient.Columns.Add("InventSizeID", typeof(string));
                dtIngredient.Columns.Add("InventColorID", typeof(string));
                dtIngredient.Columns.Add("ConfigID", typeof(string));

                dtIngredient.Columns.Add("UnitID", typeof(string));
                dtIngredient.Columns.Add("METALTYPE", typeof(int));
                dtIngredient.Columns.Add("NETQTY", typeof(decimal));
                dtIngredient.Columns.Add("QTY", typeof(decimal));
                dtIngredient.Columns.Add("PCS", typeof(decimal));
                dtIngredient.Columns.Add("CRATE", typeof(decimal));
                dtIngredient.Columns.Add("CVALUE", typeof(decimal));
                dtIngredient.Columns.Add("INGRDDISCAMT", typeof(decimal));


                dtIngredient.AcceptChanges();

                dtMetalTotal.Columns.Add("Metal", typeof(string));
                dtMetalTotal.Columns.Add("Value", typeof(decimal));
                dtMetalTotal.AcceptChanges();
                #endregion

                #region table wise calculation
                int i = 1;
                int J = 1;
                bool bPrintAll = true;
                //using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Print all ?", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk))
                //{
                //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //    if (dialog.DialogResult == DialogResult.Yes)
                //    {
                //        bPrintAll = true;
                //    }
                //}

                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    decimal dNetWt = 0;
                    DataSet dsIngredients = new DataSet();

                    bool bNetChargable = false;
                    //(bPrintAll == false && saleLineItem.PartnerData.EstimationIsDone == 0)

                    if (bPrintAll)
                    {
                        #region All item print
                        if (!saleLineItem.Voided) //dBillAmt >= dAmount &&
                        {

                            if (!ValidateServiceItem(saleLineItem.ItemId))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)) && Convert.ToString(saleLineItem.PartnerData.Ingredients) != "0")
                                {
                                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                    dsIngredients.ReadXml(reader);
                                }
                                else
                                {
                                    dsIngredients = null;
                                }
                            }
                            else
                                dsIngredients = null;

                            decimal dTotNetChargeAmt = 0;
                            //if (!ValidateServiceItem(saleLineItem.ItemId))
                            //{
                            //    if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                            //    {
                            //        if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                            //        {
                            //            iOGP = 0;
                            //            int iGrd = 1;

                            //            foreach (DataRow dr in dsIngredients.Tables[0].Rows)
                            //            {
                            //                dTotNetChargeAmt += decimal.Round(Convert.ToDecimal(dr["NETCHARGEVALUE"]), 2, MidpointRounding.AwayFromZero);
                            //            }

                            //        }
                            //    }
                            //}

                            drDtl = dtDetail.NewRow();
                            sSKUNo = saleLineItem.ItemId;
                            //dWastagePer = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastagePercentage), 2, MidpointRounding.AwayFromZero);
                            ////dMkDisc = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingDisc), 2, MidpointRounding.AwayFromZero); // commented by RM and Suman on 25/12/2014
                            //dMkDisc = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingRate), 2, MidpointRounding.AwayFromZero);
                            //sMkType = Convert.ToInt16(saleLineItem.PartnerData.MakingType);
                            string sTCSPer = "0"; //getValue("SELECT CAST(NIM_TCSPERCENTAGE AS decimal (6,2)) AS QTY  FROM RETAILPARAMETERS ");

                            dTaxAmount = decimal.Round(Convert.ToDecimal(saleLineItem.TaxAmount), 2, MidpointRounding.AwayFromZero);
                            dTaxRatePct = decimal.Round(Convert.ToDecimal(saleLineItem.TaxRatePct), 2, MidpointRounding.AwayFromZero);
                            sTaxCode = saleLineItem.TaxGroupId;

                            //dTcsAmount = decimal.Round(Convert.ToDecimal(saleLineItem.NetAmount), 2, MidpointRounding.AwayFromZero);
                            //dTcsRatePct = decimal.Round(Convert.ToDecimal(sTCSPer), 2, MidpointRounding.AwayFromZero);
                            //sTcsCode = saleLineItem.Description;

                            drDtl["ITEMID"] = saleLineItem.ItemId;
                            drDtl["LINENUM"] = i;
                            if (!ValidateServiceItem(saleLineItem.ItemId))
                            {
                                drDtl["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount), 2, MidpointRounding.AwayFromZero);
                                drDtl["MakingDisc"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingTotalDiscount), 2, MidpointRounding.AwayFromZero);
                                drDtl["WastageAmount"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount), 2, MidpointRounding.AwayFromZero);

                                if (Convert.ToDecimal(saleLineItem.PartnerData.WastagePercentage) > 0)
                                    drDtl["WASTPER"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastagePercentage), 2, MidpointRounding.AwayFromZero);
                                else
                                    drDtl["WASTPERWT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastageQty), 2, MidpointRounding.AwayFromZero);
                                drDtl["MKRATE"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingRate), 2, MidpointRounding.AwayFromZero);
                                drDtl["MKTYPE"] = Convert.ToInt16(saleLineItem.PartnerData.MakingType);
                                drDtl["TCSAMT"] = 0;
                                drDtl["TCSPER"] = 0;
                                drDtl["TCSCODE"] = 0;
                            }
                            else
                            {
                                drDtl["MAKINGAMOUNT"] = 0;
                                drDtl["MakingDisc"] = 0;
                                drDtl["WastageAmount"] = 0;
                                drDtl["WASTPER"] = 0;
                                drDtl["WASTPERWT"] = 0;
                                drDtl["MKRATE"] = 0;
                                drDtl["MKTYPE"] = 0;
                                drDtl["TCSAMT"] = 0;
                                drDtl["TCSPER"] = 0;
                                drDtl["TCSCODE"] = "";
                            }
                            drDtl["TAXAMT"] = dTaxAmount;
                            drDtl["TAXPER"] = dTaxRatePct;
                            drDtl["TAXCODE"] = sTaxCode;
                            drDtl["NETCHARGEVALUE"] = dTotNetChargeAmt;

                            dtDetail.Rows.Add(drDtl);
                            dtDetail.AcceptChanges();
                            if (!ValidateServiceItem(saleLineItem.ItemId))
                            {
                                if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                                {
                                    if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                    {
                                        iOGP = 0;
                                        int iGrd = 1;

                                        #region Ingredient  Sale
                                        foreach (DataRow dr in dsIngredients.Tables[0].Rows)
                                        {
                                            drIngrd = dtIngredient.NewRow();

                                            sGrossWt = getValue("SELECT CAST(QTY AS decimal (16,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                                            sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                            sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                                            sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
                                            string sCreatedString = sProdDesc + "  " + System.Environment.NewLine + "  " + Convert.ToString(dr["SKUNUMBER"]) + "-" + sDesignNo + System.Environment.NewLine + "  " + Convert.ToString(dr["ConfigID"]) + "  Gross Wt : " + sGrossWt; //" (Complexity " + sComplexity + ") " +

                                            drIngrd["SKUHEADER"] = sCreatedString;

                                            drIngrd["SKUNUMBER"] = Convert.ToString(dr["SKUNUMBER"]);
                                            drIngrd["ITEMID"] = Convert.ToString(dr["ITEMID"]);
                                            drIngrd["LINENUM"] = iGrd;

                                            drIngrd["REFLINENUM"] = i;
                                            drIngrd["InventSizeID"] = Convert.ToString(dr["InventSizeID"]);
                                            drIngrd["InventColorID"] = Convert.ToString(dr["InventColorID"]);
                                            drIngrd["ConfigID"] = Convert.ToString(dr["ConfigID"]);

                                            drIngrd["UnitID"] = Convert.ToString(dr["UnitID"]);
                                            drIngrd["METALTYPE"] = Convert.ToInt32(dr["METALTYPE"]);

                                            //if(iGrd == 1)
                                            dNetWt = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain
                                            //if((Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Gold) ||
                                            //        (Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Silver) ||
                                            //        (Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Platinum) ||
                                            //        (Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Palladium))
                                            //{
                                            //    dNetWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain
                                            //}

                                            ////if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd))
                                            ////    dNetWt += decimal.Round(Convert.ToDecimal(dr["QTY"]) / 5, 3, MidpointRounding.AwayFromZero);

                                            ////if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                                            drIngrd["NETQTY"] = decimal.Round(dNetWt, 3, MidpointRounding.AwayFromZero);

                                            drIngrd["QTY"] = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                                            drIngrd["PCS"] = decimal.Round(Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                                            drIngrd["CRATE"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                                            if (Convert.ToBoolean(saleLineItem.PartnerData.isMRP) == true)
                                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(saleLineItem.NetAmountWithNoTax), 2, MidpointRounding.AwayFromZero);//retailTrans.AmountDue//retailTrans.NetAmountWithNoTax
                                            else
                                            {
                                                //if (((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold) || (Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum)) && iGrd == 1)
                                                //    drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(dr["CVALUE"]) + dTotNetChargeAmt, 2, MidpointRounding.AwayFromZero);//
                                                //else
                                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(dr["CVALUE"]), 2, MidpointRounding.AwayFromZero);// - Convert.ToDecimal(dr["NetChargeValue"])
                                            }
                                            drIngrd["INGRDDISCAMT"] = decimal.Round(Convert.ToDecimal(dr["IngrdDiscTotAmt"]), 2, MidpointRounding.AwayFromZero);


                                            dtIngredient.Rows.Add(drIngrd);
                                            dtIngredient.AcceptChanges();

                                            if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                                                dGoldQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd))
                                                dDmdQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                                                dStnQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver))
                                                dSilQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum))
                                                dPltQty += Convert.ToDecimal(dr["QTY"]);

                                            iGrd++;
                                        }


                                        #endregion
                                    }
                                    else // bulk item estimation
                                    {
                                        #region bulk item estimation
                                        bIsBulkItem = true;

                                        drIngrd = dtIngredient.NewRow();
                                        int iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                        // sGrossWt = getValue("SELECT CAST(QTY AS decimal (6,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                                        sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                        sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                                        sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");

                                        #region Change the format of sCreatedString By Palas Jana @ 16/12/2014
                                        string sCreatedString = string.Empty;

                                        sCreatedString += sProdDesc + "  " + System.Environment.NewLine + Convert.ToString(saleLineItem.ItemId);
                                        if (!string.IsNullOrWhiteSpace(sDesignNo))
                                            sCreatedString += "-" + sDesignNo + System.Environment.NewLine + "  " + Convert.ToString(saleLineItem.PartnerData.ConfigId);
                                        if (!string.IsNullOrWhiteSpace(Convert.ToString(saleLineItem.Dimension.SizeId)))
                                            sCreatedString += "," + Convert.ToString(saleLineItem.Dimension.SizeId);
                                        if (!string.IsNullOrWhiteSpace(Convert.ToString(saleLineItem.Dimension.ColorId)))
                                            sCreatedString += "," + Convert.ToString(saleLineItem.Dimension.ColorId);
                                        sCreatedString += "  Gross Wt : " + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); //" (Complexity " + sComplexity + ") " +
                                        // string sCreatedString = sProdDesc + "  " + System.Environment.NewLine + "  " + Convert.ToString(saleLineItem.ItemId) + "-" + sDesignNo + System.Environment.NewLine + "  " + Convert.ToString(saleLineItem.PartnerData.ConfigId) + "," + Convert.ToString(saleLineItem.Dimension.SizeId) + "," + Convert.ToString(saleLineItem.Dimension.ColorId) + "  Gross Wt : " + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); //" (Complexity " + sComplexity + ") " +
                                        #endregion
                                        drIngrd["SKUHEADER"] = sCreatedString;

                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId;
                                        drIngrd["ITEMID"] = Convert.ToString(saleLineItem.ItemId); //
                                        drIngrd["LINENUM"] = i;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = Convert.ToString(saleLineItem.Dimension.SizeId); //PartnerData.ConfigId;
                                        drIngrd["InventColorID"] = Convert.ToString(saleLineItem.Dimension.ColorId);
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;
                                        sMetalUnit = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                                        drIngrd["UnitID"] = sMetalUnit;
                                        drIngrd["METALTYPE"] = iMetalType;

                                        if (J == 1)
                                            dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);  // added on 24/03/2014 R.Hossain

                                        drIngrd["NETQTY"] = decimal.Round(dNetWt, 3, MidpointRounding.AwayFromZero);

                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Rate)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Amount)), 2, MidpointRounding.AwayFromZero);

                                        drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount)), 2, MidpointRounding.AwayFromZero);

                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();

                                        if ((iMetalType == (int)MetalType.Gold))
                                            dGoldQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.LooseDmd))
                                            dDmdQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.Stone))
                                            dStnQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.Silver))
                                            dSilQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.Platinum))
                                            dPltQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        #endregion
                                        J++;

                                    }
                                }
                                else //if(Convert.ToInt16(saleLineItem.PartnerData.TransactionType) > 0)
                                {
                                    #region for Old Gold Purchase Estimaton
                                    string sOwn = "";
                                    string sActPurity = "";
                                    int iMetalType = 0;

                                    iOGP = 1;
                                    drIngrd = dtIngredient.NewRow();

                                    iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                    sGrossWt = "0";
                                    sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                    sDesignNo = "";
                                    sComplexity = "";

                                    if (Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                        sOwn = saleLineItem.PartnerData.ConfigId;

                                    if (Convert.ToDecimal(saleLineItem.PartnerData.Purity) > 0)
                                        sActPurity = Convert.ToString(saleLineItem.PartnerData.Purity);

                                    string sCreatedString = saleLineItem.PartnerData.ConfigId + "  " + sProdDesc + "  " + System.Environment.NewLine + "  Gross Wt (g) : " + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight)), 3, MidpointRounding.AwayFromZero) + System.Environment.NewLine + "  Actual Purity : " + sActPurity + "" + sOwn;

                                    drIngrd["SKUHEADER"] = sCreatedString;

                                    drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                    drIngrd["ITEMID"] = saleLineItem.ItemId; //saleLineItem.PartnerData.ConfigId + "-" + sActPurity; // for grouping purpose
                                    drIngrd["LINENUM"] = i;

                                    drIngrd["REFLINENUM"] = i;
                                    drIngrd["InventSizeID"] = saleLineItem.PartnerData.ConfigId;
                                    drIngrd["InventColorID"] = Convert.ToString(saleLineItem.PartnerData.OChecked);
                                    drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;
                                    sMetalUnit = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                                    drIngrd["UnitID"] = sMetalUnit;
                                    drIngrd["METALTYPE"] = iMetalType;

                                    dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain
                                    if (!Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                    {
                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Rate)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Amount)), 2, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.NETRATE)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.NETAMOUNT)), 2, MidpointRounding.AwayFromZero);
                                    }

                                    drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount)), 2, MidpointRounding.AwayFromZero);

                                    dtIngredient.Rows.Add(drIngrd);
                                    dtIngredient.AcceptChanges();

                                    #region og dmd wt
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDWT)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();

                                        iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                        sGrossWt = "0";
                                        sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                        sDesignNo = "";
                                        sComplexity = "";
                                        if (Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                            sOwn = saleLineItem.PartnerData.ConfigId;

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.Purity) > 0)
                                            sActPurity = Convert.ToString(saleLineItem.PartnerData.Purity);

                                        drIngrd["SKUHEADER"] = "";// sCreatedString;

                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = "Diamond";
                                        drIngrd["LINENUM"] = i;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = saleLineItem.PartnerData.ConfigId;
                                        drIngrd["InventColorID"] = Convert.ToString(saleLineItem.PartnerData.OChecked);
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;

                                        sDmdUnit = saleLineItem.PartnerData.DMDUNIT;

                                        drIngrd["UnitID"] = sDmdUnit;
                                        drIngrd["METALTYPE"] = (int)MetalType.LooseDmd; //DMD


                                        dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain


                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDWT)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDPCS)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = "0";
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDAMOUNT)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDAMOUNT)), 2, MidpointRounding.AwayFromZero);

                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    #region og STONE wt
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();

                                        iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                        sGrossWt = "0";
                                        sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                        sDesignNo = "";
                                        sComplexity = "";
                                        if (Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                            sOwn = saleLineItem.PartnerData.ConfigId;

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.Purity) > 0)
                                            sActPurity = Convert.ToString(saleLineItem.PartnerData.Purity);

                                        drIngrd["SKUHEADER"] = "";// sCreatedString;

                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = "Stone";
                                        drIngrd["LINENUM"] = i;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = saleLineItem.PartnerData.ConfigId;
                                        drIngrd["InventColorID"] = Convert.ToString(saleLineItem.PartnerData.OChecked);
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;
                                        sStnUnit = saleLineItem.PartnerData.STONEUNIT; ;

                                        drIngrd["UnitID"] = sStnUnit;
                                        drIngrd["METALTYPE"] = (int)MetalType.Stone; //STONE

                                        dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain

                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEPCS)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = "0";
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEAMOUNT)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEAMOUNT)), 2, MidpointRounding.AwayFromZero);

                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    #region FOR Tax line
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.TaxAmount)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();
                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = saleLineItem.TaxGroupId + "(" + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.TaxRatePct)), 2, MidpointRounding.AwayFromZero) + ")";
                                        drIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = string.Empty;
                                        drIngrd["InventColorID"] = string.Empty;
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;

                                        drIngrd["UnitID"] = string.Empty;
                                        drIngrd["METALTYPE"] = 100;

                                        drIngrd["QTY"] = 0;
                                        drIngrd["PCS"] = 0;
                                        drIngrd["CRATE"] = 0;//Convert.ToString(decimal.Round((Convert.ToDecimal(dTaxAmount)), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["CVALUE"] = Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(saleLineItem.TaxAmount))), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["INGRDDISCAMT"] = 0;
                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    #region VA % ( Loss percentage)
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossPct)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();
                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = "VA " + "(" + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossPct)), 2, MidpointRounding.AwayFromZero) + " %)";
                                        drIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = string.Empty;
                                        drIngrd["InventColorID"] = string.Empty;
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;

                                        drIngrd["UnitID"] = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                                        drIngrd["METALTYPE"] = 200;

                                        drIngrd["QTY"] = Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossWeight))), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["PCS"] = 0;
                                        drIngrd["CRATE"] = 0;//Convert.ToString(decimal.Round((Convert.ToDecimal(dTaxAmount)), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["CVALUE"] = 0;// Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(saleLineItem.TaxAmount))), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["INGRDDISCAMT"] = 0;
                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    // if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                                    if (!Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                        dGoldQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight)), 3, MidpointRounding.AwayFromZero);
                                    else
                                        dGoldQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd))
                                    dDmdQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDWT)), 3, MidpointRounding.AwayFromZero);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                                    dStnQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)), 3, MidpointRounding.AwayFromZero);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver))
                                    //    dSilQty += Convert.ToDecimal(dr["QTY"]);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum))
                                    //    dPltQty += Convert.ToDecimal(dr["QTY"]);
                                    #endregion
                                }
                                i++;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region after recall New items will print only
                        if (!saleLineItem.Voided && saleLineItem.PartnerData.EstimationIsDone == 0) //dBillAmt >= dAmount &&
                        {

                            if (!ValidateServiceItem(saleLineItem.ItemId))
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)) && Convert.ToString(saleLineItem.PartnerData.Ingredients) != "0")
                                {
                                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                    dsIngredients.ReadXml(reader);
                                }
                                else
                                {
                                    dsIngredients = null;
                                }
                            }
                            else
                                dsIngredients = null;

                            decimal dTotNetChargeAmt = 0;
                            //if (!ValidateServiceItem(saleLineItem.ItemId))
                            //{
                            //    if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                            //    {
                            //        if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                            //        {
                            //            iOGP = 0;
                            //            int iGrd = 1;

                            //            foreach (DataRow dr in dsIngredients.Tables[0].Rows)
                            //            {
                            //                dTotNetChargeAmt += decimal.Round(Convert.ToDecimal(dr["NETCHARGEVALUE"]), 2, MidpointRounding.AwayFromZero);
                            //            }

                            //        }
                            //    }
                            //}

                            drDtl = dtDetail.NewRow();
                            sSKUNo = saleLineItem.ItemId;
                            //dWastagePer = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastagePercentage), 2, MidpointRounding.AwayFromZero);
                            ////dMkDisc = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingDisc), 2, MidpointRounding.AwayFromZero); // commented by RM and Suman on 25/12/2014
                            //dMkDisc = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingRate), 2, MidpointRounding.AwayFromZero);
                            //sMkType = Convert.ToInt16(saleLineItem.PartnerData.MakingType);
                            string sTCSPer = "0";//getValue("SELECT CAST(NIM_TCSPERCENTAGE AS decimal (6,2)) AS QTY  FROM RETAILPARAMETERS ");

                            dTaxAmount = decimal.Round(Convert.ToDecimal(saleLineItem.TaxAmount), 2, MidpointRounding.AwayFromZero);
                            dTaxRatePct = decimal.Round(Convert.ToDecimal(saleLineItem.TaxRatePct), 2, MidpointRounding.AwayFromZero);
                            sTaxCode = saleLineItem.TaxGroupId;

                            //dTcsAmount = decimal.Round(Convert.ToDecimal(saleLineItem.NetAmount), 2, MidpointRounding.AwayFromZero);
                            //dTcsRatePct = decimal.Round(Convert.ToDecimal(sTCSPer), 2, MidpointRounding.AwayFromZero);
                            //sTcsCode = saleLineItem.Description;

                            drDtl["ITEMID"] = saleLineItem.ItemId;
                            drDtl["LINENUM"] = i;
                            if (!ValidateServiceItem(saleLineItem.ItemId))
                            {
                                drDtl["MAKINGAMOUNT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount), 2, MidpointRounding.AwayFromZero);
                                drDtl["MakingDisc"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingTotalDiscount), 2, MidpointRounding.AwayFromZero);
                                drDtl["WastageAmount"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount), 2, MidpointRounding.AwayFromZero);

                                if (Convert.ToDecimal(saleLineItem.PartnerData.WastagePercentage) > 0)
                                    drDtl["WASTPER"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastagePercentage), 2, MidpointRounding.AwayFromZero);
                                else
                                    drDtl["WASTPERWT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.WastageQty), 2, MidpointRounding.AwayFromZero);
                                drDtl["MKRATE"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.MakingRate), 2, MidpointRounding.AwayFromZero);
                                drDtl["MKTYPE"] = Convert.ToInt16(saleLineItem.PartnerData.MakingType);
                                drDtl["TCSAMT"] = 0;
                                drDtl["TCSPER"] = 0;
                                drDtl["TCSCODE"] = 0;
                            }
                            else
                            {
                                drDtl["MAKINGAMOUNT"] = 0;
                                drDtl["MakingDisc"] = 0;
                                drDtl["WastageAmount"] = 0;
                                drDtl["WASTPER"] = 0;
                                drDtl["WASTPERWT"] = 0;
                                drDtl["MKRATE"] = 0;
                                drDtl["MKTYPE"] = 0;
                                drDtl["TCSAMT"] = 0;
                                drDtl["TCSPER"] = 0;
                                drDtl["TCSCODE"] = "";
                            }
                            drDtl["TAXAMT"] = dTaxAmount;
                            drDtl["TAXPER"] = dTaxRatePct;
                            drDtl["TAXCODE"] = sTaxCode;
                            drDtl["NETCHARGEVALUE"] = dTotNetChargeAmt;

                            dtDetail.Rows.Add(drDtl);
                            dtDetail.AcceptChanges();
                            if (!ValidateServiceItem(saleLineItem.ItemId))
                            {
                                if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                                {
                                    if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                    {
                                        iOGP = 0;
                                        int iGrd = 1;

                                        #region Ingredient  Sale
                                        foreach (DataRow dr in dsIngredients.Tables[0].Rows)
                                        {
                                            drIngrd = dtIngredient.NewRow();

                                            sGrossWt = getValue("SELECT CAST(QTY AS decimal (16,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                                            sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                            sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                                            sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
                                            string sCreatedString = sProdDesc + "  " + System.Environment.NewLine + "  " + Convert.ToString(dr["SKUNUMBER"]) + "-" + sDesignNo + System.Environment.NewLine + "  " + Convert.ToString(dr["ConfigID"]) + "  Gross Wt : " + sGrossWt; //" (Complexity " + sComplexity + ") " +

                                            drIngrd["SKUHEADER"] = sCreatedString;

                                            drIngrd["SKUNUMBER"] = Convert.ToString(dr["SKUNUMBER"]);
                                            drIngrd["ITEMID"] = Convert.ToString(dr["ITEMID"]);
                                            drIngrd["LINENUM"] = iGrd;

                                            drIngrd["REFLINENUM"] = i;
                                            drIngrd["InventSizeID"] = Convert.ToString(dr["InventSizeID"]);
                                            drIngrd["InventColorID"] = Convert.ToString(dr["InventColorID"]);
                                            drIngrd["ConfigID"] = Convert.ToString(dr["ConfigID"]);

                                            drIngrd["UnitID"] = Convert.ToString(dr["UnitID"]);
                                            drIngrd["METALTYPE"] = Convert.ToInt32(dr["METALTYPE"]);

                                            //if(iGrd == 1)
                                            dNetWt = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain
                                            //if((Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Gold) ||
                                            //        (Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Silver) ||
                                            //        (Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Platinum) ||
                                            //        (Convert.ToInt32(drIngrd["METALTYPE"]) == (int)MetalType.Palladium))
                                            //{
                                            //    dNetWt += decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain
                                            //}

                                            drIngrd["NETQTY"] = decimal.Round(dNetWt, 3, MidpointRounding.AwayFromZero);

                                            drIngrd["QTY"] = decimal.Round(Convert.ToDecimal(dr["QTY"]), 3, MidpointRounding.AwayFromZero);
                                            drIngrd["PCS"] = decimal.Round(Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
                                            drIngrd["CRATE"] = decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero);
                                            if (Convert.ToBoolean(saleLineItem.PartnerData.isMRP) == true)
                                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(saleLineItem.NetAmountWithNoTax), 2, MidpointRounding.AwayFromZero);//retailTrans.AmountDue//retailTrans.NetAmountWithNoTax
                                            else
                                                drIngrd["CVALUE"] = decimal.Round(Convert.ToDecimal(dr["CVALUE"]) - Convert.ToDecimal(dr["NetChargeValue"]), 2, MidpointRounding.AwayFromZero);
                                            drIngrd["INGRDDISCAMT"] = decimal.Round(Convert.ToDecimal(dr["IngrdDiscTotAmt"]), 2, MidpointRounding.AwayFromZero);


                                            dtIngredient.Rows.Add(drIngrd);
                                            dtIngredient.AcceptChanges();

                                            if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                                                dGoldQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd))
                                                dDmdQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                                                dStnQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver))
                                                dSilQty += Convert.ToDecimal(dr["QTY"]);
                                            else if ((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum))
                                                dPltQty += Convert.ToDecimal(dr["QTY"]);

                                            iGrd++;
                                        }

                                        #endregion
                                    }
                                    else // bulk item estimation
                                    {
                                        #region bulk item estimation
                                        bIsBulkItem = true;

                                        drIngrd = dtIngredient.NewRow();
                                        int iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                        // sGrossWt = getValue("SELECT CAST(QTY AS decimal (6,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                                        sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                        sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                                        sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");

                                        #region Change the format of sCreatedString By Palas Jana @ 16/12/2014
                                        string sCreatedString = string.Empty;

                                        sCreatedString += sProdDesc + "  " + System.Environment.NewLine + Convert.ToString(saleLineItem.ItemId);
                                        if (!string.IsNullOrWhiteSpace(sDesignNo))
                                            sCreatedString += "-" + sDesignNo + System.Environment.NewLine + "  " + Convert.ToString(saleLineItem.PartnerData.ConfigId);
                                        if (!string.IsNullOrWhiteSpace(Convert.ToString(saleLineItem.Dimension.SizeId)))
                                            sCreatedString += "," + Convert.ToString(saleLineItem.Dimension.SizeId);
                                        if (!string.IsNullOrWhiteSpace(Convert.ToString(saleLineItem.Dimension.ColorId)))
                                            sCreatedString += "," + Convert.ToString(saleLineItem.Dimension.ColorId);
                                        sCreatedString += "  Gross Wt : " + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); //" (Complexity " + sComplexity + ") " +
                                        // string sCreatedString = sProdDesc + "  " + System.Environment.NewLine + "  " + Convert.ToString(saleLineItem.ItemId) + "-" + sDesignNo + System.Environment.NewLine + "  " + Convert.ToString(saleLineItem.PartnerData.ConfigId) + "," + Convert.ToString(saleLineItem.Dimension.SizeId) + "," + Convert.ToString(saleLineItem.Dimension.ColorId) + "  Gross Wt : " + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); //" (Complexity " + sComplexity + ") " +
                                        #endregion
                                        drIngrd["SKUHEADER"] = sCreatedString;

                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId;
                                        drIngrd["ITEMID"] = Convert.ToString(saleLineItem.ItemId); //
                                        drIngrd["LINENUM"] = i;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = Convert.ToString(saleLineItem.Dimension.SizeId); //PartnerData.ConfigId;
                                        drIngrd["InventColorID"] = Convert.ToString(saleLineItem.Dimension.ColorId);
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;
                                        sMetalUnit = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                                        drIngrd["UnitID"] = sMetalUnit;
                                        drIngrd["METALTYPE"] = iMetalType;

                                        if (J == 1)
                                            dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);  // added on 24/03/2014 R.Hossain

                                        drIngrd["NETQTY"] = decimal.Round(dNetWt, 3, MidpointRounding.AwayFromZero);

                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Rate)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Amount)), 2, MidpointRounding.AwayFromZero);

                                        drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount)), 2, MidpointRounding.AwayFromZero);

                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();

                                        if ((iMetalType == (int)MetalType.Gold))
                                            dGoldQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.LooseDmd))
                                            dDmdQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.Stone))
                                            dStnQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.Silver))
                                            dSilQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        else if ((iMetalType == (int)MetalType.Platinum))
                                            dPltQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        #endregion
                                        J++;
                                    }
                                }
                                else //if(Convert.ToInt16(saleLineItem.PartnerData.TransactionType) > 0)
                                {
                                    #region for Old Gold Purchase Estimaton
                                    string sOwn = "";
                                    string sActPurity = "";
                                    int iMetalType = 0;

                                    iOGP = 1;
                                    drIngrd = dtIngredient.NewRow();

                                    iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                    sGrossWt = "0";
                                    sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                    sDesignNo = "";
                                    sComplexity = "";

                                    if (Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                        sOwn = saleLineItem.PartnerData.ConfigId;

                                    if (Convert.ToDecimal(saleLineItem.PartnerData.Purity) > 0)
                                        sActPurity = Convert.ToString(saleLineItem.PartnerData.Purity);

                                    string sCreatedString = saleLineItem.PartnerData.ConfigId + "  " + sProdDesc + "  " + System.Environment.NewLine + "  Gross Wt (g) : " + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight)), 3, MidpointRounding.AwayFromZero) + System.Environment.NewLine + "  Actual Purity : " + sActPurity + "" + sOwn;

                                    drIngrd["SKUHEADER"] = sCreatedString;

                                    drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                    drIngrd["ITEMID"] = saleLineItem.ItemId; //saleLineItem.PartnerData.ConfigId + "-" + sActPurity; // for grouping purpose
                                    drIngrd["LINENUM"] = i;

                                    drIngrd["REFLINENUM"] = i;
                                    drIngrd["InventSizeID"] = saleLineItem.PartnerData.ConfigId;
                                    drIngrd["InventColorID"] = Convert.ToString(saleLineItem.PartnerData.OChecked);
                                    drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;
                                    sMetalUnit = saleLineItem.BackofficeSalesOrderUnitOfMeasure;

                                    drIngrd["UnitID"] = sMetalUnit;
                                    drIngrd["METALTYPE"] = iMetalType;

                                    dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain
                                    if (!Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                    {
                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Rate)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Amount)), 2, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                    {
                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.NETRATE)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.NETAMOUNT)), 2, MidpointRounding.AwayFromZero);
                                    }

                                    drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount)), 2, MidpointRounding.AwayFromZero);

                                    dtIngredient.Rows.Add(drIngrd);
                                    dtIngredient.AcceptChanges();

                                    #region og dmd wt
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDWT)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();

                                        iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                        sGrossWt = "0";
                                        sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                        sDesignNo = "";
                                        sComplexity = "";
                                        if (Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                            sOwn = saleLineItem.PartnerData.ConfigId;

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.Purity) > 0)
                                            sActPurity = Convert.ToString(saleLineItem.PartnerData.Purity);

                                        drIngrd["SKUHEADER"] = "";// sCreatedString;

                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = "Diamond";
                                        drIngrd["LINENUM"] = i;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = saleLineItem.PartnerData.ConfigId;
                                        drIngrd["InventColorID"] = Convert.ToString(saleLineItem.PartnerData.OChecked);
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;

                                        sDmdUnit = saleLineItem.PartnerData.DMDUNIT;

                                        drIngrd["UnitID"] = sDmdUnit;
                                        drIngrd["METALTYPE"] = (int)MetalType.LooseDmd; //DMD


                                        dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain


                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDWT)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDPCS)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = "0";
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDAMOUNT)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDAMOUNT)), 2, MidpointRounding.AwayFromZero);

                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    #region og STONE wt
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();

                                        iMetalType = Convert.ToInt16(getValue("SELECT metaltype FROM INVENTTABLE where ITEMID='" + sSKUNo + "'"));
                                        sGrossWt = "0";
                                        sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                                        sDesignNo = "";
                                        sComplexity = "";
                                        if (Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                            sOwn = saleLineItem.PartnerData.ConfigId;

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.Purity) > 0)
                                            sActPurity = Convert.ToString(saleLineItem.PartnerData.Purity);

                                        drIngrd["SKUHEADER"] = "";// sCreatedString;

                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = "Stone";
                                        drIngrd["LINENUM"] = i;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = saleLineItem.PartnerData.ConfigId;
                                        drIngrd["InventColorID"] = Convert.ToString(saleLineItem.PartnerData.OChecked);
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;
                                        sStnUnit = saleLineItem.PartnerData.STONEUNIT; ;

                                        drIngrd["UnitID"] = sStnUnit;
                                        drIngrd["METALTYPE"] = (int)MetalType.Stone; //STONE

                                        dNetWt = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)), 3, MidpointRounding.AwayFromZero); // added on 24/03/2014 R.Hossain

                                        drIngrd["QTY"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["PCS"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEPCS)), 3, MidpointRounding.AwayFromZero);
                                        drIngrd["CRATE"] = "0";
                                        drIngrd["CVALUE"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEAMOUNT)), 2, MidpointRounding.AwayFromZero);
                                        drIngrd["INGRDDISCAMT"] = decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEAMOUNT)), 2, MidpointRounding.AwayFromZero);

                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    #region FOR Tax line
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.TaxAmount)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();
                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = saleLineItem.TaxGroupId + "(" + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.TaxRatePct)), 2, MidpointRounding.AwayFromZero) + ")";
                                        drIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = string.Empty;
                                        drIngrd["InventColorID"] = string.Empty;
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;

                                        drIngrd["UnitID"] = string.Empty;
                                        drIngrd["METALTYPE"] = 100;

                                        drIngrd["QTY"] = 0;
                                        drIngrd["PCS"] = 0;
                                        drIngrd["CRATE"] = 0;//Convert.ToString(decimal.Round((Convert.ToDecimal(dTaxAmount)), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["CVALUE"] = Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(saleLineItem.TaxAmount))), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["INGRDDISCAMT"] = 0;
                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    #region VA % ( Loss percentage)
                                    if (Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossPct)) > 0)
                                    {
                                        drIngrd = dtIngredient.NewRow();
                                        drIngrd["SKUNUMBER"] = saleLineItem.ItemId + "-" + saleLineItem.PartnerData.ConfigId + "-" + sActPurity + "-" + sOwn;
                                        drIngrd["ITEMID"] = "VA " + "(" + decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossPct)), 2, MidpointRounding.AwayFromZero) + " %)";
                                        drIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                                        drIngrd["REFLINENUM"] = i;
                                        drIngrd["InventSizeID"] = string.Empty;
                                        drIngrd["InventColorID"] = string.Empty;
                                        drIngrd["ConfigID"] = saleLineItem.PartnerData.ConfigId;

                                        drIngrd["UnitID"] = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                                        drIngrd["METALTYPE"] = 200;

                                        drIngrd["QTY"] = Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.LossWeight))), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["PCS"] = 0;
                                        drIngrd["CRATE"] = 0;//Convert.ToString(decimal.Round((Convert.ToDecimal(dTaxAmount)), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["CVALUE"] = 0;// Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(saleLineItem.TaxAmount))), 2, MidpointRounding.AwayFromZero));
                                        drIngrd["INGRDDISCAMT"] = 0;
                                        dtIngredient.Rows.Add(drIngrd);
                                        dtIngredient.AcceptChanges();
                                    }
                                    #endregion

                                    // if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Gold))
                                    if (!Convert.ToBoolean(saleLineItem.PartnerData.OChecked))
                                        dGoldQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.TotalWeight)), 3, MidpointRounding.AwayFromZero);
                                    else
                                        dGoldQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.LooseDmd))
                                    dDmdQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.DMDWT)), 3, MidpointRounding.AwayFromZero);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Stone))
                                    dStnQty += decimal.Round(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.STONEWT)), 3, MidpointRounding.AwayFromZero);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Silver))
                                    //    dSilQty += Convert.ToDecimal(dr["QTY"]);
                                    //else if((Convert.ToInt32(dr["METALTYPE"]) == (int)MetalType.Platinum))
                                    //    dPltQty += Convert.ToDecimal(dr["QTY"]);
                                    #endregion
                                }
                                i++;
                            }
                        }
                        #endregion
                    }
                }
                #endregion

                #region start :for grand total of metal qty
                //if (dGoldQty > 0)
                //{
                //    drMetalTotal = dtMetalTotal.NewRow();
                //if(!string.IsNullOrEmpty(sMetalUnit))
                //    drMetalTotal["Metal"] = "Gold grand total ( " + sMetalUnit + " ) :"; //
                //else
                //    drMetalTotal["Metal"] = "Gold grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure
                //    drMetalTotal["Value"] = Convert.ToString(dGoldQty + (dDmdQty / 5));
                //    dtMetalTotal.Rows.Add(drMetalTotal);
                //    dtMetalTotal.AcceptChanges();
                //}
                //if (dDmdQty > 0)
                //{
                //    drMetalTotal = dtMetalTotal.NewRow();
                //    if (!string.IsNullOrEmpty(sDmdUnit))
                //        drMetalTotal["Metal"] = "Diamond grand total ( " + sDmdUnit + " ) :";
                //    else
                //        drMetalTotal["Metal"] = "Diamond grand total :";

                //    drMetalTotal["Value"] = Convert.ToString(dDmdQty);
                //    dtMetalTotal.Rows.Add(drMetalTotal);
                //    dtMetalTotal.AcceptChanges();
                //}
                //if (dStnQty > 0)
                //{
                //    drMetalTotal = dtMetalTotal.NewRow();
                //    if (!string.IsNullOrEmpty(sStnUnit))
                //        drMetalTotal["Metal"] = "Col.st grand total ( " + sStnUnit + " ) :";
                //    else
                //        drMetalTotal["Metal"] = "Col.st grand total  :";

                //    drMetalTotal["Value"] = Convert.ToString(dStnQty);
                //    dtMetalTotal.Rows.Add(drMetalTotal);
                //    dtMetalTotal.AcceptChanges();
                //}
                //if (dSilQty > 0)
                //{
                //    drMetalTotal = dtMetalTotal.NewRow();
                //    drMetalTotal["Metal"] = "Silver grand total : ";
                //    drMetalTotal["Value"] = Convert.ToString(dSilQty);
                //    dtMetalTotal.Rows.Add(drMetalTotal);
                //    dtMetalTotal.AcceptChanges();
                //}
                //if (dPltQty > 0)
                //{
                //    drMetalTotal = dtMetalTotal.NewRow();
                //    if(!string.IsNullOrEmpty(sMetalUnit))
                //        drMetalTotal["Metal"] = "Platinum grand total ( " + sMetalUnit + " ) :"; //
                //    else
                //        drMetalTotal["Metal"] = "Platinum grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure
                //    drMetalTotal["Value"] = Convert.ToString(dPltQty + (dDmdQty / 5));

                //    //drMetalTotal["Metal"] = "Platinum grand total : ";
                //    //drMetalTotal["Value"] = Convert.ToString(dPltQty + (dDmdQty / 5)); //Convert.ToString(dPltQty);
                //    dtMetalTotal.Rows.Add(drMetalTotal);
                //    dtMetalTotal.AcceptChanges();
                //}
                #endregion

                if ((dtDetail != null && dtDetail.Rows.Count > 0)
                    && (dtIngredient != null && dtIngredient.Rows.Count > 0))
                {

                    DataRow drLIngrd;


                    foreach (DataRow dr in dtDetail.Rows)
                    {
                        decimal dMetalNetWt = 0m;
                        string sUnit = string.Empty;
                        decimal dDimandWt = 0m;
                        decimal dMetalPcs = 0m;
                        int iGrd = 1;
                        int iGrd1 = 1;
                        foreach (DataRow row in dtIngredient.Select("[SKUNUMBER]='" + Convert.ToString(dr["ITEMID"]) + "'"))// DataTable.Select("[Name]<>'n/a'")
                        {
                            if (((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Gold) ||
                                (Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Silver) ||
                                (Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Platinum) ||
                                (Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Palladium)) && iGrd == 1)
                            {
                                sUnit = Convert.ToString(row["UnitID"]);
                                dMetalNetWt = decimal.Round(Convert.ToDecimal(row["QTY"]), 3, MidpointRounding.AwayFromZero);
                                dMetalPcs = decimal.Round(Convert.ToDecimal(row["PCS"]), 0, MidpointRounding.AwayFromZero);
                            }
                            if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.LooseDmd))
                            {
                                //sUnit = Convert.ToString(row["UnitID"]);
                                dDimandWt += decimal.Round(Convert.ToDecimal(row["QTY"]), 3, MidpointRounding.AwayFromZero);
                            }

                            iGrd++;
                        }
                        foreach (DataRow row in dtIngredient.Select("[SKUNUMBER]='" + Convert.ToString(dr["ITEMID"]) + "'"))
                        {
                            //if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Gold) && iGrd1 == 1)
                            //    row["QTY"] = decimal.Round(Convert.ToDecimal(dMetalNetWt + (dDimandWt / 5)), 3, MidpointRounding.AwayFromZero);
                            //if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Platinum) && iGrd1 == 1)
                            //    row["QTY"] = decimal.Round(Convert.ToDecimal(dMetalNetWt + (dDimandWt / 5)), 3, MidpointRounding.AwayFromZero);

                            if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Gold) && iGrd1 == 1)
                                row["QTY"] = decimal.Round(Convert.ToDecimal(dMetalNetWt), 3, MidpointRounding.AwayFromZero);
                            if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Platinum) && iGrd1 == 1)
                                row["QTY"] = decimal.Round(Convert.ToDecimal(dMetalNetWt), 3, MidpointRounding.AwayFromZero);


                            dtIngredient.AcceptChanges();
                            row.SetModified();
                            iGrd1++;
                        }

                        decimal dMetalNetWt1 = 0;// added for Wastage cal
                        decimal dDimandWt1 = 0;
                        foreach (DataRow row in dtIngredient.Select("[SKUNUMBER]='" + Convert.ToString(dr["ITEMID"]) + "'"))// DataTable.Select("[Name]<>'n/a'")
                        {
                            if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Gold) ||
                                (Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Silver) ||
                                (Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Platinum) ||
                                (Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Palladium))
                            {
                                dMetalNetWt1 += decimal.Round(Convert.ToDecimal(row["QTY"]), 3, MidpointRounding.AwayFromZero);
                            }
                            if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.LooseDmd))
                            {
                                //sUnit = Convert.ToString(row["UnitID"]);
                                dDimandWt1 += decimal.Round(Convert.ToDecimal(row["QTY"]), 3, MidpointRounding.AwayFromZero);
                            }
                        }

                        # region FOR MAKING + WASTAGE va();
                        if (Convert.ToDecimal(dr["WastageAmount"]) > 0)
                        {
                            drLIngrd = dtIngredient.NewRow();
                            drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["WASTPER"])))
                                drLIngrd["ITEMID"] = "VA" + "(" + Convert.ToString(dr["WASTPER"]) + "/PER)";
                            else
                                drLIngrd["ITEMID"] = "VA" + "(" + Convert.ToString(dr["WASTPERWT"]) + "/PCS)";

                            drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                            drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]);
                            drLIngrd["InventSizeID"] = string.Empty;
                            drLIngrd["InventColorID"] = string.Empty;
                            drLIngrd["ConfigID"] = string.Empty;

                            drLIngrd["UnitID"] = sUnit;
                            drLIngrd["METALTYPE"] = 0;
                            decimal dQty = 0m;
                            if (!string.IsNullOrEmpty(Convert.ToString(dr["WASTPER"])))//dMetalNetWt
                            {
                                dQty = decimal.Round((dMetalNetWt1 - (dDimandWt1 / 5)) * Convert.ToDecimal(dr["WASTPER"]) / 100, 3, MidpointRounding.AwayFromZero);
                                drLIngrd["QTY"] = dQty;
                            }
                            else
                            {
                                dQty = decimal.Round(Convert.ToDecimal(Convert.ToString(dr["WASTPERWT"])), 3, MidpointRounding.AwayFromZero);
                                drLIngrd["QTY"] = dQty;
                            }

                            drLIngrd["PCS"] = 0;// Convert.ToDecimal(dr["MKRATE"]);//

                            //if (!string.IsNullOrEmpty(Convert.ToString(dr["WASTPER"])))
                            drLIngrd["CRATE"] = decimal.Round(Convert.ToDecimal(dr["WastageAmount"]) / dQty, 2, MidpointRounding.AwayFromZero);
                            //else
                            //    drLIngrd["CRATE"] = decimal.Round(Convert.ToDecimal(dr["WastageAmount"]) / Convert.ToDecimal(Convert.ToString(dr["WASTPERWT"])), 2, MidpointRounding.AwayFromZero); // Convert.ToDecimal(dr["MKRATE"]);// Convert.ToString(decimal.Round((Convert.ToDecimal(dr["WastageAmount"])) / Convert.ToDecimal(drLIngrd["NETQTY"]), 2, MidpointRounding.AwayFromZero));

                            drLIngrd["CVALUE"] = Convert.ToDecimal(dr["WastageAmount"]);
                            drLIngrd["INGRDDISCAMT"] = Convert.ToDecimal(dr["MakingDisc"]);
                            dtIngredient.Rows.Add(drLIngrd);
                            dtIngredient.AcceptChanges();
                        }
                        #endregion

                        #region FOR MAKING CHARGE
                        if (Convert.ToDecimal(dr["MAKINGAMOUNT"]) > 0)
                        {
                            drLIngrd = dtIngredient.NewRow();
                            drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                            if (Convert.ToInt16(Convert.ToInt16(dr["MKTYPE"])) == (int)MakingType.Percentage)
                                drLIngrd["ITEMID"] = "MC" + "(" + Convert.ToDecimal(dr["MKRATE"]) + "/PER)";
                            else if (Convert.ToInt16(Convert.ToInt16(dr["MKTYPE"])) == (int)MakingType.Pieces)
                                drLIngrd["ITEMID"] = "MC" + "(" + Convert.ToDecimal(dr["MKRATE"]) + "/PCS)";
                            else if (Convert.ToInt16(Convert.ToInt16(dr["MKTYPE"])) == (int)MakingType.Tot)
                                drLIngrd["ITEMID"] = "MC" + "(" + Convert.ToDecimal(dr["MKRATE"]) + "/TOT)";
                            else if (Convert.ToInt16(Convert.ToInt16(dr["MKTYPE"])) == (int)MakingType.Weight)
                                drLIngrd["ITEMID"] = "MC" + "(" + Convert.ToDecimal(dr["MKRATE"]) + "/WT)";
                            else
                                drLIngrd["ITEMID"] = "MC" + "(" + Convert.ToDecimal(dr["MKRATE"]) + "/)";

                            drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                            drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]);
                            drLIngrd["InventSizeID"] = string.Empty;
                            drLIngrd["InventColorID"] = string.Empty;
                            drLIngrd["ConfigID"] = string.Empty;

                            drLIngrd["UnitID"] = string.Empty;
                            drLIngrd["METALTYPE"] = 0;

                            drLIngrd["QTY"] = 0;
                            drLIngrd["PCS"] = 0;
                            drLIngrd["CRATE"] = 0;// Convert.ToDecimal(dr["MKRATE"]);// Convert.ToString(decimal.Round((Convert.ToDecimal(dr["MAKINGAMOUNT"])) / Convert.ToDecimal(drLIngrd["NETQTY"]), 2, MidpointRounding.AwayFromZero));
                            //drLIngrd["CVALUE"] = Convert.ToDecimal(dr["MAKINGAMOUNT"]);
                            drLIngrd["CVALUE"] = Convert.ToDecimal(dr["MAKINGAMOUNT"]) + Convert.ToDecimal(dr["MakingDisc"]);
                            drLIngrd["INGRDDISCAMT"] = Convert.ToDecimal(dr["MakingDisc"]);
                            dtIngredient.Rows.Add(drLIngrd);
                            dtIngredient.AcceptChanges();
                        }
                        #endregion

                        //=====================Soutik
                        #region FOR Wt Discount
                        if (Convert.ToDecimal(dr["MakingDisc"]) > 0)
                        {
                            drLIngrd = dtIngredient.NewRow();
                            drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                            drLIngrd["ITEMID"] = "Dis";
                            drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;
                            drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]);
                            drLIngrd["InventSizeID"] = string.Empty;
                            drLIngrd["InventColorID"] = string.Empty;
                            drLIngrd["ConfigID"] = string.Empty;
                            drLIngrd["UnitID"] = string.Empty;
                            drLIngrd["METALTYPE"] = 0;

                            drLIngrd["QTY"] = 0;
                            drLIngrd["PCS"] = 0;
                            drLIngrd["CRATE"] = 0;// Convert.ToDecimal(dr["MKRATE"]);// Convert.ToString(decimal.Round((Convert.ToDecimal(dr["MAKINGAMOUNT"])) / Convert.ToDecimal(drLIngrd["NETQTY"]), 2, MidpointRounding.AwayFromZero));
                            //drLIngrd["CVALUE"] = Convert.ToDecimal(dr["MAKINGAMOUNT"]);
                            //===================Soutik=======================================================================
                            drLIngrd["CVALUE"] = Convert.ToDecimal(dr["MakingDisc"]) * (-1);
                            drLIngrd["INGRDDISCAMT"] = Convert.ToDecimal(dr["MakingDisc"]);
                            dtIngredient.Rows.Add(drLIngrd);
                            dtIngredient.AcceptChanges();
                        }
                        #endregion

                        #region NetChargable
                        //if (Convert.ToDecimal(dr["NETCHARGEVALUE"]) > 0)
                        //{
                        //    drLIngrd = dtIngredient.NewRow();
                        //    drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                        //    drLIngrd["ITEMID"] = "Setting Charge";
                        //    drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;
                        //    drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]);
                        //    drLIngrd["InventSizeID"] = string.Empty;
                        //    drLIngrd["InventColorID"] = string.Empty;
                        //    drLIngrd["ConfigID"] = string.Empty;

                        //    drLIngrd["UnitID"] = string.Empty;
                        //    drLIngrd["METALTYPE"] = 0;

                        //    drLIngrd["QTY"] = 0;
                        //    drLIngrd["PCS"] = 0;
                        //    drLIngrd["CRATE"] = 0;// Convert.ToDecimal(dr["MKRATE"]);// Convert.ToString(decimal.Round((Convert.ToDecimal(dr["MAKINGAMOUNT"])) / Convert.ToDecimal(drLIngrd["NETQTY"]), 2, MidpointRounding.AwayFromZero));
                        //    drLIngrd["CVALUE"] = Convert.ToDecimal(dr["NETCHARGEVALUE"]);
                        //    drLIngrd["INGRDDISCAMT"] = 0;
                        //    dtIngredient.Rows.Add(drLIngrd);
                        //    dtIngredient.AcceptChanges();
                        //}
                        #endregion

                        #region FOR Tax line
                        if (Convert.ToDecimal(dr["TAXAMT"]) > 0)
                        {
                            drLIngrd = dtIngredient.NewRow();
                            drLIngrd["SKUNUMBER"] = Convert.ToString(dr["ITEMID"]);
                            drLIngrd["ITEMID"] = "GST " + "(" + Convert.ToDecimal(dr["TaxPER"]) + "%)";//Convert.ToString(dr["TAXCODE"])
                            drLIngrd["LINENUM"] = dtIngredient.Rows.Count + 1;

                            drLIngrd["REFLINENUM"] = Convert.ToInt32(dr["LINENUM"]);
                            drLIngrd["InventSizeID"] = string.Empty;
                            drLIngrd["InventColorID"] = string.Empty;
                            drLIngrd["ConfigID"] = string.Empty;

                            drLIngrd["UnitID"] = string.Empty;
                            drLIngrd["METALTYPE"] = 100;

                            drLIngrd["QTY"] = 0;
                            drLIngrd["PCS"] = 0;
                            drLIngrd["CRATE"] = 0;//Convert.ToString(decimal.Round((Convert.ToDecimal(dTaxAmount)), 2, MidpointRounding.AwayFromZero));
                            drLIngrd["CVALUE"] = Convert.ToDecimal(dr["TAXAMT"]);
                            drLIngrd["INGRDDISCAMT"] = 0;
                            dtIngredient.Rows.Add(drLIngrd);
                            dtIngredient.AcceptChanges();
                        }
                        #endregion
                    }
                }

                if ((dtDetail != null && dtDetail.Rows.Count > 0)
                    && (dtIngredient != null && dtIngredient.Rows.Count > 0))
                {

                    #region start :for grand total of metal qty
                    DataTable table1 = new DataTable();
                    table1.Columns.Add("Metal", typeof(string));
                    table1.Columns.Add("Value", typeof(decimal));

                    DataRow drtable1;
                    Dictionary<string, decimal> dicSum = new Dictionary<string, decimal>();
                    foreach (DataRow dr in dtDetail.Rows)
                    {
                        foreach (DataRow row in dtIngredient.Select("[SKUNUMBER]='" + Convert.ToString(dr["ITEMID"]) + "'"))
                        {
                            if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Gold))
                            {
                                drMetalTotal = dtMetalTotal.NewRow();
                                drMetalTotal["Metal"] = Convert.ToString((int)MetalType.Gold); //
                                drMetalTotal["Value"] = Convert.ToString(row["QTY"]);
                                sMetalUnit = Convert.ToString(row["UnitId"]);
                                dtMetalTotal.Rows.Add(drMetalTotal);
                                dtMetalTotal.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Platinum))
                            {
                                drMetalTotal = dtMetalTotal.NewRow();
                                drMetalTotal["Metal"] = Convert.ToString((int)MetalType.Platinum); //
                                drMetalTotal["Value"] = Convert.ToString(row["QTY"]);
                                sMetalUnit = Convert.ToString(row["UnitId"]);
                                dtMetalTotal.Rows.Add(drMetalTotal);
                                dtMetalTotal.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.LooseDmd))
                            {
                                drMetalTotal = dtMetalTotal.NewRow();
                                drMetalTotal["Metal"] = Convert.ToString((int)MetalType.LooseDmd); //
                                drMetalTotal["Value"] = Convert.ToString(row["QTY"]);
                                sDmdUnit = Convert.ToString(row["UnitId"]);
                                dtMetalTotal.Rows.Add(drMetalTotal);
                                dtMetalTotal.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Stone))
                            {
                                drMetalTotal = dtMetalTotal.NewRow();
                                drMetalTotal["Metal"] = Convert.ToString((int)MetalType.Stone); //
                                drMetalTotal["Value"] = Convert.ToString(row["QTY"]);
                                sStnUnit = Convert.ToString(row["UnitId"]);
                                dtMetalTotal.Rows.Add(drMetalTotal);
                                dtMetalTotal.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Palladium))
                            {
                                drMetalTotal = dtMetalTotal.NewRow();
                                drMetalTotal["Metal"] = Convert.ToString((int)MetalType.Palladium); //
                                drMetalTotal["Value"] = Convert.ToString(row["QTY"]);
                                sMetalUnit = Convert.ToString(row["UnitId"]);
                                dtMetalTotal.Rows.Add(drMetalTotal);
                                dtMetalTotal.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(row["METALTYPE"]) == (int)MetalType.Silver))
                            {
                                drMetalTotal = dtMetalTotal.NewRow();
                                drMetalTotal["Metal"] = Convert.ToString((int)MetalType.Silver); //
                                drMetalTotal["Value"] = Convert.ToString(row["QTY"]);
                                sMetalUnit = Convert.ToString(row["UnitId"]);
                                dtMetalTotal.Rows.Add(drMetalTotal);
                                dtMetalTotal.AcceptChanges();
                            }
                        }
                    }

                    foreach (DataRow row1 in dtMetalTotal.Rows)
                    {
                        string group = row1["Metal"].ToString();
                        decimal rate = Convert.ToDecimal(row1["Value"]);
                        if (dicSum.ContainsKey(group))
                            dicSum[group] += rate;
                        else
                            dicSum.Add(group, rate);
                    }
                    DataTable table = new DataTable();

                    table.Columns.Add("Metal", typeof(string));
                    table.Columns.Add("Value", typeof(decimal));
                    foreach (string sProductType in dicSum.Keys)
                    {
                        table.Rows.Add(sProductType, dicSum[sProductType]);
                    }

                    if (table != null && table.Rows.Count > 0)
                    {
                        foreach (DataRow dr1 in table.Rows)
                        {
                            if ((Convert.ToInt32(dr1["Metal"]) == (int)MetalType.Gold))
                            {
                                drtable1 = table1.NewRow();
                                if (!string.IsNullOrEmpty(sMetalUnit))
                                    drtable1["Metal"] = "Gold grand total ( " + sMetalUnit + " ) :"; //
                                else
                                    drtable1["Metal"] = "Gold grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure

                                //drtable1["Metal"] = "Gold grand total : ";
                                drtable1["Value"] = Convert.ToString(dr1["Value"]);
                                table1.Rows.Add(drtable1);
                                table1.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(dr1["Metal"]) == (int)MetalType.Platinum))
                            {
                                drtable1 = table1.NewRow();
                                if (!string.IsNullOrEmpty(sMetalUnit))
                                    drtable1["Metal"] = "Platinum grand total ( " + sMetalUnit + " ) :"; //
                                else
                                    drtable1["Metal"] = "Platinum grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure

                                // drtable1["Metal"] = "Platinum grand total : ";
                                drtable1["Value"] = Convert.ToString(dr1["Value"]);
                                table1.Rows.Add(drtable1);
                                table1.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(dr1["Metal"]) == (int)MetalType.LooseDmd))
                            {
                                drtable1 = table1.NewRow();
                                if (!string.IsNullOrEmpty(sMetalUnit))
                                    drtable1["Metal"] = "Diamond grand total ( " + sDmdUnit + " ) :"; //
                                else
                                    drtable1["Metal"] = "Diamond grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure

                                //drtable1["Metal"] = "Diamond grand total : ";
                                drtable1["Value"] = Convert.ToString(dr1["Value"]);
                                table1.Rows.Add(drtable1);
                                table1.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(dr1["Metal"]) == (int)MetalType.Stone))
                            {
                                drtable1 = table1.NewRow();
                                if (!string.IsNullOrEmpty(sMetalUnit))
                                    drtable1["Metal"] = "Col. Stn. grand total ( " + sStnUnit + " ) :"; //
                                else
                                    drtable1["Metal"] = "Col. Stn. grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure

                                //drtable1["Metal"] = "Col. Stn. grand total : ";
                                drtable1["Value"] = Convert.ToString(dr1["Value"]);
                                table1.Rows.Add(drtable1);
                                table1.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(dr1["Metal"]) == (int)MetalType.Silver))
                            {
                                drtable1 = table1.NewRow();
                                if (!string.IsNullOrEmpty(sMetalUnit))
                                    drtable1["Metal"] = "Silver grand total ( " + sMetalUnit + " ) :"; //
                                else
                                    drtable1["Metal"] = "Silver grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure

                                // drtable1["Metal"] = "Silver grand total : ";
                                drtable1["Value"] = Convert.ToString(dr1["Value"]);
                                table1.Rows.Add(drtable1);
                                table1.AcceptChanges();
                            }
                            else if ((Convert.ToInt32(dr1["Metal"]) == (int)MetalType.Palladium))
                            {
                                drtable1 = table1.NewRow();
                                if (!string.IsNullOrEmpty(sMetalUnit))
                                    drtable1["Metal"] = "Palladiam grand total ( " + sMetalUnit + " ) :"; //
                                else
                                    drtable1["Metal"] = "Palladiam grand total : "; //saleLineItem.BackofficeSalesOrderUnitOfMeasure
                                //drtable1["Metal"] = "Palladiam grand total : ";
                                drtable1["Value"] = Convert.ToString(dr1["Value"]);
                                table1.Rows.Add(drtable1);
                                table1.AcceptChanges();
                            }
                        }
                    }

                    #endregion
                    frmEstimationReport objEstimationReport = new frmEstimationReport(posTransaction, dtDetail, dtIngredient);
                    objEstimationReport.ShowDialog();
                }
                else
                {
                    #region Nimbus
                    /*FormModulation formMod = new FormModulation(ApplicationSettings.Database.LocalConnection);
                    RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                    if (retailTransaction != null)
                    {
                        ICollection<Point> signaturePoints = null;
                        if (retailTransaction.TenderLines != null
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
                    }*/
                    #endregion
                }

            }

            #endregion
            //End:Nim
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PostSuspendTransaction", "After the suspension of a transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreRecallTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PreRecallTransaction", "Prior to the recall of a transaction from suspension...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostRecallTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("SuspendTriggers.PostRecallTransaction", "After the recall of a transaction from suspension...", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        private bool ValidateServiceItem(string item)
        {
            bool isTcsItem = false;
            //string ValidItem = string.Empty;
            //SqlConnection conn = new SqlConnection();
            //conn = ApplicationSettings.Database.LocalConnection;

            //SqlCommand cmd = null;
            //try
            //{
            //    if (conn.State == ConnectionState.Closed)
            //        conn.Open();
            //    string query = " select NIM_TCSSERVICEITM  from RETAILPARAMETERS where DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //    cmd = new SqlCommand(query, conn);
            //    ValidItem = Convert.ToString(cmd.ExecuteScalar());
            //}
            //catch (Exception ex)
            //{
            //    LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            //    throw;
            //}
            //finally
            //{
            //    if (conn.State == ConnectionState.Open)
            //        conn.Close();

            //}

            //if (item == ValidItem)
            //    isTcsItem = true;

            return isTcsItem;



        }

    }
}
