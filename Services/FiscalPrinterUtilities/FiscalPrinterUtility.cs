/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.FiscalPrinter
{
    public static class FiscalPrinterUtility
    {
        /// <summary>
        /// Returns a string array that contains substrings that fit into the maxLength value.
        /// </summary>
        /// <param name="text">The text to be split.</param>
        /// <param name="maxLength">The max length.</param>
        /// <returns>The array with the substrings.</returns>
        public static IEnumerable<string> SplitByBufferSize(string text, int maxLength)
        {
            var splitLines = text.Split(new[] { Environment.NewLine, "\r", "\n" }, StringSplitOptions.None);

            var line = 0;

            while (line < splitLines.Length)
            {
                if (splitLines[line].Length > maxLength)
                {
                    yield return splitLines[line].Substring(0, Math.Min(maxLength, splitLines[line].Length));

                    splitLines[line] = splitLines[line].Remove(0, Math.Min(maxLength, splitLines[line].Length));
                    if (splitLines[line] == string.Empty)
                    {
                        line++;
                    }
                }
                else
                {
                    var buffer = new StringBuilder();

                    do
                    {
                        buffer.AppendLine(splitLines[line]);
                        line++;
                    } while (line < splitLines.Length
                             && (buffer.Length + splitLines[line].Length) < maxLength);

                    var bufferEndCroppingLength = (splitLines[line - 1] == string.Empty && line < splitLines.Length) ? 0 : Environment.NewLine.Length;

                    yield return buffer.ToString(0, buffer.Length - bufferEndCroppingLength);                    
                }
            }
        }

        /// <summary>
        /// Execute the delegate method and retry using the confirm method when fail.
        /// </summary>
        /// <param name="executeMethod">The delegate method to be execute.</param>
        /// <param name="confirmMethod">The delegate method to confirm the retry.</param>
        /// <returns>True if the execution was successfull; false otherwise.</returns>
        public static bool Execute(Func<bool> executeMethod, Func<Exception, bool> confirmMethod)
        {
            bool retry;
            var sucessful = false;

            do
            {
                try
                {
                    sucessful = executeMethod != null && executeMethod();
                    retry = false;
                }
                catch (Exception exception)
                {
                    retry = confirmMethod != null && confirmMethod(exception);
                }
            } while (retry);

            return sucessful;
        }

        /// <summary>
        /// Shows a dialog that permits retrys the ExecuteOperationMethod delegate execution if it was failed
        /// </summary>
        /// <param name="ex">The exception catched in ExecuteOperationMethod Delegate execution</param>
        /// <returns>True if the operator chooses retry again</returns>        
        public static bool ConfirmOperation(Exception ex)
        {
            if (ex != null)
            {
                LogHelper.LogError("FiscalPrinterUtility.ConfirmOperation", ex.ToString());    
            }            
            
            return UserMessages.ShowQuestionDialog(Resources.Translate(Resources.PaymentOperationError)) == DialogResult.Yes;
        }

        /// <summary>
        /// Execute workDelegate backgroud
        /// </summary>
        /// <param name="workDelegate"></param>
        public static void Execute(Action workDelegate)
        {
            if (ApplicationSettings.Database.IsOffline)
            {
                UserMessages.ShowInformation(Resources.MessageFeatureUnavailableDatabaseOffline);
                return;
            }

            try
            {
                // To allow refresh the button
                System.Windows.Forms.Application.DoEvents();

                LSRetailPosis.POSControls.POSFormsManager.ShowPOSMessageWithBackgroundWorker(Resources.FiscalMenuProcessing, workDelegate);
            }
            catch (FiscalPrinterException)
            {
                // Silently fail on FiscalPrinterException only
            }
            catch (Exception e)
            {
                UserMessages.ShowException(e.Message);
            }
        }

        /// <summary>
        ///  Gets Date Range
        /// </summary>
        /// <param name="startRange">start date range</param>
        /// <param name="endRange">end date range</param>
        /// <returns>true to continue the generation</returns>
        public static bool GetDateRange(out DateTime startRange, out DateTime endRange)
        {
            startRange = DateTime.Now;
            endRange = DateTime.Now;

            using (var form = new frmDatePicker())
            {
                //Start date
                form.Text = Resources.Translate(Resources.MessageStartDate);

                LSRetailPosis.POSControls.POSFormsManager.ShowPOSForm(form);

                if (form.DialogResult != DialogResult.OK)
                {
                    return false;
                }

                startRange = form.SelectedDate;

                //End date
                form.Text = Resources.Translate(Resources.MessageEndDate);

                while (true)
                {
                    LSRetailPosis.POSControls.POSFormsManager.ShowPOSForm(form);
                    if (form.DialogResult != DialogResult.OK)
                    {
                        return false;
                    }

                    endRange = form.SelectedDate;

                    //Validates the value
                    if (endRange < startRange)
                    {
                        UserMessages.ShowInvalidValueMessage(endRange.ToShortDateString(),
                                                             startRange.ToShortDateString());
                    }
                    else
                    {
                        break;
                    }
                }
            }

            //Add (1 day) (- 1 second) to show - 23:59:59
            endRange = endRange.AddDays(1).AddSeconds(-1);
            return UserMessages.ContinueTheGeneration(startRange.ToShortDateString(), endRange.ToShortDateString());
        }

        /// <summary>
        /// Replaces the special characters
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns></returns>
        public static string ReplaceSpecialCharacters(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            var decomposed = input.Normalize(NormalizationForm.FormD);
            var filtered = decomposed.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);

            //The string fields shall not contain ASCII characters in the range 0 to 31, and ASCII 124. These characters shall be replaced by “”.
            var replaced = new string(filtered.ToArray());

            for (var i = 0; i < 32; i++)
            {
                replaced = replaced.Replace(Convert.ToChar(i), Convert.ToChar(0x20));
            }

            replaced = replaced.Replace(Convert.ToChar('|'), Convert.ToChar('/'));

            return replaced;
        }
    }
}
