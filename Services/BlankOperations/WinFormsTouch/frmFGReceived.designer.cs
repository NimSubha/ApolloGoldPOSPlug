namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmFGReceived
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnFetch = new System.Windows.Forms.Button();
            this.btnGTId = new DevExpress.XtraEditors.SimpleButton();
            this.btnWHSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnVendorSearch = new DevExpress.XtraEditors.SimpleButton();
            this.lblGTId = new System.Windows.Forms.Label();
            this.txtGTId = new System.Windows.Forms.TextBox();
            this.txtWarehouse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.txtVendorAc = new System.Windows.Forms.TextBox();
            this.lblVendorAccount = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAcknowledge = new System.Windows.Forms.Button();
            this.txtAckSKUNo = new System.Windows.Forms.TextBox();
            this.lblAck = new System.Windows.Forms.Label();
            this.lblAckSKUNo = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.ColSKUNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColWONo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColWOSrlNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColItemNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColConfig = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColPcs = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.ColAcknowledged = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPost = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblTotQty = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTotNoOfSKU = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotSetOf = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnFetch);
            this.panel2.Controls.Add(this.btnGTId);
            this.panel2.Controls.Add(this.btnWHSearch);
            this.panel2.Controls.Add(this.btnVendorSearch);
            this.panel2.Controls.Add(this.lblGTId);
            this.panel2.Controls.Add(this.txtGTId);
            this.panel2.Controls.Add(this.txtWarehouse);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtVendorName);
            this.panel2.Controls.Add(this.txtVendorAc);
            this.panel2.Controls.Add(this.lblVendorAccount);
            this.panel2.Location = new System.Drawing.Point(8, 63);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(720, 130);
            this.panel2.TabIndex = 11;
            // 
            // btnFetch
            // 
            this.btnFetch.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnFetch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnFetch.Location = new System.Drawing.Point(634, 92);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(78, 31);
            this.btnFetch.TabIndex = 16;
            this.btnFetch.Text = "Fetch";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // btnGTId
            // 
            this.btnGTId.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnGTId.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGTId.Location = new System.Drawing.Point(652, 49);
            this.btnGTId.Name = "btnGTId";
            this.btnGTId.Size = new System.Drawing.Size(57, 32);
            this.btnGTId.TabIndex = 9;
            this.btnGTId.Text = "Search";
            this.btnGTId.Click += new System.EventHandler(this.btnGTId_Click);
            // 
            // btnWHSearch
            // 
            this.btnWHSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnWHSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnWHSearch.Location = new System.Drawing.Point(289, 50);
            this.btnWHSearch.Name = "btnWHSearch";
            this.btnWHSearch.Size = new System.Drawing.Size(57, 32);
            this.btnWHSearch.TabIndex = 6;
            this.btnWHSearch.Text = "Search";
            this.btnWHSearch.Visible = false;
            this.btnWHSearch.Click += new System.EventHandler(this.btnWHSearch_Click);
            // 
            // btnVendorSearch
            // 
            this.btnVendorSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnVendorSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnVendorSearch.Location = new System.Drawing.Point(289, 12);
            this.btnVendorSearch.Name = "btnVendorSearch";
            this.btnVendorSearch.Size = new System.Drawing.Size(57, 32);
            this.btnVendorSearch.TabIndex = 3;
            this.btnVendorSearch.Text = "Search";
            this.btnVendorSearch.Click += new System.EventHandler(this.btnVendorSearch_Click);
            // 
            // lblGTId
            // 
            this.lblGTId.AutoSize = true;
            this.lblGTId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblGTId.Location = new System.Drawing.Point(358, 52);
            this.lblGTId.Name = "lblGTId";
            this.lblGTId.Size = new System.Drawing.Size(114, 13);
            this.lblGTId.TabIndex = 7;
            this.lblGTId.Text = "Goods Transferred Id:";
            // 
            // txtGTId
            // 
            this.txtGTId.BackColor = System.Drawing.SystemColors.Control;
            this.txtGTId.Location = new System.Drawing.Point(477, 49);
            this.txtGTId.MaxLength = 60;
            this.txtGTId.Name = "txtGTId";
            this.txtGTId.ReadOnly = true;
            this.txtGTId.Size = new System.Drawing.Size(170, 21);
            this.txtGTId.TabIndex = 8;
            this.txtGTId.TextChanged += new System.EventHandler(this.txtGTId_TextChanged);
            // 
            // txtWarehouse
            // 
            this.txtWarehouse.BackColor = System.Drawing.SystemColors.Control;
            this.txtWarehouse.Enabled = false;
            this.txtWarehouse.Location = new System.Drawing.Point(113, 50);
            this.txtWarehouse.MaxLength = 20;
            this.txtWarehouse.Name = "txtWarehouse";
            this.txtWarehouse.Size = new System.Drawing.Size(170, 21);
            this.txtWarehouse.TabIndex = 5;
            this.txtWarehouse.TextChanged += new System.EventHandler(this.txtWarehouse_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Warehouse:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(358, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 89;
            this.label1.Text = "Vendor Name:";
            // 
            // txtVendorName
            // 
            this.txtVendorName.BackColor = System.Drawing.SystemColors.Control;
            this.txtVendorName.Enabled = false;
            this.txtVendorName.Location = new System.Drawing.Point(477, 12);
            this.txtVendorName.MaxLength = 60;
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(170, 21);
            this.txtVendorName.TabIndex = 89;
            // 
            // txtVendorAc
            // 
            this.txtVendorAc.BackColor = System.Drawing.SystemColors.Control;
            this.txtVendorAc.Location = new System.Drawing.Point(113, 13);
            this.txtVendorAc.MaxLength = 20;
            this.txtVendorAc.Name = "txtVendorAc";
            this.txtVendorAc.ReadOnly = true;
            this.txtVendorAc.Size = new System.Drawing.Size(170, 21);
            this.txtVendorAc.TabIndex = 2;
            this.txtVendorAc.TextChanged += new System.EventHandler(this.txtVendorAc_TextChanged);
            // 
            // lblVendorAccount
            // 
            this.lblVendorAccount.AutoSize = true;
            this.lblVendorAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblVendorAccount.Location = new System.Drawing.Point(6, 15);
            this.lblVendorAccount.Name = "lblVendorAccount";
            this.lblVendorAccount.Size = new System.Drawing.Size(87, 13);
            this.lblVendorAccount.TabIndex = 1;
            this.lblVendorAccount.Text = "Vendor Account:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(270, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(146, 32);
            this.lblTitle.TabIndex = 12;
            this.lblTitle.Text = "FG Received";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnAcknowledge);
            this.panel1.Controls.Add(this.txtAckSKUNo);
            this.panel1.Controls.Add(this.lblAck);
            this.panel1.Controls.Add(this.lblAckSKUNo);
            this.panel1.Location = new System.Drawing.Point(10, 200);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 64);
            this.panel1.TabIndex = 13;
            // 
            // btnAcknowledge
            // 
            this.btnAcknowledge.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAcknowledge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnAcknowledge.Location = new System.Drawing.Point(291, 22);
            this.btnAcknowledge.Name = "btnAcknowledge";
            this.btnAcknowledge.Size = new System.Drawing.Size(109, 31);
            this.btnAcknowledge.TabIndex = 16;
            this.btnAcknowledge.Text = "Acknowledge";
            this.btnAcknowledge.UseVisualStyleBackColor = true;
            this.btnAcknowledge.Click += new System.EventHandler(this.btnAcknowledge_Click);
            // 
            // txtAckSKUNo
            // 
            this.txtAckSKUNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtAckSKUNo.Location = new System.Drawing.Point(111, 26);
            this.txtAckSKUNo.MaxLength = 20;
            this.txtAckSKUNo.Name = "txtAckSKUNo";
            this.txtAckSKUNo.Size = new System.Drawing.Size(170, 21);
            this.txtAckSKUNo.TabIndex = 11;
            this.txtAckSKUNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtAckSKUNo_KeyDown);
            // 
            // lblAck
            // 
            this.lblAck.AutoSize = true;
            this.lblAck.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAck.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblAck.Location = new System.Drawing.Point(4, 6);
            this.lblAck.Name = "lblAck";
            this.lblAck.Size = new System.Drawing.Size(107, 13);
            this.lblAck.TabIndex = 185;
            this.lblAck.Text = "Acknowledge SKU";
            // 
            // lblAckSKUNo
            // 
            this.lblAckSKUNo.AutoSize = true;
            this.lblAckSKUNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblAckSKUNo.Location = new System.Drawing.Point(4, 28);
            this.lblAckSKUNo.Name = "lblAckSKUNo";
            this.lblAckSKUNo.Size = new System.Drawing.Size(46, 13);
            this.lblAckSKUNo.TabIndex = 10;
            this.lblAckSKUNo.Text = "SKU No:";
            // 
            // panel3
            // 
            this.panel3.AutoScroll = true;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.grItems);
            this.panel3.Location = new System.Drawing.Point(9, 272);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(718, 171);
            this.panel3.TabIndex = 14;
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
            this.grItems.Size = new System.Drawing.Size(711, 163);
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
            this.ColWONo,
            this.ColWOSrlNo,
            this.ColItemNo,
            this.ColConfig,
            this.ColSize,
            this.ColPcs,
            this.ColQty,
            this.ColAcknowledged});
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
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
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
            // ColWONo
            // 
            this.ColWONo.Caption = "WO No";
            this.ColWONo.FieldName = "WOId";
            this.ColWONo.Name = "ColWONo";
            this.ColWONo.Visible = true;
            this.ColWONo.VisibleIndex = 1;
            this.ColWONo.Width = 93;
            // 
            // ColWOSrlNo
            // 
            this.ColWOSrlNo.Caption = "WO Srl No";
            this.ColWOSrlNo.FieldName = "wosrl";
            this.ColWOSrlNo.Name = "ColWOSrlNo";
            this.ColWOSrlNo.Visible = true;
            this.ColWOSrlNo.VisibleIndex = 2;
            this.ColWOSrlNo.Width = 84;
            // 
            // ColItemNo
            // 
            this.ColItemNo.Caption = "Item No";
            this.ColItemNo.FieldName = "ItemId";
            this.ColItemNo.Name = "ColItemNo";
            this.ColItemNo.Visible = true;
            this.ColItemNo.VisibleIndex = 3;
            this.ColItemNo.Width = 86;
            // 
            // ColConfig
            // 
            this.ColConfig.Caption = "Config";
            this.ColConfig.FieldName = "EcoResConfigurationName";
            this.ColConfig.Name = "ColConfig";
            this.ColConfig.Visible = true;
            this.ColConfig.VisibleIndex = 4;
            this.ColConfig.Width = 58;
            // 
            // ColSize
            // 
            this.ColSize.Caption = "Size";
            this.ColSize.FieldName = "inventsizeid";
            this.ColSize.Name = "ColSize";
            this.ColSize.Visible = true;
            this.ColSize.VisibleIndex = 5;
            this.ColSize.Width = 45;
            // 
            // ColPcs
            // 
            this.ColPcs.Caption = "Pcs";
            this.ColPcs.FieldName = "Pdscwqty";
            this.ColPcs.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
            this.ColPcs.Name = "ColPcs";
            this.ColPcs.Visible = true;
            this.ColPcs.VisibleIndex = 6;
            this.ColPcs.Width = 54;
            // 
            // ColQty
            // 
            this.ColQty.Caption = "Qty";
            this.ColQty.FieldName = "qty";
            this.ColQty.Name = "ColQty";
            this.ColQty.Visible = true;
            this.ColQty.VisibleIndex = 7;
            this.ColQty.Width = 61;
            // 
            // ColAcknowledged
            // 
            this.ColAcknowledged.Caption = "Acknowledged";
            this.ColAcknowledged.FieldName = "Acknowledged";
            this.ColAcknowledged.Name = "ColAcknowledged";
            this.ColAcknowledged.Visible = true;
            this.ColAcknowledged.VisibleIndex = 8;
            this.ColAcknowledged.Width = 117;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(370, 484);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 31);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPost
            // 
            this.btnPost.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnPost.Location = new System.Drawing.Point(280, 484);
            this.btnPost.Name = "btnPost";
            this.btnPost.Size = new System.Drawing.Size(75, 31);
            this.btnPost.TabIndex = 15;
            this.btnPost.Text = "Post";
            this.btnPost.UseVisualStyleBackColor = true;
            this.btnPost.Click += new System.EventHandler(this.btnPost_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.lblTotQty);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.lblTotNoOfSKU);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.lblTotSetOf);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Location = new System.Drawing.Point(9, 447);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(718, 24);
            this.panel4.TabIndex = 16;
            // 
            // lblTotQty
            // 
            this.lblTotQty.AutoSize = true;
            this.lblTotQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotQty.Location = new System.Drawing.Point(648, 6);
            this.lblTotQty.Name = "lblTotQty";
            this.lblTotQty.Size = new System.Drawing.Size(13, 13);
            this.lblTotQty.TabIndex = 198;
            this.lblTotQty.Text = "..";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(353, 6);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(69, 13);
            this.label6.TabIndex = 197;
            this.label6.Text = "Total Setof";
            // 
            // lblTotNoOfSKU
            // 
            this.lblTotNoOfSKU.AutoSize = true;
            this.lblTotNoOfSKU.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotNoOfSKU.Location = new System.Drawing.Point(104, 6);
            this.lblTotNoOfSKU.Name = "lblTotNoOfSKU";
            this.lblTotNoOfSKU.Size = new System.Drawing.Size(13, 13);
            this.lblTotNoOfSKU.TabIndex = 196;
            this.lblTotNoOfSKU.Text = "..";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(571, 6);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 195;
            this.label4.Text = "Total Qty.";
            // 
            // lblTotSetOf
            // 
            this.lblTotSetOf.AutoSize = true;
            this.lblTotSetOf.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotSetOf.Location = new System.Drawing.Point(435, 6);
            this.lblTotSetOf.Name = "lblTotSetOf";
            this.lblTotSetOf.Size = new System.Drawing.Size(13, 13);
            this.lblTotSetOf.TabIndex = 194;
            this.lblTotSetOf.Text = "..";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(2, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 193;
            this.label3.Text = "Total No. Of SKU";
            // 
            // frmFGReceived
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 518);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.btnPost);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel2);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmFGReceived";
            this.Text = "FG Received";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtVendorName;
        public System.Windows.Forms.TextBox txtVendorAc;
        private System.Windows.Forms.Label lblVendorAccount;
        private System.Windows.Forms.Label lblTitle;
        public System.Windows.Forms.TextBox txtWarehouse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblGTId;
        public System.Windows.Forms.TextBox txtGTId;
        private DevExpress.XtraEditors.SimpleButton btnVendorSearch;
        private DevExpress.XtraEditors.SimpleButton btnGTId;
        private DevExpress.XtraEditors.SimpleButton btnWHSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn ColSKUNo;
        private DevExpress.XtraGrid.Columns.GridColumn ColWONo;
        private DevExpress.XtraGrid.Columns.GridColumn ColItemNo;
        private DevExpress.XtraGrid.Columns.GridColumn ColConfig;
        private DevExpress.XtraGrid.Columns.GridColumn ColSize;
        private DevExpress.XtraGrid.Columns.GridColumn ColPcs;
        private DevExpress.XtraGrid.Columns.GridColumn ColQty;
        private DevExpress.XtraGrid.Columns.GridColumn ColAcknowledged;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblAck;
        public System.Windows.Forms.TextBox txtAckSKUNo;
        private System.Windows.Forms.Label lblAckSKUNo;
        private System.Windows.Forms.Button btnPost;
        private System.Windows.Forms.Button btnAcknowledge;
        private DevExpress.XtraGrid.Columns.GridColumn ColWOSrlNo;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Label lblTotQty;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTotNoOfSKU;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotSetOf;
        private System.Windows.Forms.Label label3;

    }
}