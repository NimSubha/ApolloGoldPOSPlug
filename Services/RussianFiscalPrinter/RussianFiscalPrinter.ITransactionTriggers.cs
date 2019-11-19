/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using LSRetailPosis.DataAccess;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        #region ITransactionTriggers implementation

        /// <summary>
        /// Triggered at the start of a new transaction, but after loading the transaction with initialisation 
        /// data, such as the store, terminal number, date, etc...
        /// </summary>
        /// <param name="posTransaction"></param>
        public void BeginTransaction(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers", "BeginTransaction() called");

            if (posTransaction == null)
            {
                throw new ArgumentNullException("posTransaction");
            }
        }

        /// <summary>
        /// Triggered at the end a transaction, before saving the transaction and printing of receipts.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreEndTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers", "PreEndTransaction() called");

            if (preTriggerResult == null)
            {
                throw new System.ArgumentNullException("preTriggerResult");
            }

            if (posTransaction == null)
            {
                throw new System.ArgumentNullException("posTransaction");
            }

            var posTrans = posTransaction as PosTransaction;

            if (posTrans == null)
            {
                return;
            }

            switch (posTrans.EntryStatus)
            {
                case PosTransaction.TransactionStatus.Normal:
                    //
                    // Break and continue to process the Pre event
                    //
                    break;
                case PosTransaction.TransactionStatus.Voided:
                    LogHelper.LogDebug("FiscalPrinter.PreEndTransaction", "ITransactionTriggers.PreEndTransaction was called to void open retail transaction.");
                    return;
                case PosTransaction.TransactionStatus.OnHold:
                    LogHelper.LogDebug("FiscalPrinter.PreEndTransaction", "ITransactionTriggers.PreEndTransaction was called to put retail transaction on hold.");
                    return;
                default:
                    LogHelper.LogDebug("FiscalPrinter.PreEndTransaction", "ITransactionTriggers.PreEndTransaction was called with TransactionStatus={0}.", posTrans.EntryStatus.ToString());
                    return;
            }

            ITrigger trigger = TriggerFactory.Create(TransactionTriggerType.PreEndTransaction, posTransaction);

            if (trigger == null)
            {
                return;
            }

            try
            {
                trigger.PreExecute();

                if (IsPrinterReady())
                {
                    trigger.Execute();
                }
                else
                {
                    preTriggerResult.ContinueOperation = false;
                    preTriggerResult.MessageId = Resources.MessagePrinterNotReady;
                }

            }
            catch (FiscalPrinterException exception)
            {
                try
                {
                    trigger.Abort(exception);
                }
                catch
                {
                    //Ignore the failure after trying to cancel the printing document
                }

                preTriggerResult.ContinueOperation = IsPrintingCommandSent;
            }
        }

        /// <summary>
        /// Triggered at the end a transaction, after saving the transaction and printing of receipts
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostEndTransaction(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers.PostEndTransaction", "posTransaction = {0}", posTransaction);
        }

        /// <summary>
        /// Triggered prior to voiding a transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreVoidTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers.PreVoidTransaction", "preTriggerResult = {0}, posTransaction = {1}", preTriggerResult, posTransaction);

            var retailTransaction = posTransaction as RetailTransaction;
            if (retailTransaction == null)
            {
                return;
            }

            retailTransaction.RefundReceiptId = string.Empty;
        }

        /// <summary>
        /// Triggered after voiding a transaction.
        /// </summary>
        /// <param name="posTransaction">POS transaction</param>
        public void PostVoidTransaction(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers", "PostVoidTransaction() called: posTransaction = {0}", posTransaction);

            var retailTransaction = posTransaction as RetailTransaction;
            if (retailTransaction == null)
            {
                LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers", "PostVoidTransaction() was called with a non retail transaction");
            }
        }

        /// <summary>
        /// Triggered prior to returning.
        /// </summary>
        /// <param name="preTriggerResult">The return parameter</param>
        /// <param name="originalTransaction">The original transaction</param>
        /// <param name="posTransaction">The transaction containing only the selected items to be returned</param>
        public void PreReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction, IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers", "PreReturnTransaction() called: preTriggerResult = {0}, originalTransaction = {1}, posTransaction = {2}", preTriggerResult, originalTransaction, posTransaction);

            var retailTransaction = posTransaction as RetailTransaction;
            if (retailTransaction == null)
            {
                return;
            }

            if (originalTransaction == null)
            {
                throw new System.ArgumentNullException("originalTransaction");
            }

            if (preTriggerResult == null)
            {
                throw new System.ArgumentNullException("preTriggerResult");
            }
        }

        /// <summary>
        /// Triggered during save of a transaction to database.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="sqlTransaction"></param>
        public void SaveTransaction(IPosTransaction posTransaction, SqlTransaction sqlTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.ITransactionTriggers", "SaveTransaction() was called: posTransaction = {0}, sqlTransaction = {1}", posTransaction, sqlTransaction);

            if (posTransaction == null)
            {
                return;
            }

            var transaction = posTransaction as PosTransaction;

            if (transaction == null)
            {
                return;
            }

            if (transaction.EntryStatus == PosTransaction.TransactionStatus.Normal &&
                ((transaction.TransactionType == PosTransaction.TypeOfTransaction.LogOn && RussianFiscalPrinterDriver.IsShiftOpenedAtLastLogOn) ||
                transaction.TransactionType == PosTransaction.TypeOfTransaction.CloseShift ||
                transaction.TransactionType == PosTransaction.TypeOfTransaction.Sales ||
                transaction.TransactionType == PosTransaction.TypeOfTransaction.StartingAmount ||
                transaction.TransactionType == PosTransaction.TypeOfTransaction.FloatEntry ||
                transaction.TransactionType == PosTransaction.TypeOfTransaction.RemoveTender))
            {
                FiscalMemoryData fiscalMemoryData = null;
                bool readSucceeded = false;

                try
                {
                    bool retrieveFiscalMemoryData = true;
                    var retailTransaction = posTransaction as RetailTransaction;
                    if (retailTransaction != null && retailTransaction.SaleIsReturnSale && !ApplicationSettings.Terminal.ProcessReturnsAsInOriginalSaleShift_RU)
                    {
                        var returnedTransaction = new TransactionData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, PosApplication.Instance).GetOriginalRetailTransaction(retailTransaction);
                        bool? hasSameShiftId = Instance.HasSameShiftId(returnedTransaction);
                        // Provide empty fiscal memory data structure for saving in the POS DB in case we have a return in a different shift and 'Process as a return in the same shift' flag is off in Retail Parameters table in AX.
                        if ((hasSameShiftId.HasValue && !hasSameShiftId.Value)
                            || (returnedTransaction.Shift != null && returnedTransaction.Shift.BatchId != PosApplication.Instance.Shift.BatchId))
                            retrieveFiscalMemoryData = false;
                    }
                    if (retrieveFiscalMemoryData)
                    {
                        fiscalMemoryData = RussianFiscalPrinterDriver.FiscalPrinterDriver.RetrieveFiscalMemoryData(transaction.TransactionType);
                    }
                    else
                    {
                        fiscalMemoryData = new FiscalMemoryData();
                    }
                    readSucceeded = true;
                }
                catch (Exception ex)
                {
                    LogHelper.LogError("RussianFiscalPrinter.SaveTransaction", ex.Message);
                    UserMessages.ShowWarning(Resources.ErrorReadingFiscalAttributesFromThePrinter);
                    readSucceeded = false;
                }

                if (readSucceeded)
                {
                    try
                    {
                        if (sqlTransaction == null)
                        {
                            throw new ArgumentNullException("sqlTransaction");
                        }

                        new TransactionFiscalData(sqlTransaction.Connection, ApplicationSettings.Database.DATAAREAID).Save(transaction, fiscalMemoryData);
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogError("RussianFiscalPrinter.SaveTransaction", ex.Message);
                        UserMessages.ShowWarning(Resources.ErrorWritingFiscalAttributesFromThePrinter);
                    }
                }

                if (transaction.TransactionType == PosTransaction.TypeOfTransaction.LogOn)
                {
                    RussianFiscalPrinterDriver.IsShiftOpenedAtLastLogOn = false; 
                }
            }
        }

        #endregion

        #region Russian fiscal printer trigger operations

        /// <summary>
        /// An interface providing an abstraction for all trigger operations that are fired by POS for the fiscal printer.
        /// </summary>
        private interface ITrigger
        {
            /// <summary>
            /// This method is called before the actual trigger action.
            /// </summary>
            void PreExecute();
            /// <summary>
            /// This method encapsulates the main trigger action.
            /// </summary>
            void Execute();
            /// <summary>
            /// This method gets called when an <see cref="FiscalPrinterException">Exception</see> gets cought during trigger execution./>
            /// </summary>
            /// <param name="ex">A <see cref="FiscalPrinterException">FiscalPrinterException</see> that was cought during trigger action execution.</param>
            void Abort(FiscalPrinterException ex);
        }

        /// <summary>
        /// A class encapsulating PreEndTransaction trigger for the <see cref="RetailTransaction">RetailTransaction</see>.
        /// </summary>
        private class PreEndRetailTransactionTrigger : ITrigger
        {
            private RetailTransaction retailTransaction;

            public PreEndRetailTransactionTrigger(RetailTransaction retailTransaction)
            {
                this.retailTransaction = retailTransaction;
            }

            public void PreExecute()
            {
                if (retailTransaction == null)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(retailTransaction.LoyaltyItem.LoyaltyCardNumber))
                {
                    PosApplication.Instance.Services.Loyalty.CalculateLoyaltyPoints(retailTransaction);
                }
            }

            public void Execute()
            {
                if (retailTransaction == null)
                {
                    return;
                }

                // Prohibit printing fiscal receipt in case the return transaction was registered in shift different from the original sale transaction shift and 'Process as a return in the same shift' flag is off in Retail parameters.
                if (retailTransaction.SaleIsReturnSale && !ApplicationSettings.Terminal.ProcessReturnsAsInOriginalSaleShift_RU)
                {
                    var returnedTransaction = new TransactionData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, PosApplication.Instance).GetOriginalRetailTransaction(retailTransaction);
                    bool? hasSameShiftId = Instance.HasSameShiftId(returnedTransaction);
                    if (hasSameShiftId.HasValue && !hasSameShiftId.Value)
                        return;

                    if (returnedTransaction.Shift != null && returnedTransaction.Shift.BatchId != PosApplication.Instance.Shift.BatchId)
                        return;
                }

                PrintFiscalDocument(retailTransaction);
            }

            public void Abort(FiscalPrinterException ex)
            {
                FiscalPrinterDriverFactory.FiscalPrinterDriver.CancelReceipt(ReceiptType.None, ex.Message);
            }
        }

        /// <summary>
        /// A class handling pre end transaction trigger logic for the <see cref="StartingAmountTransaction"/>.
        /// </summary>
        private class PreEndStartingAmountTransactionTrigger : ITrigger
        {
            private StartingAmountTransaction startingAmountTransaction;

            public PreEndStartingAmountTransactionTrigger(StartingAmountTransaction startingAmountTransaction)
            {
                this.startingAmountTransaction = startingAmountTransaction;
            }

            public void PreExecute()
            {
                // We do not have any pre-actions for this type of transaction.
            }

            public void Execute()
            {
                RussianFiscalPrinterDriver.FiscalPrinterDriver.PrintStartingAmount(startingAmountTransaction.Amount, startingAmountTransaction.Description);
            }

            public void Abort(FiscalPrinterException ex)
            {
                // We do not have any abort actions for this type of transaction.
            }
        }

        /// <summary>
        /// A class handling the pre end transaction trigger logic for the <see cref="FloatEntryTransaction"/>.
        /// </summary>
        private class PreEndFloatEntryTransactionTrigger : ITrigger
        {
            private FloatEntryTransaction floatEntryTransaction;

            public PreEndFloatEntryTransactionTrigger(FloatEntryTransaction floatEntryTransaction)
            {
                this.floatEntryTransaction = floatEntryTransaction;
            }

            public void PreExecute()
            {
                // We do not have any pre-actions for this type of transaction.
            }

            public void Execute()
            {
                FiscalPrinterDriverFactory.FiscalPrinterDriver.PrintMoneyEntry(floatEntryTransaction.Amount, floatEntryTransaction.Description);
            }

            public void Abort(FiscalPrinterException ex)
            {
                // We do not have any abort actions for this type of transaction.
            }
        }

        /// <summary>
        /// A class handling the pre end transaction trigger logic for the <see cref="RemoveTenderTransaction"/>.
        /// </summary>
        private class PreEndRemoveTenderTransactionTrigger : ITrigger
        {
            private RemoveTenderTransaction removeTenderTransaction;

            public PreEndRemoveTenderTransactionTrigger(RemoveTenderTransaction removeTenderTransaction)
            {
                this.removeTenderTransaction = removeTenderTransaction;
            }

            public void PreExecute()
            {
                // We do not have any pre-actions for this type of transaction.
            }

            public void Execute()
            {
                FiscalPrinterDriverFactory.FiscalPrinterDriver.PrintMoneyDrop(removeTenderTransaction.Amount, removeTenderTransaction.Description);
            }

            public void Abort(FiscalPrinterException ex)
            {
                // We do not have any abort actions for this type of transaction.
            }
        }

        private enum TransactionTriggerType
        {
            None,
            PreEndTransaction
        }

        /// <summary>
        /// A static factory for creating instances of classes implementing <see cref="ITrigger">ITrigger</see> interface.
        /// </summary>
        private static class TriggerFactory
        {
            /// <summary>
            /// Creates an instance of the class implementing <see cref="ITrigger">ITrigger</see> interface.
            /// </summary>
            /// <param name="triggerType">The type of the trigger.</param>
            /// <param name="posTransaction">A reference to the current <see cref="IPosTransaction">posTransaction</see>.</param>
            /// <returns></returns>
            public static ITrigger Create(TransactionTriggerType triggerType, IPosTransaction posTransaction)
            {
                ITrigger trigger = null;

                switch (triggerType)
                {
                    case TransactionTriggerType.PreEndTransaction:

                        var retailTransaction = posTransaction as RetailTransaction;
                        if (retailTransaction != null)
                        {
                            trigger = new PreEndRetailTransactionTrigger(retailTransaction);
                            break;
                        }

                        var transaction = posTransaction as StartingAmountTransaction;
                        if (transaction != null)
                        {
                            trigger = new PreEndStartingAmountTransactionTrigger(transaction);
                            break;
                        }
                        
                        var entryTransaction = posTransaction as FloatEntryTransaction;
                        if (entryTransaction != null)
                        {
                            trigger = new PreEndFloatEntryTransactionTrigger(entryTransaction);
                            break;
                        }
                        
                        var tenderTransaction = posTransaction as RemoveTenderTransaction;
                        if (tenderTransaction != null)
                        {
                            trigger = new PreEndRemoveTenderTransactionTrigger(tenderTransaction);
                        }
                        break;
                }

                return trigger;
            }
        }
        #endregion
    }
}
