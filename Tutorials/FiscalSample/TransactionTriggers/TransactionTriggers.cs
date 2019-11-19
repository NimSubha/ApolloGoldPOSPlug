using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.Discount;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.FiscalCore;
using Microsoft.Dynamics.Retail.Pos.FiscalLog;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;

namespace Microsoft.Dynamics.Retail.Pos.TransactionTriggers
{
    /// <summary>
    /// Transaction Triggers
    /// 
    /// NOTE: Exception handling is not provided for simplicty in the demo. In production level code exception handling should be provided.
    /// </summary>
    [Export(typeof(ITransactionTrigger))]
    [CLSCompliant(false)]
    public class TransactionTrigger : ITransactionTrigger
    {
        private string _posExeMd5Text;

        #region ITransactionTriggers Members

        public void BeginTransaction(IPosTransaction posTransaction)
        {
            FiscalLogSingleton.Instance.BeginTransaction();
            // how to detecect restored transaction (is not available via this hook).
        }

        public void PostEndTransaction(IPosTransaction posTransaction)
        {
            Debug.WriteLine("PostEndTransaction");
        }

        public void PostReturnTransaction(IPosTransaction posTransaction)
        {
            Debug.WriteLine("PostReturnTransaction");
        }

        public void SaveTransaction(IPosTransaction posTransaction, SqlTransaction sqlTransaction)
        {
            Debug.WriteLine("SaveTransaction");
        }

        public void PostVoidTransaction(IPosTransaction posTransaction)
        {
            FiscalPrinterSingleton fiscalCore = FiscalPrinterSingleton.Instance;
            IFiscalOperations fiscalPrinter = fiscalCore.FiscalPrinter;

            if (fiscalPrinter.OperatingState == FiscalPrinterState.FiscalReceipt)
            {
                fiscalPrinter.CancelReceipt();
            }
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces.IFiscalOperations.EndReceipt(System.String)")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "False positive")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False positive")]
        public void PreEndTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            if (preTriggerResult == null)
            {
                throw new ArgumentNullException("preTriggerResult");
            }

            if (posTransaction == null)
            {
                throw new ArgumentNullException("posTransaction");
            }

            Debug.WriteLine("PreEndTransaction");
            bool abortTransaction = false;


            FiscalPrinterSingleton fiscalCore = FiscalPrinterSingleton.Instance;
            IFiscalOperations fiscalPrinter = fiscalCore.FiscalPrinter;
            PersistentPrinterData printerData = fiscalCore.PrinterData;

            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            LinkedList<TenderLineItem> tenderLines = null;
            LinkedList<SaleLineItem> salesLines = null;

            ComputePosExeMD5Text();

