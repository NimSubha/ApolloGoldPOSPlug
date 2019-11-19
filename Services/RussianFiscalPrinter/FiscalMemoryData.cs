/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Data structure class for fiscal memory data of the printer
    /// </summary>
    public sealed class FiscalMemoryData
    {
        /// <summary>
        /// Gets or sets the fiscal printer serial number.
        /// </summary>
        public string FiscalPrinterSerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the fiscal printer shift id.
        /// </summary>
        public string FiscalPrinterShiftId { get; set; }
        /// <summary>
        /// Gets or sets the fiscal printer Control register factory number.
        /// </summary>
        public string EKLZSerialNumber { get; set; }
        /// <summary>
        /// Gets or sets the fiscal printer cryptographic control key number.
        /// </summary>
        public string KPKNumber { get; set; }
        /// <summary>
        /// Gets or sets the fiscal printer open document number.
        /// </summary>
        public string FiscalDocumentSerialNumber { get; set; }

        public override string ToString()
        {
            return string.Format("FiscalPrinterSerialNumber = {0}, FiscalPrinterShiftId = {1}, EKLZSerialNumber = {2}, KPKNumber = {3}, FiscalDocumentSerialNumber = {4}",
                FiscalPrinterSerialNumber, FiscalPrinterShiftId, EKLZSerialNumber, KPKNumber, FiscalDocumentSerialNumber);
        }
    }
}
