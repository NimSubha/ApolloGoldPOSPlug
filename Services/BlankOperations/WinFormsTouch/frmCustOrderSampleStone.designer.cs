namespace BlankOperations.WinFormsTouch
{
    partial class frmCustOrderSampleStoneStone
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
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.lblSizeId = new System.Windows.Forms.Label();
            this.cmbCode = new System.Windows.Forms.ComboBox();
            this.lblCode = new System.Windows.Forms.Label();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.lblPCS = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblUnit = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNetWt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtGrossWt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.lblRemarks = new System.Windows.Forms.Label();
            this.btnAddItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPOSItemSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label5 = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.Label();
            this.txtItemId = new System.Windows.Forms.Label();
            this.colExtendedDetails = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSTONEBATCHID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colColor = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfiguration = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPCS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQTY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNetwt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRemarks = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIsReturned = new DevExpress.XtraGrid.Columns.GridColumn();
            this.numPad1 = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.colUNITID = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 30.25F);
            this.label1.Location = new System.Drawing.Point(201, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(442, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Customer Stone Receive";
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
            // cmbSize
            // 
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbSize.Enabled = false;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Location = new System.Drawing.Point(217, 71);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(84, 21);
            this.cmbSize.TabIndex = 10;
            // 
            // lblSizeId
            // 
            this.lblSizeId.AutoSize = true;
            this.lblSizeId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblSizeId.ForeColor = System.Drawing.Color.Black;
            this.lblSizeId.Location = new System.Drawing.Point(149, 71);
            this.lblSizeId.Name = "lblSizeId";
            this.lblSizeId.Size = new System.Drawing.Size(38, 20);
            this.lblSizeId.TabIndex = 9;
            this.lblSizeId.Text = "Size";
            // 
            // cmbCode
            // 
            this.cmbCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbCode.Enabled = false;
            this.cmbCode.FormattingEnabled = true;
            this.cmbCode.Location = new System.Drawing.Point(59, 71);
            this.cmbCode.Name = "cmbCode";
            this.cmbCode.Size = new System.Drawing.Size(88, 21);
            this.cmbCode.TabIndex = 6;
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCode.ForeColor = System.Drawing.Color.Black;
            this.lblCode.Location = new System.Drawing.Point(11, 71);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(46, 20);
            this.lblCode.TabIndex = 5;
            this.lblCode.Text = "Code";
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(59, 105);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(88, 21);
            this.txtPCS.TabIndex = 14;
            this.txtPCS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
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
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblItemName.ForeColor = System.Drawing.Color.Black;
            this.lblItemName.Location = new System.Drawing.Point(11, 31);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(87, 20);
            this.lblItemName.TabIndex = 2;
            this.lblItemName.Text = "Item Name";
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblItemId.ForeColor = System.Drawing.Color.Black;
            this.lblItemId.Location = new System.Drawing.Point(11, 7);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(60, 20);
            this.lblItemId.TabIndex = 0;
            this.lblItemId.Text = "Item ID";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.cmbStyle);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblUnit);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.txtNetWt);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtGrossWt);
            this.panel1.Controls.Add(this.label4);
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
            this.panel1.Controls.Add(this.lblSizeId);
            this.panel1.Controls.Add(this.cmbSize);
            this.panel1.Controls.Add(this.txtPCS);
            this.panel1.Location = new System.Drawing.Point(12, 60);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(684, 242);
            this.panel1.TabIndex = 1;
            // 
            // cmbStyle
            // 
            this.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbStyle.Enabled = false;
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Location = new System.Drawing.Point(368, 71);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(79, 21);
            this.cmbStyle.TabIndex = 35;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(303, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 20);
            this.label3.TabIndex = 34;
            this.label3.Text = "Style";
            // 
            // lblUnit
            // 
            this.lblUnit.AutoSize = true;
            this.lblUnit.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblUnit.ForeColor = System.Drawing.Color.Black;
            this.lblUnit.Location = new System.Drawing.Point(520, 105);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Size = new System.Drawing.Size(13, 20);
            this.lblUnit.TabIndex = 33;
            this.lblUnit.Text = ".";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(461, 105);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 32;
            this.label2.Text = "Unit";
            // 
            // txtNetWt
            // 
            this.txtNetWt.Enabled = false;
            this.txtNetWt.Location = new System.Drawing.Point(368, 105);
            this.txtNetWt.MaxLength = 9;
            this.txtNetWt.Name = "txtNetWt";
            this.txtNetWt.Size = new System.Drawing.Size(79, 21);
            this.txtNetWt.TabIndex = 24;
            this.txtNetWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNetWt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStnWt_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(303, 106);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(59, 20);
            this.label7.TabIndex = 23;
            this.label7.Text = "Net Wt";
            // 
            // txtGrossWt
            // 
            this.txtGrossWt.Enabled = false;
            this.txtGrossWt.Location = new System.Drawing.Point(217, 105);
            this.txtGrossWt.MaxLength = 9;
            this.txtGrossWt.Name = "txtGrossWt";
            this.txtGrossWt.Size = new System.Drawing.Size(84, 21);
            this.txtGrossWt.TabIndex = 20;
            this.txtGrossWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrossWt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiaWt_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(148, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 20);
            this.label4.TabIndex = 19;
            this.label4.Text = "Quantity";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(88, 132);
            this.txtRemarks.MaxLength = 250;
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(359, 68);
            this.txtRemarks.TabIndex = 30;
            // 
            // lblRemarks
            // 
            this.lblRemarks.AutoSize = true;
            this.lblRemarks.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblRemarks.ForeColor = System.Drawing.Color.Black;
            this.lblRemarks.Location = new System.Drawing.Point(13, 152);
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
            this.btnAddItem.Location = new System.Drawing.Point(465, 177);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(208, 53);
            this.btnAddItem.TabIndex = 31;
            this.btnAddItem.Text = "Add Item to View Details";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPOSItemSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPOSItemSearch.Appearance.Options.UseFont = true;
            this.btnPOSItemSearch.Location = new System.Drawing.Point(465, 7);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(202, 53);
            this.btnPOSItemSearch.TabIndex = 4;
            this.btnPOSItemSearch.Text = "Product Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(13, 105);
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
            this.txtItemName.Location = new System.Drawing.Point(114, 33);
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
            this.txtItemId.Location = new System.Drawing.Point(114, 9);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Size = new System.Drawing.Size(54, 17);
            this.txtItemId.TabIndex = 1;
            this.txtItemId.Text = "Item Id";
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
            this.btnCancel.Location = new System.Drawing.Point(814, 544);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 38);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(497, 544);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 38);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(708, 544);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 38);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(602, 544);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 38);
            this.btnEdit.TabIndex = 7;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(12, 310);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(902, 228);
            this.grItems.TabIndex = 170;
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
            this.colSTONEBATCHID,
            this.colColor,
            this.colSize,
            this.colConfiguration,
            this.colStyle,
            this.colPCS,
            this.colQTY,
            this.colNetwt,
            this.colUNITID,
            this.colRemarks,
            this.colIsReturned});
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
            this.colItemId.Width = 88;
            // 
            // colSTONEBATCHID
            // 
            this.colSTONEBATCHID.Caption = "Stone Batch";
            this.colSTONEBATCHID.FieldName = "STONEBATCHID";
            this.colSTONEBATCHID.Name = "colSTONEBATCHID";
            this.colSTONEBATCHID.Visible = true;
            this.colSTONEBATCHID.VisibleIndex = 1;
            this.colSTONEBATCHID.Width = 120;
            // 
            // colColor
            // 
            this.colColor.Caption = "Code";
            this.colColor.FieldName = "COLOR";
            this.colColor.Name = "colColor";
            this.colColor.OptionsColumn.AllowEdit = false;
            this.colColor.OptionsColumn.AllowIncrementalSearch = false;
            this.colColor.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colColor.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colColor.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colColor.Visible = true;
            this.colColor.VisibleIndex = 2;
            this.colColor.Width = 83;
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
            this.colSize.VisibleIndex = 3;
            this.colSize.Width = 83;
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
            this.colStyle.Visible = true;
            this.colStyle.VisibleIndex = 4;
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
            this.colPCS.Width = 59;
            // 
            // colQTY
            // 
            this.colQTY.Caption = "Quantity";
            this.colQTY.FieldName = "QTY";
            this.colQTY.Name = "colQTY";
            this.colQTY.OptionsColumn.AllowEdit = false;
            this.colQTY.OptionsColumn.AllowIncrementalSearch = false;
            this.colQTY.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colQTY.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colQTY.Visible = true;
            this.colQTY.VisibleIndex = 6;
            this.colQTY.Width = 88;
            // 
            // colNetwt
            // 
            this.colNetwt.Caption = "Net Wt.";
            this.colNetwt.FieldName = "NETWT";
            this.colNetwt.Name = "colNetwt";
            this.colNetwt.OptionsColumn.AllowEdit = false;
            this.colNetwt.OptionsColumn.AllowIncrementalSearch = false;
            this.colNetwt.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colNetwt.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colNetwt.Visible = true;
            this.colNetwt.VisibleIndex = 7;
            this.colNetwt.Width = 89;
            // 
            // colRemarks
            // 
            this.colRemarks.Caption = "Remarks";
            this.colRemarks.FieldName = "REMARKSDTL";
            this.colRemarks.Name = "colRemarks";
            this.colRemarks.Visible = true;
            this.colRemarks.VisibleIndex = 9;
            this.colRemarks.Width = 113;
            // 
            // colIsReturned
            // 
            this.colIsReturned.Caption = "Is Returned?";
            this.colIsReturned.FieldName = "ISRETURNED";
            this.colIsReturned.Name = "colIsReturned";
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
            this.numPad1.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Quantity;
            this.numPad1.Location = new System.Drawing.Point(744, 58);
            this.numPad1.LookAndFeel.SkinName = "The Asphalt World";
            this.numPad1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.numPad1.MaskChar = "";
            this.numPad1.MaskInterval = 0;
            this.numPad1.MaxNumberOfDigits = 6;
            this.numPad1.MinimumSize = new System.Drawing.Size(170, 242);
            this.numPad1.Name = "numPad1";
            this.numPad1.NegativeMode = false;
            this.numPad1.NoOfTries = 0;
            this.numPad1.NumberOfDecimals = 2;
            this.numPad1.PromptText = "Enter Details ";
            this.numPad1.ShortcutKeysActive = false;
            this.numPad1.Size = new System.Drawing.Size(170, 242);
            this.numPad1.TabIndex = 171;
            this.numPad1.TimerEnabled = false;
            // 
            // colUNITID
            // 
            this.colUNITID.Caption = "UNIT";
            this.colUNITID.FieldName = "UNITID";
            this.colUNITID.Name = "colUNITID";
            this.colUNITID.Visible = true;
            this.colUNITID.VisibleIndex = 8;
            // 
            // frmCustOrderSampleStoneStone
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(926, 583);
            this.ControlBox = false;
            this.Controls.Add(this.numPad1);
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
            this.Name = "frmCustOrderSampleStoneStone";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Customer Stone Receive";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private DevExpress.XtraEditors.StyleController styleController;
        private System.Windows.Forms.Label label1;
        //private System.Windows.Forms.ComboBox cmbStyle;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.ComboBox cmbSize;
        private System.Windows.Forms.Label lblSizeId;
        private System.Windows.Forms.ComboBox cmbCode;
        private System.Windows.Forms.Label lblCode;
        //private System.Windows.Forms.ComboBox cmbConfig;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.Label lblPCS;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label txtItemName;
        private System.Windows.Forms.Label txtItemId;
        private System.Windows.Forms.Label label5;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPOSItemSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddItem;
        //   private DevExpress.XtraGrid.Columns.GridColumn colItemName;
        private DevExpress.XtraGrid.Columns.GridColumn colExtendedDetails;
        private DevExpress.XtraGrid.Columns.GridColumn colRowTotalAmount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label lblRemarks;
        private System.Windows.Forms.TextBox txtNetWt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtGrossWt;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colColor;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colConfiguration;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colPCS;
        private DevExpress.XtraGrid.Columns.GridColumn colQTY;
        private DevExpress.XtraGrid.Columns.GridColumn colNetwt;
        private DevExpress.XtraGrid.Columns.GridColumn colRemarks;
        private DevExpress.XtraGrid.Columns.GridColumn colIsReturned;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numPad1;
        private DevExpress.XtraGrid.Columns.GridColumn colSTONEBATCHID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.ComboBox cmbStyle;
        private System.Windows.Forms.Label label3;
        private DevExpress.XtraGrid.Columns.GridColumn colUNITID;
    }
}