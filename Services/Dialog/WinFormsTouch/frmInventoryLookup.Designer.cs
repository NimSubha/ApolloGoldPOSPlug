/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    partial class formInventoryLookup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grInventory = new DevExpress.XtraGrid.GridControl();
            this.gvInventory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colStore = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colInventory = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblHeader = new System.Windows.Forms.Label();
            this.lblItem = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.lblInventory = new System.Windows.Forms.Label();
            this.lblItemDimensions = new System.Windows.Forms.Label();
            this.lblInventoryHeading = new System.Windows.Forms.Label();
            this.lblItemIdHeading = new System.Windows.Forms.Label();
            this.lblItemHeading = new System.Windows.Forms.Label();
            this.numPad1 = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnAddToTransaction = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPageUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPageDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInventory)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.tableLayoutPanel1);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(975, 720);
            this.panelControl1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 196F));
            this.tableLayoutPanel1.Controls.Add(this.grInventory, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblItem, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblItemId, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblInventory, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblItemDimensions, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblInventoryHeading, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblItemIdHeading, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblItemHeading, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.numPad1, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 10);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel1.RowCount = 11;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(975, 720);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // grInventory
            // 
            this.grInventory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grInventory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grInventory.Location = new System.Drawing.Point(30, 139);
            this.grInventory.MainView = this.gvInventory;
            this.grInventory.Margin = new System.Windows.Forms.Padding(4);
            this.grInventory.Name = "grInventory";
            this.tableLayoutPanel1.SetRowSpan(this.grInventory, 9);
            this.grInventory.Size = new System.Drawing.Size(604, 490);
            this.grInventory.TabIndex = 3;
            this.grInventory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvInventory});
            // 
            // gvInventory
            // 
            this.gvInventory.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvInventory.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvInventory.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gvInventory.Appearance.Row.Options.UseFont = true;
            this.gvInventory.ColumnPanelRowHeight = 40;
            this.gvInventory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colStore,
            this.colInventory});
            this.gvInventory.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gvInventory.GridControl = this.grInventory;
            this.gvInventory.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gvInventory.Name = "gvInventory";
            this.gvInventory.OptionsBehavior.Editable = false;
            this.gvInventory.OptionsCustomization.AllowFilter = false;
            this.gvInventory.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvInventory.OptionsView.ShowGroupPanel = false;
            this.gvInventory.OptionsView.ShowIndicator = false;
            this.gvInventory.RowHeight = 40;
            this.gvInventory.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.gvInventory.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.gvInventory.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // colStore
            // 
            this.colStore.Caption = "Store";
            this.colStore.FieldName = "STORENAME";
            this.colStore.Name = "colStore";
            this.colStore.Visible = true;
            this.colStore.VisibleIndex = 0;
            this.colStore.Width = 378;
            // 
            // colInventory
            // 
            this.colInventory.AppearanceCell.Options.UseTextOptions = true;
            this.colInventory.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.colInventory.Caption = "Inventory";
            this.colInventory.DisplayFormat.FormatString = "n2";
            this.colInventory.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colInventory.FieldName = "INVENTORY";
            this.colInventory.Name = "colInventory";
            this.colInventory.Visible = true;
            this.colInventory.VisibleIndex = 1;
            this.colInventory.Width = 177;
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeader.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblHeader, 4);
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(298, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeader.Size = new System.Drawing.Size(379, 95);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Inventory Lookup";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblItem
            // 
            this.lblItem.AutoSize = true;
            this.lblItem.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItem.Location = new System.Drawing.Point(756, 135);
            this.lblItem.Name = "lblItem";
            this.lblItem.Size = new System.Drawing.Size(106, 21);
            this.lblItem.TabIndex = 2;
            this.lblItem.Text = "Sample Item...";
            this.lblItem.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblItemId
            // 
            this.lblItemId.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblItemId.AutoSize = true;
            this.lblItemId.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemId.Location = new System.Drawing.Point(756, 156);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(58, 21);
            this.lblItemId.TabIndex = 4;
            this.lblItemId.Text = "Item Id";
            this.lblItemId.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblInventory
            // 
            this.lblInventory.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblInventory.AutoSize = true;
            this.lblInventory.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInventory.Location = new System.Drawing.Point(756, 177);
            this.lblInventory.Name = "lblInventory";
            this.lblInventory.Size = new System.Drawing.Size(19, 21);
            this.lblInventory.TabIndex = 6;
            this.lblInventory.Text = "3";
            this.lblInventory.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblItemDimensions
            // 
            this.lblItemDimensions.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblItemDimensions.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblItemDimensions, 2);
            this.lblItemDimensions.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemDimensions.Location = new System.Drawing.Point(671, 198);
            this.lblItemDimensions.Name = "lblItemDimensions";
            this.lblItemDimensions.Size = new System.Drawing.Size(138, 21);
            this.lblItemDimensions.TabIndex = 7;
            this.lblItemDimensions.Text = "Color - Size - Style";
            this.lblItemDimensions.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lblItemDimensions.Visible = false;
            // 
            // lblInventoryHeading
            // 
            this.lblInventoryHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblInventoryHeading.AutoSize = true;
            this.lblInventoryHeading.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInventoryHeading.Location = new System.Drawing.Point(671, 177);
            this.lblInventoryHeading.Name = "lblInventoryHeading";
            this.lblInventoryHeading.Size = new System.Drawing.Size(79, 21);
            this.lblInventoryHeading.TabIndex = 5;
            this.lblInventoryHeading.Text = "Inventory:";
            this.lblInventoryHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblItemIdHeading
            // 
            this.lblItemIdHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblItemIdHeading.AutoSize = true;
            this.lblItemIdHeading.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemIdHeading.Location = new System.Drawing.Point(671, 156);
            this.lblItemIdHeading.Name = "lblItemIdHeading";
            this.lblItemIdHeading.Size = new System.Drawing.Size(63, 21);
            this.lblItemIdHeading.TabIndex = 3;
            this.lblItemIdHeading.Text = "Item ID:";
            this.lblItemIdHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // lblItemHeading
            // 
            this.lblItemHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblItemHeading.AutoSize = true;
            this.lblItemHeading.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemHeading.Location = new System.Drawing.Point(671, 135);
            this.lblItemHeading.Name = "lblItemHeading";
            this.lblItemHeading.Size = new System.Drawing.Size(44, 21);
            this.lblItemHeading.TabIndex = 1;
            this.lblItemHeading.Text = "Item:";
            this.lblItemHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // numPad1
            // 
            this.numPad1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.numPad1.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPad1.Appearance.Options.UseFont = true;
            this.numPad1.AutoSize = true;
            this.numPad1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.numPad1, 2);
            this.numPad1.CurrencyCode = null;
            this.numPad1.EnteredQuantity = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numPad1.EnteredValue = "";
            this.numPad1.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Barcode;
            this.numPad1.Location = new System.Drawing.Point(672, 315);
            this.numPad1.Margin = new System.Windows.Forms.Padding(4);
            this.numPad1.MaskChar = "";
            this.numPad1.MaskInterval = 0;
            this.numPad1.MaxNumberOfDigits = 80;
            this.numPad1.MinimumSize = new System.Drawing.Size(248, 314);
            this.numPad1.Name = "numPad1";
            this.numPad1.NegativeMode = false;
            this.numPad1.NoOfTries = 0;
            this.numPad1.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.numPad1.PromptText = null;
            this.tableLayoutPanel1.SetRowSpan(this.numPad1, 5);
            this.numPad1.ShortcutKeysActive = false;
            this.numPad1.Size = new System.Drawing.Size(273, 314);
            this.numPad1.TabIndex = 0;
            this.numPad1.TimerEnabled = true;
            this.numPad1.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numPad1_EnterButtonPressed);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 9;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 4);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnAddToTransaction, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPageUp, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnClose, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDown, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSearch, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPageDown, 8, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(26, 644);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(923, 65);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // btnAddToTransaction
            // 
            this.btnAddToTransaction.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddToTransaction.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddToTransaction.Appearance.Options.UseFont = true;
            this.btnAddToTransaction.Location = new System.Drawing.Point(381, 4);
            this.btnAddToTransaction.Margin = new System.Windows.Forms.Padding(4);
            this.btnAddToTransaction.Name = "btnAddToTransaction";
            this.btnAddToTransaction.Size = new System.Drawing.Size(160, 57);
            this.btnAddToTransaction.TabIndex = 8;
            this.btnAddToTransaction.Text = "Add to transaction";
            this.btnAddToTransaction.Click += new System.EventHandler(this.btnAddToTransaction_Click);
            // 
            // btnPageUp
            // 
            this.btnPageUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPageUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPageUp.Appearance.Options.UseFont = true;
            this.btnPageUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPageUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPageUp.Location = new System.Drawing.Point(4, 4);
            this.btnPageUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPageUp.Name = "btnPageUp";
            this.btnPageUp.Size = new System.Drawing.Size(57, 57);
            this.btnPageUp.TabIndex = 4;
            this.btnPageUp.Text = "Ç";
            this.btnPageUp.Click += new System.EventHandler(this.btnPageUp_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(549, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(127, 57);
            this.btnClose.TabIndex = 2;
            this.btnClose.Tag = "BtnLong";
            this.btnClose.Text = "Close";
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(69, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 5;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(796, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Location = new System.Drawing.Point(246, 4);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(127, 57);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnPageDown
            // 
            this.btnPageDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPageDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPageDown.Appearance.Options.UseFont = true;
            this.btnPageDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPageDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPageDown.Location = new System.Drawing.Point(861, 4);
            this.btnPageDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPageDown.Name = "btnPageDown";
            this.btnPageDown.Size = new System.Drawing.Size(57, 57);
            this.btnPageDown.TabIndex = 7;
            this.btnPageDown.Text = "Ê";
            this.btnPageDown.Click += new System.EventHandler(this.btnPageDown_Click);
            // 
            // formInventoryLookup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(975, 720);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "formInventoryLookup";
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInventory)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numPad1;
        private System.Windows.Forms.Label lblInventory;
        private System.Windows.Forms.Label lblItem;
        private System.Windows.Forms.Label lblInventoryHeading;
        private System.Windows.Forms.Label lblItemHeading;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.Label lblItemIdHeading;
        private System.Windows.Forms.Label lblItemDimensions;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPageUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPageDown;
        private DevExpress.XtraGrid.GridControl grInventory;
        private DevExpress.XtraGrid.Views.Grid.GridView gvInventory;
        private DevExpress.XtraGrid.Columns.GridColumn colStore;
        private DevExpress.XtraGrid.Columns.GridColumn colInventory;
        private System.Windows.Forms.Label lblHeader;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddToTransaction;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

    }
}