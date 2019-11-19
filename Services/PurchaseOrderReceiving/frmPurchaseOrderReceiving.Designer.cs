/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving;
using Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving.Properties;

namespace Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving
{
    partial class frmPurchaseOrderReceiving
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
                {
                    components.Dispose();
                }

                PurchaseOrderReceiving.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(OnBarcodeScan);
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
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.grInventory = new DevExpress.XtraGrid.GridControl();
            this.gvInventory = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDescription = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrdered = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReceived = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReceivedNow = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colUnit = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tablePanelArrowButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutInputs = new System.Windows.Forms.TableLayoutPanel();
            this.txtDeliveryMethod = new DevExpress.XtraEditors.TextEdit();
            this.lblReceiptNumberHeading = new System.Windows.Forms.Label();
            this.txtReceiptNumber = new DevExpress.XtraEditors.TextEdit();
            this.lblDriverHeading = new System.Windows.Forms.Label();
            this.txtDriver = new DevExpress.XtraEditors.TextEdit();
            this.lblPoNumberHeading = new System.Windows.Forms.Label();
            this.txtPoNumber = new DevExpress.XtraEditors.TextEdit();
            this.lblDeliveryHeading = new System.Windows.Forms.Label();
            this.txtDelivery = new DevExpress.XtraEditors.TextEdit();
            this.numPad1 = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.lblDeliveryMethod = new System.Windows.Forms.Label();
            this.tablePanelSideButtons = new System.Windows.Forms.TableLayoutPanel();
            this.btnRefresh = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSave = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUom = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnReceiveAll = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblHeader = new System.Windows.Forms.Label();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.tableLayoutPanelMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grInventory)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInventory)).BeginInit();
            this.tablePanelArrowButtons.SuspendLayout();
            this.tableLayoutInputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeliveryMethod.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReceiptNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDriver.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPoNumber.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDelivery.Properties)).BeginInit();
            this.tablePanelSideButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelMain.ColumnCount = 5;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 56F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.tableLayoutPanelMain.Controls.Add(this.grInventory, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tablePanelArrowButtons, 0, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutInputs, 2, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tablePanelSideButtons, 4, 0);
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(0, 33);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.RowCount = 2;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1017, 707);
            this.tableLayoutPanelMain.TabIndex = 1;
            // 
            // grInventory
            // 
            this.grInventory.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.grInventory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grInventory.Location = new System.Drawing.Point(3, 10);
            this.grInventory.MainView = this.gvInventory;
            this.grInventory.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.grInventory.Name = "grInventory";
            this.grInventory.Size = new System.Drawing.Size(541, 622);
            this.grInventory.TabIndex = 0;
            this.grInventory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvInventory});
            // 
            // gvInventory
            // 
            this.gvInventory.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.gvInventory.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvInventory.Appearance.Preview.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gvInventory.Appearance.Preview.Options.UseFont = true;
            this.gvInventory.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gvInventory.Appearance.Row.Options.UseFont = true;
            this.gvInventory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colItemNumber,
            this.colDescription,
            this.colOrdered,
            this.colReceived,
            this.colReceivedNow,
            this.colUnit});
            this.gvInventory.CustomizationFormBounds = new System.Drawing.Rectangle(854, 527, 208, 196);
            this.gvInventory.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gvInventory.GridControl = this.grInventory;
            this.gvInventory.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gvInventory.Name = "gvInventory";
            this.gvInventory.OptionsBehavior.Editable = false;
            this.gvInventory.OptionsCustomization.AllowFilter = false;
            this.gvInventory.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvInventory.OptionsView.AutoCalcPreviewLineCount = true;
            this.gvInventory.OptionsView.ShowGroupPanel = false;
            this.gvInventory.OptionsView.ShowIndicator = false;
            this.gvInventory.OptionsView.ShowPreview = true;
            this.gvInventory.PreviewFieldName = "COMMENT";
            this.gvInventory.PreviewLineCount = 5;
            this.gvInventory.RowHeight = 30;
            this.gvInventory.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.gvInventory.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.gvInventory.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gvInventory.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvInventory_FocusedRowChanged);
            // 
            // colItemNumber
            // 
            this.colItemNumber.Caption = "Item number";
            this.colItemNumber.FieldName = "ITEMNUMBER";
            this.colItemNumber.Name = "colItemNumber";
            this.colItemNumber.OptionsColumn.AllowEdit = false;
            this.colItemNumber.Visible = true;
            this.colItemNumber.VisibleIndex = 0;
            // 
            // colDescription
            // 
            this.colDescription.Caption = "Description";
            this.colDescription.FieldName = "ITEMNAME";
            this.colDescription.Name = "colDescription";
            this.colDescription.OptionsColumn.AllowEdit = false;
            this.colDescription.Visible = true;
            this.colDescription.VisibleIndex = 1;
            // 
            // colOrdered
            // 
            this.colOrdered.Caption = "Ordered";
            this.colOrdered.DisplayFormat.FormatString = "n3";
            this.colOrdered.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colOrdered.FieldName = "QUANTITYORDERED";
            this.colOrdered.Name = "colOrdered";
            this.colOrdered.OptionsColumn.AllowEdit = false;
            this.colOrdered.Visible = true;
            this.colOrdered.VisibleIndex = 2;
            // 
            // colReceived
            // 
            this.colReceived.Caption = "Quantity received";
            this.colReceived.DisplayFormat.FormatString = "n3";
            this.colReceived.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colReceived.FieldName = "QUANTITYRECEIVED";
            this.colReceived.Name = "colReceived";
            this.colReceived.OptionsColumn.AllowEdit = false;
            this.colReceived.Visible = true;
            this.colReceived.VisibleIndex = 3;
            // 
            // colReceivedNow
            // 
            this.colReceivedNow.Caption = "Quantity received now";
            this.colReceivedNow.DisplayFormat.FormatString = "n3";
            this.colReceivedNow.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colReceivedNow.FieldName = "QUANTITYRECEIVEDNOW";
            this.colReceivedNow.Name = "colReceivedNow";
            this.colReceivedNow.OptionsColumn.AllowEdit = false;
            this.colReceivedNow.Visible = true;
            this.colReceivedNow.VisibleIndex = 4;
            // 
            // colUnit
            // 
            this.colUnit.Caption = "Unit of measure";
            this.colUnit.FieldName = "UNITOFMEASURE";
            this.colUnit.Name = "colUnit";
            this.colUnit.OptionsColumn.AllowEdit = false;
            this.colUnit.Visible = true;
            this.colUnit.VisibleIndex = 5;
            // 
            // tablePanelArrowButtons
            // 
            this.tablePanelArrowButtons.ColumnCount = 4;
            this.tablePanelArrowButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tablePanelArrowButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tablePanelArrowButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tablePanelArrowButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tablePanelArrowButtons.Controls.Add(this.btnPgUp, 0, 0);
            this.tablePanelArrowButtons.Controls.Add(this.btnUp, 1, 0);
            this.tablePanelArrowButtons.Controls.Add(this.btnDown, 2, 0);
            this.tablePanelArrowButtons.Controls.Add(this.btnPgDown, 3, 0);
            this.tablePanelArrowButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanelArrowButtons.Location = new System.Drawing.Point(3, 638);
            this.tablePanelArrowButtons.Name = "tablePanelArrowButtons";
            this.tablePanelArrowButtons.RowCount = 1;
            this.tablePanelArrowButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelArrowButtons.Size = new System.Drawing.Size(541, 66);
            this.tablePanelArrowButtons.TabIndex = 19;
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(3, 3);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(129, 60);
            this.btnPgUp.TabIndex = 10;
            this.btnPgUp.Tag = "";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(138, 3);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(129, 60);
            this.btnUp.TabIndex = 11;
            this.btnUp.Tag = "";
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(273, 3);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(129, 60);
            this.btnDown.TabIndex = 12;
            this.btnDown.Tag = "";
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(408, 3);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(130, 60);
            this.btnPgDown.TabIndex = 13;
            this.btnPgDown.Tag = "";
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // tableLayoutInputs
            // 
            this.tableLayoutInputs.ColumnCount = 1;
            this.tableLayoutInputs.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutInputs.Controls.Add(this.txtDeliveryMethod, 0, 9);
            this.tableLayoutInputs.Controls.Add(this.lblReceiptNumberHeading, 0, 0);
            this.tableLayoutInputs.Controls.Add(this.txtReceiptNumber, 0, 1);
            this.tableLayoutInputs.Controls.Add(this.lblDriverHeading, 0, 2);
            this.tableLayoutInputs.Controls.Add(this.txtDriver, 0, 3);
            this.tableLayoutInputs.Controls.Add(this.lblPoNumberHeading, 0, 4);
            this.tableLayoutInputs.Controls.Add(this.txtPoNumber, 0, 5);
            this.tableLayoutInputs.Controls.Add(this.lblDeliveryHeading, 0, 6);
            this.tableLayoutInputs.Controls.Add(this.txtDelivery, 0, 7);
            this.tableLayoutInputs.Controls.Add(this.numPad1, 0, 10);
            this.tableLayoutInputs.Controls.Add(this.lblDeliveryMethod, 0, 8);
            this.tableLayoutInputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutInputs.Location = new System.Drawing.Point(570, 3);
            this.tableLayoutInputs.Name = "tableLayoutInputs";
            this.tableLayoutInputs.RowCount = 11;
            this.tableLayoutPanelMain.SetRowSpan(this.tableLayoutInputs, 2);
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutInputs.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutInputs.Size = new System.Drawing.Size(287, 701);
            this.tableLayoutInputs.TabIndex = 20;
            // 
            // txtDeliveryMethod
            // 
            this.txtDeliveryMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeliveryMethod.Location = new System.Drawing.Point(3, 219);
            this.txtDeliveryMethod.Name = "txtDeliveryMethod";
            this.txtDeliveryMethod.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDeliveryMethod.Properties.Appearance.Options.UseFont = true;
            this.txtDeliveryMethod.Properties.ReadOnly = true;
            this.txtDeliveryMethod.Size = new System.Drawing.Size(281, 24);
            this.txtDeliveryMethod.TabIndex = 12;
            // 
            // lblReceiptNumberHeading
            // 
            this.lblReceiptNumberHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblReceiptNumberHeading.AutoSize = true;
            this.lblReceiptNumberHeading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReceiptNumberHeading.Location = new System.Drawing.Point(3, 0);
            this.lblReceiptNumberHeading.Name = "lblReceiptNumberHeading";
            this.lblReceiptNumberHeading.Size = new System.Drawing.Size(158, 19);
            this.lblReceiptNumberHeading.TabIndex = 1;
            this.lblReceiptNumberHeading.Text = "Picking/Receiving no.:";
            this.lblReceiptNumberHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.lblReceiptNumberHeading.Visible = false;
            // 
            // txtReceiptNumber
            // 
            this.txtReceiptNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReceiptNumber.Location = new System.Drawing.Point(3, 22);
            this.txtReceiptNumber.Name = "txtReceiptNumber";
            this.txtReceiptNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtReceiptNumber.Properties.Appearance.Options.UseFont = true;
            this.txtReceiptNumber.Properties.Appearance.Options.UseTextOptions = true;
            this.txtReceiptNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtReceiptNumber.Properties.ReadOnly = true;
            this.txtReceiptNumber.Size = new System.Drawing.Size(281, 24);
            this.txtReceiptNumber.TabIndex = 2;
            this.txtReceiptNumber.Visible = false;
            // 
            // lblDriverHeading
            // 
            this.lblDriverHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDriverHeading.AutoSize = true;
            this.lblDriverHeading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDriverHeading.Location = new System.Drawing.Point(3, 49);
            this.lblDriverHeading.Name = "lblDriverHeading";
            this.lblDriverHeading.Size = new System.Drawing.Size(103, 19);
            this.lblDriverHeading.TabIndex = 3;
            this.lblDriverHeading.Text = "Driver details:";
            this.lblDriverHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtDriver
            // 
            this.txtDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDriver.Location = new System.Drawing.Point(3, 71);
            this.txtDriver.Name = "txtDriver";
            this.txtDriver.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDriver.Properties.Appearance.Options.UseFont = true;
            this.txtDriver.Properties.Appearance.Options.UseTextOptions = true;
            this.txtDriver.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtDriver.Size = new System.Drawing.Size(281, 24);
            this.txtDriver.TabIndex = 4;
            // 
            // lblPoNumberHeading
            // 
            this.lblPoNumberHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPoNumberHeading.AutoSize = true;
            this.lblPoNumberHeading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPoNumberHeading.Location = new System.Drawing.Point(3, 98);
            this.lblPoNumberHeading.Name = "lblPoNumberHeading";
            this.lblPoNumberHeading.Size = new System.Drawing.Size(110, 19);
            this.lblPoNumberHeading.TabIndex = 5;
            this.lblPoNumberHeading.Text = "Order number:";
            this.lblPoNumberHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtPoNumber
            // 
            this.txtPoNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPoNumber.Location = new System.Drawing.Point(3, 120);
            this.txtPoNumber.Name = "txtPoNumber";
            this.txtPoNumber.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPoNumber.Properties.Appearance.Options.UseFont = true;
            this.txtPoNumber.Properties.Appearance.Options.UseTextOptions = true;
            this.txtPoNumber.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtPoNumber.Properties.ReadOnly = true;
            this.txtPoNumber.Size = new System.Drawing.Size(281, 24);
            this.txtPoNumber.TabIndex = 6;
            // 
            // lblDeliveryHeading
            // 
            this.lblDeliveryHeading.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDeliveryHeading.AutoSize = true;
            this.lblDeliveryHeading.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeliveryHeading.Location = new System.Drawing.Point(3, 147);
            this.lblDeliveryHeading.Name = "lblDeliveryHeading";
            this.lblDeliveryHeading.Size = new System.Drawing.Size(161, 19);
            this.lblDeliveryHeading.TabIndex = 7;
            this.lblDeliveryHeading.Text = "Delivery note number:";
            this.lblDeliveryHeading.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // txtDelivery
            // 
            this.txtDelivery.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDelivery.Location = new System.Drawing.Point(3, 169);
            this.txtDelivery.Name = "txtDelivery";
            this.txtDelivery.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtDelivery.Properties.Appearance.Options.UseFont = true;
            this.txtDelivery.Properties.Appearance.Options.UseTextOptions = true;
            this.txtDelivery.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtDelivery.Size = new System.Drawing.Size(281, 24);
            this.txtDelivery.TabIndex = 8;
            // 
            // numPad1
            // 
            this.numPad1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.numPad1.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Barcode;
            this.numPad1.Location = new System.Drawing.Point(3, 384);
            this.numPad1.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.numPad1.MaskChar = "";
            this.numPad1.MaskInterval = 0;
            this.numPad1.MaxNumberOfDigits = 20;
            this.numPad1.MinimumSize = new System.Drawing.Size(248, 314);
            this.numPad1.Name = "numPad1";
            this.numPad1.NegativeMode = false;
            this.numPad1.NoOfTries = 0;
            this.numPad1.NumberOfDecimals = 2;
            this.numPad1.PromptText = "";
            this.numPad1.ShortcutKeysActive = false;
            this.numPad1.Size = new System.Drawing.Size(281, 314);
            this.numPad1.TabIndex = 9;
            this.numPad1.TimerEnabled = true;
            this.numPad1.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numPad1_EnterButtonPressed);
            // 
            // lblDeliveryMethod
            // 
            this.lblDeliveryMethod.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblDeliveryMethod.AutoSize = true;
            this.lblDeliveryMethod.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDeliveryMethod.Location = new System.Drawing.Point(3, 196);
            this.lblDeliveryMethod.Name = "lblDeliveryMethod";
            this.lblDeliveryMethod.Size = new System.Drawing.Size(126, 19);
            this.lblDeliveryMethod.TabIndex = 11;
            this.lblDeliveryMethod.Text = "Delivery method:";
            this.lblDeliveryMethod.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // tablePanelSideButtons
            // 
            this.tablePanelSideButtons.ColumnCount = 1;
            this.tablePanelSideButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tablePanelSideButtons.Controls.Add(this.btnRefresh, 0, 1);
            this.tablePanelSideButtons.Controls.Add(this.btnSave, 0, 2);
            this.tablePanelSideButtons.Controls.Add(this.btnEdit, 0, 5);
            this.tablePanelSideButtons.Controls.Add(this.btnCommit, 0, 6);
            this.tablePanelSideButtons.Controls.Add(this.btnClose, 0, 8);
            this.tablePanelSideButtons.Controls.Add(this.btnUom, 0, 4);
            this.tablePanelSideButtons.Controls.Add(this.btnReceiveAll, 0, 0);
            this.tablePanelSideButtons.Controls.Add(this.btnSearch, 0, 7);
            this.tablePanelSideButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablePanelSideButtons.Location = new System.Drawing.Point(883, 3);
            this.tablePanelSideButtons.Name = "tablePanelSideButtons";
            this.tablePanelSideButtons.RowCount = 5;
            this.tableLayoutPanelMain.SetRowSpan(this.tablePanelSideButtons, 2);
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.19512F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10.97561F));
            this.tablePanelSideButtons.Size = new System.Drawing.Size(131, 701);
            this.tablePanelSideButtons.TabIndex = 21;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.Appearance.Options.UseTextOptions = true;
            this.btnRefresh.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnRefresh.Location = new System.Drawing.Point(3, 79);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(125, 70);
            this.btnRefresh.TabIndex = 13;
            this.btnRefresh.Tag = "BtnNormal";
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Appearance.Options.UseTextOptions = true;
            this.btnSave.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnSave.Location = new System.Drawing.Point(3, 155);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(125, 70);
            this.btnSave.TabIndex = 12;
            this.btnSave.Tag = "BtnNormal";
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Appearance.Options.UseTextOptions = true;
            this.btnEdit.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnEdit.Location = new System.Drawing.Point(3, 392);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(125, 70);
            this.btnEdit.TabIndex = 15;
            this.btnEdit.Tag = "BtnNormal";
            this.btnEdit.Text = "Edit quantity";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Appearance.Options.UseTextOptions = true;
            this.btnCommit.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnCommit.Location = new System.Drawing.Point(3, 468);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(125, 70);
            this.btnCommit.TabIndex = 16;
            this.btnCommit.Tag = "BtnNormal";
            this.btnCommit.Text = "Commit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Appearance.Options.UseTextOptions = true;
            this.btnClose.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnClose.Location = new System.Drawing.Point(3, 620);
            this.btnClose.Margin = new System.Windows.Forms.Padding(3, 3, 3, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(125, 76);
            this.btnClose.TabIndex = 18;
            this.btnClose.Tag = "BtnLong";
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnUom
            // 
            this.btnUom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUom.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnUom.Appearance.Options.UseFont = true;
            this.btnUom.Appearance.Options.UseTextOptions = true;
            this.btnUom.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnUom.Location = new System.Drawing.Point(3, 316);
            this.btnUom.Name = "btnUom";
            this.btnUom.Size = new System.Drawing.Size(125, 70);
            this.btnUom.TabIndex = 14;
            this.btnUom.Tag = "BtnNormal";
            this.btnUom.Text = "Edit unit of measure";
            this.btnUom.Click += new System.EventHandler(this.btnUom_Click);
            // 
            // btnReceiveAll
            // 
            this.btnReceiveAll.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReceiveAll.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReceiveAll.Appearance.Options.UseFont = true;
            this.btnReceiveAll.Appearance.Options.UseTextOptions = true;
            this.btnReceiveAll.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnReceiveAll.Location = new System.Drawing.Point(3, 3);
            this.btnReceiveAll.Name = "btnReceiveAll";
            this.btnReceiveAll.Size = new System.Drawing.Size(125, 70);
            this.btnReceiveAll.TabIndex = 13;
            this.btnReceiveAll.Tag = "BtnNormal";
            this.btnReceiveAll.Text = "Receive all";
            this.btnReceiveAll.Click += new System.EventHandler(this.btnReceiveAll_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Appearance.Options.UseTextOptions = true;
            this.btnSearch.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.btnSearch.Location = new System.Drawing.Point(3, 544);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(125, 70);
            this.btnSearch.TabIndex = 17;
            this.btnSearch.Tag = "BtnLong";
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(0, 0);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(15);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(1018, 33);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Picking/Receiving";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.lblHeader);
            this.panelControl1.Controls.Add(this.tableLayoutPanelMain);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1018, 743);
            this.panelControl1.TabIndex = 0;
            // 
            // frmPurchaseOrderReceiving
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1018, 743);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmPurchaseOrderReceiving";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmPurchaseOrderReceiving_FormClosing);
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.tableLayoutPanelMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grInventory)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvInventory)).EndInit();
            this.tablePanelArrowButtons.ResumeLayout(false);
            this.tableLayoutInputs.ResumeLayout(false);
            this.tableLayoutInputs.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtDeliveryMethod.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtReceiptNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDriver.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPoNumber.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtDelivery.Properties)).EndInit();
            this.tablePanelSideButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblHeader;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numPad1;
        private DevExpress.XtraGrid.GridControl grInventory;
        private DevExpress.XtraGrid.Views.Grid.GridView gvInventory;
        private System.Windows.Forms.Label lblReceiptNumberHeading;
        private System.Windows.Forms.Label lblDriverHeading;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
        private System.Windows.Forms.Label lblDeliveryHeading;
        private System.Windows.Forms.Label lblPoNumberHeading;
        private DevExpress.XtraGrid.Columns.GridColumn colItemNumber;
        private DevExpress.XtraGrid.Columns.GridColumn colOrdered;
        private DevExpress.XtraGrid.Columns.GridColumn colReceived;
        private DevExpress.XtraGrid.Columns.GridColumn colReceivedNow;
        private DevExpress.XtraGrid.Columns.GridColumn colUnit;
        private DevExpress.XtraEditors.TextEdit txtDelivery;
        private DevExpress.XtraEditors.TextEdit txtDriver;
        private DevExpress.XtraEditors.TextEdit txtReceiptNumber;
        private DevExpress.XtraEditors.TextEdit txtPoNumber;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnReceiveAll;
        private System.Windows.Forms.TableLayoutPanel tablePanelArrowButtons;
        private DevExpress.XtraGrid.Columns.GridColumn colDescription;
        private System.Windows.Forms.TableLayoutPanel tableLayoutInputs;
        private System.Windows.Forms.TableLayoutPanel tablePanelSideButtons;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnRefresh;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSave;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUom;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.TextEdit txtDeliveryMethod;
        private System.Windows.Forms.Label lblDeliveryMethod;
    }
}
