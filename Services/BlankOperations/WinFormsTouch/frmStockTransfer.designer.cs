namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmGetArticleWiseStock
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
            this.lblFromCounter = new DevExpress.XtraEditors.LabelControl();
            this.lblToCounter = new DevExpress.XtraEditors.LabelControl();
            this.lblDate = new DevExpress.XtraEditors.LabelControl();
            this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
            this.cmbTransferType = new System.Windows.Forms.ComboBox();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label1 = new System.Windows.Forms.Label();
            this.txtFromCounter = new DevExpress.XtraEditors.TextEdit();
            this.txtToCounter = new DevExpress.XtraEditors.TextEdit();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtItemId = new DevExpress.XtraEditors.TextEdit();
            this.btnClearProduct = new DevExpress.XtraEditors.SimpleButton();
            this.btnPOSItemSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnFromCounterClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnFromCounterSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnToCounterClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnToCounterSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.gridStockTrans = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.dtpStockTransfer = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTotSetOf = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotQty = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblTotNoOfSKU = new System.Windows.Forms.Label();
            this.btnImport = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnShipment = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnReceive = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClearInterReceipt = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearchInterReceipt = new DevExpress.XtraEditors.SimpleButton();
            this.txtInterReceiptNo = new DevExpress.XtraEditors.TextEdit();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromCounter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtToCounter.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridStockTrans)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInterReceiptNo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // lblFromCounter
            // 
            this.lblFromCounter.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold);
            this.lblFromCounter.Location = new System.Drawing.Point(12, 80);
            this.lblFromCounter.Name = "lblFromCounter";
            this.lblFromCounter.Size = new System.Drawing.Size(88, 17);
            this.lblFromCounter.TabIndex = 1;
            this.lblFromCounter.Text = "From Counter";
            // 
            // lblToCounter
            // 
            this.lblToCounter.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold);
            this.lblToCounter.Location = new System.Drawing.Point(10, 110);
            this.lblToCounter.Name = "lblToCounter";
            this.lblToCounter.Size = new System.Drawing.Size(72, 17);
            this.lblToCounter.TabIndex = 3;
            this.lblToCounter.Text = "To Counter";
            // 
            // lblDate
            // 
            this.lblDate.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold);
            this.lblDate.Location = new System.Drawing.Point(381, 81);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(30, 17);
            this.lblDate.TabIndex = 5;
            this.lblDate.Text = "Date";
            // 
            // labelControl4
            // 
            this.labelControl4.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl4.Location = new System.Drawing.Point(381, 110);
            this.labelControl4.Name = "labelControl4";
            this.labelControl4.Size = new System.Drawing.Size(88, 17);
            this.labelControl4.TabIndex = 6;
            this.labelControl4.Text = "Transfer Type";
            // 
            // cmbTransferType
            // 
            this.cmbTransferType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransferType.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbTransferType.FormattingEnabled = true;
            this.cmbTransferType.Location = new System.Drawing.Point(493, 110);
            this.cmbTransferType.Name = "cmbTransferType";
            this.cmbTransferType.Size = new System.Drawing.Size(135, 25);
            this.cmbTransferType.TabIndex = 165;
            this.cmbTransferType.SelectedIndexChanged += new System.EventHandler(this.cmbTransferType_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(382, 462);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 42);
            this.btnCancel.TabIndex = 168;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(215, 462);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(78, 42);
            this.btnClear.TabIndex = 169;
            this.btnClear.Text = "Void";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(478, 462);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(80, 42);
            this.btnCommit.TabIndex = 170;
            this.btnCommit.Text = "Commit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 30.25F);
            this.label1.Location = new System.Drawing.Point(228, -2);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(260, 55);
            this.label1.TabIndex = 171;
            this.label1.Text = "Stock Transfer";
            // 
            // txtFromCounter
            // 
            this.txtFromCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFromCounter.Enabled = false;
            this.txtFromCounter.Location = new System.Drawing.Point(109, 78);
            this.txtFromCounter.Name = "txtFromCounter";
            this.txtFromCounter.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtFromCounter.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtFromCounter.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtFromCounter.Properties.Appearance.Options.UseBackColor = true;
            this.txtFromCounter.Properties.Appearance.Options.UseFont = true;
            this.txtFromCounter.Properties.Appearance.Options.UseForeColor = true;
            this.txtFromCounter.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtFromCounter.Properties.ReadOnly = true;
            this.txtFromCounter.Size = new System.Drawing.Size(131, 24);
            this.txtFromCounter.TabIndex = 176;
            // 
            // txtToCounter
            // 
            this.txtToCounter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToCounter.Enabled = false;
            this.txtToCounter.Location = new System.Drawing.Point(109, 108);
            this.txtToCounter.Name = "txtToCounter";
            this.txtToCounter.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtToCounter.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtToCounter.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtToCounter.Properties.Appearance.Options.UseBackColor = true;
            this.txtToCounter.Properties.Appearance.Options.UseFont = true;
            this.txtToCounter.Properties.Appearance.Options.UseForeColor = true;
            this.txtToCounter.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtToCounter.Properties.ReadOnly = true;
            this.txtToCounter.Size = new System.Drawing.Size(131, 24);
            this.txtToCounter.TabIndex = 177;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtItemId);
            this.panelControl1.Controls.Add(this.btnClearProduct);
            this.panelControl1.Controls.Add(this.btnPOSItemSearch);
            this.panelControl1.Location = new System.Drawing.Point(9, 137);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(367, 44);
            this.panelControl1.TabIndex = 178;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Location = new System.Drawing.Point(5, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(49, 17);
            this.labelControl1.TabIndex = 179;
            this.labelControl1.Text = "Product";
            // 
            // txtItemId
            // 
            this.txtItemId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemId.Location = new System.Drawing.Point(73, 10);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtItemId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtItemId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtItemId.Properties.Appearance.Options.UseBackColor = true;
            this.txtItemId.Properties.Appearance.Options.UseFont = true;
            this.txtItemId.Properties.Appearance.Options.UseForeColor = true;
            this.txtItemId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtItemId.Properties.ValidateOnEnterKey = true;
            this.txtItemId.Size = new System.Drawing.Size(169, 24);
            this.txtItemId.TabIndex = 176;
            this.txtItemId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtItemId_KeyDown);
            // 
            // btnClearProduct
            // 
            this.btnClearProduct.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnClearProduct.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClearProduct.Location = new System.Drawing.Point(305, 10);
            this.btnClearProduct.Name = "btnClearProduct";
            this.btnClearProduct.Size = new System.Drawing.Size(57, 24);
            this.btnClearProduct.TabIndex = 178;
            this.btnClearProduct.Text = "Clear";
            this.btnClearProduct.Click += new System.EventHandler(this.btnClearProduct_Click);
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnPOSItemSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPOSItemSearch.Location = new System.Drawing.Point(244, 10);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(57, 26);
            this.btnPOSItemSearch.TabIndex = 177;
            this.btnPOSItemSearch.Text = "Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // btnFromCounterClear
            // 
            this.btnFromCounterClear.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnFromCounterClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnFromCounterClear.Location = new System.Drawing.Point(317, 78);
            this.btnFromCounterClear.Name = "btnFromCounterClear";
            this.btnFromCounterClear.Size = new System.Drawing.Size(57, 24);
            this.btnFromCounterClear.TabIndex = 181;
            this.btnFromCounterClear.Text = "Clear";
            this.btnFromCounterClear.Click += new System.EventHandler(this.btnFromCounterClear_Click);
            // 
            // btnFromCounterSearch
            // 
            this.btnFromCounterSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnFromCounterSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnFromCounterSearch.Location = new System.Drawing.Point(254, 77);
            this.btnFromCounterSearch.Name = "btnFromCounterSearch";
            this.btnFromCounterSearch.Size = new System.Drawing.Size(57, 25);
            this.btnFromCounterSearch.TabIndex = 180;
            this.btnFromCounterSearch.Text = "Search";
            this.btnFromCounterSearch.Click += new System.EventHandler(this.btnFromCounterSearch_Click);
            // 
            // btnToCounterClear
            // 
            this.btnToCounterClear.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnToCounterClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnToCounterClear.Location = new System.Drawing.Point(317, 107);
            this.btnToCounterClear.Name = "btnToCounterClear";
            this.btnToCounterClear.Size = new System.Drawing.Size(57, 24);
            this.btnToCounterClear.TabIndex = 183;
            this.btnToCounterClear.Text = "Clear";
            this.btnToCounterClear.Click += new System.EventHandler(this.btnToCounterClear_Click);
            // 
            // btnToCounterSearch
            // 
            this.btnToCounterSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnToCounterSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnToCounterSearch.Location = new System.Drawing.Point(254, 107);
            this.btnToCounterSearch.Name = "btnToCounterSearch";
            this.btnToCounterSearch.Size = new System.Drawing.Size(57, 24);
            this.btnToCounterSearch.TabIndex = 182;
            this.btnToCounterSearch.Text = "Search";
            this.btnToCounterSearch.Click += new System.EventHandler(this.btnToCounterSearch_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(299, 462);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(77, 42);
            this.btnDelete.TabIndex = 184;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // gridStockTrans
            // 
            this.gridStockTrans.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.gridStockTrans.Location = new System.Drawing.Point(11, 187);
            this.gridStockTrans.MainView = this.grdView;
            this.gridStockTrans.Name = "gridStockTrans";
            this.gridStockTrans.Size = new System.Drawing.Size(708, 234);
            this.gridStockTrans.TabIndex = 185;
            this.gridStockTrans.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.FixedLineWidth = 1;
            this.grdView.GridControl = this.gridStockTrans;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdView.OptionsBehavior.AutoUpdateTotalSummary = false;
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsBehavior.KeepGroupExpandedOnSorting = false;
            this.grdView.OptionsMenu.ShowSplitItem = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedRow = false;
            this.grdView.OptionsSelection.EnableAppearanceHideSelection = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            // 
            // dtpStockTransfer
            // 
            this.dtpStockTransfer.Enabled = false;
            this.dtpStockTransfer.Location = new System.Drawing.Point(493, 83);
            this.dtpStockTransfer.Name = "dtpStockTransfer";
            this.dtpStockTransfer.Size = new System.Drawing.Size(218, 21);
            this.dtpStockTransfer.TabIndex = 186;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 424);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 187;
            this.label2.Text = "Total No. Of SKU";
            // 
            // lblTotSetOf
            // 
            this.lblTotSetOf.AutoSize = true;
            this.lblTotSetOf.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotSetOf.Location = new System.Drawing.Point(446, 424);
            this.lblTotSetOf.Name = "lblTotSetOf";
            this.lblTotSetOf.Size = new System.Drawing.Size(13, 13);
            this.lblTotSetOf.TabIndex = 188;
            this.lblTotSetOf.Text = "..";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(582, 424);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 189;
            this.label4.Text = "Total Qty.";
            // 
            // lblTotQty
            // 
            this.lblTotQty.AutoSize = true;
            this.lblTotQty.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotQty.Location = new System.Drawing.Point(659, 424);
            this.lblTotQty.Name = "lblTotQty";
            this.lblTotQty.Size = new System.Drawing.Size(13, 13);
            this.lblTotQty.TabIndex = 192;
            this.lblTotQty.Text = "..";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(364, 424);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 13);
            this.label6.TabIndex = 191;
            this.label6.Text = "Total Pcs";
            // 
            // lblTotNoOfSKU
            // 
            this.lblTotNoOfSKU.AutoSize = true;
            this.lblTotNoOfSKU.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotNoOfSKU.Location = new System.Drawing.Point(115, 424);
            this.lblTotNoOfSKU.Name = "lblTotNoOfSKU";
            this.lblTotNoOfSKU.Size = new System.Drawing.Size(13, 13);
            this.lblTotNoOfSKU.TabIndex = 190;
            this.lblTotNoOfSKU.Text = "..";
            // 
            // btnImport
            // 
            this.btnImport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnImport.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImport.Appearance.Options.UseFont = true;
            this.btnImport.Location = new System.Drawing.Point(134, 462);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 42);
            this.btnImport.TabIndex = 193;
            this.btnImport.Text = "Import";
            this.btnImport.Visible = false;
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // btnShipment
            // 
            this.btnShipment.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnShipment.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnShipment.Appearance.Options.UseFont = true;
            this.btnShipment.Location = new System.Drawing.Point(563, 462);
            this.btnShipment.Name = "btnShipment";
            this.btnShipment.Size = new System.Drawing.Size(80, 42);
            this.btnShipment.TabIndex = 194;
            this.btnShipment.Text = "Ship";
            this.btnShipment.Click += new System.EventHandler(this.btnShipment_Click);
            // 
            // btnReceive
            // 
            this.btnReceive.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReceive.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReceive.Appearance.Options.UseFont = true;
            this.btnReceive.Location = new System.Drawing.Point(646, 462);
            this.btnReceive.Name = "btnReceive";
            this.btnReceive.Size = new System.Drawing.Size(80, 42);
            this.btnReceive.TabIndex = 195;
            this.btnReceive.Text = "Receive";
            this.btnReceive.Click += new System.EventHandler(this.btnReceive_Click);
            // 
            // btnClearInterReceipt
            // 
            this.btnClearInterReceipt.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnClearInterReceipt.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClearInterReceipt.Location = new System.Drawing.Point(678, 147);
            this.btnClearInterReceipt.Name = "btnClearInterReceipt";
            this.btnClearInterReceipt.Size = new System.Drawing.Size(33, 24);
            this.btnClearInterReceipt.TabIndex = 199;
            this.btnClearInterReceipt.Text = "Clear";
            this.btnClearInterReceipt.Click += new System.EventHandler(this.btnClearInterReceipt_Click);
            // 
            // btnSearchInterReceipt
            // 
            this.btnSearchInterReceipt.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearchInterReceipt.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearchInterReceipt.Location = new System.Drawing.Point(634, 147);
            this.btnSearchInterReceipt.Name = "btnSearchInterReceipt";
            this.btnSearchInterReceipt.Size = new System.Drawing.Size(38, 24);
            this.btnSearchInterReceipt.TabIndex = 198;
            this.btnSearchInterReceipt.Text = "Search";
            this.btnSearchInterReceipt.Click += new System.EventHandler(this.btnSearchInterReceipt_Click);
            // 
            // txtInterReceiptNo
            // 
            this.txtInterReceiptNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInterReceiptNo.Enabled = false;
            this.txtInterReceiptNo.Location = new System.Drawing.Point(493, 147);
            this.txtInterReceiptNo.Name = "txtInterReceiptNo";
            this.txtInterReceiptNo.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtInterReceiptNo.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtInterReceiptNo.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtInterReceiptNo.Properties.Appearance.Options.UseBackColor = true;
            this.txtInterReceiptNo.Properties.Appearance.Options.UseFont = true;
            this.txtInterReceiptNo.Properties.Appearance.Options.UseForeColor = true;
            this.txtInterReceiptNo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtInterReceiptNo.Properties.ReadOnly = true;
            this.txtInterReceiptNo.Size = new System.Drawing.Size(135, 24);
            this.txtInterReceiptNo.TabIndex = 197;
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 10F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Location = new System.Drawing.Point(381, 147);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(106, 17);
            this.labelControl2.TabIndex = 196;
            this.labelControl2.Text = "Inter Receipt No";
            // 
            // frmGetArticleWiseStock
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(736, 507);
            this.ControlBox = false;
            this.Controls.Add(this.btnClearInterReceipt);
            this.Controls.Add(this.btnSearchInterReceipt);
            this.Controls.Add(this.txtInterReceiptNo);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.btnReceive);
            this.Controls.Add(this.btnShipment);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.lblTotQty);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblTotNoOfSKU);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTotSetOf);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtpStockTransfer);
            this.Controls.Add(this.gridStockTrans);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnToCounterClear);
            this.Controls.Add(this.btnToCounterSearch);
            this.Controls.Add(this.btnFromCounterClear);
            this.Controls.Add(this.panelControl1);
            this.Controls.Add(this.btnFromCounterSearch);
            this.Controls.Add(this.txtToCounter);
            this.Controls.Add(this.txtFromCounter);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cmbTransferType);
            this.Controls.Add(this.labelControl4);
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.lblToCounter);
            this.Controls.Add(this.lblFromCounter);
            this.KeyPreview = true;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmGetArticleWiseStock";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Stock Transfer";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFromCounter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtToCounter.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridStockTrans)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtInterReceiptNo.Properties)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl lblFromCounter;
        private DevExpress.XtraEditors.LabelControl lblToCounter;
        private DevExpress.XtraEditors.LabelControl lblDate;
        private DevExpress.XtraEditors.LabelControl labelControl4;
        private System.Windows.Forms.ComboBox cmbTransferType;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraEditors.TextEdit txtFromCounter;
        private DevExpress.XtraEditors.TextEdit txtToCounter;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtItemId;
        private DevExpress.XtraEditors.SimpleButton btnClearProduct;
        private DevExpress.XtraEditors.SimpleButton btnPOSItemSearch;
        private DevExpress.XtraEditors.SimpleButton btnFromCounterClear;
        private DevExpress.XtraEditors.SimpleButton btnFromCounterSearch;
        private DevExpress.XtraEditors.SimpleButton btnToCounterClear;
        private DevExpress.XtraEditors.SimpleButton btnToCounterSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private DevExpress.XtraGrid.GridControl gridStockTrans;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private System.Windows.Forms.DateTimePicker dtpStockTransfer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTotSetOf;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotQty;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblTotNoOfSKU;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnImport;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnShipment;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnReceive;
        private DevExpress.XtraEditors.SimpleButton btnClearInterReceipt;
        private DevExpress.XtraEditors.SimpleButton btnSearchInterReceipt;
        private DevExpress.XtraEditors.TextEdit txtInterReceiptNo;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}