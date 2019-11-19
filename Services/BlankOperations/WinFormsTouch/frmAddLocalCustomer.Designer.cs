namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmAddLocalCustomer
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
            this.txtContactNo = new System.Windows.Forms.TextBox();
            this.lblContactNo = new System.Windows.Forms.Label();
            this.txtaddress = new System.Windows.Forms.TextBox();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnGTId = new DevExpress.XtraEditors.SimpleButton();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.txtContactNo);
            this.panel2.Controls.Add(this.lblContactNo);
            this.panel2.Controls.Add(this.txtaddress);
            this.panel2.Controls.Add(this.lblCustomerAddress);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSubmit);
            this.panel2.Controls.Add(this.btnGTId);
            this.panel2.Controls.Add(this.txtCustomerName);
            this.panel2.Controls.Add(this.lblCustomerName);
            this.panel2.Location = new System.Drawing.Point(10, 78);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(365, 238);
            this.panel2.TabIndex = 17;
            // 
            // txtContactNo
            // 
            this.txtContactNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtContactNo.Location = new System.Drawing.Point(107, 122);
            this.txtContactNo.MaxLength = 20;
            this.txtContactNo.Name = "txtContactNo";
            this.txtContactNo.Size = new System.Drawing.Size(194, 21);
            this.txtContactNo.TabIndex = 3;
            this.txtContactNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtContactNo_KeyPress);
            // 
            // lblContactNo
            // 
            this.lblContactNo.AutoSize = true;
            this.lblContactNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblContactNo.Location = new System.Drawing.Point(6, 125);
            this.lblContactNo.Name = "lblContactNo";
            this.lblContactNo.Size = new System.Drawing.Size(68, 13);
            this.lblContactNo.TabIndex = 12;
            this.lblContactNo.Text = "Contact No :";
            // 
            // txtaddress
            // 
            this.txtaddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtaddress.Location = new System.Drawing.Point(107, 48);
            this.txtaddress.MaxLength = 200;
            this.txtaddress.Multiline = true;
            this.txtaddress.Name = "txtaddress";
            this.txtaddress.Size = new System.Drawing.Size(194, 68);
            this.txtaddress.TabIndex = 2;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAddress.Location = new System.Drawing.Point(6, 51);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(53, 13);
            this.lblCustomerAddress.TabIndex = 10;
            this.lblCustomerAddress.Text = "Address :";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(199, 182);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(99, 182);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 4;
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
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Location = new System.Drawing.Point(107, 21);
            this.txtCustomerName.MaxLength = 30;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(194, 21);
            this.txtCustomerName.TabIndex = 1;
            this.txtCustomerName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtCustomerName_KeyPress);
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerName.Location = new System.Drawing.Point(6, 24);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(41, 13);
            this.lblCustomerName.TabIndex = 1;
            this.lblCustomerName.Text = "Name :";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(87, 26);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(236, 32);
            this.lblTitle.TabIndex = 18;
            this.lblTitle.Text = "Add Local Customer ";
            // 
            // frmAddLocalCustomer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(390, 327);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel2);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmAddLocalCustomer";
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
        public System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.Label lblTitle;
        public System.Windows.Forms.TextBox txtContactNo;
        private System.Windows.Forms.Label lblContactNo;
        public System.Windows.Forms.TextBox txtaddress;
        private System.Windows.Forms.Label lblCustomerAddress;
    }
}