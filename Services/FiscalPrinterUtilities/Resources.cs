/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using LSRetailPosis;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// Resources class is use to retrieve strings from the resource file.
    /// The current implementation is to retrieve them from the POS
    /// ApplicationLocalizer.Language.Translate and leverage the same features
    /// available for other POS modules.
    /// 
    /// Resource string id ranges:
    /// 
    ///     - Fiscal Printer = 85050 to 85100
    ///                        86000 to 86999
    ///     - Daruma Driver  = 85000 to 85999
    ///     - Pocok Driver   = 87000 to 87999
    ///     - Bematech Driver= 88000 to 88999
    ///
    ///     - Daruma Driver (General Messages)  = 85000 to 85049
    ///     - Fiscal Printer (General Messages) = 85050 to 85099
    ///     - Daruma Driver (Printer Messages)  = 85100 to 85899
    ///     - Fiscal Printer (Settings Provider and EFT OPerations) - 85900 to 85999
    ///
    ///     - Daruma Driver  (General Messages)         = 85000 to 85049
    ///     - Fiscal Printer (General Messages)         = 85050 to 85099
    ///     - Daruma Driver  (Printer Messages)         = 85100 to 85899
    ///     - Fiscal Printer (Settings Provider)        = 85900 to 85924
    ///     - Fiscal Printer (EFT Operations)           = 85925 to 85999
    ///     - Fiscal Printer (Fiscal Menu)              = 86100 to 86149
    ///     - Fiscal Printer (General Messages 2)       = 86150 to 86199
    ///     - Fiscal Printer (Fiscal Document Model 2)  = 86200 to 86259
    ///     - Fiscal Printer (Inventory File)           = 86250 to 86279
    ///     - Fiscal Printer (Discount)                 = 86280 to 86299
    ///     
    ///     - Hungarian Messages                        = 86300 to 86399
    ///     - Russian Messages                          = 86400 to 86499
    /// 
    /// Note: the range for warning messages (85100 to 85200) is no longer used.
    /// 
    /// Sub ranges in Fiscal Printer Resources:
    ///     - general messages  - 85050 to 85099
    ///     - settings provider - 85900 to 85924
    ///     - eft operations    - 85925 to 85649
    /// 
    /// </summary>
    public static class Resources
    {
        public const int FiscalDocumentModel2AlreadyExists = 86096; //
        public const int CannotLinkModel2ReceiptVoidedLine = 86095; //You can't link the fiscal document model 02 to a fiscal receipt that contains canceled lines.
        public const int CannotLinkModel2ReceiptWithService = 86094; //You can't link the fiscal document model 02 to a fiscal receipt that contains service lines.
        public const int CannotVoidItemInLinkedModel2 = 86093; //You can't cancel a line on a fiscal receipt that is already linked to the fiscal document model 02.
        public const int CannotSellServicesItemsInLinkedModel2 = 86092; //You can't add a service line to a fiscal receipt that is already linked to the fiscal document model 02.
        public const int CannotPerformThisActionWithReceiptClosed = 86091; //A fiscal document model 2 can only be associated to an open fiscal receipt.
        public const int FiscalDocumentModel2AlreadyLinked = 86090; //The fiscal receipt is already associated to a fiscal document model 2.
        public const int FiscalDocumentModel2Linked = 86089; //The fiscal receipt was associated to a fiscal document model 2.
        public const int MessageAuthorizeEcfMd5FileListError = 86088; //The MD5 hash code from file AuthorizedECF does not match with the MD5 hash code from file Filelist.
        public const int FileDoesNotExistOrIsCorrupted = 86087; //The file {0} does not exist or is corrupted, please contact the support.
        public const int ConfigurationNonFiscalTotalizerNotCreated = 86086; // The non fiscal totalizer name is not set to issue gift card.
        public const int ConfigurationCannotConfigureNow = 86085; //Cannot create {0} at the fiscal printer.
        public const int ConfigurationErrorSlotIsFull = 86084; //There's no empty slot to create {0} at the fiscal printer.
        public const int ConfigurationErrorCreating = 86083; //An error occurred when trying to create {0} at the fiscal printer.
        public const int ConfigurationTaxes = 86082; //Taxes
        public const int ConfigurationNonFiscalTotalizers = 86081; //Non fiscal totalizers
        public const int ConfigurationTenderTypes = 86080; //Tender types
        public const int ConfigurationManagementReports = 86079; //Management reports        
        public const int MessageCannotVoidReceiptCardReversingNotSupported = 86078; //Cannot void the receipt, card reversing not supported.
        public const int MessageApproximateTaxAmountDescription = 86077; //Impostos aprox R${0} ({1}%) Fonte:{2}
        public const int MessageCannotVoidReceiptKeepReceipt = 86076; //Cannot void the receipt, keep the receipt.
        public const int MessagePrizeRedeemApplied = 86075; //Prize redeem (value {0}) was applied in total discount.
        public const int MessagePaymentCanceled = 86074; //The payment was canceled.
        public const int ParaibaRequiredMessage = 86073; //PARAIBA LEGAL RECEITA CIDADÃ - TORPEDO PREMIADO.
        public const int CannotUseFiscalPrinterForTheFirstTime = 86072; //The fiscal printer cannot be used for the first time when the fiscal receipt is open or the Z report is pending
        public const int MessageCannotPerformThisAction = 86071; // Cannot perform this action due to fiscal printer state: {0}
        public const int MessageSelectFiscalPrinter = 86070; // Select the fiscal printer.
        public const int FieldShouldNotExceedChars = 86069; // {0} should not exceed {1} chars: {2}
        public const int OriginalDocumentType = 86068;// Original document type
        public const int PrinterSerialNumber = 86067;// Printer serial number
        public const int MessageProvideOriginalReceiptInformation = 86066;// Please provide the original receipt information.        
        public const int MessageReturnTransactionItemNotValid = 86065; // This transaction contains items that cannot be returned.        
        public const int OriginalRefundReceiptNumber = 86064; // Original refund receipt information

        public const int InvalidOperationAfterStartPayment = 86063; //This operation is invalid when fiscal receipt is already totaled.
        public const int InvalidOperationAfterTotal = 86062; //This operation is invalid when payment has already started.
        public const int MessageFeatureUnavailableDatabaseOffline = 86061; //Feature unavailable when the database is offline.
        public const int MessageAccountantMissingAddress = 86060; //The accountant information is missing from the Fiscal establishment where the store belongs to.
        public const int MessageAccountantMissing = 86059; //The accountant information is missing from the Fiscal establishment where the store belongs to.
        public const int MessageAx32FileNotFound = 86058; //Ax32.exe file not found. The MD5 printed on fiscal receipt will be different from the official one

        public const int OriginalReceiptNumber = 86057; // Original receipt information
        public const int OriginalReceiptDate = 86056; // Printed at

        public const int MessageDifferentRatesForSameVat = 86055;// There are different tax rates associated with the same fiscal tax code ({0}) effective on the current date. Please fix this on Microsoft Dynamics AX Retail Headquarters.
        public const int MessageDayStartFailVatRatesMismatch = 86054; //The start of day operation failed because the VAT rates on the database and on the printer don't match.
        public const int MessageConfirmUpdatePrinterTaxRates = 86053; //VAT rate(s) from database ({0}) and printer ({1}) don't match. Do you want to update VAT rates on printer? (there are {2} updates left on current printer's EPROM)

        public const int MessageDayStartFailLastJournalMissing = 86052; //The start of day operation failed because the last Electronic Journal backup was not correctly saved.
        public const int MessageInvalidJournalSignature = 86051; //Electronic journal cannot be saved because digital signature could not be verified.
        public const int MessageInvalidJournalFormat = 86050; //Electronic journal format is invalid.

        public const int GiftCardNotCreatedDatabase = 86049;
        public const int GiftCardNotCreatedPrinter = 86048;
        public const int GiftCardCannotSaleInRegularCoupon = 86047; //a regular Cupom can’t have a gift card
        public const int GiftCardCannotHaveRegularItems = 86046; //a gift card sale can’t have a regular item

        public const int BlankOpFullCloseZ = 86045; //Full Closing report by Z number
        public const int BlankOpSimpleCloseZ = 86044; //Simple Closing report by Z number
        public const int BlankOpFullCloseDate = 86043; //Full Closing report by date
        public const int BlankOpSimpleCloseDate = 86042; //Simple Closing report by date
        public const int BlankOpRegularRefund = 86041; //Regular Refund
        public const int BlankOpBottleRefund = 86040; //Bottle Refund
        public const int BlankOpFiscalReceipt = 86039; //Fiscal Receipt
        public const int BlankOpSimplifiedInvoice = 86038; //Simplified Invoice
        public const int BlankOpStartOfDay = 86037; //Start Of Day        
        public const int BlankOpShowList = 86036; //Blank operations list
        public const int BlankOpAvailable = 86035; //Blank operations available:

        public const int MessageDiscountNotAuthorized = 86034; //Discount is not authorized.
        public const int MessageFiscalCodeTaxCodeNotFound = 86033;//Tax code {0} rate ({1}) does not match the Fiscal tax code {2} rate ({3}).
        public const int MessageFiscalCodeNotFound = 86032; //Fiscal tax code {0} was not found  in the fiscal printer Vat Table. 
        public const int MessageErrorSavingJournalToFile = 86031; //Could not save backup of Electronic Journal to file {0}
        public const int MessageFiscalTaxCodeNotFound = 86030;//The tax code {0} was not found in the fiscal tax table.
        public const int MessageFloatEntry = 86029;//Float entry amount:\t{0} {1}
        public const int MessageTenderRemoval = 86028;//Tender removal amount:\t{0} {1}
        public const int MessageCanceledByUser = 86027;//Canceled by the operator
        public const int MessageLineAmount = 86026;//Amount:\t{0}
        public const int MessageCashierReport = 86025; //Cashier Report
        public const int MessageOperationSafeDrop = 86024; //Operation: Safe drop
        public const int MessageOperationBankDrop = 86023; //Operation: Bank drop
        public const int MessageFileGenerated = 86022; //The file {0}{1}.ej was copied
        public const int MessageNoPendingEJournal = 86021; //There is no pending eletronic journal to be copied
        public const int MessageCopyingEJournalFile = 86020;//Attempt {0} to backup electronic journal in progress...
        public const int MessageEJournalChangeIsDue = 86019;//EJ card change is due
        public const int MessageEndZNumber = 86018; //Select the ending Z Number
        public const int MessageStartZNumber = 86017; //Select the starting Z Number
        public const int MessageDayAlreadyOpened = 86016; //The day is already opened
        public const int MessageReceiptIdAlreadyRefunded = 86015; //The receipt was already refunded.
        public const int MessageMustSwitchToEuroMode = 86014; //The start of day operation failed.  The system must switch to Euro mode.  Euro switch date={0}
        public const int MessageInvalidCurrency = 86013; //"Invalid store currency. Current currency={0}.  Expected currency={1}"
        public const int MessageIn = 86012;//In
        public const int MessageUnknownReturnedTransaction = 86011;//Unknown original returned transaction. Regular Refund was not selected.
        public const int MessageRefundTypeUnknown = 86010;//A refund transaction type must be specified.  Select Bottle Refund or Regular Refund.
        public const int MessageSaleItemNotValid = 86009;//A sale item cannot be added to a Return transaction
        public const int MessageReturnItemNotValid = 86008; //A return item cannot be added to a Sale transaction.
        public const int MessageConfirmStartOfDay = 86007; //Do you want to start the day?
        public const int MessageExchangeRate = 86006; //Exchange rate
        public const int MessageInEuro = 86005; //In Euro
        public const int MessageCustomerMustBeSpecified = 86004; //Customer name and address must be specified.
        public const int MessagePrintingStartedButDidNotFinished = 86003; //Check the printer.  The last printing started but didn't finish.
        public const int MessageWrongTransactionType = 86002; //Wrong transaction type.  The current document type was not changed.
        public const int MessageRegularRefundSelected = 86001; //Regular Refund Selected
        public const int MessageBottleRefundSelected = 86000; //Bottle Refund Selected
        public const int MessagePendingZReport = 85288; //The Z report for the previous day is pending.
        public const int MessageZReportAlreadyPrinted = 85289; //A Z Report for today has already been generated
        public const int MessageFiscalReceiptSelected = 85099; //Fiscal Receipt Selected
        public const int MessageSimplifiedInvoiceSelected = 85098; //Simplified Invoice Selected
        public const int MessageCouldNotOpenSerialPort = 85097; //The serial port could not be opened.  Command='{0}'.
        public const int MessagePrinterNotReady = 85096; //Printer is not ready
        public const int MessageWithoutPaper = 85095; //End of paper or paper sensor with problems
        public const int MessageOperationCanceled = 85094; //Operation canceled: {0}
        public const int MessageDisplayNotReady = 85093; //Display is not ready
        public const int MessageConfirmSwitchingEuroMode = 85092; //Do you want to switch to Euro Mode?  Once you switch to Euro Mode, you WILL NOT be able to switch back.
        public const int MessageFinancialTransaction = 85089; //"Financial Transaction"
        public const int MessageInvalidDocumentTypeChange = 85088; //"To change the document type a transaction must be opened."

        // Fiscal Printer (General Messages)
        public const int MessageInvalidGrandTotal = 85025; // The Grand Total returned by the printer is not a number = {0}
        public const int MessageInvalidSubtotal = 85026; // The Subtotal returned by the printer is not a number = {0}
        public const int MessageInvalidBalanceDue = 85027; //The 'Balance Due' returned by the printer is not a number = {0}
        public const int InvalidStateManagementReportBegin = 85030; // Attempt to open management report from an invalid state: {0}
        public const int InvalidStateManagementReportPrintLine = 85031; // Attempt to print line of management report from an invalid state: {0}
        public const int InvalidStateManagementReportPrintBarcode = 85032; // Attempt to print barcode of management report from an invalid state: {0}
        public const int InvalidStateManagementReportEnd = 85033; // Attempt to end management report from an invalid state: {0}

        public const int MessageNfpFileList = 85042;
        public const int MessageNfpFileListDoesNotExists = 85043;

        public const int CustomerTaxId = 85050;
        public const int CustomerTaxIdMessage = 85051;
        public const int CustomerTaxIdUserIdLabel = 85052;
        public const int SalesItemWithoutDescription = 85055;
        public const int PrintReceiptInvalidState = 85056;
        public const int PrintSlipInvalidState = 85057;
        public const int VoidItemWithoutOpenReceipt = 85058;
        public const int SalesItemWithoutUnitMeasure = 85059;
        public const int MessageNoPendingZReport = 85060;
        public const int MessageTaxCodeNotFound = 85061;
        public const int MessageCouldNotFindValidTax = 85062;
        public const int MessageMultipleTaxLines = 85063;
        public const int MessageNoTenderTypeFound = 85064;
        public const int MessageSubtotalMismatch = 85065;
        public const int MessageBalanceDueMismatch = 85066;
        public const int MessageCouldNotLoadPrinterDriver = 85067;
        public const int MessageAggregateItems = 85068;
        public const int MessageAggregatePayments = 85069;
        public const int MessageAggregateItemsForPrinting = 85070;
        public const int MessageZReportOnPos = 85071;
        public const int MessageCountryStoreBrazil = 85072;
        public const int MessageTaxIncludedInPrice = 85073; // deprecated
        public const int MessageDeviceTypeNone = 85074;
        public const int MessageDeviceName = 85075;
        public const int MessageTaxForItemNotFound = 85076;
        public const int MessageInvalidTimeDifference = 85077;
        public const int MessageConfirmFileGeneration = 85078;
        public const int MessageStartDate = 85079;
        public const int MessageEndDate = 85080;
        public const int MessageCouldNotFindValidTaxCodeForItem = 85081;
        public const int MessageSupportedCountryRegion = 85082;
        public const int MessageGeneratingFiles = 85083;
        public const int MessagePrinterOpened = 85084;
        public const int MessagePaperNearEnd = 85085;
        public const int MessageInvalidOperationInBrazil = 85086;
        public const int MessageMismatchBalanceCancelReceipt = 85087;

        // Fiscal Printer (Settings Provider)
        public const int SettingsProviderInvalidFileStructure = 85900;
        public const int SettingsProviderFileNotFound = 85901;
        public const int SettingsProviderConfirmFileSaveTitle = 85902;
        public const int SettingsProviderButtonClose = 85903;
        public const int SettingsProviderButtonSave = 85904;
        public const int SettingsProviderButtonRestore = 85905;
        public const int SettingsProviderGroupBoxFilter = 85906;
        public const int SettingsProviderGroupBoxKeysAndValues = 85907;
        public const int SettingsProviderLabelCurrentValue = 85908;
        public const int SettingsProviderLabelDefaultValue = 85909;
        public const int SettingsProviderLabelGroup = 85910;
        public const int SettingsProviderLabelKey = 85911;
        public const int SettingsProviderLabelKeysNotFound = 85912;
        public const int SettingsProviderRadioButtonReadWriteNo = 85913;
        public const int SettingsProviderRadioButtonReadWriteYes = 85914;
        public const int SettingsProviderTitle = 85915;
        public const int SettingsProviderMessageAllKeysAlreadyUpdated = 85916;
        public const int SettingsProviderMessageOneKeyRestored = 85917;
        public const int SettingsProviderMessageMultipleKeysRestored = 85918;
        public const int SettingsProviderConfirmFileSave = 85919;
        public const int SettingsProviderFileSavedSuccessfully = 85920;
        public const int SettingsProviderGroupAndKeyNotFound = 85921;

        // Fiscal Printer (EFT Operations)
        public const int CardPaymentDiscardTransaction = 85933;
        public const int CardPaymentErrorCancelingReceipt = 85934;
        public const int CardPaymentErrorCancelingOperation = 85935;
        public const int CardPaymentRequestFileInvalidStateClose = 85936;
        public const int CardPaymentRequestFileInvalidStateOpen = 85937;
        public const int CardPaymentFoldersNotExists = 85938;
        public const int CardPaymentInvalidTransactionId = 85939;
        public const int CardPaymentInvalidFileHeader = 85940;
        public const int CardPaymentInvalidTransactionStatus = 85941;
        public const int CardPaymentMessagePaymentCanceled = 85942;
        public const int CardPaymentInvalidTotalAmount = 85943;
        public const int CardPaymentProgramManagerNotRunning = 85945;
        public const int CardPaymentProgramManagerNotResponding = 85946;
        public const int CardPaymentNotSetup = 85947;
        public const int CardPaymentSingleCardOnly = 85948;
        public const int CardPaymentWithdrawNotAllowed = 85949;
        public const int CardOperationError = 85950;
        public const int CardPaymentNotAvailable = 85951;
        public const int CardPaymentWrongMessageFromCardProvider = 85952;
        public const int CardFormAdministrativeFunctionsTitle = 85953;
        public const int CardFormButtonCancelPayment = 85954;
        public const int CardFormButtonPrizeRedeem = 85955;
        public const int CardFormButtonReprint = 85956;
        public const int CardPaymentMaximumCardPayments = 85957;

        // Fiscal Printer (Fiscal Menu)
        public const int FiscalMenuDateInterval = 86100;
        public const int FiscalMenuCrzInterval = 86101;
        public const int FiscalMenuPrintReport = 86102;
        public const int FiscalMenuFileDocumentCopy = 86103;
        public const int FiscalMenuFileAtoCotepeIcms1704 = 86104;
        public const int FiscalMenuCooInterval = 86105;
        public const int FiscalMenuRecordReportPartialInventory = 86106;
        public const int FiscalMenuRecordReportCompleteInventory = 86107;
        public const int FiscalMenuEcfSerialNumber = 86108;
        public const int FiscalMenuFileConvenio5795 = 86109;
        public const int FiscalMenuFileAtoCotepeIcms0908 = 86110;
        public const int FiscalMenuBomTableMessage = 86111;
        public const int FiscalMenuInvalidValue = 86112;
        public const int FiscalMenuEnterValueBetween = 86113;
        public const int FiscalMenuEnterValueGreaterThan = 86114;
        public const int FiscalMenuContinueTheGeneration = 86115;
        public const int FiscalMenuXReport = 86116;
        public const int FiscalMenuFullFiscalMemory = 86117;
        public const int FiscalMenuSimplifiedFiscalMemory = 86118;
        public const int FiscalMenuPrinterMemoryDetailFile = 86120;
        public const int FiscalMenuPafEcfIdentification = 86125;
        public const int FiscalMenuSalesByDateInterval = 86126;
        public const int FiscalMenuBomTable = 86127;
        public const int FiscalMenuConfigurationParameters = 86128;
        public const int FiscalMenu = 86129;
        public const int FiscalMenuNoDataAvailable = 86130;
        public const int FiscalMenuShowFileInfolder = 86131;
        public const int FiscalMenuFileDoesNotExist = 86132;
        public const int FiscalMenuStartRange = 86133;
        public const int FiscalMenuEndRange = 86134;
        public const int FiscalMenuFileCorrupted = 86135;
        public const int FiscalMenuProcessing = 86136;
        public const int FiscalMenuMgmtRptNotCreatedDatabase = 86137;
        public const int FiscalMenuMgmtRptNotCreatedPrinter = 86138;
        public const int FiscalMenuFiscalMemoryPrinting = 86140;
        public const int FiscalMenuFiscalMemoryFile = 86141;
        public const int FiscalMenuRecordReport = 86142;

        // Fiscal Printer (General Messages 2)       = 86150 to 86199
        public const int VoidItemCannotUnvoid = 86150;
        public const int MessageCouldNotLoadPartnerExtension = 86151;
        public const int CustomerAddress = 86152;
        public const int CustomerName = 86153;
        public const int CustomerGetFromCustomer = 86154;
        public const int CustomerClearData = 86155;
        public const int MessageMd5FileListCreated = 86156;
        public const int MessageAuthorizeEcfError = 86157;
        public const int MessageFunctionalityProfileOutsideBr = 86158; // deprecated
        public const int MessageCustomerTaxIncludedInPrice = 86159; // deprecated
        public const int MessageCustomerCannotChangedOpenReceipt = 86160;
        public const int MessageConfirmZReport = 86161;
        public const int MessageEndReceiptFailed = 86162;
        public const int MessageAuthorizeEcfGrandTotalError = 86163;
        public const int MessageCityNotFound = 86164;
        public const int CannotMakePaymentWhileOpenReceipt = 86165;
        public const int CannotCloseReceiptWithoutPayment = 86166;
        public const int MessageErrorCreatingSignature = 86167;
        public const int ItemSearchUnitMeasure = 86168;
        public const int ItemSearchRoundTruncate = 86169;
        public const int ItemSearchOwnThird = 86170;
        public const int ItemSearchTributarySituation = 86171;
        public const int VoidLastTransaction = 86172;
        public const int UnableVoidLastTransaction = 86173;
        public const int CancelingEftPayment = 86174;
        public const int ErrorCancelingEftPayment = 86175;
        public const int PaymentOperationError = 86176;
        public const int FeatureNotSupportedInProfile = 86177;

        // Fiscal Printer (Fiscal Document Model 2)
        public const int FiscalDocumentModel2 = 86200;
        public const int FiscalDocumentModel2HeaderInvoice = 86201;
        public const int FiscalDocumentModel2HeaderSeries = 86202;
        public const int FiscalDocumentModel2HeaderDate = 86203;
        public const int FiscalDocumentModel2HeaderCustomer = 86204;
        public const int FiscalDocumentModel2HeaderCustomerName = 86205;
        public const int FiscalDocumentModel2HeaderCnpjCpf = 86206;
        public const int FiscalDocumentModel2HeaderRefused = 86207;
        public const int FiscalDocumentModel2LineItem = 86208;
        public const int FiscalDocumentModel2LineItemName = 86209;
        public const int FiscalDocumentModel2LineQuantity = 86210;
        public const int FiscalDocumentModel2LineUnitPrice = 86211;
        public const int FiscalDocumentModel2LineDiscountPct = 86212;
        public const int FiscalDocumentModel2LineDiscountAmount = 86213;
        public const int FiscalDocumentModel2LineTotalPrice = 86214;
        public const int FiscalDocumentModel2PaymentTenderType = 86215;
        public const int FiscalDocumentModel2PaymentAmount = 86216;
        public const int FiscalDocumentModel2PaymentTotal = 86217;
        public const int FiscalDocumentModel2SalesTotal = 86218;
        public const int FiscalDocumentModel2ButtonFiscalDocumentSearch = 86219;
        public const int FiscalDocumentModel2ButtonAddFiscalDocument = 86220;
        public const int FiscalDocumentModel2ButtonSave = 86221;
        public const int FiscalDocumentModel2ButtonCancel = 86222;
        public const int FiscalDocumentModel2ButtonClose = 86223;
        public const int FiscalDocumentModel2MessageDuplicateInvoice = 86224;
        public const int FiscalDocumentModel2MessageInvalidCnpjCpf = 86225;
        public const int FiscalDocumentModel2MessageInvalidDate = 86226;
        public const int FiscalDocumentModel2MessagePaymentDifference = 86227;
        public const int FiscalDocumentModel2MessagePaymentMissing = 86228;
        public const int FiscalDocumentModel2MessageInvalidSeries = 86229;
        public const int FiscalDocumentModel2MessageInvalidInvoice = 86230;
        public const int FiscalDocumentModel2MessageInvalidItem = 86231;
        public const int FiscalDocumentModel2MessageInvalidCustomer = 86232;
        public const int FiscalDocumentModel2MessageNegativeValue = 86233;
        public const int FiscalDocumentModel2MessageNonnumericValue = 86234;
        public const int FiscalDocumentModel2MessageDuplicatedTenderType = 86235;
        public const int FiscalDocumentModel2MessageInvalidDiscount = 86236;
        public const int FiscalDocumentModel2MessageConfirmation = 86237;
        public const int FiscalDocumentModel2MessageInvalidQuantity = 86238;
        public const int FiscalDocumentModel2MessageInvalidUnitPrice = 86239;
        public const int FiscalDocumentModel2Search = 86240;
        public const int FiscalDocumentModel2SearchDateFrom = 86241;
        public const int FiscalDocumentModel2SearchDateTo = 86242;
        public const int FiscalDocumentModel2SearchInvoice = 86243;
        public const int FiscalDocumentModel2SearchSearch = 86244;
        public const int FiscalDocumentModel2SearchCancel = 86245;
        public const int FiscalDocumentModel2TransactionOpen = 86246;
        public const int FiscalDocumentModel2MessageSeriesFormat = 86247;
        public const int FiscalDocumentModel2ServicesNotAllowed = 86248;
        public const int FiscalDocumentModel2MessageCannotShowForm = 86249;

        //Exceptions used in Fiscal Printer
        public const int ZReportTitle = 85001;

        public const int MessageTotalDiscount = 64;//Total discount
        public const int MessageDiscount = 62;//Discount

        public const int PocokDriverStartRange = 87000;

        // Fiscal Printer (Inventory File)
        public const int InventoryFileFormTitle = 86250;
        public const int InventoryFileLabelItemId = 86251;
        public const int InventoryFileLabelItemName = 86252;
        public const int InventoryFileButtonSearch = 86253;
        public const int InventoryFileLabelItemsFound = 86254;
        public const int InventoryFileButtonSelect = 86255;
        public const int InventoryFileLabelItemsSelected = 86256;
        public const int InventoryFileButtonExclude = 86257;
        public const int InventoryFileButtonOk = 86258;
        public const int InventoryFileButtonClose = 86259;
        public const int InventoryFileColumnItemId = 86260;
        public const int InventoryFileColumnItemName = 86261;

        // Fiscal Printer (Discounts)
        public const int DiscountLineAfterTotal = 86281;
        public const int DiscountTotalAfterPayment = 86282;
        public const int CannotSellItemAfterPayment = 86283;

        // Fiscal printer (General messages)
        public const int MessageDoNotUseCustomerTaxes = 86284;
        public const int MessageDoNotUseDestinationTaxes = 86285;
        public const int MessageUncheckTaxIncludedInPrice = 86286; //In the Stores form, on the General tab, the Tax included in price check box must be unchecked.

        // Fiscal printer (Hungarian messages) - 86300 - 86399
        public const int MessageCountryStoreHungary = 86300;

        // Fiscal printer (Russian messages) - 86400 - 86499

        /// <summary>
        /// The store address must be in Russia. Current country code={0}
        /// </summary>
        public const int MessageCountryStoreRussia = 86400;
        /// <summary>
        /// Document section of type {0} is allowed only in layouts of type {1}.
        /// </summary>
        public const int DocumentSectionOfTypeIsAllowedOnlyInLayoutsOfType = 86401;
        /// <summary>
        /// Line of type {0} is allowed only in layouts of type {1}.
        /// </summary>
        public const int LineOfTypeIsAllowedOnlyInLayoutsOfType = 86402;
        /// <summary>
        /// Duplicate tax code {0} is found in the tax mapping settings
        /// </summary>
        public const int DuplicateTaxMappingFound = 86403;
        /// <summary>
        /// Tax code {0} has no mapping with printer tax.
        /// </summary>
        public const int TaxMappingIsNotFound = 86404;
        /// <summary>
        /// Tender type {0} is not recognized by the fiscal printer.
        /// </summary>
        public const int TenderTypeIsNotSupported = 86405;
        /// <summary>
        /// Line of type {0} is allowed only in document sections of type {1}.
        /// </summary>
        public const int LineOfTypeIsAllowedOnlyInDocumentSectionsOfType = 86406;
        /// <summary>
        /// Total non cash paid amount alrealy exceeds the total order amount.
        /// </summary>
        public const int TotalNoncashAmountExceedsOrderAmount = 86407;
        /// <summary>
        /// Error occured while reading configuration file. {0} Contact system administrator.
        /// </summary>
        public const int FailedToLoadConfigFile = 86408;
        /// <summary>
        /// Field of type {0} is allowed only in layouts of type {1}.
        /// </summary>
        public const int FieldOfTypeIsAllowedOnlyInLayoutsOfType = 86409;
        /// <summary>
        /// Field of type {0} is allowed only in document sections of type {1}.
        /// </summary>
        public const int FieldOfTypeIsAllowedOnlyInDocumentSectionsOfType = 86410;
        /// <summary>
        /// Invalid printing submode: {0}
        /// </summary>
        public const int InvalidPrintingSubMode = 86411;
        /// <summary>
        /// Current fiscal printer shift is open more than 24 hours. Please close the shift.
        /// </summary>
        public const int PrinterDayIsOpenMoreThan24Hours = 86412;
        /// <summary>
        /// Two or more tax codes are mapped with the same printer tax.
        /// </summary>
        public const int MultipleTaxMapping = 86413;
        /// <summary>
        /// Tax code {0} has mapping with not existing printer tax {1}
        /// </summary>
        public const int TaxMappingWithWrongTaxFound = 86414;
        /// <summary>
        /// Printer tax calculation mode is not supported. Only tax calculation by operation is supported.
        /// </summary>
        public const int PrinterTaxModeNotSupported = 86415;
        /// <summary>
        /// File with image does not exist.
        /// </summary>
        public const int ImageFileDoesNotExist = 86416;
        /// <summary>
        /// Image should not be bigger than {0} x {1}.
        /// </summary>
        public const int ImageIsTooBig = 86417;
        /// <summary>
        /// Some printer parameters could not be set.
        /// </summary>
        public const int SomePrinterParametersCannotBeSet = 86418;
        /// <summary>
        /// Parameter cannot be written to printer table. Check config file. Layout '{0}', Table {1}, Row {2}, Field {3}.
        /// </summary>
        public const int ParameterCannotBeWrittenToPrinter = 86419;
        /// <summary>
        /// Only '.bmp' file can be loaded to printer.
        /// </summary>
        public const int OnlyBMPFileCanBeLoaded = 86420;
        /// <summary>
        /// Section with type SalesLine allowed only for Sales and Return layout.
        /// </summary>
        public const int SalesLineSectionValidOnlyForSalesLayout = 86421;
        /// <summary>
        /// Only one SalesFiscal line allowed per section.
        /// </summary>
        public const int OnlyOneSalesFiscalLineAllowedPerSection = 86422;
        /// <summary>
        /// Field of type {0} is allowed only in lines of type {1}.
        /// </summary>
        public const int FieldOfTypeIsAllowedOnlyInLinesOfType = 86423;
        /// <summary>
        /// Printer parameters can be updated in the table {0} only when the printer shift is closed.
        /// </summary>
        public const int TableCanBeWrittenWhenShiftIsClosed = 86424;
        /// <summary>
        /// Error in configuration file at line {0}. {1}
        /// </summary>
        public const int ConfigFileValidationError = 86425;
        /// <summary>
        /// Configuration file '{0}' has not been found.
        /// </summary>
        public const int ConfigFileNotFound = 86426;
        /// <summary>
        /// Image start line cannot be greater than end line.
        /// </summary>
        public const int ImageStartLineGreaterThanEndLine = 86427;
        /// <summary>
        /// Image start and end lines should be within 1..1200 range.
        /// </summary>
        public const int ImageLineInvalidRange = 86428;
        /// <summary>
        /// Image with id = {0} cannot be found.
        /// </summary>
        public const int InvalidImageId = 86429;
        /// <summary>
        /// Required attribute {0} is missed.
        /// </summary>
        public const int RequiredAttributeMissed = 86430;
        /// <summary>
        /// Attribute '{0}' has invalid value ('{1}').
        /// </summary>
        public const int InvalidAttributeValue = 86431;
        /// <summary>
        /// Tax mapping is not set up.
        /// </summary>
        public const int TaxMappingIsNotSet = 86432;
        /// <summary>
        /// Table {0} can only be written at the printer initialization.
        /// </summary>
        public const int TableCanOnlyBeWrittenAtInitialization = 86433;
        /// <summary>
        /// Invalid integer value '{0}' for table {1} field {2} row {3}.
        /// </summary>
        public const int InvalidIntegerNumberValue = 86434;
        /// <summary>
        /// Error during printing: {0} Contact system administrator.
        /// </summary>
        public const int ErrorWhilePrinting = 86435;
        /// <summary>
        /// Section with type TotalsHeader allowed only for Sales and Return layout.
        /// </summary>
        public const int TotalsHeaderSectionValidOnlyForSalesLayout = 86436;
        /// <summary>
        /// hideIfEmptyField attribute is not allowed in the SalesFiscal line.
        /// </summary>
        public const int HideIfEmptyFieldNotAllowedInSalesFiscalLine = 86437;
        /// <summary>
        /// Tender types mapping is empty.
        /// </summary>
        public const int TenderTypesMappingIsEmpty = 86438;
        /// <summary>
        /// Duplicate tender type {0}.
        /// </summary>
        public const int DuplicateTenderType = 86439;
        /// <summary>
        /// printerPaymentType should be in range {0} - {1}.
        /// </summary>
        public const int PrinterPaymentTypeShouldBeInRange = 86440;
        /// <summary>
        /// Fiscal printer error: {0}.
        /// </summary>
        public const int FiscalPrinterError = 86441;
        /// <summary>
        /// Layout of type {0} can only contain document section of type {1}.
        /// </summary>
        public const int LayoutOfTypeCanOnlyContainSectionOfType = 86442;
        /// <summary>
        /// hideIfEmptyField attribute is not allowed for Field of type {0}.
        /// </summary>
        public const int HideIfEmptyFieldNotAllowedForFieldOfType = 86443;
        /// <summary>
        /// An error occurred during report printing.
        /// </summary>
        /// <remarks>
        /// This message is shown in Retry/Cancel dialog when an error occurs during slip document printing.
        /// </remarks>
        public const int SlipDocumentPrintError = 86444;
        /// <summary>
        /// Layout of type {0} is not defined in the configuration file.
        /// </summary>
        public const int LayoutOfTypeIsNotDefinedInTheConfigurationFile = 86445;
        /// <summary>
        /// The fiscal receipt amount does not match the amount of the transaction. The fiscal receipt amount is {0}. The fiscal receipt will be canceled.
        /// </summary>
        public const int PrinterTotalAmountNotEqualPOSTotalAmount = 86446;
        /// <summary>
        /// Layout of type {0} can not contain section of type {1}.
        /// </summary>
        public const int LayoutOfTypeCannotContainSectionOfType = 86447;
        /// <summary>
        /// Layout of type {0} can not contain line of type {1}.
        /// </summary>
        public const int LayoutOfTypeCannotContainLineOfType = 86448;
        /// <summary>
        /// Layout of type {0} can not contain field of type {1}.
        /// </summary>
        public const int LayoutOfTypeCannotContainFieldOfType = 86449;
        /// <summary>
        /// receiptLineLength parameter value in RibbonSettings section exceeds maximum number of characters that could be printed on the current ribbon. Line length is set to printer driver value.
        /// </summary>
        public const int ReceiptLineLengthGreaterThanDriverValue = 86450;
        /// <summary>
        /// Layout of type {0} can contain only lines of type {1}.
        /// </summary>
        public const int LayoutOfTypeCanContainOnlyLinesOfType = 86451;
        /// <summary>
        /// Layout of type {0} can contain only fields of type {1}.
        /// </summary>
        public const int LayoutOfTypeCanContainOnlyFieldsOfType = 86452;
        /// <summary>
        /// Document section of type {0} can not contain line of type {1}.
        /// </summary>
        public const int DocumentSectionOfTypeCannotContainLineOfType = 86453;
        /// <summary>
        /// Document section of type {0} can not contain field of type {1}.
        /// </summary>
        public const int DocumentSectionOfTypeCannotContainFieldOfType = 86454;
        /// <summary>
        /// Doucment section of type {0} can contain only lines of type {1}.
        /// </summary>
        public const int DocumentSectionOfTypeCanContainOnlyLinesOfType = 86455;
        /// <summary>
        /// Document section of type {0} can contain only fields of type {1}.
        /// </summary>
        public const int DocumentSectionOfTypeCanContainOnlyFieldsOfType = 86456;
        /// <summary>
        /// Line of type {0} can not contain field of type {1}.
        /// </summary>
        public const int LineOfTypeCannotContainFieldOfType = 86457;
        /// <summary>
        /// Line of type {0} can only contain fields of type {1}.
        /// </summary>
        public const int LineOfTypeCanContainOnlyFieldsOfType = 86458;
        /// <summary>
        /// The layout definition cannot contain more than one line of the same discount type.
        /// </summary>
        public const int LayoutCannotContainDuplicateDiscountLines = 86459;
        /// <summary>
        /// A layout definition with a summary-discount line cannot have lines for other types of discounts.
        /// </summary>
        public const int OtherDiscountsNotAllowedWithSummaryDiscount = 86460;
        /// <summary>
        /// A discount line is not allowed before a fiscal line.
        /// </summary>
        public const int DiscountLineNotAllowedBeforeFiscalLine = 86461;
        /// <summary>
        /// A section definition cannot contain more than one line of the same discount type.
        /// </summary>
        public const int DocumentSectionCannotContainDuplicateDiscountLines = 86462;
        /// <summary>
        /// Attribute '{0}' is not supported.
        /// </summary>
        public const int InvalidAttribute = 86463;
        /// <summary>
        /// Old StrihM printer driver version. Please make sure that it is 4.8 or higher. Contact system administrator.
        /// </summary>
        public const int OldVersionOfShtrihMDriver = 86464;
        /// <summary>
        /// Error occured while loading ShtrihM fiscal printer driver. Contact system administrator.
        /// </summary>
        public const int FailedToLoadShtrihMDriver = 86465;
        /// <summary>
        /// Error writing fiscal attributes from the fiscal printer
        /// </summary>
        public const int ErrorWritingFiscalAttributesFromThePrinter = 86466;
        /// <summary>
        /// Error reading fiscal attributes from the fiscal printer
        /// </summary>
        public const int ErrorReadingFiscalAttributesFromThePrinter = 86467;
        /// <summary>
        /// receiptLineLength parameter value in RibbonSettings section cannot be less than 1. Line length is set to printer driver value.
        /// </summary>
        public const int ReceiptLineLengthLessThan1 = 86468;
        /// <summary>
        /// You cannot return and sale items in the same fiscal receipt. Register the sale of items as a separate operation.
        /// </summary>
        public const int SaleAndReturnItemsInSameReceipt = 86469;
        /// <summary>
        /// RoundingDiscount line and addRoundingToDiscount property of ReceiptDiscount or SummaryDiscount lines cannot be specified simultaneously.
        /// </summary>
        public const int RoundingDiscountLineWithAddRoundingToDiscountPropertyNotAllowed = 86470;
        /// <summary>
        /// Document section cannot contain more than one gift card payment line.
        /// </summary>
        public const int DocumentSectionCannotContainMoreThanOneGiftCardPaymentLine = 86471;

        // Existing LSPOSNET resources that does not contain a constant
        public const int FieldNameItemId = 73;
        public const int FieldNameItemDescription = 1486;
        public const int FieldNameUnitOfMeasure = 102;
        public const int FieldNameTaxCode = 74;
        public const int FieldNamePaymentMethod = 1956;
        public const int FieldNameBarcode = 72;
        public const int OperationInvalidForTransaction = 3033;
        public const int DiscountNotAllowedOnItem = 3184;
        public const int CardPaymentFormTitle = 50042;
        public const int InvalidDate = 3602; //The date is not valid.

        /// <summary>
        /// Retrieve the string identified by resourceId and format the final
        /// message based on the args based.
        /// </summary>
        /// <param name="resourceId">The resource identifier to look for.</param>
        /// <param name="args">The optional args to format message with.</param>
        /// <returns>The formated message.</returns>
        public static string Translate(int resourceId, params object[] args)
        {
            return ApplicationLocalizer.Language.Translate(resourceId, args);
        }
    }
}
