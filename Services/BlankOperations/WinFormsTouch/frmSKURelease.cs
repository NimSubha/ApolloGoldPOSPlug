using System;
using System.Data;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using Microsoft.CSharp.RuntimeBinder;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public class frmSKURelease : frmTouchBase
    {
        private DataRow selectedDataRow;
        private const string filter = "{0} LIKE '%{1}%' ";
        private readonly string orFilter = "OR " + filter;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private DataTable dataTable;
        private TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private SimpleButton btnClear;
        private SimpleButton btnSearch;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel2;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSelect;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private PanelControl basePanel;
        private bool inputHasChanged = false;
        private Label lblHeading;
        private IApplication application;

        private frmSKURelease()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
            }
        }

        /// <summary>
        /// Sets the size and position of teh form and componments.
        /// </summary>
        /// <param name="dataTable"></param>  
        /// <param name="selectedRow"></param>
        /// <param name="title"></param>
        public frmSKURelease(System.Data.DataTable dataTable, DataRow selectedRow, string title, IApplication Application)
            : this()
        {
            if (dataTable == null)
                throw new ArgumentNullException("dataTable");

            this.dataTable = dataTable;
            this.selectedDataRow = selectedRow;
            this.lblHeading.Text = title;
            application = Application;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                this.SetBounds(
                    ApplicationSettings.MainWindowLeft,
                    ApplicationSettings.MainWindowTop,
                    ApplicationSettings.MainWindowWidth,
                    ApplicationSettings.MainWindowHeight);

                TranslateLabels();

                LoadData();

                txtKeyboardInput.Select();
            }
            base.OnLoad(e);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void LoadData()
        {
            int index = 0;
            foreach (DataColumn dataCol in dataTable.Columns)
            {
                DevExpress.XtraGrid.Columns.GridColumn column = new DevExpress.XtraGrid.Columns.GridColumn();

                column.AppearanceCell.Options.UseTextOptions = true;
                if (dataCol.DataType.Name.IndexOf("STRING", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
                }
                else
                {
                    column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                    column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
                }
                column.Caption = dataCol.Caption;
                column.FieldName = dataCol.ColumnName;
                column.Name = "datacol" + index.ToString();
                column.Visible = true;
                column.VisibleIndex = index;
                this.grdView.Columns.Add(column);
                index++;
            }

            grItems.DataSource = dataTable;

            if (selectedDataRow != null)
                grdView.FocusedRowHandle = grdView.GetRowHandle(dataTable.Rows.IndexOf(selectedDataRow));
        }

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmItemSearch are reserved at 1280 - 1299
            // In use now are ID's 1280 - 1290
            //
            btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1280); //Clear         
            btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1283); //Select 
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSKURelease));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.lblHeading = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtKeyboardInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(26, 135);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(778, 38);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // txtKeyboardInput
            // 
            this.txtKeyboardInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyboardInput.Location = new System.Drawing.Point(3, 3);
            this.txtKeyboardInput.Name = "txtKeyboardInput";
            this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
            this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtKeyboardInput.Size = new System.Drawing.Size(646, 32);
            this.txtKeyboardInput.TabIndex = 1;
            this.txtKeyboardInput.TextChanged += new System.EventHandler(this.txtKeyboardInput_TextChanged);
            this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GenericSearch_KeyDown);
            // 
            // btnClear
            // 
            this.btnClear.Image = Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            //  this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(718, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearch
            // 
            //    this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.search;

            this.btnSearch.Image = Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(655, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 32);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(253, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(324, 95);
            this.lblHeading.TabIndex = 41;
            this.lblHeading.Text = "Generic search";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.grItems, 0, 2);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(830, 611);
            this.tableLayoutPanel3.TabIndex = 13;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 8;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnPgDown, 7, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSelect, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDown, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(26, 535);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(778, 65);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;

            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(717, 4);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 15;
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(393, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 14;
            this.btnCancel.Text = "Close";
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;

            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 4);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 10;
            this.btnPgUp.Tag = "BtnNormal";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(258, 4);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 13;
            this.btnSelect.Tag = "";
            this.btnSelect.Text = "Release";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;

            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(652, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 14;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click_1);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;

            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(69, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 11;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click_1);
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.grItems.Location = new System.Drawing.Point(30, 177);
            this.grItems.MainView = this.grdView;
            this.grItems.Margin = new System.Windows.Forms.Padding(4);
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(770, 343);
            this.grItems.TabIndex = 11;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView,
            this.gridView1});
            this.grItems.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // grdView
            // 
            this.grdView.Appearance.FooterPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdView.Appearance.FooterPanel.Options.UseFont = true;
            this.grdView.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.grdView.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.FooterPanelHeight = 40;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowColumnMoving = false;
            this.grdView.OptionsCustomization.AllowColumnResizing = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsCustomization.AllowGroup = false;
            this.grdView.OptionsCustomization.AllowQuickHideColumns = false;
            this.grdView.OptionsCustomization.AllowSort = false;
            this.grdView.OptionsMenu.EnableColumnMenu = false;
            this.grdView.OptionsMenu.EnableFooterMenu = false;
            this.grdView.OptionsMenu.EnableGroupPanelMenu = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsSelection.EnableAppearanceHideSelection = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 40;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // gridView1
            // 
            this.gridView1.Appearance.FooterPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.gridView1.Appearance.FooterPanel.Options.UseFont = true;
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView1.Appearance.Row.Options.UseFont = true;
            this.gridView1.GridControl = this.grItems;
            this.gridView1.Name = "gridView1";
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel3);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(834, 615);
            this.basePanel.TabIndex = 8;
            // 
            // frmGenericSearch
            // 
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(834, 615);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmSKURelease";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        /// <summary>
        /// Returns selected datarow as DataRow.
        /// </summary>
        public DataRow SelectedDataRow
        {
            get { return selectedDataRow; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
            txtKeyboardInput.Select();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();
            txtKeyboardInput.Select();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void keyboard1_EnterButtonPressed()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (DataColumn col in this.dataTable.Columns)
            {
                sb.AppendFormat(sb.Length == 0 ? filter : orFilter, col.ColumnName, txtKeyboardInput.Text);
            }

            grItems.DataSource = FilterTable("Table", dataTable, sb.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "rows and columns are added to the table, cannot dispose")]
        private static DataTable FilterTable(string tableName, DataTable sourceTable, string selectExpression)
        {
            using (DataTable dt = new DataTable(tableName))
            {
                DataColumn dataColumn;
                foreach (DataColumn sourceCol in sourceTable.Columns)
                {
                    dataColumn = new DataColumn(sourceCol.ColumnName, sourceCol.DataType);
                    dataColumn.Caption = sourceCol.Caption;
                    dt.Columns.Add(dataColumn);
                }

                foreach (DataRow dr in sourceTable.Select(selectExpression))
                {
                    int i = 0;
                    object[] o = new object[sourceTable.Columns.Count];
                    foreach (DataColumn col in sourceTable.Columns)
                        o[i++] = dr[col];

                    dt.Rows.Add(o);
                }

                return dt;
            }
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void SelectItem()
        {
            if (grdView.RowCount > 0)
            {
                selectedDataRow = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
                if (selectedDataRow != null)
                {

                    InputConfirmation inputconfirm = new InputConfirmation();
                    inputconfirm.PromptText = "SKU Release Reason ";
                    inputconfirm.InputType = InputType.Normal;
                    if (!string.IsNullOrEmpty(Convert.ToString(selectedDataRow["REASON"])))
                        inputconfirm.Text = Convert.ToString(selectedDataRow["REASON"]);

                    Interaction.frmInput Oinput = new Interaction.frmInput(inputconfirm);
                    Oinput.ShowDialog();
                    if (!string.IsNullOrEmpty(Oinput.InputText))
                    {
                        selectedDataRow["REASON"] = Oinput.InputText;
                        selectedDataRow["RELEASEDATE"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
                        ReleaseSKU(Convert.ToDateTime(selectedDataRow["RELEASEDATE"]), Convert.ToString(selectedDataRow["REASON"]), Convert.ToString(selectedDataRow["TRANSACTIONID"]), Convert.ToString(selectedDataRow["SKU"]), 1);

                        MessageBox.Show("SKU is successfully released.");
                    }
                    else
                    {
                        selectedDataRow["REASON"] = Oinput.InputText;
                        selectedDataRow["RELEASEDATE"] = "01/01/1900";
                        ReleaseSKU(null, string.Empty, Convert.ToString(selectedDataRow["TRANSACTIONID"]), Convert.ToString(selectedDataRow["SKU"]), 0);

                    }
                }

            }
            else
            {
                txtKeyboardInput.Select();
            }
        }

        #region  RELEASING SKU AND UPDATING IN TABLE
        private void ReleaseSKU(DateTime? releaseddate, string reason, string transid, string skunumber, int released)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SET DATEFORMAT DMY;  UPDATE retailcustomerdepositskudetails SET SKURELEASEDDATE='" + releaseddate + "' " +
                                      " ,RELEASEDTERMINALID='" + (releaseddate == null ? string.Empty : ApplicationSettings.Terminal.TerminalId) + "' " +
                                      " ,RELEASEDSTOREID='" + (releaseddate == null ? string.Empty : ApplicationSettings.Terminal.StoreId) + "' " +
                                      " ,RELEASEDSTAFFID='" + (releaseddate == null ? string.Empty : ApplicationSettings.Terminal.TerminalOperator.OperatorId) + "' " +
                                      " ,REASON='" + reason + "',RELEASED='" + released + "' WHERE TRANSID='" + transid + "' AND SKUNUMBER='" + skunumber + "'" +
                                      "  UPDATE SKUTABLE_POSTED SET CUSTORDERNUM = '' , CUSTORDERLINENUM=0 WHERE SKUNUMBER='" + skunumber + "'";

                if (released == 1)
                    SqlComm.CommandText += " UPDATE SKUTableTrans SET isAvailable = 1 WHERE SKUNUMBER='" + skunumber + "'"; //SKU Table 

                SqlComm.ExecuteNonQuery();

                if (isSKUFromCustOrder(skunumber))
                {
                    ReleaseSKUFromHO(skunumber);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        public bool isSKUFromCustOrder(string sSKU) 
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select A.isAvailable from SKUTableTrans A");
            sbQuery.Append(" left join SKUTable_Posted B on A.SkuNumber =B.SkuNumber");
            sbQuery.Append(" where b.CUSTORDERLINENUM !=0 and b.CUSTORDERNUM!='' and A.SkuNumber='" + sSKU + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());


        }

        private void ReleaseSKUFromHO(string sSKU)
        {
            bool bResult = false;
            try
            {
                if (this.Application.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = this.Application.TransactionServices.InvokeExtension("SKURelease", sSKU);
                                                                   
                    bResult = Convert.ToBoolean(containerArray[1]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format("Failed to release SKU from HO due to RTS issue."));
            }
            if (bResult == false)
            {
                MessageBox.Show(string.Format("Failed to release SKU from HO."));
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtKeyboardInput.Text = string.Empty;
            txtKeyboardInput.Select();
            btnSearch.PerformClick();
        }

        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            txtKeyboardInput.Select();
        }

        private void btnUp_Click_1(object sender, EventArgs e)
        {
            grdView.MovePrev();
            txtKeyboardInput.Select();
        }

        private void btnDown_Click_1(object sender, EventArgs e)
        {
            grdView.MoveNext();
            txtKeyboardInput.Select();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            keyboard1_EnterButtonPressed();
        }

        private void GenericSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    if (inputHasChanged == true)
                        keyboard1_EnterButtonPressed();
                    else
                        SelectItem();
                    inputHasChanged = false;
                    break;
                case Keys.Up:
                    btnUp_Click_1(sender, e);
                    break;
                case Keys.Down:
                    btnDown_Click_1(sender, e);
                    break;
                case Keys.PageUp:
                    grdView.MovePrevPage();
                    txtKeyboardInput.Select();
                    break;
                case Keys.PageDown:
                    grdView.MoveNextPage();
                    txtKeyboardInput.Select();
                    break;
                default:
                    break;
            }
        }

        private void txtKeyboardInput_TextChanged(object sender, EventArgs e)
        {
            inputHasChanged = true;
        }


    }
}
