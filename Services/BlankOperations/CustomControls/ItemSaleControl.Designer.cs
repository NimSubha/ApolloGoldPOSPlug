namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class ItemSaleControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colLineNum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colMakingRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colIngredientsAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.lblCustomerOrderDesc = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.lblRateType = new System.Windows.Forms.Label();
            this.txtRateType = new System.Windows.Forms.TextBox();
            this.lblMakingType = new System.Windows.Forms.Label();
            this.txtMakingRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMakingType = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMakingAmt = new System.Windows.Forms.Label();
            this.numpadEntry = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.txtMakingDisc = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTAmount = new System.Windows.Forms.Label();
            this.lblCustOrder = new System.Windows.Forms.Label();
            this.txtCustomerOrder = new System.Windows.Forms.TextBox();
            this.btnCustomerOrder = new System.Windows.Forms.Button();
            this.btnSelectItems = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 5;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.tableLayoutPanel1, 0, 12);
            this.tableLayoutPanel.Controls.Add(this.lblCustomerOrderDesc, 0, 12);
            this.tableLayoutPanel.Controls.Add(this.label4, 0, 9);
            this.tableLayoutPanel.Controls.Add(this.lblHeader, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.txtPCS, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.lblQuantity, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.txtQuantity, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.lblRate, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.txtRate, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.lblRateType, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.txtRateType, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.lblMakingType, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.txtMakingRate, 1, 6);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 7);
            this.tableLayoutPanel.Controls.Add(this.txtMakingType, 1, 7);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 8);
            this.tableLayoutPanel.Controls.Add(this.lblMakingAmt, 0, 9);
            this.tableLayoutPanel.Controls.Add(this.numpadEntry, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.txtMakingDisc, 1, 8);
            this.tableLayoutPanel.Controls.Add(this.button1, 2, 5);
            this.tableLayoutPanel.Controls.Add(this.button2, 2, 7);
            this.tableLayoutPanel.Controls.Add(this.label5, 0, 10);
            this.tableLayoutPanel.Controls.Add(this.lblTAmount, 1, 10);
            this.tableLayoutPanel.Controls.Add(this.lblCustOrder, 0, 11);
            this.tableLayoutPanel.Controls.Add(this.txtCustomerOrder, 1, 11);
            this.tableLayoutPanel.Controls.Add(this.btnCustomerOrder, 3, 11);
            this.tableLayoutPanel.Controls.Add(this.btnSelectItems, 4, 11);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 11;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(471, 382);
            this.tableLayoutPanel.TabIndex = 3;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel.SetColumnSpan(this.tableLayoutPanel1, 5);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.grItems, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableLayoutPanel1.ForeColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 313);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(400, 40);
            this.tableLayoutPanel1.TabIndex = 212;
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(3, 3);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(375, 34);
            this.grItems.TabIndex = 0;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colLineNum,
            this.colItemId,
            this.colRate,
            this.colMakingRate,
            this.colIngredientsAmount});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 20;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // colLineNum
            // 
            this.colLineNum.Caption = "Line Num";
            this.colLineNum.FieldName = "LINENUM";
            this.colLineNum.Name = "colLineNum";
            this.colLineNum.OptionsColumn.AllowEdit = false;
            this.colLineNum.OptionsColumn.AllowIncrementalSearch = false;
            this.colLineNum.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colLineNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colLineNum.Visible = true;
            this.colLineNum.VisibleIndex = 0;
            this.colLineNum.Width = 20;
            // 
            // colItemId
            // 
            this.colItemId.Caption = "Item ID";
            this.colItemId.FieldName = "ITEMID";
            this.colItemId.Name = "colItemId";
            this.colItemId.OptionsColumn.AllowEdit = false;
            this.colItemId.OptionsColumn.AllowIncrementalSearch = false;
            this.colItemId.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.Visible = true;
            this.colItemId.VisibleIndex = 1;
            this.colItemId.Width = 30;
            // 
            // colRate
            // 
            this.colRate.Caption = "Rate";
            this.colRate.FieldName = "RATE";
            this.colRate.Name = "colRate";
            this.colRate.OptionsColumn.AllowEdit = false;
            this.colRate.OptionsColumn.AllowIncrementalSearch = false;
            this.colRate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colRate.Visible = true;
            this.colRate.VisibleIndex = 2;
            this.colRate.Width = 25;
            // 
            // colMakingRate
            // 
            this.colMakingRate.Caption = "Making Rate";
            this.colMakingRate.FieldName = "MAKINGRATE";
            this.colMakingRate.Name = "colMakingRate";
            this.colMakingRate.OptionsColumn.AllowEdit = false;
            this.colMakingRate.OptionsColumn.AllowIncrementalSearch = false;
            this.colMakingRate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colMakingRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colMakingRate.Visible = true;
            this.colMakingRate.VisibleIndex = 3;
            this.colMakingRate.Width = 25;
            // 
            // colIngredientsAmount
            // 
            this.colIngredientsAmount.Caption = "Ing. Amount";
            this.colIngredientsAmount.FieldName = "INGREDIENTSAMOUNT";
            this.colIngredientsAmount.Name = "colIngredientsAmount";
            this.colIngredientsAmount.OptionsColumn.AllowEdit = false;
            this.colIngredientsAmount.OptionsColumn.AllowIncrementalSearch = false;
            this.colIngredientsAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colIngredientsAmount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colIngredientsAmount.Visible = true;
            this.colIngredientsAmount.VisibleIndex = 4;
            this.colIngredientsAmount.Width = 25;
            // 
            // lblCustomerOrderDesc
            // 
            this.lblCustomerOrderDesc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomerOrderDesc.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.lblCustomerOrderDesc, 5);
            this.lblCustomerOrderDesc.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCustomerOrderDesc.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerOrderDesc.Location = new System.Drawing.Point(3, 358);
            this.lblCustomerOrderDesc.Name = "lblCustomerOrderDesc";
            this.lblCustomerOrderDesc.Size = new System.Drawing.Size(465, 21);
            this.lblCustomerOrderDesc.TabIndex = 210;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(3, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 21);
            this.label4.TabIndex = 206;
            this.label4.Text = "Making Amount";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeader.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.lblHeader, 6);
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.ForeColor = System.Drawing.Color.Black;
            this.lblHeader.Location = new System.Drawing.Point(3, 0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(465, 50);
            this.lblHeader.TabIndex = 153;
            this.lblHeader.Text = "Item Sale Calculations";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 21);
            this.label3.TabIndex = 197;
            this.label3.Text = "Pieces";
            // 
            // txtPCS
            // 
            this.txtPCS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPCS.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPCS.Location = new System.Drawing.Point(147, 53);
            this.txtPCS.MaxLength = 8;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(54, 27);
            this.txtPCS.TabIndex = 0;
            this.txtPCS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPCS.TextChanged += new System.EventHandler(this.txtPCS_TextChanged);
            this.txtPCS.Enter += new System.EventHandler(this.txtPCS_Enter);
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
            // 
            // lblQuantity
            // 
            this.lblQuantity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblQuantity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblQuantity.Location = new System.Drawing.Point(3, 78);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(138, 21);
            this.lblQuantity.TabIndex = 155;
            this.lblQuantity.Text = "Quantity";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtQuantity.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtQuantity.Location = new System.Drawing.Point(147, 79);
            this.txtQuantity.MaxLength = 8;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(54, 27);
            this.txtQuantity.TabIndex = 1;
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQuantity.TextChanged += new System.EventHandler(this.txtQuantity_TextChanged);
            this.txtQuantity.Enter += new System.EventHandler(this.txtQuantity_Enter);
            this.txtQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuantity_KeyPress);
            // 
            // lblRate
            // 
            this.lblRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(3, 104);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(138, 21);
            this.lblRate.TabIndex = 158;
            this.lblRate.Text = "Rate";
            // 
            // txtRate
            // 
            this.txtRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRate.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtRate.Location = new System.Drawing.Point(147, 105);
            this.txtRate.MaxLength = 8;
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(54, 27);
            this.txtRate.TabIndex = 2;
            this.txtRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRate.TextChanged += new System.EventHandler(this.txtRate_TextChanged);
            this.txtRate.Enter += new System.EventHandler(this.txtRate_Enter);
            this.txtRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRate_KeyPress);
            // 
            // lblRateType
            // 
            this.lblRateType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRateType.AutoSize = true;
            this.lblRateType.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRateType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRateType.Location = new System.Drawing.Point(3, 130);
            this.lblRateType.Name = "lblRateType";
            this.lblRateType.Size = new System.Drawing.Size(138, 21);
            this.lblRateType.TabIndex = 162;
            this.lblRateType.Text = "Rate Type";
            // 
            // txtRateType
            // 
            this.txtRateType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRateType.Enabled = false;
            this.txtRateType.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtRateType.Location = new System.Drawing.Point(147, 131);
            this.txtRateType.MaxLength = 9;
            this.txtRateType.Name = "txtRateType";
            this.txtRateType.ReadOnly = true;
            this.txtRateType.Size = new System.Drawing.Size(54, 27);
            this.txtRateType.TabIndex = 174;
            this.txtRateType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRateType.TextChanged += new System.EventHandler(this.txtRateType_TextChanged);
            // 
            // lblMakingType
            // 
            this.lblMakingType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMakingType.AutoSize = true;
            this.lblMakingType.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblMakingType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblMakingType.Location = new System.Drawing.Point(3, 156);
            this.lblMakingType.Name = "lblMakingType";
            this.lblMakingType.Size = new System.Drawing.Size(138, 21);
            this.lblMakingType.TabIndex = 163;
            this.lblMakingType.Text = "Making Rate";
            // 
            // txtMakingRate
            // 
            this.txtMakingRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMakingRate.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtMakingRate.Location = new System.Drawing.Point(147, 157);
            this.txtMakingRate.MaxLength = 8;
            this.txtMakingRate.Name = "txtMakingRate";
            this.txtMakingRate.Size = new System.Drawing.Size(54, 27);
            this.txtMakingRate.TabIndex = 4;
            this.txtMakingRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMakingRate.TextChanged += new System.EventHandler(this.txtMakingRate_TextChanged);
            this.txtMakingRate.Enter += new System.EventHandler(this.txtMakingRate_Enter);
            this.txtMakingRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMakingRate_KeyPress);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(3, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 21);
            this.label2.TabIndex = 167;
            this.label2.Text = "Making Type";
            // 
            // txtMakingType
            // 
            this.txtMakingType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMakingType.Enabled = false;
            this.txtMakingType.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.txtMakingType.Location = new System.Drawing.Point(147, 183);
            this.txtMakingType.MaxLength = 15;
            this.txtMakingType.Name = "txtMakingType";
            this.txtMakingType.ReadOnly = true;
            this.txtMakingType.Size = new System.Drawing.Size(54, 25);
            this.txtMakingType.TabIndex = 176;
            this.txtMakingType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMakingType.TextChanged += new System.EventHandler(this.txtMakingType_TextChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(3, 208);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 21);
            this.label1.TabIndex = 168;
            this.label1.Text = "Making Disc. (%)";
            // 
            // lblMakingAmt
            // 
            this.lblMakingAmt.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMakingAmt.AutoSize = true;
            this.lblMakingAmt.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMakingAmt.ForeColor = System.Drawing.Color.Green;
            this.lblMakingAmt.Location = new System.Drawing.Point(148, 232);
            this.lblMakingAmt.Name = "lblMakingAmt";
            this.lblMakingAmt.Size = new System.Drawing.Size(53, 26);
            this.lblMakingAmt.TabIndex = 200;
            this.lblMakingAmt.Text = "Making Amount";
            this.lblMakingAmt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numpadEntry
            // 
            this.numpadEntry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.numpadEntry.Appearance.BackColor = System.Drawing.Color.Transparent;
            this.numpadEntry.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numpadEntry.Appearance.Options.UseBackColor = true;
            this.numpadEntry.Appearance.Options.UseFont = true;
            this.numpadEntry.AutoSize = true;
            this.numpadEntry.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.SetColumnSpan(this.numpadEntry, 2);
            this.numpadEntry.CurrencyCode = null;
            this.numpadEntry.EnteredQuantity = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numpadEntry.EnteredValue = "";
            this.numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.IntegerPositive;
            this.numpadEntry.Location = new System.Drawing.Point(287, 53);
            this.numpadEntry.MaskChar = "";
            this.numpadEntry.MaskInterval = 0;
            this.numpadEntry.MaxNumberOfDigits = 4;
            this.numpadEntry.MinimumSize = new System.Drawing.Size(150, 190);
            this.numpadEntry.Name = "numpadEntry";
            this.numpadEntry.NegativeMode = false;
            this.numpadEntry.NoOfTries = 0;
            this.numpadEntry.NumberOfDecimals = 2;
            this.numpadEntry.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.numpadEntry.PromptText = "Enter Pieces:";
            this.tableLayoutPanel.SetRowSpan(this.numpadEntry, 6);
            this.numpadEntry.ShortcutKeysActive = false;
            this.numpadEntry.Size = new System.Drawing.Size(150, 190);
            this.numpadEntry.TabIndex = 196;
            this.numpadEntry.TimerEnabled = false;
            this.numpadEntry.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numpadEntry_EnterButtonPressed);
            // 
            // txtMakingDisc
            // 
            this.txtMakingDisc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMakingDisc.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtMakingDisc.Location = new System.Drawing.Point(147, 209);
            this.txtMakingDisc.MaxLength = 8;
            this.txtMakingDisc.Name = "txtMakingDisc";
            this.txtMakingDisc.Size = new System.Drawing.Size(54, 27);
            this.txtMakingDisc.TabIndex = 6;
            this.txtMakingDisc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMakingDisc.TextChanged += new System.EventHandler(this.txtMakingDisc_TextChanged);
            this.txtMakingDisc.Enter += new System.EventHandler(this.txtMakingDisc_Enter);
            this.txtMakingDisc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMakingDisc_KeyPress);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button1.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.button1.Location = new System.Drawing.Point(207, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(40, 20);
            this.button1.TabIndex = 3;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.button2.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.button2.Location = new System.Drawing.Point(207, 183);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(40, 20);
            this.button2.TabIndex = 5;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(3, 260);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 21);
            this.label5.TabIndex = 199;
            this.label5.Text = "Total Amount";
            // 
            // lblTAmount
            // 
            this.lblTAmount.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblTAmount.AutoSize = true;
            this.lblTAmount.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTAmount.ForeColor = System.Drawing.Color.Green;
            this.lblTAmount.Location = new System.Drawing.Point(151, 258);
            this.lblTAmount.Name = "lblTAmount";
            this.lblTAmount.Size = new System.Drawing.Size(50, 26);
            this.lblTAmount.TabIndex = 202;
            this.lblTAmount.Text = "lblTAmount";
            this.lblTAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblCustOrder
            // 
            this.lblCustOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustOrder.AutoSize = true;
            this.lblCustOrder.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCustOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustOrder.Location = new System.Drawing.Point(3, 286);
            this.lblCustOrder.Name = "lblCustOrder";
            this.lblCustOrder.Size = new System.Drawing.Size(138, 21);
            this.lblCustOrder.TabIndex = 207;
            this.lblCustOrder.Text = "Customer Order";
            // 
            // txtCustomerOrder
            // 
            this.txtCustomerOrder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.SetColumnSpan(this.txtCustomerOrder, 2);
            this.txtCustomerOrder.Enabled = false;
            this.txtCustomerOrder.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtCustomerOrder.Location = new System.Drawing.Point(147, 287);
            this.txtCustomerOrder.MaxLength = 8;
            this.txtCustomerOrder.Name = "txtCustomerOrder";
            this.txtCustomerOrder.ReadOnly = true;
            this.txtCustomerOrder.Size = new System.Drawing.Size(103, 27);
            this.txtCustomerOrder.TabIndex = 208;
            this.txtCustomerOrder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnCustomerOrder
            // 
            this.btnCustomerOrder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCustomerOrder.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnCustomerOrder.Location = new System.Drawing.Point(256, 287);
            this.btnCustomerOrder.Name = "btnCustomerOrder";
            this.btnCustomerOrder.Size = new System.Drawing.Size(40, 20);
            this.btnCustomerOrder.TabIndex = 209;
            this.btnCustomerOrder.UseVisualStyleBackColor = true;
            this.btnCustomerOrder.Click += new System.EventHandler(this.btnCustomerOrder_Click);
            // 
            // btnSelectItems
            // 
            this.btnSelectItems.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnSelectItems.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnSelectItems.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnSelectItems.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelectItems.Location = new System.Drawing.Point(302, 287);
            this.btnSelectItems.Name = "btnSelectItems";
            this.btnSelectItems.Size = new System.Drawing.Size(77, 20);
            this.btnSelectItems.TabIndex = 211;
            this.btnSelectItems.Text = "Select Items\r\n";
            this.btnSelectItems.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectItems.UseVisualStyleBackColor = true;
            this.btnSelectItems.Click += new System.EventHandler(this.btnSelectItems_Click);
            // 
            // ItemSaleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "ItemSaleControl";
            this.Size = new System.Drawing.Size(471, 382);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label lblRateType;
        private System.Windows.Forms.TextBox txtRateType;
        private System.Windows.Forms.Label lblMakingType;
        private System.Windows.Forms.TextBox txtMakingRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMakingType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblMakingAmt;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblTAmount;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtMakingDisc;
        private System.Windows.Forms.Label label4;
        public LSRetailPosis.POSProcesses.WinControls.NumPad numpadEntry;
        private System.Windows.Forms.Label lblCustOrder;
        private System.Windows.Forms.TextBox txtCustomerOrder;
        private System.Windows.Forms.Button btnCustomerOrder;
        private System.Windows.Forms.Label lblCustomerOrderDesc;
        private System.Windows.Forms.Button btnSelectItems;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        private DevExpress.XtraGrid.Columns.GridColumn colLineNum;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colRate;
        private DevExpress.XtraGrid.Columns.GridColumn colMakingRate;
        private DevExpress.XtraGrid.Columns.GridColumn colIngredientsAmount;

        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;


    }
}
