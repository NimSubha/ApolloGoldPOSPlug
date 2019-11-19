/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.StockCount
{
    [Export(typeof(IStockCount))]
    public class StockCount : IStockCount
    {
        private bool repeatCall = false;

        #region IStockCount Members
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

        /// <summary>
        /// Performs stock count.
        /// </summary>
        public void PerformStockCount()
        {
            ShowStockCountHeaders();
        }

        private void ShowStockCountHeaders()
        {
            DialogResult result = DialogResult.Cancel;
            string selectedJournalId = null;
            string selectedRecId = null;

            if (ApplicationSettings.Terminal.TerminalOperator.UserIsInventoryUser)
            {
                using (frmStockCountJournals dialog = new frmStockCountJournals())
                {
                    dialog.RepeatCall = repeatCall;
                    this.Application.ApplicationFramework.POSShowForm(dialog);
                    result = dialog.DialogResult;
                    selectedJournalId = dialog.SelectedJournalId;
                    selectedRecId = dialog.SelectedRecordId;
                }

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(selectedJournalId) && !string.IsNullOrWhiteSpace(selectedRecId))
                {
                    ShowStockCountLines(selectedJournalId, selectedRecId);
                }
            }
            else
            {
                POSFormsManager.ShowPOSMessageDialog(3540);             // Not allowed.
            }
        }

        private void ShowStockCountLines(string selectedJournalId, string selectedRecId)
        {
            DialogResult result = DialogResult.Cancel;
            if (ApplicationSettings.Terminal.TerminalOperator.UserIsInventoryUser)
            {
                using (frmStockCount dialog = new frmStockCount())
                {
                    dialog.SelectedJournalId = selectedJournalId;
                    dialog.SelectedRecordId = selectedRecId;
                    this.Application.ApplicationFramework.POSShowForm(dialog);
                    result = dialog.DialogResult;
                }

                if (result == DialogResult.OK)
                {
                    this.repeatCall = true;
                    ShowStockCountHeaders();
                }
            }
            else
            {
                POSFormsManager.ShowPOSMessageDialog(3540);             // Not allowed.
            }
        }
        #endregion
    }
}
