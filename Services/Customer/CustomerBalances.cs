/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Xml.Serialization;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    /// <summary>
    /// Class to hold customer balances and credit limits.
    /// If the customer has an invoice account it will include the values for that account too.
    /// </summary>
    [Serializable]
    [XmlRoot("CustomerBalances")]
    public class CustomerBalances
    {
        /// <summary>
        /// Balance on the account as returned by AX
        /// </summary>
        [XmlElement("Balance")]
        public decimal Balance { get; set; }

        /// <summary>
        /// Local balances of customer account not yet synced to AX.
        /// </summary>
        [XmlElement("LocalPendingBalance")]
        public decimal LocalPendingBalance { get; set; }

        /// <summary>
        /// Credit limit of the account as returned by AX
        /// </summary>
        [XmlElement("CreditLimit")]
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Balance on the invoice account (if any) as returned by AX
        /// </summary>
        [XmlElement("InvoiceAccountBalance")]
        public decimal InvoiceAccountBalance { get; set; }

        /// <summary>
        /// Local balances related to invoice account not yet synced to AX.
        /// </summary>
        [XmlElement("LocalInvoicePendingBalance")]
        public decimal LocalInvoicePendingBalance { get; set; }

        /// <summary>
        /// Credit limit of the invoice account (if any) as returned by AX
        /// </summary>
        [XmlElement("InvoiceAccountCreditLimit")]
        public decimal InvoiceAccountCreditLimit { get; set; }

        /// <summary>
        /// Maximum replication counter for the trasaction in AX. 
        /// It helps identify what transactions were not send when calculating local balance.  
        /// </summary>
        [XmlElement("LastReplicatedTransactionCounter")]
        public long LastReplicatedTransactionCounter { get; set; }
    }
}
