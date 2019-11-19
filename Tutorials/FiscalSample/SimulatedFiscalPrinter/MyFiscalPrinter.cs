using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;

namespace Microsoft.Dynamics.Retail.Pos.SimulatedFiscalPrinter
{
    /// <summary>
    /// Demonstrates an example implementation of IFiscalOperations
    /// This class would interface with the device driver or library.
    /// In this example, we create a form that acts as the physical fiscal printer.
    /// 
    /// The primary purpose of this class is to absract the physical printer into a generic
    /// interface so that the rest of the sample can be used with different physcial printer
    /// without the need for device specific code.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification="Non-Modal form is disposed internally")]
    public sealed class MyFiscalPrinter : IFiscalOperations
    {
        private const string CashPaymentId = "Cash";

        private SimulatedFiscalPrinterForm _printerUserInterface;
        private FiscalPrinterState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyFiscalPrinter"/> class.
        /// </summary>
        public MyFiscalPrinter()
        {
            _state = FiscalPrinterState.None;
        }

        ~MyFiscalPrinter()
        {
            if (_printerUserInterface != null)
            {   // Inform the form to go ahead and close
                // Since it is non-modal it will dispose on its own.
                _printerUserInterface.Close();
                _printerUserInterface = null;
            }
        }

        private bool UserInterfaceShowing
        {
            get 
            {
                return 
                    (_printerUserInterface != null) && 
                    (!_printerUserInterface.IsDisposed);
            }
        }

        #region IFiscalOperations Members

        public FiscalPrinterState OperatingState
        {
            get 
            {
                return _state;                
            }
        }

        public void Initialize()
        {
            if (_printerUserInterface == null)
            {
                _printerUserInterface = new SimulatedFiscalPrinterForm();
                _printerUserInterface.Show();
                _printerUserInterface.FormClosed += new System.Windows.Forms.FormClosedEventHandler(_printerUserInterface_FormClosed);
            }
        }

        void _printerUserInterface_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            _printerUserInterface.FormClosed -= new System.Windows.Forms.FormClosedEventHandler(_printerUserInterface_FormClosed);
            _printerUserInterface = null;
        }

        public void Open(string deviceId)
        {
            if (UserInterfaceShowing)
            {
                if (_printerUserInterface.OpenPrinter())
                {
                    _state = FiscalPrinterState.Open;
                }
            }
        }

        public void PrintXReport()
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.Open))
            {
                _printerUserInterface.PrintXReport();
            }
        }

        public void PrintZReport()
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.Open))
            {
                _printerUserInterface.PrintZReport();
            }
        }

        public string TryReadLastZReport()
        {
            string result = string.Empty;

            if (UserInterfaceShowing)
            {
                result = _printerUserInterface.GetLastZReport();
            }

            return result;
        }

        public Dictionary<string, decimal> GetTaxRates()
        {
            if (UserInterfaceShowing)
            {
                return _printerUserInterface.GetTaxRates();
            }
            else
            {
                return null;
            }
        }

        public string GetSerialNumber()
        {
            if (UserInterfaceShowing)
            {
                return _printerUserInterface.SerialNumber;
            }
            else
            {
                return null;
            }
        }

        public decimal GetGrandTotal()
        {
            decimal result = 0m;

            if (UserInterfaceShowing)
            {
                result = _printerUserInterface.GrandTotal;
            }

            return result;
        }

        public decimal GetSubtotal()
        {
            decimal result = 0m;

            if (UserInterfaceShowing)
            {
                result = _printerUserInterface.Subtotal;
            }

            return result;
        }

        public decimal GetBalanceDue()
        {
            decimal result = 0m;

            if (UserInterfaceShowing)
            {
                result = _printerUserInterface.BalanceDue;
            }

            return result;
        }

        public decimal GetChangeDue()
        {
            decimal result = 0m;

            if (UserInterfaceShowing)
            {
                result = _printerUserInterface.ChangeDue;
            }

            return result;
        }

        public string GetCouponNumber()
        {
            if (UserInterfaceShowing)
            {
                return _printerUserInterface.ReceiptNumber;
            }
            else
            {
                return null;
            }
        }

        public void BeginReceipt(string customerTaxNo)
        {
            if (UserInterfaceShowing)
            {
                if (_printerUserInterface.BeginReceipt(customerTaxNo))
                {
                    _state = FiscalPrinterState.FiscalReceipt;
                }
            }
        }

        public void CancelReceipt()
        {
            if (UserInterfaceShowing)
            {
                _printerUserInterface.CancelReceipt();
                _state = FiscalPrinterState.Open;
            }
        }

        public int AddItem(string itemLookupCode, string itemDescription, decimal itemPrice, string taxRateId, int itemQuantity)
        {
            int result = 0;

            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                result = _printerUserInterface.AddItem(itemLookupCode, itemDescription, itemPrice, taxRateId, itemQuantity);
            }

            return result;
        }

        public void RemoveItem(int printerItemNumber)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.RemoveItem(printerItemNumber);
            }
        }

        public void ApplyDiscount(int printerItemNumber, int percentToHundredth)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.DiscountItemPercent(printerItemNumber, percentToHundredth / 100m);
            }
        }

        public void ApplyDiscount(int printerItemNumber, decimal amount)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.DiscountItemAmount(printerItemNumber, amount);
            }
        }

        public void StartTotalPayment()
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.BeginPayment();
            }
        }

        public void StartTotalPaymentWithSurcharge(decimal surcharge)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.BeginPaymentAmountAdjust(surcharge);
            }
        }

        public void StartTotalPaymentWithSurcharge(int percentToHundredth)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.BeginPaymentPercentAdjust(percentToHundredth / 100m);
            }
        }

        public void StartTotalPaymentWithDiscount(decimal discount)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.BeginPaymentAmountAdjust(-discount);
            }
        }

        public void StartTotalPaymentWithDiscount(int percentToHundredth)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.BeginPaymentPercentAdjust(decimal.Negate(percentToHundredth / 100m));
            }
        }

        public void MakePayment(string paymentMethod, decimal paymentValue)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.MakePayment(paymentMethod, paymentValue);
            }
        }

        public void EndReceiptExactCash(string message)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.BeginPayment();
                _printerUserInterface.MakePayment(CashPaymentId, _printerUserInterface.Subtotal);
                EndReceipt(message);
            }
        }

        public void EndReceipt(string message)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.FiscalReceipt))
            {
                _printerUserInterface.EndReceipt(message);
                _state = FiscalPrinterState.Open;
            }
        }

        public bool ManagementReportTryBegin()
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.Open))
            {
                if (_printerUserInterface.BeginManagementReport())
                {
                    _state = FiscalPrinterState.ManagementReport;
                }
            }

            return (_state == FiscalPrinterState.ManagementReport);
        }

        public void ManagementReportPrintLine(string line)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.ManagementReport))
            {
                _printerUserInterface.ManagementReportPrintLine(line);
            }
        }

        public void ManagementReportPrintBarcode(FiscalBarcodeSymbology barcodeType, string barcodeValue, int width, int height, BarcodeTextPosition position)
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.ManagementReport))
            {
                _printerUserInterface.ManagementReportPrintBarcode(barcodeType, barcodeValue, width, height, position);
            }
        }

        public void ManagementReportEnd()
        {
            if (UserInterfaceShowing && (_state == FiscalPrinterState.ManagementReport))
            {
                _printerUserInterface.ManagementReportEnd();
                _state = FiscalPrinterState.Open;
            }
        }

        #endregion
    }
}
