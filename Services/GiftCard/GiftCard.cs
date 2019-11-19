/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.GiftCertificateItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using LSRetailPosis.Transaction.Line.SaleItem;

namespace Microsoft.Dynamics.Retail.Pos.GiftCard
{
    // Get all text through the Translation function in the ApplicationLocalizer
    //
    // TextID's for the GiftCard service are reserved at 55000 - 55999
    // TextID's for the following modules are as follows:
    //
    // GiftCard.cs:             55000 - 55399. The last in use: 55001
    // GiftCardForm.cs:         55400 - 55499  The last in use:

    [Export(typeof(IGiftCard))]
    public class GiftCard : IGiftCard
    {

        #region IGiftCard Members
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
                InternalApplication = value;
            }
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

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        /// <summary>
        /// Issues gift card.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="gcTenderInfo"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void IssueGiftCard(IPosTransaction posTransaction, ITender gcTenderInfo)
        {

            //Nimbus Start
            //Start: added on 16/07/2014 for customer selection is must
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            CustomerPaymentTransaction custTrans = posTransaction as CustomerPaymentTransaction;

            if (retailTrans != null) //SaleAdjustmentGoldAmt
            {
                if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty || string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Add a customer to transaction before making a deposit", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                if ((Convert.ToDecimal(retailTrans.AmountDue)) < 0) // add on 31/12/14 req mail by pranay as wel as Mr.A.MitraretailTrans.PartnerData.SaleAdjustmentGoldAmt
                {

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Gift Card Can not be Adjusted with this transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if (!saleLineItem.Voided)
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please complete the existing transaction for issue gift card.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                }
            }
           
            //End: added on 16/07/2014 for customer selection is must

            //Nimbus End


            LogMessage("Issuing a gift card",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.IssueGiftCard");

            if (GiftCard.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                //The operation should proceed after the fiscal printer handles IssueGiftCard
                GiftCard.InternalApplication.Services.Peripherals.FiscalPrinter.IssueGiftCard(posTransaction, gcTenderInfo);
            }

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardIssue, (PosTransaction)posTransaction, (Tender)gcTenderInfo);

            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                
                POSFormsManager.ShowPOSForm(giftCardForm);

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    if (validAmountDenomination(controller.Amount))
                    {
                        // Add the gift card to the transaction.
                        RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                        GiftCertificateItem giftCardItem = (GiftCertificateItem)this.Application.BusinessLogic.Utility.CreateGiftCardLineItem(
                            ApplicationSettings.Terminal.StoreCurrency,
                            this.Application.Services.Rounding, retailTransaction);

                        if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                        {
                            InitGiftCardItem(giftCardItem);
                        }

                        giftCardItem.SerialNumber = controller.CardNumber;
                        giftCardItem.StoreId = posTransaction.StoreId;
                        giftCardItem.TerminalId = posTransaction.TerminalId;
                        giftCardItem.StaffId = posTransaction.OperatorId;
                        giftCardItem.TransactionId = posTransaction.TransactionId;
                        giftCardItem.ReceiptId = posTransaction.ReceiptId;
                        giftCardItem.Amount = controller.Amount;
                        giftCardItem.Balance = controller.Balance;
                        giftCardItem.Date = DateTime.Now;

                        // Necessary property settings for the the gift certificate "item"...
                        giftCardItem.Price = giftCardItem.Amount;
                        giftCardItem.StandardRetailPrice = giftCardItem.Amount;
                        giftCardItem.Quantity = 1;
                        giftCardItem.TaxRatePct = 0;
                        if (!ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                        {
                            giftCardItem.Description = ApplicationLocalizer.Language.Translate(55001);  // Gift Card
                        }
                        giftCardItem.Comment = controller.CardNumber;
                        giftCardItem.NoDiscountAllowed = true;
                        giftCardItem.Found = true;

                        retailTransaction.Add(giftCardItem);

                        if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                        {
                            // Calculate the tax for gift card item
                            this.Application.Services.Tax.CalculateTax(retailTransaction);
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Invalid gift voucher denomination entered.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                }

                //Nimbus Start
                if (controller.Amount > 0 && controller.Context == Microsoft.Dynamics.Retail.Pos.GiftCard.GiftCardController.ContextType.GiftCardIssue)
                    retailTrans.PartnerData.IsGiftCardIssue = "1";
                else
                    retailTrans.PartnerData.IsGiftCardIssue = "0";

                //Nimbus end
            }
        }

        /// <summary>
        /// Updates gift card.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="gcLineItem"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Grandfather")]
        public void VoidGiftCard(ref bool voided, ref string comment, IGiftCardLineItem gcLineItem)
        {
            LogMessage("Voiding a gift card",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.VoidGiftCard");

            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.VoidGiftCard(ref voided, ref comment, gcLineItem.SerialNumber);
            }
        }

        /// <summary>
        /// Gift card payment related methods
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="comment"></param>
        /// <param name="posTransaction"></param>
        /// <param name="cardInfo"></param>
        /// <param name="gcTenderInfo"></param>
        public void AuthorizeGiftCardPayment(ref bool valid, ref string comment, IPosTransaction posTransaction, ICardInfo cardInfo, ITender gcTenderInfo)
        {
            #region Base
            /*if (cardInfo == null)
                throw new ArgumentNullException("cardInfo");

            LogMessage("Authorizing a gift card payment", LogTraceLevel.Trace);

            valid = false;

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardPayment, (PosTransaction)posTransaction, (Tender)gcTenderInfo);

            controller.CardNumber = cardInfo.CardNumber;
            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    valid = true;
                    cardInfo.CardNumber = controller.CardNumber;
                    cardInfo.Amount = controller.Amount;
                    cardInfo.CurrencyCode = controller.Currency;
                }
            }*/
            #endregion

            RetailTransaction retailTrans;

            if (cardInfo == null)
                throw new ArgumentNullException("cardInfo");

            LogMessage("Authorizing a gift card payment", LogTraceLevel.Trace);

            valid = false;

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardPayment, (PosTransaction)posTransaction, (Tender)gcTenderInfo);

            controller.CardNumber = cardInfo.CardNumber;
            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);

