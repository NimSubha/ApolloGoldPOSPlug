/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Pos.StockCount
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "frm", Justification = "Grandfather")]
    public partial class frmStockCount : LSRetailPosis.POSProcesses.frmTouchBase
    {
        private IPosTransaction posTransaction;
        private DataTable entryTable;
        private SaleLineItem saleLineItem;
        private StockCountData scData;
        private NumPadMode inputMode;
        private bool isEdit;

        /// <summary>
        /// Get/set property for PosTransaction.
        /// </summary>
        public PosTransaction PosTransaction { get; set; }

        /// <summary>
        /// Get/set property for SelectedJournalId
        /// </summary>
        public string SelectedJournalId { get; set; }

        /// <summary>
        /// Get/set property for SelectedRecId
        /// </summary>
        public string SelectedRecordId { get; set; }

        /// <summary>
        /// Line entry for a scanned item
        /// </summary>
        private struct StockCountItem
        {
            public string ItemNumber;
            public string ItemName;
            public decimal Quantity;
            public string Unit;
        }

        /// <summary>
        /// Input mode of the numberpad area
        /// </summary>
        private enum NumPadMode
        {
            Barcode = 0,
            Quantity = 1
        }

        /// <summary>
        /// Set input mode for stock count.
        /// </summary>
        public frmStockCount()
        {
            InitializeComponent();

            SetInputMode(NumPadMode.Barcode);
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();

                StockCount.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(OnBarcodeScan);
                StockCount.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent += new ScannerMessageEventHandler(OnBarcodeScan);
                StockCount.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();

                scData = new StockCountData(
                    ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID,
                    ApplicationSettings.Terminal.StorePrimaryId);

                posTransaction = StockCount.InternalApplication.BusinessLogic.Utility.CreateSalesOrderTransaction(
                    ApplicationSettings.Terminal.StoreId,
                    ApplicationSettings.Terminal.StoreCurrency,
                    ApplicationSettings.Terminal.TaxIncludedInPrice,
                    StockCount.InternalApplication.Services.Rounding);

                ClearForm();
            }

            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for frmStockCount are reserved at 103100 - 103119
            // In use now are ID's: 103100 - 103111


            this.Text = lblHeader.Text = ApplicationLocalizer.Language.Translate(103100); //Stock Counting
            btnClose.Text = ApplicationLocalizer.Language.Translate(2605);   //Close
            btnRefresh.Text = ApplicationLocalizer.Language.Translate(103116);     //Refresh
            btnSave.Text = ApplicationLocalizer.Language.Translate(103117);    //Save
            btnReceiveAll.Text = ApplicationLocalizer.Language.Translate(103118);    //Receive All
            btnSearch.Text = ApplicationLocalizer.Language.Translate(2607);   //Search
            btnRemove.Text = ApplicationLocalizer.Language.Translate(103105); //Remove
            btnEdit.Text = ApplicationLocalizer.Language.Translate(103106); //Edit
            btnCommit.Text = ApplicationLocalizer.Language.Translate(103107); //Commit
            colDescription.Caption = ApplicationLocalizer.Language.Translate(103102); //Description
            colID.Caption = ApplicationLocalizer.Language.Translate(103101); //Item Number
            colCounted.Caption = ApplicationLocalizer.Language.Translate(1031031); //Counted
            colQuantity.Caption = ApplicationLocalizer.Language.Translate(103103); //Quantity
            colUnit.Caption = ApplicationLocalizer.Language.Translate(103104); //Units
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StockCount.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
        }

        void OnBarcodeScan(IScanInfo scanInfo)
        {
            InventoryLookup(scanInfo.ScanDataLabel);
        }

        private void SetInputMode(NumPadMode mode)
        {
            if (mode == NumPadMode.Barcode)
            {
                StockCount.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();
                numPad1.EntryType = NumpadEntryTypes.Barcode;
                numPad1.PromptText = ApplicationLocalizer.Language.Translate(103110);               //Scan or enter barcode
                numPad1.EnteredValue = string.Empty;
                isEdit = false;
            }
            else
            {
                StockCount.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
                numPad1.EntryType = NumpadEntryTypes.Quantity;
                numPad1.PromptText = ApplicationLocalizer.Language.Translate(103111);               //Scan or enter barcode
                numPad1.EnteredValue = string.Empty;
            }

            inputMode = mode;
        }

        private void ClearForm()
        {
            if (entryTable != null)
            {
                entryTable.Clear();
            }

            grInventory.DataSource = this.entryTable;

            ResetNumpad();
            LoadStockCounts();
        }

        private void LoadStockCountsFromDB()
        {
            this.entryTable = scData.GetStockCountLines(this.SelectedJournalId);
            this.entryTable = AddVariantInfo(this.entryTable);
        }

        private void LoadStockCounts()
        {
            LoadStockCountsFromDB();

            if (entryTable != null && entryTable.Rows.Count == 0)
            {
                GetStockCountLinesFromAX();
                SaveStockCounts();
                LoadStockCountsFromDB();
            }

            // Reset RowState for the first time of bringing lines from AX into an empty grid, 
            // or from DB, therefore it will NOT trigger the prompt of saving changes before exiting the form.
            entryTable.AcceptChanges();
            grInventory.DataSource = this.entryTable;
        }

        private void SaveStockCounts()
        {
            scData.SaveStockCounts(this.entryTable);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (entryTable.GetChanges() != null)
            {
                POSFormsManager.ShowPOSMessageDialog(103119);
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void frmstockCount_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // Search will put the input mode to barcode mode
                inputMode = NumPadMode.Barcode;

                string selectedItemId = numPad1.EnteredValue;

                // Show the search dialog through the item service
                if (!StockCount.InternalApplication.Services.Item.ItemSearch(ref selectedItemId, 500))
                    return;

                numPad1.EnteredValue = selectedItemId;
                InventoryLookup(numPad1.EnteredValue);
                numPad1.Focus();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        void numPad1_EnterButtonPressed()
        {
            InventoryLookup(numPad1.EnteredValue);
        }

        private void InventoryLookup(string barcode)
        {
            if (inputMode == NumPadMode.Barcode)
            {
                if (GetItemInfo(barcode))
                {
                    // set mode to quantity
                    SetInputMode(NumPadMode.Quantity);
                }
                else
                {
                    // clear the current item so the user can try again
                    ResetNumpad();
                }
            }
            else if ((inputMode == NumPadMode.Quantity) && (saleLineItem != null) && !string.IsNullOrEmpty(numPad1.EnteredValue))
            {
                UnitOfMeasureData unitOfMeasureData = new UnitOfMeasureData(
                    StockCount.InternalApplication.Settings.Database.Connection,
                    StockCount.InternalApplication.Settings.Database.DataAreaID,
                    LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId,
                    StockCount.InternalApplication);

                // add to list
                StockCountItem item = new StockCountItem();

                item.ItemNumber = this.saleLineItem.ItemId;
                item.Unit = this.saleLineItem.InventOrderUnitOfMeasure;
                item.ItemName = this.saleLineItem.Description;

                Decimal quantity = 0;
                Decimal.TryParse(numPad1.EnteredValue, out quantity);

                item.Quantity = StockCount.InternalApplication.Services.Rounding.Round(quantity, unitOfMeasureData.GetUnitDecimals(item.Unit));

                AddItem(item);
                SetInputMode(NumPadMode.Barcode);
            }

            // When finished, put focus back into the numpad
            numPad1.Select();
        }

        private bool GetItemInfo(string barcode)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return false;
            }

            saleLineItem = (SaleLineItem)StockCount.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                ApplicationSettings.Terminal.StoreCurrency,
                StockCount.InternalApplication.Services.Rounding,
                posTransaction);

            IScanInfo scanInfo = StockCount.InternalApplication.BusinessLogic.Utility.CreateScanInfo();
            scanInfo.ScanDataLabel = barcode;
            scanInfo.EntryType = BarcodeEntryType.ManuallyEntered;

            IBarcodeInfo barcodeInfo = StockCount.InternalApplication.Services.Barcode.ProcessBarcode(scanInfo);

            if ((barcodeInfo.InternalType == BarcodeInternalType.Item) && (barcodeInfo.ItemId != null))
            {
                // The entry was a barcode which was found and now we have the item id...
                saleLineItem.ItemId = barcodeInfo.ItemId;
                saleLineItem.Description = barcodeInfo.Description;
                saleLineItem.BarcodeId = barcodeInfo.BarcodeId;
                saleLineItem.SalesOrderUnitOfMeasure = barcodeInfo.UnitId;
                saleLineItem.EntryType = barcodeInfo.EntryType;
                saleLineItem.Dimension.ColorId = barcodeInfo.InventColorId;
                saleLineItem.Dimension.SizeId = barcodeInfo.InventSizeId;
                saleLineItem.Dimension.StyleId = barcodeInfo.InventStyleId;
                saleLineItem.Dimension.ConfigId = barcodeInfo.ConfigId;
                saleLineItem.Dimension.VariantId = barcodeInfo.VariantId;
            }
            else
            {
                // It could be an ItemId
                saleLineItem.ItemId = barcodeInfo.BarcodeId;
                saleLineItem.EntryType = barcodeInfo.EntryType;
            }

            // fetch all the addtional item properties
            StockCount.InternalApplication.Services.Item.ProcessItem(saleLineItem, true);

            if (!saleLineItem.Found)
            {
                POSFormsManager.ShowPOSMessageDialog(2611);             // Item not found.
                return false;
            }
            else if ((saleLineItem.Dimension != null)
                && (saleLineItem.Dimension.EnterDimensions || !string.IsNullOrEmpty(saleLineItem.Dimension.VariantId)))
            {
                if ((!isEdit) && (!barcodeInfo.Found))
                {
                    return OpenItemDimensionsDialog(barcodeInfo);
                }

                return true;
            }
            else
            {
                return true;
            }
        }

        private void AddItem(StockCountItem item)
        {
            DataRow[] rows = entryTable.Select(string.Format("ITEMNUMBER = '{0}'", item.ItemNumber));
            bool isItemVarinatExists = false;
            DataRow selectedRow = GetCurrentRow();
            if (isEdit && selectedRow != null)
            {
                // Edit Quantity of an existing row
                selectedRow[DataAccessConstants.Quantity] = item.Quantity;
                selectedRow[DataAccessConstants.JournalId] = this.SelectedJournalId;
            }
            else
            {
                if (rows != null && rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        if (row[DataAccessConstants.InventSizeId].ToString() == (saleLineItem.Dimension.SizeId ?? string.Empty)
                            && row[DataAccessConstants.InventColorId].ToString() == (saleLineItem.Dimension.ColorId ?? string.Empty)
                            && row[DataAccessConstants.InventStyleId].ToString() == (saleLineItem.Dimension.StyleId ?? string.Empty)
                            && row[DataAccessConstants.ConfigId].ToString() == (saleLineItem.Dimension.ConfigId ?? string.Empty))
                        {
                            row[DataAccessConstants.JournalId] = this.SelectedJournalId;
                            // Increment Quantity of an existing row
                            decimal oldQuantity = (decimal)row[DataAccessConstants.Quantity];
                            row[DataAccessConstants.Quantity] = oldQuantity + item.Quantity;
                            row[DataAccessConstants.UserId] = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                            row[DataAccessConstants.TerminalId] = ApplicationSettings.Terminal.TerminalId;
                            row[DataAccessConstants.CountDate] = DateTime.Now;
                            row[DataAccessConstants.InventSizeId] = saleLineItem.Dimension.SizeId ?? string.Empty;
                            row[DataAccessConstants.InventColorId] = saleLineItem.Dimension.ColorId ?? string.Empty;
                            row[DataAccessConstants.InventStyleId] = saleLineItem.Dimension.StyleId ?? string.Empty;
                            row[DataAccessConstants.ConfigId] = saleLineItem.Dimension.ConfigId ?? string.Empty;
                            isItemVarinatExists = true;
                            break;
                        }
                    }

                    if (!isItemVarinatExists)
                    {
                        AddNewRow(item);
                    }
                }
                else
                {
                    AddNewRow(item);
                }
            }

            ResetNumpad();
        }

        /// <summary>
        /// This method Adds the variant information.
        /// </summary>
        /// <param name="countTableComment"></param>
        /// <returns></returns>
        private DataTable AddVariantInfo(DataTable countTableComment)
        {
            DataColumn col = new DataColumn("COMMENT", typeof(string));
            countTableComment.Columns.Add(col);
            saleLineItem = (SaleLineItem)StockCount.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                    ApplicationSettings.Terminal.StoreCurrency,
                    StockCount.InternalApplication.Services.Rounding,
                    posTransaction);
            foreach (DataRow row in countTableComment.Rows)
            {
                saleLineItem.Dimension.ColorName = row["COLOR"] as string;
                saleLineItem.Dimension.SizeName = row["SIZE"] as string;
                saleLineItem.Dimension.StyleName = row["STYLE"] as string;
                saleLineItem.Dimension.ConfigName = row["CONFIG"] as string;
                row["COMMENT"] = ColorSizeStyleConfig(saleLineItem);
            }

            return countTableComment;
        }

        private void ResetNumpad()
        {
            saleLineItem = null;
            numPad1.ClearValue();
        }

        /// <summary>
        /// Return the data-row for the currently selected/focused row in the grid
        /// </summary>
        /// <returns>DataRow if it exists, null otherwise</returns>
        private DataRow GetCurrentRow()
        {
            ColumnView view = (ColumnView)grInventory.MainView;
            if (view != null)
            {
                return view.GetFocusedDataRow();
            }

            return null;
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // commit local stock counts to AX via webservice
                SaveStockCounts();

                LoadStockCountsFromDB();

                // Commit local stock counts to AX via webservice
                ISCJournal scJournal = StockCount.InternalApplication.Services.StoreInventoryServices.CommitStockCounts(this.SelectedJournalId, this.SelectedRecordId);

                // Remove rows that are successfully submitted
                List<DataRow> removeRows = new List<DataRow>();

                if (scJournal != null)
                {
                    foreach (DataRow row in entryTable.Rows)
                    {
                        ISCJournalTransaction updatedLine = scJournal.SCJournalLines.Where(line => string.Equals(line.Guid, row["GUID"].ToString(), StringComparison.OrdinalIgnoreCase)
                            && line.UpdatedInAx == true).FirstOrDefault();

                        if (updatedLine != null)
                        {
                            removeRows.Add(row);
                        }
                    }
                }

                // If lines were submitted successfully
                if (removeRows.Count > 0)
                {
                    foreach (DataRow row in removeRows)
                    {
                        // Remove line from local DB
                        scData.DeleteStockCount(row["GUID"].ToString());

                        // Remove line from form
                        entryTable.Rows.Remove(row);
                        entryTable.AcceptChanges();
                    }

                    if (removeRows.Count == scJournal.SCJournalLines.Count)
                    {
                        // Delete header
                        scData.DeleteJournal(scJournal.RecId);

                        // Show commit succeeded message
                        using (frmMessage dialog = new frmMessage(10314011, MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            POSFormsManager.ShowPOSForm(dialog);
                            if (dialog.DialogResult == DialogResult.OK)
                            {
                                this.DialogResult = DialogResult.OK;
                                Close();
                            }
                        }
                    }
                    else
                    {
                        // Show partial commit success message
                        using (frmMessage dialog = new frmMessage(10314012, MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            POSFormsManager.ShowPOSForm(dialog);
                        }
                        grInventory.DataSource = entryTable;
                        entryTable.AcceptChanges();
                    }
                }
                else
                {
                    // Show commit failure message
                    using (frmMessage dialog = new frmMessage(10314013, MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        POSFormsManager.ShowPOSForm(dialog);
                    }
                    grInventory.DataSource = entryTable;
                    entryTable.AcceptChanges();
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void GetStockCountLinesFromAX()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                entryTable.Rows.Clear();

                IList<ISCJournalTransaction> scLines = StockCount.InternalApplication.Services.StoreInventoryServices.GetStockCountJournalTransactions(this.SelectedJournalId);

                if (scLines != null)
                {
                    // Append lines to grid control
                    foreach (ISCJournalTransaction newLine in scLines)
                    {
                        DataRow row = entryTable.NewRow();
                        entryTable.Rows.Add(row);
                        UpdateRowWithStockCountLine(row, newLine);
                    }
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GetStockCountLinesFromAX();
            entryTable.AcceptChanges();

            this.SaveStockCounts();
            this.LoadStockCounts();
        }

        private void UpdateRowWithStockCountLine(DataRow row, ISCJournalTransaction line)
        {
            // Update columns that are included in ISCJournalTransaction
            row[DataAccessConstants.ItemNumber] = line.ItemId;
            row[DataAccessConstants.ItemName] = line.ItemName;
            row[DataAccessConstants.Counted] = line.Counted;
            row[DataAccessConstants.JournalId] = this.SelectedJournalId;
            row[DataAccessConstants.RecId] = line.RecId;

            // Always set to zero because only delta will be shown and submitted
            row[DataAccessConstants.Quantity] = 0;

            row[DataAccessConstants.ConfigId] = line.ConfigId;
            row[DataAccessConstants.InventSizeId] = line.InventSizeId;
            row[DataAccessConstants.InventColorId] = line.InventColorId;
            row[DataAccessConstants.InventStyleId] = line.InventStyleId;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                SaveStockCounts();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            // set quantity on an existing line
            DataRow row = GetCurrentRow();
            numPad1.Focus();

            if (row != null)
            {
                isEdit = true;
                string itemNumber = (string)row[DataAccessConstants.ItemNumber];
                InventoryLookup(itemNumber);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            // Remove the current row from the grid
            DataRow row = GetCurrentRow();

            if (row != null)
            {
                row.Delete();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gvInventory.MoveNext();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gvInventory.MovePrev();
        }

        /// <summary>
        /// This method formats the Variant information 
        /// </summary>
        /// <param name="saleItem"></param>
        /// <returns></returns>
        private static string ColorSizeStyleConfig(SaleLineItem saleItem)
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig = new StringBuilder(saleItem.Dimension.ColorName);

            if (!string.IsNullOrEmpty(saleItem.Dimension.SizeName))
            {
                if (colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(saleItem.Dimension.SizeName);
            }

            if (!string.IsNullOrEmpty(saleItem.Dimension.StyleName))
            {
                if (colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(saleItem.Dimension.StyleName);
            }

            if (!string.IsNullOrEmpty(saleItem.Dimension.ConfigName))
            {
                if (colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(saleItem.Dimension.ConfigName);
            }

            return colorSizeStyleConfig.ToString();
        }

        /// <summary>
        /// This method Opens Dimesions Screen.
        /// </summary>
        /// <param name="barcodeInfo"></param>
        private bool OpenItemDimensionsDialog(IBarcodeInfo barcodeInfo)
        {
            bool returnValue = false;
            DataTable inventDimCombination = StockCount.InternalApplication.Services.Dimension.GetDimensions(saleLineItem.ItemId);

            // Get the dimensions
            DimensionConfirmation dimensionConfirmation = new DimensionConfirmation()
            {
                InventDimCombination = inventDimCombination,
                DimensionData = saleLineItem.Dimension,
                DisplayDialog = !barcodeInfo.Found
            };

            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(dimensionConfirmation, () =>
            {
                if (dimensionConfirmation.Confirmed)
                {
                    if (dimensionConfirmation.SelectDimCombination != null)
                    {
                        DataRow dr = dimensionConfirmation.SelectDimCombination;
                        saleLineItem.Dimension.VariantId = dr.Field<string>("VARIANTID");
                        saleLineItem.Dimension.ColorId = dr.Field<string>("COLORID");
                        saleLineItem.Dimension.ColorName = dr.Field<string>("COLOR");
                        saleLineItem.Dimension.SizeId = dr.Field<string>("SIZEID");
                        saleLineItem.Dimension.SizeName = dr.Field<string>("SIZE");
                        saleLineItem.Dimension.StyleId = dr.Field<string>("STYLEID");
                        saleLineItem.Dimension.StyleName = dr.Field<string>("STYLE");
                        saleLineItem.Dimension.ConfigId = dr.Field<string>(DataAccessConstants.ConfigId);
                        saleLineItem.Dimension.ConfigName = dr.Field<string>("CONFIG");
                        saleLineItem.Dimension.DistinctProductVariantId = (Int64)dr["DISTINCTPRODUCTVARIANT"];

                        if (string.IsNullOrEmpty(saleLineItem.BarcodeId))
                        {   // Pick up if not previously set
                            saleLineItem.BarcodeId = dr.Field<string>("ITEMBARCODE");
                        }

                        string unitId = dr.Field<string>("UNITID");
                        if (!String.IsNullOrEmpty(unitId))
                        {
                            saleLineItem.SalesOrderUnitOfMeasure = unitId;
                        }
                    }
                    returnValue = true;
                }
            }
            );

            StockCount.InternalApplication.Services.Interaction.InteractionRequest(request);

            return returnValue;
        }


        private void AddNewRow(StockCountItem item)
        {
            // add a new row
            DataRow row = entryTable.NewRow();
            row[DataAccessConstants.JournalId] = this.SelectedJournalId;

            row[DataAccessConstants.OperationType] = 0;
            row[DataAccessConstants.ItemNumber] = item.ItemNumber;
            row[DataAccessConstants.ItemName] = item.ItemName;
            row[DataAccessConstants.Counted] = 0;
            row[DataAccessConstants.Quantity] = item.Quantity;
            row[DataAccessConstants.Unit] = item.Unit;
            row[DataAccessConstants.UserId] = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
            row[DataAccessConstants.TerminalId] = ApplicationSettings.Terminal.TerminalId;
            row[DataAccessConstants.CountDate] = DateTime.Now;
            row[DataAccessConstants.Status] = (int)StockCountStatus.Pending;
            row[DataAccessConstants.InventSizeId] = saleLineItem.Dimension.SizeId ?? string.Empty;
            row[DataAccessConstants.InventColorId] = saleLineItem.Dimension.ColorId ?? string.Empty;
            row[DataAccessConstants.InventStyleId] = saleLineItem.Dimension.StyleId ?? string.Empty;
            row[DataAccessConstants.ConfigId] = saleLineItem.Dimension.ConfigId ?? string.Empty;
            row["COMMENT"] = ColorSizeStyleConfig(saleLineItem);

            try
            {
                entryTable.Rows.Add(row);
            }
            catch (ArgumentException ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            }
            catch (NoNullAllowedException ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            }
            catch (ConstraintException ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            }
        }

    }
}
