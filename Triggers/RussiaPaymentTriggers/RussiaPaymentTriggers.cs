/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Settings.TransactionServicesProfiles;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Commerce.Runtime.TransactionService.Serialization;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.Transaction.MemoryTables;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Localization.Russia.PaymentTriggers
{
    [Export(typeof(IPaymentTrigger))]
    public class PaymentTriggers : IPaymentTrigger, IGlobalization
    {
        #region Globalization
        private readonly ReadOnlyCollection<string> supportedCountryRegions = new ReadOnlyCollection<string>(new string[] { SupportedCountryRegion.RU.ToString() });

        /// <summary>
        /// Defines ISO country region codes this functionality is applicable for.
        /// </summary>
        public ReadOnlyCollection<string> SupportedCountryRegions
        {
            get { return supportedCountryRegions; }
        }
        #endregion

        #region Private declarations
        /// <summary>
        /// Holds disbursement slip ID between PrePayment trigger and OnPayment trigger calls.
        /// </summary>
        private string disbursementSlipNumber;
        /// <summary>
        /// Holds the Cash Tender ID between PrePayment and OnPayment trigger calls.
        /// </summary>
        private string cashTenderId;
        #endregion

        #region Properties
        /// <summary>
        /// Application instance.
        /// </summary>
        [Import]
        public IApplication Application { get; set; }
        #endregion

        #region Triggers
        // TODO: add XML-Doc to all public methods.
        public void PrePayCustomerAccount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, decimal amount)
        {
            // Left empty on purpose.
        }

        public void PrePayCardAuthorization(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, Pos.Contracts.DataEntity.ICardInfo cardInfo, decimal amount)
        {
            if (preTriggerResult == null)
                throw new ArgumentNullException("preTriggerResult");

            if (posTransaction == null)
                throw new ArgumentNullException("posTransaction");

            if (cardInfo == null)
                throw new ArgumentNullException("cardInfo");

            PerformRefundAmountValidation(preTriggerResult, posTransaction, cardInfo.TenderTypeId, amount, cardInfo.CardType == Pos.Contracts.Services.CardTypes.LoyaltyCard);
        }

        public void OnPayment(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("RussianPaymentTriggers.OnPayment", "On the addition of a tender...", LSRetailPosis.LogTraceLevel.Trace);

            SetDisbursememtSlipNumber(posTransaction);
        }

        public void PrePayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId)
        {
            LSRetailPosis.ApplicationLog.Log("RussianPaymentTriggers.PrePayment", "On the start of a payment operation...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreVoidPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PostVoidPayment(IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PreRegisterPayment(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, object posOperation, string tenderId, string currencyCode, decimal amount)
        {
            LSRetailPosis.ApplicationLog.Log("RussianPaymentTriggers.PreRegisterPayment", "Before registering a payment...", LogTraceLevel.Trace);

            PerformReturnShiftValidation(preTriggerResult, posTransaction, tenderId, currencyCode, amount);

            PerformRefundAmountValidation(preTriggerResult, posTransaction, tenderId, amount);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Prompts input dialog window to the user requesting disbursement slip number to be entered.
        /// </summary>
        /// <param name="slipNumber">An output parameter for transferring of the disbursement slip number entered by the user.</param>
        /// <returns>true if the the user entered a valid slip number and pressed OK; false otherwise.</returns>
        private bool RequestDisbursementSlipNumber(out string slipNumber)
        {
            slipNumber = null;

            var inputConfirmation = new InputConfirmation()
            {
                PromptText = ApplicationLocalizer.Language.Translate(Resources.PromptText)
            };

            string enteredText = null;

            var request = new InteractionRequestedEventArgs(inputConfirmation, () =>
            {
                if (inputConfirmation.Confirmed)
                {
                    enteredText = inputConfirmation.EnteredText;
                }
            }
            );

            Application.Services.Interaction.InteractionRequest(request);

            if (inputConfirmation.Confirmed)
            {
                slipNumber = enteredText;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates the correctness of the <paramref name="slipNumber"/>.
        /// </summary>
        /// <param name="slipNumber">The disbursement slip number to validate.</param>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <param name="tenderId">The tender ID.</param>
        /// <param name="currencyCode">Payment currency code.</param>
        /// <param name="amount">Payment amount.</param>
        /// <param name="transactionServiceIsAvailable">An output parameter for determining whether Transaction Service was available.</param>
        /// <returns>true if disbursement slip number is valid; false otherwise.</returns>
        /// <remarks>
        /// Validates that <paramref name="slipNumber"/> is not empty or whitespace.
        /// Validates that <paramref name="slipNumber"/> is not used in local retail store database.
        /// Validates that <paramref name="slipNumber"/> is not used in AX database if transaction service connection is available.
        /// </remarks>
        private bool ValidateSlipNumber(string slipNumber, RetailTransaction retailTransaction, string currencyCode, decimal amount, out bool? transactionServiceIsAvailable)
        {
            return ValidateSlipNumber(slipNumber, retailTransaction, currencyCode, amount, out transactionServiceIsAvailable, true);
        }

        /// <summary>
        /// Validates the correctness of the <paramref name="slipNumber"/>.
        /// </summary>
        /// <param name="slipNumber">The disbursement slip number to validate.</param>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <param name="tenderId">The tender ID.</param>
        /// <param name="currencyCode">Payment currency code.</param>
        /// <param name="amount">Payment amount.</param>
        /// <param name="transactionServiceIsAvailable">An output parameter for determining whether Transaction Service was available.</param>
        /// <param name="validateViaTS">A Boolean determining whether to validate slip number via Transaction Services.</param>
        /// <returns>true is disbursement slip number is valid; false otherwise.</returns>
        /// <remarks>
        /// Validates that <paramref name="slipNumber"/> is not empty or whitespace.
        /// Validates that <paramref name="slipNumber"/> is not used in local retail store database.
        /// Optionally validates that <paramref name="slipNumber"/> is not used in AX database if transaction service connection is available.
        /// </remarks>
        private bool ValidateSlipNumber(string slipNumber, RetailTransaction retailTransaction, string currencyCode, decimal amount, out bool? transactionServiceIsAvailable, bool validateViaTS)
        {
            transactionServiceIsAvailable = null;

            if (string.IsNullOrWhiteSpace(slipNumber))
            {
                // As we can not make the dialog field mandatory and thus verify it is filled before closing the dialog, we show this Warning message here to the user.
                Application.Services.Dialog.ShowMessage(Resources.PromptText, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return false;
            }

            if (DoesSlipExistInRetailTransaction(slipNumber, retailTransaction) || DoesSlipExistInPosDb(slipNumber))
            {
                Application.Services.Dialog.ShowMessage(
                    Resources.DisbursementSlipUsedInTheReturn,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return false;
            }

            if (validateViaTS && TransactionServiceProfile.UseTransactionServices)
            {
                try
                {
                    transactionServiceIsAvailable = Application.TransactionServices.CheckConnection();
                }
                catch
                {
                    transactionServiceIsAvailable = false;
                }

                if (!transactionServiceIsAvailable.HasValue || !transactionServiceIsAvailable.Value)
                {
                    DialogResult dialogResult = Application.Services.Dialog.ShowMessage(
                        Resources.TransactionServiceIsNotAvailable,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    return dialogResult == DialogResult.Yes;
                }

                bool operationSucceeded;
                string errorMessage;
                bool? disbursementSlipExist;

                CashDisbursementSlipExist_RU(
                    ApplicationSettings.Terminal.StoreId, slipNumber, amount, currencyCode, DateTime.Today, retailTransaction.Customer.CustomerId ?? ApplicationSettings.Terminal.DefaultCustomer, out operationSucceeded, out errorMessage, out disbursementSlipExist);

                if (!operationSucceeded)
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        Application.Services.Dialog.ShowMessage(errorMessage, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                if (!disbursementSlipExist.HasValue)
                    throw new InvalidOperationException("Disbursement slip existence in undetermined.");

                if (!disbursementSlipExist.Value)
                {
                    Application.Services.Dialog.ShowMessage(Resources.DisbursementSlipIsInvalid, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                bool? paymentExistInAX;
                CashDisbursementSlipIsUsedInPayment_RU(slipNumber, ApplicationSettings.Terminal.StoreId, out operationSucceeded, out errorMessage, out paymentExistInAX);

                if (!operationSucceeded)
                {
                    if (!string.IsNullOrWhiteSpace(errorMessage))
                        Application.Services.Dialog.ShowMessage(errorMessage, MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return false;
                }

                if (paymentExistInAX.HasValue && paymentExistInAX.Value)
                {
                    Application.Services.Dialog.ShowMessage(
                        Resources.DisbursementSlipUsedInTheReturn,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines whether slip exists in the local POS Database.
        /// </summary>
        /// <param name="slipNumber">Slip number</param>
        /// <returns>true if the slip exists; false otherwise.</returns>
        private static bool DoesSlipExistInPosDb(string slipNumber)
        {
            LSRetailPosis.DataAccess.TransactionTenderData transactionTenderData = new LSRetailPosis.DataAccess.TransactionTenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            bool paymentExist = transactionTenderData.PaymentWithCashDocIdExist(slipNumber, ApplicationSettings.Terminal.StoreId);
            return paymentExist;
        }

        /// <summary>
        /// Determines whether slip number exists in the Retail transaction.
        /// </summary>
        /// <param name="slipNumber">Slip number</param>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <returns>true if slip exists; false otherwise.</returns>
        private static bool DoesSlipExistInRetailTransaction(string slipNumber, RetailTransaction retailTransaction)
        { 
            return (from tl in retailTransaction.TenderLines where !tl.Voided && string.Equals(tl.CashDocId_RU, slipNumber, StringComparison.OrdinalIgnoreCase) select tl).Any();
        }

        /// <summary>
        /// Performs shift validation for return transactions.
        /// </summary>
        /// <param name="preTriggerResult">Pre trigger result.</param>
        /// <param name="posTransaction">POS transaction.</param>
        /// <param name="posOperation">POS operation boxed into object.</param>
        /// <param name="tenderId">Tender type ID.</param>
        /// <param name="currencyCode">Payment currency code.</param>
        /// <param name="amount">Payment amount.</param>
        private void PerformReturnShiftValidation(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, string tenderId, string currencyCode, decimal amount)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;

            if (retailTransaction == null)
                return;

            if (!retailTransaction.SaleIsReturnSale)
                return;

            // Verify that it is a true Cash tender type by comparing it to the tender type set up for the Pay Cash operation for the current store.
            string storeCashTenderId = new TenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID).GetTenderID(ApplicationSettings.Terminal.StoreId, ((int)PosisOperations.PayCash).ToString());
            if (!string.Equals(tenderId, storeCashTenderId, StringComparison.OrdinalIgnoreCase))
                return;

            IRetailTransaction originalTransaction = new TransactionData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, Application).GetOriginalRetailTransaction(retailTransaction);

            bool? hasSameShiftId =
                Application.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled() ?
                Application.Services.Peripherals.FiscalPrinter.HasSameShiftId(originalTransaction) :
                null;

            if (((hasSameShiftId.HasValue && !hasSameShiftId.Value) || (originalTransaction.Shift != null && originalTransaction.Shift.BatchId != Application.Shift.BatchId))
                && !LSRetailPosis.Settings.ApplicationSettings.Terminal.ProcessReturnsAsInOriginalSaleShift_RU)
            {
                string slipNumber;
                bool slipNumberObtained = false;
                bool createDisbursementSlipAutimatically = false;

                if (IsTransactionServiceAvailable())
                {
                    createDisbursementSlipAutimatically = Application.Services.Dialog.ShowMessage(106038, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                }

                if (createDisbursementSlipAutimatically)
                {
                    slipNumberObtained = CreateDisbursementSlip(preTriggerResult, currencyCode, amount, retailTransaction, out slipNumber);
                }
                else
                {
                    slipNumberObtained = ObtainDisbursementSlipFromUser(preTriggerResult, currencyCode, amount, retailTransaction, out slipNumber);
                }

                if (slipNumberObtained && !string.IsNullOrWhiteSpace(slipNumber))
                {
                    // Save the entered slip number into the class member variable for later usage in the OnPayment trigger.
                    disbursementSlipNumber = slipNumber;
                    cashTenderId = tenderId;
                }
            }
        }

        /// <summary>
        /// Obtains cash disbursement slip nunber from the user.
        /// </summary>
        /// <param name="preTriggerResult">Pre trigger result</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="amount">Amount</param>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <param name="slipNumber">An output parameters for returning the entered cash disbursement slip number.</param>
        /// <returns>true if the slip number was obtained from teh user; false otherwise.</returns>
        private bool ObtainDisbursementSlipFromUser(IPreTriggerResult preTriggerResult, string currencyCode, decimal amount, RetailTransaction retailTransaction, out string slipNumber)
        {
            bool slipNumberValidatedOk = false;
            bool transactionServiceIsUnavailable = false;
            bool operationCancelled = false;
            bool requestSlipNumber;

            do
            {
                if (RequestDisbursementSlipNumber(out slipNumber))
                {
                    bool? transactionServiceIsAvailable;
                    slipNumberValidatedOk = ValidateSlipNumber(slipNumber, retailTransaction, currencyCode, amount, out transactionServiceIsAvailable);
                    transactionServiceIsUnavailable = transactionServiceIsAvailable.HasValue && !transactionServiceIsAvailable.Value;
                }
                else
                {
                    operationCancelled = true;
                }

                requestSlipNumber = !operationCancelled && !slipNumberValidatedOk && !transactionServiceIsUnavailable;
            } while (requestSlipNumber);

            if (operationCancelled || !slipNumberValidatedOk)
            {
                preTriggerResult.ContinueOperation = false;
                return false;
            }

            return true;
        }

        /// <summary>
        /// Creates cash disbursement slip via TS.
        /// </summary>
        /// <param name="preTriggerResult">Pre trigger result</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="amount">Amount</param>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <param name="slipNumber">An output parameter for returning the created cash disbursement slip ID.</param>
        /// <returns>true if creation succeeded; false otherwise.</returns>
        private bool CreateDisbursementSlip(IPreTriggerResult preTriggerResult, string currencyCode, decimal amount, RetailTransaction retailTransaction, out string slipNumber)
        {
            DisbursementSlipCreationConfirmation confirmation = new DisbursementSlipCreationConfirmation();
            IDisbursementSlipInfo disbursementSlipInfo = null;
            bool confirmed = false;
            InteractionRequestedEventArgs interactionRequestEventArgs = new InteractionRequestedEventArgs(confirmation, () =>
            {
                if (confirmation.Confirmed)
                {
                    confirmed = true;
                    disbursementSlipInfo = (IDisbursementSlipInfo)confirmation.DisbursementSlipInfo;
                }
            });

            Application.Services.Interaction.InteractionRequest(interactionRequestEventArgs);

            if (!confirmed)
            {
                preTriggerResult.ContinueOperation = false;
                slipNumber = null;
                return false;
            }

            bool retValue;
            string comment;
            DateTime axDateNull = new DateTime(1900, 1, 1);

            CreateCashDisbursementSlip_RU(
                retailTransaction.StoreId,
                currencyCode,
                amount,
                DateTime.Today,
                retailTransaction.Customer.CustomerId ?? ApplicationSettings.Terminal.DefaultCustomer,
                disbursementSlipInfo.ReasonOfReturn,
                disbursementSlipInfo.PersonName,
                disbursementSlipInfo.CardInfo,
                disbursementSlipInfo.DocumentNum,
                !string.IsNullOrWhiteSpace(disbursementSlipInfo.DocumentNum) ? disbursementSlipInfo.DocumentDate : axDateNull,
                out retValue,
                out comment,
                out slipNumber);

            if (!retValue)
            {
                if (!string.IsNullOrEmpty(comment))
                {
                    Application.Services.Dialog.ShowMessage(comment, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LSRetailPosis.ApplicationLog.Log("RussiaPaymentTriggers.CreateCashDisbursementSlip_RU", comment, LogTraceLevel.Error);
                }

                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 106035;
                return false;
            }

            if (string.IsNullOrWhiteSpace(slipNumber))
                throw new InvalidOperationException("SLip number returned from TS is null or whitespace");

            if (DoesSlipExistInPosDb(slipNumber))
            {
                Application.Services.Dialog.ShowMessage(106036, MessageBoxButtons.OK, MessageBoxIcon.Error);
                preTriggerResult.ContinueOperation = false;
                return false;
            }

            Application.Services.Dialog.ShowMessage(ApplicationLocalizer.Language.Translate(106037, slipNumber), MessageBoxButtons.OK, MessageBoxIcon.Information);

            return true;
        }

        /// <summary>
        /// Sets the captured disbursememt slip number.
        /// </summary>
        /// <param name="posTransaction">POS transaction.</param>
        private void SetDisbursememtSlipNumber(IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            if (retailTransaction == null)
                return;

            if (!retailTransaction.SaleIsReturnSale || string.IsNullOrWhiteSpace(disbursementSlipNumber) || string.IsNullOrWhiteSpace(cashTenderId))
                return;

            var cashReturnTenderLineWithNoCashDocId = (from tl in retailTransaction.TenderLines where tl.TenderTypeId == cashTenderId && string.IsNullOrEmpty(tl.CashDocId_RU) && !tl.Voided select tl).FirstOrDefault();
            if (cashReturnTenderLineWithNoCashDocId != null)
            {
                cashReturnTenderLineWithNoCashDocId.CashDocId_RU = disbursementSlipNumber;
            }
            else
            {
                throw new InvalidOperationException("Cash return tender line for disbursement slip not found");
            }

            // Clear member variables as they make sense only for single subsequent call of OnPayment trigger.
            disbursementSlipNumber = null;
            cashTenderId = null;
        }

        /// <summary>
        /// Performs refund amount validation.
        /// </summary>
        /// <param name="preTriggerResult">Pre trigger result</param>
        /// <param name="posTransaction">POS transaction</param>
        /// <param name="tenderId">Tender type ID</param>
        /// <param name="amount">Amount</param>
        /// <param name="isLoyaltyPayment">Determines whether we deal with a loyalty card payment; optional</param>
        private void PerformRefundAmountValidation(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, string tenderId, decimal amount, bool isLoyaltyPayment = false)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            if (retailTransaction == null || !retailTransaction.SaleIsReturnSale)
                return;

            if (!ApplicationSettings.Terminal.TerminalOperator.AllowDifferentPaymentMethodRefunds_RU)
            {
                RetailTransaction originalTransaction = new TransactionData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, Application).GetOriginalRetailTransaction(retailTransaction);
                if (originalTransaction == null)
                    throw new InvalidOperationException("Original transaction is not found.");

                decimal amountAvailableForReturn = new TransactionTenderData(ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID).GetTransactionRefundRemainderTenderTypeAmount(originalTransaction, tenderId, isLoyaltyPayment);

                // Consider current transaction refund
                amountAvailableForReturn += retailTransaction.CalculateTenderTypePaymentAmount(tenderId, isLoyaltyPayment);

                if (amountAvailableForReturn < decimal.Zero)
                    amountAvailableForReturn = decimal.Zero;

                if (Math.Abs(amount) > Math.Abs(amountAvailableForReturn))
                { 
                    Application.Services.Dialog.ShowMessage(
                        Resources.RefundAmountCannotBeGreaterThanAvailableForTenderType,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    preTriggerResult.ContinueOperation = false;
                }
            }
        }

        /// <summary>
        /// Calls the Transaction Service method CashDisbursementSlipExist_RU that checks whether disbursement slip exists.
        /// </summary>
        /// <param name="storeId">Retail store ID</param>
        /// <param name="cashDocId">Disbursement slip ID</param>
        /// <param name="amountCur">Disbursememt slip amount</param>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="transDate">Transaction date</param>
        /// <param name="retValue">An output parameter determining the status of the TS method call: success or failure.</param>
        /// <param name="comment">An output parameter determining the application error message if the method call failed.</param>
        /// <param name="slipExist">An output parameter determining whether cash disbursement slip exists.</param>
        /// <exception cref="TransactionServiceParseResultsException">
        /// Throws <see cref="TransactionServiceParseResultsException"/> if Transaction Service returns null response or the response has insufficient length.
        /// </exception>
        private void CashDisbursementSlipExist_RU(string storeId, string cashDocId, decimal amountCur, string currencyCode, DateTime transDate, string customer, out bool retValue, out string comment, out bool? slipExist)
        {
            const string methodName = "CashDisbursementSlipExist_RU";
            const int dateSequence = 321;
            const int successIndex = 1; // index of the success/fail result.
            const int commentIndex = 2; // index of the error message or comment.
            const int payloadIndex = 3; // index of the content/payload.

            ReadOnlyCollection<object> containerArray = null;

            containerArray =
                Application.TransactionServices.Invoke(
                    methodName,
                    new object[]
                    {
                        storeId,
                        cashDocId,
                        amountCur,
                        currencyCode,
                        SerializationHelper.ConvertDateTimeToAXDateString(transDate, dateSequence),
                        customer
                    });

            if (containerArray == null)
            {
                throw new TransactionServiceParseResultsException(string.Format("Transaction Service call to the method {0} returned empty response.", methodName));
            }

            if (containerArray.Count < payloadIndex)
            {
                throw new TransactionServiceParseResultsException(string.Format("Transaction Service response to the method {0} call has insufficient length.", methodName));
            }

            retValue = (bool)containerArray[successIndex];
            comment = (string)containerArray[commentIndex];
            slipExist = null;

            if (retValue)
                slipExist = (bool)containerArray[payloadIndex];
        }

        /// <summary>
        /// Calls the Transaction Service method CashDisbursementSlipIsUsedInPayment_RU that determines whether the specified disbursement slip is used in a payment.
        /// </summary>
        /// <param name="cashDocId">Disbursement slip ID</param>
        /// <param name="storeId">Retail store ID</param>
        /// <param name="retValue">An output parameter determining the status of the TS method call: success or failure.</param>
        /// <param name="comment">An output parameter determining the application error message if the method call failed.</param>
        /// <param name="usedInPayment">An output parameter determining whether the specified disbursement slip is used in a payment.</param>
        /// <exception cref="TransactionServiceParseResultsException">
        /// Throws <see cref="TransactionServiceParseResultsException"/> if Transaction Service returns null response or the response has insufficient length.
        /// </exception>
        private void CashDisbursementSlipIsUsedInPayment_RU(string cashDocId, string storeId, out bool retValue, out string comment, out bool? usedInPayment)
        {
            const string methodName = "CashDisbursementSlipIsUsedInPayment_RU";
            const int successIndex = 1; // index of the success/fail result.
            const int commentIndex = 2; // index of the error message or comment.
            const int payloadIndex = 3; // index of the content/payload.

            ReadOnlyCollection<object> containerArray = null;

            containerArray =
                Application.TransactionServices.Invoke(
                    methodName,
                    new object[]
                    {
                        storeId,
                        cashDocId
                    });

            if (containerArray == null)
            {
                throw new TransactionServiceParseResultsException(string.Format("Transaction Service call to the method {0} returned empty response.", methodName));
            }

            if (containerArray.Count < payloadIndex)
            {
                throw new TransactionServiceParseResultsException(string.Format("Transaction Service response to the method {0} call has insufficient length.", methodName));
            }

            retValue = (bool)containerArray[successIndex];
            comment = (string)containerArray[commentIndex];
            usedInPayment = null;

            if (retValue)
                usedInPayment = (bool)containerArray[payloadIndex];
        }

        /// <summary>
        /// Calls the Transaction Service method CreateCashDisbursementSlip_RU.
        /// </summary>
        /// <param name="storeId">Retail store ID</param>
        /// <param name="currency">Currency code</param>
        /// <param name="amount">Amount in currency</param>
        /// <param name="transDate">Transaction date</param>
        /// <param name="custAccount">Customer account code</param>
        /// <param name="reasonOfReturn">Reason of cash return</param>
        /// <param name="personName">Person name</param>
        /// <param name="cardInfo">Card info</param>
        /// <param name="documentNum">Document number</param>
        /// <param name="documentDate">Document date</param>
        /// <param name="retValue">An output parameter determining the status of the TS method call: success or failure.</param>
        /// <param name="comment">An output parameter determining the application error message if the method call failed.</param>
        /// <param name="disbursementSlipId">An output parameter for returning the number of the created disbursement slip ID.</param>
        /// <exception cref="TransactionServiceParseResultsException">
        /// Throws <see cref="TransactionServiceParseResultsException"/> if Transaction Service returns null response or the response has insufficient length.
        /// </exception>
        private void CreateCashDisbursementSlip_RU(
            string storeId, string currency, decimal amount, DateTime transDate, string custAccount, string reasonOfReturn, string personName, string cardInfo,
            string documentNum, DateTime documentDate, out bool retValue, out string comment, out string disbursementSlipId)
        {
            const string methodName = "CreateCashDisbursementSlip_RU";
            const int dateSequence = 321;
            const int successIndex = 1; // index of the success/fail result.
            const int commentIndex = 2; // index of the error message or comment.
            const int payloadIndex = 3; // index of the content/payload.

            ReadOnlyCollection<object> containerArray = null;

            containerArray =
                Application.TransactionServices.Invoke(
                    methodName,
                    new object[]
                    {
                        storeId,
                        currency,
                        amount,
                        SerializationHelper.ConvertDateTimeToAXDateString(transDate, dateSequence),
                        custAccount,
                        reasonOfReturn,
                        personName,
                        cardInfo,
                        documentNum,
                        SerializationHelper.ConvertDateTimeToAXDateString(documentDate, dateSequence)
                    });

            if (containerArray == null)
            {
                throw new TransactionServiceParseResultsException(string.Format("Transaction Service call to the method {0} returned empty response.", methodName));
            }

            if (containerArray.Count < payloadIndex)
            {
                throw new TransactionServiceParseResultsException(string.Format("Transaction Service response to the method {0} call has insufficient length.", methodName));
            }

            retValue = (bool)containerArray[successIndex];
            comment = (string)containerArray[commentIndex];

            if (retValue)
            {
                disbursementSlipId = (string)containerArray[payloadIndex];
            }
            else
            {
                disbursementSlipId = null;
            }
        }

        /// <summary>
        /// Determines whether Transaction Service is currently available.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private bool IsTransactionServiceAvailable()
        {
            bool available = false;
            if (TransactionServiceProfile.UseTransactionServices)
            {
                try
                {
                    available = Application.TransactionServices.CheckConnection();
                }
                catch
                {
                    available = false;
                }
            }

            return available;
        }
        #endregion

        #region Nested types
        /// <summary>
        /// Class holding local resources.
        /// </summary>
        private static class Resources
        {
            public const int PromptText = 106010;
            public const int DisbursementSlipUsedInTheReturn = 106011;
            public const int DisbursementSlipIsInvalid = 106012;
            public const int TransactionServiceIsNotAvailable = 106013;
            public const int RefundAmountCannotBeGreaterThanAvailableForTenderType = 106019;
        }
        #endregion
    }
}
