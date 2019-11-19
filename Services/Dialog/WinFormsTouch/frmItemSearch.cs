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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    /// <summary>
    /// The types of Brazilian taxes.
    /// </summary>
    internal enum BrazilianTaxType
    {
        Blank = 0,
        Pis = 1,
        Icms = 2,
        Cofins = 3,
        Iss = 4,
        Irrf = 5,
        Inss = 6,
        Ipi = 8,
        ImportTax = 10,
        OtherTax = 11,
        InssRetained = 12
    }

    /// <summary>
    /// Summary description for frmItemSearch.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
    public class frmItemSearch : frmTouchBase
    {
        private DevExpress.XtraGrid.Blending.XtraGridBlending xtraGridBlending1;
        private String selectedItemId;
        private PanelControl basePanel;
        private System.Data.DataTable itemTable;
        private int getHowManyRows;

        private PanelControl panelControl1;
        private System.Windows.Forms.Timer clockTimer;
        private int loadedCount; // = 0;
        private string sortBy;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colItemName;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        private DevExpress.XtraGrid.Columns.GridColumn colItemPrice;
        private DevExpress.XtraGrid.Columns.GridColumn colItemUnitOfMeasure;
        private DevExpress.XtraGrid.Columns.GridColumn colItemTaxRate;
        private DevExpress.XtraGrid.Columns.GridColumn colItemRoundTrunc;
        private DevExpress.XtraGrid.Columns.GridColumn colItemOwnThirdProduc;
        private bool sortAsc = true;
        private TableLayoutPanel tableLayoutPanel1;
        private SimpleButton btnSearch;
        private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private SimpleButton btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSelect;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnGetprice;
        private TableLayoutPanel tableLayoutPanel6;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
        private Label lblHeading;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.ComponentModel.IContainer components;
        private bool inputHasChanged = false;

        //Start:Nim   
        public string selectedItemName { get; set; }
        public string selectedItemPrice { get; set; }
        private string sCondition = string.Empty;
        private string sCallName = string.Empty;
        //End:Nim

        protected frmItemSearch()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        public frmItemSearch(int howManyRows)
            : this()
        {
            if (howManyRows == 0)
            {
                getHowManyRows = 1000;
            }
            else if (howManyRows == -1)
            {
                // Just to be safe we handle -1 since in the past -1 was used to represent that all of the
                // records should be fetched.
                getHowManyRows = 500;
            }
            else
            {
                getHowManyRows = howManyRows;
            }
        }

        //Start:Nim
        public frmItemSearch(int howManyRows, string condition = "", string sCallFrom = "")
            : this()
        {
            sCondition = condition;
            sCallName = sCallFrom;

            if (howManyRows == 0)
            {
                getHowManyRows = 1000;
            }
            else if (howManyRows == -1)
            {
                // Just to be safe we handle -1 since in the past -1 was used to represent that all of the
                // records should be fetched.
                getHowManyRows = 500;
            }
            else
            {
                getHowManyRows = howManyRows;
            }
            btnGetprice.Visible = false; ///-------Nimbus -- 13.08.2013
        }
        //End:Nim

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                grdView.FocusedColumn = colItemName;

                //
                // Get all text through the Translation function in the ApplicationLocalizer
                //
                // TextID's for frmItemSearch are reserved at 61500 - 61599
                // In use now are ID's 61500 - 61513

                colItemId.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(61507); //Item number
                colItemName.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(61506); //Item name
                colItemPrice.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(61513); //Item price
                btnGetprice.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61511); //Show price
                btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61514); //OK
                btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51114); //Cancel
                //title
                this.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61612); //Item search
                lblHeading.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(61612); //Item search
                sortBy = colItemName.FieldName;

                colItemUnitOfMeasure.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(86168); //Unit of measure
                colItemRoundTrunc.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(86169); //Round or trunc
                colItemOwnThirdProduc.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(86170); //Own or third
                colItemTaxRate.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(86171); //Trib Situation

                this.clockTimer.InvokeOnTick(
                    ApplicationSettings.Terminal.AutoLogOffTimeOutInMin * 30, /*seconds per half-minute (half-time for popups)*/
                    this.Close);

                itemTable = GetItemList(0, string.Empty, getHowManyRows);

                grItems.DataSource = itemTable;
                CheckRowPosition();
                txtKeyboardInput.Select();

                if (Functions.CountryRegion == SupportedCountryRegion.BR)
                {
                    colItemPrice.Visible = colItemOwnThirdProduc.Visible = colItemRoundTrunc.Visible = colItemUnitOfMeasure.Visible = colItemTaxRate.Visible = colItemPrice.Visible = true;
                    btnGetprice.Visible = false;
                }
            }

            base.OnLoad(e);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmItemSearch));
            this.xtraGridBlending1 = new DevExpress.XtraGrid.Blending.XtraGridBlending();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnGetprice = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemPrice = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemUnitOfMeasure = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemTaxRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemOwnThirdProduc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colItemRoundTrunc = new DevExpress.XtraGrid.Columns.GridColumn();
            this.clockTimer = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraGridBlending1
            // 
            this.xtraGridBlending1.AlphaStyles.AddReplace("Row", 220);
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.panelControl1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1024, 768);
            this.basePanel.TabIndex = 0;
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.tableLayoutPanel2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(2, 2);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1020, 764);
            this.panelControl1.TabIndex = 4;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel6, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1016, 760);
            this.tableLayoutPanel2.TabIndex = 22;
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(409, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(198, 95);
            this.lblHeading.TabIndex = 24;
            this.lblHeading.Tag = "";
            this.lblHeading.Text = "Heading";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 9;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.btnCancel, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnPgDown, 8, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnGetprice, 5, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnDown, 7, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnSelect, 3, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(26, 684);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(964, 65);
            this.tableLayoutPanel3.TabIndex = 23;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(417, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(130, 57);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Cancel";
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(895, 4);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(65, 57);
            this.btnPgDown.TabIndex = 6;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 4);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(65, 57);
            this.btnPgUp.TabIndex = 0;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // btnGetprice
            // 
            this.btnGetprice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetprice.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGetprice.Appearance.Options.UseFont = true;
            this.btnGetprice.Location = new System.Drawing.Point(555, 4);
            this.btnGetprice.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetprice.Name = "btnGetprice";
            this.btnGetprice.Size = new System.Drawing.Size(127, 57);
            this.btnGetprice.TabIndex = 4;
            this.btnGetprice.Tag = "";
            this.btnGetprice.Text = "Show price";
            this.btnGetprice.Click += new System.EventHandler(this.btnGetprice_Click_1);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(77, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(65, 57);
            this.btnUp.TabIndex = 1;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click_2);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(822, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(65, 57);
            this.btnDown.TabIndex = 5;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click_2);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(282, 4);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Tag = "";
            this.btnSelect.Text = "OK";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click_1);
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 8;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.grItems, 0, 1);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(30, 139);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(956, 530);
            this.tableLayoutPanel6.TabIndex = 21;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel6.SetColumnSpan(this.tableLayoutPanel1, 8);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtKeyboardInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(956, 45);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // txtKeyboardInput
            // 
            this.txtKeyboardInput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtKeyboardInput.Location = new System.Drawing.Point(3, 6);
            this.txtKeyboardInput.Name = "txtKeyboardInput";
            this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
            this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtKeyboardInput.Size = new System.Drawing.Size(824, 32);
            this.txtKeyboardInput.TabIndex = 0;
            this.txtKeyboardInput.TextChanged += new System.EventHandler(this.txtKeyboardInput_TextChanged);
            this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemSearch_KeyDown);
            this.txtKeyboardInput.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyboardInput_PreviewKeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(833, 6);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 32);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(896, 6);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click_1);
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel6.SetColumnSpan(this.grItems, 8);
            this.grItems.Cursor = System.Windows.Forms.Cursors.Default;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(0, 45);
            this.grItems.MainView = this.grdView;
            this.grItems.Margin = new System.Windows.Forms.Padding(0);
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(956, 485);
            this.grItems.TabIndex = 20;
            this.grItems.TabStop = false;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            this.grItems.Click += new System.EventHandler(this.grItems_Click);
            this.grItems.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            this.grItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colItemName,
            this.colItemId,
            this.colItemPrice,
            this.colItemUnitOfMeasure,
            this.colItemTaxRate,
            this.colItemOwnThirdProduc,
            this.colItemRoundTrunc});
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
            this.grdView.Click += new System.EventHandler(this.grdView_Click);
            // 
            // colItemName
            // 
            this.colItemName.Caption = "Item Name";
            this.colItemName.FieldName = "ITEMNAME";
            this.colItemName.Name = "colItemName";
            this.colItemName.OptionsColumn.AllowEdit = false;
            this.colItemName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemName.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colItemName.Visible = true;
            this.colItemName.VisibleIndex = 1;
            this.colItemName.Width = 291;
            // 
            // colItemId
            // 
            this.colItemId.Caption = "Item number";
            this.colItemId.FieldName = "ITEMID";
            this.colItemId.Name = "colItemId";
            this.colItemId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colItemId.Visible = true;
            this.colItemId.VisibleIndex = 0;
            this.colItemId.Width = 161;
            // 
            // colItemPrice
            // 
            this.colItemPrice.AppearanceCell.Options.UseTextOptions = true;
            this.colItemPrice.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colItemPrice.AppearanceHeader.Options.UseTextOptions = true;
            this.colItemPrice.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colItemPrice.Caption = "Price";
            this.colItemPrice.DisplayFormat.FormatString = "n:0";
            this.colItemPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colItemPrice.FieldName = "ITEMPRICE";
            this.colItemPrice.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colItemPrice.Name = "colItemPrice";
            this.colItemPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemPrice.Width = 130;
            // 
            // colItemUnitOfMeasure
            // 
            this.colItemUnitOfMeasure.Caption = "Unit of Measure";
            this.colItemUnitOfMeasure.FieldName = "UNITOFMEASURE";
            this.colItemUnitOfMeasure.Name = "colItemUnitOfMeasure";
            this.colItemUnitOfMeasure.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemUnitOfMeasure.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colItemUnitOfMeasure.Width = 57;
            // 
            // colItemTaxRate
            // 
            this.colItemTaxRate.Caption = "TAX";
            this.colItemTaxRate.FieldName = "TAX";
            this.colItemTaxRate.Name = "colItemTaxRate";
            this.colItemTaxRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemTaxRate.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colItemTaxRate.Width = 50;
            // 
            // colItemOwnThirdProduc
            // 
            this.colItemOwnThirdProduc.Caption = "IPPT";
            this.colItemOwnThirdProduc.FieldName = "OWNTHIRDPROD";
            this.colItemOwnThirdProduc.Name = "colItemOwnThirdProduc";
            this.colItemOwnThirdProduc.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
            this.colItemOwnThirdProduc.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colItemOwnThirdProduc.Width = 57;
            // 
            // colItemRoundTrunc
            // 
            this.colItemRoundTrunc.Caption = "IAT";
            this.colItemRoundTrunc.FieldName = "ROUNDTRUNC";
            this.colItemRoundTrunc.Name = "colItemRoundTrunc";
            this.colItemRoundTrunc.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemRoundTrunc.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colItemRoundTrunc.Width = 50;
            // 
            // clockTimer
            // 
            this.clockTimer.Interval = 1000;
            // 
            // frmItemSearch
            // 
            this.Appearance.BackColor = System.Drawing.Color.White;
            this.Appearance.Options.UseBackColor = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmItemSearch";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "Item search";
            this.Shown += new System.EventHandler(this.frmItemSearch_Shown);
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public void txtKeyboardInput_TextChanged(object sender, EventArgs e)
        {
            inputHasChanged = true;
        }

        private void ItemSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    EnterPress();
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

        /// <summary>
        /// Call this function when enter button has been pressed
        /// </summary>
        private void EnterPress()
        {
            if (!inputHasChanged)
            {
                keyboard1_EnterButtonPressedWithoutInputChange();
            }
            else
            {
                keyboard1_EnterButtonPressed();
            }

            inputHasChanged = false;
        }

        public String SelectedItemId
        {
            get { return selectedItemId; }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MoveNextPage();
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                using (DataTable newItemTable = GetItemList(loadedCount, txtKeyboardInput.Text, getHowManyRows))
                {
                    itemTable.Merge(newItemTable);
                }
                grdView.TopRowIndex = topRowIndex;
            }
            FillInPrices();
            CheckRowPosition();
            txtKeyboardInput.Select();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MovePrevPage();
            FillInPrices();
            CheckRowPosition();
            txtKeyboardInput.Select();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            SelectItem();
        }

        private void keyboard1_EnterButtonPressed()
        {
            clockTimer.Restart(); //reset the timer.

            if (txtKeyboardInput.Text.Trim().Length == 0)
            {
                btnSelect_Click(null, null);
            }
            else
            {
                DataTable newItemTable = GetItemList(0, txtKeyboardInput.Text, getHowManyRows);

                if (itemTable != null)
                {
                    itemTable.Dispose();
                }

                grItems.DataSource = itemTable = newItemTable;

                FillInPrices();
            }

            CheckRowPosition();
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.

            Point p = grdView.GridControl.PointToClient(MousePosition);
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest != DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.Column)
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
                selectedItemId = (string)Row[colItemId.FieldName];
                selectedItemName = Convert.ToString(Row[colItemName.FieldName]);//added on 011018 by NIm
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
            clockTimer.Restart(); //reset the timer.
            txtKeyboardInput.Text = string.Empty;

            DataTable newItemTable = GetItemList(0, string.Empty, getHowManyRows);

            if (itemTable != null)
            {
                itemTable.Dispose();
            }

            grItems.DataSource = itemTable = newItemTable;
            FillInPrices();

            txtKeyboardInput.Select();
        }

        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            txtKeyboardInput.Select();
        }

        private void btnUp_Click_1(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MovePrev();
            FillInPrices();
            CheckRowPosition();
            txtKeyboardInput.Select();
        }

        private void btnDown_Click_1(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            grdView.MoveNext();
            int topRowIndex = grdView.TopRowIndex;
            if ((grdView.IsLastRow) && (grdView.RowCount > 0))
            {
                using (DataTable newItemTable = GetItemList(loadedCount, txtKeyboardInput.Text, getHowManyRows))
                {
                    itemTable.Merge(newItemTable);
                }
                grdView.TopRowIndex = topRowIndex;
            }

            FillInPrices();
            CheckRowPosition();
            txtKeyboardInput.Select();
        }

        private void FillInPrices()
        {
            if (colItemPrice.Visible)
            {
                try
                {
                    int mappedRow;

                    decimal price = 0;
                    int visibleItems = (grdView.ViewRect.Height / grdView.RowHeight);

                    Debug.Assert(itemTable != null, "itemTable != null");

                    itemTable.Columns[colItemPrice.FieldName].ReadOnly = false;
                    itemTable.Columns[colItemPrice.FieldName].MaxLength = 50;

                    // Find the prices for the rows that currently show on the screen
                    for (int i = grdView.TopRowIndex; i <= grdView.TopRowIndex + visibleItems && i < itemTable.Rows.Count; i++)
                    {
                        mappedRow = grdView.GetDataSourceRowIndex(i);

                        if (itemTable.Rows[mappedRow][colItemPrice.FieldName].ToString().Length == 0)
                        {
                            price = Dialog.InternalApplication.Services.Price.GetItemPrice(itemTable.Rows[mappedRow][colItemId.FieldName].ToString(), itemTable.Rows[mappedRow]["UNITOFMEASURE"].ToString());
                            itemTable.Rows[mappedRow][colItemPrice.FieldName] = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(price, true, false);
                        }
                    }

                    grItems.DataSource = itemTable;

                    itemTable.Columns[colItemPrice.FieldName].ReadOnly = true;
                }
                catch (Exception ex)
                {
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("frmItemSearch.FillInPrices", ex);
                    throw;
                }
            }
        }

        private void btnGetPrice_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            try
            {
                colItemPrice.Visible = !colItemPrice.Visible;

                int messageId = 61511; // Show price

                if (colItemPrice.Visible)
                {
                    FillInPrices();
                    messageId = 61512; // Hide price
                }

                btnGetprice.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(messageId);
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException("frmItemSearch.btnGetPrice_Click", ex);
            }
        }

        /// <summary>
        /// Checks if the tax code is one of the pre-defined according to PAF-ECF legislation.
        /// </summary>
        /// <param name="taxCode">The tax code.</param>
        /// <returns>True if it is a pre-defined tax code; false, otherwise.</returns>
        private static bool IsPredefinedTaxCode(string taxCode)
        {
            if (taxCode == null)
            {
                throw new ArgumentNullException("taxCode");
            }

            switch (taxCode)
            {
                case "I1":
                case "I2":
                case "F1":
                case "F2":
                case "N1":
                case "N2":
                case "IS1":
                case "IS2":
                case "FS1":
                case "FS2":
                case "NS1":
                case "NS2":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the tax value formatted accordingly to the Brazilian PAF-ECF requirements.
        /// </summary>
        /// <param name="itemId">The given item ID.</param>
        /// <returns>The formatted tax value to be shown.</returns>
        private static string GetTaxLine(string itemId)
        {
            const string query = @"
                            SELECT TT.TAXCODE, TD.TAXVALUE, TT.TAXTYPE_BR
                            FROM INVENTTABLEMODULE ITM
                            JOIN TAXONITEM AS TOI ON TOI.TAXITEMGROUP = ITM.TAXITEMGROUPID
                                    AND TOI.DATAAREAID = @DATAAREAID
                            JOIN TAXTABLE AS TT ON TT.TAXCODE = TOI.TAXCODE AND (TT.TAXTYPE_BR = @ICMS OR TT.TAXTYPE_BR = @ISS) 
                                    AND TT.DATAAREAID = @DATAAREAID
                            JOIN TAXDATA AS TD ON TD.TAXCODE = TOI.TAXCODE 
                                    AND TD.DATAAREAID = @DATAAREAID
                            WHERE ITM.ITEMID = @ITEMID AND ITM.MODULETYPE = 2 AND ITM.DATAAREAID = @DATAAREAID 
	                            AND TT.TAXCODE IN
                                (
                                    -- TAX CODE MUST BE ALLOWED IN THE STORE
                                    SELECT TGD.TAXCODE 
                                    FROM TAXGROUPDATA AS TGD
                                    JOIN RETAILSTORETABLE AS RST ON RST.STORENUMBER = @STOREID
                                    WHERE TGD.TAXGROUP = RST.TAXGROUP AND TGD.DATAAREAID = @DATAAREAID
                                )";


            var result = LSRetailPosis.ApplicationLocalizer.Language.Translate(10); //Error 

            using (var taxLines = new DataTable())
            {
                try
                {
                    using (var command = new SqlCommand())
                    {
                        command.Parameters.AddWithValue("@DATAAREAID", ApplicationSettings.Database.DATAAREAID);
                        command.Parameters.AddWithValue("@STOREID", ApplicationSettings.Database.StoreID);
                        command.Parameters.AddWithValue("@ITEMID", itemId);
                        command.Parameters.AddWithValue("@ICMS", BrazilianTaxType.Icms);
                        command.Parameters.AddWithValue("@ISS", BrazilianTaxType.Iss);
                        command.CommandText = query;
                        command.Connection = ApplicationSettings.Database.LocalConnection;

                        if (command.Connection.State != ConnectionState.Open)
                        {
                            command.Connection.Open();
                        }

                        var reader = command.ExecuteReader();
                        taxLines.Load(reader);
                    }
                }
                finally
                {
                    if (ApplicationSettings.Database.LocalConnection.State == ConnectionState.Open)
                    {
                        ApplicationSettings.Database.LocalConnection.Close();
                    }
                }

                //The item must have only 1 ICMS or ISS tax
                if (taxLines.Rows.Count == 1)
                {
                    var taxLine = taxLines.Rows[0];

                    var taxCode = taxLine["TAXCODE"].ToString();
                    var taxValue = PosApplication.Instance.BusinessLogic.Utility.ToDecimal(taxLine["TAXVALUE"]);
                    var taxType = (BrazilianTaxType) taxLine["TAXTYPE_BR"];

                    if (IsPredefinedTaxCode(taxCode))
                    {
                        result = taxCode.Substring(0, taxCode.Length - 1);
                    }
                    else if (taxType == BrazilianTaxType.Iss)
                    {
                        result = string.Format("S{0:0000}", taxValue*100);
                    }
                    else if (taxType == BrazilianTaxType.Icms)
                    {
                        result = string.Format("T{0:0000}", taxValue*100);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <remarks>CA2100 The queries are already parametrized and the value of "sortBy" is hard coded. No sql injection threat.</remarks>
        /// <param name="fromRow"></param>
        /// <param name="searchString"></param>
        /// <param name="numberOfItems"></param>
        /// <returns>Datatable (Caller is responsible for disposing returned object)</returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "The queries are already parametrized and the value of sortBy is hard coded. No sql injection threat.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]
        private DataTable GetItemList(int fromRow, string searchString, int numberOfItems)
        {
            // Note: Query uses server side sorting using the SQL Server "over" operator to limit the rowset.

            String asc = sortAsc ? " ASC" : " DESC";
            SqlConnection connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            SqlCommand command = new SqlCommand();
            string search = string.Empty;

            if (searchString.Length > 0)
            {
                //if (sCallName == "O")
                //{
                //    search = " AND (IT.ITEMID = @SEARCHSTRING) ";
                //}
                //else
                //{
                //    search = " AND (ISNULL(TR.NAME, IT.ITEMNAME) LIKE @SEARCHSTRING OR PR.SEARCHNAME LIKE @SEARCHSTRING OR IT.ITEMID LIKE @SEARCHSTRING) ";
                //}
                search = " AND (ISNULL(TR.NAME, IT.ITEMNAME) LIKE @SEARCHSTRING OR PR.SEARCHNAME LIKE @SEARCHSTRING OR IT.ITEMID LIKE @SEARCHSTRING) ";
            }

            string query = string.Format("SELECT ITEMID, ITEMNAME, '' AS ITEMPRICE, I.UNITOFMEASURE, I.INVENTPRODUCTTYPE_BR   " +
                           "FROM ( " +
                           "    SELECT IT.INVENTPRODUCTTYPE_BR, IT.ITEMID, COALESCE(TR.NAME, IT.ITEMNAME, IT.ITEMID) AS ITEMNAME, IT.DATAAREAID, ISNULL(IM.UNITID, '') AS UNITOFMEASURE, ROW_NUMBER() " +
                           "    OVER (ORDER BY IT.{0} {1}) AS ROW " +
                           "    FROM ASSORTEDINVENTITEMS IT " +
                           "    JOIN INVENTTABLEMODULE IM ON IT.ITEMID = IM.ITEMID AND IM.MODULETYPE = 2 " +
                           "    JOIN ECORESPRODUCT AS PR ON PR.RECID = IT.PRODUCT " +
                           "    LEFT JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT AND TR.LANGUAGEID = @CULTUREID " + 
                           "    WHERE IT.STORERECID = @STORERECID {2}) I " +
                           "WHERE I.DATAAREAID=@DATAAREAID AND I.ROW > @FROMROW AND I.ROW <= @TOROW ", sortBy, asc, search);

            if (!string.IsNullOrEmpty(sCondition))
            {
                query += sCondition;
            }

            SqlParameter languageIdParm = command.Parameters.Add("@CULTUREID", SqlDbType.NVarChar, 7);
            languageIdParm.Value = ApplicationSettings.Terminal.CultureName;
            SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
            dataAreaIdParm.Value = ApplicationSettings.Database.DATAAREAID;
            command.Parameters.AddWithValue("@FROMROW", fromRow);
            command.Parameters.AddWithValue("@TOROW", (fromRow + numberOfItems));
            command.Parameters.AddWithValue("@STORERECID", ApplicationSettings.Terminal.StorePrimaryId);

            if (searchString.Length > 0)
            {
                SqlParameter searchStringParm = command.Parameters.Add("@SEARCHSTRING", SqlDbType.NVarChar, 100);
                searchStringParm.Value = '%' + searchString + '%';

                //if (sCallName == "O")
                //{
                //    searchStringParm.Value = searchString;
                //}
                //else
                //{
                //    searchStringParm.Value = '%' + searchString + '%';
                //}
            }

            foreach (DevExpress.XtraGrid.Columns.GridColumn col in grdView.Columns)
            {
                col.SortOrder = DevExpress.Data.ColumnSortOrder.None;
            }

            grdView.Columns[sortBy].SortOrder = sortAsc ? DevExpress.Data.ColumnSortOrder.Ascending : DevExpress.Data.ColumnSortOrder.Descending;

            DataTable itemsTable = new DataTable();

            try
            {
                loadedCount = fromRow + numberOfItems;

                command.CommandText = query;
                command.Connection = connection;
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                SqlDataReader reader = command.ExecuteReader();
                itemsTable.Load(reader);

                // Brazil localization
                if (Functions.CountryRegion == SupportedCountryRegion.BR)
                {
                    //Brazilian law requires these fields
                    var tax = new DataColumn("TAX");

                    var roundTrunc = new DataColumn("ROUNDTRUNC");
                    roundTrunc.DefaultValue = "A"; //Fixed

                    var ownThirdProduc = new DataColumn("OWNTHIRDPROD");
                    ownThirdProduc.DefaultValue = "P";

                    itemsTable.Columns.Add(ownThirdProduc);
                    itemsTable.Columns.Add(roundTrunc);
                    itemsTable.Columns.Add(tax);

                    foreach (DataRow row in itemsTable.Rows)
                    {
                        string productTypeId = row["INVENTPRODUCTTYPE_BR"].ToString();
                        if (productTypeId == "00" ||
                                productTypeId == "07" ||
                                productTypeId == "08" ||
                                productTypeId == "10" ||
                                productTypeId == "99")
                            row["OWNTHIRDPROD"] = "T";

                        row["TAX"] = GetTaxLine(row["ITEMID"].ToString());
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return itemsTable;
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

        private void grdView_EndSorting(object sender, EventArgs e)
        {
            if (colItemPrice.Visible)
            {
                FillInPrices();
            }
            else
            {
                grItems.DataSource = itemTable;
            }
        }

        private void keyboard1_EnterButtonPressedWithoutInputChange()
        {
            SelectItem();
        }

        private void grItems_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
        }

        private void grdView_TopRowChanged(object sender, EventArgs e)
        {
            FillInPrices();
        }

        private void grItems_KeyDown(object sender, KeyEventArgs e)
        {
            clockTimer.Restart(); //reset the timer.

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnSelect_Click(null, null);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.

            inputHasChanged = true;
            if (!string.IsNullOrEmpty(txtKeyboardInput.Text))
            {
                EnterPress();
            }
            else
            {
                btnClear_Click(sender, e);
            }

            CheckRowPosition();
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnClear_Click(sender, e);
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnUp_Click(sender, e);
        }

        private void btnUp_Click_2(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnUp_Click_1(sender, e);
        }

        private void btnGetprice_Click_1(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnGetPrice_Click(sender, e);
        }

        private void btnSelect_Click_1(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnSelect_Click(sender, e);
        }

        private void btnDown_Click_2(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnDown_Click_1(sender, e);
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            btnDown_Click(sender, e);
        }

        private void CheckRowPosition()
        {
            btnPgDown.Enabled = !grdView.IsLastRow;
            btnDown.Enabled = btnPgDown.Enabled;
            btnPgUp.Enabled = !grdView.IsFirstRow;
            btnUp.Enabled = btnPgUp.Enabled;
        }

        private void grdView_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.

            Point p = grdView.GridControl.PointToClient(MousePosition);
            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest == DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitTest.Column)
            {
                if (info.Column.FieldName != colItemPrice.FieldName)
                {
                    if (Functions.CountryRegion == SupportedCountryRegion.BR
                        && (info.Column.FieldName == colItemOwnThirdProduc.FieldName
                        || info.Column.FieldName == colItemRoundTrunc.FieldName
                        || info.Column.FieldName == colItemUnitOfMeasure.FieldName
                        || info.Column.FieldName == colItemTaxRate.FieldName))
                        return;

                    if (sortBy == info.Column.FieldName)
                    {
                        sortAsc = !sortAsc;
                    }
                    else
                    {
                        sortAsc = true;
                    }

                    sortBy = info.Column.FieldName;

                    loadedCount = 0;

                    DataTable newItemTable = GetItemList(0, txtKeyboardInput.Text, getHowManyRows);

                    if (itemTable != null)
                    {
                        itemTable.Dispose();
                    }

                    grItems.DataSource = itemTable = newItemTable;

                    FillInPrices();
                }
            }

            CheckRowPosition();
            txtKeyboardInput.Select();
        }

        private void txtKeyboardInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void frmItemSearch_Shown(object sender, EventArgs e)
        {
            if (Functions.CountryRegion == SupportedCountryRegion.BR)
            {
                FillInPrices();
            }
        }
    }
}
