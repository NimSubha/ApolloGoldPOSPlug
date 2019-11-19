/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public partial class RussianFiscalPrinterDriver
    {
        // Constants defining the default predefined value for timeoutStep and timeoutDuration respectively.
        // The unit is miliseconds.
        private const int defaultTimeoutStep = 100;
        private const int defaultTimeoutDuration = 5000;

        private static int? _timeoutStep;
        private static int? _timeoutDuration;
        private static int? _timeoutBeforeExit;

        internal static RussianFiscalPrinterDriver FiscalPrinterDriver
        {
            get
            {
                return (RussianFiscalPrinterDriver) FiscalPrinterDriverFactory.FiscalPrinterDriver;
            }
        }

        /// <summary>
        /// Denotes that an irreversible command was successfuly sent to the printer driver.
        /// </summary>
        /// <remarks>
        /// This property is used to track irreversible commands state.
        /// </remarks>
        public static bool IrreversibleCommandSent
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates that the fiscal printer shift was opened at last logon.
        /// </summary>
        public static bool IsShiftOpenedAtLastLogOn
        {
            get;
            set;
        }

        // These properties are designed to cache the values of timeouts into class static variables as their values are extensively used and reading these values from configuration file is not a 'free' operation.

        /// <summary>
        /// Gets the effective timeout step in milliseconds.
        /// </summary>
        private static int TimeoutStep
        {
            get 
            {
                if (_timeoutStep == null)
                {
                    _timeoutStep = Properties.Settings.Default.TimeoutStep;
                    // If the configured value is invalid, then replace it with our predefined constant value.
                    if (_timeoutStep <= 0)
                    {
                        _timeoutStep = defaultTimeoutStep;
                    }
                }

                return _timeoutStep.Value; 
            }
        }

        /// <summary>
        /// Gets the effective timeout duration in milliseconds.
        /// </summary>
        private static int TimeoutDuration
        {
            get 
            {
                if (_timeoutDuration == null)
                {
                    _timeoutDuration = Properties.Settings.Default.TimeoutDuration;
                    // If the configured value is invalid, then replace it with our predefined constant value.
                    if (_timeoutDuration <= 0)
                    {
                        _timeoutDuration = defaultTimeoutDuration;
                    }
                }

                return _timeoutDuration.Value;
            }
        }

        /// <summary>
        /// Gets the effective timeout before exit in milliseconds.
        /// </summary>
        private static int TimeoutBeforeExit
        {
            get
            {
                if (_timeoutBeforeExit == null)
                {
                    _timeoutBeforeExit = Properties.Settings.Default.TimeoutBeforeExit;
                    // Replace invalid value with zero.
                    if (_timeoutBeforeExit < 0)
                        _timeoutBeforeExit = 0;
                }

                return _timeoutBeforeExit.Value;
            }
        }

        /// <summary>
        /// Defines a delegate to the printer driver method.
        /// </summary>
        /// <returns>
        /// An integer result (error) code determining the successfulness of the method call.
        /// </returns>
        /// <remarks>
        /// All interaction with the printer driver is done through this interface.
        /// Make your printer driver methods fit into the shape defined by this delegate.
        /// This can easily be done using lambda expressions, even if the driver exposes methods that take parameters and do not return a value, but the result code can still be read using some property (or making additional method call).
        /// </remarks>
        protected delegate int PrinterMethod();

        /// <summary>
        /// Confirms retry operation with the user by showing a dialog.
        /// </summary>
        /// <param name="msg">The message to be shown in the dialog.</param>
        /// <param name="throwErrorOnCancel">A Boolean determining whether to throw an error when the user presses Cancel; optional.</param>
        private static void ConfirmRetryOperation(string msg, bool throwErrorOnCancel = true)
        {
            // We need to throw exception in case we are printing slip document without showing this confirmation dialog, as we have another dialog in place where we handle slip document printing exceptions.
            if (RussianFiscalPrinter.Instance.IsPrintingSlipDocument || UserMessages.ShowRetryDialog(msg) == DialogResult.Cancel)
            {
                if (throwErrorOnCancel)
                {
                    FiscalPrinter.ExceptionHelper.ThrowException(Resources.MessageOperationCanceled, msg);
                }
            }
        }

        /// <summary>
        /// Confirms retry operation with the user by showing a dialog.
        /// </summary>
        /// <param name="resourceId">The ID of resource string to show in the dialog.</param>
        /// <param name="throwErrorOnCancel">A Boolean determining whether to throw an error when the user presses Cancel; optional.</param>
        private static void ConfirmRetryOperation(int resourceId, bool throwErrorOnCancel = true)
        {
            var msg = Resources.Translate(resourceId);
            ConfirmRetryOperation(msg, throwErrorOnCancel);
        }

        /// <summary>
        /// Performs printing submodes state transition handling.
        /// </summary>
        /// <param name="executionStatus">An output parameter determining the status of printing operation execution.</param>
        /// <param name="isPostOperation">A Boolean determining whether the method has been called after printing operation submission; false by default.</param>
        /// <param name="operationType">Denotes the type of the operation being called.</param>
        /// <param name="waitWhileBusyPrinting">A Boolean determining whether to produce an additional delay if printer is busy printing; optional, is true by default.</param>
        /// <returns>Current <see cref="PrintingReadinessStatus"/></returns>
        /// <remarks>Handles the printing submodes state transition. executionStatus output parameter is set a meaningful value only when <value>isPostOperation</value> is true.</remarks>
        private static PrintingReadinessStatus CheckPrinterIsReadyForPrinting(out OperationExecutionStatus executionStatus, bool isPostOperation = false, PrinterOperationType operationType = PrinterOperationType.None, bool waitWhileBusyPrinting = true)
        {
            bool retry = false;
            executionStatus = OperationExecutionStatus.None;
            int waitTime = 0;
            PrintingReadinessStatus status;

            do
            {
                switch (status = FiscalPrinterDriver.GetPrintingReadinessStatus())
                {
                    case PrintingReadinessStatus.Ready:
                        if (executionStatus == OperationExecutionStatus.None)
                            executionStatus = OperationExecutionStatus.Okay;
                        retry = false;
                        break;
                    case PrintingReadinessStatus.PassiveOutOfPaper:
                        ConfirmRetryOperation(Resources.MessageWithoutPaper);
                        if (isPostOperation)
                            executionStatus = OperationExecutionStatus.Retry;
                        retry = true;
                        break;
                    case PrintingReadinessStatus.ActiveOutOfPaper:
                        ConfirmRetryOperation(Resources.MessagePrintingStartedButDidNotFinished, !IrreversibleCommandSent);
                        retry = true;
                        break;
                    case PrintingReadinessStatus.AwaitingContinuePrinting:
                        if (operationType != PrinterOperationType.ContinueOperation)
                        {
                            FiscalPrinterDriver.ResumePrinting();
                            if (isPostOperation)
                                executionStatus = (operationType == PrinterOperationType.PrintReport || operationType == PrinterOperationType.CloseReceipt) ? OperationExecutionStatus.Recovered : OperationExecutionStatus.Retry;
                        }
                        retry = false;
                        break;
                    case PrintingReadinessStatus.BusyPrinting:
                        if (operationType == PrinterOperationType.PollPrintingStatus)
                        {
                            retry = false;
                            break;
                        }
                        if (waitWhileBusyPrinting && waitTime < TimeoutDuration)
                        {
                            Thread.Sleep(TimeoutStep);
                            waitTime += TimeoutStep;
                            retry = true;
                            break;
                        }
                        ConfirmRetryOperation(Resources.MessagePrinterNotReady, !IrreversibleCommandSent);
                        retry = true;
                        break;
                }
            } while (retry);

            return status;
        }

        /// <summary>
        /// Wraps a call to any driver method and handles error states and error codes.
        /// </summary>
        /// <param name="method">A delegate to the driver method being called.</param>
        /// <remarks>Performs general exception handling and takes care of the printing submodes state transition graph.</remarks>
        protected static void CheckResultCode(PrinterMethod method)
        {
            CheckResultCode(method, PrinterOperationType.None);
        }

        /// <summary>
        /// Wraps a call to any driver method and handles error states and error codes.
        /// </summary>
        /// <param name="method">A delegate to the driver method being called.</param>
        /// <param name="operationType">Denotes the type of the operation being called.</param>
        /// <remarks>Performs general exception handling and takes care of the printing submodes state transition graph.</remarks>
        protected static void CheckResultCode(PrinterMethod method, PrinterOperationType operationType)
        {
            OperationExecutionStatus executionStatus = OperationExecutionStatus.None;
            bool isPrintingOperation = operationType == PrinterOperationType.PrintingOperation || operationType == PrinterOperationType.PrintReport || operationType == PrinterOperationType.ContinueOperation || operationType == PrinterOperationType.CloseReceipt;

            if (isPrintingOperation)
            {
                CheckPrinterIsReadyForPrinting(out executionStatus, operationType: operationType);
            }

            int result = 0;
            bool timeoutRetry = false;
            bool printerIsNotConnected = false;
            string errorMessage = string.Empty;

            do
            {
                if (method == null)
                {
                    return;
                }

                if (operationType == PrinterOperationType.PrintReport || operationType == PrinterOperationType.CloseReceipt)
                {
                    IrreversibleCommandSent = false;
                }

                result = method();

                if (result == FiscalPrinterDriver.SuccessResultCode)
                {
                    // Set the flag immediately after we call printer command.
                    if (operationType == PrinterOperationType.PrintReport || operationType == PrinterOperationType.CloseReceipt)
                    {
                        IrreversibleCommandSent = true;
                    }

                    // Reset 'Not connected' flag in case we have no errors after printer operation call.
                    FiscalPrinterDriver.IsNotConnected = false;
                    return;
                }

                errorMessage = FiscalPrinterDriver.ResultCodeDescription;

                if (result == FiscalPrinterDriver.ShiftIsOpenMoreThan24HoursErrorCode && FiscalPrinterDriver.IsDayOpenedMoreThan24Hours())
                {
                    FiscalPrinter.ExceptionHelper.ThrowException(Resources.PrinterDayIsOpenMoreThan24Hours);
                }

                printerIsNotConnected = FiscalPrinterDriver.PrinterIsNotConnectedErrorCodes.Contains(result);

                if (isPrintingOperation && !printerIsNotConnected)
                {
                    CheckPrinterIsReadyForPrinting(out executionStatus, true, operationType);
                    // If we have some nonzero error code returned for the previous printing operation and we did not catch any incorrect printing submode,
                    // let it have another try to execute, as this may be related to tiny timeout that needs to take place in order for the command execution to succeed.
                    // timeoutRetry flag is used to control this kind of retry takes place only once.
                    if ((executionStatus == OperationExecutionStatus.None || executionStatus == OperationExecutionStatus.Okay)
                        && operationType == PrinterOperationType.PrintingOperation && !timeoutRetry)
                    {
                        executionStatus = OperationExecutionStatus.Retry;
                        timeoutRetry = true;
                    }
                }
                // Loop until executionStatus is set to Retry for any printing operation except for report printing.
                // Report printing failure is recovered by ContinuePrint operation (in CheckPrinterIsReady method), so there is no need for resubmission of the command.
            } while (executionStatus == OperationExecutionStatus.Retry && operationType != PrinterOperationType.PrintReport);

            // Handle the printer error
            if (result != FiscalPrinterDriver.SuccessResultCode)
            {
                // Throw error for any operation failure exept for printing operation that has been recovered successfully.
                if (!isPrintingOperation || (executionStatus != OperationExecutionStatus.Recovered))
                {
                    // We do not want to show multiple consequent 'Not connected' error messages to the user, so we track FiscalPrinterDriver.IsNotConnected flag.
                    bool promptUser = !printerIsNotConnected || !FiscalPrinterDriver.IsNotConnected;
                    // Update the flag before throwing exception in order to reflect connection status of the last operation.
                    FiscalPrinterDriver.IsNotConnected = printerIsNotConnected;
                    FiscalPrinter.ExceptionHelper.ThrowException(promptUser, Resources.Translate(Resources.FiscalPrinterError, errorMessage));
                }
            }

            FiscalPrinterDriver.IsNotConnected = printerIsNotConnected;
        }

        /// <summary>
        /// Queries the printer for the printing status.
        /// </summary>
        /// <returns>Current printer <see cref="PrintingReadinessStatus"/>.</returns>
        private static PrintingReadinessStatus PollPrintingStatus()
        {
            OperationExecutionStatus executionStatus = OperationExecutionStatus.None;
            return CheckPrinterIsReadyForPrinting(out executionStatus, operationType: PrinterOperationType.PollPrintingStatus);
        }

        /// <summary>
        /// Checks printer printing status and shows dialog if the printer is busy.
        /// </summary>
        private static void CheckPrinterPrintingStatusAndShowDialogIfBusy()
        {
            OperationExecutionStatus executionStatus = OperationExecutionStatus.None;
            CheckPrinterIsReadyForPrinting(out executionStatus, operationType: PrinterOperationType.PrintingOperation, waitWhileBusyPrinting: false);
        }

        /// <summary>
        /// Suspends the current thread until the printer comes into one of the valid states.
        /// </summary>
        /// <param name="exitPredicate">A delegate to the method determining whether the printer has reached one of the valid states.</param>
        /// <param name="timeoutStep">A frequency in milliseconds to query the printer for it's state.</param>
        /// <param name="timeoutDuration">Total duration in milliseconds to wait.</param>
        /// <param name="timeoutBeforeExit">A duration of time in milliseconds to suspend the current thread before exiting.</param>
        /// <param name="exitMethod">A delegate of the method to call before exit.</param>
        /// <param name="timedOutAction">A delegate to the action to call after timed out.</param>
        private static void WaitForThePrinter(Func<bool> exitPredicate, int timeoutStep, int timeoutDuration, int timeoutBeforeExit, PrinterMethod exitMethod = null, Action timedOutAction = null)
        {
            for (int waitTime = 0; waitTime < timeoutDuration; waitTime += timeoutStep)
            {
                if (exitPredicate())
                {
                    if (timeoutBeforeExit != 0)
                        Thread.Sleep(timeoutBeforeExit);
                    if (exitMethod != null)
                        CheckResultCode(exitMethod, PrinterOperationType.PrintingOperation);
                    return;
                }
                Thread.Sleep(timeoutStep);
            }
            if (timedOutAction != null)
                timedOutAction();
        }

        /// <summary>
        /// Suspends the current thread until the printer reaches one of the valid <see cref="PrintingReadinessStatus">statuses</see>.
        /// </summary>
        /// <param name="printingReadinessStatuses">A set of valid <see cref="PrintingReadinessStatus">statuses</see>.</param>
        /// <param name="timeoutStep">A frequency in milliseconds to query the printer for it's state.</param>
        /// <param name="timeoutDuration">Total duration in milliseconds to wait.</param>
        /// <param name="timeoutBeforeExit">A duration of time in milliseconds to suspend the current thread before exiting.</param>
        private static void WaitForCompletionOfPrinting(ISet<PrintingReadinessStatus> printingReadinessStatuses, int timeoutStep, int timeoutDuration, int timeoutBeforeExit)
        {
            WaitForThePrinter(() => printingReadinessStatuses.Contains(PollPrintingStatus()), timeoutStep, timeoutDuration, timeoutBeforeExit, null, CheckPrinterPrintingStatusAndShowDialogIfBusy);
        }

        /// <summary>
        /// Suspends the current thread until the printer reaches a Ready for printing state.
        /// </summary>
        /// <param name="timeoutStep">A frequency in milliseconds to query the printer for it's state.</param>
        /// <param name="timeoutDuration">Total duration in milliseconds to wait.</param>
        /// <param name="timeoutBeforeExit">A duration of time in milliseconds to suspend the current thread before exiting.</param>
        private static void WaitForCompletionOfPrinting(int timeoutStep, int timeoutDuration, int timeoutBeforeExit)
        {
            WaitForCompletionOfPrinting(new HashSet<PrintingReadinessStatus> { PrintingReadinessStatus.Ready }, timeoutStep, timeoutDuration, timeoutBeforeExit);
        }

        /// <summary>
        /// Suspends the current thread until the printer reaches a Ready for printing state.
        /// </summary>
        /// <remarks>Utilizes timeout settings defined in the FiscalPrinter configuration file.</remarks>
        private static void WaitForCompletionOfPrinting()
        {
            WaitForCompletionOfPrinting(TimeoutStep, TimeoutDuration, TimeoutBeforeExit);
        }

        /// <summary>
        /// Submits the printing command to the printer, performs the necessary printing errors handling and validates the successful completion of printing.
        /// </summary>
        /// <param name="printerMethod">A delegate to the driver method being called.</param>
        /// <param name="operationType">Denotes the type of the operation being called.</param>
        /// <param name="waitForCompletionOfPrinting">A Boolean determining whether to wait for successful completion of the printing operation.</param>
        /// <param name="isLastPrintingOperation">A Boolean determining whether the current operation is the last printing operation in a sequence.</param>
        /// <param name="startNewOperationSequence">A Boolean determining whether the operation starts a new execution sequence.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Design")]
        protected static void ExecutePrintingOperation(PrinterMethod printerMethod, PrinterOperationType operationType = PrinterOperationType.None, bool waitForCompletionOfPrinting = true, bool isLastPrintingOperation = false, bool startNewOperationSequence = true)
        {
            if (operationType == PrinterOperationType.PrintReport || operationType == PrinterOperationType.CloseReceipt)
            {
                IrreversibleCommandSent = false;
            }

            if (startNewOperationSequence)
            {
                FiscalPrinterDriver.ResetControlsVariables();
            }

            CheckResultCode(printerMethod, operationType);

            if (waitForCompletionOfPrinting)
            {
                WaitForCompletionOfPrinting();
            }

            if (isLastPrintingOperation)
            {
                FiscalPrinterDriver.RibbonFeed();
                FiscalPrinterDriver.ExecutePaperCut(false);
            }
        }
    }
}
