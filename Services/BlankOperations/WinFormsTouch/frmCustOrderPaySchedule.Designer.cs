namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmCustOrderPaySchedule
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
            this.dtPayDtae = new System.Windows.Forms.DateTimePicker();
            this.lblDate = new DevExpress.XtraEditors.LabelControl();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtAmt = new System.Windows.Forms.TextBox();
            this.txtPer = new System.Windows.Forms.TextBox();
            this.lblGridTotAmt = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTotPer = new System.Windows.Forms.Label();
            this.gridPaySchedule = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPerAmt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblOrderNo = new DevExpress.XtraEditors.LabelControl();
            this.lblTotAmt = new DevExpress.XtraEditors.LabelControl();
            this.btnAdd = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
            ((System.ComponentModel.ISupportInitialize)(this.gridPaySchedule)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // dtPayDtae
            // 
            this.dtPayDtae.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtPayDtae.Location = new System.Drawing.Point(109, 21);
            this.dtPayDtae.Name = "dtPayDtae";
            this.dtPayDtae.Size = new System.Drawing.Size(146, 20);
            this.dtPayDtae.TabIndex = 0;
            // 
            // lblDate
            // 
            this.lblDate.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.lblDate.Location = new System.Drawing.Point(13, 21);
            this.lblDate.Name = "lblDate";
            this.lblDate.Size = new System.Drawing.Size(28, 15);
            this.lblDate.TabIndex = 189;
            this.lblDate.Text = "Date";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(9, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 15);
            this.label6.TabIndex = 202;
            this.label6.Text = "Amount";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(9, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 15);
            this.label5.TabIndex = 201;
            this.label5.Text = "Amount (%)";
            // 
            // txtAmt
            // 
            this.txtAmt.Enabled = false;
            this.txtAmt.Location = new System.Drawing.Point(109, 87);
            this.txtAmt.MaxLength = 20;
            this.txtAmt.Name = "txtAmt";
            this.txtAmt.ReadOnly = true;
            this.txtAmt.Size = new System.Drawing.Size(146, 20);
            this.txtAmt.TabIndex = 200;
            this.txtAmt.TabStop = false;
            this.txtAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtPer
            // 
            this.txtPer.Location = new System.Drawing.Point(109, 57);
            this.txtPer.MaxLength = 20;
            this.txtPer.Name = "txtPer";
            this.txtPer.Size = new System.Drawing.Size(146, 20);
            this.txtPer.TabIndex = 1;
            this.txtPer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPer.TextChanged += new System.EventHandler(this.txtPer_TextChanged);
            this.txtPer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPer_KeyPress);
            this.txtPer.Leave += new System.EventHandler(this.txtPer_Leave);
            // 
            // lblGridTotAmt
            // 
            this.lblGridTotAmt.AutoSize = true;
            this.lblGridTotAmt.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGridTotAmt.Location = new System.Drawing.Point(655, 359);
            this.lblGridTotAmt.Name = "lblGridTotAmt";
            this.lblGridTotAmt.Size = new System.Drawing.Size(13, 13);
            this.lblGridTotAmt.TabIndex = 213;
            this.lblGridTotAmt.Text = "..";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(306, 359);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(104, 13);
            this.label2.TabIndex = 212;
            this.label2.Text = "Total Percentage";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(530, 359);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 13);
            this.label4.TabIndex = 210;
            this.label4.Text = "Total Amount.";
            // 
            // lblTotPer
            // 
            this.lblTotPer.AutoSize = true;
            this.lblTotPer.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotPer.Location = new System.Drawing.Point(442, 359);
            this.lblTotPer.Name = "lblTotPer";
            this.lblTotPer.Size = new System.Drawing.Size(13, 13);
            this.lblTotPer.TabIndex = 209;
            this.lblTotPer.Text = "..";
            // 
            // gridPaySchedule
            // 
            this.gridPaySchedule.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.gridPaySchedule.Location = new System.Drawing.Point(7, 159);
            this.gridPaySchedule.MainView = this.grdView;
            this.gridPaySchedule.Name = "gridPaySchedule";
            this.gridPaySchedule.Size = new System.Drawing.Size(708, 197);
            this.gridPaySchedule.TabIndex = 207;
            this.gridPaySchedule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDate,
            this.colPerAmt,
            this.colAmt});
            this.grdView.FixedLineWidth = 1;
            this.grdView.GridControl = this.gridPaySchedule;
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
            // colDate
            // 
            this.colDate.AppearanceHeader.Options.UseTextOptions = true;
            this.colDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colDate.Caption = "Pay Date";
            this.colDate.DisplayFormat.FormatString = "dd-MM-yyyy";
            this.colDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colDate.FieldName = "PayDate";
            this.colDate.Name = "colDate";
            this.colDate.Visible = true;
            this.colDate.VisibleIndex = 0;
            // 
            // colPerAmt
            // 
            this.colPerAmt.AppearanceHeader.Options.UseTextOptions = true;
            this.colPerAmt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colPerAmt.Caption = "% Amount";
            this.colPerAmt.FieldName = "PerAmt";
            this.colPerAmt.Name = "colPerAmt";
            this.colPerAmt.Visible = true;
            this.colPerAmt.VisibleIndex = 1;
            // 
            // colAmt
            // 
            this.colAmt.AppearanceHeader.Options.UseTextOptions = true;
            this.colAmt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colAmt.Caption = "Amount";
            this.colAmt.FieldName = "Amount";
            this.colAmt.Name = "colAmt";
            this.colAmt.Visible = true;
            this.colAmt.VisibleIndex = 2;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(377, 402);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(108, 42);
            this.btnDelete.TabIndex = 5;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(605, 402);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(108, 42);
            this.btnCommit.TabIndex = 3;
            this.btnCommit.Text = "Commit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(491, 402);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(108, 42);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblOrderNo.Location = new System.Drawing.Point(445, 21);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(14, 20);
            this.lblOrderNo.TabIndex = 214;
            this.lblOrderNo.Text = "--";
            // 
            // lblTotAmt
            // 
            this.lblTotAmt.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblTotAmt.Location = new System.Drawing.Point(445, 47);
            this.lblTotAmt.Name = "lblTotAmt";
            this.lblTotAmt.Size = new System.Drawing.Size(14, 20);
            this.lblTotAmt.TabIndex = 215;
            this.lblTotAmt.Text = "--";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdd.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Appearance.Options.UseFont = true;
            this.btnAdd.Location = new System.Drawing.Point(561, 100);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(154, 53);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Add pay schedule";
            this.btnAdd.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(150, 402);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(108, 42);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Void";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(263, 402);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(108, 42);
            this.btnEdit.TabIndex = 4;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Location = new System.Drawing.Point(309, 21);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(57, 15);
            this.labelControl1.TabIndex = 218;
            this.labelControl1.Text = "Order No";
            // 
            // labelControl2
            // 
            this.labelControl2.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 9F, System.Drawing.FontStyle.Bold);
            this.labelControl2.Location = new System.Drawing.Point(309, 55);
            this.labelControl2.Name = "labelControl2";
            this.labelControl2.Size = new System.Drawing.Size(86, 15);
            this.labelControl2.TabIndex = 219;
            this.labelControl2.Text = "Order Amount";
            // 
            // frmCustOrderPaySchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(725, 456);
            this.Controls.Add(this.labelControl2);
            this.Controls.Add(this.labelControl1);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblTotAmt);
            this.Controls.Add(this.lblOrderNo);
            this.Controls.Add(this.lblGridTotAmt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTotPer);
            this.Controls.Add(this.gridPaySchedule);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAmt);
            this.Controls.Add(this.txtPer);
            this.Controls.Add(this.dtPayDtae);
            this.Controls.Add(this.lblDate);
            this.MaximizeBox = false;
            this.Name = "frmCustOrderPaySchedule";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer order pay schedule";
            ((System.ComponentModel.ISupportInitialize)(this.gridPaySchedule)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtPayDtae;
        private DevExpress.XtraEditors.LabelControl lblDate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtAmt;
        private System.Windows.Forms.TextBox txtPer;
        private System.Windows.Forms.Label lblGridTotAmt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTotPer;
        private DevExpress.XtraGrid.GridControl gridPaySchedule;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private DevExpress.XtraGrid.Columns.GridColumn colDate;
        private DevExpress.XtraGrid.Columns.GridColumn colPerAmt;
        private DevExpress.XtraGrid.Columns.GridColumn colAmt;
        private DevExpress.XtraEditors.LabelControl lblOrderNo;
        private DevExpress.XtraEditors.LabelControl lblTotAmt;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAdd;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl2;
    }
}