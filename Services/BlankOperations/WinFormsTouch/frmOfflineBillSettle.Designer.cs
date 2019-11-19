namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmOfflineBillSettle
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
            this.btnAddItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtSettleBill = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRefInvoiceNo = new System.Windows.Forms.TextBox();
            this.lblRefInvoiceNo = new System.Windows.Forms.Label();
            this.btnSearchBill = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtCustGSTIN = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtCustLoyalty = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtCustMobile = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtCustAadhar = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtCustPAN = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtCustAcc = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.grbStoneWt = new System.Windows.Forms.GroupBox();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colReceiptId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustAccount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustPAN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAadhar = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustMobile = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustLoyaltyNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustGSTIN = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colISSETTLE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSettledInvNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSettledDate = new DevExpress.XtraGrid.Columns.GridColumn();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.grbStoneWt.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddItem.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddItem.Appearance.Options.UseFont = true;
            this.btnAddItem.Location = new System.Drawing.Point(584, 131);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(116, 42);
            this.btnAddItem.TabIndex = 226;
            this.btnAddItem.Text = "Add Item";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSettleBill);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtRefInvoiceNo);
            this.groupBox1.Controls.Add(this.lblRefInvoiceNo);
            this.groupBox1.Controls.Add(this.btnSearchBill);
            this.groupBox1.Controls.Add(this.txtCustGSTIN);
            this.groupBox1.Controls.Add(this.btnAddItem);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.txtCustLoyalty);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.txtCustMobile);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.txtCustAadhar);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.txtCustPAN);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtAddress);
            this.groupBox1.Controls.Add(this.txtCustName);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtCustAcc);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.Green;
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(707, 179);
            this.groupBox1.TabIndex = 227;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Customer Info";
            // 
            // txtSettleBill
            // 
            this.txtSettleBill.BackColor = System.Drawing.SystemColors.Window;
            this.txtSettleBill.Location = new System.Drawing.Point(322, 152);
            this.txtSettleBill.MaxLength = 20;
            this.txtSettleBill.Name = "txtSettleBill";
            this.txtSettleBill.Size = new System.Drawing.Size(123, 21);
            this.txtSettleBill.TabIndex = 230;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(214, 155);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 232;
            this.label2.Text = "Settlement Bill No";
            // 
            // txtRefInvoiceNo
            // 
            this.txtRefInvoiceNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtRefInvoiceNo.Enabled = false;
            this.txtRefInvoiceNo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRefInvoiceNo.Location = new System.Drawing.Point(85, 20);
            this.txtRefInvoiceNo.MaxLength = 20;
            this.txtRefInvoiceNo.Name = "txtRefInvoiceNo";
            this.txtRefInvoiceNo.ReadOnly = true;
            this.txtRefInvoiceNo.Size = new System.Drawing.Size(123, 20);
            this.txtRefInvoiceNo.TabIndex = 228;
            // 
            // lblRefInvoiceNo
            // 
            this.lblRefInvoiceNo.AutoSize = true;
            this.lblRefInvoiceNo.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Bold);
            this.lblRefInvoiceNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblRefInvoiceNo.Location = new System.Drawing.Point(10, 19);
            this.lblRefInvoiceNo.Name = "lblRefInvoiceNo";
            this.lblRefInvoiceNo.Size = new System.Drawing.Size(64, 18);
            this.lblRefInvoiceNo.TabIndex = 229;
            this.lblRefInvoiceNo.Text = "Invoice";
            // 
            // btnSearchBill
            // 
            this.btnSearchBill.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchBill.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchBill.Appearance.Options.UseFont = true;
            this.btnSearchBill.Location = new System.Drawing.Point(584, 75);
            this.btnSearchBill.Name = "btnSearchBill";
            this.btnSearchBill.Size = new System.Drawing.Size(116, 42);
            this.btnSearchBill.TabIndex = 227;
            this.btnSearchBill.Text = "Search Bill";
            this.btnSearchBill.Click += new System.EventHandler(this.btnSearchBill_Click);
            // 
            // txtCustGSTIN
            // 
            this.txtCustGSTIN.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustGSTIN.Enabled = false;
            this.txtCustGSTIN.Location = new System.Drawing.Point(322, 48);
            this.txtCustGSTIN.MaxLength = 20;
            this.txtCustGSTIN.Name = "txtCustGSTIN";
            this.txtCustGSTIN.Size = new System.Drawing.Size(123, 21);
            this.txtCustGSTIN.TabIndex = 6;
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label23.Location = new System.Drawing.Point(214, 48);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(41, 13);
            this.label23.TabIndex = 208;
            this.label23.Text = "GSTIN";
            // 
            // txtCustLoyalty
            // 
            this.txtCustLoyalty.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustLoyalty.Enabled = false;
            this.txtCustLoyalty.Location = new System.Drawing.Point(322, 22);
            this.txtCustLoyalty.MaxLength = 20;
            this.txtCustLoyalty.Name = "txtCustLoyalty";
            this.txtCustLoyalty.Size = new System.Drawing.Size(123, 21);
            this.txtCustLoyalty.TabIndex = 5;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label22.Location = new System.Drawing.Point(214, 22);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(49, 13);
            this.label22.TabIndex = 206;
            this.label22.Text = "Loyalty";
            // 
            // txtCustMobile
            // 
            this.txtCustMobile.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustMobile.Enabled = false;
            this.txtCustMobile.Location = new System.Drawing.Point(85, 151);
            this.txtCustMobile.MaxLength = 13;
            this.txtCustMobile.Name = "txtCustMobile";
            this.txtCustMobile.Size = new System.Drawing.Size(123, 21);
            this.txtCustMobile.TabIndex = 4;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label21.Location = new System.Drawing.Point(10, 154);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(44, 13);
            this.label21.TabIndex = 204;
            this.label21.Text = "Mobile";
            // 
            // txtCustAadhar
            // 
            this.txtCustAadhar.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustAadhar.Enabled = false;
            this.txtCustAadhar.Location = new System.Drawing.Point(85, 124);
            this.txtCustAadhar.MaxLength = 15;
            this.txtCustAadhar.Name = "txtCustAadhar";
            this.txtCustAadhar.Size = new System.Drawing.Size(123, 21);
            this.txtCustAadhar.TabIndex = 3;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label18.Location = new System.Drawing.Point(10, 127);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(55, 13);
            this.label18.TabIndex = 202;
            this.label18.Text = "AADHAR";
            // 
            // txtCustPAN
            // 
            this.txtCustPAN.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustPAN.Enabled = false;
            this.txtCustPAN.Location = new System.Drawing.Point(85, 98);
            this.txtCustPAN.MaxLength = 10;
            this.txtCustPAN.Name = "txtCustPAN";
            this.txtCustPAN.Size = new System.Drawing.Size(123, 21);
            this.txtCustPAN.TabIndex = 2;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label17.Location = new System.Drawing.Point(10, 101);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(29, 13);
            this.label17.TabIndex = 200;
            this.label17.Text = "PAN";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label16.Location = new System.Drawing.Point(451, 25);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 13);
            this.label16.TabIndex = 198;
            this.label16.Text = "Address";
            // 
            // txtAddress
            // 
            this.txtAddress.Enabled = false;
            this.txtAddress.Location = new System.Drawing.Point(510, 22);
            this.txtAddress.MaxLength = 250;
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAddress.Size = new System.Drawing.Size(190, 47);
            this.txtAddress.TabIndex = 7;
            // 
            // txtCustName
            // 
            this.txtCustName.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustName.Enabled = false;
            this.txtCustName.Location = new System.Drawing.Point(85, 71);
            this.txtCustName.MaxLength = 60;
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(123, 21);
            this.txtCustName.TabIndex = 1;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label15.Location = new System.Drawing.Point(10, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(39, 13);
            this.label15.TabIndex = 196;
            this.label15.Text = "Name";
            // 
            // txtCustAcc
            // 
            this.txtCustAcc.BackColor = System.Drawing.SystemColors.Window;
            this.txtCustAcc.Enabled = false;
            this.txtCustAcc.Location = new System.Drawing.Point(85, 45);
            this.txtCustAcc.MaxLength = 20;
            this.txtCustAcc.Name = "txtCustAcc";
            this.txtCustAcc.Size = new System.Drawing.Size(123, 21);
            this.txtCustAcc.TabIndex = 0;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label14.Location = new System.Drawing.Point(10, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(70, 13);
            this.label14.TabIndex = 194;
            this.label14.Text = "Account No";
            // 
            // grbStoneWt
            // 
            this.grbStoneWt.Controls.Add(this.btnSubmit);
            this.grbStoneWt.Controls.Add(this.btnClose);
            this.grbStoneWt.Controls.Add(this.btnDelete);
            this.grbStoneWt.Controls.Add(this.grItems);
            this.grbStoneWt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.grbStoneWt.ForeColor = System.Drawing.Color.Green;
            this.grbStoneWt.Location = new System.Drawing.Point(12, 197);
            this.grbStoneWt.Name = "grbStoneWt";
            this.grbStoneWt.Size = new System.Drawing.Size(707, 220);
            this.grbStoneWt.TabIndex = 228;
            this.grbStoneWt.TabStop = false;
            this.grbStoneWt.Text = "Item List";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(510, 169);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(93, 42);
            this.btnSubmit.TabIndex = 202;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Location = new System.Drawing.Point(610, 169);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 42);
            this.btnClose.TabIndex = 37;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(352, 169);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(93, 42);
            this.btnDelete.TabIndex = 38;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(6, 19);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(695, 142);
            this.grItems.TabIndex = 201;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.HeaderPanel.Options.UseForeColor = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colReceiptId,
            this.colCustAccount,
            this.colCustName,
            this.colCustPAN,
            this.colAadhar,
            this.colCustMobile,
            this.colCustLoyaltyNo,
            this.colCustGSTIN,
            this.colCustAddress,
            this.colISSETTLE,
            this.colSettledInvNo,
            this.colSettledDate});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollStep = 1;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
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
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
            // 
            // colReceiptId
            // 
            this.colReceiptId.Caption = "Receipt Id";
            this.colReceiptId.FieldName = "ReceiptId";
            this.colReceiptId.Name = "colReceiptId";
            this.colReceiptId.Visible = true;
            this.colReceiptId.VisibleIndex = 0;
            // 
            // colCustAccount
            // 
            this.colCustAccount.Caption = "Cust Account";
            this.colCustAccount.FieldName = "CustAccount";
            this.colCustAccount.Name = "colCustAccount";
            this.colCustAccount.Visible = true;
            this.colCustAccount.VisibleIndex = 1;
            // 
            // colCustName
            // 
            this.colCustName.Caption = "Cust Name";
            this.colCustName.FieldName = "CustName";
            this.colCustName.Name = "colCustName";
            this.colCustName.Visible = true;
            this.colCustName.VisibleIndex = 2;
            // 
            // colCustPAN
            // 
            this.colCustPAN.Caption = "Cust PAN";
            this.colCustPAN.FieldName = "CustPAN";
            this.colCustPAN.Name = "colCustPAN";
            this.colCustPAN.Visible = true;
            this.colCustPAN.VisibleIndex = 3;
            // 
            // colAadhar
            // 
            this.colAadhar.Caption = "Cust Aadhar";
            this.colAadhar.FieldName = "CustAadhar";
            this.colAadhar.Name = "colAadhar";
            this.colAadhar.Visible = true;
            this.colAadhar.VisibleIndex = 4;
            // 
            // colCustMobile
            // 
            this.colCustMobile.Caption = "Cust Mobile";
            this.colCustMobile.FieldName = "CustMobile";
            this.colCustMobile.Name = "colCustMobile";
            this.colCustMobile.Visible = true;
            this.colCustMobile.VisibleIndex = 5;
            // 
            // colCustLoyaltyNo
            // 
            this.colCustLoyaltyNo.Caption = "Cust Loyalty No";
            this.colCustLoyaltyNo.FieldName = "CustLoyaltyNo";
            this.colCustLoyaltyNo.Name = "colCustLoyaltyNo";
            this.colCustLoyaltyNo.Visible = true;
            this.colCustLoyaltyNo.VisibleIndex = 6;
            // 
            // colCustGSTIN
            // 
            this.colCustGSTIN.Caption = "Cust GSTIN";
            this.colCustGSTIN.FieldName = "CustGSTIN";
            this.colCustGSTIN.Name = "colCustGSTIN";
            this.colCustGSTIN.Visible = true;
            this.colCustGSTIN.VisibleIndex = 7;
            // 
            // colCustAddress
            // 
            this.colCustAddress.Caption = "Cust Address";
            this.colCustAddress.FieldName = "CustAddress";
            this.colCustAddress.Name = "colCustAddress";
            this.colCustAddress.Visible = true;
            this.colCustAddress.VisibleIndex = 8;
            // 
            // colISSETTLE
            // 
            this.colISSETTLE.Caption = "Settled";
            this.colISSETTLE.FieldName = "ISSETTLED";
            this.colISSETTLE.Name = "colISSETTLE";
            this.colISSETTLE.Visible = true;
            this.colISSETTLE.VisibleIndex = 9;
            // 
            // colSettledInvNo
            // 
            this.colSettledInvNo.Caption = "Settled Inv No";
            this.colSettledInvNo.FieldName = "SettledInvNo";
            this.colSettledInvNo.Name = "colSettledInvNo";
            this.colSettledInvNo.Visible = true;
            this.colSettledInvNo.VisibleIndex = 10;
            // 
            // colSettledDate
            // 
            this.colSettledDate.Caption = "Settled Date";
            this.colSettledDate.FieldName = "SettledDate";
            this.colSettledDate.Name = "colSettledDate";
            this.colSettledDate.Visible = true;
            this.colSettledDate.VisibleIndex = 11;
            // 
            // frmOfflineBillSettle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(734, 429);
            this.Controls.Add(this.grbStoneWt);
            this.Controls.Add(this.groupBox1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmOfflineBillSettle";
            this.Text = "Offline Bill Settle";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grbStoneWt.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddItem;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.TextBox txtCustGSTIN;
        private System.Windows.Forms.Label label23;
        public System.Windows.Forms.TextBox txtCustLoyalty;
        private System.Windows.Forms.Label label22;
        public System.Windows.Forms.TextBox txtCustMobile;
        private System.Windows.Forms.Label label21;
        public System.Windows.Forms.TextBox txtCustAadhar;
        private System.Windows.Forms.Label label18;
        public System.Windows.Forms.TextBox txtCustPAN;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtAddress;
        public System.Windows.Forms.TextBox txtCustName;
        private System.Windows.Forms.Label label15;
        public System.Windows.Forms.TextBox txtCustAcc;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.GroupBox grbStoneWt;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchBill;
        public System.Windows.Forms.TextBox txtRefInvoiceNo;
        private System.Windows.Forms.Label lblRefInvoiceNo;
        public System.Windows.Forms.TextBox txtSettleBill;
        private System.Windows.Forms.Label label2;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private DevExpress.XtraGrid.Columns.GridColumn colReceiptId;
        private DevExpress.XtraGrid.Columns.GridColumn colCustAccount;
        private DevExpress.XtraGrid.Columns.GridColumn colCustName;
        private DevExpress.XtraGrid.Columns.GridColumn colCustMobile;
        private DevExpress.XtraGrid.Columns.GridColumn colCustLoyaltyNo;
        private DevExpress.XtraGrid.Columns.GridColumn colCustAddress;
        private DevExpress.XtraGrid.Columns.GridColumn colISSETTLE;
        private DevExpress.XtraGrid.Columns.GridColumn colSettledInvNo;
        private DevExpress.XtraGrid.Columns.GridColumn colSettledDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCustPAN;
        private DevExpress.XtraGrid.Columns.GridColumn colAadhar;
        private DevExpress.XtraGrid.Columns.GridColumn colCustGSTIN;
    }
}