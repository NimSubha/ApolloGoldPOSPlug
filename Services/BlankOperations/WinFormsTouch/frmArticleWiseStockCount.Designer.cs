namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmArticleWiseStockCount
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
            this.btnFetch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblDC = new System.Windows.Forms.Label();
            this.lblOHQ = new System.Windows.Forms.Label();
            this.lblPC = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAdd = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.gridStock = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.col1 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col2 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSUBARTICLE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSubArtName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMETALTYPE = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col3 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col4 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colBLW = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col6 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col7 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col9 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiffQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col11 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col12 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col13 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.col14 = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbProductType = new System.Windows.Forms.ComboBox();
            this.lblEditLine = new System.Windows.Forms.Label();
            this.btnSearchCountedBy = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtCountedBy = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtGrossWt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.lblPhyCW = new System.Windows.Forms.Label();
            this.cCmbSubArticle = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.cCmbArticle = new DevExpress.XtraEditors.CheckedComboBoxEdit();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbStockType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbStockTakingType = new System.Windows.Forms.ComboBox();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.dtpTransDate = new System.Windows.Forms.DateTimePicker();
            this.lblOHC = new System.Windows.Forms.Label();
            this.lblPQ = new System.Windows.Forms.Label();
            this.lblDQ = new System.Windows.Forms.Label();
            this.btnRePrint = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblBLW = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridStock)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cCmbSubArticle.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cCmbArticle.Properties)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnFetch
            // 
            this.btnFetch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnFetch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFetch.Appearance.Options.UseFont = true;
            this.btnFetch.Location = new System.Drawing.Point(830, 6);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(123, 42);
            this.btnFetch.TabIndex = 220;
            this.btnFetch.Text = "Fetch";
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // lblDC
            // 
            this.lblDC.AutoSize = true;
            this.lblDC.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDC.Location = new System.Drawing.Point(613, 458);
            this.lblDC.Name = "lblDC";
            this.lblDC.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblDC.Size = new System.Drawing.Size(13, 13);
            this.lblDC.TabIndex = 244;
            this.lblDC.Text = "..";
            // 
            // lblOHQ
            // 
            this.lblOHQ.AutoSize = true;
            this.lblOHQ.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOHQ.Location = new System.Drawing.Point(249, 458);
            this.lblOHQ.Name = "lblOHQ";
            this.lblOHQ.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblOHQ.Size = new System.Drawing.Size(13, 13);
            this.lblOHQ.TabIndex = 243;
            this.lblOHQ.Text = "..";
            // 
            // lblPC
            // 
            this.lblPC.AutoSize = true;
            this.lblPC.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPC.Location = new System.Drawing.Point(388, 458);
            this.lblPC.Name = "lblPC";
            this.lblPC.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblPC.Size = new System.Drawing.Size(13, 13);
            this.lblPC.TabIndex = 242;
            this.lblPC.Text = "..";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 458);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 241;
            this.label2.Text = "Total";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(596, 487);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 42);
            this.btnEdit.TabIndex = 240;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(830, 487);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(123, 42);
            this.btnCommit.TabIndex = 239;
            this.btnCommit.Text = "Commit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(490, 487);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 42);
            this.btnClear.TabIndex = 238;
            this.btnClear.Text = "Void";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(702, 487);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(122, 42);
            this.btnCancel.TabIndex = 237;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAdd.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Appearance.Options.UseFont = true;
            this.btnAdd.Location = new System.Drawing.Point(830, 101);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(123, 42);
            this.btnAdd.TabIndex = 257;
            this.btnAdd.Text = "Add";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // gridStock
            // 
            this.gridStock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.gridStock.Location = new System.Drawing.Point(0, 0);
            this.gridStock.MainView = this.grdView;
            this.gridStock.Name = "gridStock";
            this.gridStock.Size = new System.Drawing.Size(1825, 282);
            this.gridStock.TabIndex = 259;
            this.gridStock.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
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
            this.col1,
            this.col2,
            this.colSUBARTICLE,
            this.colSubArtName,
            this.colMETALTYPE,
            this.col3,
            this.col4,
            this.colBLW,
            this.col6,
            this.col7,
            this.col9,
            this.colDiffQty,
            this.col11,
            this.col12,
            this.col13,
            this.col14});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.gridStock;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Always;
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
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            // 
            // col1
            // 
            this.col1.Caption = "Article Code";
            this.col1.FieldName = "Article Code";
            this.col1.Name = "col1";
            this.col1.Visible = true;
            this.col1.VisibleIndex = 0;
            this.col1.Width = 100;
            // 
            // col2
            // 
            this.col2.Caption = "Article Name";
            this.col2.FieldName = "Article Name";
            this.col2.Name = "col2";
            this.col2.Visible = true;
            this.col2.VisibleIndex = 1;
            this.col2.Width = 100;
            // 
            // colSUBARTICLE
            // 
            this.colSUBARTICLE.Caption = "Sub Article";
            this.colSUBARTICLE.FieldName = "SUBARTICLE";
            this.colSUBARTICLE.Name = "colSUBARTICLE";
            this.colSUBARTICLE.Visible = true;
            this.colSUBARTICLE.VisibleIndex = 2;
            this.colSUBARTICLE.Width = 100;
            // 
            // colSubArtName
            // 
            this.colSubArtName.Caption = "Sub Article Name";
            this.colSubArtName.FieldName = "Sub Article Name";
            this.colSubArtName.Name = "colSubArtName";
            this.colSubArtName.Visible = true;
            this.colSubArtName.VisibleIndex = 3;
            this.colSubArtName.Width = 118;
            // 
            // colMETALTYPE
            // 
            this.colMETALTYPE.Caption = "Metal Type";
            this.colMETALTYPE.FieldName = "METALTYPE";
            this.colMETALTYPE.Name = "colMETALTYPE";
            this.colMETALTYPE.Visible = true;
            this.colMETALTYPE.VisibleIndex = 4;
            this.colMETALTYPE.Width = 100;
            // 
            // col3
            // 
            this.col3.Caption = "On Hand CW";
            this.col3.FieldName = "On Hand CW";
            this.col3.Name = "col3";
            this.col3.Visible = true;
            this.col3.VisibleIndex = 5;
            this.col3.Width = 100;
            // 
            // col4
            // 
            this.col4.Caption = "On Hand Qty";
            this.col4.FieldName = "On Hand Qty";
            this.col4.Name = "col4";
            this.col4.Visible = true;
            this.col4.VisibleIndex = 6;
            this.col4.Width = 99;
            // 
            // colBLW
            // 
            this.colBLW.Caption = "Barcode label weight";
            this.colBLW.FieldName = "Barcode label weight";
            this.colBLW.Name = "colBLW";
            this.colBLW.Visible = true;
            this.colBLW.VisibleIndex = 9;
            this.colBLW.Width = 98;
            // 
            // col6
            // 
            this.col6.Caption = "Physical CW";
            this.col6.FieldName = "Physical CW";
            this.col6.Name = "col6";
            this.col6.Visible = true;
            this.col6.VisibleIndex = 7;
            this.col6.Width = 100;
            // 
            // col7
            // 
            this.col7.Caption = "Physical Qty";
            this.col7.FieldName = "Physical Qty";
            this.col7.Name = "col7";
            this.col7.Visible = true;
            this.col7.VisibleIndex = 8;
            this.col7.Width = 100;
            // 
            // col9
            // 
            this.col9.Caption = "Difference CW";
            this.col9.FieldName = "Difference CW";
            this.col9.Name = "col9";
            this.col9.Visible = true;
            this.col9.VisibleIndex = 10;
            this.col9.Width = 100;
            // 
            // colDiffQty
            // 
            this.colDiffQty.Caption = "Difference Qty";
            this.colDiffQty.FieldName = "Difference Qty";
            this.colDiffQty.Name = "colDiffQty";
            this.colDiffQty.Visible = true;
            this.colDiffQty.VisibleIndex = 11;
            this.colDiffQty.Width = 100;
            // 
            // col11
            // 
            this.col11.Caption = "Counted By";
            this.col11.FieldName = "Counted By";
            this.col11.Name = "col11";
            this.col11.Visible = true;
            this.col11.VisibleIndex = 12;
            this.col11.Width = 100;
            // 
            // col12
            // 
            this.col12.Caption = "Name";
            this.col12.FieldName = "Name";
            this.col12.Name = "col12";
            this.col12.Visible = true;
            this.col12.VisibleIndex = 13;
            this.col12.Width = 125;
            // 
            // col13
            // 
            this.col13.Caption = "Remarks";
            this.col13.FieldName = "Remarks";
            this.col13.Name = "col13";
            this.col13.Visible = true;
            this.col13.VisibleIndex = 14;
            this.col13.Width = 383;
            // 
            // col14
            // 
            this.col14.Caption = "ISCW";
            this.col14.FieldName = "ISCW";
            this.col14.MinWidth = 10;
            this.col14.Name = "col14";
            this.col14.Width = 10;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Location = new System.Drawing.Point(830, 53);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(123, 42);
            this.btnSearch.TabIndex = 260;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbProductType);
            this.panel1.Controls.Add(this.lblEditLine);
            this.panel1.Controls.Add(this.btnSearchCountedBy);
            this.panel1.Controls.Add(this.txtCountedBy);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.txtGrossWt);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtQuantity);
            this.panel1.Controls.Add(this.lblQuantity);
            this.panel1.Controls.Add(this.txtPCS);
            this.panel1.Controls.Add(this.lblPhyCW);
            this.panel1.Controls.Add(this.cCmbSubArticle);
            this.panel1.Controls.Add(this.cCmbArticle);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.txtRemarks);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cmbStockType);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cmbStockTakingType);
            this.panel1.Controls.Add(this.txtVoucherNo);
            this.panel1.Controls.Add(this.dtpTransDate);
            this.panel1.Location = new System.Drawing.Point(12, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(812, 145);
            this.panel1.TabIndex = 261;
            // 
            // cmbProductType
            // 
            this.cmbProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProductType.FormattingEnabled = true;
            this.cmbProductType.Location = new System.Drawing.Point(501, 47);
            this.cmbProductType.Name = "cmbProductType";
            this.cmbProductType.Size = new System.Drawing.Size(142, 21);
            this.cmbProductType.TabIndex = 285;
            // 
            // lblEditLine
            // 
            this.lblEditLine.AutoSize = true;
            this.lblEditLine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.lblEditLine.Location = new System.Drawing.Point(664, 5);
            this.lblEditLine.Name = "lblEditLine";
            this.lblEditLine.Size = new System.Drawing.Size(15, 13);
            this.lblEditLine.TabIndex = 284;
            this.lblEditLine.Text = "--";
            // 
            // btnSearchCountedBy
            // 
            this.btnSearchCountedBy.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchCountedBy.Appearance.Font = new System.Drawing.Font("Segoe UI", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchCountedBy.Appearance.Options.UseFont = true;
            this.btnSearchCountedBy.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearchCountedBy.Location = new System.Drawing.Point(651, 115);
            this.btnSearchCountedBy.Name = "btnSearchCountedBy";
            this.btnSearchCountedBy.Size = new System.Drawing.Size(40, 20);
            this.btnSearchCountedBy.TabIndex = 283;
            this.btnSearchCountedBy.ToolTip = "Click to search counted by";
            this.btnSearchCountedBy.Click += new System.EventHandler(this.simpleButtonEx1_Click);
            // 
            // txtCountedBy
            // 
            this.txtCountedBy.Enabled = false;
            this.txtCountedBy.Location = new System.Drawing.Point(501, 112);
            this.txtCountedBy.MaxLength = 9;
            this.txtCountedBy.Name = "txtCountedBy";
            this.txtCountedBy.Size = new System.Drawing.Size(142, 21);
            this.txtCountedBy.TabIndex = 281;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(425, 115);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 13);
            this.label6.TabIndex = 282;
            this.label6.Text = "Counted By";
            // 
            // txtGrossWt
            // 
            this.txtGrossWt.Location = new System.Drawing.Point(636, 81);
            this.txtGrossWt.MaxLength = 9;
            this.txtGrossWt.Name = "txtGrossWt";
            this.txtGrossWt.Size = new System.Drawing.Size(102, 21);
            this.txtGrossWt.TabIndex = 279;
            this.txtGrossWt.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(555, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 280;
            this.label4.Text = "Phy. Gross Wt";
            this.label4.Visible = false;
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(236, 115);
            this.txtQuantity.MaxLength = 8;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(102, 21);
            this.txtQuantity.TabIndex = 276;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblQuantity.Location = new System.Drawing.Point(183, 115);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(50, 13);
            this.lblQuantity.TabIndex = 278;
            this.lblQuantity.Text = "Phy. Qty";
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(70, 115);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(102, 21);
            this.txtPCS.TabIndex = 275;
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
            // 
            // lblPhyCW
            // 
            this.lblPhyCW.AutoSize = true;
            this.lblPhyCW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblPhyCW.Location = new System.Drawing.Point(17, 115);
            this.lblPhyCW.Name = "lblPhyCW";
            this.lblPhyCW.Size = new System.Drawing.Size(49, 13);
            this.lblPhyCW.TabIndex = 277;
            this.lblPhyCW.Text = "Phy. CW";
            // 
            // cCmbSubArticle
            // 
            this.cCmbSubArticle.Location = new System.Drawing.Point(501, 24);
            this.cCmbSubArticle.Name = "cCmbSubArticle";
            this.cCmbSubArticle.Properties.AllowMultiSelect = true;
            this.cCmbSubArticle.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cCmbSubArticle.Properties.Appearance.Options.UseFont = true;
            this.cCmbSubArticle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cCmbSubArticle.Size = new System.Drawing.Size(142, 20);
            this.cCmbSubArticle.TabIndex = 274;
            // 
            // cCmbArticle
            // 
            this.cCmbArticle.Location = new System.Drawing.Point(501, 2);
            this.cCmbArticle.Name = "cCmbArticle";
            this.cCmbArticle.Properties.AllowMultiSelect = true;
            this.cCmbArticle.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 8F);
            this.cCmbArticle.Properties.Appearance.Options.UseFont = true;
            this.cCmbArticle.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.cCmbArticle.Size = new System.Drawing.Size(142, 20);
            this.cCmbArticle.TabIndex = 273;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label11.Location = new System.Drawing.Point(15, 27);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(34, 13);
            this.label11.TabIndex = 271;
            this.label11.Text = "Date:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label10.Location = new System.Drawing.Point(14, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 13);
            this.label10.TabIndex = 270;
            this.label10.Text = "Remarks:";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(70, 53);
            this.txtRemarks.MaxLength = 60;
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(353, 49);
            this.txtRemarks.TabIndex = 269;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label9.Location = new System.Drawing.Point(426, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 268;
            this.label9.Text = "Sub Article:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label7.Location = new System.Drawing.Point(426, 2);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 267;
            this.label7.Text = "Article:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label8.Location = new System.Drawing.Point(426, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 13);
            this.label8.TabIndex = 266;
            this.label8.Text = "Product Type:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(188, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 265;
            this.label5.Text = "Stock type:";
            // 
            // cmbStockType
            // 
            this.cmbStockType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStockType.FormattingEnabled = true;
            this.cmbStockType.Location = new System.Drawing.Point(283, 25);
            this.cmbStockType.Name = "cmbStockType";
            this.cmbStockType.Size = new System.Drawing.Size(140, 21);
            this.cmbStockType.TabIndex = 264;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(14, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 13);
            this.label3.TabIndex = 263;
            this.label3.Text = "Voucher:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(188, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 262;
            this.label1.Text = "Stock takingType:";
            // 
            // cmbStockTakingType
            // 
            this.cmbStockTakingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStockTakingType.FormattingEnabled = true;
            this.cmbStockTakingType.Location = new System.Drawing.Point(283, 2);
            this.cmbStockTakingType.Name = "cmbStockTakingType";
            this.cmbStockTakingType.Size = new System.Drawing.Size(140, 21);
            this.cmbStockTakingType.TabIndex = 261;
            // 
            // txtVoucherNo
            // 
            this.txtVoucherNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtVoucherNo.Enabled = false;
            this.txtVoucherNo.Location = new System.Drawing.Point(70, 2);
            this.txtVoucherNo.MaxLength = 20;
            this.txtVoucherNo.Name = "txtVoucherNo";
            this.txtVoucherNo.ReadOnly = true;
            this.txtVoucherNo.Size = new System.Drawing.Size(102, 21);
            this.txtVoucherNo.TabIndex = 260;
            // 
            // dtpTransDate
            // 
            this.dtpTransDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTransDate.Location = new System.Drawing.Point(70, 24);
            this.dtpTransDate.Name = "dtpTransDate";
            this.dtpTransDate.Size = new System.Drawing.Size(102, 21);
            this.dtpTransDate.TabIndex = 259;
            // 
            // lblOHC
            // 
            this.lblOHC.AutoSize = true;
            this.lblOHC.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOHC.Location = new System.Drawing.Point(195, 458);
            this.lblOHC.Name = "lblOHC";
            this.lblOHC.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblOHC.Size = new System.Drawing.Size(13, 13);
            this.lblOHC.TabIndex = 262;
            this.lblOHC.Text = "..";
            // 
            // lblPQ
            // 
            this.lblPQ.AutoSize = true;
            this.lblPQ.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPQ.Location = new System.Drawing.Point(457, 458);
            this.lblPQ.Name = "lblPQ";
            this.lblPQ.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblPQ.Size = new System.Drawing.Size(13, 13);
            this.lblPQ.TabIndex = 264;
            this.lblPQ.Text = "..";
            // 
            // lblDQ
            // 
            this.lblDQ.AutoSize = true;
            this.lblDQ.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDQ.Location = new System.Drawing.Point(685, 458);
            this.lblDQ.Name = "lblDQ";
            this.lblDQ.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblDQ.Size = new System.Drawing.Size(13, 13);
            this.lblDQ.TabIndex = 265;
            this.lblDQ.Text = "..";
            // 
            // btnRePrint
            // 
            this.btnRePrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRePrint.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRePrint.Appearance.Options.UseFont = true;
            this.btnRePrint.Location = new System.Drawing.Point(367, 487);
            this.btnRePrint.Name = "btnRePrint";
            this.btnRePrint.Size = new System.Drawing.Size(100, 42);
            this.btnRePrint.TabIndex = 268;
            this.btnRePrint.Text = "Re-Print";
            this.btnRePrint.Click += new System.EventHandler(this.btnRePrint_Click);
            // 
            // lblBLW
            // 
            this.lblBLW.AutoSize = true;
            this.lblBLW.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBLW.Location = new System.Drawing.Point(537, 458);
            this.lblBLW.Name = "lblBLW";
            this.lblBLW.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblBLW.Size = new System.Drawing.Size(13, 13);
            this.lblBLW.TabIndex = 269;
            this.lblBLW.Text = "..";
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.gridStock);
            this.panel2.Location = new System.Drawing.Point(12, 153);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(941, 302);
            this.panel2.TabIndex = 270;
            // 
            // frmArticleWiseStockCount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(972, 532);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblBLW);
            this.Controls.Add(this.btnRePrint);
            this.Controls.Add(this.lblDQ);
            this.Controls.Add(this.lblPQ);
            this.Controls.Add(this.lblOHC);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.lblDC);
            this.Controls.Add(this.lblOHQ);
            this.Controls.Add(this.lblPC);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnFetch);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmArticleWiseStockCount";
            this.Text = "Article wise count";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridStock)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cCmbSubArticle.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cCmbArticle.Properties)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnFetch;
        private System.Windows.Forms.Label lblDC;
        private System.Windows.Forms.Label lblOHQ;
        private System.Windows.Forms.Label lblPC;
        private System.Windows.Forms.Label label2;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAdd;
        private DevExpress.XtraGrid.GridControl gridStock;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn col1;
        private DevExpress.XtraGrid.Columns.GridColumn col2;
        private DevExpress.XtraGrid.Columns.GridColumn col3;
        private DevExpress.XtraGrid.Columns.GridColumn col4;
        private DevExpress.XtraGrid.Columns.GridColumn col6;
        private DevExpress.XtraGrid.Columns.GridColumn col7;
        private DevExpress.XtraGrid.Columns.GridColumn col9;
        private DevExpress.XtraGrid.Columns.GridColumn col11;
        private DevExpress.XtraGrid.Columns.GridColumn col12;
        private DevExpress.XtraGrid.Columns.GridColumn col13;
        private DevExpress.XtraGrid.Columns.GridColumn col14;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearch;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblEditLine;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchCountedBy;
        private System.Windows.Forms.TextBox txtCountedBy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtGrossWt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.Label lblPhyCW;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cCmbSubArticle;
        private DevExpress.XtraEditors.CheckedComboBoxEdit cCmbArticle;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtRemarks;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbStockType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbStockTakingType;
        public System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.DateTimePicker dtpTransDate;
        private System.Windows.Forms.Label lblOHC;
        private System.Windows.Forms.Label lblPQ;
        private System.Windows.Forms.Label lblDQ;
        private DevExpress.XtraGrid.Columns.GridColumn colDiffQty;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnRePrint;
        private System.Windows.Forms.ComboBox cmbProductType;
        private DevExpress.XtraGrid.Columns.GridColumn colSUBARTICLE;
        private DevExpress.XtraGrid.Columns.GridColumn colMETALTYPE;
        private DevExpress.XtraGrid.Columns.GridColumn colBLW;
        private System.Windows.Forms.Label lblBLW;
        private DevExpress.XtraGrid.Columns.GridColumn colSubArtName;
        private System.Windows.Forms.Panel panel2;
    }
}