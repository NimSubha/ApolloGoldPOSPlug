/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using LSRetailPosis.Transaction;
    using LSRetailPosis.Transaction.Line.TenderItem;

    /// <summary>
    /// Class that extends the RetailTransaction using the PartnerObject property.
    /// </summary>
    [Serializable]
    public sealed class RetailTransactionExtension
    {
        /// <summary>
        /// Gets or sets a value indicating whether the parent RetailTransaction will generate a Fiscal document model 2.
        /// </summary>
        /// <value><c>true</c> if it will generate a Fiscal document model 2; otherwise, <c>false</c>.</value>
        public bool IsFiscalDocumentModel2 { get; set; }
        /// <summary>
        /// Gets or sets the receipt serie of the Fiscal document model 2 (which does not depend on the Fiscal printer).
        /// </summary>
        /// <value>The serie.</value>
        public string FiscalDocumentModel2Series { get; set; }
        /// <summary>
        /// Gets or sets the invoice number of the Fiscal document model 2 (which does not depend on the Fiscal printer).
        /// </summary>
        /// <value>The invoice number.</value>
        public string FiscalDocumentModel2Number { get; set; }
        /// <summary>
        /// Gets or sets the customer CNPJ/CPF number to be printed in the receipt.
        /// </summary>
        /// <value>The customer CNPJ/CPF number.</value>
        public string ReceiptCustomerCnpjCpfNumber { get; set; }
        /// <summary>
        /// Gets or sets the customer name to be printed in the receipt.
        /// </summary>
        /// <value>The customer name.</value>
        public string ReceiptCustomerName { get; set; }
        /// <summary>
        /// Gets or sets the customer address to be printed in the receipt.
        /// </summary>
        /// <value>The customer address.</value>
        public string ReceiptCustomerAddress { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the RetailTransaction had a previous total discount.
        /// </summary>
        /// <value><c>true</c> if it had a previous total discount; otherwise, <c>false</c>.</value>
        public bool HasPreviousTotalDiscount { get; set; }
        /// <summary>
        /// Gets or sets the fiscal document type the retail transaction will be printed as.
        /// </summary>
        /// <value>The fiscal document type.</value>
        public ReceiptType ReceiptType { get; set; }
        /// <summary>
        /// Gets or sets a generic object that partners can create to extend the RetailTransaction.
        /// </summary>
        /// <value>Any object created by the partner that is marked as Serializable.</value>
        public object PartnerObject { get; set; }
        /// <summary>
        /// Gets or sets the sales line item data.  This provides a centralized location for
        /// keeping track of additional "tag along" data with the transacation sales line items.
        /// </summary>
        /// <value>The sales line item data.</value>
        public Dictionary<int, LineItemTagalong> SalesLineItemData { get; private set; }
        /// <summary>
        /// Gets or sets the tender lines sent to the printer.
        /// </summary>
        public Collection<TenderLineItem> TenderSentToPrinter { get; private set; }
        /// <summary>
        /// Gets or sets the card payment prize redeem amount.
        /// </summary>
        public decimal CardPaymentPrizeRedeemAmount { get; set; }
        /// <summary>
        /// Gets or sets if the last logoff was forced.
        /// </summary>
        public bool LastLogOffWasForced { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the fiscal document model 2 is linked to fiscal receipt.
        /// </summary>
        public bool IsFiscalDocumentModel2Linked { get; set; }

        public RetailTransactionExtension()
        {
            SalesLineItemData = new Dictionary<int, LineItemTagalong>();
            TenderSentToPrinter = new Collection<TenderLineItem>();
        }

        /// <summary>
        /// Recovers the <see cref="RetailTransactionExtension"/> object associate with the given <see cref="RetailTransaction"/>. 
        /// </summary>
        /// <param name="retailTransaction">The given <see cref="RetailTransaction"/> object.</param>
        /// <returns>An instance of <see cref="RetailTransactionExtension"/>. If there is none, create a new instance.</returns>
        public static RetailTransactionExtension GetExtension(RetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                return null;
            }

            if (!(retailTransaction.PartnerObject is RetailTransactionExtension))
            {
                retailTransaction.PartnerObject = new RetailTransactionExtension();
            }

            return retailTransaction.PartnerObject as RetailTransactionExtension;
        }
    }
}