                retailTrans = posTransaction as RetailTransaction;

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    #region Nimbus

                    DataTable dtSKUInfo = new DataTable();
                    //string strQRY = "select top 1 isnull(StoreGroup,'') StoreGroup," +
                    //                " isnull(MinPurAmt,0) MinPurAmt,isnull(FreeGift,0) FreeGift," +
                    //                " CRWGiftType,isnull(GiftVoucherCode,'') GiftVoucherCode," +
                    //                " isnull(ACCOUNTTYPE,0) ACCOUNTTYPE," +
                    //                " isnull(ValidityDay,0) ValidityDay from SKUTable_Posted" +
                    //                " Where SkuNumber='" + controller.CardNumber + "'";

                    //dtSKUInfo = GetAdvDetailInfo(strQRY);
                    dtSKUInfo = getGiftCardDetails(controller.CardNumber);

                    string strGVCode = string.Empty;
                    string strStoreGroup = string.Empty;
                    decimal dMinPurAmt = 0m;
                    int iFreeGift = 0;
                    DateTime dateExp = Convert.ToDateTime("1900-01-01");
                    int iGiftType = 0;
                    string sStoreCurrency = string.Empty;
                    sStoreCurrency = ApplicationSettings.Terminal.StoreCurrency;
                    int iAccType = 0;
                    string sStrFormat = getStoreFormatCode();

                    string sCreatedTransId = "";
                    string sCreatedTerminalId = "";
                    string sCreatedStoreId = "";
                    int isCashBackGV = 0;
                    int isPayment = 0;

