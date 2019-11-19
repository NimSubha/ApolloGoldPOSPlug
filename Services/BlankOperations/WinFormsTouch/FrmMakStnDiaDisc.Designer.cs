namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class FrmMakStnDiaDisc
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
            this.btnCLose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtMakDiscValue = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.rdbMakStnDia = new System.Windows.Forms.RadioButton();
            this.txtStnDiscValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDiaDiscValue = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCLose
            // 
            this.btnCLose.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCLose.Appearance.Options.UseForeColor = true;
            this.btnCLose.Location = new System.Drawing.Point(163, 161);
            this.btnCLose.Name = "btnCLose";
            this.btnCLose.Size = new System.Drawing.Size(100, 30);
            this.btnCLose.TabIndex = 5;
            this.btnCLose.Text = "Close";
            this.btnCLose.Click += new System.EventHandler(this.btnCLose_Click);
            // 
            // txtMakDiscValue
            // 
            this.txtMakDiscValue.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMakDiscValue.Location = new System.Drawing.Point(164, 56);
            this.txtMakDiscValue.MaxLength = 9;
            this.txtMakDiscValue.Name = "txtMakDiscValue";
            this.txtMakDiscValue.Size = new System.Drawing.Size(101, 27);
            this.txtMakDiscValue.TabIndex = 1;
            this.txtMakDiscValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMakDiscValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMakDiscValue_KeyPress);
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(16, 59);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(144, 20);
            this.lblRate.TabIndex = 212;
            this.lblRate.Text = "Making Discount %";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Appearance.Options.UseForeColor = true;
            this.btnSubmit.Location = new System.Drawing.Point(47, 161);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 30);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // rdbMakStnDia
            // 
            this.rdbMakStnDia.AutoSize = true;
            this.rdbMakStnDia.Checked = true;
            this.rdbMakStnDia.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.rdbMakStnDia.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.rdbMakStnDia.Location = new System.Drawing.Point(8, 12);
            this.rdbMakStnDia.Name = "rdbMakStnDia";
            this.rdbMakStnDia.Size = new System.Drawing.Size(206, 22);
            this.rdbMakStnDia.TabIndex = 207;
            this.rdbMakStnDia.TabStop = true;
            this.rdbMakStnDia.Text = "Making/Stone/Diamond";
            this.rdbMakStnDia.UseVisualStyleBackColor = true;
            // 
            // txtStnDiscValue
            // 
            this.txtStnDiscValue.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtStnDiscValue.Location = new System.Drawing.Point(164, 91);
            this.txtStnDiscValue.MaxLength = 9;
            this.txtStnDiscValue.Name = "txtStnDiscValue";
            this.txtStnDiscValue.Size = new System.Drawing.Size(101, 27);
            this.txtStnDiscValue.TabIndex = 2;
            this.txtStnDiscValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtStnDiscValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtStnDiscValue_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(28, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 20);
            this.label1.TabIndex = 214;
            this.label1.Text = "Stone Discount %";
            // 
            // txtDiaDiscValue
            // 
            this.txtDiaDiscValue.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiaDiscValue.Location = new System.Drawing.Point(164, 124);
            this.txtDiaDiscValue.MaxLength = 9;
            this.txtDiaDiscValue.Name = "txtDiaDiscValue";
            this.txtDiaDiscValue.Size = new System.Drawing.Size(101, 27);
            this.txtDiaDiscValue.TabIndex = 3;
            this.txtDiaDiscValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiaDiscValue.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiaDiscValue_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(4, 127);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 20);
            this.label2.TabIndex = 216;
            this.label2.Text = "Diamond Discount %";
            // 
            // FrmMakStnDiaDisc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(270, 199);
            this.Controls.Add(this.txtDiaDiscValue);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtStnDiscValue);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCLose);
            this.Controls.Add(this.txtMakDiscValue);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.rdbMakStnDia);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "FrmMakStnDiaDisc";
            this.Text = "Making/Stone/Diamond Discount";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCLose;
        public System.Windows.Forms.TextBox txtMakDiscValue;
        private System.Windows.Forms.Label lblRate;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private System.Windows.Forms.RadioButton rdbMakStnDia;
        public System.Windows.Forms.TextBox txtStnDiscValue;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtDiaDiscValue;
        private System.Windows.Forms.Label label2;
    }
}