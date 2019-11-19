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

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Helper class for printing receipts paid by gift cards.
    /// </summary>
    public static class GiftCardHelper
    {
        /// <summary>
        /// Gets retail transaction amount paid by gift cards.
        /// </summary>
        /// <param name="_transaction">Retail transaction</param>
        public static decimal GetTransactionGiftCardPaidAmount(IRetailTransaction transaction)
        {
            var retailTransaction = transaction as RetailTransaction;

            if (retailTransaction == null)
            {
                return decimal.Zero;
            }

            return retailTransaction.TenderLines.OfType<IGiftCardTenderLineItem>().Where(t => !t.Voided).Sum(t => t.Amount);
        }
    }
}