                    if (dtSKUInfo != null && dtSKUInfo.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtSKUInfo.Rows)
                        {
                            strGVCode = Convert.ToString(dr["GiftVoucherCode"]);
                            strStoreGroup = Convert.ToString(dr["StoreGroup"]);
                            dMinPurAmt = Convert.ToDecimal(dr["MinPurAmt"]);
                            iFreeGift = Convert.ToInt16(dr["FreeGift"]);
                            dateExp = Convert.ToDateTime(dr["ExpDate"]);
                            iGiftType = Convert.ToInt16(dr["CRWGiftType"]);
                            iAccType = Convert.ToInt16(dr["ACCOUNTTYPE"]);

                            sCreatedTransId = Convert.ToString(dr["TransactionId"]);
                            sCreatedTerminalId = Convert.ToString(dr["TerminalId"]);
                            sCreatedStoreId = Convert.ToString(dr["StoreId"]);
                           // isCashBackGV = Convert.ToInt16(dr["isCashBackGV"]);
                            isPayment = Convert.ToInt16(dr["isPayment"]);

                            if (retailTrans != null)
                            {
                                if (retailTrans.TransactionId == sCreatedTransId
                                    && retailTrans.TerminalId == sCreatedTerminalId
                                    && retailTrans.StoreId == sCreatedStoreId)
                                {
                                    UnlockGiftCard(controller);

                                    MessageBox.Show("This gift card is not applicable for the same transaction.");
                                    valid = false;
                                    return;
                                }
                                else if (isCashBackGV == 1 && isPayment == 0)
                                {
                                    string sTransId = getPaymentTransId(sCreatedTransId, sCreatedTerminalId, sCreatedStoreId);
                                    if (string.IsNullOrEmpty(sTransId))
                                    {
                                        UnlockGiftCard(controller);
                                        MessageBox.Show("This gift card is not valid as of now.");
                                        valid = false;
                                        return;
                                    }
                                }
                            }
                        }
                    }

                    if (retailTrans != null)
                    {
                        if (iFreeGift == 1)
                        {
                            if (dMinPurAmt > retailTrans.NetAmountWithTax)
                            {
                                UnlockGiftCard(controller);
                                MessageBox.Show("This gift card not applicable for this item.");
                                valid = false;
                                return;
                            }
                        }

                        if (DateTime.Now.Date > dateExp.Date)
                        {
                            UnlockGiftCard(controller);
                            MessageBox.Show("This gift card has been expired.");
                            valid = false;
                            return;

                        }

                        if (iAccType == (int)TableGroupAll.Table)
                        {
                            if (!string.IsNullOrEmpty(strStoreGroup))
                            {
                                if (strStoreGroup != ApplicationSettings.Database.StoreID)
                                {
                                    UnlockGiftCard(controller);
                                    MessageBox.Show("This gift card not applicable for this store.");
                                    valid = false;
                                    return;
                                }
                            }
                            //else
                            //{
                            //    if(controller.Amount == controller.Balance)
                            //    {
                            //        valid = true;
                            //        cardInfo.CardNumber = controller.CardNumber;
                            //        cardInfo.Amount = controller.Amount;
                            //        cardInfo.CurrencyCode = controller.Currency;
                            //    }
                            //}
                        }
                        else if (iAccType == (int)TableGroupAll.GroupId)
                        {
                            if (strStoreGroup != sStrFormat)
                            {
                                UnlockGiftCard(controller);
                                MessageBox.Show("This gift card not applicable for this store.");

                                valid = false;
                                return;
                            }
                        }

                        foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                        {
                            if (retailTrans.EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                if (!saleLineItem.Voided)
                                {
                                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                    {
                                        string cmdStr1 = string.Empty;
                                        int iMetalType = 0;

                                        iMetalType = getMetalType(saleLineItem.ItemId);

                                        if (iGiftType == (int)CRWGiftType.Gold)
                                        {
                                            if (iMetalType == (int)MetalType.Gold)
                                            {
                                                if (controller.Amount == controller.Balance)
                                                {
                                                    valid = true;
                                                    cardInfo.CardNumber = controller.CardNumber;
                                                    cardInfo.Amount = controller.Amount;
                                                    cardInfo.CurrencyCode = controller.Currency;
                                                }
                                                else
                                                {
                                                    UnlockGiftCard(controller);

                                                    MessageBox.Show("Enter exact gift card amount");
                                                    valid = false;
                                                    return;
                                                }

                                            }
                                            else
                                            {
                                                UnlockGiftCard(controller);
                                                MessageBox.Show("This gift card not applicable for this item.");
                                                valid = false;
                                                return;
                                            }
                                        }
                                        else if (iGiftType == (int)CRWGiftType.Diamond)
                                        {
                                            if ((iMetalType == (int)MetalType.Jewellery)
                                                || (iMetalType == (int)MetalType.Diamond)
                                                || (iMetalType == (int)MetalType.LooseDmd)
                                                )
                                            {

                                                if (controller.Amount == controller.Balance)
                                                {
                                                    valid = true;
                                                    cardInfo.CardNumber = controller.CardNumber;
                                                    cardInfo.Amount = controller.Amount;
                                                    cardInfo.CurrencyCode = controller.Currency;
                                                }
                                                else
                                                {
                                                    UnlockGiftCard(controller);
                                                    MessageBox.Show("Enter exact gift card amount");
                                                    valid = false;
                                                    return;
                                                }

                                            }
                                            else
                                            {
                                                UnlockGiftCard(controller);
                                                MessageBox.Show("This gift card not applicable for this item.");
                                                valid = false;
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            if (controller.Amount == controller.Balance)
                                            {
                                                valid = true;
                                                cardInfo.CardNumber = controller.CardNumber;
                                                cardInfo.Amount = controller.Amount;
                                                cardInfo.CurrencyCode = controller.Currency;
                                            }
                                            else
                                            {
                                                UnlockGiftCard(controller);
                                                MessageBox.Show("Enter exact gift card amount");
                                                valid = false;
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (saleLineItem.NetAmount > 0)//31.05.17 net amount validation added for service item
                                        {
                                            UnlockGiftCard(controller);
                                            MessageBox.Show("This gift card not applicable for this item.");
                                            valid = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        UnlockGiftCard(controller);
                        MessageBox.Show("This gift card not applicable for this item.");
                        valid = false;
                        return;
                    }
                    #endregion
                }
            }
        }

        private void UnlockGiftCard(GiftCardController controller)
        {
            try
            {
                string sMsg = string.Empty;
                ReadOnlyCollection<object> containerArray1;

                if (this.Application.TransactionServices.CheckConnection())
                {
                    containerArray1 = this.Application.TransactionServices.InvokeExtension("GiftCardUnlocked", controller.CardNumber);
                    sMsg = Convert.ToString(containerArray1[2]);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private int getMetalType(string sItemId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(METALTYPE,0) FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            int iMetalType = Convert.ToInt32(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iMetalType;

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

        private string getPaymentTransId(string sTransId, string sTerminalId, string sStore)
        {
            StringBuilder commandText = new StringBuilder();
            //commandText.Append("select TRANSACTIONID from RETAILTRANSACTIONPAYMENTTRANS where TRANSACTIONSTATUS!=1 ");
            //commandText.Append(" and TRANSACTIONID ='" + sTransId + "' and STORE='" + sStore + "' and TERMINAL='" + sTerminalId + "'");

            commandText.Append("select TRANSACTIONID from RETAILTRANSACTIONTABLE where ENTRYSTATUS=0 ");
            commandText.Append(" and TRANSACTIONID ='" + sTransId + "' and STORE='" + sStore + "'");
            commandText.Append(" and TERMINAL='" + sTerminalId + "' and isnull(RECEIPTID,'')!=''");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            string sResult = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sResult;
        }


        private bool validAmountDenomination(decimal dAmt) 
        {
            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT AMOUNT from CRWDENOMINATIONMASTER where AMOUNT=" + dAmt + "");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            decimal  dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (dResult > 0)
                return true;
            else
                return false;
        }

        private DataTable GetAdvDetailInfo(string sStringSql)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = sStringSql;

                DataTable CustAdvDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustAdvDt);
                return CustAdvDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DataTable getGiftCardDetails(string sCardNum)
        {
            DataTable dtReturn = new DataTable();
            try
            {
                if (this.Application.TransactionServices.CheckConnection())
                {

                    ReadOnlyCollection<object> containerArray;

                    containerArray = this.Application.TransactionServices.InvokeExtension("GetGiftCardDetails", sCardNum);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        dtReturn = dsWH.Tables[0];
                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }

            return dtReturn;
        }

        /// <summary>
        /// Void payment of gift card.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="gcTenderLineItem"></param>
        public void VoidGiftCardPayment(ref bool voided, ref string comment, IGiftCardTenderLineItem gcTenderLineItem)
        {
            LogMessage("Cancelling the used marking of the gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.VoidGiftCardPayment");

            GiftCertificateTenderLineItem giftCardTenderLineItem = gcTenderLineItem as GiftCertificateTenderLineItem;
            if (giftCardTenderLineItem == null)
            {
                throw new ArgumentNullException("gcTenderLineItem");
            }

            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.VoidGiftCardPayment(ref voided, ref comment, giftCardTenderLineItem.SerialNumber,
                    giftCardTenderLineItem.Transaction.StoreId, giftCardTenderLineItem.Transaction.TerminalId);
            }
        }

        /// <summary>
        /// Updates gift card details.
        /// </summary>
        /// <param name="updated"></param>
        /// <param name="comment"></param>
        /// <param name="gcTenderLineItem"></param>
        public void UpdateGiftCard(ref bool updated, ref string comment, IGiftCardTenderLineItem gcTenderLineItem)
        {
            LogMessage("Reedming money from gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.UpdateGiftCard");

            GiftCertificateTenderLineItem giftCardTenderLineItem = gcTenderLineItem as GiftCertificateTenderLineItem;
            if (giftCardTenderLineItem == null)
            {
                throw new ArgumentNullException("gcTenderLineItem");
            }

            decimal balance = 0;

            // Begin by checking if there is a connection to the Transaction Service
            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.GiftCardPayment(ref updated, ref comment, ref balance,
                    giftCardTenderLineItem.SerialNumber, giftCardTenderLineItem.Transaction.StoreId,
                    giftCardTenderLineItem.Transaction.TerminalId, giftCardTenderLineItem.Transaction.OperatorId,
                    giftCardTenderLineItem.Transaction.TransactionId, giftCardTenderLineItem.Transaction.ReceiptId,
                    ApplicationSettings.Terminal.StoreCurrency, giftCardTenderLineItem.Amount, DateTime.Now);

                // Update the balance in Tender line item.
                giftCardTenderLineItem.Balance = balance;
            }
        }

        /// <summary>
        /// Handles Gift Card Balance operation.
        /// </summary>
        public void GiftCardBalance(IPosTransaction posTransaction)
        {
            LogMessage("Inquiring gift card balance.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.GiftCardBalance");

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardBalance, (PosTransaction)posTransaction);

            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);
            }
        }

        /// <summary>
        /// Handles AddTo Gift Card operation.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="gcTenderInfo"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddToGiftCard(IRetailTransaction retailTransaction, ITender gcTenderInfo)
        {

            //Nimbus Start
            //Start: added on 16/07/2014 for customer selection is must
            RetailTransaction retailTrans = retailTransaction as RetailTransaction;


            if (retailTrans != null) //SaleAdjustmentGoldAmt
            {

                if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty || string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                {

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Add a customer to transaction before making a deposit", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                if ((Convert.ToDecimal(retailTrans.AmountDue)) < 0) // add on 31/12/14 req mail by pranay as wel as Mr.A.MitraretailTrans.PartnerData.SaleAdjustmentGoldAmt
                {

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Gift Card Can not be Adjusted with this transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }
            }
            //End: added on 16/07/2014 for customer selection is must

            //Nimbus End


            LogMessage("Adding money to gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.AddToGiftCard");

            if (GiftCard.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                //The operation should proceed after the fiscal printer handles AddToGiftCard
                GiftCard.InternalApplication.Services.Peripherals.FiscalPrinter.AddToGiftCard(retailTransaction, gcTenderInfo);
            }

            GiftCardController controller = new GiftCardController(GiftCardController.ContextType.GiftCardAddTo, (PosTransaction)retailTransaction, (Tender)gcTenderInfo);

            if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
            {
                try
                {
                    controller.PreValidateAddToGiftCard();
                }
                catch (GiftCardException ex)
                {
                    ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                    Application.Services.Dialog.ShowMessage(ex.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            using (GiftCardForm giftCardForm = new GiftCardForm(controller))
            {
                POSFormsManager.ShowPOSForm(giftCardForm);

                if (giftCardForm.DialogResult == DialogResult.OK)
                {
                    // Add the gift card to the transaction.
                    GiftCertificateItem giftCardItem = (GiftCertificateItem)this.Application.BusinessLogic.Utility.CreateGiftCardLineItem(
                        ApplicationSettings.Terminal.StoreCurrency, this.Application.Services.Rounding, retailTransaction);

                    if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                    {
                        InitGiftCardItem(giftCardItem);
                    }

                    giftCardItem.SerialNumber = controller.CardNumber;
                    giftCardItem.StoreId = retailTransaction.StoreId;
                    giftCardItem.TerminalId = retailTransaction.TerminalId;
                    giftCardItem.StaffId = retailTransaction.OperatorId;
                    giftCardItem.TransactionId = retailTransaction.TransactionId;
                    giftCardItem.ReceiptId = retailTransaction.ReceiptId;
                    giftCardItem.Amount = controller.Amount;
                    giftCardItem.Balance = controller.Balance;
                    giftCardItem.Date = DateTime.Now;
                    giftCardItem.AddTo = true;

                    giftCardItem.Price = giftCardItem.Amount;
                    giftCardItem.StandardRetailPrice = giftCardItem.Amount;
                    giftCardItem.Quantity = 1;
                    giftCardItem.TaxRatePct = 0;
                    if (!ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                    {
                        giftCardItem.Description = ApplicationLocalizer.Language.Translate(55000);  // Add to Gift Card
                    }
                    giftCardItem.Comment = controller.CardNumber;
                    giftCardItem.NoDiscountAllowed = true;
                    giftCardItem.Found = true;

                    ((RetailTransaction)retailTransaction).Add(giftCardItem);

                    if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                    {
                        // Calculate the tax for gift card item
                        this.Application.Services.Tax.CalculateTax(retailTransaction);
                    }
                }

                //Nimbus Start
                if (controller.Amount > 0 && controller.Context == Microsoft.Dynamics.Retail.Pos.GiftCard.GiftCardController.ContextType.GiftCardAddTo)
                    retailTrans.PartnerData.IsAddToGiftCard = "1";
                else
                    retailTrans.PartnerData.IsAddToGiftCard = "0";
                //Nimbus end
            }
        }

        /// <summary>
        /// Void a gift card deposit line item.
        /// </summary>
        /// <param name="voided">Return true if sucessful, false otherwise.</param>
        /// <param name="comment">Error text if failed.</param>
        /// <param name="gcLineItem">Gift card line item.</param>
        public void VoidAddToGiftCard(ref bool voided, ref string comment, IGiftCardLineItem gcLineItem)
        {
            LogMessage("Voiding money addition to gift card.",
                LSRetailPosis.LogTraceLevel.Trace,
                "GiftCard.VoidGiftCardDeposit");

            if (gcLineItem == null)
                throw new ArgumentNullException("gcLineItem");

            decimal balance = 0;

            if (this.Application.TransactionServices.CheckConnection())
            {
                this.Application.TransactionServices.AddToGiftCard(ref voided, ref comment, ref balance, gcLineItem.SerialNumber,
                    gcLineItem.StoreId, gcLineItem.TerminalId, gcLineItem.StaffId, gcLineItem.TransactionId, gcLineItem.ReceiptId,
                    ApplicationSettings.Terminal.StoreCurrency, decimal.Negate(gcLineItem.Amount), DateTime.Now);
            }
        }

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceLevel"></param>
        /// <param name="args"></param>
        private void LogMessage(string message, LogTraceLevel traceLevel, params object[] args)
        {
            ApplicationLog.Log(this.GetType().Name, string.Format(message, args), traceLevel);
        }

        /// <summary>
        /// Inits GiftCertificateItem from the default gift card item specified in the RetailParameters.GiftCardItem field
        /// </summary>
        /// <param name="giftCardItem">Gift card item</param>
        private void InitGiftCardItem(GiftCertificateItem giftCardItem)
        {
            giftCardItem.ItemId = ApplicationSettings.Terminal.ItemToGiftCard;
            this.Application.Services.Item.ProcessItem(giftCardItem, true);
            giftCardItem.ItemId = string.Empty; // by design
        }

        #endregion
    }
}
