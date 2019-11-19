/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    /// <summary>
    /// Enumeration determining status of the execution of the operation.
    /// </summary>
    /// <remarks>Used to track the state of printing operation after the command has been submitted to the printer driver.</remarks>
    public enum OperationExecutionStatus
    {
        /// <summary>
        /// Uninitialized value of the status.
        /// </summary>
        /// <remarks>Denotes that the status has not been determined.</remarks>
        None,
        /// <summary>
        /// The operation should be resubmitted to the printer driver.
        /// </summary>
        Retry,
        /// <summary>
        /// The printer has successfully recovered after an operation failure, no need to resubmit the operation to the printer.
        /// </summary>
        Recovered,
        /// <summary>
        /// Operation has been successfully executed by the printer.
        /// </summary>
        /// <remarks>This status is treated the same way as <value>None</value> for now.</remarks>
        Okay
    }

    /// <summary>
    /// Enumeration distinguishing between different Fiscal printer operation types.
    /// </summary>
    public enum PrinterOperationType
    {
        /// <summary>
        /// Unspecified operation type, i.e. 'default' type.
        /// </summary>
        None,
        /// <summary>
        /// Regular printing operation.
        /// </summary>
        PrintingOperation,
        /// <summary>
        /// ContinuePrint operation.
        /// </summary>
        /// <remarks>Plays a special role, as we use this operation to recover from AwaitingContinuePrinting printing state.</remarks>
        ContinueOperation,
        /// <summary>
        /// Printing Z or X report operation.
        /// </summary>
        PrintReport,
        /// <summary>
        /// Query for printing status.
        /// </summary>
        /// <remarks>We treat this operation differently when in CheckPrinterIsReady method.</remarks>
        PollPrintingStatus,
        /// <summary>
        /// Close receipt command.
        /// </summary>
        /// <remarks>
        /// This command is treated as irreversible: once it's sent to the printer, it can not be reversed.
        /// </remarks>
        CloseReceipt
    }

    /// <summary>
    /// Enumeration of the statuses reflecting the fiscal printer readiness for printing.
    /// </summary>
    public enum PrintingReadinessStatus
    {
        Ready,
        PassiveOutOfPaper,
        ActiveOutOfPaper,
        AwaitingContinuePrinting,
        BusyPrinting
    }

    /// <summary>
    /// Enumeration determining receipt type.
    /// </summary>
    public enum CheckType
    {
        Sales = 0,
        Buy = 1,
        ReturnSales = 2,
        ReturnBuy = 3
    }
}
