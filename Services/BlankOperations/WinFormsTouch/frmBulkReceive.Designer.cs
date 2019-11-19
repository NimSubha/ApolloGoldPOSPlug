namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmBulkReceive
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfigId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBatch = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPdsCWQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQtyReceived = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnFetchItem = new DevExpress.XtraEditors.SimpleButton();
            this.btnTransferSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtTransferId = new System.Windows.Forms.TextBox();
            this.lblTransferId = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblTotQty = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblQty = new System.Windows.Forms.Label();
            this.lblTotPcs = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.grItems);
            this.panel2.Location = new System.Drawing.Point(13, 154);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(718, 203);
            this.panel2.TabIndex = 25;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(0, 0);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(716, 201);
            this.grItems.TabIndex = 12;
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
            this.colConfigId,
            this.colSize,
            this.colColor,
            this.colStyle,
            this.colBatch,
            this.colPdsCWQty,
            this.colQtyReceived});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
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
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            // 
            // colItemId
            // 
            this.colItemId.Caption = "Item";
            this.colItemId.FieldName = "ItemId";
            this.colItemId.Name = "colItemId";
            this.colItemId.Visible = true;
            this.colItemId.VisibleIndex = 0;
            // 
            // colConfigId
            // 
            this.colConfigId.Caption = "Configuration";
            this.colConfigId.FieldName = "configId";
            this.colConfigId.Name = "colConfigId";
            this.colConfigId.Visible = true;
            this.colConfigId.VisibleIndex = 1;
            // 
            // colSize
            // 
            this.colSize.Caption = "Size";
            this.colSize.FieldName = "InventSizeId";
            this.colSize.Name = "colSize";
            this.colSize.Visible = true;
            this.colSize.VisibleIndex = 2;
            // 
            // colColor
            // 
            this.colColor.Caption = "Color";
            this.colColor.FieldName = "InventColorId";
            this.colColor.Name = "colColor";
            this.colColor.Visible = true;
            this.colColor.VisibleIndex = 3;
            // 
            // colStyle
            // 
            this.colStyle.Caption = "Style";
            this.colStyle.FieldName = "InventStyleId";
            this.colStyle.Name = "colStyle";
            this.colStyle.Visible = true;
            this.colStyle.VisibleIndex = 4;
            // 
            // colBatch
            // 
            this.colBatch.Caption = "Batch";
            this.colBatch.FieldName = "inventBatchId";
            this.colBatch.Name = "colBatch";
            this.colBatch.Visible = true;
            this.colBatch.VisibleIndex = 5;
            // 
            // colPdsCWQty
            // 
            this.colPdsCWQty.Caption = "PDS CW Qty.";
            this.colPdsCWQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colPdsCWQty.FieldName = "PdsCWQtyReceived";
            this.colPdsCWQty.Name = "colPdsCWQty";
            this.colPdsCWQty.Visible = true;
            this.colPdsCWQty.VisibleIndex = 6;
            // 
            // colQtyReceived
            // 
            this.colQtyReceived.Caption = "Qty.";
            this.colQtyReceived.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colQtyReceived.FieldName = "QtyReceived";
            this.colQtyReceived.Name = "colQtyReceived";
            this.colQtyReceived.Visible = true;
            this.colQtyReceived.VisibleIndex = 7;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(637, 392);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 24;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(537, 392);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 23;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnFetchItem);
            this.panel1.Controls.Add(this.btnTransferSearch);
            this.panel1.Controls.Add(this.txtTransferId);
            this.panel1.Controls.Add(this.lblTransferId);
            this.panel1.Location = new System.Drawing.Point(12, 77);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(720, 60);
            this.panel1.TabIndex = 22;
            // 
            // btnFetchItem
            // 
            this.btnFetchItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnFetchItem.Location = new System.Drawing.Point(344, 18);
            this.btnFetchItem.Name = "btnFetchItem";
            this.btnFetchItem.Size = new System.Drawing.Size(88, 32);
            this.btnFetchItem.TabIndex = 7;
            this.btnFetchItem.Text = "Fetch Item";
            this.btnFetchItem.Click += new System.EventHandler(this.btnFetchItem_Click);
            // 
            // btnTransferSearch
            // 
            this.btnTransferSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnTransferSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnTransferSearch.Location = new System.Drawing.Point(281, 18);
            this.btnTransferSearch.Name = "btnTransferSearch";
            this.btnTransferSearch.Size = new System.Drawing.Size(57, 32);
            this.btnTransferSearch.TabIndex = 6;
            this.btnTransferSearch.Text = "Search";
            this.btnTransferSearch.Click += new System.EventHandler(this.btnTransferSearch_Click);
            // 
            // txtTransferId
            // 
            this.txtTransferId.BackColor = System.Drawing.SystemColors.Control;
            this.txtTransferId.Enabled = false;
            this.txtTransferId.Location = new System.Drawing.Point(105, 23);
            this.txtTransferId.MaxLength = 20;
            this.txtTransferId.Name = "txtTransferId";
            this.txtTransferId.ReadOnly = true;
            this.txtTransferId.Size = new System.Drawing.Size(170, 20);
            this.txtTransferId.TabIndex = 5;
            // 
            // lblTransferId
            // 
            this.lblTransferId.AutoSize = true;
            this.lblTransferId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTransferId.Location = new System.Drawing.Point(6, 25);
            this.lblTransferId.Name = "lblTransferId";
            this.lblTransferId.Size = new System.Drawing.Size(90, 13);
            this.lblTransferId.TabIndex = 4;
            this.lblTransferId.Text = "Transfer Order Id:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(288, 28);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(205, 32);
            this.lblTitle.TabIndex = 21;
            this.lblTitle.Text = "Bulk Item Receive";
            // 
            // lblTotQty
            // 
            this.lblTotQty.AutoSize = true;
            this.lblTotQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotQty.Location = new System.Drawing.Point(660, 360);
            this.lblTotQty.Name = "lblTotQty";
            this.lblTotQty.Size = new System.Drawing.Size(13, 13);
            this.lblTotQty.TabIndex = 255;
            this.lblTotQty.Text = "..";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(468, 360);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 254;
            this.label3.Text = "Total Pcs";
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQty.Location = new System.Drawing.Point(600, 360);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(59, 13);
            this.lblQty.TabIndex = 253;
            this.lblQty.Text = "Total Qty";
            // 
            // lblTotPcs
            // 
            this.lblTotPcs.AutoSize = true;
            this.lblTotPcs.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotPcs.Location = new System.Drawing.Point(535, 360);
            this.lblTotPcs.Name = "lblTotPcs";
            this.lblTotPcs.Size = new System.Drawing.Size(13, 13);
            this.lblTotPcs.TabIndex = 252;
            this.lblTotPcs.Text = "..";
            // 
            // frmBulkReceive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 440);
            this.Controls.Add(this.lblTotQty);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.lblTotPcs);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.Name = "frmBulkReceive";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bulk Item Receive";
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraEditors.SimpleButton btnTransferSearch;
        public System.Windows.Forms.TextBox txtTransferId;
        private System.Windows.Forms.Label lblTransferId;
        private System.Windows.Forms.Label lblTitle;
        private DevExpress.XtraEditors.SimpleButton btnFetchItem;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colConfigId;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colPdsCWQty;
        private DevExpress.XtraGrid.Columns.GridColumn colQtyReceived;
        private DevExpress.XtraGrid.Columns.GridColumn colColor;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colBatch;
        private System.Windows.Forms.Label lblTotQty;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblQty;
        private System.Windows.Forms.Label lblTotPcs;
    }
}