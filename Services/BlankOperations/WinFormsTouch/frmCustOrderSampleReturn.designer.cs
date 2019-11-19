namespace BlankOperations.WinFormsTouch
{
    partial class frmCustOrderSampleReturn
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
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraGrid.StyleFormatCondition();
            DevExpress.XtraGrid.StyleFormatCondition styleFormatCondition2 = new DevExpress.XtraGrid.StyleFormatCondition();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIsReturned = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnReturn = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label1 = new System.Windows.Forms.Label();
            this.simpleButtonEx1 = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grReturmItem = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn5 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn8 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn10 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridColumn15 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfiguration = new DevExpress.XtraGrid.Columns.GridColumn();
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
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.btnSearchOrder = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.pnlEntry = new System.Windows.Forms.Panel();
            this.txtOrderNo = new System.Windows.Forms.TextBox();
            this.lblOrderNo = new System.Windows.Forms.Label();
            this.btnReturnedItemPrint = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grReturmItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            this.pnlEntry.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Return";
            this.gridColumn1.FieldName = "ISRETURNED";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 71;
            // 
            // colIsReturned
            // 
            this.colIsReturned.Caption = "Return";
            this.colIsReturned.FieldName = "ISRETURNED";
            this.colIsReturned.Name = "colIsReturned";
            this.colIsReturned.UnboundType = DevExpress.Data.UnboundColumnType.Boolean;
            this.colIsReturned.Visible = true;
            this.colIsReturned.VisibleIndex = 0;
            this.colIsReturned.Width = 100;
            // 
            // btnReturn
            // 
            this.btnReturn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReturn.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReturn.Appearance.Options.UseFont = true;
            this.btnReturn.Location = new System.Drawing.Point(236, 307);
            this.btnReturn.Name = "btnReturn";
            this.btnReturn.Size = new System.Drawing.Size(132, 62);
            this.btnReturn.TabIndex = 32;
            this.btnReturn.Text = "Submit";
            this.btnReturn.Click += new System.EventHandler(this.btnReturn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 30.25F);
            this.label1.Location = new System.Drawing.Point(166, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(567, 55);
            this.label1.TabIndex = 33;
            this.label1.Text = "Customer Order Sample Return";
            // 
            // simpleButtonEx1
            // 
            this.simpleButtonEx1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.simpleButtonEx1.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.simpleButtonEx1.Appearance.Options.UseFont = true;
            this.simpleButtonEx1.Location = new System.Drawing.Point(528, 307);
            this.simpleButtonEx1.Name = "simpleButtonEx1";
            this.simpleButtonEx1.Size = new System.Drawing.Size(135, 62);
            this.simpleButtonEx1.TabIndex = 32;
            this.simpleButtonEx1.Text = "Returned Items";
            this.simpleButtonEx1.Click += new System.EventHandler(this.simpleButtonEx1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.grReturmItem);
            this.panel1.Location = new System.Drawing.Point(12, 307);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(283, 62);
            this.panel1.TabIndex = 34;
            this.panel1.Visible = false;
            // 
            // grReturmItem
            // 
            this.grReturmItem.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grReturmItem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grReturmItem.Location = new System.Drawing.Point(0, 0);
            this.grReturmItem.MainView = this.gridView1;
            this.grReturmItem.Name = "grReturmItem";
            this.grReturmItem.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit2});
            this.grReturmItem.Size = new System.Drawing.Size(283, 62);
            this.grReturmItem.TabIndex = 2;
            this.grReturmItem.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            this.grReturmItem.DoubleClick += new System.EventHandler(this.grReturmItem_DoubleClick);
            // 
            // gridView1
            // 
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gridView1.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView1.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridView1.Appearance.Row.Options.UseFont = true;
            this.gridView1.Appearance.Row.Options.UseForeColor = true;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4,
            this.gridColumn5,
            this.gridColumn6,
            this.gridColumn7,
            this.gridColumn8,
            this.gridColumn9,
            this.gridColumn10,
            this.gridColumn11,
            this.gridColumn12,
            this.gridColumn13,
            this.gridColumn14,
            this.gridColumn15});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            styleFormatCondition1.ApplyToRow = true;
            styleFormatCondition1.Column = this.gridColumn1;
            styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
            styleFormatCondition1.Expression = "[ISRETURNED] == True";
            this.gridView1.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition1});
            this.gridView1.GridControl = this.grReturmItem;
            this.gridView1.HorzScrollStep = 1;
            this.gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsBehavior.SmartVertScrollBar = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.AutoCalcPreviewLineCount = true;
            this.gridView1.OptionsView.ColumnAutoWidth = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.OptionsView.ShowPreview = true;
            this.gridView1.PreviewFieldName = "DIMENSIONS";
            this.gridView1.PreviewIndent = 2;
            this.gridView1.PreviewLineCount = 1;
            this.gridView1.RowHeight = 30;
            this.gridView1.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveHorzScroll;
            this.gridView1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Item Id";
            this.gridColumn2.FieldName = "ITEMID";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.OptionsColumn.AllowEdit = false;
            this.gridColumn2.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn2.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 83;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Size";
            this.gridColumn3.FieldName = "SIZE";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.OptionsColumn.AllowEdit = false;
            this.gridColumn3.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn3.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn3.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 43;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "Color";
            this.gridColumn4.FieldName = "COLOR";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.OptionsColumn.AllowEdit = false;
            this.gridColumn4.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn4.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn4.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 43;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "Configuration";
            this.gridColumn5.FieldName = "CONFIGURATION";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.OptionsColumn.AllowEdit = false;
            this.gridColumn5.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn5.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn5.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 94;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Style";
            this.gridColumn6.FieldName = "STYLE";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.OptionsColumn.AllowEdit = false;
            this.gridColumn6.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn6.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn6.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn6.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "PCS";
            this.gridColumn7.FieldName = "PCS";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.OptionsColumn.AllowEdit = false;
            this.gridColumn7.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn7.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn7.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn7.ShowUnboundExpressionMenu = true;
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 5;
            this.gridColumn7.Width = 40;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Gross Wt";
            this.gridColumn8.FieldName = "QUANTITY";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.OptionsColumn.AllowEdit = false;
            this.gridColumn8.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn8.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn8.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 6;
            this.gridColumn8.Width = 64;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Nett Wt";
            this.gridColumn9.FieldName = "NETTWT";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.OptionsColumn.AllowEdit = false;
            this.gridColumn9.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn9.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn9.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn9.Width = 100;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Diamond Wt";
            this.gridColumn10.FieldName = "DIAWT";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.OptionsColumn.AllowEdit = false;
            this.gridColumn10.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn10.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn10.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 7;
            this.gridColumn10.Width = 89;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Diamond Value";
            this.gridColumn11.FieldName = "DIAAMT";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.OptionsColumn.AllowEdit = false;
            this.gridColumn11.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn11.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn11.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 8;
            this.gridColumn11.Width = 104;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Stone Wt";
            this.gridColumn12.FieldName = "STNWT";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.OptionsColumn.AllowEdit = false;
            this.gridColumn12.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn12.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn12.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 9;
            this.gridColumn12.Width = 85;
            // 
            // gridColumn13
            // 
            this.gridColumn13.Caption = "Stone Value";
            this.gridColumn13.FieldName = "STNAMT";
            this.gridColumn13.Name = "gridColumn13";
            this.gridColumn13.OptionsColumn.AllowEdit = false;
            this.gridColumn13.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn13.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn13.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn13.Visible = true;
            this.gridColumn13.VisibleIndex = 10;
            this.gridColumn13.Width = 95;
            // 
            // gridColumn14
            // 
            this.gridColumn14.Caption = "Total Approx Value";
            this.gridColumn14.FieldName = "TOTALAMT";
            this.gridColumn14.Name = "gridColumn14";
            this.gridColumn14.OptionsColumn.AllowEdit = false;
            this.gridColumn14.OptionsColumn.AllowIncrementalSearch = false;
            this.gridColumn14.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn14.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.gridColumn14.Visible = true;
            this.gridColumn14.VisibleIndex = 11;
            this.gridColumn14.Width = 127;
            // 
            // gridColumn15
            // 
            this.gridColumn15.Caption = "Remarks";
            this.gridColumn15.FieldName = "RemarksDtl";
            this.gridColumn15.Name = "gridColumn15";
            this.gridColumn15.Visible = true;
            this.gridColumn15.VisibleIndex = 12;
            this.gridColumn15.Width = 125;
            // 
            // repositoryItemCheckEdit2
            // 
            this.repositoryItemCheckEdit2.AutoHeight = false;
            this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(382, 307);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(132, 62);
            this.btnCancel.TabIndex = 32;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // repositoryItemCheckEdit1
            // 
            this.repositoryItemCheckEdit1.AutoHeight = false;
            this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
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
            this.colIsReturned,
            this.colItemId,
            this.colSize,
            this.colColor,
            this.colConfiguration,
            this.colStyle,
            this.colPCS,
            this.colGrWt,
            this.colNtWt,
            this.colDiaWt,
            this.colDiaAmt,
            this.colStnWt,
            this.colStnAmt,
            this.colTotalAmt,
            this.colRemarks});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            styleFormatCondition2.ApplyToRow = true;
            styleFormatCondition2.Column = this.colIsReturned;
            styleFormatCondition2.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
            styleFormatCondition2.Expression = "[ISRETURNED] == True";
            this.grdView.FormatConditions.AddRange(new DevExpress.XtraGrid.StyleFormatCondition[] {
            styleFormatCondition2});
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollStep = 1;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.SmartVertScrollBar = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsMenu.EnableColumnMenu = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.AutoCalcPreviewLineCount = true;
            this.grdView.OptionsView.ColumnAutoWidth = false;
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
            this.colItemId.VisibleIndex = 1;
            this.colItemId.Width = 85;
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
            this.colPCS.VisibleIndex = 2;
            this.colPCS.Width = 52;
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
            this.colGrWt.VisibleIndex = 3;
            this.colGrWt.Width = 69;
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
            this.colNtWt.VisibleIndex = 4;
            this.colNtWt.Width = 62;
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
            this.colDiaWt.VisibleIndex = 5;
            this.colDiaWt.Width = 97;
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
            this.colDiaAmt.Visible = true;
            this.colDiaAmt.VisibleIndex = 6;
            this.colDiaAmt.Width = 112;
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
            this.colStnWt.Visible = true;
            this.colStnWt.VisibleIndex = 7;
            this.colStnWt.Width = 87;
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
            this.colStnAmt.Visible = true;
            this.colStnAmt.VisibleIndex = 8;
            this.colStnAmt.Width = 93;
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
            this.colTotalAmt.Visible = true;
            this.colTotalAmt.VisibleIndex = 9;
            this.colTotalAmt.Width = 134;
            // 
            // colRemarks
            // 
            this.colRemarks.Caption = "Remarks";
            this.colRemarks.FieldName = "REMARKSDTL";
            this.colRemarks.Name = "colRemarks";
            this.colRemarks.Visible = true;
            this.colRemarks.VisibleIndex = 10;
            this.colRemarks.Width = 96;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(8, 95);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemCheckEdit1});
            this.grItems.Size = new System.Drawing.Size(883, 174);
            this.grItems.TabIndex = 35;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // btnSearchOrder
            // 
            this.btnSearchOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearchOrder.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearchOrder.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearchOrder.Location = new System.Drawing.Point(790, 0);
            this.btnSearchOrder.Margin = new System.Windows.Forms.Padding(0);
            this.btnSearchOrder.Name = "btnSearchOrder";
            this.btnSearchOrder.Padding = new System.Windows.Forms.Padding(3);
            this.btnSearchOrder.Size = new System.Drawing.Size(90, 38);
            this.btnSearchOrder.TabIndex = 44;
            this.btnSearchOrder.Click += new System.EventHandler(this.btnSearchOrder_Click);
            // 
            // pnlEntry
            // 
            this.pnlEntry.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlEntry.Controls.Add(this.btnSearchOrder);
            this.pnlEntry.Controls.Add(this.txtOrderNo);
            this.pnlEntry.Controls.Add(this.lblOrderNo);
            this.pnlEntry.Location = new System.Drawing.Point(8, 49);
            this.pnlEntry.Name = "pnlEntry";
            this.pnlEntry.Size = new System.Drawing.Size(883, 40);
            this.pnlEntry.TabIndex = 37;
            this.pnlEntry.Visible = false;
            // 
            // txtOrderNo
            // 
            this.txtOrderNo.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtOrderNo.Enabled = false;
            this.txtOrderNo.Location = new System.Drawing.Point(91, 3);
            this.txtOrderNo.MaxLength = 20;
            this.txtOrderNo.Multiline = true;
            this.txtOrderNo.Name = "txtOrderNo";
            this.txtOrderNo.Size = new System.Drawing.Size(696, 32);
            this.txtOrderNo.TabIndex = 5;
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.AutoSize = true;
            this.lblOrderNo.Font = new System.Drawing.Font("Tahoma", 10.25F, System.Drawing.FontStyle.Bold);
            this.lblOrderNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderNo.Location = new System.Drawing.Point(4, 15);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(81, 17);
            this.lblOrderNo.TabIndex = 4;
            this.lblOrderNo.Text = "Order No.:";
            // 
            // btnReturnedItemPrint
            // 
            this.btnReturnedItemPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReturnedItemPrint.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReturnedItemPrint.Appearance.Options.UseFont = true;
            this.btnReturnedItemPrint.Location = new System.Drawing.Point(669, 307);
            this.btnReturnedItemPrint.Name = "btnReturnedItemPrint";
            this.btnReturnedItemPrint.Size = new System.Drawing.Size(169, 62);
            this.btnReturnedItemPrint.TabIndex = 38;
            this.btnReturnedItemPrint.Text = "Returned Item Print";
            this.btnReturnedItemPrint.Click += new System.EventHandler(this.btnReturnedItemPrint_Click);
            // 
            // frmCustOrderSampleReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(898, 381);
            this.Controls.Add(this.btnReturnedItemPrint);
            this.Controls.Add(this.pnlEntry);
            this.Controls.Add(this.grItems);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.simpleButtonEx1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReturn);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmCustOrderSampleReturn";
            this.Text = "Sample Return";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grReturmItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            this.pnlEntry.ResumeLayout(false);
            this.pnlEntry.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnReturn;
        private System.Windows.Forms.Label label1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx simpleButtonEx1;
        private System.Windows.Forms.Panel panel1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private DevExpress.XtraGrid.GridControl grReturmItem;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn1;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn2;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn3;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn4;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn5;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn6;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn7;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn8;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn9;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn10;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn11;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn12;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn13;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn14;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit2;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit1;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colIsReturned;
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
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Columns.GridColumn gridColumn15;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchOrder;
        private System.Windows.Forms.Panel pnlEntry;
        public System.Windows.Forms.TextBox txtOrderNo;
        private System.Windows.Forms.Label lblOrderNo;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnReturnedItemPrint;
    }
}