namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmBulkItemShipment
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
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfiguration = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBatch = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMetalType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPCS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnWHSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtWarehouse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnBIRFetch = new DevExpress.XtraEditors.SimpleButton();
            this.txtIssueReceipt = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFetch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblAirwayBillNo = new System.Windows.Forms.Label();
            this.txtAirwayBillNo = new System.Windows.Forms.TextBox();
            this.lblWayBillNo = new System.Windows.Forms.Label();
            this.txtWayBillNo = new System.Windows.Forms.TextBox();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblTotQty = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblTotPcs = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(2, 142);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(878, 250);
            this.grItems.TabIndex = 225;
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
            // colSize
            // 
            this.colSize.Caption = "Size";
            this.colSize.FieldName = "INVENTSIZEID";
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
            // colColor
            // 
            this.colColor.Caption = "Color";
            this.colColor.FieldName = "INVENTCOLORID";
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
            this.colConfiguration.FieldName = "CONFIGID";
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
            // colBatch
            // 
            this.colBatch.Caption = "Batch";
            this.colBatch.FieldName = "INVENTBATCHID";
            this.colBatch.Name = "colBatch";
            this.colBatch.Visible = true;
            this.colBatch.VisibleIndex = 4;
            // 
            // colMetalType
            // 
            this.colMetalType.Caption = "Metal type";
            this.colMetalType.FieldName = "METALTYPE";
            this.colMetalType.Name = "colMetalType";
            // 
            // colStyle
            // 
            this.colStyle.Caption = "Style";
            this.colStyle.FieldName = "INVENTSTYLEID";
            this.colStyle.Name = "colStyle";
            this.colStyle.OptionsColumn.AllowEdit = false;
            this.colStyle.OptionsColumn.AllowIncrementalSearch = false;
            this.colStyle.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colStyle.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colStyle.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colStyle.Width = 50;
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
            // colUnit
            // 
            this.colUnit.Caption = "Unit";
            this.colUnit.FieldName = "UNITID";
            this.colUnit.Name = "colUnit";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(338, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(245, 32);
            this.lblTitle.TabIndex = 226;
            this.lblTitle.Text = "Transfer Order Create";
            // 
            // btnWHSearch
            // 
            this.btnWHSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnWHSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnWHSearch.Location = new System.Drawing.Point(289, 57);
            this.btnWHSearch.Name = "btnWHSearch";
            this.btnWHSearch.Size = new System.Drawing.Size(57, 32);
            this.btnWHSearch.TabIndex = 229;
            this.btnWHSearch.Text = "Search";
            this.btnWHSearch.Click += new System.EventHandler(this.btnWHSearch_Click);
            // 
            // txtWarehouse
            // 
            this.txtWarehouse.BackColor = System.Drawing.SystemColors.Control;
            this.txtWarehouse.Location = new System.Drawing.Point(113, 62);
            this.txtWarehouse.MaxLength = 20;
            this.txtWarehouse.Name = "txtWarehouse";
            this.txtWarehouse.ReadOnly = true;
            this.txtWarehouse.Size = new System.Drawing.Size(170, 20);
            this.txtWarehouse.TabIndex = 228;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(17, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 227;
            this.label2.Text = "To Warehouse:";
            // 
            // btnBIRFetch
            // 
            this.btnBIRFetch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnBIRFetch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnBIRFetch.Location = new System.Drawing.Point(665, 56);
            this.btnBIRFetch.Name = "btnBIRFetch";
            this.btnBIRFetch.Size = new System.Drawing.Size(57, 32);
            this.btnBIRFetch.TabIndex = 232;
            this.btnBIRFetch.Text = "Search";
            this.btnBIRFetch.Click += new System.EventHandler(this.btnBIRFetch_Click);
            // 
            // txtIssueReceipt
            // 
            this.txtIssueReceipt.BackColor = System.Drawing.SystemColors.Control;
            this.txtIssueReceipt.Location = new System.Drawing.Point(456, 61);
            this.txtIssueReceipt.MaxLength = 20;
            this.txtIssueReceipt.Name = "txtIssueReceipt";
            this.txtIssueReceipt.ReadOnly = true;
            this.txtIssueReceipt.Size = new System.Drawing.Size(203, 20);
            this.txtIssueReceipt.TabIndex = 231;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(354, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 13);
            this.label1.TabIndex = 230;
            this.label1.Text = "Bulk Issue Receipt";
            // 
            // btnFetch
            // 
            this.btnFetch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFetch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFetch.Appearance.Options.UseFont = true;
            this.btnFetch.Location = new System.Drawing.Point(754, 62);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(98, 36);
            this.btnFetch.TabIndex = 233;
            this.btnFetch.Text = "Fetch";
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Location = new System.Drawing.Point(754, 424);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(98, 36);
            this.btnClose.TabIndex = 234;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(624, 424);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(98, 36);
            this.btnSubmit.TabIndex = 235;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblAirwayBillNo
            // 
            this.lblAirwayBillNo.AutoSize = true;
            this.lblAirwayBillNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblAirwayBillNo.Location = new System.Drawing.Point(17, 115);
            this.lblAirwayBillNo.Name = "lblAirwayBillNo";
            this.lblAirwayBillNo.Size = new System.Drawing.Size(74, 13);
            this.lblAirwayBillNo.TabIndex = 238;
            this.lblAirwayBillNo.Text = "Airway Bill No:";
            // 
            // txtAirwayBillNo
            // 
            this.txtAirwayBillNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtAirwayBillNo.Location = new System.Drawing.Point(112, 113);
            this.txtAirwayBillNo.MaxLength = 20;
            this.txtAirwayBillNo.Name = "txtAirwayBillNo";
            this.txtAirwayBillNo.Size = new System.Drawing.Size(170, 20);
            this.txtAirwayBillNo.TabIndex = 239;
            // 
            // lblWayBillNo
            // 
            this.lblWayBillNo.AutoSize = true;
            this.lblWayBillNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblWayBillNo.Location = new System.Drawing.Point(17, 89);
            this.lblWayBillNo.Name = "lblWayBillNo";
            this.lblWayBillNo.Size = new System.Drawing.Size(65, 13);
            this.lblWayBillNo.TabIndex = 236;
            this.lblWayBillNo.Text = "Way Bill No:";
            // 
            // txtWayBillNo
            // 
            this.txtWayBillNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtWayBillNo.Location = new System.Drawing.Point(112, 87);
            this.txtWayBillNo.MaxLength = 20;
            this.txtWayBillNo.Name = "txtWayBillNo";
            this.txtWayBillNo.Size = new System.Drawing.Size(170, 20);
            this.txtWayBillNo.TabIndex = 237;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(487, 424);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(98, 36);
            this.btnDelete.TabIndex = 240;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lblTotQty
            // 
            this.lblTotQty.AutoSize = true;
            this.lblTotQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotQty.Location = new System.Drawing.Point(793, 395);
            this.lblTotQty.Name = "lblTotQty";
            this.lblTotQty.Size = new System.Drawing.Size(13, 13);
            this.lblTotQty.TabIndex = 251;
            this.lblTotQty.Text = "..";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(601, 395);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 250;
            this.label3.Text = "Total Pcs";
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQty.Location = new System.Drawing.Point(733, 395);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(59, 13);
            this.lblQty.TabIndex = 249;
            this.lblQty.Text = "Total Qty";
            // 
            // lblTotPcs
            // 
            this.lblTotPcs.AutoSize = true;
            this.lblTotPcs.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotPcs.Location = new System.Drawing.Point(668, 395);
            this.lblTotPcs.Name = "lblTotPcs";
            this.lblTotPcs.Size = new System.Drawing.Size(13, 13);
            this.lblTotPcs.TabIndex = 248;
            this.lblTotPcs.Text = "..";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(354, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 253;
            this.label4.Text = "Remarks";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(409, 92);
            this.txtRemarks.MaxLength = 60;
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(250, 44);
            this.txtRemarks.TabIndex = 252;
            // 
            // frmBulkItemShipment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 472);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtRemarks);
            this.Controls.Add(this.lblTotQty);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.lblTotPcs);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.lblAirwayBillNo);
            this.Controls.Add(this.txtAirwayBillNo);
            this.Controls.Add(this.lblWayBillNo);
            this.Controls.Add(this.txtWayBillNo);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.btnBIRFetch);
            this.Controls.Add(this.txtIssueReceipt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnWHSearch);
            this.Controls.Add(this.txtWarehouse);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.grItems);
            this.Name = "frmBulkItemShipment";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulk Item Shipment";
            this.Load += new System.EventHandler(this.frmBulkItemShipment_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colColor;
        private DevExpress.XtraGrid.Columns.GridColumn colConfiguration;
        private DevExpress.XtraGrid.Columns.GridColumn colBatch;
        private DevExpress.XtraGrid.Columns.GridColumn colMetalType;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colPCS;
        private DevExpress.XtraGrid.Columns.GridColumn colQty;
        private DevExpress.XtraGrid.Columns.GridColumn colUnit;
        private System.Windows.Forms.Label lblTitle;
        private DevExpress.XtraEditors.SimpleButton btnWHSearch;
        public System.Windows.Forms.TextBox txtWarehouse;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraEditors.SimpleButton btnBIRFetch;
        public System.Windows.Forms.TextBox txtIssueReceipt;
        private System.Windows.Forms.Label label1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnFetch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private System.Windows.Forms.Label lblAirwayBillNo;
        public System.Windows.Forms.TextBox txtAirwayBillNo;
        private System.Windows.Forms.Label lblWayBillNo;
        public System.Windows.Forms.TextBox txtWayBillNo;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private System.Windows.Forms.Label lblTotQty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblTotPcs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRemarks;
    }
}