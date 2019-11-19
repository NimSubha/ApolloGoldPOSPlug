using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.FiscalCore;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    public partial class GeneralFiscalForm : Form
    {
        private IFiscalOperations fiscalPrinter;

        public GeneralFiscalForm()
        {
            InitializeComponent();

            fiscalPrinter = FiscalPrinterSingleton.Instance.FiscalPrinter;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.Windows.Forms.IWin32Window,System.String,System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void btnGrandTotal_Click(object sender, EventArgs e)
        {
            decimal grandTotal = fiscalPrinter.GetGrandTotal();

            MessageBox.Show(this, grandTotal.ToString("C", CultureInfo.CurrentCulture), "Grand Total");

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.Windows.Forms.IWin32Window,System.String,System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void btnSubtotal_Click(object sender, EventArgs e)
        {
            decimal total = fiscalPrinter.GetSubtotal();

            MessageBox.Show(this, total.ToString(CultureInfo.CurrentCulture), "Subtotal");

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.Windows.Forms.IWin32Window,System.String,System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void btnChangeDue_Click(object sender, EventArgs e)
        {
            decimal total = fiscalPrinter.GetChangeDue();

            MessageBox.Show(this, total.ToString(CultureInfo.CurrentCulture), "Change due");

        }

        private void btnXReport_Click(object sender, EventArgs e)
        {
            fiscalPrinter.PrintXReport();
        }

        private void btnZReport_Click(object sender, EventArgs e)
        {
            fiscalPrinter.PrintZReport();
            UpdateZReport();

            // Save the ZReport to a file for other to access
            string zReport = FiscalPrinterSingleton.Instance.PrinterData.ZReport;
            string filename = Path.Combine(FiscalPrinterSingleton.FiscalCorePath, "ZReport.txt");
            try
            {
                using (TextWriter stream = new StreamWriter(filename))
                {
                    stream.Write(zReport);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (System.Security.SecurityException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void btnManagmentReport_Click(object sender, EventArgs e)
        {
            if (fiscalPrinter.ManagementReportTryBegin())
            {
                fiscalPrinter.ManagementReportPrintLine("================================================");
                fiscalPrinter.ManagementReportPrintLine("\r\n" + "Barcodes EAN13" + "\r\n");
                fiscalPrinter.ManagementReportPrintBarcode(FiscalBarcodeSymbology.EAN13, "789103774450", 0, 0, BarcodeTextPosition.Below);
                fiscalPrinter.ManagementReportPrintBarcode(FiscalBarcodeSymbology.EAN13, "789103774450", 3, 120, BarcodeTextPosition.None);
                fiscalPrinter.ManagementReportPrintBarcode(FiscalBarcodeSymbology.EAN13, "789103774450", 4, 150, BarcodeTextPosition.Below);
                fiscalPrinter.ManagementReportPrintBarcode(FiscalBarcodeSymbology.EAN13, "789103774450", 5, 170, BarcodeTextPosition.None);
                fiscalPrinter.ManagementReportPrintBarcode(FiscalBarcodeSymbology.EAN13, "789103774450", 0, 200, BarcodeTextPosition.Below);
                fiscalPrinter.ManagementReportPrintLine("================================================");
                fiscalPrinter.ManagementReportEnd();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.String,System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void btnTaxRates_Click(object sender, EventArgs e)
        {
            Dictionary<string, decimal> taxRates = fiscalPrinter.GetTaxRates();
            StringBuilder rates = new StringBuilder();

            foreach (string token in taxRates.Keys)
            {
                rates.AppendLine(string.Format(CultureInfo.InvariantCulture, "Tax: {0} Rate: {1}", token, taxRates[token]));
            }

            MessageBox.Show(rates.ToString(), "Tax Rates");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.Windows.Forms.IWin32Window,System.String,System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void btnGetCouponNo_Click(object sender, EventArgs e)
        {
            string couponNumber = fiscalPrinter.GetCouponNumber();

            MessageBox.Show(this, couponNumber, "Coupon #");

        }

        private void UpdateZReport()
        {
            string deviceZReport = fiscalPrinter.TryReadLastZReport();
            if (!string.IsNullOrEmpty(deviceZReport))
            {
                FiscalPrinterSingleton.Instance.PrinterData.SetZReport(deviceZReport);
            }

        }

        private void btnZReportData_Click(object sender, EventArgs e)
        {
            UpdateZReport();
        }

        private void btnComputeMD5_Click(object sender, EventArgs e)
        {
            FiscalPrinterSingleton.SaveProgramListMD5();
        }
    }
}
