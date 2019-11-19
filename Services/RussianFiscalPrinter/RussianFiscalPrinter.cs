/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Settings.HardwareProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using FiscalPrinterProfile = LSRetailPosis.Settings.HardwareProfiles.FiscalPrinter;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// This is the main Russian fiscal printer class.  It holds
    /// all the events called from core POS.
    /// </summary>    
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Design")]
    [Export(typeof(IFiscalPrinter))]
    public sealed partial class RussianFiscalPrinter : IFiscalPrinter
    {
        private readonly static Lazy<IDictionary<string, string>> lazyTaxMapping = new Lazy<IDictionary<string, string>>(() => RussianFiscalPrinterDriver.FiscalPrinterDriver.GetTaxMapping());
        private readonly static Lazy<IDictionary<string, string>> lazyTenderTypesMapping = new Lazy<IDictionary<string, string>>(() => RussianFiscalPrinterDriver.FiscalPrinterDriver.GetTenderTypesMapping());
        private readonly static CultureInfo _targetCulture = CultureInfo.CreateSpecificCulture("ru-RU");

        /// <summary>
        /// Current receipt ID accessor.
        /// </summary>
        public static string CurrentReceiptId
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Design")]
        public string DeviceName
        {
            get;
            private set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Design")]
        public string DeviceDescription
        {
            get;
            private set;
        }

        public bool IsActive
        {
            get { return CanBeInitialized() && FiscalPrinterEnabled(); }
        }

        public bool Initialized
        {
            get;
            private set;
        }

        public bool IsPrintingCommandSent
        {
            get { return RussianFiscalPrinterDriver.IrreversibleCommandSent; }
        }

        public bool CanBeInitialized()
        {
            return SupportedCountryRegion.RU == Functions.CountryRegion;
        }

        /// <summary>
        /// Verifies if the fiscal printer extension is enabled 
        /// for the current store.  By default this method returns 
        /// FALSE, therefore Fiscal Printer implementations must
        /// this method and return TRUE.
        /// </summary>
        /// <returns>Returns true if there is a fiscal printer implementation that 
        /// must be called by the extension libraries; otherwise retuns FALSE.</returns>
        public bool FiscalPrinterEnabled()
        {
            // We depend upon the Fiscal printer driver setting in the Fiscal printer hardware profile settings section.
            return FiscalPrinterProfile.DriverType == FiscalPrinterDriver.ManufacturerDriver;
        }

        /// <summary>
        /// Issues a gift card given the current transaction.
        /// </summary>
        /// <param name="posTransaction">Pos transaction</param>
        /// <param name="gcTenderInfo">Tender info</param>
        public void IssueGiftCard(IPosTransaction posTransaction, ITender gcTenderInfo)
        {
            // No specific logic is necessary to support gift cards.
        }

        /// <summary>
        /// Add to gift card given the current transaction.
        /// </summary>
        /// <param name="retailTransaction">Retail transaction</param>
        /// <param name="gcTenderInfo">Tender info</param>
        public void AddToGiftCard(IRetailTransaction retailTransaction, ITender gcTenderInfo)
        {
            // No specific logic is necessary to support gift cards.
        }

        /// <summary>
        /// Initializes a new Instance of the <see cref="FiscalPrinter">class</see>.
        /// </summary>
        internal RussianFiscalPrinter()
        {
            // Reset this control flag to maintain proper initial state.
            IsPrintingSlipDocument = false;
        }

        /// <summary>
        /// Tries to initialize the class instance.
        /// </summary>
        public void Initialize()
        {
            if (Initialized)
            {
                return;
            }

            // Verify the store address is in Russia
            if (Functions.CountryRegion != SupportedCountryRegion.RU)
            {
                ExceptionHelper.ThrowException(Resources.MessageCountryStoreRussia, ApplicationSettings.Terminal.StoreCountry);
            }

            Initialized = true;
        }

        /// <summary>
        /// Tries to initialize the printer driver, connect to the 
        /// serial port, verifies the printer state, paper status 
        /// and recover any pending transaction
        /// </summary>
        public void InitializePrinter()
        {
            FiscalPrinterDriverFactory.FiscalPrinterDriver.Initialize(
                Properties.Settings.Default.SerialConnectionString,
                Properties.Settings.Default.SerialConnectionBaudRate,
                Properties.Settings.Default.SerialConnectionTimeout,
                Properties.Settings.Default.LogFileName);
        }

        /// <summary>
        /// Denotes whether the fiscal printer supports simultaneous printing on normal printers.
        /// </summary>
        public bool SupportNormalPrinters
        {
            get { return true; }
        }

        /// <summary>
        /// Checks if the printer is ready, Z report is not pending 
        /// and day is opened.
        /// </summary>
        /// <returns></returns>
        private static bool IsPrinterReady()
        {
            return FiscalPrinterDriverFactory.FiscalPrinterDriver.IsPrinterReady();
        }

        /// <summary>
        /// Retrieves the next receipt ID to use in the fiscal coupon.
        /// </summary>
        /// <param name="transaction">The PosTransaction that sets the context.</param>
        /// <returns>The string representing the receipt ID.</returns>
        /// <remarks>Delegates the retrieval of new receipt ID to the Application Service.</remarks>
        public string GetNextReceiptId(IPosTransaction transaction)
        {
            var retailTransaction = transaction as RetailTransaction;
            if (retailTransaction != null && retailTransaction.SaleIsReturnSale && !ApplicationSettings.Terminal.ProcessReturnsAsInOriginalSaleShift_RU)
            {
                var originalTransaction = new TransactionData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, PosApplication.Instance).GetOriginalRetailTransaction(retailTransaction);
                if (originalTransaction != null)
                {
                    bool? hasSameShiftId = Instance.HasSameShiftId(originalTransaction);
                    // Return empty receipt ID string for return transactions registered in a shift diffrent from the original transaction's shift as they are not treated as fiscal transactions.
                    if ((hasSameShiftId.HasValue && !hasSameShiftId.Value)
                        || (originalTransaction.Shift != null && originalTransaction.Shift.BatchId != PosApplication.Instance.Shift.BatchId))
                        return string.Empty;
                }
            }
            return CurrentReceiptId ?? (CurrentReceiptId = PosApplication.Instance.Services.ApplicationService.GetNextReceiptId(transaction));
        }

        /// <summary>
        /// Determines whether the fiscal printer supports numbering function.
        /// </summary>
        /// <returns>Returns true if the numbering function is supported.</returns>
        public bool HasReceiptIdNumbering()
        {
            return CurrentReceiptId != null;
        }

        /// <summary>
        /// Checks if Fiscal printer shift ID is equal to POS's.
        /// </summary>
        /// <param name="transaction">A Retail transaction.</param>
        /// <returns>Returns true if the fiscal printer shift ID is equal to POS's.</returns>
        public bool? HasSameShiftId(IRetailTransaction transaction)
        {
            var sqlConnection = ApplicationSettings.Database.LocalConnection;

            try
            {
                string fiscalPrinterShiftId = RussianFiscalPrinterDriver.FiscalPrinterDriver.GetShiftId(((PosTransaction)transaction).TransactionType);
                string transactionFiscalShiftId = new TransactionFiscalData(sqlConnection, ApplicationSettings.Database.DATAAREAID).GetFiscalShiftId(transaction);

                if (!string.IsNullOrEmpty(fiscalPrinterShiftId) && !string.IsNullOrEmpty(transactionFiscalShiftId))
                {
                    return fiscalPrinterShiftId.Equals(transactionFiscalShiftId);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                // Do not throw any exceptions caught while reading shift IDs to not break the operation.
                return null;
            }
        }

        #region Private methods
        /// <summary>
        /// Handles printing of a receipt.
        /// </summary>
        /// <param name="retailTransaction">The RetailTransaction representing the sale/return operation.</param>
        private static void PrintFiscalDocument(RetailTransaction retailTransaction)
        {
            try
            {
                BeginFiscalDocument(retailTransaction);

                foreach (var line in retailTransaction.SaleItems)
                {
                    if (line.Voided)
                    {
                        continue;
                    }

                    PrintLine(line, retailTransaction);
                    UpdateTaxLines(line);
                }

                PrintTotalsHeader(retailTransaction);

                DiscountHelper.DistributeTaxFromTotalsHeaderDiscounts();

                ValidateTotalAmount(retailTransaction);

                PrintTenderLines(retailTransaction);

                EndFiscalDocument(retailTransaction);
            }
            catch (FiscalPrinterException)
            {
                // We do not want to handle FiscalPrinterException here, we want to enforce standard handling defined up the call stack.
                throw;
            }
            catch (Exception ex)
            {
                ExceptionHelper.ThrowException(Resources.ErrorWhilePrinting, ex.Message);
            }
        }

        /// <summary>
        /// Prints totals header section.
        /// </summary>
        /// <param name="retailTransaction">The RetailTransaction representing the current operation.</param>
        private static void PrintTotalsHeader(RetailTransaction retailTransaction)
        {
            RussianFiscalPrinterDriver.FiscalPrinterDriver.PrintTotalsHeader(GetReceiptType(retailTransaction), retailTransaction);
        }

        /// <summary>
        /// Prints tender lines information.
        /// </summary>
        /// <param name="retailTransaction">The RetailTransaction representing the current operation.</param>
        private static void PrintTenderLines(RetailTransaction retailTransaction)
        {
            var paymentMethodAmounts = new Dictionary<string, decimal>(StringComparer.Create(_targetCulture, true));
            decimal paidAmountInCash = 0m;
            decimal totalPaidAmount = 0m;
            decimal totalPaidAmountExceptCash;

            RussianFiscalPrinterDriver.FiscalPrinterDriver.StartTotalPayment();

            foreach (var tenderLine in retailTransaction.TenderLines.Where<TenderLineItem>(
                t => !t.ChangeBack &&
                     !t.Voided &&
                     (!ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments ||
                      !RussianFiscalPrinterDriver.FiscalPrinterDriver.ProcessGiftCardPaymentAsDiscount() || 
                      !(t is IGiftCardTenderLineItem))))
            {
                string printerPaymentTypeId = GetPrinterPaymentTypeIdFromTenderTypeId(tenderLine.TenderTypeId);

                if (RussianFiscalPrinterDriver.FiscalPrinterDriver.IsCashPaymentType(printerPaymentTypeId))
                {
                    paidAmountInCash += tenderLine.Amount;
                }

                decimal amount;

                if (paymentMethodAmounts.TryGetValue(printerPaymentTypeId, out amount))
                {
                    amount += tenderLine.Amount;
                    paymentMethodAmounts[printerPaymentTypeId] = amount;
                }
                else
                {
                    paymentMethodAmounts.Add(printerPaymentTypeId, tenderLine.Amount);
                }

                totalPaidAmount += tenderLine.Amount;
            }

            totalPaidAmountExceptCash = totalPaidAmount - paidAmountInCash;
            if (Math.Abs(totalPaidAmountExceptCash) > Math.Abs(retailTransaction.AmountDue) && Math.Sign(totalPaidAmountExceptCash) == Math.Sign(retailTransaction.AmountDue))
            {
                ExceptionHelper.ThrowException(true, Resources.TotalNoncashAmountExceedsOrderAmount);
            }

            foreach (var payment in paymentMethodAmounts)
            {
                FiscalPrinterDriverFactory.FiscalPrinterDriver.AddPayment(GetReceiptType(retailTransaction), payment.Key, payment.Value);
            }
        }

        /// <summary>
        /// Validates that printer total amount is equal to POS total amount.
        /// </summary>
        /// <param name="retailTransaction">The Retail transaction.</param>
        private static void ValidateTotalAmount(RetailTransaction retailTransaction)
        {
            var printerSubTotal = FiscalPrinterDriverFactory.FiscalPrinterDriver.GetSubtotal();
            var giftCardPaidAmount = ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments &&
                                     RussianFiscalPrinterDriver.FiscalPrinterDriver.ProcessGiftCardPaymentAsDiscount() ? 
                GiftCardHelper.GetTransactionGiftCardPaidAmount(retailTransaction) : 0;

            if (Math.Abs(retailTransaction.AmountDue - retailTransaction.RoundingSalePmtDiff - giftCardPaidAmount) != Math.Abs(printerSubTotal))
            {
                ExceptionHelper.ThrowException(Resources.PrinterTotalAmountNotEqualPOSTotalAmount, printerSubTotal);
            }
        }

        /// <summary>
        /// Performs BeginReceipt operation.
        /// </summary>
        /// <param name="retailTransaction">The current RetailTransaction.</param>
        private static void BeginFiscalDocument(RetailTransaction retailTransaction)
        {
            CurrentReceiptId = null;
            FiscalPrinterDriverFactory.FiscalPrinterDriver.BeginReceipt(GetReceiptType(retailTransaction), retailTransaction);
        }

        /// <summary>
        /// Performs EndReceipt operation.
        /// </summary>
        /// <param name="retailTransaction">The current RetailTransaction.</param>
        private static void EndFiscalDocument(RetailTransaction retailTransaction)
        {
            FiscalPrinterDriverFactory.FiscalPrinterDriver.EndReceipt(GetReceiptType(retailTransaction), retailTransaction);
        }

        /// <summary>
        /// Determines receipt type for retail transaction
        /// </summary>
        /// <param name="retailTransaction">The Retail transaction.</param>
        /// <returns>The type of the receipt.</returns>
        private static ReceiptType GetReceiptType(RetailTransaction retailTransaction)
        {
            var type = ReceiptType.FiscalReceipt;

            bool negativeQuantityOrdered = false;
            
            if (retailTransaction.SaleItems != null)
            {
                var firstValidlSaleItem = retailTransaction.SaleItems.Where(i => i != null && !i.Voided).FirstOrDefault();
                if (firstValidlSaleItem != null)
                {
                    negativeQuantityOrdered = firstValidlSaleItem.Quantity < 0;
                }
                else
                {
                    throw new InvalidOperationException("retailTransaction.SaleItems doesn't contain valid items");
                }
            }
            else
            {
                throw new InvalidOperationException("retailTransaction.SaleItems is null");
            }

            if (negativeQuantityOrdered)
            {
                type = ReceiptType.RegularRefund;
            }

            return type;
        }

        /// <summary>
        /// Prints the receipt line item.
        /// </summary>
        /// <param name="line">The Receipt line item to print.</param>
        /// <param name="retailTransaction">The Retail transaction.</param>
        private static void PrintLine(BaseSaleItem line, RetailTransaction retailTransaction)
        {
            FiscalPrinterDriverFactory.FiscalPrinterDriver.AddItem(GetReceiptType(retailTransaction), (SaleLineItem)line, GetTaxCodes(line));
        }

        /// <summary>
        /// Builds a string with printer tax IDs that the Russian fiscal printer driver understands.
        /// </summary>
        /// <remarks>
        /// TaxMappingIsNotFound exception is thrown in case a tax code is used in the line but it
        /// is not found in tax mapping.
        /// MultipleTaxMapping exception is thrown in case a two or more tax codes of same sales line
        /// are mapped to the same printer tax ID.
        /// </remarks>
        /// <param name="item">An object representing sale line.</param>
        /// <returns>Tax codes separated by space.</returns>
        private static string GetTaxCodes(BaseSaleItem item)
        {
            var taxCodes = new HashSet<string>();
            var resultString = new StringBuilder();

            foreach (var tax in item.TaxLines)
            {
                var taxId = GetPrinterTaxIdFromTaxCode(tax.TaxCode);
                if (!taxCodes.Contains(taxId))
                {
                    taxCodes.Add(taxId);
                    resultString.Append(taxId + " ");
                }
                else
                {
                    throw new Exception(Resources.Translate(Resources.MultipleTaxMapping));
                }
            }

            return resultString.ToString();
        }

        /// <summary>
        /// Update tax lines with amounts from fiscal printer
        /// </summary>
        /// <param name="item">
        /// An object representing a sale line.
        /// </param>
        private static void UpdateTaxLines(BaseSaleItem item)
        {
            var taxAmounts = RussianFiscalPrinterDriver.FiscalPrinterDriver.GetTaxAmounts();

            if (taxAmounts == null)
            {
                return;
            }

            int signOfSaleItemQty = Math.Sign(item.Quantity);

            foreach (var tax in item.TaxLines)
            {
                var printerTaxCodeId = GetPrinterTaxIdFromTaxCode(tax.TaxCode);
                var printerTaxAmount = taxAmounts[printerTaxCodeId];
                if (printerTaxAmount != decimal.Zero)
                {
                    tax.Amount = signOfSaleItemQty * Math.Abs(printerTaxAmount);
                }
            }
        }

        /// <summary>
        /// Tries to retrieve a printer tax ID based on the POS tax code provided.
        /// </summary>
        /// <param name="taxCode">The POS tax code.</param>
        /// <param name="taxId">An output parameter, used to return the mapped printer tax ID.</param>
        /// <returns>A Boolean denoting whether the operation was successful.</returns>
        private static bool TryGetPrinterTaxIdFromTaxCode(string taxCode, out string taxId)
        {
            return lazyTaxMapping.Value.TryGetValue(taxCode.ToUpperInvariant(), out taxId);
        }

        /// <summary>
        /// Gets the printer tax ID for the POS tax code.
        /// </summary>
        /// <param name="taxCode">POS Tax code.</param>
        /// <returns>Printer tax ID.</returns>
        /// <remarks>
        /// Throws <see cref="FiscalPrinterException"/> if the given POS tax code does not have a matching printer tax ID.
        /// </remarks>
        internal static string GetPrinterTaxIdFromTaxCode(string taxCode)
        {
            string result;

            if (!TryGetPrinterTaxIdFromTaxCode(taxCode, out result))
            {
                throw new Exception(Resources.Translate(Resources.TaxMappingIsNotFound, taxCode));
            }

            return result;
        }

        /// <summary>
        /// Tries to retrieve a printer payment type ID based on the POS tender type ID provided.
        /// </summary>
        /// <param name="tenderTypeId">The POS tender type ID.</param>
        /// <param name="paymentTypeId">An output parameter, used to return the mapped printer payment type ID.</param>
        /// <returns>A Boolean denoting whether the operation was successful.</returns>
        private static bool TryGetPrinterPaymentTypeIdFromTenderTypeId(string tenderTypeId, out string paymentTypeId)
        {
            return lazyTenderTypesMapping.Value.TryGetValue(tenderTypeId, out paymentTypeId);
        }

        /// <summary>
        /// Gets the printer payment type ID from the POS tender type ID.
        /// </summary>
        /// <param name="tenderTypeId">POS tender type ID.</param>
        /// <returns>Printer payment type ID.</returns>
        /// <remarks>
        /// Throws <see cref="FiscalPrinterException"/> if the given POS tender type ID does not have a matching printer payment type ID.
        /// </remarks>
        private static string GetPrinterPaymentTypeIdFromTenderTypeId(string tenderTypeId)
        {
            string result;

            if (!TryGetPrinterPaymentTypeIdFromTenderTypeId(tenderTypeId, out result))
            {
                throw new Exception(Resources.Translate(Resources.TenderTypeIsNotSupported, tenderTypeId));
            }

            return result;
        }

        #endregion

        #region Static methods

        public static RussianFiscalPrinter Instance
        {
            get
            {
                return (RussianFiscalPrinter)PosApplication.Instance.Services.Peripherals.FiscalPrinter;
            }
        }

        #endregion
    }
}
