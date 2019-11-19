using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Dynamics.Retail.Pos.SimulatedFiscalPrinter;
using Microsoft.Dynamics.Retail.Pos.FiscalPrinterInterfaces;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.Tax;
using System.Reflection;

namespace Microsoft.Dynamics.Retail.Pos.FiscalCore
{
    /// <summary>
    /// Full Lazy singleton for a Fiscal printer operations
    /// 
    /// <remarks>POSHARDWAREPROFILE table may be used for the Fiscal Printer identifier.  If required, AX jobs must me
    /// modified to include this data being sent down for the terminal.</remarks>
    /// </summary>
    public sealed class FiscalPrinterSingleton
    {
        private const string MD5Filename = "MD5Snapshot.txt";
        private const string PosExeFilename = "POS.exe";

        private IFiscalOperations _fiscalPrinter; // The Fisacl Printer
        private Dictionary<string, string> _tenderIdToPaymentMap; // Maps TenderId to printer Payment
        private Dictionary<string, string> _salesTaxCodeToPrinterTaxCodeMap;  // Maps TaxCode to printer Tax Code
        private PersistentPrinterData _printerData; // Printer persistent data
        private Dictionary<string, decimal> _taxRates; // Current tax rates
        private byte[] _posExeMD5; // POS.EXE MD5 computed value

        /// <summary>
        /// Dynamics the load fiscal printer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom")]
        private void DynamicLoadFiscalPrinter()
        {
            try
            {   // Allow the user to override the default printer loaded.

                string assemblyName; // e.g., "SimulatedFiscalPrinter.dll";
                string className; // e.g., "SimulatedFiscalPrinter.MyFiscalPrinter";

                assemblyName = Properties.Settings.Default.FiscalPrinterAssembly;
                className = Properties.Settings.Default.FiscalPrinterClass;

                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(className))
                {   // Try to dynamically load the user specified assembly
                    Assembly theAssembly = Assembly.LoadFrom(assemblyName);
                    _fiscalPrinter = theAssembly.CreateInstance(className) as IFiscalOperations;
                }

            }
            catch (Exception ex)
            {   // Ignore errors 
                Debug.WriteLine(ex.Message);
            }

