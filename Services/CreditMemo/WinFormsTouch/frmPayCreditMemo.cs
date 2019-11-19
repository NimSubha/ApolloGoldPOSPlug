/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.CreditMemo.WinFormsTouch
{
    public partial class frmPayCreditMemo : frmTouchBase
    {
        #region Member variables

        private string creditMemoId;
        private decimal amount;
        private string currencyCode;
        private bool creditMemoValid;

        #endregion

        #region Properties

        public string CreditMemoId
        {
            set { creditMemoId = value; }
            get { return creditMemoId; }
        }

        public string CurrencyCode
        {
            set { currencyCode = value; }
            get { return currencyCode; }
        }

        public decimal Amount
        {
            set { amount = value; }
            get { return amount; }
        }

        #endregion

        /// <summary>
        /// Required designer variable.
        /// </summary>
        public frmPayCreditMemo()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                //
                // Get all text through the Translation function in the ApplicationLocalizer
                //
                // TextID's for frmCashManagement are reserved at 52600 - 52649
                // In use now are ID's: 52606
                //

                this.Text = lblHeading.Text = ApplicationLocalizer.Language.Translate(52603);  // Credit Memo
                lblCreditMemoIdHeading.Text = ApplicationLocalizer.Language.Translate(52600);  // Enter the credit memo id
                lblCreditMemoAmountHeading.Text = ApplicationLocalizer.Language.Translate(52605);  // Credit memo amount
                lblCreditMemoAmount.Text = string.Empty;
                btnOk.Text = ApplicationLocalizer.Language.Translate(52601); // "Ok";
                btnCancel.Text = ApplicationLocalizer.Language.Translate(52602); // "Cancel";
                btnValidateCreditMemo.Text = ApplicationLocalizer.Language.Translate(52604);  // Get
            }

            base.OnLoad(e);
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (creditMemoValid == true)
            {
                VoidCreditMemo();
            }

            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (creditMemoValid == true)
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "currencyCode")]
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "creditMemoId")]
        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "amount")]
        private void ValidateCreditMemo()
        {
            try
            {
                LogMessage("Validating a credit memo...",
                    LogTraceLevel.Trace,
                    "CreditMemo.frmPayCreditMemo.ValidateCreditMemo");

                if (txtCreditMemoId.Text.Trim().Length == 0)
                {
                    txtCreditMemoId.Focus();
                    return;
                }

                // Begin by checking if there is a connection to the Transaction Service
                CreditMemo.InternalApplication.TransactionServices.CheckConnection();

                bool validated = false;
                string comment = String.Empty;
                decimal amount = 0M;
                string currencyCode = String.Empty;
                string creditMemoId = txtCreditMemoId.Text.Trim();
                string storeId = ApplicationSettings.Terminal.StoreId;
                string terminalId = ApplicationSettings.Terminal.TerminalId;

                CreditMemo.InternalApplication.TransactionServices.ValidateCreditMemo(
                    ref validated, ref comment, ref currencyCode, ref amount, creditMemoId, storeId, terminalId);

                if (validated == true)
                {
                    this.creditMemoValid = true;
                    this.creditMemoId = creditMemoId;
                    this.amount = amount;
                    this.currencyCode = currencyCode;

                    lblCreditMemoAmount.Text = CreditMemo.InternalApplication.Services.Rounding.RoundForDisplay(amount, currencyCode, true, false);
                    txtCreditMemoId.Focus();
                    txtCreditMemoId.Select(txtCreditMemoId.Text.Length, 0);
                }
                else
                {
                    this.creditMemoValid = false;
                    this.creditMemoId = String.Empty;
                    this.amount = 0M;
                    this.currencyCode = String.Empty;

                    using (frmMessage dialog = new frmMessage(comment, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                    {
                        CreditMemo.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                    }

                    lblCreditMemoAmount.Text = String.Empty;
                    comment = String.Empty;
                    txtCreditMemoId.Text = String.Empty;
                    txtCreditMemoId.Focus();
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "creditMemoId")]
        private void VoidCreditMemo()
        {
            try
            {
                LogMessage("Void a credit memo...", LogTraceLevel.Trace, "CreditMemo.frmPayCreditMemo.VoidCreditMemo");

                if (creditMemoValid == false)
                {
                    txtCreditMemoId.Focus();
                    return;
                }

                // Begin by checking if there is a connection to the Transaction Service
                CreditMemo.InternalApplication.TransactionServices.CheckConnection();

                bool ok = false;
                string comment = String.Empty;
                string creditMemoId = txtCreditMemoId.Text.Trim();
                string storeId = ApplicationSettings.Terminal.StoreId;
                string terminalId = ApplicationSettings.Terminal.TerminalId;

                CreditMemo.InternalApplication.TransactionServices.VoidCreditMemoPayment(ref ok, ref comment, this.creditMemoId, storeId, terminalId);

                if (ok)
                {
                    this.creditMemoValid = false;

                    lblCreditMemoAmount.Text = CreditMemo.InternalApplication.Services.Rounding.RoundForDisplay(0, true, false);
                    txtCreditMemoId.Focus();
                    txtCreditMemoId.Select(txtCreditMemoId.Text.Length, 0);
                }
                else
                {
                    this.creditMemoValid = true;

                    using (frmMessage dialog = new frmMessage(comment, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error))
                    {
                        CreditMemo.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                    }
                    txtCreditMemoId.Text = String.Empty;
                    txtCreditMemoId.Focus();
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        private void txtCreditMemoId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (!this.creditMemoValid)
                {
                    ValidateCreditMemo();
                }
                else
                {
                    btnOk_Click(this, new EventArgs());
                }
            }
        }

        private void btnValidateCreditMemo_Click(object sender, EventArgs e)
        {
            ValidateCreditMemo();
        }

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceLevel"></param>
        /// <param name="args"></param>
        private void LogMessage(string message, LogTraceLevel traceLevel, params object[] args)
        {
            ApplicationLog.Log(this.GetType().Name, string.Format(message, args), traceLevel);
        }
    }
}
