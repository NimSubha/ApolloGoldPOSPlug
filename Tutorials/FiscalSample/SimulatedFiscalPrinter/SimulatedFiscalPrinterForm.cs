using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;

namespace Microsoft.Dynamics.Retail.Pos.SimulatedFiscalPrinter
{
    internal partial class SimulatedFiscalPrinterForm : Form
    {
        /// <summary>
        /// Internal printer state information
        /// </summary>
        private enum PrinterStates { None, Open };

        // Emulator state info 
        // Possible Enhancments:
        //     Future enhancment would move this outside the UI class
        //     so that the data can be serizlized for restoration at restart).
        //     Compute taxInclusive and taxExclusive amount
        private ZReport _lastZReport;
        private ZReport _activeZReport;
        private List<TaxInfo> _taxRates;
        private List<LineItem> _lineItems;
        private Dictionary<string, decimal> _payments;
        private decimal _subtotal;
        private decimal _balanceDue;
        private decimal _changeDue;
        private decimal _grandTotal;
        private bool _managementReportActive;
        private int _receiptNumber;
        private string _serialNumber;

        private PrinterStates State { get; set; }

        /// <summary>
        /// Gets the serial number.
        /// </summary>
        /// <value>The serial number.</value>
        public string SerialNumber
        {
            get { return _serialNumber; }
        }

