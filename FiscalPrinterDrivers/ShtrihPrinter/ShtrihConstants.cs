/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter.Constants
{
    /// <summary>
    /// Class holding the various constants specific to Shtrih-M printer driver.
    /// </summary>
    internal static class ShtrihConstants
    {
        public const int ReceiptOperationRegisterNumberValue = 148,
             TaxTableNumber = 6,
             TenderTypeTableNumber = 5,
             PasswordsTable = 2,
             TimeShiftTable = 3,
             DepartmetsTable = 7,
             DefaultPrinterPassword = 30,
             NumberOfFeedLines = 5,
             TaxCashRegisterNumberValue = 104,
             MaximumNumberOfTaxCodesSupported = 4,
             MaximumNumberOfPaymentTypesSupported = 4,
             TaxCalculationModePrinterParameter = 14,
             CashPaymentTypeIndex = 1,
             FontType = 1;

        /// <summary>
        /// Class holding the Shtrih-M error codes constants.
        /// </summary>
        public static class ErrorCodes
        {
            public const int Success = 0,
                NotConnected = -1,
                ComPortIsUnavailable = -2,
                ComPortIsBusyWithAnotherApplication = -3,
                NotConnected2 = -4, // No meaningful description in the driver's manual.
                NotConnected3 = -5, // No meaningful description in the driver's manual.
                NotConnected4 = -6, // No Meaningful description in the driver's manual.
                CommandIsNotSupportedInThisMode = 115;
        }
    }

    /// <summary>
    /// Enumeration defining printer internal modes.
    /// </summary>
    internal enum ECRMode
    {
        PrinterIsReady = 0,
        DataOutput = 1,
        ShiftIsOpenLessThan24Hours = 2,
        ShiftIsOpenMoreThan24Hours = 3,
        ShiftIsClosed = 4,
        BlockingDueToIncorrectTaxInspectorPwd = 5,
        DateInputConfirmationWaiting = 6,
        DecimalPointPositionChangePermission = 7,
        OpenDocument = 8,
        TechnicalResetPermission = 9,
        TestRun = 10
    }

    /// <summary>
    /// Enumeration defining printer internal submodes.
    /// </summary>
    /// <remarks>It is used for proper error handling.</remarks>
    internal enum ECRAdvancedMode
    {
        HasPaperNoPrinting = 0,
        PassiveOutOfPaper = 1,
        ActiveOutOfPaper = 2,
        AwaitingContinuePrinting = 3,
        PrintingOperation = 4,
        LongReportPrinting = 5
    }
}