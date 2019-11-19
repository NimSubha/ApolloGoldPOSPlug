/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Dialog.ViewModels;
using Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using System.Data;

namespace Microsoft.Dynamics.Retail.Pos.Dialog
{
    [Export(typeof(IDialog))]
    public class Dialog : IDialog
    {
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

        #region IDialog Members

        /// <summary>
        /// Displays journal dialogue as for touch based or keyboard based.
        /// </summary>
        /// <param name="journalDialogResults"></param>
        /// <param name="dialogResultObject"></param>
        /// <param name="posTransaction"></param>
        public void ShowJournalDialog(ref JournalDialogResults journalDialogResults, ref object dialogResultObject, IPosTransaction posTransaction)
        {
            // Touch based hardware
            using (frmJournal journalDialog = new frmJournal(posTransaction, Application))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(journalDialog);

                journalDialogResults = journalDialog.JournalDialogResults;
                dialogResultObject = journalDialog.JournalDialogResultObject;
            }
        }

        //Start:Nim
        public DialogResult MyItemSearch(int howManyRows, ref string selectedItemID, out DataSet dsItemDetails, string condition = "")
        {
            using (frmItemSearch itemSearch = new frmItemSearch(howManyRows, condition, "O"))
            {
                dsItemDetails = new DataSet();
                DataTable dtItemDetails = new DataTable("dtItemDetails");
                dtItemDetails.Columns.Add("ITEMID", typeof(string));
                dtItemDetails.Columns.Add("ITEMNAME", typeof(string));
                dtItemDetails.Columns.Add("ITEMPRICE", typeof(string));
                DataRow drItem;

                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(itemSearch);

                selectedItemID = itemSearch.SelectedItemId;
                drItem = dtItemDetails.NewRow();
                drItem["ITEMID"] = itemSearch.SelectedItemId;
                drItem["ITEMNAME"] = itemSearch.selectedItemName;
                drItem["ITEMPRICE"] = itemSearch.selectedItemPrice;
                dtItemDetails.Rows.Add(drItem);
                dsItemDetails.Tables.Add(dtItemDetails);
                return itemSearch.DialogResult;
            }
        }
        //End:Nim

        /// <summary>
        /// Returns message as per given message id as DialogResult.
        /// </summary>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public DialogResult ShowMessage(int messageId)
        {
            using (frmMessage dialog = new frmMessage(messageId))
            {
				POSFormsManager.ShowPOSForm(dialog);

                return dialog.DialogResult;
            }
        }

        /// <summary>
        /// Displays message as per given message id.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="msgBoxButtons"></param>
        /// <returns></returns>
        public DialogResult ShowMessage(int messageId, MessageBoxButtons msgBoxButtons)
        {
            using (frmMessage dialog = new frmMessage(messageId, msgBoxButtons))
            {
				POSFormsManager.ShowPOSForm(dialog);
                return dialog.DialogResult;
            }
        }

        /// <summary>
        /// Displays message as per given message id.
        /// </summary>
        /// <param name="messageId"></param>
        /// <param name="msgBoxButtons"></param>
        /// <param name="msgBoxIcon"></param>
        /// <returns></returns>
        public DialogResult ShowMessage(int messageId, MessageBoxButtons msgBoxButtons, MessageBoxIcon msgBoxIcon)
        {
            using (frmMessage dialog = new frmMessage(messageId, msgBoxButtons, msgBoxIcon))
            {
				POSFormsManager.ShowPOSForm(dialog);
                return dialog.DialogResult;
            }
        }

        /// <summary>
        /// Displays message as per given message id.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="msgBoxButtons"></param>
        /// <param name="msgBoxIcon"></param>
        /// <returns></returns>
        public DialogResult ShowMessage(string message, MessageBoxButtons msgBoxButtons, MessageBoxIcon msgBoxIcon)
        {
            using (frmMessage dialog = new frmMessage(message, msgBoxButtons, msgBoxIcon))
            {
				POSFormsManager.ShowPOSForm(dialog);
                return dialog.DialogResult;
            }
        }

