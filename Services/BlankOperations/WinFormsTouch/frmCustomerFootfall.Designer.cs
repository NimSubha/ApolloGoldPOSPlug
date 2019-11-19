namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmCustomerFootfall
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblTotalCust = new System.Windows.Forms.Label();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.label4 = new System.Windows.Forms.Label();
            this.txtHNICust = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNewCust = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.txtExistCust = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCLose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnShowRec = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grdCustFootFall = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNOOFEXISTCUST = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNOOFNEWCUST = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNOOFHNICUST = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colHidden = new DevExpress.XtraGrid.Columns.GridColumn();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.label6 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.btnExportExcel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdCustFootFall)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblTotalCust);
            this.groupBox1.Controls.Add(this.btnSubmit);
            this.groupBox1.Controls.Add(this.btnEdit);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtHNICust);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtNewCust);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.dtpDate);
            this.groupBox1.Controls.Add(this.txtExistCust);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(2, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(622, 111);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // lblTotalCust
            // 
            this.lblTotalCust.AutoSize = true;
            this.lblTotalCust.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCust.Location = new System.Drawing.Point(400, 95);
            this.lblTotalCust.Name = "lblTotalCust";
            this.lblTotalCust.Size = new System.Drawing.Size(15, 13);
            this.lblTotalCust.TabIndex = 204;
            this.lblTotalCust.Text = "--";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(516, 55);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 34);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "SUBMIT";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(516, 17);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 34);
            this.btnEdit.TabIndex = 6;
            this.btnEdit.Text = "EDIT";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(210, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(104, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "No Of HNI Customer";
            this.label4.Visible = false;
            // 
            // txtHNICust
            // 
            this.txtHNICust.Location = new System.Drawing.Point(337, 62);
            this.txtHNICust.MaxLength = 4;
            this.txtHNICust.Name = "txtHNICust";
            this.txtHNICust.Size = new System.Drawing.Size(134, 20);
            this.txtHNICust.TabIndex = 2;
            this.txtHNICust.Text = "0";
            this.txtHNICust.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtHNICust.Visible = false;
            this.txtHNICust.TextChanged += new System.EventHandler(this.txtHNICust_TextChanged);
            this.txtHNICust.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtHNICust_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "No Of New Customer";
            // 
            // txtNewCust
            // 
            this.txtNewCust.Location = new System.Drawing.Point(337, 40);
            this.txtNewCust.MaxLength = 4;
            this.txtNewCust.Name = "txtNewCust";
            this.txtNewCust.Size = new System.Drawing.Size(134, 20);
            this.txtNewCust.TabIndex = 1;
            this.txtNewCust.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNewCust.TextChanged += new System.EventHandler(this.txtNewCust_TextChanged);
            this.txtNewCust.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtNewCust_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(210, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "No Of Existing Customer";
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(52, 20);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(102, 20);
            this.dtpDate.TabIndex = 2;
            // 
            // txtExistCust
            // 
            this.txtExistCust.Location = new System.Drawing.Point(337, 17);
            this.txtExistCust.MaxLength = 4;
            this.txtExistCust.Name = "txtExistCust";
            this.txtExistCust.Size = new System.Drawing.Size(134, 20);
            this.txtExistCust.TabIndex = 0;
            this.txtExistCust.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtExistCust.TextChanged += new System.EventHandler(this.txtExistCust_TextChanged);
            this.txtExistCust.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtExistCust_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Date";
            // 
            // btnCLose
            // 
            this.btnCLose.Location = new System.Drawing.Point(518, 375);
            this.btnCLose.Name = "btnCLose";
            this.btnCLose.Size = new System.Drawing.Size(100, 34);
            this.btnCLose.TabIndex = 5;
            this.btnCLose.Text = "CLOSE";
            this.btnCLose.Click += new System.EventHandler(this.btnCLose_Click);
            // 
            // btnShowRec
            // 
            this.btnShowRec.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnShowRec.Location = new System.Drawing.Point(518, 113);
            this.btnShowRec.Name = "btnShowRec";
            this.btnShowRec.Size = new System.Drawing.Size(100, 34);
            this.btnShowRec.TabIndex = 4;
            this.btnShowRec.Text = "SHOW RECORD";
            this.btnShowRec.Click += new System.EventHandler(this.btnShowRec_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.grdCustFootFall);
            this.panel1.Location = new System.Drawing.Point(4, 153);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(616, 216);
            this.panel1.TabIndex = 203;
            // 
            // grdCustFootFall
            // 
            this.grdCustFootFall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdCustFootFall.Location = new System.Drawing.Point(0, 0);
            this.grdCustFootFall.MainView = this.gridView;
            this.grdCustFootFall.Name = "grdCustFootFall";
            this.grdCustFootFall.Size = new System.Drawing.Size(616, 216);
            this.grdCustFootFall.TabIndex = 7;
            this.toolTip1.SetToolTip(this.grdCustFootFall, "Double click on row for edit.");
            this.grdCustFootFall.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            this.grdCustFootFall.DoubleClick += new System.EventHandler(this.grdCustFootFall_DoubleClick);
            // 
            // gridView
            // 
            this.gridView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gridView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.gridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView.Appearance.Row.BackColor = System.Drawing.Color.Silver;
            this.gridView.Appearance.Row.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gridView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.gridView.Appearance.Row.Options.UseBackColor = true;
            this.gridView.Appearance.Row.Options.UseFont = true;
            this.gridView.Appearance.Row.Options.UseForeColor = true;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDate,
            this.colNOOFEXISTCUST,
            this.colNOOFNEWCUST,
            this.colNOOFHNICUST,
            this.colTotal,
            this.colHidden});
            this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridView.GridControl = this.grdCustFootFall;
            this.gridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsView.AutoCalcPreviewLineCount = true;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            this.gridView.RowHeight = 30;
            // 
            // colDate
            // 
            this.colDate.Caption = "Date";
            this.colDate.DisplayFormat.FormatString = "\"dd-MMM-yyyy\"";
            this.colDate.FieldName = "ENTRYDATE";
            this.colDate.Name = "colDate";
            this.colDate.OptionsColumn.AllowEdit = false;
            this.colDate.OptionsColumn.AllowIncrementalSearch = false;
            this.colDate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colDate.Visible = true;
            this.colDate.VisibleIndex = 0;
            this.colDate.Width = 30;
            // 
            // colNOOFEXISTCUST
            // 
            this.colNOOFEXISTCUST.Caption = "No Of Existing Customer";
            this.colNOOFEXISTCUST.FieldName = "NOOFEXISTCUST";
            this.colNOOFEXISTCUST.Name = "colNOOFEXISTCUST";
            this.colNOOFEXISTCUST.OptionsColumn.AllowEdit = false;
            this.colNOOFEXISTCUST.OptionsColumn.AllowIncrementalSearch = false;
            this.colNOOFEXISTCUST.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colNOOFEXISTCUST.Visible = true;
            this.colNOOFEXISTCUST.VisibleIndex = 1;
            this.colNOOFEXISTCUST.Width = 45;
            // 
            // colNOOFNEWCUST
            // 
            this.colNOOFNEWCUST.Caption = "No Of New Customer";
            this.colNOOFNEWCUST.FieldName = "NOOFNEWCUST";
            this.colNOOFNEWCUST.Name = "colNOOFNEWCUST";
            this.colNOOFNEWCUST.OptionsColumn.AllowEdit = false;
            this.colNOOFNEWCUST.OptionsColumn.AllowIncrementalSearch = false;
            this.colNOOFNEWCUST.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colNOOFNEWCUST.Visible = true;
            this.colNOOFNEWCUST.VisibleIndex = 2;
            this.colNOOFNEWCUST.Width = 45;
            // 
            // colNOOFHNICUST
            // 
            this.colNOOFHNICUST.Caption = "No Of HNI Customer";
            this.colNOOFHNICUST.FieldName = "NOOFHNICUST";
            this.colNOOFHNICUST.Name = "colNOOFHNICUST";
            this.colNOOFHNICUST.Width = 45;
            // 
            // colTotal
            // 
            this.colTotal.Caption = "Total Customer";
            this.colTotal.FieldName = "TOTAL";
            this.colTotal.Name = "colTotal";
            this.colTotal.Visible = true;
            this.colTotal.VisibleIndex = 4;
            this.colTotal.Width = 30;
            // 
            // colHidden
            // 
            this.colHidden.Caption = "gridColumn1";
            this.colHidden.FieldName = "REPLICATIONCOUNTER";
            this.colHidden.Name = "colHidden";
            this.colHidden.Width = 20;
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFromDate.Location = new System.Drawing.Point(211, 120);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(102, 20);
            this.dtpFromDate.TabIndex = 205;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 122);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 204;
            this.label5.Text = "From Date";
            // 
            // dtpToDate
            // 
            this.dtpToDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpToDate.Location = new System.Drawing.Point(371, 120);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(102, 20);
            this.dtpToDate.TabIndex = 207;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(319, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 206;
            this.label6.Text = "To Date";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(385, 375);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(100, 34);
            this.btnExportExcel.TabIndex = 208;
            this.btnExportExcel.Text = "Export To Excel";
            this.btnExportExcel.Click += new System.EventHandler(this.btnExportExcel_Click);
            // 
            // frmCustomerFootfall
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 416);
            this.Controls.Add(this.btnExportExcel);
            this.Controls.Add(this.dtpToDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.dtpFromDate);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnShowRec);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCLose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Name = "frmCustomerFootfall";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Foot Fall";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdCustFootFall)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtHNICust;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNewCust;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.TextBox txtExistCust;
        private System.Windows.Forms.Label label1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCLose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnShowRec;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.Label label6;
        private DevExpress.XtraGrid.GridControl grdCustFootFall;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn colDate;
        private DevExpress.XtraGrid.Columns.GridColumn colNOOFEXISTCUST;
        private DevExpress.XtraGrid.Columns.GridColumn colNOOFNEWCUST;
        private DevExpress.XtraGrid.Columns.GridColumn colNOOFHNICUST;
        private DevExpress.XtraGrid.Columns.GridColumn colHidden;
        private System.Windows.Forms.ToolTip toolTip1;
        private DevExpress.XtraGrid.Columns.GridColumn colTotal;
        private System.Windows.Forms.Label lblTotalCust;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnExportExcel;
    }
}