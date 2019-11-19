/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.DataAccess;

namespace Microsoft.Dynamics.Retail.Pos.Item.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmSerialIdSearch.
    /// </summary>
    public class frmSerialIdSearch : LSRetailPosis.POSProcesses.frmTouchBase
    {
        private DevExpress.XtraGrid.Blending.XtraGridBlending xtraGridBlending1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colInventSerialId;
        private DevExpress.XtraGrid.Columns.GridColumn colRFIDTagId;
        private String selectedSerialId;
        private string selectedRFIDTagId;
        private PanelControl basePanel;
        private System.Data.DataTable itemTable;
        private int getHowManyRows;
        private string itemId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSelect;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private SimpleButton btnSearch;
        private SimpleButton btnClear;
        private bool inputHasChanged = false;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel7;
        private Label lblHeader;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        /// <summary>
        /// Set the property of item id.
        /// </summary>
        public string ItemId
        {
            set { itemId = value; }
        }

        /// <summary>
        /// Fetch details of all those serial id as per given no of rows.
        /// </summary>
        /// <param name="howManyRows"></param>
        public frmSerialIdSearch(int howManyRows)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            SettingsData settingsData = new SettingsData(LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection, LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);

            grdView.FocusedColumn = grdView.Columns["INVENTSERIALID"];

            //Set the size of the form the same as the main form
            this.Bounds = new Rectangle(
                    LSRetailPosis.Settings.ApplicationSettings.MainWindowLeft,
                    LSRetailPosis.Settings.ApplicationSettings.MainWindowTop,
                    LSRetailPosis.Settings.ApplicationSettings.MainWindowWidth,
                    LSRetailPosis.Settings.ApplicationSettings.MainWindowHeight
                    );

            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmSerialIdSearch are reserved at 61700 - 61799
            // In use now are ID's 61700 - 61713
            //
            btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61700); //Clear			
            btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61703); //Select 

            grdView.Columns["INVENTSERIALID"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(61705); //Serial no.
            grdView.Columns["RFIDTAGID"].Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(61706); //RFID tag     

            lblHeader.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61708); //Serial ID search

            if (howManyRows == 0) { getHowManyRows = 1000; } else { getHowManyRows = howManyRows; }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSerialIdSearch));
            this.xtraGridBlending1 = new DevExpress.XtraGrid.Blending.XtraGridBlending();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colInventSerialId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRFIDTagId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            this.tableLayoutPanel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // xtraGridBlending1
            // 
            this.xtraGridBlending1.AlphaStyles.AddReplace("Row", 220);
            this.xtraGridBlending1.GridControl = this.grItems;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(29, 181);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(956, 466);
            this.grItems.TabIndex = 20;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colInventSerialId,
            this.colRFIDTagId});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridView1_FocusedColumnChanged);
            this.grdView.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colInventSerialId
            // 
            this.colInventSerialId.Caption = "Serial number";
            this.colInventSerialId.FieldName = "INVENTSERIALID";
            this.colInventSerialId.Name = "colInventSerialId";
            this.colInventSerialId.OptionsColumn.AllowEdit = false;
            this.colInventSerialId.Visible = true;
            this.colInventSerialId.VisibleIndex = 0;
            this.colInventSerialId.Width = 350;
            // 
            // colRFIDTagId
            // 
            this.colRFIDTagId.Caption = "RFID tag";
            this.colRFIDTagId.FieldName = "RFIDTAGID";
            this.colRFIDTagId.Name = "colRFIDTagId";
            this.colRFIDTagId.Visible = true;
            this.colRFIDTagId.VisibleIndex = 1;
            this.colRFIDTagId.Width = 365;
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel6);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1018, 743);
            this.basePanel.TabIndex = 8;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.grItems, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel6.RowCount = 4;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1014, 739);
            this.tableLayoutPanel6.TabIndex = 11;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 85.51881F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 58F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tableLayoutPanel1.Controls.Add(this.txtKeyboardInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(29, 138);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(954, 37);
            this.tableLayoutPanel1.TabIndex = 10;
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
            this.txtKeyboardInput.Size = new System.Drawing.Size(831, 32);
            this.txtKeyboardInput.TabIndex = 1;
            this.txtKeyboardInput.TextChanged += new System.EventHandler(this.txtKeyboardInput_TextChanged);
            this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyboardInput_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(840, 3);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(50, 30);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(900, 3);
            this.btnClear.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(50, 30);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 7;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel7.Controls.Add(this.btnPgDown, 6, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnSelect, 3, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnDown, 5, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(29, 653);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(956, 72);
            this.tableLayoutPanel7.TabIndex = 12;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = ((System.Drawing.Image)(resources.GetObject("btnPgDown.Image")));
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(887, 11);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(65, 57);
            this.btnPgDown.TabIndex = 8;
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(417, 11);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Tag = "";
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(814, 11);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(65, 57);
            this.btnDown.TabIndex = 7;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click_1);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(77, 11);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Padding = new System.Windows.Forms.Padding(10, 3, 3, 3);
            this.btnUp.Size = new System.Drawing.Size(70, 57);
            this.btnUp.TabIndex = 5;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click_1);
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = ((System.Drawing.Image)(resources.GetObject("btnPgUp.Image")));
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 11);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(65, 57);
            this.btnPgUp.TabIndex = 4;
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(343, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(328, 65);
            this.lblHeader.TabIndex = 21;
            this.lblHeader.Text = "Serial id search";
            // 
            // frmSerialIdSearch
            // 
            this.ClientSize = new System.Drawing.Size(1018, 743);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmSerialIdSearch";
            this.Load += new System.EventHandler(this.frmSerialIdSearch_Load);
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            this.tableLayoutPanel7.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary>
        /// Get property of serial id as string return type.
        /// </summary>
        public String SelectedSerialId
        {
            get { return selectedSerialId; }
        }

        /// <summary>
        /// Get property of RFIDTagId as string return type.
        /// </summary>
        public String SelectedRFIDTagId
        {
            get { return selectedRFIDTagId; }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmSerialIdSearch_Load(object sender, EventArgs e)
        {
            this.Top = LSRetailPosis.Settings.ApplicationSettings.MainWindowTop;
            this.Left = LSRetailPosis.Settings.ApplicationSettings.MainWindowLeft;

            itemTable = GetItemList("", "", getHowManyRows);
            grItems.DataSource = itemTable;
            txtKeyboardInput.Select();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                System.Data.DataRow Row = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
                string lastItemName = Row["INVENTSERIALID"].ToString();
                itemTable.Merge(GetItemList(lastItemName, txtKeyboardInput.Text, getHowManyRows));
                grdView.TopRowIndex = topRowIndex;
            }

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
            if (inputHasChanged == true)
            {
                itemTable = GetItemList("", txtKeyboardInput.Text, getHowManyRows);
                grItems.DataSource = itemTable;
            }
            else
            {
                SelectItem();
            }
            inputHasChanged = false;
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
                selectedSerialId = (string)Row["INVENTSERIALID"];
                selectedRFIDTagId = (string)Row["RFIDTAGID"];
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
            else
            {
                txtKeyboardInput.Select();
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtKeyboardInput.Text = string.Empty;
            itemTable = GetItemList("", "", getHowManyRows);
            grItems.DataSource = itemTable;
            txtKeyboardInput.Select();
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
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                System.Data.DataRow Row = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
                string lastItemName = Row["INVENTSERIALID"].ToString();
                itemTable.Merge(GetItemList(lastItemName, txtKeyboardInput.Text, getHowManyRows));
                grdView.TopRowIndex = topRowIndex;
            }

            txtKeyboardInput.Select();
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The queries are already parametrized and the value of numberOfItems is hard coded. No sql injection threat.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]

        private DataTable GetItemList(string fromName, string searchString, int numberOfItems)
        {

            string queryString = "SELECT ";

            using (SqlConnection connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection)
            {

                using (SqlCommand command = new SqlCommand())
                {
                    if (numberOfItems == -1)
                    {
                        queryString += " ISNULL(INVENTSERIALID, '') AS INVENTSERIALID, ISNULL(RFIDTAGID, '') AS RFIDTAGID FROM INVENTSERIAL ";
                    }
                    else
                    {
                        queryString += "TOP " + Convert.ToString(numberOfItems) + " ISNULL(INVENTSERIALID, '') AS INVENTSERIALID, ISNULL(RFIDTAGID, '') AS RFIDTAGID FROM INVENTSERIAL ";
                    }

                    queryString += "WHERE DATAAREAID=@DATAAREAID AND ITEMID = @ITEMID ";
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID;
                    SqlParameter itemIdParm = command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20);
                    itemIdParm.Value = itemId;

                    if (searchString.Length > 0)
                    {
                        queryString += "AND (INVENTSERIALID LIKE @SEARCHSTRING OR RFIDTAGID LIKE @SEARCHSTRING) ";
                        SqlParameter searchStringParm = command.Parameters.Add("@SEARCHSTRING", SqlDbType.NVarChar, 100);
                        searchStringParm.Value = '%' + searchString + '%';
                    }
                    if (fromName.Length > 0)
                    {
                        queryString += "AND INVENTSERIALID > @FROMSERIALID ";
                        SqlParameter fromItemParm = command.Parameters.Add("@FROMSERIALID", SqlDbType.NVarChar, 30);
                        fromItemParm.Value = fromName;
                    }

                    queryString += "ORDER BY INVENTSERIALID";

                    DataTable customerTable = new DataTable();

                    command.CommandText = queryString;
                    command.Connection = connection;
                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    SqlDataReader reader = command.ExecuteReader();
                    customerTable.Load(reader);

                    return customerTable;
                }
            }

        }

        private void keyboard1_DownButtonPressed()
        {
            btnDown_Click_1(this, new EventArgs());
        }

        private void keyboard1_UpButtonPressed()
        {
            btnUp_Click_1(this, new EventArgs());
        }

        private void keyboard1_PgDownButtonPressed()
        {
            btnDown_Click(this, new EventArgs());
        }

        private void keyboard1_PgUpButtonPressed()
        {
            btnUp_Click(this, new EventArgs());
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            itemTable = GetItemList("", txtKeyboardInput.Text, getHowManyRows);
            grItems.DataSource = itemTable;
            inputHasChanged = false;
        }

        private void txtKeyboardInput_KeyDown(object sender, KeyEventArgs e)
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
            inputHasChanged = true;
        }
    }
}

