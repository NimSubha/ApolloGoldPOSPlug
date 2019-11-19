namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmGetEstimateion
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
            this.txtSKU = new System.Windows.Forms.TextBox();
            this.lblRefInv = new System.Windows.Forms.Label();
            this.btnAdd = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblRate = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCode = new System.Windows.Forms.Label();
            this.cmbCode = new System.Windows.Forms.ComboBox();
            this.cmbStyle = new System.Windows.Forms.ComboBox();
            this.lblSizeId = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSize = new System.Windows.Forms.ComboBox();
            this.lblConfig = new System.Windows.Forms.Label();
            this.cmbConfig = new System.Windows.Forms.ComboBox();
            this.txtPCS = new System.Windows.Forms.TextBox();
            this.txtQuantity = new System.Windows.Forms.TextBox();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.cmbMakingType = new System.Windows.Forms.ComboBox();
            this.lblMakingType = new System.Windows.Forms.Label();
            this.cmbRateType = new System.Windows.Forms.ComboBox();
            this.lblRateType = new System.Windows.Forms.Label();
            this.txtgval = new System.Windows.Forms.TextBox();
            this.txtTotalAmount = new System.Windows.Forms.TextBox();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.txtMakingAmount = new System.Windows.Forms.TextBox();
            this.cmbWastage = new System.Windows.Forms.ComboBox();
            this.cmbMakingDiscType = new System.Windows.Forms.ComboBox();
            this.txtWastageAmount = new System.Windows.Forms.TextBox();
            this.txtMakingDiscTotAmt = new System.Windows.Forms.TextBox();
            this.txtMakingDisc = new System.Windows.Forms.TextBox();
            this.txtWastagePercentage = new System.Windows.Forms.TextBox();
            this.txtWastageQty = new System.Windows.Forms.TextBox();
            this.txtMakingRate = new System.Windows.Forms.TextBox();
            this.btnPOSItemSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtRate = new System.Windows.Forms.TextBox();
            this.txtActMakingRate = new System.Windows.Forms.TextBox();
            this.txtChangedMakingRate = new System.Windows.Forms.TextBox();
            this.txtActToAmount = new System.Windows.Forms.TextBox();
            this.txtChangedTotAmount = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // txtSKU
            // 
            this.txtSKU.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSKU.Location = new System.Drawing.Point(129, 12);
            this.txtSKU.MaxLength = 20;
            this.txtSKU.Name = "txtSKU";
            this.txtSKU.Size = new System.Drawing.Size(144, 33);
            this.txtSKU.TabIndex = 221;
            this.txtSKU.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtSKU_KeyPress);
            // 
            // lblRefInv
            // 
            this.lblRefInv.AutoSize = true;
            this.lblRefInv.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRefInv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRefInv.Location = new System.Drawing.Point(17, 13);
            this.lblRefInv.Name = "lblRefInv";
            this.lblRefInv.Size = new System.Drawing.Size(49, 25);
            this.lblRefInv.TabIndex = 222;
            this.lblRefInv.Text = "SKU";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(279, 59);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(100, 39);
            this.btnAdd.TabIndex = 220;
            this.btnAdd.Text = "Get Estimation";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(124, 48);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(50, 25);
            this.lblRate.TabIndex = 223;
            this.lblRate.Text = "0.00";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(17, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 25);
            this.label2.TabIndex = 224;
            this.label2.Text = "Rate";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(-4, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 20);
            this.label5.TabIndex = 237;
            this.label5.Text = "PCS";
            // 
            // lblCode
            // 
            this.lblCode.AutoSize = true;
            this.lblCode.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblCode.ForeColor = System.Drawing.Color.Black;
            this.lblCode.Location = new System.Drawing.Point(-4, 182);
            this.lblCode.Name = "lblCode";
            this.lblCode.Size = new System.Drawing.Size(46, 20);
            this.lblCode.TabIndex = 233;
            this.lblCode.Text = "Code";
            // 
            // cmbCode
            // 
            this.cmbCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbCode.Enabled = false;
            this.cmbCode.FormattingEnabled = true;
            this.cmbCode.Location = new System.Drawing.Point(105, 181);
            this.cmbCode.Name = "cmbCode";
            this.cmbCode.Size = new System.Drawing.Size(51, 21);
            this.cmbCode.TabIndex = 228;
            // 
            // cmbStyle
            // 
            this.cmbStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbStyle.Enabled = false;
            this.cmbStyle.FormattingEnabled = true;
            this.cmbStyle.Location = new System.Drawing.Point(345, 181);
            this.cmbStyle.Name = "cmbStyle";
            this.cmbStyle.Size = new System.Drawing.Size(51, 21);
            this.cmbStyle.TabIndex = 230;
            // 
            // lblSizeId
            // 
            this.lblSizeId.AutoSize = true;
            this.lblSizeId.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblSizeId.ForeColor = System.Drawing.Color.Black;
            this.lblSizeId.Location = new System.Drawing.Point(-4, 213);
            this.lblSizeId.Name = "lblSizeId";
            this.lblSizeId.Size = new System.Drawing.Size(38, 20);
            this.lblSizeId.TabIndex = 234;
            this.lblSizeId.Text = "Size";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(215, 179);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 20);
            this.label1.TabIndex = 236;
            this.label1.Text = "Style";
            // 
            // cmbSize
            // 
            this.cmbSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbSize.Enabled = false;
            this.cmbSize.FormattingEnabled = true;
            this.cmbSize.Location = new System.Drawing.Point(105, 214);
            this.cmbSize.Name = "cmbSize";
            this.cmbSize.Size = new System.Drawing.Size(51, 21);
            this.cmbSize.TabIndex = 229;
            // 
            // lblConfig
            // 
            this.lblConfig.AutoSize = true;
            this.lblConfig.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblConfig.ForeColor = System.Drawing.Color.Black;
            this.lblConfig.Location = new System.Drawing.Point(215, 215);
            this.lblConfig.Name = "lblConfig";
            this.lblConfig.Size = new System.Drawing.Size(105, 20);
            this.lblConfig.TabIndex = 231;
            this.lblConfig.Text = "Configuration";
            // 
            // cmbConfig
            // 
            this.cmbConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.cmbConfig.Enabled = false;
            this.cmbConfig.FormattingEnabled = true;
            this.cmbConfig.Location = new System.Drawing.Point(345, 215);
            this.cmbConfig.Name = "cmbConfig";
            this.cmbConfig.Size = new System.Drawing.Size(51, 21);
            this.cmbConfig.TabIndex = 232;
            // 
            // txtPCS
            // 
            this.txtPCS.Location = new System.Drawing.Point(105, 247);
            this.txtPCS.MaxLength = 9;
            this.txtPCS.Name = "txtPCS";
            this.txtPCS.Size = new System.Drawing.Size(51, 21);
            this.txtPCS.TabIndex = 226;
            this.txtPCS.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtQuantity
            // 
            this.txtQuantity.Location = new System.Drawing.Point(345, 248);
            this.txtQuantity.MaxLength = 9;
            this.txtQuantity.Name = "txtQuantity";
            this.txtQuantity.Size = new System.Drawing.Size(51, 21);
            this.txtQuantity.TabIndex = 227;
            this.txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblQuantity.ForeColor = System.Drawing.Color.Black;
            this.lblQuantity.Location = new System.Drawing.Point(215, 249);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(69, 20);
            this.lblQuantity.TabIndex = 235;
            this.lblQuantity.Text = "Quantity";
            // 
            // cmbMakingType
            // 
            this.cmbMakingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMakingType.FormattingEnabled = true;
            this.cmbMakingType.Location = new System.Drawing.Point(345, 289);
            this.cmbMakingType.Name = "cmbMakingType";
            this.cmbMakingType.Size = new System.Drawing.Size(90, 21);
            this.cmbMakingType.TabIndex = 240;
            this.cmbMakingType.SelectedIndexChanged += new System.EventHandler(this.cmbMakingType_SelectedIndexChanged);
            // 
            // lblMakingType
            // 
            this.lblMakingType.AutoSize = true;
            this.lblMakingType.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblMakingType.ForeColor = System.Drawing.Color.Black;
            this.lblMakingType.Location = new System.Drawing.Point(215, 289);
            this.lblMakingType.Name = "lblMakingType";
            this.lblMakingType.Size = new System.Drawing.Size(99, 20);
            this.lblMakingType.TabIndex = 239;
            this.lblMakingType.Text = "Making Type";
            // 
            // cmbRateType
            // 
            this.cmbRateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRateType.FormattingEnabled = true;
            this.cmbRateType.Location = new System.Drawing.Point(105, 290);
            this.cmbRateType.Name = "cmbRateType";
            this.cmbRateType.Size = new System.Drawing.Size(90, 21);
            this.cmbRateType.TabIndex = 238;
            // 
            // lblRateType
            // 
            this.lblRateType.AutoSize = true;
            this.lblRateType.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblRateType.ForeColor = System.Drawing.Color.Black;
            this.lblRateType.Location = new System.Drawing.Point(-4, 290);
            this.lblRateType.Name = "lblRateType";
            this.lblRateType.Size = new System.Drawing.Size(79, 20);
            this.lblRateType.TabIndex = 241;
            this.lblRateType.Text = "Rate Type";
            // 
            // txtgval
            // 
            this.txtgval.Enabled = false;
            this.txtgval.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtgval.Location = new System.Drawing.Point(161, 141);
            this.txtgval.MaxLength = 9;
            this.txtgval.Multiline = true;
            this.txtgval.Name = "txtgval";
            this.txtgval.Size = new System.Drawing.Size(46, 33);
            this.txtgval.TabIndex = 244;
            this.txtgval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtgval.Visible = false;
            // 
            // txtTotalAmount
            // 
            this.txtTotalAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtTotalAmount.Enabled = false;
            this.txtTotalAmount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotalAmount.Location = new System.Drawing.Point(241, 314);
            this.txtTotalAmount.MaxLength = 12;
            this.txtTotalAmount.Multiline = true;
            this.txtTotalAmount.Name = "txtTotalAmount";
            this.txtTotalAmount.ReadOnly = true;
            this.txtTotalAmount.Size = new System.Drawing.Size(51, 33);
            this.txtTotalAmount.TabIndex = 243;
            this.txtTotalAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtAmount
            // 
            this.txtAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtAmount.Enabled = false;
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAmount.Location = new System.Drawing.Point(12, 139);
            this.txtAmount.MaxLength = 12;
            this.txtAmount.Multiline = true;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.ReadOnly = true;
            this.txtAmount.Size = new System.Drawing.Size(140, 33);
            this.txtAmount.TabIndex = 242;
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMakingAmount
            // 
            this.txtMakingAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtMakingAmount.Enabled = false;
            this.txtMakingAmount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMakingAmount.Location = new System.Drawing.Point(80, 320);
            this.txtMakingAmount.MaxLength = 12;
            this.txtMakingAmount.Multiline = true;
            this.txtMakingAmount.Name = "txtMakingAmount";
            this.txtMakingAmount.ReadOnly = true;
            this.txtMakingAmount.Size = new System.Drawing.Size(49, 32);
            this.txtMakingAmount.TabIndex = 245;
            this.txtMakingAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // cmbWastage
            // 
            this.cmbWastage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWastage.Enabled = false;
            this.cmbWastage.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.cmbWastage.FormattingEnabled = true;
            this.cmbWastage.Location = new System.Drawing.Point(66, 357);
            this.cmbWastage.Name = "cmbWastage";
            this.cmbWastage.Size = new System.Drawing.Size(63, 33);
            this.cmbWastage.TabIndex = 247;
            // 
            // cmbMakingDiscType
            // 
            this.cmbMakingDiscType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMakingDiscType.Enabled = false;
            this.cmbMakingDiscType.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.cmbMakingDiscType.FormattingEnabled = true;
            this.cmbMakingDiscType.Location = new System.Drawing.Point(124, 208);
            this.cmbMakingDiscType.Name = "cmbMakingDiscType";
            this.cmbMakingDiscType.Size = new System.Drawing.Size(145, 33);
            this.cmbMakingDiscType.TabIndex = 248;
            // 
            // txtWastageAmount
            // 
            this.txtWastageAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtWastageAmount.Enabled = false;
            this.txtWastageAmount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWastageAmount.Location = new System.Drawing.Point(278, 361);
            this.txtWastageAmount.MaxLength = 12;
            this.txtWastageAmount.Multiline = true;
            this.txtWastageAmount.Name = "txtWastageAmount";
            this.txtWastageAmount.ReadOnly = true;
            this.txtWastageAmount.Size = new System.Drawing.Size(85, 32);
            this.txtWastageAmount.TabIndex = 249;
            this.txtWastageAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMakingDiscTotAmt
            // 
            this.txtMakingDiscTotAmt.BackColor = System.Drawing.SystemColors.Control;
            this.txtMakingDiscTotAmt.Enabled = false;
            this.txtMakingDiscTotAmt.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMakingDiscTotAmt.Location = new System.Drawing.Point(0, 405);
            this.txtMakingDiscTotAmt.MaxLength = 9;
            this.txtMakingDiscTotAmt.Multiline = true;
            this.txtMakingDiscTotAmt.Name = "txtMakingDiscTotAmt";
            this.txtMakingDiscTotAmt.Size = new System.Drawing.Size(80, 32);
            this.txtMakingDiscTotAmt.TabIndex = 250;
            this.txtMakingDiscTotAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMakingDisc
            // 
            this.txtMakingDisc.BackColor = System.Drawing.SystemColors.Control;
            this.txtMakingDisc.Enabled = false;
            this.txtMakingDisc.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMakingDisc.Location = new System.Drawing.Point(14, 325);
            this.txtMakingDisc.MaxLength = 9;
            this.txtMakingDisc.Multiline = true;
            this.txtMakingDisc.Name = "txtMakingDisc";
            this.txtMakingDisc.Size = new System.Drawing.Size(50, 28);
            this.txtMakingDisc.TabIndex = 251;
            this.txtMakingDisc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtWastagePercentage
            // 
            this.txtWastagePercentage.Enabled = false;
            this.txtWastagePercentage.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWastagePercentage.Location = new System.Drawing.Point(177, 407);
            this.txtWastagePercentage.MaxLength = 9;
            this.txtWastagePercentage.Name = "txtWastagePercentage";
            this.txtWastagePercentage.Size = new System.Drawing.Size(88, 33);
            this.txtWastagePercentage.TabIndex = 253;
            this.txtWastagePercentage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtWastageQty
            // 
            this.txtWastageQty.Enabled = false;
            this.txtWastageQty.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtWastageQty.Location = new System.Drawing.Point(177, 443);
            this.txtWastageQty.MaxLength = 9;
            this.txtWastageQty.Name = "txtWastageQty";
            this.txtWastageQty.Size = new System.Drawing.Size(88, 33);
            this.txtWastageQty.TabIndex = 252;
            this.txtWastageQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtMakingRate
            // 
            this.txtMakingRate.Enabled = false;
            this.txtMakingRate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMakingRate.Location = new System.Drawing.Point(8, 357);
            this.txtMakingRate.MaxLength = 9;
            this.txtMakingRate.Multiline = true;
            this.txtMakingRate.Name = "txtMakingRate";
            this.txtMakingRate.Size = new System.Drawing.Size(45, 32);
            this.txtMakingRate.TabIndex = 254;
            this.txtMakingRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Location = new System.Drawing.Point(279, 13);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(100, 40);
            this.btnPOSItemSearch.TabIndex = 255;
            this.btnPOSItemSearch.Text = "Item Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // txtRate
            // 
            this.txtRate.Enabled = false;
            this.txtRate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRate.Location = new System.Drawing.Point(375, 408);
            this.txtRate.MaxLength = 9;
            this.txtRate.Multiline = true;
            this.txtRate.Name = "txtRate";
            this.txtRate.Size = new System.Drawing.Size(60, 32);
            this.txtRate.TabIndex = 256;
            this.txtRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtRate.Visible = false;
            // 
            // txtActMakingRate
            // 
            this.txtActMakingRate.Enabled = false;
            this.txtActMakingRate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActMakingRate.Location = new System.Drawing.Point(135, 372);
            this.txtActMakingRate.MaxLength = 9;
            this.txtActMakingRate.Multiline = true;
            this.txtActMakingRate.Name = "txtActMakingRate";
            this.txtActMakingRate.Size = new System.Drawing.Size(91, 32);
            this.txtActMakingRate.TabIndex = 258;
            this.txtActMakingRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtActMakingRate.Visible = false;
            // 
            // txtChangedMakingRate
            // 
            this.txtChangedMakingRate.Enabled = false;
            this.txtChangedMakingRate.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChangedMakingRate.Location = new System.Drawing.Point(135, 338);
            this.txtChangedMakingRate.MaxLength = 9;
            this.txtChangedMakingRate.Name = "txtChangedMakingRate";
            this.txtChangedMakingRate.Size = new System.Drawing.Size(91, 33);
            this.txtChangedMakingRate.TabIndex = 257;
            this.txtChangedMakingRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtChangedMakingRate.Visible = false;
            // 
            // txtActToAmount
            // 
            this.txtActToAmount.BackColor = System.Drawing.SystemColors.Control;
            this.txtActToAmount.Enabled = false;
            this.txtActToAmount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtActToAmount.Location = new System.Drawing.Point(131, 211);
            this.txtActToAmount.MaxLength = 12;
            this.txtActToAmount.Multiline = true;
            this.txtActToAmount.Name = "txtActToAmount";
            this.txtActToAmount.ReadOnly = true;
            this.txtActToAmount.Size = new System.Drawing.Size(145, 32);
            this.txtActToAmount.TabIndex = 259;
            this.txtActToAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtActToAmount.Visible = false;
            // 
            // txtChangedTotAmount
            // 
            this.txtChangedTotAmount.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtChangedTotAmount.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtChangedTotAmount.Location = new System.Drawing.Point(131, 277);
            this.txtChangedTotAmount.MaxLength = 16;
            this.txtChangedTotAmount.Name = "txtChangedTotAmount";
            this.txtChangedTotAmount.Size = new System.Drawing.Size(144, 33);
            this.txtChangedTotAmount.TabIndex = 260;
            this.txtChangedTotAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtChangedTotAmount.Visible = false;
            // 
            // frmGetEstimateion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 112);
            this.Controls.Add(this.txtActToAmount);
            this.Controls.Add(this.txtChangedTotAmount);
            this.Controls.Add(this.txtActMakingRate);
            this.Controls.Add(this.txtChangedMakingRate);
            this.Controls.Add(this.txtRate);
            this.Controls.Add(this.btnPOSItemSearch);
            this.Controls.Add(this.txtMakingRate);
            this.Controls.Add(this.txtWastagePercentage);
            this.Controls.Add(this.txtWastageQty);
            this.Controls.Add(this.txtMakingDisc);
            this.Controls.Add(this.txtMakingDiscTotAmt);
            this.Controls.Add(this.txtWastageAmount);
            this.Controls.Add(this.cmbMakingDiscType);
            this.Controls.Add(this.cmbWastage);
            this.Controls.Add(this.txtMakingAmount);
            this.Controls.Add(this.txtTotalAmount);
            this.Controls.Add(this.txtgval);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.cmbMakingType);
            this.Controls.Add(this.lblMakingType);
            this.Controls.Add(this.cmbRateType);
            this.Controls.Add(this.lblRateType);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblCode);
            this.Controls.Add(this.cmbCode);
            this.Controls.Add(this.cmbStyle);
            this.Controls.Add(this.lblSizeId);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmbSize);
            this.Controls.Add(this.lblConfig);
            this.Controls.Add(this.cmbConfig);
            this.Controls.Add(this.txtPCS);
            this.Controls.Add(this.txtQuantity);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.txtSKU);
            this.Controls.Add(this.lblRefInv);
            this.Controls.Add(this.btnAdd);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmGetEstimateion";
            this.Text = "Get Estimateion";
            this.Load += new System.EventHandler(this.frmGetEstimateion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSKU;
        private System.Windows.Forms.Label lblRefInv;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnAdd;
        private System.Windows.Forms.Label lblRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCode;
        private System.Windows.Forms.ComboBox cmbCode;
        private System.Windows.Forms.ComboBox cmbStyle;
        private System.Windows.Forms.Label lblSizeId;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSize;
        private System.Windows.Forms.Label lblConfig;
        private System.Windows.Forms.ComboBox cmbConfig;
        private System.Windows.Forms.TextBox txtPCS;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.ComboBox cmbMakingType;
        private System.Windows.Forms.Label lblMakingType;
        private System.Windows.Forms.ComboBox cmbRateType;
        private System.Windows.Forms.Label lblRateType;
        private System.Windows.Forms.TextBox txtgval;
        public System.Windows.Forms.TextBox txtTotalAmount;
        public System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.TextBox txtMakingAmount;
        private System.Windows.Forms.ComboBox cmbWastage;
        private System.Windows.Forms.ComboBox cmbMakingDiscType;
        private System.Windows.Forms.TextBox txtWastageAmount;
        private System.Windows.Forms.TextBox txtMakingDiscTotAmt;
        private System.Windows.Forms.TextBox txtMakingDisc;
        private System.Windows.Forms.TextBox txtWastagePercentage;
        private System.Windows.Forms.TextBox txtWastageQty;
        private System.Windows.Forms.TextBox txtMakingRate;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPOSItemSearch;
        public System.Windows.Forms.TextBox txtRate;
        private System.Windows.Forms.TextBox txtActMakingRate;
        private System.Windows.Forms.TextBox txtChangedMakingRate;
        private System.Windows.Forms.TextBox txtActToAmount;
        private System.Windows.Forms.TextBox txtChangedTotAmount;
    }
}