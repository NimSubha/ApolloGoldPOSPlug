/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// FiscalPrinterDriverFactory class is the entry point for all the call
    /// from the extensibility libraries.  
    /// </summary>
    public static class FiscalPrinterDriverFactory
    {
        public const string PrinterDriversFolder = "FiscalPrinterDrivers";        

        #region Static singleton

        private static class Nested
        {
            /// <summary>
            /// Initializes the <see cref="Nested"/> class.
            /// Explict static ctor required to ensure compiler does not mark type as "beforefieldinit"
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Avoid beforefieldinit")]
            static Nested()
            {
            }

            internal static readonly IFiscalPrinterDriver fiscalPrinterDriver = ConstructInstance();
        }

        private static IFiscalPrinterDriver ConstructInstance()
        {
            IFiscalPrinterDriver fiscalPrinterDriver = null;
            string assemblyName = null;
            string className = null;
            string fullAssemblyPath = null;

            var storeCountry = Functions.CountryRegion.ToString();

            try
            {   // Allow the user to override the default printer loaded.
                assemblyName = (string)Properties.Settings.Default["FiscalPrinterAssembly_" + storeCountry];
                className = (string)Properties.Settings.Default["FiscalPrinterClass_" + storeCountry];

                //Remove invalid chars
                assemblyName = RemoveInvalidAssemblyNameChars(assemblyName);
                assemblyName = Environment.ExpandEnvironmentVariables(assemblyName);
                className = RemoveInvalidClassNameChars(className);

                fullAssemblyPath = Path.IsPathRooted(assemblyName)
                                       ? assemblyName
                                       : Path.Combine(ApplicationSettings.GetAppPath(), PrinterDriversFolder, assemblyName);

                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(className))
                {
                    using (var catalog = new AggregateCatalog())
                    {
                        catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(ApplicationSettings.GetAppPath(), PrinterDriversFolder)));
                        using (var container = new CompositionContainer(catalog))
                        {
                            fiscalPrinterDriver = container.GetExportedValues<IFiscalPrinterDriver>().SingleOrDefault(fpd =>
                                fpd.GetType().Module.Name.ToUpperInvariant().Equals(assemblyName.ToUpperInvariant()) && fpd.GetType().FullName.Equals(className));
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                LogHelper.LogError("FiscalPrinterDriver.ConstructInstance",
                                       "Exception: {0}, AssemblyPath: {1}, ClassName: {2}",
                                       exception.Message, fullAssemblyPath, className);
                fiscalPrinterDriver = null;
            }

            if (fiscalPrinterDriver == null)
            {
                ExceptionHelper.ThrowException(Resources.MessageCouldNotLoadPrinterDriver, fullAssemblyPath, className);
            }

            return fiscalPrinterDriver;
        }

        internal static string RemoveInvalidClassNameChars(string className)
        {
            var regexFileName = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars()))));
            var cleanedClassName = regexFileName.Replace(className, string.Empty).Trim();

            return cleanedClassName;
        }

        internal static string RemoveInvalidAssemblyNameChars(string assemblyName)
        {
            var regexPath = new Regex(string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidPathChars()))));
            var cleanedAssemlyName = regexPath.Replace(assemblyName, string.Empty).Trim();

            return cleanedAssemlyName;
        }

        /// <summary>
        /// Gets the singleton Instance, initializing the printer if necessary.
        /// </summary>
        /// <value>The Instance.</value>
        public static IFiscalPrinterDriver FiscalPrinterDriver
        {
            get
            {
                var fiscalOperations = Nested.fiscalPrinterDriver;

                if (fiscalOperations.OperatingState == FiscalPrinterState.Unknown)
                {
                    fiscalOperations.SetInitializing();
                    // Calling Initialize method on the driver is outside the scope of responsibilities of this factory.
                    // Initialize method is called by the FiscalPrinter peripheral device as it is fully responsible for handling FiscalPrinterDriver component.
                }

                return fiscalOperations;
            }
        }

        #endregion
    }
}
