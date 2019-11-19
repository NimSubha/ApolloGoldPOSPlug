namespace BlankOperations.WinFormsTouch
{
    partial class frmCustOrderSample
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
            this.label1 = new System.Windows.Forms.Label();
            this.lblRate = new System.Windows.Forms.Label();
            this.lblPCS = new System.Windows.Forms.Label();
            this.colExtendedDetails = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfiguration = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPCS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGrWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNtWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiaWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiaAmt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStnWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStnAmt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotalAmt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRemarks = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIsReturned = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTOCKSAMPLE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnStockSample = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAddImage = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtTotalAmt = new System.Windows.Forms.TextBox();
            this.txtStnAmt = new System.Windows.Forms.TextBox();
            this.txtDiaAmt = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtStnWt = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDiaWt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtNettWt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.btnAddItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPOSItemSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label5 = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.Label();
            this.txtItemId = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.cmbCode = new System.Windows.Forms.ComboBox();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.lblSizeId = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.lblConfig = new System.Windows.Forms.Label();
            this.cmbConfig = new System.Windows.Forms.ComboBox();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 30.25F);
            this.label1.Location = new System.Drawing.Point(201, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(441, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer Order Sample";
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(-35, 239);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(30, 13);
            this.lblRate.TabIndex = 168;
            this.lblRate.Text = "Rate";
            // 
            // lblPCS
            // 
            this.lblPCS.AutoSize = true;
            this.lblPCS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblPCS.Location = new System.Drawing.Point(-35, 214);
            this.lblPCS.Name = "lblPCS";
            this.lblPCS.Size = new System.Drawing.Size(26, 13);
            this.lblPCS.TabIndex = 163;
            this.lblPCS.Text = "PCS";
            // 
            // colExtendedDetails
            // 
            this.colExtendedDetails.Caption = "Extended Details";
            this.colExtendedDetails.FieldName = "EXTENDEDDETAILS";
            this.colExtendedDetails.Name = "colExtendedDetails";
            this.colExtendedDetails.OptionsColumn.AllowEdit = false;
            this.colExtendedDetails.OptionsColumn.AllowIncrementalSearch = false;
            this.colExtendedDetails.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colExtendedDetails.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colExtendedDetails.Visible = true;
            this.colExtendedDetails.VisibleIndex = 13;
            this.colExtendedDetails.Width = 140;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(758, 502);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 54);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(440, 502);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 54);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(652, 502);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 54);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(546, 502);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 54);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(12, 313);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.grItems.Size = new System.Drawing.Size(875, 167);
            this.grItems.TabIndex = 169;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colItemId,
            this.colConfiguration,
            this.colSize,
            this.colColor,
            this.colStyle,
            this.colPCS,
            this.colGrWt,
            this.colNtWt,
            this.colDiaWt,
            this.colDiaAmt,
            this.colStnWt,
            this.colStnAmt,
            this.colTotalAmt,
            this.colRemarks,
            this.colIsReturned,
            this.colSTOCKSAMPLE});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollStep = 1;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsBehavior.SmartVertScrollBar = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.AutoCalcPreviewLineCount = true;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.OptionsView.ShowPreview = true;
            this.grdView.PreviewFieldName = "DIMENSIONS";
            this.grdView.PreviewIndent = 2;
            this.grdView.PreviewLineCount = 1;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
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
            this.colItemId.Width = 156;
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
            this.colConfiguration.VisibleIndex = 1;
            this.colConfiguration.Width = 50;
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
            this.colSize.VisibleIndex = 2;
            this.colSize.Width = 40;
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
            this.colColor.VisibleIndex = 3;
            this.colColor.Width = 40;
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
            this.colStyle.Visible = true;
            this.colStyle.VisibleIndex = 4;
            this.colStyle.Width = 39;
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
            this.colPCS.Width = 45;
            // 
            // colGrWt
            // 
            this.colGrWt.Caption = "Gross Wt";
            this.colGrWt.FieldName = "QUANTITY";
            this.colGrWt.Name = "colGrWt";
            this.colGrWt.OptionsColumn.AllowEdit = false;
            this.colGrWt.OptionsColumn.AllowIncrementalSearch = false;
            this.colGrWt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colGrWt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colGrWt.Visible = true;
            this.colGrWt.VisibleIndex = 6;
            this.colGrWt.Width = 94;
            // 
            // colNtWt
            // 
            this.colNtWt.Caption = "Nett Wt";
            this.colNtWt.FieldName = "NETTWT";
            this.colNtWt.Name = "colNtWt";
            this.colNtWt.OptionsColumn.AllowEdit = false;
            this.colNtWt.OptionsColumn.AllowIncrementalSearch = false;
            this.colNtWt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colNtWt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colNtWt.Visible = true;
            this.colNtWt.VisibleIndex = 7;
            this.colNtWt.Width = 86;
            // 
            // colDiaWt
            // 
            this.colDiaWt.Caption = "Diamond Wt";
            this.colDiaWt.FieldName = "DIAWT";
            this.colDiaWt.Name = "colDiaWt";
            this.colDiaWt.OptionsColumn.AllowEdit = false;
            this.colDiaWt.OptionsColumn.AllowIncrementalSearch = false;
            this.colDiaWt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colDiaWt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colDiaWt.Visible = true;
            this.colDiaWt.VisibleIndex = 8;
            this.colDiaWt.Width = 128;
            // 
            // colDiaAmt
            // 
            this.colDiaAmt.Caption = "Diamond Value";
            this.colDiaAmt.FieldName = "DIAAMT";
            this.colDiaAmt.Name = "colDiaAmt";
            this.colDiaAmt.OptionsColumn.AllowEdit = false;
            this.colDiaAmt.OptionsColumn.AllowIncrementalSearch = false;
            this.colDiaAmt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colDiaAmt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colDiaAmt.Width = 110;
            // 
            // colStnWt
            // 
            this.colStnWt.Caption = "Stone Wt";
            this.colStnWt.FieldName = "STNWT";
            this.colStnWt.Name = "colStnWt";
            this.colStnWt.OptionsColumn.AllowEdit = false;
            this.colStnWt.OptionsColumn.AllowIncrementalSearch = false;
            this.colStnWt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colStnWt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colStnWt.Width = 73;
            // 
            // colStnAmt
            // 
            this.colStnAmt.Caption = "Stone Value";
            this.colStnAmt.FieldName = "STNAMT";
            this.colStnAmt.Name = "colStnAmt";
            this.colStnAmt.OptionsColumn.AllowEdit = false;
            this.colStnAmt.OptionsColumn.AllowIncrementalSearch = false;
            this.colStnAmt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colStnAmt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colStnAmt.Width = 92;
            // 
            // colTotalAmt
            // 
            this.colTotalAmt.Caption = "Total Approx Value";
            this.colTotalAmt.FieldName = "TOTALAMT";
            this.colTotalAmt.Name = "colTotalAmt";
            this.colTotalAmt.OptionsColumn.AllowEdit = false;
            this.colTotalAmt.OptionsColumn.AllowIncrementalSearch = false;
            this.colTotalAmt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colTotalAmt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colTotalAmt.Width = 97;
            // 
            // colRemarks
            // 
            this.colRemarks.Caption = "Remarks";
            this.colRemarks.FieldName = "REMARKSDTL";
            this.colRemarks.Name = "colRemarks";
            this.colRemarks.Visible = true;
            this.colRemarks.VisibleIndex = 9;
            this.colRemarks.Width = 202;
            // 
            // colIsReturned
            // 
            this.colIsReturned.Caption = "Is Returned?";
            this.colIsReturned.FieldName = "ISRETURNED";
            this.colIsReturned.Name = "colIsReturned";
            this.colIsReturned.Width = 20;
            // 
            // colSTOCKSAMPLE
            // 
            this.colSTOCKSAMPLE.Caption = "STOCKSAMPLE";
            this.colSTOCKSAMPLE.FieldName = "STOCKSAMPLE";
            this.colSTOCKSAMPLE.Name = "colSTOCKSAMPLE";
            this.colSTOCKSAMPLE.Visible = true;
            this.colSTOCKSAMPLE.VisibleIndex = 10;
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnStockSample);
            this.panel1.Controls.Add(this.btnAddImage);
            this.panel1.Controls.Add(this.txtTotalAmt);
            this.panel1.Controls.Add(this.txtStnAmt);
            this.panel1.Controls.Add(this.txtDiaAmt);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtStnWt);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtDiaWt);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtNettWt);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtRemarks);
            this.panel1.Controls.Add(this.lblRemarks);
            this.panel1.Controls.Add(this.btnAddItem);
            this.panel1.Controls.Add(this.btnPOSItemSearch);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtItemName);
            this.panel1.Controls.Add(this.txtItemId);
            this.panel1.Controls.Add(this.lblItemId);
            this.panel1.Controls.Add(this.lblItemName);
            this.panel1.Controls.Add(this.lblCode);
            this.panel1.Controls.Add(this.cmbCode);
            this.panel1.Controls.Add(this.cmbStyle);
            this.panel1.Controls.Add(this.lblSizeId);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbSize);
            this.panel1.Controls.Add(this.lblConfig);
            this.panel1.Controls.Add(this.cmbConfig);
            this.panel1.Controls.Add(this.txtPCS);
            this.panel1.Controls.Add(this.txtQuantity);
            this.panel1.Controls.Add(this.lblQuantity);
            this.panel1.Location = new System.Drawing.Point(10, 59);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(882, 232);
            this.panel1.TabIndex = 1;
            // 
            // btnStockSample
            // 
            this.btnStockSample.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStockSample.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStockSample.Appearance.Options.UseFont = true;
            this.btnStockSample.Location = new System.Drawing.Point(708, 57);
            this.btnStockSample.Name = "btnStockSample";
            this.btnStockSample.Size = new System.Drawing.Size(154, 39);
            this.btnStockSample.TabIndex = 196;
            this.btnStockSample.Text = "Stock Sample";
            this.btnStockSample.Click += new System.EventHandler(this.btnStockSample_Click);
            // 
            // btnAddImage
            // 
            this.btnAddImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddImage.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddImage.Appearance.Options.UseFont = true;
            this.btnAddImage.Location = new System.Drawing.Point(708, 179);
            this.btnAddImage.Name = "btnAddImage";
            this.btnAddImage.Size = new System.Drawing.Size(154, 43);
            this.btnAddImage.TabIndex = 195;
            this.btnAddImage.Text = "Add Image";
            this.btnAddImage.Click += new System.EventHandler(this.btnAddImage_Click);
            // 
            // txtTotalAmt
            // 
            this.txtTotalAmt.AcceptsReturn = true;
            this.txtTotalAmt.Location = new System.Drawing.Point(591, 196);
            this.txtTotalAmt.MaxLength = 9;
            this.txtTotalAmt.Name = "txtTotalAmt";
            this.txtTotalAmt.Size = new System.Drawing.Size(100, 21);
            this.txtTotalAmt.TabIndex = 28;
            this.txtTotalAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTotalAmt.Visible = false;
            this.txtTotalAmt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // txtStnAmt
            // 
            this.txtStnAmt.Location = new System.Drawing.Point(335, 190);
            this.txtStnAmt.MaxLength = 9;
            this.txtStnAmt.Name = "txtStnAmt";
            this.txtStnAmt.Size = new System.Drawing.Size(106, 21);
            this.txtStnAmt.TabIndex = 26;
            this.txtStnAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStnAmt.Visible = false;
            this.txtStnAmt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // txtDiaAmt
            // 
            this.txtDiaAmt.Location = new System.Drawing.Point(550, 3);
            this.txtDiaAmt.MaxLength = 9;
            this.txtDiaAmt.Name = "txtDiaAmt";
            this.txtDiaAmt.Size = new System.Drawing.Size(106, 21);
            this.txtDiaAmt.TabIndex = 22;
            this.txtDiaAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiaAmt.Visible = false;
            this.txtDiaAmt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(447, 197);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 20);
            this.label9.TabIndex = 27;
            this.label9.Text = "Total Approx Value";
            this.label9.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(216, 191);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 20);
            this.label8.TabIndex = 25;
            this.label8.Text = "Stone Value";
            this.label8.Visible = false;
            // 
            // txtStnWt
            // 
            this.txtStnWt.Location = new System.Drawing.Point(120, 190);
            this.txtStnWt.MaxLength = 9;
            this.txtStnWt.Name = "txtStnWt";
            this.txtStnWt.Size = new System.Drawing.Size(90, 21);
            this.txtStnWt.TabIndex = 24;
            this.txtStnWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStnWt.Visible = false;
            this.txtStnWt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(520, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 20);
            this.label6.TabIndex = 21;
            this.label6.Text = "Diamond Value";
            this.label6.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(11, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 20);
            this.label7.TabIndex = 23;
            this.label7.Text = "Stone Wt";
            this.label7.Visible = false;
            // 
            // txtDiaWt
            // 
            this.txtDiaWt.Location = new System.Drawing.Point(591, 112);
            this.txtDiaWt.MaxLength = 9;
            this.txtDiaWt.Name = "txtDiaWt";
            this.txtDiaWt.Size = new System.Drawing.Size(100, 21);
            this.txtDiaWt.TabIndex = 20;
            this.txtDiaWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiaWt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(447, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 20);
            this.label4.TabIndex = 19;
            this.label4.Text = "Diamond Wt";
            // 
            // txtNettWt
            // 
            this.txtNettWt.Location = new System.Drawing.Point(591, 139);
            this.txtNettWt.MaxLength = 9;
            this.txtNettWt.Name = "txtNettWt";
            this.txtNettWt.Size = new System.Drawing.Size(100, 21);
            this.txtNettWt.TabIndex = 18;
            this.txtNettWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNettWt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(447, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 20);
            this.label3.TabIndex = 17;
            this.label3.Text = "Net Wt";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(120, 113);
            this.txtRemarks.MaxLength = 250;
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(321, 44);
            this.txtRemarks.TabIndex = 30;
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblRemarks.ForeColor = System.Drawing.Color.Black;
            this.lblRemarks.Location = new System.Drawing.Point(11, 113);
            this.lblRemarks.Name = "lblRemarks";
            this.lblRemarks.Size = new System.Drawing.Size(69, 20);
            this.lblRemarks.TabIndex = 29;
            this.lblRemarks.Text = "Remarks";
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddItem.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddItem.Appearance.Options.UseFont = true;
            this.btnAddItem.Location = new System.Drawing.Point(708, 113);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(154, 44);
            this.btnAddItem.TabIndex = 31;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPOSItemSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPOSItemSearch.Appearance.Options.UseFont = true;
            this.btnPOSItemSearch.Location = new System.Drawing.Point(708, 3);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(154, 39);
            this.btnPOSItemSearch.TabIndex = 4;
            this.btnPOSItemSearch.Text = "Product Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(447, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "PCS";
            // 
            // txtItemName
            // 
            this.txtItemName.AutoSize = true;
            this.txtItemName.Font = new System.Drawing.Font("Segoe UI Light", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtItemName.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtItemName.Location = new System.Drawing.Point(114, 38);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(79, 17);
            this.txtItemName.TabIndex = 3;
            this.txtItemName.Text = "Item Name";
            // 
            // txtItemId
            // 
            this.txtItemId.AutoSize = true;
            this.txtItemId.Font = new System.Drawing.Font("Segoe UI Light", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtItemId.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtItemId.Location = new System.Drawing.Point(114, 15);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Size = new System.Drawing.Size(54, 17);
            this.txtItemId.TabIndex = 1;
            this.txtItemId.Text = "Item Id";
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblItemId.ForeColor = System.Drawing.Color.Black;
            this.lblItemId.Location = new System.Drawing.Point(11, 13);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(60, 20);
            this.lblItemId.TabIndex = 0;
            this.lblItemId.Text = "Item ID";
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblItemName.ForeColor = System.Drawing.Color.Black;
            this.lblItemName.Location = new System.Drawing.Point(11, 36);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(87, 20);
            this.lblItemName.TabIndex = 2;
            this.lblItemName.Text = "Item Name";
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCode.ForeColor = System.Drawing.Color.Black;
            this.lblCode.Location = new System.Drawing.Point(11, 61);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(46, 20);
            this.lblCode.TabIndex = 5;
            this.lblCode.Text = "Code";
            // 
            // cmbCode
            // 
            this.cmbCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbCode.Enabled = false;
            this.cmbCode.FormattingEnabled = true;
            this.cmbCode.Location = new System.Drawing.Point(120, 60);
            this.cmbCode.Name = "cmbCode";
            this.cmbCode.Size = new System.Drawing.Size(90, 21);
            this.cmbCode.TabIndex = 6;
            // 
            // cmbStyle
            // 
            this.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbStyle.Enabled = false;
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Location = new System.Drawing.Point(335, 60);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(106, 21);
            this.cmbStyle.TabIndex = 8;
            // 
            // lblSizeId
            // 
            this.lblSizeId.AutoSize = true;
            this.lblSizeId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblSizeId.ForeColor = System.Drawing.Color.Black;
            this.lblSizeId.Location = new System.Drawing.Point(11, 84);
            this.lblSizeId.Name = "lblSizeId";
            this.lblSizeId.Size = new System.Drawing.Size(38, 20);
            this.lblSizeId.TabIndex = 9;
            this.lblSizeId.Text = "Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(216, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Style";
            // 
            // cmbSize
            // 
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbSize.Enabled = false;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Location = new System.Drawing.Point(120, 85);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(90, 21);
            this.cmbSize.TabIndex = 10;
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblConfig.ForeColor = System.Drawing.Color.Black;
            this.lblConfig.Location = new System.Drawing.Point(216, 86);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(105, 20);
            this.lblConfig.TabIndex = 11;
            this.lblConfig.Text = "Configuration";
            // 
            // cmbConfig
            // 
            this.cmbConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbConfig.Enabled = false;
            this.cmbConfig.FormattingEnabled = true;
            this.cmbConfig.Location = new System.Drawing.Point(335, 86);
            this.cmbConfig.Name = "cmbConfig";
            this.cmbConfig.Size = new System.Drawing.Size(106, 21);
            this.cmbConfig.TabIndex = 12;
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(590, 57);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(100, 21);
            this.txtPCS.TabIndex = 14;
            this.txtPCS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(591, 85);
            this.txtQuantity.MaxLength = 9;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(100, 21);
            this.txtQuantity.TabIndex = 16;
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NumericKeyPress);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblQuantity.Location = new System.Drawing.Point(447, 86);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(73, 20);
            this.lblQuantity.TabIndex = 15;
            this.lblQuantity.Text = "Gross Wt";
            // 
            // frmCustOrderSample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 558);
            this.ControlBox = false;
            this.Controls.Add(this.grItems);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.lblPCS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = false;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmCustOrderSample";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Sample Details";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private DevExpress.XtraEditors.StyleController styleController;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label lblPCS;
        //   private DevExpress.XtraGrid.Columns.GridColumn colItemName;
        private DevExpress.XtraGrid.Columns.GridColumn colExtendedDetails;
        private DevExpress.XtraGrid.Columns.GridColumn colRowTotalAmount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colColor;
        private DevExpress.XtraGrid.Columns.GridColumn colConfiguration;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colPCS;
        private DevExpress.XtraGrid.Columns.GridColumn colGrWt;
        private DevExpress.XtraGrid.Columns.GridColumn colNtWt;
        private DevExpress.XtraGrid.Columns.GridColumn colDiaWt;
        private DevExpress.XtraGrid.Columns.GridColumn colDiaAmt;
        private DevExpress.XtraGrid.Columns.GridColumn colStnWt;
        private DevExpress.XtraGrid.Columns.GridColumn colStnAmt;
        private DevExpress.XtraGrid.Columns.GridColumn colTotalAmt;
        private DevExpress.XtraGrid.Columns.GridColumn colRemarks;
        private DevExpress.XtraGrid.Columns.GridColumn colIsReturned;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private System.Windows.Forms.Panel panel1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnStockSample;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddImage;
        private System.Windows.Forms.TextBox txtTotalAmt;
        private System.Windows.Forms.TextBox txtStnAmt;
        private System.Windows.Forms.TextBox txtDiaAmt;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtStnWt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDiaWt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtNettWt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label lblRemarks;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddItem;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPOSItemSearch;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label txtItemName;
        private System.Windows.Forms.Label txtItemId;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.ComboBox cmbCode;
        private System.Windows.Forms.ComboBox cmbStyle;
        private System.Windows.Forms.Label lblSizeId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSize;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.ComboBox cmbConfig;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn colSTOCKSAMPLE;
    }
}