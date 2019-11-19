/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Linq;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {

        #region Implementation of IItemTriggers

        /// <summary>
        /// Triggered prior to adding the item to the transaction, but after all item properties have been fetched from the database.
        /// Note that the item's dimensions have not been checked.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="posTransaction"></param>
        public void PreSale(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction)
        {
            if (preTriggerResult == null)
            {
                throw new ArgumentNullException("preTriggerResult");
            }

            if (saleLineItem == null)
            {
                throw new ArgumentNullException("saleLineItem");
            }

            //Transactions with both return and sale operations are not allowed if fiscal printer is connected
            var retailTransaction = posTransaction as LSRetailPosis.Transaction.RetailTransaction;
            if (retailTransaction != null)
            {
                if (PosApplication.Instance.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    bool quantityPositive = false, quantityNegative = false;
                    foreach (var saleItem in retailTransaction.SaleItems.Where(si => !si.Voided && si.Quantity != 0))
                    {
                        if (saleItem.Quantity > 0) quantityPositive = true;
                        else if (saleItem.Quantity < 0) quantityNegative = true;
                    }

                    if ((quantityNegative && !quantityPositive && saleLineItem.Quantity > decimal.Zero) ||
                        (quantityPositive && !quantityNegative && saleLineItem.Quantity < decimal.Zero) ||
                        (quantityNegative && quantityNegative))
                    {
                        preTriggerResult.ContinueOperation = false;
                        //"You cannot return and sale items in the same fiscal receipt. Register the sale of items as a separate operation."
                        preTriggerResult.MessageId = 86469;
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Triggered after adding the item to the transaction.
        /// Prices and discounts have been calculated but the event is triggered before 
        /// processing any infocodes or linked items.
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostSale(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }
               
        /// <summary>
        /// Triggered prior to returning a sale line item to the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreReturnItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after voiding a sale line item at the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PostVoidItem(IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered prior to setting the quantity of a sale line item at the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PreSetQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after setting the quantity of a sale line item at the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="saleLineItem"></param>
        public void PostSetQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered prior to clearing the quantity of a sale line item at the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PreClearQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after clearing the quantity of a sale line item at the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="saleLineItem"></param>
        public void PostClearQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered prior to overriding the price of a sale line item at the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="saleLineItem"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PrePriceOverride(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered prior to voiding a sale line item at the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PreVoidItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        #endregion
    }
}
