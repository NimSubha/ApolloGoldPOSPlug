namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class FrmIncomeExpenseRegister
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
            this.btnTerminalClear = new System.Windows.Forms.Button();
            this.btnCashierClear = new System.Windows.Forms.Button();
            this.btnTerminal = new System.Windows.Forms.Button();
            this.txtTerminal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSM = new System.Windows.Forms.Button();
            this.txtSM = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtpTransDate = new System.Windows.Forms.DateTimePicker();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.lblTransDate = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Verdana", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(4, 4);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(372, 29);
            this.lblTitle.TabIndex = 20;
            this.lblTitle.Text = "Income Expense Register       ";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnTerminalClear);
            this.panel2.Controls.Add(this.btnCashierClear);
            this.panel2.Controls.Add(this.btnTerminal);
            this.panel2.Controls.Add(this.txtTerminal);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.btnSM);
            this.panel2.Controls.Add(this.txtSM);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.dtpTransDate);
            this.panel2.Controls.Add(this.btnClose);
            this.panel2.Controls.Add(this.btnSubmit);
            this.panel2.Controls.Add(this.lblTransDate);
            this.panel2.Location = new System.Drawing.Point(6, 35);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(378, 144);
            this.panel2.TabIndex = 19;
            // 
            // btnTerminalClear
            // 
            this.btnTerminalClear.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTerminalClear.Location = new System.Drawing.Point(323, 34);
            this.btnTerminalClear.Name = "btnTerminalClear";
            this.btnTerminalClear.Size = new System.Drawing.Size(47, 23);
            this.btnTerminalClear.TabIndex = 185;
            this.btnTerminalClear.Text = "Clear";
            this.btnTerminalClear.UseVisualStyleBackColor = true;
            this.btnTerminalClear.Click += new System.EventHandler(this.btnTerminalClear_Click_1);
            // 
            // btnCashierClear
            // 
            this.btnCashierClear.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCashierClear.Location = new System.Drawing.Point(323, 63);
            this.btnCashierClear.Name = "btnCashierClear";
            this.btnCashierClear.Size = new System.Drawing.Size(47, 23);
            this.btnCashierClear.TabIndex = 184;
            this.btnCashierClear.Text = "Clear";
            this.btnCashierClear.UseVisualStyleBackColor = true;
            this.btnCashierClear.Click += new System.EventHandler(this.btnCashierClear_Click_1);
            // 
            // btnTerminal
            // 
            this.btnTerminal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTerminal.Location = new System.Drawing.Point(253, 34);
            this.btnTerminal.Name = "btnTerminal";
            this.btnTerminal.Size = new System.Drawing.Size(67, 23);
            this.btnTerminal.TabIndex = 183;
            this.btnTerminal.Text = "Terminal";
            this.btnTerminal.UseVisualStyleBackColor = true;
            this.btnTerminal.Click += new System.EventHandler(this.btnTerminal_Click_1);
            // 
            // txtTerminal
            // 
            this.txtTerminal.BackColor = System.Drawing.SystemColors.Control;
            this.txtTerminal.Enabled = false;
            this.txtTerminal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTerminal.Location = new System.Drawing.Point(120, 34);
            this.txtTerminal.MaxLength = 20;
            this.txtTerminal.Name = "txtTerminal";
            this.txtTerminal.ReadOnly = true;
            this.txtTerminal.Size = new System.Drawing.Size(127, 21);
            this.txtTerminal.TabIndex = 182;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(57, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 181;
            this.label1.Text = "Terminal:";
            // 
            // btnSM
            // 
            this.btnSM.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSM.Location = new System.Drawing.Point(253, 63);
            this.btnSM.Name = "btnSM";
            this.btnSM.Size = new System.Drawing.Size(67, 23);
            this.btnSM.TabIndex = 180;
            this.btnSM.Text = "Cashier";
            this.btnSM.UseVisualStyleBackColor = true;
            this.btnSM.Click += new System.EventHandler(this.btnSM_Click_1);
            // 
            // txtSM
            // 
            this.txtSM.BackColor = System.Drawing.SystemColors.Control;
            this.txtSM.Enabled = false;
            this.txtSM.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSM.Location = new System.Drawing.Point(120, 63);
            this.txtSM.MaxLength = 20;
            this.txtSM.Name = "txtSM";
            this.txtSM.ReadOnly = true;
            this.txtSM.Size = new System.Drawing.Size(127, 21);
            this.txtSM.TabIndex = 179;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label6.Location = new System.Drawing.Point(62, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(56, 13);
            this.label6.TabIndex = 178;
            this.label6.Text = "Cashier:";
            // 
            // dtpTransDate
            // 
            this.dtpTransDate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpTransDate.Location = new System.Drawing.Point(120, 7);
            this.dtpTransDate.Name = "dtpTransDate";
            this.dtpTransDate.Size = new System.Drawing.Size(200, 21);
            this.dtpTransDate.TabIndex = 91;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(277, 92);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 90;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click_1);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(177, 92);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 16;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click_1);
            // 
            // lblTransDate
            // 
            this.lblTransDate.AutoSize = true;
            this.lblTransDate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTransDate.Location = new System.Drawing.Point(6, 8);
            this.lblTransDate.Name = "lblTransDate";
            this.lblTransDate.Size = new System.Drawing.Size(112, 13);
            this.lblTransDate.TabIndex = 1;
            this.lblTransDate.Text = "Transaction Date :";
            // 
            // FrmIncomeExpenseRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 183);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "FrmIncomeExpenseRegister";
            this.Text = "Income Expense Register";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnTerminalClear;
        private System.Windows.Forms.Button btnCashierClear;
        private System.Windows.Forms.Button btnTerminal;
        public System.Windows.Forms.TextBox txtTerminal;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSM;
        public System.Windows.Forms.TextBox txtSM;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker dtpTransDate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label lblTransDate;
    }
}