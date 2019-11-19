namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmDisplayMetalRate
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnClose = new System.Windows.Forms.Button();
            this.grdMetalRate = new DevExpress.XtraGrid.GridControl();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.grdcolConfiguration = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdcolRateType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdcolRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.grdTransDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.cmbMetalType = new System.Windows.Forms.ComboBox();
            this.lblMetalType = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMetalRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMain.Controls.Add(this.label1);
            this.pnlMain.Controls.Add(this.dtpDate);
            this.pnlMain.Controls.Add(this.btnClose);
            this.pnlMain.Controls.Add(this.grdMetalRate);
            this.pnlMain.Controls.Add(this.cmbMetalType);
            this.pnlMain.Controls.Add(this.lblMetalType);
            this.pnlMain.Location = new System.Drawing.Point(13, 61);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(648, 467);
            this.pnlMain.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(341, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 19);
            this.label1.TabIndex = 11;
            this.label1.Text = "Date :";
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(404, 26);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(184, 21);
            this.dtpDate.TabIndex = 10;
            this.dtpDate.ValueChanged += new System.EventHandler(this.dtpDate_ValueChanged);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(557, 433);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // grdMetalRate
            // 
            this.grdMetalRate.Location = new System.Drawing.Point(21, 70);
            this.grdMetalRate.MainView = this.gridView;
            this.grdMetalRate.Name = "grdMetalRate";
            this.grdMetalRate.Size = new System.Drawing.Size(610, 354);
            this.grdMetalRate.TabIndex = 5;
            this.grdMetalRate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            // 
            // gridView
            // 
            this.gridView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gridView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.gridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView.Appearance.Row.BackColor = System.Drawing.Color.Silver;
            this.gridView.Appearance.Row.BackColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.gridView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.gridView.Appearance.Row.Options.UseBackColor = true;
            this.gridView.Appearance.Row.Options.UseFont = true;
            this.gridView.Appearance.Row.Options.UseForeColor = true;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.grdcolConfiguration,
            this.grdcolRateType,
            this.grdcolRate,
            this.grdTransDate});
            this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridView.GridControl = this.grdMetalRate;
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
            // grdcolConfiguration
            // 
            this.grdcolConfiguration.Caption = "Configuration";
            this.grdcolConfiguration.FieldName = "CONFIGIDSTANDARD";
            this.grdcolConfiguration.Name = "grdcolConfiguration";
            this.grdcolConfiguration.OptionsColumn.AllowEdit = false;
            this.grdcolConfiguration.OptionsColumn.AllowIncrementalSearch = false;
            this.grdcolConfiguration.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.grdcolConfiguration.Visible = true;
            this.grdcolConfiguration.VisibleIndex = 0;
            this.grdcolConfiguration.Width = 115;
            // 
            // grdcolRateType
            // 
            this.grdcolRateType.Caption = "Rate Type";
            this.grdcolRateType.FieldName = "RATETYPE";
            this.grdcolRateType.Name = "grdcolRateType";
            this.grdcolRateType.OptionsColumn.AllowEdit = false;
            this.grdcolRateType.OptionsColumn.AllowIncrementalSearch = false;
            this.grdcolRateType.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.grdcolRateType.Visible = true;
            this.grdcolRateType.VisibleIndex = 1;
            this.grdcolRateType.Width = 202;
            // 
            // grdcolRate
            // 
            this.grdcolRate.Caption = "Rate";
            this.grdcolRate.FieldName = "RATES";
            this.grdcolRate.Name = "grdcolRate";
            this.grdcolRate.OptionsColumn.AllowEdit = false;
            this.grdcolRate.OptionsColumn.AllowIncrementalSearch = false;
            this.grdcolRate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.grdcolRate.Visible = true;
            this.grdcolRate.VisibleIndex = 2;
            this.grdcolRate.Width = 88;
            // 
            // grdTransDate
            // 
            this.grdTransDate.Caption = "Date";
            this.grdTransDate.DisplayFormat.FormatString = "f";
            this.grdTransDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.grdTransDate.FieldName = "TRANSDATE";
            this.grdTransDate.Name = "grdTransDate";
            this.grdTransDate.Visible = true;
            this.grdTransDate.VisibleIndex = 3;
            this.grdTransDate.Width = 229;
            // 
            // cmbMetalType
            // 
            this.cmbMetalType.BackColor = System.Drawing.Color.Lavender;
            this.cmbMetalType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbMetalType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbMetalType.FormattingEnabled = true;
            this.cmbMetalType.Location = new System.Drawing.Point(119, 22);
            this.cmbMetalType.Name = "cmbMetalType";
            this.cmbMetalType.Size = new System.Drawing.Size(159, 25);
            this.cmbMetalType.TabIndex = 4;
            this.cmbMetalType.SelectedIndexChanged += new System.EventHandler(this.cmbMetalType_SelectedIndexChanged);
            // 
            // lblMetalType
            // 
            this.lblMetalType.AutoSize = true;
            this.lblMetalType.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblMetalType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblMetalType.Location = new System.Drawing.Point(23, 23);
            this.lblMetalType.Name = "lblMetalType";
            this.lblMetalType.Size = new System.Drawing.Size(91, 19);
            this.lblMetalType.TabIndex = 1;
            this.lblMetalType.Text = "Metal Type :";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(275, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(130, 32);
            this.lblTitle.TabIndex = 8;
            this.lblTitle.Text = "Metal Rate";
            // 
            // frmDisplayMetalRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 542);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pnlMain);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmDisplayMetalRate";
            this.Text = "Metal Rate";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdMetalRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblMetalType;
        private System.Windows.Forms.ComboBox cmbMetalType;
        private DevExpress.XtraGrid.GridControl grdMetalRate;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private DevExpress.XtraGrid.Columns.GridColumn grdcolConfiguration;
        private DevExpress.XtraGrid.Columns.GridColumn grdcolRateType;
        private DevExpress.XtraGrid.Columns.GridColumn grdcolRate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label label1;
        private DevExpress.XtraGrid.Columns.GridColumn grdTransDate;

    }
}