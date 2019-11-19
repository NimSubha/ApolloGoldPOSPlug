/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Interface that extends IFiscalPrinterDriver to add Russian specific methods.
    /// </summary>
    public abstract partial class RussianFiscalPrinterDriver : IFiscalPrinterDriver
    {
        #region Ctor
        protected RussianFiscalPrinterDriver()
        {
            ResetControlsVariables();
        }
        #endregion

        #region Methods specific to Russian printer driver

        /// <summary>
        /// Gets the printer internal register number used for retrieving the next receipt number.
        /// </summary>
        public abstract int ReceiptOperationRegisterNumber
        {
            get;
        }

        /// <summary>
        /// Prints receipt for starting amount declaration.
        /// </summary>
        /// <param name="total">The starting amount.</param>
        /// <param name="message">The message to be printed in the receipt.</param>
        public abstract void PrintStartingAmount(decimal total, string message);

        /// <summary>
        /// Queries the printer for printing readiness status.
        /// </summary>
        /// <returns>A <see cref="PrintingReadinessStatus">status</see> reflecting the printer readiness for printing.</returns>
        public abstract PrintingReadinessStatus GetPrintingReadinessStatus();

        /// <summary>
        /// Gets succsess result code.
        /// </summary>
        public abstract int SuccessResultCode
        { get; }

        /// <summary>
        /// Gets error code description for the last executed command.
        /// </summary>
        public abstract string ResultCodeDescription
        { get; }

        /// <summary>
        /// Gets the printer 'Not connected' error codes set.
        /// </summary>
        public abstract ISet<int> PrinterIsNotConnectedErrorCodes
        {
            get;
        }

        /// <summary>
        /// Gets an error code integer value, indicating that the shift is open more than 24 hours.
        /// </summary>
        public abstract int ShiftIsOpenMoreThan24HoursErrorCode
        {
            get;
        }

        /// <summary>
        /// Resumes the interrupted printing operation.
        /// </summary>
        public abstract void ResumePrinting();

        /// <summary>
        /// Checks if the printer has an opened period more than 24 hours.
        /// </summary>
        public abstract bool IsDayOpenedMoreThan24Hours();

        /// <summary>
        /// Feeds the printer ribbon.
        /// </summary>
        public abstract void RibbonFeed();

        /// <summary>
        /// Prints a copy of last fiscal document
        /// </summary>
        public abstract void PrintLastReceiptCopy();

        /// <summary>
        /// Retrieves the tax mapping dictionary where key is the POS tax code and value is printer tax Id
        /// </summary>
        /// <returns>The tax mapping dictionary</returns>
        public abstract IDictionary<string, string> GetTaxMapping();

        /// <summary>
        /// Retrieves the tax amounts dictionary from the printer, where key is printer tax Id and value is tax amount
        /// </summary>
        /// <returns>The tax amounts dictionary</returns>
        /// <remarks>If fiscal printer does not support retrieving of calculated tax amounts, overrided method should return null</remarks>
        public abstract IDictionary<string, decimal> GetTaxAmounts();

        /// <summary>
        /// Gets a Dictionary, mapping POS tender types IDs to fiscal printer payment types.
        /// </summary>
        /// <returns>The tender types mapping dictionary.</returns>
        public abstract IDictionary<string, string> GetTenderTypesMapping();

        /// <summary>
        /// Determines whether the payment type is Cash payment type.
        /// </summary>
        /// <param name="paymentTypeId">
        /// Printer payment type ID.
        /// </param>
        /// <returns>
        /// true if the given <paramref name="paymentTypeId"/> is Cash; false otherwise.
        /// </returns>
        public abstract bool IsCashPaymentType(string paymentTypeId);

        /// <summary>
        /// Starts the payment stage in a receipt
        /// </summary>
        public abstract void StartTotalPayment();

        /// <summary>
        /// Prints totals header section.
        /// </summary>
        /// <param name="receiptType">The receipt type.</param>
        /// <param name="retailTransaction">Retail transaction.</param>
        public abstract void PrintTotalsHeader(ReceiptType receiptType, IRetailTransaction retailTransaction);

        /// <summary>
        /// Retrieves the list of totals header discount lines data from the printer for tax adjustments distribution.
        /// </summary>
        /// <returns>The list of TotalsHeaderDiscountLine.</returns>
        /// <remarks>If the fiscal printer does not support retrieving of calculated tax amounts, the overriden method should return null.</remarks>
        public abstract IList<ITotalsHeaderDiscountLine> GetTotalsHeaderDiscountLines();

        /// <summary>
        /// Retrieves the discount sales lines dictionary from the printer, 
        /// where key is printer tax IDs set (printer tax group), value - list of sales lines.
        /// </summary>
        /// <returns>The tax amounts dictionary</returns>
        /// <remarks>If the fiscal printer does not support retrieving of calculated tax amounts, the overrided method should return null.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Design")]
        public abstract IDictionary<string, IList<ISaleLineItem>> GetDiscountSalesLines();

        /// <summary>
        /// Prints the loyalty card balance.
        /// </summary>
        /// <param name="loyaltyCardData">The <see cref="ILoyaltyCardData"/> object containing the information about the loyalty card.</param>
        public abstract void PrintLoyaltyCardBalance(ILoyaltyCardData loyaltyCardData);

        /// <summary>
        /// Retrieves the fiscal printer memory data.
        /// </summary>
        /// <param name="transactionType">Type of transaction.</param>
        /// <returns>The fiscal memory data.</returns>
        public abstract FiscalMemoryData RetrieveFiscalMemoryData(PosTransaction.TypeOfTransaction transactionType);

        /// <summary>
        /// Retrieves the fiscal printer shift ID.
        /// </summary>
        /// <param name="transactionType">Type of transaction.</param>
        /// <returns>The fiscal printer shift ID.</returns>
        public abstract string GetShiftId(PosTransaction.TypeOfTransaction transactionType);

        /// <summary>
        /// Should the gift card payment be processed as discount by the fiscal printer.
        /// </summary>
        /// <returns>True if gift card payment should be processed as discount; otherwise false.</returns>
        public abstract bool ProcessGiftCardPaymentAsDiscount();
        #endregion

        #region Russian fiscal printer control flags
        /// <summary>
        /// Gets or sets a value indicating that the fiscal printer is in 'Not connected' mode.
        /// </summary>
        protected bool IsNotConnected { get; set; }

        /// <summary>
        /// Resets all fiscal printer control flags.
        /// </summary>
        /// <remarks>
        /// This method resets all the fiscal printer driver control flags.
        /// Call this method before starting new operation sequence handling.
        /// </remarks>
        protected void ResetControlsVariables()
        {
            IsNotConnected = false;
            IrreversibleCommandSent = false;
        }
        #endregion

        #region Fiscal memory data
        private readonly Lazy<FiscalMemoryData> lazyFiscalMemoryData = new Lazy<FiscalMemoryData>();

        /// <summary>
        /// Gets the fiscal memory data.
        /// </summary>
        protected FiscalMemoryData FiscalMemoryData { get { return lazyFiscalMemoryData.Value; } }
        #endregion

        #region Abstract methods

        public abstract FiscalPrinterState OperatingState
        {
            get;
        }

        public abstract bool SuppressErrorMessages
        {
            get;
            set;
        }

        public abstract void ShowPrinterConfigurationWindow();

        public abstract void Initialize(params object[] args);

        public abstract void SetInitializing();

        public abstract DateTime GetDateTime();

        public abstract decimal GetCurrentZReportCount();

        public abstract decimal GetLastZReportCount();

        public abstract FiscalPrinterStatus GetStatus();

        public abstract IDictionary<string, decimal> GetTaxRates();

        public abstract string GetSerialNumber();

        public abstract decimal GetGrandTotal();

        public abstract string GetTerminalNumber();

        public abstract string GetInternalFeatureStatus(int featureEnumerator);

        public abstract void CancelOpenedDocument(bool reverseCreditDebitReceipt, bool keepFiscalReceipt);

        public abstract void CancelLastPayment(string paymentMethod, decimal amount);

        public abstract void BeginReceipt(ReceiptType receiptType, params object[] args);

        public abstract void EndReceipt(ReceiptType receiptType, params object[] args);

        public abstract void CancelReceipt(ReceiptType receiptType, params object[] args);

        public abstract decimal GetBalanceDue();

        public abstract int AddItem(ReceiptType receiptType, LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem lineItem, string taxRateId);

        public abstract decimal GetChangeDue();

        public abstract decimal GetSubtotal();

        public abstract void RemoveItem(ReceiptType receiptType, int printerItemNumber);

        public abstract void AddItemPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, int printerItemNumber, decimal amount, string description);

        public abstract void RemoveItemPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, int printerItemNumber);

        public abstract void AddTotalPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, decimal amount, string description);

        public abstract void RemoveTotalPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType);

        public abstract void AddPayment(ReceiptType receiptType, string paymentMethod, decimal paymentValue);

        public abstract void ManagementReportBegin(string title);

        public abstract void ManagementReportPrintLine(string line);

        public abstract void ManagementReportPrint(string text);

        public abstract void ManagementReportEnd();

        public abstract void StartOfDay();

        public abstract bool IsDayOpened();

        public abstract void GenerateZReportAndFiles(string filePath);

        public abstract string GetLastZReport();

        public abstract bool HasZReportPending();

        public abstract void GenerateZReportFiles(System.DateTime startDate, System.DateTime endDate, string fileName);

        public abstract void PrintMoneyDrop(decimal total, string message);

        public abstract void PrintMoneyEntry(decimal total, string message);

        public abstract void CreditDebitReceiptBegin(string tenderType, int installmentCount, string originalDocumentOperationNumber, decimal transactionValue);

        public abstract void CreditDebitReceiptPrintText(string text);

        public abstract void CreditDebitReceiptEnd();

        public abstract void ExecutePaperCut(bool checkReturn);

        public abstract void OpenDrawer();

        public abstract void PrintXReport();

        public abstract string GenerateReport(int reportEnumeration, params object[] args);

        public abstract bool IsPrinterReady();

        public abstract string GetCounterNumber(int counterEnumerator);

        public abstract DateTime GetTransactionDate();

        public abstract DateTime GetAccountingDate();

        #endregion
    }
}
