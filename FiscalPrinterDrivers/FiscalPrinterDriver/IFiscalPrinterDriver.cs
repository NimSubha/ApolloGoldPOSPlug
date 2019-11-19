/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LSRetailPosis.Transaction.Line.SaleItem;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// Interface that provides the methods available to operate the Fiscal Printer.
    /// </summary>
    [ComImport]
    [Guid("68DD5A38-CC92-4A34-B1AE-881284D5FEF6")]
    public interface IFiscalPrinterDriver
    {
        #region Properties

        /// <summary>
        /// The currenct fiscal printer driver operating state.
        /// </summary>
        FiscalPrinterState OperatingState { get; }

        /// <summary>
        /// Determines if an error message is displayed to the 
        /// user before throwing the FiscalPrinterException exception.
        /// </summary>
        bool SuppressErrorMessages { get; set; }

        #endregion

        #region Initialization and setup operations

        /// <summary>
        /// Show the printer driver configuration window
        /// that allows the user to specify COM port,
        /// baud rate and any other printer setting.
        /// </summary>
        void ShowPrinterConfigurationWindow();

        /// <summary>
        /// Initializes the component, if required.
        /// </summary>
        /// <param name="args">Parameters for the component initialization, if necessary.</param>
        void Initialize(params object[] args);

        /// <summary>
        /// Sets the state to initializing
        /// </summary>
        void SetInitializing();

        #endregion

        #region Get information operations
        /// <summary>
        /// Retrieves the fiscal printer's internal clock date and time.
        /// </summary>
        /// <returns>The current date and time.</returns>
        DateTime GetDateTime();

        /// <summary>
        /// Retrieves the current Z Report count.
        /// </summary>
        /// <returns>The current Z Report count.</returns>
        decimal GetCurrentZReportCount();

        /// <summary>
        /// Retrieves the last Z Report count.
        /// </summary>
        /// <returns>The last Z Report count.</returns>
        decimal GetLastZReportCount();

        /// <summary>
        /// Retrieves the FiscalPrinterStatus flags that indicates the status 
        /// of many fiscal printer sensors.
        /// </summary>
        /// <returns>A bitmask containing all the statuses set.</returns>
        FiscalPrinterStatus GetStatus();

        /// <summary>
        /// Retrieves the tax rates dictionary where key is the tax rate or tax code for predefined taxes
        /// and value is tax index in the fiscal printer memory.
        /// </summary>
        /// <returns>The tax index.</returns>
        IDictionary<string, decimal> GetTaxRates();

        /// <summary>
        /// Retrieves the fiscal printer serial number.
        /// </summary>
        /// <returns>The serial number</returns>
        string GetSerialNumber();

        /// <summary>
        /// Retrieves the fiscal printer grand total.
        /// </summary>
        /// <returns>The grand total.</returns>
        decimal GetGrandTotal();

        /// <summary>
        /// Retrieves the user configurable terminal number associated with the fiscal printer.
        /// </summary>
        /// <returns>The terminal number.</returns>
        string GetTerminalNumber();

        /// <summary>
        /// Retrieves the internal features of the fiscal printer.
        /// </summary>
        /// <param name="featureEnumerator">The index of the internal feature to be retrieved.</param>
        /// <returns>The value of the internal feature.</returns>
        string GetInternalFeatureStatus(int featureEnumerator);

        #endregion

        #region Cancelation
        /// <summary>
        /// Cancels the currently opened document in the printer.  
        /// If there is no opened document, nothing happens.
        /// </summary>
        void CancelOpenedDocument(bool reverseCreditDebitReceipt, bool keepFiscalReceipt);

        /// <summary>
        /// Cancels the last payment line.
        /// </summary>
        void CancelLastPayment(string paymentMethod, decimal amount);

        #endregion

        #region Receipt operations

        /// <summary>
        /// Begin printing a fiscal document.
        /// </summary>
        void BeginReceipt(ReceiptType receiptType, params object[] args);

        /// <summary>
        /// Closes the current fiscal document printing.
        /// </summary>
        void EndReceipt(ReceiptType receiptType, params object[] args);

        /// <summary>
        /// Cancels the [active] receipt.
        /// </summary>
        /// <param name="receiptType">The receipt type.</param>
        /// <param name="args">The args.</param>
        void CancelReceipt(ReceiptType receiptType, params object[] args);

        /// <summary>
        /// Retrieves the fiscal printer balance due.
        /// <remarks>May be invoked after AddTotalPriceAdjustment</remarks>
        /// </summary>
        /// <returns>The balance due.</returns>
        decimal GetBalanceDue();

        /// <summary>
        /// Adds the item.
        /// <remarks>Valid after BeginReceipt and before AddTotalPriceAdjustment</remarks>
        /// </summary>
        /// <param name="receiptType">The receipt type. </param>
        /// <param name="taxRateId">The tax rate id. </param>
        /// <param name="lineItem">The line item. </param>
        /// <returns>The fiscal printer line item number.</returns>
        int AddItem(ReceiptType receiptType, SaleLineItem lineItem, string taxRateId);

        /// <summary>
        /// Retrieves the fiscal printer change due.
        /// <remarks>May be invoked after EndReceipt().</remarks>
        /// </summary>
        /// <returns>The change due.</returns>
        decimal GetChangeDue();

        /// <summary>
        /// Retrieves the fiscal printer subtotal.
        /// </summary>
        /// <returns>The subtotal.</returns>
        decimal GetSubtotal();

        /// <summary>
        /// Removes the item.
        /// <remarks>Valid after BeginReceipt and before AddTotalPriceAdjustment.</remarks>
        /// </summary>
        /// <param name="receiptType">The receipt type.  </param>
        /// <param name="printerItemNumber">The printer item number.</param>
        void RemoveItem(ReceiptType receiptType, int printerItemNumber);

        /// <summary>
        /// Applies the item price adjustment.
        /// <remarks>Valid after BeginReceipt and before AddTotalPriceAdjustment.</remarks>
        /// </summary>
        /// <param name="receiptType">The receipt type. </param>
        /// <param name="adjustmentType">The adjustment type. </param>
        /// <param name="printerItemNumber">The printer item number.</param>
        /// <param name="amount">The adjusment amount. </param>
        /// <param name="description">The description.</param>
        void AddItemPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, int printerItemNumber, decimal amount, string description);

        /// <summary>
        /// Removes the item price adjustment.
        /// <remarks>Valid after BeginReceipt and before AddTotalPriceAdjustment.</remarks>
        /// </summary>
        /// <param name="receiptType">The receipt type.  </param>
        /// <param name="adjustmentType">The adjustment type. </param>
        /// <param name="printerItemNumber">The printer item number.</param>
        void RemoveItemPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, int printerItemNumber);

        /// <summary>
        /// Starts the total payment with total price adjustment.
        /// </summary>
        /// <param name="receiptType">The receipt type.  </param>
        /// <param name="adjustmentType">The adjustment type. </param>
        /// <param name="amount">The adjustment amount. </param>
        /// <param name="description">The description. </param>
        void AddTotalPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, decimal amount, string description);

        /// <summary>
        /// Remove the price adjustment from the total payment.
        /// </summary>
        /// <param name="receiptType">The receipt type. </param>
        /// <param name="adjustmentType">The adjustment to be removed. </param>
        void RemoveTotalPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType);

        /// <summary>
        /// Makes the payment.
        /// <remarks>Valid after AddTotalPriceAdjustment and before EndReceipt.</remarks>
        /// </summary>
        /// <param name="receiptType">The receipt type. </param>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="paymentValue">The payment value.</param>
        void AddPayment(ReceiptType receiptType, string paymentMethod, decimal paymentValue);

        #endregion

        #region Management Reports

        /// <summary>
        /// Begins a managment report.
        /// </summary>
        /// <param name="title">The printed title of the report.</param>
        void ManagementReportBegin(string title);

        /// <summary>
        /// Prints a line on the managment report and
        /// then adds a line feed.
        /// </summary>  
        /// <param name="line">The line to be printed.</param>
        void ManagementReportPrintLine(string line);

        /// <summary>
        /// Prints a text on the managment report 
        /// without advancing to the next line.
        /// </summary>  
        /// <param name="text">The text to be printed.</param>
        void ManagementReportPrint(string text);

        /// <summary>
        /// Ends the management report.
        /// </summary>
        void ManagementReportEnd();

        #endregion

        #region Day opened and Z Report
        /// <summary>
        /// Starts a new fiscal day.
        /// </summary>
        void StartOfDay();

        /// <summary>
        /// Checks if the printer has an opened period to issue fiscal documents.
        /// </summary>
        bool IsDayOpened();        

        /// <summary>
        /// Prints the Z report. If necessary, backs it up to the specified directory.        
        /// </summary>
        /// <param name="filePath">The directory to backup the Electronic Journal file to.</param>
        /// <remarks>Once this is done no additional fiscal coupons may be initiated for the date that the Z report was opened.</remarks>
        void GenerateZReportAndFiles(string filePath);

        /// <summary>
        /// Retrieves the last Z report from the fiscal printer.
        /// </summary>
        /// <returns>A string with the Z report encoded (specific to each manufacturer).</returns>
        string GetLastZReport();

        /// <summary>
        /// Verifies if there is a Z report pending to be printed.
        /// </summary>
        /// <returns>True if there is one pending; false otherwise.</returns>
        bool HasZReportPending();

        /// <summary>
        /// Generates the Sintegra and NFP files for the given date range and stores it on the given file name.
        /// </summary>
        /// <param name="startDate">The start date range.</param>
        /// <param name="endDate">The end date range.</param>
        /// <param name="fileName">The file name that will be generated.</param>
        void GenerateZReportFiles(DateTime startDate, DateTime endDate, string fileName);

        #endregion

        #region Non fiscal operations

        /// <summary>
        /// Prints the non-fiscal receipt for money drops.
        /// </summary>
        /// <param name="total">The amount being removed from POS.</param>
        /// <param name="message">The message to be printed in the non-fiscal receipt.</param>
        void PrintMoneyDrop(decimal total, string message);

        /// <summary>
        /// Prints the non-fiscal receipt for float entries.
        /// </summary>
        /// <param name="total">The amount being added to POS.</param>
        /// <param name="message">The message to be printed in the non-fiscal receipt.</param>
        void PrintMoneyEntry(decimal total, string message);

        #endregion

        #region CCD Operations

        /// <summary>
        /// Opens a new Credit/debit document
        /// </summary>
        /// <param name="tenderType">The tender type used.</param>
        /// <param name="installmentCount">The installment count.</param>
        /// <param name="originalDocumentOperationNumber">The original document operation number.</param>
        /// <param name="transactionValue">The transaction value.</param>
        void CreditDebitReceiptBegin(string tenderType, int installmentCount,
                                     string originalDocumentOperationNumber,
                                     decimal transactionValue);

        /// <summary>
        /// Print text on the Credit/debit document.
        /// </summary>
        /// <param name="text">Text to be printed.</param>
        void CreditDebitReceiptPrintText(string text);

        /// <summary>
        /// Ends the current Credit/debit document.
        /// </summary>
        void CreditDebitReceiptEnd();

        #endregion

        #region Paper cut

        /// <summary>
        /// Executes a paper cut operation.
        /// </summary>
        void ExecutePaperCut(bool checkReturn);

        #endregion

        #region Drawer

        /// <summary>
        /// Opens the drawer attached to the fiscal printer
        /// </summary>
        void OpenDrawer();

        #endregion

        #region Other Report operations

        /// <summary>
        /// Prints the X Report.
        /// </summary>
        void PrintXReport();

        /// <summary>
        /// Generates the Sintegra and NFP files.
        /// </summary>
        /// <param name="reportEnumeration">Specifies the report to be generated.</param>
        /// <param name="args">Each report should have its own set of parameters</param>
        /// <returns>The file name of the report, if applicable.</returns>
        string GenerateReport(int reportEnumeration, params object[] args);

        /// <summary>
        /// Verifies if the printer is ready to begin printing fiscal documents.
        /// </summary>
        /// <returns>True if the printer is ready; false otherwise.</returns>
        bool IsPrinterReady();

        /// <summary>
        /// Retrieves the value of one of the internal counters of the fiscal printer.
        /// </summary>
        /// <param name="counterEnumerator">The index of the counter.</param>
        /// <returns>The counter value.</returns>
        string GetCounterNumber(int counterEnumerator);

        /// <summary>
        /// Retrieves the fiscal printer transaction date.
        /// </summary>
        /// <returns>The transaction date.</returns>
        DateTime GetTransactionDate();

        /// <summary>
        /// Retrieves the fiscal printer accounting date.
        /// </summary>
        /// <returns>The accounting date.</returns>
        DateTime GetAccountingDate();

        #endregion
    }
}
