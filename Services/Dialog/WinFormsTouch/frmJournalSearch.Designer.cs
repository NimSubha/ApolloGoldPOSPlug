/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    partial class frmJournalSearch : frmTouchBase
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnTransactionId = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.searchPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblTransactionIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.txtTransactionId = new DevExpress.XtraEditors.TextEdit();
            this.lblItemIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.tableLayoutPaneItemSearch = new System.Windows.Forms.TableLayoutPanel();
            this.txtItemId = new DevExpress.XtraEditors.TextEdit();
            this.btnItemSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblCustomerIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerId = new DevExpress.XtraEditors.TextEdit();
            this.lblStoreIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.txtStoreId = new DevExpress.XtraEditors.TextEdit();
            this.lblCustomerFirstNameHeading = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerFirstName = new DevExpress.XtraEditors.TextEdit();
            this.lblStartDateHeading = new DevExpress.XtraEditors.LabelControl();
            this.lblStaffIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.txtStaffId = new DevExpress.XtraEditors.TextEdit();
            this.lblCustomerLastNameHeading = new DevExpress.XtraEditors.LabelControl();
            this.txtCustomerLastName = new DevExpress.XtraEditors.TextEdit();
            this.lblEndDateHeading = new DevExpress.XtraEditors.LabelControl();
            this.dtpFromDate = new System.Windows.Forms.DateTimePicker();
            this.dtpToDate = new System.Windows.Forms.DateTimePicker();
            this.lblHeader = new System.Windows.Forms.Label();
            this.basePanel = new System.Windows.Forms.Panel();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtMobileNo = new DevExpress.XtraEditors.TextEdit();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanelBottom.SuspendLayout();
            this.searchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionId.Properties)).BeginInit();
            this.tableLayoutPaneItemSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStoreId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerFirstName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStaffId.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerLastName.Properties)).BeginInit();
            this.basePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtMobileNo.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanelBottom, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.searchPanel, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1016, 741);
            this.tableLayoutPanel1.TabIndex = 40;
            // 
            // tableLayoutPanelBottom
            // 
            this.tableLayoutPanelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelBottom.AutoSize = true;
            this.tableLayoutPanelBottom.ColumnCount = 3;
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBottom.Controls.Add(this.btnClear, 1, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.btnClose, 2, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.btnTransactionId, 0, 0);
            this.tableLayoutPanelBottom.Location = new System.Drawing.Point(33, 661);
            this.tableLayoutPanelBottom.Margin = new System.Windows.Forms.Padding(3, 15, 3, 3);
            this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            this.tableLayoutPanelBottom.RowCount = 1;
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelBottom.Size = new System.Drawing.Size(950, 66);
            this.tableLayoutPanelBottom.TabIndex = 48;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(410, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(0);
            this.btnClear.Size = new System.Drawing.Size(130, 60);
            this.btnClear.TabIndex = 36;
            this.btnClear.Tag = "";
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.AutoWidthInLayoutControl = true;
            this.btnClose.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClose.Location = new System.Drawing.Point(546, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(0);
            this.btnClose.Size = new System.Drawing.Size(130, 60);
            this.btnClose.TabIndex = 43;
            this.btnClose.Tag = "";
            this.btnClose.Text = "Cancel";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnTransactionId
            // 
            this.btnTransactionId.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnTransactionId.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTransactionId.Appearance.Options.UseFont = true;
            this.btnTransactionId.AutoWidthInLayoutControl = true;
            this.btnTransactionId.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnTransactionId.Location = new System.Drawing.Point(274, 3);
            this.btnTransactionId.Name = "btnTransactionId";
            this.btnTransactionId.Padding = new System.Windows.Forms.Padding(0);
            this.btnTransactionId.Size = new System.Drawing.Size(130, 60);
            this.btnTransactionId.TabIndex = 35;
            this.btnTransactionId.Tag = "";
            this.btnTransactionId.Text = "Search";
            this.btnTransactionId.Click += new System.EventHandler(this.btnTransactionId_Click);
            // 
            // searchPanel
            // 
            this.searchPanel.AllowDrop = true;
            this.searchPanel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.searchPanel.ColumnCount = 3;
            this.searchPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.searchPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.searchPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.searchPanel.Controls.Add(this.txtMobileNo, 0, 7);
            this.searchPanel.Controls.Add(this.lblTransactionIdHeading, 0, 0);
            this.searchPanel.Controls.Add(this.txtTransactionId, 0, 1);
            this.searchPanel.Controls.Add(this.lblItemIdHeading, 0, 2);
            this.searchPanel.Controls.Add(this.tableLayoutPaneItemSearch, 0, 3);
            this.searchPanel.Controls.Add(this.lblCustomerIdHeading, 0, 4);
            this.searchPanel.Controls.Add(this.txtCustomerId, 0, 5);
            this.searchPanel.Controls.Add(this.lblStoreIdHeading, 1, 0);
            this.searchPanel.Controls.Add(this.txtStoreId, 1, 1);
            this.searchPanel.Controls.Add(this.lblCustomerFirstNameHeading, 1, 4);
            this.searchPanel.Controls.Add(this.txtCustomerFirstName, 1, 5);
            this.searchPanel.Controls.Add(this.lblStartDateHeading, 1, 2);
            this.searchPanel.Controls.Add(this.lblStaffIdHeading, 2, 0);
            this.searchPanel.Controls.Add(this.txtStaffId, 2, 1);
            this.searchPanel.Controls.Add(this.lblCustomerLastNameHeading, 2, 4);
            this.searchPanel.Controls.Add(this.txtCustomerLastName, 2, 5);
            this.searchPanel.Controls.Add(this.lblEndDateHeading, 2, 2);
            this.searchPanel.Controls.Add(this.dtpFromDate, 1, 3);
            this.searchPanel.Controls.Add(this.dtpToDate, 2, 3);
            this.searchPanel.Controls.Add(this.labelControl1, 0, 6);
            this.searchPanel.Location = new System.Drawing.Point(30, 167);
            this.searchPanel.Margin = new System.Windows.Forms.Padding(0);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new System.Windows.Forms.Padding(30, 10, 30, 11);
            this.searchPanel.RowCount = 8;
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 12.69841F));
            this.searchPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 11.11111F));
            this.searchPanel.Size = new System.Drawing.Size(950, 446);
            this.searchPanel.TabIndex = 41;
            // 
            // lblTransactionIdHeading
            // 
            this.lblTransactionIdHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTransactionIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransactionIdHeading.Location = new System.Drawing.Point(33, 35);
            this.lblTransactionIdHeading.Name = "lblTransactionIdHeading";
            this.lblTransactionIdHeading.Size = new System.Drawing.Size(119, 25);
            this.lblTransactionIdHeading.TabIndex = 33;
            this.lblTransactionIdHeading.Text = "Transaction Id";
            // 
            // txtTransactionId
            // 
            this.txtTransactionId.Location = new System.Drawing.Point(33, 66);
            this.txtTransactionId.Name = "txtTransactionId";
            this.txtTransactionId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtTransactionId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtTransactionId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtTransactionId.Properties.Appearance.Options.UseBackColor = true;
            this.txtTransactionId.Properties.Appearance.Options.UseFont = true;
            this.txtTransactionId.Properties.Appearance.Options.UseForeColor = true;
            this.txtTransactionId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtTransactionId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtTransactionId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtTransactionId.Size = new System.Drawing.Size(288, 32);
            this.txtTransactionId.TabIndex = 0;
            // 
            // lblItemIdHeading
            // 
            this.lblItemIdHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblItemIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblItemIdHeading.Location = new System.Drawing.Point(33, 141);
            this.lblItemIdHeading.Name = "lblItemIdHeading";
            this.lblItemIdHeading.Size = new System.Drawing.Size(89, 25);
            this.lblItemIdHeading.TabIndex = 33;
            this.lblItemIdHeading.Text = "Product ID";
            // 
            // tableLayoutPaneItemSearch
            // 
            this.tableLayoutPaneItemSearch.ColumnCount = 2;
            this.tableLayoutPaneItemSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPaneItemSearch.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPaneItemSearch.Controls.Add(this.txtItemId, 0, 0);
            this.tableLayoutPaneItemSearch.Controls.Add(this.btnItemSearch, 1, 0);
            this.tableLayoutPaneItemSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPaneItemSearch.Location = new System.Drawing.Point(30, 169);
            this.tableLayoutPaneItemSearch.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPaneItemSearch.Name = "tableLayoutPaneItemSearch";
            this.tableLayoutPaneItemSearch.RowCount = 1;
            this.tableLayoutPaneItemSearch.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPaneItemSearch.Size = new System.Drawing.Size(296, 53);
            this.tableLayoutPaneItemSearch.TabIndex = 41;
            // 
            // txtItemId
            // 
            this.txtItemId.Location = new System.Drawing.Point(3, 3);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtItemId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtItemId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtItemId.Properties.Appearance.Options.UseBackColor = true;
            this.txtItemId.Properties.Appearance.Options.UseFont = true;
            this.txtItemId.Properties.Appearance.Options.UseForeColor = true;
            this.txtItemId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtItemId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtItemId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtItemId.Size = new System.Drawing.Size(225, 32);
            this.txtItemId.TabIndex = 3;
            // 
            // btnItemSearch
            // 
            this.btnItemSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnItemSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.search;
            this.btnItemSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnItemSearch.Location = new System.Drawing.Point(234, 3);
            this.btnItemSearch.Name = "btnItemSearch";
            this.btnItemSearch.Padding = new System.Windows.Forms.Padding(3);
            this.btnItemSearch.Size = new System.Drawing.Size(59, 32);
            this.btnItemSearch.TabIndex = 27;
            this.btnItemSearch.Click += new System.EventHandler(this.btnItemSearch_Click);
            // 
            // lblCustomerIdHeading
            // 
            this.lblCustomerIdHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCustomerIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerIdHeading.Location = new System.Drawing.Point(33, 247);
            this.lblCustomerIdHeading.Name = "lblCustomerIdHeading";
            this.lblCustomerIdHeading.Size = new System.Drawing.Size(102, 25);
            this.lblCustomerIdHeading.TabIndex = 33;
            this.lblCustomerIdHeading.Text = "Customer Id";
            // 
            // txtCustomerId
            // 
            this.txtCustomerId.Location = new System.Drawing.Point(33, 278);
            this.txtCustomerId.Name = "txtCustomerId";
            this.txtCustomerId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtCustomerId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtCustomerId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCustomerId.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustomerId.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerId.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustomerId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtCustomerId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtCustomerId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtCustomerId.Size = new System.Drawing.Size(290, 32);
            this.txtCustomerId.TabIndex = 7;
            // 
            // lblStoreIdHeading
            // 
            this.lblStoreIdHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStoreIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStoreIdHeading.Location = new System.Drawing.Point(329, 35);
            this.lblStoreIdHeading.Name = "lblStoreIdHeading";
            this.lblStoreIdHeading.Size = new System.Drawing.Size(65, 25);
            this.lblStoreIdHeading.TabIndex = 33;
            this.lblStoreIdHeading.Text = "Store Id";
            // 
            // txtStoreId
            // 
            this.txtStoreId.Location = new System.Drawing.Point(329, 66);
            this.txtStoreId.Name = "txtStoreId";
            this.txtStoreId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtStoreId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtStoreId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtStoreId.Properties.Appearance.Options.UseBackColor = true;
            this.txtStoreId.Properties.Appearance.Options.UseFont = true;
            this.txtStoreId.Properties.Appearance.Options.UseForeColor = true;
            this.txtStoreId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtStoreId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtStoreId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtStoreId.Size = new System.Drawing.Size(290, 32);
            this.txtStoreId.TabIndex = 1;
            // 
            // lblCustomerFirstNameHeading
            // 
            this.lblCustomerFirstNameHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCustomerFirstNameHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerFirstNameHeading.Location = new System.Drawing.Point(329, 247);
            this.lblCustomerFirstNameHeading.Name = "lblCustomerFirstNameHeading";
            this.lblCustomerFirstNameHeading.Size = new System.Drawing.Size(176, 25);
            this.lblCustomerFirstNameHeading.TabIndex = 33;
            this.lblCustomerFirstNameHeading.Text = "Customer First Name";
            // 
            // txtCustomerFirstName
            // 
            this.txtCustomerFirstName.Location = new System.Drawing.Point(329, 278);
            this.txtCustomerFirstName.Name = "txtCustomerFirstName";
            this.txtCustomerFirstName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtCustomerFirstName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtCustomerFirstName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCustomerFirstName.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustomerFirstName.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerFirstName.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustomerFirstName.Properties.Appearance.Options.UseTextOptions = true;
            this.txtCustomerFirstName.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtCustomerFirstName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtCustomerFirstName.Size = new System.Drawing.Size(290, 32);
            this.txtCustomerFirstName.TabIndex = 8;
            // 
            // lblStartDateHeading
            // 
            this.lblStartDateHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStartDateHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStartDateHeading.Location = new System.Drawing.Point(329, 141);
            this.lblStartDateHeading.Name = "lblStartDateHeading";
            this.lblStartDateHeading.Size = new System.Drawing.Size(83, 25);
            this.lblStartDateHeading.TabIndex = 3;
            this.lblStartDateHeading.Text = "Start Date";
            // 
            // lblStaffIdHeading
            // 
            this.lblStaffIdHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStaffIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStaffIdHeading.Location = new System.Drawing.Point(625, 35);
            this.lblStaffIdHeading.Name = "lblStaffIdHeading";
            this.lblStaffIdHeading.Size = new System.Drawing.Size(99, 25);
            this.lblStaffIdHeading.TabIndex = 33;
            this.lblStaffIdHeading.Text = "Operator ID";
            // 
            // txtStaffId
            // 
            this.txtStaffId.Location = new System.Drawing.Point(625, 66);
            this.txtStaffId.Name = "txtStaffId";
            this.txtStaffId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtStaffId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtStaffId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtStaffId.Properties.Appearance.Options.UseBackColor = true;
            this.txtStaffId.Properties.Appearance.Options.UseFont = true;
            this.txtStaffId.Properties.Appearance.Options.UseForeColor = true;
            this.txtStaffId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtStaffId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtStaffId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtStaffId.Size = new System.Drawing.Size(290, 32);
            this.txtStaffId.TabIndex = 2;
            // 
            // lblCustomerLastNameHeading
            // 
            this.lblCustomerLastNameHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCustomerLastNameHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerLastNameHeading.Location = new System.Drawing.Point(625, 247);
            this.lblCustomerLastNameHeading.Name = "lblCustomerLastNameHeading";
            this.lblCustomerLastNameHeading.Size = new System.Drawing.Size(174, 25);
            this.lblCustomerLastNameHeading.TabIndex = 33;
            this.lblCustomerLastNameHeading.Text = "Customer Last Name";
            // 
            // txtCustomerLastName
            // 
            this.txtCustomerLastName.Location = new System.Drawing.Point(625, 278);
            this.txtCustomerLastName.Name = "txtCustomerLastName";
            this.txtCustomerLastName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtCustomerLastName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtCustomerLastName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCustomerLastName.Properties.Appearance.Options.UseBackColor = true;
            this.txtCustomerLastName.Properties.Appearance.Options.UseFont = true;
            this.txtCustomerLastName.Properties.Appearance.Options.UseForeColor = true;
            this.txtCustomerLastName.Properties.Appearance.Options.UseTextOptions = true;
            this.txtCustomerLastName.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtCustomerLastName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtCustomerLastName.Size = new System.Drawing.Size(290, 32);
            this.txtCustomerLastName.TabIndex = 9;
            // 
            // lblEndDateHeading
            // 
            this.lblEndDateHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblEndDateHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEndDateHeading.Location = new System.Drawing.Point(625, 141);
            this.lblEndDateHeading.Name = "lblEndDateHeading";
            this.lblEndDateHeading.Size = new System.Drawing.Size(76, 25);
            this.lblEndDateHeading.TabIndex = 33;
            this.lblEndDateHeading.Text = "End Date";
            // 
            // dtpFromDate
            // 
            this.dtpFromDate.CalendarFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFromDate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFromDate.Location = new System.Drawing.Point(329, 172);
            this.dtpFromDate.Name = "dtpFromDate";
            this.dtpFromDate.Size = new System.Drawing.Size(288, 29);
            this.dtpFromDate.TabIndex = 42;
            // 
            // dtpToDate
            // 
            this.dtpToDate.CalendarFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpToDate.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpToDate.Location = new System.Drawing.Point(625, 172);
            this.dtpToDate.Name = "dtpToDate";
            this.dtpToDate.Size = new System.Drawing.Size(288, 29);
            this.dtpToDate.TabIndex = 41;
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(351, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(314, 65);
            this.lblHeader.TabIndex = 40;
            this.lblHeader.Text = "Search journal";
            // 
            // basePanel
            // 
            this.basePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.basePanel.Controls.Add(this.tableLayoutPanel1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1018, 743);
            this.basePanel.TabIndex = 0;
            // 
            // labelControl1
            // 
            this.labelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelControl1.Location = new System.Drawing.Point(33, 353);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(89, 25);
            this.labelControl1.TabIndex = 43;
            this.labelControl1.Text = "Mobile No";
            // 
            // txtMobileNo
            // 
            this.txtMobileNo.Location = new System.Drawing.Point(33, 384);
            this.txtMobileNo.Name = "txtMobileNo";
            this.txtMobileNo.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtMobileNo.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtMobileNo.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtMobileNo.Properties.Appearance.Options.UseBackColor = true;
            this.txtMobileNo.Properties.Appearance.Options.UseFont = true;
            this.txtMobileNo.Properties.Appearance.Options.UseForeColor = true;
            this.txtMobileNo.Properties.Appearance.Options.UseTextOptions = true;
            this.txtMobileNo.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtMobileNo.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtMobileNo.Size = new System.Drawing.Size(290, 32);
            this.txtMobileNo.TabIndex = 44;
            // 
            // frmJournalSearch
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1018, 743);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmJournalSearch";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanelBottom.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtTransactionId.Properties)).EndInit();
            this.tableLayoutPaneItemSearch.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtItemId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStoreId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerFirstName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtStaffId.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtCustomerLastName.Properties)).EndInit();
            this.basePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtMobileNo.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel basePanel;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.TableLayoutPanel searchPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPaneItemSearch;
        private DevExpress.XtraEditors.TextEdit txtTransactionId;
        private DevExpress.XtraEditors.LabelControl lblCustomerIdHeading;
        private DevExpress.XtraEditors.TextEdit txtCustomerId;
        private DevExpress.XtraEditors.LabelControl lblCustomerFirstNameHeading;
        private DevExpress.XtraEditors.TextEdit txtCustomerFirstName;
        private DevExpress.XtraEditors.LabelControl lblCustomerLastNameHeading;
        private DevExpress.XtraEditors.TextEdit txtCustomerLastName;
        private DevExpress.XtraEditors.LabelControl lblStoreIdHeading;
        private DevExpress.XtraEditors.TextEdit txtStoreId;
        private DevExpress.XtraEditors.LabelControl lblStaffIdHeading;
        private DevExpress.XtraEditors.TextEdit txtStaffId;
        private DevExpress.XtraEditors.LabelControl lblItemIdHeading;
        private DevExpress.XtraEditors.TextEdit txtItemId;
        private DevExpress.XtraEditors.LabelControl lblStartDateHeading;
        private DevExpress.XtraEditors.LabelControl lblEndDateHeading;
        private System.Windows.Forms.DateTimePicker dtpToDate;
        private System.Windows.Forms.DateTimePicker dtpFromDate;
        private DevExpress.XtraEditors.LabelControl lblTransactionIdHeading;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnTransactionId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnItemSearch;
        private DevExpress.XtraEditors.TextEdit txtMobileNo;
        private DevExpress.XtraEditors.LabelControl labelControl1;
    }
}