        /// <summary>
        /// Gets the receipt number.
        /// </summary>
        /// <value>The receipt number.</value>
        public string ReceiptNumber
        {
            get { return _receiptNumber.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the grand total.
        /// </summary>
        /// <value>The grand total.</value>
        public decimal GrandTotal
        {
            get { return _grandTotal; }
        }

        /// <summary>
        /// Gets the subtotal.
        /// </summary>
        /// <value>The subtotal.</value>
        public decimal Subtotal
        {
            get { return _subtotal; }
        }

        /// <summary>
        /// Gets the balance due.
        /// </summary>
        /// <value>The balance due.</value>
        public decimal BalanceDue
        {
            get { return _balanceDue; }
        }

        /// <summary>
        /// Gets the change due.
        /// </summary>
        /// <value>The change due.</value>
        public decimal ChangeDue
        {
            get { return _changeDue; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimulatedFiscalPrinterForm"/> class.
        /// </summary>
        public SimulatedFiscalPrinterForm()
        {
            InitializeComponent();

        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                // Default initialization
                _receiptNumber = 1;
                _serialNumber = "111-222-333";
                _lastZReport = new ZReport();
                _activeZReport = new ZReport(_lastZReport);

                _taxRates = new List<TaxInfo>();
                _taxRates.Add(new TaxInfo("TA", 12.00m));
                _taxRates.Add(new TaxInfo("TB", 5.00m));
                _taxRates.Add(new TaxInfo("TC", 5.00m));
                _taxRates.Add(new TaxInfo("TD", 10.00m));

                this.taxGridView1.DataSource = _taxRates;

                UpdateUserInterfaceValues();
            }

            // For the OnLoad override, call base last to ensure proper DPI scale.
            base.OnLoad(e);
        }


        /// <summary>
        /// Opens the printer.
        /// </summary>
        /// <returns></returns>
        public bool OpenPrinter()
        {
            if (State == PrinterStates.None)
            {
                State = PrinterStates.Open;
            }

            return (State == PrinterStates.Open);
        }

        /// <summary>
        /// Prints the X report.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        public void PrintXReport()
        {
            richTextBox1.AppendText("X Report");
            richTextBox1.AppendText(Environment.NewLine);
            richTextBox1.ScrollToCaret();
            // Future Enhancment: Add additional data to the x-report
        }

        /// <summary>
        /// Closes and Prints the Z report.
        /// </summary>
        public void PrintZReport()
        {
            _activeZReport.Close(_grandTotal);
            richTextBox1.AppendText(_activeZReport.GetReport());
            richTextBox1.ScrollToCaret();

            _lastZReport = _activeZReport;
            _activeZReport = new ZReport(_lastZReport);
        }

        /// <summary>
        /// Gets the last Z report.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public string GetLastZReport()
        {
            return _lastZReport.GetReport();
        }

        /// <summary>
        /// Gets the tax rates.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public Dictionary<string, decimal> GetTaxRates()
        {
            Dictionary<string, decimal> result = new Dictionary<string, decimal>();

            foreach (TaxInfo tax in _taxRates)
            {
                result.Add(tax.TaxIdentifier, tax.TaxRate);
            }

            return result;
        }

        /// <summary>
        /// Begins the receipt.
        /// </summary>
        /// <param name="customerTaxId">The customer tax id.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "Cust"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        public bool BeginReceipt(string customerTaxId)
        {
            bool result = false;
            if (State == PrinterStates.Open && (_lineItems == null) && !_managementReportActive)
            {
                txtCustomerId.Text = customerTaxId;
                _lineItems = new List<LineItem>();

                richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "Begin Receipt: {0} Cust: {1} ---->", _receiptNumber, customerTaxId));
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.ScrollToCaret();
                result = true;

                _subtotal = 0m;
                _balanceDue = 0m;
                _changeDue = 0m;
                ComputeInternTotals();
            }

            return result;
        }

        /// <summary>
        /// Cancels the receipt.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        public void CancelReceipt()
        {
            if (State == PrinterStates.Open)
            {
                txtCustomerId.Text = string.Empty;

                richTextBox1.AppendText("***** Cancel Receipt" + Environment.NewLine);

                if (_lineItems != null)
                {   // Cancels current receipt
                }
                else
                {   
                }


                _lineItems = null;

                _subtotal = 0m;
                _balanceDue = 0m;
                _changeDue = 0m;
                ComputeInternTotals();
            }
        }

        /// <summary>
        /// Computes the intern totals.
        /// </summary>
        private void ComputeInternTotals()
        {
            _subtotal = 0m;
            _balanceDue = 0m;

            if (_lineItems != null)
            {
                foreach (LineItem item in _lineItems)
                {
                    if (!item.Voided)
                    {
                        _subtotal += item.Quantity * item.AdjustedPrice;
                        // Note: For now only tax-inclusive is supported'
                    }
                }

                _balanceDue = _subtotal;
            }

            if (_balanceDue > 0)
            {
                _changeDue = 0m;
            }

            UpdateUserInterfaceValues();
        }

        /// <summary>
        /// Updates the user interface values.
        /// </summary>
        private void UpdateUserInterfaceValues()
        {
            txtSubTotal.Text = _subtotal.ToString(CultureInfo.CurrentCulture);
            txtBalanceDue.Text = _balanceDue.ToString(CultureInfo.CurrentCulture);
            txtChange.Text = _changeDue.ToString(CultureInfo.CurrentCulture);
            txtGrandTotal.Text = _grandTotal.ToString(CultureInfo.CurrentCulture);

            chkManagementReportActive.Checked = _managementReportActive;
            chkFiscalReceiptMode.Checked = (_lineItems != null);
            chkTendering.Checked = (_payments != null);

            txtReceiptNumber.Text = this.ReceiptNumber;
            txtSerialNumber.Text = this.SerialNumber;
        }

        /// <summary>
        /// Adds the item.
        /// </summary>
        /// <param name="itemLookupCode">The item lookup code.</param>
        /// <param name="itemDescription">The item description.</param>
        /// <param name="itemPrice">The item price.</param>
        /// <param name="taxRateId">The tax rate id.</param>
        /// <param name="itemQuantity">The item quantity.</param>
        /// <returns></returns>
        public int AddItem(string itemLookupCode, string itemDescription, decimal itemPrice, string taxRateId, int itemQuantity)
        {
            int result = 0;

            if ((State == PrinterStates.Open) && (_lineItems != null) && (_payments == null))
            {   // We are in receipt mode (but did not start payments)

                if (IsValidTaxId(taxRateId) && !string.IsNullOrEmpty(itemLookupCode) && !string.IsNullOrEmpty(itemDescription) && (itemQuantity > 0))
                {
                    _lineItems.Add(new LineItem(itemLookupCode, itemDescription, itemPrice, taxRateId, itemQuantity));

                    result = _lineItems.Count;

                    PrintLineInfo(result);

                    ComputeInternTotals();
                }
                else
                {   // Unsupported tax rate id
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified taxRateId is reconized by as a configured tax rate.
        /// </summary>
        /// <param name="taxRateId">The tax rate id.</param>
        /// <returns>
        /// 	<c>true</c> if [is valid tax id] [the specified tax rate id]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsValidTaxId(string taxRateId)
        {
            // See true if any _taxRate has the same taxRateId
            return this._taxRates.Any(taxItem => taxItem.TaxIdentifier == taxRateId);
        }

        /// <summary>
        /// Removes the item.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        public void RemoveItem(int lineNumber)
        {
            LineItem theLineItem = TryGetLineItem(lineNumber);
            
            if ((theLineItem != null) && (_payments == null))
            {   // We are in receipt mode (but did not start payments)
                theLineItem.Voided = true;
                PrintLineInfo(lineNumber);
                ComputeInternTotals();
            }
        }

        /// <summary>
        /// Discounts the item percent.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="discountPercent">The discount percent.</param>
        public void DiscountItemPercent(int lineNumber, decimal discountPercent)
        {
            LineItem theLineItem = TryGetLineItem(lineNumber);

            if ((theLineItem != null) && (_payments == null))
            {   // We are in receipt mode (but did not start payments)
                theLineItem.ApplyDiscountPercent(discountPercent);
                PrintLineInfo(lineNumber);
                ComputeInternTotals();
            }
        }

        /// <summary>
        /// Discounts the item amount.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="discountAmount">The discount amount.</param>
        public void DiscountItemAmount(int lineNumber, decimal discountAmount)
        {
            LineItem theLineItem = TryGetLineItem(lineNumber);

            if ((theLineItem != null)&& (_payments == null))
            {   // We are in receipt mode (but did not start payments)
                theLineItem.ApplyDiscountPercent(discountAmount);
                PrintLineInfo(lineNumber);
                ComputeInternTotals();
            }
        }

        /// <summary>
        /// Prints the line info.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        private void PrintLineInfo(int lineNumber)
        {
            LineItem theLineItem = TryGetLineItem(lineNumber);

            if (theLineItem != null)
            {
                if (theLineItem.Voided)
                {
                    richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "   VOID: {0}", lineNumber));
                }
                else
                {
                    decimal tax = 0m;
                    string inclusiveExclusive = string.Empty;
                    if (IsValidTaxId(theLineItem.TaxRateId))
                    {
                        TaxInfo info = _taxRates.First(taxInfo => taxInfo.TaxIdentifier == theLineItem.TaxRateId);

                        tax = info.TaxRate;
                        inclusiveExclusive = info.IsTaxInclusive ? "(I)" : "(E)";
                    }
                    richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "{0,4} {1,-8} {2,-15} {3,3}un Tax:{4:00.00}%{5} {6:c} {7}",
                        lineNumber.ToString(CultureInfo.InvariantCulture),
                        theLineItem.ItemLookupCode,
                        theLineItem.Description,
                        theLineItem.Quantity,
                        tax,
                        inclusiveExclusive,
                        theLineItem.AdjustedPrice,
                        (theLineItem.AdjustedPrice != theLineItem.Price) ? "DP" : "FP"));
                }
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.ScrollToCaret();
            }
        }

