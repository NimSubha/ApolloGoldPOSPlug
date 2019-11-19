namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmRepairReturn
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
            this.pnlMain = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnOK = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtItemId = new System.Windows.Forms.TextBox();
            this.lblItemId = new System.Windows.Forms.Label();
            this.txtOrderNo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPhoneNumber = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtCustomerAddress = new System.Windows.Forms.TextBox();
            this.lblCustomerAddress = new System.Windows.Forms.Label();
            this.dtPickerDeliveryDate = new System.Windows.Forms.DateTimePicker();
            this.lblDeliveryDate = new System.Windows.Forms.Label();
            this.txtReturnNo = new System.Windows.Forms.TextBox();
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
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMain.Controls.Add(this.panel2);
            this.pnlMain.Controls.Add(this.lblTitle);
            this.pnlMain.Location = new System.Drawing.Point(5, 6);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(710, 312);
            this.pnlMain.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.txtItemId);
            this.panel2.Controls.Add(this.lblItemId);
            this.panel2.Controls.Add(this.txtOrderNo);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.txtPhoneNumber);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.txtCustomerAddress);
            this.panel2.Controls.Add(this.lblCustomerAddress);
            this.panel2.Controls.Add(this.dtPickerDeliveryDate);
            this.panel2.Controls.Add(this.lblDeliveryDate);
            this.panel2.Controls.Add(this.txtReturnNo);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtCustomerName);
            this.panel2.Controls.Add(this.txtCustomerAccount);
            this.panel2.Controls.Add(this.lblCustomerAccount);
            this.panel2.Controls.Add(this.dTPickerOrderDate);
            this.panel2.Controls.Add(this.lblOrderDate);
            this.panel2.Controls.Add(this.lblOrderNo);
            this.panel2.Location = new System.Drawing.Point(5, 42);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(697, 248);
            this.panel2.TabIndex = 3;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(255, 184);
            this.btnOK.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Padding = new System.Windows.Forms.Padding(0);
            this.btnOK.ShowToolTips = false;
            this.btnOK.Size = new System.Drawing.Size(97, 45);
            this.btnOK.TabIndex = 5;
            this.btnOK.Tag = "btnOK";
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(377, 184);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(0);
            this.btnCancel.ShowToolTips = false;
            this.btnCancel.Size = new System.Drawing.Size(97, 45);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Tag = "btnCancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txtItemId
            // 
            this.txtItemId.BackColor = System.Drawing.SystemColors.Control;
            this.txtItemId.Enabled = false;
            this.txtItemId.Location = new System.Drawing.Point(113, 58);
            this.txtItemId.MaxLength = 20;
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.ReadOnly = true;
            this.txtItemId.Size = new System.Drawing.Size(211, 21);
            this.txtItemId.TabIndex = 7;
            // 
            // lblItemId
            // 
            this.lblItemId.AutoSize = true;
            this.lblItemId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblItemId.Location = new System.Drawing.Point(6, 61);
            this.lblItemId.Name = "lblItemId";
            this.lblItemId.Size = new System.Drawing.Size(50, 13);
            this.lblItemId.TabIndex = 22;
            this.lblItemId.Text = "Item Id.:";
            // 
            // txtOrderNo
            // 
            this.txtOrderNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtOrderNo.Enabled = false;
            this.txtOrderNo.Location = new System.Drawing.Point(113, 33);
            this.txtOrderNo.MaxLength = 20;
            this.txtOrderNo.Name = "txtOrderNo";
            this.txtOrderNo.ReadOnly = true;
            this.txtOrderNo.Size = new System.Drawing.Size(211, 21);
            this.txtOrderNo.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(6, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Repair Batch Id.:";
            // 
            // txtPhoneNumber
            // 
            this.txtPhoneNumber.BackColor = System.Drawing.SystemColors.Control;
            this.txtPhoneNumber.Enabled = false;
            this.txtPhoneNumber.Location = new System.Drawing.Point(113, 139);
            this.txtPhoneNumber.MaxLength = 20;
            this.txtPhoneNumber.Name = "txtPhoneNumber";
            this.txtPhoneNumber.ReadOnly = true;
            this.txtPhoneNumber.Size = new System.Drawing.Size(211, 21);
            this.txtPhoneNumber.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(6, 144);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 13);
            this.label3.TabIndex = 16;
            this.label3.Text = "Phone Number:";
            // 
            // txtCustomerAddress
            // 
            this.txtCustomerAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAddress.Enabled = false;
            this.txtCustomerAddress.Location = new System.Drawing.Point(113, 112);
            this.txtCustomerAddress.MaxLength = 20;
            this.txtCustomerAddress.Name = "txtCustomerAddress";
            this.txtCustomerAddress.ReadOnly = true;
            this.txtCustomerAddress.Size = new System.Drawing.Size(575, 21);
            this.txtCustomerAddress.TabIndex = 10;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.AutoSize = true;
            this.lblCustomerAddress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAddress.Location = new System.Drawing.Point(6, 115);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAddress.TabIndex = 14;
            this.lblCustomerAddress.Text = "Customer Address:";
            // 
            // dtPickerDeliveryDate
            // 
            this.dtPickerDeliveryDate.Location = new System.Drawing.Point(477, 6);
            this.dtPickerDeliveryDate.Name = "dtPickerDeliveryDate";
            this.dtPickerDeliveryDate.Size = new System.Drawing.Size(211, 21);
            this.dtPickerDeliveryDate.TabIndex = 4;
            // 
            // lblDeliveryDate
            // 
            this.lblDeliveryDate.AutoSize = true;
            this.lblDeliveryDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDeliveryDate.Location = new System.Drawing.Point(370, 8);
            this.lblDeliveryDate.Name = "lblDeliveryDate";
            this.lblDeliveryDate.Size = new System.Drawing.Size(76, 13);
            this.lblDeliveryDate.TabIndex = 12;
            this.lblDeliveryDate.Text = "Delivery Date:";
            // 
            // txtReturnNo
            // 
            this.txtReturnNo.BackColor = System.Drawing.SystemColors.Control;
            this.txtReturnNo.Enabled = false;
            this.txtReturnNo.Location = new System.Drawing.Point(113, 6);
            this.txtReturnNo.MaxLength = 20;
            this.txtReturnNo.Name = "txtReturnNo";
            this.txtReturnNo.ReadOnly = true;
            this.txtReturnNo.Size = new System.Drawing.Size(211, 21);
            this.txtReturnNo.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(370, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Customer Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerName.Enabled = false;
            this.txtCustomerName.Location = new System.Drawing.Point(477, 84);
            this.txtCustomerName.MaxLength = 60;
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.ReadOnly = true;
            this.txtCustomerName.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerName.TabIndex = 9;
            // 
            // txtCustomerAccount
            // 
            this.txtCustomerAccount.BackColor = System.Drawing.SystemColors.Control;
            this.txtCustomerAccount.Enabled = false;
            this.txtCustomerAccount.Location = new System.Drawing.Point(113, 85);
            this.txtCustomerAccount.MaxLength = 20;
            this.txtCustomerAccount.Name = "txtCustomerAccount";
            this.txtCustomerAccount.ReadOnly = true;
            this.txtCustomerAccount.Size = new System.Drawing.Size(211, 21);
            this.txtCustomerAccount.TabIndex = 8;
            // 
            // lblCustomerAccount
            // 
            this.lblCustomerAccount.AutoSize = true;
            this.lblCustomerAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblCustomerAccount.Location = new System.Drawing.Point(6, 87);
            this.lblCustomerAccount.Name = "lblCustomerAccount";
            this.lblCustomerAccount.Size = new System.Drawing.Size(99, 13);
            this.lblCustomerAccount.TabIndex = 8;
            this.lblCustomerAccount.Text = "Customer Account:";
            // 
            // dTPickerOrderDate
            // 
            this.dTPickerOrderDate.Enabled = false;
            this.dTPickerOrderDate.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dTPickerOrderDate.Location = new System.Drawing.Point(477, 33);
            this.dTPickerOrderDate.Name = "dTPickerOrderDate";
            this.dTPickerOrderDate.Size = new System.Drawing.Size(211, 21);
            this.dTPickerOrderDate.TabIndex = 6;
            // 
            // lblOrderDate
            // 
            this.lblOrderDate.AutoSize = true;
            this.lblOrderDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblOrderDate.Location = new System.Drawing.Point(370, 34);
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
            this.lblOrderNo.Size = new System.Drawing.Size(64, 13);
            this.lblOrderNo.TabIndex = 2;
            this.lblOrderNo.Text = "Return No.:";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(225, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(273, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Ornament Repair Return";
            // 
            // frmRepairReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(720, 321);
            this.Controls.Add(this.pnlMain);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmRepairReturn";
            this.Text = "Repair Return";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.TextBox txtPhoneNumber;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox txtCustomerAddress;
        private System.Windows.Forms.Label lblCustomerAddress;
        private System.Windows.Forms.DateTimePicker dtPickerDeliveryDate;
        private System.Windows.Forms.Label lblDeliveryDate;
        private System.Windows.Forms.TextBox txtReturnNo;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtCustomerName;
        public System.Windows.Forms.TextBox txtCustomerAccount;
        private System.Windows.Forms.Label lblCustomerAccount;
        private System.Windows.Forms.DateTimePicker dTPickerOrderDate;
        private System.Windows.Forms.Label lblOrderDate;
        private System.Windows.Forms.Label lblOrderNo;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtOrderNo;
        private System.Windows.Forms.Label label4;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOK;
        private System.Windows.Forms.TextBox txtItemId;
        private System.Windows.Forms.Label lblItemId;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
    }
}