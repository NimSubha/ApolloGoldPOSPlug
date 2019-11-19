/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.Linq;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        /// <summary>
        /// Returns true if print preview ids shown.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <returns></returns>
        public bool ShowPrintPreview(FormType formType, IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.ShowPrintPreview", "formType = {0}, posTransaction = {1}", formType, posTransaction);

            return true; //ok to proceed
        }

        /// <summary>
        /// Print the standard slip, returns false if printing should be aborted altogether.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="copyReceipt"></param>
        /// <returns></returns>
        public bool PrintReceipt(FormType formType, IPosTransaction posTransaction, bool copyReceipt)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintReceipt", "formType = {0}, posTransaction = {1}, copyReceipt = {2}", formType, posTransaction, copyReceipt);

            var retail = posTransaction as RetailTransaction;

            if (retail != null &&
                formType == FormType.Receipt &&
                copyReceipt)
            {
                RussianFiscalPrinterDriver.FiscalPrinterDriver.PrintLastReceiptCopy(); 
            }

            return true;
        }

        /// <summary>
        /// Prints Bank drop Receipt.
        /// </summary>
        /// <param name="posTransaction">BankDropTransaction</param>
        public void PrintBankDrop(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintBankDrop", "posTransaction = {0}", posTransaction);

            PrintMoneyDrop(posTransaction, Resources.Translate(Resources.MessageOperationBankDrop));
        }

        /// <summary>
        /// Prints safe drop Receipt.
        /// </summary>
        /// <param name="posTransaction">SafeDropTransaction</param>
        public void PrintSafeDrop(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintSafeDrop", "posTransaction = {0}", posTransaction);

            PrintMoneyDrop(posTransaction, Resources.Translate(Resources.MessageOperationSafeDrop));
        }

        private static void PrintMoneyDrop(IPosTransaction posTransaction, string sourceOperation)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintMoneyDrop", "posTransaction = {0}, sourceOperation = {1}", posTransaction, sourceOperation);

            var transaction = posTransaction as TenderCountTransaction;

            if (transaction != null)
            {
                var total = transaction.TenderLines.Sum(tenderLine => tenderLine.Amount);
                FiscalPrinterDriverFactory.FiscalPrinterDriver.PrintMoneyDrop(total, sourceOperation);
            }
        }

        /// <summary>
        /// Print invoice receipt.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="copyInvoice"></param>
        /// <param name="printPreview"></param>
        /// <returns></returns>
        public void PrintInvoice(IPosTransaction posTransaction, bool copyInvoice, bool printPreview)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintInvoice", "posTransaction = {0}, copyInvoice = {1}, printPreview = {2}", posTransaction, copyInvoice, printPreview);
        }

        /// <summary>
        /// Print Tender Removal.
        /// </summary>
        /// <param name="posTransaction">RemoveTenderTransaction</param>
        public void PrintRemoveTender(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintRemoveTender", "posTransaction = {0}", posTransaction);

            // We handle tender removal declaration printing in the PreEndTransaction trigger.
        }

        /// <summary>
        /// Print Float Entry Receipt
        /// </summary>
        /// <param name="posTransaction">FloatEntryTransaction</param>
        public void PrintFloatEntry(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintFloatEntry", "posTransaction = {0}", posTransaction);

            // We handle float entry declaration printing in PreEndTransaction trigger.
        }

        /// <summary>
        /// Print Starting Amount Declaration Receipt
        /// </summary>
        /// <param name="posTransaction">StartingAmountTransaction</param>
        public void PrintStartingAmount(IPosTransaction posTransaction)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintStartAmount", "posTransaction = {0}", posTransaction);

            // We handle starting amount declaration printing in PreEndTransaction trigger.
        }

        /// <summary>
        /// Prints the loyalty card balance.
        /// </summary>
        /// <param name="loyaltyCardData">The <see cref="LoyaltyCardData"/> object containing the information about the loyalty card.</param>
        public void PrintLoyaltyCardBalance(ILoyaltyCardData loyaltyCardData)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinting.PrintLoyaltyCardBalance", "loyaltyCardData = {0}", loyaltyCardData);

            RussianFiscalPrinterDriver.FiscalPrinterDriver.PrintLoyaltyCardBalance(loyaltyCardData);
        }
    }
}
