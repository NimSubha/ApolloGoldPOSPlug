/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>
    /// Class holding the various constants specific to Tax Engine retail integration.
    /// </summary>
    public static class TaxEngineRetailConstants
    {
        //Fields
        public const string CompositionScheme = "Composition Scheme";
        public const string ExportOrder = "Export Order";
        public const string ImportOrder = "Import Order";
        public const string ForeignParty = "Foreign Party";
        public const string LedgerCurrency = "Ledger Currency";
        public const string Currency = "Currency";
        public const string PricesIncludeSalesTax = "Prices include sales tax";
        public const string NatureOfAssesse = "Nature of Assesse";
        public const string PreferrentialParty = "Preferrential Party";
        public const string Skipped = "Skipped";
        public const string TaxAsPerOriginalInvoice = "Tax as per Original Invoice";
        public const string TaxableDocumentType = "Taxable Document Type";
        public const string AssessableValue = "Assessable Value";
        public const string CustomTariffCode = "Custom Tariff Code";
        public const string DeliveryDate = "Delivery Date";
        public const string DiscountAmount = "DiscountAmount";
        public const string Exempt = "Exempt";
        public const string GSTRegistrationNumber = "GST Registration Number";
        public const string HSNCode = "HSN Code";
        public const string IECNumber = "IEC Number";
        public const string InterState = "Inter-State";
        public const string NatureOfGoodsAndService = "Nature of Goods and Service";
        public const string NetAmount = "Net Amount";
        public const string ProductType = "Product Type";
        public const string Purpose = "Purpose";
        public const string Quantity = "Quantity";
        public const string SAC = "SAC";
        public const string ConsumptionState = "Consumption State";
        public const string ServiceCategory = "Service Category";
        public const string ITCCategory = "ITC Category";
        public const string PartyGSTRegistrationNumber = "Party GST Registration Number";
        public const string TaxDocumentPurpose = "Tax Document Purpose";
        public const string TransactionCurrency = "Transaction Currency";
        public const string TransactionDate = "Transaction Date";
        public const string AmountInTransactionCurrency = "Amount in transaction currency";
        public const string CalculatedValue = "Calculated value";
        public const string ChargeDate = "Charge date";
        public const string ChargesCategory = "Charges category";
        public const string ChargesCode = "Charges code";
        public const string ChargesValue = "Charges value";
        public const string MaximumRetailPrice = "Maximum Retail Price";
        public const string RecId = "RecId";
        public const string TableId = "TableId";
        public const string SubLines = "Miscellaneous Charge Line";
        public const string Header = "Header";
        public const string Postable = "Postable";
        public const string TaxDirection = "Tax Direction";
        public const string Return = "Return";
        public const string PostToLedger = "Post To Ledger";
        public const string EnableAccounting = "Enable Accounting";
        public const string IsScrap = "Is Scrap";
	public const string LineType = "Line Type";
        public const string LineTypeLine = "Line Type Line";
        public const string LineTypeMiscChargeLine = "Line Type Miscellaneous Charge Line";
        public const string TotalDiscountPercentage = "Total Discount Percentage";
        
        //Const values
        public const string No = "No";
        public const string Yes = "Yes";
        public const string Input = "Input";
        public const string Inward = "Inward";
        public const string Transaction = "Transaction";
        public const string Item = "Item";
        public const string Service = "Service";
        public const string SalesOrder = "Sales order";
        public const string SalesInvoice = "Sales invoice";
        public const string SalesTaxPayable = "Sales tax payable";

        public const string PathDelimeter = "/";
        public const char EREnumSeparator = '#';

        public const string TaxDocumentMeasureTaxAmount = "Tax Amount";
        public const string TaxDocumentMeasureBaseAmount = "Base Amount";
        public const string TaxDocumentMeasureRate = "Rate";

        public const long TransactionRecordIdFirstElement = 1;
        public const long TransactionRecordId = 1;
        public const int TransactionTableId = 1;
        public const int TransactionLineTableId = 2;
        public const int TransactionLineChargeTableId = 3;
        public const int TransactionHeaderChargeTableId = 4;

        public const long TaxContextPartition = 0;
        public const int TaxContextSessionId = 1;

        // The string of the value of enum ITCCategory
        public const string ITCCategoryInput = "Input";
        public const string ITCCategoryCapitalGoods = "Capital Goods";
        public const string ITCCategoryOthers = "Others";

        // The string of the value of enum ServiceCategory
        public const string ServiceCategoryInward = "Inward";
        public const string ServiceCategoryInterUnit = "Inter-unit or input";
        public const string ServiceCategoryOthers = "Others";
    }
}
