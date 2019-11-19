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
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
	public partial class frmCustomerTransactions : frmTouchBase
    {
        private const int maxRowsAtEachQuery = 200;
        private System.Data.DataTable customerTransactions;
        private CustomerData customerData;
        private string customerId = string.Empty;
        private string selectedTransactionId = string.Empty;
        private string selectedStoreId = string.Empty;
        private string selectedTerminalId = string.Empty;
        private IApplication Application;

        protected frmCustomerTransactions()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        public frmCustomerTransactions(string customerId, IApplication application) : this()
        {
            this.customerId  = customerId;
            this.Application = application;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!DesignMode)
            {
                this.Bounds = new System.Drawing.Rectangle(
                    ApplicationSettings.MainWindowLeft,
                    ApplicationSettings.MainWindowTop,
                    ApplicationSettings.MainWindowWidth,
                    ApplicationSettings.MainWindowHeight);

                #region Translation
                // Get all text through the Translation function in the ApplicationLocalizer
                // TextID's for Customer BalanceReport are reserved at 51080 - 51099
                // Used textid's 51080 - 51093

                colDate.Caption = ApplicationLocalizer.Language.Translate(51080); //Date
                colTransactionType.Caption = ApplicationLocalizer.Language.Translate(51081); //Type
                colAmount.Caption = ApplicationLocalizer.Language.Translate(51082); //Amount
                colTransactionId.Caption = ApplicationLocalizer.Language.Translate(51083); //Transaction Id
                colReceiptId.Caption = ApplicationLocalizer.Language.Translate(51084); //Receipt Id
                colStore.Caption = ApplicationLocalizer.Language.Translate(51085); //Store
                colTerminal.Caption = ApplicationLocalizer.Language.Translate(51086); //Terminal
                colBalance.Caption = ApplicationLocalizer.Language.Translate(51094); //Balance

                btnClose.Text = ApplicationLocalizer.Language.Translate(51087);
                btnPrintTransaction.Text = ApplicationLocalizer.Language.Translate(51091);
                btnReport.Text = ApplicationLocalizer.Language.Translate(51092);
                
                string[] custTransactionsLanguageTexts = new string[3];
                custTransactionsLanguageTexts[0] = ApplicationLocalizer.Language.Translate(51151);
                custTransactionsLanguageTexts[1] = ApplicationLocalizer.Language.Translate(51152);
                custTransactionsLanguageTexts[2] = ApplicationLocalizer.Language.Translate(51153);
                #endregion

                this.receipt1.DesignAllowedOnPos = LSRetailPosis.Settings.VisualProfiles.VisualProfile.DesignAllowedOnPos;
                this.receipt1.InitCustomFields();

                // TransactionType and Balance columns are only meaningful if the terminal is Standalone, 
                // otherwise this information is only available from AX.
                colTransactionType.Visible = ApplicationSettings.Terminal.Standalone;
                colBalance.Visible = ApplicationSettings.Terminal.Standalone;

                customerData = new CustomerData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                customerTransactions = customerData.GetCustomerTransactions(string.Empty, customerId, maxRowsAtEachQuery, custTransactionsLanguageTexts);
                gridTransactions.DataSource = customerTransactions;
            }

            base.OnLoad(e);
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            grViewTransactions.MovePrev();
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            grViewTransactions.MovePrevPage();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            grViewTransactions.MoveNextPage();
            GetMoreTransactions();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grViewTransactions.MoveNext();
            GetMoreTransactions();
        }

        private void GetMoreTransactions()
        {
            string[] custTransactionsLanguageTexts = new string[3];
            custTransactionsLanguageTexts[0] = ApplicationLocalizer.Language.Translate(51151);
            custTransactionsLanguageTexts[1] = ApplicationLocalizer.Language.Translate(51152);
            custTransactionsLanguageTexts[2] = ApplicationLocalizer.Language.Translate(51153);

            int topRowIndex = this.grViewTransactions.TopRowIndex;
            if ((grViewTransactions.IsLastRow) && (grViewTransactions.RowCount > 0))
            {
                System.Data.DataRow Row = grViewTransactions.GetDataRow(grViewTransactions.GetSelectedRows()[0]);
                string lastTransactionId = Row["TRANSACTIONID"].ToString();
                customerTransactions.Merge(customerData.GetCustomerTransactions(lastTransactionId, customerId, maxRowsAtEachQuery, custTransactionsLanguageTexts));
                grViewTransactions.TopRowIndex = topRowIndex;
            }
        }

        private PosTransaction LoadTransaction(string transactionId, string storeId, string terminalId)
        {
            try
            {
                TransactionData transData = new TransactionData(Application.Settings.Database.Connection,
                    Application.Settings.Database.DataAreaID, Application);

                return transData.LoadSerializedTransaction(transactionId, storeId, terminalId);
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (selectedTransactionId.Length != 0)
            {
                using (frmPrintSelection printSelection = new frmPrintSelection(customerId))
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(printSelection);
                }
            }
        }

        private void grViewTransactions_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            System.Data.DataRow Row = grViewTransactions.GetDataRow(grViewTransactions.GetSelectedRows()[0]);
            selectedTransactionId = (string)Row["TRANSACTIONID"];
            selectedStoreId = (string)Row["STORE"];
            selectedTerminalId = (string)Row["TERMINAL"];
            receipt1.ShowTransaction(LoadTransaction(selectedTransactionId, selectedStoreId, selectedTerminalId));
        }

        private void btnPrintTransaction_Click(object sender, EventArgs e)
        {
            if (selectedTransactionId.Length != 0)
            {
                ITransactionSystem transSys = Customer.InternalApplication.BusinessLogic.TransactionSystem;
                transSys.PrintTransaction(LoadTransaction(selectedTransactionId, selectedStoreId, selectedTerminalId), true, true);
            }
        }
    }
}
