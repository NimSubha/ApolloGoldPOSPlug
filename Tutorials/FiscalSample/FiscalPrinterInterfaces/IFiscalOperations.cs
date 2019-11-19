using System;
using System.Collections.Generic;

namespace Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces
{
    // Consider providing common Exception (if desired)

    /// <summary>
    /// Fiscal Printer State - The various states that the fiscal printer may be in.
    /// </summary>
    public enum FiscalPrinterState
    {
        None, // Unknown or initial state
        Open,
        FiscalReceipt, // Coupon is open
        ManagementReport
    }

    /// <summary>
    /// Supported barcode symbologies for the fiscal printer
    /// </summary>
    public enum FiscalBarcodeSymbology 
    { 
        EAN13, 
        EAN8, 
        Interleaved2Of5, 
        Code128, 
        Cod39, 
        Code93, 
        UniversalProductCodeA, 
        CodaBar, 
        MSI 
    };

    /// <summary>
    /// Position of barcode text in relation to the barcode
    /// </summary>
    public enum BarcodeTextPosition 
    { 
        None, 
        Below 
    };

    /// <summary>
    /// The set of operations supported by the fiscal printer
    /// </summary>
    public interface IFiscalOperations
    {
        FiscalPrinterState OperatingState { get; }

        /// <summary>
        /// Initializes the component (if required).
        /// </summary>
        void Initialize();

        /// <summary>
        /// Opens the specified device id.
        /// </summary>
        /// <param name="deviceId">The device id (null indicates default device)</param>
        void Open(string deviceId);

        /// <summary>
        /// Prints the X report.
        /// </summary>
        void PrintXReport();

        /// <summary>
        /// Prints the Z report.
        /// <remarks>Once this is done no additional fiscal coupons may be done for the date that the Z report was opened</remarks>
        /// </summary>
        void PrintZReport();

        /// <summary>
        /// Tries the read last Z report data from the printer.
        /// </summary>
        /// <returns></returns>
        string TryReadLastZReport();


        /// <summary>
        /// Gets the tax rates where Token=Tax Code Identifier (TA, TB, ...) and value is tax rate
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        Dictionary<string, decimal> GetTaxRates();

        /// <summary>
        /// Gets the printers serial number.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetSerialNumber();

        /// <summary>
        /// Gets the grand total.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        decimal GetGrandTotal();

        /// <summary>
        /// Gets the subtotal.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        decimal GetSubtotal();

        /// <summary>
        /// Gets the balance due.
        /// <remarks>May be invoked after StartTotalPayment</remarks>
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        decimal GetBalanceDue();

        /// <summary>
        /// Gets the change due.
        /// <remarks>May be invoked after EndReceipt()</remarks>
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        decimal GetChangeDue();

        /// <summary>
        /// Gets the fiscal coupon number.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        string GetCouponNumber();

        /// <summary>
        /// Begins the receipt.
        /// </summary>
        /// <param name="customerTaxNo">The customer tax no. (may be null)</param>
        void BeginReceipt(string customerTaxNo);

        /// <summary>
        /// Cancels the [active] receipt.
        /// </summary>
        void CancelReceipt();

        /// <summary>
        /// Adds the item.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment</remarks>
        /// </summary>
        /// <param name="itemLookupCode">The item lookup code.</param>
        /// <param name="itemDescription">The item description.</param>
        /// <param name="itemPrice">The item price.</param>
        /// <param name="taxRateId">The tax rate id.</param>
        /// <param name="itemQuantity">The item quantity.</param>
        /// <returns>printer line item #</returns>
        int AddItem(string itemLookupCode, string itemDescription, decimal itemPrice, string taxRateId, int itemQuantity);
        // User Enhancement - Add other options, such as float qty, and discount.

        /// <summary>
        /// Removes the item.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment</remarks>
        /// </summary>
        /// <param name="printerItemNumber">The printer item number.</param>
        void RemoveItem(int printerItemNumber);

        /// <summary>
        /// Applies the discount.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment</remarks>
        /// </summary>
        /// <param name="printerItemNumber">The printer item number.</param>
        /// <param name="percentToHundredth">The percent to hundredth.</param>
        void ApplyDiscount(int printerItemNumber, int percentToHundredth);

        /// <summary>
        /// Applies the discount.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment</remarks>
        /// </summary>
        /// <param name="printerItemNumber">The printer item number.</param>
        /// <param name="amount">The amount.</param>
        void ApplyDiscount(int printerItemNumber, decimal amount);


        /// <summary>
        /// Starts the total payment.
        /// </summary>
        void StartTotalPayment();

        /// <summary>
        /// Starts the total payment with surcharge.
        /// </summary>
        /// <param name="surcharge">The surcharge.</param>
        void StartTotalPaymentWithSurcharge(decimal surcharge);

        /// <summary>
        /// Starts the total payment with surcharge.
        /// </summary>
        /// <param name="percentToHundredth">The percent to hundredth.</param>
        void StartTotalPaymentWithSurcharge(int percentToHundredth);

        /// <summary>
        /// Starts the total payment with discount.
        /// </summary>
        /// <param name="discount">The discount.</param>
        void StartTotalPaymentWithDiscount(decimal discount);

        /// <summary>
        /// Starts the total payment with discount.
        /// </summary>
        /// <param name="percentToHundredth">The percent to hundredth.</param>
        void StartTotalPaymentWithDiscount(int percentToHundredth);

        /// <summary>
        /// Makes the payment.
        /// <remarks>Valid after StartTotalPayment and before EndReceipt</remarks>
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="paymentValue">The payment value.</param>
        void MakePayment(string paymentMethod, decimal paymentValue);

        /// <summary>
        /// Ends the receipt exact cash.
        /// </summary>
        /// <param name="message">The message.</param>
        void EndReceiptExactCash(string message);

        /// <summary>
        /// Ends the receipt.
        /// </summary>
        /// <param name="message">The message.</param>
        void EndReceipt(string message);

        // Management Reports

        /// <summary>
        /// Begin Managment Report
        /// </summary>
        /// <returns>true on success</returns>
        bool ManagementReportTryBegin();

        /// <summary>
        /// Print a line on the managment report
        /// </summary>  
        /// <param name="line">The line.</param>
        void ManagementReportPrintLine(string line);

        /// <summary>
        /// Prints a barcode on the management report
        /// </summary>
        /// <param name="barcodeType">Type of the barcode.</param>
        /// <param name="barcodeValue">The barcode value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="position">The position.</param>
        void ManagementReportPrintBarcode(FiscalBarcodeSymbology barcodeType, string barcodeValue, int width, int height, BarcodeTextPosition position);

        /// <summary>
        /// End the management report
        /// </summary>
        void ManagementReportEnd();

    }
}
