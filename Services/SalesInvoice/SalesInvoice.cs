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
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SalesInvoiceItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.SalesInvoice
{
    [Export(typeof(ISalesInvoice))]
    public class SalesInvoice : ISalesInvoice
    {

        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for the SalesOrder service are reserved at 57000 - 57999
        // TextID's for the following modules are as follows:
        //
        // SalesInvoice.cs:             57000 - 57099.  The last in use: 57003
        // frmGetSalesInvoice.cs:       57100 - 57199.  the last in use: 57110

        #region ISalesInvoice Members

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application { get; set; }

        /// <summary>
        /// Executes sales invoice functionality.
        /// </summary>
        /// <param name="posTransaction"></param>
        public void SalesInvoices(IPosTransaction posTransaction)
        {
            // The sales invoice functionality is only allowed if a customer has already been added to the transaction
            if (string.IsNullOrEmpty(((RetailTransaction)posTransaction).Customer.Name))
            {
                POSFormsManager.ShowPOSMessageDialog(57000);
                return;
            }

            PaySalesInvoice(posTransaction);
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "GrandFather PS6015")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot pass as ref within using block")]
        private void PaySalesInvoice(IPosTransaction posTransaction)
        {
            try
            {
                DataTable salesInvoices = new DataTable();
                GetSalesInvoicesForCustomer(ref salesInvoices, posTransaction);

                if (salesInvoices.Rows.Count == 0)
                {
                    // There are no sales invoices in the database for this customer....
                    using (LSRetailPosis.POSProcesses.frmMessage messageDialog = new LSRetailPosis.POSProcesses.frmMessage(57001, MessageBoxButtons.OK, MessageBoxIcon.Exclamation))
                    {
                        this.Application.ApplicationFramework.POSShowForm(messageDialog);

                        return;
                    }
                }

                // Show the available sales invoices for selection...
                using (WinFormsTouch.frmGetSalesInvoice salesInvoicesDialog = new global::Microsoft.Dynamics.Retail.Pos.SalesInvoice.WinFormsTouch.frmGetSalesInvoice())
                {
                    salesInvoicesDialog.SalesInvoices = salesInvoices;
                    this.Application.ApplicationFramework.POSShowForm(salesInvoicesDialog);

                    RetailTransaction retailTransaction = posTransaction as RetailTransaction;

                    if (salesInvoicesDialog.SelectedSalesInvoiceId != null && retailTransaction != null)
                    {
                        // Check if this Sales Invoice has already been added to the transaction
                        foreach (ISaleLineItem salesInvoiceInTrans in retailTransaction.SaleItems)
                        {
                            if (salesInvoiceInTrans is SalesInvoiceLineItem)
                            {
                                if (!salesInvoiceInTrans.Voided)
                                {
                                    if (((SalesInvoiceLineItem)salesInvoiceInTrans).SalesInvoiceId == salesInvoicesDialog.SelectedSalesInvoiceId)
                                    {
                                        using (LSRetailPosis.POSProcesses.frmMessage msgDialog = new LSRetailPosis.POSProcesses.frmMessage(57003, MessageBoxButtons.OK, MessageBoxIcon.Error))  // This sales invoice has already been added to the transaction
                                        {
                                            this.Application.ApplicationFramework.POSShowForm(msgDialog);
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        // There is a valid sales invoice selected and it's not been already added to the transaction. So let's get the details for it...
                        SalesInvoiceLineItem salesInvoiceLineItem = (SalesInvoiceLineItem)this.Application.BusinessLogic.Utility.CreateSalesInvoiceLineItem(
                            ApplicationSettings.Terminal.StoreCurrency,
                            this.Application.Services.Rounding, retailTransaction);

                        GetSalesInvoice(posTransaction, ref salesInvoiceLineItem, salesInvoicesDialog.SelectedSalesInvoiceId);

                        // And add it to the transaction
                        retailTransaction.Add(salesInvoiceLineItem);
                        retailTransaction.SalesInvoiceAmounts += salesInvoiceLineItem.Amount;
                    }
                }
            }
            catch (PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                throw;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
            finally
            {
                if (posTransaction is SalesInvoiceTransaction)
                {
                    ((SalesInvoiceTransaction)posTransaction).CalcTotals();
                }
            }
        }

        /// <summary>
        /// Get a list of sales invoices for a specific customer...
        /// </summary>
        /// <returns></returns>
        private void GetSalesInvoicesForCustomer(ref DataTable salesInvoices, IPosTransaction posTransaction)
        {
            LogMessage("Getting the list of sales invoices for a customer",
                LSRetailPosis.LogTraceLevel.Trace,
                "SalesOrder.GetSalesInvoicesForCustomer");

            bool retVal = false;
            string comment = string.Empty;

            try
            {
                // Begin by checking if there is a connection to the Transaction Service
                this.Application.TransactionServices.CheckConnection();

                // Publish the credit memo to the Head Office through the Transaction Services...
                this.Application.TransactionServices.GetSalesInvoiceList(ref retVal, ref comment, ref salesInvoices,
                    ((RetailTransaction)posTransaction).Customer.CustomerId);

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

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "posTransaction", Justification = "Grandfather")]
        private void GetSalesInvoice(IPosTransaction posTransaction, ref SalesInvoiceLineItem salesInvoiceLineItem, string salesInvoiceId)
        {
            try
            {
                bool retval = false;
                string comment = string.Empty;

                string invoiceId = salesInvoiceId;
                decimal totalPaidAmount = 0M;
                decimal totalInvoiceAmount = 0M;
                string customerAccount = string.Empty;
                string customerName = string.Empty;
                DateTime creationDate = new DateTime();

                // Begin by checking if there is a connection to the Transaction Service
                this.Application.TransactionServices.CheckConnection();

                this.Application.TransactionServices.GetSalesInvoice(ref retval, ref comment, ref invoiceId, ref totalPaidAmount, ref totalInvoiceAmount,
                    ref customerAccount, ref customerName, ref creationDate);


                // Populate the salesInvoiceLineItem with the respective values...
                salesInvoiceLineItem.SalesInvoiceId = invoiceId;
                salesInvoiceLineItem.Description = LSRetailPosis.ApplicationLocalizer.Language.Translate(57002); // Sales Invoice
                salesInvoiceLineItem.CreationDate = creationDate;
                salesInvoiceLineItem.Amount = totalInvoiceAmount - totalPaidAmount;  // The balance/remainder of the sales invoice

                // Necessary property settings for the the sales invoice "item"...
                salesInvoiceLineItem.Price = salesInvoiceLineItem.Amount;
                salesInvoiceLineItem.StandardRetailPrice = salesInvoiceLineItem.Amount;
                salesInvoiceLineItem.Quantity = 1;
                salesInvoiceLineItem.TaxRatePct = 0;
                salesInvoiceLineItem.Comment = salesInvoiceLineItem.SalesInvoiceId;
                salesInvoiceLineItem.NoDiscountAllowed = true;
                salesInvoiceLineItem.Found = true;
            }
            catch (PosisException px)
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
