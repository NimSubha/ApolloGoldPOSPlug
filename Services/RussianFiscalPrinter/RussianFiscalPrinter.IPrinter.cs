/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        public void Load()
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinter.Load", "Load called");

            InitializePrinter();
        }

        /// <summary>
        /// Gets or sets a value indicating that fiscal printer is in the process of printing slip document.
        /// </summary>
        /// <remarks>
        /// This value is used to track the state of printing slip document on the fiscal printer.
        /// The slip document is a non-fiscal document and is printed as a sequence of print line commands.
        /// This flag is used to let the driver know that it is printing lines of a slip document to enforce proper error handling.
        /// </remarks>
        public bool IsPrintingSlipDocument { get; private set; }
        
        private bool ExecutePrintOperation(Action operation)
        {
            bool retry = true;

            while (retry)
            {
                try
                {
                    IsPrintingSlipDocument = true;
                    operation();
                    IsPrintingSlipDocument = false;
                    return true;
                }
                catch
                {
                    var dialogResult = UserMessages.ShowRetryDialog(Resources.Translate(Resources.SlipDocumentPrintError));
                    retry = dialogResult == System.Windows.Forms.DialogResult.Retry;
                }
            }

            return false;
        }

        public void Unload()
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinter.Unload", "Unload called");

            // Put uninitialization logic here if needed
        }
        
        /// <summary>
        /// This method will print the text as management report in the fiscal printer
        /// </summary>
        /// <param name="text"></param>
        public void PrintReceipt(string text)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinter.PrintReceipt", "text = {0}", text);

            Action operation = () => { FiscalPrinterDriverFactory.FiscalPrinterDriver.ManagementReportPrintLine(text); };
            if (ExecutePrintOperation(operation))
            {
                RussianFiscalPrinterDriver.FiscalPrinterDriver.RibbonFeed();
                FiscalPrinterDriverFactory.FiscalPrinterDriver.ExecutePaperCut(false);
            }
        }

        /// <summary>
        /// Prints a slip containing the text in the textToPrint parameter
        /// </summary>
        /// <param name="header"></param>
        /// <param name="details"></param>
        /// <param name="footer"></param>
        public void PrintSlip(string header, string details, string footer)
        {
            LogHelper.LogTrace("FiscalPrinter.IPrinter.PrintSlip", "header = {0}, details = {1}, footer = {2}", header, details, footer);

            Action operation = () => 
            {
                FiscalPrinterDriverFactory.FiscalPrinterDriver.ManagementReportPrintLine(header);
                FiscalPrinterDriverFactory.FiscalPrinterDriver.ManagementReportPrintLine(details);
                FiscalPrinterDriverFactory.FiscalPrinterDriver.ManagementReportPrintLine(footer);
            };

            if (ExecutePrintOperation(operation))
            {
                RussianFiscalPrinterDriver.FiscalPrinterDriver.RibbonFeed();
                FiscalPrinterDriverFactory.FiscalPrinterDriver.ExecutePaperCut(false);
            }
        }

        /// <summary>Determines whether the fiscal printer supports printing receipt in non fiscal mode.</summary>
        /// <param name="copyReceipt">Denotes that this is a copy of a receipt; optional, false by default.</param>
        /// <returns>True if the fiscal printer supports printing receipt in non fiscal mode; false otherwise.</returns>
        public bool SupportPrintingReceiptInNonFiscalMode(bool copyReceipt)
        {
            return copyReceipt;
        }

        /// <summary> Determines whether the fiscal printer prohibits printing receipt on non fiscal printers. </summary>
        /// <param name="copyReceipt">Denotes that this is a copy of a receipt; optional, false by default. </param>
        /// <returns>True if the fiscal printer prohibits printing receipt on non fiscal printers; false otherwise. </returns>
        public bool ProhibitPrintingReceiptOnNonFiscalPrinters(bool copyReceipt)
        {
            return false;
        }
    }
}
