/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.BarcodeService
{
    using System.ComponentModel.Composition;
    using LSRetailPosis.Transaction;
    using LSRetailPosis.Transaction.Line.SaleItem;
    using Microsoft.Dynamics.Retail.Pos.Contracts;
    using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
    using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

    /// <summary>
    /// Class implementing the interface ICharges
    /// </summary>
    [Export(typeof(ICharges))]
    public class Charges : ICharges
    {
        [Import]
        public IApplication Application { get; set; }

        /// <summary>
        /// Calculates the charges.
        /// </summary>
        /// <param name="transaction ">The retail transaction.</param>
        public void CalculateCharges(IRetailTransaction transaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)transaction;

            // put charges on the transaction lines
            foreach (var salesLine in retailTransaction.SaleItems)
            {
                CalculatePriceCharges(salesLine);
            }
        }

        /// <summary>
        /// Calculates the price charges.
        /// </summary>
        /// <param name="salesLine">The sales line.</param>
        /// <returns>
        /// MiscellaneousCharge.
        /// </returns>
        private static void CalculatePriceCharges(SaleLineItem salesLine)
        {
            // check to see whether this is a price charge or not
            if (salesLine.Markup != 0)
            {
                // there is a price charge associated with the item.
                decimal amount = 0;
                decimal quantity = 1;
    
                // check type price charge 
                if (salesLine.AllocateMarkup)
                {
                    // Per Unit markup
                    if (salesLine.PriceQty == 0M)
                    {
                        // default AX behaviour is that, is PriceQty = 0, it means it is 1 per line
                        quantity = 1M;
                    }
                    else
                    {
                        quantity = salesLine.PriceQty;
                    }

                    amount = salesLine.Markup / quantity;
                }
                else
                {
                    // per line markup
                    amount = salesLine.Markup;
                }

                // there is a price charge associated with this item. need to update the price accordingly.
                salesLine.SalesMarkup = amount;
            }
        }
    }
}
