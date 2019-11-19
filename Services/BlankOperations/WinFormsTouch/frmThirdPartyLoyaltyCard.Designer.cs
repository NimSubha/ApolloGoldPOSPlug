namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmThirdPartyLoyaltyCard
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
            if(disposing && (components != null))
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
            this.btnOk = new System.Windows.Forms.Button();
            this.txtLoyaltyProvider = new System.Windows.Forms.TextBox();
            this.lblVendorAccount = new System.Windows.Forms.Label();
            this.txtLoyaltyNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbTypeCode = new System.Windows.Forms.ComboBox();
            this.lblRateType = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnOk.Location = new System.Drawing.Point(200, 116);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(94, 43);
            this.btnOk.TabIndex = 2;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtLoyaltyProvider
            // 
            this.txtLoyaltyProvider.BackColor = System.Drawing.SystemColors.Control;
            this.txtLoyaltyProvider.Enabled = false;
            this.txtLoyaltyProvider.Location = new System.Drawing.Point(125, 39);
            this.txtLoyaltyProvider.MaxLength = 20;
            this.txtLoyaltyProvider.Name = "txtLoyaltyProvider";
            this.txtLoyaltyProvider.Size = new System.Drawing.Size(146, 20);
            this.txtLoyaltyProvider.TabIndex = 92;
            // 
            // lblVendorAccount
            // 
            this.lblVendorAccount.AutoSize = true;
            this.lblVendorAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblVendorAccount.Location = new System.Drawing.Point(16, 42);
            this.lblVendorAccount.Name = "lblVendorAccount";
            this.lblVendorAccount.Size = new System.Drawing.Size(82, 13);
            this.lblVendorAccount.TabIndex = 93;
            this.lblVendorAccount.Text = "Loyalty Provider";
            // 
            // txtLoyaltyNo
            // 
            this.txtLoyaltyNo.BackColor = System.Drawing.SystemColors.HighlightText;
            this.txtLoyaltyNo.Location = new System.Drawing.Point(125, 65);
            this.txtLoyaltyNo.MaxLength = 20;
            this.txtLoyaltyNo.Name = "txtLoyaltyNo";
            this.txtLoyaltyNo.Size = new System.Drawing.Size(146, 20);
            this.txtLoyaltyNo.TabIndex = 94;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(16, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 95;
            this.label1.Text = "Loyalty No";
            // 
            // cmbTypeCode
            // 
            this.cmbTypeCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTypeCode.FormattingEnabled = true;
            this.cmbTypeCode.Location = new System.Drawing.Point(125, 12);
            this.cmbTypeCode.Name = "cmbTypeCode";
            this.cmbTypeCode.Size = new System.Drawing.Size(90, 21);
            this.cmbTypeCode.TabIndex = 170;
            this.cmbTypeCode.SelectedIndexChanged += new System.EventHandler(this.cmbTypeCode_SelectedIndexChanged);
            // 
            // lblRateType
            // 
            this.lblRateType.AutoSize = true;
            this.lblRateType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.lblRateType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRateType.Location = new System.Drawing.Point(16, 12);
            this.lblRateType.Name = "lblRateType";
            this.lblRateType.Size = new System.Drawing.Size(95, 13);
            this.lblRateType.TabIndex = 171;
            this.lblRateType.Text = "Loyalty Type Code";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(100, 116);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 172;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmThirdPartyLoyaltyCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 162);
            this.ControlBox = false;
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.cmbTypeCode);
            this.Controls.Add(this.lblRateType);
            this.Controls.Add(this.txtLoyaltyNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtLoyaltyProvider);
            this.Controls.Add(this.lblVendorAccount);
            this.Controls.Add(this.btnOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmThirdPartyLoyaltyCard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Third Party Loyalty Card Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        public System.Windows.Forms.TextBox txtLoyaltyProvider;
        private System.Windows.Forms.Label lblVendorAccount;
        public System.Windows.Forms.TextBox txtLoyaltyNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbTypeCode;
        private System.Windows.Forms.Label lblRateType;
        private System.Windows.Forms.Button btnClose;
    }
}