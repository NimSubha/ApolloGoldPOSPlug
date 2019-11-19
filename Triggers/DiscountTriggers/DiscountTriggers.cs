//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Settings;
using System.Data;
using System;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;

namespace Microsoft.Dynamics.Retail.Pos.DiscountTriggers
{

    [Export(typeof(IDiscountTrigger))]
    public class DiscountTriggers : IDiscountTrigger
    {
        //Start:Nim
        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
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
                //InternalApplication = value;
            }
        }

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
        }
        #endregion
        //End:Nim

        #region Constructor - Destructor

        public DiscountTriggers()
        {
            
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for DiscountTriggers are reserved at 53000 - 53999

        }

        #endregion

        //Start:Nim
        #region Check for Service Item
        private bool isServiceItem(IPosTransaction transaction, int lineid)
        {
            bool isServiceItemExists = false;
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(transaction)).SaleItems);
            foreach (var sale in saleline)
            {
                if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                {
                    if (sale.LineId == lineid)
                    {
                        isServiceItemExists = true;
                        break;
                    }
                }
            }
            return isServiceItemExists;
        }
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
        private bool isGiftItemInSalesLine(IPosTransaction transaction)
        {
            bool isGiftItemExists = false;
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(transaction)).SaleItems);
            foreach (var sale in saleline)
            {
                if (!sale.Voided)
                {
                    if (IsGiftItem(sale.ItemId))
                    {
                        isGiftItemExists = true;
                        break;
                    }
                }
            }
            return isGiftItemExists;
        }
        private int getMetalType(string sItemId)
        {
            int iMetalType = 100;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT METALTYPE FROM INVENTTABLE WHERE ITEMID='" + sItemId + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return iMetalType;
        }
        private bool chkOGTaxApplicable()
        {
            bool bResult = false;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT isnull(OGTaxApplicable,0) FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int iResult = (int)reader.GetValue(0);

                    if (iResult == 1)
                        bResult = true;
                    else
                        bResult = false;

                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return bResult;
        }
        #endregion
        //End:Nim

        #region IDiscountTriggersV1 Members

        public void PreLineDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int LineId)
        {
            //Start:Nim
            if (isGiftItemInSalesLine(transaction))
            {
                MessageBox.Show("Discount not allowed if free gift item is there in sales line.");
                preTriggerResult.ContinueOperation = false;
            }
            RetailTransaction retailTrans = transaction as RetailTransaction;
            if (retailTrans.PartnerData.SingaporeTaxCal == "1")
            {
                MessageBox.Show("If OG tax calculation is applied, this operation is invalid", "This operation is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                preTriggerResult.ContinueOperation = false;
                return;
            }
            if (retailTrans.PartnerData.APPLYGSSDISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GSS discount is done.");
                preTriggerResult.ContinueOperation = false;
                return;
            }
            if (retailTrans.PartnerData.APPLYGMADISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GMA discount is done.");
                preTriggerResult.ContinueOperation = false;
                return;
            }

            foreach (SaleLineItem SLineItem in retailTrans.SaleItems)
            {
                #region If excahnge then  For Singapur only
                if (retailTrans.IncomeExpenseAmounts == 0
                    && retailTrans.SalesInvoiceAmounts == 0)
                {
                    if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                    {
                        bool bIsOGTaxApplicable = chkOGTaxApplicable();
                        if (bIsOGTaxApplicable)
                        {
                            decimal dTotOgTaxAmt = 0m;
                            decimal dTotSalesGoldTaxAmt = 0m;

                            foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                            {
                                if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                    && !saleLineItem1.Voided)
                                {
                                    dTotSalesGoldTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                }
                                if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                    && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                    && !saleLineItem1.Voided)
                                {
                                    //dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                    int iPM = getMetalType(saleLineItem1.ItemId);
                                    if (iPM == (int)MetalType.Gold)
                                    {
                                        dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                    }
                                }
                            }

                            if (dTotOgTaxAmt > 0 && dTotSalesGoldTaxAmt > 0 && retailTrans.PartnerData.SingaporeTaxCal == "0")
                            {
                                MessageBox.Show("Please enter Tax recalculate button.", "Please enter Tax recalculate button.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                }
                #endregion
            }

            //End:Nim

            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV1.PreLineDiscountAmount", "Triggered before adding line discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreLineDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int LineId)
        {
            //Start:Nim
            RetailTransaction retailTrans = transaction as RetailTransaction;

            if (retailTrans.PartnerData.SingaporeTaxCal == "1")
            {
                MessageBox.Show("If OG tax calculation is applied, this operation is invalid", "This operation is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                preTriggerResult.ContinueOperation = false;
                return;
            }

            foreach (SaleLineItem SLineItem in retailTrans.SaleItems)
            {
                #region If excahnge then  For Singapur only
                if (retailTrans.IncomeExpenseAmounts == 0
                    && retailTrans.SalesInvoiceAmounts == 0)
                {
                    if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                    {
                        bool bIsOGTaxApplicable = chkOGTaxApplicable();
                        if (bIsOGTaxApplicable)
                        {
                            decimal dTotOgTaxAmt = 0m;
                            decimal dTotSalesGoldTaxAmt = 0m;

                            foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                            {
                                if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                    && !saleLineItem1.Voided)
                                {
                                    dTotSalesGoldTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                }
                                if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                    && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                    && !saleLineItem1.Voided)
                                {
                                    //dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                    int iPM = getMetalType(saleLineItem1.ItemId);
                                    if (iPM == (int)MetalType.Gold)
                                    {
                                        dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                    }
                                }
                            }

                            if (dTotOgTaxAmt > 0 && dTotSalesGoldTaxAmt > 0 && retailTrans.PartnerData.SingaporeTaxCal == "0")
                            {
                                MessageBox.Show("Please enter Tax recalculate button.", "Please enter Tax recalculate button.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                }
                #endregion
            }

            //if (retailTrans.PartnerData.IsSpecialDisc == false)//IsDisableLineDiscount() &&
            //{
            //    MessageBox.Show("Discoutn not allowed for this item.");
            //    preTriggerResult.ContinueOperation = false;
            //}
            if (isGiftItemInSalesLine(transaction))
            {
                MessageBox.Show("Discount not allowed if free gift item is there in sales line.");
                preTriggerResult.ContinueOperation = false;
            }

            if (retailTrans.PartnerData.APPLYGSSDISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GSS discount is done.");
                preTriggerResult.ContinueOperation = false;
            }

            if (retailTrans.PartnerData.APPLYGMADISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GMA discount is done.");
                preTriggerResult.ContinueOperation = false;
                return;
            }
            //end:Nim

            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV1.PreLineDiscountPercent", "Triggered before adding line discount percentange.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region IDiscountTriggersV2 Members

        public void PostLineDiscountAmount(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostLineDiscountAmount", "Triggered after adding line discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostLineDiscountPercent(IPosTransaction posTransaction, int lineId)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostLineDiscountPercent", "Triggered after adding line discount percentange.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreTotalDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Start:Nim
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans.PartnerData.SingaporeTaxCal == "1")
            {
                MessageBox.Show("If OG tax calculation is applied, this operation is invalid", "This operation is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                preTriggerResult.ContinueOperation = false;
                return;
            }

            foreach (SaleLineItem SLineItem in retailTrans.SaleItems)
            {
                #region If excahnge then  For Singapur only
                if (retailTrans.IncomeExpenseAmounts == 0
                    && retailTrans.SalesInvoiceAmounts == 0)
                {
                    if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                    {
                        bool bIsOGTaxApplicable = chkOGTaxApplicable();
                        if (bIsOGTaxApplicable)
                        {
                            decimal dTotOgTaxAmt = 0m;
                            decimal dTotSalesGoldTaxAmt = 0m;

                            foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                            {
                                if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                    && !saleLineItem1.Voided)
                                {
                                    dTotSalesGoldTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                }
                                if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                    && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                    && !saleLineItem1.Voided)
                                {
                                    //dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                    int iPM = getMetalType(saleLineItem1.ItemId);
                                    if (iPM == (int)MetalType.Gold)
                                    {
                                        dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                    }
                                }
                            }

                            if (dTotOgTaxAmt > 0 && dTotSalesGoldTaxAmt > 0 && retailTrans.PartnerData.SingaporeTaxCal == "0")
                            {
                                MessageBox.Show("Please enter Tax recalculate button.", "Please enter Tax recalculate button.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                }
                #endregion
            }

            if (isGiftItemInSalesLine(posTransaction))
            {
                MessageBox.Show("Discount not allowed if free gift item is there in sales line.");
                preTriggerResult.ContinueOperation = false;
            }

            if (retailTrans.PartnerData.APPLYGSSDISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GSS discount is done.");
                preTriggerResult.ContinueOperation = false;
            }
            if (retailTrans.PartnerData.APPLYGMADISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GMA discount is done.");
                preTriggerResult.ContinueOperation = false;
                return;
            }
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PreTotalDiscountAmount", "Triggered before total discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostTotalDiscountAmount(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostTotalDiscountAmount", "Triggered after total discount amount.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreTotalDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Start:Nim
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans.PartnerData.SingaporeTaxCal == "1")
            {
                MessageBox.Show("If OG tax calculation is applied, this operation is invalid", "This operation is invalid.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                preTriggerResult.ContinueOperation = false;
                return;
            }

            foreach (SaleLineItem SLineItem in retailTrans.SaleItems)
            {
                #region If excahnge then  For Singapur only
                if (retailTrans.IncomeExpenseAmounts == 0
                    && retailTrans.SalesInvoiceAmounts == 0)
                {
                    if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                    {
                        bool bIsOGTaxApplicable = chkOGTaxApplicable();
                        if (bIsOGTaxApplicable)
                        {
                            decimal dTotOgTaxAmt = 0m;
                            decimal dTotSalesGoldTaxAmt = 0m;

                            foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                            {
                                if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                    && !saleLineItem1.Voided)
                                {
                                    dTotSalesGoldTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                }
                                if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                    && saleLineItem1.PartnerData.NimReturnLine == 0
                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                    && !saleLineItem1.Voided)
                                {
                                    //dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                    int iPM = getMetalType(saleLineItem1.ItemId);
                                    if (iPM == (int)MetalType.Gold)
                                    {
                                        dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                    }
                                }
                            }

                            if (dTotOgTaxAmt > 0 && dTotSalesGoldTaxAmt > 0 && retailTrans.PartnerData.SingaporeTaxCal == "0")
                            {
                                MessageBox.Show("Please enter Tax recalculate button.", "Please enter Tax recalculate button.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }
                        }
                    }
                }
                #endregion
            }

            if (isGiftItemInSalesLine(posTransaction))
            {
                MessageBox.Show("Discount not allowed if free gift item is there in sales line.");
                preTriggerResult.ContinueOperation = false;
            }

            if (retailTrans.PartnerData.APPLYGSSDISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GSS discount is done.");
                preTriggerResult.ContinueOperation = false;
            }
            if (retailTrans.PartnerData.APPLYGMADISCDONE == true)
            {
                MessageBox.Show("Discount not allowed if GMA discount is done.");
                preTriggerResult.ContinueOperation = false;
                return;
            }
            //end:Nim

            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PreTotalDiscountPercent", "Triggered before total discount percent.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostTotalDiscountPercent(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IDiscountTriggersV2.PostTotalDiscountPercent", "Triggered after total discount percent.", LSRetailPosis.LogTraceLevel.Trace);
        }
        
        #endregion

    }
}
