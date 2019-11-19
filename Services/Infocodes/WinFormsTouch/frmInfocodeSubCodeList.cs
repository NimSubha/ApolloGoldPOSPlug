/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.Services.InfoCodes
{
    /// <summary>
    /// Summary description for frmInfocodeSubCodeList.
    /// </summary>
    internal class FormInfoCodeSubCodeList : FormInfoCodeSubCodeBase
    {
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private SimpleButtonEx btnPgUp;
        private SimpleButtonEx btnPgDown;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private DevExpress.XtraGrid.Columns.GridColumn colPrice;
        private SimpleButtonEx btnSelect;
        private SimpleButtonEx btnCancel;
        private PanelControl basePanel;
        private SimpleButtonEx btnUp;
        private SimpleButtonEx btnDown;
        private System.Data.DataTable itemTable;
        private Label lblPrompt;
        private DevExpress.XtraGrid.Columns.GridColumn colSubcodeId;
        private DevExpress.XtraGrid.Columns.GridColumn colTriggerFunction;
        private DevExpress.XtraGrid.Columns.GridColumn colTriggerCode;
        private DevExpress.XtraGrid.Columns.GridColumn colPriceType;
        private DevExpress.XtraGrid.Columns.GridColumn colAmountPercent;
        private TableLayoutPanel tableLayoutPanel1;

        private const int DefaultNumberOfRows = 100;
        private TableLayoutPanel tableLayoutPanel2;
        private SimpleButton simpleButton1;
        private DevExpress.XtraEditors.TextEdit textBox;
        private SimpleButton btnSearch;
        private SimpleButton btnClear;
        private bool isValueChanged;
        private Label labelItemDescription;       
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent == null)
                throw new ArgumentNullException("hevent");

            LSRetailPosis.POSControls.POSFormsManager.ShowHelp(System.Windows.Forms.Form.ActiveForm);
            hevent.Handled = true;
        }

        public FormInfoCodeSubCodeList()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                lblPrompt.Text = infoCodePrompt;

                // If an item is specified, set the description label
                if (this.Item != null)
                    labelItemDescription.Text = string.Format(FormInfoCodeSubCodeBase.ItemDescriptionFormat, this.Item.ItemId, this.Item.Description);

                TranslateLabels();

                grdView.FocusedColumn = grdView.Columns["DESCRIPTION"];

                //Set the size of the form the same as the main form
                this.Bounds = new Rectangle(
                    ApplicationSettings.MainWindowLeft,
                    ApplicationSettings.MainWindowTop,
                    ApplicationSettings.MainWindowWidth,
                    ApplicationSettings.MainWindowHeight);

                itemTable = GetInfoCodeList(string.Empty, string.Empty, DefaultNumberOfRows);
                grItems.DataSource = itemTable;
                CheckRowPosition();

                if (inputRequired && ((itemTable != null) && (itemTable.Rows.Count > 0)) )
                {   // Hide the Canel button when input is required (and available)
                    this.btnCancel.Visible = false;
                }
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Disable close button if input is required
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (inputRequired)
                {
                    cp.ClassStyle |= 0x200; // CP_NOCLOSE_BUTTON
                }

                return cp;
            }
        }

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmInfocodeSubCodeList are reserved at 61500 - 61599
            // In use now are ID's 61500 - 61513
            //
            btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(60000); //Clear
            btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(60004); //Select           
            btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(60012);//Cancel

            grdView.Columns["DESCRIPTION"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(60007); //Description
            grdView.Columns["PRICE"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(60008); //Price
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInfoCodeSubCodeList));
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSubcodeId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTriggerFunction = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTriggerCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPriceType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAmountPercent = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPrompt = new System.Windows.Forms.Label();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.textBox = new DevExpress.XtraEditors.TextEdit();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelItemDescription = new System.Windows.Forms.Label();
            this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBox.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.grItems, 8);
            this.grItems.Location = new System.Drawing.Point(29, 225);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(962, 449);
            this.grItems.TabIndex = 0;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 12F);
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDescription,
            this.colPrice,
            this.colSubcodeId,
            this.colTriggerFunction,
            this.colTriggerCode,
            this.colPriceType,
            this.colAmountPercent});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsSelection.EnableAppearanceHideSelection = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 40;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Click += new System.EventHandler(this.grdView_Click);
            this.grdView.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colDescription
            // 
            this.colDescription.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colDescription.AppearanceCell.Options.UseFont = true;
            this.colDescription.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colDescription.AppearanceHeader.Options.UseFont = true;
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "DESCRIPTION";
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.AllowEdit = false;
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 0;
            this.colDescription.Width = 350;
            // 
            // colPrice
            // 
            this.colPrice.AppearanceCell.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colPrice.AppearanceCell.Options.UseFont = true;
            this.colPrice.AppearanceHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.colPrice.AppearanceHeader.Options.UseFont = true;
            this.colPrice.Caption = "Price";
            this.colPrice.FieldName = "PRICE";
            this.colPrice.Name = "colPrice";
            this.colPrice.Visible = true;
            this.colPrice.VisibleIndex = 1;
            this.colPrice.Width = 365;
            // 
            // colSubcodeId
            // 
            this.colSubcodeId.Caption = "SubCodeId";
            this.colSubcodeId.FieldName = "SUBCODEID";
            this.colSubcodeId.Name = "colSubcodeId";
            // 
            // colTriggerFunction
            // 
            this.colTriggerFunction.Caption = "TriggerFunction";
            this.colTriggerFunction.FieldName = "TRIGGERFUNCTION";
            this.colTriggerFunction.Name = "colTriggerFunction";
            // 
            // colTriggerCode
            // 
            this.colTriggerCode.Caption = "TriggerCode";
            this.colTriggerCode.FieldName = "TRIGGERCODE";
            this.colTriggerCode.Name = "colTriggerCode";
            // 
            // colPriceType
            // 
            this.colPriceType.Caption = "PriceType";
            this.colPriceType.FieldName = "PRICETYPE";
            this.colPriceType.Name = "colPriceType";
            // 
            // colAmountPercent
            // 
            this.colAmountPercent.Caption = "AmountPercent";
            this.colAmountPercent.FieldName = "AMOUNTPERCENT";
            this.colAmountPercent.Name = "colAmountPercent";
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = ((System.Drawing.Image)(resources.GetObject("btnPgUp.Image")));
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(30, 690);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Padding = new System.Windows.Forms.Padding(0);
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 14;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = ((System.Drawing.Image)(resources.GetObject("btnPgDown.Image")));
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(925, 688);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(65, 61);
            this.btnPgDown.TabIndex = 19;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSelect.Location = new System.Drawing.Point(375, 690);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 16;
            this.btnSelect.Tag = "BtnNormal";
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1024, 768);
            this.basePanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblPrompt, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPgUp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnUp, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.grItems, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnPgDown, 7, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnDown, 6, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnSelect, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelItemDescription, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1020, 764);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblPrompt
            // 
            this.lblPrompt.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblPrompt.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblPrompt, 8);
            this.lblPrompt.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblPrompt.Location = new System.Drawing.Point(442, 40);
            this.lblPrompt.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblPrompt.Name = "lblPrompt";
            this.lblPrompt.Size = new System.Drawing.Size(135, 65);
            this.lblPrompt.TabIndex = 7;
            this.lblPrompt.Text = "Label";
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(95, 688);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(65, 61);
            this.btnUp.TabIndex = 15;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click_1);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(852, 688);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(65, 61);
            this.btnDown.TabIndex = 18;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click_1);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel1.SetColumnSpan(this.tableLayoutPanel2, 8);
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnClear, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSearch, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.textBox, 0, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(26, 185);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(968, 37);
            this.tableLayoutPanel2.TabIndex = 15;
            // 
            // btnClear
            // 
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(908, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 31);
            this.btnClear.TabIndex = 13;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(845, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 31);
            this.btnSearch.TabIndex = 12;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.Location = new System.Drawing.Point(3, 3);
            this.textBox.Name = "textBox";
            this.textBox.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBox.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.textBox.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBox.Properties.Appearance.Options.UseBackColor = true;
            this.textBox.Properties.Appearance.Options.UseFont = true;
            this.textBox.Properties.Appearance.Options.UseForeColor = true;
            this.textBox.Size = new System.Drawing.Size(836, 32);
            this.textBox.TabIndex = 9;
            this.textBox.TextChanged += new System.EventHandler(this.textBox_TextChanged);
            this.textBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InfocodeKeyDown_KeyDown);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnCancel.Location = new System.Drawing.Point(510, 690);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            // 
            // labelItemDescription
            // 
            this.labelItemDescription.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelItemDescription.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelItemDescription, 8);
            this.labelItemDescription.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelItemDescription.Location = new System.Drawing.Point(510, 141);
            this.labelItemDescription.Name = "labelItemDescription";
            this.labelItemDescription.Size = new System.Drawing.Size(0, 37);
            this.labelItemDescription.TabIndex = 20;
            // 
            // simpleButton1
            // 
            this.simpleButton1.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.simpleButton1.Location = new System.Drawing.Point(965, 3);
            this.simpleButton1.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.simpleButton1.Name = "simpleButton1";
            this.simpleButton1.Size = new System.Drawing.Size(50, 30);
            this.simpleButton1.TabIndex = 12;
            this.simpleButton1.Text = "Clear";
            // 
            // FormInfoCodeSubCodeList
            // 
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "FormInfoCodeSubCodeList";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textBox.Properties)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private void btnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();

            this.grItems.Select();

            CheckRowPosition();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            itemTable = GetInfoCodeList(string.Empty, this.textBox.Text, DefaultNumberOfRows);
            grItems.DataSource = itemTable;

            this.grItems.Select();
            isValueChanged = false;
            CheckRowPosition();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void SelectItem()
        {
            if (grdView.RowCount > 0)
            {
                System.Data.DataRow Row = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
                selectedDescription = (string)Row["DESCRIPTION"];
                selectedSubCodeId = (string)Row["SUBCODEID"];
                triggerCode = (string)Row["TRIGGERCODE"];
                triggerFunction = (int)Row["TRIGGERFUNCTION"];
                priceType = (int)Row["PRICETYPE"];
                amountPercent = (decimal)Row["AMOUNTPERCENT"];
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.textBox.Text = String.Empty;

            itemTable = GetInfoCodeList(string.Empty, string.Empty, DefaultNumberOfRows);
            grItems.DataSource = itemTable;

            this.grItems.Select();
            isValueChanged = true;

            textBox.Select();
        }

        private void btnUp_Click_1(object sender, EventArgs e)
        {
            grdView.MovePrev();

            this.grItems.Select();

            CheckRowPosition();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();

            GetNextRows();

            this.grItems.Select();

            CheckRowPosition();           
        }

        private void btnDown_Click_1(object sender, EventArgs e)
        {
            grdView.MoveNext();

            GetNextRows();

            this.grItems.Select();

            CheckRowPosition();           
        }

        private void GetNextRows()
        {
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                System.Data.DataRow Row = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
                string lastSubCodeId = Row["SUBCODEID"].ToString();

                itemTable.Merge(GetInfoCodeList(lastSubCodeId, this.textBox.Text, DefaultNumberOfRows));
                grdView.TopRowIndex = topRowIndex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <remarks>CA2100 The queries are already parametrized and the value of "numberOfItems" is hard coded. No sql injection threat.</remarks>
        /// <param name="fromName"></param>
        /// <param name="searchString"></param>
        /// <param name="numberOfItems"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The queries are already parametrized and the value of numberOfItems is hard coded. No sql injection threat.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing return object")]
        private DataTable GetInfoCodeList(string fromName, string searchString, int numberOfItems)
        {
            SqlConnection connection = ApplicationSettings.Database.LocalConnection;
            using (SqlCommand command = new SqlCommand())
            {
                StringBuilder queryString = new StringBuilder();

                queryString.AppendLine("SELECT ");

                if (numberOfItems != -1)
                {
                    queryString.AppendLine("TOP " + Convert.ToString(numberOfItems) + " ");
                }

                queryString.AppendLine(" ISNULL(SUBCODEID, '') AS SUBCODEID, ISNULL(PRICETYPE, 0) AS PRICETYPE, ISNULL(TRIGGERFUNCTION, 0) AS TRIGGERFUNCTION, ");
                queryString.AppendLine(" ISNULL(TRIGGERCODE, '') AS TRIGGERCODE, ISNULL(AMOUNTPERCENT, 0) AS AMOUNTPERCENT, ");
                queryString.AppendLine(" ISNULL(DESCRIPTION, '') AS DESCRIPTION ");
                queryString.AppendLine(" FROM RETAILINFORMATIONSUBCODETABLE ");
                queryString.AppendLine(" WHERE DATAAREAID=@DATAAREAID ");
                queryString.AppendLine(" AND INFOCODEID=@INFOCODEID ");

                SqlParameter dataAreaIdParm = command.Parameters.AddWithValue("@DATAAREAID", ApplicationSettings.Database.DATAAREAID);
                SqlParameter infoCodeParam = command.Parameters.AddWithValue("@INFOCODEID", infoCodeId);

                if (searchString.Length > 0)
                {
                    queryString.AppendLine("AND (DESCRIPTION LIKE @SearchString) "); // OR PRICE LIKE @SearchString) ";
                    command.Parameters.AddWithValue("@SEARCHSTRING", '%' + searchString + '%');
                }

                if (fromName.Length > 0)
                {
                    queryString.AppendLine("AND SUBCODEID > @FROMDESCRIPTION ");
                    command.Parameters.AddWithValue("@FROMDESCRIPTION", fromName);
                }

                queryString.AppendLine("ORDER BY SUBCODEID");

                DataTable subcodeTable = new DataTable();

                try
                {
                    command.CommandText = queryString.ToString();
                    command.Connection = connection;
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        subcodeTable.Load(reader);
                    }
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }

                FormatDisplayText(subcodeTable);

                return subcodeTable;
            }

        }

        /// <summary>
        /// Format the Price/Amount text for display
        /// </summary>
        /// <param name="subcodeTable"></param>
        private static void FormatDisplayText(DataTable subcodeTable)
        {
            // Set DisplayPrice
            subcodeTable.Columns.Add(new DataColumn("PRICE", typeof(string)));

            string normalPrice = LSRetailPosis.ApplicationLocalizer.Language.Translate(60009);
            string displayPrice = string.Empty;
            IRounding rounding = InfoCodes.InternalApplication.Services.Rounding;

            foreach (DataRow row in subcodeTable.Rows)
            {
                decimal amount = (decimal)row["AMOUNTPERCENT"];
                switch ((LSRetailPosis.POSProcesses.PriceTypeEnum)row["PRICETYPE"])
                {
                    case LSRetailPosis.POSProcesses.PriceTypeEnum.FromItem:
                        displayPrice = normalPrice;
                        break;

                    case LSRetailPosis.POSProcesses.PriceTypeEnum.Price:
                        displayPrice = rounding.RoundForDisplay(amount, ApplicationSettings.Terminal.StoreCurrency, true, false);
                        break;

                    case LSRetailPosis.POSProcesses.PriceTypeEnum.Percent:
                        displayPrice = string.Format("{0}%", rounding.Round(amount, false));
                        break;

                    default:
                        displayPrice = string.Empty;
                        break;
                }

                row["PRICE"] = displayPrice;
            }
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

            isValueChanged = true;
            textBox.Select();          
        }

        private void CheckRowPosition()
        {
            btnPgDown.Enabled = (grdView.IsLastRow) ? false : true;
            btnDown.Enabled = btnPgDown.Enabled;
            btnPgUp.Enabled = (grdView.IsFirstRow) ? false : true;
            btnUp.Enabled = btnPgUp.Enabled;
            textBox.Select();
        }

        private void grdView_Click(object sender, EventArgs e)
        {
            CheckRowPosition();           
        }

        private void InfocodeKeyDown_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {               
                case Keys.Return:
                    if (isValueChanged == true)
                        btnSearch_Click(sender, e);
                    else
                        SelectItem();
                    break;
                case Keys.Up:
                    btnUp_Click_1(sender, e);
                    break;
                case Keys.Down:
                    btnDown_Click_1(sender, e);
                    break;
                case Keys.PageUp:
                    btnUp_Click(sender, e);
                    break;
                case Keys.PageDown:
                    btnDown_Click(sender, e);
                    break;                
                default:
                    break;
            }
        }      
    }
}