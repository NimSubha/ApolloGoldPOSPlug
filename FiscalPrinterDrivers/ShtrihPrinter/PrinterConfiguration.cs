/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{

    #region config section enums

    /// <summary>
    /// Represents enumaration of allowed text alignment methods
    /// </summary>
    public enum TextAlignment
    {
        Left = -1,
        Right = 1
    }

    /// <summary>
    /// Represents enumaration of allowed field categories
    /// </summary>
    public enum FieldCategory
    {
        Text,
        Parameter
    }

    /// <summary>
    /// Represents printer parameter type
    /// </summary>
    public enum PrinterParameterType
    {
        String,
        Integer
    }

    /// <summary>
    /// Represents enumeration of allowed field types
    /// </summary>
    public enum FieldType
    {
        [FieldCategory(FieldCategory.Text)]
        Text,
        [FieldCategory(FieldCategory.Parameter)]
        Cashier,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        ReceiptNumber,
        [FieldCategory(FieldCategory.Parameter)]
        Address,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        Customer,
        [FieldCategory(FieldCategory.Parameter)]
        TerminalId,
        [FieldCategory(FieldCategory.Parameter)]
        StorePhoneNo,
        [FieldCategory(FieldCategory.Parameter)]
        StoreName,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        Salesperson,
        [FieldCategory(FieldCategory.Parameter)]
        [LineTypeAllowed(LineType.SalesFiscal, LineType.SalesText)]
        ItemNo,
        [FieldCategory(FieldCategory.Parameter)]
        [LineTypeAllowed(LineType.SalesFiscal, LineType.SalesText)]
        ItemName,
        [FieldCategory(FieldCategory.Parameter)]
        [LineTypeAllowed(LineType.SalesFiscal, LineType.SalesText)]
        ItemAlias,
        [FieldCategory(FieldCategory.Parameter)]
        [LineTypeAllowed(LineType.SalesFiscal, LineType.SalesText)]
        ItemBarcode,
        [FieldCategory(FieldCategory.Parameter)]
        [LineTypeAllowed(LineType.SalesFiscal, LineType.SalesText)]
        UnitOfMeasure,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        LoyaltyCard,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        LoyaltyBalance,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        LoyaltyAdded,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeProhibited(DocumentSectionType.SalesLine)]
        LoyaltyUsed,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        LoyaltyAmount,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        LoyaltyPercent,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltyCustomerName,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltyCardStatus,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltyCardType,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltySchemeDescription,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltyAddedTotal,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltyUsedTotal,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        LoyaltyExpiredTotal,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        LineDiscountAmount,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        PeriodicDiscountAmount,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        ReceiptDiscountAmount,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        SummaryDiscountAmount,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        LineDiscountPercent,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        PeriodicDiscountPercent,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        ReceiptDiscountPercent,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        SummaryDiscountPercent,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        PeriodicDiscountName,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.TotalsHeader)]
        RoundingDiscountPercent,
        [FieldCategory(FieldCategory.Parameter)]
        [DocumentSectionTypeAllowed(DocumentSectionType.TotalsHeader)]
        RoundingDiscountAmount,
        [FieldCategory(FieldCategory.Parameter)]
        [LineTypeAllowed(LineType.SalesFiscal, LineType.SalesText, LineType.GiftCardDiscount)]
        GiftCardId,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        POSDate,
        [FieldCategory(FieldCategory.Parameter)]
        [LayoutTypeAllowed(LayoutType.LoyaltyCardBalance)]
        POSTime,
    }

    /// <summary>
    /// Represents enumeration of allowed line types
    /// </summary>
    public enum LineType
    {
        Text,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine)]
        SalesFiscal,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine)]
        SalesText,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        SummaryDiscount,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        LineDiscount,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        PeriodicDiscount,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        ReceiptDiscount,        
        [DocumentSectionTypeAllowed(DocumentSectionType.TotalsHeader)]
        RoundingDiscount,
        [DocumentSectionTypeAllowed(DocumentSectionType.TotalsHeader)]
        GiftCardDiscount,
        [DocumentSectionTypeAllowed(DocumentSectionType.SalesLine, DocumentSectionType.TotalsHeader)]
        LoyaltyDiscount
    }

    /// <summary>
    /// Represents enumeration of allowed document sections
    /// </summary>
    public enum DocumentSectionType
    {
        Header,
        Footer,
        SalesLine,
        TotalsHeader,
        [FieldTypeAllowed(FieldType.Text,
                          FieldType.LoyaltyCard,
                          FieldType.LoyaltyCustomerName,
                          FieldType.LoyaltySchemeDescription,
                          FieldType.LoyaltyCardStatus,
                          FieldType.LoyaltyCardType,
                          FieldType.LoyaltyAddedTotal,
                          FieldType.LoyaltyUsedTotal,
                          FieldType.LoyaltyExpiredTotal,
                          FieldType.LoyaltyBalance,
                          FieldType.Address,
                          FieldType.TerminalId,
                          FieldType.Cashier,
                          FieldType.StoreName,
                          FieldType.StorePhoneNo,
                          FieldType.Salesperson,
                          FieldType.Customer,
                          FieldType.POSDate,
                          FieldType.POSTime)]
        SimpleSection
    }

    /// <summary>
    /// Represents enumeration of allowed layout types
    /// </summary>
    public enum LayoutType
    {
        Default,
        Sale,
        Return,
        ReportX,
        ReportZ,
        FloatEntry,
        TenderRemoval,
        StartAmount,
        [DocumentSectionTypeAllowed(DocumentSectionType.SimpleSection)]
        LoyaltyCardBalance
    }

    #endregion

    /// <summary>
    /// Interface to access configuration element info.
    /// </summary>
    internal interface IConfigurationElementInfo
    {
        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        int LineNumber { get; }
    }

    /// <summary>
    /// Represents a counter to generate unique field key values
    /// </summary>
    internal sealed class PrinterConfigurationCounter
    {
        private int count = 0;
        /// <summary>
        /// Get current count
        /// </summary>        
        /// <returns>
        /// Returns field count
        /// </returns>
        public int GetCount()
        {
            return count;
        }
        /// <summary>
        /// Gets the new counter number.
        /// </summary>                
        /// <returns>New counter number</returns>
        public int GetNewNumber()
        {
            return ++count;
        }
    }

    /// <summary>
    /// Represents a PrinterConfiguration configuration section within a configuration file.
    /// </summary>
    internal sealed class PrinterConfiguration : ConfigurationSection, IConfigurationElementInfo
    {
        /// <summary>
        /// Line number shift from the beginning of the file.
        /// </summary>
        public static int LineNumberShift { get; private set; }

        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets the ImageList collection
        /// </summary>
        [ConfigurationProperty("ImageList")]
        public ImageListCollection ImageList
        {
            get { return ((ImageListCollection)(base["ImageList"])); }
        }

        /// <summary>
        /// Gets the Layout collection
        /// </summary>
        [ConfigurationProperty("Layouts")]
        public LayoutsCollection Layouts
        {
            get { return ((LayoutsCollection)(base["Layouts"])); }
        }

        /// <summary>
        /// Gets the TaxMapping collection
        /// </summary>
        [ConfigurationProperty("TaxMapping")]
        public TaxMappingCollection TaxMapping
        {
            get { return ((TaxMappingCollection)(base["TaxMapping"])); }
        }

        /// <summary>
        /// Gets the TenderTypesMapping collection.
        /// </summary>
        [ConfigurationProperty("TenderTypesMapping")]
        public TenderTypesMappingCollection TenderTypesMapping
        {
            get { return (TenderTypesMappingCollection)base["TenderTypesMapping"]; }
        }

        /// <summary>
        /// Gets the PrinterSettings collection
        /// </summary>
        [ConfigurationProperty("PrinterSettings")]
        public PrinterSettingsCollection PrinterParameters
        {
            get { return ((PrinterSettingsCollection)(base["PrinterSettings"])); }
        }

        /// <summary>
        /// Gets the RibbonSettings element
        /// </summary>
        [ConfigurationProperty("RibbonSettings")]
        public RibbonSettings RibbonSettings
        {
            get { return ((RibbonSettings)(base["RibbonSettings"])); }
        }

        /// <summary>
        /// Assembly resolving event handler
        /// </summary>
        /// <param name="sender">The sender object</param>
        /// <param name="args">The ResolveEventArgs args</param>
        /// <returns>Current executing assembly</returns>
        /// <remarks>Resolves current assembly name specified in the config file. Should be subscribed before config file loading and unsubscribed after that</remarks>
        static System.Reflection.Assembly AssemblyResolveEventHandler(System.Object sender, System.ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter"))
                return System.Reflection.Assembly.GetExecutingAssembly();
            else
                return null;
        }

        /// <summary>
        /// Loads data from the configuration file
        /// </summary>
        /// <returns><see cref="PrinterConfiguration"/> object</returns>
        /// <remarks>Configuration file full name should be assembly full file name + ".config" extension at the end.</remarks>
        public static PrinterConfiguration GetConfig()
        {
            PrinterConfiguration section = null;

            try
            {
                
                string appDllPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
                string extension = string.Concat(Path.GetExtension(appDllPath), ".config");
                string configFile = Path.ChangeExtension(appDllPath, extension);

                // If the configuration file does not exist return
                if (!File.Exists(configFile))
                {
                    throw new FileNotFoundException(Resources.Translate(Resources.ConfigFileNotFound, configFile));
                }

                var handler = new System.ResolveEventHandler(AssemblyResolveEventHandler);

                try
                {
                    // subscribe AssemblyResolveEventHandler to AssemblyResolve event
                    AppDomain.CurrentDomain.AssemblyResolve += handler;

                    CalcLineNumberShift(configFile);
                    // Open config file
                    System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(appDllPath);
                    // Read config section
                    section = config.GetSection(typeof(PrinterConfiguration).Name) as PrinterConfiguration;
                    PrinterConfigurationValidator.ValidateConfig(section);
                }
                finally
                {
                    // unsubscribe AssemblyResolveEventHandler from AssemblyResolve event
                    AppDomain.CurrentDomain.AssemblyResolve -= handler;
                }
            }
            catch (Exception ex)
            {
                ex = ex.InnerException ?? ex;
                ExceptionHelper.ThrowException(true, Resources.FailedToLoadConfigFile, ex.Message);
            }

            return section;
        }

        /// <summary>
        /// Calculates start position of the PrinterConfiguration element in configuration file.
        /// </summary>
        /// <param name="configFile">File name</param>
        private static void CalcLineNumberShift(string configFile)
        {
            string PrinterConfigurationTag = typeof(PrinterConfiguration).Name;
            using (XmlTextReader xmlReader = new XmlTextReader(configFile))
            {
                LineNumberShift = 0;
                while (xmlReader.ReadToFollowing(PrinterConfigurationTag))
                {
                    if (xmlReader.NodeType == XmlNodeType.Element)
                    {
                        LineNumberShift = xmlReader.LineNumber;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Represents a Layouts section within a configuration file.
    /// </summary>
    /// <remarks>
    /// Consists of the Layout configuration elements.
    /// </remarks>
    [ConfigurationCollection(typeof(Layout), AddItemName = "Layout")]
    internal sealed class LayoutsCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates new Layout object.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Layout();
        }

        /// <summary>
        /// Retrives the element key in the collection.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Layout)element).Type;
        }

        /// <summary>
        /// Gets the Layout element from the elements collection by its index.
        /// </summary>
        public Layout this[int idx]
        {
            get { return (Layout)BaseGet(idx); }
        }

        /// <summary>
        /// Gets the Layout element from the elements collection by its key.
        /// </summary>
        public new Layout this[string key]
        {
            get { return (Layout)BaseGet(key); }
        }

        /// <summary>
        /// Looks up for layout by key, if not found then looks for default layout.
        /// </summary>
        /// <param name="key">key to find</param>
        /// <param name="strictMatch">Determines whether the match must be strict; false by default.</param>
        /// <returns>Layout or null if no layout found.</returns>
        public Layout FindLayout(LayoutType key, bool strictMatch = false)
        {
            Layout retval = null;

            foreach (Layout layout in this)
            {
                if (layout.Type == key)
                {
                    retval = layout;
                    break;
                }

                if (!strictMatch && layout.Type == LayoutType.Default)
                {
                    retval = layout;
                }
            }

            return retval;
        }
    }


    /// <summary>
    /// Represents a Layout cofiguration section within a configuration file.
    /// </summary>
    /// <remarks>   
    /// </remarks>
    internal sealed class Layout : ConfigurationSection, IConfigurationElementInfo
    {
        /// <summary>
        /// Line number in configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets or sets the type attribute.
        /// </summary>
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public LayoutType Type
        {
            get { return ((LayoutType)(base["type"])); }
            set { base["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the imageid attribute.
        /// </summary>
        [ConfigurationProperty("imageid", DefaultValue = "", IsKey = false, IsRequired = false)]
        public string ImageId
        {
            get { return ((string)(base["imageid"])); }
            set { base["imageid"] = value; }
        }

        /// <summary>
        /// Gets the DocumentSections collection as <see cref="DocumentSectionCollection"/> object.
        /// </summary>
        [ConfigurationProperty("", IsKey = false, IsDefaultCollection = true)]
        public DocumentSectionCollection DocumentSectionCollection
        {
            get { return ((DocumentSectionCollection)(base[""])); }
        }

        /// <summary>
        /// Gets the PrinterSettings collection as <see cref="PrinterSettingsCollection"/> object.
        /// </summary>
        [ConfigurationProperty("PrinterSettings")]
        public PrinterSettingsCollection PrinterParameters
        {
            get { return ((PrinterSettingsCollection)(base["PrinterSettings"])); }
        }
    }

    /// <summary>
    /// Represents a DocumentSection cofiguration section within a configuration file.
    /// </summary>
    internal sealed class DocumentSection : ConfigurationSection, IConfigurationElementInfo
    {

        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets or sets the type attribute.
        /// </summary>
        [ConfigurationProperty("type", IsKey = true, IsRequired = true)]
        public DocumentSectionType Type
        {
            get { return ((DocumentSectionType)(base["type"])); }
            set { base["type"] = value; }
        }


        /// <summary>
        /// Gets the Line collection as <see cref="LineCollection"/> object.
        /// </summary>        
        [ConfigurationProperty("", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
        public LineCollection LineCollection
        {
            get { return ((LineCollection)(base[""])); }
        }
    }

    /// <summary>
    /// Represents a Line section within the configuration file.
    /// </summary>
    internal sealed class Line : ConfigurationSection, IConfigurationElementInfo
    {
        private int key;

        /// <summary>
        /// Initializes a <see cref="Line"/> instance.
        /// </summary>
        /// <param name="key">Unique key value within collection.</param>
        public Line(int key)
        {
            this.key = key;
        }

        /// <summary>
        /// Unique key of the field.
        /// </summary>
        [ConfigurationProperty("Key", IsKey = true)]
        public int Key
        {
            get { return key; }
        }

        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets or sets the type attribute.
        /// </summary>
        [ConfigurationProperty("type", IsKey = false, IsRequired = true)]
        public LineType Type
        {
            get { return ((LineType)(base["type"])); }
            set { base["type"] = value; }
        }

        /// <summary>
        /// Gets or sets the hideIfEmptyField attribute.
        /// </summary>
        [FieldTypeProhibited(FieldType.LoyaltyAddedTotal, 
                             FieldType.LoyaltyUsedTotal, 
                             FieldType.LoyaltyExpiredTotal, 
                             FieldType.LoyaltyCard,
                             FieldType.LoyaltyCardStatus, 
                             FieldType.LoyaltyCardType, 
                             FieldType.LoyaltyCustomerName, 
                             FieldType.LoyaltySchemeDescription)]
        [ConfigurationProperty("hideIfEmptyField", IsKey = false, IsRequired = false)]
        public FieldType? HideIfEmptyField
        {
            get { return (FieldType?)base["hideIfEmptyField"]; }
            set { base["hideIfEmptyField"] = value; }
        }

        /// <summary>
        /// Gets or sets the addRoundingToDiscount attribute.
        /// </summary>
        [LineTypeAllowed(LineType.ReceiptDiscount)]
        [LineTypeProhibited(LineType.Text,
            LineType.SalesFiscal,
            LineType.SalesText,
            LineType.SummaryDiscount,
            LineType.LineDiscount,
            LineType.PeriodicDiscount)]
        [ConfigurationProperty("addRoundingToDiscount", IsKey = false, IsRequired = false)]
        public bool? AddRoundingToDiscount
        {
            get { return (bool?)base["addRoundingToDiscount"]; }
            set { base["addRoundingToDiscount"] = value; }
        }

        /// <summary>
        /// Gets the Field collection as a <see cref="FieldCollection"/> object.
        /// </summary>        
        [ConfigurationProperty("", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
        public FieldCollection FieldCollection
        {
            get { return ((FieldCollection)(base[""])); }
        }
    }

    /// <summary>
    /// Represents the ImageList configuration section within the configuration file.
    /// </summary>
    /// <remarks>
    /// Consists of the <see cref="Image"/> configuration elements.
    /// </remarks>
    [ConfigurationCollection(typeof(Image), AddItemName = "Image")]
    internal sealed class ImageListCollection : ConfigurationElementCollection, IConfigurationElementInfo
    {
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Creates a new <see cref="Image"/> object.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Image();
        }

        /// <summary>
        /// Retrives the element key in the collection.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Image)(element)).Id;
        }

        /// <summary>
        /// Gets or sets the default image ID attribute.
        /// </summary>
        [ConfigurationProperty("default", DefaultValue = "")]
        public string DefaultImageId
        {
            get { return ((string)(base["default"])); }
            set { base["default"] = value; }
        }

        /// <summary>
        /// Gets the <see cref="Image"/> element from the elements collection by its index.
        /// </summary>
        public Image this[int idx]
        {
            get { return (Image)BaseGet(idx); }
        }

        /// <summary>
        /// Gets the <see cref="Image"/> element from the elements collection by its key.
        /// </summary>
        public new Image this[string key]
        {
            get { return (Image)BaseGet(key); }
        }

        /// <summary>
        /// Looks for image by its ID.
        /// </summary>
        /// <param name="imageId">Image ID</param>
        /// <returns><see cref="Image"/> object or null if no image was found.</returns>
        public Image FindImage(string imageId)
        {
            Image retval = null;

            foreach (Image image in this)
            {
                if (image.Id == imageId)
                {
                    retval = image;
                    break;
                }
            }

            return retval;
        }
    }

    /// <summary>
    /// Represents the Image configuration element within the configuration file.
    /// </summary>
    internal sealed class Image : ConfigurationElement, IConfigurationElementInfo
    {
        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets or sets the ID attribute.
        /// </summary>
        [ConfigurationProperty("id", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Id
        {
            get { return ((string)(base["id"])); }
            set { base["id"] = value; }
        }

        /// <summary>
        /// Gets or sets the path attribute.
        /// </summary>
        [ConfigurationProperty("path", DefaultValue = "false", IsKey = true, IsRequired = true)]
        public string Path
        {
            get { return ((string)(base["path"])); }
            set { base["path"] = value; }
        }

        /// <summary>
        /// Gets or sets the startLine attribute.
        /// </summary>
        [ConfigurationProperty("startline", DefaultValue = 2, IsKey = false, IsRequired = true)]
        public int StartLine
        {
            get { return ((int)(base["startline"])); }
            set { base["startline"] = value; }
        }


        /// <summary>
        /// Gets or sets the endLine attribute
        /// </summary>
        [ConfigurationProperty("endline", DefaultValue = 110, IsKey = false, IsRequired = true)]
        public int EndLine
        {
            get { return ((int)(base["endline"])); }
            set { base["endline"] = value; }
        }

        /// <summary>
        /// Gets or sets the center attribute.
        /// </summary>
        [ConfigurationProperty("center", DefaultValue = true)]
        public bool Center
        {
            get { return ((bool)(base["center"])); }
            set { base["center"] = value; }
        }
    }


    /// <summary>
    /// A collection of the <see cref="DocumentSection"/> configuration elements.
    /// </summary>
    /// <remarks>
    /// Is not represented as xml element in the configuration file.
    /// </remarks>
    [ConfigurationCollection(typeof(DocumentSection), AddItemName = "DocumentSection")]
    internal sealed class DocumentSectionCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates an instance of a new <see cref="DocumentSection"/> object.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DocumentSection();
        }

        /// <summary>
        /// Retrives the element key in the collection.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DocumentSection)(element)).Type;
        }

        /// <summary>
        /// Gets the <see cref="DocumentSection"/> element from the elements collection by its index.
        /// </summary>
        public DocumentSection this[int idx]
        {
            get { return (DocumentSection)BaseGet(idx); }
        }

        /// <summary>
        /// Gets the <see cref="DocumentSection"/> element from the elements collection by its key.
        /// </summary>
        public new DocumentSection this[string key]
        {
            get { return (DocumentSection)BaseGet(key); }
        }

        /// <summary>
        /// Looks up for the <see cref="DocumentSection"/> by its key.
        /// </summary>
        /// <param name="key">The key value to search for.</param>
        /// <returns><see cref="DocumentSection"/> object or null if one was not found.</returns>
        public DocumentSection FindDocumentSection(DocumentSectionType key)
        {
            DocumentSection retval = null;

            foreach (DocumentSection documentSection in this)
            {
                if (documentSection.Type == key)
                {
                    retval = documentSection;
                    break;
                }
            }

            return retval;
        }
        
    }

    /// <summary>
    /// Represents a TaxMapping configuration section within the configuration file.
    /// </summary>
    /// <remarks>
    /// Consists of the <see cref="Tax"/> configuration elements.
    /// </remarks>
    [ConfigurationCollection(typeof(Tax), AddItemName = "Tax")]
    internal sealed class TaxMappingCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="Tax"/> object.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Tax();
        }

        /// <summary>
        /// Retrives the element's key in the collection.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Tax)(element)).TaxCode;
        }

        /// <summary>
        /// Gets the <see cref="Tax"/> element from the collection by its index.
        /// </summary>
        public Tax this[int idx]
        {
            get { return (Tax)BaseGet(idx); }
        }

        /// <summary>
        /// Gets the Tax element from the collection by its key.
        /// </summary>
        public new Tax this[string key]
        {
            get { return (Tax)BaseGet(key); }
        }
    }

    /// <summary>
    /// Represents a Tax configuration element within the configuration file.
    /// </summary>
    /// <remarks>
    /// Consists of the taxCode key attribute and the printerTaxId attribute.
    /// </remarks>
    internal sealed class Tax : ConfigurationElement, IConfigurationElementInfo
    {
        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets or sets the taxCode attribute.
        /// </summary>
        [ConfigurationProperty("taxCode", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string TaxCode
        {
            get { return ((string)(base["taxCode"])); }
            set { base["taxCode"] = value; }
        }

        /// <summary>
        /// Gets or sets the printerTaxId attribute.
        /// </summary>
        [ConfigurationProperty("printerTaxId", DefaultValue = 0, IsKey = false, IsRequired = true)]
        public int PrinterTaxId
        {
            get { return ((int)(base["printerTaxId"])); }
            set { base["printerTaxId"] = value; }
        }
    }

    /// <summary>
    /// Represents a TenderTypesMapping configuration section in the configuration file.
    /// </summary>
    /// <remarks>
    /// Contains of <see cref="TenderType"/> elements.
    /// </remarks>
    [ConfigurationCollection(typeof(TenderType), AddItemName = "TenderType")]
    internal sealed class TenderTypesMappingCollection : ConfigurationElementCollection, IConfigurationElementInfo
    {
        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TenderType();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TenderType)element).TenderTypeId;
        }
    }

    /// <summary>
    /// Represents a TenderType configuration element within the configuration file.
    /// </summary>
    /// <remarks>
    /// Contains tenderTypeId attribute identifying AX tender type and printerPaymentType attribute identifying printer payment type.
    /// </remarks>
    internal sealed class TenderType : ConfigurationElement, IConfigurationElementInfo
    {
        /// <summary>
        /// Line number in the configuration file.
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets the tenderTypeId attribute.
        /// </summary>
        [ConfigurationProperty("tenderTypeId", IsKey = true, IsRequired = true)]
        public int TenderTypeId
        {
            get { return (int)base["tenderTypeId"]; }
        }

        /// <summary>
        /// Gets the printerPaymentType attribute.
        /// </summary>
        [ConfigurationProperty("printerPaymentType", IsKey = false, IsRequired = true)]
        public int PrinterPaymentType
        {
            get { return (int)base["printerPaymentType"]; }
        }
    }

    /// <summary>
    /// A collection of <see cref="Line"/> configuration elements.
    /// </summary>
    /// <remarks>
    /// Is not represented as xml element in the configuration file.
    /// </remarks>
    [ConfigurationCollection(typeof(Line), AddItemName = "Line", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    internal class LineCollection : ConfigurationElementCollection
    {
        private PrinterConfigurationCounter counter = new PrinterConfigurationCounter();

        /// <summary>
        /// Creates a new <see cref="Line"/> object.
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new Line(counter.GetNewNumber());
        }

        /// <summary>
        /// Retrives the element key in the collection.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Line)(element)).Key.ToString();
        }

        /// <summary>
        /// Gets the <see cref="Line"/> element from the collection by its index.
        /// </summary>
        public Line this[int idx]
        {
            get { return (Line)BaseGet(idx); }
        }

        /// <summary>
        /// Gets the <see cref="Line"/> element from the collection by its key.
        /// </summary>
        public new Line this[string key]
        {
            get { return (Line)BaseGet(key); }
        }
    }

    /// <summary>
    /// A collection of <see cref="Field"/> configuration elements.
    /// </summary>
    /// <remarks>
    /// Is not represented as an xml element in the configuration file.
    /// </remarks>
    [ConfigurationCollection(typeof(Field), AddItemName = "Field", CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
    internal class FieldCollection : ConfigurationElementCollection
    {
        private PrinterConfigurationCounter counter = new PrinterConfigurationCounter();

        /// <summary>
        /// Creates a new <see cref="Field"/>.
        /// </summary
        protected override ConfigurationElement CreateNewElement()
        {
            return new Field(counter.GetNewNumber());
        }

        /// <summary>
        /// Gets the element's key.
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Field)(element)).Key.ToString();
        }

        /// <summary>
        /// Gets the <see cref="Field"/> element from the elements collection by its index.
        /// </summary>
        public Field this[int idx]
        {
            get { return (Field)BaseGet(idx); }
        }

        /// <summary>
        /// Gets the <see cref="Field"/> element from the elements collection by its key.
        /// </summary>
        public new Field this[string key]
        {
            get { return (Field)BaseGet(key); }
        }
    }

    /// <summary>
    /// Represents a PrinterSettings configuration section in the configuration file
    /// </summary>
    /// <remarks>
    /// Consists of the PrinterParameter configuration elements
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface", Justification = "Design"), ConfigurationCollection(typeof(PrinterParameter), AddItemName = "Parameter")]
    public class PrinterSettingsCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates new PrinterParameter object
        /// </summary>
        protected override ConfigurationElement CreateNewElement()
        {
            return new PrinterParameter();
        }

        /// <summary>
        /// Retrives the element key in the collection
        /// </summary>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var printerParameter = element as PrinterParameter;

            var s = string.Empty;

            if (printerParameter != null)
            {
                s = printerParameter.TableId + "#" + printerParameter.RowId + "#" + printerParameter.FieldId;
            }

            return s;
        }

        /// <summary>
        /// Gets the PrinterParameter element from the elements collection by its index
        /// </summary>
        public PrinterParameter this[int index]
        {
            get { return (PrinterParameter)BaseGet(index); }
        }

        /// <summary>
        /// Gets the PrinterParameter element from the elements collection by its key
        /// </summary>
        public new PrinterParameter this[string key]
        {
            get { return (PrinterParameter)BaseGet(key); }
        }
    }

    /// <summary>
    /// Represents a Field configuration element within a configuration file
    /// </summary>
    /// <remarks>
    /// Consists of the type key attribute and the element's value
    /// </remarks>
    internal sealed class Field : ConfigurationElement, IConfigurationElementInfo
    {

        private int lineNumberInFile;

        public Field(int index)
        {
            Key = index;
        }

        /// <summary>
        /// Line Number in config file
        /// </summary>
        public int LineNumber { get { return lineNumberInFile + PrinterConfiguration.LineNumberShift - 1; } }

        /// <summary>
        /// Unique key of the field
        /// </summary>
        [ConfigurationProperty("Key", IsKey = true)]
        public int Key
        {
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the type attribute
        /// </summary>
        [ConfigurationProperty("type", IsKey = false, IsRequired = true)]
        public FieldType Type
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets or sets the length attribute
        /// </summary>
        [ConfigurationProperty("length", DefaultValue = 0)]
        public int Length
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets or sets the alignment attribute
        /// </summary>
        [ConfigurationProperty("alignment", DefaultValue = TextAlignment.Left)]
        public TextAlignment Alignment
        {
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the element's value
        /// </summary>
        public string Value { get; private set; }


        /// <summary>
        /// Loads Line element from it's xml
        /// </summary>
        protected override void DeserializeElement(XmlReader reader, bool s)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // NB: Do not use base.DeserializeElement - it can't work with elements containing text value
            var textReader = reader as IXmlLineInfo;
            if (textReader != null)
            {
                lineNumberInFile = textReader.LineNumber;
            }

            Type = reader.GetEnumAttributeValue<FieldType>("type", this);
            Length = reader.GetIntegerAttributeValue("length", this, 0, Int16.MaxValue);
            Alignment = reader.GetEnumAttributeValue<TextAlignment>("alignment", this);
            reader.ValidateForExcessAttributes(this);
            Value = reader.ReadElementContentAsString();
        }
    }

    /// <summary>
    /// Represents field category attribute class for FieldType enumeration attributes
    /// </summary>
    sealed class FieldCategoryAttribute : Attribute
    {
        /// <summary>
        /// Categoty of the field
        /// </summary>                
        public FieldCategory Category { get; private set; }

        /// <summary>
        /// Constructor for the class
        /// </summary>        
        /// <param name="category">Field category </param>        
        public FieldCategoryAttribute(FieldCategory category) 
        { 
            this.Category = category; 
        }
    }

    /// <summary>
    /// Provides access to the restrictions of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the restriction values.</typeparam>
    internal interface IRestrictions<T>
    {
        /// <summary>
        /// Gets the restrictions of type <typeparam name="T"/>.
        /// </summary>
        T[] Restrictions { get; }
    }

    /// <summary>
    /// Defines the restriction on the <see cref="FieldType"/>.
    /// </summary>
    abstract internal class FieldTypeRestrictionAttribute : Attribute, IRestrictions<FieldType>
    {
        /// <summary>
        /// Gets or sets the <see cref="FieldType"/> restriction values.
        /// </summary>
        public FieldType[] FieldTypes { get; protected set; }

        public FieldType[] Restrictions { get { return FieldTypes; } }

        /// <summary>
        /// Constructs an instance of the <see cref="FieldTypeRestrictionAttribute"/> based on th <paramref name="fieldTypes"/> provided.
        /// </summary>
        /// <param name="fieldTypes"><see cref="FieldType"/> restriction values.</param>
        public FieldTypeRestrictionAttribute(params FieldType[] fieldTypes)
        {
            FieldTypes = fieldTypes;
        }
    }

    /// <summary>
    /// Defines the allowed <see cref="FieldType"/> values.
    /// </summary>
    internal sealed class FieldTypeAllowedAttribute : FieldTypeRestrictionAttribute 
    {
        /// <summary>
        /// Constructs an instance of the <see cref="FieldTypeAllowedAttribute"/> based on the <paramref name="fieldTypes"/> provided.
        /// </summary>
        /// <param name="fieldTypes">Allowed <see cref="FieldType"/> values.</param>
        public FieldTypeAllowedAttribute(params FieldType[] fieldTypes) : base(fieldTypes) { }
    }

    /// <summary>
    /// Defines the prohibited <see cref="FieldType"/> values.
    /// </summary>
    internal sealed class FieldTypeProhibitedAttribute : FieldTypeRestrictionAttribute 
    {
        /// <summary>
        /// Constructs an instance of the <see cref="FieldTypeProhibitedAttribute"/> based on the <paramref name="fieldTypes"/> provided.
        /// </summary>
        /// <param name="fieldTypes">Prohibited <see cref="FieldType"/> values.</param>
        public FieldTypeProhibitedAttribute(params FieldType[] fieldTypes) : base(fieldTypes) { }
    }

    /// <summary>
    /// Defines the restriction on the <see cref="LineType"/> values.
    /// </summary>
    abstract internal class LineTypeRestrictionAttribute : Attribute, IRestrictions<LineType>
    {
        /// <summary>
        /// Gets or sets the <see cref="LineType"/> restriction values.
        /// </summary>
        public LineType[] LineTypes { get; protected set; }

        public LineType[] Restrictions { get { return LineTypes; } }

        /// <summary>
        /// Constructs an instance of <see cref="LineTypeRestrictionAttribute"/> based on the <paramref name="lineTypes"/> provided.
        /// </summary>
        /// <param name="lineTypes"><see cref="LineType"/> restriction values.</param>
        public LineTypeRestrictionAttribute(params LineType[] lineTypes)
        {
            LineTypes = lineTypes;
        }
    }

    /// <summary>
    /// Defines the allowed <see cref="LineType"/> values.
    /// </summary>
    internal sealed class LineTypeAllowedAttribute : LineTypeRestrictionAttribute
    {
        /// <summary>
        /// Constructs an instance of the <see cref="LineTypeAllowedAttribute"/> based on the <paramref name="lineTypes"/> provided.
        /// </summary>
        /// <param name="lineTypes">Allowed <see cref="LineType"/> values.</param>
        public LineTypeAllowedAttribute(params LineType[] lineTypes) : base(lineTypes) { }
    }

    /// <summary>
    /// Defines the prohibited <see cref="LineType"/> values.
    /// </summary>
    internal sealed class LineTypeProhibitedAttribute : LineTypeRestrictionAttribute
    {
        /// <summary>
        /// Constructs an instance of the <see cref="LineTypeProhibitedAttribute"/> based in the <paramref name="lineTypes"/> provided.
        /// </summary>
        /// <param name="lineTypes">Prohibited <see cref="LineType"/> values.</param>
        public LineTypeProhibitedAttribute(params LineType[] lineTypes) : base(lineTypes) { }
    }

    /// <summary>
    /// Defines the restriction on the <see cref="DocumentSectionType"/> values.
    /// </summary>
    abstract internal class DocumentSectionTypeRestrictionAttribute : Attribute, IRestrictions<DocumentSectionType>
    {
        /// <summary>
        /// Gets or sets the <see cref="DocumentSectionTypes"/> restriction values.
        /// </summary>
        public DocumentSectionType[] DocumentSectionTypes { get; protected set; }

        public DocumentSectionType[] Restrictions { get { return DocumentSectionTypes; } }

        /// <summary>
        /// Constructs an insnace of the <see cref="DocumentSectionTypeRestrictionAttribute"/> based on the <paramref name="documentSectionTypes"/> provided.
        /// </summary>
        /// <param name="documentSectionTypes"><see cref="DocumentSectionType"/> restriction values.</param>
        protected DocumentSectionTypeRestrictionAttribute(params DocumentSectionType[] documentSectionTypes)
        {
            DocumentSectionTypes = documentSectionTypes;
        }
    }

    /// <summary>
    /// Defines the allowed <see cref="DocumentSectionType"/> values.
    /// </summary>
    internal sealed class DocumentSectionTypeAllowedAttribute : DocumentSectionTypeRestrictionAttribute
    {
        /// <summary>
        /// Constructs an instance of <see cref="DocumentSectionTypeAllowedAttribute"/> based on the <paramref name="documentSectionTypes"/> provided.
        /// </summary>
        /// <param name="documentSectionTypes">Allowed <see cref="DocumentSectionType"/> values.</param>
        public DocumentSectionTypeAllowedAttribute(params DocumentSectionType[] documentSectionTypes) : base(documentSectionTypes) { }
    }

    /// <summary>
    /// Defines the prohibited <see cref="DocumentSectionType"/> values.
    /// </summary>
    internal sealed class DocumentSectionTypeProhibitedAttribute : DocumentSectionTypeRestrictionAttribute
    {
        /// <summary>
        /// Constructs an instance of <see cref="DocumentSectionTypeProhibitedAttribute"/> based on the <paramref name="documentSectionTypes"/> provided.
        /// </summary>
        /// <param name="documentSectionTypes">Profibited <see cref="DocumentSectionType"/> values.</param>
        public DocumentSectionTypeProhibitedAttribute(params DocumentSectionType[] documentSectionTypes) : base(documentSectionTypes) { }
    }

    /// <summary>
    /// Defines the restriction on the <see cref="LayoutType"/> values.
    /// </summary>
    abstract internal class LayoutTypeRestrictionAttribute : Attribute, IRestrictions<LayoutType>
    {
        /// <summary>
        /// Gets or sets the <see cref="LayoutType"/> retsriction values.
        /// </summary>
        public LayoutType[] LayoutTypes { get; protected set; }

        public LayoutType[] Restrictions { get { return LayoutTypes; } }

        /// <summary>
        /// Constructs an instance of the <see cref="LayoutTypeRestrictionAttribute"/> based on the <paramref name="layoutTypes"/> provided.
        /// </summary>
        /// <param name="layoutTypes"><see cref="LayoutType"/> restriction values.</param>
        public LayoutTypeRestrictionAttribute(params LayoutType[] layoutTypes)
        {
            LayoutTypes = layoutTypes;
        }
    }

    /// <summary>
    /// Defines an allowed <see cref="LayoutType"/> values.
    /// </summary>
    internal sealed class LayoutTypeAllowedAttribute : LayoutTypeRestrictionAttribute
    {
        /// <summary>
        /// Constructs an instance of the <see cref="LayoutTypeAllowedAttribute"/> based on the <paramref name="layoutTypes"/> provided.
        /// </summary>
        /// <param name="layoutTypes">Allowed <see cref="LayoutType"/> values.</param>
        public LayoutTypeAllowedAttribute(params LayoutType[] layoutTypes) : base(layoutTypes) { }
    }

    /// <summary>
    /// Defines a prohibited <see cref="LayoutType"/> values.
    /// </summary>
    internal sealed class LayoutTypeProhibitedAttribute : LayoutTypeRestrictionAttribute
    {
        /// <summary>
        /// Constructs an instance of the <see cref="LayoutTypeProhibitedAttribute"/> based on the <see cref="LayoutType"/> provided.
        /// </summary>
        /// <param name="layoutTypes">Prohibited <see cref="LayoutType"/> values.</param>
        public LayoutTypeProhibitedAttribute(params LayoutType[] layoutTypes) : base(layoutTypes) { }
    }

    /// <summary>
    /// Represents a PrinterParameter configuration element within a configuration file
    /// </summary>
    /// <remarks>
    /// Consists of the tableId, rowId, fieldId key attributes, type attribute and the element's value
    /// </remarks>
    public class PrinterParameter : ConfigurationElement, IConfigurationElementInfo
    {
        private int lineNumberInFile;
        /// <summary>
        /// Line Number in config file
        /// </summary>
        public int LineNumber { get { return lineNumberInFile + PrinterConfiguration.LineNumberShift - 1; } }

        /// <summary>
        /// Gets or sets the tableId attribute
        /// </summary>
        [ConfigurationProperty("tableId", DefaultValue = 0, IsKey = true, IsRequired = true)]
        public int TableId
        {
            get;
            private set; 
        }

        /// <summary>
        /// Gets or sets the rowId attribute
        /// </summary>
        [ConfigurationProperty("rowId", DefaultValue = 0, IsKey = true, IsRequired = true)]
        public int RowId
        {
            get; 
            private set; 
        }

        /// <summary>
        /// Gets or sets the fieldId attribute
        /// </summary>
        [ConfigurationProperty("fieldId", DefaultValue = 0, IsKey = true, IsRequired = true)]
        public int FieldId
        {
            get; 
            private set;
        }

        /// <summary>
        /// Gets or sets the element's value
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Loads Parameter element from it's xml
        /// </summary>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // NB: Do not use base.DeserializeElement - it can't work with elements containing text value
            var textReader = reader as IXmlLineInfo;

            if (textReader != null)
            {
                lineNumberInFile = textReader.LineNumber;
            }

            TableId = reader.GetIntegerAttributeValue("tableId", this, 0, byte.MaxValue);
            RowId = reader.GetIntegerAttributeValue("rowId", this, 0, byte.MaxValue);
            FieldId = reader.GetIntegerAttributeValue("fieldId", this, 0, byte.MaxValue);
            reader.ValidateForExcessAttributes(this);
            Value = reader.ReadElementContentAsString();
        }
    }

    /// <summary>
    /// Represents a RibbonSettings configuration element within a configuration file
    /// </summary>
    internal sealed class RibbonSettings : ConfigurationElement, IConfigurationElementInfo
    {
        
        /// <summary>
        /// Line Number in config file
        /// </summary>
        public int LineNumber { get { return ElementInformation.LineNumber; } }

        /// <summary>
        /// Gets or sets the cutRibbon attribute.
        /// </summary>
        [ConfigurationProperty("cutRibbon", DefaultValue = false, IsKey = false, IsRequired = true)]
        public bool CutRibbon
        {
            get { return ((bool)(base["cutRibbon"])); }
            set { base["cutRibbon"] = value; }
        }

        /// <summary>
        /// Gets or sets the partialCut attribute.
        /// </summary>
        [ConfigurationProperty("partialCut", DefaultValue = false, IsKey = false, IsRequired = true)]
        public bool CutType
        {
            get { return ((bool)(base["partialCut"])); }
            set { base["partialCut"] = value; }
        }

        /// <summary>
        /// Gets or sets the feedLinesCount attribute.
        /// </summary>
        [ConfigurationProperty("feedLinesCount", DefaultValue = 5, IsKey = false, IsRequired = true)]
        [IntegerValidator(ExcludeRange=false, MinValue = byte.MinValue ,MaxValue = byte.MaxValue)]
        public int FeedLinesCount
        {
            get { return ((int)(base["feedLinesCount"])); }
            set { base["feedLinesCount"] = value; }
        }

        /// <summary>
        /// Gets or sets the receiptLineLength attribute.
        /// </summary>
        [ConfigurationProperty("receiptLineLength", IsKey = false, IsRequired = false)]
        public int? ReceiptLineLength
        {
            get { return ((int?)(base["receiptLineLength"])); }
            set { base["receiptLineLength"] = value; }
        }
    }

    internal static class ConfigurationFileReader
    {
        
        /// <summary>
        /// Converts string to enum
        /// </summary>
        /// <typeparam name="T">Enum type</typeparam>
        /// <param name="configElement">Configuration element</param>
        /// <param name="attributeName">Attribute name</param>
        /// <param name="value">string value</param>
        /// <returns></returns>
        private static T ConvertStringToEnum<T>(IConfigurationElementInfo configElement, string attributeName, string value) where T: struct
        {
            int intval;
            T retval;

            if (int.TryParse(value, out intval) || !Enum.TryParse(value, true, out retval))
            {
                ThrowInvalidValueExeption(configElement, attributeName, value);
            }

            return (T)Enum.Parse(typeof(T), value, true);
        }

        /// <summary>
        /// Throws exception.
        /// </summary>
        /// <param name="configElement"></param>
        /// <param name="resourceId"></param>
        /// <param name="args"></param>
        public static void ThrowExeption(IConfigurationElementInfo configElement, int resourceId, params object[] args)
        {
            string message = Resources.Translate(resourceId, args);
            ThrowExeption(configElement, message);
        }
        
        private static void ThrowExeption(IConfigurationElementInfo configElement, string message)
        {
            message = Resources.Translate(Resources.ConfigFileValidationError, configElement.LineNumber, message);
            throw new Exception(message);
        }

        private static void ThrowInvalidValueExeption(IConfigurationElementInfo configElement, string attibuteName, string value)
        {
            ThrowExeption(configElement, Resources.InvalidAttributeValue, attibuteName, value);
        }

        private static void ThrowRequiredAttrMissedExeption(IConfigurationElementInfo configElement, string attibuteName)
        {
            ThrowExeption(configElement, Resources.RequiredAttributeMissed, attibuteName);
        }

        /// <summary>
        /// Gets attribute value and converts it to enum type with errors processing
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="reader">XmlReader</param>
        /// <param name="attibuteName">attribute name</param>
        /// <param name="configElement"></param>
        /// <returns></returns>
        public static T GetEnumAttributeValue<T>(this XmlReader reader, string attibuteName, IConfigurationElementInfo configElement) where T: struct
        {
            string value = GetAttributeValue(reader, attibuteName, configElement);
            return ConvertStringToEnum<T>(configElement, attibuteName, value);
        }

        /// <summary>
        /// Gets attribute value and converts it to int type with errors processing
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="attibuteName"></param>
        /// <param name="configElement"></param>
        /// <returns></returns>
        public static int GetIntegerAttributeValue(this XmlReader reader, string attibuteName, IConfigurationElementInfo configElement, int minValue = int.MinValue, int maxValue = int.MaxValue)
        {
            int result = 0;
            string value = GetAttributeValue(reader, attibuteName, configElement);

            if (!int.TryParse(value, out result))
            {
                ThrowInvalidValueExeption(configElement, attibuteName, value);
            }

            if (result < minValue || result > maxValue)
            {
                ThrowInvalidValueExeption(configElement, attibuteName, value);
            }

            return result;
        }

        public static void ValidateForExcessAttributes(this XmlReader reader, IConfigurationElementInfo configElement)
        {
            for (bool attrExists = reader.MoveToFirstAttribute(); attrExists; attrExists = reader.MoveToNextAttribute())
            {
                if (FindAttribute(configElement, reader.Name) == null)
                {
                    ThrowExeption(configElement, Resources.InvalidAttribute, reader.Name);
                }
            }

            reader.MoveToElement();
        }

        private static Tuple<PropertyInfo, ConfigurationPropertyAttribute> FindAttribute(IConfigurationElementInfo configElement, string attibuteName)
        {
            var props = configElement.GetType().GetProperties();

            foreach (var pi in props)
            {
                foreach (var attr in pi.GetCustomAttributes(false))
                {
                    var configAttr = attr as ConfigurationPropertyAttribute;
                    if (configAttr != null && configAttr.Name == attibuteName)
                    {
                        return Tuple.Create(pi, configAttr);
                    }
                }
            }

            return null;
        }

        private static string GetAttributeDefaultValue(Tuple<PropertyInfo, ConfigurationPropertyAttribute> attr)
        {
            var defaultValue = attr.Item2.DefaultValue;
            string result = (defaultValue != null && !defaultValue.GetType().Equals(typeof(System.Object))) ? defaultValue.ToString() : String.Empty;
            return result;
        }

        private static string GetAttributeValue(XmlReader reader, string attibuteName, IConfigurationElementInfo configElement)
        {
            string value = reader.GetAttribute(attibuteName);

            if (string.IsNullOrEmpty(value))
            {
                var attr = FindAttribute(configElement, attibuteName);

                if (value == null && attr.Item2.IsRequired)
                {
                    ThrowRequiredAttrMissedExeption(configElement, attibuteName);
                }

                value = GetAttributeDefaultValue(attr);
            }

            return value;
        }

    }
    
}
