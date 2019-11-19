/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.ComponentModel.Composition;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving.WinFormsTouch;

namespace Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving
{
    [Export(typeof(IPurchaseOrder))]
    public class PurchaseOrderReceiving : IPurchaseOrder
    {
        private bool repeatCall = false;

        #region IPurchaseOrder Members
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
        /// Shows purchase order list.
        /// </summary>
        public void ShowPurchaseOrderList()
        {
            DialogResult result = DialogResult.Cancel;
            string selectedReceiptNumber = string.Empty;
            string selectedPONumber = string.Empty;
            PRCountingType prType;

            if (ApplicationSettings.Terminal.TerminalOperator.UserIsInventoryUser)
            {
                using (frmPurchaseOrderReceiptSearch dialog = new frmPurchaseOrderReceiptSearch())
                {
                    dialog.RepeatCalled = this.repeatCall;
                    this.Application.ApplicationFramework.POSShowForm(dialog);
                    result = dialog.DialogResult;
                    selectedReceiptNumber = dialog.SelectedReceiptNumber;
                    selectedPONumber = dialog.SelectedPONumber;
                    prType = dialog.SelectedPRType;
                }

                if (result == DialogResult.OK && !string.IsNullOrEmpty(selectedReceiptNumber) && !string.IsNullOrEmpty(selectedPONumber))
                {
                    ReceivePurchaseOrder(selectedPONumber, selectedReceiptNumber, prType);
                }
            }
            else
            {
                POSFormsManager.ShowPOSMessageDialog(3540);             // Not allowed.
            }
        }

        /// <summary>
        /// Receive purchase order.
        /// </summary>
        /// <param name="poNumber"></param>
        /// <param name="receiptNumber"></param>
        public void ReceivePurchaseOrder(string poNumber, string receiptNumber, PRCountingType prType)
        {
            if (!ApplicationSettings.Terminal.TerminalOperator.UserIsInventoryUser)
            {
                POSFormsManager.ShowPOSMessageDialog(3540);             // Not allowed.
            }
            else
            {
                DialogResult result = DialogResult.Cancel;

                using (frmPurchaseOrderReceiving dialog = new frmPurchaseOrderReceiving())
                {
                    dialog.ReceiptNumber = receiptNumber;
                    dialog.PONumber = poNumber;
                    dialog.PRType = prType;
                    this.Application.ApplicationFramework.POSShowForm(dialog);
                    result = dialog.DialogResult;
                }

                if (result == DialogResult.OK)
                {
                    this.repeatCall = true;
                    ShowPurchaseOrderList();
                }
            }
        }

        #endregion
    }
}
