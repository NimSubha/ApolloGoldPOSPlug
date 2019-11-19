namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmRetailStockCounting1
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ColSKUNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColPcs = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSalesMan = new DevExpress.XtraGrid.Columns.GridColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtArticle = new System.Windows.Forms.TextBox();
            this.txtProdType = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCountedBy = new System.Windows.Forms.Button();
            this.cmbVoucherSelection = new System.Windows.Forms.ComboBox();
            this.txtCountedBy = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.txtSKUNo = new System.Windows.Forms.TextBox();
            this.lblAck = new System.Windows.Forms.Label();
            this.lblAckSKUNo = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTotQty = new System.Windows.Forms.Label();
            this.lblTotNoOfSKU = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnPost = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.grItems);
            this.panel3.Location = new System.Drawing.Point(2, 91);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(803, 171);
            this.panel3.TabIndex = 21;
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(4, 3);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(796, 163);
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
            this.ColSKUNo,
            this.colDescription,
            this.ColPcs,
            this.ColQty,
            this.colSalesMan});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
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
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            // 
            // ColSKUNo
            // 
            this.ColSKUNo.Caption = "SKU No";
            this.ColSKUNo.FieldName = "SKUNumber";
            this.ColSKUNo.Name = "ColSKUNo";
            this.ColSKUNo.Visible = true;
            this.ColSKUNo.VisibleIndex = 0;
            this.ColSKUNo.Width = 111;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "Name";
            this.colDescription.Name = "colDescription";
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            // 
            // ColPcs
            // 
            this.ColPcs.Caption = "Pcs";
            this.ColPcs.FieldName = "PCS";
            this.ColPcs.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.ColPcs.Name = "ColPcs";
            this.ColPcs.Visible = true;
            this.ColPcs.VisibleIndex = 2;
            this.ColPcs.Width = 54;
            // 
            // ColQty
            // 
            this.ColQty.Caption = "Qty";
            this.ColQty.FieldName = "QTY";
            this.ColQty.Name = "ColQty";
            this.ColQty.Visible = true;
            this.ColQty.VisibleIndex = 3;
            this.ColQty.Width = 61;
            // 
            // colSalesMan
            // 
            this.colSalesMan.Caption = "Sales man";
            this.colSalesMan.FieldName = "SalesMan";
            this.colSalesMan.Name = "colSalesMan";
            this.colSalesMan.Visible = true;
            this.colSalesMan.VisibleIndex = 4;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtArticle);
            this.panel1.Controls.Add(this.txtProdType);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnCountedBy);
            this.panel1.Controls.Add(this.cmbVoucherSelection);
            this.panel1.Controls.Add(this.txtCountedBy);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.txtSKUNo);
            this.panel1.Controls.Add(this.lblAck);
            this.panel1.Controls.Add(this.lblAckSKUNo);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(803, 86);
            this.panel1.TabIndex = 20;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(468, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 200;
            this.label6.Text = "Article:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(250, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 199;
            this.label1.Text = "Product Type:";
            // 
            // txtArticle
            // 
            this.txtArticle.BackColor = System.Drawing.SystemColors.Control;
            this.txtArticle.Enabled = false;
            this.txtArticle.Location = new System.Drawing.Point(529, 23);
            this.txtArticle.MaxLength = 20;
            this.txtArticle.Name = "txtArticle";
            this.txtArticle.ReadOnly = true;
            this.txtArticle.Size = new System.Drawing.Size(127, 20);
            this.txtArticle.TabIndex = 198;
            // 
            // txtProdType
            // 
            this.txtProdType.BackColor = System.Drawing.SystemColors.Control;
            this.txtProdType.Enabled = false;
            this.txtProdType.Location = new System.Drawing.Point(330, 23);
            this.txtProdType.MaxLength = 20;
            this.txtProdType.Name = "txtProdType";
            this.txtProdType.ReadOnly = true;
            this.txtProdType.Size = new System.Drawing.Size(127, 20);
            this.txtProdType.TabIndex = 197;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(3, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 196;
            this.label2.Text = "Select voucher :";
            // 
            // btnCountedBy
            // 
            this.btnCountedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCountedBy.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnCountedBy.Location = new System.Drawing.Point(662, 58);
            this.btnCountedBy.Name = "btnCountedBy";
            this.btnCountedBy.Size = new System.Drawing.Size(61, 23);
            this.btnCountedBy.TabIndex = 2;
            this.btnCountedBy.UseVisualStyleBackColor = true;
            this.btnCountedBy.Click += new System.EventHandler(this.btnCountedBy_Click);
            // 
            // cmbVoucherSelection
            // 
            this.cmbVoucherSelection.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVoucherSelection.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbVoucherSelection.FormattingEnabled = true;
            this.cmbVoucherSelection.Location = new System.Drawing.Point(89, 22);
            this.cmbVoucherSelection.Name = "cmbVoucherSelection";
            this.cmbVoucherSelection.Size = new System.Drawing.Size(155, 21);
            this.cmbVoucherSelection.TabIndex = 0;
            this.cmbVoucherSelection.SelectedIndexChanged += new System.EventHandler(this.cmbVoucherSelection_SelectedIndexChanged);
            // 
            // txtCountedBy
            // 
            this.txtCountedBy.BackColor = System.Drawing.SystemColors.Control;
            this.txtCountedBy.Enabled = false;
            this.txtCountedBy.Location = new System.Drawing.Point(529, 60);
            this.txtCountedBy.MaxLength = 20;
            this.txtCountedBy.Name = "txtCountedBy";
            this.txtCountedBy.ReadOnly = true;
            this.txtCountedBy.Size = new System.Drawing.Size(127, 20);
            this.txtCountedBy.TabIndex = 195;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(468, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 13);
            this.label5.TabIndex = 194;
            this.label5.Text = "Counted by:";
            // 
            // btnAdd
            // 
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnAdd.Location = new System.Drawing.Point(713, 15);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(85, 31);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // txtSKUNo
            // 
            this.txtSKUNo.BackColor = System.Drawing.Color.White;
            this.txtSKUNo.Location = new System.Drawing.Point(330, 60);
            this.txtSKUNo.MaxLength = 20;
            this.txtSKUNo.Name = "txtSKUNo";
            this.txtSKUNo.Size = new System.Drawing.Size(127, 20);
            this.txtSKUNo.TabIndex = 1;
            this.txtSKUNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSKUNo_KeyDown);
            this.txtSKUNo.Validating += new System.ComponentModel.CancelEventHandler(this.txtSKUNo_Validating);
            // 
            // lblAck
            // 
            this.lblAck.AutoSize = true;
            this.lblAck.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblAck.Location = new System.Drawing.Point(4, 5);
            this.lblAck.Name = "lblAck";
            this.lblAck.Size = new System.Drawing.Size(89, 13);
            this.lblAck.TabIndex = 185;
            this.lblAck.Text = "Start Counting";
            // 
            // lblAckSKUNo
            // 
            this.lblAckSKUNo.AutoSize = true;
            this.lblAckSKUNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblAckSKUNo.Location = new System.Drawing.Point(250, 63);
            this.lblAckSKUNo.Name = "lblAckSKUNo";
            this.lblAckSKUNo.Size = new System.Drawing.Size(49, 13);
            this.lblAckSKUNo.TabIndex = 10;
            this.lblAckSKUNo.Text = "SKU No:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblTotQty);
            this.panel4.Controls.Add(this.lblTotNoOfSKU);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(2, 268);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(803, 24);
            this.panel4.TabIndex = 24;
            // 
            // lblTotQty
            // 
            this.lblTotQty.AutoSize = true;
            this.lblTotQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotQty.Location = new System.Drawing.Point(562, 6);
            this.lblTotQty.Name = "lblTotQty";
            this.lblTotQty.Size = new System.Drawing.Size(13, 13);
            this.lblTotQty.TabIndex = 198;
            this.lblTotQty.Text = "..";
            // 
            // lblTotNoOfSKU
            // 
            this.lblTotNoOfSKU.AutoSize = true;
            this.lblTotNoOfSKU.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotNoOfSKU.Location = new System.Drawing.Point(417, 6);
            this.lblTotNoOfSKU.Name = "lblTotNoOfSKU";
            this.lblTotNoOfSKU.Size = new System.Drawing.Size(13, 13);
            this.lblTotNoOfSKU.TabIndex = 196;
            this.lblTotNoOfSKU.Text = "..";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(485, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 195;
            this.label4.Text = "Total Qty.";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(315, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 193;
            this.label3.Text = "Total No. Of SKU";
            // 
            // btnPost
            // 
            this.btnPost.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnPost.Location = new System.Drawing.Point(625, 298);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 31);
            this.btnPost.TabIndex = 4;
            this.btnPost.Text = "Submit";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(715, 298);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 31);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmRetailStockCounting1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(807, 334);
            this.ControlBox = false;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Name = "frmRetailStockCounting1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Retail Stock Counting";
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn ColSKUNo;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn ColPcs;
        private DevExpress.XtraGrid.Columns.GridColumn ColQty;
        private DevExpress.XtraGrid.Columns.GridColumn colSalesMan;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCountedBy;
        private System.Windows.Forms.ComboBox cmbVoucherSelection;
        public System.Windows.Forms.TextBox txtCountedBy;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnAdd;
        public System.Windows.Forms.TextBox txtSKUNo;
        private System.Windows.Forms.Label lblAck;
        private System.Windows.Forms.Label lblAckSKUNo;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblTotQty;
        private System.Windows.Forms.Label lblTotNoOfSKU;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtArticle;
        public System.Windows.Forms.TextBox txtProdType;
    }
}