/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmPriceCheck.
    /// </summary>
	public class frmPriceCheck : frmTouchBase
    {
        private PanelControl panelControl1;
        private Label lblPrice;
        private Label lblItemId;
        private Label lblCustomerId;
        private Label lblCustomerName;

        private decimal price;
        private string itemId;
        private string itemDescription;
        private string customerName;
        private string customerId;
        private string barcode;
        private DevExpress.XtraLayout.LayoutControl layPriceInfo;
        private DevExpress.XtraLayout.LayoutControl layoutControl1;
        private DevExpress.XtraLayout.LayoutControlItem layItemId;
        private DevExpress.XtraLayout.LayoutControlGroup layItem;
        private DevExpress.XtraLayout.LayoutControlGroup layCustomerInfo;
        private Label lblItemDescription;
        private DevExpress.XtraLayout.LayoutControlItem layItemDescription;
        private DevExpress.XtraLayout.LayoutControlItem layPrice;
        private DevExpress.XtraLayout.LayoutControlItem layCustomerId;
        private DevExpress.XtraLayout.LayoutControlItem layCustomerName;
        private DevExpress.XtraLayout.LayoutControlGroup layoutCGItemInfo;
        private bool useScanner; // = false;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numPad1;
        private DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
        private Timer clockTimer;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel1;
        private SimpleButtonEx btnSearch;
        private SimpleButtonEx btnSellItem;
        private SimpleButtonEx btnClose;
        private Label labelHeading;
        private PanelControl panelControl2;
        private System.ComponentModel.IContainer components;
        protected PosTransaction posTransaction;

        /// <summary>
        /// Get/set property for price.
        /// </summary>
        public decimal Price
        {
            set { price = value; }
            get { return price; }
        }
        /// <summary>
        /// Get/set property for ItemId.
        /// </summary>
        public string ItemId
        {
            set { itemId = value; }
            get { return itemId; }
        }
        /// <summary>
        /// Returns item description as string.
        /// </summary>
        public string ItemDescription
        {
            set { itemDescription = value; }
        }
        /// <summary>
        /// Returns customer name as string.
        /// </summary>
        public string CustomerName
        {
            set { customerName = value; }
        }
        /// <summary>
        /// Returns customer id as string.
        /// </summary>
        public string CustomerId
        {
            set { customerId = value; }
        }
        /// <summary>
        /// Get/set barcode proeprty.
        /// </summary>
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; }
        }
        /// <summary>
        /// Sets the form for price check.
        /// </summary>
        public frmPriceCheck()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }
        /// <summary>
        /// Checks for price check.
        /// </summary>
        /// <param name="posTransaction"></param>
        public frmPriceCheck(IPosTransaction posTransaction)
            : this()
        {
            this.posTransaction = (PosTransaction)posTransaction;
        }

        protected override void OnLoad(EventArgs e)
        {

            if (!this.DesignMode)
            {
                //
                // Get all text through the Translation function in the ApplicationLocalizer
                //
                // TextID's for frmPriceCheck are reserved at 1480 - 1499
                // In use now are ID's 1480 - 1489
                //
                btnClose.Text = ApplicationLocalizer.Language.Translate(1480); //Close
                btnSellItem.Text = ApplicationLocalizer.Language.Translate(1481); //Add item 
                layCustomerId.Text = ApplicationLocalizer.Language.Translate(1482); //Customer id
                layCustomerInfo.Text = ApplicationLocalizer.Language.Translate(1483); //Customer
                layCustomerName.Text = ApplicationLocalizer.Language.Translate(1484); //Customer name
                layItem.Text = ApplicationLocalizer.Language.Translate(1485); //Item
                layItemDescription.Text = ApplicationLocalizer.Language.Translate(1486); //Item description
                layItemId.Text = ApplicationLocalizer.Language.Translate(1487); //Item id
                layPrice.Text = ApplicationLocalizer.Language.Translate(1488); //Price  
                //layPriceInfo.Root.CustomizationFormText = ApplicationLocalizer.Language.Translate(1489); //Price Info
                btnSearch.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1262); //Search 
                numPad1.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(2367); //barcode

                this.loadItemInfo();

                this.clockTimer.InvokeOnTick(
                    ApplicationSettings.Terminal.AutoLogOffTimeOutInMin * 30, /*seconds per half-minute (half-time for popups)*/
                    this.Close);
            }

            base.OnLoad(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (!this.DesignMode)
            {
                Dialog.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(this.ProcessScannedItem);
                Dialog.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
            }

            base.OnClosed(e);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.layPriceInfo = new DevExpress.XtraLayout.LayoutControl();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.lblItemDescription = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblCustomerId = new System.Windows.Forms.Label();
            this.layoutCGItemInfo = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layItem = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layItemId = new DevExpress.XtraLayout.LayoutControlItem();
            this.layItemDescription = new DevExpress.XtraLayout.LayoutControlItem();
            this.layPrice = new DevExpress.XtraLayout.LayoutControlItem();
            this.layCustomerInfo = new DevExpress.XtraLayout.LayoutControlGroup();
            this.layCustomerId = new DevExpress.XtraLayout.LayoutControlItem();
            this.layCustomerName = new DevExpress.XtraLayout.LayoutControlItem();
            this.numPad1 = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
            this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
            this.clockTimer = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSellItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelHeading = new System.Windows.Forms.Label();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layPriceInfo)).BeginInit();
            this.layPriceInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layoutCGItemInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layItem)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layItemId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layItemDescription)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layCustomerInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layCustomerId)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layCustomerName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.panelControl2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.layPriceInfo);
            this.panelControl1.Controls.Add(this.numPad1);
            this.panelControl1.Location = new System.Drawing.Point(100, 182);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(828, 340);
            this.panelControl1.TabIndex = 0;
            // 
            // layPriceInfo
            // 
            this.layPriceInfo.AllowCustomizationMenu = false;
            this.layPriceInfo.Controls.Add(this.lblCustomerName);
            this.layPriceInfo.Controls.Add(this.lblItemDescription);
            this.layPriceInfo.Controls.Add(this.lblItemId);
            this.layPriceInfo.Controls.Add(this.lblPrice);
            this.layPriceInfo.Controls.Add(this.lblCustomerId);
            this.layPriceInfo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.layPriceInfo.Location = new System.Drawing.Point(3, 3);
            this.layPriceInfo.Name = "layPriceInfo";
            this.layPriceInfo.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layPriceInfo.Root = this.layoutCGItemInfo;
            this.layPriceInfo.Size = new System.Drawing.Size(558, 329);
            this.layPriceInfo.TabIndex = 0;
            this.layPriceInfo.TabStop = false;
            this.layPriceInfo.Text = "layoutControl2";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCustomerName.Location = new System.Drawing.Point(125, 264);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(409, 41);
            this.lblCustomerName.TabIndex = 4;
            this.lblCustomerName.Text = "lblCustomerName";
            this.lblCustomerName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblItemDescription
            // 
            this.lblItemDescription.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblItemDescription.Location = new System.Drawing.Point(125, 131);
            this.lblItemDescription.Name = "lblItemDescription";
            this.lblItemDescription.Size = new System.Drawing.Size(409, 37);
            this.lblItemDescription.TabIndex = 2;
            this.lblItemDescription.Text = "lblItemDescription";
            this.lblItemDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblItemId
            // 
            this.lblItemId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblItemId.Location = new System.Drawing.Point(125, 91);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(409, 36);
            this.lblItemId.TabIndex = 1;
            this.lblItemId.Text = "lblItemId";
            this.lblItemId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPrice
            // 
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblPrice.Location = new System.Drawing.Point(125, 51);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(409, 36);
            this.lblPrice.TabIndex = 0;
            this.lblPrice.Text = "lblPrice";
            this.lblPrice.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCustomerId
            // 
            this.lblCustomerId.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblCustomerId.Location = new System.Drawing.Point(125, 223);
            this.lblCustomerId.Name = "lblCustomerId";
            this.lblCustomerId.Size = new System.Drawing.Size(409, 37);
            this.lblCustomerId.TabIndex = 3;
            this.lblCustomerId.Text = "lblCustomerId";
            this.lblCustomerId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // layoutCGItemInfo
            // 
            this.layoutCGItemInfo.AppearanceGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.layoutCGItemInfo.AppearanceGroup.Options.UseFont = true;
            this.layoutCGItemInfo.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.layoutCGItemInfo.AppearanceItemCaption.Options.UseFont = true;
            this.layoutCGItemInfo.CustomizationFormText = "Price Info";
            this.layoutCGItemInfo.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layItem,
            this.layCustomerInfo});
            this.layoutCGItemInfo.Location = new System.Drawing.Point(0, 0);
            this.layoutCGItemInfo.Name = "layoutCGItemInfo";
            this.layoutCGItemInfo.Size = new System.Drawing.Size(558, 329);
            this.layoutCGItemInfo.Text = "Price Info";
            this.layoutCGItemInfo.TextVisible = false;
            // 
            // layItem
            // 
            this.layItem.AppearanceGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.layItem.AppearanceGroup.Options.UseFont = true;
            this.layItem.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.layItem.AppearanceItemCaption.Options.UseFont = true;
            this.layItem.CustomizationFormText = "Item";
            this.layItem.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layItemId,
            this.layItemDescription,
            this.layPrice});
            this.layItem.Location = new System.Drawing.Point(0, 0);
            this.layItem.Name = "layItem";
            this.layItem.Size = new System.Drawing.Size(538, 172);
            this.layItem.Text = "Item";
            // 
            // layItemId
            // 
            this.layItemId.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layItemId.AppearanceItemCaption.Options.UseFont = true;
            this.layItemId.Control = this.lblItemId;
            this.layItemId.CustomizationFormText = "Item Id";
            this.layItemId.Location = new System.Drawing.Point(0, 40);
            this.layItemId.Name = "layItemId";
            this.layItemId.Size = new System.Drawing.Size(514, 40);
            this.layItemId.Text = "Item ID:";
            this.layItemId.TextSize = new System.Drawing.Size(97, 17);
            // 
            // layItemDescription
            // 
            this.layItemDescription.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layItemDescription.AppearanceItemCaption.Options.UseFont = true;
            this.layItemDescription.Control = this.lblItemDescription;
            this.layItemDescription.CustomizationFormText = "Item Description";
            this.layItemDescription.Location = new System.Drawing.Point(0, 80);
            this.layItemDescription.Name = "layItemDescription";
            this.layItemDescription.Size = new System.Drawing.Size(514, 41);
            this.layItemDescription.Text = "Item description:";
            this.layItemDescription.TextSize = new System.Drawing.Size(97, 17);
            // 
            // layPrice
            // 
            this.layPrice.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layPrice.AppearanceItemCaption.Options.UseFont = true;
            this.layPrice.Control = this.lblPrice;
            this.layPrice.CustomizationFormText = "Price";
            this.layPrice.Location = new System.Drawing.Point(0, 0);
            this.layPrice.Name = "layPrice";
            this.layPrice.Size = new System.Drawing.Size(514, 40);
            this.layPrice.Text = "Price:";
            this.layPrice.TextSize = new System.Drawing.Size(97, 17);
            // 
            // layCustomerInfo
            // 
            this.layCustomerInfo.AppearanceGroup.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.layCustomerInfo.AppearanceGroup.Options.UseFont = true;
            this.layCustomerInfo.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.layCustomerInfo.AppearanceItemCaption.Options.UseFont = true;
            this.layCustomerInfo.CustomizationFormText = "Customer";
            this.layCustomerInfo.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
            this.layCustomerId,
            this.layCustomerName});
            this.layCustomerInfo.Location = new System.Drawing.Point(0, 172);
            this.layCustomerInfo.Name = "layCustomerInfo";
            this.layCustomerInfo.Size = new System.Drawing.Size(538, 137);
            this.layCustomerInfo.Text = "Customer";
            // 
            // layCustomerId
            // 
            this.layCustomerId.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layCustomerId.AppearanceItemCaption.Options.UseFont = true;
            this.layCustomerId.Control = this.lblCustomerId;
            this.layCustomerId.CustomizationFormText = "Customer Id";
            this.layCustomerId.Location = new System.Drawing.Point(0, 0);
            this.layCustomerId.Name = "layCustomerId";
            this.layCustomerId.Size = new System.Drawing.Size(514, 41);
            this.layCustomerId.Text = "Customer ID:";
            this.layCustomerId.TextSize = new System.Drawing.Size(97, 17);
            // 
            // layCustomerName
            // 
            this.layCustomerName.AppearanceItemCaption.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.layCustomerName.AppearanceItemCaption.Options.UseFont = true;
            this.layCustomerName.Control = this.lblCustomerName;
            this.layCustomerName.CustomizationFormText = "Customer Name";
            this.layCustomerName.Location = new System.Drawing.Point(0, 41);
            this.layCustomerName.Name = "layCustomerName";
            this.layCustomerName.Size = new System.Drawing.Size(514, 45);
            this.layCustomerName.Text = "Customer name:";
            this.layCustomerName.TextSize = new System.Drawing.Size(97, 17);
            // 
            // numPad1
            // 
            this.numPad1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numPad1.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPad1.Appearance.Options.UseFont = true;
            this.numPad1.AutoSize = true;
            this.numPad1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.numPad1.CurrencyCode = null;
            this.numPad1.EnteredQuantity = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numPad1.EnteredValue = "";
            this.numPad1.Location = new System.Drawing.Point(577, 3);
            this.numPad1.MaskChar = "";
            this.numPad1.MaskInterval = 0;
            this.numPad1.MaxNumberOfDigits = 20;
            this.numPad1.MinimumSize = new System.Drawing.Size(248, 314);
            this.numPad1.Name = "numPad1";
            this.numPad1.NegativeMode = false;
            this.numPad1.NoOfTries = 0;
            this.numPad1.PromptText = null;
            this.numPad1.ShortcutKeysActive = false;
            this.numPad1.Size = new System.Drawing.Size(248, 314);
            this.numPad1.TabIndex = 1;
            this.numPad1.TimerEnabled = true;
            this.numPad1.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numPad1_EnterButtonPressed);
            // 
            // layoutControl1
            // 
            this.layoutControl1.Location = new System.Drawing.Point(0, 0);
            this.layoutControl1.Name = "layoutControl1";
            this.layoutControl1.OptionsView.UseDefaultDragAndDropRendering = false;
            this.layoutControl1.Root = this.layoutControlGroup1;
            this.layoutControl1.Size = new System.Drawing.Size(180, 120);
            this.layoutControl1.TabIndex = 0;
            // 
            // layoutControlGroup1
            // 
            this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
            this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
            this.layoutControlGroup1.Name = "layoutControlGroup1";
            this.layoutControlGroup1.Size = new System.Drawing.Size(180, 120);
            this.layoutControlGroup1.Text = "layoutControlGroup1";
            // 
            // clockTimer
            // 
            this.clockTimer.Interval = 1000;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.panelControl1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.labelHeading, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1028, 651);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSellItem, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(296, 572);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(435, 65);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Location = new System.Drawing.Point(19, 4);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(127, 57);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Tag = "BtnExtraLong";
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnSellItem
            // 
            this.btnSellItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSellItem.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSellItem.Appearance.Options.UseFont = true;
            this.btnSellItem.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSellItem.Location = new System.Drawing.Point(154, 4);
            this.btnSellItem.Margin = new System.Windows.Forms.Padding(4);
            this.btnSellItem.Name = "btnSellItem";
            this.btnSellItem.Size = new System.Drawing.Size(127, 57);
            this.btnSellItem.TabIndex = 1;
            this.btnSellItem.Tag = "BtnExtraLong";
            this.btnSellItem.Text = "Sell Item";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(289, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(127, 57);
            this.btnClose.TabIndex = 2;
            this.btnClose.Tag = "BtnExtraLong";
            this.btnClose.Text = "Close";
            // 
            // labelHeading
            // 
            this.labelHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelHeading.AutoSize = true;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.labelHeading.Location = new System.Drawing.Point(388, 40);
            this.labelHeading.Margin = new System.Windows.Forms.Padding(0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.labelHeading.Size = new System.Drawing.Size(251, 95);
            this.labelHeading.TabIndex = 14;
            this.labelHeading.Tag = "";
            this.labelHeading.Text = "Price check";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.tableLayoutPanel2);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(1032, 655);
            this.panelControl2.TabIndex = 2;
            // 
            // frmPriceCheck
            // 
            this.ClientSize = new System.Drawing.Size(1032, 655);
            this.Controls.Add(this.panelControl2);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmPriceCheck";
            this.Text = "Price check";
            this.Controls.SetChildIndex(this.panelControl2, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.layPriceInfo)).EndInit();
            this.layPriceInfo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.layoutCGItemInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layItem)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layItemId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layItemDescription)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layCustomerInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layCustomerId)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layCustomerName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.panelControl2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion
        /// <summary>
        /// Get/set usescanner proeprty.
        /// </summary>
        public bool UseScanner
        {
            get
            {
                return useScanner;
            }
            set
            {
                useScanner = value;
                if (useScanner)
                {
                    Dialog.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent += new ScannerMessageEventHandler(this.ProcessScannedItem);
                    Dialog.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();
                }
            }
        }


        /// <summary>
        /// Occures when opos scanner is used.
        /// </summary>
        /// <param name="scannedMessage"></param>
        private void ProcessScannedItem(IScanInfo scanInfo)
        {
            clearItemInfo();
            if (useScanner)
            {
                Dialog.InternalApplication.Services.Peripherals.Scanner.DisableForScan();
            }

            numPad1.EnteredValue = scanInfo.ScanDataLabel;
            checkItemPrice(numPad1.EnteredValue);
            if (useScanner)
            {
                Dialog.InternalApplication.Services.Peripherals.Scanner.ReEnableForScan();
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        private void checkItemPrice(string barcode)
        {
            System.Diagnostics.Debug.Assert(barcode != null, "barcode should not be null.");

            if (string.IsNullOrEmpty(barcode))
            {
                return;
            }

            SaleLineItem saleLineItem = (SaleLineItem)Dialog.InternalApplication.BusinessLogic.Utility.CreateSaleLineItem(
                ApplicationSettings.Terminal.StoreCurrency,
                Dialog.InternalApplication.Services.Rounding,
                posTransaction);

            IScanInfo scaninfo = Dialog.InternalApplication.BusinessLogic.Utility.CreateScanInfo();
            scaninfo.ScanDataLabel = barcode;
            IBarcodeInfo barcodeInfo = Dialog.InternalApplication.Services.Barcode.ProcessBarcode(scaninfo);

            if ((barcodeInfo.InternalType == BarcodeInternalType.Item) && (barcodeInfo.ItemId != null))
            {
                // The entry was a barcode which was found and now we have the item id...
                saleLineItem.ItemId = barcodeInfo.ItemId;
                saleLineItem.BarcodeId = barcodeInfo.BarcodeId;
                saleLineItem.SalesOrderUnitOfMeasure = barcodeInfo.UnitId;

                if (barcodeInfo.BarcodeQuantity > 0)
                {
                    saleLineItem.Quantity = barcodeInfo.BarcodeQuantity;
                }
                else
                {
                    saleLineItem.Quantity = 1;
                }

                if (barcodeInfo.BarcodePrice > 0)
                {
                    saleLineItem.Price = barcodeInfo.BarcodePrice;
                }

                saleLineItem.EntryType = barcodeInfo.EntryType;
                //saleLineItem.DimensionGroupId = barcodeInfo.DimensionGroupId;
                saleLineItem.Dimension.ColorId = barcodeInfo.InventColorId;
                saleLineItem.Dimension.SizeId = barcodeInfo.InventSizeId;
                saleLineItem.Dimension.StyleId = barcodeInfo.InventStyleId;
                saleLineItem.Dimension.VariantId = barcodeInfo.VariantId;
            }
            else
            {
                saleLineItem.ItemId = barcodeInfo.BarcodeId;
                saleLineItem.EntryType = barcodeInfo.EntryType;
                saleLineItem.Quantity = 1;
            }

            Dialog.InternalApplication.Services.Item.ProcessItem(saleLineItem, true);

            if (!saleLineItem.Found)
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(3341);
            }
            else
            {
                //Color,Size,Style,Config
                if (saleLineItem.Dimension.EnterDimensions)
                {
                    if (!SetItemDimensions(saleLineItem))
                    {
                        return;
                    }
                }

                // Create a deep copy of the transaction to get the correct price calculation.
                RetailTransaction tempTransaction = (RetailTransaction)this.posTransaction.CloneTransaction();
                tempTransaction.Add(saleLineItem);

                // Get the item info...
                // Get the price, tax and discount info
                Dialog.InternalApplication.BusinessLogic.ItemSystem.CalculatePriceTaxDiscount(tempTransaction);

                saleLineItem = tempTransaction.GetItem(saleLineItem.LineId);
                saleLineItem.CalculateLine();

                // Find the last ItemSale entry..
                LinkedListNode<SaleLineItem> lastItem = tempTransaction.SaleItems.Last;

                ISaleLineItem lineItem = lastItem.Value;
                this.ItemId = lineItem.ItemId;
                this.ItemDescription = lineItem.Description;
                this.Price = lineItem.NetAmount;

                this.CustomerId = tempTransaction.Customer.CustomerId;
                this.CustomerName = tempTransaction.Customer.Name;
                loadItemInfo();
                this.barcode = numPad1.EnteredValue;
            }

            numPad1.ClearValue();
        }

        private bool SetItemDimensions(SaleLineItem saleLineItem)
        {
            LSRetailPosis.POSProcesses.SetDimensions dimensions = new LSRetailPosis.POSProcesses.SetDimensions();
            dimensions.OperationID = PosisOperations.SetDimensions;
            dimensions.POSTransaction = this.posTransaction;
            dimensions.SaleLineItem = saleLineItem;
            dimensions.RunOperation();

            return dimensions.ValidDimensions;
        }

        private void loadItemInfo()
        {
            lblItemId.Text = itemId;
            lblItemDescription.Text = itemDescription;
            lblCustomerId.Text = customerId;
            lblCustomerName.Text = customerName;
            lblPrice.Text = Dialog.InternalApplication.Services.Rounding.Round(price, true); //Currency formatting
        }

        private void clearItemInfo()
        {
            lblItemId.Text = string.Empty;
            lblItemDescription.Text = string.Empty;
            lblCustomerId.Text = string.Empty;
            lblCustomerName.Text = string.Empty;
            lblPrice.Text = Dialog.InternalApplication.Services.Rounding.Round(0, true);
        }

        private void numPad1_EnterButtonPressed()
        {
            clockTimer.Restart(); //reset the timer.
            clearItemInfo();
            checkItemPrice(numPad1.EnteredValue);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            clockTimer.Restart(); //reset the timer.
            string selectedItemId = string.Empty;

            // Show the search dialog through the item service
            if (!Dialog.InternalApplication.Services.Item.ItemSearch(ref selectedItemId, 500))
            {
                return;
            }

            if (string.IsNullOrEmpty(numPad1.EnteredValue))
            {
                numPad1.EnteredValue = selectedItemId;
            }

            checkItemPrice(numPad1.EnteredValue);
        }
    }
}
