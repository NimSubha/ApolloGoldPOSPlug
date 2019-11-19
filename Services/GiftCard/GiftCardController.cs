/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LSRetailPosis;
using LSRetailPosis.POSProcesses.Common;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.HardwareProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.GiftCard
{
    sealed class GiftCardController
    {
        #region Types

        internal enum ContextType
        {
            GiftCardIssue,
            GiftCardPayment,
            GiftCardAddTo,
            GiftCardBalance
        }

        #endregion

        #region Fields

        private string validatedCardNumber;
        private decimal validatedCardBalance;
        private string validatedCardCurrency;

        private PosTransaction Transaction;
        private Tender tenderInfo;

        private decimal? proposedPaymentAmount;

        #endregion

        #region Properties

        /// <summary>
        /// Get controller context.
        /// </summary>
        public ContextType Context { get; private set; }

        /// <summary>
        /// Get or set GiftCard number.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Get GiftCard amount captured from UI.
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// Get GiftCard balance.
        /// </summary>
        public decimal Balance { get; private set; }

        /// <summary>
        /// Gets the proposed payment amount.
        /// </summary>
        public decimal ProposedPaymentAmount 
        {
            get
            {
                return proposedPaymentAmount != null ? (decimal)proposedPaymentAmount : TransactionAmount;
            }

            private set
            {
                proposedPaymentAmount = value;
            }
        }

        /// <summary>
        /// Get GiftCard currency.
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Get amount of the transaction.
        /// </summary>
        public decimal TransactionAmount { get; private set; }

        /// <summary>
        /// Get maximun transaction amount allowed (up to gift card balance).
        /// </summary>
        public decimal MaxTransactionAmountAllowed
        {
            get { return Math.Min(this.TransactionAmount, this.validatedCardBalance); }
        }

        /// <summary>
        /// Gets the tender info.
        /// </summary>
        internal Tender TenderInfo
        {
            get { return this.tenderInfo; }
        }

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="context">Context of the gift card form</param>
        /// <param name="posTransaction">Transaction object.</param>
        public GiftCardController(ContextType context, PosTransaction posTransaction)
            : this(context, posTransaction, null)
        {
        }

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Context of the gift card form</param>
        /// <param name="posTransaction">Transaction object.</param>
        /// <param name="tenderInfo">Tender information about GC (Required for Payment Context) </param>
        public GiftCardController(ContextType context, PosTransaction posTransaction, Tender tenderInfo)
        {
            this.Context = context;
            this.Transaction = posTransaction;
            this.tenderInfo = tenderInfo;
            this.CardNumber = string.Empty;

            // Get the balance of the transaction.
            IRetailTransaction retailTransaction = Transaction as IRetailTransaction;
            CustomerPaymentTransaction customerPaymentTransaction = Transaction as CustomerPaymentTransaction;

            if (retailTransaction != null)
            {
                TransactionAmount = retailTransaction.TransSalePmtDiff;

                // Adjust amount for return transaction under Russian country context
                if (retailTransaction.SaleIsReturnSale
                    && LSRetailPosis.Settings.FunctionalityProfiles.Functions.CountryRegion == LSRetailPosis.Settings.FunctionalityProfiles.SupportedCountryRegion.RU
                    && ApplicationSettings.Terminal.ProposeRefundPaymentAmount_RU)
                {
                    RetailTransaction returnTransaction = (RetailTransaction)retailTransaction;

                    var originalTransaction =
                        new LSRetailPosis.DataAccess.TransactionData(ApplicationSettings.Database.LocalConnection,
                            ApplicationSettings.Database.DATAAREAID, GiftCard.InternalApplication).GetOriginalRetailTransaction(returnTransaction);

                    if (originalTransaction == null)
                        throw new InvalidOperationException("Original transaction is not found");

                    decimal availableForRefundInTenderType =
                        new LSRetailPosis.DataAccess.TransactionTenderData(ApplicationSettings.Database.LocalConnection,
                            ApplicationSettings.Database.DATAAREAID).GetTransactionRefundRemainderTenderTypeAmount(originalTransaction, tenderInfo.TenderID);

                    // Consider current refund in tender type
                    decimal currentRefundAmountInTenderType = (from tl in returnTransaction.TenderLines
                                                               where tl != null && string.Equals(tl.TenderTypeId.Trim(), tenderInfo.TenderID, StringComparison.OrdinalIgnoreCase) && !tl.Voided
                                                               select tl.Amount).Sum();
                    // Refunds have negative sign and remaining amount in non-negative
                    availableForRefundInTenderType += currentRefundAmountInTenderType;

                    if (availableForRefundInTenderType < decimal.Zero)
                        availableForRefundInTenderType = decimal.Zero;

                    if (availableForRefundInTenderType < Math.Abs(TransactionAmount))
                        ProposedPaymentAmount = availableForRefundInTenderType;
                }
            }
            else if (customerPaymentTransaction != null)
            {
                TransactionAmount = customerPaymentTransaction.TransSalePmtDiff;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the title of the form based on current context.
        /// </summary>
        /// <returns></returns>
        public string GetFormTitle()
        {
            string result = string.Empty;

            switch (Context)
            {
                case ContextType.GiftCardIssue:
                    result = ApplicationLocalizer.Language.Translate(55400);
                    break;

                case ContextType.GiftCardAddTo:
                    result = ApplicationLocalizer.Language.Translate(55407);
                    break;

                case ContextType.GiftCardBalance:
                    result = ApplicationLocalizer.Language.Translate(55408);
                    break;

                case ContextType.GiftCardPayment:
                    result = ApplicationLocalizer.Language.Translate(55409);
                    break;
            }

            return result;
        }

        /// <summary>
        /// Round the amount according to tender rule.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public decimal RoundAmount(decimal source)
        {
            return GiftCard.InternalApplication.Services.Rounding.RoundAmount(source, ApplicationSettings.Terminal.StoreId, tenderInfo.TenderID);
        }

        /// <summary>
        /// Validate and lock a gift card.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>Balance on the card as string</returns>
        /// <exception cref="GiftCardException"></exception>
        public string ValidateGiftCardPayment(string cardNumber)
        {
            if (!ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments || !(Transaction is RetailTransaction))
            {
                AssertNotAlreadyAdded(cardNumber);
            }

            // Called this method on already validated number. Save TS call.
            if (cardNumber.Equals(validatedCardNumber, StringComparison.OrdinalIgnoreCase))
            {
                return RoundForDisplay(validatedCardBalance, validatedCardCurrency, GiftCard.InternalApplication);
            }

            string result = string.Empty;

            // Begin by checking if there is a connection to the Transaction Service
            if (GiftCard.InternalApplication.TransactionServices.CheckConnection())
            {
                // If used another card, release currenty locked gift card from HQ.
                if (!string.IsNullOrEmpty(validatedCardNumber))
                {
                    VoidGiftCardPayment();
                }

                string comment = string.Empty;
                bool success = false;

                validatedCardCurrency = string.Empty;
                validatedCardBalance = 0M;

                GiftCard.InternalApplication.TransactionServices.ValidateGiftCard(ref success, ref comment, ref validatedCardCurrency, ref validatedCardBalance,
                    cardNumber, ApplicationSettings.Terminal.StoreId, ApplicationSettings.Terminal.TerminalId);

                if (success)
                {
                    validatedCardNumber = cardNumber;
                    result = RoundForDisplay(validatedCardBalance, validatedCardCurrency, GiftCard.InternalApplication);
                }
                else
                {
                    throw new GiftCardException(comment);
                }
            }

            return result;
        }

        /// <summary>
        /// Void/unlock the existing validate/locked card.
        /// </summary>
        /// <exception cref="GiftCardException"></exception>
        public void VoidGiftCardPayment()
        {
            // There is no card to void.
            if (string.IsNullOrEmpty(validatedCardNumber))
            {
                return;
            }

            try
            {
                // Begin by checking if there is a connection to the Transaction Service
                if (GiftCard.InternalApplication.TransactionServices.CheckConnection())
                {
                    bool success = false;
                    string comment = string.Empty;

                    GiftCard.InternalApplication.TransactionServices.VoidGiftCardPayment(ref success, ref comment, validatedCardNumber, ApplicationSettings.Terminal.StoreId,
                        ApplicationSettings.Terminal.TerminalId);

                    if (success)
                    {
                        validatedCardNumber = string.Empty;
                    }
                    else
                    {
                        throw new GiftCardException(comment);
                    }
                }
            }
            catch (PosisException ex)
            {
                // Throw custom exception if TS call failed.
                string message = ApplicationLocalizer.Language.Translate(55415);

                throw new GiftCardException(message, ex);
            }
        }

        /// <summary>
        /// Validate amount with tender rules.
        /// </summary>
        /// <param name="amount"></param>
        /// <exception cref="GiftCardException"></exception>
        public void ValidateTenderAmount(decimal amount)
        {
            if (!string.IsNullOrEmpty(validatedCardNumber))
            {
                if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments && (Transaction is RetailTransaction))
                {
                    ValidatePayByGiftCard(validatedCardNumber, amount);
                }
                
                // Check if the card is issued with different currency.
                if (validatedCardCurrency != ApplicationSettings.Terminal.StoreCurrency)
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(4324));
                }

                // First check fo gift card balance.
                if (amount > validatedCardBalance)
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(55411));
                }
                else
                {
                    TenderRequirement tenderReq = new TenderRequirement(this.tenderInfo, amount, true, TransactionAmount);
                    if (!string.IsNullOrEmpty(tenderReq.ErrorText))
                    {
                        throw new GiftCardException(tenderReq.ErrorText);
                    }

                    CardNumber = validatedCardNumber;
                    Balance = validatedCardBalance;
                    Amount = amount;
                    Currency = validatedCardCurrency;
                }
            }
        }

        /// <summary>
        /// Query balance of a gift card.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <returns>Balance on gift card</returns>
        /// <exception cref="GiftCardException"></exception>
        public string GetGiftCardBalance(string cardNumber)
        {
            string result = string.Empty;

            if (GiftCard.InternalApplication.TransactionServices.CheckConnection())
            {
                bool succeeded = false;
                string comment = string.Empty;
                string currencyCode = string.Empty;
                decimal giftCardBalance = 0M;

                CardNumber = string.Empty;
                Balance = 0;

                GiftCard.InternalApplication.TransactionServices.GetGiftCardBalance(ref succeeded, ref comment, ref currencyCode, ref giftCardBalance, cardNumber);

                if (succeeded)
                {
                    CardNumber = cardNumber;
                    Balance = giftCardBalance;
                    Currency = currencyCode;
                    result = RoundForDisplay(giftCardBalance, currencyCode, GiftCard.InternalApplication);
                }
                else
                {
                    throw new GiftCardException(comment);
                }
            }

            return result;
        }

        /// <summary>
        /// Print the gift card balance receipt.
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <exception cref="GiftCardException"></exception>
        public void PrintGiftCardBalance(string cardNumber)
        {
            if (Printer.DeviceType == DeviceTypes.None)
            {
                throw new GiftCardException(ApplicationLocalizer.Language.Translate(10060));
            }

            // If balance already queried skip TS call.
            if ((string.IsNullOrEmpty(CardNumber)) || (!cardNumber.Equals(CardNumber, StringComparison.OrdinalIgnoreCase)))
            {
                this.GetGiftCardBalance(cardNumber);
            }

            if (Currency != ApplicationSettings.Terminal.StoreCurrency)
            {
                throw new GiftCardException(ApplicationLocalizer.Language.Translate(4324));
            }

            IGiftCardLineItem gcItem = GiftCard.InternalApplication.BusinessLogic.Utility.CreateGiftCardLineItem(
                ApplicationSettings.Terminal.StoreCurrency, GiftCard.InternalApplication.Services.Rounding, (IRetailTransaction)Transaction);
            gcItem.SerialNumber = CardNumber;
            gcItem.Balance = Balance;
            // Required for receipt header
            ((IPosTransactionV2)Transaction).EndDateTime = DateTime.Now;

            GiftCard.InternalApplication.Services.Printing.PrintGiftCertificate(FormType.GiftCertificate, Transaction, gcItem, false);
        }

        /// <summary>
        /// Issue a gift card
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="amount"></param>
        /// <exception cref="GiftCardException"></exception>
        public void IssueGiftCard(string cardNumber, decimal amount)
        {
            if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments && (Transaction is RetailTransaction))
            {
                ValidateGiftCardIssue(cardNumber, amount);
            }
            else
            {
                AssertNotAlreadyAdded(cardNumber);
            }

            // Begin by checking if there is a connection to the Transaction Service
            if (GiftCard.InternalApplication.TransactionServices.CheckConnection())
            {
                bool succeeded = false;
                string comment = string.Empty;

                // Issue the gift card through the transaction services and add it to the transaction
                GiftCard.InternalApplication.TransactionServices.IssueGiftCard(ref succeeded, ref comment, ref cardNumber,
                        ApplicationSettings.Terminal.StoreId, ApplicationSettings.Terminal.TerminalId, Transaction.OperatorId,
                        Transaction.TransactionId, Transaction.ReceiptId, ApplicationSettings.Terminal.StoreCurrency,
                        amount, DateTime.Now);

                if (!succeeded || cardNumber.Length == 0)
                {
                    throw new GiftCardException(comment);
                }
                else
                {
                    CardNumber = cardNumber;
                    Amount = amount;
                    Balance = amount;
                    Currency = ApplicationSettings.Terminal.StoreCurrency;
                }
            }
        }

        /// <summary>
        /// Add money to gift card.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <param name="amount"></param>
        /// <exception cref="GiftCardException"></exception>
        public void AddToGiftCard(string cardNumber, decimal amount)
        {
            if (ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments && (Transaction is RetailTransaction))
            {
                PreValidateAddToGiftCard(); // run once more just in case method could be called form some other places
                ValidateAddToGiftCard(cardNumber);
            }
            else
            {
                AssertNotAlreadyAdded(cardNumber);
            }

            // Begin by checking if there is a connection to the Transaction Service
            if (GiftCard.InternalApplication.TransactionServices.CheckConnection())
            {
                bool succeeded = false;
                string comment = string.Empty;
                decimal balance = 0;

                // Depsit money to gift card through the transaction services and add it to the transaction
                GiftCard.InternalApplication.TransactionServices.AddToGiftCard(ref succeeded, ref comment, ref balance, cardNumber,
                    ApplicationSettings.Terminal.StoreId, ApplicationSettings.Terminal.TerminalId, Transaction.OperatorId,
                    Transaction.TransactionId, Transaction.ReceiptId, ApplicationSettings.Terminal.StoreCurrency, amount, DateTime.Now);

                if (!succeeded)
                {
                    throw new GiftCardException(comment);
                }
                else
                {
                    CardNumber = cardNumber;
                    Amount = amount;
                    Balance = balance;
                    Currency = ApplicationSettings.Terminal.StoreCurrency;
                }
            }
        }

        /// <summary>
        /// Pre validate current transaction before add to gift card operation.
        /// </summary>
        /// <exception cref="GiftCardException"></exception>
        public void PreValidateAddToGiftCard()
        {
            RetailTransaction retailTransaction = Transaction as RetailTransaction;

            if (retailTransaction != null)
            {
                if (retailTransaction.SaleIsReturnSale)
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107004));
                }

                if (retailTransaction.SaleItems != null &&
                    retailTransaction.SaleItems.OfType<ISaleLineItem>().Any(l => (!l.Voided && l.Quantity < 0)))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107004));
                }

                // Add to gift card operation is not allowed, if transaction already contains any gift card payment.
                if (retailTransaction.TenderLines != null &&
                    retailTransaction.TenderLines.OfType<IGiftCardTenderLineItem>().Any(l => !l.Voided))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107000));
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Check if Gift card is already added to transaction either as sale or payment.
        /// </summary>
        /// <param name="cardNumber"></param>
        /// <exception cref="GiftCardException"></exception>
        private void AssertNotAlreadyAdded(string cardNumber)
        {
            // Once a Gift card is added to the transaction for any operation (Issue, AddTo, Payment), then
            // the same card cannot be added to the transaction again for any other operation.

            RetailTransaction retailTransaction = Transaction as RetailTransaction;
            CustomerPaymentTransaction customerPaymentTransaction = Transaction as CustomerPaymentTransaction;
            LinkedList<SaleLineItem> salesLines = null;
            LinkedList<TenderLineItem> tenderLines = null;

            if (retailTransaction != null)
            {
                tenderLines = retailTransaction.TenderLines;
                salesLines = retailTransaction.SaleItems;
            }
            else if (customerPaymentTransaction != null)
            {
                tenderLines = customerPaymentTransaction.TenderLines;
            }

            // Check for all sales lines of type GiftCertificateItem where they are not voided and the serial number matches the card number.
            if (salesLines != null
                && salesLines.OfType<IGiftCardLineItem>().Any(l => (!l.Voided) && string.Equals(l.SerialNumber, cardNumber, StringComparison.Ordinal)))
            {
                throw new GiftCardException(ApplicationLocalizer.Language.Translate(4323));
            }

            // Check for all tender lines of type GiftCertificateTenderLineItem where they are not voided and the serial number matches the card number.
            if (tenderLines != null
                && tenderLines.OfType<IGiftCardTenderLineItem>().Any(l => (!l.Voided) && string.Equals(l.SerialNumber, cardNumber, StringComparison.Ordinal)))
            {
                throw new GiftCardException(ApplicationLocalizer.Language.Translate(4323));
            }
        }

        private static string RoundForDisplay(decimal amount, string currencyCode, IApplication application)
        {
            return string.Format(CultureInfo.CurrentUICulture, "{0} ({1})",
                application.Services.Rounding.RoundForDisplay(amount, currencyCode, false, false), currencyCode);
        }

        /// <summary>
        /// Validates gift card issue operation.
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Issue amount</param>
        /// <exception cref="GiftCardException"></exception>
        private void ValidateGiftCardIssue(string cardNumber, decimal amount)
        {
            RetailTransaction retailTransaction = Transaction as RetailTransaction;

            if (retailTransaction != null)
            {
                if (amount != decimal.Zero && 
                    retailTransaction.SaleIsReturnSale)
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107003));
                }

                if (amount != decimal.Zero &&
                    retailTransaction.SaleItems != null &&
                    retailTransaction.SaleItems.OfType<ISaleLineItem>().Any(l => (!l.Voided && l.Quantity < 0)))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107003));
                }

                // Gift card issue operation is not allowed, if transaction already contains any gift card payment. 
                // Exception: zero amount card issue is allowed in the same transaction with with gift card refund oeprations.
                if (retailTransaction.TenderLines != null &&
                    retailTransaction.TenderLines.OfType<IGiftCardTenderLineItem>().Any(l => !l.Voided && !(amount == decimal.Zero && l.Amount < decimal.Zero)))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107000));
                }

                // Gift card issue operation is not allowed, if transaction already contains replanishment with the same gift card number (core behavior)
                if (retailTransaction.SaleItems != null &&
                    retailTransaction.SaleItems.OfType<IGiftCardLineItem>().Any(l => !l.Voided && string.Equals(l.SerialNumber, cardNumber, StringComparison.Ordinal)))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(4323));
                }
            }
        }

        /// <summary>
        /// Validates add to gift card operation.
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Replenishment amount</param>
        /// <exception cref="GiftCardException"></exception>
        private void ValidateAddToGiftCard(string cardNumber)
        {
            RetailTransaction retailTransaction = Transaction as RetailTransaction;

            // Add to gift card operation is not allowed, if transaction already contains replanishment with the same gift card number (core behavior)
            if (retailTransaction != null &&
                retailTransaction.SaleItems != null &&
                retailTransaction.SaleItems.OfType<IGiftCardLineItem>().Any(l => (!l.Voided) && string.Equals(l.SerialNumber, cardNumber, StringComparison.Ordinal)))
            {
                throw new GiftCardException(ApplicationLocalizer.Language.Translate(4323));
            }
        }

        /// <summary>
        /// Validates pay by gift card operation.
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="amount">Issue amount</param>
        /// <exception cref="GiftCardException"></exception>
        private void ValidatePayByGiftCard(string cardNumber, decimal amount)
        {
            RetailTransaction retailTransaction = Transaction as RetailTransaction;

            if (retailTransaction != null)
            {
                // Gift card payment is not allowed, if transaction contains any Gift card issue or Add to gift card operation. 
                // Exception: gift card refund is allowed in the same transaction with with the zero gift card issue operation.
                if (retailTransaction.SaleItems != null &&
                    retailTransaction.SaleItems.OfType<IGiftCardLineItem>().Any(l => !l.Voided && !(amount < decimal.Zero && l.Amount == decimal.Zero && !l.AddTo)))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(107001));
                }

                // Gift card payment is not allowed, if transaction contains any payment with the same gift card number (core behavior).
                if (retailTransaction.TenderLines != null &&
                    retailTransaction.TenderLines.OfType<IGiftCardTenderLineItem>().Any(l => (!l.Voided) && string.Equals(l.SerialNumber, cardNumber, StringComparison.Ordinal)))
                {
                    throw new GiftCardException(ApplicationLocalizer.Language.Translate(4323));
                }
            }
        }

        #endregion
    }
}
