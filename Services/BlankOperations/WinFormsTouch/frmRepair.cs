
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using Microsoft.Dynamics.Retail.Pos.Customer;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
//using GenCode128;
using System.IO;
using Microsoft.Reporting.WinForms;
//using OnBarcode.Barcode;
#region Add new namespace by palas on 08-12-2014
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;

using Microsoft.Dynamics.Retail.Pos.BlankOperations;
using LSRetailPosis.Transaction;
#endregion
namespace BlankOperations
{
    public partial class frmRepair : frmTouchBase
    {
        public System.ComponentModel.IContainer components = null;

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
        public void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPrint = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnVoid = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearchOrder = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnCustomerSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSave = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnOrderDetails = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAddCustomer = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSM = new System.Windows.Forms.Button();
            this.txtSM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRepairBagNo = new System.Windows.Forms.TextBox();
            this.lblRepairId = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbRepairType = new System.Windows.Forms.ComboBox();
            this.dtPickerDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.lblDeliveryDate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.txtOrderNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.txtCustomerAccount = new System.Windows.Forms.TextBox();
            this.lblCustomerAccount = new System.Windows.Forms.Label();
            this.dTPickerOrderDate = new System.Windows.Forms.DateTimePicker();
            this.lblOrderDate = new System.Windows.Forms.Label();
            this.lblOrderNo = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.pnlMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMain.Controls.Add(this.tableLayoutPanel1);
            this.pnlMain.Controls.Add(this.tableLayoutPanel);
            this.pnlMain.Controls.Add(this.panel2);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Location = new System.Drawing.Point(6, 6);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(710, 396);
            this.pnlMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.tableLayoutPanel1.Controls.Add(this.btnPrint, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnVoid, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnSearchOrder, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(357, 224);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.71428F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.14286F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 32.14286F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(345, 166);
            this.tableLayoutPanel1.TabIndex = 35;
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.ForeColor = System.Drawing.Color.Red;
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.Appearance.Options.UseForeColor = true;
            this.btnPrint.Location = new System.Drawing.Point(4, 115);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(164, 46);
            this.btnPrint.TabIndex = 172;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Location = new System.Drawing.Point(176, 62);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(164, 46);
            this.btnClose.TabIndex = 33;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(4, 6);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(164, 46);
            this.btnClear.TabIndex = 34;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnVoid
            // 
            this.btnVoid.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnVoid.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVoid.Appearance.Options.UseFont = true;
            this.btnVoid.Location = new System.Drawing.Point(4, 62);
            this.btnVoid.Name = "btnVoid";
            this.btnVoid.Size = new System.Drawing.Size(164, 46);
            this.btnVoid.TabIndex = 32;
            this.btnVoid.Text = "Void";
            // 
            // btnSearchOrder
            // 
            this.btnSearchOrder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchOrder.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchOrder.Appearance.Options.UseFont = true;
            this.btnSearchOrder.Location = new System.Drawing.Point(176, 6);
            this.btnSearchOrder.Name = "btnSearchOrder";
            this.btnSearchOrder.Size = new System.Drawing.Size(164, 46);
            this.btnSearchOrder.TabIndex = 35;
            this.btnSearchOrder.Text = "Search Order";
            this.btnSearchOrder.Click += new System.EventHandler(this.btnSearchOrder_Click);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Controls.Add(this.btnCustomerSearch, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.btnSave, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.btnOrderDetails, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.btnAddCustomer, 1, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(5, 224);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(345, 167);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // btnCustomerSearch
            // 
            this.btnCustomerSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCustomerSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerSearch.Appearance.Options.UseFont = true;
            this.btnCustomerSearch.Location = new System.Drawing.Point(4, 7);
            this.btnCustomerSearch.Name = "btnCustomerSearch";
            this.btnCustomerSearch.Size = new System.Drawing.Size(164, 70);
            this.btnCustomerSearch.TabIndex = 28;
            this.btnCustomerSearch.Text = "Customer Search";
            this.btnCustomerSearch.Click += new System.EventHandler(this.btnCustomerSearch_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSave.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Appearance.Options.UseFont = true;
            this.btnSave.Location = new System.Drawing.Point(176, 90);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(164, 70);
            this.btnSave.TabIndex = 34;
            this.btnSave.Text = "Save and Close";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOrderDetails
            // 
            this.btnOrderDetails.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOrderDetails.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOrderDetails.Appearance.Options.UseFont = true;
            this.btnOrderDetails.Location = new System.Drawing.Point(4, 90);
            this.btnOrderDetails.Name = "btnOrderDetails";
            this.btnOrderDetails.Size = new System.Drawing.Size(164, 70);
            this.btnOrderDetails.TabIndex = 30;
            this.btnOrderDetails.Text = "Order Details";
            this.btnOrderDetails.Click += new System.EventHandler(this.btnOrderDetails_Click);
            // 
            // btnAddCustomer
            // 
            this.btnAddCustomer.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddCustomer.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddCustomer.Appearance.Options.UseFont = true;
            this.btnAddCustomer.Location = new System.Drawing.Point(175, 7);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(165, 70);
            this.btnAddCustomer.TabIndex = 29;
            this.btnAddCustomer.Text = "Add Customer";
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnSM);
            this.panel2.Controls.Add(this.txtSM);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.txtRepairBagNo);
            this.panel2.Controls.Add(this.lblRepairId);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.cmbRepairType);
            this.panel2.Controls.Add(this.dtPickerDeliveryDate);
            this.panel2.Controls.Add(this.lblDeliveryDate);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtTotalAmount);
            this.panel2.Controls.Add(this.txtPhoneNumber);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtCustomerAddress);
            this.panel2.Controls.Add(this.lblCustomerAddress);
            this.panel2.Controls.Add(this.txtOrderNo);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtCustomerName);
            this.panel2.Controls.Add(this.txtCustomerAccount);
            this.panel2.Controls.Add(this.lblCustomerAccount);
            this.panel2.Controls.Add(this.dTPickerOrderDate);
            this.panel2.Controls.Add(this.lblOrderDate);
            this.panel2.Controls.Add(this.lblOrderNo);
            this.panel2.Location = new System.Drawing.Point(5, 42);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(697, 176);
            this.panel2.TabIndex = 3;
            // 
            // btnSM
            // 
            this.btnSM.Location = new System.Drawing.Point(589, 5);
            this.btnSM.Name = "btnSM";
            this.btnSM.Size = new System.Drawing.Size(99, 23);
            this.btnSM.TabIndex = 180;
            this.btnSM.Text = "Search Sales man";
            this.btnSM.UseVisualStyleBackColor = true;
            this.btnSM.Click += new System.EventHandler(this.btnSM_Click);
            // 
            // txtSM
            // 
            this.txtSM.BackColor = System.Drawing.SystemColors.Control;
            this.txtSM.Enabled = false;
            this.txtSM.Location = new System.Drawing.Point(459, 6);
            this.txtSM.MaxLength = 20;
            this.txtSM.Name = "txtSM";
            this.txtSM.ReadOnly = true;
            this.txtSM.Size = new System.Drawing.Size(127, 21);
            this.txtSM.TabIndex = 179;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(336, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 178;
            this.label6.Text = "Sales man:";
            // 
            // txtRepairBagNo
            // 
            this.txtRepairBagNo.BackColor = System.Drawing.SystemColors.Window;
            this.txtRepairBagNo.Location = new System.Drawing.Point(459, 101);
            this.txtRepairBagNo.MaxLength = 25;
            this.txtRepairBagNo.Name = "txtRepairBagNo";
            this.txtRepairBagNo.Size = new System.Drawing.Size(229, 21);
            this.txtRepairBagNo.TabIndex = 14;
            // 
            // lblRepairId
            // 
            this.lblRepairId.AutoSize = true;
            this.lblRepairId.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lblRepairId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRepairId.Location = new System.Drawing.Point(336, 104);
            this.lblRepairId.Name = "lblRepairId";
            this.lblRepairId.Size = new System.Drawing.Size(75, 13);
            this.lblRepairId.TabIndex = 177;
            this.lblRepairId.Text = "Repair Bag No";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(336, 80);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 175;
            this.label4.Text = "Repair Type:";
            // 
            // cmbRepairType
            // 
            this.cmbRepairType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRepairType.FormattingEnabled = true;
            this.cmbRepairType.Location = new System.Drawing.Point(459, 77);
            this.cmbRepairType.Name = "cmbRepairType";
            this.cmbRepairType.Size = new System.Drawing.Size(137, 21);
            this.cmbRepairType.TabIndex = 174;
            // 
            // dtPickerDeliveryDate
            // 
            this.dtPickerDeliveryDate.Location = new System.Drawing.Point(459, 54);
            this.dtPickerDeliveryDate.Name = "dtPickerDeliveryDate";
            this.dtPickerDeliveryDate.Size = new System.Drawing.Size(229, 21);
            this.dtPickerDeliveryDate.TabIndex = 21;
            this.dtPickerDeliveryDate.ValueChanged += new System.EventHandler(this.dtPickerDeliveryDate_ValueChanged);
            // 
            // lblDeliveryDate
            // 
            this.lblDeliveryDate.AutoSize = true;
            this.lblDeliveryDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDeliveryDate.Location = new System.Drawing.Point(336, 56);
            this.lblDeliveryDate.Name = "lblDeliveryDate";
            this.lblDeliveryDate.Size = new System.Drawing.Size(125, 13);
            this.lblDeliveryDate.TabIndex = 20;
            this.lblDeliveryDate.Text = "Tentative Delivery Date:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(336, 128);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Total Amount:";
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtTotalAmount.Enabled = false;
            this.txtTotalAmount.Location = new System.Drawing.Point(459, 125);
            this.txtTotalAmount.MaxLength = 60;
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.Size = new System.Drawing.Size(229, 21);
            this.txtTotalAmount.TabIndex = 19;
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPhoneNumber.Enabled = false;
            this.txtPhoneNumber.Location = new System.Drawing.Point(113, 144);
            this.txtPhoneNumber.MaxLength = 20;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.ReadOnly = true;
            this.txtPhoneNumber.Size = new System.Drawing.Size(211, 21);
            this.txtPhoneNumber.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(6, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Phone Number:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAddress.Enabled = false;
            this.txtCustomerAddress.Location = new System.Drawing.Point(113, 77);
            this.txtCustomerAddress.MaxLength = 20;
            this.txtCustomerAddress.Multiline = true;
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCustomerAddress.Size = new System.Drawing.Size(211, 64);
            this.txtCustomerAddress.TabIndex = 15;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAddress.Location = new System.Drawing.Point(6, 80);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAddress.TabIndex = 14;
            this.lblCustomerAddress.Text = "Customer Address:";
            // 
            // txtOrderNo
            // 
            this.txtOrderNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtOrderNo.Enabled = false;
            this.txtOrderNo.Location = new System.Drawing.Point(113, 6);
            this.txtOrderNo.MaxLength = 20;
            this.txtOrderNo.Name = "txtOrderNo";
            this.txtOrderNo.ReadOnly = true;
            this.txtOrderNo.Size = new System.Drawing.Size(211, 21);
            this.txtOrderNo.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(6, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Customer Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(113, 54);
            this.txtCustomerName.MaxLength = 60;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerName.TabIndex = 11;
            // 
            // txtCustomerAccount
            // 
            this.txtCustomerAccount.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAccount.Enabled = false;
            this.txtCustomerAccount.Location = new System.Drawing.Point(113, 30);
            this.txtCustomerAccount.MaxLength = 20;
            this.txtCustomerAccount.Name = "txtCustomerAccount";
            this.txtCustomerAccount.ReadOnly = true;
            this.txtCustomerAccount.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerAccount.TabIndex = 9;
            // 
            // lblCustomerAccount
            // 
            this.lblCustomerAccount.AutoSize = true;
            this.lblCustomerAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAccount.Location = new System.Drawing.Point(6, 32);
            this.lblCustomerAccount.Name = "lblCustomerAccount";
            this.lblCustomerAccount.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAccount.TabIndex = 8;
            this.lblCustomerAccount.Text = "Customer Account:";
            // 
            // dTPickerOrderDate
            // 
            this.dTPickerOrderDate.Enabled = false;
            this.dTPickerOrderDate.Location = new System.Drawing.Point(459, 30);
            this.dTPickerOrderDate.Name = "dTPickerOrderDate";
            this.dTPickerOrderDate.Size = new System.Drawing.Size(229, 21);
            this.dTPickerOrderDate.TabIndex = 5;
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderDate.Location = new System.Drawing.Point(336, 32);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(68, 13);
            this.lblOrderDate.TabIndex = 4;
            this.lblOrderDate.Text = "Repair Date:";
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.AutoSize = true;
            this.lblOrderNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderNo.Location = new System.Drawing.Point(6, 9);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(62, 13);
            this.lblOrderNo.TabIndex = 2;
            this.lblOrderNo.Text = "Repair No.:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(240, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(197, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Ornament Repair";
            // 
            // frmRepair
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 404);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMain);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmRepair";
            this.ShowIcon = false;
            this.Text = "Ornament Repair";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlMain;
        public System.Windows.Forms.Label lblTitle;
        public System.Windows.Forms.Panel panel2;
        //  public System.Windows.Forms.DateTimePicker dtPickerDeliveryDate;
        // public System.Windows.Forms.Label lblDeliveryDate;
        public System.Windows.Forms.TextBox txtOrderNo;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        public System.Windows.Forms.Label lblCustomerAccount;
        public System.Windows.Forms.DateTimePicker dTPickerOrderDate;
        public System.Windows.Forms.Label lblOrderDate;
        public System.Windows.Forms.Label lblOrderNo;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchOrder;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnVoid;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddCustomer;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCustomerSearch;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOrderDetails;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSave;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        public System.Windows.Forms.TextBox txtCustomerAddress;
        public System.Windows.Forms.Label lblCustomerAddress;
        public System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtTotalAmount;
        public System.Windows.Forms.TextBox txtPhoneNumber;
        public System.Windows.Forms.Label label3;


        #region Variable Declaration
        public string CustAddress { get; set; }
        public string CustPhoneNo { get; set; }
        public IPosTransaction pos { get; set; }
        public DataTable dtOrderDetails = new DataTable("dtOrderDetails");
        public DataTable dtSampleDetails = new DataTable("dtSampleDetails");
        public DataTable dtSubOrderDetails = new DataTable("dtSubOrderDetails");
        public DataTable dtSketchDetails = new DataTable("dtSketchDetails");

        public string sOrderDetailsAmount = string.Empty;
        public string sSubOrderDetailsAmount = string.Empty;
        public string sCustOrderSearchNumber = string.Empty;
        public DataSet dsOrderSearched = new DataSet();

        public bool bDataSaved = false;
        public string sCustAcc = string.Empty;
        public string sCustOrder = string.Empty;
        public string sTotalAmt = string.Empty;
        public DateTimePicker dtPickerDeliveryDate;
        public Label lblDeliveryDate;
        string sCurrencySymbol = "";
        string sAmtinwds = "";
        string sBagNo = "";
        string sEmail = "";
        string sCompanyName = string.Empty;
        string sCustOrderReceiptDate = "";

        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations oBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        public DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                 LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
                 LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
        private Label label4;
        private ComboBox cmbRepairType;
        public TextBox txtRepairBagNo;
        private Label lblRepairId;
        private Button btnSM;
        public TextBox txtSM;
        private Label label6;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPrint;
        [Import]
        public IApplication application;


        enum RepairType
        {
            None = 0,
            HO_Repair = 1,
            Store_Repair = 2,
        }

        #endregion

        #region Initialization
        public frmRepair(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(726, 420);
            pos = posTransaction;
            application = Application;
            txtOrderNo.Text = GetOrderNum();
            btnCustomerSearch.Focus();

            cmbRepairType.DataSource = Enum.GetValues(typeof(RepairType));

        }
        #endregion

        #region GetOrderNum()
        public string GetOrderNum()
        {

            string OrderNum = string.Empty;
            OrderNum = GetNextRepairOrderID();
            return OrderNum;
        }
        #endregion

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum ReceiptTransactionType
        {
            // CustomerGoldOrder = 8
            OrnamentRepair = 9
        }

        public string GetNextRepairOrderID()
        {
            try
            {
                // ReceiptTransactionType transType = ReceiptTransactionType.CustomerGoldOrder;

                ReceiptTransactionType transType = ReceiptTransactionType.OrnamentRepair;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                orderNumber((int)transType, funcProfileId, out mask);
                if (string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }
        #endregion

        #region GetOrderNum()  - CHANGED BY NIMBUS
        public void orderNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }



        }

        #endregion

        #region GetSeedVal() - CHANGED BY NIMBUS
        public string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.OrnamentRepair;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                // string queryString = " SELECT  MAX(CAST(ISNULL(SUBSTRING(CUSTORDER_HEADER.ORDERNUM,3,LEN(CUSTORDER_HEADER.ORDERNUM)),0) AS INTEGER)) + 1 from CUSTORDER_HEADER ";

                //string queryString = " SELECT  MAX(CAST(ISNULL(SUBSTRING(RetailRepairHdr.RepairId,5,LEN(RetailRepairHdr.RepairId)),0) AS INTEGER)) + 1 from RetailRepairHdr ";
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(RepairId,@VAL,LEN(RepairId)),0) AS INTEGER)) + 1 from RetailRepairHdr";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());

                    if (!string.IsNullOrEmpty(Val))
                        return Val;
                    else
                        return "1";

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }



        }

        #endregion

        #region Customer Button Click
        public void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);
            obfrm.ShowDialog();
        }
        #endregion

        #region Click Order Details
        public void btnOrderDetails_Click(object sender, EventArgs e)
        {
            BlankOperations.WinFormsTouch.frmRepairDetail objOrderdetails = null;
            if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            {
                objOrderdetails = new BlankOperations.WinFormsTouch.frmRepairDetail(dtSubOrderDetails, dtOrderDetails, pos, application, this, dtSketchDetails);
            }

            else if (dsOrderSearched != null && dsOrderSearched.Tables.Count > 0 && dsOrderSearched.Tables[1].Rows.Count > 0)
            {
                objOrderdetails = new BlankOperations.WinFormsTouch.frmRepairDetail(dsOrderSearched, pos, application, this);
            }
            else
            {
                dtOrderDetails = new DataTable();
                objOrderdetails = new BlankOperations.WinFormsTouch.frmRepairDetail(pos, application, this);
            }


            objOrderdetails.ShowDialog();
            txtTotalAmount.Text = sOrderDetailsAmount;
        }
        #endregion

        #region ADD NEW CUSTOMER
        public void btnAddCustomer_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.Customer obj = new Microsoft.Dynamics.Retail.Pos.Customer.Customer();
            string strCustId = string.Empty;
            string strCustName = string.Empty;
            string strCustCurrency = string.Empty;

            obj.AddNew(out strCustId, out  strCustName, out strCustCurrency);
            {
                txtCustomerAccount.Text = strCustId;
                txtCustomerName.Text = strCustName;
            }
        }
        #endregion

        #region Save Click
        public void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtOrderNo.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order Number is not defined.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                return;
            }
            if (isValid())
            {
                SaveOrder();
            }
        }
        #endregion

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pcs = 1,
            Tot = 2,
        }

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion

        #endregion

        #region SaveFuction
        public void SaveOrder()
        {
            int iCustOrder_Header = 0;
            int iCustOrder_Details = 0;
            int iCustOrder_SubDetails = 0;
            // int iCustOrder_SampleDetails = 0;
            //   int iSubOrder_LineNum = 0;

            SqlTransaction transaction = null;

            #region CUSTOMER ORDER HEADER

            // string commandText = " INSERT INTO [CUSTORDER_HEADER]([ORDERNUM],[STOREID],[TERMINALID],[ORDERDATE],[DELIVERYDATE],[CUSTACCOUNT],[CUSTNAME],[CUSTADDRESS],[CUSTPHONE] " +
            //                      " ,[DATAAREAID],[STAFFID],[TOTALAMOUNT])  VALUES(@ORDERNUM,@STOREID,@TERMINALID,@ORDERDATE,@DELIVERYDATE,@CUSTACCOUNT,@CUSTNAME,@CUSTADDRESS,@CUSTPHONE,@DATAAREAID,@STAFFID,@TOTALAMOUNT)";


            string commandText = " INSERT INTO [RetailRepairHdr]([RepairId],[RetailStoreId],[RetailTerminalId]," +
                                 " [ORDERDATE],[DELIVERYDATE],[CUSTACCOUNT],[CUSTNAME],[CUSTADDRESS],[CUSTPHONE] " +
                                  " ,[DATAAREAID],[RetailStaffId],RepairType,REPAIRBAGNO,SalesManID) " +
                                  " VALUES(@ORDERNUM,@STOREID,@TERMINALID,@ORDERDATE,@DELIVERYDATE,@CUSTACCOUNT," +
                                  " @CUSTNAME,@CUSTADDRESS,@CUSTPHONE," +
                                  " @DATAAREAID,@STAFFID,@RepairType,@REPAIRBAGNO,@SalesManID)";

            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar).Value = txtOrderNo.Text.Trim();
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@ORDERDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                command.Parameters.Add("@DELIVERYDATE", SqlDbType.DateTime).Value = dtPickerDeliveryDate.Value;
                command.Parameters.Add("@CUSTACCOUNT", SqlDbType.NVarChar, 20).Value = txtCustomerAccount.Text.Trim();
                command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 60).Value = txtCustomerName.Text.Trim();
                command.Parameters.Add("@CUSTADDRESS", SqlDbType.NVarChar, 250).Value = CustAddress == null ? string.Empty : CustAddress;
                if (string.IsNullOrEmpty(CustPhoneNo))
                    command.Parameters.Add("@CUSTPHONE", SqlDbType.Decimal).Value = DBNull.Value;
                else
                    command.Parameters.Add(new SqlParameter("@CUSTPHONE", CustPhoneNo));

                if (application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                command.Parameters.Add("@RepairType", SqlDbType.Int).Value = cmbRepairType.SelectedIndex;
                //   command.Parameters.Add("@TOTALAMOUNT", SqlDbType.Decimal, 250).Value = sOrderDetailsAmount;
                command.Parameters.Add("@REPAIRBAGNO", SqlDbType.NVarChar, 20).Value = txtRepairBagNo.Text.Trim();
                command.Parameters.Add("@SalesManID", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtSM.Tag) == null ? string.Empty : Convert.ToString(txtSM.Tag);

            #endregion

                command.CommandTimeout = 0;
                iCustOrder_Header = command.ExecuteNonQuery();

                if (iCustOrder_Header == 1)
                {
                    if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                    {
                        #region ORDER DETAILS


                        /*
                        string commandCustOrder_Detail = " INSERT INTO [CUSTORDER_DETAILS]([ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID] " +
                                                 " ,[CODE],[SIZEID],[STYLE],[PCS],[QTY],[CRATE],[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE] " +
                                                 " ,[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[STAFFID],[INVENTDIMID],[UNITID]) " +
                                                 " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID ,@CONFIGID,@CODE ,@SIZEID,@STYLE,@PCS ,@QTY " +
                                                 " ,@RATE ,@RATETYPE,@AMOUNT  ,@MAKINGRATE ,@MAKINGRATETYPE,@MAKINGAMOUNT,@EXTENDEDDETAILSAMOUNT " +
                                                 " ,@DATAAREAID  ,@STAFFID , @INVENTDIMID, @UNITID) ";
                        */


                        string commandCustOrder_Detail = " INSERT INTO [RetailRepairDetail]([RepairId],[LINENUM],[RetailStoreId]" +
                                                         " ,[RetailTerminalId],[ITEMID],[PCS],[QTY],[AMOUNT] " +
                                                         " ,[DATAAREAID],[RetailStaffId],[INVENTDIMID],[UNITID],REPAIRJOBDETAILS  " +
                                                         " ,CONFIGID,CODE,SIZEID,STYLE,NETTWT,DIAWT,DIAAMT,STNWT," +
                                                         " STNAMT,TOTALAMT,JOBTYPE,BatchId,ARTICLECODE,GROSSQTY )" +
                                                         " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID,@PCS,@QTY,@AMOUNT" +
                                                         " ,@DATAAREAID,@STAFFID,@INVENTDIMID,@UNITID,@REPAIRJOBDETAILS " +
                                                         ",@CONFIGID,@CODE,@SIZEID,@STYLE,@NETTWT,@DIAWT,@DIAAMT,@STNWT" +
                                                         " ,@STNAMT,@TOTALAMT,@JOBTYPE,@BatchId,@ARTICLECODE,@GROSSQTY )";


                        if (cmbRepairType.SelectedIndex == (int)RepairType.HO_Repair)
                        {
                            commandCustOrder_Detail += " INSERT INTO [RetailRepairTransaction]([RepairId],[LINENUM],[RetailStoreId]" +
                                                " ,[RetailTerminalId],[ITEMID],[PCS],[QTY] " +
                                                " ,[DATAAREAID],[RetailStaffId],[INVENTDIMID],[UNITID]  " +
                                                " ,CONFIGID,CODE,SIZEID,STYLE,NETTWT,DIAWT,DIAAMT,STNWT,STNAMT,BatchId,ARTICLECODE )" +
                                                " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID,@PCS,@QTY" +
                                                " ,@DATAAREAID,@STAFFID,@INVENTDIMID,@UNITID " +
                                                ",@CONFIGID,@CODE,@SIZEID,@STYLE,@NETTWT,@DIAWT," +
                                                " @DIAAMT,@STNWT,@STNAMT,@BatchId,@ARTICLECODE )";

                        }

                        for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                        {
                            string sBatchId = txtOrderNo.Text.Trim() + "" + (ItemCount + 1);

                            SqlCommand cmdCustOrder_Detail = new SqlCommand(commandCustOrder_Detail, connection, transaction);
                            cmdCustOrder_Detail.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                            cmdCustOrder_Detail.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = ItemCount + 1;
                            cmdCustOrder_Detail.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdCustOrder_Detail.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdCustOrder_Detail.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["PCS"])))
                                cmdCustOrder_Detail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["PCS"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["QUANTITY"])))
                                cmdCustOrder_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["QUANTITY"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["AMOUNT"])))
                                cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["AMOUNT"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["REPAIRJOBDETAILS"])))
                                cmdCustOrder_Detail.Parameters.Add("@REPAIRJOBDETAILS", SqlDbType.NVarChar, 250).Value = string.Empty;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@REPAIRJOBDETAILS", SqlDbType.NVarChar, 250).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["REPAIRJOBDETAILS"]);

                            if (application != null)
                                cmdCustOrder_Detail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                            cmdCustOrder_Detail.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["INVENTDIMID"])))
                                cmdCustOrder_Detail.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdCustOrder_Detail.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["INVENTDIMID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNITID"])))
                                cmdCustOrder_Detail.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdCustOrder_Detail.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNITID"]);

                            cmdCustOrder_Detail.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["CONFIGURATION"]);
                            cmdCustOrder_Detail.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["COLOR"]);
                            cmdCustOrder_Detail.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["SIZE"]);
                            cmdCustOrder_Detail.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["STYLE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["NETTWT"])))
                                cmdCustOrder_Detail.Parameters.Add("@NETTWT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.AddWithValue("@NETTWT", Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["NETTWT"]));

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["DIAWT"])))
                                cmdCustOrder_Detail.Parameters.Add("@DIAWT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.AddWithValue("@DIAWT", Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["DIAWT"]));

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["DIAAMT"])))
                                cmdCustOrder_Detail.Parameters.Add("@DIAAMT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.AddWithValue("@DIAAMT", Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["DIAAMT"]));

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["STNWT"])))
                                cmdCustOrder_Detail.Parameters.Add("@STNWT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.AddWithValue("@STNWT", Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["STNWT"]));

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["STNAMT"])))
                                cmdCustOrder_Detail.Parameters.Add("@STNAMT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.AddWithValue("@STNAMT", Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["STNAMT"]));

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["TOTALAMT"])))
                                cmdCustOrder_Detail.Parameters.Add("@TOTALAMT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.AddWithValue("@TOTALAMT", Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["TOTALAMT"]));

                            cmdCustOrder_Detail.Parameters.AddWithValue("@JOBTYPE", dtOrderDetails.Rows[ItemCount]["JOBTYPE"]);

                            cmdCustOrder_Detail.Parameters.AddWithValue("@BatchId", sBatchId);

                            cmdCustOrder_Detail.Parameters.AddWithValue("@ARTICLECODE", dtOrderDetails.Rows[ItemCount]["ARTICLE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["GROSSQTY"])))
                                cmdCustOrder_Detail.Parameters.Add("@GROSSQTY", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@GROSSQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["GROSSQTY"]);


                            cmdCustOrder_Detail.CommandTimeout = 0;
                            iCustOrder_Details = cmdCustOrder_Detail.ExecuteNonQuery();
                            cmdCustOrder_Detail.Dispose();
                        #endregion

                            if (iCustOrder_Details > 0)
                            {

                                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                                {
                                    #region SUB ORDER DETAILS

                                    string commandCust_SubOrderDetails = " INSERT INTO [RetailRepairSubDetail]([RepairId],[LINENUMDtl],[LINENUM],[RetailStoreId],[RetailTerminalId],[ITEMID] "
                                                                        + " ,[PCS],[QTY],[DATAAREAID],[AMOUNT],[RetailStaffId],[INVENTDIMID],[UNITID],[CONFIGID],[CODE], " +
                                                                          " [SIZEID],[STYLE]) VALUES (@ORDERNUM,@ORDERDETAILNUM,@LINENUM, " +
                                                                          " @STOREID,@TERMINALID,@ITEMID,@PCS,@QTY,@DATAAREAID,@AMOUNT,@STAFFID,@INVENTDIMID,@UNITID,@CONFIGID" +
                                                                          ",@CODE,@SIZEID,@STYLE) ";

                                    int i = 1;
                                    int index = 1;
                                    for (int PaymentCount = 0; PaymentCount < dtSubOrderDetails.Rows.Count; PaymentCount++)
                                    {
                                        if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNIQUEID"]).Trim() == Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["UNIQUEID"]).Trim())
                                        {
                                            index = i;

                                            SqlCommand cmdOGP_PAYMENT = new SqlCommand(commandCust_SubOrderDetails, connection, transaction);
                                            cmdOGP_PAYMENT.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                                            cmdOGP_PAYMENT.Parameters.Add("@ORDERDETAILNUM", SqlDbType.NVarChar, 20).Value = ItemCount + 1;
                                            cmdOGP_PAYMENT.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = index;
                                            cmdOGP_PAYMENT.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                            cmdOGP_PAYMENT.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["ITEMID"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                            cmdOGP_PAYMENT.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["CONFIGURATION"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["COLOR"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["SIZE"]);
                                            cmdOGP_PAYMENT.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["STYLE"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["PCS"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@PCS", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["PCS"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["QUANTITY"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["QUANTITY"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["AMOUNT"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["AMOUNT"]);

                                            if (application != null)
                                                cmdOGP_PAYMENT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                                            cmdOGP_PAYMENT.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["INVENTDIMID"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = string.Empty;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = dtSubOrderDetails.Rows[PaymentCount]["INVENTDIMID"];

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["UNITID"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = string.Empty;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = dtSubOrderDetails.Rows[PaymentCount]["UNITID"];


                                            cmdOGP_PAYMENT.CommandTimeout = 0;
                                            iCustOrder_SubDetails = cmdOGP_PAYMENT.ExecuteNonQuery();
                                            cmdOGP_PAYMENT.Dispose();
                                            i++;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }

                        #region SAMPLE DETAILS
                        /*
                        if (dtSampleDetails != null && dtSampleDetails.Rows.Count > 0)
                        {

                            string commandCustOrder_SampleDetail = " INSERT INTO [CUSTORDER_SAMPLE] ([ORDERNUM],[LINENUM],[RETAILSTOREID],[RETAILTERMINALID],[RETAILDATAAREAID] " +
                           " ,[RETAILSTAFFID],[ARTICLE],[APPROXWT],[APPROXVAL],[AAPROXDIAMONDWT],[APPROXDIAMONDVAL],[APPROXGOLDWT] " +
                           " ,[APPROXGOLDVAL],[APPROXOVERALLWT],[APPROXOVERALLVAL]) " +
                           " VALUES (@ORDERNUM, @LINENUM, @RETAILSTOREID,@RETAILTERMINALID,@RETAILDATAAREAID,@RETAILSTAFFID,@ARTICLE,@APPROXWT " +
                           " ,@APPROXVAL,@AAPROXDIAMONDWT,@APPROXDIAMONDVAL,@APPROXGOLDWT,@APPROXGOLDVAL,@APPROXOVERALLWT,@APPROXOVERALLVAL) ";

                            for (int SampleCount = 0; SampleCount < dtSampleDetails.Rows.Count; SampleCount++)
                            {
                                SqlCommand cmdCustOrder_SampleDetail = new SqlCommand(commandCustOrder_SampleDetail, connection, transaction);
                                cmdCustOrder_SampleDetail.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                                cmdCustOrder_SampleDetail.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["LINENUM"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                cmdCustOrder_SampleDetail.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                cmdCustOrder_SampleDetail.Parameters.Add("@RETAILDATAAREAID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Database.DATAAREAID;
                                cmdCustOrder_SampleDetail.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                                cmdCustOrder_SampleDetail.Parameters.Add("@ARTICLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["ARTICLEID"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXWT", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXWT"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXVAL", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXVAL"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@AAPROXDIAMONDWT", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXDIAMONDWT"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXDIAMONDVAL", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXDIAMONDVAL"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXGOLDWT", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXGOLDWT"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXGOLDVAL", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXGOLDVAL"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXOVERALLWT", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXOVERALLWT"]);
                                cmdCustOrder_SampleDetail.Parameters.Add("@APPROXOVERALLVAL", SqlDbType.Decimal).Value = Convert.ToString(dtSampleDetails.Rows[SampleCount]["APPROXOVERALLVAL"]);

                                cmdCustOrder_SampleDetail.CommandTimeout = 0;
                                iCustOrder_SampleDetails = cmdCustOrder_SampleDetail.ExecuteNonQuery();
                                cmdCustOrder_SampleDetail.Dispose();
                            }
                        }
                        */
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();
                if (iCustOrder_SubDetails == 1 || iCustOrder_Details > 0)
                {
                    //start : image save into folder
                    NIM_OrderUpdateWithSketch();
                    //end : 

                    sCustAcc = txtCustomerAccount.Text;
                    sTotalAmt = txtTotalAmount.Text;

                    sAmtinwds = Amtinwds(Math.Abs(Convert.ToDouble(sTotalAmt)));

                    string sCustName = txtCustomerName.Text.Trim();

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                        try
                        {
                            PrintVoucher();
                        }
                        catch { }

                        sCustOrder = txtOrderNo.Text;
                        txtOrderNo.Text = GetOrderNum();
                        dtOrderDetails = new DataTable();
                        dtSubOrderDetails = new DataTable();
                        sOrderDetailsAmount = string.Empty;
                        sSubOrderDetailsAmount = string.Empty;
                        sCustOrderSearchNumber = string.Empty;
                        dsOrderSearched = new DataSet();

                        CLearControls();
                        bDataSaved = true;

                        #region Send SMS
                        // string sContactNo = oBlank.GetCustomerMobilePrimery(sCustAcc);                       

                        //Dear ##CUSTOMER##, Thank U for placing ##ORDER## @ ##COMPANY##. Kindly note your order no. ##ORD NO##. we will keep u updated on its status.
                        //oBlank.SendSMS("Dear " + oBlank.GetFirstName(sCustAcc) + "," + " Thank U for placing Repair order @Maganlal. Kindly note your order no." + sCustOrder + ". we will keep u updated on its status.", sContactNo);
                        #endregion

                        this.Close();

                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }

            }
        }
        #endregion

        private string Amtinwds(double amt)
        {
            MultiCurrency objMulC = null;
            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) != "INR")
                objMulC = new MultiCurrency(Criteria.Foreign);
            else
                objMulC = new MultiCurrency(Criteria.Indian);
            Color cBlack = Color.FromName("Black");

            return GetCurrencyNameWithCode() + " " + objMulC.ConvertToWord(Convert.ToString(amt));
        }

        private string GetCurrencyNameWithCode()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TXT FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        #region Print Voucher

        public void PrintVoucher(string sOrderNo, Int16 iCopy, RetailTransaction retailTransaction = null)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string sCompName = string.Empty;
            string sStoreName = string.Empty;
            string sStoreAddress = string.Empty;
            string sStorePhNo = string.Empty;
            string sGSTNo = string.Empty;
            string sStoreTaxState = "";
            string sInvoiceTime = "";


            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);

            if (Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber) != string.Empty)
                sGSTNo = Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber);

            string sCompanyName = GetCompanyName();
            //-------

            if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

            if (retailTransaction != null)
                sInvoiceTime = (retailTransaction).BeginDateTime.ToString("HH:mm");

            sCompName = GetCompanyName();
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo(sOrderNo)));
            rds.Add(new ReportDataSource("REPAIRDETAILINFO", (DataTable)GetDetailInfo(sOrderNo)));
            string sAmtinwds = Amtinwds(Math.Abs(Convert.ToDouble(getTotAmtOfOrder(sOrderNo)))); // added on 28/04/2014 RHossain               
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
           
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", "Repair Receipt", true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("cs", sCurrencySymbol, true));
            rps.Add(new ReportParameter("pPincode", sPincode)); // Added on 05-12-2014 Abhishek Dey
            rps.Add(new ReportParameter("Amtinwds", sAmtinwds));

            string reportName = @"rptRepairOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Repair Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();

            // Export(reportName.);
        }

        public DataTable GetHeaderInfo(string sOrderNo)
        {
            var SelectedCust = customerDataManager.GetTransactionalCustomer(txtCustomerAccount.Text);
           
            sPincode = SelectedCust.PostalCode;
            sEmail = SelectedCust.Email;

            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select [RepairId] ORDERNO,[RetailStoreId],[RetailTerminalId]," +
                                    " [ORDERDATE],[DELIVERYDATE],[CUSTACCOUNT] CUSTID,[CUSTNAME] CUSTNAME,[CUSTADDRESS] CUSTADD,[CUSTPHONE] " +
                                    " ,[DATAAREAID],[RetailStaffId],RepairType,REPAIRBAGNO RepairBagNo,SalesManID,'" + sEmail + "' as CustEmail," +
                                    " 'true' DELVERYDATEVISIBLE FROM RetailRepairHdr" +
                                    " WHERE RepairId='" + sOrderNo + "'";

                DataTable dtHeader = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtHeader);

                if (dtHeader.Rows.Count > 0)
                    sCustOrderReceiptDate = Convert.ToString(dtHeader.Rows[0]["ORDERDATE"]);

                return dtHeader;

               

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //
        public DataTable GetDetailInfo(string sOrderNo)
        {

            string sArchivePath = GetArchivePathFromImage();
            string sSaleItem = string.Empty;
            string sItemParentId = string.Empty;

            DataTable dtDetails = new DataTable();
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select [RepairId],[LINENUM],[RetailStoreId]" +
                                    " ,[RetailTerminalId],[ITEMID],[PCS],[QTY],[AMOUNT] " +
                                    " ,[DATAAREAID],[RetailStaffId],[INVENTDIMID],[UNITID],REPAIRJOBDETAILS JOBDETAILS  " +
                                    " ,CONFIGID,CODE,SIZEID,STYLE,NETTWT,DIAWT,DIAAMT,STNWT," +
                                    " STNAMT,TOTALAMT APPXVALUE,JOBTYPE,BatchId BATCHID,ARTICLECODE Article,GROSSQTY,'' ORDERLINEIMAGE from RetailRepairDetail" +
                                    " WHERE RepairId='" + sOrderNo + "'"; // SKUID for get the itemid for image selection of parent item id of that item


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtDetails);

                int i = 1;
                foreach (DataRow item in dtOrderDetails.Rows)
                {
                    DataRow dr = dtDetails.NewRow();
                    dr["ITEMID"] = item["ITEMID"];
                    //dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(item["PCS"])))
                        dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                    else
                        dr["PCS"] = 0;
                    //dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(item["QUANTITY"])))
                        dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
                    else
                        dr["QTY"] = 0;

                    dr["NETTWT"] = Convert.ToDecimal(item["NETTWT"]);
                    dr["APPXVALUE"] = Convert.ToDecimal(item["TOTALAMT"]);
                    dr["JOBTYPE"] = Convert.ToString(item["JOBTYPE"]);
                    dr["JOBDETAILS"] = Convert.ToString(item["REPAIRJOBDETAILS"]);
                    dr["TOTALAMOUNT"] = Convert.ToDecimal(item["RATE"]);

                    string path = sArchivePath + "" + txtOrderNo.Text + "_" + i + ".jpeg"; //

                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        byte[] arr;
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        dr["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                    }
                    else
                    {
                        sSaleItem = Convert.ToString(dr["ITEMID"]);
                        sItemParentId = GetItemParentId(Convert.ToString(dr["ITEMID"]));


                        path = sArchivePath + "" + sItemParentId + "" + ".jpg"; //

                        if (File.Exists(path))
                        {
                            Image img = Image.FromFile(path);
                            byte[] arr;
                            using (MemoryStream ms1 = new MemoryStream())
                            {
                                img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                                arr = ms1.ToArray();
                            }

                            dr["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                        }
                        else
                            dr["ORDERLINEIMAGE"] = "";

                    }
                    dr["BATCHID"] = txtOrderNo.Text + "" + i;
                    dr["Article"] = Convert.ToString(item["Article"]);

                    i++;
                    dtDetails.Rows.Add(dr);



                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return dtDetails;

        }


        private string getTotAmtOfOrder(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sCName = string.Empty;

            string sQry = "SELECT SUM(TOTALAMT) FROM RetailRepairDetail WHERE RepairId='" + sOrderNo + "'";

            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sCName;
        }

        private string GetCompanyName()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sCName = string.Empty;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            //}

            return sCName;

        }

        public void PrintVoucher()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            sCurrencySymbol = oBlank.GetCurrencySymbol();

            // PageSettings ps = new PageSettings { Landscape = false, PaperSize = new PaperSize { RawKind = (int)PaperKind.A4 }, Margins = new Margins { Top = 0, Right = 0, Bottom = 0, Left = 0 } };
            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo()));
            rds.Add(new ReportDataSource("REPAIRDETAILINFO", (DataTable)GetDetailInfo()));
            // rds.Add(new ReportDataSource("BARCODEIMGTABLE", (DataTable)null));//GetBarcodeInfo(txtOrderNo.Text))
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", "Repair Receipt", true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("cs", sCurrencySymbol, true));
            rps.Add(new ReportParameter("pPincode", sPincode)); // Added on 05-12-2014 Abhishek Dey
            rps.Add(new ReportParameter("Amtinwds", sAmtinwds));
            // rps.Add(new ReportParameter("Email", sBagNo));

            string reportName = @"rptRepairOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Repair Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();
        }

        public DataTable GetHeaderInfo()
        {
            DataTable dtHeader = new DataTable();
            dtHeader.Columns.Add("ORDERNO", typeof(string));
            dtHeader.Columns.Add("ORDERDATE", typeof(DateTime));
            dtHeader.Columns.Add("DELIVERYDATE", typeof(DateTime));
            dtHeader.Columns.Add("CUSTID", typeof(string));
            dtHeader.Columns.Add("CUSTNAME", typeof(string));
            dtHeader.Columns.Add("CUSTADD", typeof(string));
            dtHeader.Columns.Add("CUSTPHONE", typeof(string));
            dtHeader.Columns.Add("TOTALAMOUNT", typeof(decimal));
            dtHeader.Columns.Add("DELVERYDATEVISIBLE", typeof(bool));
            dtHeader.Columns.Add("RepairBagNo", typeof(string));
            dtHeader.Columns.Add("CustEmail", typeof(string));

            DataRow dr = dtHeader.NewRow();
            dr["ORDERNO"] = txtOrderNo.Text;
            dr["ORDERDATE"] = dTPickerOrderDate.Value;
            dr["DELIVERYDATE"] = dtPickerDeliveryDate.Value;
            dr["CUSTID"] = txtCustomerAccount.Text;

            #region Customer Address format Changed by Palas Jana @ 08-12-2014
            var SelectedCust = customerDataManager.GetTransactionalCustomer(txtCustomerAccount.Text);
            dr["CUSTNAME"] = txtCustomerName.Text;// Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.GetCustomerNameWithSalutation(SelectedCust);//txtCustomerName.Text;
            dr["CUSTADD"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.AddressLines(SelectedCust);//txtCustomerAddress.Text;
            sPincode = SelectedCust.PostalCode;
            sEmail = SelectedCust.Email;
            #endregion
            dr["CUSTPHONE"] = txtPhoneNumber.Text;
            dr["TOTALAMOUNT"] = Convert.ToDecimal(txtTotalAmount.Text);
            dr["DELVERYDATEVISIBLE"] = true;
            dr["RepairBagNo"] = txtRepairBagNo.Text;
            dr["CustEmail"] = sEmail;

            dtHeader.Rows.Add(dr);

            return dtHeader;
        }

        public DataTable GetDetailInfo()
        {
            string sArchivePath = GetArchivePathFromImage();
            string sSaleItem = string.Empty;
            string sItemParentId = string.Empty;

            DataTable dtDetails = new DataTable();
            dtDetails.Columns.Add("ITEMID", typeof(string));
            dtDetails.Columns.Add("PCS", typeof(decimal));
            dtDetails.Columns.Add("QTY", typeof(decimal));
            dtDetails.Columns.Add("NETTWT", typeof(decimal));
            dtDetails.Columns.Add("APPXVALUE", typeof(decimal));
            dtDetails.Columns.Add("JOBTYPE", typeof(string));
            dtDetails.Columns.Add("JOBDETAILS", typeof(string));
            dtDetails.Columns.Add("TOTALAMOUNT", typeof(decimal));
            dtDetails.Columns.Add("ORDERLINEIMAGE", typeof(string));
            dtDetails.Columns.Add("BATCHID", typeof(string));
            dtDetails.Columns.Add("Article", typeof(string));


            int i = 1;
            foreach (DataRow item in dtOrderDetails.Rows)
            {
                DataRow dr = dtDetails.NewRow();
                dr["ITEMID"] = item["ITEMID"];
                //dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                if (!string.IsNullOrEmpty(Convert.ToString(item["PCS"])))
                    dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                else
                    dr["PCS"] = 0;
                //dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
                if (!string.IsNullOrEmpty(Convert.ToString(item["QUANTITY"])))
                    dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
                else
                    dr["QTY"] = 0;

                dr["NETTWT"] = Convert.ToDecimal(item["NETTWT"]);
                dr["APPXVALUE"] = Convert.ToDecimal(item["TOTALAMT"]);
                dr["JOBTYPE"] = Convert.ToString(item["JOBTYPE"]);
                dr["JOBDETAILS"] = Convert.ToString(item["REPAIRJOBDETAILS"]);
                dr["TOTALAMOUNT"] = Convert.ToDecimal(item["RATE"]);

                string path = sArchivePath + "" + txtOrderNo.Text + "_" + i + ".jpeg"; //

                if (File.Exists(path))
                {
                    Image img = Image.FromFile(path);
                    byte[] arr;
                    using (MemoryStream ms1 = new MemoryStream())
                    {
                        img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                        arr = ms1.ToArray();
                    }

                    dr["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                }
                else
                {
                    sSaleItem = Convert.ToString(dr["ITEMID"]);
                    sItemParentId = GetItemParentId(Convert.ToString(dr["ITEMID"]));


                    path = sArchivePath + "" + sItemParentId + "" + ".jpg"; //

                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        byte[] arr;
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        dr["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                    }
                    else
                        dr["ORDERLINEIMAGE"] = "";

                }
                dr["BATCHID"] = txtOrderNo.Text + "" + i;
                dr["Article"] = Convert.ToString(item["Article"]);

                i++;
                dtDetails.Rows.Add(dr);
            }

            return dtDetails;
        }
        public string GetItemParentId(string sSalesItem)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select ITEMIDPARENT  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        public string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) ARCHIVEPATH  from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        #endregion

        #region Search Order
        public void btnSearchOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                DataTable dtGridItems = new DataTable();

                //string commandText = " SELECT ORDERNUM,CONVERT(VARCHAR(15),ORDERDATE,103) AS ORDERDATE ,CONVERT(VARCHAR(15),DELIVERYDATE,103) AS DELIVERYDATE , " +
                //                     " CUSTNAME ,CONVERT(VARCHAR(15),TOTALAMOUNT) AS TOTALAMOUNT  FROM CUSTORDER_HEADER ORDER BY ORDERNUM ";

                string commandText = " SELECT REPAIRID,CONVERT(VARCHAR(15),ORDERDATE,103) AS [ORDERDATE],CONVERT(VARCHAR(15),DELIVERYDATE,103) AS [DELIVERYDATE], " +
                                    " CUSTNAME as CUSTOMER FROM RetailRepairHdr ORDER BY REPAIRID "; // ORDER BY ORDERNUM ";


                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                SqlCommand command = new SqlCommand(commandText, connection);

                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                dtGridItems = new DataTable();
                dtGridItems.Load(reader);
                if (dtGridItems != null && dtGridItems.Rows.Count > 0)
                {
                    DataRow selRow = null;
                    Dialog objCustOrderSearch = new Dialog();

                    // objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Customer Order");

                    objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Ornament Repair");
                    if (selRow != null)
                        sCustOrderSearchNumber = Convert.ToString(selRow["RepairId"]);
                    else
                        return;
                    //  grItems.DataSource = dtGridItems.DefaultView;
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                //BlankOperations.WinFormsTouch.frmSearchOrder objSearchOrder = new BlankOperations.WinFormsTouch.frmSearchOrder(pos, application, this);
                //objSearchOrder.ShowDialog();
            }

            if (!string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                dsOrderSearched = new DataSet();

                //string commandText = " SELECT ORDERNUM, ORDERDATE, DELIVERYDATE, CUSTACCOUNT, CUSTNAME,CUSTADDRESS,CUSTPHONE,TOTALAMOUNT FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sCustOrderSearchNumber.Trim() + "'; " +
                //                     " SELECT ORDERNUM, LINENUM, ITEMID, CONFIGID, CODE, SIZEID, STYLE, PCS, QTY, CRATE AS RATE, " +
                //                     " CASE WHEN RATETYPE=0 THEN '" + RateType.Weight + "' WHEN RATETYPE=1 THEN '" + RateType.Pcs + "' WHEN RATETYPE=2 THEN '" + RateType.Tot + "' END AS RATETYPE, " +
                //                     " AMOUNT, MAKINGRATE, " +
                //                     " CASE WHEN MAKINGRATETYPE=0 THEN '" + MakingType.Weight + "' WHEN MAKINGRATETYPE=1 THEN '" + MakingType.Pieces + "' WHEN MAKINGRATETYPE=2 THEN '" + MakingType.Tot + "' WHEN MAKINGRATETYPE=3 THEN '" + MakingType.Percentage + "' END AS MAKINGTYPE, " +
                //                     " MAKINGAMOUNT, EXTENDEDDETAILSAMOUNT AS EXTENDEDDETAILS, (ISNULL(AMOUNT,0) + ISNULL(MAKINGAMOUNT,0) + ISNULL(EXTENDEDDETAILSAMOUNT,0)) AS ROWTOTALAMOUNT FROM CUSTORDER_DETAILS WHERE ORDERNUM ='" + sCustOrderSearchNumber.Trim() + "'; " +
                //                     " SELECT ORDERNUM, ORDERDETAILNUM, LINENUM, ITEMID, CONFIGID, CODE, SIZEID, STYLE, PCS, QTY, CRATE AS RATE, " +
                //                     " CASE WHEN RATETYPE=0 THEN '" + RateType.Weight + "' WHEN RATETYPE=1 THEN '" + RateType.Pcs + "' WHEN RATETYPE=2 THEN '" + RateType.Tot + "' END AS RATETYPE, " +
                //                     " AMOUNT FROM CUSTORDER_SUBDETAILS WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'; " +
                //                     " SELECT [ORDERNUM],[LINENUM],[ARTICLE],[APPROXWT],[APPROXVAL],[AAPROXDIAMONDWT] " +
                //                     " ,[APPROXDIAMONDVAL],[APPROXGOLDWT],[APPROXGOLDVAL],[APPROXOVERALLWT] " +
                //                     " ,[APPROXOVERALLVAL] FROM [CUSTORDER_SAMPLE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "' ";

                string commandText = " SELECT RepairId as ORDERNUM, ORDERDATE, DELIVERYDATE, CUSTACCOUNT, CUSTNAME,CUSTADDRESS,CUSTPHONE, 0 as TOTALAMOUNT FROM RetailRepairHdr WHERE RepairId = '" + sCustOrderSearchNumber.Trim() + "'; " +
                                     " SELECT RepairId as ORDERNUM, LINENUM, ITEMID, CONFIGID,CODE,SIZEID,STYLE, PCS, QTY AS QUANTITY, AMOUNT AS RATE, " +
                                     " NETTWT,DIAWT,DIAAMT,STNWT,STNAMT,TOTALAMT,JOBTYPE,REPAIRJOBDETAILS, " +
                                     " 4 AS RATETYPE, " +
                                     " AMOUNT, 0 as MAKINGRATE, " +
                                     " 4 as MAKINGTYPE, " +
                                     "  0 as MAKINGAMOUNT, 0 AS EXTENDEDDETAILS, 0 AS ROWTOTALAMOUNT FROM RetailRepairDetail WHERE RepairId ='" + sCustOrderSearchNumber.Trim() + "'; " +
                                     " SELECT RepairId as ORDERNUM, LineNumDtl as ORDERDETAILNUM, LINENUM, ITEMID, CONFIGID as CONFIGURATION,  CODE,  SIZEID, STYLE, PCS, QTY AS QUANTITY, AMOUNT RATE, " +
                                     " 4 AS RATETYPE, " +
                                     " AMOUNT FROM RetailRepairSubDetail WHERE RepairId ='" + sCustOrderSearchNumber.Trim() + "'; ";

                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                    connection.Open();


                SqlCommand command = new SqlCommand(commandText, connection);


                command.CommandTimeout = 0;

                SqlDataAdapter adapter = new SqlDataAdapter(commandText, connection);
                dsOrderSearched = new DataSet();
                adapter.Fill(dsOrderSearched);

                if (dsOrderSearched != null && dsOrderSearched.Tables.Count > 0)
                {
                    panel2.Enabled = false;
                    btnVoid.Enabled = false;
                    btnCustomerSearch.Enabled = false;
                    btnAddCustomer.Enabled = false;
                    btnSave.Enabled = false;
                    txtOrderNo.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["ORDERNUM"]);
                    txtCustomerAccount.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTACCOUNT"]);
                    txtCustomerAddress.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTADDRESS"]);
                    txtPhoneNumber.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTPHONE"]);
                    txtCustomerName.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["CUSTNAME"]);
                    txtTotalAmount.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["TOTALAMOUNT"]);
                    dTPickerOrderDate.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["ORDERDATE"]);
                    dtPickerDeliveryDate.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["DELIVERYDATE"]);
                    dtOrderDetails = new DataTable();
                }
            }


        }
        #endregion

        #region CLOSE CLICK
        public void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region isValid()
        public bool isValid()
        {
            if (string.IsNullOrEmpty(txtCustomerAccount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Account can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtCustomerName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            //if(string.IsNullOrEmpty(txtRepairBagNo.Text.Trim()))
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Repair bag no can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}

            if (dtOrderDetails.Rows.Count == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Give repair order details", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dTPickerOrderDate.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Delivery Date cannot be less than Receipt Date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;
                    return false;
                }
            }
            if (cmbRepairType.SelectedIndex == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select a repair type", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            else
            {
                return true;
            }
        }
        #endregion

        #region Clear Controls
        public void CLearControls()
        {
            txtCustomerName.Text = string.Empty;
            txtCustomerAccount.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            dTPickerOrderDate.Text = DateTime.Now.ToString();
            dtPickerDeliveryDate.Text = DateTime.Now.ToString();
        }
        #endregion

        #region Clear CLick
        public void btnClear_Click(object sender, EventArgs e)
        {
            dtOrderDetails = new DataTable();
            dtSubOrderDetails = new DataTable();
            sOrderDetailsAmount = string.Empty;
            sSubOrderDetailsAmount = string.Empty;
            sCustOrderSearchNumber = string.Empty;
            dsOrderSearched = new DataSet();
            txtOrderNo.Text = GetOrderNum();
            panel2.Enabled = true;
            btnVoid.Enabled = true;
            btnCustomerSearch.Enabled = true;
            btnAddCustomer.Enabled = true;
            btnSave.Enabled = true;
            CLearControls();
        }
        #endregion

        public void dtPickerDeliveryDate_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dTPickerOrderDate.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Delivery Date cannot be less than Receipt Date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;

                }
            }
        }

        byte[] NIM_ReadFile(string sPath)
        {
            //Initialize byte array with a null value initially.
            byte[] data = null;

            //Use FileInfo object to get file size.
            FileInfo fInfo = new FileInfo(sPath);
            long numBytes = fInfo.Length;

            //Open FileStream to read file
            FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);

            //Use BinaryReader to read file stream into byte array.
            BinaryReader br = new BinaryReader(fStream);

            //When you use BinaryReader, you need to supply number of bytes 
            //to read from file.
            //In this case we want to read entire file. 
            //So supplying total number of bytes.
            data = br.ReadBytes((int)numBytes);

            return data;
        }

        #region NIM_OrderUpdateWithSketch
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 24/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :Update Custom order with Sketch image 
        /// </summary>
        public void NIM_OrderUpdateWithSketch()
        {
            for (int sketchLine = 0; sketchLine < dtSketchDetails.Rows.Count; sketchLine++)
            {
                //SqlConnection connection = new SqlConnection();
                //if(application != null)
                //    connection = application.Settings.Database.Connection;
                //else
                //    connection = ApplicationSettings.Database.LocalConnection;

                //string commandText = " UPDATE [CUSTORDER_DETAILS] SET [SKETCH] = @SKETCH" +
                //                   " WHERE ORDERNUM = @ORDERNUM AND LINENUM = @LINENUM AND STOREID = @STOREID AND TERMINALID = @TERMINALID";
                //SqlCommand command = new SqlCommand(commandText, connection);

                //if(string.IsNullOrEmpty(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"])))
                //    command.Parameters.Add("@SKETCH", SqlDbType.Image).Value = DBNull.Value;
                //else
                //{
                byte[] imageData = NIM_ReadFile(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"]));

                //command.Parameters.Add("@SKETCH", SqlDbType.Image).Value = imageData;// Base64ToImage(hexData); //hexData;

                #region save into folder
                string sArchivePath = string.Empty;
                sArchivePath = GetArchivePathFromImage();
                string path = sArchivePath + "" + txtOrderNo.Text + "_" + Convert.ToInt16(dtSketchDetails.Rows[sketchLine]["LINENUM"]) + "" + ".jpeg"; //

                Bitmap b = new Bitmap(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"]));
                b.Save(Convert.ToString(path));

                #endregion
                //}

                //command.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                //command.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = Convert.ToInt16(dtSketchDetails.Rows[sketchLine]["LINENUM"]);
                //command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                //command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;

                //if(connection.State == ConnectionState.Closed)
                //    connection.Open();

                //command.ExecuteNonQuery();
                //connection.Close();
            }

        }
        #endregion

        public string sPincode { get; set; }

        private void btnSM_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtSM.Tag = dialog6.SelectedEmployeeId;
                txtSM.Text = dialog6.SelectEmployeeName;
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                PrintVoucher(sCustOrderSearchNumber, 1);
            }
        }
    }
}
