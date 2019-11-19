namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmCustomerOrderChequeInfo
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
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAdd = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblTotAmt = new DevExpress.XtraEditors.LabelControl();
            this.lblOrderNo = new DevExpress.XtraEditors.LabelControl();
            this.lblGridTotAmt = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gridCheque = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colChqNo = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBank = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBranch = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIFSC = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMICRNO = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBank = new System.Windows.Forms.TextBox();
            this.dtpChqDtae = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new DevExpress.XtraEditors.LabelControl();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIFSC = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBranch = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtMICR = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtChqNo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gridCheque)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Location = new System.Drawing.Point(491, 8);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(86, 15);
            this.labelControl2.TabIndex = 240;
            this.labelControl2.Text = "Order Amount";
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Location = new System.Drawing.Point(255, 9);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(57, 15);
            this.labelControl1.TabIndex = 239;
            this.labelControl1.Text = "Order No";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(241, 387);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(108, 42);
            this.btnEdit.TabIndex = 11;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdd.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Appearance.Options.UseFont = true;
            this.btnAdd.Location = new System.Drawing.Point(547, 91);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(154, 53);
            this.btnAdd.TabIndex = 7;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblTotAmt
            // 
            this.lblTotAmt.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblTotAmt.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblTotAmt.Location = new System.Drawing.Point(587, 4);
            this.lblTotAmt.Name = "lblTotAmt";
            this.lblTotAmt.Size = new System.Drawing.Size(14, 20);
            this.lblTotAmt.TabIndex = 238;
            this.lblTotAmt.Text = "--";
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblOrderNo.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblOrderNo.Location = new System.Drawing.Point(334, 6);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(14, 20);
            this.lblOrderNo.TabIndex = 237;
            this.lblOrderNo.Text = "--";
            // 
            // lblGridTotAmt
            // 
            this.lblGridTotAmt.AutoSize = true;
            this.lblGridTotAmt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGridTotAmt.Location = new System.Drawing.Point(655, 350);
            this.lblGridTotAmt.Name = "lblGridTotAmt";
            this.lblGridTotAmt.Size = new System.Drawing.Size(13, 13);
            this.lblGridTotAmt.TabIndex = 236;
            this.lblGridTotAmt.Text = "..";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(530, 350);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 234;
            this.label4.Text = "Total Amount.";
            // 
            // gridCheque
            // 
            this.gridCheque.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.gridCheque.Location = new System.Drawing.Point(7, 150);
            this.gridCheque.MainView = this.grdView;
            this.gridCheque.Name = "gridCheque";
            this.gridCheque.Size = new System.Drawing.Size(694, 197);
            this.gridCheque.TabIndex = 232;
            this.gridCheque.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colChqNo,
            this.colDate,
            this.colAmount,
            this.colBank,
            this.colBranch,
            this.colIFSC,
            this.colMICRNO});
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
            // colChqNo
            // 
            this.colChqNo.Caption = "Cheque No";
            this.colChqNo.FieldName = "ChequeNo";
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
            this.colDate.FieldName = "ChqDate";
            this.colDate.Name = "colDate";
            this.colDate.Visible = true;
            this.colDate.VisibleIndex = 1;
            // 
            // colAmount
            // 
            this.colAmount.Caption = "Amount";
            this.colAmount.FieldName = "ChqAmt";
            this.colAmount.Name = "colAmount";
            this.colAmount.Visible = true;
            this.colAmount.VisibleIndex = 2;
            // 
            // colBank
            // 
            this.colBank.AppearanceHeader.Options.UseTextOptions = true;
            this.colBank.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colBank.Caption = "Bank";
            this.colBank.FieldName = "Bank";
            this.colBank.Name = "colBank";
            this.colBank.Visible = true;
            this.colBank.VisibleIndex = 3;
            // 
            // colBranch
            // 
            this.colBranch.AppearanceHeader.Options.UseTextOptions = true;
            this.colBranch.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colBranch.Caption = "Branch";
            this.colBranch.FieldName = "Branch";
            this.colBranch.Name = "colBranch";
            this.colBranch.Visible = true;
            this.colBranch.VisibleIndex = 4;
            // 
            // colIFSC
            // 
            this.colIFSC.Caption = "IFSC Code";
            this.colIFSC.FieldName = "IFSCCODE";
            this.colIFSC.Name = "colIFSC";
            this.colIFSC.Visible = true;
            this.colIFSC.VisibleIndex = 5;
            // 
            // colMICRNO
            // 
            this.colMICRNO.Caption = "MICR No";
            this.colMICRNO.FieldName = "MICRNO";
            this.colMICRNO.Name = "colMICRNO";
            this.colMICRNO.Visible = true;
            this.colMICRNO.VisibleIndex = 6;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(355, 387);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(108, 42);
            this.btnDelete.TabIndex = 12;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(583, 387);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(108, 42);
            this.btnCommit.TabIndex = 9;
            this.btnCommit.Text = "Commit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(128, 387);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(108, 42);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "Void";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(469, 387);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 42);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(255, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 15);
            this.label5.TabIndex = 230;
            this.label5.Text = "Bank";
            // 
            // txtBank
            // 
            this.txtBank.Location = new System.Drawing.Point(334, 38);
            this.txtBank.MaxLength = 60;
            this.txtBank.Name = "txtBank";
            this.txtBank.Size = new System.Drawing.Size(146, 20);
            this.txtBank.TabIndex = 2;
            this.txtBank.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dtpChqDtae
            // 
            this.dtpChqDtae.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpChqDtae.Location = new System.Drawing.Point(103, 38);
            this.dtpChqDtae.Name = "dtpChqDtae";
            this.dtpChqDtae.Size = new System.Drawing.Size(146, 20);
            this.dtpChqDtae.TabIndex = 1;
            // 
            // lblDate
            // 
            this.lblDate.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.lblDate.Location = new System.Drawing.Point(11, 35);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(76, 15);
            this.lblDate.TabIndex = 228;
            this.lblDate.Text = "Cheque Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(11, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 242;
            this.label1.Text = "IFSC Code";
            // 
            // txtIFSC
            // 
            this.txtIFSC.Location = new System.Drawing.Point(103, 95);
            this.txtIFSC.MaxLength = 20;
            this.txtIFSC.Name = "txtIFSC";
            this.txtIFSC.Size = new System.Drawing.Size(146, 20);
            this.txtIFSC.TabIndex = 5;
            this.txtIFSC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(255, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 15);
            this.label3.TabIndex = 244;
            this.label3.Text = "Branch";
            // 
            // txtBranch
            // 
            this.txtBranch.Location = new System.Drawing.Point(334, 67);
            this.txtBranch.MaxLength = 60;
            this.txtBranch.Name = "txtBranch";
            this.txtBranch.Size = new System.Drawing.Size(146, 20);
            this.txtBranch.TabIndex = 4;
            this.txtBranch.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(255, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(60, 15);
            this.label7.TabIndex = 246;
            this.label7.Text = "MICR No";
            // 
            // txtMICR
            // 
            this.txtMICR.Location = new System.Drawing.Point(334, 95);
            this.txtMICR.MaxLength = 9;
            this.txtMICR.Name = "txtMICR";
            this.txtMICR.Size = new System.Drawing.Size(146, 20);
            this.txtMICR.TabIndex = 6;
            this.txtMICR.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMICR.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMICR_KeyPress);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(11, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 15);
            this.label8.TabIndex = 248;
            this.label8.Text = "Amount";
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(103, 67);
            this.txtAmount.MaxLength = 20;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(146, 20);
            this.txtAmount.TabIndex = 3;
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAmount_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(8, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 15);
            this.label2.TabIndex = 250;
            this.label2.Text = "Cheque No";
            // 
            // txtChqNo
            // 
            this.txtChqNo.Location = new System.Drawing.Point(103, 8);
            this.txtChqNo.MaxLength = 6;
            this.txtChqNo.Name = "txtChqNo";
            this.txtChqNo.Size = new System.Drawing.Size(146, 20);
            this.txtChqNo.TabIndex = 0;
            this.txtChqNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtChqNo_KeyPress);
            // 
            // frmCustomerOrderChequeInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 443);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtChqNo);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtMICR);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtBranch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtIFSC);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblTotAmt);
            this.Controls.Add(this.lblOrderNo);
            this.Controls.Add(this.lblGridTotAmt);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.gridCheque);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBank);
            this.Controls.Add(this.dtpChqDtae);
            this.Controls.Add(this.lblDate);
            this.Name = "frmCustomerOrderChequeInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Order Cheque Info";
            ((System.ComponentModel.ISupportInitialize)(this.gridCheque)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.LabelControl labelControl2;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAdd;
        private DevExpress.XtraEditors.LabelControl lblTotAmt;
        private DevExpress.XtraEditors.LabelControl lblOrderNo;
        private System.Windows.Forms.Label lblGridTotAmt;
        private System.Windows.Forms.Label label4;
        private DevExpress.XtraGrid.GridControl gridCheque;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colDate;
        private DevExpress.XtraGrid.Columns.GridColumn colBank;
        private DevExpress.XtraGrid.Columns.GridColumn colBranch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBank;
        private System.Windows.Forms.DateTimePicker dtpChqDtae;
        private DevExpress.XtraEditors.LabelControl lblDate;
        private DevExpress.XtraGrid.Columns.GridColumn colIFSC;
        private DevExpress.XtraGrid.Columns.GridColumn colMICRNO;
        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIFSC;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBranch;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtMICR;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtChqNo;
        private DevExpress.XtraGrid.Columns.GridColumn colChqNo;
    }
}