        /// <summary>
        /// Tries the get line item.
        /// </summary>
        /// <param name="lineNumber">The line number.</param>
        /// <returns></returns>
        private LineItem TryGetLineItem(int lineNumber)
        {
            if ((State == PrinterStates.Open) && (_lineItems != null) && (lineNumber >= 1))
            {   // We are in receipt mode and at least one item exist
                int index = lineNumber - 1;
                if (index < _lineItems.Count)
                {
                    return _lineItems[index];
                }
            }

            return null;
        }

        /// <summary>
        /// Begins the payment.
        /// </summary>
        public void BeginPayment()
        {
            BeginPaymentAmountAdjust(0m);
        }

        /// <summary>
        /// Begins the payment percent adjust.
        /// </summary>
        /// <param name="surchargeDiscountAdjustPercent">The surcharge discount adjust percent.</param>
        public void BeginPaymentPercentAdjust(decimal surchargeDiscountAdjustPercent)
        {
            decimal adjustAmount = _subtotal * (surchargeDiscountAdjustPercent / 100m);
            BeginPaymentAmountAdjust(adjustAmount);
        }

        /// <summary>
        /// Begins the payment amount adjust.
        /// </summary>
        /// <param name="surchargeDiscountAdjustAmount">The surcharge discount adjust amount.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        public void BeginPaymentAmountAdjust(decimal surchargeDiscountAdjustAmount)
        {
            if ((State == PrinterStates.Open) && (_lineItems != null) && (_payments == null))
            {   // We are in receipt mode (but did not start payments)
                // May want to adde check to ensure that line items are not all void

                if (_subtotal + surchargeDiscountAdjustAmount >= 0m)
                {
                    _subtotal += surchargeDiscountAdjustAmount;
                    _balanceDue = _subtotal;
                    _changeDue = 0m;

                    _payments = new Dictionary<string, decimal>();

                    richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "*** Begin Payments Adjust: {0:c}, Subtotal {1:c}", surchargeDiscountAdjustAmount, _subtotal));
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.ScrollToCaret();
                    UpdateUserInterfaceValues();
                }
            }
        }

        /// <summary>
        /// Makes the payment.
        /// </summary>
        /// <param name="paymentMethod">The payment method.</param>
        /// <param name="paymentValue">The payment value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        public void MakePayment(string paymentMethod, decimal paymentValue)
        {
            Debug.Assert(paymentValue > 0m, "Payments must be positive amounts");

            if ((State == PrinterStates.Open) && (_lineItems != null) && (_payments != null))
            {   // We are in receipt mode and payments have been started
                if (_payments.ContainsKey(paymentMethod))
                {   // Additional payment for this tender type
                    _payments[paymentMethod] += paymentValue;
                }
                else
                {
                    _payments.Add(paymentMethod, paymentValue);
                }

                richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "Payment: {0,-10} {1:c}", paymentMethod, paymentValue));
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.ScrollToCaret();

                _balanceDue -= paymentValue;
                if (_balanceDue < 0m)
                {
                    _balanceDue = 0m;
                }

                UpdateUserInterfaceValues();
            }
        }

        /// <summary>
        /// Ends the receipt.
        /// </summary>
        /// <param name="message">The message.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)")]
        public void EndReceipt(string message)
        {
            if ((State == PrinterStates.Open) && (_lineItems != null) && (_payments != null))
            {  // We are in receipt mode and payments have been started

                if (_balanceDue == 0m)
                {
                    // Determine change due, update grand total, clean-up receipt coupon, print end receipt

                    richTextBox1.AppendText(Environment.NewLine);

                    richTextBox1.AppendText("Payment Summary");
                    richTextBox1.AppendText(Environment.NewLine);

                    decimal totalPayments = 0m;
                    foreach (string paymentMethod in _payments.Keys)
                    {   // Show payment method data
                        decimal payment = _payments[paymentMethod];
                        totalPayments += payment;

                        richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "Pay: {0,-10} {1:c}", paymentMethod, payment));
                        richTextBox1.AppendText(Environment.NewLine);

                        // Update Z-report payment methods
                        _activeZReport.AddTender(paymentMethod, payment);
                    }

                    richTextBox1.AppendText("Tax Summary");
                    richTextBox1.AppendText(Environment.NewLine);
                    foreach (LineItem theItem in _lineItems)
                    {   // Show tax data

                        if (!theItem.Voided)
                        {
                            TaxInfo info = _taxRates.First(taxInfo => taxInfo.TaxIdentifier == theItem.TaxRateId);
                            decimal taxAmount = theItem.AdjustedPrice * (info.TaxRate / 100m);
                            richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "Tax: {0,-10} {1:00.00}% {2:c}", info.TaxIdentifier, info.TaxRate, taxAmount));
                            richTextBox1.AppendText(Environment.NewLine);

                            // Update Z-rport tax data
                            _activeZReport.AddTax(theItem.TaxRateId, taxAmount);
                        }
                    }

                    _grandTotal += _subtotal;
                    _changeDue = decimal.Negate(_subtotal - totalPayments);
                    _balanceDue = 0m;
                    _subtotal = 0m;

                    richTextBox1.AppendText(string.Format(CultureInfo.CurrentCulture, "Change Due: {0:c}", _changeDue));
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.AppendText(message);
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.AppendText("<---- End Receipt");
                    richTextBox1.AppendText(Environment.NewLine);
                    richTextBox1.ScrollToCaret();

                    _payments = null;
                    _lineItems = null;

                    _receiptNumber += 1;

                    UpdateUserInterfaceValues();
                }
            }
        }

        /// <summary>
        /// Begins the management report.
        /// </summary>
        /// <returns></returns>
        public bool BeginManagementReport()
        {
            if ((State == PrinterStates.Open) && (_lineItems == null) && !_managementReportActive)
            {   // We are open but not in receipt mode and have not already started a management report
                _managementReportActive = true;
                UpdateUserInterfaceValues();
            }

            return _managementReportActive;
        }

        public void ManagementReportPrintLine(string line)
        {
            if ((State == PrinterStates.Open) && (_lineItems == null) && _managementReportActive)
            {   // We are open but not in receipt mode and have already started a management report
                richTextBox1.AppendText(line);
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.ScrollToCaret();
            }

        }

        /// <summary>
        /// Managements the report print barcode.
        /// </summary>
        /// <param name="barcodeType">Type of the barcode.</param>
        /// <param name="barcodeValue">The barcode value.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="position">The position.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.TextBoxBase.AppendText(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "width"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "position"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "height"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "barcodeValue"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "barcodeType")]
        public void ManagementReportPrintBarcode(FiscalBarcodeSymbology barcodeType, string barcodeValue, int width, int height, BarcodeTextPosition position)
        {
            if ((State == PrinterStates.Open) && (_lineItems == null) && _managementReportActive)
            {   // We are open but not in receipt mode and have already started a management report
                richTextBox1.AppendText("BARCODE"); 
                richTextBox1.AppendText(Environment.NewLine);
                richTextBox1.ScrollToCaret();
            }
        }

        /// <summary>
        /// Managements the report end.
        /// </summary>
        public void ManagementReportEnd()
        {
            if ((State == PrinterStates.Open) && (_lineItems == null) && _managementReportActive)
            {   // We are open but not in receipt mode and have already started a management report
                _managementReportActive = false;
                UpdateUserInterfaceValues();
            }

        }
    }
}
