using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.FiscalCore;
using Microsoft.Dynamics.Retail.Pos.FiscalLog;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;

namespace Microsoft.Dynamics.Retail.Pos.ApplicationTriggers
{
    /// <summary>
    /// Implements the IApplicationTriggers interface
    /// 
    /// NOTE: Exception handling is not provided for simplicty in the demo. In production level code exception handling should be provided.
    /// <remarks>references must be added for ApplicationTriggerInterface and PreTriggerResults</remarks>
    /// </summary>
    [Export(typeof(IApplicationTrigger))]
    public class ApplicationTrigger : IApplicationTrigger
    {
        #region IApplicationTriggers Members

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        public void ApplicationStart()
        {
            FiscalLogSingleton.Instance.ApplicationStart();

            FiscalPrinterSingleton fiscalCore = FiscalPrinterSingleton.Instance;
            IFiscalOperations fiscalPrinter = fiscalCore.FiscalPrinter;
            PersistentPrinterData printerData = fiscalCore.PrinterData;

            fiscalPrinter.Initialize();
            fiscalPrinter.Open(null);

            Debug.WriteLine(string.Format("Grand Total Persisted: {0} Printer: {1}", printerData.GrandTotal, fiscalPrinter.GetGrandTotal()));
            Debug.WriteLine(string.Format("Serial Number Persisted: {0} Printer: {1}", printerData.SerialNumber, fiscalPrinter.GetSerialNumber()));

            if (printerData.Dirty || 
                (fiscalPrinter.GetGrandTotal() != printerData.GrandTotal) ||
                (fiscalPrinter.GetSerialNumber() != printerData.SerialNumber))
            {
                // New printer
                Debug.WriteLine("Printer serial number or grand total does not match persisted value");

                string deviceZReport = fiscalPrinter.TryReadLastZReport();

                // We will ignore the error if the Z-Reports match...
                if (printerData.ZReport != deviceZReport)
                {   // Z-Reports also do not match
                    if (!UserMessages.ContinueWithPrinterMismatch())
                    {
                        // Note: This trigger is called before the message pump is created for the applicaiton
                        // So instead of System.Windows.Forms.Application.Exit() we will use Environment.Exit
                        System.Environment.Exit(1);
                        return;
                    }
                }

                // Persist the new values
                printerData.SetGrandTotal(fiscalPrinter.GetGrandTotal());
                printerData.SetSerialNumber(fiscalPrinter.GetSerialNumber());
                printerData.SetZReport(deviceZReport);
            }

            fiscalCore.SetTaxRateFromPrinter();

            if (fiscalPrinter.GetSubtotal() != 0)
            {   // If fiscal printer is in Coupon/Docoument mode then reset it.
                // by canceling the current coupon during startup.
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Fiscal printer has starting sub-total: {0}", fiscalPrinter.GetSubtotal()));
                fiscalPrinter.BeginReceipt(null); // Begin new or current receipt
                fiscalPrinter.CancelReceipt(); // Cancel any previous pending receipts
            }
        }

        public void ApplicationStop()
        {
            FiscalLogSingleton.Instance.ApplicationEnd();
            //Debug.WriteLine("ApplicationStop");
        }

        public void LoginWindowVisible()
        {
            Debug.WriteLine("LoginWindowVisible");
        }

        public void Logoff(string operatorId, string name)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Logoff: {0} {1}", operatorId, name));
        }

        public void PostLogon(bool loginSuccessful, string operatorId, string name)
        {
            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "PostLogon: success: {0} {1} {2}", 
                loginSuccessful.ToString(CultureInfo.InvariantCulture), operatorId, name));
        }

        [CLSCompliant(false)]
        public void PreLogon(IPreTriggerResult preTriggerResult, string operatorId, string name)
        {
            Debug.WriteLine("PreLogon");
        }

        #endregion

    }
}
