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
using DevExpress.XtraGrid.Views.Grid;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Too many form framework types.")]
    public partial class frmPurchaseOrderReceiving : LSRetailPosis.POSProcesses.frmTouchBase
    {
        private PosTransaction posTransaction;
        private DataTable headerTable;
        private DataTable entryTable;
        private SaleLineItem saleLineItem;
        private NumPadMode inputMode;
        private PurchaseOrderReceiptData receiptData;
        private bool isEdit; // = false;
        private PRCountingType prType;
        private bool isMixedDeliveryMode = false;

        /// <summary>
        /// Mode of the numberpad:  Barcode entry, or Quantity entry
        /// </summary>
        private enum NumPadMode
        {
            Barcode = 0,
            Quantity = 1
        }

        /// <summary>
        /// Simple container for PO Receipt line details
        /// </summary>
        private struct EntryItem
        {
            public string ItemNumber;
            public string ItemName;
            public decimal QuantityReceived;
            public string Unit;
        }

        /// <summary>
        /// Get/Set the receipt number for the receipt to be shown
        /// </summary>
        public string ReceiptNumber { get; set; }

        /// <summary>
        /// Get/set the receipt type for the receipt to b shown
        /// </summary>
        public PRCountingType PRType { get; set; }

        /// <summary>
        /// Get/Set the purchase Id for the receipt to be shown
        /// </summary>
        public string PONumber { get; set; }

        /// <summary>
        /// Sets Pos transaction to value.
        /// </summary>
        public PosTransaction PosTransaction
        {
            set { posTransaction = value; }
        }

        /// <summary>
        /// Displays purchase order receiving form.
        /// </summary>
        public frmPurchaseOrderReceiving()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                receiptData = new PurchaseOrderReceiptData(
                    ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID,
                    ApplicationSettings.Terminal.StorePrimaryId);

                posTransaction = (PosTransaction)PurchaseOrderReceiving.InternalApplication.BusinessLogic.Utility.CreateSalesOrderTransaction(
                    ApplicationSettings.Terminal.StoreId,
                    ApplicationSettings.Terminal.StoreCurrency,
                    ApplicationSettings.Terminal.TaxIncludedInPrice,
                    PurchaseOrderReceiving.InternalApplication.Services.Rounding);

                ClearForm();

                PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(OnBarcodeScan);
                PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent += new ScannerMessageEventHandler(OnBarcodeScan);
                PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();
            }

            LoadReceiptLines();
            TranslateLabels();
            base.OnLoad(e);

            if (prType == PRCountingType.PurchaseOrder)
            {
                this.btnUom.Visible = false;
            }

            this.gvInventory.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.GridView_RowClickEventHandler);
        }		

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmPurchaseOrderReceiving are reserved at 103140 - 103159
            // In use now are ID's: 103140 - 103157
            //

            colItemNumber.Caption = ApplicationLocalizer.Language.Translate(103146); //Item
            colOrdered.Caption = ApplicationLocalizer.Language.Translate(103147); //Ordered
            colReceived.Caption = ApplicationLocalizer.Language.Translate(103148); //Received
            colReceivedNow.Caption = ApplicationLocalizer.Language.Translate(1031482); //Received Now
            this.Text = lblHeader.Text = ApplicationLocalizer.Language.Translate(1031202); //Picking/Receiving
            switch (prType)
            {
                case PRCountingType.PurchaseOrder:
                    colReceived.Caption = ApplicationLocalizer.Language.Translate(103148); //Received
                    colReceivedNow.Caption = ApplicationLocalizer.Language.Translate(1031482); //Received Now
                    this.Text = lblHeader.Text = ApplicationLocalizer.Language.Translate(103140); //Receiving 
                    btnReceiveAll.Text = ApplicationLocalizer.Language.Translate(103118); //Receive All
                    break;
                case PRCountingType.TransferIn:
                    colReceived.Caption = ApplicationLocalizer.Language.Translate(103148); //Received
                    colReceivedNow.Caption = ApplicationLocalizer.Language.Translate(1031482); //Received Now
                    this.Text = lblHeader.Text = ApplicationLocalizer.Language.Translate(1031402); //Transfer Order Receiving 
                    btnReceiveAll.Text = ApplicationLocalizer.Language.Translate(103118); //Receive All
                    break;
                case PRCountingType.TransferOut:
                    colReceived.Caption = ApplicationLocalizer.Language.Translate(1031481); //Picked
                    colReceivedNow.Caption = ApplicationLocalizer.Language.Translate(1031483); //Picked Now
                    this.Text = lblHeader.Text = ApplicationLocalizer.Language.Translate(1031401); //Transfer Order Picking 
                    this.btnReceiveAll.Text = ApplicationLocalizer.Language.Translate(1031181); // Pick All
                    break;
                default:
                    break;
            }

            colUnit.Caption = ApplicationLocalizer.Language.Translate(103149); //UoM
            numPad1.PromptText = ApplicationLocalizer.Language.Translate(103154); //Scan or enter barcode
            lblReceiptNumberHeading.Text = ApplicationLocalizer.Language.Translate(103141); //Picking/Receiving no.
            lblPoNumberHeading.Text = ApplicationLocalizer.Language.Translate(103142); //Order number
            lblDriverHeading.Text = ApplicationLocalizer.Language.Translate(103143); //Driver details
            lblDeliveryHeading.Text = ApplicationLocalizer.Language.Translate(103144); //Delivery note number
            lblDeliveryMethod.Text = ApplicationLocalizer.Language.Translate(56362); //Delivery method
            btnClose.Text = ApplicationLocalizer.Language.Translate(103153); //Close
            btnSearch.Text = ApplicationLocalizer.Language.Translate(103152); //Search
            btnEdit.Text = ApplicationLocalizer.Language.Translate(103106); //Edit quantity
            btnSave.Text = ApplicationLocalizer.Language.Translate(103117); //Save
            btnRefresh.Text = ApplicationLocalizer.Language.Translate(103116); //Refresh
            btnUom.Text = ApplicationLocalizer.Language.Translate(103113); //Edit unit of measure
            btnCommit.Text = ApplicationLocalizer.Language.Translate(103107); //Commit
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
        }

        void OnBarcodeScan(IScanInfo scanInfo)
        {
            InventoryLookup(scanInfo.ScanDataLabel);
        }

        private void ClearForm()
        {
            this.saleLineItem = null;

            this.txtReceiptNumber.Text = string.Empty;

            if (this.entryTable != null)
            {
                this.entryTable.Clear();
            }
            this.grInventory.DataSource = this.entryTable;

            this.numPad1.ClearValue();
        }

        private void LoadReceiptLinesFromDB()
        {
            this.entryTable = receiptData.GetPurchaseOrderReceiptLines(this.PONumber, this.ReceiptNumber);
            this.entryTable = AddVariantInfo(this.entryTable);
        }

        private void LoadReceiptHeadersFromDB()
        {
            this.headerTable = receiptData.GetPurchaseOrderReceipt(this.PONumber, this.ReceiptNumber);
        }

        private void LoadReceiptLines()
        {
            LoadReceiptHeadersFromDB();
            LoadReceiptLinesFromDB();

            if (entryTable != null && entryTable.Rows.Count == 0)
            {
                GetReceiptLinesFromAX();
                SaveReceipt();
                LoadReceiptHeadersFromDB();
                LoadReceiptLinesFromDB();
            }

            // Reset RowState for the first time of bringing lines from AX into an empty grid, 
            // or from DB, therefore it will NOT trigger the prompt of saving changes before exiting the form.
            this.entryTable.AcceptChanges();

            this.txtReceiptNumber.Text = headerTable.Rows[0].Field<string>(DataAccessConstants.ReceiptNumber);
            this.txtPoNumber.Text = headerTable.Rows[0].Field<string>(DataAccessConstants.PoNumber);
            this.txtDriver.Text = headerTable.Rows[0].Field<string>(DataAccessConstants.DriverDetails);
            this.txtDelivery.Text = headerTable.Rows[0].Field<string>(DataAccessConstants.DeliveryNoteNumber);
            this.txtDeliveryMethod.Text = headerTable.Rows[0].Field<string>(DataAccessConstants.DeliveryMethod);
            this.isMixedDeliveryMode = string.IsNullOrEmpty(this.txtDeliveryMethod.Text);
            this.prType = headerTable.Rows[0].Field<PRCountingType>(DataAccessConstants.OrderType);

            this.grInventory.DataSource = this.entryTable;
        }

        private void SetInputMode(NumPadMode mode)
        {
            if (mode == NumPadMode.Barcode)
            {
                this.isEdit = false;
                PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();
                numPad1.EntryType = NumpadEntryTypes.Barcode;
                numPad1.PromptText = ApplicationLocalizer.Language.Translate(103154);               //Scan or enter barcode
            }
            else if (mode == NumPadMode.Quantity)
            {
                PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
                numPad1.EntryType = NumpadEntryTypes.Quantity;
                numPad1.PromptText = ApplicationLocalizer.Language.Translate(103155);               //Scan or enter barcode
            }
            inputMode = mode;
        }

        private void ResetNumpad()
        {
            this.saleLineItem = null;
            numPad1.ClearValue();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (entryTable.GetChanges() != null)
            {
                POSFormsManager.ShowPOSMessageDialog(103119);
                return;
            }
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void frmPurchaseOrderReceiving_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void SaveReceipt()
        {
            string driver = headerTable.Rows[0].Field<string>(DataAccessConstants.DriverDetails);
            string delivery = headerTable.Rows[0].Field<string>(DataAccessConstants.DeliveryNoteNumber);

            if (!txtDriver.Text.Equals(driver))
            {
                headerTable.Rows[0][DataAccessConstants.DriverDetails] = txtDriver.Text;
            }
            if (!txtDelivery.Text.Equals(delivery))
            {
                headerTable.Rows[0][DataAccessConstants.DeliveryNoteNumber] = txtDelivery.Text;
            }

            try
            {
                this.UseWaitCursor = true;
                receiptData.SaveReceipt(headerTable, entryTable);
            }
            finally
            {
                this.UseWaitCursor = false;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                string selectedItemId = string.Empty;
                SetInputMode(NumPadMode.Barcode);

                // Show the search dialog through the item service
                if (!PurchaseOrderReceiving.InternalApplication.Services.Item.ItemSearch(ref selectedItemId, 500))
                {
                    return;
                }

                ItemData itemData = new ItemData(
                    ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID,
                    ApplicationSettings.Terminal.StorePrimaryId);

                numPad1.EnteredValue = itemData.GetBarcodeForItem(selectedItemId);

                if (string.IsNullOrEmpty(numPad1.EnteredValue))
                {
                    numPad1.EnteredValue = selectedItemId;
                }
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


        private void btnEdit_Click(object sender, EventArgs e)
        {
            // set quantity on an existing line
            DataRow row = GetCurrentRow();
            numPad1.Focus();

            if (row != null)
            {
                this.isEdit = true;
                string itemNumber = row.Field<string>(DataAccessConstants.ItemNumber);
                this.InventoryLookup(itemNumber);
            }
        }

        private void btnSave_Click(object sender, EventArgs s)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                SaveReceipt();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void GetReceiptLinesFromAX()
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                UnitOfMeasureData unitOfMeasureData = new UnitOfMeasureData(
                    PurchaseOrderReceiving.InternalApplication.Settings.Database.Connection,
                    PurchaseOrderReceiving.InternalApplication.Settings.Database.DataAreaID,
                    LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId,
                    PurchaseOrderReceiving.InternalApplication);

                // Purge all rows on grid
                entryTable.Rows.Clear();

                IList<IPRDocumentLine> poLines = PurchaseOrderReceiving.InternalApplication.Services.StoreInventoryServices.GetOrderReceiptLines(this.PONumber, this.PRType);

                // Append lines to grid control
                foreach (IPRDocumentLine newLine in poLines)
                {
                    DataRow row = entryTable.NewRow();
                    entryTable.Rows.Add(row);
                    UpdateRowWithPurchLine(row, newLine, unitOfMeasureData);
                }
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void btnRefresh_Click(object sender, EventArgs s)
        {
            GetReceiptLinesFromAX();
            entryTable.AcceptChanges();

            this.SaveReceipt();
            LoadReceiptLines();
        }

        private void btnReceiveAll_Click(object sender, EventArgs e)
        {
            numPad1.ClearValue();

            foreach (DataRow row in entryTable.Rows)
            {
                string recId = row.Field<string>(DataAccessConstants.LineReceiptNumber);

                if (!string.IsNullOrWhiteSpace(recId))
                {
                    decimal quantity = row.Field<decimal>(DataAccessConstants.QuantityOrdered) - row.Field<decimal>(DataAccessConstants.QuantityReceived);
                    if (quantity < 0)
                    {
                        quantity = 0;
                    }

                    row[DataAccessConstants.QuantityReceivedNow] = quantity;
                }
            }

            grInventory.DataSource = entryTable;
        }

        private static void UpdateRowWithPurchLine(DataRow row, IPRDocumentLine line, UnitOfMeasureData unitOfMeasureData)
        {
            int unitDecimals = unitOfMeasureData.GetUnitDecimals(line.PurchUnit);

            // Update columns that are included in IPRDocumentLine
            row[DataAccessConstants.LineReceiptNumber] = line.RecId;
            row[DataAccessConstants.ItemNumber] = line.ItemId;
            row[DataAccessConstants.ItemName] = line.ItemName;
            // If the item is a service, qtyOrdered is always 0.
            decimal quantity = line.QtyOrdered == 0 ? line.PurchQty : line.QtyOrdered;
            row[DataAccessConstants.QuantityOrdered] = PurchaseOrderReceiving.InternalApplication.Services.Rounding.Round(quantity, unitDecimals);
            row[DataAccessConstants.QuantityReceived] = PurchaseOrderReceiving.InternalApplication.Services.Rounding.Round(line.PurchReceived, unitDecimals);

            if (row.RowState == DataRowState.Added)
            {
                // Always set to zero because only delta will be shown and submitted
                row[DataAccessConstants.QuantityReceivedNow] = PurchaseOrderReceiving.InternalApplication.Services.Rounding.Round(line.PurchReceivedNow, unitDecimals);
            }

            row[DataAccessConstants.UnitOfMeasure] = line.PurchUnit;
            row[DataAccessConstants.ConfigId] = line.ConfigId;
            row[DataAccessConstants.InventSizeId] = line.InventSizeId;
            row[DataAccessConstants.InventColorId] = line.InventColorId;
            row[DataAccessConstants.InventStyleId] = line.InventStyleId;
            row[DataAccessConstants.DeliveryMethod] = line.DeliveryMethod;

            // Does not assign GUID. GUID cannot be used as PK because it will change every time on AX side
        }

        private void btnUom_Click(object sender, EventArgs e)
        {
            OpenUnitOfMeasureDialog();
        }

        private bool GetItemInfo(string barcode, bool skipDimensionDialog)
        {
            if (string.IsNullOrEmpty(barcode))
            {
                return false;
            }

            this.saleLineItem = (SaleLineItem)PurchaseOrderReceiving.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                ApplicationSettings.Terminal.StoreCurrency,
                PurchaseOrderReceiving.InternalApplication.Services.Rounding,
                posTransaction);

            IScanInfo scanInfo = PurchaseOrderReceiving.InternalApplication.BusinessLogic.Utility.CreateScanInfo();
            scanInfo.ScanDataLabel = barcode;
            scanInfo.EntryType = BarcodeEntryType.ManuallyEntered;

            IBarcodeInfo barcodeInfo = PurchaseOrderReceiving.InternalApplication.Services.Barcode.ProcessBarcode(scanInfo);

            if ((barcodeInfo.InternalType == BarcodeInternalType.Item) && (barcodeInfo.ItemId != null))
            {
                // The entry was a barcode which was found and now we have the item id...
                this.saleLineItem.ItemId = barcodeInfo.ItemId;
                this.saleLineItem.BarcodeId = barcodeInfo.BarcodeId;
                this.saleLineItem.SalesOrderUnitOfMeasure = barcodeInfo.UnitId;
                this.saleLineItem.EntryType = barcodeInfo.EntryType;
                this.saleLineItem.Dimension.ColorId = barcodeInfo.InventColorId;
                this.saleLineItem.Dimension.SizeId = barcodeInfo.InventSizeId;
                this.saleLineItem.Dimension.StyleId = barcodeInfo.InventStyleId;
                this.saleLineItem.Dimension.VariantId = barcodeInfo.VariantId;
            }
            else
            {
                // It could be an ItemId
                this.saleLineItem.ItemId = barcodeInfo.BarcodeId;
                this.saleLineItem.EntryType = barcodeInfo.EntryType;
            }

            // fetch all the addtional item properties
            PurchaseOrderReceiving.InternalApplication.Services.Item.ProcessItem(saleLineItem, true);

            if (saleLineItem.Found == false)
            {
                POSFormsManager.ShowPOSMessageDialog(2611);             // Item not found.
                return false;
            }
            else if ((saleLineItem.Dimension != null)
                && (saleLineItem.Dimension.EnterDimensions || !string.IsNullOrEmpty(saleLineItem.Dimension.VariantId))
                && !skipDimensionDialog)
            {
                if (!barcodeInfo.Found)
                {
                    return OpenItemDimensionsDialog(barcodeInfo);
                }
            }

            return true;
        }

        private void InventoryLookup(string barcode)
        {
            if (inputMode == NumPadMode.Barcode)
            {
                if (GetItemInfo(barcode, this.isEdit) && (this.isEdit || this.ValidateItemIdAndVariantsForPicking(this.saleLineItem)))
                {
                    this.SetQuantitiesFromRow(this.saleLineItem);

                    // Set mode to quantity
                    SetInputMode(NumPadMode.Quantity);
                }
                else
                {
                    // Clear the current item so the user can try again
                    ResetNumpad();
                }
            }
            else if ((inputMode == NumPadMode.Quantity) && (saleLineItem != null) && !string.IsNullOrEmpty(numPad1.EnteredValue))
            {
                // Add to list
                EntryItem item = new EntryItem()
                {
                    ItemNumber = this.saleLineItem.ItemId,
                    Unit = this.saleLineItem.InventOrderUnitOfMeasure,
                    ItemName = this.saleLineItem.Description,
                    QuantityReceived = decimal.Parse(numPad1.EnteredValue)
                };

                if (this.ValidateData(item, this.saleLineItem))
                {
                    AddItem(item);
                    SetInputMode(NumPadMode.Barcode);
                }
                else
                {
                    // Clear the current quantity so the user can try again
                    numPad1.ClearValue();
                    SetInputMode(NumPadMode.Quantity);
                }
            }
            numPad1.Select();
        }

        private void AddItem(EntryItem item)
        {
            DataRow rowInEdit = this.GetRowInEdit(this.saleLineItem);
            if (this.isEdit && rowInEdit != null)
            {
                // Edit QuantityReceived of an existing row
                rowInEdit[DataAccessConstants.QuantityReceivedNow] = item.QuantityReceived;
            }
            else
            {
                if (rowInEdit != null)
                {
                    // increment an existing row
                    decimal oldQuantity = rowInEdit.Field<decimal>(DataAccessConstants.QuantityReceivedNow);
                    rowInEdit[DataAccessConstants.QuantityReceivedNow] = oldQuantity + item.QuantityReceived;
                    rowInEdit[DataAccessConstants.ReceiptDate] = DateTime.Now;
                    rowInEdit[DataAccessConstants.UserId] = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                    rowInEdit[DataAccessConstants.TerminalId] = ApplicationSettings.Terminal.TerminalId;
                    rowInEdit[DataAccessConstants.InventSizeId] = this.saleLineItem.Dimension.SizeId ?? string.Empty;
                    rowInEdit[DataAccessConstants.InventColorId] = this.saleLineItem.Dimension.ColorId ?? string.Empty;
                    rowInEdit[DataAccessConstants.InventStyleId] = this.saleLineItem.Dimension.StyleId ?? string.Empty;
                    rowInEdit[DataAccessConstants.ConfigId] = this.saleLineItem.Dimension.ConfigId ?? string.Empty;
                }
                else
                {
                    AddNewRow(item);
                }
            }

            ResetNumpad();
        }

        private static bool IsSameVariants(DataRow row, Dimensions dimension)
        {
            return row[DataAccessConstants.InventSizeId].ToString() == (dimension.SizeId ?? string.Empty)
                && row[DataAccessConstants.InventColorId].ToString() == (dimension.ColorId ?? string.Empty)
                && row[DataAccessConstants.InventStyleId].ToString() == (dimension.StyleId ?? string.Empty)
                && row[DataAccessConstants.ConfigId].ToString() == (dimension.ConfigId ?? string.Empty);
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

        private DataRow GetRowInEdit(ISaleLineItem lineItem)
        {
            DataRow row = null;

            if (this.isEdit)
            {
                row = this.GetCurrentRow();
            }
            else
            {
                row = this.GetRowByItemIdAndVariants(lineItem);
            }

            return row;
        }

        private DataRow GetRowByItemIdAndVariants(ISaleLineItem lineItem)
        {
            DataRow[] rows = this.entryTable.Select(string.Format("ITEMNUMBER = '{0}'", lineItem.ItemId));

            DataRow ret = null;
            if (rows != null && rows.Length > 0)
            {
                foreach (DataRow row in rows)
                {
                    if (IsSameVariants(row, ((SaleLineItem)lineItem).Dimension))
                    {
                        ret = row;
                        break;
                    }
                }
            }

            return ret;
        }

        private void SetQuantitiesFromRow(ISaleLineItem lineItem)
        {
            UnitOfMeasureData unitOfMeasureData = new UnitOfMeasureData(
                PurchaseOrderReceiving.InternalApplication.Settings.Database.Connection,
                PurchaseOrderReceiving.InternalApplication.Settings.Database.DataAreaID,
                LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId,
                PurchaseOrderReceiving.InternalApplication);

            DataRow row = this.GetRowInEdit(lineItem);

            if (row != null)
            {
                int unitDecimals = unitOfMeasureData.GetUnitDecimals(row.Field<string>(DataAccessConstants.UnitOfMeasure));

                lineItem.QuantityOrdered = PurchaseOrderReceiving.InternalApplication.Services.Rounding.Round(row.Field<decimal>(DataAccessConstants.QuantityOrdered), unitDecimals);
                lineItem.QuantityPickedUp = PurchaseOrderReceiving.InternalApplication.Services.Rounding.Round(row.Field<decimal>(DataAccessConstants.QuantityReceived), unitDecimals);
            }
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // Save lines to local database
                SaveReceipt();

                LoadReceiptLinesFromDB();

                // Commit receipt to AX via webservice
                IPRDocument prDoc = PurchaseOrderReceiving.InternalApplication.Services.StoreInventoryServices.CommitOrderReceipt(this.PONumber, this.ReceiptNumber, this.prType);

                // Remove rows that are successfully submitted
                List<DataRow> removeRows = new List<DataRow>();

                if (prDoc != null)
                {
                    foreach (DataRow row in entryTable.Rows)
                    {
                        IPRDocumentLine updatedLine = prDoc.PRDocumentLines.Where(line => string.Equals(line.Guid, row.Field<string>(DataAccessConstants.Guid), StringComparison.OrdinalIgnoreCase)
                            && line.UpdatedInAx == true).FirstOrDefault();

                        if (updatedLine != null)
                        {
                            removeRows.Add(row);
                        }
                    }
                }

                if (removeRows.Count > 0)
                {
                    foreach (DataRow row in removeRows)
                    {
                        // Remove line from local DB
                        receiptData.DeleteReceiptLine(row.Field<string>(DataAccessConstants.Guid));

                        // Remove line from form
                        entryTable.Rows.Remove(row);
                        entryTable.AcceptChanges();
                    }

                    if (removeRows.Count == prDoc.PRDocumentLines.Count)
                    {
                        // Delete header if all lines are removed
                        receiptData.DeleteReceipt(prDoc.RecId);

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

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            gvInventory.MoveNextPage();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gvInventory.MoveNext();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gvInventory.MovePrev();
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            gvInventory.MovePrevPage();
        }

        /// <summary>
        /// This method Opens Dimesions Screen.
        /// </summary>
        /// <param name="barcodeInfo"></param>
        private bool OpenItemDimensionsDialog(IBarcodeInfo barcodeInfo)
        {
            bool returnValue = false;
            // Get the dimensions
            DataTable inventDimCombination = PurchaseOrderReceiving.InternalApplication.Services.Dimension.GetDimensions(saleLineItem.ItemId);

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

            PurchaseOrderReceiving.InternalApplication.Services.Interaction.InteractionRequest(request);

            return returnValue;
        }

        private void OpenUnitOfMeasureDialog()
        {
            try
            {
                DataRow selectedRow = GetCurrentRow();
                string itemNumber = (string)selectedRow[DataAccessConstants.ItemNumber];
                string uom = (string)selectedRow[DataAccessConstants.UnitOfMeasure];

                SaleLineItem selectedItem = (SaleLineItem)PurchaseOrderReceiving.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem();

                selectedItem.ItemId = itemNumber;
                selectedItem.InventOrderUnitOfMeasure = uom;
                selectedItem.SalesOrderUnitOfMeasure = uom;

                if (frmUOMList.PromptAndChangeSalesUnitOfMeasure(selectedItem))
                {
                    selectedRow[DataAccessConstants.UnitOfMeasure] = selectedItem.SalesOrderUnitOfMeasure;
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// This method Adds the variant information.
        /// </summary>
        /// <param name="countTableComment"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private DataTable AddVariantInfo(DataTable countTableComment)
        {
            string commentColName = "COMMENT";

            DataColumn col = new DataColumn(commentColName, typeof(string));

            if (!countTableComment.Columns.Contains(commentColName))
            {
                countTableComment.Columns.Add(col);
            }

            this.saleLineItem = (SaleLineItem)PurchaseOrderReceiving.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                    ApplicationSettings.Terminal.StoreCurrency,
                    PurchaseOrderReceiving.InternalApplication.Services.Rounding,
                    posTransaction);
            foreach (DataRow row in countTableComment.Rows)
            {
                this.saleLineItem.Dimension.ColorName = row.Field<string>("COLOR");
                this.saleLineItem.Dimension.SizeName = row.Field<string>("SIZE");
                this.saleLineItem.Dimension.StyleName = row.Field<string>("STYLE");
                this.saleLineItem.Dimension.ConfigName = row.Field<string>("CONFIG");
                row[commentColName] = ColorSizeStyleConfig(saleLineItem.Dimension);
            }

            return countTableComment;
        }

        /// <summary>
        /// This method formats the Variant information 
        /// </summary>
        /// <param name="saleItem"></param>
        /// <returns></returns>
        private static string ColorSizeStyleConfig(Dimensions dimension)
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig = new StringBuilder(dimension.ColorName);

            if (!string.IsNullOrEmpty(dimension.SizeName))
            {
                if (colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(dimension.SizeName);
            }

            if (!string.IsNullOrEmpty(dimension.StyleName))
            {
                if (colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(dimension.StyleName);
            }

            if (!string.IsNullOrEmpty(dimension.ConfigName))
            {
                if (colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(dimension.ConfigName);
            }

            return colorSizeStyleConfig.ToString();
        }

        private bool ValidateData(EntryItem item, ISaleLineItem lineItem)
        {
            bool valid = true;

            // Check if the unit allows any decimals
            UnitOfMeasureData uomData = new UnitOfMeasureData(
                ApplicationSettings.Database.LocalConnection,
                ApplicationSettings.Database.DATAAREAID,
                ApplicationSettings.Terminal.StorePrimaryId,
                PurchaseOrderReceiving.InternalApplication);

            int unitDecimals = uomData.GetUnitDecimals(item.Unit);

            if ((unitDecimals == 0) && ((item.QuantityReceived - (int)item.QuantityReceived) != 0))
            {
                string msg = ApplicationLocalizer.Language.Translate(103157, (object)item.ItemNumber);

                using (frmMessage frm = new frmMessage(msg, MessageBoxButtons.OK, MessageBoxIcon.Warning))
                {
                    POSFormsManager.ShowPOSForm(frm);
                }
                valid = false;
            }

            if (this.prType == PRCountingType.TransferOut || this.prType == PRCountingType.PickingList)
            {
                decimal qtyReceived = item.QuantityReceived;
                if (!this.isEdit)
                {
                    DataRow row = this.GetRowByItemIdAndVariants(saleLineItem);
                    if (row != null)
                    {
                        qtyReceived += row.Field<decimal>(DataAccessConstants.QuantityReceivedNow);
                    }
                }

                if (qtyReceived + lineItem.QuantityPickedUp > lineItem.QuantityOrdered)
                {
                    using (frmMessage frm = new frmMessage(99521, MessageBoxButtons.OK, MessageBoxIcon.Warning))
                    {
                        POSFormsManager.ShowPOSForm(frm);
                    }
                    valid = false;
                }
            }

            return valid;
        }

        private bool ValidateItemIdAndVariantsForPicking(ISaleLineItem lineItem)
        {
            bool isValid = true;
            if (this.prType == PRCountingType.TransferOut || this.prType == PRCountingType.PickingList)
            {
                DataRow row = this.GetRowByItemIdAndVariants(lineItem);

                isValid = (row != null);

                if (!isValid)
                {
                    using (frmMessage frm = new frmMessage(99523, MessageBoxButtons.OK, MessageBoxIcon.Warning))
                    {
                        POSFormsManager.ShowPOSForm(frm);
                    }
                }
            }

            return isValid;
        }

        private void AddNewRow(EntryItem item)
        {
            // Add a new row
            DataRow row = entryTable.NewRow();

            row[DataAccessConstants.ItemNumber] = item.ItemNumber;
            row[DataAccessConstants.ItemName] = item.ItemName;
            row[DataAccessConstants.QuantityOrdered] = 0;
            row[DataAccessConstants.QuantityReceived] = 0;
            row[DataAccessConstants.QuantityReceivedNow] = item.QuantityReceived;
            row[DataAccessConstants.UnitOfMeasure] = item.Unit;
            row[DataAccessConstants.ReceiptDate] = DateTime.Now;
            row[DataAccessConstants.UserId] = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
            row[DataAccessConstants.TerminalId] = ApplicationSettings.Terminal.TerminalId;
            row[DataAccessConstants.InventSizeId] = this.saleLineItem.Dimension.SizeId ?? string.Empty;
            row[DataAccessConstants.InventColorId] = this.saleLineItem.Dimension.ColorId ?? string.Empty;
            row[DataAccessConstants.InventStyleId] = this.saleLineItem.Dimension.StyleId ?? string.Empty;
            row[DataAccessConstants.ConfigId] = this.saleLineItem.Dimension.ConfigId ?? string.Empty;
            row["COMMENT"] = ColorSizeStyleConfig(saleLineItem.Dimension);

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

        private void GridView_RowClickEventHandler(object sender, RowClickEventArgs e)
        {
            DataRow row = GetCurrentRow();
            numPad1.Focus();

            if (row != null)
            {
                string itemNumber = row.Field<string>(DataAccessConstants.ItemNumber);
                if (this.GetItemInfo(itemNumber, true))
                {
                    this.SetQuantitiesFromRow(this.saleLineItem);
                }
            }
        }

        private void gvInventory_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            if (this.isMixedDeliveryMode)
            {
                this.txtDeliveryMethod.Text = string.Empty;
                DataRow row = this.gvInventory.GetDataRow(e.FocusedRowHandle);

                if (row != null)
                {
                    string lineItemDeliveryMethod = row[DataAccessConstants.DeliveryMethod] as string;

                    if (lineItemDeliveryMethod != null)
                    {
                        this.txtDeliveryMethod.Text = lineItemDeliveryMethod;
                    }
                }
            }
        }
    }
}
