/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using System.Data.SqlClient;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.GiftCard
{
    partial class GiftCardForm : frmTouchBase
    {

        [Import]
        private IApplication application;
        #region Member variables

        GiftCardController controller;

        #endregion

        #region Construction

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Context of the gift card form</param>
        /// <param name="posTransaction">Transaction object.</param>
        /// <param name="tenderInfo">Tender information about GC (Required for Payment Context) </param>
        public GiftCardForm(GiftCardController giftCardController)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            this.controller = giftCardController;

            // Initial field values.//commented for Apollo
            //if (giftCardController.Context == Microsoft.Dynamics.Retail.Pos.GiftCard.GiftCardController.ContextType.GiftCardIssue)
            //{
            //    padGiftCardNumber.Enabled = false;
            //    this.controller.CardNumber = GetNextNum();
            //}
            //else
            //    padGiftCardNumber.Enabled = true;

            padGiftCardNumber.EnteredValue = this.controller.CardNumber;


        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                //
                // Get all text through the Translation function in the ApplicationLocalizer
                //
                // TextID's for frmPayGiftCard are reserved at 55400 - 55499
                // In use now are ID's: 55400 - 55406
                //
                TranslateAndSetupControls();

                // Hook up MSR/Scanner
                GiftCard.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(ProcessScannedItem);
                GiftCard.InternalApplication.Services.Peripherals.MSR.MSRMessageEvent -= new MSRMessageEventHandler(ProcessSwipedCard);
                GiftCard.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent += new ScannerMessageEventHandler(ProcessScannedItem);
                GiftCard.InternalApplication.Services.Peripherals.MSR.MSRMessageEvent += new MSRMessageEventHandler(ProcessSwipedCard);
                EnablePosDevices();
            }

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DisablePosDevices();
        }

        private void TranslateAndSetupControls()
        {
            this.Text = lblHeading.Text = controller.GetFormTitle();
            padGiftCardNumber.PromptText = ApplicationLocalizer.Language.Translate(55401);  // Gift card number:
            lblGiftCardBalanceTitle.Text = ApplicationLocalizer.Language.Translate(55402);  // Gift card balance:
            lblGiftCardBalance.Text = string.Empty;
            this.btnOk.Text = ApplicationLocalizer.Language.Translate(55403); // Ok
            btnCancel.Text = ApplicationLocalizer.Language.Translate(55404); // Cancel
            btnGet.Text = ApplicationLocalizer.Language.Translate(55405);  // Check balance
            numAmount.PromptText = ApplicationLocalizer.Language.Translate(55406);  // Gift card amount:
            this.lblPayAmt.Text = ApplicationLocalizer.Language.Translate(1442); // Pay amount

            switch (controller.Context)
            {
                case GiftCardController.ContextType.GiftCardBalance:
                    this.numAmount.Visible = false;
                    this.btnOk.Text = ApplicationLocalizer.Language.Translate(55410); // Print;

                    break;

                case GiftCardController.ContextType.GiftCardIssue:
                case GiftCardController.ContextType.GiftCardAddTo:
                    this.btnGet.Visible = false;
                    this.lblGiftCardBalanceTitle.Visible = false;
                    this.lblGiftCardBalance.Visible = false;
                    this.tableLayoutPanel4.SetColumnSpan(this.padGiftCardNumber, 3);

                    break;

                case GiftCardController.ContextType.GiftCardPayment:
                    panelAmount.Visible = true;
                    this.lblHeading.Text = ApplicationLocalizer.Language.Translate(55416,
                        GiftCard.InternalApplication.Services.Rounding.Round(this.controller.TransactionAmount, true));

                    SetQuickAmountButtons(this.controller.ProposedPaymentAmount);

                    if (controller.TransactionAmount < 0)
                    {
                        numAmount.NegativeMode = true;
                    }

                    break;
            }
        }

        private void SetQuickAmountButtons(decimal quickButtonAmount)
        {
            amtGiftCardAmounts.LocalCurrencyCode = ApplicationSettings.Terminal.StoreCurrency;
            amtGiftCardAmounts.UsedCurrencyCode = ApplicationSettings.Terminal.StoreCurrency;

            IRounding rounding = GiftCard.InternalApplication.Services.Rounding;
            amtGiftCardAmounts.SoldLocalAmount = rounding.RoundAmount(quickButtonAmount, ApplicationSettings.Terminal.StoreId, this.controller.TenderInfo.TenderID.ToString());
            amtGiftCardAmounts.ForeignCurrencyMode = false;

            amtGiftCardAmounts.HighestOptionAmount = this.controller.TenderInfo.MaximumAmountAllowed;
            amtGiftCardAmounts.LowesetOptionAmount = this.controller.TenderInfo.MinimumAmountAllowed;

            amtGiftCardAmounts.ViewOption = AmountViewer.ViewOptions.ExcactAmountOnly;

            amtGiftCardAmounts.SetButtons();
        }

        #endregion

        #region Methods

        private void PreExecute()
        {
            DisablePosDevices();

            try
            {
                switch (controller.Context)
                {
                    case GiftCardController.ContextType.GiftCardPayment:
                        lblGiftCardBalance.Text = string.Empty;
                        lblGiftCardBalance.Text = controller.ValidateGiftCardPayment(padGiftCardNumber.EnteredValue);
                        SetQuickAmountButtons(this.controller.MaxTransactionAmountAllowed);
                        numAmount.Focus();
                        break;

                    case GiftCardController.ContextType.GiftCardBalance:
                        lblGiftCardBalance.Text = string.Empty;
                        lblGiftCardBalance.Text = controller.GetGiftCardBalance(padGiftCardNumber.EnteredValue);
                        padGiftCardNumber.SelectAll();
                        break;

                    case GiftCardController.ContextType.GiftCardIssue:
                    case GiftCardController.ContextType.GiftCardAddTo:
                        numAmount.Focus();
                        break;
                }
            }
            catch (PosisException px)
            {
                HandlePosIsException(px);
            }
            catch (GiftCardException gex)
            {
                HandleGiftCardException(gex);
            }

            EnablePosDevices();
        }

        private bool Execute()
        {
            bool result = false;

            DisablePosDevices();

            if (numAmount.Visible)
            {
                decimal roundedAmount = controller.RoundAmount(numAmount.EnteredDecimalValue);

                numAmount.ClearValue();
                numAmount.EnteredValue = roundedAmount.ToString();
            }

            try
            {
                switch (controller.Context)
                {
                    case GiftCardController.ContextType.GiftCardPayment:
                        lblGiftCardBalance.Text = string.Empty;
                        lblGiftCardBalance.Text = controller.ValidateGiftCardPayment(padGiftCardNumber.EnteredValue);
                        controller.ValidateTenderAmount(numAmount.EnteredDecimalValue);
                        result = true;
                        break;

                    case GiftCardController.ContextType.GiftCardIssue:
                        controller.IssueGiftCard(padGiftCardNumber.EnteredValue, numAmount.EnteredDecimalValue);
                        result = true;
                        break;

                    case GiftCardController.ContextType.GiftCardAddTo:
                        controller.AddToGiftCard(padGiftCardNumber.EnteredValue, numAmount.EnteredDecimalValue);
                        result = true;
                        break;

                    case GiftCardController.ContextType.GiftCardBalance:
                        controller.PrintGiftCardBalance(padGiftCardNumber.EnteredValue);
                        padGiftCardNumber.SelectAll();
                        break;
                }
            }
            catch (PosisException px)
            {
                HandlePosIsException(px);
            }
            catch (GiftCardException gex)
            {
                HandleGiftCardException(gex);
            }

            EnablePosDevices();

            return result;
        }

        private bool RollBack()
        {
            bool result = false;

            try
            {
                switch (controller.Context)
                {
                    case GiftCardController.ContextType.GiftCardPayment:
                        controller.VoidGiftCardPayment();
                        break;
                }

                result = true;
            }
            catch (PosisException px)
            {
                HandlePosIsException(px);
            }
            catch (GiftCardException gex)
            {
                HandleGiftCardException(gex);
            }

            return result;
        }

        private void HandleGiftCardException(GiftCardException ex)
        {
            ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            ShowErrorMessage(ex.Message);
        }

        private void HandlePosIsException(PosisException ex)
        {
            string message = string.Format(ApplicationLocalizer.Language.Translate(55002),
                    LSRetailPosis.Settings.ApplicationSettings.ShortApplicationTitle);

            ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            ShowErrorMessage(message);
        }

        private void ShowErrorMessage(string message)
        {
            using (frmMessage dialog = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Error))
            {
                dialog.ShowDialog(this);
            }

            padGiftCardNumber.SelectAll();
        }

        private static void EnablePosDevices()
        {
            GiftCard.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();
            GiftCard.InternalApplication.Services.Peripherals.MSR.EnableForSwipe();
        }

        private static void DisablePosDevices()
        {
            GiftCard.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
            GiftCard.InternalApplication.Services.Peripherals.MSR.DisableForSwipe();
        }

        #endregion

        #region Events

        private void padGiftCardId_EnterButtonPressed()
        {
            if (padGiftCardNumber.EnteredValue.Length > 0)
            {
                PreExecute();
            }
        }

        private void padGiftCardId_CardSwept(string[] trackParts)
        {
            if ((trackParts != null) && (trackParts.Length > 0))
            {
                padGiftCardNumber.EnteredValue = trackParts[0];
                padGiftCardNumber.Refresh();
                PreExecute();
            }
        }

        private void btnValidateGiftCard_Click(object sender, EventArgs e)
        {
            PreExecute();
        }

        private void ProcessScannedItem(IScanInfo scanInfo)
        {
            if (scanInfo.ScanData.Length > 0)
            {
                padGiftCardNumber.EnteredValue = scanInfo.ScanData;
                padGiftCardNumber.Refresh();
                PreExecute();
            }
        }

        private void ProcessSwipedCard(ICardInfo cardInfo)
        {
            padGiftCardId_CardSwept(cardInfo.Track2Parts);
        }

        private void numPad_EnterButtonPressed()
        {
            btnOk_Click(this, null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (RollBack())
            {
                this.DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (padGiftCardNumber.EnteredValue.Length > 0
                && (!numAmount.Visible || numAmount.EnteredDecimalValue != 0 || ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments)
                && Execute())
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void amtGiftCardAmounts_AmountChanged(decimal outAmount, string currCode)
        {
            if (padGiftCardNumber.EnteredValue.Length > 0)
            {
                numAmount.EnteredValue = outAmount.ToString();

                if (Execute())
                {
                    this.DialogResult = DialogResult.OK;
                    Close();
                }
            }
        }

        #endregion

        #region GetNextNum()
        public string GetNextNum()
        {

            string OrderNum = string.Empty;
            //OrderNum = GetNextSeqID();//from local
            #region [call ts]

            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getAutoGiftCard");

                    OrderNum = Convert.ToString(containerArray[2]);
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }

            #endregion
            return OrderNum;
        }
        #endregion

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum ReceiptTransactionType
        {
            IssueGiftCard = 20
        }

        public string GetNextSeqID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.IssueGiftCard;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                orderNumber((int)transType, funcProfileId, out mask);
                if (string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }
        #endregion

        #region GetOrderNum()  - CHANGED BY NIMBUS
        private void orderNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        #endregion

        #region GetSeedVal() - CHANGED BY NIMBUS
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.IssueGiftCard;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT MAX(ISNULL(GiftCardNumberSeedValue,0)) + 1 from CRWGiftCardIssueNumberSeq";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                    {
                        Val = "1";
                    }

                    return Val;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        #endregion

    }
}