            if (_fiscalPrinter == null)
            {   // Use default
                _fiscalPrinter = new MyFiscalPrinter();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FiscalPrinterSingleton"/> class.
        /// </summary>
        private FiscalPrinterSingleton()
        {
            DynamicLoadFiscalPrinter();

            _printerData = PersistentPrinterData.PersistedObject();

            SalesLineItemData = new Dictionary<int, LineItemTagalong>();

            _tenderIdToPaymentMap = new Dictionary<string, string>();
            _tenderIdToPaymentMap.Add("1", "Dinheiro");     // Cash
            _tenderIdToPaymentMap.Add("2", "Visa Credito"); // VISA - Credit Card
            _tenderIdToPaymentMap.Add("3", "Visa Debito");  // VISA - Debit Card

            _salesTaxCodeToPrinterTaxCodeMap = new Dictionary<string, string>();
            _salesTaxCodeToPrinterTaxCodeMap.Add("AV_DCST", "TA"); // 12.00%
            _salesTaxCodeToPrinterTaxCodeMap.Add("AV_MDST", "TB"); // 5.0% 
            _salesTaxCodeToPrinterTaxCodeMap.Add("SP_DCST", "TC"); // 5.0%
            // "TD" is 10.00%

            _taxRates = new Dictionary<string, decimal>();

            _posExeMD5 = ComputeFileHash(Path.Combine(FiscalCorePath, PosExeFilename));
        }

        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        /// <value>The instance.</value>
        public static FiscalPrinterSingleton Instance
        {
            get { return Nested.instance; }
        }

        /// <summary>
        /// Gets the fiscal core DLL path (i.e., get this assemblies path).
        /// </summary>
        /// <value>The fiscal core path.</value>
        public static string FiscalCorePath
        {
            get 
            {
                string currentAssembly = System.Reflection.Assembly.GetExecutingAssembly().Location;

                return Path.GetDirectoryName(currentAssembly);
            }
        }

        /// <summary>
        /// Gets the pos exe MD5. 
        /// <remarks>May return NULL if there was an error reading the value.</remarks>
        /// </summary>
        /// <value>The pos exe MD5.</value>
        public byte[] GetPosExeMD5()
        {
            return _posExeMD5.Clone() as byte[];
        }


        /// <summary>
        /// Gets the fiscal printer.
        /// </summary>
        /// <value>The fiscal printer.</value>
        public IFiscalOperations FiscalPrinter
        {
            get { return _fiscalPrinter; }
        }

        /// <summary>
        /// Gets the persisted printer data.  This object is used to keep track of printer data
        /// and is persited accross mutliple runs of the application.
        /// </summary>
        /// <value>The printer data.</value>
        public PersistentPrinterData PrinterData
        {
            get { return _printerData; }
        }


        /// <summary>
        /// Gets or sets the sales line item data.  This provides a centrallized location for
        /// keeping track of additional "tag along" data with the transacation sales line items.
        /// </summary>
        /// <value>The sales line item data.</value>
        public Dictionary<int, LineItemTagalong> SalesLineItemData { get; private set; }

        /// <summary>
        /// Sets the tax rate from printer.
        /// </summary>
        public void SetTaxRateFromPrinter()
        {
            _taxRates = FiscalPrinter.GetTaxRates();
        }

        /// <summary>
        /// Finds the tax rate id for the given line item.  Attempts to map this to the printers set
        /// of tax rate identifies.  Ideally, AX would be configured with the same tax rate identifiers
        /// in the printer (thus no additional mapping woudl be required).
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <returns></returns>
        private string FindTaxRateId(SaleLineItem lineItem)
        {
            string result = null;

            foreach (TaxItem taxItem in lineItem.TaxLines)
            {
                // Ideally the "Sales Tax Code" could be configured to match the printer, such as
                // "TA", "TB", "TC" ...
                // However, if it is not configured as such, we can use a mapping function as in this
                // example.  
                // Since the printer only supports 1 tax, we will exit once the first match is found...
                Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "TaxCode: {0}", taxItem.TaxCode));

                if (!taxItem.Exempt)
                {   // Only consider tax if it is not exempt.

                    if (_salesTaxCodeToPrinterTaxCodeMap.ContainsKey(taxItem.TaxCode))
                    {   // Match is found... save results and exit.
                        result = _salesTaxCodeToPrinterTaxCodeMap[taxItem.TaxCode];

                        if (_taxRates[result] == taxItem.Percentage)
                        {   // Unsupported tax mapping in the DB to the sample addin.
                            Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Invalid mapping of tax code to printer {0} != {1}", _taxRates[result], taxItem.Percentage));
                            //Debug.Fail("Invalid mapping of tax code to printer... Check configuration in AX");
                        }

                        // Fiscal printer only supports one tax - so once found return result.
                        break;
                    }
                }
            }

            // Need to ensure AX is configured properly.  Sample will use default
            // to cover cases where it is not.  Production code would need to fail the operation.
            // Two options for specifing the default.  
            // Specify default value in terms of % for the line item tax rate
            // result = lineItem.TaxRatePct.ToString(targetCulture);  
            // Or just use "TA
            return string.IsNullOrEmpty(result) ? "TA" : result;
        }

