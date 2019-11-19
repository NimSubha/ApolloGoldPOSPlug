/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter;
using Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter.Constants;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.Globalization;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{
    /// <summary>
    /// Class encapsulating Shtrih-M field information.
    /// </summary>
    public class ShtrihMFieldInfo
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="fieldName">The name of the field.</param>
        /// <param name="fieldType">The type of the field.</param>
        /// <param name="fieldSize">The size of the field.</param>
        /// <param name="minValue">The minimum integer value for the field.</param>
        /// <param name="maxValue">The maximum integer value for the field.</param>
        public ShtrihMFieldInfo(string fieldName, PrinterParameterType fieldType, Byte fieldSize, Int32 minValue, Int32 maxValue)
        {
            this.FieldName = fieldName;
            this.FieldType = fieldType;
            this.FieldSize = fieldSize;
            this.MinValue = minValue;
            this.MaxValue = maxValue;
        }

        /// <summary>
        /// Field name.
        /// </summary>
        public string FieldName { get; private set; }
        /// <summary>
        /// Field type.
        /// </summary>
        public PrinterParameterType FieldType { get; private set; }
        /// <summary>
        /// Field size.
        /// </summary>
        public Byte FieldSize { get; private set; }
        /// <summary>
        /// Minimum integer value.
        /// </summary>
        public Int32 MinValue { get; private set; }
        /// <summary>
        /// Maximum integer value.
        /// </summary>
        public Int32 MaxValue { get; private set; }

    }

    /// <summary>
    /// Interface for printer service functions.
    /// </summary>
    public interface IShtrihMServiceFunctions
    {
        /// <summary>
        /// Loads the image file to the printer.
        /// </summary>
        /// <param name="fileName">The fileName of the image to load to the printer.</param>
        /// <param name="centerImage">Denotes whether we should center the image.</param>
        void LoadImageToPrinter(string fileName, bool centerImage);

        /// <summary>
        /// Writes an integer value to the table row cell.
        /// </summary>
        /// <param name="tableNumber">The number of the table to write into.</param>
        /// <param name="rowNumber">The number of the row to write into.</param>
        /// <param name="fieldNumber">The number of the field (column) to write into.</param>
        /// <param name="value">The integer value to be written.</param>
        void WriteTableFieldValueOfInteger(int tableNumber, int rowNumber, int fieldNumber, int value);

        /// <summary>
        /// Writes a string value to the table row cell.
        /// </summary>
        /// <param name="tableNumber">The number of the table to write into.</param>
        /// <param name="rowNumber">The number of the row to write into.</param>
        /// <param name="fieldNumber">The number of the field (column) to write into.</param>
        /// <param name="value">The string value to be written.</param>
        void WriteTableFieldValueOfString(int tableNumber, int rowNumber, int fieldNumber, string value);

        /// <summary>
        /// Retrieves the information about table field (data type, name etc).
        /// </summary>
        /// <param name="tableNumber">The number of the table.</param>
        /// <param name="fieldNumber">The number of the field.</param>
        /// <returns>Field information object.</returns>
        ShtrihMFieldInfo GetTableFieldInfo(int tableNumber, int fieldNumber);

        /// <summary>
        /// Determines whether the specified table can be written.
        /// </summary>
        /// <param name="setDefaultParameters">Denotes that we are setting default parameters (at printer initialization).</param>
        /// <param name="tableNumber">The number of the table to write into.</param>
        /// <param name="reasonMessage">An output message, describing the reason of denial.</param>
        /// <param name="showErrorMessage">An output parameter for determining whether to show the error message to the user.</param>
        /// <returns>true if the table can be written; false otherwise.</returns>
        bool CanWriteToTable(bool setDefaultParameters, int tableNumber, out string reasonMessage, out bool showErrorMessage);
    }

    /// <summary>
    /// Implements fiscal printer business logic for Shtrih-M printers family.  
    /// </summary>
    /// <remarks>
    /// Depends heavily on the Shtrih-M driver component.
    /// </remarks>
    [Export(typeof(IFiscalPrinterDriver))]
    public class ShtrihPrinter : RussianFiscalPrinterDriver, IShtrihMServiceFunctions
    {
        #region Class member declarations

        private FiscalPrinterState _operatingState = FiscalPrinterState.Unknown;
        private readonly Dictionary<string, int> _baudRatesDict = new Dictionary<string, int>() { { "2400", 0 }, { "4800", 1 }, { "9600", 2 }, { "19200", 3 }, { "38400", 4 }, { "57600", 5 }, { "115200", 6 } };
        private DrvFRLib.IDrvFR48 _driver;
        private PrinterConfiguration _config;
        private IDictionary<string, decimal> _taxAmounts;
        private List<ITotalsHeaderDiscountLine> _totalsHeaderDiscountLines;
        private IDictionary<string, IList<ISaleLineItem>> _discountSalesLines;
        private PrinterImageController _imageController;
        private PrinterTableController _tableController;
        private LayoutType? _lastPrintedReceiptlayout = null;
        private IDictionary<string, string> _tenderTypesMapping;
        private bool isEKLZInstalled = false;

        /// <summary>
        /// Accessor to the ReceiptLineLength property intialized from config or driver properties.
        /// </summary>
        private int ReceiptLineLength
        {
            get;
            set;
        }

        /// <summary>
        /// Defines a set of Shtrih-M specific driver error codes that are treated as 'Not connected'.
        /// </summary>
        private readonly ISet<int> _notConnectedErrorCodes = new HashSet<int> { ShtrihConstants.ErrorCodes.NotConnected, 
                                                                                ShtrihConstants.ErrorCodes.ComPortIsUnavailable, 
                                                                                ShtrihConstants.ErrorCodes.ComPortIsBusyWithAnotherApplication, 
                                                                                ShtrihConstants.ErrorCodes.NotConnected2, 
                                                                                ShtrihConstants.ErrorCodes.NotConnected3, 
                                                                                ShtrihConstants.ErrorCodes.NotConnected4 };

        // ToDo need to add LogTrace() calls into all methods for logging purposes

        #endregion

        #region IFiscalPrinterDriver Members

        private static string PreprocessMessageWithBraces(string message)
        {
            message = message.Replace("{", "{{");
            message = message.Replace("}", "}}");
            return message;
        }

        private void InitializePrinterDriver()
        {
            try
            {
                var type = Type.GetTypeFromProgID("Addin.DrvFR");
                object obj = Activator.CreateInstance(type);
                _driver = obj as DrvFRLib.IDrvFR48;

                if (_driver == null)
                {
                    ExceptionHelper.ThrowException(false,Resources.OldVersionOfShtrihMDriver);
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError("InitializePrinterDriver", PreprocessMessageWithBraces(ex.Message));
                ExceptionHelper.ThrowException(Resources.FailedToLoadShtrihMDriver);
            }
        }
        
        /// <summary>
        /// Gets the currenct fiscal printer driver operating state.
        /// </summary>
        public override FiscalPrinterState OperatingState
        {
            get { return _operatingState; }
        }

        /// <summary>
        /// Determines whether an error message is displayed to the 
        /// user before throwing the FiscalPrinterException exception.
        /// </summary>
        public override bool SuppressErrorMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Show the printer driver configuration window
        /// that allows the user to specify COM port,
        /// baud rate and any other printer setting.
        /// </summary>
        public override void ShowPrinterConfigurationWindow()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Initializes the driver.
        /// </summary>
        /// <param name="args">Various arguments passed in.</param>
        public override void Initialize(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (OperatingState == FiscalPrinterState.Initialized)
            {
                return;
            }

            if (args.Length != 4)
            {
                throw new ArgumentException("args length is not equal to 4");
            }

            InitializePrinterDriver();
            _config = PrinterConfiguration.GetConfig();

            var comString = (string)args[0];
            var baudRateString = (string)args[1];
            var connectionTimeout = (int)args[2];
            var logFilename = (string)args[3];

            Regex re = new Regex(@"\d+");
            Match m = re.Match(comString);

            if (m.Success)
            {
                _driver.ComNumber = Convert.ToInt32(m.Value);
            }
            else
            {
                throw new ArgumentException("Invalid COM port");
            }

            if (_baudRatesDict.Keys.Contains(baudRateString))
            {
                _driver.BaudRate = _baudRatesDict[baudRateString];
            }
            else
            {
                throw new ArgumentException("Invalid baud rate");
            }

            _driver.ConnectionTimeout = connectionTimeout;
            _driver.ComLogFile = logFilename;
            _driver.Password = ShtrihConstants.DefaultPrinterPassword;

            LogHelper.LogDebug("ShtrihPrinter", "connection properties = ComNumber={0}, Speed={1}, Password={2}, Log={3}",
                _driver.ComNumber,
                _driver.BaudRate,
                _driver.Password,
                _driver.ComLogFile);

            // We treat Connect as a printing operation because the printer might be in some transient printing state when it is connected and the only way to get passed this state is to enable printing errors handling.
            CheckResultCode(_driver.Connect);

            // Call cancel receipt in order to cancel the open check if one remains from the previously failed operation.
            // Calling this method is safe as it cancels receipt only if one is in open state.
            CancelReceipt(ReceiptType.None);

            if (_config != null)
            {
                _tableController = new PrinterTableController(this, _config.PrinterParameters);
                _tableController.SetPrinterDefaultParameters();

                _imageController = new PrinterImageController(this, _config);
                _imageController.LoadDefaultImage();
            }

            ValidateTaxModeIsSupported();

            InitializeReceiptLineLength();

            _operatingState = FiscalPrinterState.Initialized;

            QueryPrinterStatus();
            isEKLZInstalled = _driver.EKLZIsPresent;
        }

        /// <summary>
        /// Searches for printer parameters for specified layout.
        /// </summary>
        /// <param name="layoutType">Layout type to search for.</param>
        /// <returns>Printer parameters collection.</returns>
        private PrinterSettingsCollection FindPrinterParameters(LayoutType layoutType)
        {
            PrinterSettingsCollection parameters = null;
            var layout = FindLayout(layoutType);

            if (layout != null)
            {
                parameters = layout.PrinterParameters;
            }

            return parameters;
        }

        /// <summary>
        /// Determines whether the specified table can be written.
        /// </summary>
        /// <param name="setDefaultParameters">Denotes that we are setting default parameters (at printer initialization).</param>
        /// <param name="tableNumber">The number of table to write into.</param>
        /// <param name="reasonMessage">An output message, describing the reason of denial.</param>
        /// <param name="showErrorMessage">An output parameter for determining whether to show the error message to the user.</param>
        /// <returns>true if the table can be written; false otherwise.</returns>
        public bool CanWriteToTable(bool setDefaultParameters, int tableNumber, out string reasonMessage, out bool showErrorMessage)
        {
            reasonMessage = string.Empty;
            showErrorMessage = false;
            bool result = true;

            if (setDefaultParameters)
            {
                result = !(tableNumber == ShtrihConstants.TaxTableNumber && IsDayOpened());

                if (!result)
                {
                    reasonMessage = Resources.Translate(Resources.TableCanBeWrittenWhenShiftIsClosed, tableNumber);
                }
            }
            else
            {
                result = !(tableNumber == ShtrihConstants.TaxTableNumber || tableNumber == ShtrihConstants.TenderTypeTableNumber ||
                           tableNumber == ShtrihConstants.PasswordsTable || tableNumber == ShtrihConstants.TimeShiftTable ||
                           tableNumber == ShtrihConstants.DepartmetsTable);

                if (!result)
                {
                    reasonMessage = Resources.Translate(Resources.TableCanOnlyBeWrittenAtInitialization, tableNumber);
                    showErrorMessage = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Sets the printer parameters for the specific layout.
        /// </summary>
        /// <param name="layout">The layout type to take parameters from.</param>
        /// <remarks>
        /// If layout is not found, then general printer parameners will be taken.
        /// If layout is found, then parameters defined in the layout will be taken.
        /// </remarks>
        private void SetPrinterParameters(LayoutType layout)
        {
            ValidatePrinterConnected();

            var parameters = FindPrinterParameters(layout);
            if (parameters != null)
            {
                _tableController.SetPrinterLayoutParameters(layout, parameters);
            }
        }

        public override void SetInitializing()
        {
            if (_operatingState == FiscalPrinterState.Unknown)
            {
                _operatingState = FiscalPrinterState.Initializing;
            }
        }

        public override void GenerateZReportAndFiles(string filePath)
        {
            const LayoutType layout = LayoutType.ReportZ;

            // Reset control flags at the beginning of every compound operation, i.e., operation that consists of a driver operation call sequence.
            ResetControlsVariables();
            ValidatePrinterConnected();

            // Call cancel receipt in order to cancel the open check if one remains from the previously failed operation.
            // Calling this method is safe as it cancels receipt only if one is in open state.
            CancelReceipt(ReceiptType.None);

            if (IsDayOpened())
            {
                LogHelper.LogTrace("ShtrihPrinter.GenerateZReportAndFiles", "Day is opened, start printing Z report.");
                SetPrinterParameters(layout);
                PrintDocumentHeader(layout);
                FillFiscalMemoryData(PosTransaction.TypeOfTransaction.CloseShift, FiscalMemoryData, false);
                ExecutePrintingOperation(_driver.PrintReportWithCleaning, PrinterOperationType.PrintReport);
                PrintDocumentFooter(layout);
            }
        }

        /// <summary>
        /// Starts a new fiscal day.
        /// </summary>
        public override void StartOfDay()
        {
            LogHelper.LogTrace("ShtrihPrinter.StartOfDay", "Onening new day on the printer.");
            CheckResultCode(_driver.OpenSession);
            IsShiftOpenedAtLastLogOn = true;
        }

        /// <summary>
        /// Executes a paper cut operation.
        /// </summary>
        public override void ExecutePaperCut(bool checkReturn)
        {
            if (!CutRibbon)
            {
                return;
            }

            _driver.CutType = CutType;
            CheckResultCode(_driver.CutCheck);
        }

        /// <summary>
        /// Opens the drawer attached to the fiscal printer
        /// </summary>
        public override void OpenDrawer()
        {
            LogHelper.LogTrace("ShtrihPrinter.OpenDrawer", "Open drawer.");
            CheckResultCode(_driver.OpenDrawer);
        }

        /// <summary>
        /// Prints the X Report.
        /// </summary>
        public override void PrintXReport()
        {
            const LayoutType layout = LayoutType.ReportX;

            LogHelper.LogTrace("ShtrihPrinter.PrintXReport", "Start printing X report.");

            // Reset control flags at the beginning of every compound operation, i.e., operation that consists of a driver operation call sequence.
            ResetControlsVariables();
            ValidatePrinterConnected();
            SetPrinterParameters(layout);
            PrintDocumentHeader(layout);
            ExecutePrintingOperation(_driver.PrintReportWithoutCleaning, PrinterOperationType.PrintReport);
            PrintDocumentFooter(layout);
        }

        /// <summary>
        /// Checks if the printer has an opened period to issue fiscal documents.
        /// </summary>
        public override bool IsDayOpened()
        {
            QueryShortPrinterStatus();
            return _driver.ECRMode == (int)ECRMode.ShiftIsOpenLessThan24Hours || _driver.ECRMode == (int)ECRMode.ShiftIsOpenMoreThan24Hours;
        }

        /// <summary>
        /// Checks if the printer has an opened period more than 24 hours.
        /// </summary>
        public override bool IsDayOpenedMoreThan24Hours()
        {
            QueryShortPrinterStatus();
            return _driver.ECRMode == (int)ECRMode.ShiftIsOpenMoreThan24Hours;
        }

        /// <summary>
        /// Tries to read last Z report data from the printer.
        /// </summary>
        /// <returns>A string with the Z report encoded (specific to each manufacturer).</returns>
        public override string GetLastZReport()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the current Z Report count.
        /// </summary>
        /// <returns>The current Z Report count.</returns>
        public override decimal GetCurrentZReportCount()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the last Z Report count.
        /// </summary>
        /// <returns>The last Z Report count.</returns>
        public override decimal GetLastZReportCount()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the FiscalPrinterStatus flags that indicates the status 
        /// of many fiscal printer sensors.
        /// </summary>
        /// <returns>A bitmask containing all the statuses set.</returns>
        public override FiscalPrinterStatus GetStatus()
        {
            FiscalPrinterStatus printerStatus = 0;

            CheckResultCode(_driver.GetECRStatus);

            if (_driver.IsDrawerOpen)
            {
                printerStatus |= FiscalPrinterStatus.DrawerStatus;
            }

            if (_driver.ECRAdvancedMode == (int)ECRAdvancedMode.ActiveOutOfPaper || _driver.ECRAdvancedMode == (int)ECRAdvancedMode.PassiveOutOfPaper)
            {
                printerStatus |= FiscalPrinterStatus.PaperStatus;
            }

            if (_driver.ECRAdvancedMode == (int)ECRAdvancedMode.PrintingOperation || _driver.ECRAdvancedMode == (int)ECRAdvancedMode.LongReportPrinting)
            {
                printerStatus |= FiscalPrinterStatus.PrintingInProgress;
            }

            if (_driver.FMOverflow)
            {
                printerStatus |= FiscalPrinterStatus.FiscalMemoryFull;
            }

            if (_driver.IsBatteryLow)
            {
                printerStatus |= FiscalPrinterStatus.FiscalMemoryAlmostFull;
            }

            return printerStatus;
        }

        /// <summary>
        /// Verifies if there is a Z report pending to be printed.
        /// </summary>
        /// <returns>True if there is one pending; false otherwise.</returns>
        public override bool HasZReportPending()
        {
            // This method is not used in Russian country context.
            return true;
        }

        /// <summary>
        /// Retrieves the tax rates dictionary where key is the tax rate or tax code for predefined taxes
        /// and value is tax index in the fiscal printer memory.
        /// </summary>
        /// <returns>Dictionary with tax rates</returns>
        public override IDictionary<string, decimal> GetTaxRates()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the tax mapping dictionary where key is the POS tax code and value is printer tax ID.
        /// </summary>
        /// <returns>The tax mapping dictionary.</returns>
        public override IDictionary<string, string> GetTaxMapping()
        {
            var taxMapping = new Dictionary<string, string>();

            foreach (Tax tax in _config.TaxMapping)
            {
                if (tax.PrinterTaxId > ShtrihConstants.MaximumNumberOfTaxCodesSupported)
                {
                    throw new Exception(Resources.Translate(Resources.TaxMappingWithWrongTaxFound, tax.TaxCode, tax.PrinterTaxId));
                }

                if (taxMapping.ContainsKey(tax.TaxCode.ToUpperInvariant()))
                {
                    throw new Exception(Resources.Translate(Resources.DuplicateTaxMappingFound, tax.TaxCode));
                }

                taxMapping.Add(tax.TaxCode.ToUpperInvariant(), tax.PrinterTaxId.ToString());
            }

            LogHelper.LogTrace("ShtrihPrinter.GetTaxMapping", "taxMapping = {0}", taxMapping);

            return taxMapping;
        }

        /// <summary>
        /// Retrieves the tax amounts dictionary from the printer, where key is printer tax ID and value is tax amount.
        /// </summary>
        /// <returns>The tax amounts dictionary.</returns>
        public override IDictionary<string, decimal> GetTaxAmounts()
        {
            return _taxAmounts;
        }

        public override IDictionary<string, string> GetTenderTypesMapping()
        {
            return _tenderTypesMapping ?? (_tenderTypesMapping = GetTenderTypesMappingFromConfig());
        }

        /// <summary>
        /// Retrieves the list of totals header discount data from the printer for tax adjusting.
        /// </summary>
        /// <returns>The list of discount data.</returns>
        /// <remarks>Note: The list is inited in BeginReceipt method, and is filled in PrintTotalsHeaderDiscountLine method.</remarks>
        public override IList<ITotalsHeaderDiscountLine> GetTotalsHeaderDiscountLines()
        {
            return _totalsHeaderDiscountLines;  
        }

        /// <summary>
        /// Retrieves the discount sales lines dictionary from the printer, 
        /// where key is printer tax IDs set (printer tax group), value - list of sales lines.
        /// </summary>
        /// <returns>The tax amounts dictionary.</returns>
        /// <remarks>Note: The dictionary is inited in BeginReceipt method, and is filled in AddItem method.</remarks>
        public override IDictionary<string, IList<ISaleLineItem>> GetDiscountSalesLines()
        {
            return _discountSalesLines;
        }

        /// <summary>
        /// Retrieves the fiscal printer memory data.
        /// </summary>
        /// <param name="transactionType">Type of transaction</param>
        /// <returns>The fiscal memory data.</returns>
        public override FiscalMemoryData RetrieveFiscalMemoryData(PosTransaction.TypeOfTransaction transactionType)
        {
            if (!IsPrinterConnected())
            {
                return FiscalMemoryData;
            }

            var fiscalMemoryData = new FiscalMemoryData();

            FillFiscalMemoryData(transactionType, fiscalMemoryData);

            LogHelper.LogTrace("ShtrihPrinter.RetrieveFiscalMemoryData", "fiscalMemoryData = {0}", fiscalMemoryData);

            return fiscalMemoryData;
        }

        /// <summary>
        /// Fills the <see cref="FiscalMemoryData"/> passed in according to the <paramref name="transactionType"/> value.
        /// </summary>
        /// <param name="transactionType">The type of the transaction.</param>
        /// <param name="fiscalMemoryData">Fiscal memory data object to fill.</param>
        /// <param name="isCalledAfterOperation">A Boolean determining whether the method is called after the completion of the operation; optional.</param>
        /// <remarks>
        /// The <paramref name="fiscalMemoryData"/> must be initialized.
        /// </remarks>
        private void FillFiscalMemoryData(PosTransaction.TypeOfTransaction transactionType, RussianFiscalPrinter.FiscalMemoryData fiscalMemoryData, bool isCalledAfterOperation = true)
        {
            if (isCalledAfterOperation && isEKLZInstalled)
            {
                CheckResultCode(_driver.GetEKLZCode1Report);
                fiscalMemoryData.KPKNumber = _driver.LastKPKNumber.ToString();
            }

            fiscalMemoryData.FiscalPrinterShiftId = GetShiftId(transactionType);

            if (isEKLZInstalled)
            {
                CheckResultCode(_driver.GetEKLZSerialNumber);
                fiscalMemoryData.EKLZSerialNumber = _driver.EKLZNumber;
            }

            CheckResultCode(_driver.GetECRStatus);
            fiscalMemoryData.FiscalPrinterSerialNumber = GetSerialNumber();
            
            int openDocumentNumber = _driver.OpenDocumentNumber;
            // OpenDocumentNumber should be adjusted manually in case the fiscal printer is not available for reading of fiscal data after operation
            if (!isCalledAfterOperation && (transactionType != PosTransaction.TypeOfTransaction.Sales || !IsFiscalHeaderPrintedInTheBeginning()))
            {
                openDocumentNumber++;
            }
            fiscalMemoryData.FiscalDocumentSerialNumber = openDocumentNumber.ToString();

            LogHelper.LogTrace("ShtrihPrinter.FillFiscalMemoryData", "fiscalMemoryData = {0}", fiscalMemoryData);
        }

        /// <summary>
        /// Retrieves the fiscal printer shift ID.
        /// </summary>
        /// <param name="transactionType">Type of transaction</param>
        /// <returns>The fiscal printer shift ID.</returns>
        public override string GetShiftId(PosTransaction.TypeOfTransaction transactionType)
        {
            if (!isEKLZInstalled)
            {
                return String.Empty;
            }
            CheckResultCode(_driver.GetEKLZCode2Report);
            int shiftId = _driver.SessionNumber;
            
            // Decrement shiftId due to Shtrih printer driver PrintReportWithCleanig command automatically increments it.
            if ((transactionType == PosTransaction.TypeOfTransaction.CloseShift) && (shiftId != 0))
            {
                shiftId--;
            }

            LogHelper.LogTrace("ShtrihPrinter.GetShiftId", "shiftId = {0}", shiftId);
            
            return shiftId.ToString();
        }

        /// <summary>
        /// Retrieves the fiscal printer serial number.
        /// </summary>
        /// <returns>The serial number.</returns>
        public override string GetSerialNumber()
        {
            return _driver.SerialNumber;
        }

        /// <summary>
        /// Retrieves the fiscal printer grand total.
        /// </summary>
        /// <returns>The grand total.</returns>
        public override decimal GetGrandTotal()
        {
            // Is not used under Russian country context.
            return decimal.Zero;
        }

        /// <summary>
        /// Retrieves the fiscal printer subtotal.
        /// </summary>
        /// <returns>The subtotal.</returns>
        public override decimal GetSubtotal()
        {
            CheckResultCode(_driver.CheckSubTotal);

            return _driver.Summ1;
        }

        /// <summary>
        /// Retrieves the user configurable terminal number associated with the fiscal printer.
        /// </summary>
        /// <returns>The terminal number.</returns>
        public override string GetTerminalNumber()
        {
            LogHelper.LogError("ShtrihPrinter.GetTerminalNumber", "Throwing NotSupportedException. OperationState={0}", OperatingState);
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the internal features of the fiscal printer.
        /// </summary>
        /// <param name="featureEnumerator">The index of the internal feature to be retrieved.</param>
        /// <returns>The value of the internal feature.</returns>
        public override string GetInternalFeatureStatus(int featureEnumerator)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Cancels the last payment line.
        /// </summary>
        public override void CancelLastPayment(string paymentMethod, decimal amount)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the fiscal printer balance due.
        /// <remarks>May be invoked after StartTotalPayment.</remarks>
        /// </summary>
        /// <returns>The balance due.</returns>
        public override decimal GetBalanceDue()
        {
            return decimal.Zero;
        }

        /// <summary>
        /// Retrieves the fiscal printer change due.
        /// <remarks>May be invoked after EndReceipt().</remarks>
        /// </summary>
        /// <returns>The change due.</returns>
        public override decimal GetChangeDue()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Begins the receipt.
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="args">Data about the customer.</param>
        public override void BeginReceipt(ReceiptType receiptType, params object[] args)
        {
            LogHelper.LogTrace("ShtrihPrinter.BeginReceipt", "Begin receipt called with receipt type = {0}", receiptType);

            // Reset printer control flags as we are starting a new operation handling sequence.
            ResetControlsVariables();

            ValidatePrinterConnected();

            RetailTransaction transaction = GetRetailTransactionFromArgs(args);
            LayoutType layout = GetLayoutTypeFromReceiptType(receiptType);

            _lastPrintedReceiptlayout = null;
            _totalsHeaderDiscountLines = new List<ITotalsHeaderDiscountLine>();
            _discountSalesLines = new Dictionary<string, IList<ISaleLineItem>>();

            // Call cancel receipt in order to cancel the open receipt if one remains from the previously failed operation.
            // Calling this method is safe as it tries to cancel receipt only if one is in open state.
            CancelReceipt(ReceiptType.None);
            InitCheckTypeFromReceiptType(receiptType);
            SetPrinterParameters(layout);
            ExecutePrintingOperation(_driver.OpenCheck, PrinterOperationType.PrintingOperation);
            FillFiscalMemoryData(PosTransaction.TypeOfTransaction.Sales, FiscalMemoryData, false);
            PrintDocumentHeader(layout, transaction);
        }

        /// <summary>
        /// Cancels the [active] receipt.
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="args">Various arguments passed in.</param>
        /// <remarks>
        /// Calling this method is safe as it first checks whether we have an open receipt in place.
        /// </remarks>
        public override void CancelReceipt(ReceiptType receiptType, params object[] args)
        {
            if (IsReceiptOpened())
            {
                LogHelper.LogTrace("ShtrihPrinter.CancelReceipt", "Cancelling receipt");
                ExecutePrintingOperation(_driver.CancelCheck, PrinterOperationType.PrintingOperation, isLastPrintingOperation: true, startNewOperationSequence: false);
            }
        }

        /// <summary>
        /// Determines whether the Receipt is opened.
        /// </summary>
        /// <returns>A Boolean determining whether the receipt is opened.</returns>
        /// <remarks>Makes a decision based on the printer mode.</remarks>
        private bool IsReceiptOpened()
        {
            return GetECRMode() == (int)ECRMode.OpenDocument;
        }

        /// <summary>
        /// Cancels the currently opened document in the printer.  
        /// If there is no opened document, nothing happens.
        /// </summary>
        public override void CancelOpenedDocument(bool reverseCreditDebitReceipt, bool keepFiscalReceipt)
        {
            if (!IsReceiptOpened()) return;

            try
            {
                LogHelper.LogTrace("ShtrihPrinter.CancelOpenedDocument", "Cancelling receipt");
                ExecutePrintingOperation(_driver.CancelCheck, PrinterOperationType.PrintingOperation, isLastPrintingOperation: true, startNewOperationSequence: false);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Adds the item.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment</remarks>
        /// </summary>
        /// <param name="receiptType">Type of the receipt.</param>
        /// <param name="lineItem">Sales line item - contains information about sales line.</param>        
        /// <param name="taxRateId">The tax rate IDs separated by space.</param>        
        /// <returns>The fiscal printer line item number.</returns>
        public override int AddItem(ReceiptType receiptType, SaleLineItem lineItem, string taxRateId)
        {
            if (lineItem == null)
            {
                throw new ArgumentNullException("lineItem");
            }

            LogHelper.LogTrace("ShtrihPrinter.AddItem", "receiptType = {0}, lineItem = {1}, taxRateId = {2}", receiptType, lineItem, taxRateId);

            if (DiscountHelper.SalesLineHasDiscount(lineItem))
            {
                if (!_discountSalesLines.ContainsKey(taxRateId))
                {
                    _discountSalesLines.Add(taxRateId, new List<ISaleLineItem>());
                }

                _discountSalesLines[taxRateId].Add(lineItem);
            }

            LayoutType layoutType = GetLayoutTypeFromReceiptType(receiptType);
            bool fiscalLineWasPrinted = false;
            var lineManager = new LineManager(lineItem);
            var lineCollection = LineManager.GetLineCollection(_config, layoutType, DocumentSectionType.SalesLine);

            foreach (Line line in lineCollection)
            {
                if (line.Type == LineType.SalesFiscal)
                {
                    string itemDescription = line.FieldCollection.Count > 0 ? lineManager.GetTextLine(line) : null;
                    PrintFiscalLine(lineItem, taxRateId, itemDescription);
                    fiscalLineWasPrinted = true;
                }

                if (LineManager.LineTypeIsOfDiscountType(line.Type))
                {
                    if (!fiscalLineWasPrinted)
                    {
                        throw new Exception(Resources.Translate(Resources.DiscountLineNotAllowedBeforeFiscalLine));
                    }

                    PrintSalesDiscountLine(line.Type, lineItem, taxRateId, lineManager.GetTextLine(line));
                }

                if (line.Type == LineType.SalesText &&
                    !lineManager.HideLine(line))
                {
                    PrintTextString(lineManager.GetTextLine(line));
                }                    
            }

            if (!fiscalLineWasPrinted)
            {
                PrintFiscalLine(lineItem, taxRateId);
            }            

            return 0;
        }

        /// <summary>
        /// Removes the item.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment.</remarks>
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="printerItemNumber">The printer item number.</param>
        public override void RemoveItem(ReceiptType receiptType, int printerItemNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Applies the discount.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment.</remarks>
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="adjustmentType">The type of the adjustment.</param>
        /// <param name="printerItemNumber">The printer item number.</param>
        /// <param name="percentToHundredth">The percent to hundredth.</param>
        /// <param name="amount">The amount value.</param>
        /// <param name="description">The description text.</param>
        public override void AddItemPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, int printerItemNumber, decimal amount, string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the discount.
        /// <remarks>Valid after BeginReceipt and before StartTotalPayment.</remarks>
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="adjustmentType">The type of the adjustment.</param>
        /// <param name="printerItemNumber">The printer item number.</param>
        public override void RemoveItemPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, int printerItemNumber)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts the total payment.
        /// </summary>
        public override void StartTotalPayment()
        {
            // Clear all amount properties to be sure in their initial state before handling payments.
            _driver.Summ1 = decimal.Zero;
            _driver.Summ2 = decimal.Zero;
            _driver.Summ3 = decimal.Zero;
            _driver.Summ4 = decimal.Zero;
        }

        /// <summary>
        /// Starts the total payment with surcharge.
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="adjustmentType">The type of the adjustment.</param>
        /// <param name="amount">The amount value.</param>
        /// <param name="description">The description text.</param>
        public override void AddTotalPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType, decimal amount, string description)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Remove the discount from the total payment.
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="adjustmentType">The type of the adjustment.</param>
        public override void RemoveTotalPriceAdjustment(ReceiptType receiptType, AdjustmentType adjustmentType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Makes the payment.
        /// <remarks>Valid after StartTotalPayment and before EndReceipt.</remarks>
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="paymentMethod">The printer payment method.</param>
        /// <param name="paymentValue">The payment value.</param>
        public override void AddPayment(ReceiptType receiptType, string paymentMethod, decimal paymentValue)
        {
            int paymentMethodIndex = int.Parse(paymentMethod); // This is a safe convertion as we know it contains an integer payment type index.

            _driver.Summ1 += paymentMethodIndex == 1 ? System.Math.Abs(paymentValue) : 0m;
            _driver.Summ2 += paymentMethodIndex == 2 ? System.Math.Abs(paymentValue) : 0m;
            _driver.Summ3 += paymentMethodIndex == 3 ? System.Math.Abs(paymentValue) : 0m;
            _driver.Summ4 += paymentMethodIndex == 4 ? System.Math.Abs(paymentValue) : 0m;
        }

        /// <summary>
        /// Prints totals header section.
        /// </summary>
        /// <param name="receiptType">The receipt type.</param>
        /// <param name="retailTransaction">Retail transaction.</param>
        public override void PrintTotalsHeader(ReceiptType receiptType, IRetailTransaction retailTransaction)
        {
            LogHelper.LogTrace("ShtrihPrinter.PrintTotalsHeader", "receiptType = {0}, retailTransaction = {1}", receiptType, retailTransaction);
            LayoutType layoutType = GetLayoutTypeFromReceiptType(receiptType);
            PrintDocumentTextLines(layoutType, DocumentSectionType.TotalsHeader, (RetailTransaction)retailTransaction);
        }
        
        /// <summary>
        /// Ends the receipt.
        /// </summary>
        /// <param name="receiptType">The type of the receipt.</param>
        /// <param name="args">Various arguments.</param>
        public override void EndReceipt(ReceiptType receiptType, params object[] args)
        {
            LogHelper.LogTrace("ShtrihPrinter.EndReceipt", "receiptType = {0}", receiptType);

            RetailTransaction transaction = GetRetailTransactionFromArgs(args);
            LayoutType layoutType = GetLayoutTypeFromReceiptType(receiptType);

            try
            {
                ExecutePrintingOperation(_driver.CloseCheck, PrinterOperationType.CloseReceipt);
            }
            catch (Exception ex)
            {
                // Suppress error if we have already sent an inrreversible command to the printer driver.
                if (IrreversibleCommandSent)
                {
                    LogHelper.LogError("ShtrihPrinter.EndReceipt", ex.Message);
                    return;
                }

                throw;
            }

            try
            {
                PrintDocumentFooter(layoutType, transaction);
                _lastPrintedReceiptlayout = layoutType;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("ShtrihPrinter.EndReceipt Footer", ex.Message);
            }
        }

        /// <summary>
        /// Begins a management report.
        /// </summary>
        /// <param name="title">The printed title of the report.</param>
        public override void ManagementReportBegin(string title)
        {
            ManagementReportPrintLine(title);
        }

        /// <summary>
        /// Prints a line on the management report and
        /// then adds a line feed.
        /// </summary>  
        /// <param name="line">The line to be printed.</param>
        public override void ManagementReportPrintLine(string line)
        {
            ManagementReportPrint(line + "\n");
        }

        /// <summary>
        /// Prints a text on the managment report 
        /// without advancing to the next line.
        /// </summary>  
        /// <param name="text">The text to be printed.</param>
        public override void ManagementReportPrint(string text)
        {
            PrintTextString(text);
        }

        /// <summary>
        /// Ends the management report.
        /// </summary>
        public override void ManagementReportEnd()
        {
            // We do not use this method under Russian country context.
        }

        /// <summary>
        /// Generates the Sintegra and NFP files for the given date range and stores it on the given file name.
        /// </summary>
        /// <param name="startDate">The start date range.</param>
        /// <param name="endDate">The end date range.</param>
        /// <param name="fileName">The file name that will be generated.</param>
        public override void GenerateZReportFiles(DateTime startDate, DateTime endDate, string fileName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Prints the non-fiscal receipt for money drops.
        /// </summary>
        /// <param name="total">The amount being removed from POS.</param>
        /// <param name="message">The message to be printed in the non-fiscal receipt.</param>
        public override void PrintMoneyDrop(decimal total, string message)
        {
            const LayoutType layout = LayoutType.TenderRemoval;

            LogHelper.LogTrace("ShtrihPrinter.PrintMoneyDrop", "total = {0}, message = {1}", total, message);

            // Reset control flags at the beginning of every compound operation, i.e., an operation that consists of a driver operation call sequence.
            ResetControlsVariables();
            ValidatePrinterConnected();
            SetPrinterParameters(layout);
            PrintDocumentHeader(layout);
            FillFiscalMemoryData(PosTransaction.TypeOfTransaction.RemoveTender, FiscalMemoryData, false);
            _driver.Summ1 = total;
            ExecutePrintingOperation(_driver.CashOutcome, PrinterOperationType.PrintReport);
            PrintDocumentFooter(layout);
        }

        /// <summary>
        /// Prints the non-fiscal receipt for float entries.
        /// </summary>
        /// <param name="total">The amount being added to POS.</param>
        /// <param name="message">The message to be printed in the non-fiscal receipt.</param>
        public override void PrintMoneyEntry(decimal total, string message)
        {
            const LayoutType layout = LayoutType.FloatEntry;

            LogHelper.LogTrace("ShtrihPrinter.PrintMoneyEntry", "total = {0}, message = {1}", total, message);

            // Reset control flags at the beginning of every compound operation, i.e., operation that consists of a driver operation call sequence.
            ResetControlsVariables();
            ValidatePrinterConnected();
            SetPrinterParameters(layout);
            PrintDocumentHeader(layout);
            FillFiscalMemoryData(PosTransaction.TypeOfTransaction.FloatEntry, FiscalMemoryData, false);
            _driver.Summ1 = total;
            ExecutePrintingOperation(_driver.CashIncome, PrinterOperationType.PrintReport);
            PrintDocumentFooter(layout);
        }

        /// <summary>
        /// Prints the starting amount declaration receipt.
        /// </summary>
        /// <param name="total">The starting amount.</param>
        /// <param name="message">The message to be printed in the receipt.</param>
        public override void PrintStartingAmount(decimal total, string message)
        {
            const LayoutType layout = LayoutType.StartAmount;

            LogHelper.LogTrace("ShtrihPrinter.PrintStartingAmount", "total = {0}, message = {1}", total, message);

            // Reset control flags at the beginning of every compound operation, i.e., an operation that consists of a driver operation call sequence.
            ResetControlsVariables();
            ValidatePrinterConnected();
            SetPrinterParameters(layout);
            PrintDocumentHeader(layout);
            FillFiscalMemoryData(PosTransaction.TypeOfTransaction.StartingAmount, FiscalMemoryData, false);
            _driver.Summ1 = total;
            ExecutePrintingOperation(_driver.CashIncome, PrinterOperationType.PrintReport);
            PrintDocumentFooter(layout);
        }

        /// <summary>
        /// Opens a new Credit/debit document.
        /// </summary>
        /// <param name="tenderType">The tender type used.</param>
        /// <param name="installmentCount">The installment count.</param>
        /// <param name="originalDocumentOperationNumber">The original document operation number.</param>
        /// <param name="transactionValue">The transaction value.</param>
        public override void CreditDebitReceiptBegin(string tenderType, int installmentCount, string originalDocumentOperationNumber, decimal transactionValue)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Print text on the Credit/debit document.
        /// </summary>
        /// <param name="text">Text to be printed.</param>
        public override void CreditDebitReceiptPrintText(string text)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Ends the current Credit/debit document.
        /// </summary>
        public override void CreditDebitReceiptEnd()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Retrieves the fiscal printer's internal clock date and time.
        /// </summary>
        /// <returns>The current date and time.</returns>
        public override DateTime GetDateTime()
        {
            QueryPrinterStatus();

            DateTime time = _driver.Time;
            DateTime date = _driver.Date;

            DateTime dateTime = new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second, time.Millisecond);

            LogHelper.LogTrace("ShtrihPrinter.GetDateTime", "Printer DateTime = {0}", dateTime.ToString(CultureInfo.CreateSpecificCulture("ru-RU")));

            return dateTime;
        }

        /// <summary>
        /// Generates the Sintegra and NFP files.
        /// </summary>
        /// <param name="reportEnumeration">Specifies the report to be generated.</param>
        /// <param name="args">Each report should have its own set of parameters.</param>
        /// <returns>The file name of the report, if applicable.</returns>
        public override string GenerateReport(int reportEnumeration, params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            LogHelper.LogTrace("ShtrihPrinter.GenerateReport", "reportType = {0}", (RussianReports)reportEnumeration);

            switch ((RussianReports)reportEnumeration)
            {
                case RussianReports.TaxReport:
                    ExecutePrintingOperation(_driver.PrintTaxReport, PrinterOperationType.PrintReport, isLastPrintingOperation: true);
                    break;
                case RussianReports.OperationalTaxReport:
                    ExecutePrintingOperation(_driver.PrintOperationReg, PrinterOperationType.PrintReport, isLastPrintingOperation: true);
                    break;
            }

            return string.Empty;
        }

        /// <summary>
        /// Verifies if the printer is ready to begin printing fiscal documents.
        /// </summary>
        /// <returns>True if the printer is ready; false otherwise.</returns>
        public override bool IsPrinterReady()
        {
            return IsPrinterConnected();
        }

        private bool IsPrinterConnected()
        {
            bool isNotConnected = false;

            try
            {
                // We set 'Not connected' flag here in order to trick exception handling mechanism and make it supress showing the 'Not connected' error message to the user, as we need to query it silently.
                IsNotConnected = true;
                ValidatePrinterConnected();
            }
            catch (FiscalPrinterException)
            {
                isNotConnected = IsNotConnected;
            }

            return !isNotConnected;
        }

        /// <summary>
        /// Throws exception if the printer is not connected; does nothing otherwise.
        /// </summary>
        private void ValidatePrinterConnected()
        {
            CheckResultCode(_driver.GetDeviceMetrics);
        }

        /// <summary>
        /// Retrieves the value of one of the internal counters of the fiscal printer.
        /// </summary>
        /// <param name="counterEnumerator">The index of the counter.</param>
        /// <returns>The counter value.</returns>
        public override string GetCounterNumber(int counterEnumerator)
        {
            _driver.RegisterNumber = counterEnumerator;
            CheckResultCode(_driver.GetOperationReg);

            return _driver.ContentsOfOperationRegister.ToString();
        }

        /// <summary>
        /// Retrieves the fiscal printer transaction date.
        /// </summary>
        /// <returns>The transaction date.</returns>
        public override DateTime GetTransactionDate()
        {
            throw new NotSupportedException();
        }


        /// <summary>
        /// Retrieves the fiscal printer accounting date.
        /// </summary>
        /// <returns>The accounting date.</returns>
        public override DateTime GetAccountingDate()
        {
            throw new NotSupportedException();
        }

        public override int ReceiptOperationRegisterNumber
        {
            get
            {
                return ShtrihConstants.ReceiptOperationRegisterNumberValue;
            }
        }

        public override PrintingReadinessStatus GetPrintingReadinessStatus()
        {
            QueryShortPrinterStatus();

            PrintingReadinessStatus status = PrintingReadinessStatus.Ready;

            switch (_driver.ECRAdvancedMode)
            {
                case (int)ECRAdvancedMode.HasPaperNoPrinting:
                    status = PrintingReadinessStatus.Ready;
                    break;
                case (int)ECRAdvancedMode.PassiveOutOfPaper:
                    status = PrintingReadinessStatus.PassiveOutOfPaper;
                    break;
                case (int)ECRAdvancedMode.ActiveOutOfPaper:
                    status = PrintingReadinessStatus.ActiveOutOfPaper;
                    break;
                case (int)ECRAdvancedMode.AwaitingContinuePrinting:
                    status = PrintingReadinessStatus.AwaitingContinuePrinting;
                    break;
                case (int)ECRAdvancedMode.PrintingOperation:
                case (int)ECRAdvancedMode.LongReportPrinting:
                    status = PrintingReadinessStatus.BusyPrinting;
                    break;
                default:
                    throw new Exception(Resources.Translate(Resources.InvalidPrintingSubMode, _driver.ECRAdvancedMode));
            }

            return status;
        }

        public override string ResultCodeDescription
        {
            get { return _driver.ResultCodeDescription; }
        }

        /// <summary>
        /// Gets the printer 'Not connected' error codes set.
        /// </summary>
        public override ISet<int> PrinterIsNotConnectedErrorCodes
        {
            get
            {
                return _notConnectedErrorCodes;
            }
        }

        /// <summary>
        /// Gets succsess result code.
        /// </summary>
        public override int SuccessResultCode
        {
            get { return ShtrihConstants.ErrorCodes.Success; }
        }

        /// <summary>
        /// Gets an error code integer value, indicating that the shift is open more than 24 hours.
        /// </summary>
        public override int ShiftIsOpenMoreThan24HoursErrorCode
        {
            get { return ShtrihConstants.ErrorCodes.CommandIsNotSupportedInThisMode; }
        }

        public override void ResumePrinting()
        {
            CheckResultCode(_driver.ContinuePrint, PrinterOperationType.ContinueOperation);
        }

        public override void RibbonFeed()
        {
            _driver.StringQuantity = FeedLinesCount;
            CheckResultCode(_driver.FeedDocument, PrinterOperationType.PrintingOperation);
        }

        /// <summary>
        /// Prints a copy of the last fiscal document.
        /// </summary>
        public override void PrintLastReceiptCopy()
        {
            bool printFooter = _lastPrintedReceiptlayout != null;

            LogHelper.LogTrace("ShtrihPrinter.PrintLastReceiptCopy", "Print last receipt copy called.");

            ExecutePrintingOperation(_driver.RepeatDocument, PrinterOperationType.PrintingOperation, isLastPrintingOperation: !printFooter);

            if (printFooter)
            {
                PrintDocumentFooter(_lastPrintedReceiptlayout.Value);
            }
        }

        public override bool IsCashPaymentType(string paymentTypeId)
        {
            return int.Parse(paymentTypeId) == ShtrihConstants.CashPaymentTypeIndex;
        }

        public override void PrintLoyaltyCardBalance(ILoyaltyCardData loyaltyCardData)
        {
            LogHelper.LogTrace("ShtrihPrinter.PrintLoyaltyCardBalance", "loyaltyCardData = {0}", loyaltyCardData);
            PrintDocumentLogo(LayoutType.LoyaltyCardBalance);
            PrintLoyaltyCardBalanceDocument(loyaltyCardData);
            CutReceipt();
        }

        /// <summary>
        /// Should the gift card payment be processed as discount by the fiscal printer.
        /// </summary>
        /// <returns>True if gift card payment should be processed as discount; otherwise false.</returns>
        public override bool ProcessGiftCardPaymentAsDiscount()
        {
            LayoutType layoutType = (_driver.CheckType == (int)CheckType.Sales) ? LayoutType.Sale : LayoutType.Return;

            return LineManager.GetLineCollection(_config, layoutType, DocumentSectionType.TotalsHeader).Any(l => l.Type == LineType.GiftCardDiscount);
        }

        #endregion

        #region Private helper methods
        /// <summary>
        /// Prints text string.
        /// </summary>
        /// <param name="textToPrint">Text to be printed out.</param>
        private void PrintTextString(string textToPrint)
        {
            if (!string.IsNullOrEmpty(textToPrint))
            {
                foreach (var s in textToPrint.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    for (int i = 0; i <= (s.Length - 1) / ReceiptLineLength; i++)
                    {
                        _driver.StringForPrinting = s.Substring(i * ReceiptLineLength, Math.Min(s.Length - i * ReceiptLineLength, ReceiptLineLength));
                        ExecutePrintingOperation(_driver.PrintString, PrinterOperationType.PrintingOperation);
                    }
                }
            }

            _driver.StringForPrinting = string.Empty;
        }

        /// <summary>
        /// Finds layout by type.
        /// </summary>
        /// <param name="layout">The layout type to search for.</param>
        /// <param name="strictMatch">Determines whether the match must be strict; false by default.</param>
        /// <returns>Found layout; null if not found.</returns>
        private Layout FindLayout(LayoutType layout, bool strictMatch = false)
        {
            return _config.Layouts.FindLayout(layout, strictMatch);
        }

        /// <summary>
        /// Prints image.
        /// </summary>
        /// <param name="imageId">Image ID</param>
        private void PrintImage(string imageId)
        {
            var printerImage = _imageController.GetPrinterImage(imageId);
            _driver.FirstLineNumber = printerImage.StartLine;
            _driver.LastLineNumber = printerImage.EndLine;
            ExecutePrintingOperation(_driver.DrawEx, PrinterOperationType.PrintingOperation);
        }

        /// <summary>
        /// Loads a 'bmp' file to printer.
        /// </summary>
        /// <param name="fileName">Filename</param>
        /// <param name="centerImage">Denotes that the image should be centered.</param>
        public void LoadImageToPrinter(string fileName, bool centerImage)
        {
            _driver.FileName = fileName;
            _driver.ShowProgress = false;
            _driver.CenterImage = centerImage;
            CheckResultCode(_driver.LoadImage);
        }


        /// <summary>
        /// Prints document header.
        /// </summary>
        /// <param name="layout">Type of the layout.</param>
        /// <param name="transaction">Transaction containing the context for ptinting.</param>
        private void PrintDocumentHeader(LayoutType layout, RetailTransaction transaction = null)
        {
            _lastPrintedReceiptlayout = null;
            PrintDocumentLogo(layout);
            PrintDocumentTextLines(layout, DocumentSectionType.Header, transaction);                       
        }

        
        /// <summary>
        /// Cuts receipt
        /// </summary>
        private void CutReceipt()
        {
            RibbonFeed();
            ExecutePaperCut(false);
        }

        /// <summary>
        /// Prints document footer.
        /// </summary>
        /// <param name="layout">Type of the layout.</param>
        /// <param name="transaction">Transaction containing the context for ptinting.</param>
        private void PrintDocumentFooter(LayoutType layout, RetailTransaction transaction = null)
        {
            PrintDocumentTextLines(layout, DocumentSectionType.Footer, transaction);
            CutReceipt();
        }

        /// <summary>
        /// Prints loyalty card balance document.
        /// </summary>
        /// <param name="loyaltyCardData">The <see cref="LoyaltyCardData"/> object containing the information about loyalty card.</param>
        private void PrintLoyaltyCardBalanceDocument(ILoyaltyCardData loyaltyCardData)
        {
            if (FindLayout(LayoutType.LoyaltyCardBalance, true) == null)
            {
                throw new Exception(Resources.Translate(Resources.LayoutOfTypeIsNotDefinedInTheConfigurationFile, LayoutType.LoyaltyCardBalance));
            }

            LineManager lineManager = new LineManager(loyaltyCardData, DateTime.Now);

            foreach (Line lineConfiguration in LineManager.GetLineCollection(_config, LayoutType.LoyaltyCardBalance, DocumentSectionType.SimpleSection).Where(_ => _.Type == LineType.Text))
            {
                PrintTextLine(lineManager, lineConfiguration);
            }
        }

        /// <summary>
        /// Prints text line on the printer.
        /// </summary>
        /// <param name="lineManager">The <see cref="LineManager"/> obejct.</param>
        /// <param name="lineConfiguration">The <see cref="Line"/> object.</param>
        private void PrintTextLine(LineManager lineManager, Line lineConfiguration)
        {
            if (!lineManager.HideLine(lineConfiguration))
            {
                PrintTextString(lineManager.GetTextLine(lineConfiguration));
            }
        }

        /// <summary>
        /// Prints Document logo.
        /// </summary>
        /// <param name="layout">Type of the layout.</param>
        private void PrintDocumentLogo(LayoutType layout)
        {
            if (_config != null)
            {
                var configLayout = FindLayout(layout);
                if (configLayout != null)
                {
                    //Print Logo
                    if (!string.IsNullOrWhiteSpace(configLayout.ImageId))
                    {
                        PrintImage(configLayout.ImageId);
                    }
                }
            }
        }

        /// <summary>
        /// Prints text lines for specific section of the layout.
        /// </summary>
        /// <param name="layout">Type of the layout.</param>
        /// <param name="documentSectionType">Document section for printing.</param>
        /// <param name="transaction">Retail transaction.</param>        
        private void PrintDocumentTextLines(LayoutType layout, DocumentSectionType documentSectionType, RetailTransaction transaction = null)
        {
            var lineManager = new LineManager(transaction);
            LineManager discountLineManager = null;
            var lineCollection = LineManager.GetLineCollection(_config, layout, documentSectionType);

            foreach (Line line in lineCollection)
            {
                if (documentSectionType == DocumentSectionType.TotalsHeader &&
                    LineManager.LineTypeIsOfDiscountType(line.Type))
                {
                    if (discountLineManager == null)
                    {
                        discountLineManager = new LineManager(transaction);
                    }
                    else
                    {
                        discountLineManager.InitDiscountFieldsFromTransaction(transaction);
                    }

                    decimal? roundingDiscount = null;
                    if ((line.Type == LineType.ReceiptDiscount || line.Type == LineType.SummaryDiscount) && line.AddRoundingToDiscount.HasValue && line.AddRoundingToDiscount.Value)
                    {
                        discountLineManager.UpdateDiscountFieldsWithRounding(transaction, line.Type);
                        roundingDiscount = transaction.RoundingSalePmtDiff;
                    }

                    PrintTotalsHeaderDiscountLine(line, discountLineManager, roundingDiscount);
                }
                else if (documentSectionType == DocumentSectionType.TotalsHeader &&
                    line.Type == LineType.RoundingDiscount)
                {
                    if (discountLineManager == null)
                    {
                        discountLineManager = new LineManager(transaction);
                    }
                    discountLineManager.UpdateDiscountFieldsWithRounding(transaction, line.Type);
                    PrintDiscountAsRounding(line, discountLineManager, transaction.RoundingSalePmtDiff);
                }
                else if (line.Type == LineType.Text &&
                    !lineManager.HideLine(line))
                {
                    PrintTextString(lineManager.GetTextLine(line));
                }
                else if (line.Type == LineType.GiftCardDiscount &&
                         ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                {
                    PrintGiftCardPaymentLine(transaction, lineManager.GetTextLine(line));
                }
            }                       
        }

        /// <summary>
        /// Prints paid by gift card line in the totals header section.
        /// </summary>
        /// <param name="transaction">Retail transaction</param>
        /// <param name="text">Text to print</param>
        private void PrintGiftCardPaymentLine(RetailTransaction transaction, string text)
        {
            var giftCardPaymentAmount = GiftCardHelper.GetTransactionGiftCardPaidAmount(transaction);

            if (giftCardPaymentAmount != decimal.Zero)
            {
                SetupPrinterTaxCodes(string.Empty);
                ExecuteDiscount(giftCardPaymentAmount, text);
            }
        }

        /// <summary>
        /// Prints sales fiscal line.
        /// </summary>
        /// <param name="lineItem">Sales line item - contains information about sales line.</param>        
        /// <param name="taxRateId">Tax rate IDs separated by space.</param>                   
        /// /// <param name="fiscalLineDescription">Text to print in fiscal line.</param>               
        private void PrintFiscalLine(SaleLineItem lineItem, string taxRateId, string fiscalLineDescription = null)
        {
            if (fiscalLineDescription == null)
            {
                fiscalLineDescription = lineItem.Description;            
            }

            _driver.StringForPrinting = FieldManager.Truncate(fiscalLineDescription, ReceiptLineLength);
            _driver.Price = lineItem.Price;
            _driver.Quantity = Convert.ToDouble(System.Math.Abs(lineItem.Quantity));

            // We do not handle different departments for now.
            _driver.Department = 1;

            SetupPrinterTaxCodes(taxRateId);

            var prevTaxValues = GetCalculatedTaxValuesFromFiscalPrinter();

            switch (_driver.CheckType)
            {
                case (int)CheckType.Sales:
                    ExecuteSale();
                    break;
                case (int)CheckType.ReturnSales:
                    ExecuteReturnSale();
                    break;
            }

            var curTaxValues = GetCalculatedTaxValuesFromFiscalPrinter();
            InitializeTaxAmounts(prevTaxValues, curTaxValues);
        }

        /// <summary>
        /// Sets up printer tax codes.
        /// </summary>
        /// <param name="taxRateId">Tax rate IDs separated by space.</param>                   
        private void SetupPrinterTaxCodes(string taxRateId)
        {
            _driver.Tax1 = 0;
            _driver.Tax2 = 0;
            _driver.Tax3 = 0;
            _driver.Tax4 = 0;

            if (!string.IsNullOrEmpty(taxRateId))
            {
                int i = 1;
                foreach (var s in taxRateId.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                {
                    switch (i)
                    {
                        case 1:
                            _driver.Tax1 = int.Parse(s);
                            break;
                        case 2:
                            _driver.Tax2 = int.Parse(s);
                            break;
                        case 3:
                            _driver.Tax3 = int.Parse(s);
                            break;
                        case 4:
                            _driver.Tax4 = int.Parse(s);
                            break;
                    }
                    i++;
                }
            }
        }

        /// <summary>
        /// Prints discount line in the sales line document section.
        /// </summary>
        /// <param name="receiptType">Type of the receipt.</param>
        /// <param name="lineItem">Sales line item - contains information about sales line.</param>        
        /// <param name="taxRateId">Tax rate IDs separated by space.</param>                   
        /// <param name="discountText">Text to print in discount line; optional.</param>               
        private void PrintSalesDiscountLine(LineType lineType, SaleLineItem lineItem, string taxRateId, string discountText = null)
        {
            if (discountText == null)
            {
                // Print line item description if no discount text is provided.
                discountText = lineItem.Description;
            }

            decimal discountAmount = DiscountHelper.GetSalesLineDiscountAmount(LineType2DiscountType(lineType), lineItem);

            if (discountAmount == decimal.Zero) return;

            SetupPrinterTaxCodes(taxRateId);

            var prevTaxValues = GetCalculatedTaxValuesFromFiscalPrinter();
            ExecuteDiscount(discountAmount, discountText);
            var curTaxValues = GetCalculatedTaxValuesFromFiscalPrinter();

            UpdateTaxAmounts(prevTaxValues, curTaxValues);
        }

        /// <summary>
        /// Prints discount line in the totals header section.
        /// </summary>
        /// <param name="line">Configuration line.</param>
        /// <param name="lineManager">Line manager.</param>
        /// <param name="rounding">Rounding to be added to discount.</param>
        private void PrintTotalsHeaderDiscountLine(Line line, LineManager lineManager, decimal? rounding)
        {
            DiscountType discountType = LineType2DiscountType(line.Type);

            if ((line.Type == LineType.ReceiptDiscount || line.Type == LineType.SummaryDiscount) 
                && line.AddRoundingToDiscount.HasValue && line.AddRoundingToDiscount == true
                && _discountSalesLines.Count == 0 && rounding.HasValue)
            {
                PrintDiscountAsRounding(line, lineManager, rounding.Value);
            }

            foreach (var discount in _discountSalesLines)
            {
                decimal discountAmount = discount.Value.Sum(salesLine => DiscountHelper.GetSalesLineDiscountAmount(discountType, salesLine));

                decimal discountAmountWithRounding = discountAmount;
                if (rounding.HasValue)
                    discountAmountWithRounding += rounding.Value;

                if (discountAmountWithRounding == decimal.Zero) continue;

                lineManager.UpdateDiscountAmountFieldValue(discountType, Math.Abs(discountAmountWithRounding));

                SetupPrinterTaxCodes(discount.Key);

                var prevTaxValues = GetCalculatedTaxValuesFromFiscalPrinter();

                ExecuteDiscount(discountAmountWithRounding, lineManager.GetTextLine(line));
                var curTaxValues = GetCalculatedTaxValuesFromFiscalPrinter();

                var discountTaxAmounts = new Dictionary<string, decimal>();

                for (int i = 0; i < ShtrihConstants.MaximumNumberOfTaxCodesSupported; i++)
                {
                    discountTaxAmounts.Add((i + 1).ToString(), curTaxValues[i] - prevTaxValues[i]);
                }

                var discountLine = new TotalsHeaderDiscountLine() { DiscountType = discountType, PrinterTaxGroup = discount.Key, DiscountAmount = discountAmount, DiscountTaxAmounts = discountTaxAmounts };

                _totalsHeaderDiscountLines.Add(discountLine);
            }
        }

        private void PrintDiscountAsRounding(Line line, LineManager lineManager, decimal paymentDiff)
        {
            if (paymentDiff == decimal.Zero)
                return;

            ExecuteDiscount(paymentDiff, lineManager.GetTextLine(line));
        }

        /// <summary>
        /// Converts line type to discount type.
        /// </summary>
        /// <param name="lineType">Line type</param>
        /// <returns>Discount type</returns>
        private static DiscountType LineType2DiscountType(LineType lineType)
        {
            DiscountType discountType;

            switch (lineType)
            {
                case LineType.SummaryDiscount:
                    discountType = DiscountType.SummaryDiscount;
                    break;
                case LineType.LineDiscount:
                    discountType = DiscountType.LineDiscount;
                    break;
                case LineType.PeriodicDiscount:
                    discountType = DiscountType.PeriodicDiscount;
                    break;
                case LineType.ReceiptDiscount:
                    discountType = DiscountType.ReceiptDiscount;
                    break;
                case LineType.LoyaltyDiscount:
                    discountType = DiscountType.LoyaltyDiscount;
                    break;
                default:
                    throw new ArgumentException();
            }

            return discountType;
        }

        /// <summary>
        /// Retrieves the information about table field (data type, name etc).
        /// </summary>
        /// <param name="tableNumber">The number of the table.</param>
        /// <param name="fieldNumber">The number of the field.</param>
        /// <returns>Field information</returns>
        public ShtrihMFieldInfo GetTableFieldInfo(int tableNumber, int fieldNumber)
        {
            _driver.TableNumber = tableNumber;
            _driver.FieldNumber = fieldNumber;
            CheckResultCode(_driver.GetFieldStruct);

            return new ShtrihMFieldInfo(_driver.FieldName,
                _driver.FieldType ? PrinterParameterType.String : PrinterParameterType.Integer,
                (byte)_driver.FieldSize, _driver.MINValueOfField, _driver.MAXValueOfField);
        }
       
        /// <summary>
        /// Performs the read operation from the table row field without returning any value.
        /// </summary>
        /// <param name="tableNumber">The number of the table.</param>
        /// <param name="rowNumber">The number of the row.</param>
        /// <param name="fieldNumber">The number of the field.</param>
        private void ReadTableFieldValue(int tableNumber, int rowNumber, int fieldNumber)
        {
            _driver.TableNumber = tableNumber;
            _driver.RowNumber = rowNumber;
            _driver.FieldNumber = fieldNumber;
            CheckResultCode(_driver.ReadTable);
        }

        /// <summary>
        /// Writes a string value to the table row cell.
        /// </summary>
        /// <param name="tableNumber">The number of the table to write into.</param>
        /// <param name="rowNumber">The number of the row to write into.</param>
        /// <param name="fieldNumber">The number of the field (column) to write into.</param>
        /// <param name="value">The string value to be written.</param>
        public void WriteTableFieldValueOfString(int tableNumber, int rowNumber, int fieldNumber, string value)
        {
            _driver.TableNumber = tableNumber;
            _driver.RowNumber = rowNumber;
            _driver.FieldNumber = fieldNumber;
            _driver.ValueOfFieldString = value;
            CheckResultCode(_driver.WriteTable);
        }

        /// <summary>
        /// Writes a integer value to the table row cell.
        /// </summary>
        /// <param name="tableNumber">The number of the table to write into.</param>
        /// <param name="rowNumber">The number of the row to write into.</param>
        /// <param name="fieldNumber">The number of the field (column) to write into.</param>
        /// <param name="value">The integer value to be written.</param>
        public void WriteTableFieldValueOfInteger(int tableNumber, int rowNumber, int fieldNumber, int value)
        {
            _driver.TableNumber = tableNumber;
            _driver.RowNumber = rowNumber;
            _driver.FieldNumber = fieldNumber;
            _driver.ValueOfFieldInteger = value;
            // WriteTable is treated as a printing operation as it is used in layout setup and having paper related issues affects correct execution of this command, so we should enable handling printer related errors.
            CheckResultCode(_driver.WriteTable, PrinterOperationType.PrintingOperation);
        }

        /// <summary>
        /// Retrieves the integer value of the table row field out of fiscal printer internal memory.
        /// </summary>
        /// <param name="tableNumber">Thew number of the table.</param>
        /// <param name="rowNumber">The number of the row.</param>
        /// <param name="fieldNumber">The number of the field.</param>
        /// <returns>An integer table row field value.</returns>
        private int GetTableFieldValueOfInteger(int tableNumber, int rowNumber, int fieldNumber)
        {
            ReadTableFieldValue(tableNumber, rowNumber, fieldNumber);
            return _driver.ValueOfFieldInteger;
        }

        /// <summary>
        /// Queries the printer for the SHORT status.
        /// </summary>
        /// <remarks>Expected to execute FAST.</remarks>
        private void QueryShortPrinterStatus(bool isPrintingOperation = false)
        {
            CheckResultCode(_driver.GetShortECRStatus, isPrintingOperation ? PrinterOperationType.PrintingOperation : PrinterOperationType.None);
        }

        /// <summary>
        /// Queries the printer for the FULL status.
        /// </summary>
        /// <remarks>Expected to execute SLOW.</remarks>
        private void QueryPrinterStatus()
        {
            CheckResultCode(_driver.GetECRStatus);
        }

        /// <summary>
        /// Gets the printer ECRMode reflecting it's internal state.
        /// </summary>
        /// <returns>Returns the value of the printer ECRMode as an Integer.</returns>
        private int GetECRMode(bool checkPrintingMode = false)
        {
            QueryShortPrinterStatus(checkPrintingMode);
            return _driver.ECRMode;
        }

        /// <summary>
        /// Executes printer Sale operation.
        /// </summary>
        private void ExecuteSale()
        {
            ExecutePrintingOperation(_driver.Sale, PrinterOperationType.PrintingOperation);
            _driver.StringForPrinting = string.Empty;
        }

        /// <summary>
        /// Executes printer ReturnSale operation.
        /// </summary>
        private void ExecuteReturnSale()
        {
            ExecutePrintingOperation(_driver.ReturnSale, PrinterOperationType.PrintingOperation);
            _driver.StringForPrinting = string.Empty;
        }

        /// <summary>
        /// Executes the fiscal printer discount operation.
        /// </summary>
        /// <param name="discountAmount">Discount amount.</param>
        /// <param name="discountText">Text to print in discount line.</param>               
        private void ExecuteDiscount(decimal discountAmount, string discountText)
        {
            _driver.StringForPrinting = FieldManager.Truncate(discountText, ReceiptLineLength);
            _driver.Summ1 = Math.Abs(discountAmount);
            ExecutePrintingOperation(_driver.Discount, PrinterOperationType.PrintingOperation);
            _driver.Summ1 = decimal.Zero;
            _driver.StringForPrinting = string.Empty;
        }

        /// <summary>
        /// Retrives the calculated tax values from the fiscal printer.
        /// </summary>
        /// <returns>
        /// An array with calculated tax amounts.
        /// </returns>
        private decimal[] GetCalculatedTaxValuesFromFiscalPrinter()
        {
            int numOfCheckTypes = Enum.GetValues(typeof(CheckType)).Length;
            int firstTaxRegisterNumber = ShtrihConstants.TaxCashRegisterNumberValue + _driver.CheckType;
            decimal[] taxValues = new decimal[ShtrihConstants.MaximumNumberOfTaxCodesSupported];

            for (int i = 0; i < ShtrihConstants.MaximumNumberOfTaxCodesSupported; i++)
            {
                _driver.RegisterNumber = firstTaxRegisterNumber + i * numOfCheckTypes;
                CheckResultCode(_driver.GetCashReg);
                taxValues[i] = _driver.ContentsOfCashRegister;
            }

            return taxValues;
        }

        /// <summary>
        /// Validates tax calculation mode is supported.
        /// </summary>
        /// <remarks>
        /// Throws <see cref="FiscalPrinterException"/> if tax calculation mode is not supported.
        /// </remarks>
        private void ValidateTaxModeIsSupported()
        {
            if (GetTableFieldValueOfInteger(1, 1, ShtrihConstants.TaxCalculationModePrinterParameter) != 0)
            {
                throw new Exception(Resources.Translate(Resources.PrinterTaxModeNotSupported));
            }
        }

        /// <summary>
        /// Apply receiptLineLength value from config file in case ReceiptLineLength is specified correctly. Otherwise it's taken from printer driver
        /// </summary>
        private void InitializeReceiptLineLength()
        {
            //Take a real value from a driver to check that configured value doesn't exceed it
            ReceiptLineLength = CalcLineLengthFromDriver();
            if (_config != null && _config.RibbonSettings != null && _config.RibbonSettings.ReceiptLineLength.HasValue)
            {
                int lineLengthFromConfig = _config.RibbonSettings.ReceiptLineLength.Value;

                if (lineLengthFromConfig < 1)
                {
                    string warningMessage = Resources.Translate(Resources.ReceiptLineLengthLessThan1);
                    LogHelper.LogError("ShtrihPrinter.InitializeReceiptLineLength", warningMessage);
                    UserMessages.ShowWarning(Resources.Translate(Resources.FailedToLoadConfigFile, warningMessage));
                }
                else if (lineLengthFromConfig > ReceiptLineLength)
                {
                    string warningMessage = Resources.Translate(Resources.ReceiptLineLengthGreaterThanDriverValue);
                    LogHelper.LogError("ShtrihPrinter.InitializeReceiptLineLength", warningMessage);
                    UserMessages.ShowWarning(Resources.Translate(Resources.FailedToLoadConfigFile, warningMessage));
                }
                else
                {
                    ReceiptLineLength = lineLengthFromConfig;
                }
            }
        }

        /// <summary>
        /// Calculates max line length for default printer font 
        /// </summary>
        /// <returns></returns>
        private int CalcLineLengthFromDriver()
        {
            _driver.FontType = ShtrihConstants.FontType;
            CheckResultCode(_driver.GetFontMetrics);
            LogHelper.LogTrace("CalcLineLengthFromDriver", "PrintWidth from printer driver is {0}", _driver.PrintWidth);
            LogHelper.LogTrace("CalcLineLengthFromDriver", "CharWidth from printer driver is {0}", _driver.CharWidth);
            int lineLength = _driver.PrintWidth / _driver.CharWidth;
            return lineLength;
        }

        /// <summary>
        /// Initializes tax amounts dictionary with the calculated tax values from the fiscal printer.
        /// </summary>
        /// <param name="prevTaxValues">Fiscal printer tax registers values before fiscal operation.</param>
        /// <param name="curTaxValues">Fiscal printer tax registers values after fiscal operation.</param>
        private void InitializeTaxAmounts(decimal[] prevTaxValues, decimal[] curTaxValues)
        {
            _taxAmounts = new Dictionary<string, decimal>(ShtrihConstants.MaximumNumberOfTaxCodesSupported);

            for (int i = 0; i < ShtrihConstants.MaximumNumberOfTaxCodesSupported; i++)
            {
                _taxAmounts.Add((i + 1).ToString(), curTaxValues[i] - prevTaxValues[i]);
            }
        }

        /// <summary>
        /// Updates tax amounts dictionary with the calculated tax values from the fiscal printer.
        /// </summary>
        /// <param name="prevTaxValues">Fiscal printer tax registers values before fiscal operation.</param>
        /// <param name="curTaxValues">Fiscal printer tax registers values after fiscal operation.</param>
        private void UpdateTaxAmounts(decimal[] prevTaxValues, decimal[] curTaxValues)
        {
            for (int i = 0; i < ShtrihConstants.MaximumNumberOfTaxCodesSupported; i++)
            {
                _taxAmounts[(i + 1).ToString()] +=  curTaxValues[i] - prevTaxValues[i];
            }
        }

        /// <summary>
        /// Retrieves the lookup table mapping POS tender types to printer payment types.
        /// </summary>
        /// <returns>
        /// A Dictionary mapping tender type IDs to the printer payment types.
        /// </returns>
        private IDictionary<string, string> GetTenderTypesMappingFromConfig()
        {
            var dictionary = new Dictionary<string, string>();

            if (_config != null && _config.TenderTypesMapping != null)
            {
                foreach (TenderType tenderType in _config.TenderTypesMapping)
                {
                    dictionary.Add(tenderType.TenderTypeId.ToString(), tenderType.PrinterPaymentType.ToString());
                }
            }

            return dictionary;
        }

        /// <summary>
        /// Retrieves <see cref="RetailTransaction"/> from <paramref name="args"/>.
        /// </summary>
        /// <param name="args">Args</param>
        /// <returns>Retail transaction</returns>
        /// <remarks>
        /// Throws <see cref="AgrumentNullException"/> if <paramref name="args"/> is null or empty.
        /// Throws <see cref="ArgumentException"/> if args[0] does not contain <see cref="RetailTransaction"/> .
        /// </remarks>
        private static RetailTransaction GetRetailTransactionFromArgs(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (args.Length == 0)
            {
                throw new ArgumentNullException("args");
            }

            var transaction = args[0] as RetailTransaction;

            if (transaction == null)
            {
                throw new ArgumentException("Missing RetailTransaction argument", "args");
            }

            return transaction;
        }

        /// <summary>
        /// Determines layout type by receipt type.
        /// </summary>
        /// <param name="receiptType">ReceiptType</param>
        /// <returns>Layout type</returns>
        /// <remarks>
        /// Throws <see cref="ArgumentException"/> if <paramref name="receiptType"/> has invalid value.
        /// </remarks>
        private static LayoutType GetLayoutTypeFromReceiptType(ReceiptType receiptType)
        {
            LayoutType layoutType;

            switch (receiptType)
            {
                case ReceiptType.FiscalReceipt:
                    layoutType = LayoutType.Sale;
                    break;
                case ReceiptType.RegularRefund:
                    layoutType = LayoutType.Return;
                    break;
                default:
                    throw new ArgumentException("receiptType");
            }
            
            return layoutType;
        }

        /// <summary>
        /// Initializes driver check type from receipt type.
        /// </summary>
        /// <param name="receiptType">ReceiptType</param>
        /// <remarks>
        /// Throws <see cref="ArgumentException"/> if <paramref name="receiptType"/> has invalid value.
        /// </remarks>
        private void InitCheckTypeFromReceiptType(ReceiptType receiptType)
        {
            CheckType checkType;

            switch (receiptType)
            {
                case ReceiptType.FiscalReceipt:
                    checkType = CheckType.Sales;
                    break;
                case ReceiptType.RegularRefund:
                    checkType = CheckType.ReturnSales;
                    break;
                default:
                    throw new ArgumentException("receiptType");
            }

            _driver.CheckType = (int)checkType;
        }

        /// <summary>
        /// Determines whether the fiscal header printed in the beginning of document or in the end.
        /// </summary>
        /// <returns>True, if fiscal header printed in the beginning</returns>
        private bool IsFiscalHeaderPrintedInTheBeginning()
        {
            return GetTableFieldValueOfInteger(1, 1, 17) == 0;
        }

        #endregion

        #region Configuration accessors
        private bool? cutRibbon;
        /// <summary>
        /// Accessor to the CutRibbon configuration property.
        /// </summary>
        private bool CutRibbon
        {
            get
            {
                if (cutRibbon == null)
                {
                    if (_config != null && _config.RibbonSettings != null)
                    {
                        cutRibbon = _config.RibbonSettings.CutRibbon;
                    }
                    else
                    {
                        cutRibbon = false;
                    }
                }

                return cutRibbon.Value;
            }
        }

        private bool? cutType;
        /// <summary>
        /// Accessor to the CutType configuration property.
        /// </summary>
        private bool CutType
        {
            get
            {
                if (cutType == null)
                {
                    if (_config != null && _config.RibbonSettings != null)
                    {
                        cutType = _config.RibbonSettings.CutType;
                    }
                    else
                    {
                        cutType = false;
                    }
                }

                return cutType.Value;
            }
        }

        private int? feedLinesCount;
        /// <summary>
        /// Accessor to the FeedLinesCount configuration property.
        /// </summary>
        private int FeedLinesCount
        {
            get
            {
                if (feedLinesCount == null)
                {
                    if (_config != null && _config.RibbonSettings != null)
                    {
                        feedLinesCount = _config.RibbonSettings.FeedLinesCount;
                    }
                    else
                    {
                        feedLinesCount = ShtrihConstants.NumberOfFeedLines;
                    }
                }

                return feedLinesCount.Value;
            }
        }
        #endregion
    }
}