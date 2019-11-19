namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmDiscount
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
            this.rdbMaking = new System.Windows.Forms.RadioButton();
            this.rdbMRP = new System.Windows.Forms.RadioButton();
            this.btnCLose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtDiscValue = new System.Windows.Forms.TextBox();
            this.lblRate = new System.Windows.Forms.Label();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.txtPromoCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rdbPromoMRP = new System.Windows.Forms.RadioButton();
            this.rdbPromoMaking = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // rdbMaking
            // 
            this.rdbMaking.AutoSize = true;
            this.rdbMaking.Checked = true;
            this.rdbMaking.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.rdbMaking.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.rdbMaking.Location = new System.Drawing.Point(161, 40);
            this.rdbMaking.Name = "rdbMaking";
            this.rdbMaking.Size = new System.Drawing.Size(80, 22);
            this.rdbMaking.TabIndex = 0;
            this.rdbMaking.TabStop = true;
            this.rdbMaking.Text = "Making";
            this.rdbMaking.UseVisualStyleBackColor = true;
            this.rdbMaking.CheckedChanged += new System.EventHandler(this.rdbMaking_CheckedChanged);
            // 
            // rdbMRP
            // 
            this.rdbMRP.AutoSize = true;
            this.rdbMRP.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.rdbMRP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.rdbMRP.Location = new System.Drawing.Point(28, 40);
            this.rdbMRP.Name = "rdbMRP";
            this.rdbMRP.Size = new System.Drawing.Size(60, 22);
            this.rdbMRP.TabIndex = 1;
            this.rdbMRP.Text = "MRP";
            this.rdbMRP.UseVisualStyleBackColor = true;
            this.rdbMRP.CheckedChanged += new System.EventHandler(this.rdbMRP_CheckedChanged);
            // 
            // btnCLose
            // 
            this.btnCLose.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnCLose.Appearance.Options.UseForeColor = true;
            this.btnCLose.Location = new System.Drawing.Point(97, 137);
            this.btnCLose.Name = "btnCLose";
            this.btnCLose.Size = new System.Drawing.Size(100, 30);
            this.btnCLose.TabIndex = 2;
            this.btnCLose.Text = "CLOSE";
            this.btnCLose.Click += new System.EventHandler(this.btnCLose_Click);
            // 
            // txtDiscValue
            // 
            this.txtDiscValue.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDiscValue.Location = new System.Drawing.Point(143, 83);
            this.txtDiscValue.MaxLength = 9;
            this.txtDiscValue.Name = "txtDiscValue";
            this.txtDiscValue.Size = new System.Drawing.Size(145, 27);
            this.txtDiscValue.TabIndex = 0;
            this.txtDiscValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblRate
            // 
            this.lblRate.AutoSize = true;
            this.lblRate.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRate.Location = new System.Drawing.Point(24, 86);
            this.lblRate.Name = "lblRate";
            this.lblRate.Size = new System.Drawing.Size(113, 20);
            this.lblRate.TabIndex = 200;
            this.lblRate.Text = "Discount Value";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Appearance.Options.UseForeColor = true;
            this.btnSubmit.Location = new System.Drawing.Point(203, 137);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 30);
            this.btnSubmit.TabIndex = 3;
            this.btnSubmit.Text = "SUBMIT";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // txtPromoCode
            // 
            this.txtPromoCode.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPromoCode.Location = new System.Drawing.Point(143, 83);
            this.txtPromoCode.MaxLength = 20;
            this.txtPromoCode.Name = "txtPromoCode";
            this.txtPromoCode.Size = new System.Drawing.Size(160, 27);
            this.txtPromoCode.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(24, 86);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 20);
            this.label1.TabIndex = 203;
            this.label1.Text = "Promo code";
            // 
            // rdbPromoMRP
            // 
            this.rdbPromoMRP.AutoSize = true;
            this.rdbPromoMRP.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.rdbPromoMRP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.rdbPromoMRP.Location = new System.Drawing.Point(28, 40);
            this.rdbPromoMRP.Name = "rdbPromoMRP";
            this.rdbPromoMRP.Size = new System.Drawing.Size(112, 22);
            this.rdbPromoMRP.TabIndex = 205;
            this.rdbPromoMRP.Text = "Promo MRP";
            this.rdbPromoMRP.UseVisualStyleBackColor = true;
            this.rdbPromoMRP.CheckedChanged += new System.EventHandler(this.rdbPromoMRP_CheckedChanged);
            // 
            // rdbPromoMaking
            // 
            this.rdbPromoMaking.AutoSize = true;
            this.rdbPromoMaking.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold);
            this.rdbPromoMaking.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.rdbPromoMaking.Location = new System.Drawing.Point(161, 40);
            this.rdbPromoMaking.Name = "rdbPromoMaking";
            this.rdbPromoMaking.Size = new System.Drawing.Size(132, 22);
            this.rdbPromoMaking.TabIndex = 204;
            this.rdbPromoMaking.Text = "Promo Making";
            this.rdbPromoMaking.UseVisualStyleBackColor = true;
            this.rdbPromoMaking.CheckedChanged += new System.EventHandler(this.rdbPromoMaking_CheckedChanged);
            // 
            // frmDiscount
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 200);
            this.Controls.Add(this.rdbPromoMRP);
            this.Controls.Add(this.rdbPromoMaking);
            this.Controls.Add(this.txtPromoCode);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCLose);
            this.Controls.Add(this.txtDiscValue);
            this.Controls.Add(this.lblRate);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.rdbMRP);
            this.Controls.Add(this.rdbMaking);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmDiscount";
            this.Text = "Discount";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbMaking;
        private System.Windows.Forms.RadioButton rdbMRP;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCLose;
        public System.Windows.Forms.TextBox txtDiscValue;
        private System.Windows.Forms.Label lblRate;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        public System.Windows.Forms.TextBox txtPromoCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdbPromoMRP;
        private System.Windows.Forms.RadioButton rdbPromoMaking;
    }
}