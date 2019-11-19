namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmGSSMaturity
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
            this.btnSM = new System.Windows.Forms.Button();
            this.txtSM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCustName = new System.Windows.Forms.TextBox();
            this.btnSearchCustomer = new DevExpress.XtraEditors.SimpleButton();
            this.txtCustAcc = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnGSSSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnGTId = new DevExpress.XtraEditors.SimpleButton();
            this.txtGSSNumber = new System.Windows.Forms.TextBox();
            this.lblGSSNumber = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnSM);
            this.panel2.Controls.Add(this.txtSM);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.txtCustName);
            this.panel2.Controls.Add(this.btnSearchCustomer);
            this.panel2.Controls.Add(this.txtCustAcc);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnGSSSearch);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSubmit);
            this.panel2.Controls.Add(this.btnGTId);
            this.panel2.Controls.Add(this.txtGSSNumber);
            this.panel2.Controls.Add(this.lblGSSNumber);
            this.panel2.Location = new System.Drawing.Point(12, 44);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(389, 176);
            this.panel2.TabIndex = 16;
            // 
            // btnSM
            // 
            this.btnSM.Location = new System.Drawing.Point(319, 92);
            this.btnSM.Name = "btnSM";
            this.btnSM.Size = new System.Drawing.Size(53, 21);
            this.btnSM.TabIndex = 180;
            this.btnSM.Text = "Search Sales man";
            this.btnSM.UseVisualStyleBackColor = true;
            this.btnSM.Click += new System.EventHandler(this.btnSM_Click);
            // 
            // txtSM
            // 
            this.txtSM.BackColor = System.Drawing.SystemColors.Control;
            this.txtSM.Enabled = false;
            this.txtSM.Location = new System.Drawing.Point(108, 92);
            this.txtSM.MaxLength = 20;
            this.txtSM.Name = "txtSM";
            this.txtSM.ReadOnly = true;
            this.txtSM.Size = new System.Drawing.Size(194, 21);
            this.txtSM.TabIndex = 179;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(9, 95);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 178;
            this.label6.Text = "Sales man:";
            // 
            // txtCustName
            // 
            this.txtCustName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustName.Enabled = false;
            this.txtCustName.Location = new System.Drawing.Point(108, 36);
            this.txtCustName.MaxLength = 20;
            this.txtCustName.Name = "txtCustName";
            this.txtCustName.Size = new System.Drawing.Size(194, 21);
            this.txtCustName.TabIndex = 13;
            // 
            // btnSearchCustomer
            // 
            this.btnSearchCustomer.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearchCustomer.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearchCustomer.Location = new System.Drawing.Point(319, 3);
            this.btnSearchCustomer.Name = "btnSearchCustomer";
            this.btnSearchCustomer.Size = new System.Drawing.Size(57, 32);
            this.btnSearchCustomer.TabIndex = 12;
            this.btnSearchCustomer.Text = "Search";
            this.btnSearchCustomer.Click += new System.EventHandler(this.btnSearchCustomer_Click);
            // 
            // txtCustAcc
            // 
            this.txtCustAcc.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustAcc.Enabled = false;
            this.txtCustAcc.Location = new System.Drawing.Point(108, 9);
            this.txtCustAcc.MaxLength = 20;
            this.txtCustAcc.Name = "txtCustAcc";
            this.txtCustAcc.Size = new System.Drawing.Size(194, 21);
            this.txtCustAcc.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(9, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Customer Account :";
            // 
            // btnGSSSearch
            // 
            this.btnGSSSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnGSSSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGSSSearch.Location = new System.Drawing.Point(319, 56);
            this.btnGSSSearch.Name = "btnGSSSearch";
            this.btnGSSSearch.Size = new System.Drawing.Size(57, 32);
            this.btnGSSSearch.TabIndex = 2;
            this.btnGSSSearch.Text = "Search";
            this.btnGSSSearch.Click += new System.EventHandler(this.btnGSSSearch_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(282, 125);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 4;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(182, 125);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnGTId
            // 
            this.btnGTId.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnGTId.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGTId.Location = new System.Drawing.Point(652, 67);
            this.btnGTId.Name = "btnGTId";
            this.btnGTId.Size = new System.Drawing.Size(57, 32);
            this.btnGTId.TabIndex = 9;
            this.btnGTId.Text = "Search";
            // 
            // txtGSSNumber
            // 
            this.txtGSSNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtGSSNumber.Enabled = false;
            this.txtGSSNumber.Location = new System.Drawing.Point(108, 63);
            this.txtGSSNumber.MaxLength = 20;
            this.txtGSSNumber.Name = "txtGSSNumber";
            this.txtGSSNumber.Size = new System.Drawing.Size(194, 21);
            this.txtGSSNumber.TabIndex = 1;
            // 
            // lblGSSNumber
            // 
            this.lblGSSNumber.AutoSize = true;
            this.lblGSSNumber.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblGSSNumber.Location = new System.Drawing.Point(9, 66);
            this.lblGSSNumber.Name = "lblGSSNumber";
            this.lblGSSNumber.Size = new System.Drawing.Size(72, 13);
            this.lblGSSNumber.TabIndex = 1;
            this.lblGSSNumber.Text = "FPP Number :";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(135, -2);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(150, 32);
            this.lblTitle.TabIndex = 17;
            this.lblTitle.Text = "FPP Maturity";
            // 
            // frmGSSMaturity
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 222);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel2);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmGSSMaturity";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSubmit;
        private DevExpress.XtraEditors.SimpleButton btnGTId;
        public System.Windows.Forms.TextBox txtGSSNumber;
        private System.Windows.Forms.Label lblGSSNumber;
        private DevExpress.XtraEditors.SimpleButton btnGSSSearch;
        private System.Windows.Forms.Label lblTitle;
        public System.Windows.Forms.TextBox txtCustName;
        private DevExpress.XtraEditors.SimpleButton btnSearchCustomer;
        public System.Windows.Forms.TextBox txtCustAcc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSM;
        public System.Windows.Forms.TextBox txtSM;
        private System.Windows.Forms.Label label6;
    }
}