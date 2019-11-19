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
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
//

namespace BlankOperations
{
    public class frmCustomerOrder : frmTouchBase
    {
        private System.ComponentModel.IContainer components = null;

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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCashierTask = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPrint = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearchOrder = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnStoneAdv = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnCustomerSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSave = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnOrderDetails = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAddCustomer = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.btnGMASchemeSearch = new System.Windows.Forms.Button();
            this.cmbGMA = new System.Windows.Forms.ComboBox();
            this.chkGMA = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnSVSchemeSearch = new System.Windows.Forms.Button();
            this.cmbSVSchemeCode = new System.Windows.Forms.ComboBox();
            this.chkSUVARNAVRUDDHI = new System.Windows.Forms.CheckBox();
            this.txtReasonForDateChange = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtRemarks = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dtpReqDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.btnSM = new System.Windows.Forms.Button();
            this.txtSM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbVoucher = new System.Windows.Forms.ComboBox();
            this.cmbAdvAgainst = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.dtPickerDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.lblDeliveryDate = new System.Windows.Forms.Label();
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
            this.pnlMain.Size = new System.Drawing.Size(710, 440);
            this.pnlMain.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.btnCashierTask, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPrint, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnSearchOrder, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClose, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnStoneAdv, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(357, 279);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 51F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(345, 155);
            this.tableLayoutPanel1.TabIndex = 35;
            // 
            // btnCashierTask
            // 
            this.btnCashierTask.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCashierTask.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCashierTask.Appearance.Options.UseFont = true;
            this.btnCashierTask.Location = new System.Drawing.Point(4, 58);
            this.btnCashierTask.Name = "btnCashierTask";
            this.btnCashierTask.Size = new System.Drawing.Size(164, 38);
            this.btnCashierTask.TabIndex = 172;
            this.btnCashierTask.Text = "Cashier Task";
            this.btnCashierTask.Click += new System.EventHandler(this.btnCashierTask_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPrint.Appearance.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrint.Appearance.ForeColor = System.Drawing.Color.Red;
            this.btnPrint.Appearance.Options.UseFont = true;
            this.btnPrint.Appearance.Options.UseForeColor = true;
            this.btnPrint.Location = new System.Drawing.Point(7, 109);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(158, 39);
            this.btnPrint.TabIndex = 171;
            this.btnPrint.Text = "Print";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnSearchOrder
            // 
            this.btnSearchOrder.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearchOrder.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchOrder.Appearance.Options.UseFont = true;
            this.btnSearchOrder.Location = new System.Drawing.Point(176, 5);
            this.btnSearchOrder.Name = "btnSearchOrder";
            this.btnSearchOrder.Size = new System.Drawing.Size(164, 41);
            this.btnSearchOrder.TabIndex = 35;
            this.btnSearchOrder.Text = "Search Order";
            this.btnSearchOrder.Click += new System.EventHandler(this.btnSearchOrder_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Location = new System.Drawing.Point(176, 108);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(164, 41);
            this.btnClose.TabIndex = 33;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(176, 56);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(164, 42);
            this.btnClear.TabIndex = 34;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnStoneAdv
            // 
            this.btnStoneAdv.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnStoneAdv.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStoneAdv.Appearance.Options.UseFont = true;
            this.btnStoneAdv.Location = new System.Drawing.Point(4, 7);
            this.btnStoneAdv.Name = "btnStoneAdv";
            this.btnStoneAdv.Size = new System.Drawing.Size(164, 38);
            this.btnStoneAdv.TabIndex = 32;
            this.btnStoneAdv.Text = "Void";
            this.btnStoneAdv.Click += new System.EventHandler(this.btnStoneAdvance_Click);
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
            this.tableLayoutPanel.Location = new System.Drawing.Point(5, 279);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(345, 155);
            this.tableLayoutPanel.TabIndex = 4;
            // 
            // btnCustomerSearch
            // 
            this.btnCustomerSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCustomerSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCustomerSearch.Appearance.Options.UseFont = true;
            this.btnCustomerSearch.Location = new System.Drawing.Point(4, 4);
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
            this.btnSave.Location = new System.Drawing.Point(176, 81);
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
            this.btnOrderDetails.Location = new System.Drawing.Point(4, 81);
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
            this.btnAddCustomer.Location = new System.Drawing.Point(175, 4);
            this.btnAddCustomer.Name = "btnAddCustomer";
            this.btnAddCustomer.Size = new System.Drawing.Size(165, 70);
            this.btnAddCustomer.TabIndex = 29;
            this.btnAddCustomer.Text = "Add Customer";
            this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.btnGMASchemeSearch);
            this.panel2.Controls.Add(this.cmbGMA);
            this.panel2.Controls.Add(this.chkGMA);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.btnSVSchemeSearch);
            this.panel2.Controls.Add(this.cmbSVSchemeCode);
            this.panel2.Controls.Add(this.chkSUVARNAVRUDDHI);
            this.panel2.Controls.Add(this.txtReasonForDateChange);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.txtRemarks);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.dtpReqDeliveryDate);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.btnSM);
            this.panel2.Controls.Add(this.txtSM);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.cmbVoucher);
            this.panel2.Controls.Add(this.cmbAdvAgainst);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtTotalAmount);
            this.panel2.Controls.Add(this.txtPhoneNumber);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtCustomerAddress);
            this.panel2.Controls.Add(this.lblCustomerAddress);
            this.panel2.Controls.Add(this.dtPickerDeliveryDate);
            this.panel2.Controls.Add(this.lblDeliveryDate);
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
            this.panel2.Size = new System.Drawing.Size(697, 235);
            this.panel2.TabIndex = 3;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label11.Location = new System.Drawing.Point(330, 201);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(99, 13);
            this.label11.TabIndex = 214;
            this.label11.Text = "GMA Scheme code:";
            // 
            // btnGMASchemeSearch
            // 
            this.btnGMASchemeSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGMASchemeSearch.ForeColor = System.Drawing.Color.Red;
            this.btnGMASchemeSearch.Location = new System.Drawing.Point(670, 198);
            this.btnGMASchemeSearch.Name = "btnGMASchemeSearch";
            this.btnGMASchemeSearch.Size = new System.Drawing.Size(18, 19);
            this.btnGMASchemeSearch.TabIndex = 213;
            this.btnGMASchemeSearch.Text = "S";
            this.btnGMASchemeSearch.UseVisualStyleBackColor = true;
            this.btnGMASchemeSearch.Click += new System.EventHandler(this.btnGMASchemeSearch_Click);
            // 
            // cmbGMA
            // 
            this.cmbGMA.BackColor = System.Drawing.SystemColors.Control;
            this.cmbGMA.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbGMA.Enabled = false;
            this.cmbGMA.FormattingEnabled = true;
            this.cmbGMA.Location = new System.Drawing.Point(456, 196);
            this.cmbGMA.Name = "cmbGMA";
            this.cmbGMA.Size = new System.Drawing.Size(208, 21);
            this.cmbGMA.TabIndex = 212;
            // 
            // chkGMA
            // 
            this.chkGMA.AutoSize = true;
            this.chkGMA.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkGMA.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.chkGMA.Location = new System.Drawing.Point(276, 198);
            this.chkGMA.Name = "chkGMA";
            this.chkGMA.Size = new System.Drawing.Size(48, 17);
            this.chkGMA.TabIndex = 211;
            this.chkGMA.Text = "GMA";
            this.chkGMA.UseVisualStyleBackColor = true;
            this.chkGMA.CheckedChanged += new System.EventHandler(this.chkGMA_CheckedChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label10.Location = new System.Drawing.Point(330, 188);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(89, 13);
            this.label10.TabIndex = 206;
            this.label10.Text = "SV Scheme code:";
            this.label10.Visible = false;
            // 
            // btnSVSchemeSearch
            // 
            this.btnSVSchemeSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSVSchemeSearch.ForeColor = System.Drawing.Color.Red;
            this.btnSVSchemeSearch.Location = new System.Drawing.Point(670, 185);
            this.btnSVSchemeSearch.Name = "btnSVSchemeSearch";
            this.btnSVSchemeSearch.Size = new System.Drawing.Size(18, 19);
            this.btnSVSchemeSearch.TabIndex = 205;
            this.btnSVSchemeSearch.Text = "S";
            this.btnSVSchemeSearch.UseVisualStyleBackColor = true;
            this.btnSVSchemeSearch.Visible = false;
            this.btnSVSchemeSearch.Click += new System.EventHandler(this.btnSVSchemeSearch_Click);
            // 
            // cmbSVSchemeCode
            // 
            this.cmbSVSchemeCode.BackColor = System.Drawing.SystemColors.Control;
            this.cmbSVSchemeCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbSVSchemeCode.Enabled = false;
            this.cmbSVSchemeCode.FormattingEnabled = true;
            this.cmbSVSchemeCode.Location = new System.Drawing.Point(456, 183);
            this.cmbSVSchemeCode.Name = "cmbSVSchemeCode";
            this.cmbSVSchemeCode.Size = new System.Drawing.Size(208, 21);
            this.cmbSVSchemeCode.TabIndex = 204;
            this.cmbSVSchemeCode.Visible = false;
            // 
            // chkSUVARNAVRUDDHI
            // 
            this.chkSUVARNAVRUDDHI.AutoSize = true;
            this.chkSUVARNAVRUDDHI.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSUVARNAVRUDDHI.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.chkSUVARNAVRUDDHI.Location = new System.Drawing.Point(203, 187);
            this.chkSUVARNAVRUDDHI.Name = "chkSUVARNAVRUDDHI";
            this.chkSUVARNAVRUDDHI.Size = new System.Drawing.Size(121, 17);
            this.chkSUVARNAVRUDDHI.TabIndex = 184;
            this.chkSUVARNAVRUDDHI.Text = "SUVARNA VRUDDHI";
            this.chkSUVARNAVRUDDHI.UseVisualStyleBackColor = true;
            this.chkSUVARNAVRUDDHI.Visible = false;
            this.chkSUVARNAVRUDDHI.CheckedChanged += new System.EventHandler(this.chkSUVARNAVRUDDHI_CheckedChanged);
            // 
            // txtReasonForDateChange
            // 
            this.txtReasonForDateChange.Location = new System.Drawing.Point(456, 77);
            this.txtReasonForDateChange.MaxLength = 200;
            this.txtReasonForDateChange.Multiline = true;
            this.txtReasonForDateChange.Name = "txtReasonForDateChange";
            this.txtReasonForDateChange.Size = new System.Drawing.Size(232, 32);
            this.txtReasonForDateChange.TabIndex = 183;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label9.Location = new System.Drawing.Point(330, 80);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(127, 13);
            this.label9.TabIndex = 182;
            this.label9.Text = "Reason for date change:";
            // 
            // txtRemarks
            // 
            this.txtRemarks.Location = new System.Drawing.Point(456, 111);
            this.txtRemarks.MaxLength = 500;
            this.txtRemarks.Multiline = true;
            this.txtRemarks.Name = "txtRemarks";
            this.txtRemarks.Size = new System.Drawing.Size(232, 44);
            this.txtRemarks.TabIndex = 181;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label8.Location = new System.Drawing.Point(330, 113);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 180;
            this.label8.Text = "Remarks:";
            // 
            // dtpReqDeliveryDate
            // 
            this.dtpReqDeliveryDate.Location = new System.Drawing.Point(456, 54);
            this.dtpReqDeliveryDate.Name = "dtpReqDeliveryDate";
            this.dtpReqDeliveryDate.Size = new System.Drawing.Size(232, 21);
            this.dtpReqDeliveryDate.TabIndex = 179;
            this.dtpReqDeliveryDate.ValueChanged += new System.EventHandler(this.dtpReqDeliveryDate_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label7.Location = new System.Drawing.Point(330, 56);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 13);
            this.label7.TabIndex = 178;
            this.label7.Text = "Request Delivery Date:";
            // 
            // btnSM
            // 
            this.btnSM.Location = new System.Drawing.Point(589, 6);
            this.btnSM.Name = "btnSM";
            this.btnSM.Size = new System.Drawing.Size(99, 23);
            this.btnSM.TabIndex = 177;
            this.btnSM.Text = "Search Sales man";
            this.btnSM.UseVisualStyleBackColor = true;
            this.btnSM.Click += new System.EventHandler(this.btnSM_Click);
            // 
            // txtSM
            // 
            this.txtSM.BackColor = System.Drawing.SystemColors.Control;
            this.txtSM.Enabled = false;
            this.txtSM.Location = new System.Drawing.Point(456, 6);
            this.txtSM.MaxLength = 20;
            this.txtSM.Name = "txtSM";
            this.txtSM.ReadOnly = true;
            this.txtSM.Size = new System.Drawing.Size(127, 21);
            this.txtSM.TabIndex = 176;
            this.txtSM.TextChanged += new System.EventHandler(this.txtSM_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(330, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 175;
            this.label6.Text = "Sales man:";
            this.label6.Click += new System.EventHandler(this.label6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label5.Location = new System.Drawing.Point(21, 211);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 174;
            this.label5.Text = "Voucher against:";
            this.label5.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(6, 204);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 173;
            this.label4.Text = "Advance against:";
            this.label4.Visible = false;
            // 
            // cmbVoucher
            // 
            this.cmbVoucher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVoucher.FormattingEnabled = true;
            this.cmbVoucher.Location = new System.Drawing.Point(24, 211);
            this.cmbVoucher.Name = "cmbVoucher";
            this.cmbVoucher.Size = new System.Drawing.Size(73, 21);
            this.cmbVoucher.TabIndex = 172;
            this.cmbVoucher.Visible = false;
            // 
            // cmbAdvAgainst
            // 
            this.cmbAdvAgainst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAdvAgainst.FormattingEnabled = true;
            this.cmbAdvAgainst.Location = new System.Drawing.Point(24, 204);
            this.cmbAdvAgainst.Name = "cmbAdvAgainst";
            this.cmbAdvAgainst.Size = new System.Drawing.Size(41, 21);
            this.cmbAdvAgainst.TabIndex = 170;
            this.cmbAdvAgainst.Visible = false;
            this.cmbAdvAgainst.SelectedIndexChanged += new System.EventHandler(this.cmbAdvAgainst_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(330, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Total Amount:";
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtTotalAmount.Enabled = false;
            this.txtTotalAmount.Location = new System.Drawing.Point(456, 158);
            this.txtTotalAmount.MaxLength = 60;
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.Size = new System.Drawing.Size(232, 21);
            this.txtTotalAmount.TabIndex = 19;
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPhoneNumber.Enabled = false;
            this.txtPhoneNumber.Location = new System.Drawing.Point(113, 158);
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
            this.label3.Location = new System.Drawing.Point(6, 161);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Phone Number:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAddress.Enabled = false;
            this.txtCustomerAddress.Location = new System.Drawing.Point(113, 100);
            this.txtCustomerAddress.MaxLength = 20;
            this.txtCustomerAddress.Multiline = true;
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(211, 55);
            this.txtCustomerAddress.TabIndex = 15;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAddress.Location = new System.Drawing.Point(6, 103);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAddress.TabIndex = 14;
            this.lblCustomerAddress.Text = "Customer Address:";
            // 
            // dtPickerDeliveryDate
            // 
            this.dtPickerDeliveryDate.Enabled = false;
            this.dtPickerDeliveryDate.Location = new System.Drawing.Point(456, 30);
            this.dtPickerDeliveryDate.Name = "dtPickerDeliveryDate";
            this.dtPickerDeliveryDate.Size = new System.Drawing.Size(232, 21);
            this.dtPickerDeliveryDate.TabIndex = 13;
            this.dtPickerDeliveryDate.ValueChanged += new System.EventHandler(this.dtPickerDeliveryDate_ValueChanged);
            // 
            // lblDeliveryDate
            // 
            this.lblDeliveryDate.AutoSize = true;
            this.lblDeliveryDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDeliveryDate.Location = new System.Drawing.Point(330, 32);
            this.lblDeliveryDate.Name = "lblDeliveryDate";
            this.lblDeliveryDate.Size = new System.Drawing.Size(76, 13);
            this.lblDeliveryDate.TabIndex = 12;
            this.lblDeliveryDate.Text = "Delivery Date:";
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
            this.txtOrderNo.TextChanged += new System.EventHandler(this.txtOrderNo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(6, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Customer Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(113, 77);
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
            this.txtCustomerAccount.Location = new System.Drawing.Point(113, 54);
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
            this.lblCustomerAccount.Location = new System.Drawing.Point(6, 56);
            this.lblCustomerAccount.Name = "lblCustomerAccount";
            this.lblCustomerAccount.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAccount.TabIndex = 8;
            this.lblCustomerAccount.Text = "Customer Account:";
            // 
            // dTPickerOrderDate
            // 
            this.dTPickerOrderDate.Enabled = false;
            this.dTPickerOrderDate.Location = new System.Drawing.Point(113, 30);
            this.dTPickerOrderDate.Name = "dTPickerOrderDate";
            this.dTPickerOrderDate.Size = new System.Drawing.Size(211, 21);
            this.dTPickerOrderDate.TabIndex = 5;
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderDate.Location = new System.Drawing.Point(6, 31);
            this.lblOrderDate.Name = "lblOrderDate";
            this.lblOrderDate.Size = new System.Drawing.Size(65, 13);
            this.lblOrderDate.TabIndex = 4;
            this.lblOrderDate.Text = "Order Date:";
            // 
            // lblOrderNo
            // 
            this.lblOrderNo.AutoSize = true;
            this.lblOrderNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderNo.Location = new System.Drawing.Point(6, 9);
            this.lblOrderNo.Name = "lblOrderNo";
            this.lblOrderNo.Size = new System.Drawing.Size(59, 13);
            this.lblOrderNo.TabIndex = 2;
            this.lblOrderNo.Text = "Order No.:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(240, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(219, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "CUSTOMER ORDER";
            // 
            // frmCustomerOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 448);
            this.ControlBox = false;
            this.Controls.Add(this.pnlMain);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmCustomerOrder";
            this.ShowIcon = false;
            this.Text = "Customer Order";
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

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DateTimePicker dtPickerDeliveryDate;
        private System.Windows.Forms.Label lblDeliveryDate;
        public TextBox txtOrderNo;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        private System.Windows.Forms.Label lblCustomerAccount;
        private System.Windows.Forms.DateTimePicker dTPickerOrderDate;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label lblOrderNo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSearchOrder;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnStoneAdv;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddCustomer;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCustomerSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOrderDetails;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSave;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        public System.Windows.Forms.TextBox txtCustomerAddress;
        private System.Windows.Forms.Label lblCustomerAddress;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtTotalAmount;
        public System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label label3;


        #region Variable Declaration
        public string CustAddress { get; set; }
        public string CustPhoneNo { get; set; }
        public IPosTransaction pos { get; set; }
        public DataTable dtOrderDetails = new DataTable("dtOrderDetails");
        public DataTable dtSampleDetails = new DataTable("dtSampleDetails");
        public DataTable dtSubOrderDetails = new DataTable("dtSubOrderDetails");
        public DataTable dtOrderStnAdv = new DataTable("dtOrderStnAdv");


        public DataTable dtSketchDetails = new DataTable("dtSketchDetails");
        public DataTable dtSampleSketch = new DataTable("dtSampleSketch");

        public string sOrderDetailsAmount = string.Empty;
        public string sSubOrderDetailsAmount = string.Empty;
        public string sCustOrderSearchNumber = string.Empty;
        DataSet dsOrderSearched = new DataSet();

        public bool bDataSaved = false;
        public string sCustAcc = string.Empty;
        public string sCustOrder = string.Empty;
        public string sTotalAmt = string.Empty;
        public decimal dTotalAdvAmt = 0m;

        public decimal dFixedMetalRateVal = 0m; // Fixed Metal Rate New
        public string sFixedMetalRateConfigID = string.Empty; // Fixed Metal Rate New

        public DataTable dtRecvStoneDetails = new DataTable("dtRecvStoneDetails"); //Add by Palas Jana
        public DataTable dtPaySchedule = new DataTable("dtPaySchedule");
        public DataTable dtChequeInfo = new DataTable("dtChequeInfo");
        int isBookedItem = 0;
        public int iIsCustOrderWithAdv = 0;
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        private Label label5;
        private Label label4;
        private ComboBox cmbVoucher;
        private ComboBox cmbAdvAgainst;
        public TextBox txtSM;
        private Label label6;
        private Button btnSM;
        DataTable skuItem = new DataTable();
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPrint;
        private DateTimePicker dtpReqDeliveryDate;
        private Label label7;
        private Label label8;
        private TextBox txtRemarks;
        private TextBox txtReasonForDateChange;
        private Label label9;
        public CheckBox chkSUVARNAVRUDDHI;
        public DateTime dtSVDeliveryDate = new DateTime();
        private Label label10;
        public Button btnSVSchemeSearch;
        public ComboBox cmbSVSchemeCode;
        
        decimal dParamMetalQtyTolPct = 0m;
        public decimal dSVTaxPct = 0m;
        public int iCancel = 0;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCashierTask;
        private Label label11;
        public Button btnGMASchemeSearch;
        public ComboBox cmbGMA;
        public CheckBox chkGMA;

        public decimal dGMADiscPct = 0m;
        public string sGMAPurity = "";
        public string sGMAItem = "";
        public decimal dGMAMinQty = 0m;
        

        [Import]
        private IApplication application;

        enum BulkTransactionType
        {
            None = 0,
            Receive = 1,
            Issue = 2,
            Sales = 3,
            SalesReturn = 4,
            OGPurchase = 5,
            OGExchange = 6,
            OGPurchaseReturn = 7,
            OGExchangeReturn = 8,
            OrderSampleReceive =9,
            OrderStnDmdReceive = 10,
            GMA =11,
        }

        enum AdvAgainst
        {
            None = 0,
            OGPurchase = 1,
            OGExchange = 2,
            SaleReturn = 3,
        }

        enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }
        #endregion

        #region Initialization
        public frmCustomerOrder(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            this.ClientSize = new System.Drawing.Size(726, 477);
            pos = posTransaction;
            application = Application;
            txtOrderNo.Text = GetOrderNum();
            dFixedMetalRateVal = GetFixedMetalRate(ref sFixedMetalRateConfigID); // Fixed Metal Rate New
            btnCustomerSearch.Focus();
            cmbAdvAgainst.DataSource = Enum.GetValues(typeof(AdvAgainst));
            btnPrint.Enabled = false;
            btnGMASchemeSearch.Enabled = false;

            int sOrderLeadTime = 0;
            sOrderLeadTime = getOrderLeadDays();
            dtPickerDeliveryDate.Value = Convert.ToDateTime(DateTime.Now).AddDays(sOrderLeadTime);
            dtpReqDeliveryDate.Value = dtPickerDeliveryDate.Value;

            if (string.IsNullOrEmpty(txtCustomerAccount.Text))
            {
                cmbAdvAgainst.Enabled = false;
                cmbVoucher.Enabled = false;
            }
            else
            {
                cmbAdvAgainst.Enabled = true;
                cmbVoucher.Enabled = true;
            }

            dParamMetalQtyTolPct = GetMetalQtyTolPct();
            btnCashierTask.Enabled = false;
        }
        #endregion

        #region GetOrderNum()
        public string GetOrderNum()
        {
            string OrderNum = string.Empty;
            OrderNum = GetNextCustomerOrderID();
            return OrderNum;
        }
        #endregion

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum ReceiptTransactionType
        {
            CustomerGoldOrder = 8
        }

        public string GetNextCustomerOrderID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.CustomerGoldOrder;
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
        private void orderNumber(int transType, string funcProfile, out string mask)
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
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.CustomerGoldOrder;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                // string queryString = " SELECT  MAX(CAST(ISNULL(SUBSTRING(CUSTORDER_HEADER.ORDERNUM,3,LEN(CUSTORDER_HEADER.ORDERNUM)),0) AS INTEGER)) + 1 from CUSTORDER_HEADER ";

                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(ORDERNUM,@VAL,LEN(ORDERNUM)),0) AS INTEGER)) + 1 from CUSTORDER_HEADER where ORDERDATE >='05-Apr-2019' ";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                    {
                        Val = "1";
                    }

                    return Val;
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

        #region Customer Button Click
        private void btnCustomerSearch_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch obfrm = new Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch.frmCustomerSearch(this);
            obfrm.ShowDialog();

            if (string.IsNullOrEmpty(txtCustomerAccount.Text))
            {
                cmbAdvAgainst.Enabled = false;
                cmbVoucher.Enabled = false;
            }
            else
            {
                cmbAdvAgainst.Enabled = true;
                cmbVoucher.Enabled = true;
            }
        }
        #endregion

        #region Delivery Date Changed
        private void dtPickerDeliveryDate_ValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(dtPickerDeliveryDate.Value) < Convert.ToDateTime(dTPickerOrderDate.Value))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DELIVERY DATE CANNOT BE LESS THAN ORDER DATE.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;

                }
            }
        }
        #endregion

        #region Click Order Details
        private void btnOrderDetails_Click(object sender, EventArgs e)
        {
            if (isValid())
            {
                BlankOperations.WinFormsTouch.frmOrderDetails objOrderdetails = null;
                if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                {
                    objOrderdetails = new BlankOperations.WinFormsTouch.frmOrderDetails(dtSampleDetails, dtSubOrderDetails, dtOrderDetails, dtPaySchedule, dtChequeInfo, pos, application, this);
                }

                else if (dsOrderSearched != null && dsOrderSearched.Tables.Count > 0 && dsOrderSearched.Tables[1].Rows.Count > 0)
                {
                    objOrderdetails = new BlankOperations.WinFormsTouch.frmOrderDetails(dsOrderSearched, pos, application, this);
                }
                else
                {
                    dtOrderDetails = new DataTable();
                    objOrderdetails = new BlankOperations.WinFormsTouch.frmOrderDetails(pos, application, this);
                }


                objOrderdetails.ShowDialog();
                txtTotalAmount.Text = sOrderDetailsAmount;
                //dtpReqDeliveryDate.Text =Convert.ToDateTime(dtSVDeliveryDate).ToShortDateString();
            }
        }
        #endregion

        #region ADD NEW CUSTOMER
        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            Customer obj = new Customer();
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
        private void btnSave_Click(object sender, EventArgs e)
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
                if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                {
                    SaveOrder();
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order has no line.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }
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

        #region Wastage Type

        enum WastageType // Added for wastage 
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }

        #endregion
        #endregion

        #region SaveFuction
        private void SaveOrder()
        {
            int iCustOrder_Header = 0;
            int iCustOrder_Details = 0;
            int iCustOrder_SubDetails = 0;
            //int iCustOrder_SampleDetails = 0;
            //int iCustOrder_StoneAdv = 0;
            DataTable dtBookedGroup = new DataTable();
            DataTable dtFromCounterGroup = new DataTable();

            SqlTransaction transaction = null;

            #region CUSTOMER ORDER HEADER
            string commandText = " INSERT INTO [CUSTORDER_HEADER]([ORDERNUM],[STOREID],[TERMINALID],[ORDERDATE],[DELIVERYDATE]," +
                                 " [CUSTACCOUNT],[CUSTNAME],[CUSTADDRESS],[CUSTPHONE], " +
                                 " [DATAAREAID],[STAFFID],[TOTALAMOUNT],[FIXEDMETALRATE],[FIXEDMETALRATECONFIGID]," +
                                 " [SAMPLEFLAG],[STONEFLAG],IsConfirmed,[WITHADVANCE],ADVAGAINST,VOUCHERAGAINST,SalesManID," +
                                 " REQUESTDELIVERYDATE,REASONFORREQUESTDELIVERYDATE,ORDERREMARKS,SUVARNAVRUDHI,FESTIVECODE,GMA,GMASCHEMECODE,GMADISCPER)" +
                                 " VALUES(@ORDERNUM,@STOREID,@TERMINALID,@ORDERDATE,@DELIVERYDATE,@CUSTACCOUNT,@CUSTNAME," +
                                 " @CUSTADDRESS,@CUSTPHONE,@DATAAREAID,@STAFFID,@TOTALAMOUNT,@FIXEDMETALRATE,@FIXEDMETALRATECONFIGID," +
                                 " @SAMPLEFLAG,@STONEFLAG,@IsConfirmed,@WITHADVANCE,@ADVAGAINST,@VOUCHERAGAINST,@SalesManID," +
                                 " @REQUESTDELIVERYDATE,@REASONFORREQUESTDELIVERYDATE,@ORDERREMARKS,@SUVARNAVRUDHI,@FESTIVECODE,@GMA,@GMASCHEMECODE,@GMADISCPER)";
            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
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
                command.Parameters.Add("@TOTALAMOUNT", SqlDbType.Decimal, 250).Value = sOrderDetailsAmount;
                command.Parameters.Add("@FIXEDMETALRATE", SqlDbType.Decimal, 250).Value = dFixedMetalRateVal; // Fixed Metal Rate New
                command.Parameters.Add("@FIXEDMETALRATECONFIGID", SqlDbType.NVarChar, 10).Value = sFixedMetalRateConfigID;// Fixed Metal Rate New
                command.Parameters.Add("@SAMPLEFLAG", SqlDbType.Int).Value = (dtSampleDetails.Rows.Count > 0) ? 1 : 0;
                command.Parameters.Add("@STONEFLAG", SqlDbType.Int).Value = (dtOrderStnAdv.Rows.Count > 0) ? 1 : 0;
                //command.Parameters.Add("@IsConfirmed", SqlDbType.Int).Value = 0;
                if (chkGMA.Checked)
                    command.Parameters.Add("@IsConfirmed", SqlDbType.Int).Value = 1;
                else
                    command.Parameters.Add("@IsConfirmed", SqlDbType.Int).Value = 0;

                command.Parameters.Add("@WITHADVANCE", SqlDbType.Int).Value = iIsCustOrderWithAdv;
                if (iIsCustOrderWithAdv > 0)
                {
                    command.Parameters.Add("@ADVAGAINST", SqlDbType.Int).Value = cmbAdvAgainst.SelectedIndex;
                    command.Parameters.Add("@VOUCHERAGAINST", SqlDbType.NVarChar, 20).Value = cmbVoucher.Text.Trim() == null ? string.Empty : cmbVoucher.Text.Trim();
                }
                else
                {
                    command.Parameters.Add("@ADVAGAINST", SqlDbType.Int).Value = 0;
                    command.Parameters.Add("@VOUCHERAGAINST", SqlDbType.NVarChar, 20).Value = string.Empty;
                }
                command.Parameters.Add("@SalesManID", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtSM.Tag) == null ? string.Empty : Convert.ToString(txtSM.Tag);
                command.Parameters.Add("@REQUESTDELIVERYDATE", SqlDbType.DateTime).Value = dtpReqDeliveryDate.Value;
                command.Parameters.Add("@REASONFORREQUESTDELIVERYDATE", SqlDbType.NVarChar, 200).Value = txtReasonForDateChange.Text.Trim();
                command.Parameters.Add("@ORDERREMARKS", SqlDbType.NVarChar, 500).Value = txtRemarks.Text.Trim();
                command.Parameters.Add("@SUVARNAVRUDHI", SqlDbType.Int).Value = chkSUVARNAVRUDDHI.Checked ? 1 : 0;
                command.Parameters.Add("@FESTIVECODE", SqlDbType.NVarChar, 99).Value = cmbSVSchemeCode.Text.Trim();
                command.Parameters.Add("@GMA", SqlDbType.Int).Value = chkGMA.Checked ? 1 : 0;
                command.Parameters.Add("@GMASCHEMECODE", SqlDbType.NVarChar, 20).Value = cmbGMA.Text.Trim();
                command.Parameters.Add("@GMADISCPER", SqlDbType.Decimal).Value = dGMADiscPct;

            #endregion

                command.CommandTimeout = 0;
                iCustOrder_Header = command.ExecuteNonQuery();

                if (iCustOrder_Header == 1)
                {
                    #region Stone advance save //commented RH
                    //if(dtOrderStnAdv != null && dtOrderStnAdv.Rows.Count > 0)
                    //{

                    //    string commandCustOrder_StoneAdv = " INSERT INTO [CustOrderStoneAdvance]([ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID] " +
                    //                 " ,[CODE],[SIZEID],[STYLE],[PCS],[QTY],[DATAAREAID],[INVENTDIMID],[UNITID],REMARKS)" +
                    //                 " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID ,@CONFIGID,@CODE ,@SIZEID,@STYLE,@PCS ,@QTY " +
                    //                 " ,@DATAAREAID , @INVENTDIMID, @UNITID, @REMARKS)";

                    //    for(int StoneAdvCount = 0; StoneAdvCount < dtOrderStnAdv.Rows.Count; StoneAdvCount++)
                    //    {

                    //        SqlCommand commandStoneAdv = new SqlCommand(commandCustOrder_StoneAdv, connection, transaction);
                    //        commandStoneAdv.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                    //        commandStoneAdv.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = StoneAdvCount + 1;
                    //        commandStoneAdv.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                    //        commandStoneAdv.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                    //        commandStoneAdv.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["ITEMID"]);
                    //        commandStoneAdv.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["CONFIGURATION"]);
                    //        commandStoneAdv.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["COLOR"]);
                    //        commandStoneAdv.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["SIZE"]);
                    //        commandStoneAdv.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["STYLE"]);
                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["PCS"])))
                    //            commandStoneAdv.Parameters.Add("@PCS", SqlDbType.Decimal).Value = DBNull.Value;
                    //        else
                    //            commandStoneAdv.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderStnAdv.Rows[StoneAdvCount]["PCS"]);

                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["QUANTITY"])))
                    //            commandStoneAdv.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                    //        else
                    //            commandStoneAdv.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderStnAdv.Rows[StoneAdvCount]["QUANTITY"]);

                    //        if(application != null)
                    //            commandStoneAdv.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                    //        else
                    //            commandStoneAdv.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;


                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["INVENTDIMID"])))
                    //            commandStoneAdv.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                    //        else
                    //            commandStoneAdv.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["INVENTDIMID"]);


                    //        if(string.IsNullOrEmpty(Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["UNITID"])))
                    //            commandStoneAdv.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = "";
                    //        else
                    //            commandStoneAdv.Parameters.Add("@UNITID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["UNITID"]);

                    //        commandStoneAdv.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 250).Value = Convert.ToString(dtOrderStnAdv.Rows[StoneAdvCount]["REMARKS"]);

                    //        commandStoneAdv.CommandTimeout = 0;
                    //        iCustOrder_StoneAdv = commandStoneAdv.ExecuteNonQuery();
                    //        commandStoneAdv.Dispose();
                    //    }
                    //}
                    #endregion

                    if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                    {
                        #region ORDER DETAILS
                        string commandCustOrder_Detail = " INSERT INTO [CUSTORDER_DETAILS]([ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID] " +
                                                 " ,[CODE],[SIZEID],[STYLE],[PCS],[QTY],[CRATE],[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE] " +
                                                 " ,[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT],[DATAAREAID],[STAFFID],[INVENTDIMID],[UNITID]" +
                                                 " , WastageRate,WastageType,WastageQty,WastagePercentage,WastageAmount," +
                                                 "  LineTotalAmt,IsBookedSKU,REMARKSDTL,LineFixRate,PRODUCT," +
                                                 " COLLECTIONCODE,ARTICLE,CertAgentCode,ACTUALITEMID," +
                                                 " STOREDELIVERYDATE,FESTIVECODE,ADVANCEAMT,GoldRate,SilverRate,PlatinumRate,MetalQty,SalesManID,GMADISCWT)" +
                                                 " VALUES(@ORDERNUM,@LINENUM,@STOREID,@TERMINALID,@ITEMID ,@CONFIGID,@CODE ,@SIZEID,@STYLE,@PCS ,@QTY " +
                                                 " ,@RATE ,@RATETYPE,@AMOUNT  ,@MAKINGRATE ,@MAKINGRATETYPE,@MAKINGAMOUNT,@EXTENDEDDETAILSAMOUNT " +
                                                 " ,@DATAAREAID  ,@STAFFID , @INVENTDIMID, @UNITID " +
                                                 " ,@WastageRate,@WastageType,@WastageQty,@WastagePercentage,@WastageAmount,@LineTotalAmt" +
                                                 " ,@IsBookedSKU,@REMARKSDTL,@LineFixRate,@PRODUCT," +
                                                 " @COLLECTIONCODE,@ARTICLE,@CertAgentCode,@ACTUALITEMID," +
                                                 " @STOREDELIVERYDATE,@FESTIVECODE,@ADVANCEAMT,@GoldRate,@SilverRate,@PlatinumRate,@MetalQty,@SalesManID,@GMADISCWT)";

                        for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                        {
                            //decimal sLineConfigRate = GetFixedMetalRateByOrderLineConfig(Convert.ToString(dtOrderDetails.Rows[ItemCount]["CONFIGURATION"]));

                            SqlCommand cmdCustOrder_Detail = new SqlCommand(commandCustOrder_Detail, connection, transaction);
                            cmdCustOrder_Detail.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                            cmdCustOrder_Detail.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = ItemCount + 1;
                            cmdCustOrder_Detail.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdCustOrder_Detail.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdCustOrder_Detail.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]);
                            cmdCustOrder_Detail.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["CONFIGURATION"]);
                            cmdCustOrder_Detail.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["COLOR"]);
                            cmdCustOrder_Detail.Parameters.Add("@SIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["SIZE"]);
                            cmdCustOrder_Detail.Parameters.Add("@STYLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["STYLE"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["PCS"])))
                                cmdCustOrder_Detail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["PCS"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["QUANTITY"])))
                                cmdCustOrder_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["QUANTITY"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATE"])))
                                cmdCustOrder_Detail.Parameters.Add("@RATE", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@RATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["RATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"])))
                                cmdCustOrder_Detail.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                            {
                                //    cmdCustOrder_Detail.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"]);
                                string rType = string.Empty;
                                if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"]) == Convert.ToString(RateType.Weight))
                                    rType = Convert.ToString((int)RateType.Weight);
                                else if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                    rType = Convert.ToString((int)RateType.Pcs);
                                else
                                    rType = Convert.ToString((int)RateType.Tot);
                                cmdCustOrder_Detail.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(rType);
                            }
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["AMOUNT"])))
                                //  cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                                cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["AMOUNT"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGRATE"])))
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATE", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MAKINGRATE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"])))
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATETYPE", SqlDbType.NVarChar).Value = DBNull.Value;
                            else
                            {
                                string mType = string.Empty;
                                if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"]) == Convert.ToString(MakingType.Weight))
                                    mType = Convert.ToString((int)MakingType.Weight);
                                else if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"]) == Convert.ToString(MakingType.Pieces))
                                    mType = Convert.ToString((int)MakingType.Pieces);
                                else if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGTYPE"]) == Convert.ToString(MakingType.Tot))
                                    mType = Convert.ToString((int)MakingType.Tot);
                                else
                                    mType = Convert.ToString((int)MakingType.Percentage);

                                cmdCustOrder_Detail.Parameters.Add("@MAKINGRATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(mType);
                            }

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"])))
                                // cmdCustOrder_Detail.Parameters.Add("@MAKINGAMOUNT", SqlDbType.Decimal).Value = DBNull.Value;
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGAMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@MAKINGAMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["EXTENDEDDETAILS"])))
                                cmdCustOrder_Detail.Parameters.Add("@EXTENDEDDETAILSAMOUNT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@EXTENDEDDETAILSAMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["EXTENDEDDETAILS"]);



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

                            // Added for wastage 
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageRate"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageRate", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastageRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageRate"]);


                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageType"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageType", SqlDbType.NVarChar).Value = "0";
                            else
                            {
                                string rType = string.Empty;
                                if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageType"]) == Convert.ToString(WastageType.Weight))
                                    rType = Convert.ToString((int)WastageType.Weight);
                                else
                                    rType = Convert.ToString((int)WastageType.Percentage);
                                cmdCustOrder_Detail.Parameters.Add("@WastageType", SqlDbType.NVarChar).Value = Convert.ToString(rType);
                            }

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageQty"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageQty", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastageQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageQty"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastagePercentage"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastagePercentage", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastagePercentage", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastagePercentage"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageAmount"])))
                                cmdCustOrder_Detail.Parameters.Add("@WastageAmount", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@WastageAmount", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageAmount"]);

                            cmdCustOrder_Detail.Parameters.Add("@LineTotalAmt", SqlDbType.Decimal).Value = (!string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["AMOUNT"])) ? Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["AMOUNT"]) : 0)
                                                                                                            + (!string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"])) ? Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MAKINGAMOUNT"]) : 0)
                                                                                                            + (!string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["WastageAmount"])) ? Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["WastageAmount"]) : 0);

                            cmdCustOrder_Detail.Parameters.Add("@IsBookedSKU", SqlDbType.Int).Value = Convert.ToInt32(dtOrderDetails.Rows[ItemCount]["IsBookedSKU"]);
                            cmdCustOrder_Detail.Parameters.Add("@REMARKSDTL", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["REMARKSDTL"]);
                            cmdCustOrder_Detail.Parameters.Add("@LineFixRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["Rate"]);// sLineConfigRate;

                            cmdCustOrder_Detail.Parameters.Add("@PRODUCT", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["PRODUCT"]);
                            cmdCustOrder_Detail.Parameters.Add("@COLLECTIONCODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["COLLECTION"]);
                            cmdCustOrder_Detail.Parameters.Add("@ARTICLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["ARTICLE"]);
                            cmdCustOrder_Detail.Parameters.Add("@CertAgentCode", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["CertAgentCode"]);
                            cmdCustOrder_Detail.Parameters.Add("@ACTUALITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["ACTUALITEMID"]);
                            cmdCustOrder_Detail.Parameters.Add("@STOREDELIVERYDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dtPickerDeliveryDate.Value).AddDays(-Convert.ToInt16(dtOrderDetails.Rows[ItemCount]["CUSTORDSTOREDELIVERYDAYS"]));

                            cmdCustOrder_Detail.Parameters.Add("@FESTIVECODE", SqlDbType.NVarChar, 99).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["FESTIVECODE"]);
                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["ADVANCEAMT"])))
                                cmdCustOrder_Detail.Parameters.Add("@ADVANCEAMT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@ADVANCEAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["ADVANCEAMT"]);

                            dTotalAdvAmt += Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["ADVANCEAMT"]);

                            cmdCustOrder_Detail.Parameters.Add("@GoldRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["GoldRate"]);
                            cmdCustOrder_Detail.Parameters.Add("@SilverRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["SilverRate"]);
                            cmdCustOrder_Detail.Parameters.Add("@PlatinumRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["PlatinumRate"]);

                            cmdCustOrder_Detail.Parameters.Add("@MetalQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MetalQty"]) + (Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["MetalQty"]) * dParamMetalQtyTolPct) / 100;

                            //dr["CUSTORDSTOREDELIVERYDAYS"] = iOrderStoreDeliveryDays; // added on 250119
                            //dr["ACTUALITEMID"] = sActualItemId;
                            cmdCustOrder_Detail.Parameters.Add("@SalesManID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtOrderDetails.Rows[ItemCount]["SalesManID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtOrderDetails.Rows[ItemCount]["QUANTITY"])))
                                cmdCustOrder_Detail.Parameters.Add("@GMADISCWT", SqlDbType.Decimal).Value = 0;
                            else if (dGMADiscPct > 0)
                                cmdCustOrder_Detail.Parameters.Add("@GMADISCWT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["QUANTITY"]) * dGMADiscPct / 100;
                            else
                                cmdCustOrder_Detail.Parameters.Add("@GMADISCWT", SqlDbType.Decimal).Value = 0;

                            cmdCustOrder_Detail.CommandTimeout = 0;
                            iCustOrder_Details = cmdCustOrder_Detail.ExecuteNonQuery();
                            cmdCustOrder_Detail.Dispose();
                        #endregion

                            if (iCustOrder_Details == 1)
                            {
                                if (dtSubOrderDetails != null && dtSubOrderDetails.Rows.Count > 0)
                                {
                                    #region SUB ORDER DETAILS

                                    string commandCust_SubOrderDetails = " INSERT INTO [CUSTORDER_SUBDETAILS]([ORDERNUM],[ORDERDETAILNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE] "
                                                                + " ,[SIZEID],[STYLE],[PCS],[QTY],[CRATE],[RATETYPE],[DATAAREAID],[AMOUNT],[STAFFID],[INVENTDIMID],[UNITID]) VALUES (@ORDERNUM,@ORDERDETAILNUM,@LINENUM, "
                                                                + " @STOREID,@TERMINALID,@ITEMID,@CONFIGID,@CODE,@SIZEID,@STYLE,@PCS,@QTY,@RATE,@RATETYPE,@DATAAREAID,@AMOUNT,@STAFFID,@INVENTDIMID,@UNITID) ";

                                    int i = 1;
                                    int index = 1;
                                    for (int PaymentCount = 0; PaymentCount < dtSubOrderDetails.Rows.Count; PaymentCount++)
                                    {
                                        if (Convert.ToString(dtOrderDetails.Rows[ItemCount]["UNIQUEID"]).Trim() == Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["UNIQUEID"]).Trim())
                                        {
                                            index = i;
                                            //int iLine = iSubOrder_LineNum + 1;
                                            //iSubOrder_LineNum = iLine;
                                            //iLine = iSubOrder_LineNum + 1;
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
                                                cmdOGP_PAYMENT.Parameters.Add("@PCS", SqlDbType.Decimal).Value = 0;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@PCS", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["PCS"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["QUANTITY"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@QTY", SqlDbType.Decimal).Value = 0;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["QUANTITY"]);


                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATE"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@RATE", SqlDbType.Decimal).Value = 0;
                                            else
                                                cmdOGP_PAYMENT.Parameters.Add("@RATE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSubOrderDetails.Rows[PaymentCount]["RATE"]);

                                            if (string.IsNullOrEmpty(Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATETYPE"])))
                                                cmdOGP_PAYMENT.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = DBNull.Value;
                                            else
                                            {
                                                string rType = string.Empty;
                                                if (Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATETYPE"]) == Convert.ToString(RateType.Weight))
                                                    rType = Convert.ToString((int)RateType.Weight);
                                                else if (Convert.ToString(dtSubOrderDetails.Rows[PaymentCount]["RATETYPE"]) == Convert.ToString(RateType.Pcs))
                                                    rType = Convert.ToString((int)RateType.Pcs);
                                                else
                                                    rType = Convert.ToString((int)RateType.Tot);
                                                cmdOGP_PAYMENT.Parameters.Add("@RATETYPE", SqlDbType.NVarChar).Value = Convert.ToString(rType);
                                            }

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

                        #region Insert Into BulkItemTransTable for GMA

                        if (chkGMA.Checked)
                        {
                            int ItemCount = 0;

                            foreach (DataRow dr in dtOrderDetails.Rows)
                            {
                                string strQry = " INSERT INTO [BulkItemTransTable]([TransReceiptId],[LineNumber]," +
                                                      " [TransDate],[TransType],[ItemId],[ConfigId]," +
                                                      " [InventSizeId],[InventColorId],[InventStyleId]," +
                                                      " InventBatchId,[PdsCWQty],[Qty]," +
                                                      " [RetailStaffId],[RetailStoreId]," +
                                                      " [RetailTerminalId],DATAAREAID,GrossWt,NetWt)" +
                                                      " VALUES(@ReceiptId  ,@LineNumber ," +
                                                      " @TransDate,@TransType, @ItemId," +
                                                      " @ConfigId,@InventSizeId,@InventColorId,@InventStyleId," +
                                                      " @InventBatchId,@PdsCWQty,@Qty,@RetailStaffId," +
                                                      " @RetailStoreId,@RetailTerminalId, @DATAAREAID,@GrossWt,@NetWt) ";

                                //for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                                //{
                                using (SqlCommand cmdCustOrder_GMA = new SqlCommand(strQry, connection, transaction))
                                {
                                    cmdCustOrder_GMA.CommandTimeout = 0;
                                    cmdCustOrder_GMA.Parameters.Add("@ReceiptId", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text.Trim();
                                    cmdCustOrder_GMA.Parameters.Add("@LineNumber", SqlDbType.Int).Value = ItemCount + 1;
                                    cmdCustOrder_GMA.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
                                    cmdCustOrder_GMA.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)BulkTransactionType.GMA;
                                    cmdCustOrder_GMA.Parameters.Add("@ItemId", SqlDbType.VarChar, 50).Value = dr["ITEMID"].ToString();
                                    cmdCustOrder_GMA.Parameters.Add("@ConfigId", SqlDbType.VarChar, 50).Value = dr["CONFIGURATION"].ToString();
                                    cmdCustOrder_GMA.Parameters.Add("@InventSizeId", SqlDbType.NVarChar, 50).Value = dr["SIZE"].ToString();
                                    cmdCustOrder_GMA.Parameters.Add("@InventColorId", SqlDbType.NVarChar, 50).Value = dr["COLOR"].ToString();
                                    cmdCustOrder_GMA.Parameters.Add("@InventStyleId", SqlDbType.NVarChar, 50).Value = dr["STYLE"].ToString();

                                    if (IsBatchItem(dr["ITEMID"].ToString(), transaction))
                                    {
                                        string sBatchId = string.Empty;
                                        sBatchId = txtOrderNo.Text.Trim() + "" + Convert.ToString(ItemCount + 1);

                                        cmdCustOrder_GMA.Parameters.Add(new SqlParameter("@InventBatchId", sBatchId));
                                    }
                                    else
                                        cmdCustOrder_GMA.Parameters.Add(new SqlParameter("@InventBatchId", string.Empty));

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["PCS"])))
                                        cmdCustOrder_GMA.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = 0;
                                    else
                                        cmdCustOrder_GMA.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = Convert.ToDecimal(Convert.ToString(dr["PCS"]));

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["QUANTITY"])))
                                        cmdCustOrder_GMA.Parameters.Add("@Qty", SqlDbType.Decimal).Value = 0;
                                    else
                                        cmdCustOrder_GMA.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Convert.ToString(dr["QUANTITY"]));

                                    cmdCustOrder_GMA.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                                    cmdCustOrder_GMA.Parameters.Add("@RetailStoreId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdCustOrder_GMA.Parameters.Add("@RetailTerminalId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdCustOrder_GMA.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdCustOrder_GMA.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QUANTITY"].ToString());
                                    cmdCustOrder_GMA.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QUANTITY"].ToString());
                                    cmdCustOrder_GMA.ExecuteNonQuery();
                                    cmdCustOrder_GMA.Dispose();
                                }

                                ItemCount++;
                                //}
                            }
                        }
                        #endregion

                        #region Sample Details Entry, //Nimbus by MIAM @ 09Jun14 : Added
                        if (dtSampleDetails != null && dtSampleDetails.Rows.Count > 0)
                        {
                            #region CUSTORDSAMPLE
                            StringBuilder commandText1 = new StringBuilder();
                            foreach (DataRow dr in dtSampleDetails.Rows)
                            {
                                commandText1.Append(@"INSERT INTO [CUSTORDSAMPLE]
                                                               ([ORDERNUM]
                                                                ,[LINENUM],[STOREID]
                                                                ,[TERMINALID],[ITEMID]
                                                                ,[CONFIGID],[CODE]
                                                                ,[SIZEID],[STYLE]
                                                                ,[INVENTDIMID],[PCS]
                                                                ,[QTY],[NETTWT]
		                                                        ,[DIAWT],[DIAAMT]
                                                                ,[STNWT],[STNAMT]
                                                                ,[TOTALAMT],[DATAAREAID]
                                                                ,[STAFFID],[UNITID]
                                                                ,[REMARKSDTL]
                                                                ,[ISRETURNED],STOCKSAMPLE)
                                                             VALUES");
                                commandText1.AppendLine();
                                commandText1.AppendFormat("('{0}'", txtOrderNo.Text);
                                commandText1.AppendFormat(",'{0}','{1}'", dr["LINENUM"].ToString(), ApplicationSettings.Terminal.StoreId);
                                commandText1.AppendFormat(",'{0}','{1}'", ApplicationSettings.Terminal.TerminalId, dr["ITEMID"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["CONFIGURATION"].ToString(), dr["COLOR"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["SIZE"].ToString(), dr["STYLE"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["INVENTDIMID"].ToString(), dr["PCS"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["QUANTITY"].ToString(), dr["NETTWT"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["DIAWT"].ToString(), dr["DIAAMT"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["STNWT"].ToString(), dr["STNAMT"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}'", dr["TOTALAMT"].ToString(), ApplicationSettings.Database.DATAAREAID);
                                commandText1.AppendFormat(",'{0}','{1}'", pos.OperatorId, dr["UNITID"].ToString());
                                commandText1.AppendFormat(",'{0}'", dr["RemarksDtl"].ToString());
                                commandText1.AppendFormat(",'{0}','{1}');", Convert.ToInt16(Convert.ToBoolean(dr["ISRETURNED"].ToString())), Convert.ToString(dr["STOCKSAMPLE"].ToString()) == "False" ? 0 : 1);
                                commandText1.AppendLine();
                            }

                            using (SqlCommand cmdCustOrder_SampleDetail = new SqlCommand(commandText1.ToString(), connection, transaction))
                            {
                                cmdCustOrder_SampleDetail.CommandTimeout = 0;
                                cmdCustOrder_SampleDetail.ExecuteNonQuery();
                                cmdCustOrder_SampleDetail.Dispose();
                            }
                            #endregion

                            #region Insert Into BulkItemTransTable for sample
                            foreach (DataRow dr in dtSampleDetails.Rows)
                            {
                                string strQry = " INSERT INTO [BulkItemTransTable]([TransReceiptId],[LineNumber]," +
                                                      " [TransDate],[TransType],[ItemId],[ConfigId]," +
                                                      " [InventSizeId],[InventColorId],[InventStyleId]," +
                                                      " InventBatchId,[PdsCWQty],[Qty]," +
                                                      " [RetailStaffId],[RetailStoreId]," +
                                                      " [RetailTerminalId],DATAAREAID,GrossWt,NetWt)" +
                                                      " VALUES(@ReceiptId  ,@LineNumber ," +
                                                      " @TransDate,@TransType, @ItemId," +
                                                      " @ConfigId,@InventSizeId,@InventColorId,@InventStyleId," +
                                                      " @InventBatchId,@PdsCWQty,@Qty,@RetailStaffId," +
                                                      " @RetailStoreId,@RetailTerminalId, @DATAAREAID,@GrossWt,@NetWt) ";

                                using (SqlCommand cmdCustOrder_Sample = new SqlCommand(strQry, connection, transaction))
                                {
                                    cmdCustOrder_Sample.CommandTimeout = 0;
                                    cmdCustOrder_Sample.Parameters.Add("@ReceiptId", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text.Trim();
                                    cmdCustOrder_Sample.Parameters.Add("@LineNumber", SqlDbType.Int).Value = Convert.ToInt16(Convert.ToDecimal(dr["LINENUM"].ToString()));
                                    cmdCustOrder_Sample.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
                                    cmdCustOrder_Sample.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)BulkTransactionType.OrderSampleReceive;
                                    cmdCustOrder_Sample.Parameters.Add("@ItemId", SqlDbType.VarChar, 50).Value = dr["ITEMID"].ToString();
                                    cmdCustOrder_Sample.Parameters.Add("@ConfigId", SqlDbType.VarChar, 50).Value = dr["CONFIGURATION"].ToString();
                                    cmdCustOrder_Sample.Parameters.Add("@InventSizeId", SqlDbType.NVarChar, 50).Value = dr["SIZE"].ToString();
                                    cmdCustOrder_Sample.Parameters.Add("@InventColorId", SqlDbType.NVarChar, 50).Value = dr["COLOR"].ToString();
                                    cmdCustOrder_Sample.Parameters.Add("@InventStyleId", SqlDbType.NVarChar, 50).Value = dr["STYLE"].ToString();

                                    if (IsBatchItem(dr["ITEMID"].ToString(), transaction))
                                    {
                                        string sBatchId = string.Empty;
                                        sBatchId = txtOrderNo.Text.Trim() + "" + dr["LINENUM"].ToString(); ;

                                        cmdCustOrder_Sample.Parameters.Add(new SqlParameter("@InventBatchId", sBatchId));
                                    }
                                    else
                                        cmdCustOrder_Sample.Parameters.Add(new SqlParameter("@InventBatchId", string.Empty));

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["PCS"])))
                                        cmdCustOrder_Sample.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = 0;
                                    else
                                        cmdCustOrder_Sample.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = Convert.ToDecimal(Convert.ToString(dr["PCS"]));

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["QUANTITY"])))
                                        cmdCustOrder_Sample.Parameters.Add("@Qty", SqlDbType.Decimal).Value = 0;
                                    else
                                        cmdCustOrder_Sample.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Convert.ToString(dr["QUANTITY"]));

                                    cmdCustOrder_Sample.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                                    cmdCustOrder_Sample.Parameters.Add("@RetailStoreId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdCustOrder_Sample.Parameters.Add("@RetailTerminalId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdCustOrder_Sample.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdCustOrder_Sample.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QUANTITY"].ToString());
                                    cmdCustOrder_Sample.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["NETTWT"].ToString());

                                    cmdCustOrder_Sample.ExecuteNonQuery();
                                    cmdCustOrder_Sample.Dispose();
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region Receive Stone or loose Diamond Details Entry,
                        if (dtRecvStoneDetails != null && dtRecvStoneDetails.Rows.Count > 0)
                        {
                            #region Stone or dmd recv
                            int iIsreturn = 0;
                            int iL = 1;
                            foreach (DataRow dr in dtRecvStoneDetails.Rows)
                            {
                                string QRY = "INSERT INTO [CUSTORDSTONE] " +
                                           "([ORDERNUM],[LINENUM],[REFLINENUM],[STOREID],[TERMINALID],[ITEMID],[CODE],[SIZEID],[STYLE] " +
                                           " ,[PCS],[QTY],[NETWT],[DATAAREAID] " +
                                           ",[STAFFID],[REMARKSDTL],[ISRETURNED],[STONEBATCHID],[UNITID]) " +
                                           " VALUES" +
                                           "(@OrderNum,@LineNUM,@REFLINENUM,@STOREID,@TERMINALID,@ITEMID,@Code,@sizeid,@STYLE,@PCS," +
                                           " @QTY,@NETWT,@DATAAREAID," +
                                           " @STAFFID,@REMARKSDTL,@ISRETURNED,@STONEBATCHID,@UNITID)";

                                using (SqlCommand cmdCustOrder_StoneDetail = new SqlCommand(QRY, connection, transaction))
                                {
                                    cmdCustOrder_StoneDetail.CommandTimeout = 0;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@ITEMID", SqlDbType.VarChar, 50).Value = dr["itemID"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STAFFID", SqlDbType.VarChar, 50).Value = pos.OperatorId;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QTY"].ToString());
                                    cmdCustOrder_StoneDetail.Parameters.Add("@NETWT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["NETWT"].ToString());
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STOREID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.StoreID;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@TERMINALID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.TerminalID;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@RECVDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@OrderNum", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text.Trim();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@PCS", SqlDbType.Int).Value = Convert.ToInt16(Convert.ToDecimal(dr["PCS"].ToString()));
                                    cmdCustOrder_StoneDetail.Parameters.Add("@LineNUM", SqlDbType.Int).Value = Convert.ToInt16(Convert.ToDecimal(dr["LineNum"].ToString()));
                                    cmdCustOrder_StoneDetail.Parameters.Add("@REFLINENUM", SqlDbType.Int).Value = Convert.ToInt16(Convert.ToDecimal(dr["REFLINENUM"].ToString()));

                                    cmdCustOrder_StoneDetail.Parameters.Add("@REMARKSDTL", SqlDbType.NVarChar, 250).Value = dr["RemarksDtl"].ToString();
                                    if (Convert.ToBoolean(dr["ISRETURNED"]))
                                        iIsreturn = 1;
                                    else
                                        iIsreturn = 0;

                                    cmdCustOrder_StoneDetail.Parameters.Add("@ISRETURNED", SqlDbType.Int).Value = iIsreturn;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@Code", SqlDbType.NVarChar, 50).Value = dr["COLOR"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@sizeid", SqlDbType.NVarChar, 50).Value = dr["Size"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STYLE", SqlDbType.NVarChar, 50).Value = dr["STYLE"].ToString();
                                    cmdCustOrder_StoneDetail.Parameters.Add("@STONEBATCHID", SqlDbType.NVarChar, 50).Value = dr["STONEBATCHID"].ToString(); //txtOrderNo.Text.Trim() + "/" + Convert.ToInt32(dr["LineNum"].ToString()) + "/" + iL;
                                    cmdCustOrder_StoneDetail.Parameters.Add("@UNITID", SqlDbType.NVarChar, 50).Value = dr["UNITID"].ToString();

                                    cmdCustOrder_StoneDetail.ExecuteNonQuery();
                                    cmdCustOrder_StoneDetail.Dispose();
                                    iL++;
                                }
                            }
                            #endregion

                            #region Insert Into BulkItemTransTable for stn or dmd

                            foreach (DataRow dr in dtRecvStoneDetails.Rows)
                            {
                                string strQry = " INSERT INTO [BulkItemTransTable]([TransReceiptId],[LineNumber]," +
                                                      " [TransDate],[TransType],[ItemId],[ConfigId]," +
                                                      " [InventSizeId],[InventColorId],[InventStyleId]," +
                                                      " InventBatchId,[PdsCWQty],[Qty]," +
                                                      " [RetailStaffId],[RetailStoreId]," +
                                                      " [RetailTerminalId],DATAAREAID,GrossWt,NetWt)" +
                                                      " VALUES(@ReceiptId  ,@LineNumber ," +
                                                      " @TransDate,@TransType, @ItemId," +
                                                      " @ConfigId,@InventSizeId,@InventColorId,@InventStyleId," +
                                                      " @InventBatchId,@PdsCWQty,@Qty,@RetailStaffId," +
                                                      " @RetailStoreId,@RetailTerminalId, @DATAAREAID,@GrossWt,@NetWt) ";

                                using (SqlCommand cmdCustOrder_StnOrDmd = new SqlCommand(strQry, connection, transaction))
                                {
                                    cmdCustOrder_StnOrDmd.CommandTimeout = 0;
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@ReceiptId", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text.Trim();
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@LineNumber", SqlDbType.Int).Value = Convert.ToInt16(Convert.ToDecimal(dr["LINENUM"].ToString()));
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)BulkTransactionType.OrderStnDmdReceive;
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@ItemId", SqlDbType.VarChar, 50).Value = dr["ITEMID"].ToString();
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@ConfigId", SqlDbType.VarChar, 50).Value = "";
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@InventSizeId", SqlDbType.NVarChar, 50).Value = dr["SIZE"].ToString();
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@InventColorId", SqlDbType.NVarChar, 50).Value = dr["COLOR"].ToString();
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@InventStyleId", SqlDbType.NVarChar, 50).Value = dr["STYLE"].ToString();
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@InventBatchId", SqlDbType.NVarChar, 50).Value = dr["STONEBATCHID"].ToString();

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["PCS"])))
                                        cmdCustOrder_StnOrDmd.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = 0;
                                    else
                                        cmdCustOrder_StnOrDmd.Parameters.Add("@PdsCWQty", SqlDbType.Decimal).Value = Convert.ToDecimal(Convert.ToString(dr["PCS"]));

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Qty"])))
                                        cmdCustOrder_StnOrDmd.Parameters.Add("@Qty", SqlDbType.Decimal).Value = 0;
                                    else
                                        cmdCustOrder_StnOrDmd.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(Convert.ToString(dr["Qty"]));

                                    cmdCustOrder_StnOrDmd.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@RetailStoreId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@RetailTerminalId", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QTY"].ToString());
                                    cmdCustOrder_StnOrDmd.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["NETWT"].ToString());

                                    cmdCustOrder_StnOrDmd.ExecuteNonQuery();
                                    cmdCustOrder_StnOrDmd.Dispose();
                                }
                            }
                            #endregion
                        }

                        #endregion

                        #region Order payment schedule
                        if (dtPaySchedule != null && dtPaySchedule.Rows.Count > 0)
                        {

                            int iIsreturn = 0;
                            int iL = 1;
                            foreach (DataRow dr in dtPaySchedule.Rows)
                            {
                                string QRY = "INSERT INTO [CUSTORDERPAYSCHEDULE] " +
                                           "([ORDERNUM],[STOREID],[TERMINALID],[ORDERDATE],[PAYDATE],[PEROFAMT],[AMOUNT]) " +
                                           " VALUES" +
                                           "(@ORDERNUM,@STOREID,@TERMINALID,@ORDERDATE,@PAYDATE,@PEROFAMT," +
                                           " @AMOUNT)";

                                using (SqlCommand cmdCustOrder_PaySch = new SqlCommand(QRY, connection, transaction))
                                {
                                    cmdCustOrder_PaySch.CommandTimeout = 0;
                                    cmdCustOrder_PaySch.Parameters.Add("@ORDERNUM", SqlDbType.VarChar, 50).Value = txtOrderNo.Text;
                                    cmdCustOrder_PaySch.Parameters.Add("@STOREID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdCustOrder_PaySch.Parameters.Add("@TERMINALID", SqlDbType.VarChar, 50).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdCustOrder_PaySch.Parameters.Add("@ORDERDATE", SqlDbType.DateTime).Value = dTPickerOrderDate.Value;
                                    cmdCustOrder_PaySch.Parameters.Add("@PAYDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dr["PAYDATE"].ToString());
                                    cmdCustOrder_PaySch.Parameters.Add("@PEROFAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["PerAmt"].ToString());
                                    cmdCustOrder_PaySch.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["Amount"].ToString());
                                    cmdCustOrder_PaySch.ExecuteNonQuery();
                                    cmdCustOrder_PaySch.Dispose();
                                    iL++;
                                }
                            }
                        }
                        #endregion

                        #region Order chq info
                        if (dtChequeInfo != null && dtChequeInfo.Rows.Count > 0)
                        {
                            int iL = 1;
                            foreach (DataRow dr in dtChequeInfo.Rows)
                            {
                                string chqQRY = "INSERT INTO [CUSTORDERCHQINFO] " +
                                           "([ORDERNUM],[STOREID],[TERMINALID]," +
                                           " [CHEQUENO],[CHQDATE],[CHQAMT],[BANK],[BRANCH],[IFSCCODE],[MICRNO],DATAAREAID) " +
                                           " VALUES" +
                                           "(@ORDERNUM,@STOREID,@TERMINALID,@CHEQUENO,@CHQDATE,@CHQAMT," +
                                           " @BANK,@BRANCH,@IFSCCODE,@MICRNO,@DATAAREAID)";
                                using (SqlCommand cmdCustOrder_ChqInfo = new SqlCommand(chqQRY, connection, transaction))
                                {
                                    cmdCustOrder_ChqInfo.CommandTimeout = 0;
                                    cmdCustOrder_ChqInfo.Parameters.Add("@ORDERNUM", SqlDbType.VarChar, 20).Value = txtOrderNo.Text;
                                    cmdCustOrder_ChqInfo.Parameters.Add("@STOREID", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdCustOrder_ChqInfo.Parameters.Add("@TERMINALID", SqlDbType.VarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdCustOrder_ChqInfo.Parameters.Add("@CHEQUENO", SqlDbType.VarChar, 6).Value = Convert.ToString(dr["CHEQUENO"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@CHQDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dr["CHQDATE"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@CHQAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["CHQAMT"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@BANK", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["BANK"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@BRANCH", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["BRANCH"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@IFSCCODE", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["IFSCCODE"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@MICRNO", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["MICRNO"].ToString());
                                    cmdCustOrder_ChqInfo.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdCustOrder_ChqInfo.ExecuteNonQuery();
                                    cmdCustOrder_ChqInfo.Dispose();
                                    iL++;
                                }
                            }
                        }
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();

                //Updating order with sketch
                if (dtSketchDetails != null && dtSketchDetails.Rows.Count > 0)
                {
                    NIM_OrderUpdateWithSketch();
                }
                if (dtSampleSketch != null && dtSampleSketch.Rows.Count > 0)
                {
                    NIM_SaveOrderSampleSketch();
                }

                if (iCustOrder_Header == 1 || iCustOrder_Details == 1)
                {
                    #region LINQ Qry ITEM, FROMCOUNTER wise
                    DataTable tblProduct = new DataTable("Product");
                    tblProduct.Columns.Add("ITEMID", typeof(string));
                    tblProduct.Columns.Add("FROMCOUNTER", typeof(string));
                    tblProduct.Columns.Add("QUANTITY", typeof(decimal));
                    DataTable dtGroupByFromCounter = new DataTable("dtGroupByFromCounter");


                    if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
                    {
                        for (int ItemCount = 0; ItemCount < dtOrderDetails.Rows.Count; ItemCount++)
                        {
                            if (Convert.ToInt32(dtOrderDetails.Rows[ItemCount]["IsBookedSKU"]) == 1)
                            {
                                string sItemFROMCOUNTER = GetFromCounter(Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]));

                                tblProduct.Rows.Add(Convert.ToString(dtOrderDetails.Rows[ItemCount]["ITEMID"]),
                                    sItemFROMCOUNTER, Convert.ToDecimal(dtOrderDetails.Rows[ItemCount]["QUANTITY"]));
                            }
                        }
                    }

                    var query1 =
                           tblProduct.AsEnumerable()
                           .Select(x =>
                               new
                               {
                                   ITEMID = x["ITEMID"],
                                   FROMCOUNTER = x["FROMCOUNTER"],
                                   QUANTITY = x["QUANTITY"],
                               }
                            )
                            .GroupBy(s => new { s.ITEMID, s.FROMCOUNTER })
                            .Select(groupedTable =>
                                   new
                                   {
                                       ITEMID = groupedTable.Key.ITEMID,
                                       FROMCOUNTER = groupedTable.Key.FROMCOUNTER,
                                       QUANTITY = groupedTable.Sum(x => Math.Round(Convert.ToDecimal(x.QUANTITY), 3)),
                                   }
                            );
                    dtBookedGroup = ConvertToDataTable(query1);


                    var query2 =
                           tblProduct.AsEnumerable()
                           .Select(x =>
                               new
                               {
                                   FROMCOUNTER = x["FROMCOUNTER"],
                                   QUANTITY = x["QUANTITY"],
                               }
                            )
                            .GroupBy(s => new { s.FROMCOUNTER })
                            .Select(groupedTable =>
                                   new
                                   {
                                       FROMCOUNTER = groupedTable.Key.FROMCOUNTER,
                                       QUANTITY = groupedTable.Sum(x => Math.Round(Convert.ToDecimal(x.QUANTITY), 3)),
                                   }
                            );
                    dtFromCounterGroup = ConvertToDataTable(query2);

                    if (dtFromCounterGroup != null && dtFromCounterGroup.Rows.Count > 0)
                    {
                        string TransId = string.Empty;
                        string sTransitCounter = getTransitCounter();

                        if (dtBookedGroup != null && dtBookedGroup.Rows.Count > 0)
                        {
                            for (int bookedItemCount = 0; bookedItemCount < dtFromCounterGroup.Rows.Count; bookedItemCount++)
                            {
                                dtGroupByFromCounter = new DataTable("dtGroupByFromCounter");
                                dtGroupByFromCounter.Columns.Add("ITEMID", typeof(string));
                                dtGroupByFromCounter.Columns.Add("QUANTITY", typeof(decimal));
                                string sFCounter = "";
                                sFCounter = Convert.ToString(dtFromCounterGroup.Rows[bookedItemCount]["FROMCOUNTER"]);

                                foreach (DataRow mmRow in dtBookedGroup.Select("FROMCOUNTER='" + Convert.ToString(dtFromCounterGroup.Rows[bookedItemCount]["FROMCOUNTER"]) + "'"))
                                {
                                    dtGroupByFromCounter.Rows.Add(Convert.ToString(mmRow["ITEMID"]),
                                         Convert.ToDecimal(mmRow["QUANTITY"]));

                                }
                                if (dtGroupByFromCounter != null && dtGroupByFromCounter.Rows.Count > 0)
                                {
                                    for (int gCount = 0; gCount < dtGroupByFromCounter.Rows.Count; gCount++)
                                    {
                                        #region Update
                                        if (!string.IsNullOrEmpty(Convert.ToString(dtGroupByFromCounter.Rows[gCount]["ITEMID"])))
                                        {
                                            SqlConnection conn = new SqlConnection();
                                            if (application != null)
                                                conn = application.Settings.Database.Connection;
                                            else
                                                conn = ApplicationSettings.Database.LocalConnection;

                                            foreach (DataRow drTotal in dtGroupByFromCounter.Rows)
                                            {
                                                string sqlUpd = " UPDATE SKUTableTrans ";
                                                sqlUpd = sqlUpd + " SET ";
                                                sqlUpd = sqlUpd + " TOCOUNTER = '" + sTransitCounter + "'";
                                                sqlUpd = sqlUpd + " ,FROMCOUNTER = '" + sFCounter + "'";
                                                sqlUpd = sqlUpd + " ,BOOKEDORDERNO = '" + txtOrderNo.Text.Trim() + "'";
                                                sqlUpd = sqlUpd + " WHERE SKUNUMBER = '" + Convert.ToString(dtGroupByFromCounter.Rows[gCount]["ITEMID"]) + "'";

                                                DBUtil dbUtil = new DBUtil(conn);
                                                dbUtil.Execute(sqlUpd);
                                            }

                                            if (conn.State == ConnectionState.Open)
                                                conn.Close();
                                        }
                                        #endregion
                                    }
                                }

                                SaveStckTransfer(Convert.ToString(dtFromCounterGroup.Rows[bookedItemCount]["FROMCOUNTER"]), dtGroupByFromCounter);
                            }
                        }
                    }
                    #endregion =============================

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Order " + txtOrderNo.Text + " has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        //try
                        //{
                        //    PrintVoucher();
                        //}
                        //catch { }

                        if (IsGMA(txtOrderNo.Text))
                        {
                            try
                            {
                                frmOfflineBillPrint objVoucher = new frmOfflineBillPrint(pos, txtOrderNo.Text, 0, 1);
                                objVoucher.ShowDialog();

                                //frmOfflineBillPrint objVoucher1 = new frmOfflineBillPrint(pos, txtOrderNo.Text, 1, 1);
                                //objVoucher1.ShowDialog();

                            }
                            catch { }
                        }

                        sCustOrder = txtOrderNo.Text;
                        txtOrderNo.Text = GetOrderNum();
                        dtOrderDetails = new DataTable();
                        dtSubOrderDetails = new DataTable();
                        sOrderDetailsAmount = string.Empty;
                        sSubOrderDetailsAmount = string.Empty;
                        sCustOrderSearchNumber = string.Empty;
                        dsOrderSearched = new DataSet();
                        sCustAcc = txtCustomerAccount.Text;
                        sTotalAmt = txtTotalAmount.Text;
                        CLearControls();
                        bDataSaved = true;
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

        private bool IsGMA(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GMA  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        #region Search Order
        private void btnSearchOrder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                DataTable dtGridItems = new DataTable();

                string commandText = " SELECT CUSTACCOUNT,ORDERNUM,CONVERT(VARCHAR(15),ORDERDATE,103) AS ORDERDATE ,";
                commandText += " CONVERT(VARCHAR(15),DELIVERYDATE,103) AS DELIVERYDATE , ";
                commandText += " CUSTNAME ,CONVERT(VARCHAR(15),TOTALAMOUNT) AS TOTALAMOUNT,";
                commandText += " SalesManID, CONVERT(VARCHAR(15),REQUESTDELIVERYDATE,103) AS REQUESTDELIVERYDATE,";
                commandText += " isnull(REASONFORREQUESTDELIVERYDATE,'') REASONFORREQUESTDELIVERYDATE,";
                commandText += " isnull(ORDERREMARKS,'') ORDERREMARKS,SUVARNAVRUDHI,isnull(FESTIVECODE,'') FESTIVECODE";
                commandText += " FROM CUSTORDER_HEADER";

                if (!string.IsNullOrEmpty(txtCustomerAccount.Text))
                {
                    commandText += " where CUSTACCOUNT='" + txtCustomerAccount.Text.Trim() + "'";
                }
                commandText += " ORDER BY ORDERNUM ";

                SqlConnection connection = new SqlConnection();

                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }


                SqlCommand command = new SqlCommand(commandText, connection);


                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                dtGridItems = new DataTable();
                dtGridItems.Load(reader);
                if (dtGridItems != null && dtGridItems.Rows.Count > 0)
                {
                    DataRow selRow = null;
                    Dialog objCustOrderSearch = new Dialog();

                    objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Customer Order");
                    if (selRow != null)
                    {
                        sCustOrderSearchNumber = Convert.ToString(selRow["ORDERNUM"]);
                        sTotalAmt = Convert.ToString(selRow["TOTALAMOUNT"]);
                        btnPrint.Enabled = true;
                        btnCashierTask.Enabled = true;
                    }
                    else
                    {
                        btnPrint.Enabled = false;
                        btnCashierTask.Enabled = false;
                        return;
                    }
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

                string commandText = " SELECT ORDERNUM, ORDERDATE, DELIVERYDATE, CUSTACCOUNT, CUSTNAME,CUSTADDRESS,CUSTPHONE,TOTALAMOUNT,SalesManID," +
                                     " CONVERT(VARCHAR(15),REQUESTDELIVERYDATE,103) AS REQUESTDELIVERYDATE," +
                                     " isnull(REASONFORREQUESTDELIVERYDATE,'') REASONFORREQUESTDELIVERYDATE," +
                                     " isnull(ORDERREMARKS,'') ORDERREMARKS,SUVARNAVRUDHI,isnull(FESTIVECODE,'') FESTIVECODE,"+
                                     "  isnull(GMA,0) GMA,ISNULL(GMASCHEMECODE,'') GMASCHEMECODE FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sCustOrderSearchNumber.Trim() + "'; " +
                                     " SELECT ORDERNUM, LINENUM, ITEMID, CONFIGID, CODE, SIZEID, STYLE, PCS, QTY AS QUANTITY, CRATE AS RATE, " +
                                     " CASE WHEN RATETYPE=0 THEN '" + RateType.Weight + "' WHEN RATETYPE=1 THEN '" + RateType.Pcs + "' WHEN RATETYPE=2 THEN '" + RateType.Tot + "' END AS RATETYPE, " +
                                     " AMOUNT, MAKINGRATE, " +
                                     " CASE WHEN MAKINGRATETYPE=0 THEN '" + MakingType.Weight + "' WHEN MAKINGRATETYPE=1 THEN '" + MakingType.Pieces + "' WHEN MAKINGRATETYPE=2 THEN '" + MakingType.Tot + "' WHEN MAKINGRATETYPE=3 THEN '" + MakingType.Percentage + "' END AS MAKINGTYPE, " +
                    //   " MAKINGAMOUNT, EXTENDEDDETAILSAMOUNT AS EXTENDEDDETAILS, (ISNULL(AMOUNT,0) + ISNULL(MAKINGAMOUNT,0) + ISNULL(EXTENDEDDETAILSAMOUNT,0)) AS ROWTOTALAMOUNT," +
                                     " MAKINGAMOUNT, EXTENDEDDETAILSAMOUNT AS EXTENDEDDETAILS, (ISNULL(AMOUNT,0) + ISNULL(MAKINGAMOUNT,0) + ISNULL(WASTAGEAMOUNT,0)) AS ROWTOTALAMOUNT," +
                                     " CASE WHEN WastageType=0 THEN '" + WastageType.Weight + "' WHEN WastageType=1 THEN '" + WastageType.Percentage + "' END AS WastageType" +
                                     " ,ISNULL(WastageRate,0) AS WastageRate, ISNULL(WastageQty,0) AS WastageQty, ISNULL(WastagePercentage,0) AS WastagePercentage,ISNULL(WastageAmount,0) AS WastageAmount,CAST(ISNULL(IsBookedSKU,0) AS BIT) AS IsBookedSKU" +
                                     " ,SKETCH FROM CUSTORDER_DETAILS WHERE ORDERNUM ='" + sCustOrderSearchNumber.Trim() + "'; " +
                                     " SELECT ORDERNUM, ORDERDETAILNUM, LINENUM, ITEMID, CONFIGID, CODE, SIZEID, STYLE, PCS, QTY AS QUANTITY, CRATE AS RATE, " +
                                     " CASE WHEN RATETYPE=0 THEN '" + RateType.Weight + "' WHEN RATETYPE=1 THEN '" + RateType.Pcs + "' WHEN RATETYPE=2 THEN '" + RateType.Tot + "' END AS RATETYPE, " +
                                     " AMOUNT FROM CUSTORDER_SUBDETAILS WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'; " +

                                     " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                     " ,[CONFIGID] [CONFIGURATION],[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[INVENTDIMID],[PCS],[QTY] [QUANTITY],[NETTWT],[DIAWT],[DIAAMT],[STNWT] " +
                                     " ,[STNAMT],[TOTALAMT],[DATAAREAID],[STAFFID],[REPLICATIONCOUNTER],[UNITID],[REMARKSDTL],[ISRETURNED]  FROM [CUSTORDSAMPLE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "' " +

                                     //" SELECT ORDERNUM, LINENUM, ITEMID, CONFIGID as CONFIGURATION, CODE as COLOR, SIZEID, STYLE, PCS, QTY AS QUANTITY, " +
                    //" REMARKS FROM CUSTORDERSTONEADVANCE WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'";

                                        " SELECT [ORDERNUM],[LINENUM],REFLINENUM,[STOREID],[TERMINALID],[ITEMID] " +
                                       " ,[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[PCS],[QTY] " +
                                       " ,[NETWT],[DATAAREAID],[STAFFID],[REMARKSDTL],[ISRETURNED],[STONEBATCHID],UNITID" +
                                       " FROM [CUSTORDSTONE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'" +

                                       " SELECT " +
                                       " [PAYDATE] as PayDate ,[PEROFAMT] as PerAmt ,[AMOUNT] as Amount  " +
                                       " FROM [CUSTORDERPAYSCHEDULE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'"+
                                    
                                       " SELECT " +
                                       " CHEQUENO,CHQDATE,CHQAMT,BANK,BRANCH,IFSCCODE,MICRNO  " +
                                       " FROM [CUSTORDERCHQINFO] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'";


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
                    //btnStoneAdv.Enabled = false;
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
                    txtSM.Tag = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["SalesManID"]);
                    txtSM.Text = getSalesManName(Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["SalesManID"]));

                    dtpReqDeliveryDate.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["REQUESTDELIVERYDATE"]);
                    txtReasonForDateChange.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["REASONFORREQUESTDELIVERYDATE"]);
                    txtRemarks.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["ORDERREMARKS"]);

                    if (Convert.ToInt16(dsOrderSearched.Tables[0].Rows[0]["SUVARNAVRUDHI"]) == 1)
                        chkSUVARNAVRUDDHI.Checked = true;
                    else
                        chkSUVARNAVRUDDHI.Checked = false;

                    if (Convert.ToInt16(dsOrderSearched.Tables[0].Rows[0]["GMA"]) == 1)
                        chkGMA.Checked = true;
                    else
                        chkGMA.Checked = false;

                    cmbGMA.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["GMASCHEMECODE"]);
                    cmbSVSchemeCode.Text = Convert.ToString(dsOrderSearched.Tables[0].Rows[0]["FESTIVECODE"]);

                    dtOrderDetails = new DataTable();

                    dtChequeInfo = dsOrderSearched.Tables[6];

                }
            }


        }
        #endregion

        #region CLOSE CLICK
        private void btnClose_Click(object sender, EventArgs e)
        {
            if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtOrderDetails.Rows)
                {
                    if (Convert.ToBoolean(dr["IsBookedSKU"]))
                    {
                        SKUInfo(Convert.ToString(dr["ITEMID"]), false);
                    }
                }
            }
            iCancel = 1;
            this.Close();
        }
        #endregion

        private string GetFromCounter(string sItemId)
        {
            string sCName = string.Empty;
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Open)
                conn.Close();

            string sQry = "select TOCOUNTER from SKUTableTrans where SkuNumber= '" + sItemId.Trim() + "'";

            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sCName;

        }

        private string getSalesManName(string sStaffId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select d.NAME as Name from RETAILSTAFFTABLE r" +
                                " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID" +
                                " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
                                " where r.STAFFID='" + sStaffId + "'";

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

        #region isValid()
        private bool isValid()
        {
            if (string.IsNullOrEmpty(sCustOrderSearchNumber))
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
                if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dTPickerOrderDate.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Delivery Date cannot be less than Order Date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        dtPickerDeliveryDate.Text = dTPickerOrderDate.Text;
                        return false;
                    }

                }
                if (string.IsNullOrEmpty(txtSM.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Sales man can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        btnSM.Focus();
                        return false;
                    }
                }

                if (chkSUVARNAVRUDDHI.Checked)
                {
                    if (string.IsNullOrEmpty(cmbSVSchemeCode.Text.Trim()))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Suvarna vridhi scheme code can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            btnSVSchemeSearch.Focus();
                            return false;
                        }
                    }
                }

                if (Convert.ToDateTime(dtPickerDeliveryDate.Text) != Convert.ToDateTime(dtpReqDeliveryDate.Text) && string.IsNullOrEmpty(txtReasonForDateChange.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Enter reason for change the delivery date", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        dtpReqDeliveryDate.Focus();
                        return false;
                    }
                }
                if (Convert.ToDateTime(dtPickerDeliveryDate.Text) < Convert.ToDateTime(dtpReqDeliveryDate.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DELIVERY DATE CANNOT BE LESS THAN ORDER REQUEST DELIVERY DATE.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        dtpReqDeliveryDate.Focus();
                        return false;
                    }
                }

                if (Convert.ToDateTime(dTPickerOrderDate.Text) > Convert.ToDateTime(dtpReqDeliveryDate.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("ORDER DATE CANNOT BE GREATER THAN ORDER REQUEST DELIVERY DATE.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        dtpReqDeliveryDate.Focus();
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region Clear Controls
        private void CLearControls()
        {
            txtCustomerName.Text = string.Empty;
            txtCustomerAccount.Text = string.Empty;
            txtCustomerAddress.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtTotalAmount.Text = string.Empty;
            dTPickerOrderDate.Text = DateTime.Now.ToString();
            dtPickerDeliveryDate.Text = DateTime.Now.ToString();
            btnPrint.Enabled = false;
            txtRemarks.Text = "";
            txtReasonForDateChange.Text = "";
            dtpReqDeliveryDate.Text = DateTime.Now.ToString();
            //chkSUVARNAVRUDDHI.Checked = false;
            cmbSVSchemeCode.Text = "";
            if (string.IsNullOrEmpty(txtCustomerAccount.Text))
            {
                cmbAdvAgainst.Enabled = false;
                cmbVoucher.Enabled = false;
            }
            else
            {
                cmbAdvAgainst.Enabled = true;
                cmbVoucher.Enabled = true;
            }

            chkGMA.Checked = false;
            btnGMASchemeSearch.Enabled = false;
            cmbGMA.Text = "";
        }
        #endregion

        #region Clear CLick
        private void btnClear_Click(object sender, EventArgs e)
        {
            if (dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtOrderDetails.Rows)
                {
                    if (Convert.ToBoolean(dr["IsBookedSKU"]))
                    {
                        SKUInfo(Convert.ToString(dr["ITEMID"]), false);
                    }
                }
            }

            dtOrderDetails = new DataTable();
            dtSubOrderDetails = new DataTable();
            sOrderDetailsAmount = string.Empty;
            sSubOrderDetailsAmount = string.Empty;
            sCustOrderSearchNumber = string.Empty;
            dsOrderSearched = new DataSet();
            txtOrderNo.Text = GetOrderNum();
            panel2.Enabled = true;
            btnStoneAdv.Enabled = true;
            btnCustomerSearch.Enabled = true;
            btnAddCustomer.Enabled = true;
            btnSave.Enabled = true;
            CLearControls();
        }
        #endregion

        private int getOrderLeadDays()
        {
            int iReturn = 0;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT top 1 isnull(CUSTORDERLEADDAYS,0) FROM RETAILPARAMETERS"; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            iReturn = Convert.ToInt16(command.ExecuteScalar());

            return iReturn;
        }

        private decimal GetFixedMetalRate(ref string sConfigId) // Fixed Metal Rate New
        {
            decimal dMetalRate = 0m;
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");//INVENTPARAMETERS

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS NUMERIC(28,2))AS RATES,@CONFIGID FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 AND RETAIL = 0 "); // RATETYPE -- > SALE
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            //  enum RateTypeNew
            //   {
            //       Purchase = 0,
            //       OGP = 1,
            //       OGOP = 2,
            //       Sale = 3,
            //       GSS = 4,
            //       Exchange = 6,
            //   }

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dMetalRate = Convert.ToDecimal(reader.GetValue(0));
                    sConfigId = Convert.ToString(reader.GetValue(1));
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return dMetalRate;
        }

        private bool SKUInfo(string sItemId, bool bMode)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if (bMode)
            {
                commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS   (SELECT TOP(1) [SkuNumber]");
                commandText.Append(" FROM [SKUTableTrans] WHERE  [SkuNumber] = '" + sItemId + "'");
                commandText.Append(" AND isAvailable='True' AND isLocked='False' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "') ");
                commandText.Append(" BEGIN SET @IsSKU = 1  UPDATE SKUTableTrans SET isAvailable='False',");
                commandText.Append(" isLocked='False',BOOKEDORDERNO='" + txtOrderNo.Text + "'");//BOOKEDORDERNO
                commandText.Append(" WHERE SkuNumber = '" + sItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' END SELECT @IsSKU");
            }
            else
            {
                commandText.Append("DECLARE @IsSKU AS INT; SET @IsSKU = 1; UPDATE SKUTableTrans SET isAvailable='True',isLocked='False'");
                commandText.Append(" ,BOOKEDORDERNO='' WHERE SkuNumber = '" + sItemId + "' AND");
                commandText.Append(" DATAAREAID='" + application.Settings.Database.DataAreaID + "'  SELECT @IsSKU");
            }


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }

        private void btnStoneAdvance_Click(object sender, EventArgs e)
        {
            btnClear_Click(sender, e);

            #region stop stn adv against order, start against line
            ////frmStoneAdvance objOrderStnAdv = null;

            ////if(dtOrderDetails != null && dtOrderDetails.Rows.Count > 0)
            ////{
            ////    if(dtOrderStnAdv != null && dtOrderStnAdv.Rows.Count > 0)
            ////    {
            ////        objOrderStnAdv = new frmStoneAdvance(dtOrderStnAdv, pos, application, this);
            ////    }
            ////    else
            ////    {
            ////        dtOrderStnAdv = new DataTable();
            ////        objOrderStnAdv = new frmStoneAdvance(pos, application, this);
            ////    }
            ////    objOrderStnAdv.ShowDialog();
            ////}
            ////else if(dsOrderSearched != null && dsOrderSearched.Tables.Count > 0 && dsOrderSearched.Tables[4].Rows.Count > 0)
            ////{
            ////    objOrderStnAdv = new frmStoneAdvance(dsOrderSearched, pos, application, this);
            ////    objOrderStnAdv.ShowDialog();
            ////}
            ////else
            ////{
            ////    MessageBox.Show("No order item has been selected");
            ////}
            #endregion
        }

        #region Print Voucher
        private void PrintVoucher()
        {
            //PageSettings ps = new PageSettings { Landscape = false, PaperSize = new PaperSize { RawKind = (int)PaperKind.A4 }, Margins = new Margins { Top = 0, Right = 0, Bottom = 0, Left = 0 } };
            string sCompanyName = GetCompanyName();//aded on 14/04/2014 R.Hossain

            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo()));
            rds.Add(new ReportDataSource("DETAILINFO", (DataTable)GetDetailInfo()));
            string sAmtinwds = Amtinwds(Math.Abs(Convert.ToDouble(sTotalAmt))); // added on 28/04/2014 RHossain               
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", "Customer Order Voucher", true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("Amtinwds", sAmtinwds, true));

            string reportName = @"rptCustOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Customer Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();
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


        private decimal GetMetalQtyTolPct()
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

            decimal dResult = 0m;

            string sQry = "SELECT ISNULL(MetalQtyTolPct,0) FROM RETAILPARAMETERS Where DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            dResult = Convert.ToDecimal(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            //}

            return dResult;

        }

        private DataTable GetHeaderInfo()
        {

            sTotalAmt = Convert.ToString(txtTotalAmount.Text);

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

            DataRow dr = dtHeader.NewRow();
            dr["ORDERNO"] = txtOrderNo.Text;
            dr["ORDERDATE"] = dTPickerOrderDate.Value;
            dr["DELIVERYDATE"] = dtPickerDeliveryDate.Value;
            dr["CUSTID"] = txtCustomerAccount.Text;
            dr["CUSTNAME"] = txtCustomerName.Text;
            dr["CUSTADD"] = txtCustomerAddress.Text;
            dr["CUSTPHONE"] = txtPhoneNumber.Text;
            dr["TOTALAMOUNT"] = Convert.ToDecimal(txtTotalAmount.Text);
            dr["DELVERYDATEVISIBLE"] = false;
            dtHeader.Rows.Add(dr);

            return dtHeader;
        }

        private DataTable GetDetailInfo()
        {
            DataTable dtDetails = new DataTable();
            dtDetails.Columns.Add("ITEMID", typeof(string));
            dtDetails.Columns.Add("PCS", typeof(decimal));
            dtDetails.Columns.Add("QTY", typeof(decimal));
            dtDetails.Columns.Add("RATE", typeof(decimal));
            dtDetails.Columns.Add("AMOUNT", typeof(decimal));
            dtDetails.Columns.Add("MAKINGRATE", typeof(decimal));
            dtDetails.Columns.Add("MAKINGAMOUNT", typeof(decimal));
            dtDetails.Columns.Add("TOTALAMOUNT", typeof(decimal));
            dtDetails.Columns.Add("REMARKS", typeof(string));

            foreach (DataRow item in dtOrderDetails.Rows)
            {
                DataRow dr = dtDetails.NewRow();
                dr["ITEMID"] = item["ITEMID"];
                dr["PCS"] = Convert.ToDecimal(item["PCS"]);
                dr["QTY"] = Convert.ToDecimal(item["QUANTITY"]);
                dr["RATE"] = Convert.ToDecimal(item["RATE"]);
                dr["AMOUNT"] = Convert.ToDecimal(item["AMOUNT"]);
                dr["MAKINGRATE"] = Convert.ToDecimal(item["MAKINGRATE"]);
                dr["MAKINGAMOUNT"] = Convert.ToDecimal(item["MAKINGAMOUNT"]);
                dr["TOTALAMOUNT"] = Convert.ToDecimal(item["ROWTOTALAMOUNT"]);
                dr["REMARKS"] = item["RemarksDtl"];
                dtDetails.Rows.Add(dr);
            }

            return dtDetails;
        }
        #endregion

        #region NIM_OrderUpdateWithSketch
        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 24/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :Update Custom order with Sketch image 
        /// </summary>
        private void NIM_OrderUpdateWithSketch()
        {
            for (int sketchLine = 0; sketchLine < dtSketchDetails.Rows.Count; sketchLine++)
            {
                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                string commandText = " UPDATE [CUSTORDER_DETAILS] SET [SKETCH] = @SKETCH" +
                                   " WHERE ORDERNUM = @ORDERNUM AND LINENUM = @LINENUM AND STOREID = @STOREID AND TERMINALID = @TERMINALID";
                SqlCommand command = new SqlCommand(commandText, connection);

                if (string.IsNullOrEmpty(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"])))
                    command.Parameters.Add("@SKETCH", SqlDbType.Image).Value = DBNull.Value;
                else
                {
                    byte[] imageData = NIM_ReadFile(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"]));

                    command.Parameters.Add("@SKETCH", SqlDbType.Image).Value = imageData;// Base64ToImage(hexData); //hexData;

                    #region save into folder
                    string sArchivePath = string.Empty;
                    sArchivePath = GetArchivePathFromImage();
                    string path = sArchivePath + "" + txtOrderNo.Text + "_" + Convert.ToInt16(dtSketchDetails.Rows[sketchLine]["LINENUM"]) + "" + ".jpeg"; //

                    Bitmap b = new Bitmap(Convert.ToString(dtSketchDetails.Rows[sketchLine]["SKETCH"]));
                    b.Save(Convert.ToString(path));

                    #endregion
                }

                command.Parameters.Add("@ORDERNUM", SqlDbType.NVarChar, 20).Value = txtOrderNo.Text;
                command.Parameters.Add("@LINENUM", SqlDbType.NVarChar, 20).Value = Convert.ToInt16(dtSketchDetails.Rows[sketchLine]["LINENUM"]);
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                command.ExecuteNonQuery();
                connection.Close();
            }

        }
        #endregion

        /// <summary>
        /// Created by : Ripan Hossain
        /// Created on : 24/04/2013
        /// Modified by :
        /// Modified on : 
        /// Purpose :Open file in to a filestream and read data in a byte array.
        /// Read Image Bytes into a byte array
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
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

        private string GetArchivePathFromImage()
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

        private void NIM_SaveOrderSampleSketch()
        {
            string sArchivePaths = string.Empty;
            for (int sketchLine = 0; sketchLine < dtSampleSketch.Rows.Count; sketchLine++)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dtSampleSketch.Rows[sketchLine]["SKETCH"])))
                {
                    sArchivePaths = GetArchivePathFromImage();
                    string path = sArchivePaths + "" + txtOrderNo.Text + "_" + Convert.ToInt64(dtSampleSketch.Rows[sketchLine]["LINENUM"]) + "_S" + ".jpeg"; //

                    Bitmap b = new Bitmap(Convert.ToString(dtSampleSketch.Rows[sketchLine]["SKETCH"]));
                    b.Save(Convert.ToString(path));
                }
            }

        }

        private decimal GetFixedMetalRateByOrderLineConfig(string sConfigId) // Fixed Metal Rate New// new dev
        {
            decimal dMetalRate = 0m;
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");

            // commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [INVENTPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE in(1,2,3) AND ACTIVE=1 "); // METALTYPE -- > GOLD and Silver , Silver added on 13/04/2016
            commandText.Append(" AND CONFIGIDSTANDARD='" + sConfigId + "'  AND RATETYPE = 3 AND RETAIL = 1 "); // RATETYPE -- > SALE
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dMetalRate = Convert.ToDecimal(reader.GetValue(0));
                    //sConfigId = Convert.ToString(reader.GetValue(1));
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return dMetalRate;
        }

        private void cmbAdvAgainst_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iTransType = 0;
            cmbVoucher.DataSource = null;

            switch (Convert.ToString(cmbAdvAgainst.SelectedIndex))
            {
                case "1":
                    iTransType = (int)TransactionType.Purchase;
                    break;
                case "2":
                    iTransType = (int)TransactionType.Exchange;
                    break;
                case "3":
                    iTransType = 100;//sales return
                    break;
                default:
                    iTransType = 0;
                    break;
            }

            if (iTransType > 0)
            {
                string sSQl = "select rct.TRANSACTIONTYPE as TRANSACTIONTYPE,rtst.RECEIPTID as RECEIPTID from RETAIL_CUSTOMCALCULATIONS_TABLE rct";
                sSQl = sSQl + " left join RETAILTRANSACTIONSALESTRANS rtst on rct.TRANSACTIONID=rtst.TRANSACTIONID";
                sSQl = sSQl + " and rct.TERMINALID=rtst.TERMINALID and rct.STOREID=rtst.STORE";
                sSQl = sSQl + " and rct.LINENUM=rtst.LINENUM where rtst.CUSTACCOUNT='" + txtCustomerAccount.Text.Trim() + "'";
                sSQl = sSQl + " and isnull(rtst.RECEIPTID,'')!=''";
                sSQl = sSQl + " and CONVERT(VARCHAR(11),TRANSDATE,103)='" + DateTime.Now.Date.ToShortDateString() + "'";
                sSQl = sSQl + " and isnull(rtst.RECEIPTID,'') not in (select VOUCHERAGAINST from CUSTORDER_HEADER where CUSTACCOUNT='" + txtCustomerAccount.Text.Trim() + "')";
                sSQl = sSQl + " and isnull(rtst.RECEIPTID,'') not in (select RECEIPTID from RETAILTRANSACTIONTABLE where CUSTACCOUNT='" + txtCustomerAccount.Text.Trim() + "' AND ADVANCEDONE=1)";
                //select * from RETAILTRANSACTIONTABLE where RECEIPTID='4001R102IR001063' AND ADVANCEDONE=0

                if (iTransType == 100)
                    sSQl = sSQl + " and rtst.RETURNNOSALE=1 ";
                else
                    sSQl = sSQl + " and rct.TRANSACTIONTYPE =" + iTransType + "";

                sSQl = sSQl + " order by rct.TRANSACTIONID desc";


                cmbVoucher.Items.Add(" ");
                cmbVoucher.DataSource = objBlank.NIM_LoadCombo("", "", "", sSQl);
                cmbVoucher.DisplayMember = "RECEIPTID";
                cmbVoucher.ValueMember = "TRANSACTIONTYPE";
            }

        }

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

        private void txtSM_TextChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum StockTransactionId
        {
            StockTransaction = 11
        }
        private void transactionNumber(int transType, string funcProfile, out string mask)
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
        public string GetNextStockTransferTransactionIdForAutoCounterShip()
        {
            try
            {
                StockTransactionId transType = StockTransactionId.StockTransaction;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                transactionNumber((int)transType, funcProfileId, out mask);
                if (string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedValForAutoCounterShip().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private string GetSeedValForAutoCounterShip()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)StockTransactionId.StockTransaction;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(STOCKTRANSFERID,@VAL,LEN(STOCKTRANSFERID)),0) AS INTEGER)) + 1 from SKUTRANSFER_HEADER";

                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                        Val = "1";


                    return Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }
        private string getTransitCounter()
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(COUNTERCODE,'') COUNTERCODE FROM RETAILSTORECOUNTERTABLE WHERE RETAILSTOREID = '" + ApplicationSettings.Database.StoreID + "' and TransitCounter=1");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            string sStoreFormat = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sStoreFormat;

        }
        private string getDeliveryCounter()
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(COUNTERCODE,'') COUNTERCODE FROM RETAILSTORECOUNTERTABLE WHERE RETAILSTOREID = '" + ApplicationSettings.Database.StoreID + "' and DeliveryCounter=1");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            string sStoreFormat = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sStoreFormat;

        }

        private void SaveStckTransfer(string sFromCounter, DataTable dtGropByFromCounter)
        {
            int iStockTransfer_Header = 0;
            int iStockTransfer_Details = 0;
            string TransferId = GetNextStockTransferTransactionIdForAutoCounterShip();
            SqlTransaction transaction = null;
            string sTransitCounter = getTransitCounter();
            string sDeliveryCounter = getDeliveryCounter();

            #region STOCK TRANSFER HEADER
            string commandText = " INSERT INTO [SKUTransfer_Header]([STOCKTRANSFERID],[STOCKTRANSFERDATE]," +
                                 " [STOCKTRANSFERTYPE],[FROMCOUNTER],[TOCOUNTER],[RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID]," +
                                 " [DATAAREAID],[CREATEDON],[TRANSITCOUNTER],[ShippedOrReceived])" +
                                 " VALUES(@STOCKTRANSFERID,@STOCKTRANSFERDATE,@STOCKTRANSFERTYPE,@FROMCOUNTER,@TOCOUNTER," +
                                 " @RETAILSTOREID,@RETAILTERMINALID,@RETAILSTAFFID," +
                                 " @DATAAREAID,@CREATEDON,@TRANSITCOUNTER,@ShippedOrReceived)";

            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@STOCKTRANSFERID", SqlDbType.NVarChar).Value = TransferId.Trim();
                command.Parameters.Add("@STOCKTRANSFERDATE", SqlDbType.DateTime).Value = DateTime.Now.ToShortDateString();
                command.Parameters.Add("@STOCKTRANSFERTYPE", SqlDbType.NVarChar, 10).Value = "Inter";
                command.Parameters.Add("@FROMCOUNTER", SqlDbType.NVarChar, 20).Value = sFromCounter;
                command.Parameters.Add("@TOCOUNTER", SqlDbType.NVarChar, 20).Value = sTransitCounter;
                command.Parameters.Add("@TRANSITCOUNTER", SqlDbType.NVarChar, 20).Value = sDeliveryCounter;
                command.Parameters.Add("@ShippedOrReceived", SqlDbType.Int).Value = 1;//ship
                command.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                if (application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@CREATEDON", SqlDbType.DateTime).Value = DateTime.Today.ToShortDateString();

                command.CommandTimeout = 0;
                iStockTransfer_Header = command.ExecuteNonQuery();

                if (iStockTransfer_Header == 1)
                {
                    if (dtGropByFromCounter != null && dtGropByFromCounter.Rows.Count > 0)
                    {
                        #region ORDER DETAILS
                        //MODIFIED DATE :: 18/03/2013 ; MODIFIED BY : RIPAN HOSSAIN
                        string commandCustOrder_Detail = " INSERT INTO [SKUTRANSFER_DETAILS]([STOCKTRANSFERID],[LINENUMBER],[SKUNUMBER],[QTY]," +
                                                         " [RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID],[DATAAREAID])" +
                                                         " VALUES(@STOCKTRANSFERID  ,@LINENUMBER , @SKUNUMBER," +
                                                         " @QTY,@RETAILSTOREID,@RETAILTERMINALID,@RETAILSTAFFID,@DATAAREAID) ";



                        for (int ItemCount = 0; ItemCount < dtGropByFromCounter.Rows.Count; ItemCount++)
                        {

                            SqlCommand cmdStcokTransfer_Detail = new SqlCommand(commandCustOrder_Detail, connection, transaction);
                            cmdStcokTransfer_Detail.Parameters.Add("@STOCKTRANSFERID", SqlDbType.NVarChar, 20).Value = TransferId.Trim();
                            cmdStcokTransfer_Detail.Parameters.Add("@LINENUMBER", SqlDbType.Int, 10).Value = ItemCount + 1;

                            if (string.IsNullOrEmpty(Convert.ToString(dtGropByFromCounter.Rows[ItemCount]["ITEMID"])))
                                cmdStcokTransfer_Detail.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = DBNull.Value;
                            else
                                cmdStcokTransfer_Detail.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtGropByFromCounter.Rows[ItemCount]["ITEMID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtGropByFromCounter.Rows[ItemCount]["QUANTITY"])))
                                cmdStcokTransfer_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = DBNull.Value;
                            else
                                cmdStcokTransfer_Detail.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtGropByFromCounter.Rows[ItemCount]["QUANTITY"]);

                            cmdStcokTransfer_Detail.Parameters.Add("@RETAILSTOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                            cmdStcokTransfer_Detail.Parameters.Add("@RETAILTERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                            cmdStcokTransfer_Detail.Parameters.Add("@RETAILSTAFFID", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                            cmdStcokTransfer_Detail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            cmdStcokTransfer_Detail.Parameters.Add("@ConfigId", SqlDbType.NVarChar, 10).Value = "";
                            cmdStcokTransfer_Detail.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = DateTime.Now.ToShortDateString();


                            cmdStcokTransfer_Detail.CommandTimeout = 0;
                            iStockTransfer_Details = cmdStcokTransfer_Detail.ExecuteNonQuery();
                            cmdStcokTransfer_Detail.Dispose();

                        }
                        #endregion
                    }
                }

                transaction.Commit();
                command.Dispose();
                transaction.Dispose();
            }

            #endregion

            #region Exception
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
            #endregion
        }

        private void getSkuDetails(string _productSku)
        {
            SqlConnection conn = new SqlConnection();
            DataTable dtSku = new DataTable();

            conn = application.Settings.Database.Connection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = string.Empty;
            commandText = " SELECT top 1 SKUNUMBER,f.NAME,isnull(I.SetOf,0) as PCS,CONVERT(DECIMAL(10,3),QTY) as QUANTITY " +
                           " FROM SKUTable_Posted " +
                           " LEFT JOIN INVENTTABLE I ON SKUTable_Posted.SKUNUMBER = I.ITEMID " +
                           " LEFT OUTER JOIN ECORESPRODUCT E ON i.PRODUCT = E.RECID" +
                           " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
                           " WHERE SKUNUMBER='" + _productSku + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtSku);

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (dtSku != null)
            {
                string s = _productSku;
                DataColumn[] columns = new DataColumn[1];
                columns[0] = dtSku.Columns["SKUNUMBER"];
                dtSku.PrimaryKey = columns;

                skuItem = dtSku;
            }
        }

        #endregion

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBl = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();

            if (IsCustOrderConfirmed(txtOrderNo.Text))
            {
               // objBl.PrintVoucher(txtOrderNo.Text, 2);
                if (IsGMA(txtOrderNo.Text))
                {
                    try
                    {
                        frmOfflineBillPrint objVoucher = new frmOfflineBillPrint(pos, txtOrderNo.Text, 0, 1);
                        objVoucher.ShowDialog();

                        //frmOfflineBillPrint objVoucher1 = new frmOfflineBillPrint(pos, txtOrderNo.Text, 1, 1);
                        //objVoucher1.ShowDialog();

                    }
                    catch { }
                }
                else
                {
                    objBl.PrintVoucher(txtOrderNo.Text, 2);
                }
            }
            else
            {
                MessageBox.Show("Order is not confirmed by cashier");
            }
        }

        #region IsCustOrderConfirmed
        private bool IsCustOrderConfirmed(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT IsConfirmed  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }
        #endregion

        private void dtpReqDeliveryDate_ValueChanged(object sender, EventArgs e)
        {

        }

        private void btnSVSchemeSearch_Click(object sender, EventArgs e)
        {
            string sSQl = "select FESTIVECODE,FESTIVENAME,CONVERT(VARCHAR(11),DELIVERYDATE,106) as DELIVERYDATE," +
                " CONVERT(VARCHAR(11),FROMDATE,106) FROMDATE,CONVERT(VARCHAR(11),TODATE,106) TODATE,isnull(SVTAX,0) SVTAX from CRWSUVARNA_VRUDDHI where ACTIVE=1" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE)  ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "Suvarna Vrudhi");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbSVSchemeCode.Text = string.Empty;
                cmbSVSchemeCode.Text = Convert.ToString(drRes["FESTIVECODE"]);
                dtPickerDeliveryDate.Text = Convert.ToString(drRes["DELIVERYDATE"]);
                dtpReqDeliveryDate.Text = Convert.ToString(drRes["DELIVERYDATE"]);

                dSVTaxPct = Convert.ToDecimal(drRes["SVTAX"]);
            }
        }

        private DataTable NIM_LoadCombo(string _tableName, string _fieldName, string _condition = null, string _sqlStr = null)
        {
            try
            {
                // Open Sql Connection  
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(_sqlStr))
                    SqlComm.CommandText = _sqlStr;
                else
                    SqlComm.CommandText = "select  " + _fieldName + " " +
                                            " FROM " + _tableName + " " +
                                            " " + _condition + " ";

                DataTable dtComboField = new DataTable();
                // DataRow row = dtComboField.NewRow();
                // dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void chkSUVARNAVRUDDHI_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSUVARNAVRUDDHI.Checked == false)
            {
                cmbSVSchemeCode.Text = "";
                btnSVSchemeSearch.Enabled = false;

            }
            else
            {
                btnSVSchemeSearch.Enabled = true;
                chkGMA.Checked = false;
            }
        }

        private void txtOrderNo_TextChanged(object sender, EventArgs e)
        {

        }

        private bool IsBatchItem(string sItemId, SqlTransaction sqlTransaction)
        {

            string commandText = string.Empty;
            commandText = "select ISNULL(TRACKINGDIMENSIONGROUP,'') TRACKINGDIMENSIONGROUP from ECORESTRACKINGDIMENSIONGROUPFLDSETUP " +
                        " where TRACKINGDIMENSIONGROUP = (select TRACKINGDIMENSIONGROUP  from ECORESTRACKINGDIMENSIONGROUPITEM where ITEMID ='" + sItemId + "'" +
                        " and ITEMDATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') and DIMENSIONFIELDID=2 and ISACTIVE=1";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private int getMetalType(string sItemId, SqlTransaction sqlTransaction)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(METALTYPE,0) FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            int iMetalType = Convert.ToInt32(command.ExecuteScalar());
            return iMetalType;
        }

        private string getDefaultBatchId(string sItemId, SqlTransaction sqlTransaction)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(CRWDEFAULTBATCHID,'') FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            string sDefaultBatchId = Convert.ToString(command.ExecuteScalar());
            return sDefaultBatchId;
        }

        private bool IsOGItemType(string item, SqlTransaction sqlTrans) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
        {
            int iOWNDMD = 0;
            int iOWNOG = 0;
            int iOTHERDMD = 0;
            int iOTHEROG = 0;
            try
            {
                //if(conn.State == ConnectionState.Closed)
                //  conn.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";
                //SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
                SqlCommand cmd = new SqlCommand(query.ToString(), sqlTrans.Connection, sqlTrans);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iOWNDMD = Convert.ToInt16(reader.GetValue(0));
                        iOWNOG = Convert.ToInt16(reader.GetValue(1));
                        iOTHERDMD = Convert.ToInt16(reader.GetValue(2));
                        iOTHEROG = Convert.ToInt16(reader.GetValue(3));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }

            if (iOWNOG == 1 || iOTHEROG == 1)
                return true;
            else
                return false;
        }

        private void btnCashierTask_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                objBlank.PostCustOrderSaveCashierTask(pos, this, application);
            }
        }

        private void btnGMASchemeSearch_Click(object sender, EventArgs e)
        {
            string sSQl = "select GMASCHEMECODE,GMASCHEMENAME," +
               " CONVERT(VARCHAR(11), DATEADD(day,GMAFROMDAYS,getdate()),106) as DELIVERYDATE," +
               " CONVERT(VARCHAR(11),FROMDATE,106) FROMDATE,CONVERT(VARCHAR(11),TODATE,106) TODATE," +
               " CAST(ISNULL(DISCPER,0)AS DECIMAL(28,2)) AS DISCPER," +//isnull(PURITY,'') PURITY,
               " isnull(RECEIVEDITEMCODE,'') RECEIVEDITEMCODE,CAST(ISNULL(MINQTY,0)AS DECIMAL(28,3)) AS MINQTY" +
               " from CRWGoldMetalAdvance where ACTIVATE=1" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE)  ";
            DataTable dtResult = NIM_LoadCombo("", "", "", sSQl);
            DataRow drRes = null;

            Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch
                = new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtResult, drRes = null, "GMA Scheme");

            oSearch.ShowDialog();
            drRes = oSearch.SelectedDataRow;
            if (drRes != null)
            {
                cmbGMA.Text = string.Empty;
                cmbGMA.Text = Convert.ToString(drRes["GMASCHEMECODE"]);
                dtPickerDeliveryDate.Text = Convert.ToString(drRes["DELIVERYDATE"]);
                dtpReqDeliveryDate.Text = Convert.ToString(drRes["DELIVERYDATE"]);
                //purity
                dGMADiscPct = Convert.ToDecimal(drRes["DISCPER"]);
                //sGMAPurity = Convert.ToString(drRes["PURITY"]);
                sGMAItem = Convert.ToString(drRes["RECEIVEDITEMCODE"]);
                dGMAMinQty = Convert.ToDecimal(drRes["MINQTY"]);
            }
        }

        private void chkGMA_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGMA.Checked)
            {
                btnGMASchemeSearch.Enabled = true;
                chkSUVARNAVRUDDHI.Checked = false;
            }
            else
            {
                btnGMASchemeSearch.Enabled = false;
                cmbGMA.Text = "";
            }
        }
    }
}
