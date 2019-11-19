namespace BlankOperations
{
    partial class frmRepairIssueReceive
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.cmbLine = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnEnter = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.lblRepairId = new System.Windows.Forms.Label();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtRepairId = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnKarigarSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtKarigar = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnCustomerSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtCustomerAccount = new System.Windows.Forms.TextBox();
            this.lblCustomerAccount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRepairType = new System.Windows.Forms.ComboBox();
            this.dtTransDate = new System.Windows.Forms.DateTimePicker();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.txtIssueRecNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
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
            this.btnSubmit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.cmbLine);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.btnEnter);
            this.panel3.Controls.Add(this.btnClear);
            this.panel3.Controls.Add(this.lblRepairId);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.txtRepairId);
            this.panel3.Location = new System.Drawing.Point(6, 190);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(792, 53);
            this.panel3.TabIndex = 23;
            // 
            // cmbLine
            // 
            this.cmbLine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLine.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbLine.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbLine.FormattingEnabled = true;
            this.cmbLine.Location = new System.Drawing.Point(399, 14);
            this.cmbLine.Name = "cmbLine";
            this.cmbLine.Size = new System.Drawing.Size(76, 24);
            this.cmbLine.TabIndex = 181;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(351, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 180;
            this.label1.Text = "Line No";
            // 
            // btnEnter
            // 
            this.btnEnter.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.down;
            this.btnEnter.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnEnter.Location = new System.Drawing.Point(618, 12);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(152, 32);
            this.btnEnter.TabIndex = 179;
            this.btnEnter.Text = "Clear";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(248, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 179;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblRepairId
            // 
            this.lblRepairId.AutoSize = true;
            this.lblRepairId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRepairId.Location = new System.Drawing.Point(7, 20);
            this.lblRepairId.Name = "lblRepairId";
            this.lblRepairId.Size = new System.Drawing.Size(51, 13);
            this.lblRepairId.TabIndex = 7;
            this.lblRepairId.Text = "Repair Id";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(185, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 32);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtRepairId
            // 
            this.txtRepairId.BackColor = System.Drawing.SystemColors.Control;
            this.txtRepairId.Enabled = false;
            this.txtRepairId.Location = new System.Drawing.Point(63, 17);
            this.txtRepairId.MaxLength = 60;
            this.txtRepairId.Name = "txtRepairId";
            this.txtRepairId.Size = new System.Drawing.Size(119, 21);
            this.txtRepairId.TabIndex = 8;
            this.txtRepairId.TextChanged += new System.EventHandler(this.txtRepairId_TextChanged);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnKarigarSearch);
            this.panel2.Controls.Add(this.txtKarigar);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.btnCustomerSearch);
            this.panel2.Controls.Add(this.txtPhoneNumber);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.txtCustomerAddress);
            this.panel2.Controls.Add(this.lblCustomerAddress);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtCustomerName);
            this.panel2.Controls.Add(this.txtCustomerAccount);
            this.panel2.Controls.Add(this.lblCustomerAccount);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.cmbRepairType);
            this.panel2.Controls.Add(this.dtTransDate);
            this.panel2.Controls.Add(this.lblOrderDate);
            this.panel2.Controls.Add(this.txtIssueRecNo);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(6, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(792, 140);
            this.panel2.TabIndex = 22;
            // 
            // btnKarigarSearch
            // 
            this.btnKarigarSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnKarigarSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnKarigarSearch.Appearance.Options.UseFont = true;
            this.btnKarigarSearch.Location = new System.Drawing.Point(618, 100);
            this.btnKarigarSearch.Name = "btnKarigarSearch";
            this.btnKarigarSearch.Size = new System.Drawing.Size(152, 36);
            this.btnKarigarSearch.TabIndex = 190;
            this.btnKarigarSearch.Text = "Karigar Search";
            this.btnKarigarSearch.Click += new System.EventHandler(this.btnKarigarSearch_Click);
            // 
            // txtKarigar
            // 
            this.txtKarigar.BackColor = System.Drawing.SystemColors.Control;
            this.txtKarigar.Enabled = false;
            this.txtKarigar.Location = new System.Drawing.Point(112, 87);
            this.txtKarigar.MaxLength = 20;
            this.txtKarigar.Name = "txtKarigar";
            this.txtKarigar.ReadOnly = true;
            this.txtKarigar.Size = new System.Drawing.Size(130, 21);
            this.txtKarigar.TabIndex = 189;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(7, 90);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 13);
            this.label6.TabIndex = 188;
            this.label6.Text = "Karigar:";
            // 
            // btnCustomerSearch
            // 
            this.btnCustomerSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCustomerSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerSearch.Appearance.Options.UseFont = true;
            this.btnCustomerSearch.Location = new System.Drawing.Point(618, 63);
            this.btnCustomerSearch.Name = "btnCustomerSearch";
            this.btnCustomerSearch.Size = new System.Drawing.Size(152, 36);
            this.btnCustomerSearch.TabIndex = 186;
            this.btnCustomerSearch.Text = "Customer Search";
            this.btnCustomerSearch.Click += new System.EventHandler(this.btnCustomerSearch_Click);
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPhoneNumber.Enabled = false;
            this.txtPhoneNumber.Location = new System.Drawing.Point(357, 62);
            this.txtPhoneNumber.MaxLength = 20;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.ReadOnly = true;
            this.txtPhoneNumber.Size = new System.Drawing.Size(202, 21);
            this.txtPhoneNumber.TabIndex = 185;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(250, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(81, 13);
            this.label5.TabIndex = 184;
            this.label5.Text = "Phone Number:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAddress.Enabled = false;
            this.txtCustomerAddress.Location = new System.Drawing.Point(357, 36);
            this.txtCustomerAddress.MaxLength = 20;
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(430, 21);
            this.txtCustomerAddress.TabIndex = 183;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAddress.Location = new System.Drawing.Point(250, 39);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAddress.TabIndex = 182;
            this.lblCustomerAddress.Text = "Customer Address:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(6, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 180;
            this.label3.Text = "Customer Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(112, 60);
            this.txtCustomerName.MaxLength = 60;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(130, 21);
            this.txtCustomerName.TabIndex = 181;
            // 
            // txtCustomerAccount
            // 
            this.txtCustomerAccount.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAccount.Enabled = false;
            this.txtCustomerAccount.Location = new System.Drawing.Point(112, 34);
            this.txtCustomerAccount.MaxLength = 20;
            this.txtCustomerAccount.Name = "txtCustomerAccount";
            this.txtCustomerAccount.ReadOnly = true;
            this.txtCustomerAccount.Size = new System.Drawing.Size(130, 21);
            this.txtCustomerAccount.TabIndex = 179;
            // 
            // lblCustomerAccount
            // 
            this.lblCustomerAccount.AutoSize = true;
            this.lblCustomerAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAccount.Location = new System.Drawing.Point(6, 36);
            this.lblCustomerAccount.Name = "lblCustomerAccount";
            this.lblCustomerAccount.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAccount.TabIndex = 178;
            this.lblCustomerAccount.Text = "Customer Account:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(587, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 177;
            this.label4.Text = "Issue/Receive :";
            // 
            // cmbRepairType
            // 
            this.cmbRepairType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRepairType.Enabled = false;
            this.cmbRepairType.FormattingEnabled = true;
            this.cmbRepairType.Location = new System.Drawing.Point(676, 8);
            this.cmbRepairType.Name = "cmbRepairType";
            this.cmbRepairType.Size = new System.Drawing.Size(111, 21);
            this.cmbRepairType.TabIndex = 176;
            // 
            // dtTransDate
            // 
            this.dtTransDate.Location = new System.Drawing.Point(357, 8);
            this.dtTransDate.Name = "dtTransDate";
            this.dtTransDate.Size = new System.Drawing.Size(202, 21);
            this.dtTransDate.TabIndex = 7;
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderDate.Location = new System.Drawing.Point(250, 8);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(34, 13);
            this.lblOrderDate.TabIndex = 6;
            this.lblOrderDate.Text = "Date:";
            // 
            // txtIssueRecNo
            // 
            this.txtIssueRecNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtIssueRecNo.Enabled = false;
            this.txtIssueRecNo.Location = new System.Drawing.Point(112, 8);
            this.txtIssueRecNo.MaxLength = 20;
            this.txtIssueRecNo.Name = "txtIssueRecNo";
            this.txtIssueRecNo.ReadOnly = true;
            this.txtIssueRecNo.Size = new System.Drawing.Size(130, 21);
            this.txtIssueRecNo.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(6, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Issue / Receive No";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(236, 3);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(245, 32);
            this.lblTitle.TabIndex = 21;
            this.lblTitle.Text = "Repair Issue / Receive";
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnDelete.Location = new System.Drawing.Point(491, 459);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(94, 43);
            this.btnDelete.TabIndex = 28;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(700, 459);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 27;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grItems);
            this.panel1.Location = new System.Drawing.Point(6, 248);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(793, 206);
            this.panel1.TabIndex = 26;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(0, 0);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(791, 204);
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
            this.gridColumn12});
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
            // gridColumn1
            // 
            this.gridColumn1.Caption = "Repair Id";
            this.gridColumn1.FieldName = "RepairId";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.Visible = true;
            this.gridColumn1.VisibleIndex = 0;
            this.gridColumn1.Width = 150;
            // 
            // gridColumn2
            // 
            this.gridColumn2.Caption = "Line";
            this.gridColumn2.FieldName = "LINENUM";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.Visible = true;
            this.gridColumn2.VisibleIndex = 1;
            this.gridColumn2.Width = 46;
            // 
            // gridColumn3
            // 
            this.gridColumn3.Caption = "Item";
            this.gridColumn3.FieldName = "ITEMID";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.Visible = true;
            this.gridColumn3.VisibleIndex = 2;
            this.gridColumn3.Width = 93;
            // 
            // gridColumn4
            // 
            this.gridColumn4.Caption = "PCS";
            this.gridColumn4.FieldName = "PCS";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.Visible = true;
            this.gridColumn4.VisibleIndex = 3;
            this.gridColumn4.Width = 46;
            // 
            // gridColumn5
            // 
            this.gridColumn5.Caption = "QTY";
            this.gridColumn5.FieldName = "QTY";
            this.gridColumn5.Name = "gridColumn5";
            this.gridColumn5.Visible = true;
            this.gridColumn5.VisibleIndex = 4;
            this.gridColumn5.Width = 46;
            // 
            // gridColumn6
            // 
            this.gridColumn6.Caption = "Config";
            this.gridColumn6.FieldName = "CONFIGID";
            this.gridColumn6.Name = "gridColumn6";
            this.gridColumn6.Visible = true;
            this.gridColumn6.VisibleIndex = 5;
            this.gridColumn6.Width = 85;
            // 
            // gridColumn7
            // 
            this.gridColumn7.Caption = "CODE";
            this.gridColumn7.FieldName = "CODE";
            this.gridColumn7.Name = "gridColumn7";
            this.gridColumn7.Visible = true;
            this.gridColumn7.VisibleIndex = 6;
            this.gridColumn7.Width = 46;
            // 
            // gridColumn8
            // 
            this.gridColumn8.Caption = "Size";
            this.gridColumn8.FieldName = "SIZEID";
            this.gridColumn8.Name = "gridColumn8";
            this.gridColumn8.Visible = true;
            this.gridColumn8.VisibleIndex = 7;
            this.gridColumn8.Width = 46;
            // 
            // gridColumn9
            // 
            this.gridColumn9.Caption = "Nett Wt";
            this.gridColumn9.FieldName = "NETTWT";
            this.gridColumn9.Name = "gridColumn9";
            this.gridColumn9.Visible = true;
            this.gridColumn9.VisibleIndex = 8;
            this.gridColumn9.Width = 60;
            // 
            // gridColumn10
            // 
            this.gridColumn10.Caption = "Batch";
            this.gridColumn10.FieldName = "BatchId";
            this.gridColumn10.Name = "gridColumn10";
            this.gridColumn10.Visible = true;
            this.gridColumn10.VisibleIndex = 9;
            this.gridColumn10.Width = 171;
            // 
            // gridColumn11
            // 
            this.gridColumn11.Caption = "Job type";
            this.gridColumn11.FieldName = "JOBTYPE";
            this.gridColumn11.Name = "gridColumn11";
            this.gridColumn11.Visible = true;
            this.gridColumn11.VisibleIndex = 10;
            // 
            // gridColumn12
            // 
            this.gridColumn12.Caption = "Bag no";
            this.gridColumn12.FieldName = "REPAIRBAGNO";
            this.gridColumn12.Name = "gridColumn12";
            this.gridColumn12.Visible = true;
            this.gridColumn12.VisibleIndex = 11;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(597, 459);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 25;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // frmRepairIssueReceive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(811, 505);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblTitle);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmRepairIssueReceive";
            this.Text = "Repair Issue Receive";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraEditors.SimpleButton btnEnter;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private System.Windows.Forms.Label lblRepairId;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        public System.Windows.Forms.TextBox txtRepairId;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.TextBox txtIssueRecNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLine;
        public System.Windows.Forms.DateTimePicker dtTransDate;
        public System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbRepairType;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        public System.Windows.Forms.Label lblCustomerAccount;
        public System.Windows.Forms.TextBox txtCustomerAddress;
        public System.Windows.Forms.Label lblCustomerAddress;
        public System.Windows.Forms.TextBox txtPhoneNumber;
        public System.Windows.Forms.Label label5;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCustomerSearch;
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
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnKarigarSearch;
        public System.Windows.Forms.TextBox txtKarigar;
        public System.Windows.Forms.Label label6;
    }
}