            // Note: Other transaction types (CustomerPaymentTransaction) may also apply.
            if (retailTransaction != null)
            {   // We are a retail transaction
                // Add all payments to the fiscal printer
                // Alternative option is to do this as ar result of IPaymentTriggers

                tenderLines = retailTransaction.TenderLines;
                salesLines = retailTransaction.SaleItems;


                if (fiscalPrinter.OperatingState == FiscalPrinterState.FiscalReceipt)
                {
                    Decimal totalDiscountAmount = 0m;
                    Decimal totalDiscountPercent = 0m;

                    // Post any quantity changes to the printer
                    fiscalCore.UpdateFiscalCouponSalesItemsQty(retailTransaction);

                    UpdateFiscalPrinterTransactionData(fiscalCore, fiscalPrinter, salesLines, ref totalDiscountAmount, ref totalDiscountPercent);

                    // Start payment, apply total/transaction discount or surcharge
                    // Note: we are not implementing a surcharge
                    if (totalDiscountPercent != 0)
                    {   // Transaction level % discount
                        fiscalPrinter.StartTotalPaymentWithDiscount((int)(totalDiscountPercent * 100));
                    }
                    else if (totalDiscountAmount != 0)
                    {   // Transaction level amount discount
                        fiscalPrinter.StartTotalPaymentWithDiscount(totalDiscountAmount);
                    }
                    else
                    {   // No transaction level discounts or surcharge
                        fiscalPrinter.StartTotalPayment();
                    }

                    // Process Payments...
                    Decimal posPaymentTotal = 0m;
                    foreach (TenderLineItem tenderLine in tenderLines)
                    {
                        if (!tenderLine.Voided)
                        {
                            string paymentMethod = fiscalCore.MapTenderTypeIdToPaymentMethod(tenderLine.TenderTypeId);
                            decimal paymentAmount = tenderLine.Amount;

                            if (paymentAmount > 0m)
                            {   // only process positive payments with the fiscal printer
                                // Cash-back should is ignored
                                fiscalPrinter.MakePayment(paymentMethod, paymentAmount);

                                posPaymentTotal += paymentAmount;
                            }
                        }
                    }

                    string couponNumber = fiscalPrinter.GetCouponNumber();
                    posTransaction.FiscalDocumentId = couponNumber;
                    string serialNumber = fiscalPrinter.GetSerialNumber();
                    posTransaction.FiscalSerialId = serialNumber;

                    Debug.WriteLine("Balance due: " + fiscalPrinter.GetBalanceDue());
                    Debug.WriteLine("Subtotal " + fiscalPrinter.GetSubtotal());
                    Debug.WriteLine("Pos Payment total " + posPaymentTotal);
                    Debug.Assert(fiscalPrinter.GetBalanceDue() == 0m, "Not enough payment was made as expected by the fiscal printer");

                    if (fiscalPrinter.GetBalanceDue() > 0m)
                    {   // user will need to void transaction or fix the shortage to proceed.
                        preTriggerResult.ContinueOperation = false;

                        preTriggerResult.MessageId = 4042; // The action is not valid for this type of transaction.
                    }
                    else
                    {   // End and finalize the Fiscal Coupon

                        fiscalPrinter.EndReceipt(string.Format(CultureInfo.CurrentCulture, "Thank you! MD5:{0}", _posExeMd5Text));
                        printerData.SetGrandTotal(fiscalPrinter.GetGrandTotal());

                        PrintTenderManagementReports(tenderLines);
                    }
                }
                else
                {
                    // Check to see if there are sales items on this transaction

                    if (ContainsItemsRequiringFiscalPrinter(retailTransaction))
                    {
                        // A Fiscal Coupon has not been created - Abort this operation.
                        preTriggerResult.ContinueOperation = false;

                        preTriggerResult.MessageId = 4042; // The action is not valid for this type of transaction.
                    }
                }

                if (abortTransaction)
                {   // Abort the transaction
                    fiscalPrinter.CancelReceipt();
                    preTriggerResult.ContinueOperation = false;

                    preTriggerResult.MessageId = 4042; // The action is not valid for this type of transaction.
                }

            }
        }

        private static void UpdateFiscalPrinterTransactionData(FiscalPrinterSingleton fiscalCore, IFiscalOperations fiscalPrinter, LinkedList<SaleLineItem> salesLines, ref Decimal totalDiscountAmount, ref Decimal totalDiscountPercent)
        {
            foreach (SaleLineItem lineItem in salesLines)
            {   // Check for changes from what was last sent to fiscal printer...

                if (fiscalCore.SalesLineItemData.ContainsKey(lineItem.LineId))
                {   // We have a tag along
                    LineItemTagalong tagalong = fiscalCore.SalesLineItemData[lineItem.LineId];

                    if (!tagalong.Voided)
                    {
                        // Process line discounts/additional charges...

                        // Consdier if check lineItem.NoDiscountAllowed should be done first
                        foreach (DiscountItem discount in lineItem.DiscountLines)
                        {   // Future enhancment - determine if discounts must be applied in a specific order and
                            // if % discounts are applied to the running total or to the base price
                            // and if multiple % discounts are allowed?

                            if (discount is LineDiscountItem)
                            {
                                if (discount.Amount != 0)
                                {   // Apply amount discount
                                    fiscalPrinter.ApplyDiscount(tagalong.PrinterItemNumber, discount.Amount);
                                }

                                if (discount.Percentage != 0)
                                {   // Apply % discount
                                    fiscalPrinter.ApplyDiscount(tagalong.PrinterItemNumber, (int)(discount.Percentage * 100));
                                }
                            }
                            else if (discount is TotalDiscountItem)
                            {
                                Debug.Assert((totalDiscountAmount == 0) || (totalDiscountAmount == discount.Amount), "TotalDiscountAmount has multiple value");
                                totalDiscountAmount = discount.Amount;

                                Debug.Assert((totalDiscountPercent == 0) || (totalDiscountPercent == discount.Percentage), "totalDiscountPercent has multiple value");
                                totalDiscountPercent = discount.Percentage;
                            }
                        }
                    }
                }
            }
        }

        public void PreReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction, IPosTransaction posTransaction)
        {
            if (preTriggerResult == null)
            {
                throw new ArgumentNullException("preTriggerResult");
            }

            // Don't allow returns
            preTriggerResult.ContinueOperation = false;

            // Optional - Specify resource ID and to display message box to user
            preTriggerResult.MessageId = 3033; // "This operation is invalid for this type of transaction.

            Debug.WriteLine("PreReturnTransaction");
        }

        public void PreVoidTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            Debug.WriteLine("PreVoidTransaction");
        }

        #endregion


        private static bool ContainsItemsRequiringFiscalPrinter(RetailTransaction retailPosTransaction)
        {
            if (retailPosTransaction.EntryStatus == PosTransaction.TransactionStatus.Voided ||
                retailPosTransaction.EntryStatus == PosTransaction.TransactionStatus.Cancelled)
            {   // Ignore any voided or canceled transactions
                return false;
            }

            foreach (SaleLineItem lineItem in retailPosTransaction.SaleItems)
            {
                if (!(lineItem is LSRetailPosis.Transaction.Line.IncomeExpenseItem.IncomeExpenseItem))
                {
                    if (!lineItem.Voided)
                    {   // Sales line items require fiscal printer (for example)
                        return true;
                    }
                }
            }

            return false;
        }

        private static void PrintTenderManagementReports(LinkedList<TenderLineItem> tenderLines)
        {
            FiscalPrinterSingleton fiscalCore = FiscalPrinterSingleton.Instance;
            IFiscalOperations fiscalPrinter = fiscalCore.FiscalPrinter;

            // Print any desired management reports (CC, etc)
            foreach (TenderLineItem tenderLine in tenderLines)
            {
                if (!tenderLine.Voided && (tenderLine.TenderTypeId != "1"))
                {   // filtered out voided or cash

                    if (fiscalPrinter.ManagementReportTryBegin())
                    {
                        fiscalPrinter.ManagementReportPrintLine("================================================");
                        fiscalPrinter.ManagementReportPrintLine("\r\n" + "Tender Payment" + "\r\n");
                        fiscalPrinter.ManagementReportPrintLine("\r\n" + "TenderTypeId: " + tenderLine.TenderTypeId + "\r\n");
                        fiscalPrinter.ManagementReportPrintLine("================================================");
                        fiscalPrinter.ManagementReportEnd();

                    }
                }
            }
        }

        private void ComputePosExeMD5Text()
        {
            FiscalPrinterSingleton fiscalCore = FiscalPrinterSingleton.Instance;

            if (string.IsNullOrEmpty(_posExeMd5Text))
            {   // Get the POS.exe MD5 text value for the first time needed

                byte[] posExeMD5 = fiscalCore.GetPosExeMD5();
                if (posExeMD5 != null)
                {
                    _posExeMd5Text = BitConverter.ToString(posExeMD5).Replace("-", string.Empty);
                }
                else
                {
                    _posExeMd5Text = new string(' ', 34);
                }
            }

        }
    }
}
