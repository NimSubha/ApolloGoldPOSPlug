/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// Enumerates the different document types handled by 
    /// fiscal printers.
    /// </summary>
    public enum ReceiptType
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// Fiscal receipt (BR, HU)
        /// </summary>
        FiscalReceipt,
        /// <summary>
        /// Non fiscal receipt (BR)
        /// </summary>
        NonFiscalReceipt,
        /// <summary>
        /// Simplified invoice (HU)
        /// </summary>
        SimplifiedInvoice,
        /// <summary>
        /// Bottle refund (HU)
        /// </summary>
        BottleRefund,
        /// <summary>
        /// Regular refund (HU)
        /// </summary>
        RegularRefund
    }

    /// <summary>
    /// Fiscal Printer State - The various states that the fiscal printer may be in.
    /// </summary>
    public enum FiscalPrinterState
    {
        /// <summary>
        /// Not Initialized
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Initializing the printer
        /// </summary>
        Initializing,
        /// <summary>
        /// Printer Initialized
        /// </summary>
        Initialized,
        /// <summary>
        /// Fiscal receipt is open
        /// </summary>
        FiscalReceipt,
        /// <summary>
        /// Fiscal receipt in payment mode
        /// </summary>
        FiscalReceiptPayment,
        /// <summary>
        /// Finalizing fiscal receipt
        /// </summary>
        FiscalReceiptTotal,
        /// <summary>
        /// Non fiscal receipt is open
        /// </summary>
        NonFiscalReceipt,
        /// <summary>
        /// Non fiscal receipt in payment mode
        /// </summary>
        NonFiscalReceiptPayment,
        /// <summary>
        /// Finalizing non fiscal receipt
        /// </summary>
        NonFiscalReceiptTotal,
        /// <summary>
        /// Management report is open
        /// </summary>
        ManagementReport,
        /// <summary>
        /// Credit debit receipt is open
        /// </summary>
        CreditDebitReceipt,
        /// <summary>
        /// Z Report already printed
        /// </summary>
        ZReportAlreadyPrinted,
        /// <summary>
        /// Z Report is pending
        /// </summary>
        ZReportIsPending,
    }

    /// <summary>
    /// Fiscal Printer State - The various states that the fiscal printer may be in.
    /// </summary>
    public enum AdjustmentType
    {
        None = 0,
        DiscountAmount,
        DiscountPercent,
        SurchargeAmount,
        SurchargePercent
    }

    /// <summary>
    /// Indicates the fiscal printer status such as paper near end, 
    /// paper present or not, and printer opened.
    /// </summary>
    [Flags]
    public enum FiscalPrinterStatus
    {
        DrawerStatus = 0x0001,
        PaperNearEnd = 0x0002,
        PaperStatus = 0x0004,
        PrintingInProgress = 0x0008,
        PrinterOpened = 0x0010,
        FiscalMemoryAlmostFull = 0x0020,
        FiscalMemoryFull = 0x0040
    }
}
