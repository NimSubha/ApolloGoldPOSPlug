/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSControls;
using LSRetailPosis.POSControls.Touch;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    [Export(typeof(IEOD))]
    public class EOD : IEOD
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for the EOD service are reserved at 51300 - 52299
        // In use now are ID's: 51315

        #region Fields

        private const int AllowMultipleShiftLogOnPermission = 1019;

         #endregion

        #region Properties

        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        #endregion

        #region IEOD Members

        /// <summary>
        /// Open a shift.
        /// </summary>
        /// <param name="shift">Opened shift reference. null if shift was not opened (Non-drawer mode).</param>
        /// <returns>True if user selected any drawer or non-drawer mode, false if user canceled the operation</returns>
        public bool OpenShift(ref IPosBatchStaging shift)
        {
            string operatorId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;

            // If already opened shift in memory belongs to logged on user (or assigned to user), then just return back.
            if (shift != null && (operatorId.Equals(shift.StaffId, StringComparison.OrdinalIgnoreCase) || ShiftUsersCache.Contains(shift, operatorId)))
            {
                return true; // return without any interaction.
            }

            BatchData batchData = new BatchData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            IList<IPosBatchStaging> openedShifts = batchData.GetOpenedPosBatchesForTerminal(ApplicationSettings.Terminal.TerminalId);
            int maxSupporedOpenedShifts = Math.Max(1, GetAvailableDrawers().Count()); // Minimum 1 shift is supported even no cash drawer available
            int currentOpenedShifts = openedShifts != null ? openedShifts.Count : 0;
            bool canOpenMoreShifts = (currentOpenedShifts < maxSupporedOpenedShifts);
            bool allowMultipleShiftLogons = false; // Allow access to the shifts from other users.
            bool allowMultipleLogons = false; // Allow multiple logons (hence multiple open shifts)
            ShiftActionResult shiftActionResult = ShiftActionResult.None;
            IList<IPosBatchStaging> suspendedShifts = null;

            // Try finding opened batch from database.
            if (currentOpenedShifts > 0
                && (shift = openedShifts.FirstOrDefault(s => operatorId.Equals(s.StaffId, StringComparison.OrdinalIgnoreCase))) != null)
            {
                return true; // Open shift found, return without any interaction.
            }

            GetPermissions(ref allowMultipleLogons, ref allowMultipleShiftLogons);

            if (canOpenMoreShifts) // Have vacant drawers on current terminal
            {
                IPosBatchStaging shiftOnAnotherTerminal = batchData.GetPosBatchesWithStatus(PosBatchStatus.Open, operatorId).FirstOrDefault();

                if ((!allowMultipleLogons) && (shiftOnAnotherTerminal != null))
                {
                    // User is not allowed for multiple logons and
                    // owns a open shift on another terminal. Ask for action (Non-Drawer or cancel)
                    shiftActionResult = ShowShiftActionForm(51305, false, false, false, shiftOnAnotherTerminal.OpenedAtTerminal);
                }
                else
                {
                    suspendedShifts = batchData.GetPosBatchesWithStatus(PosBatchStatus.Suspended, allowMultipleShiftLogons ? null : operatorId);

                    // If there are suspended shifts?
                    if ((suspendedShifts.Count > 0) && (!Application.Settings.Database.IsOffline))
                    {
                        // And user with multiple shift permission is logging in then prompt for action.
                        if (allowMultipleShiftLogons)
                        {
                            shiftActionResult = ShowShiftActionForm(51306, true, true, false);
                        }
                        else
                        {
                            // and user without multiple shift permssions is logging in
                            shiftActionResult = ShowShiftActionForm(51307, false, true, false);
                        }
                    }
                    else
                    {
                        // A shift is not currently open on this register.
                        shiftActionResult = ShowShiftActionForm(51306, true, false, false);
                    }
                }
            }
            else // No more shifts can be opened on this register
            { 
                if (allowMultipleShiftLogons)
                {
                    // User allowed for multiple shifts (Use existing, Non-Drawer or cancel)
                    shiftActionResult = ShowShiftActionForm(51309, false, false, true);
                }
                else
                {
                    // Ask for action (Non-Drawer or cancel)
                    shiftActionResult = ShowShiftActionForm(51304, false, false, false);
                }
            }

            return ProcessShiftAction(shiftActionResult, openedShifts, suspendedShifts, ref shift);
        }

        /// <summary>
        /// Closes the current shift and print it as Z-Report.
        /// </summary>
        /// <param name="transaction">The current transaction instance.</param>
	    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        public void CloseShift(IPosTransaction transaction)
        {
            if (transaction == null)
            {
                NetTracer.Warning("transaction parameter is null");
                throw new ArgumentNullException("transaction");
            }

            Batch batch = null;

            // Are you sure you want to close the shift ?
            if (this.Application.Services.Dialog.ShowMessage(51302, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                batch = new Batch(transaction.Shift);

                // Verify if all offline transacitons has been uploaded.
                if (!batch.VerifyOfflineTransactions())
                {
                    batch = null;
                    this.Application.Services.Dialog.ShowMessage(51341);
                }
            }

            // Calculate and verify amounts.
            if (batch != null)
            {
                // Calculate batch in background
                POSFormsManager.ShowPOSMessageWithBackgroundWorker(51303, delegate { batch.Calculate(); });

                Action<decimal, int, int> verifyAmount = delegate(decimal amount, int errorMsg, int warningMsg)
                {
                    if (amount == 0)
                    {
                        // Warning or error based on configration in HQ.
                        if ((Functions.RequireAmountDeclaration
                            && this.Application.Services.Dialog.ShowMessage(errorMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            || (this.Application.Services.Dialog.ShowMessage(warningMsg, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No))
                        {
                            batch = null;
                        }
                    }
                };

                // Verify starting amounts.
                if (batch != null)
                    verifyAmount(batch.StartingAmountTotal, 51344, 51343);

                // Verify tender delcartion.
                if (batch != null)
                    verifyAmount(batch.DeclareTenderAmountTotal, 51346, 51345);
            }

            if (batch != null &&
                EOD.InternalApplication.Services.EFTServiceOperations.IsSupported())
            {
                bool runBankTotalsVerification = Functions.EODBankTotalsVerification;

                if (!runBankTotalsVerification)
                {
                    // Dialog is shown: Do you want to generate bank total verification?
                    runBankTotalsVerification = this.Application.Services.Dialog.ShowMessage(105011, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
                }

                if (runBankTotalsVerification)
                {
                    try
                    {
                        EOD.InternalApplication.Services.EFTServiceOperations.RunTotalsVerification(transaction);
                    }
                    catch (EFTServiceOperationException)
                    {
                        // Bank total verification has not been performed. Do you want to close this shift anyway?
                        if (this.Application.Services.Dialog.ShowMessage(105010, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No)
                        {
                            batch = null;
                        }
                    }
                }
            }

            if (batch != null &&
                EOD.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled() &&
                Functions.CountryRegion == SupportedCountryRegion.RU)
            {
                try
                {
                    //print z report
                    EOD.InternalApplication.Services.Peripherals.FiscalPrinter.PrintZReport(transaction);
                }
                catch
                {
                    if (!EOD.InternalApplication.Services.Peripherals.FiscalPrinter.IsPrintingCommandSent)
                    {
                        batch = null;
                    }
                }
            }

            // Close the batch and Print Z report if everything is ok.
            if (batch != null)
            {
                batch.Status = PosBatchStatus.Closed;
                batch.CloseDateTime = DateTime.Now;
                batch.ClosedAtTerminal = ApplicationSettings.Terminal.TerminalId;

                BatchData batchData = new BatchData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
                batchData.CloseBatch(batch);
                transaction.Shift.Status = PosBatchStatus.Closed;
                ShiftUsersCache.Remove(transaction.Shift);

                // Check the Print X/Z report on POS function whether is available. 
                if (Functions.PrintXZreportOnPos)
                {
                    // Print Z report if user has permissions.
                    IUserAccessSystem userAccessSystem = Application.BusinessLogic.UserAccessSystem;

                    if (userAccessSystem.UserHasAccess(ApplicationSettings.Terminal.TerminalOperator.OperatorId, PosisOperations.PrintZ))
                    {
                        POSFormsManager.ShowPOSMessageWithBackgroundWorker(99, delegate { batch.Print(ReportType.ZReport); });
                    }
                }
            }
            else
            {
                NetTracer.Information("Setting status of the transaction to 'cancelled'");
                ((PosTransaction)transaction).EntryStatus = PosTransaction.TransactionStatus.Cancelled;
            }
        }

        /// <summary>
        /// Suspend the current batch.
        /// </summary>
        /// <param name="transaction">The current transaction instance.</param>
        public void SuspendShift(IPosTransaction transaction)
        {
            if (transaction == null)
            {
                NetTracer.Warning("transaction parameter is null");
                throw new ArgumentNullException("transaction");
            }

            BatchData batchData = new BatchData(Application.Settings.Database.Connection,
                Application.Settings.Database.DataAreaID);

            transaction.Shift.OpenedAtTerminal = string.Empty;
            transaction.Shift.CashDrawer = string.Empty;
            transaction.Shift.Status = PosBatchStatus.Suspended;
            transaction.Shift.StatusDateTime = DateTime.Now;

            batchData.UpdateBatch(transaction.Shift);
            ShiftUsersCache.Remove(transaction.Shift);
            transaction.Shift.Print();
        }

        /// <summary>
        /// Blind closes the current shift.
        /// </summary>
        /// <param name="transaction">The current transaction instance.</param>
        public void BlindCloseShift(IPosTransaction transaction)
        {
            if (transaction == null)
            {
                NetTracer.Warning("transaction parameter is null");
                throw new ArgumentNullException("transaction");
            }

            Batch batch = null;

            // Are you sure you want to close the batch?
            if (this.Application.Services.Dialog.ShowMessage(51308, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                batch = new Batch(transaction.Shift);
            }

            if (batch != null)
            {
                POSFormsManager.ShowPOSMessageWithBackgroundWorker(51303, delegate { batch.Calculate(); });

                Action<decimal, int, int> verifyAmount = delegate(decimal amount, int errorMsg, int warningMsg)
                {
                    if (amount == 0)
                    {
                        // Warning or error based on configuration in HQ.
                        if ((Functions.RequireAmountDeclaration
                            && this.Application.Services.Dialog.ShowMessage(errorMsg, MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                            || (this.Application.Services.Dialog.ShowMessage(warningMsg, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.No))
                        {
                            batch = null;
                        }
                    }
                };

                // Verify starting amounts.
                if (batch != null)
                    verifyAmount(batch.StartingAmountTotal, 51344, 51343);
            }

            if (batch != null)
            {
                BatchData batchData = new BatchData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);

                transaction.Shift.Status = PosBatchStatus.BlindClosed;
                transaction.Shift.StatusDateTime = DateTime.Now;
                transaction.Shift.OpenedAtTerminal = string.Empty;
                transaction.Shift.CashDrawer = string.Empty;

                batchData.UpdateBatch(transaction.Shift);
                ShiftUsersCache.Remove(transaction.Shift);
                transaction.Shift.Print();
            }
            else
            {
                ((PosTransaction)transaction).EntryStatus = PosTransaction.TransactionStatus.Cancelled;
            }
        }

        /// <summary>
        /// Show blind closed shifts.
        /// </summary>
        /// <param name="transaction">The current transaction instance.</param>
        public void ShowBlindClosedShifts(IPosTransaction transaction)
        {
            if (transaction == null)
            {
                NetTracer.Warning("transaction parameter is null");
                throw new ArgumentNullException("transaction");
            }

            BatchData batchData = new BatchData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            string operatorId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
            bool multipleShiftLogOn = Application.BusinessLogic.UserAccessSystem.UserHasPermission(operatorId, AllowMultipleShiftLogOnPermission);
            IList<IPosBatchStaging> blindClosedShifts = batchData.GetPosBatchesWithStatus(PosBatchStatus.BlindClosed, multipleShiftLogOn ? null : operatorId);

            using (BlindClosedShiftsForm blindClosedShiftsForm = new BlindClosedShiftsForm(blindClosedShifts))
            {
                POSFormsManager.ShowPOSForm(blindClosedShiftsForm);
            }
        }

        /// <summary>
        /// Print Report for currently opend batch (X-Report)
        /// </summary>
        /// <param name="transaction"></param>
        public void PrintXReport(IPosTransaction transaction)
        {
            ApplicationLog.Log("EOD.PrintXReport", "Printing X report.", LogTraceLevel.Trace);

            if (EOD.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                EOD.InternalApplication.Services.Peripherals.FiscalPrinter.PrintXReport(transaction);
                return;
            }

            if (transaction == null)
            {
                NetTracer.Warning("transaction parameter is null");
                throw new ArgumentNullException("transaction");
            }

            Batch batch = new Batch(transaction.Shift);

            POSFormsManager.ShowPOSMessageWithBackgroundWorker(51303, delegate { batch.Calculate(); }); // Calculating transactions...
            POSFormsManager.ShowPOSMessageWithBackgroundWorker(99, delegate { batch.Print(ReportType.XReport); }); // Printing in progress...

        }

        /// <summary>
        /// Print recently closed batch report (Z-Report)
        /// </summary>
        /// <param name="transaction"></param>
        public void PrintZReport(IPosTransaction transaction)
        {
            ApplicationLog.Log("EOD.PrintZReport", "Printing Z report.", LogTraceLevel.Trace);

            if (EOD.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                EOD.InternalApplication.Services.Peripherals.FiscalPrinter.PrintZReport(transaction);
                return;
            }

            BatchData batchData = new BatchData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            Batch batch = batchData.ReadRecentlyClosedBatch(ApplicationSettings.Terminal.TerminalId);

            if (batch != null)
            {
                // Print batch.
                POSFormsManager.ShowPOSMessageWithBackgroundWorker(99, delegate { batch.Print(ReportType.ZReport); });
            }
            else
            {
                NetTracer.Information("EDO::PrintZReport: batch is null");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the permissions for the current logged on user.
        /// </summary>
        /// <param name="allowMultipleLogons"></param>
        /// <param name="allowMultipleShiftLogons"></param>
        private static void GetPermissions(ref bool allowMultipleLogons, ref bool allowMultipleShiftLogons)
        {
            const int CHECKED = 1;
            LogonData logonData = new LogonData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            PosPermission userPermission = logonData.GetUserPosPermission(ApplicationSettings.Terminal.StoreId, ApplicationSettings.Terminal.TerminalOperator.OperatorId);

            if (userPermission != null)
            {
                if ((userPermission.ManagerPrivileges == CHECKED)
                    || (userPermission.AllowMultipleLogins == CHECKED))
                {
                    allowMultipleLogons = true;
                }

                if ((userPermission.ManagerPrivileges == CHECKED)
                    || (userPermission.AllowMultipleShiftLogOn == CHECKED))
                {
                    allowMultipleShiftLogons = true;
                }
            }
        }

        /// <summary>
        /// Show the shift action form
        /// </summary>
        /// <param name="message">Message string id</param>
        /// <param name="newButton">Show new button</param>
        /// <param name="resumeButton">Show resume button</param>
        /// <param name="useButton">Show use button</param>
        /// <param name="args">Arguments for message string</param>
        /// <returns></returns>
        private static ShiftActionResult ShowShiftActionForm(int message, bool newButton, bool resumeButton, bool useButton, params object[] args)
        {
            using (ShiftActionForm shiftActionForm = new ShiftActionForm(ApplicationLocalizer.Language.Translate(message, args), newButton, resumeButton, useButton))
            {
                // Form is shown on top of Login form, so should not use POSFormsManager
                shiftActionForm.ShowDialog();
                return shiftActionForm.FormResult;
            }
        }

        /// <summary>
        /// Shows the shift selection form.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="shifts">The shifts.</param>
        /// <returns></returns>
        private static IPosBatchStaging ShowShiftSelectionForm(ShiftSelectionMode mode, IList<IPosBatchStaging> shifts)
        {
            IPosBatchStaging shift = null;

            using (ResumeShiftForm dialog = new ResumeShiftForm(mode, shifts))
            {
                // Form is shown on top of Login form, so should not use POSFormsManager
                dialog.ShowDialog();

                if (dialog.DialogResult == DialogResult.OK)
                {
                    shift = dialog.SelectedShift;
                }
            }

            return shift;
        }

        /// <summary>
        /// Get the cash drawer for shift if available.
        /// </summary>
        /// <param name="openedShifts">The opened shifts.</param>
        /// <param name="cashDrawerName">Name of the cash drawer.</param>
        /// <returns>
        /// True if cash drawer selected or available, false other.
        /// </returns>
        private static bool TryGetCashDrawerForShift(IList<IPosBatchStaging> openedShifts, ref string cashDrawerName)
        {
            bool result = true;
            ICollection<Tuple<string, string>> cashDrawers = GetAvailableDrawers();

            // IF more than one drawer available, then only show the cash drawer selection UI
            if (cashDrawers.Count > 1)
            {
                using (CashDrawerSelectionForm drawerSelectionForm = new CashDrawerSelectionForm(openedShifts))
                {
                    // Form is shown on top of Login form, so should not use POSFormsManager
                    drawerSelectionForm.ShowDialog();

                    if (drawerSelectionForm.DialogResult == DialogResult.OK)
                    {
                        cashDrawerName = drawerSelectionForm.SelectedCashDrawer;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            else // Otherwise, get the first one if available.
            {
                Tuple<string, string> drawerInfo = cashDrawers.FirstOrDefault();

                if (drawerInfo != null)
                    cashDrawerName = drawerInfo.Item1; // Drawer name
            }

            return result;
        }

        /// <summary>
        /// Gets the available drawers if CashDrawer V2 service is loaded.
        /// </summary>
        /// <returns>Cash drawer collection.</returns>
        private static ICollection<Tuple<string, string>> GetAvailableDrawers()
        {
            ICollection<Tuple<string, string>> cashDrawers = new Collection<Tuple<string, string>>();

            // If CashDrawer V2 available then only get available drawers.
            if (((object)EOD.InternalApplication.Services.Peripherals.CashDrawer) is ICashDrawerV2)
            {
                cashDrawers = EOD.InternalApplication.Services.Peripherals.CashDrawer.GetAvailableDrawers();
            }

            return cashDrawers;
        }

        /// <summary>
        /// Processes the shift action.
        /// </summary>
        /// <param name="shiftActionResult">The shift action result.</param>
        /// <param name="openedShifts">The opened shifts.</param>
        /// <param name="suspendedShifts">The suspended shifts.</param>
        /// <param name="shift">The shift.</param>
        /// <returns>
        /// True if processed, false if canceled.
        /// </returns>
        private bool ProcessShiftAction(ShiftActionResult shiftActionResult, IList<IPosBatchStaging> openedShifts, IList<IPosBatchStaging> suspendedShifts, ref IPosBatchStaging shift)
        {
            BatchData batchData = new BatchData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            bool result = false;
            string cashDrawer = null;

            switch (shiftActionResult)
            {
                case ShiftActionResult.New:

                    if (TryGetCashDrawerForShift(openedShifts, ref cashDrawer))
                    {
                        PosBatchStaging newPosBatch = new PosBatchStaging();

                        newPosBatch.StoreId = ApplicationSettings.Terminal.StoreId;
                        newPosBatch.TerminalId = newPosBatch.OpenedAtTerminal = ApplicationSettings.Terminal.TerminalId;
                        newPosBatch.CashDrawer = cashDrawer;
                        newPosBatch.StaffId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                        newPosBatch.StartDateTime = newPosBatch.StatusDateTime = DateTime.Now;
                        newPosBatch.Status = PosBatchStatus.Open;
                        if (!ApplicationSettings.Terminal.TrainingMode) // Don't create shift in traning mode.
                        {
                            newPosBatch.BatchId = Application.Services.ApplicationService.GetAndIncrementTerminalSeed(NumberSequenceSeedType.BatchId);
                            batchData.CreateBatch(newPosBatch);
                        }
                        shift = newPosBatch;

                        result = true;
                    }
                    break;

                case ShiftActionResult.Resume:
                    // Let user select the shift to resume.
                    shift = ShowShiftSelectionForm(ShiftSelectionMode.Resume, suspendedShifts);

                    if (shift != null && TryGetCashDrawerForShift(openedShifts, ref cashDrawer))
                    {
                        shift.Status = PosBatchStatus.Open;
                        shift.StatusDateTime = DateTime.Now;
                        shift.OpenedAtTerminal = ApplicationSettings.Terminal.TerminalId;
                        shift.CashDrawer = cashDrawer;
                        if (!ApplicationSettings.Terminal.TrainingMode) // Don't update batch in traning mode.
                        {
                            batchData.UpdateBatch(shift);
                        }

                        result = true;
                    }
                    break;

                case ShiftActionResult.UseOpened:

                    if (openedShifts.Count == 1)
                    {
                        shift = openedShifts.First();
                    }
                    else
                    {
                        // Let user select the opened shift to use.
                        shift = ShowShiftSelectionForm(ShiftSelectionMode.UseOpened, openedShifts);
                    }

                    if (shift != null)
                    {
                        ShiftUsersCache.Add(shift, ApplicationSettings.Terminal.TerminalOperator.OperatorId);
                        result = true;
                    }
                    break;

                case ShiftActionResult.NonDrawer:
                    shift = null;
                    result = true;
                    break;
            }

            return result;
        }

        #endregion

    }

    /// <summary>
    /// Enum to specifies the report type.
    /// </summary>
    internal enum ReportType
    {
        /// <summary>
        /// X-Report
        /// </summary>
        XReport = 1,

        /// <summary>
        /// Z-Report
        /// </summary>
        ZReport = 2
    }
}
