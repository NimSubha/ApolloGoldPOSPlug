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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmTaxOverrideSearch.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "frm")]
	public class frmTaxOverrideSearch : frmTouchBase
    {
        /// <summary>
        /// Formatter that converts PurchaseOrderReceiptStatus enum values in a grid column to user-displayable text
        /// </summary>
        private class EmptyStringFormatter : IFormatProvider, ICustomFormatter
        {
            private readonly string EmptyText;

            /// <summary>
            /// Formats status settings.
            /// </summary>
            public EmptyStringFormatter(string text)
            {
                this.EmptyText = text;
            }

            /// <summary>
            /// The GetFormat method of the IFormatProvider interface.
            /// This must return an object that provides formatting services for the specified type.
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public object GetFormat(System.Type type)
            {
                return this;
            }

            /// <summary>
            /// The Format method of the ICustomFormatter interface.
            /// This must format the specified value according to the specified format settings.
            /// </summary>
            /// <param name="format"></param>
            /// <param name="arg"></param>
            /// <param name="formatProvider"></param>
            /// <returns></returns>
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                string value = string.Empty;
                if (arg != null)
                {
                    value = arg.ToString();
                }
                return (string.IsNullOrEmpty(value)) ? this.EmptyText : value;
            }
        }

        private String selectedOverrideCode;
        private TaxOverrideBy taxOverrideBy;
        private System.Data.DataTable overrideTable;
        private bool showGroupInfo; // = false;
        private int getHowManyRows;
        private System.Windows.Forms.Timer clockTimer;
        private int loadedCount; // = 0;
        private string sortBy = "CODE";
        private PanelControl basePanel;
        private bool sortAsc = true;
        private TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private GridColumn colCode;
        private GridColumn colDescription;
        private GridColumn colFromGroup;
        private GridColumn colToGroup;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSelect;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnGetTaxgrpInfo;
        private TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private SimpleButton btnSearch;
        private SimpleButton btnClear;
        private Label lblHeading;
        private System.ComponentModel.IContainer components;     
        private bool inputTextChange = false;

        private enum SearchBy
        {
            Name = 0,
            ItemId = 1,
            Group = 2
        }

        public frmTaxOverrideSearch()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public frmTaxOverrideSearch(TaxOverrideBy overrideBy, int howManyRows)
            : this()
        {
            this.taxOverrideBy = overrideBy;
            getHowManyRows = (howManyRows <= 0) ? 1000 : howManyRows;

            colFromGroup.DisplayFormat.Format = new EmptyStringFormatter(LSRetailPosis.ApplicationLocalizer.Language.Translate(3986));  //any
            colToGroup.DisplayFormat.Format = new EmptyStringFormatter(LSRetailPosis.ApplicationLocalizer.Language.Translate(3987));    //none

        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.clockTimer = new System.Windows.Forms.Timer();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colFromGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colToGroup = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.lblHeading = new System.Windows.Forms.Label();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnGetTaxgrpInfo = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // clockTimer
            // 
            this.clockTimer.Interval = 1000;
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel2);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Margin = new System.Windows.Forms.Padding(0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1024, 768);
            this.basePanel.TabIndex = 10;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 9;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.grItems, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnPgDown, 8, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnDown, 7, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnPgUp, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnUp, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnSelect, 3, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnGetTaxgrpInfo, 4, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 5, 3);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel2.RowCount = 4;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1020, 764);
            this.tableLayoutPanel2.TabIndex = 11;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel2.SetColumnSpan(this.grItems, 9);
            this.grItems.Cursor = System.Windows.Forms.Cursors.Default;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(30, 184);
            this.grItems.MainView = this.grdView;
            this.grItems.Margin = new System.Windows.Forms.Padding(4);
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(960, 485);
            this.grItems.TabIndex = 4;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            this.grItems.Click += new System.EventHandler(this.grItems_Click);
            this.grItems.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            this.grItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colCode,
            this.colDescription,
            this.colFromGroup,
            this.colToGroup});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
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
            this.grdView.EndSorting += new System.EventHandler(this.grdView_EndSorting);
            this.grdView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            this.grdView.Click += new System.EventHandler(this.grdView_Click);
            // 
            // colCode
            // 
            this.colCode.Caption = "Override Name";
            this.colCode.FieldName = "CODE";
            this.colCode.Name = "colCode";
            this.colCode.OptionsColumn.AllowEdit = false;
            this.colCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCode.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colCode.Visible = true;
            this.colCode.VisibleIndex = 0;
            this.colCode.Width = 168;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "DESCRIPTION";
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colDescription.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            this.colDescription.Width = 482;
            // 
            // colFromGroup
            // 
            this.colFromGroup.Caption = "From Group";
            this.colFromGroup.FieldName = "SOURCETAXGROUP";
            this.colFromGroup.Name = "colFromGroup";
            this.colFromGroup.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colFromGroup.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colFromGroup.Width = 147;
            // 
            // colToGroup
            // 
            this.colToGroup.Caption = "To Group";
            this.colToGroup.FieldName = "DESTINATIONTAXGROUP";
            this.colToGroup.Name = "colToGroup";
            this.colToGroup.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colToGroup.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colToGroup.Width = 113;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(924, 688);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(65, 61);
            this.btnPgDown.TabIndex = 16;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPageDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(851, 688);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(65, 61);
            this.btnDown.TabIndex = 15;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(30, 688);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(65, 61);
            this.btnPgUp.TabIndex = 10;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPageUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(103, 688);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(65, 61);
            this.btnUp.TabIndex = 11;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel1, 9);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtKeyboardInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(30, 139);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(960, 37);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // txtKeyboardInput
            // 
            this.txtKeyboardInput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtKeyboardInput.Location = new System.Drawing.Point(0, 3);
            this.txtKeyboardInput.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.txtKeyboardInput.Name = "txtKeyboardInput";
            this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
            this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtKeyboardInput.Size = new System.Drawing.Size(831, 32);
            this.txtKeyboardInput.TabIndex = 1;
            this.txtKeyboardInput.TextChanged += new System.EventHandler(this.txtKeyboardInput_TextChanged);
            this.txtKeyboardInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmTaxOverrideSearch_KeyUp);
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(837, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 31);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(900, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 31);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.lblHeading, 9);
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(305, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(410, 95);
            this.lblHeading.TabIndex = 17;
            this.lblHeading.Tag = "";
            this.lblHeading.Text = "Tax override search";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(305, 690);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 13;
            this.btnSelect.Tag = "";
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnGetTaxgrpInfo
            // 
            this.btnGetTaxgrpInfo.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnGetTaxgrpInfo.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnGetTaxgrpInfo.Appearance.Options.UseFont = true;
            this.btnGetTaxgrpInfo.Appearance.Options.UseTextOptions = true;
            this.btnGetTaxgrpInfo.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnGetTaxgrpInfo.Location = new System.Drawing.Point(440, 690);
            this.btnGetTaxgrpInfo.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnGetTaxgrpInfo.Name = "btnGetTaxgrpInfo";
            this.btnGetTaxgrpInfo.Padding = new System.Windows.Forms.Padding(0);
            this.btnGetTaxgrpInfo.Size = new System.Drawing.Size(134, 57);
            this.btnGetTaxgrpInfo.TabIndex = 14;
            this.btnGetTaxgrpInfo.Tag = "";
            this.btnGetTaxgrpInfo.Text = "Get Tax Grp Info";
            this.btnGetTaxgrpInfo.Click += new System.EventHandler(this.btnShowInfo_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(584, 690);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 15, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Cancel";
            // 
            // frmTaxOverrideSearch
            // 
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmTaxOverrideSearch";
            this.Text = "Tax override search";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        public String SelectedTaxOverrideCode
        {
            get { return selectedOverrideCode; }
        }

        protected override void OnLoad(EventArgs e)
        {

            grdView.FocusedColumn = grdView.Columns["CODE"];            

            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmItemSearch are reserved at 3980-4000
            // In use now are ID's 3980 - 3985
            //

            btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61500);//Clear
			btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61508); //OK 
            btnGetTaxgrpInfo.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(3980); //Get tax group information
            btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(103125); //Cancel
                               
            grdView.Columns["CODE"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(3982); //Override name
            grdView.Columns["DESCRIPTION"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(3983); //Description
            grdView.Columns["SOURCETAXGROUP"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(3984); //From tax group
            grdView.Columns["DESTINATIONTAXGROUP"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(3985); //To tax group      

            lblHeading.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61515); //Tax override search
            this.Bounds = new Rectangle(
                new Point(ApplicationSettings.MainWindowLeft, ApplicationSettings.MainWindowTop),
                new Size(ApplicationSettings.MainWindowWidth, ApplicationSettings.MainWindowHeight));

            overrideTable = GetOverrideList(0, string.Empty, getHowManyRows);
            grItems.DataSource = overrideTable;
            CheckRowPosition();
            txtKeyboardInput.Select();

            this.clockTimer.InvokeOnTick(
                ApplicationSettings.Terminal.AutoLogOffTimeOutInMin * 30, /*seconds per half-minute (half-time for popups)*/
                this.Close); 
            
            base.OnLoad(e);
        }

        private void btnPageDown_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MoveNextPage();
            int topRowIndex = grdView.TopRowIndex;

            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                overrideTable.Merge(GetOverrideList(loadedCount, txtKeyboardInput.Text, getHowManyRows));
                grdView.TopRowIndex = topRowIndex;
            }


            txtKeyboardInput.Select();
            CheckRowPosition();

        }

        private void btnPageUp_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MovePrevPage();
            txtKeyboardInput.Select();
            CheckRowPosition();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            SelectItem();
        }

        private void keyboard1_EnterButtonPressed()
        {
            clockTimer.Restart(); //reset the timer.

            if (txtKeyboardInput.Text.Trim().Length == 0 || inputTextChange == false)
            {
                btnSelect_Click(null, null);

            }
            else
            {
                overrideTable = GetOverrideList(0, txtKeyboardInput.Text, getHowManyRows);
                grItems.DataSource = overrideTable;
            }
            inputTextChange = false;
            CheckRowPosition();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.

            Point p = grdView.GridControl.PointToClient(MousePosition);
            GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest != GridHitTest.Column)
            {
                SelectItem();
            }
        }

        private void SelectItem()
        {
            clockTimer.Restart(); //reset the timer.
            if (grdView.RowCount > 0)
            {
                System.Data.DataRow Row = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
                selectedOverrideCode = (string)Row["CODE"];
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                txtKeyboardInput.Select();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            txtKeyboardInput.Text = string.Empty;
            overrideTable = GetOverrideList(0, "", getHowManyRows);
            grItems.DataSource = overrideTable;
            txtKeyboardInput.Select();
        }

        private void gridView1_FocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            txtKeyboardInput.Select();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MovePrev();
            txtKeyboardInput.Select();
            CheckRowPosition();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MoveNext();
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {

                overrideTable.Merge(GetOverrideList(loadedCount, txtKeyboardInput.Text, getHowManyRows));
                grdView.TopRowIndex = topRowIndex;
            }

            txtKeyboardInput.Select();
            CheckRowPosition();
        }

        private void btnShowInfo_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            try
            {
                showGroupInfo = !showGroupInfo;
                colFromGroup.Visible = showGroupInfo;
                colToGroup.Visible = showGroupInfo;
                btnGetTaxgrpInfo.Text = showGroupInfo ? ApplicationLocalizer.Language.Translate(3981) //Get tax information
                                                            : ApplicationLocalizer.Language.Translate(3980); //Hide tax information				
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException("frmItemSearch.btnShowInfo_Click", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <remarks>CA2100 The queries are already parametrized and the value of "sortBy" is hard coded. No sql injection threat.</remarks>
        /// <param name="fromRow"></param>
        /// <param name="searchString"></param>
        /// <param name="numberOfRows"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The queries are already parametrized and the value of sortBy is hard coded. No sql injection threat.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]
        private DataTable GetOverrideList(int fromRow, string searchString, int numberOfRows)
        {
            /*
             * Server side sorting using the SQL Server 2005 "over" operator to limit the rowset
             * The raw SQL statement with populated example parameters 
             * 
               Select I.ItemGroupId, I.ItemId, I.ItemName, I.RetailItemGroupId,'' as ItemPrice from 
                    (select it.ItemGroupId,it.ItemID,it.ItemName,it.DATAAREAID,rt.ItemGroup as RetailItemGroupId, ROW_NUMBER()     
                     over (order by it.ItemName Desc) as row     
                     from InventTable it     join RboInventTable rt on rt.DATAAREAID = it.DATAAREAID and it.ItemId = rt.ItemId
                     where(it.ItemName like '%4050200%' or it.ItemGroupId like '%4050200%' or it.ItemId like '%4050200%')  ) I 
                     where I.DATAAREAID='wis' and I.row > 0 and I.row <= 500 
            */

            String asc = sortAsc ? " ASC" : " DESC";
            SqlConnection connection = ApplicationSettings.Database.LocalConnection;
            using (SqlCommand command = new SqlCommand())
            {
                string search = string.Empty;

                if (searchString.Length > 0)
                {
                    search = " WHERE ((STO.CODE LIKE @SEARCHSTRING) OR (STO.DESCRIPTION LIKE @SEARCHSTRING) "
                            + " OR (STO.SOURCETAXGROUP LIKE @SEARCHSTRING) OR (STO.DESTINATIONTAXGROUP LIKE @SEARCHSTRING) "
                            + " OR (STO.SOURCEITEMTAXGROUP LIKE @SEARCHSTRING) OR (STO.DESTINATIONITEMTAXGROUP LIKE @SEARCHSTRING)) ";
                }

                string query = "SELECT T.CODE, T.DESCRIPTION, " +
                                " CASE WHEN T.OVERRIDETYPE = 0 THEN T.SOURCEITEMTAXGROUP ELSE T.SOURCETAXGROUP END AS SOURCETAXGROUP, " +
                                " CASE WHEN T.OVERRIDETYPE = 0 THEN T.DESTINATIONITEMTAXGROUP ELSE T.DESTINATIONTAXGROUP END AS DESTINATIONTAXGROUP " +
                               "FROM ( " +
                               "    SELECT STO.CODE, STO.DESCRIPTION, STO.SOURCETAXGROUP, STO.DESTINATIONTAXGROUP, STO.SOURCEITEMTAXGROUP, " +
                               "    STO.DESTINATIONITEMTAXGROUP, STO.OVERRIDETYPE, STO.OVERRIDEBY, STO.DATAAREAID, S.STORENUMBER, " +
                               "    ROW_NUMBER() OVER (ORDER BY STO." + sortBy + asc + ") AS ROW " +
                               "    FROM RETAILSALESTAXOVERRIDE STO " +
                               "      INNER JOIN RETAILSALESTAXOVERRIDEGROUPMEMBER GM ON STO.CODE = GM.RBOSALESTAXOVERRIDECODE " +
                               "      INNER JOIN RETAILSALESTAXOVERRIDEGROUP ROG ON ROG.CODE = GM.RBOSALESTAXOVERRIDEGROUPCODE " +
                               "      INNER JOIN RETAILSTORETABLE S ON S.TAXOVERRIDEGROUP = ROG.RECID " + search + ") T " +
                               "WHERE (T.OVERRIDEBY = @OVERRIDEBY) AND (T.DATAAREAID = @DATAAREAID) AND " +
                               "    (T.STORENUMBER = @STORENUMBER) AND (T.ROW > @FROMROW) AND (T.ROW <= @TOROW) ";

                command.Parameters.AddWithValue("@DATAAREAID", ApplicationSettings.Database.DATAAREAID);
                command.Parameters.AddWithValue("@FROMROW", fromRow);
                command.Parameters.AddWithValue("@TOROW", (fromRow + numberOfRows));
                command.Parameters.AddWithValue("@OVERRIDEBY", this.taxOverrideBy);
                command.Parameters.AddWithValue("@STORENUMBER", ApplicationSettings.Terminal.StoreId);

                if (searchString.Length > 0)
                {
                    command.Parameters.Add("@SEARCHSTRING", SqlDbType.NVarChar, 100).Value = '%' + searchString + '%';
                }

                foreach (GridColumn col in grdView.Columns)
                {
                    col.SortOrder = ColumnSortOrder.None;
                }
                grdView.Columns[sortBy].SortOrder = sortAsc ? ColumnSortOrder.Ascending : ColumnSortOrder.Descending;

                DataTable table = new DataTable();

                try
                {
                    loadedCount = fromRow + numberOfRows;

                    command.CommandText = query;
                    command.Connection = connection;
                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        table.Load(reader);
                    }
                }
                finally
                {
                    if (connection.State == ConnectionState.Open) { connection.Close(); }
                }
                return table;
            }
        }

        private void keyboard1_DownButtonPressed()
        {
            btnDown_Click(this, new EventArgs());
        }

        private void keyboard1_UpButtonPressed()
        {
            btnUp_Click(this, new EventArgs());
        }

        private void keyboard1_PgDownButtonPressed()
        {
            btnPageDown_Click(this, new EventArgs());
        }

        private void keyboard1_PgUpButtonPressed()
        {
            btnPageUp_Click(this, new EventArgs());
        }

        private void grdView_EndSorting(object sender, EventArgs e)
        {
            grItems.DataSource = overrideTable;
        }

        private void keyboard1_EnterButtonPressedWithoutInputChange()
        {
            SelectItem();
        }

        private void grItems_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
        }

        private void grItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnSelect_Click(null, null);
            }
            CheckRowPosition();
        }

        private void frmTaxOverrideSearch_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    keyboard1_EnterButtonPressed();
                    break;
                case Keys.Up:
                    keyboard1_UpButtonPressed();
                    break;
                case Keys.Down:
                    keyboard1_DownButtonPressed();
                    break;
                case Keys.PageUp:
                    keyboard1_PgUpButtonPressed();
                    break;
                case Keys.PageDown:
                    keyboard1_PgDownButtonPressed();
                    break;
                default:
                    break;
            }
        }

        private void txtKeyboardInput_TextChanged(object sender, EventArgs e)
        {
            inputTextChange = true;
        }


        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (inputTextChange == true)
            {
                overrideTable = GetOverrideList(0, txtKeyboardInput.Text, getHowManyRows);
                grItems.DataSource = overrideTable;
                inputTextChange = false;
                CheckRowPosition();
            }

        }
        private void CheckRowPosition()
        {
            btnPgDown.Enabled = (grdView.IsLastRow) ? false : true;
            btnDown.Enabled = btnPgDown.Enabled;
            btnPgUp.Enabled = (grdView.IsFirstRow) ? false : true;
            btnUp.Enabled = btnPgUp.Enabled;
        }

        private void grdView_Click(object sender, EventArgs e)
        {
            Point p = grdView.GridControl.PointToClient(MousePosition);
            GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest == GridHitTest.Column)
            {
                sortAsc = (sortBy == info.Column.FieldName) ? (!sortAsc) : true;
                sortBy = info.Column.FieldName;
                loadedCount = 0;

                overrideTable = GetOverrideList(0, txtKeyboardInput.Text, getHowManyRows);
                grItems.DataSource = overrideTable;
            }
            CheckRowPosition();
            txtKeyboardInput.Select();
        }

    }
}

