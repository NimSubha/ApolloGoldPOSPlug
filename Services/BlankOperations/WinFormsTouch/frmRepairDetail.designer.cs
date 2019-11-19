//namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
namespace BlankOperations.WinFormsTouch
{
    partial class frmRepairDetail
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
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblGrQuantity = new System.Windows.Forms.Label();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.lblPCS = new System.Windows.Forms.Label();
            this.lblItemName = new System.Windows.Forms.Label();
            this.lblItemId = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtGrossWt = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.lblArticleDesc = new System.Windows.Forms.Label();
            this.cmbArticle = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbRate = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtTotalAmt = new System.Windows.Forms.TextBox();
            this.txtStnAmt = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtStnWt = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtDiaAmt = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDiaWt = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtNettWt = new System.Windows.Forms.TextBox();
            this.txtTotalAmount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.cmbCode = new System.Windows.Forms.ComboBox();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.lblSizeId = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.lblConfig = new System.Windows.Forms.Label();
            this.cmbConfig = new System.Windows.Forms.ComboBox();
            this.lblJobDetails = new System.Windows.Forms.Label();
            this.txtJobDetails = new System.Windows.Forms.TextBox();
            this.btnAddItem = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPOSItemSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtMakingAmount = new System.Windows.Forms.Label();
            this.lblAmt = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.Label();
            this.txtItemId = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.numPad1 = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colItemId = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPCS = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colGROSSQTY = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colRate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCode = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStyle = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colSize = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colConfig = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNettWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiaWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDiaVal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStnWt = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStnVal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotAprxVal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colJobType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colJobDetails = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colExtendedDetails = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDelete = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnExtendedDetails = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSample = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnAddImage = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 30.25F);
            this.label1.Location = new System.Drawing.Point(201, -8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(428, 55);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ornament Repair Detail";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(473, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 20);
            this.label3.TabIndex = 36;
            // 
            // txtRate
            // 
            this.txtRate.Location = new System.Drawing.Point(359, 220);
            this.txtRate.MaxLength = 9;
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(79, 21);
            this.txtRate.TabIndex = 10;
            this.txtRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtRate_KeyPress);
            this.txtRate.Leave += new System.EventHandler(this.txtRate_Leave);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(-35, 239);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(30, 13);
            this.lblRate.TabIndex = 168;
            this.lblRate.Text = "Rate";
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(138, 149);
            this.txtQuantity.MaxLength = 9;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(79, 21);
            this.txtQuantity.TabIndex = 3;
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtQuantity.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtQuantity_KeyPress);
            this.txtQuantity.Leave += new System.EventHandler(this.txtQuantity_Leave);
            // 
            // lblGrQuantity
            // 
            this.lblGrQuantity.AutoSize = true;
            this.lblGrQuantity.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblGrQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblGrQuantity.Location = new System.Drawing.Point(219, 125);
            this.lblGrQuantity.Name = "lblGrQuantity";
            this.lblGrQuantity.Size = new System.Drawing.Size(82, 20);
            this.lblGrQuantity.TabIndex = 15;
            this.lblGrQuantity.Text = "Gross Qty.";
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(137, 125);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(80, 21);
            this.txtPCS.TabIndex = 14;
            this.txtPCS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtPCS.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPCS_KeyPress);
            // 
            // lblPCS
            // 
            this.lblPCS.AutoSize = true;
            this.lblPCS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblPCS.Location = new System.Drawing.Point(-35, 214);
            this.lblPCS.Name = "lblPCS";
            this.lblPCS.Size = new System.Drawing.Size(26, 13);
            this.lblPCS.TabIndex = 163;
            this.lblPCS.Text = "PCS";
            // 
            // lblItemName
            // 
            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblItemName.ForeColor = System.Drawing.Color.Black;
            this.lblItemName.Location = new System.Drawing.Point(5, 26);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(87, 20);
            this.lblItemName.TabIndex = 2;
            this.lblItemName.Text = "Item Name";
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblItemId.ForeColor = System.Drawing.Color.Black;
            this.lblItemId.Location = new System.Drawing.Point(5, 0);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(60, 20);
            this.lblItemId.TabIndex = 0;
            this.lblItemId.Text = "Item ID";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtGrossWt);
            this.panel1.Controls.Add(this.lblQuantity);
            this.panel1.Controls.Add(this.lblArticleDesc);
            this.panel1.Controls.Add(this.cmbArticle);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.cmbRate);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.txtTotalAmt);
            this.panel1.Controls.Add(this.txtStnAmt);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.txtStnWt);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.txtDiaAmt);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.txtDiaWt);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.txtNettWt);
            this.panel1.Controls.Add(this.txtTotalAmount);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.lblCode);
            this.panel1.Controls.Add(this.cmbCode);
            this.panel1.Controls.Add(this.cmbStyle);
            this.panel1.Controls.Add(this.lblSizeId);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbSize);
            this.panel1.Controls.Add(this.lblConfig);
            this.panel1.Controls.Add(this.cmbConfig);
            this.panel1.Controls.Add(this.lblJobDetails);
            this.panel1.Controls.Add(this.txtJobDetails);
            this.panel1.Controls.Add(this.btnAddItem);
            this.panel1.Controls.Add(this.btnPOSItemSearch);
            this.panel1.Controls.Add(this.txtMakingAmount);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lblAmt);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtItemName);
            this.panel1.Controls.Add(this.txtItemId);
            this.panel1.Controls.Add(this.lblItemId);
            this.panel1.Controls.Add(this.lblItemName);
            this.panel1.Controls.Add(this.txtRate);
            this.panel1.Controls.Add(this.txtPCS);
            this.panel1.Controls.Add(this.txtQuantity);
            this.panel1.Controls.Add(this.lblGrQuantity);
            this.panel1.Location = new System.Drawing.Point(5, 50);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(649, 328);
            this.panel1.TabIndex = 1;
            // 
            // txtGrossWt
            // 
            this.txtGrossWt.Location = new System.Drawing.Point(359, 124);
            this.txtGrossWt.MaxLength = 9;
            this.txtGrossWt.Name = "txtGrossWt";
            this.txtGrossWt.Size = new System.Drawing.Size(79, 21);
            this.txtGrossWt.TabIndex = 2;
            this.txtGrossWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtGrossWt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtGrossWt_KeyPress);
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblQuantity.Location = new System.Drawing.Point(5, 150);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(69, 20);
            this.lblQuantity.TabIndex = 41;
            this.lblQuantity.Text = "Quantity";
            // 
            // lblArticleDesc
            // 
            this.lblArticleDesc.AutoSize = true;
            this.lblArticleDesc.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblArticleDesc.ForeColor = System.Drawing.Color.Black;
            this.lblArticleDesc.Location = new System.Drawing.Point(262, 52);
            this.lblArticleDesc.Name = "lblArticleDesc";
            this.lblArticleDesc.Size = new System.Drawing.Size(0, 20);
            this.lblArticleDesc.TabIndex = 40;
            // 
            // cmbArticle
            // 
            this.cmbArticle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbArticle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbArticle.FormattingEnabled = true;
            this.cmbArticle.Location = new System.Drawing.Point(137, 51);
            this.cmbArticle.Name = "cmbArticle";
            this.cmbArticle.Size = new System.Drawing.Size(110, 21);
            this.cmbArticle.TabIndex = 1;
            this.cmbArticle.SelectedIndexChanged += new System.EventHandler(this.cmbArticle_SelectedIndexChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(5, 51);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 20);
            this.label6.TabIndex = 38;
            this.label6.Text = "Article";
            // 
            // cmbRate
            // 
            this.cmbRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmbRate.FormattingEnabled = true;
            this.cmbRate.Location = new System.Drawing.Point(138, 219);
            this.cmbRate.Name = "cmbRate";
            this.cmbRate.Size = new System.Drawing.Size(79, 21);
            this.cmbRate.TabIndex = 9;
            this.cmbRate.SelectedIndexChanged += new System.EventHandler(this.cmbRate_SelectedIndexChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(5, 220);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 20);
            this.label12.TabIndex = 29;
            this.label12.Text = "Job Type";
            // 
            // txtTotalAmt
            // 
            this.txtTotalAmt.AcceptsReturn = true;
            this.txtTotalAmt.Location = new System.Drawing.Point(359, 244);
            this.txtTotalAmt.MaxLength = 9;
            this.txtTotalAmt.Name = "txtTotalAmt";
            this.txtTotalAmt.Size = new System.Drawing.Size(79, 21);
            this.txtTotalAmt.TabIndex = 11;
            this.txtTotalAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTotalAmt.Leave += new System.EventHandler(this.txtTotalAmt_Leave);
            // 
            // txtStnAmt
            // 
            this.txtStnAmt.Location = new System.Drawing.Point(359, 195);
            this.txtStnAmt.MaxLength = 9;
            this.txtStnAmt.Name = "txtStnAmt";
            this.txtStnAmt.Size = new System.Drawing.Size(79, 21);
            this.txtStnAmt.TabIndex = 8;
            this.txtStnAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStnAmt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStnAmt_KeyPress);
            this.txtStnAmt.Leave += new System.EventHandler(this.txtStnAmt_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(219, 244);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(140, 20);
            this.label9.TabIndex = 27;
            this.label9.Text = "Total Approx Value";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(219, 196);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 20);
            this.label10.TabIndex = 25;
            this.label10.Text = "Stone Value";
            // 
            // txtStnWt
            // 
            this.txtStnWt.Location = new System.Drawing.Point(137, 195);
            this.txtStnWt.MaxLength = 9;
            this.txtStnWt.Name = "txtStnWt";
            this.txtStnWt.Size = new System.Drawing.Size(80, 21);
            this.txtStnWt.TabIndex = 7;
            this.txtStnWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStnWt.Leave += new System.EventHandler(this.txtStnWt_Leave);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(5, 196);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(73, 20);
            this.label11.TabIndex = 23;
            this.label11.Text = "Stone Wt";
            // 
            // txtDiaAmt
            // 
            this.txtDiaAmt.Location = new System.Drawing.Point(359, 171);
            this.txtDiaAmt.MaxLength = 9;
            this.txtDiaAmt.Name = "txtDiaAmt";
            this.txtDiaAmt.Size = new System.Drawing.Size(79, 21);
            this.txtDiaAmt.TabIndex = 6;
            this.txtDiaAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiaAmt.Leave += new System.EventHandler(this.txtDiaAmt_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(219, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(115, 20);
            this.label7.TabIndex = 21;
            this.label7.Text = "Diamond Value";
            // 
            // txtDiaWt
            // 
            this.txtDiaWt.Location = new System.Drawing.Point(137, 172);
            this.txtDiaWt.MaxLength = 9;
            this.txtDiaWt.Name = "txtDiaWt";
            this.txtDiaWt.Size = new System.Drawing.Size(80, 21);
            this.txtDiaWt.TabIndex = 5;
            this.txtDiaWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiaWt.Leave += new System.EventHandler(this.txtDiaWt_Leave);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(5, 173);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(96, 20);
            this.label8.TabIndex = 19;
            this.label8.Text = "Diamond Wt";
            // 
            // txtNettWt
            // 
            this.txtNettWt.Enabled = false;
            this.txtNettWt.Location = new System.Drawing.Point(359, 148);
            this.txtNettWt.MaxLength = 9;
            this.txtNettWt.Name = "txtNettWt";
            this.txtNettWt.Size = new System.Drawing.Size(79, 21);
            this.txtNettWt.TabIndex = 4;
            this.txtNettWt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtNettWt.Leave += new System.EventHandler(this.txtNettWt_Leave);
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.AutoSize = true;
            this.txtTotalAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTotalAmount.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtTotalAmount.ForeColor = System.Drawing.Color.Black;
            this.txtTotalAmount.Location = new System.Drawing.Point(495, 200);
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.Size = new System.Drawing.Size(2, 22);
            this.txtTotalAmount.TabIndex = 11;
            this.txtTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtTotalAmount.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(219, 149);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 20);
            this.label4.TabIndex = 17;
            this.label4.Text = "Nett Wt";
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCode.ForeColor = System.Drawing.Color.Black;
            this.lblCode.Location = new System.Drawing.Point(5, 77);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(46, 20);
            this.lblCode.TabIndex = 5;
            this.lblCode.Text = "Code";
            // 
            // cmbCode
            // 
            this.cmbCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbCode.Enabled = false;
            this.cmbCode.FormattingEnabled = true;
            this.cmbCode.Location = new System.Drawing.Point(137, 76);
            this.cmbCode.Name = "cmbCode";
            this.cmbCode.Size = new System.Drawing.Size(80, 21);
            this.cmbCode.TabIndex = 6;
            // 
            // cmbStyle
            // 
            this.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbStyle.Enabled = false;
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Location = new System.Drawing.Point(359, 78);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(79, 21);
            this.cmbStyle.TabIndex = 8;
            // 
            // lblSizeId
            // 
            this.lblSizeId.AutoSize = true;
            this.lblSizeId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblSizeId.ForeColor = System.Drawing.Color.Black;
            this.lblSizeId.Location = new System.Drawing.Point(5, 99);
            this.lblSizeId.Name = "lblSizeId";
            this.lblSizeId.Size = new System.Drawing.Size(38, 20);
            this.lblSizeId.TabIndex = 9;
            this.lblSizeId.Text = "Size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(219, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Style";
            // 
            // cmbSize
            // 
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbSize.Enabled = false;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Location = new System.Drawing.Point(137, 100);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(80, 21);
            this.cmbSize.TabIndex = 10;
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblConfig.ForeColor = System.Drawing.Color.Black;
            this.lblConfig.Location = new System.Drawing.Point(219, 101);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(105, 20);
            this.lblConfig.TabIndex = 11;
            this.lblConfig.Text = "Configuration";
            // 
            // cmbConfig
            // 
            this.cmbConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbConfig.Enabled = false;
            this.cmbConfig.FormattingEnabled = true;
            this.cmbConfig.Location = new System.Drawing.Point(359, 101);
            this.cmbConfig.Name = "cmbConfig";
            this.cmbConfig.Size = new System.Drawing.Size(79, 21);
            this.cmbConfig.TabIndex = 12;
            // 
            // lblJobDetails
            // 
            this.lblJobDetails.AutoSize = true;
            this.lblJobDetails.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblJobDetails.ForeColor = System.Drawing.Color.Black;
            this.lblJobDetails.Location = new System.Drawing.Point(5, 272);
            this.lblJobDetails.Name = "lblJobDetails";
            this.lblJobDetails.Size = new System.Drawing.Size(86, 20);
            this.lblJobDetails.TabIndex = 33;
            this.lblJobDetails.Text = "Job Details";
            // 
            // txtJobDetails
            // 
            this.txtJobDetails.Location = new System.Drawing.Point(137, 272);
            this.txtJobDetails.MaxLength = 250;
            this.txtJobDetails.Multiline = true;
            this.txtJobDetails.Name = "txtJobDetails";
            this.txtJobDetails.Size = new System.Drawing.Size(301, 47);
            this.txtJobDetails.TabIndex = 12;
            // 
            // btnAddItem
            // 
            this.btnAddItem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddItem.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddItem.Appearance.Options.UseFont = true;
            this.btnAddItem.Location = new System.Drawing.Point(469, 267);
            this.btnAddItem.Name = "btnAddItem";
            this.btnAddItem.Size = new System.Drawing.Size(173, 55);
            this.btnAddItem.TabIndex = 13;
            this.btnAddItem.Text = "Add to Grid";
            this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPOSItemSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPOSItemSearch.Appearance.Options.UseFont = true;
            this.btnPOSItemSearch.Location = new System.Drawing.Point(469, 21);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(173, 55);
            this.btnPOSItemSearch.TabIndex = 0;
            this.btnPOSItemSearch.Text = "Product Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // txtMakingAmount
            // 
            this.txtMakingAmount.AutoSize = true;
            this.txtMakingAmount.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtMakingAmount.ForeColor = System.Drawing.Color.Green;
            this.txtMakingAmount.Location = new System.Drawing.Point(630, 107);
            this.txtMakingAmount.Name = "txtMakingAmount";
            this.txtMakingAmount.Size = new System.Drawing.Size(0, 20);
            this.txtMakingAmount.TabIndex = 37;
            this.txtMakingAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblAmt
            // 
            this.lblAmt.AutoSize = true;
            this.lblAmt.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblAmt.ForeColor = System.Drawing.Color.Black;
            this.lblAmt.Location = new System.Drawing.Point(219, 219);
            this.lblAmt.Name = "lblAmt";
            this.lblAmt.Size = new System.Drawing.Size(134, 20);
            this.lblAmt.TabIndex = 31;
            this.lblAmt.Text = "Estimated Charge";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(5, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 20);
            this.label5.TabIndex = 13;
            this.label5.Text = "PCS";
            // 
            // txtItemName
            // 
            this.txtItemName.AutoSize = true;
            this.txtItemName.Font = new System.Drawing.Font("Segoe UI Light", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtItemName.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtItemName.Location = new System.Drawing.Point(134, 28);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(79, 17);
            this.txtItemName.TabIndex = 3;
            this.txtItemName.Text = "Item Name";
            // 
            // txtItemId
            // 
            this.txtItemId.AutoSize = true;
            this.txtItemId.Font = new System.Drawing.Font("Segoe UI Light", 9.75F, System.Drawing.FontStyle.Bold);
            this.txtItemId.ForeColor = System.Drawing.SystemColors.MenuHighlight;
            this.txtItemId.Location = new System.Drawing.Point(134, 2);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Size = new System.Drawing.Size(54, 17);
            this.txtItemId.TabIndex = 1;
            this.txtItemId.Text = "Item Id";
            // 
            // txtAmount
            // 
            this.txtAmount.AutoSize = true;
            this.txtAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtAmount.ForeColor = System.Drawing.Color.Green;
            this.txtAmount.Location = new System.Drawing.Point(292, 624);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(2, 22);
            this.txtAmount.TabIndex = 10;
            this.txtAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtAmount.Visible = false;
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.AutoSize = true;
            this.lblTotalAmount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTotalAmount.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalAmount.ForeColor = System.Drawing.Color.Black;
            this.lblTotalAmount.Location = new System.Drawing.Point(372, 624);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(2, 22);
            this.lblTotalAmount.TabIndex = 12;
            this.lblTotalAmount.Visible = false;
            // 
            // numPad1
            // 
            this.numPad1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.numPad1.Appearance.BackColor = System.Drawing.Color.White;
            this.numPad1.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numPad1.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.numPad1.Appearance.Options.UseBackColor = true;
            this.numPad1.Appearance.Options.UseFont = true;
            this.numPad1.Appearance.Options.UseForeColor = true;
            this.numPad1.AutoSize = true;
            this.numPad1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.numPad1.CurrencyCode = null;
            this.numPad1.EnteredQuantity = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numPad1.EnteredValue = "";
            this.numPad1.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Quantity;
            this.numPad1.Location = new System.Drawing.Point(675, 78);
            this.numPad1.LookAndFeel.SkinName = "The Asphalt World";
            this.numPad1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.numPad1.MaskChar = "";
            this.numPad1.MaskInterval = 0;
            this.numPad1.MaxNumberOfDigits = 6;
            this.numPad1.MinimumSize = new System.Drawing.Size(170, 242);
            this.numPad1.Name = "numPad1";
            this.numPad1.NegativeMode = false;
            this.numPad1.NoOfTries = 0;
            this.numPad1.NumberOfDecimals = 2;
            this.numPad1.PromptText = "Enter Details ";
            this.numPad1.ShortcutKeysActive = false;
            this.numPad1.Size = new System.Drawing.Size(170, 242);
            this.numPad1.TabIndex = 6;
            this.numPad1.TimerEnabled = false;
            this.numPad1.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numPad1_EnterButtonPressed);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 99.0099F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 0.990099F));
            this.tableLayoutPanel1.Controls.Add(this.grItems, 0, 0);
            this.tableLayoutPanel1.ForeColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 390);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(878, 180);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(3, 3);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(872, 174);
            this.grItems.TabIndex = 0;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.Black;
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colItemId,
            this.colPCS,
            this.colGROSSQTY,
            this.colQty,
            this.colRate,
            this.colCode,
            this.colStyle,
            this.colSize,
            this.colConfig,
            this.colNettWt,
            this.colDiaWt,
            this.colDiaVal,
            this.colStnWt,
            this.colStnVal,
            this.colTotAprxVal,
            this.colJobType,
            this.colJobDetails});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.AutoCalcPreviewLineCount = true;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.OptionsView.ShowPreview = true;
            this.grdView.PreviewFieldName = "DIMENSIONS";
            this.grdView.PreviewIndent = 2;
            this.grdView.PreviewLineCount = 1;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // colItemId
            // 
            this.colItemId.Caption = "Item Id";
            this.colItemId.FieldName = "ITEMID";
            this.colItemId.Name = "colItemId";
            this.colItemId.OptionsColumn.AllowEdit = false;
            this.colItemId.OptionsColumn.AllowIncrementalSearch = false;
            this.colItemId.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colItemId.Visible = true;
            this.colItemId.VisibleIndex = 0;
            this.colItemId.Width = 100;
            // 
            // colPCS
            // 
            this.colPCS.Caption = "PCS";
            this.colPCS.FieldName = "PCS";
            this.colPCS.Name = "colPCS";
            this.colPCS.OptionsColumn.AllowEdit = false;
            this.colPCS.OptionsColumn.AllowIncrementalSearch = false;
            this.colPCS.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colPCS.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colPCS.ShowUnboundExpressionMenu = true;
            this.colPCS.Visible = true;
            this.colPCS.VisibleIndex = 1;
            this.colPCS.Width = 80;
            // 
            // colGROSSQTY
            // 
            this.colGROSSQTY.Caption = "Gross Qty.";
            this.colGROSSQTY.FieldName = "GROSSQTY";
            this.colGROSSQTY.Name = "colGROSSQTY";
            this.colGROSSQTY.Visible = true;
            this.colGROSSQTY.VisibleIndex = 2;
            // 
            // colQty
            // 
            this.colQty.Caption = "Qty";
            this.colQty.FieldName = "QUANTITY";
            this.colQty.Name = "colQty";
            this.colQty.OptionsColumn.AllowEdit = false;
            this.colQty.OptionsColumn.AllowIncrementalSearch = false;
            this.colQty.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colQty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colQty.Visible = true;
            this.colQty.VisibleIndex = 3;
            this.colQty.Width = 80;
            // 
            // colRate
            // 
            this.colRate.Caption = "Amount";
            this.colRate.FieldName = "RATE";
            this.colRate.Name = "colRate";
            this.colRate.OptionsColumn.AllowEdit = false;
            this.colRate.OptionsColumn.AllowIncrementalSearch = false;
            this.colRate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colRate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colRate.Visible = true;
            this.colRate.VisibleIndex = 4;
            this.colRate.Width = 80;
            // 
            // colCode
            // 
            this.colCode.Caption = "Code";
            this.colCode.Name = "colCode";
            this.colCode.OptionsColumn.AllowEdit = false;
            this.colCode.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colStyle
            // 
            this.colStyle.Caption = "Style";
            this.colStyle.Name = "colStyle";
            this.colStyle.OptionsColumn.AllowEdit = false;
            this.colStyle.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colStyle.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colSize
            // 
            this.colSize.Caption = "Size";
            this.colSize.Name = "colSize";
            this.colSize.OptionsColumn.AllowEdit = false;
            this.colSize.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colSize.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colConfig
            // 
            this.colConfig.Caption = "Config";
            this.colConfig.Name = "colConfig";
            this.colConfig.OptionsColumn.AllowEdit = false;
            this.colConfig.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colConfig.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            // 
            // colNettWt
            // 
            this.colNettWt.Caption = "Nett Wt";
            this.colNettWt.FieldName = "NETTWT";
            this.colNettWt.Name = "colNettWt";
            this.colNettWt.Visible = true;
            this.colNettWt.VisibleIndex = 5;
            // 
            // colDiaWt
            // 
            this.colDiaWt.Caption = "Diamond Wt";
            this.colDiaWt.FieldName = "DIAWT";
            this.colDiaWt.Name = "colDiaWt";
            this.colDiaWt.Visible = true;
            this.colDiaWt.VisibleIndex = 6;
            this.colDiaWt.Width = 88;
            // 
            // colDiaVal
            // 
            this.colDiaVal.Caption = "Diamond Value";
            this.colDiaVal.FieldName = "DIAAMT";
            this.colDiaVal.Name = "colDiaVal";
            this.colDiaVal.Visible = true;
            this.colDiaVal.VisibleIndex = 7;
            this.colDiaVal.Width = 105;
            // 
            // colStnWt
            // 
            this.colStnWt.Caption = "Stone Wt";
            this.colStnWt.FieldName = "STNWT";
            this.colStnWt.Name = "colStnWt";
            this.colStnWt.Visible = true;
            this.colStnWt.VisibleIndex = 8;
            // 
            // colStnVal
            // 
            this.colStnVal.Caption = "Stone Value";
            this.colStnVal.FieldName = "STNAMT";
            this.colStnVal.Name = "colStnVal";
            this.colStnVal.Visible = true;
            this.colStnVal.VisibleIndex = 9;
            this.colStnVal.Width = 83;
            // 
            // colTotAprxVal
            // 
            this.colTotAprxVal.Caption = "Total Approx Value";
            this.colTotAprxVal.FieldName = "TOTALAMT";
            this.colTotAprxVal.Name = "colTotAprxVal";
            this.colTotAprxVal.Visible = true;
            this.colTotAprxVal.VisibleIndex = 10;
            this.colTotAprxVal.Width = 129;
            // 
            // colJobType
            // 
            this.colJobType.Caption = "Job Type";
            this.colJobType.FieldName = "JOBTYPE";
            this.colJobType.Name = "colJobType";
            this.colJobType.Visible = true;
            this.colJobType.VisibleIndex = 11;
            // 
            // colJobDetails
            // 
            this.colJobDetails.Caption = "Job Details";
            this.colJobDetails.FieldName = "REPAIRJOBDETAILS";
            this.colJobDetails.Name = "colJobDetails";
            this.colJobDetails.OptionsColumn.AllowEdit = false;
            this.colJobDetails.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colJobDetails.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colJobDetails.Visible = true;
            this.colJobDetails.VisibleIndex = 12;
            this.colJobDetails.Width = 78;
            // 
            // colExtendedDetails
            // 
            this.colExtendedDetails.Caption = "Extended Details";
            this.colExtendedDetails.FieldName = "EXTENDEDDETAILS";
            this.colExtendedDetails.Name = "colExtendedDetails";
            this.colExtendedDetails.OptionsColumn.AllowEdit = false;
            this.colExtendedDetails.OptionsColumn.AllowIncrementalSearch = false;
            this.colExtendedDetails.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colExtendedDetails.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colExtendedDetails.Visible = true;
            this.colExtendedDetails.VisibleIndex = 13;
            this.colExtendedDetails.Width = 250;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(368, 576);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(100, 42);
            this.btnClear.TabIndex = 19;
            this.btnClear.Text = "Void";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(495, 576);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 42);
            this.btnCancel.TabIndex = 16;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.Location = new System.Drawing.Point(240, 576);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 42);
            this.btnSubmit.TabIndex = 15;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDelete.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Appearance.Options.UseFont = true;
            this.btnDelete.Location = new System.Drawing.Point(623, 576);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 42);
            this.btnDelete.TabIndex = 17;
            this.btnDelete.Text = "Delete";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(748, 576);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(100, 42);
            this.btnEdit.TabIndex = 18;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnExtendedDetails
            // 
            this.btnExtendedDetails.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnExtendedDetails.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExtendedDetails.Appearance.Options.UseFont = true;
            this.btnExtendedDetails.Location = new System.Drawing.Point(75, 576);
            this.btnExtendedDetails.Name = "btnExtendedDetails";
            this.btnExtendedDetails.Size = new System.Drawing.Size(141, 42);
            this.btnExtendedDetails.TabIndex = 14;
            this.btnExtendedDetails.Text = "Add Ingredients";
            this.btnExtendedDetails.Click += new System.EventHandler(this.btnExtendedDetails_Click);
            // 
            // btnSample
            // 
            this.btnSample.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSample.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSample.Appearance.Options.UseFont = true;
            this.btnSample.Location = new System.Drawing.Point(870, 688);
            this.btnSample.Name = "btnSample";
            this.btnSample.Size = new System.Drawing.Size(100, 42);
            this.btnSample.TabIndex = 13;
            this.btnSample.Text = "Sample";
            // 
            // btnAddImage
            // 
            this.btnAddImage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnAddImage.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddImage.Appearance.Options.UseFont = true;
            this.btnAddImage.Location = new System.Drawing.Point(675, 326);
            this.btnAddImage.Name = "btnAddImage";
            this.btnAddImage.Size = new System.Drawing.Size(170, 53);
            this.btnAddImage.TabIndex = 194;
            this.btnAddImage.Text = "Add Image";
            this.btnAddImage.Click += new System.EventHandler(this.btnAddImage_Click);
            // 
            // frmRepairDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 613);
            this.ControlBox = false;
            this.Controls.Add(this.btnAddImage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnExtendedDetails);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnSample);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.numPad1);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.lblPCS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblTotalAmount);
            this.Controls.Add(this.txtAmount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = false;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmRepairDetail";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Ornament Repair Detail";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        //private DevExpress.XtraEditors.StyleController styleController;
        private System.Windows.Forms.Label label1;
        // private System.Windows.Forms.TextBox txtMakingRate;
        private System.Windows.Forms.Label label3;
        //  private System.Windows.Forms.ComboBox cmbMakingType;
        //  private System.Windows.Forms.Label lblMakingType;
        //private System.Windows.Forms.Label lblMakingRate;
        // private System.Windows.Forms.ComboBox cmbStyle;
        // private System.Windows.Forms.Label label2;
        // private System.Windows.Forms.ComboBox cmbRateType;
        // private System.Windows.Forms.Label lblRateType;
        private System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.Label lblRate;
        //  private System.Windows.Forms.ComboBox cmbSize;
        //private System.Windows.Forms.Label lblSizeId;
        //  private System.Windows.Forms.ComboBox cmbCode;
        // private System.Windows.Forms.Label lblCode;
        //  private System.Windows.Forms.Label lblConfig;
        //   private System.Windows.Forms.ComboBox cmbConfig;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblGrQuantity;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.Label lblPCS;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.Label lblItemId;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label txtItemName;
        private System.Windows.Forms.Label txtItemId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblAmt;
        private System.Windows.Forms.Label txtTotalAmount;
        private System.Windows.Forms.Label txtMakingAmount;
        private System.Windows.Forms.Label txtAmount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPOSItemSearch;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numPad1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddItem;
        private DevExpress.XtraGrid.Columns.GridColumn colItemId;
        //   private DevExpress.XtraGrid.Columns.GridColumn colItemName;
        //private DevExpress.XtraGrid.Columns.GridColumn colSize;
        //private DevExpress.XtraGrid.Columns.GridColumn colColor;
        //private DevExpress.XtraGrid.Columns.GridColumn colConfiguration;
        //private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colPCS;

        private DevExpress.XtraGrid.Columns.GridColumn colQty;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;

        private DevExpress.XtraGrid.Columns.GridColumn colRate;
        // private DevExpress.XtraGrid.Columns.GridColumn colRateType;

        private DevExpress.XtraGrid.Columns.GridColumn colAmount;
        //private DevExpress.XtraGrid.Columns.GridColumn colMakingRate;
        //private DevExpress.XtraGrid.Columns.GridColumn colMakingType;
        //private DevExpress.XtraGrid.Columns.GridColumn colMakingAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colExtendedDetails;
        //private DevExpress.XtraGrid.Columns.GridColumn colRowTotalAmount;
        //  private DevExpress.XtraGrid.Columns.GridColumn colTotalAmount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDelete;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSample;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnExtendedDetails;
        private System.Windows.Forms.Label lblJobDetails;
        private System.Windows.Forms.TextBox txtJobDetails;
        private DevExpress.XtraGrid.Columns.GridColumn colJobDetails;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.ComboBox cmbCode;
        private System.Windows.Forms.ComboBox cmbStyle;
        private System.Windows.Forms.Label lblSizeId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSize;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.ComboBox cmbConfig;
        private System.Windows.Forms.TextBox txtNettWt;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDiaAmt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDiaWt;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTotalAmt;
        private System.Windows.Forms.TextBox txtStnAmt;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtStnWt;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cmbRate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblTotalAmount;
        private DevExpress.XtraGrid.Columns.GridColumn colCode;
        private DevExpress.XtraGrid.Columns.GridColumn colStyle;
        private DevExpress.XtraGrid.Columns.GridColumn colSize;
        private DevExpress.XtraGrid.Columns.GridColumn colConfig;
        private DevExpress.XtraGrid.Columns.GridColumn colNettWt;
        private DevExpress.XtraGrid.Columns.GridColumn colDiaWt;
        private DevExpress.XtraGrid.Columns.GridColumn colDiaVal;
        private DevExpress.XtraGrid.Columns.GridColumn colStnWt;
        private DevExpress.XtraGrid.Columns.GridColumn colStnVal;
        private DevExpress.XtraGrid.Columns.GridColumn colTotAprxVal;
        private DevExpress.XtraGrid.Columns.GridColumn colJobType;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAddImage;
        private System.Windows.Forms.ComboBox cmbArticle;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblArticleDesc;
        private System.Windows.Forms.TextBox txtGrossWt;
        private System.Windows.Forms.Label lblQuantity;
        private DevExpress.XtraGrid.Columns.GridColumn colGROSSQTY;
    }
}