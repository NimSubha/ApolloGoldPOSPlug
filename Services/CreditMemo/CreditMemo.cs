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
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.CreditMemoItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.CreditMemo.WinFormsTouch;

namespace Microsoft.Dynamics.Retail.Pos.CreditMemo
{
    [Export(typeof(ICreditMemo))]
    public class CreditMemo : ICreditMemo
    {
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

        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for the CreditMemo service are reserved at 52300 - 52999
        // TextID's for the following modules are as follows:
        //
        // CreditMemo.cs:           52300 - 52599.  The last in use: 52300
        // frmPayCreditMemo.cs:     52600 - 52649


        #region ICreditMemo Members

        /// <summary>
        /// Issueing the Credit Memo, that is we have already created it so here we are publishing to the
        /// e.g. HO that we have issued it.
        /// </summary>
        /// <param name="creditMemoItem"></param>
        /// <param name="transaction"></param>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#")]
        public void IssueCreditMemo(ICreditMemoTenderLineItem creditMemoItem, IRetailTransaction transaction)
        {
            if (creditMemoItem == null)
            {
                throw new ArgumentNullException("creditMemoItem");
            }

            RetailTransaction retailTransaction = transaction as RetailTransaction;
            if (retailTransaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            try
            {
                LogMessage("Issuing a credit memo....", LogTraceLevel.Trace, "CreditMemo.IssueCreditMemo");

                bool retVal = false;
                string comment = string.Empty;
                string creditMemoNumber = string.Empty;
                string currencyCode = ApplicationSettings.Terminal.StoreCurrency;

                try
                {
                    // Begin by checking if there is a connection to the Transaction Service
                    this.Application.TransactionServices.CheckConnection();

                    // Publish the credit memo to the Head Office through the Transaction Services...
                    this.Application.TransactionServices.IssueCreditMemo(ref retVal, ref comment, ref creditMemoNumber,
                        retailTransaction.StoreId,
                        retailTransaction.TerminalId,
                        retailTransaction.OperatorId,
                        retailTransaction.TransactionId,
                        retailTransaction.ReceiptId,
                        "1",
                        currencyCode,
                        creditMemoItem.Amount * -1,
                        DateTime.Now);

                    retailTransaction.CreditMemoItem.CreditMemoNumber = creditMemoNumber;
                    retailTransaction.CreditMemoItem.Amount = creditMemoItem.Amount * -1;
                    creditMemoItem.SerialNumber = creditMemoNumber;
                    creditMemoItem.Comment = creditMemoNumber;
                }
                catch (LSRetailPosis.PosisException px)
                {
                    // We cannot publish the credit memo to the HO, so we need to take action...

                    retailTransaction.TenderLines.RemoveLast();
                    retailTransaction.CalcTotals();

                    retailTransaction.CreditMemoItem = (CreditMemoItem)this.Application.BusinessLogic.Utility.CreateCreditMemoItem();

                    LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                    throw;
                }
                catch (Exception x)
                {
                    // We cannot publish the credit memo to the HO, so we need to take action...

                    retailTransaction.TenderLines.RemoveLast();
                    retailTransaction.CalcTotals();
                    retailTransaction.CreditMemoItem = (CreditMemoItem)this.Application.BusinessLogic.Utility.CreateCreditMemoItem();

                    LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                    throw new LSRetailPosis.PosisException(52300, x);
                }

                if (!retVal)
                {
                    LogMessage("Error storing the credit memo centrally...",
                        LSRetailPosis.LogTraceLevel.Error,
                        "CreditMemo.IssueCreditMemo");

                    retailTransaction.TenderLines.RemoveLast();
                    retailTransaction.CalcTotals();
                    retailTransaction.CreditMemoItem = (CreditMemoItem)this.Application.BusinessLogic.Utility.CreateCreditMemoItem();

                    throw new LSRetailPosis.PosisException(52300, new Exception(comment));
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Authorize payment with a credit memo
        /// </summary>
        public void AuthorizeCreditMemoPayment(ref bool valid, ref string comment, ref string creditMemoId, ref string currencyCode, ref decimal amount, IPosTransaction posTransaction)
        {
            try
            {
                //View the credit memo dialog to enter the credit memo number and to verify the credit memo exists.

                using (frmPayCreditMemo creditMemoForm = new frmPayCreditMemo())
                {
                    this.Application.ApplicationFramework.POSShowForm(creditMemoForm);
                    if (creditMemoForm.DialogResult == DialogResult.Cancel)
                    {
                        valid = false;
                        comment = string.Empty;
                        creditMemoId = string.Empty;
                        amount = 0M;
                        currencyCode = string.Empty;
                    }
                    else
                    {
                        valid = true;
                        comment = string.Empty;
                        creditMemoId = creditMemoForm.CreditMemoId;
                        amount = creditMemoForm.Amount;
                        currencyCode = creditMemoForm.CurrencyCode;
                    }
                }
            }
            catch (LSRetailPosis.PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                throw;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Updates credit memo.
        /// </summary>
        /// <param name="creditMemoNumber"></param>
        /// <param name="amount"></param>
        /// <param name="retailTransaction"></param>
        /// <param name="creditMemoTenderLineItem"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Grandfather")]
        public void UpdateCreditMemo(string creditMemoNumber, decimal amount, IRetailTransaction retailTransaction, ICreditMemoTenderLineItem creditMemoTenderLineItem)
        {
            try
            {
                LogMessage("Marking a credit memo as used....", LogTraceLevel.Trace, "CreditMemo.UpdateCreditMemo");

                bool retVal = false;
                string comment = string.Empty;

                try
                {
                    // Begin by checking if there is a connection to the Transaction Service
                    this.Application.TransactionServices.CheckConnection();

                    this.Application.TransactionServices.UpdateCreditMemo(ref retVal, ref comment, creditMemoNumber,
                        retailTransaction.StoreId,
                        retailTransaction.TerminalId,
                        retailTransaction.OperatorId,
                        retailTransaction.TransactionId,
                        retailTransaction.ReceiptId,
                        "1",
                        amount,
                        DateTime.Now);
                }
                catch (Exception)
                {
                    LogMessage("Error updating the credit memo centrally, as used...",
                        LogTraceLevel.Error,
                        "CreditMemo.UpdateCreditMemo");
                }

                if (retVal == false)
                {
                    LogMessage("Error updating the credit memo centrally, as used...",
                        LogTraceLevel.Error,
                        "CreditMemo.UpdateCreditMemo");
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Updates credit memo payment.
        /// </summary>
        /// <param name="voided"></param>
        /// <param name="comment"></param>
        /// <param name="creditMemoNumber"></param>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "3", Justification = "Grandfather")]
        public void VoidCreditMemoPayment(ref bool voided, ref string comment, string creditMemoNumber, IRetailTransaction retailTransaction)
        {
            try
            {
                LogMessage("Cancelling the used marking of the credit memo...",
                    LogTraceLevel.Trace,
                    "CreditMemo.VoidCreditMemoPayment");

                // Begin by checking if there is a connection to the Transaction Service
                this.Application.TransactionServices.CheckConnection();

                try
                {
                    this.Application.TransactionServices.VoidCreditMemoPayment(ref voided, ref comment, creditMemoNumber,
                        retailTransaction.StoreId,
                        retailTransaction.TerminalId);
                }
                catch (Exception x)
                {
                    LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                    throw;
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Gets credit memo balance.
        /// </summary>
        /// <param name="creditMemoNumber"></param>
        /// <param name="balance"></param>
        public void GetCreditMemoBalance(string creditMemoNumber, ref decimal balance)
        {
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

        #endregion
    }
}
