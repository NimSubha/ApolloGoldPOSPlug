namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmFGTransferred
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnGTId = new DevExpress.XtraEditors.SimpleButton();
            this.btnWHSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnVendorSearch = new DevExpress.XtraEditors.SimpleButton();
            this.lblGTId = new System.Windows.Forms.Label();
            this.txtGTId = new System.Windows.Forms.TextBox();
            this.txtWarehouse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtVendorName = new System.Windows.Forms.TextBox();
            this.txtVendorAc = new System.Windows.Forms.TextBox();
            this.lblVendorAccount = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(205, 21);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(309, 32);
            this.lblTitle.TabIndex = 13;
            this.lblTitle.Text = "FG Transferred to Company";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSubmit);
            this.panel2.Controls.Add(this.btnGTId);
            this.panel2.Controls.Add(this.btnWHSearch);
            this.panel2.Controls.Add(this.btnVendorSearch);
            this.panel2.Controls.Add(this.lblGTId);
            this.panel2.Controls.Add(this.txtGTId);
            this.panel2.Controls.Add(this.txtWarehouse);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtVendorName);
            this.panel2.Controls.Add(this.txtVendorAc);
            this.panel2.Controls.Add(this.lblVendorAccount);
            this.panel2.Location = new System.Drawing.Point(9, 73);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(720, 185);
            this.panel2.TabIndex = 14;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(372, 127);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 90;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(272, 127);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 16;
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
            this.btnGTId.Click += new System.EventHandler(this.btnGTId_Click);
            // 
            // btnWHSearch
            // 
            this.btnWHSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnWHSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnWHSearch.Location = new System.Drawing.Point(289, 68);
            this.btnWHSearch.Name = "btnWHSearch";
            this.btnWHSearch.Size = new System.Drawing.Size(57, 32);
            this.btnWHSearch.TabIndex = 6;
            this.btnWHSearch.Text = "Search";
            this.btnWHSearch.Visible = false;
            this.btnWHSearch.Click += new System.EventHandler(this.btnWHSearch_Click);
            // 
            // btnVendorSearch
            // 
            this.btnVendorSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnVendorSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnVendorSearch.Location = new System.Drawing.Point(289, 15);
            this.btnVendorSearch.Name = "btnVendorSearch";
            this.btnVendorSearch.Size = new System.Drawing.Size(57, 32);
            this.btnVendorSearch.TabIndex = 3;
            this.btnVendorSearch.Text = "Search";
            this.btnVendorSearch.Click += new System.EventHandler(this.btnVendorSearch_Click);
            // 
            // lblGTId
            // 
            this.lblGTId.AutoSize = true;
            this.lblGTId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblGTId.Location = new System.Drawing.Point(358, 75);
            this.lblGTId.Name = "lblGTId";
            this.lblGTId.Size = new System.Drawing.Size(114, 13);
            this.lblGTId.TabIndex = 7;
            this.lblGTId.Text = "Goods Transferred Id:";
            // 
            // txtGTId
            // 
            this.txtGTId.BackColor = System.Drawing.SystemColors.Control;
            this.txtGTId.Location = new System.Drawing.Point(477, 72);
            this.txtGTId.MaxLength = 60;
            this.txtGTId.Name = "txtGTId";
            this.txtGTId.ReadOnly = true;
            this.txtGTId.Size = new System.Drawing.Size(170, 21);
            this.txtGTId.TabIndex = 8;
            // 
            // txtWarehouse
            // 
            this.txtWarehouse.BackColor = System.Drawing.SystemColors.Control;
            this.txtWarehouse.Enabled = false;
            this.txtWarehouse.Location = new System.Drawing.Point(113, 73);
            this.txtWarehouse.MaxLength = 20;
            this.txtWarehouse.Name = "txtWarehouse";
            this.txtWarehouse.Size = new System.Drawing.Size(170, 21);
            this.txtWarehouse.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Warehouse:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(358, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 89;
            this.label1.Text = "Vendor Name:";
            // 
            // txtVendorName
            // 
            this.txtVendorName.BackColor = System.Drawing.SystemColors.Control;
            this.txtVendorName.Enabled = false;
            this.txtVendorName.Location = new System.Drawing.Point(477, 20);
            this.txtVendorName.MaxLength = 60;
            this.txtVendorName.Name = "txtVendorName";
            this.txtVendorName.ReadOnly = true;
            this.txtVendorName.Size = new System.Drawing.Size(170, 21);
            this.txtVendorName.TabIndex = 89;
            // 
            // txtVendorAc
            // 
            this.txtVendorAc.BackColor = System.Drawing.SystemColors.Control;
            this.txtVendorAc.Location = new System.Drawing.Point(113, 21);
            this.txtVendorAc.MaxLength = 20;
            this.txtVendorAc.Name = "txtVendorAc";
            this.txtVendorAc.ReadOnly = true;
            this.txtVendorAc.Size = new System.Drawing.Size(170, 21);
            this.txtVendorAc.TabIndex = 2;
            // 
            // lblVendorAccount
            // 
            this.lblVendorAccount.AutoSize = true;
            this.lblVendorAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblVendorAccount.Location = new System.Drawing.Point(6, 23);
            this.lblVendorAccount.Name = "lblVendorAccount";
            this.lblVendorAccount.Size = new System.Drawing.Size(87, 13);
            this.lblVendorAccount.TabIndex = 1;
            this.lblVendorAccount.Text = "Vendor Account:";
            // 
            // frmFGTransferred
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 271);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblTitle);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmFGTransferred";
            this.Text = "FG Transferred to Company";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSubmit;
        private DevExpress.XtraEditors.SimpleButton btnGTId;
        private DevExpress.XtraEditors.SimpleButton btnWHSearch;
        private DevExpress.XtraEditors.SimpleButton btnVendorSearch;
        private System.Windows.Forms.Label lblGTId;
        public System.Windows.Forms.TextBox txtGTId;
        public System.Windows.Forms.TextBox txtWarehouse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtVendorName;
        public System.Windows.Forms.TextBox txtVendorAc;
        private System.Windows.Forms.Label lblVendorAccount;
        private System.Windows.Forms.Button btnClose;

        
    }
}