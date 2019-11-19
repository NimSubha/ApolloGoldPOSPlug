namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmPDCEntry
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
            this.colPDCREFERENCE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.colMICRNO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.txtChqNo = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMICR = new System.Windows.Forms.TextBox();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAdd = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.gridCheque = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colUNIQUEID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colChqNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBank = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPDCTYPE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBank = new System.Windows.Forms.TextBox();
            this.dtpChqDtae = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new DevExpress.XtraEditors.LabelControl();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtCustomerAccount = new System.Windows.Forms.TextBox();
            this.lblCustomerAccount = new System.Windows.Forms.Label();
            this.btnSearchCustomer = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtPDCReceipt = new System.Windows.Forms.TextBox();
            this.txtPDCRef = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbPDCType = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBankS = new System.Windows.Forms.Button();
            this.btnPrint = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSM = new System.Windows.Forms.Button();
            this.txtSM = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.gridCheque)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // colPDCREFERENCE
            // 
            this.colPDCREFERENCE.Caption = "PDC Ref.";
            this.colPDCREFERENCE.FieldName = "PDCREFERENCE";
            this.colPDCREFERENCE.Name = "colPDCREFERENCE";
            this.colPDCREFERENCE.Visible = true;
            this.colPDCREFERENCE.VisibleIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(4, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 277;
            this.label2.Text = "Cheque No";
            // 
            // colMICRNO
            // 
            this.colMICRNO.Caption = "MICR No";
            this.colMICRNO.FieldName = "MICRCODE";
            this.colMICRNO.Name = "colMICRNO";
            this.colMICRNO.Visible = true;
            this.colMICRNO.VisibleIndex = 5;
            // 
            // txtChqNo
            // 
            this.txtChqNo.Location = new System.Drawing.Point(68, 32);
            this.txtChqNo.MaxLength = 6;
            this.txtChqNo.Name = "txtChqNo";
            this.txtChqNo.Size = new System.Drawing.Size(146, 21);
            this.txtChqNo.TabIndex = 4;
            this.txtChqNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtChqNo_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label8.Location = new System.Drawing.Point(471, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(43, 13);
            this.label8.TabIndex = 276;
            this.label8.Text = "Amount";
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(519, 57);
            this.txtAmount.MaxLength = 15;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(123, 21);
            this.txtAmount.TabIndex = 8;
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAmount_KeyPress);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label7.Location = new System.Drawing.Point(268, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 13);
            this.label7.TabIndex = 275;
            this.label7.Text = "MICR No";
            // 
            // txtMICR
            // 
            this.txtMICR.Location = new System.Drawing.Point(337, 32);
            this.txtMICR.MaxLength = 9;
            this.txtMICR.Name = "txtMICR";
            this.txtMICR.Size = new System.Drawing.Size(123, 21);
            this.txtMICR.TabIndex = 6;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelControl1.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.labelControl1.Location = new System.Drawing.Point(8, 22);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(65, 13);
            this.labelControl1.TabIndex = 271;
            this.labelControl1.Text = "PDC Voucher";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(374, 381);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(108, 42);
            this.btnEdit.TabIndex = 12;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdd.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Appearance.Options.UseFont = true;
            this.btnAdd.Location = new System.Drawing.Point(670, 142);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(134, 46);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gridCheque
            // 
            this.gridCheque.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.gridCheque.Location = new System.Drawing.Point(5, 206);
            this.gridCheque.MainView = this.grdView;
            this.gridCheque.Name = "gridCheque";
            this.gridCheque.Size = new System.Drawing.Size(819, 169);
            this.gridCheque.TabIndex = 266;
            this.gridCheque.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colUNIQUEID,
            this.colChqNo,
            this.colDate,
            this.colAmount,
            this.colBank,
            this.colPDCREFERENCE,
            this.colMICRNO,
            this.colPDCTYPE});
            this.grdView.FixedLineWidth = 1;
            this.grdView.GridControl = this.gridCheque;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
            this.grdView.OptionsBehavior.AutoUpdateTotalSummary = false;
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsBehavior.KeepGroupExpandedOnSorting = false;
            this.grdView.OptionsMenu.ShowSplitItem = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            // 
            // colUNIQUEID
            // 
            this.colUNIQUEID.Caption = "UNIQUEID";
            this.colUNIQUEID.FieldName = "UNIQUEID";
            this.colUNIQUEID.Name = "colUNIQUEID";
            // 
            // colChqNo
            // 
            this.colChqNo.Caption = "Cheque No";
            this.colChqNo.FieldName = "CHEQUENO";
            this.colChqNo.Name = "colChqNo";
            this.colChqNo.Visible = true;
            this.colChqNo.VisibleIndex = 0;
            // 
            // colDate
            // 
            this.colDate.AppearanceHeader.Options.UseTextOptions = true;
            this.colDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDate.Caption = "Cheque Date";
            this.colDate.DisplayFormat.FormatString = "dd-MM-yyyy";
            this.colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colDate.FieldName = "CHEQUEDATE";
            this.colDate.Name = "colDate";
            this.colDate.Visible = true;
            this.colDate.VisibleIndex = 1;
            // 
            // colAmount
            // 
            this.colAmount.Caption = "Amount";
            this.colAmount.FieldName = "CHEQUEAMOUNT";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 2;
            // 
            // colBank
            // 
            this.colBank.AppearanceHeader.Options.UseTextOptions = true;
            this.colBank.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colBank.Caption = "Bank";
            this.colBank.FieldName = "ISSUEBANK";
            this.colBank.Name = "colBank";
            this.colBank.Visible = true;
            this.colBank.VisibleIndex = 3;
            // 
            // colPDCTYPE
            // 
            this.colPDCTYPE.Caption = "PDC Type";
            this.colPDCTYPE.FieldName = "PDCTYPE";
            this.colPDCTYPE.Name = "colPDCTYPE";
            this.colPDCTYPE.Visible = true;
            this.colPDCTYPE.VisibleIndex = 6;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(488, 381);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(108, 42);
            this.btnDelete.TabIndex = 14;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(716, 381);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(108, 42);
            this.btnCommit.TabIndex = 11;
            this.btnCommit.Text = "Commit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(261, 381);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(108, 42);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "Void";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(602, 381);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 42);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(4, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 265;
            this.label5.Text = "Bank";
            // 
            // txtBank
            // 
            this.txtBank.BackColor = System.Drawing.SystemColors.Control;
            this.txtBank.Enabled = false;
            this.txtBank.Location = new System.Drawing.Point(68, 56);
            this.txtBank.MaxLength = 20;
            this.txtBank.Name = "txtBank";
            this.txtBank.Size = new System.Drawing.Size(146, 21);
            this.txtBank.TabIndex = 4;
            // 
            // dtpChqDtae
            // 
            this.dtpChqDtae.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpChqDtae.Location = new System.Drawing.Point(337, 57);
            this.dtpChqDtae.Name = "dtpChqDtae";
            this.dtpChqDtae.Size = new System.Drawing.Size(123, 21);
            this.dtpChqDtae.TabIndex = 7;
            // 
            // lblDate
            // 
            this.lblDate.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblDate.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDate.Location = new System.Drawing.Point(271, 60);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(63, 13);
            this.lblDate.TabIndex = 264;
            this.lblDate.Text = "Cheque Date";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(481, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 280;
            this.label4.Text = "Customer Name";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(581, 19);
            this.txtCustomerName.MaxLength = 60;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerName.TabIndex = 281;
            // 
            // txtCustomerAccount
            // 
            this.txtCustomerAccount.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAccount.Enabled = false;
            this.txtCustomerAccount.Location = new System.Drawing.Point(342, 19);
            this.txtCustomerAccount.MaxLength = 20;
            this.txtCustomerAccount.Name = "txtCustomerAccount";
            this.txtCustomerAccount.ReadOnly = true;
            this.txtCustomerAccount.Size = new System.Drawing.Size(128, 21);
            this.txtCustomerAccount.TabIndex = 279;
            // 
            // lblCustomerAccount
            // 
            this.lblCustomerAccount.AutoSize = true;
            this.lblCustomerAccount.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblCustomerAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAccount.Location = new System.Drawing.Point(242, 20);
            this.lblCustomerAccount.Name = "lblCustomerAccount";
            this.lblCustomerAccount.Size = new System.Drawing.Size(95, 13);
            this.lblCustomerAccount.TabIndex = 278;
            this.lblCustomerAccount.Text = "Customer Account";
            // 
            // btnSearchCustomer
            // 
            this.btnSearchCustomer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchCustomer.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchCustomer.Appearance.Options.UseFont = true;
            this.btnSearchCustomer.Location = new System.Drawing.Point(658, 53);
            this.btnSearchCustomer.Name = "btnSearchCustomer";
            this.btnSearchCustomer.Size = new System.Drawing.Size(147, 36);
            this.btnSearchCustomer.TabIndex = 2;
            this.btnSearchCustomer.Text = "Customer Search";
            this.btnSearchCustomer.Click += new System.EventHandler(this.btnSearchCustomer_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSM);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtSM);
            this.groupBox1.Controls.Add(this.txtPDCReceipt);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearchCustomer);
            this.groupBox1.Controls.Add(this.txtPDCRef);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.cmbPDCType);
            this.groupBox1.Controls.Add(this.txtCustomerAccount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.labelControl1);
            this.groupBox1.Controls.Add(this.txtCustomerName);
            this.groupBox1.Controls.Add(this.lblCustomerAccount);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox1.ForeColor = System.Drawing.Color.Green;
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(812, 97);
            this.groupBox1.TabIndex = 283;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "PDC Entry Header";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label9.Location = new System.Drawing.Point(242, 47);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 279;
            this.label9.Text = "PDC Reference";
            // 
            // txtPDCReceipt
            // 
            this.txtPDCReceipt.BackColor = System.Drawing.SystemColors.Control;
            this.txtPDCReceipt.Enabled = false;
            this.txtPDCReceipt.Location = new System.Drawing.Point(83, 19);
            this.txtPDCReceipt.MaxLength = 20;
            this.txtPDCReceipt.Name = "txtPDCReceipt";
            this.txtPDCReceipt.ReadOnly = true;
            this.txtPDCReceipt.Size = new System.Drawing.Size(146, 21);
            this.txtPDCReceipt.TabIndex = 284;
            // 
            // txtPDCRef
            // 
            this.txtPDCRef.Location = new System.Drawing.Point(342, 44);
            this.txtPDCRef.MaxLength = 20;
            this.txtPDCRef.Name = "txtPDCRef";
            this.txtPDCRef.Size = new System.Drawing.Size(128, 21);
            this.txtPDCRef.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(6, 47);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 283;
            this.label6.Text = "PDC Type";
            // 
            // cmbPDCType
            // 
            this.cmbPDCType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPDCType.FormattingEnabled = true;
            this.cmbPDCType.Location = new System.Drawing.Point(83, 44);
            this.cmbPDCType.Name = "cmbPDCType";
            this.cmbPDCType.Size = new System.Drawing.Size(146, 21);
            this.cmbPDCType.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnBankS);
            this.groupBox2.Controls.Add(this.txtChqNo);
            this.groupBox2.Controls.Add(this.lblDate);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.dtpChqDtae);
            this.groupBox2.Controls.Add(this.txtBank);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtAmount);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.txtMICR);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.Color.Green;
            this.groupBox2.Location = new System.Drawing.Point(12, 112);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(648, 88);
            this.groupBox2.TabIndex = 285;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PDC Entry Details";
            // 
            // btnBankS
            // 
            this.btnBankS.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBankS.ForeColor = System.Drawing.Color.Red;
            this.btnBankS.Location = new System.Drawing.Point(220, 57);
            this.btnBankS.Name = "btnBankS";
            this.btnBankS.Size = new System.Drawing.Size(18, 19);
            this.btnBankS.TabIndex = 5;
            this.btnBankS.Text = "S";
            this.btnBankS.UseVisualStyleBackColor = true;
            this.btnBankS.Click += new System.EventHandler(this.btnBankS_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.ForeColor = System.Drawing.Color.Red;
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.Appearance.Options.UseForeColor = true;
            this.btnPrint.Location = new System.Drawing.Point(5, 384);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(158, 39);
            this.btnPrint.TabIndex = 286;
            this.btnPrint.Text = "Re-Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSM
            // 
            this.btnSM.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSM.ForeColor = System.Drawing.Color.Red;
            this.btnSM.Location = new System.Drawing.Point(245, 70);
            this.btnSM.Name = "btnSM";
            this.btnSM.Size = new System.Drawing.Size(23, 21);
            this.btnSM.TabIndex = 3;
            this.btnSM.Text = "S";
            this.btnSM.UseVisualStyleBackColor = true;
            this.btnSM.Click += new System.EventHandler(this.btnSM_Click);
            // 
            // txtSM
            // 
            this.txtSM.BackColor = System.Drawing.SystemColors.Control;
            this.txtSM.Enabled = false;
            this.txtSM.Location = new System.Drawing.Point(83, 70);
            this.txtSM.MaxLength = 20;
            this.txtSM.Name = "txtSM";
            this.txtSM.ReadOnly = true;
            this.txtSM.Size = new System.Drawing.Size(146, 21);
            this.txtSM.TabIndex = 280;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(5, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 13);
            this.label1.TabIndex = 279;
            this.label1.Text = "Sales man:";
            // 
            // frmPDCEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(836, 435);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.gridCheque);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Name = "frmPDCEntry";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PDC Entry";
            ((System.ComponentModel.ISupportInitialize)(this.gridCheque)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.Columns.GridColumn colPDCREFERENCE;
        private System.Windows.Forms.Label label2;
        private DevExpress.XtraGrid.Columns.GridColumn colMICRNO;
        private System.Windows.Forms.TextBox txtChqNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMICR;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAdd;
        private DevExpress.XtraGrid.GridControl gridCheque;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colChqNo;
        private DevExpress.XtraGrid.Columns.GridColumn colDate;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colBank;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBank;
        private System.Windows.Forms.DateTimePicker dtpChqDtae;
        private DevExpress.XtraEditors.LabelControl lblDate;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        private System.Windows.Forms.Label lblCustomerAccount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchCustomer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbPDCType;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtPDCRef;
        public System.Windows.Forms.TextBox txtPDCReceipt;
        private DevExpress.XtraGrid.Columns.GridColumn colPDCTYPE;
        private DevExpress.XtraGrid.Columns.GridColumn colUNIQUEID;
        private System.Windows.Forms.Button btnBankS;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPrint;
        private System.Windows.Forms.Button btnSM;
        public System.Windows.Forms.TextBox txtSM;
        private System.Windows.Forms.Label label1;
    }
}