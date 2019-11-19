namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmRetailStockCounting
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCreateVoucher = new System.Windows.Forms.Button();
            this.btnCreatedBy = new System.Windows.Forms.Button();
            this.txtCreatedBY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVoucherNo = new System.Windows.Forms.TextBox();
            this.lblVouNo = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnCloseCounting = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbClosingVoucher = new System.Windows.Forms.ComboBox();
            this.btnCountingClosedBy = new System.Windows.Forms.Button();
            this.txtCountingClosedBy = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbProdType = new System.Windows.Forms.ComboBox();
            this.txtArticle = new System.Windows.Forms.TextBox();
            this.btnArticleSearch = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnArticleSearch);
            this.panel2.Controls.Add(this.txtArticle);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.cmbProdType);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnCreatedBy);
            this.panel2.Controls.Add(this.txtCreatedBY);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtVoucherNo);
            this.panel2.Controls.Add(this.lblVouNo);
            this.panel2.Location = new System.Drawing.Point(4, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(668, 64);
            this.panel2.TabIndex = 187;
            // 
            // btnCreateVoucher
            // 
            this.btnCreateVoucher.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnCreateVoucher.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCreateVoucher.Location = new System.Drawing.Point(678, 12);
            this.btnCreateVoucher.Name = "btnCreateVoucher";
            this.btnCreateVoucher.Size = new System.Drawing.Size(129, 64);
            this.btnCreateVoucher.TabIndex = 3;
            this.btnCreateVoucher.Text = "Create New Voucher";
            this.btnCreateVoucher.UseVisualStyleBackColor = true;
            this.btnCreateVoucher.Click += new System.EventHandler(this.btnCreateVoucher_Click);
            // 
            // btnCreatedBy
            // 
            this.btnCreatedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCreatedBy.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnCreatedBy.Location = new System.Drawing.Point(607, 2);
            this.btnCreatedBy.Name = "btnCreatedBy";
            this.btnCreatedBy.Size = new System.Drawing.Size(42, 23);
            this.btnCreatedBy.TabIndex = 0;
            this.btnCreatedBy.UseVisualStyleBackColor = true;
            this.btnCreatedBy.Click += new System.EventHandler(this.btnCreatedBy_Click);
            // 
            // txtCreatedBY
            // 
            this.txtCreatedBY.BackColor = System.Drawing.SystemColors.Control;
            this.txtCreatedBY.Enabled = false;
            this.txtCreatedBY.Location = new System.Drawing.Point(437, 5);
            this.txtCreatedBY.MaxLength = 20;
            this.txtCreatedBY.Name = "txtCreatedBY";
            this.txtCreatedBY.ReadOnly = true;
            this.txtCreatedBY.Size = new System.Drawing.Size(155, 20);
            this.txtCreatedBY.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(335, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 191;
            this.label1.Text = "Voucher created by:";
            // 
            // txtVoucherNo
            // 
            this.txtVoucherNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtVoucherNo.Enabled = false;
            this.txtVoucherNo.Location = new System.Drawing.Point(89, 6);
            this.txtVoucherNo.MaxLength = 20;
            this.txtVoucherNo.Name = "txtVoucherNo";
            this.txtVoucherNo.ReadOnly = true;
            this.txtVoucherNo.Size = new System.Drawing.Size(179, 20);
            this.txtVoucherNo.TabIndex = 188;
            // 
            // lblVouNo
            // 
            this.lblVouNo.AutoSize = true;
            this.lblVouNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblVouNo.Location = new System.Drawing.Point(3, 9);
            this.lblVouNo.Name = "lblVouNo";
            this.lblVouNo.Size = new System.Drawing.Size(70, 13);
            this.lblVouNo.TabIndex = 187;
            this.lblVouNo.Text = "Voucher No.:";
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.btnCloseCounting);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.cmbClosingVoucher);
            this.panel5.Controls.Add(this.btnCountingClosedBy);
            this.panel5.Controls.Add(this.txtCountingClosedBy);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Location = new System.Drawing.Point(4, 82);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(803, 59);
            this.panel5.TabIndex = 198;
            // 
            // btnCloseCounting
            // 
            this.btnCloseCounting.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCloseCounting.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCloseCounting.Location = new System.Drawing.Point(505, 15);
            this.btnCloseCounting.Name = "btnCloseCounting";
            this.btnCloseCounting.Size = new System.Drawing.Size(70, 31);
            this.btnCloseCounting.TabIndex = 6;
            this.btnCloseCounting.Text = "Close";
            this.btnCloseCounting.UseVisualStyleBackColor = true;
            this.btnCloseCounting.Click += new System.EventHandler(this.btnCloseCounting_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label8.Location = new System.Drawing.Point(3, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(85, 13);
            this.label8.TabIndex = 198;
            this.label8.Text = "Select voucher :";
            // 
            // cmbClosingVoucher
            // 
            this.cmbClosingVoucher.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbClosingVoucher.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbClosingVoucher.FormattingEnabled = true;
            this.cmbClosingVoucher.Location = new System.Drawing.Point(89, 22);
            this.cmbClosingVoucher.Name = "cmbClosingVoucher";
            this.cmbClosingVoucher.Size = new System.Drawing.Size(117, 21);
            this.cmbClosingVoucher.TabIndex = 5;
            // 
            // btnCountingClosedBy
            // 
            this.btnCountingClosedBy.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCountingClosedBy.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnCountingClosedBy.Location = new System.Drawing.Point(408, 20);
            this.btnCountingClosedBy.Name = "btnCountingClosedBy";
            this.btnCountingClosedBy.Size = new System.Drawing.Size(71, 21);
            this.btnCountingClosedBy.TabIndex = 4;
            this.btnCountingClosedBy.UseVisualStyleBackColor = true;
            this.btnCountingClosedBy.Click += new System.EventHandler(this.btnCountingClosedBy_Click);
            // 
            // txtCountingClosedBy
            // 
            this.txtCountingClosedBy.BackColor = System.Drawing.SystemColors.Control;
            this.txtCountingClosedBy.Enabled = false;
            this.txtCountingClosedBy.Location = new System.Drawing.Point(272, 21);
            this.txtCountingClosedBy.MaxLength = 20;
            this.txtCountingClosedBy.Name = "txtCountingClosedBy";
            this.txtCountingClosedBy.ReadOnly = true;
            this.txtCountingClosedBy.Size = new System.Drawing.Size(127, 20);
            this.txtCountingClosedBy.TabIndex = 195;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(212, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 194;
            this.label6.Text = "Closed by:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label7.Location = new System.Drawing.Point(4, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 13);
            this.label7.TabIndex = 185;
            this.label7.Text = "Close counting";
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnExit.Location = new System.Drawing.Point(726, 147);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 31);
            this.btnExit.TabIndex = 7;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(335, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 200;
            this.label2.Text = "Article :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(3, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 202;
            this.label3.Text = "Product Type :";
            // 
            // cmbProdType
            // 
            this.cmbProdType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProdType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbProdType.FormattingEnabled = true;
            this.cmbProdType.Location = new System.Drawing.Point(89, 32);
            this.cmbProdType.Name = "cmbProdType";
            this.cmbProdType.Size = new System.Drawing.Size(179, 21);
            this.cmbProdType.TabIndex = 1;
            // 
            // txtArticle
            // 
            this.txtArticle.BackColor = System.Drawing.SystemColors.Control;
            this.txtArticle.Enabled = false;
            this.txtArticle.Location = new System.Drawing.Point(437, 32);
            this.txtArticle.MaxLength = 20;
            this.txtArticle.Name = "txtArticle";
            this.txtArticle.ReadOnly = true;
            this.txtArticle.Size = new System.Drawing.Size(155, 20);
            this.txtArticle.TabIndex = 203;
            // 
            // btnArticleSearch
            // 
            this.btnArticleSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnArticleSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnArticleSearch.Location = new System.Drawing.Point(607, 32);
            this.btnArticleSearch.Name = "btnArticleSearch";
            this.btnArticleSearch.Size = new System.Drawing.Size(42, 23);
            this.btnArticleSearch.TabIndex = 2;
            this.btnArticleSearch.UseVisualStyleBackColor = true;
            this.btnArticleSearch.Click += new System.EventHandler(this.btnArticleSearch_Click);
            // 
            // frmRetailStockCounting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(808, 181);
            this.ControlBox = false;
            this.Controls.Add(this.btnCreateVoucher);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel2);
            this.Name = "frmRetailStockCounting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Retail Stock Counting Start / Close";
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCreateVoucher;
        private System.Windows.Forms.Button btnCreatedBy;
        public System.Windows.Forms.TextBox txtCreatedBY;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtVoucherNo;
        private System.Windows.Forms.Label lblVouNo;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnCountingClosedBy;
        public System.Windows.Forms.TextBox txtCountingClosedBy;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbClosingVoucher;
        private System.Windows.Forms.Button btnCloseCounting;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbProdType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnArticleSearch;
        public System.Windows.Forms.TextBox txtArticle;
    }
}