        /// <summary>
        /// Updates the fiscal coupon sales items qty.
        /// Find all items that are new or changed in quantity and update the information on the 
        /// printer.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        [CLSCompliant(false)]
        public void UpdateFiscalCouponSalesItemsQty(RetailTransaction retailTransaction)
        {
            if (retailTransaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            LinkedList<SaleLineItem> salesLines = retailTransaction.SaleItems;
            IFiscalOperations fiscalPrinter = this.FiscalPrinter;

            foreach (SaleLineItem lineItem in salesLines)
            {   // Check for changes from what was last sent to fiscal printer...

                LineItemTagalong tagalong;

                if (this.SalesLineItemData.ContainsKey(lineItem.LineId))
                {   // Check an existing item for changes...

                    tagalong = this.SalesLineItemData[lineItem.LineId];
                    if (!tagalong.Voided)
                    {
                        // Process any quantity changes...
                        if (lineItem.Quantity != tagalong.PostedQuantity)
                        {   // Update the qty for the item...
                            int newQuantity = (int)lineItem.Quantity;
                            fiscalPrinter.RemoveItem(tagalong.PrinterItemNumber);
                            tagalong.PrinterItemNumber = fiscalPrinter.AddItem(lineItem.ItemId, lineItem.Description, tagalong.PostedPrice, tagalong.TaxRateId, newQuantity);
                            tagalong.PostedQuantity = (decimal)newQuantity;
                        }
                    }
                }
                else
                {   // Add a new item
                    // Update the printer for this change
                    string itemLookupCode = lineItem.ItemId;
                    string itemDescription = lineItem.Description;
                    decimal itemPrice = lineItem.Price;
                    string taxRateId = FindTaxRateId(lineItem); 
                    int itemQuantity = (int)lineItem.Quantity;

                    int printerItemNumber = fiscalPrinter.AddItem(itemLookupCode, itemDescription, itemPrice, taxRateId, itemQuantity);

                    tagalong = new LineItemTagalong(printerItemNumber, itemPrice, (decimal)itemQuantity, taxRateId);
                    this.SalesLineItemData.Add(lineItem.LineId, tagalong);
                }
            }
        }

        /// <summary>
        /// Maps the tender type id to payment method.
        /// </summary>
        /// <param name="tenderTypeId">The tender type id.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string MapTenderTypeIdToPaymentMethod(string tenderTypeId)
        {
            if (tenderTypeId == null)
            {
                throw new ArgumentNullException("tenderTypeId");
            }

            string result = string.Empty;
            string invariantKey = tenderTypeId.ToUpper(System.Globalization.CultureInfo.InvariantCulture);

            if (_tenderIdToPaymentMap.ContainsKey(invariantKey))
            {
                result = _tenderIdToPaymentMap[invariantKey];
            }
            else
            {
                Debug.Fail("No mapping for tenderTypeId found: " + tenderTypeId);
            }

            return result;
        }

        /// <summary>
        /// Computes the file hash.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>Hash or null</returns>
        private static byte[] ComputeFileHash(string filename)
        {
            byte[] result = null;

            try
            {
                FileInfo fi = new FileInfo(filename);
                using (MD5 provider = new MD5CryptoServiceProvider())
                {
                    // Must use FileInfo.OpenRead() to open as read-only
                    using (FileStream stream = fi.OpenRead())
                    {
                        result = provider.ComputeHash(stream);
                    }
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

            return result;
        }

        /// <summary>
        /// Programs the file list.
        /// All files that end with ".EXE" or ".DLL" including sub-folders.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        private static List<string> ProgramFileList(string filePath)
        {
            string[] exeList = Directory.GetFiles(filePath, "*.exe", SearchOption.AllDirectories);
            string[] dllList = Directory.GetFiles(filePath, "*.dll", SearchOption.AllDirectories);

            List<string> fullList = new List<string>();
            fullList.AddRange(exeList);
            fullList.AddRange(dllList);

            return fullList;
        }

        /// <summary>
        /// Saves the MD5 data for all EXE and DLL files for the POS app into a TEXT file.
        /// This can then be used to check for tampering.
        /// </summary>
        public static void SaveProgramListMD5()
        {
            List<string> fullList = ProgramFileList(FiscalCorePath);

            string saveFilename = Path.Combine(FiscalCorePath, MD5Filename);

            try
            {
                using (TextWriter stream = new StreamWriter(saveFilename))
                {

                    foreach (string filename in fullList)
                    {
                        byte[] md5Hash = ComputeFileHash(filename);

                        if (md5Hash != null)
                        {
                            // We write the MD5 and filename/path.  To just write the filename use: Path.GetFileName(filename)
                            stream.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} - {1}", BitConverter.ToString(md5Hash).Replace("-", string.Empty), filename));
                        }
                    }
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        class Nested
        {
            /// <summary>
            /// Initializes the <see cref="Nested"/> class.
            /// Explict static ctor required to ensure compiler does not marek type as "beforefieldinit"
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
            static Nested()
            {
            }

            internal static readonly FiscalPrinterSingleton instance = new FiscalPrinterSingleton();
        }
    }

}