        /// <summary>
        /// Displays items as per given item id as well as no of rows to be displayed.
        /// </summary>
        /// <param name="howManyRows"></param>
        /// <param name="selectedItemID"></param>
        /// <returns></returns>
        public DialogResult ItemSearch(int howManyRows, ref string selectedItemID)
        {
            using (frmItemSearch itemSearch = new frmItemSearch(howManyRows))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(itemSearch);
                selectedItemID = itemSearch.SelectedItemId;
                return itemSearch.DialogResult;
            }
        }

        /// <summary>
        /// Does generic search of the items.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="selectedRow"></param>      
        /// <returns></returns>
        public DialogResult GenericSearch(System.Data.DataTable dataTable, ref System.Data.DataRow selectedRow)
        {
                    return GenericSearch(dataTable,ref selectedRow,null);           
        }

        /// <summary>
        /// Does generic search of the items.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="selectedRow"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public DialogResult GenericSearch(System.Data.DataTable dataTable, ref System.Data.DataRow selectedRow, string title)
        {
            using (frmGenericSearch frmGenericSearch = new frmGenericSearch(dataTable, selectedRow, title))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frmGenericSearch);
                selectedRow = frmGenericSearch.SelectedDataRow;
                return frmGenericSearch.DialogResult;
            }
        }

        /// <summary>
        /// Generic lookup.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="displayColumn"></param>
        /// <param name="selectedRow"></param>
        /// <param name="sizeFactor"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public DialogResult GenericLookup(System.Data.DataTable dataTable, int displayColumn, ref System.Data.DataRow selectedRow, string defaultValue)
        {
            using (frmGenericLookup frmGenericLookup = new frmGenericLookup(dataTable, displayColumn, defaultValue))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frmGenericLookup);
                selectedRow = frmGenericLookup.SelectedDataRow;
                return frmGenericLookup.DialogResult;
            }
        }

        /// <summary>
        /// Generic lookup which shows a list of class instances
        /// </summary>
        /// <param name="dataTable">List of classes whose properties are shown</param>
        /// <param name="displayPropertyName">Name of property which should be shown in the lookup form</param>
        /// <param name="displayColumnCaption">Text label for the column on the lookup</param>
        /// <param name="returnPropertyName">Name of property which should be returned from selected record</param>
        /// <param name="returnPropertyValue">out string of the value in the return property</param>
        /// <param name="sizeFactor"></param>
        /// <param name="defaultValue">default focused row when lookup form opens</param>
        /// <returns>Exit result of the lookup dialog</returns>
        public DialogResult GenericLookup(IList dataTable, string displayPropertyName, string displayColumnCaption, string returnPropertyName, out String returnPropertyValue, string defaultValue)
        {
            using (frmGenericLookup frmGenericLookup = new frmGenericLookup(dataTable, displayPropertyName, displayColumnCaption, returnPropertyName, defaultValue))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frmGenericLookup);
                returnPropertyValue = frmGenericLookup.SelectedString;
                return frmGenericLookup.DialogResult;
            }
        }

        /// <summary>
        /// Displays price check in a dialog.
        /// </summary>
        /// <param name="useScanner"></param>
        /// <param name="posTransaction"></param>
        /// <param name="inputText"></param>
        /// <returns></returns>
        public DialogResult PriceCheck(bool useScanner, IPosTransaction posTransaction, ref string inputText)
        {
            DialogResult result = DialogResult.Cancel;
            using (frmPriceCheck frmPriceCheck = new frmPriceCheck(posTransaction))
            {
                frmPriceCheck.UseScanner = useScanner;
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frmPriceCheck);
                result = frmPriceCheck.DialogResult;
                inputText = frmPriceCheck.Barcode;
            }

            return result;
        }

        /// <summary>
        /// Displays tender declaration in a dialog box.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <returns></returns>
        public DialogResult TenderDeclaration(IPosTransaction posTransaction)
        {
            DialogResult result = DialogResult.Cancel;
            using (frmTenderCount frmTenderCount = new frmTenderCount(posTransaction))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frmTenderCount);
                result = frmTenderCount.DialogResult;
            }
            return result;
        }

        /// <summary>
        /// Displays possible tax override codes in a list and returns the selected one
        /// </summary>
        /// <param name="howManyRows">Number of rows to display in list</param>
        /// <param name="overrideBy">Value of TaxOverrideBy enum, telling whether line or transaction overrides should be listed</param>
        /// <param name="selectedTaxGroupId">The selected tax override</param>
        /// <returns></returns>
        public DialogResult TaxOverrideSearch(int howManyRows, int overrideBy, ref string selectedTaxCodeId)
        {
            DialogResult result = DialogResult.Cancel;
            using (frmTaxOverrideSearch taxOverrideSearch = new frmTaxOverrideSearch((TaxOverrideBy)overrideBy, howManyRows))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(taxOverrideSearch);
                selectedTaxCodeId = taxOverrideSearch.SelectedTaxOverrideCode;
                result = taxOverrideSearch.DialogResult;
            }
            return result;
        }

        /// <summary>
        /// Displays inventory lookup form.
        /// </summary>
        /// <param name="posTransaction">Current POS transaction</param>
        /// <returns>Dialog result</returns>
        public DialogResult InventoryLookup(IPosTransaction posTransaction)
        {
            using (formInventoryLookup frm = new formInventoryLookup(posTransaction))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frm);

                return frm.DialogResult;
            }
        }

        /// <summary>
        /// Displays inventory lookup form.
        /// </summary>
        /// <param name="itemOrVariantId">item id or variant id</param>
        /// <returns>Dialog result</returns>
        public DialogResult InventoryLookup(string itemOrVariantId)
        {
            using (formInventoryLookup frm = new formInventoryLookup(itemOrVariantId))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(frm);

                return frm.DialogResult;
            }
        }

        /// <summary>
        /// Searches the specified search term.
        /// </summary>
        /// <param name="searchType">Startup type of the search.</param>
        /// <param name="searchTerm">The search term.</param>
        /// <param name="showUIForEmptyResult">if set to <c>true</c> then show UI even when result is empty.</param>
        /// <returns>
        /// Selected result if found, null otherwise.
        /// </returns>
        public ISearchResult Search(SearchType searchType, string searchTerm, bool showUIForEmptyResult)
        {
            SearchResult result = null;
            SearchViewModel viewModel = new SearchViewModel(searchType, searchTerm);

            viewModel.ExecuteSearch();

            // We will not show the UI for emtpy result if not requested.
            if (showUIForEmptyResult
                || viewModel.Results.Count > 0
                || viewModel.SearchTerms.Length < viewModel.MinimumSearchTermLengh)
            {
                result = new SearchResult();

                using (frmSmartSearch searchForm = new frmSmartSearch(viewModel))
                {
                    POSFormsManager.ShowPOSForm(searchForm);

                    if (searchForm.DialogResult == DialogResult.OK)
                    {
                        result.SearchType = (SearchResultType)viewModel.SelectedResult.SearchType;
                        result.Number = viewModel.SelectedResult.Number;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Displays enter bank drop bag number form.
        /// </summary>
        /// <param name="posTransaction">Current POS transaction</param>
        public void InputBankDropNum(IPosTransaction posTransaction)
        {
            InputConfirmation inputConfirmation = new InputConfirmation()
            {
                MaxLength = 30,
                PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(3924),      // "Enter Bank Bag No:";
            };

            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
            {
                if (inputConfirmation.Confirmed)
                {
                    ((BankDropTransaction)posTransaction).BankBagNo = inputConfirmation.EnteredText;
                }
            }
            );

            InternalApplication.Services.Interaction.InteractionRequest(request);
            ((PosTransaction)posTransaction).EntryStatus = PosTransaction.TransactionStatus.Normal;
        }

        #endregion

    }
}
