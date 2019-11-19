/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using frmMessage = LSRetailPosis.POSProcesses.frmMessage;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    /// <summary>
    /// Provides auxiliary methods to show and deal with User Messages.
    /// </summary>
    public static class UserMessages
    {
        public const int DefaultTimeout = 10;

        /// <summary>
        /// Updates the user panel in the main POS window
        /// </summary>
        /// <param name="resourceId">The message resource ID.</param>
        /// <param name="args">The arguments for the message placeholders.</param>
        public static void UpdateUserPanel(int resourceId, params object[] args)
        {
            UpdateUserPanel(Resources.Translate(resourceId, args));
        }

        /// <summary>
        /// Updates the user panel in the main POS window
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static void UpdateUserPanel(string message)
        {
            POSFormsManager.ShowPOSStatusPanelText(message);
        }

        /// <summary>
        /// Shows main POS window if it is not Null and handle created, otherwise invokes posForm.ShowDialog()
        /// </summary>
        /// <param name="posForm">POS form</param>
        private static void ShowPosForm(Form posForm)
        {
            var posMainWindow = LSRetailPosis.ApplicationFramework.POSMainWindow as Control;

            if ((posMainWindow != null) && posMainWindow.IsHandleCreated)
            {
                LSRetailPosis.POSControls.POSFormsManager.ShowPOSForm(posForm);
            }
            else
            {
                posForm.ShowDialog();
            }
        }

        /// <summary>
        /// Ask for user confirmation before printing Z Report
        /// </summary>
        /// <returns>TRUE if user selects YES, FALSE otherwise</returns>
        public static bool ContinueWithPrintZReport()
        {
            return (ShowQuestionDialog(Resources.Translate(Resources.MessageConfirmZReport)) == DialogResult.Yes);
        }

        /// <summary>
        /// Request a number using the numeric pad
        /// </summary>
        /// <param name="resourceId">The message resource ID.</param>
        public static int RequestNumber(int resourceId)
        {
            using (var form = new frmInputNumpad())
            {
                form.EntryTypes = NumpadEntryTypes.IntegerPositive;
                form.PromptText = Resources.Translate(resourceId);

                return form.ShowDialog() == DialogResult.OK ? int.Parse(form.InputText) : -1;
            }
        }

        /// <summary>
        /// Request a date using the Date picker
        /// </summary>
        /// <param name="resourceId">The message resource ID.</param>
        public static DateTime RequestDate(int resourceId)
        {
            using (var form = new frmDatePicker())
            {
                form.Text = Resources.Translate(resourceId);
                return form.ShowDialog() == DialogResult.OK ? form.SelectedDate : DateTime.MinValue;
            }
        }

        /// <summary>
        /// Display an error message and asks for Retry
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static DialogResult ShowRetryDialog(string message)
        {
            DialogResult result;
            using (var form = new frmMessage(message, MessageBoxButtons.RetryCancel, MessageBoxIcon.Question))
            {
                result = form.ShowDialog();
            }

            return result;
        }

        /// <summary>
        /// Display a message with Yes or No buttons.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static DialogResult ShowQuestionDialog(string message)
        {
            DialogResult result;
            using (var form = new frmMessage(message, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                ShowPosForm(form);
                result = form.DialogResult;
            }

            return result;
        }

        /// <summary>
        /// Display a message with Yes or No buttons.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static DialogResult ShowQuestionDialogWithTimeout(string message)
        {
            return ShowQuestionDialogWithTimeout(message, DefaultTimeout);
        }

        /// <summary>
        /// Display a message with Yes or No buttons.
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="waitingSeconds">The countdown timer in seconds.</param>
        public static DialogResult ShowQuestionDialogWithTimeout(string message, int waitingSeconds)
        {
            DialogResult result;
            using (var form = new frmMessage(message, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            {
                form.SecondsToCloseForm = waitingSeconds;
                form.TimeoutButton = DialogResult.No;
                ShowPosForm(form);
                result = form.DialogResult;
            }

            return result;
        }

        /// <summary>
        /// Show a message in the status panel in the main POS window
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static void ShowMessageInStatusPanel(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            POSFormsManager.ShowPOSStatusPanelText(message);
            POSFormsManager.CalcSizes();
            Application.DoEvents();
        }

        /// <summary>
        /// Display a window with a exception error message 
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static void ShowException(string message)
        {
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error))
            {
                ShowPosForm(form);
            }
        }

        /// <summary>
        /// Handles the <see cref="FiscalPrinterException"/>.
        /// </summary>
        /// <param name="ex">The exception that is handled.</param>
        public static void HandleException(FiscalPrinterException ex)
        {
            if (ex != null && !ex.MessageWasShownToTheUser && !string.IsNullOrEmpty(ex.Message))
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// Display an error message with OK button
        /// </summary>
        /// <param name="resourceId">The message resource ID.</param>
        /// <param name="args">The arguments for the message placeholders.</param>
        public static void ShowMessage(int resourceId, params object[] args)
        {
            ShowMessage(Resources.Translate(resourceId, args));
        }

        /// <summary>
        /// Display an error message with OK button
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static void ShowMessage(string message)
        {
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error))
            {
                ShowPosForm(form);
            }
        }

        /// <summary>
        /// Display an information with OK button
        /// </summary>
        /// <param name="resourceId">The message resource ID.</param>
        /// <param name="args">The arguments for the message placeholders.</param>
        public static void ShowInformation(int resourceId, params object[] args)
        {
            var message = Resources.Translate(resourceId, args);
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                ShowPosForm(form);
            }
        }

        /// <summary>
        /// Display an information with OK button
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static void ShowInformation(string message)
        {
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                ShowPosForm(form);
            }
        }

        /// <summary>
        /// Display an warning with OK button
        /// </summary>
        /// <param name="resourceId">The message resource ID.</param>
        /// <param name="args">The arguments for the message placeholders.</param>
        public static void ShowWarning(int resourceId, params object[] args)
        {
            var message = Resources.Translate(resourceId, args);
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Warning))
            {
                ShowPosForm(form);
            }
        }

        /// <summary>
        /// Display an warning with OK button
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        public static void ShowWarning(string message)
        {
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Warning))
            {
                ShowPosForm(form);
            }
        }

        /// <summary>
        /// Display an information with OK button and timeout
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <returns>True if the user clicked in the OK button; false, otherwise.</returns>
        public static void ShowInformationTimeout(string message)
        {
            ShowInformationTimeout(message, DefaultTimeout);
        }

        /// <summary>
        /// Display an information with OK button and timeout
        /// </summary>
        /// <param name="message">The message to be shown.</param>
        /// <param name="waitingSeconds">The countdown timer in seconds.</param>
        /// <returns>True if the user clicked in the OK button; false, otherwise.</returns>
        public static void ShowInformationTimeout(string message, int waitingSeconds)
        {
            using (var form = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
            {
                form.SecondsToCloseForm = waitingSeconds;
                form.ShowDialog();
            }
        }

        /// <summary>
        /// Asks for user confirmation to generate some report within a range.
        /// </summary>
        /// <param name="startRange">The start value of the range.</param>
        /// <param name="endRange">The end value of the range.</param>
        /// <returns>True if the user accepts; false, otherwise.</returns>
        public static bool ContinueTheGeneration(object startRange, object endRange)
        {
            var message = Resources.Translate(Resources.FiscalMenuContinueTheGeneration,
                                              startRange,
                                              endRange
                                              );
            return ShowQuestionDialog(message) == DialogResult.Yes;
        }

        /// <summary>
        /// Asks for user confirmation to generate a file within a range.
        /// </summary>
        /// <param name="fileType">The type of file.</param>
        /// <param name="startRange">The start value of the range.</param>
        /// <param name="endRange">The end value of the range.</param>
        /// <returns>True if the user accepts; false, otherwise.</returns>
        public static bool ContinueFileGeneration(string fileType, string startRange, string endRange)
        {
            var message = Resources.Translate(Resources.MessageConfirmFileGeneration, 
                                              fileType, 
                                              startRange, 
                                              endRange
                                              );

            return ShowQuestionDialog(message) == DialogResult.Yes;
        }

        /// <summary>
        /// Shows an exception message about a invalid value.
        /// </summary>
        /// <param name="invalidValue">The actual wrong value.</param>
        /// <param name="correctValue">The expected correct value.</param>
        public static void ShowInvalidValueMessage(object invalidValue, object correctValue)
        {
            var message = Resources.Translate(Resources.FiscalMenuInvalidValue , invalidValue);
            message += ("\n" + Resources.Translate(Resources.FiscalMenuEnterValueGreaterThan , correctValue));
            ShowException(message);
        }

        /// <summary>
        /// Shows an exception message about a invalid value.
        /// </summary>
        /// <param name="invalidValue">The actual wrong value.</param>
        /// <param name="minorCorrectValue">The lower bound of the acceptable range.</param>
        /// <param name="greaterCorrectValue">The upper bound of the acceptable range.</param>
        public static void ShowInvalidValueMessage(object invalidValue, object minorCorrectValue, object greaterCorrectValue)
        {
            var message = Resources.Translate(Resources.FiscalMenuInvalidValue, invalidValue);
            message += ("\n" + Resources.Translate(Resources.FiscalMenuEnterValueBetween, minorCorrectValue, greaterCorrectValue));
            ShowException(message);
        }
    }
}
