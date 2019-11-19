namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmBulkItemIssue
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
            if(disposing && (components != null))
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
            this.cmbSize = new System.Windows.Forms.TextBox();
            this.colUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbCode = new System.Windows.Forms.TextBox();
            this.cmbConfig = new System.Windows.Forms.TextBox();
            this.lblUnit = new System.Windows.Forms.Label();
            this.lblSizeId = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.lblConfig = new System.Windows.Forms.Label();
            this.btnAddItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.colQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.lblPCS = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.colPCS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtItemId = new System.Windows.Forms.TextBox();
            this.colColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfiguration = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMetalType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colBatch = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGrossWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNetWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtBatch = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOgFetch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblDeliveryDate = new System.Windows.Forms.Label();
            this.lblTotQty = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblTotPcs = new System.Windows.Forms.Label();
            this.lblTotGrWt = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotNetWt = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnFetchOD = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtGrossWt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNetWt = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbMetalType = new System.Windows.Forms.ComboBox();
            this.dtpFDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // cmbSize
            // 
            this.cmbSize.Location = new System.Drawing.Point(91, 88);
            this.cmbSize.MaxLength = 20;
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(111, 21);
            this.cmbSize.TabIndex = 4;
            this.cmbSize.Leave += new System.EventHandler(this.cmbSize_Leave);
            // 
            // colUnit
            // 
            this.colUnit.Caption = "Unit";
            this.colUnit.FieldName = "UNITID";
            this.colUnit.Name = "colUnit";
            // 
            // cmbCode
            // 
            this.cmbCode.Location = new System.Drawing.Point(245, 62);
            this.cmbCode.MaxLength = 20;
            this.cmbCode.Name = "cmbCode";
            this.cmbCode.Size = new System.Drawing.Size(112, 21);
            this.cmbCode.TabIndex = 3;
            this.cmbCode.Leave += new System.EventHandler(this.cmbCode_Leave);
            // 
            // cmbConfig
            // 
            this.cmbConfig.Location = new System.Drawing.Point(91, 62);
            this.cmbConfig.MaxLength = 20;
            this.cmbConfig.Name = "cmbConfig";
            this.cmbConfig.Size = new System.Drawing.Size(111, 21);
            this.cmbConfig.TabIndex = 2;
            this.cmbConfig.Leave += new System.EventHandler(this.cmbConfig_Leave);
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblUnit.Location = new System.Drawing.Point(569, 39);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(26, 13);
            this.lblUnit.TabIndex = 238;
            this.lblUnit.Text = "Unit";
            // 
            // lblSizeId
            // 
            this.lblSizeId.AutoSize = true;
            this.lblSizeId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblSizeId.Location = new System.Drawing.Point(5, 95);
            this.lblSizeId.Name = "lblSizeId";
            this.lblSizeId.Size = new System.Drawing.Size(26, 13);
            this.lblSizeId.TabIndex = 229;
            this.lblSizeId.Text = "Size";
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCode.Location = new System.Drawing.Point(207, 65);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(32, 13);
            this.lblCode.TabIndex = 228;
            this.lblCode.Text = "Color";
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblConfig.Location = new System.Drawing.Point(5, 65);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(72, 13);
            this.lblConfig.TabIndex = 227;
            this.lblConfig.Text = "Configuration";
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddItem.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddItem.Appearance.Options.UseFont = true;
            this.btnAddItem.Location = new System.Drawing.Point(810, 82);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(111, 36);
            this.btnAddItem.TabIndex = 12;
            this.btnAddItem.Text = "Add";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(456, 36);
            this.txtQuantity.MaxLength = 8;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(107, 21);
            this.txtQuantity.TabIndex = 7;
            this.txtQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuantity_KeyPress);
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
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblQuantity.Location = new System.Drawing.Point(371, 39);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(49, 13);
            this.lblQuantity.TabIndex = 231;
            this.lblQuantity.Text = "Quantity";
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(456, 10);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(107, 21);
            this.txtPCS.TabIndex = 6;
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
            this.txtPCS.Leave += new System.EventHandler(this.txtPCS_Leave);
            // 
            // lblPCS
            // 
            this.lblPCS.AutoSize = true;
            this.lblPCS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblPCS.Location = new System.Drawing.Point(371, 15);
            this.lblPCS.Name = "lblPCS";
            this.lblPCS.Size = new System.Drawing.Size(26, 13);
            this.lblPCS.TabIndex = 230;
            this.lblPCS.Text = "PCS";
            // 
            // txtItemName
            // 
            this.txtItemName.BackColor = System.Drawing.SystemColors.Window;
            this.txtItemName.Enabled = false;
            this.txtItemName.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtItemName.Location = new System.Drawing.Point(91, 36);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(266, 21);
            this.txtItemName.TabIndex = 1;
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblItemName.Location = new System.Drawing.Point(5, 39);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(59, 13);
            this.lblItemName.TabIndex = 226;
            this.lblItemName.Text = "Item Name";
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblItemId.Location = new System.Drawing.Point(5, 16);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(42, 13);
            this.lblItemId.TabIndex = 225;
            this.lblItemId.Text = "Item Id";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(735, 432);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(78, 36);
            this.btnClear.TabIndex = 222;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(821, 432);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 36);
            this.btnCancel.TabIndex = 223;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(545, 432);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(98, 36);
            this.btnSubmit.TabIndex = 219;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(649, 432);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(78, 36);
            this.btnDelete.TabIndex = 221;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(461, 432);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(78, 36);
            this.btnEdit.TabIndex = 220;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
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
            // txtItemId
            // 
            this.txtItemId.BackColor = System.Drawing.SystemColors.Window;
            this.txtItemId.ForeColor = System.Drawing.SystemColors.WindowText;
            this.txtItemId.Location = new System.Drawing.Point(91, 13);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Size = new System.Drawing.Size(266, 21);
            this.txtItemId.TabIndex = 0;
            this.txtItemId.Leave += new System.EventHandler(this.txtItemId_Leave);
            // 
            // colColor
            // 
            this.colColor.Caption = "Color";
            this.colColor.FieldName = "COLOR";
            this.colColor.Name = "colColor";
            this.colColor.OptionsColumn.AllowEdit = false;
            this.colColor.OptionsColumn.AllowIncrementalSearch = false;
            this.colColor.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colColor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colColor.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colColor.Visible = true;
            this.colColor.VisibleIndex = 2;
            this.colColor.Width = 50;
            // 
            // colConfiguration
            // 
            this.colConfiguration.Caption = "Configuration";
            this.colConfiguration.FieldName = "CONFIGURATION";
            this.colConfiguration.Name = "colConfiguration";
            this.colConfiguration.OptionsColumn.AllowEdit = false;
            this.colConfiguration.OptionsColumn.AllowIncrementalSearch = false;
            this.colConfiguration.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colConfiguration.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colConfiguration.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colConfiguration.Visible = true;
            this.colConfiguration.VisibleIndex = 3;
            this.colConfiguration.Width = 100;
            // 
            // colSize
            // 
            this.colSize.Caption = "Size";
            this.colSize.FieldName = "SIZE";
            this.colSize.Name = "colSize";
            this.colSize.OptionsColumn.AllowEdit = false;
            this.colSize.OptionsColumn.AllowIncrementalSearch = false;
            this.colSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colSize.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colSize.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colSize.Visible = true;
            this.colSize.VisibleIndex = 1;
            this.colSize.Width = 50;
            // 
            // colMetalType
            // 
            this.colMetalType.Caption = "Metal type";
            this.colMetalType.FieldName = "METALTYPE";
            this.colMetalType.Name = "colMetalType";
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
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(8, 124);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(913, 276);
            this.grItems.TabIndex = 224;
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
            this.colSize,
            this.colColor,
            this.colConfiguration,
            this.colBatch,
            this.colMetalType,
            this.colStyle,
            this.colPCS,
            this.colQty,
            this.colGrossWt,
            this.colNetWt,
            this.colUnit});
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
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            // 
            // colBatch
            // 
            this.colBatch.Caption = "Batch";
            this.colBatch.FieldName = "BATCH";
            this.colBatch.Name = "colBatch";
            this.colBatch.Visible = true;
            this.colBatch.VisibleIndex = 4;
            // 
            // colStyle
            // 
            this.colStyle.Caption = "Style";
            this.colStyle.FieldName = "STYLE";
            this.colStyle.Name = "colStyle";
            this.colStyle.OptionsColumn.AllowEdit = false;
            this.colStyle.OptionsColumn.AllowIncrementalSearch = false;
            this.colStyle.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colStyle.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colStyle.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colStyle.Width = 50;
            // 
            // colGrossWt
            // 
            this.colGrossWt.Caption = "Gross Weight";
            this.colGrossWt.FieldName = "GrossWt";
            this.colGrossWt.Name = "colGrossWt";
            this.colGrossWt.Visible = true;
            this.colGrossWt.VisibleIndex = 7;
            // 
            // colNetWt
            // 
            this.colNetWt.Caption = "Net Weight";
            this.colNetWt.FieldName = "NetWt";
            this.colNetWt.Name = "colNetWt";
            this.colNetWt.Visible = true;
            this.colNetWt.VisibleIndex = 8;
            // 
            // txtBatch
            // 
            this.txtBatch.Location = new System.Drawing.Point(245, 89);
            this.txtBatch.MaxLength = 20;
            this.txtBatch.Name = "txtBatch";
            this.txtBatch.Size = new System.Drawing.Size(112, 21);
            this.txtBatch.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(207, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 240;
            this.label1.Text = "Batch";
            // 
            // btnOgFetch
            // 
            this.btnOgFetch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOgFetch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOgFetch.Appearance.Options.UseFont = true;
            this.btnOgFetch.Location = new System.Drawing.Point(810, 5);
            this.btnOgFetch.Name = "btnOgFetch";
            this.btnOgFetch.Size = new System.Drawing.Size(111, 36);
            this.btnOgFetch.TabIndex = 10;
            this.btnOgFetch.Text = "Fetch OG";
            this.btnOgFetch.Click += new System.EventHandler(this.btnOgFetch_Click);
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(689, 35);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(111, 21);
            this.dtpToDate.TabIndex = 243;
            // 
            // lblDeliveryDate
            // 
            this.lblDeliveryDate.AutoSize = true;
            this.lblDeliveryDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDeliveryDate.Location = new System.Drawing.Point(619, 38);
            this.lblDeliveryDate.Name = "lblDeliveryDate";
            this.lblDeliveryDate.Size = new System.Drawing.Size(49, 13);
            this.lblDeliveryDate.TabIndex = 242;
            this.lblDeliveryDate.Text = "To Date:";
            // 
            // lblTotQty
            // 
            this.lblTotQty.AutoSize = true;
            this.lblTotQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotQty.Location = new System.Drawing.Point(612, 403);
            this.lblTotQty.Name = "lblTotQty";
            this.lblTotQty.Size = new System.Drawing.Size(13, 13);
            this.lblTotQty.TabIndex = 247;
            this.lblTotQty.Text = "..";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(450, 403);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 246;
            this.label2.Text = "Total Pcs";
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQty.Location = new System.Drawing.Point(552, 403);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(59, 13);
            this.lblQty.TabIndex = 245;
            this.lblQty.Text = "Total Qty";
            // 
            // lblTotPcs
            // 
            this.lblTotPcs.AutoSize = true;
            this.lblTotPcs.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotPcs.Location = new System.Drawing.Point(517, 403);
            this.lblTotPcs.Name = "lblTotPcs";
            this.lblTotPcs.Size = new System.Drawing.Size(13, 13);
            this.lblTotPcs.TabIndex = 244;
            this.lblTotPcs.Text = "..";
            // 
            // lblTotGrWt
            // 
            this.lblTotGrWt.AutoSize = true;
            this.lblTotGrWt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotGrWt.Location = new System.Drawing.Point(734, 403);
            this.lblTotGrWt.Name = "lblTotGrWt";
            this.lblTotGrWt.Size = new System.Drawing.Size(13, 13);
            this.lblTotGrWt.TabIndex = 249;
            this.lblTotGrWt.Text = "..";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(663, 403);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 248;
            this.label4.Text = "Total Gr Wt";
            // 
            // lblTotNetWt
            // 
            this.lblTotNetWt.AutoSize = true;
            this.lblTotNetWt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotNetWt.Location = new System.Drawing.Point(869, 403);
            this.lblTotNetWt.Name = "lblTotNetWt";
            this.lblTotNetWt.Size = new System.Drawing.Size(13, 13);
            this.lblTotNetWt.TabIndex = 251;
            this.lblTotNetWt.Text = "..";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(792, 403);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 250;
            this.label6.Text = "Total Net Wt";
            // 
            // btnFetchOD
            // 
            this.btnFetchOD.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFetchOD.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFetchOD.Appearance.Options.UseFont = true;
            this.btnFetchOD.Location = new System.Drawing.Point(810, 43);
            this.btnFetchOD.Name = "btnFetchOD";
            this.btnFetchOD.Size = new System.Drawing.Size(111, 36);
            this.btnFetchOD.TabIndex = 11;
            this.btnFetchOD.Text = "Fetch OD";
            this.btnFetchOD.Click += new System.EventHandler(this.btnFetchOD_Click);
            // 
            // txtGrossWt
            // 
            this.txtGrossWt.Location = new System.Drawing.Point(456, 62);
            this.txtGrossWt.MaxLength = 8;
            this.txtGrossWt.Name = "txtGrossWt";
            this.txtGrossWt.Size = new System.Drawing.Size(107, 21);
            this.txtGrossWt.TabIndex = 8;
            this.txtGrossWt.Leave += new System.EventHandler(this.txtGrossWt_Leave);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(371, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 254;
            this.label3.Text = "Gross Wt.";
            // 
            // txtNetWt
            // 
            this.txtNetWt.Location = new System.Drawing.Point(456, 89);
            this.txtNetWt.MaxLength = 8;
            this.txtNetWt.Name = "txtNetWt";
            this.txtNetWt.Size = new System.Drawing.Size(107, 21);
            this.txtNetWt.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(371, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 256;
            this.label5.Text = "Net Wt.";
            // 
            // cmbMetalType
            // 
            this.cmbMetalType.BackColor = System.Drawing.Color.Lavender;
            this.cmbMetalType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMetalType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMetalType.FormattingEnabled = true;
            this.cmbMetalType.Location = new System.Drawing.Point(689, 61);
            this.cmbMetalType.Name = "cmbMetalType";
            this.cmbMetalType.Size = new System.Drawing.Size(111, 25);
            this.cmbMetalType.TabIndex = 258;
            // 
            // dtpFDate
            // 
            this.dtpFDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFDate.Location = new System.Drawing.Point(689, 10);
            this.dtpFDate.Name = "dtpFDate";
            this.dtpFDate.Size = new System.Drawing.Size(111, 21);
            this.dtpFDate.TabIndex = 260;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label7.Location = new System.Drawing.Point(619, 13);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 13);
            this.label7.TabIndex = 259;
            this.label7.Text = "From Date:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label8.Location = new System.Drawing.Point(619, 66);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 13);
            this.label8.TabIndex = 261;
            this.label8.Text = "Metal Type :";
            // 
            // frmBulkItemIssue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 479);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dtpFDate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbMetalType);
            this.Controls.Add(this.txtNetWt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtGrossWt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnFetchOD);
            this.Controls.Add(this.lblTotNetWt);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblTotGrWt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTotQty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.lblTotPcs);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.lblDeliveryDate);
            this.Controls.Add(this.btnOgFetch);
            this.Controls.Add(this.txtBatch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSize);
            this.Controls.Add(this.cmbCode);
            this.Controls.Add(this.cmbConfig);
            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.lblSizeId);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.lblConfig);
            this.Controls.Add(this.btnAddItem);
            this.Controls.Add(this.txtQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.txtPCS);
            this.Controls.Add(this.lblPCS);
            this.Controls.Add(this.txtItemName);
            this.Controls.Add(this.lblItemName);
            this.Controls.Add(this.lblItemId);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.txtItemId);
            this.Controls.Add(this.grItems);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmBulkItemIssue";
            this.Text = "Bulk Item Issue";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox cmbSize;
        private DevExpress.XtraGrid.Columns.GridColumn colUnit;
        private System.Windows.Forms.TextBox cmbCode;
        private System.Windows.Forms.TextBox cmbConfig;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.Label lblSizeId;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.Label lblConfig;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddItem;
        private System.Windows.Forms.TextBox txtQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn colQty;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.Label lblPCS;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblItemId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private DevExpress.XtraGrid.Columns.GridColumn colPCS;
        private System.Windows.Forms.TextBox txtItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colColor;
        private DevExpress.XtraGrid.Columns.GridColumn colConfiguration;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colMetalType;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private System.Windows.Forms.TextBox txtBatch;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraGrid.Columns.GridColumn colBatch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOgFetch;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label lblDeliveryDate;
        private DevExpress.XtraGrid.Columns.GridColumn colGrossWt;
        private DevExpress.XtraGrid.Columns.GridColumn colNetWt;
        private System.Windows.Forms.Label lblTotQty;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblTotPcs;
        private System.Windows.Forms.Label lblTotGrWt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotNetWt;
        private System.Windows.Forms.Label label6;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnFetchOD;
        private System.Windows.Forms.TextBox txtGrossWt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNetWt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbMetalType;
        private System.Windows.Forms.DateTimePicker dtpFDate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;

    }
}