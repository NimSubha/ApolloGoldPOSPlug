/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class frmNewShippingAddress
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
            this.components = new System.ComponentModel.Container();
            this.themedPanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanelMain = new System.Windows.Forms.TableLayoutPanel();
            this.labelHeading = new System.Windows.Forms.Label();
            this.tableLayoutPanelBottom = new System.Windows.Forms.TableLayoutPanel();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSave = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.addressUserControl1 = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.AddressUserControl();
            this.tableLayoutPanelLeft = new System.Windows.Forms.TableLayoutPanel();
            this.labelName = new System.Windows.Forms.Label();
            this.labelAddressType = new System.Windows.Forms.Label();
            this.labelSalesTaxGroup = new System.Windows.Forms.Label();
            this.labelPhone = new System.Windows.Forms.Label();
            this.labelEmail = new System.Windows.Forms.Label();
            this.labelWebSite = new System.Windows.Forms.Label();
            this.textBoxName = new DevExpress.XtraEditors.TextEdit();
            this.addressViewModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.textBoxAddressType = new DevExpress.XtraEditors.TextEdit();
            this.textBoxSalesTaxGroup = new DevExpress.XtraEditors.TextEdit();
            this.textBoxPhone = new DevExpress.XtraEditors.TextEdit();
            this.textBoxEmail = new DevExpress.XtraEditors.TextEdit();
            this.textBoxWebSite = new DevExpress.XtraEditors.TextEdit();
            this.labelContactInfo = new System.Windows.Forms.Label();
            this.labelGeneral = new System.Windows.Forms.Label();
            this.btnAddressType = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSalesTaxGroup = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.themedPanel)).BeginInit();
            this.themedPanel.SuspendLayout();
            this.tableLayoutPanelMain.SuspendLayout();
            this.tableLayoutPanelBottom.SuspendLayout();
            this.tableLayoutPanelLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxName.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.addressViewModelBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxAddressType.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxSalesTaxGroup.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxPhone.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxEmail.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxWebSite.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // themedPanel
            // 
            this.themedPanel.Controls.Add(this.tableLayoutPanelMain);
            this.themedPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.themedPanel.Location = new System.Drawing.Point(0, 0);
            this.themedPanel.Name = "themedPanel";
            this.themedPanel.Size = new System.Drawing.Size(1024, 768);
            this.themedPanel.TabIndex = 0;
            // 
            // tableLayoutPanelMain
            // 
            this.tableLayoutPanelMain.ColumnCount = 2;
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelMain.Controls.Add(this.labelHeading, 0, 0);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelBottom, 0, 2);
            this.tableLayoutPanelMain.Controls.Add(this.addressUserControl1, 1, 1);
            this.tableLayoutPanelMain.Controls.Add(this.tableLayoutPanelLeft, 0, 1);
            this.tableLayoutPanelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelMain.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanelMain.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelMain.Name = "tableLayoutPanelMain";
            this.tableLayoutPanelMain.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanelMain.RowCount = 3;
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelMain.Size = new System.Drawing.Size(1020, 764);
            this.tableLayoutPanelMain.TabIndex = 0;
            // 
            // labelHeading
            // 
            this.labelHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelHeading.AutoSize = true;
            this.tableLayoutPanelMain.SetColumnSpan(this.labelHeading, 2);
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.labelHeading.Location = new System.Drawing.Point(274, 40);
            this.labelHeading.Margin = new System.Windows.Forms.Padding(0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.labelHeading.Size = new System.Drawing.Size(472, 95);
            this.labelHeading.TabIndex = 1;
            this.labelHeading.Tag = "";
            this.labelHeading.Text = "New shipping address";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanelBottom
            // 
            this.tableLayoutPanelBottom.ColumnCount = 5;
            this.tableLayoutPanelMain.SetColumnSpan(this.tableLayoutPanelBottom, 2);
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelBottom.Controls.Add(this.btnClear, 1, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.btnSave, 2, 0);
            this.tableLayoutPanelBottom.Controls.Add(this.btnCancel, 3, 0);
            this.tableLayoutPanelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelBottom.Location = new System.Drawing.Point(30, 688);
            this.tableLayoutPanelBottom.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanelBottom.Name = "tableLayoutPanelBottom";
            this.tableLayoutPanelBottom.RowCount = 1;
            this.tableLayoutPanelBottom.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelBottom.Size = new System.Drawing.Size(960, 65);
            this.tableLayoutPanelBottom.TabIndex = 2;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(281, 4);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(127, 57);
            this.btnClear.TabIndex = 1;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(416, 4);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(127, 57);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "Save";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(551, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            // 
            // addressUserControl1
            // 
            this.addressUserControl1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressUserControl1.Location = new System.Drawing.Point(513, 138);
            this.addressUserControl1.Name = "addressUserControl1";
            this.addressUserControl1.Size = new System.Drawing.Size(463, 536);
            this.addressUserControl1.TabIndex = 1;
            // 
            // tableLayoutPanelLeft
            // 
            this.tableLayoutPanelLeft.ColumnCount = 3;
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelLeft.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelLeft.Controls.Add(this.labelName, 0, 1);
            this.tableLayoutPanelLeft.Controls.Add(this.labelAddressType, 0, 3);
            this.tableLayoutPanelLeft.Controls.Add(this.labelSalesTaxGroup, 0, 5);
            this.tableLayoutPanelLeft.Controls.Add(this.labelPhone, 0, 8);
            this.tableLayoutPanelLeft.Controls.Add(this.labelEmail, 0, 10);
            this.tableLayoutPanelLeft.Controls.Add(this.labelWebSite, 0, 12);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxName, 0, 2);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxAddressType, 0, 4);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxSalesTaxGroup, 0, 6);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxPhone, 0, 9);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxEmail, 0, 11);
            this.tableLayoutPanelLeft.Controls.Add(this.textBoxWebSite, 0, 13);
            this.tableLayoutPanelLeft.Controls.Add(this.labelContactInfo, 0, 7);
            this.tableLayoutPanelLeft.Controls.Add(this.labelGeneral, 0, 0);
            this.tableLayoutPanelLeft.Controls.Add(this.btnAddressType, 2, 4);
            this.tableLayoutPanelLeft.Controls.Add(this.btnSalesTaxGroup, 2, 6);
            this.tableLayoutPanelLeft.Location = new System.Drawing.Point(30, 135);
            this.tableLayoutPanelLeft.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanelLeft.Name = "tableLayoutPanelLeft";
            this.tableLayoutPanelLeft.RowCount = 15;
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelLeft.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelLeft.Size = new System.Drawing.Size(469, 542);
            this.tableLayoutPanelLeft.TabIndex = 0;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelName, 2);
            this.labelName.Location = new System.Drawing.Point(3, 29);
            this.labelName.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(112, 21);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "Address name:";
            // 
            // labelAddressType
            // 
            this.labelAddressType.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelAddressType, 2);
            this.labelAddressType.Location = new System.Drawing.Point(3, 89);
            this.labelAddressType.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelAddressType.Name = "labelAddressType";
            this.labelAddressType.Size = new System.Drawing.Size(103, 21);
            this.labelAddressType.TabIndex = 2;
            this.labelAddressType.Text = "Address type:";
            // 
            // labelSalesTaxGroup
            // 
            this.labelSalesTaxGroup.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelSalesTaxGroup, 2);
            this.labelSalesTaxGroup.Location = new System.Drawing.Point(3, 153);
            this.labelSalesTaxGroup.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelSalesTaxGroup.Name = "labelSalesTaxGroup";
            this.labelSalesTaxGroup.Size = new System.Drawing.Size(119, 21);
            this.labelSalesTaxGroup.TabIndex = 5;
            this.labelSalesTaxGroup.Text = "Sales tax group:";
            // 
            // labelPhone
            // 
            this.labelPhone.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelPhone, 2);
            this.labelPhone.Location = new System.Drawing.Point(3, 248);
            this.labelPhone.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelPhone.Name = "labelPhone";
            this.labelPhone.Size = new System.Drawing.Size(116, 21);
            this.labelPhone.TabIndex = 9;
            this.labelPhone.Text = "Phone number:";
            // 
            // labelEmail
            // 
            this.labelEmail.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelEmail, 2);
            this.labelEmail.Location = new System.Drawing.Point(3, 308);
            this.labelEmail.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelEmail.Name = "labelEmail";
            this.labelEmail.Size = new System.Drawing.Size(57, 21);
            this.labelEmail.TabIndex = 11;
            this.labelEmail.Text = "E-mail:";
            // 
            // labelWebSite
            // 
            this.labelWebSite.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelWebSite, 2);
            this.labelWebSite.Location = new System.Drawing.Point(3, 368);
            this.labelWebSite.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.labelWebSite.Name = "labelWebSite";
            this.labelWebSite.Size = new System.Drawing.Size(73, 21);
            this.labelWebSite.TabIndex = 15;
            this.labelWebSite.Text = "Web site:";
            // 
            // textBoxName
            // 
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxName, 2);
            this.textBoxName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.addressViewModelBindingSource, "AddressName", true));
            this.textBoxName.Location = new System.Drawing.Point(3, 53);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxName.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxName.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxName.Properties.Appearance.Options.UseFont = true;
            this.textBoxName.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxName.Properties.MaxLength = 20;
            this.textBoxName.Size = new System.Drawing.Size(400, 28);
            this.textBoxName.TabIndex = 1;
            // 
            // addressViewModelBindingSource
            // 
            this.addressViewModelBindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.Customer.ViewModels.AddressViewModel);
            // 
            // textBoxAddressType
            // 
            this.textBoxAddressType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxAddressType, 2);
            this.textBoxAddressType.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.addressViewModelBindingSource, "AddressType", true));
            this.textBoxAddressType.Location = new System.Drawing.Point(3, 115);
            this.textBoxAddressType.Name = "textBoxAddressType";
            this.textBoxAddressType.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxAddressType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxAddressType.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxAddressType.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxAddressType.Properties.Appearance.Options.UseFont = true;
            this.textBoxAddressType.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxAddressType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxAddressType.Properties.ReadOnly = true;
            this.textBoxAddressType.Size = new System.Drawing.Size(400, 28);
            this.textBoxAddressType.TabIndex = 4;
            // 
            // textBoxSalesTaxGroup
            // 
            this.textBoxSalesTaxGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxSalesTaxGroup, 2);
            this.textBoxSalesTaxGroup.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.addressViewModelBindingSource, "SalesTaxGroup", true));
            this.textBoxSalesTaxGroup.Location = new System.Drawing.Point(3, 179);
            this.textBoxSalesTaxGroup.Name = "textBoxSalesTaxGroup";
            this.textBoxSalesTaxGroup.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxSalesTaxGroup.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxSalesTaxGroup.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxSalesTaxGroup.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxSalesTaxGroup.Properties.Appearance.Options.UseFont = true;
            this.textBoxSalesTaxGroup.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxSalesTaxGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxSalesTaxGroup.Properties.ReadOnly = true;
            this.textBoxSalesTaxGroup.Size = new System.Drawing.Size(400, 28);
            this.textBoxSalesTaxGroup.TabIndex = 7;
            // 
            // textBoxPhone
            // 
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxPhone, 2);
            this.textBoxPhone.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.addressViewModelBindingSource, "Phone", true));
            this.textBoxPhone.Location = new System.Drawing.Point(3, 272);
            this.textBoxPhone.Name = "textBoxPhone";
            this.textBoxPhone.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxPhone.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPhone.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxPhone.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxPhone.Properties.Appearance.Options.UseFont = true;
            this.textBoxPhone.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxPhone.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxPhone.Properties.MaxLength = 20;
            this.textBoxPhone.Size = new System.Drawing.Size(400, 28);
            this.textBoxPhone.TabIndex = 10;
            // 
            // textBoxEmail
            // 
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxEmail, 2);
            this.textBoxEmail.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.addressViewModelBindingSource, "Email", true));
            this.textBoxEmail.Location = new System.Drawing.Point(3, 332);
            this.textBoxEmail.Name = "textBoxEmail";
            this.textBoxEmail.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxEmail.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxEmail.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxEmail.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxEmail.Properties.Appearance.Options.UseFont = true;
            this.textBoxEmail.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxEmail.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxEmail.Properties.MaxLength = 80;
            this.textBoxEmail.Size = new System.Drawing.Size(400, 28);
            this.textBoxEmail.TabIndex = 12;
            // 
            // textBoxWebSite
            // 
            this.tableLayoutPanelLeft.SetColumnSpan(this.textBoxWebSite, 2);
            this.textBoxWebSite.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.addressViewModelBindingSource, "Website", true));
            this.textBoxWebSite.Location = new System.Drawing.Point(3, 392);
            this.textBoxWebSite.Name = "textBoxWebSite";
            this.textBoxWebSite.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.textBoxWebSite.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxWebSite.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.textBoxWebSite.Properties.Appearance.Options.UseBackColor = true;
            this.textBoxWebSite.Properties.Appearance.Options.UseFont = true;
            this.textBoxWebSite.Properties.Appearance.Options.UseForeColor = true;
            this.textBoxWebSite.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.textBoxWebSite.Properties.MaxLength = 255;
            this.textBoxWebSite.Size = new System.Drawing.Size(400, 28);
            this.textBoxWebSite.TabIndex = 16;
            // 
            // labelContactInfo
            // 
            this.labelContactInfo.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelContactInfo, 2);
            this.labelContactInfo.Location = new System.Drawing.Point(3, 222);
            this.labelContactInfo.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.labelContactInfo.Name = "labelContactInfo";
            this.labelContactInfo.Size = new System.Drawing.Size(190, 21);
            this.labelContactInfo.TabIndex = 8;
            this.labelContactInfo.Text = "CONTACT INFORMATION";
            // 
            // labelGeneral
            // 
            this.labelGeneral.AutoSize = true;
            this.tableLayoutPanelLeft.SetColumnSpan(this.labelGeneral, 2);
            this.labelGeneral.Location = new System.Drawing.Point(3, 3);
            this.labelGeneral.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.labelGeneral.Name = "labelGeneral";
            this.labelGeneral.Size = new System.Drawing.Size(77, 21);
            this.labelGeneral.TabIndex = 17;
            this.labelGeneral.Text = "GENERAL";
            // 
            // btnAddressType
            // 
            this.btnAddressType.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddressType.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnAddressType.Appearance.Options.UseFont = true;
            this.btnAddressType.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnAddressType.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnAddressType.Location = new System.Drawing.Point(409, 113);
            this.btnAddressType.MaximumSize = new System.Drawing.Size(57, 32);
            this.btnAddressType.Name = "btnAddressType";
            this.btnAddressType.Padding = new System.Windows.Forms.Padding(3);
            this.btnAddressType.Size = new System.Drawing.Size(57, 32);
            this.btnAddressType.TabIndex = 3;
            this.btnAddressType.Text = "$";
            this.btnAddressType.Click += new System.EventHandler(this.OnAddressType_Click);
            // 
            // btnSalesTaxGroup
            // 
            this.btnSalesTaxGroup.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSalesTaxGroup.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnSalesTaxGroup.Appearance.Options.UseFont = true;
            this.btnSalesTaxGroup.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnSalesTaxGroup.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSalesTaxGroup.Location = new System.Drawing.Point(409, 177);
            this.btnSalesTaxGroup.MaximumSize = new System.Drawing.Size(57, 32);
            this.btnSalesTaxGroup.Name = "btnSalesTaxGroup";
            this.btnSalesTaxGroup.Padding = new System.Windows.Forms.Padding(3);
            this.btnSalesTaxGroup.Size = new System.Drawing.Size(57, 32);
            this.btnSalesTaxGroup.TabIndex = 6;
            this.btnSalesTaxGroup.Text = "$";
            this.btnSalesTaxGroup.Click += new System.EventHandler(this.OnSalesTaxGroup_Click);
            // 
            // frmNewShippingAddress
            // 
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.themedPanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmNewShippingAddress";
            this.Text = "Address";
            this.Controls.SetChildIndex(this.themedPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.themedPanel)).EndInit();
            this.themedPanel.ResumeLayout(false);
            this.tableLayoutPanelMain.ResumeLayout(false);
            this.tableLayoutPanelMain.PerformLayout();
            this.tableLayoutPanelBottom.ResumeLayout(false);
            this.tableLayoutPanelLeft.ResumeLayout(false);
            this.tableLayoutPanelLeft.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxName.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.addressViewModelBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxAddressType.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxSalesTaxGroup.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxPhone.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxEmail.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textBoxWebSite.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl themedPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelBottom;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSave;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private AddressUserControl addressUserControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelLeft;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelAddressType;
        private System.Windows.Forms.Label labelSalesTaxGroup;
        private System.Windows.Forms.Label labelPhone;
        private System.Windows.Forms.Label labelEmail;
        private System.Windows.Forms.Label labelWebSite;
        private DevExpress.XtraEditors.TextEdit textBoxName;
        private DevExpress.XtraEditors.TextEdit textBoxAddressType;
        private DevExpress.XtraEditors.TextEdit textBoxSalesTaxGroup;
        private DevExpress.XtraEditors.TextEdit textBoxPhone;
        private DevExpress.XtraEditors.TextEdit textBoxEmail;
        private DevExpress.XtraEditors.TextEdit textBoxWebSite;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddressType;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSalesTaxGroup;
        private System.Windows.Forms.Label labelContactInfo;
        private System.Windows.Forms.BindingSource addressViewModelBindingSource;
        private System.Windows.Forms.Label labelHeading;
        private System.Windows.Forms.Label labelGeneral;
    }
}