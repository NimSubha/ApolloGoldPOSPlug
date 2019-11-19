/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    partial class frmSmartSearch
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
                timer.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601:DoNotUseTimersThatPreventPowerStateChanges")]
        private void InitializeComponent()
        {
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.grResult = new DevExpress.XtraGrid.GridControl();
            this.bindingSource = new System.Windows.Forms.BindingSource();
            this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAllResults = new DevExpress.XtraEditors.CheckButton();
            this.btnItemsOnly = new DevExpress.XtraEditors.CheckButton();
            this.btnCustomersOnly = new DevExpress.XtraEditors.CheckButton();
            this.btnTransactions = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnProductDetails = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelResults = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.labelName = new System.Windows.Forms.Label();
            this.labelAddress = new System.Windows.Forms.Label();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelPhone = new System.Windows.Forms.Label();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnShowPrice = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanelTop = new System.Windows.Forms.TableLayoutPanel();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.textBoxSearch = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblHeading = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grResult)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.flowLayoutPanel.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxSearch.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // basePanel
            // 
            this.basePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.basePanel.Controls.Add(this.tableLayoutPanelMain);
            this.basePanel.Controls.Add(this.tableLayoutPanelTop);
            this.basePanel.Controls.Add(this.lblHeading);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(30, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(959, 659);
            this.basePanel.TabIndex = 0;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 7;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelMain.Controls.Add(this.grResult, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.flowLayoutPanel, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.btnTransactions, 5, 2);
            this.tableLayoutPanelMain.Controls.Add(this.btnProductDetails, 5, 3);
            this.tableLayoutPanelMain.Controls.Add(this.labelResults, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.flowLayoutPanel1, 5, 4);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanel1, 0, 7);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 153);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 8;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(959, 506);
            this.tableLayoutPanelMain.TabIndex = 1;
            // 
            // grResult
            // 
            this.grResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grResult.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanelMain.SetColumnSpan(this.grResult, 5);
            this.grResult.DataMember = "Results";
            this.grResult.DataSource = this.bindingSource;
            this.grResult.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
            this.grResult.Location = new System.Drawing.Point(3, 67);
            this.grResult.MainView = this.gridView;
            this.grResult.Name = "grResult";
            this.tableLayoutPanelMain.SetRowSpan(this.grResult, 4);
            this.grResult.Size = new System.Drawing.Size(676, 345);
            this.grResult.TabIndex = 2;
            this.grResult.TabStop = false;
            this.grResult.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView});
            this.grResult.DoubleClick += new System.EventHandler(this.OnGridResult_DoubleClick);
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.Dialog.ViewModels.SearchViewModel);
            // 
            // gridView
            // 
            this.gridView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView.Appearance.Row.Options.UseFont = true;
            this.gridView.ColumnPanelRowHeight = 40;
            this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colNumber,
            this.colName,
            this.colPrice,
            this.colType});
            this.gridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridView.GridControl = this.grResult;
            this.gridView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView.Name = "gridView";
            this.gridView.OptionsBehavior.Editable = false;
            this.gridView.OptionsCustomization.AllowFilter = false;
            this.gridView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
            this.gridView.OptionsView.ShowGroupPanel = false;
            this.gridView.OptionsView.ShowIndicator = false;
            this.gridView.RowHeight = 40;
            this.gridView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.gridView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.OnGridView_FocusedRowChanged);
            // 
            // colNumber
            // 
            this.colNumber.Caption = "Number";
            this.colNumber.FieldName = "Number";
            this.colNumber.Name = "colNumber";
            this.colNumber.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colNumber.Visible = true;
            this.colNumber.VisibleIndex = 0;
            // 
            // colName
            // 
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 1;
            // 
            // colPrice
            // 
            this.colPrice.Caption = "Price";
            this.colPrice.FieldName = "ItemPrice";
            this.colPrice.Name = "colPrice";
            this.colPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colType
            // 
            this.colType.Caption = "Type";
            this.colType.FieldName = "FormattedResultType";
            this.colType.Name = "colType";
            this.colType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colType.Visible = true;
            this.colType.VisibleIndex = 2;
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.flowLayoutPanel, 7);
            this.flowLayoutPanel.Controls.Add(this.btnAllResults);
            this.flowLayoutPanel.Controls.Add(this.btnItemsOnly);
            this.flowLayoutPanel.Controls.Add(this.btnCustomersOnly);
            this.flowLayoutPanel.Location = new System.Drawing.Point(3, 24);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 0);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(956, 40);
            this.flowLayoutPanel.TabIndex = 1;
            // 
            // btnAllResults
            // 
            this.btnAllResults.AllowFocus = false;
            this.btnAllResults.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAllResults.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnAllResults.Appearance.Options.UseFont = true;
            this.btnAllResults.Checked = true;
            this.btnAllResults.GroupIndex = 0;
            this.btnAllResults.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAllResults.Location = new System.Drawing.Point(0, 3);
            this.btnAllResults.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.btnAllResults.Name = "btnAllResults";
            this.btnAllResults.Size = new System.Drawing.Size(135, 37);
            this.btnAllResults.TabIndex = 0;
            this.btnAllResults.Tag = "";
            this.btnAllResults.Text = "All results";
            this.btnAllResults.CheckedChanged += new System.EventHandler(this.OnFilterButtons_CheckedChanged);
            this.btnAllResults.TextChanged += new System.EventHandler(this.tabButtons_TextChanged);
            // 
            // btnItemsOnly
            // 
            this.btnItemsOnly.AllowFocus = false;
            this.btnItemsOnly.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnItemsOnly.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnItemsOnly.Appearance.Options.UseFont = true;
            this.btnItemsOnly.GroupIndex = 0;
            this.btnItemsOnly.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnItemsOnly.Location = new System.Drawing.Point(135, 3);
            this.btnItemsOnly.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.btnItemsOnly.Name = "btnItemsOnly";
            this.btnItemsOnly.Size = new System.Drawing.Size(135, 37);
            this.btnItemsOnly.TabIndex = 1;
            this.btnItemsOnly.TabStop = false;
            this.btnItemsOnly.Tag = "";
            this.btnItemsOnly.Text = "Items only";
            this.btnItemsOnly.CheckedChanged += new System.EventHandler(this.OnFilterButtons_CheckedChanged);
            this.btnItemsOnly.TextChanged += new System.EventHandler(this.tabButtons_TextChanged);
            // 
            // btnCustomersOnly
            // 
            this.btnCustomersOnly.AllowFocus = false;
            this.btnCustomersOnly.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCustomersOnly.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCustomersOnly.Appearance.Options.UseFont = true;
            this.btnCustomersOnly.GroupIndex = 0;
            this.btnCustomersOnly.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCustomersOnly.Location = new System.Drawing.Point(270, 3);
            this.btnCustomersOnly.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.btnCustomersOnly.Name = "btnCustomersOnly";
            this.btnCustomersOnly.Size = new System.Drawing.Size(135, 37);
            this.btnCustomersOnly.TabIndex = 2;
            this.btnCustomersOnly.TabStop = false;
            this.btnCustomersOnly.Tag = "";
            this.btnCustomersOnly.Text = "Customers only";
            this.btnCustomersOnly.CheckedChanged += new System.EventHandler(this.OnFilterButtons_CheckedChanged);
            this.btnCustomersOnly.TextChanged += new System.EventHandler(this.tabButtons_TextChanged);
            // 
            // btnTransactions
            // 
            this.btnTransactions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTransactions.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnTransactions.Appearance.Options.UseFont = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.btnTransactions, 2);
            this.btnTransactions.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.btnTransactions.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnTransactions.Location = new System.Drawing.Point(685, 67);
            this.btnTransactions.Name = "btnTransactions";
            this.btnTransactions.Size = new System.Drawing.Size(271, 60);
            this.btnTransactions.TabIndex = 3;
            this.btnTransactions.Tag = "";
            this.btnTransactions.Text = "Customer transactions";
            this.btnTransactions.Click += new System.EventHandler(this.OnBtnTransactions_Click);
            // 
            // btnProductDetails
            // 
            this.btnProductDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnProductDetails.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnProductDetails.Appearance.Options.UseFont = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.btnProductDetails, 2);
            this.btnProductDetails.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.btnProductDetails.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnProductDetails.Location = new System.Drawing.Point(685, 133);
            this.btnProductDetails.Name = "btnProductDetails";
            this.btnProductDetails.Size = new System.Drawing.Size(271, 60);
            this.btnProductDetails.TabIndex = 4;
            this.btnProductDetails.Tag = "";
            this.btnProductDetails.Text = "Product details";
            this.btnProductDetails.Click += new System.EventHandler(this.OnBtnProductDetails_Click);
            // 
            // labelResults
            // 
            this.labelResults.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelResults.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.labelResults, 7);
            this.labelResults.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SearchTerms", true));
            this.labelResults.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelResults.Location = new System.Drawing.Point(3, 0);
            this.labelResults.Name = "labelResults";
            this.labelResults.Size = new System.Drawing.Size(0, 21);
            this.labelResults.TabIndex = 1;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.labelName);
            this.flowLayoutPanel1.Controls.Add(this.labelAddress);
            this.flowLayoutPanel1.Controls.Add(this.labelEmail);
            this.flowLayoutPanel1.Controls.Add(this.labelPhone);
            this.flowLayoutPanel1.Controls.Add(this.pictureBox);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(685, 199);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(271, 207);
            this.flowLayoutPanel1.TabIndex = 20;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelName.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelName.Location = new System.Drawing.Point(3, 3);
            this.labelName.Margin = new System.Windows.Forms.Padding(3);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(44, 17);
            this.labelName.TabIndex = 5;
            this.labelName.Text = "Name";
            // 
            // labelAddress
            // 
            this.labelAddress.AutoSize = true;
            this.labelAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelAddress.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelAddress.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAddress.Location = new System.Drawing.Point(3, 26);
            this.labelAddress.Margin = new System.Windows.Forms.Padding(3);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(56, 17);
            this.labelAddress.TabIndex = 0;
            this.labelAddress.Text = "Address";
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.labelEmail.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelEmail.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelEmail.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEmail.Location = new System.Drawing.Point(3, 49);
            this.labelEmail.Margin = new System.Windows.Forms.Padding(3);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(39, 17);
            this.labelEmail.TabIndex = 2;
            this.labelEmail.Text = "Email";
            // 
            // labelPhone
            // 
            this.labelPhone.AutoSize = true;
            this.labelPhone.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelPhone.DataBindings.Add(new System.Windows.Forms.Binding("Visible", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.labelPhone.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPhone.Location = new System.Drawing.Point(3, 72);
            this.labelPhone.Margin = new System.Windows.Forms.Padding(3);
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size(44, 17);
            this.labelPhone.TabIndex = 4;
            this.labelPhone.Text = "Phone";
            // 
            // pictureBox
            // 
            this.pictureBox.DataBindings.Add(new System.Windows.Forms.Binding("Image", this.bindingSource, "SelectedImage", true));
            this.pictureBox.Location = new System.Drawing.Point(65, 3);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(264, 204);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 19;
            this.pictureBox.TabStop = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 9;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanel1, 7);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnShowPrice, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSelect, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPgDown, 8, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnDown, 7, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 438);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(953, 65);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // btnPgUp
            // 
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 4);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(65, 57);
            this.btnPgUp.TabIndex = 5;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.OnBtnPgUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(77, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(65, 57);
            this.btnUp.TabIndex = 6;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.OnBtnUp_Click);
            // 
            // btnShowPrice
            // 
            this.btnShowPrice.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnShowPrice.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnShowPrice.Appearance.Options.UseFont = true;
            this.btnShowPrice.DataBindings.Add(new System.Windows.Forms.Binding("Enabled", this.bindingSource, "SelectedResult", true, System.Windows.Forms.DataSourceUpdateMode.Never));
            this.btnShowPrice.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnShowPrice.Location = new System.Drawing.Point(548, 4);
            this.btnShowPrice.Margin = new System.Windows.Forms.Padding(4);
            this.btnShowPrice.Name = "btnShowPrice";
            this.btnShowPrice.Size = new System.Drawing.Size(127, 57);
            this.btnShowPrice.TabIndex = 10;
            this.btnShowPrice.Tag = "";
            this.btnShowPrice.Text = "Show price";
            this.btnShowPrice.Click += new System.EventHandler(this.OnBtnShowPrice_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(278, 4);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 7;
            this.btnSelect.Text = "OK";
            this.btnSelect.Click += new System.EventHandler(this.OnBtnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(413, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Cancel";
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(884, 4);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(65, 57);
            this.btnPgDown.TabIndex = 12;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.OnBtnPgDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(811, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(65, 57);
            this.btnDown.TabIndex = 11;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.OnBtnDown_Click);
            // 
            // tableLayoutPanelTop
            // 
            this.tableLayoutPanelTop.ColumnCount = 3;
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTop.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelTop.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanelTop.Controls.Add(this.textBoxSearch, 0, 0);
            this.tableLayoutPanelTop.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelTop.Location = new System.Drawing.Point(0, 116);
            this.tableLayoutPanelTop.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            this.tableLayoutPanelTop.Name = "tableLayoutPanelTop";
            this.tableLayoutPanelTop.RowCount = 1;
            this.tableLayoutPanelTop.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelTop.Size = new System.Drawing.Size(959, 37);
            this.tableLayoutPanelTop.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(899, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 31);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.OnBtnClear_Click);
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearch.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.bindingSource, "SearchTerms", true));
            this.textBoxSearch.Location = new System.Drawing.Point(3, 3);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxSearch.Properties.Appearance.BackColor2 = System.Drawing.Color.White;
            this.textBoxSearch.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.textBoxSearch.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxSearch.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxSearch.Properties.Appearance.Options.UseFont = true;
            this.textBoxSearch.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxSearch.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxSearch.Size = new System.Drawing.Size(827, 32);
            this.textBoxSearch.TabIndex = 0;
            this.textBoxSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnTextBoxSearch_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(836, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Padding = new System.Windows.Forms.Padding(0);
            this.btnSearch.Size = new System.Drawing.Size(57, 31);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Click += new System.EventHandler(this.OnBtnSearch_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(0, 0);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Size = new System.Drawing.Size(959, 116);
            this.lblHeading.TabIndex = 25;
            this.lblHeading.Tag = "";
            this.lblHeading.Text = "Search";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer
            // 
            this.timer.Interval = 500;
            this.timer.Tick += new System.EventHandler(this.OnTimer_Tick);
            // 
            // frmSmartSearch
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1019, 670);
            this.Controls.Add(this.basePanel);
            this.KeyPreview = true;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmSmartSearch";
            this.Padding = new System.Windows.Forms.Padding(30, 0, 30, 11);
            this.Text = "Search";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grResult)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.flowLayoutPanel.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanelTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textBoxSearch.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl basePanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelTop;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearch;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private DevExpress.XtraEditors.TextEdit textBoxSearch;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private DevExpress.XtraGrid.GridControl grResult;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSelect;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnShowPrice;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private DevExpress.XtraEditors.CheckButton btnAllResults;
        private DevExpress.XtraEditors.CheckButton btnItemsOnly;
        private DevExpress.XtraEditors.CheckButton btnCustomersOnly;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnTransactions;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnProductDetails;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label labelResults;
        private DevExpress.XtraGrid.Columns.GridColumn colNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colType;
        private System.Windows.Forms.BindingSource bindingSource;
        private DevExpress.XtraGrid.Columns.GridColumn colPrice;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label labelAddress;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelPhone;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblHeading;
    }
}
