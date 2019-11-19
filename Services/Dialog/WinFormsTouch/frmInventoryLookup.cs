/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    partial class formInventoryLookup : frmTouchBase
    {
        private const string dimensionSeparator = " - ";
        private IPosTransaction posTransaction;
        private DataTable inventoryTable;
        private SaleLineItem saleLineItem;
        private string itemId;
        private Lazy<Dimensions> dimension = new Lazy<Dimensions>();

        /// <summary>
        /// Property to hide Lazy wrapper
        /// </summary>
        private Dimensions Dimension
        {
            get { return this.dimension.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="formInventoryLookup"/> class.
        /// </summary>
        /// <param name="posTransaction">The pos transaction.</param>
        public formInventoryLookup(IPosTransaction posTransaction)
            : this()
        {
            this.posTransaction = posTransaction;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="formInventoryLookup"/> class.
        /// </summary>
        /// <param name="itemOrVariantId">item id or variant id</param>
        public formInventoryLookup(string itemOrVariantId)
            : this()
        {
            // Store itemOrVariantId in dimension object as a variant to be determined later
            this.Dimension.VariantId = itemOrVariantId;
        }

        protected formInventoryLookup()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                numPad1.EntryType = Contracts.UI.NumpadEntryTypes.Barcode;
                numPad1.EnteredValue = string.Empty;

                TranslateLabels();

                Dialog.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(Scanner_ScannerMessageEvent);
                Dialog.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent += new ScannerMessageEventHandler(Scanner_ScannerMessageEvent);
                Dialog.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();

                // Create the inventory table....
                CreateInventoryTable();

                ClearForm();

                if (posTransaction == null)
                {
                    btnSearch.Enabled = btnAddToTransaction.Enabled = false;

                    LSRetailPosis.DataAccess.ItemData itemData = new LSRetailPosis.DataAccess.ItemData(
                    ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, ApplicationSettings.Terminal.StorePrimaryId);

                    // Determine if ID passed in is an item ID or variant ID, update itemId var accordingly
                    string foundItemId = itemData.DoesVariantExist(this.Dimension.VariantId);

                    if (foundItemId == null)
                    {
                        // We didn't find an item based on the variant ID passed in which means the ID was already an item ID; assign and clear
                        this.itemId = this.Dimension.VariantId;
                        this.Dimension.VariantId = null;
                    }
                    else
                    {
                        this.itemId = foundItemId;
                    }

                    numPad1.EnteredValue = itemId;
                    InventoryLookup(numPad1.EnteredValue);
                }
            }

            base.OnLoad(e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Ideally after OnShown(), UI should have be been rendered, but it is not.
            // Adding explicit update call to render UI and then perform IO operation.
            Update();
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!this.DesignMode)
            {
                Dialog.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(Scanner_ScannerMessageEvent);
                Dialog.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
            }

            base.OnClosed(e);
        }

        void Scanner_ScannerMessageEvent(IScanInfo scanInfo)
        {
            InventoryLookup(scanInfo.ScanDataLabel);
        }

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmInventoryLookup are reserved at 2600 - 2649
            // In use now are ID's: 2600 - 2611
            //

            this.Text = lblHeader.Text = ApplicationLocalizer.Language.Translate(2600);     // Inventory Lookup
            numPad1.PromptText = ApplicationLocalizer.Language.Translate(2601);     // Scan or enter barcode
            lblItemHeading.Text = ApplicationLocalizer.Language.Translate(2602);     // Item
            lblInventoryHeading.Text = ApplicationLocalizer.Language.Translate(2603);     // Inventory
            lblItemIdHeading.Text = ApplicationLocalizer.Language.Translate(2604);     // Item id
            btnClose.Text = ApplicationLocalizer.Language.Translate(2605);     // Close
            btnSearch.Text = ApplicationLocalizer.Language.Translate(2607);     // Search
            colInventory.Caption = string.Format(ApplicationLocalizer.Language.Translate(2613), string.Empty); // Inventory ({0}), {0} = unit
            colStore.Caption = ApplicationLocalizer.Language.Translate(2608);     // Store
            btnAddToTransaction.Text = ApplicationLocalizer.Language.Translate(2615);     // Add to transaction
        }

        private void CreateInventoryTable()
        {
            inventoryTable = new DataTable();
            inventoryTable.Columns.Add(new DataColumn("ITEMID", typeof(string)));
            inventoryTable.Columns.Add(new DataColumn("INVENTLOCATIONID", typeof(string)));
            inventoryTable.Columns.Add(new DataColumn("STORENAME", typeof(string)));
            inventoryTable.Columns.Add(new DataColumn("INVENTORY", typeof(string)));
        }

        private void ClearForm()
        {
            saleLineItem = null;

            lblItem.Text = string.Empty;
            lblInventory.Text = string.Empty;
            lblItemId.Text = string.Empty;
            lblItemDimensions.Text = string.Empty;

            inventoryTable.Clear();
            grInventory.DataSource = this.inventoryTable;

            colInventory.Caption = string.Format(ApplicationLocalizer.Language.Translate(2613), string.Empty); // Inventory ({0}), {0} = unit

            numPad1.ClearValue();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string barcodeId;
            string selectedItemId = string.Empty;

            // Show the search dialog through the item service
            if (!Dialog.InternalApplication.Services.Item.ItemSearch(ref selectedItemId, 500))
            {
                return;
            }

            LSRetailPosis.DataAccess.ItemData itemData = new LSRetailPosis.DataAccess.ItemData(
                ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID, ApplicationSettings.Terminal.StorePrimaryId);
            System.Data.DataTable barcodeTable = itemData.GetBarcodesForItem(selectedItemId);

            barcodeId = string.Empty;
            if (barcodeTable != null)
            {
                switch (barcodeTable.Rows.Count)
                {

                    case 0:
                        //If no barcode is found then use the item id
                        barcodeId = string.Empty;
                        break;

                    case 1:
                        //If one barcode is found then use it.
                        barcodeId = barcodeTable.Rows[0]["ITEMBARCODE"].ToString();
                        break;

                    default:
                        //If more than one barcode is found then select a barcode
                        BarcodeConfirmation barCodeConfirmation = new BarcodeConfirmation()
                        {
                            BarcodeTable = barcodeTable
                        };

                        InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(barCodeConfirmation, () =>
                        {
                            if (barCodeConfirmation.Confirmed)
                            {
                                barcodeId = barCodeConfirmation.SelectedBarcodeId;  // Use the selected barcode

                            }
                        }
                        );

                        Dialog.InternalApplication.Services.Interaction.InteractionRequest(request);
                        break;
                }
            }

            if (barcodeId.Length != 0)
            {
                numPad1.EnteredValue = barcodeId;
            }
            else
            {
                numPad1.EnteredValue = selectedItemId;
            }

            InventoryLookup(numPad1.EnteredValue);
        }

        private void numPad1_EnterButtonPressed()
        {
            // Perform new search. Assume anything entered from this NumPad is an item ID
            this.itemId = numPad1.EnteredValue;
            if (this.dimension.IsValueCreated)
                this.dimension = new Lazy<Dimensions>();

            InventoryLookup(this.itemId);
        }

        private void InventoryLookup(string barcode)
        {
            ClearForm();

            if (GetItemInfo(barcode) == false)
            {
                ClearForm();
                return;
            }
            
            // Get the inventory through the Transaction Services....
            bool retVal = false;
            string comment = string.Empty;

            try
            {
                // Begin by checking if there is a connection to the Transaction Service
                Dialog.InternalApplication.TransactionServices.CheckConnection();

                // Get the inventory status from the Transaction Services...
                Dialog.InternalApplication.TransactionServices.InventoryLookup(ref retVal, ref comment, ref inventoryTable, saleLineItem.ItemId, saleLineItem.Dimension.VariantId);
            }
            catch (PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSErrorDialog(px);

                ClearForm();
                return;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSErrorDialog(new PosisException(13010, x));      // Could not connect to the transaction services.

                ClearForm();
                return;
            }

            // Populate the foreground of the dialog => The local store and remove it from the list of other stores....
            LSRetailPosis.DataAccess.SettingsData settingsData = new LSRetailPosis.DataAccess.SettingsData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            int tableIndex = -1;

            lblItem.Text = saleLineItem.Description;
            lblItemId.Text = saleLineItem.ItemId;
            colInventory.Caption = string.Format(ApplicationLocalizer.Language.Translate(2613), saleLineItem.SalesOrderUnitOfMeasure);

            // Display dimensions if it is a variant item {Color - Size - Style - Config}
            if (!string.IsNullOrEmpty(saleLineItem.Dimension.VariantId))
            {
                StringBuilder dimensions = new StringBuilder();

                if (!string.IsNullOrEmpty(saleLineItem.Dimension.ColorName))
                {
                    dimensions.Append(saleLineItem.Dimension.ColorName);
                }

                if (!string.IsNullOrEmpty(saleLineItem.Dimension.SizeName))
                {
                    if (dimensions.Length > 0)
                    {
                        dimensions.Append(dimensionSeparator);
                    }

                    dimensions.Append(saleLineItem.Dimension.SizeName);
                }

                if (!string.IsNullOrEmpty(saleLineItem.Dimension.StyleName))
                {
                    if (dimensions.Length > 0)
                    {
                        dimensions.Append(dimensionSeparator);
                    }

                    dimensions.Append(saleLineItem.Dimension.StyleName);
                }

                if (!string.IsNullOrEmpty(saleLineItem.Dimension.ConfigName))
                {
                    if (dimensions.Length > 0)
                    {
                        dimensions.Append(dimensionSeparator);
                    }

                    dimensions.Append(saleLineItem.Dimension.ConfigName);
                }

                lblItemDimensions.Text = dimensions.ToString();

                if (!lblItemDimensions.Visible)
                {
                    lblItemDimensions.Visible = true;
                }
            }
            else
            {
                lblItemDimensions.Text = string.Empty;

                if (lblItemDimensions.Visible)
                {
                    lblItemDimensions.Visible = false;
                }
            }

            if (inventoryTable.Rows.Count > 0)
            {
                foreach (DataRow row in inventoryTable.Rows)
                {
                    // Convert the quantity from Inventory units to Sales units.
                    decimal inventory = saleLineItem.UnitQtyConversion.Convert(DBUtil.ToDecimal(row["INVENTORY"]));
                    row["INVENTORY"] = Dialog.InternalApplication.Services.Rounding.RoundQuantity(inventory, saleLineItem.SalesOrderUnitOfMeasure);

                    // Inventory for the local store will be shown on the right panel of dialog (and not in grid)
                    string itemInventLocation = DBUtil.ToStr(row["INVENTLOCATIONID"]);

                    if (ApplicationSettings.Terminal.InventLocationId.Equals(itemInventLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        lblInventory.Text = string.Format(ApplicationLocalizer.Language.Translate(2614), row["INVENTORY"], saleLineItem.SalesOrderUnitOfMeasure);
                        tableIndex = inventoryTable.Rows.IndexOf(row);
                    }
                }

                if (tableIndex != -1)
                {
                    inventoryTable.Rows.RemoveAt(tableIndex);
                }

                grInventory.DataSource = this.inventoryTable;
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage msgBox = new LSRetailPosis.POSProcesses.frmMessage(2609, MessageBoxButtons.OK, MessageBoxIcon.Error))  // Unable to retrieve the inventory status.
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(msgBox);
                }

                ClearForm();
            }
        }

        private bool GetItemInfo(string barcode)
        {
            System.Diagnostics.Debug.Assert(barcode != null, "barcode should not be null");

            if (string.IsNullOrEmpty(barcode))
            {
                return false;
            }

            saleLineItem = (SaleLineItem)Dialog.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                ApplicationSettings.Terminal.StoreCurrency,
                Dialog.InternalApplication.Services.Rounding,
                posTransaction);

            IScanInfo scanInfo = Dialog.InternalApplication.BusinessLogic.Utility.CreateScanInfo();
            scanInfo.ScanDataLabel = barcode;
            scanInfo.EntryType = BarcodeEntryType.ManuallyEntered;

            IBarcodeInfo barcodeInfo = Dialog.InternalApplication.BusinessLogic.Utility.CreateBarcodeInfo();
            barcodeInfo = Dialog.InternalApplication.Services.Barcode.ProcessBarcode(scanInfo);

            if ((barcodeInfo.InternalType == BarcodeInternalType.Item) && (barcodeInfo.ItemId != null))
            {
                // The entry was a barcode which was found and now we have the item id...
                saleLineItem.ItemId = barcodeInfo.ItemId;
                saleLineItem.BarcodeId = barcodeInfo.BarcodeId;
                saleLineItem.SalesOrderUnitOfMeasure = barcodeInfo.UnitId;
                if (barcodeInfo.BarcodeQuantity > 0)
                {
                    saleLineItem.Quantity = barcodeInfo.BarcodeQuantity;
                }

                if (barcodeInfo.BarcodePrice > 0)
                {
                    saleLineItem.Price = barcodeInfo.BarcodePrice;
                }

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

            Dialog.InternalApplication.Services.Item.ProcessItem(saleLineItem, true);

            if (saleLineItem.Found == false)
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(2611);             // Item not found.
                return false;
            }

            //Color,Size,Style,config
            if (saleLineItem.Dimension.VariantId == null)
            {
                saleLineItem.Dimension.VariantId = string.Empty;
            }

            if (saleLineItem.Dimension.EnterDimensions)
            {
                if (!string.IsNullOrWhiteSpace(this.Dimension.VariantId))
                {
                    // If we know the variant ID, load the dimensions
                    Dialog.InternalApplication.Services.Dimension.GetDimensionForVariant(this.Dimension);
                    if (this.Dimension.DistinctProductVariantId != 0)
                    {
                        this.Dimension.EnterDimensions = false;
                        saleLineItem.Dimension = this.Dimension;
                    }
                }
                else
                {
                    Dialog.InternalApplication.RunOperation(PosisOperations.SetDimensions, saleLineItem);
                }
            }

            if (!saleLineItem.SalesOrderUnitOfMeasure.Equals(saleLineItem.InventOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase))
            {
                UnitOfMeasureData uomData = new UnitOfMeasureData(
                    ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID,
                    ApplicationSettings.Terminal.StorePrimaryId, Dialog.InternalApplication);

                saleLineItem.UnitQtyConversion = uomData.GetUOMFactor(saleLineItem.InventOrderUnitOfMeasure, saleLineItem.SalesOrderUnitOfMeasure, saleLineItem);
            }

            return true;
        }

        private void btnPageUp_Click(object sender, EventArgs e)
        {
            gvInventory.MovePrevPage();
        }

        private void btnPageDown_Click(object sender, EventArgs e)
        {
            gvInventory.MoveNextPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gvInventory.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gvInventory.MoveNext();
        }

        private void btnAddToTransaction_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lblItemId.Text))
            {
                if (saleLineItem != null && saleLineItem.Dimension.EnterDimensions)
                {
                    Dialog.InternalApplication.RunOperation(PosisOperations.ItemSale, saleLineItem);
                }
                else
                {
                    Dialog.InternalApplication.RunOperation(PosisOperations.ItemSale, lblItemId.Text);
                }
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }
    }
}