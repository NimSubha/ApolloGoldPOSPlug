//namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch

namespace BlankOperations.WinFormsTouch
{
    partial class frmRepairSubDetail
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
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfig = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPCS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtConfig = new System.Windows.Forms.TextBox();
            this.lblConfig = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtStyle = new System.Windows.Forms.TextBox();
            this.lblStyle = new System.Windows.Forms.Label();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.numPad1 = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.txtItemId = new System.Windows.Forms.TextBox();
            this.btnPOSItemSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.btnAddItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.lblPCS = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.xtraGridBlending1 = new DevExpress.XtraGrid.Blending.XtraGridBlending();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.grItems);
            this.panelControl1.Controls.Add(this.txtConfig);
            this.panelControl1.Controls.Add(this.lblConfig);
            this.panelControl1.Controls.Add(this.txtSize);
            this.panelControl1.Controls.Add(this.lblSize);
            this.panelControl1.Controls.Add(this.txtStyle);
            this.panelControl1.Controls.Add(this.lblStyle);
            this.panelControl1.Controls.Add(this.txtCode);
            this.panelControl1.Controls.Add(this.lblCode);
            this.panelControl1.Controls.Add(this.btnClear);
            this.panelControl1.Controls.Add(this.btnCancel);
            this.panelControl1.Controls.Add(this.btnSubmit);
            this.panelControl1.Controls.Add(this.btnDelete);
            this.panelControl1.Controls.Add(this.btnEdit);
            this.panelControl1.Controls.Add(this.numPad1);
            this.panelControl1.Controls.Add(this.txtItemId);
            this.panelControl1.Controls.Add(this.btnPOSItemSearch);
            this.panelControl1.Controls.Add(this.txtRate);
            this.panelControl1.Controls.Add(this.lblRate);
            this.panelControl1.Controls.Add(this.btnAddItem);
            this.panelControl1.Controls.Add(this.txtQuantity);
            this.panelControl1.Controls.Add(this.lblQuantity);
            this.panelControl1.Controls.Add(this.txtPCS);
            this.panelControl1.Controls.Add(this.lblPCS);
            this.panelControl1.Controls.Add(this.txtItemName);
            this.panelControl1.Controls.Add(this.lblItemName);
            this.panelControl1.Controls.Add(this.lblItemId);
            this.panelControl1.Location = new System.Drawing.Point(6, 7);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(943, 433);
            this.panelControl1.TabIndex = 3;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(13, 160);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(692, 179);
            this.grItems.TabIndex = 145;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colItemId,
            this.colCode,
            this.colStyle,
            this.colSize,
            this.colConfig,
            this.colPCS,
            this.colQty,
            this.colRate});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // colItemId
            // 
            this.colItemId.Caption = "Item Id";
            this.colItemId.FieldName = "ITEMID";
            this.colItemId.Name = "colItemId";
            this.colItemId.OptionsColumn.AllowEdit = false;
            this.colItemId.OptionsColumn.AllowIncrementalSearch = false;
            this.colItemId.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.Visible = true;
            this.colItemId.VisibleIndex = 0;
            this.colItemId.Width = 100;
            // 
            // colCode
            // 
            this.colCode.Caption = "Code";
            this.colCode.FieldName = "COLOR";
            this.colCode.Name = "colCode";
            this.colCode.Visible = true;
            this.colCode.VisibleIndex = 1;
            // 
            // colStyle
            // 
            this.colStyle.Caption = "Style";
            this.colStyle.FieldName = "STYLE";
            this.colStyle.Name = "colStyle";
            this.colStyle.Visible = true;
            this.colStyle.VisibleIndex = 2;
            // 
            // colSize
            // 
            this.colSize.Caption = "Size";
            this.colSize.FieldName = "SIZE";
            this.colSize.Name = "colSize";
            this.colSize.Visible = true;
            this.colSize.VisibleIndex = 3;
            // 
            // colConfig
            // 
            this.colConfig.Caption = "Configuration";
            this.colConfig.FieldName = "CONFIGURATION";
            this.colConfig.Name = "colConfig";
            this.colConfig.Visible = true;
            this.colConfig.VisibleIndex = 4;
            // 
            // colPCS
            // 
            this.colPCS.Caption = "PCS";
            this.colPCS.FieldName = "PCS";
            this.colPCS.Name = "colPCS";
            this.colPCS.OptionsColumn.AllowEdit = false;
            this.colPCS.OptionsColumn.AllowIncrementalSearch = false;
            this.colPCS.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colPCS.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colPCS.ShowUnboundExpressionMenu = true;
            this.colPCS.Visible = true;
            this.colPCS.VisibleIndex = 5;
            this.colPCS.Width = 50;
            // 
            // colQty
            // 
            this.colQty.Caption = "Qty";
            this.colQty.FieldName = "QUANTITY";
            this.colQty.Name = "colQty";
            this.colQty.OptionsColumn.AllowEdit = false;
            this.colQty.OptionsColumn.AllowIncrementalSearch = false;
            this.colQty.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colQty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colQty.Visible = true;
            this.colQty.VisibleIndex = 6;
            this.colQty.Width = 50;
            // 
            // colRate
            // 
            this.colRate.Caption = "Amount";
            this.colRate.FieldName = "RATE";
            this.colRate.Name = "colRate";
            this.colRate.OptionsColumn.AllowEdit = false;
            this.colRate.OptionsColumn.AllowIncrementalSearch = false;
            this.colRate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colRate.Visible = true;
            this.colRate.VisibleIndex = 7;
            this.colRate.Width = 80;
            // 
            // txtConfig
            // 
            this.txtConfig.BackColor = System.Drawing.SystemColors.Control;
            this.txtConfig.Enabled = false;
            this.txtConfig.Location = new System.Drawing.Point(364, 78);
            this.txtConfig.MaxLength = 9;
            this.txtConfig.Name = "txtConfig";
            this.txtConfig.Size = new System.Drawing.Size(215, 21);
            this.txtConfig.TabIndex = 142;
            this.txtConfig.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblConfig.Location = new System.Drawing.Point(286, 81);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(72, 13);
            this.lblConfig.TabIndex = 144;
            this.lblConfig.Text = "Configuration";
            // 
            // txtSize
            // 
            this.txtSize.BackColor = System.Drawing.SystemColors.Control;
            this.txtSize.Enabled = false;
            this.txtSize.Location = new System.Drawing.Point(98, 78);
            this.txtSize.MaxLength = 9;
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(167, 21);
            this.txtSize.TabIndex = 141;
            this.txtSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblSize
            // 
            this.lblSize.AutoSize = true;
            this.lblSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblSize.Location = new System.Drawing.Point(10, 88);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(26, 13);
            this.lblSize.TabIndex = 143;
            this.lblSize.Text = "Size";
            // 
            // txtStyle
            // 
            this.txtStyle.BackColor = System.Drawing.SystemColors.Control;
            this.txtStyle.Enabled = false;
            this.txtStyle.Location = new System.Drawing.Point(364, 56);
            this.txtStyle.MaxLength = 9;
            this.txtStyle.Name = "txtStyle";
            this.txtStyle.Size = new System.Drawing.Size(215, 21);
            this.txtStyle.TabIndex = 138;
            this.txtStyle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblStyle
            // 
            this.lblStyle.AutoSize = true;
            this.lblStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblStyle.Location = new System.Drawing.Point(285, 61);
            this.lblStyle.Name = "lblStyle";
            this.lblStyle.Size = new System.Drawing.Size(31, 13);
            this.lblStyle.TabIndex = 140;
            this.lblStyle.Text = "Style";
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.SystemColors.Control;
            this.txtCode.Enabled = false;
            this.txtCode.Location = new System.Drawing.Point(98, 56);
            this.txtCode.MaxLength = 9;
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(167, 21);
            this.txtCode.TabIndex = 137;
            this.txtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCode.Location = new System.Drawing.Point(10, 61);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(32, 13);
            this.lblCode.TabIndex = 139;
            this.lblCode.Text = "Code";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(428, 362);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(134, 52);
            this.btnClear.TabIndex = 37;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(571, 362);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(134, 52);
            this.btnCancel.TabIndex = 38;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(10, 362);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(134, 52);
            this.btnSubmit.TabIndex = 34;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(288, 362);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(134, 52);
            this.btnDelete.TabIndex = 36;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(150, 362);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(134, 52);
            this.btnEdit.TabIndex = 35;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // numPad1
            // 
            this.numPad1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numPad1.Appearance.BackColor = System.Drawing.Color.White;
            this.numPad1.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPad1.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.numPad1.Appearance.Options.UseBackColor = true;
            this.numPad1.Appearance.Options.UseFont = true;
            this.numPad1.Appearance.Options.UseForeColor = true;
            this.numPad1.AutoSize = true;
            this.numPad1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.numPad1.CurrencyCode = null;
            this.numPad1.EnteredQuantity = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numPad1.EnteredValue = "";
            this.numPad1.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.None;
            this.numPad1.Location = new System.Drawing.Point(741, 15);
            this.numPad1.LookAndFeel.SkinName = "The Asphalt World";
            this.numPad1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.numPad1.MaskChar = null;
            this.numPad1.MaskInterval = 0;
            this.numPad1.MaxNumberOfDigits = 13;
            this.numPad1.MinimumSize = new System.Drawing.Size(180, 230);
            this.numPad1.Name = "numPad1";
            this.numPad1.NegativeMode = false;
            this.numPad1.NoOfTries = 0;
            this.numPad1.NumberOfDecimals = 2;
            this.numPad1.PromptText = null;
            this.numPad1.ShortcutKeysActive = false;
            this.numPad1.Size = new System.Drawing.Size(180, 230);
            this.numPad1.TabIndex = 136;
            this.numPad1.TimerEnabled = false;
            this.numPad1.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numPad1_EnterButtonPressed_1);
            // 
            // txtItemId
            // 
            this.txtItemId.BackColor = System.Drawing.SystemColors.Control;
            this.txtItemId.Enabled = false;
            this.txtItemId.ForeColor = System.Drawing.SystemColors.Window;
            this.txtItemId.Location = new System.Drawing.Point(98, 12);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.ReadOnly = true;
            this.txtItemId.Size = new System.Drawing.Size(481, 21);
            this.txtItemId.TabIndex = 1;
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPOSItemSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPOSItemSearch.Appearance.Options.UseFont = true;
            this.btnPOSItemSearch.Location = new System.Drawing.Point(594, 12);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(111, 36);
            this.btnPOSItemSearch.TabIndex = 2;
            this.btnPOSItemSearch.Text = "Item Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(98, 126);
            this.txtRate.MaxLength = 9;
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(167, 21);
            this.txtRate.TabIndex = 13;
            this.txtRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRate_KeyPress);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(11, 129);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(44, 13);
            this.lblRate.TabIndex = 24;
            this.lblRate.Text = "Amount";
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddItem.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddItem.Appearance.Options.UseFont = true;
            this.btnAddItem.Location = new System.Drawing.Point(594, 118);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(111, 36);
            this.btnAddItem.TabIndex = 17;
            this.btnAddItem.Text = "Add";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(364, 103);
            this.txtQuantity.MaxLength = 9;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(215, 21);
            this.txtQuantity.TabIndex = 8;
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuantity_KeyPress);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblQuantity.Location = new System.Drawing.Point(286, 106);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(49, 13);
            this.lblQuantity.TabIndex = 14;
            this.lblQuantity.Text = "Quantity";
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(98, 103);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(167, 21);
            this.txtPCS.TabIndex = 7;
            this.txtPCS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
            // 
            // lblPCS
            // 
            this.lblPCS.AutoSize = true;
            this.lblPCS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblPCS.Location = new System.Drawing.Point(11, 111);
            this.lblPCS.Name = "lblPCS";
            this.lblPCS.Size = new System.Drawing.Size(26, 13);
            this.lblPCS.TabIndex = 12;
            this.lblPCS.Text = "PCS";
            // 
            // txtItemName
            // 
            this.txtItemName.BackColor = System.Drawing.SystemColors.Control;
            this.txtItemName.Enabled = false;
            this.txtItemName.ForeColor = System.Drawing.SystemColors.Window;
            this.txtItemName.Location = new System.Drawing.Point(98, 33);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.ReadOnly = true;
            this.txtItemName.Size = new System.Drawing.Size(481, 21);
            this.txtItemName.TabIndex = 2;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblItemName.Location = new System.Drawing.Point(10, 36);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(59, 13);
            this.lblItemName.TabIndex = 4;
            this.lblItemName.Text = "Item Name";
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblItemId.Location = new System.Drawing.Point(10, 15);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(42, 13);
            this.lblItemId.TabIndex = 2;
            this.lblItemId.Text = "Item Id";
            // 
            // frmRepairSubDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 495);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmRepairSubDetail";
            this.Text = "Extended Details";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion


        //private DevExpress.XtraGrid.Columns.GridColumn colSize;
        //private DevExpress.XtraGrid.Columns.GridColumn colColor;
        //private DevExpress.XtraGrid.Columns.GridColumn colConfiguration;
        //private DevExpress.XtraGrid.Columns.GridColumn colStyle;
       // private DevExpress.XtraGrid.Columns.GridColumn colRateType;

       // private DevExpress.XtraGrid.Columns.GridColumn colAmount;

        private DevExpress.XtraEditors.PanelControl panelControl1;
       // private System.Windows.Forms.TextBox txtAmount;
      //  private System.Windows.Forms.Label label3;



      //  private System.Windows.Forms.ComboBox cmbStyle;
       // private System.Windows.Forms.Label label2;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numPad1;
        private System.Windows.Forms.TextBox txtItemId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPOSItemSearch;
      //  private System.Windows.Forms.TextBox txtTotalAmount;
      //  private System.Windows.Forms.Label lblTotalAmount;
      //  private System.Windows.Forms.ComboBox cmbRateType;
        private System.Windows.Forms.Label lblRateType;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label lblRate;
       // private System.Windows.Forms.ComboBox cmbSize;
       // private System.Windows.Forms.Label lblSizeId;
        //private System.Windows.Forms.ComboBox cmbCode;
       // private System.Windows.Forms.Label lblCode;
       // private System.Windows.Forms.Label lblConfig;
       // private System.Windows.Forms.ComboBox cmbConfig;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddItem;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.Label lblPCS;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblItemId;
        private DevExpress.XtraGrid.Blending.XtraGridBlending xtraGridBlending1;
        private System.Windows.Forms.TextBox txtConfig;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtStyle;
        private System.Windows.Forms.Label lblStyle;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label lblCode;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colCode;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colConfig;
        private DevExpress.XtraGrid.Columns.GridColumn colPCS;
        private DevExpress.XtraGrid.Columns.GridColumn colQty;
        private DevExpress.XtraGrid.Columns.GridColumn colRate;
      //  private DevExpress.XtraEditors.StyleController styleController;
    }
}