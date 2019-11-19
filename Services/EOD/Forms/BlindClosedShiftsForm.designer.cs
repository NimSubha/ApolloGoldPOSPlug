/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    partial class BlindClosedShiftsForm
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
            if (disposing)
            {
                if (components != null)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BlindClosedShiftsForm));
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.grdShifts = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colRegister = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colShift = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOpenedDateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBlindClosedDateTime = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStaff = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCloseShift = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPrintX = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnTenderDeclaration = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDeclareStartAmount = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeading = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.tableLayoutMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdShifts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.ColumnCount = 1;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.Controls.Add(this.grdShifts, 0, 0);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(26, 135);
            this.tableLayoutMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 2;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutMain.Size = new System.Drawing.Size(966, 521);
            this.tableLayoutMain.TabIndex = 0;
            // 
            // grdShifts
            // 
            this.grdShifts.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grdShifts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdShifts.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
            this.grdShifts.Location = new System.Drawing.Point(4, 4);
            this.grdShifts.MainView = this.grdView;
            this.grdShifts.Margin = new System.Windows.Forms.Padding(4);
            this.grdShifts.Name = "grdShifts";
            this.grdShifts.Size = new System.Drawing.Size(958, 513);
            this.grdShifts.TabIndex = 0;
            this.grdShifts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colRegister,
            this.colShift,
            this.colOpenedDateTime,
            this.colBlindClosedDateTime,
            this.colStaff});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grdShifts;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 40;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.grdView_KeyUp);
            this.grdView.Click += new System.EventHandler(this.grdView_Click);
            // 
            // colRegister
            // 
            this.colRegister.Caption = "Register Number";
            this.colRegister.FieldName = "TerminalId";
            this.colRegister.Name = "colRegister";
            this.colRegister.OptionsColumn.AllowEdit = false;
            this.colRegister.OptionsColumn.AllowIncrementalSearch = false;
            this.colRegister.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colRegister.Visible = true;
            this.colRegister.VisibleIndex = 0;
            this.colRegister.Width = 167;
            // 
            // colShift
            // 
            this.colShift.Caption = "Shift number";
            this.colShift.FieldName = "BatchId";
            this.colShift.Name = "colShift";
            this.colShift.OptionsColumn.AllowEdit = false;
            this.colShift.OptionsColumn.AllowIncrementalSearch = false;
            this.colShift.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colShift.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colShift.Visible = true;
            this.colShift.VisibleIndex = 1;
            this.colShift.Width = 150;
            // 
            // colOpenedDateTime
            // 
            this.colOpenedDateTime.Caption = "Opened";
            this.colOpenedDateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colOpenedDateTime.FieldName = "StartDateTime";
            this.colOpenedDateTime.Name = "colOpenedDateTime";
            this.colOpenedDateTime.OptionsColumn.AllowEdit = false;
            this.colOpenedDateTime.Visible = true;
            this.colOpenedDateTime.VisibleIndex = 2;
            this.colOpenedDateTime.Width = 159;
            // 
            // colBlindClosedDateTime
            // 
            this.colBlindClosedDateTime.Caption = "Blind closed";
            this.colBlindClosedDateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
            this.colBlindClosedDateTime.FieldName = "StatusDateTime";
            this.colBlindClosedDateTime.Name = "colBlindClosedDateTime";
            this.colBlindClosedDateTime.Visible = true;
            this.colBlindClosedDateTime.VisibleIndex = 3;
            this.colBlindClosedDateTime.Width = 183;
            // 
            // colStaff
            // 
            this.colStaff.Caption = "Staff";
            this.colStaff.FieldName = "StaffId";
            this.colStaff.Name = "colStaff";
            this.colStaff.OptionsColumn.AllowEdit = false;
            this.colStaff.Visible = true;
            this.colStaff.VisibleIndex = 4;
            this.colStaff.Width = 185;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 11;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnPgDown, 10, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnClose, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDown, 9, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCloseShift, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPrintX, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnTenderDeclaration, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDeclareStartAmount, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(26, 667);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(966, 65);
            this.tableLayoutPanel2.TabIndex = 2;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = ((System.Drawing.Image)(resources.GetObject("btnPgDown.Image")));
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(904, 4);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 8;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Appearance.Options.UseTextOptions = true;
            this.btnClose.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClose.Location = new System.Drawing.Point(689, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(127, 57);
            this.btnClose.TabIndex = 9;
            this.btnClose.Tag = "";
            this.btnClose.Text = "Close";
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(839, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 7;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnCloseShift
            // 
            this.btnCloseShift.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCloseShift.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCloseShift.Appearance.Options.UseFont = true;
            this.btnCloseShift.Appearance.Options.UseTextOptions = true;
            this.btnCloseShift.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnCloseShift.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCloseShift.Location = new System.Drawing.Point(554, 4);
            this.btnCloseShift.Margin = new System.Windows.Forms.Padding(4);
            this.btnCloseShift.Name = "btnCloseShift";
            this.btnCloseShift.Size = new System.Drawing.Size(127, 57);
            this.btnCloseShift.TabIndex = 6;
            this.btnCloseShift.Tag = "";
            this.btnCloseShift.Text = "Close Shift";
            this.btnCloseShift.Click += new System.EventHandler(this.PerformOperation);
            // 
            // btnPrintX
            // 
            this.btnPrintX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPrintX.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrintX.Appearance.Options.UseFont = true;
            this.btnPrintX.Appearance.Options.UseTextOptions = true;
            this.btnPrintX.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnPrintX.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPrintX.Location = new System.Drawing.Point(419, 4);
            this.btnPrintX.Margin = new System.Windows.Forms.Padding(4);
            this.btnPrintX.Name = "btnPrintX";
            this.btnPrintX.Size = new System.Drawing.Size(127, 57);
            this.btnPrintX.TabIndex = 5;
            this.btnPrintX.Tag = "";
            this.btnPrintX.Text = "Print X";
            this.btnPrintX.Click += new System.EventHandler(this.PerformOperation);
            // 
            // btnTenderDeclaration
            // 
            this.btnTenderDeclaration.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnTenderDeclaration.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTenderDeclaration.Appearance.Options.UseFont = true;
            this.btnTenderDeclaration.Appearance.Options.UseTextOptions = true;
            this.btnTenderDeclaration.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnTenderDeclaration.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnTenderDeclaration.Location = new System.Drawing.Point(284, 4);
            this.btnTenderDeclaration.Margin = new System.Windows.Forms.Padding(4);
            this.btnTenderDeclaration.Name = "btnTenderDeclaration";
            this.btnTenderDeclaration.Size = new System.Drawing.Size(127, 57);
            this.btnTenderDeclaration.TabIndex = 4;
            this.btnTenderDeclaration.Tag = "";
            this.btnTenderDeclaration.Text = "Tender declaration";
            this.btnTenderDeclaration.Click += new System.EventHandler(this.PerformOperation);
            // 
            // btnDeclareStartAmount
            // 
            this.btnDeclareStartAmount.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDeclareStartAmount.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeclareStartAmount.Appearance.Options.UseFont = true;
            this.btnDeclareStartAmount.Appearance.Options.UseTextOptions = true;
            this.btnDeclareStartAmount.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnDeclareStartAmount.Location = new System.Drawing.Point(149, 4);
            this.btnDeclareStartAmount.Margin = new System.Windows.Forms.Padding(4);
            this.btnDeclareStartAmount.Name = "btnDeclareStartAmount";
            this.btnDeclareStartAmount.Size = new System.Drawing.Size(127, 57);
            this.btnDeclareStartAmount.TabIndex = 3;
            this.btnDeclareStartAmount.Tag = "";
            this.btnDeclareStartAmount.Text = "Declare start amount";
            this.btnDeclareStartAmount.Click += new System.EventHandler(this.PerformOperation);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(69, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 2;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnPgUp
            // 
            this.btnPgUp.AllowFocus = false;
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = ((System.Drawing.Image)(resources.GetObject("btnPgUp.Image")));
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 4);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 1;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutMain, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1018, 743);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblHeading, 2);
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(318, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(382, 95);
            this.lblHeading.TabIndex = 1;
            this.lblHeading.Tag = "";
            this.lblHeading.Text = "Blind closed shifts";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BlindClosedShiftsForm
            // 
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.Appearance.Options.UseFont = true;
            this.ClientSize = new System.Drawing.Size(1018, 743);
            this.Controls.Add(this.tableLayoutPanel1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "BlindClosedShiftsForm";
            this.Text = "Blind closed shifts";
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.tableLayoutMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdShifts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDeclareStartAmount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private DevExpress.XtraGrid.GridControl grdShifts;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colOpenedDateTime;
        private DevExpress.XtraGrid.Columns.GridColumn colRegister;
        private DevExpress.XtraGrid.Columns.GridColumn colShift;
        private DevExpress.XtraGrid.Columns.GridColumn colStaff;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnTenderDeclaration;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPrintX;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCloseShift;
        private DevExpress.XtraGrid.Columns.GridColumn colBlindClosedDateTime;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;

    }
}
