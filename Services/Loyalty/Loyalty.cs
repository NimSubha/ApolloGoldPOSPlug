/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.POSControls.Touch;
using LSRetailPosis.POSProcesses.Common;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.LoyaltyItem;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using LoyaltyItemUsageType = LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType;
using LP = LSRetailPosis.POSProcesses;
using System.Data.SqlClient;
using System.Text;
using System.Net;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.Loyalty
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [Export(typeof(ILoyalty))]
    public class Loyalty : ILoyalty
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Loyalty are reserved at 50050 - 50099

        //transaction context
        private RetailTransaction transaction;

        //local UI message
        private frmMessage popupDialog;

        private enum OTPTransactionType
        {
            None = 0,
            Loaylty = 1,
        }

        #region Properties

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

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        private IUtility Utility
        {
            get { return this.Application.BusinessLogic.Utility; }
        }

        private ICustomerSystem CustomerSystem
        {
            get { return this.Application.BusinessLogic.CustomerSystem; }
        }

        #endregion

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

        #region ILoyalty Members

        /// <summary>
        /// Returns true if add/update loyalty item to transaction is successfull.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="cardNumber"></param>
        /// <returns></returns>
        public bool AddLoyaltyRequest(IRetailTransaction retailTransaction, string cardNumber)
        {
            ICardInfo cardInfo = Utility.CreateCardInfo();
            cardInfo.CardNumber = cardNumber;

            return AddLoyaltyRequest(retailTransaction, cardInfo);
        }

        /// <summary>
        /// Returns true if add/update loyalty item to transaction is successfull.
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public bool AddLoyaltyRequest(IRetailTransaction retailTransaction, ICardInfo cardInfo)
        {
            try
            {
                try
                {
                    NewMessageWindow(50050, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    LogMessage("Adding a loyalty record to the transaction...",
                        LSRetailPosis.LogTraceLevel.Trace,
                        "Loyalty.AddLoyaltyItem");

                    this.transaction = (RetailTransaction)retailTransaction;

                    // If a previous loyalty item exists on the transaction, the system should prompt the user whether to 
                    // overwrite the existing loyalty item or cancel the operation.
                    if (transaction.LoyaltyItem.LoyaltyCardNumber != null)
                    {
                        // Display the dialog
                        using (frmMessage dialog = new frmMessage(50055, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        {
                            LP.POSFormsManager.ShowPOSForm(dialog);
                            DialogResult result = dialog.DialogResult;

                            if (result != System.Windows.Forms.DialogResult.Yes)
                            {
                                return false;
                            }
                        }

                        // If card to be overridden is being used as tender type then block loyalty payment.
                        if (transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyTender)
                        {
                            LP.POSFormsManager.ShowPOSMessageDialog(3223);  // This transaction already contains a loyalty request.
                            return false;
                        }
                    }

                    // Add the loyalty item to the transaction
                    //LoyaltyItem loyaltyItem = GetLoyaltyItem(ref cardInfo);//base

                    string sLoyaltyCardId = GetLOYALTYCARDNoByCustAcc(this.transaction.Customer.CustomerId);
                    if (!string.IsNullOrEmpty(sLoyaltyCardId))
                    {
                        LoyaltyItem loyaltyItem = GetLoyaltyItem(ref cardInfo, sLoyaltyCardId);

                        if (loyaltyItem != null)
                        {
                            transaction.LoyaltyItem = loyaltyItem;
                            this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyRequest;

                            UpdateTransactionWithNewCustomer(loyaltyItem.CustID, true);

                            UpdateTempTableCustLoyaltyNo(sLoyaltyCardId);

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        UpdateTempTableCustLoyaltyNo(sLoyaltyCardId);
                        return false;
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::AddLoyaltyRequest failed for retailTransaction {0} cardInfo {1}", retailTransaction.TransactionId, cardInfo.CardNumber);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// This creates a loyalty line item based on the passed in card info, or prompts
        ///  to initialize default loyalty card info if no card info is supplied.
        /// </summary>
        /// <param name="cardInfo">Card info of loyalty card. If null, it will be initialized through user prompt.</param>
        /// <returns>Loyalty line item and loyalty card info if un-initialized</returns>
        private LoyaltyItem GetLoyaltyItem(ref ICardInfo cardInfo, string sCardNo = "")
        {
            if (cardInfo == null)
            {
                // The loyalty card was not swiped and therefore we need to prompt for the card number...

                if (string.IsNullOrEmpty(sCardNo))
                {
                    using (LSRetailPosis.POSProcesses.frmInputNumpad inputDialog = new LSRetailPosis.POSProcesses.frmInputNumpad(true, true))
                    {
                        inputDialog.EntryTypes = NumpadEntryTypes.CardValidation;
                        inputDialog.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(50056);   //Loyalty card number
                        inputDialog.Text = ApplicationLocalizer.Language.Translate(50062); // Add loyalty card
                        LP.POSFormsManager.ShowPOSForm(inputDialog);
                        DialogResult result = inputDialog.DialogResult;
                        // Quit if cancel is pressed...
                        if (result != System.Windows.Forms.DialogResult.OK)
                        {
                            return null;
                        }
                        else
                        {
                            cardInfo = Utility.CreateCardInfo();
                            cardInfo.CardEntryType = CardEntryTypes.MANUALLY_ENTERED;
                            cardInfo.CardNumber = inputDialog.InputText;
                            // Set card type to Loyalty card since this is a loyalty payment
                            //  Calling GetCardType sets the tender type properties on the cardInfo object or prompts user for more information.
                            cardInfo.CardType = CardTypes.LoyaltyCard;
                            this.Application.Services.Card.GetCardType(ref cardInfo);
                        }
                    }
                }
                else
                {
                    cardInfo = Utility.CreateCardInfo();
                    cardInfo.CardEntryType = CardEntryTypes.MANUALLY_ENTERED;
                    cardInfo.CardNumber = sCardNo;
                    // Set card type to Loyalty card since this is a loyalty payment
                    //  Calling GetCardType sets the tender type properties on the cardInfo object or prompts user for more information.
                    cardInfo.CardType = CardTypes.LoyaltyCard;
                    this.Application.Services.Card.GetCardType(ref cardInfo);
                }
            }

            // Create the loyalty item
            LoyaltyItem loyaltyItem = (LoyaltyItem)this.Application.BusinessLogic.Utility.CreateLoyaltyItem();

            // Set its properties
            if (cardInfo.CardEntryType == CardEntryTypes.MAGNETIC_STRIPE_READ)
            {
                loyaltyItem.LoyaltyCardNumber = cardInfo.Track2Parts[0];
            }
            else
            {
                loyaltyItem.LoyaltyCardNumber = cardInfo.CardNumber;
            }

            // Check whether the card is allowed to collect loyalty points
            bool cardIsValid = false;
            string comment = string.Empty;
            int loyaltyTenderType = 0;

            // Try to find the loyalty card from local database first
            GetLoyaltyInfoFromDB(loyaltyItem, ref loyaltyTenderType);

            // If card is found at local (scheme ID is not set) and not blocked, the card is valid.
            // Otherwise, try to find the card from HQ.
            if (!string.IsNullOrEmpty(loyaltyItem.SchemeID) && (LoyaltyTenderTypeBase)loyaltyTenderType != LoyaltyTenderTypeBase.Blocked)
            {
                cardIsValid = true;
            }
            else
            {
                int loyaltyTenderTypeBase = 0;
                decimal pointsEarned = 0;
                string loyaltySchemeId = String.Empty;
                GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, ref loyaltySchemeId, loyaltyItem.LoyaltyCardNumber);
                if (string.IsNullOrEmpty(loyaltyItem.SchemeID) && !string.IsNullOrEmpty(loyaltySchemeId))
                {
                    loyaltyItem.SchemeID = loyaltySchemeId;
                }
            }

            if (cardIsValid)
            {
                if (string.IsNullOrEmpty(this.transaction.Customer.CustomerId)
                    || string.IsNullOrEmpty(loyaltyItem.CustID)
                    || string.Compare(this.transaction.Customer.CustomerId, loyaltyItem.CustID, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return loyaltyItem;
                }
                else
                {
                    // loyalty payment could change customer, which is not desirable under various condition. 
                    // All logic is captured in CustomerClear action, so we ask cashier to do that first.              
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(3222);  // You must clear the customer before performing this operation.
                    return null;
                }
            }
            else
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSStatusBarText("Card ID: " + loyaltyItem.LoyaltyCardNumber); //License only allows limited number of item sales
                LSRetailPosis.POSProcesses.frmMessage dialog = null;
                try
                {
                    if (string.IsNullOrEmpty(comment))
                    {
                        dialog = new LSRetailPosis.POSProcesses.frmMessage(50058, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }
                    else
                    {
                        dialog = new LSRetailPosis.POSProcesses.frmMessage(comment, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    }

                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);    // Invalid loyaltycard
                    return null;
                }
                finally
                {
                    if (dialog != null)
                    {
                        dialog.Dispose();
                    }
                }
            }
        }

        private void GetPointStatus(ref decimal points, ref bool valid, ref string comment, ref int loyaltyTenderTypeBase, string loyaltyCardNumber)
        {
            string loyaltySchemeId = String.Empty;
            GetPointStatus(ref points, ref valid, ref comment, ref loyaltyTenderTypeBase, ref loyaltySchemeId, loyaltyCardNumber);
        }

        private void GetPointStatus(ref decimal points, ref bool valid, ref string comment, ref int loyaltyTenderTypeBase, ref string loyaltySchemeId, string loyaltyCardNumber)
        {
            try
            {
                try
                {
                    NewMessageWindow(50051, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information); // Retrieving loyalty status 
                    this.Application.TransactionServices.GetLoyaltyPointsStatus(ref valid, ref comment, ref points, ref loyaltyTenderTypeBase, ref loyaltySchemeId, loyaltyCardNumber);
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Warning(ex, ex.Message);
                valid = false;
            }
        }

        /// <summary>
        /// Adds loyalty points as per given information.
        /// </summary>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void CalculateLoyaltyPoints(IRetailTransaction retailTransaction)
        {
            try
            {
                LogMessage("Adding loyalty points...",
                    LSRetailPosis.LogTraceLevel.Trace,
                    "Loyalty.CalculateLoyaltyPoints");

                this.transaction = (RetailTransaction)retailTransaction;

                //if we already have a loyalty item for tender, we don't accumulated points for this transaction.
                if (this.transaction.LoyaltyItem != null && this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyTender)
                {
                    return;
                }

                //calculate points.
                if (this.transaction.LoyaltyItem != null && this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyDiscount)
                {
                    //Retrieve calculation parameters
                    decimal qtyAmountLimit, points;
                    bool calcParametersFound = GetLoyaltyDiscountCalcParameters(this.transaction.LoyaltyItem.SchemeID, out qtyAmountLimit, out points);
                    if (calcParametersFound && qtyAmountLimit != decimal.Zero)
                    {
                        //Calculate number of points to use
                        decimal companyCurrencyAmount = Loyalty.InternalApplication.Services.Currency.CurrencyToCurrency(
                            ApplicationSettings.Terminal.StoreCurrency,
                            ApplicationSettings.Terminal.CompanyCurrency,
                            this.transaction.LoyaltyDiscount);
                        int sign = Math.Sign(companyCurrencyAmount);
                        this.transaction.LoyaltyItem.CalculatedLoyaltyPoints = Math.Floor(Math.Abs(companyCurrencyAmount) / qtyAmountLimit * points) * (decimal)sign;
                    }
                }
                else
                {
                    this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.NotUsed;
                    decimal totalNumberOfPoints = 0;

                    // Get the table containing the point logic
                    DataTable loyaltyPointsTable = GetLoyaltyPointsSchemeFromDB(this.transaction.LoyaltyItem.SchemeID);

                    // Loop through the transaction and calculate the aquired loyalty points. 
                    if (loyaltyPointsTable != null && loyaltyPointsTable.Rows.Count > 0)
                    {
                        totalNumberOfPoints = CalculatePointsForTransaction(loyaltyPointsTable);

                        this.transaction.LoyaltyItem.CalculatedLoyaltyPoints = totalNumberOfPoints;
                        this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyRequest;
                    }

                    UpdateTransactionAccumulatedLoyaltyPoint();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::CalculateLoyaltyPoints failed for retailTransaction {0}", retailTransaction.TransactionId);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Adds loyalty payments for the given card information.
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="amount"></param>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddLoyaltyPayment(IRetailTransaction retailTransaction, ICardInfo cardInfo, decimal amount)
        {
            try
            {
                try
                {
                    NewMessageWindow(50051, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    this.transaction = (RetailTransaction)retailTransaction;

                    // Getting the loyalty info for the card, not including the points status
                    // Points status will be retrieved later.
                    LoyaltyItem paymentLoyaltyItem = GetLoyaltyItem(ref cardInfo);

                    if (paymentLoyaltyItem != null)
                    {

                        #region Nim added  start
                        /*if (BarcodeService.Barcode.IsMakingChargeItemExist(transaction))
                        {
                            balancepoints = LoyaltyTransactionService.GetLoyaltyPointsBalance(transaction.LoyaltyItem.LoyaltyCardNumber);
                            int intBalPoint = Convert.ToInt32(balancepoints);
                            decimal value = LoyaltyTransactionService.GetLoyaltyPointsValue(transaction.LoyaltyItem.LoyaltyCardNumber);

                            if (balancepoints > 0)
                            {
                                string queryString1 = "SELECT CARDNUMBER FROM RETAILLOYALTYMSRCARDTABLE WHERE LOYALTYCUSTID='" + transaction.Customer.CustomerId + "' ";
                                DataTable trData;
                                using (SqlCommand command1 = new SqlCommand(queryString1, LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection))
                                {
                                    if (LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection.State != ConnectionState.Open) { LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection.Open(); }
                                    using (SqlDataReader reader1 = command1.ExecuteReader())
                                    {
                                        using (DataTable TransTable1 = new DataTable())
                                        {
                                            TransTable1.Load(reader1);
                                            trData = TransTable1.Copy();
                                        }
                                    }
                                }
                                String msg = String.Empty;
                                if (!String.IsNullOrEmpty(trData.Rows[0]["LOYALTYTIERID"].ToString()))
                                {
                                    msg = "Available Points: " + intBalPoint + ", Amount : " + value.ToString("#.00") + " and Currenet TR: " + trData.Rows[0]["LOYALTYTIERID"].ToString();
                                }
                                else
                                {
                                    msg = "Available Points: " + intBalPoint + ", Amount : " + value.ToString("#.00");
                                }
                                if (LoyaltyGUI.PromptInfoMessageAcx(msg + " Are you sure to redeem"))
                                {
                                    LoyaltyTransactionService.RequestOTP(transaction.Customer.CustomerId);
                                    dtOtprequestTime = DateTime.Now;
                                    frmLoyaltyOTP frmobj = new frmLoyaltyOTP(transaction.Customer.CustomerId);
                                    frmobj.ShowDialog();
                                }
                                if (!IsValidOTP)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                LoyaltyGUI.PromptError("Sorry!Does not have balance to redeem!");
                                return;

                            }

                        }
                        else
                        {
                            LoyaltyGUI.PromptError("Transaction does not contain any making charge item!");
                            return;
                        }*/

                        #endregion Nim added end


                        //customerData.
                        if (this.transaction.Customer == null || string.Equals(this.transaction.Customer.CustomerId, paymentLoyaltyItem.CustID, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            UpdateTransactionWithNewCustomer(paymentLoyaltyItem.CustID, false);
                        }

                        // if the amount is higher than the "new" NetAmountWithTax, then it is acceptable to lower the amount
                        if (Math.Abs(amount) > Math.Abs(this.transaction.TransSalePmtDiff))
                        {
                            amount = this.transaction.TransSalePmtDiff;
                        }

                        // Getting all possible loyalty posssiblities for the found scheme id
                        DataTable loyaltyPointsTable = GetLoyaltyPointsSchemeFromDB(paymentLoyaltyItem.SchemeID);
                        //decimal dMakingAmt = 0m;
                        //decimal dTotLineDisc = 0m;
                        //dTotLineDisc = this.transaction.LineDiscount;

                        //dMakingAmt = getMakingInfo();

                        // CRWValidateLoyaltyAmt(cardInfo, dMakingAmt - dTotLineDisc, paymentLoyaltyItem, loyaltyPointsTable);// extraced method from base by nim on 290319
                        //if ((dMakingAmt - dTotLineDisc) < amount)
                        //{
                        //    string sMsg = "The redeemed value is greater than the remaining making charges : " + Convert.ToDecimal(decimal.Round(dMakingAmt - dTotLineDisc, 2, MidpointRounding.AwayFromZero));

                        //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sMsg, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                        //    {
                        //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);//pointsEarned
                        //    }
                        //}
                        //else
                        //{
                            CRWValidateLoyaltyAmt(cardInfo, amount, paymentLoyaltyItem, loyaltyPointsTable);// extraced method from base by nim on 290319
                        //}
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::AddLoyaltyPayment failed for retailTransaction {0} cardInfo {1} amount {2}", retailTransaction.TransactionId, cardInfo.CardNumber, amount);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private void CRWValidateLoyaltyAmt(ICardInfo cardInfo, decimal amount, LoyaltyItem paymentLoyaltyItem, DataTable loyaltyPointsTable)
        {
            decimal totalNumberOfPoints = 0;
            bool tenderRuleFound = false;
            // now we add the points needed to pay current tender
            totalNumberOfPoints = CalculatePointsForTender(ref tenderRuleFound, cardInfo.TenderTypeId, amount, loyaltyPointsTable);

            if (tenderRuleFound)
            {
                bool cardIsValid = false;
                string comment = string.Empty;
                int loyaltyTenderTypeBase = 0;
                decimal pointsEarned = 0;

                // check to see if the user can afford so many points
                GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, paymentLoyaltyItem.LoyaltyCardNumber);
                paymentLoyaltyItem.AccumulatedLoyaltyPoints = pointsEarned;

                if ((cardIsValid) && ((LoyaltyTenderTypeBase)loyaltyTenderTypeBase != LoyaltyTenderTypeBase.NoTender))
                {
                    if (pointsEarned >= (totalNumberOfPoints * -1))
                    {
                        //customerData.
                        if (this.transaction.Customer == null || string.Equals(this.transaction.Customer.CustomerId, paymentLoyaltyItem.CustID, StringComparison.OrdinalIgnoreCase) == false)
                        {
                            UpdateTransactionWithNewCustomer(paymentLoyaltyItem.CustID, false);
                        }

                        //Add loyalty item to transaction.
                        this.transaction.LoyaltyItem = paymentLoyaltyItem;
                        this.transaction.LoyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyTender;

                        // Gathering tender information
                        TenderData tenderData = new TenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                        ITender tenderInfo = tenderData.GetTender(cardInfo.TenderTypeId, ApplicationSettings.Terminal.StoreId);

                        // this is the grand total
                        decimal totalAmountDue = this.transaction.TransSalePmtDiff - amount;

                        TenderRequirement tenderRequirement = new TenderRequirement((Tender)tenderInfo, amount, true, this.transaction.TransSalePmtDiff);
                        if (!string.IsNullOrWhiteSpace(tenderRequirement.ErrorText))
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(tenderRequirement.ErrorText, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }

                        //Add a loyalty tender item to transaction.
                        LoyaltyTenderLineItem loyaltyTenderItem = (LoyaltyTenderLineItem)this.Application.BusinessLogic.Utility.CreateLoyaltyTenderLineItem();
                        loyaltyTenderItem.CardNumber = paymentLoyaltyItem.LoyaltyCardNumber;
                        loyaltyTenderItem.CardTypeId = cardInfo.CardTypeId;
                        loyaltyTenderItem.Amount = amount;

                        //tenderInfo.
                        loyaltyTenderItem.Description = tenderInfo.TenderName;
                        loyaltyTenderItem.TenderTypeId = cardInfo.TenderTypeId;
                        loyaltyTenderItem.LoyaltyPoints = totalNumberOfPoints;

                        //convert from the store-currency to the company-currency...
                        loyaltyTenderItem.CompanyCurrencyAmount = this.Application.Services.Currency.CurrencyToCurrency(
                            ApplicationSettings.Terminal.StoreCurrency,
                            ApplicationSettings.Terminal.CompanyCurrency,
                            amount);

                        // the exchange rate between the store amount(not the paid amount) and the company currency
                        loyaltyTenderItem.ExchrateMST = this.Application.Services.Currency.ExchangeRate(
                            ApplicationSettings.Terminal.StoreCurrency) * 100;

                        // card tender processing and printing require an EFTInfo object to be attached. 
                        // however, we don't want loyalty info to show up where other EFT card info would on the receipt 
                        //  because loyalty has its own receipt template fields, so we just assign empty EFTInfo object
                        loyaltyTenderItem.EFTInfo = Application.BusinessLogic.Utility.CreateEFTInfo();
                        // we don't want Loyalty to be 'captured' by payment service, so explicitly set not to capture to be safe
                        loyaltyTenderItem.EFTInfo.IsPendingCapture = false;

                        loyaltyTenderItem.SignatureData = LSRetailPosis.POSProcesses.TenderOperation.ProcessSignatureCapture(tenderInfo, loyaltyTenderItem);

                        this.transaction.Add(loyaltyTenderItem);


                        #region OTP sms sent validate by Nim -- commented for Apollo
                        //string sMNo = getCustomerMobilePrimary(this.transaction.Customer.CustomerId);
                        //string sTextSMS = "";
                        //int iOTP = GenerateRandomNo();
                        //DateTime dtOtprequestTime = DateTime.Now;
                        //sTextSMS = "Dear " + this.transaction.Customer.Name + " Your OTP to authorize loyalty point redemption at PNG Jewellers Store is " + iOTP + ". This OTP will be valid for 2 minutes";

                        //SendSMS(sTextSMS, sMNo);
                        //SaveLoyaltyOTPInfo(this.transaction, Convert.ToString(iOTP), this.transaction.Customer.CustomerId);
                        //frmLoyaltyOTP objLo = new frmLoyaltyOTP(this.transaction.Customer.CustomerId, this.transaction.Customer.Name, this.transaction.TransactionId, dtOtprequestTime);
                        //objLo.ShowDialog();

                        //if (objLo.IsValidOTP)
                        //{
                        //    this.transaction.Add(loyaltyTenderItem);
                        //}
                        //else
                        //{
                        //    cardIsValid = false;
                        //    return;
                        //}
                        #endregion
                    }
                    else
                    {
                        //using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(50057, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                        //{
                        //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);//pointsEarned
                        //} // Not enough points available to complete payment
                        decimal dAmt = getLoayltyRedeemableAmt(paymentLoyaltyItem.LoyaltyCardNumber, pointsEarned);
                        string sMsg = "Not enough points available to complete payment. " + " Total redeemable value is :  " + dAmt;

                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sMsg, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);//pointsEarned
                        }
                    }
                }
                else
                {
                    LSRetailPosis.POSProcesses.frmMessage dialog = null;
                    try
                    {
                        if (string.IsNullOrEmpty(comment))
                        {
                            dialog = new LSRetailPosis.POSProcesses.frmMessage(50058, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }
                        else
                        {
                            dialog = new LSRetailPosis.POSProcesses.frmMessage(comment, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        }

                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);    // Invalid loyaltycard  
                    }
                    finally
                    {
                        if (dialog != null)
                        {
                            dialog.Dispose();
                        }
                    }
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(50059, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog); // Not enough points available to complete payment

                }
            }
        }

        /// <summary>
        /// Returns true if add/update loyalty item to transaction is successfull
        /// </summary>
        /// <remarks>This method to be invoked in Non-CustomerOrder mode only</remarks>
        /// <param name="retailTransaction"></param>
        /// <param name="cardNumber"></param>
        /// <param name="discountAmount"></param>
        /// <returns></returns>
        public bool AddLoyaltyDiscount(IRetailTransaction retailTransaction, string cardNumber, decimal discountAmount)
        {
            try
            {
                try
                {
                    NewMessageWindow(50050, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    LogMessage("Adding a loyalty record to the transaction...",
                        LSRetailPosis.LogTraceLevel.Trace,
                        "Loyalty.AddLoyaltyItem");

                    this.transaction = (RetailTransaction)retailTransaction;

                    // If a previous loyalty item exists on the transaction, the system should prompt the user whether to 
                    // overwrite the existing loyalty item or cancel the operation
                    if ((transaction.LoyaltyItem != null) && (!string.IsNullOrWhiteSpace(transaction.LoyaltyItem.LoyaltyCardNumber)))
                    {
                        if (!transaction.LoyaltyItem.LoyaltyCardNumber.Equals(cardNumber, StringComparison.Ordinal))
                        {
                            using (frmMessage dialog = new frmMessage(50055, MessageBoxButtons.YesNo, MessageBoxIcon.Question)) // Do you want to overwrite the loyalty record?
                            {
                                LP.POSFormsManager.ShowPOSForm(dialog);
                                if (dialog.DialogResult != DialogResult.Yes)
                                {
                                    return false;
                                }
                            }
                        }
                    }

                    // Add the loyalty item to the transaction
                    LoyaltyItem loyaltyItem = GetLoyaltyItem(cardNumber) as LoyaltyItem;

                    if (loyaltyItem == null)
                    {
                        return false;
                    }

                    //Update customer
                    if (!string.IsNullOrEmpty(loyaltyItem.CustID))
                    {
                        if (this.transaction.Customer == null || string.IsNullOrEmpty(this.transaction.Customer.CustomerId))
                        {
                            UpdateTransactionWithNewCustomer(loyaltyItem.CustID, false);
                        }
                        else if (!string.Equals(transaction.Customer.CustomerId, loyaltyItem.CustID, StringComparison.OrdinalIgnoreCase))
                        {
                            // loyalty payment could change customer, which is not desirable under various condition. 
                            // All logic is captured in CustomerClear action, so we ask cashier to do that first.              
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(3222);  // You must clear the customer before performing this operation.
                            return false;
                        }
                    }

                    // if the amount is higher than the "new" NetAmountWithTax, then it is acceptable to lower the amount
                    if (Math.Abs(discountAmount) > Math.Abs(this.transaction.TransSalePmtDiff))
                    {
                        discountAmount = this.transaction.TransSalePmtDiff;
                    }

                    if (discountAmount != decimal.Zero)
                    {
                        loyaltyItem.UsageType = LoyaltyItemUsageType.UsedForLoyaltyDiscount;
                    }

                    //Retrieve calculation parameters
                    decimal qtyAmountLimit, points;
                    bool calcParametersFound = GetLoyaltyDiscountCalcParameters(loyaltyItem.SchemeID, out qtyAmountLimit, out points);
                    if (!calcParametersFound || qtyAmountLimit == decimal.Zero)
                    {
                        return false;
                    }

                    //Calculate number of points to use
                    decimal companyCurrencyAmount = Loyalty.InternalApplication.Services.Currency.CurrencyToCurrency(
                        ApplicationSettings.Terminal.StoreCurrency,
                        ApplicationSettings.Terminal.CompanyCurrency,
                        discountAmount);
                    int sign = Math.Sign(companyCurrencyAmount);
                    loyaltyItem.CalculatedLoyaltyPoints = Math.Floor(Math.Abs(companyCurrencyAmount) / qtyAmountLimit * points) * (decimal)sign;

                    if ((sign > decimal.Zero) && (Math.Abs(loyaltyItem.CalculatedLoyaltyPoints) > loyaltyItem.AccumulatedLoyaltyPoints))
                    {
                        LP.POSFormsManager.ShowPOSErrorDialog(new PosisException(ApplicationLocalizer.Language.Translate(50057))); // The loyalty card does not have enough points.
                        return false;
                    }

                    // Add loyalty item to transaction
                    this.transaction.LoyaltyItem = loyaltyItem;

                    return true;
                }
                finally
                {
                    CloseExistingMessageWindow();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::AddLoyaltyDiscount failed for retailTransaction {0} cardNumber {1} discountAmount {2}", this.transaction.TransactionId, cardNumber, discountAmount);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new loyalty item based on the passed in card number
        /// </summary>
        /// <param name="cardNumber">Loyalty Card number</param>
        /// <returns>Loyalty item</returns>
        public ILoyaltyItem GetLoyaltyItem(string cardNumber)
        {
            ILoyaltyItem loyaltyItem = null;

            bool cardIsValid = false;
            string comment = string.Empty;
            int loyaltyTenderTypeBase = 0;
            decimal pointsEarned = 0;

            GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, cardNumber);

            if (cardIsValid)
            {
                loyaltyItem = this.Application.BusinessLogic.Utility.CreateLoyaltyItem();
                loyaltyItem.LoyaltyCardNumber = cardNumber;
                loyaltyItem.AccumulatedLoyaltyPoints = pointsEarned;
                GetLoyaltyInfoFromDB(loyaltyItem as LoyaltyItem, ref loyaltyTenderTypeBase);

                decimal qtyAmountLimit, points;
                bool calcParametersFound = GetLoyaltyDiscountCalcParameters(loyaltyItem.SchemeID, out qtyAmountLimit, out points);
                if (calcParametersFound && points != decimal.Zero)
                {
                    loyaltyItem.Amount = Loyalty.InternalApplication.Services.Currency.CurrencyToCurrency(
                        ApplicationSettings.Terminal.CompanyCurrency,
                        ApplicationSettings.Terminal.StoreCurrency,
                        Math.Abs(loyaltyItem.AccumulatedLoyaltyPoints / points * qtyAmountLimit));
                }
                else
                {
                    loyaltyItem = null;
                }
            }
            else
            {
                comment = string.IsNullOrWhiteSpace(comment) ? ApplicationLocalizer.Language.Translate(50058) : comment; // Invalid loyaltycard
                FormatAndShowErrorMessage(comment);
            }

            return loyaltyItem;
        }

        /// <summary>
        /// Retrieves parameters for amount or points calculation for the loyalty scheme line type Discount
        /// </summary>
        /// <param name="schemeId"></param>
        /// <param name="qtyAmountLimit"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private bool GetLoyaltyDiscountCalcParameters(string schemeId, out decimal qtyAmountLimit, out decimal points)
        {
            qtyAmountLimit = 0;
            points = 0;

            DataTable loyaltyPointsTable = GetLoyaltyPointsSchemeFromDB(schemeId);

            DataRow loyaltyDiscountRow = null;
            if (loyaltyPointsTable != null)
            {
                foreach (DataRow row in loyaltyPointsTable.Rows)
                {
                    LoyaltyPointTypeBase type = (LoyaltyPointTypeBase)row["PRODUCTTENDERTYPE"];
                    if ((type == LoyaltyPointTypeBase.Discount) && IsValidDate(Convert.ToDateTime(row["VALIDFROM"]), Convert.ToDateTime(row["VALIDTO"])))
                    {
                        loyaltyDiscountRow = row;
                        break; // Pick up the first found loyalty points discount row - confirmed by PM
                    }
                }
            }

            string comment = null;

            if (loyaltyDiscountRow != null)
            {
                qtyAmountLimit = Utility.ToDecimal(loyaltyDiscountRow["QTYAMOUNTLIMIT"].ToString());
                points = Utility.ToDecimal(loyaltyDiscountRow["POINTS"]);

                if (qtyAmountLimit > decimal.Zero && points < decimal.Zero)
                {
                    return true;
                }
                else
                {
                    comment = string.Format(ApplicationLocalizer.Language.Translate(55604), schemeId); //The points to discount conversion is incorrectly set up for the loyalty scheme {0}.
                }
            }
            else
            {
                comment = string.Format(ApplicationLocalizer.Language.Translate(55605), schemeId); //Loyalty point discounts are not configured for the cards that have the loyalty scheme {0}.
            }

            FormatAndShowErrorMessage(comment);

            return false;
        }

        /// <summary>
        /// Called when used points are being voided.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="retailTransaction"></param>
        /// <param name="loyaltyTenderItem"></param>
        public bool VoidLoyaltyPayment(IRetailTransaction retailTransaction, ILoyaltyTenderLineItem loyaltyTenderItem)
        {
            if (retailTransaction == null)
                throw new ArgumentNullException("retailTransaction");

            if (loyaltyTenderItem == null)
                throw new ArgumentNullException("loyaltyTenderItem");

            this.transaction = (RetailTransaction)retailTransaction;

            this.transaction.VoidPaymentLine(loyaltyTenderItem.LineId);
            this.transaction.LoyaltyItem = (LoyaltyItem)this.Application.BusinessLogic.Utility.CreateLoyaltyItem();

            UpdateTransactionWithNewCustomer(null, true);

            return true;
        }

        /// <summary>
        /// Called to update points when conclude a transaction.
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void UpdateLoyaltyPoints(IRetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
                throw new ArgumentNullException("retailTransaction");

            this.transaction = (RetailTransaction)retailTransaction;

            // Sending confirmation to the transactions service about earned points
            if (this.transaction.LoyaltyItem != null)
            {
                if (this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyRequest)
                {
                    UpdateIssuedLoyaltyPoints(this.transaction.LoyaltyItem);

                    if (this.transaction.LoyaltyItem.CalculatedLoyaltyPoints < 0)
                    {
                        bool cardIsValid = false;
                        string comment = string.Empty;
                        int loyaltyTenderTypeBase = 0;
                        decimal pointsEarned = 0M;

                        GetPointStatus(ref pointsEarned, ref cardIsValid, ref comment, ref loyaltyTenderTypeBase, this.transaction.LoyaltyItem.LoyaltyCardNumber);

                        if (cardIsValid && pointsEarned < 0)
                        {
                            string message = string.Format(LSRetailPosis.ApplicationLocalizer.Language.Translate(50500), pointsEarned);

                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(message, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog); // Not enough points available to complete payment
                            }
                        }
                    }
                }
                else if (this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyTender)
                {
                    // Sending confirmation to the transaction service about used points
                    foreach (ITenderLineItem tenderItem in this.transaction.TenderLines)
                    {
                        ILoyaltyTenderLineItem asLoyaltyTenderLineItem = tenderItem as ILoyaltyTenderLineItem;

                        if ((asLoyaltyTenderLineItem != null) && !asLoyaltyTenderLineItem.Voided)
                        {
                            if (asLoyaltyTenderLineItem.LoyaltyPoints != 0)
                            {
                                UpdateUsedLoyaltyPoints(asLoyaltyTenderLineItem);
                            }
                        }
                    }
                }
                else if (this.transaction.LoyaltyItem.UsageType == LoyaltyItemUsageType.UsedForLoyaltyDiscount)
                {
                    if (this.transaction.LoyaltyDiscount != decimal.Zero)
                    {
                        UpdateUsedLoyaltyPoints();
                    }
                }
            }
        }

        /// <summary>
        /// Submits the request to print loyalty card balance to the printer.
        /// </summary>
        /// <param name="loyaltyCardData">An instance of the <see cref="ILoyaltyCardData"/> holding the information about the loyalty card.</param>
        public void PrintLoyaltyBalance(ILoyaltyCardData loyaltyCardData)
        {
            if (Loyalty.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Loyalty.InternalApplication.Services.Peripherals.FiscalPrinter.PrintLoyaltyCardBalance(loyaltyCardData);
            }
            else
            {
                Loyalty.InternalApplication.Services.Dialog.ShowMessage(86501, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        private void FormatAndShowErrorMessage(string comment)
        {
            if (this.transaction != null && this.transaction.SaleIsReturnSale)
            {
                comment = string.Format(ApplicationLocalizer.Language.Translate(55609), comment); //An error occurred while refunding loyalty points: {0) Refund points manually.
            }
            else
            {
                comment = string.Format(ApplicationLocalizer.Language.Translate(55610), comment); //{0} Enter another card number or contact your system administrator to resolve this issue.
            }
            using (frmMessage dialog = new frmMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Error))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            }
        }

        private void GetLoyaltyInfoFromDB(LoyaltyItem loyaltyItem, ref int loyaltyTenderType)
        {
            SqlSelect sql = new SqlSelect("RETAILLOYALTYMSRCARDTABLE M "
                + "INNER JOIN RETAILLOYALTYCUSTTABLE C ON M.LOYALTYCUSTID = C.LOYALTYCUSTID "
                + "INNER JOIN RETAILLOYALTYSCHEMESTABLE S ON S.LOYALTYSCHEMEID = M.LOYALTYSCHEMEID ");
            sql.Select("M.LOYALTYSCHEMEID");
            sql.Select("M.LOYALTYCUSTID");
            sql.Select("ACCOUNTNUM");
            sql.Select("EXPIRATIONTIMEUNIT");
            sql.Select("EXPIRATIONTIMEVALUE");
            sql.Select("LOYALTYTENDER");
            sql.Where("M.DATAAREAID", Application.Settings.Database.DataAreaID, true);
            sql.Where("M.CARDNUMBER", loyaltyItem.LoyaltyCardNumber, true);   // Sale Unit of Measure

            DataTable dataTable = new DBUtil(Application.Settings.Database.Connection).GetTable(sql);
            if (dataTable.Rows.Count > 0)
            {
                loyaltyItem.SchemeID = Utility.ToString(dataTable.Rows[0]["LOYALTYSCHEMEID"]);
                loyaltyItem.LoyaltyCustID = Utility.ToString(dataTable.Rows[0]["LOYALTYCUSTID"]);
                loyaltyItem.CustID = Utility.ToString(dataTable.Rows[0]["ACCOUNTNUM"]);
                loyaltyItem.ExpireUnit = Utility.ToInt(dataTable.Rows[0]["EXPIRATIONTIMEUNIT"]);
                loyaltyItem.ExpireValue = Utility.ToInt(dataTable.Rows[0]["EXPIRATIONTIMEVALUE"]);
                loyaltyTenderType = Utility.ToInt(dataTable.Rows[0]["LOYALTYTENDER"]);
            }
        }

        private void UpdateTransactionAccumulatedLoyaltyPoint()
        {
            try
            {
                bool valid = false;
                decimal points = 0;
                int loyaltyTenderTypeBase = 0;
                string comment1 = string.Empty;

                GetPointStatus(ref points, ref valid, ref comment1, ref loyaltyTenderTypeBase, this.transaction.LoyaltyItem.LoyaltyCardNumber);
                if (valid && (this.transaction.LoyaltyItem != null))
                {
                    this.transaction.LoyaltyItem.AccumulatedLoyaltyPoints = points;
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::UpdateTransactionAccumulatedLoyaltyPoint failed for LoyaltyCardNumber {0}", this.transaction.LoyaltyItem.LoyaltyCardNumber);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private DataRow GetRetailGroupLineMember(Int64 recId)
        {
            try
            {
                SqlSelect sqlSelect = new SqlSelect("RETAILGROUPMEMBERLINE");
                sqlSelect.Select("*");
                sqlSelect.Where("RECID", recId, true);
                DataTable groupLineMember = new DBUtil(Application.Settings.Database.Connection).GetTable(sqlSelect);
                if (groupLineMember.Rows.Count == 1)
                {
                    return groupLineMember.Rows[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::GetRetailGroupLineMember failed for recId {0}. Returning null.", recId);
                return null;
            }
        }

        private DataTable GetLoyaltyPointsSchemeFromDB(string schemeID)
        {
            try
            {
                SqlSelect sqlSelect = new SqlSelect("RETAILLOYALTYPOINTSTABLE");
                sqlSelect.Select("*");
                sqlSelect.Where("LOYALTYSCHEMEID", schemeID, true);
                sqlSelect.Where("DATAAREAID", Application.Settings.Database.DataAreaID, true);
                return new DBUtil(Application.Settings.Database.Connection).GetTable(sqlSelect);
            }
            catch (Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::GetLoyaltyPointsSchemeFromDB failed for schemeID {0}. Returning null.", schemeID);
                return null;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private decimal CalculatePointsForTransactionByScheme(Int64 groupMemberLineRecId, decimal qtyAmountLimit, decimal points, CalculationTypeBase baseType, LoyaltyPointTypeBase type)
        {
            try
            {
                decimal totalQty = 0;
                decimal totalAmount = 0;
                Int64 variantId;
                Int64 productId;
                Int64 categoryId;
                DataRow groupMemberLine = GetRetailGroupLineMember(groupMemberLineRecId);
                if (groupMemberLine == null)
                {
                    NetTracer.Warning("Loyalty:CalculatePointsForTranactionByScheme: groupMemberLine is null");
                    return decimal.Zero;
                }

                categoryId = (Int64)groupMemberLine["Category"];
                productId = (Int64)groupMemberLine["Product"];
                variantId = (Int64)groupMemberLine["Variant"];

                if (type != LoyaltyPointTypeBase.Tender)
                {
                    foreach (SaleLineItem saleLineItem in this.transaction.SaleItems)
                    {
                        bool found = false;

                        if (!saleLineItem.Voided)
                        {
                            ItemData itemData = new ItemData(
                                ApplicationSettings.Database.LocalConnection,
                                ApplicationSettings.Database.DATAAREAID,
                                ApplicationSettings.Terminal.StorePrimaryId);

                            // check for a variant being put on loyalty
                            if (variantId != 0)
                            {
                                found = (variantId == saleLineItem.Dimension.DistinctProductVariantId);
                            }
                            // Check for a product or product master being put on loyalty
                            else if (productId != 0)
                            {
                                found = (productId == saleLineItem.ProductId);
                            }
                            // Check for a category being put on loyalty
                            else if (categoryId != 0)
                            {
                                found = itemData.ProductInCategory(saleLineItem.ProductId, saleLineItem.Dimension.DistinctProductVariantId, categoryId);
                            }
                        }

                        if (found)
                        {
                            //totalQty += saleLineItem.UnitQtyConversion.Convert(saleLineItem.Quantity);
                            //totalAmount += saleLineItem.NetAmount;

                            //=====================================Soutik=================================Calculate Loyalty Points Over Nett Wt rather than Qty=== Changes on 26-08-19 As per Supapta da
                            if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)) && Convert.ToString(saleLineItem.PartnerData.Ingredients) != "0")
                            {
                                int iMetalType = 0;
                                DataSet dsIngredients = new DataSet();
                                StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                dsIngredients.ReadXml(reader);
                                foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                {
                                    iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                    if ((iMetalType == (int)MetalType.Gold))
                                    {
                                        totalQty += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                    }
                                }

                                totalAmount += saleLineItem.NetAmount;
                            }
                            else
                            {
                                totalQty += saleLineItem.UnitQtyConversion.Convert(saleLineItem.Quantity);
                                totalAmount += saleLineItem.NetAmount;                                
                            }
                            //===========================================================================================
                        }
                    }
                }

                //when check limit, we use absolute value, as in return transaction, qty and amount could be nagative.
                if (qtyAmountLimit > 0)
                {
                    if (baseType == CalculationTypeBase.Amounts)
                    {
                        decimal companyCurrencyAmount = this.Application.Services.Currency.CurrencyToCurrency(
                                        ApplicationSettings.Terminal.StoreCurrency,
                                        ApplicationSettings.Terminal.CompanyCurrency,
                                        totalAmount);

                        //Check QtyAmountLimit only for non-tender loyalty point type.
                        if (Math.Abs(companyCurrencyAmount) >= qtyAmountLimit || type == LoyaltyPointTypeBase.Tender)
                        {
                            return companyCurrencyAmount > 0 ?
                                Math.Floor(companyCurrencyAmount / qtyAmountLimit * points) :
                                Math.Ceiling(companyCurrencyAmount / qtyAmountLimit * points);
                        }
                    }
                    else
                    {
                        if (Math.Abs(totalQty) >= qtyAmountLimit)
                        {
                            return totalQty > 0 ?
                                Math.Floor(totalQty / qtyAmountLimit * points) :
                                Math.Ceiling(totalQty / qtyAmountLimit * points);
                        }
                    }
                }

                // default
                return 0;
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Loyalty::CalculatePointsForTransactionByScheme failed for groupMemberLineRecId {0} qtyAmountLimit {1} points {2}", groupMemberLineRecId, qtyAmountLimit, points);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private decimal CalculatePointsForTransaction(DataTable loyaltyPointsTable)
        {
            decimal totalCollectedPoints = 0;
            foreach (DataRow row in loyaltyPointsTable.Rows)
            {
                LoyaltyPointTypeBase type = (LoyaltyPointTypeBase)row["PRODUCTTENDERTYPE"];
                Int64 groupMemberLine = (Int64)row["RETAILGROUPMEMBERLINE"];
                decimal qtyAmountLimit = Convert.ToDecimal(row["QTYAMOUNTLIMIT"].ToString());
                decimal points = Convert.ToDecimal(row["POINTS"]);
                CalculationTypeBase baseCalculationOn = (CalculationTypeBase)row["BASECALCULATIONON"];

                // Loyalty scheme ending date on the AX server is in a date only format
                // E.g. an expiry date of 4/12/2010 means that the loyalty scheme expires by the end of 4/12/2010
                // 

                if (IsValidDate(Convert.ToDateTime(row["VALIDFROM"]), Convert.ToDateTime(row["VALIDTO"])))
                {
                    totalCollectedPoints += CalculatePointsForTransactionByScheme(groupMemberLine, qtyAmountLimit, points, baseCalculationOn, type);
                }
            }

            return totalCollectedPoints;
        }

        private static bool IsValidDate(DateTime validFrom, DateTime validTo)
        {
            DateTime emptyDate = new DateTime(1900, 1, 1);
            bool isValidDate = ((validFrom <= DateTime.Today) && (DateTime.Today <= validTo)
                    || ((validFrom <= DateTime.Today) && (validTo == emptyDate))
                    || ((validFrom == emptyDate) && (validTo == emptyDate)));
            return isValidDate;
        }

        private static decimal CalculatePointsForTender(ref bool tenderRuleFound, string tenderTypeID, decimal amount, DataTable loyaltyPointsTable)
        {
            tenderRuleFound = false;
            foreach (DataRow row in loyaltyPointsTable.Rows)
            {
                LoyaltyPointTypeBase type = (LoyaltyPointTypeBase)row["PRODUCTTENDERTYPE"];
                if (type == LoyaltyPointTypeBase.Tender)
                {
                    string loyaltytenderTypeId = row["RETAILTENDERTYPEID"].ToString();
                    decimal qtyAmountLimit = Convert.ToDecimal(row["QTYAMOUNTLIMIT"].ToString());
                    decimal points = Convert.ToDecimal(row["POINTS"]);
                    CalculationTypeBase baseCalculationOn = (CalculationTypeBase)row["BASECALCULATIONON"];

                    if (IsValidDate(Convert.ToDateTime(row["VALIDFROM"]), Convert.ToDateTime(row["VALIDTO"])))
                    {
                        if (tenderTypeID == loyaltytenderTypeId)
                        {
                            tenderRuleFound = true;
                            if ((qtyAmountLimit > 0) && (baseCalculationOn == CalculationTypeBase.Amounts))
                            {
                                decimal companyCurrencyAmount = Loyalty.InternalApplication.Services.Currency.CurrencyToCurrency(
                                        ApplicationSettings.Terminal.StoreCurrency,
                                        ApplicationSettings.Terminal.CompanyCurrency,
                                        amount);

                                // If the company currency amount sign is not held constant, we give back less points than we charge.
                                int sign = Math.Sign(companyCurrencyAmount);
                                return Math.Floor(Math.Abs(companyCurrencyAmount) / qtyAmountLimit * points) * (decimal)sign;
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private void NewMessageWindow(int textID, LSPosMessageTypeButton buttonType, System.Windows.Forms.MessageBoxIcon icon)
        {
            CloseExistingMessageWindow();
            popupDialog = new frmMessage(textID, buttonType, icon);
            this.Application.ApplicationFramework.POSShowFormModeless(popupDialog);
        }

        private void CloseExistingMessageWindow()
        {
            if (popupDialog != null)
            {
                popupDialog.Close();
                popupDialog.Dispose();
            }
        }

        private void UpdateTransactionWithNewCustomer(string customerID, bool restoreItemPrices)
        {
            if (string.IsNullOrEmpty(customerID))
            {
                NetTracer.Warning("Loyalty::UpdateTransactionWithNewCustomer: customerID is null");
                return;
            }

            Contracts.DataEntity.ICustomer customer = this.CustomerSystem.GetCustomerInfo(customerID);

            if (this.transaction.Customer == null || this.transaction.Customer.CustomerId != customerID)
            {
                this.CustomerSystem.SetCustomer(this.transaction, customer, customer);
            }

            IItemSystem itemSystem = this.Application.BusinessLogic.ItemSystem;

            itemSystem.RecalcPriceTaxDiscount(transaction, restoreItemPrices);

            // Calc total.
            this.transaction.CalcTotals();
        }

        private void UpdateIssuedLoyaltyPoints(LoyaltyItem loyaltyItem)
        {
            bool valid = false;
            string comment = string.Empty;
            try
            {
                /*try //commented on 28-03-2019 for PNG --   this work is doing through batch job in ax
                {
                    NewMessageWindow(50054, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    this.Application.TransactionServices.UpdateIssuedLoyaltyPoints(
                        ref valid,
                        ref comment,
                        this.transaction.TransactionId,
                        "1",
                        this.transaction.StoreId,
                        this.transaction.TerminalId,
                        loyaltyItem.LoyaltyCardNumber,
                        ((IPosTransactionV1)this.transaction).BeginDateTime,
                        loyaltyItem.CalculatedLoyaltyPoints,
                        this.transaction.ReceiptId,
                        this.transaction.OperatorId);

                    if (valid)
                    {
                        UpdateTransactionAccumulatedLoyaltyPoint();
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }*/

                CloseExistingMessageWindow();
            }
            catch (Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::UpdateIssuedLoyaltyPoints failed for loyaltyItem {0}. Setting valid to false.", loyaltyItem.LoyaltyCardNumber);
                valid = false;
            }

            if (!valid)
            {
                frmMessage errDialog = null;
                try
                {
                    //if (string.IsNullOrEmpty(comment))
                    //{
                    //    errDialog = new frmMessage(50058, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    //else
                    //{
                    //    errDialog = new frmMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}

                    //this.Application.ApplicationFramework.POSShowForm(errDialog);
                    CloseExistingMessageWindow();
                }
                finally
                {
                    if (errDialog != null)
                    {
                        errDialog.Dispose();
                    }
                }
            }
        }

        private void UpdateUsedLoyaltyPoints(ILoyaltyTenderLineItem loyaltyTenderItem = null)
        {
            bool valid = false;
            string comment = string.Empty;

            string cardNumber = string.Empty;
            decimal loyaltyPoints = decimal.Zero;
            if (loyaltyTenderItem != null)
            {
                cardNumber = loyaltyTenderItem.CardNumber;
                loyaltyPoints = loyaltyTenderItem.LoyaltyPoints;
            }
            else
            {
                cardNumber = transaction.LoyaltyItem.LoyaltyCardNumber;
                loyaltyPoints = transaction.LoyaltyItem.CalculatedLoyaltyPoints;
            }

            try
            {
                try
                {
                    NewMessageWindow(50053, LSPosMessageTypeButton.NoButtons, System.Windows.Forms.MessageBoxIcon.Information);

                    this.Application.TransactionServices.UpdateUsedLoyaltyPoints(
                        ref valid,
                        ref comment,
                        this.transaction.TransactionId,
                        "1",
                        this.transaction.StoreId,
                        this.transaction.TerminalId,
                        cardNumber,
                        ((IPosTransactionV1)this.transaction).BeginDateTime,
                        loyaltyPoints,
                        this.transaction.ReceiptId,
                        this.transaction.OperatorId);

                    if (valid)
                    {
                        UpdateTransactionAccumulatedLoyaltyPoint();
                    }
                }
                finally
                {
                    CloseExistingMessageWindow();
                }

                CloseExistingMessageWindow();
            }
            catch (Exception ex)
            {
                NetTracer.Warning(ex, "Loyalty::UpdateUsedLoyaltyPoints failed for loyaltyTenderItem {0}. Setting valid to false.", cardNumber);
                valid = false;
            }

            if (!valid)
            {
                frmMessage errDialog = null;
                try
                {
                    if (string.IsNullOrEmpty(comment))
                    {
                        errDialog = new frmMessage(50058, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        errDialog = new frmMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.Application.ApplicationFramework.POSShowForm(errDialog);
                    if (loyaltyTenderItem != null)
                    {
                        loyaltyTenderItem.LoyaltyPoints = 0;
                    }
                    CloseExistingMessageWindow();
                }
                finally
                {
                    if (errDialog != null)
                    {
                        errDialog.Dispose();
                    }
                }
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

        private string GetLOYALTYCARDNoByCustAcc(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select top 1 B.CARDNUMBER from RETAILLOYALTYCUSTTABLE A ");
            commandText.Append(" left join RETAILLOYALTYMSRCARDTABLE B on A.LOYALTYCUSTID=B.LOYALTYCUSTID");
            commandText.Append(" where ACCOUNTNUM ='" + sCustAcc + "'");

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
                return "";
        }

        private string getCustomerMobilePrimary(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select RETAILMOBILEPRIMARY from CUSTTABLE A ");
            commandText.Append(" where ACCOUNTNUM ='" + sCustAcc + "'");

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
                return "";
        }

        private decimal getLoayltyRedeemableAmt(string sLCardNo, decimal dCurrentPoins)
        {
            decimal dResult = 0;
            decimal dPoints = 0;
            decimal dQtyAmt = 0;
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT abs(POINTS),QTYAMOUNTLIMIT FROM RETAILLOYALTYPOINTSTABLE A ");
            commandText.Append(" left join RETAILLOYALTYMSRCARDTABLE B on a.LOYALTYSCHEMEID=b.LOYALTYSCHEMEID");
            commandText.Append(" where PRODUCTTENDERTYPE=1 and b.CARDNUMBER = '" + sLCardNo + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dPoints = Convert.ToDecimal(reader.GetValue(0));
                    dQtyAmt = Convert.ToDecimal(reader.GetValue(1));
                }
            }
            command.CommandTimeout = 0;
            reader.Dispose();

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (dPoints > 0 && dQtyAmt > 0)
            {
                dResult = (dCurrentPoins / dPoints) * dQtyAmt;
            }

            return decimal.Round(dResult, 2, MidpointRounding.AwayFromZero);
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public void SendSMS(string sTextSMS, string sMNo)
        {

            string _webURL = "";// "http://bulksmspune.mobi/sendurlcomma.asp?user=";
            string _userId = "";//"20064043";
            string _password = "";// "smst2018";

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(PASSWORD,''),isnull(USERID,''),isnull(WEBURL,'') FROM CRWSMSSETUP ");
            commandText.Append(" where ISACTIVE=1 AND DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command1 = new SqlCommand(commandText.ToString(), conn);
            SqlDataReader reader1 = command1.ExecuteReader();

            if (reader1.HasRows)
            {
                while (reader1.Read())
                {
                    _password = Convert.ToString(reader1.GetValue(0));
                    _userId = Convert.ToString(reader1.GetValue(1));
                    _webURL = Convert.ToString(reader1.GetValue(2));
                }
            }
            command1.CommandTimeout = 0;
            reader1.Dispose();

            // http://bulksmspune.mobi/sendurlcomma.asp?user=20064043&pwd=smst2018&smstype=3&mobileno=9850991010&msgtext=Hello
            string sSMSAPI = "";

            if (!string.IsNullOrEmpty(_password) && !string.IsNullOrEmpty(_userId))
                sSMSAPI = _webURL + _userId + "&pwd=" + _password + "&smstype=3&mobileno=" + sMNo + "&msgtext=" + sTextSMS + "";
            else
                sSMSAPI = _webURL + "&message=" + sTextSMS + "&to=" + sMNo + "&msgtext=" + sTextSMS + "&sender=PNGJEW";//=PNGJEW

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(sSMSAPI);
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();

            respStreamReader.Close();
            myResp.Close();

        }

        private void SaveLoyaltyOTPInfo(IPosTransaction POSTrans, string sOTP, string sCustId)
        {
            string commandText = " INSERT INTO [CRWOTPTransTable] " +
                                   " ([TransactionId],[TransType],[TransactionOTP]," +
                                   " [CustAccount],[TransDate],[RetailStaffId],[RetailStoreId] " +
                                   " ,RetailTerminalId,[DATAAREAID],[OTPUsed],[OTPCanceled]) " +
                                   "  VALUES " +
                                   " (@TransactionId,@TransType,@TransactionOTP,@CustAccount," +
                                   "  @TransDate,@RetailStaffId,@RetailStoreId,@RetailTerminalId,@DATAAREAID " +
                                   " ,@OTPUsed,@OTPCanceled) ";

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                command.Parameters.Add("@TransactionId", SqlDbType.NVarChar, 20).Value = Convert.ToString(POSTrans.TransactionId);
                command.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)OTPTransactionType.Loaylty;
                command.Parameters.Add("@TransactionOTP", SqlDbType.NVarChar, 20).Value = sOTP;
                command.Parameters.Add("@CustAccount", SqlDbType.NVarChar, 20).Value = sCustId;
                command.Parameters.Add("@TransDate", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                command.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 20).Value = Convert.ToString(ApplicationSettings.Terminal.TerminalOperator.OperatorId);
                command.Parameters.Add("@RetailStoreId", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RetailTerminalId", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@OTPUsed", SqlDbType.Int).Value = 0;
                command.Parameters.Add("@OTPCanceled", SqlDbType.Int).Value = 0;

                command.ExecuteNonQuery();
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }


        private void UpdateTempTableCustLoyaltyNo(string sLNo)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE RETAILTEMPTABLE SET SELECTEDCUSTLOYALTYNO='" + sLNo + "' WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Clear();

            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private decimal getMakingInfo()
        {
            string sTblName = "MAKINGINFO" + ApplicationSettings.Terminal.TerminalId;
            decimal dResult = 0m;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')");
            commandText.Append(" BEGIN  Select MakingAmt from " + sTblName + " END");// Where TRANSACTIONID = '" + sTransactionId + "'


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            dResult = Convert.ToDecimal(command.ExecuteScalar());


            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;

        }
    }
}
