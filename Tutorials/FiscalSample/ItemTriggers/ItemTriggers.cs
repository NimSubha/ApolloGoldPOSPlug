using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.IncomeExpenseItem;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.FiscalCore;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.ItemTriggers
{
    /// <summary>
    /// Item Triggers
    /// 
    /// NOTE: Exception handling is not provided for simplicty in the demo. In production level code exception handling should be provided.
    /// </summary>
    [Export(typeof(IItemTrigger))]
    [CLSCompliant(false)]
    public class ItemTrigger : IItemTrigger
    {
        #region IItemTriggers Members

        public void PostPriceOverride(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
        }

        public void PostReturnItem(IPosTransaction posTransaction)
        {
        }

        public void PostSale(IPosTransaction posTransaction)
        {
            Debug.WriteLine("PostSale");

            RetailTransaction retailTransaction = posTransaction as RetailTransaction;

            if (retailTransaction != null)
            {   // We are a retail transaction

                // The Sale Item may not alway be the last item.  
                // If the last has alredy been added, then it is a qty change so consder
                // handling later...
                SaleLineItem saleLineItem = retailTransaction.SaleItems.Last.Value;

                if (saleLineItem != null)
                {
                    if (!(saleLineItem is IncomeExpenseItem))
                    {
                        // Note: Depending upon requriments, we may want to skip fiscal receipt coupons for certain
                        // types of saleLineItems such as Income or Expense account

                        FiscalPrinterSingleton fiscalCore = FiscalPrinterSingleton.Instance;
                        IFiscalOperations fiscalPrinter = fiscalCore.FiscalPrinter;
                        if (fiscalPrinter.OperatingState != FiscalPrinterState.FiscalReceipt)
                        {
                            FiscalPrinterSingleton.Instance.SalesLineItemData.Clear();

                            string customerAccountNumber = UserMessages.RequestCustomerTaxId(string.Empty);
                            fiscalPrinter.BeginReceipt(customerAccountNumber);
                        }


                        if (!fiscalCore.SalesLineItemData.ContainsKey(saleLineItem.LineId))
                        {   // We found at leat one new item (not just a quantity change)
                            fiscalCore.UpdateFiscalCouponSalesItemsQty(retailTransaction);
                        }
                    }
                }
            }

        }

        public void PostSetQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            // NOTE: Does not get invoked for clear quantity.  Nor is SalesLineItem fully populated at this point.  
            // Thus this API will not be of much help for Fiscal Printer.
        }

        public void PostVoidItem(IPosTransaction posTransaction, int lineId)
        {
            IFiscalOperations fiscalPrinter = FiscalPrinterSingleton.Instance.FiscalPrinter;

            if (fiscalPrinter != null && FiscalPrinterSingleton.Instance.SalesLineItemData.ContainsKey(lineId))
            {
                LineItemTagalong tagalong = FiscalPrinterSingleton.Instance.SalesLineItemData[lineId];

                if (!tagalong.Voided)
                {
                    fiscalPrinter.RemoveItem(tagalong.PrinterItemNumber);
                    tagalong.Voided = true;
                }
            }
        }

        public void PrePriceOverride(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            if (preTriggerResult == null)
            {
                throw new ArgumentNullException("preTriggerResult");
            }

            // Do not allow price overrides.  
            preTriggerResult.ContinueOperation = false;
            preTriggerResult.MessageId = 3033; // "This operation is invalid for this type of transaction.
        }

        public void PreReturnItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            if (preTriggerResult == null)
            {
                throw new ArgumentNullException("preTriggerResult");
            }

            // Don't allow returns
            preTriggerResult.ContinueOperation = false;
            
            // Optional - Specify resource ID and to display message box to user
            preTriggerResult.MessageId = 1827;
        }

        public void PreSale(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction)
        {
            Debug.WriteLine("PreSale");

            // NOTE: This is the ideal place to update the fiscal printer for a new sales line item 
            // except for the fact that key data in saleLineItem such as price and tax are not yet 
            // set when this API is invoked.
        }

        public void PreSetQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {  
            // NOTE properties such as saleLineItem.Quantity are not yet set when this trigger is invoked
        }

        public void PreVoidItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
        }


        public void PostClearQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
        }

        public void PreClearQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
        }

        #endregion


    }
}
