namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class FrmCustomerAdvBookQtyDays
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
            this.TxtDays = new System.Windows.Forms.TextBox();
            this.lblDays = new System.Windows.Forms.Label();
            this.TxtBookQty = new System.Windows.Forms.TextBox();
            this.lblQty = new System.Windows.Forms.Label();
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.chkAutoGenCommitedQty = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // TxtDays
            // 
            this.TxtDays.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtDays.Location = new System.Drawing.Point(162, 44);
            this.TxtDays.MaxLength = 9;
            this.TxtDays.Name = "TxtDays";
            this.TxtDays.Size = new System.Drawing.Size(101, 27);
            this.TxtDays.TabIndex = 218;
            this.TxtDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblDays
            // 
            this.lblDays.AutoSize = true;
            this.lblDays.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDays.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblDays.Location = new System.Drawing.Point(4, 44);
            this.lblDays.Name = "lblDays";
            this.lblDays.Size = new System.Drawing.Size(155, 20);
            this.lblDays.TabIndex = 224;
            this.lblDays.Text = "Commited For Days :";
            // 
            // TxtBookQty
            // 
            this.TxtBookQty.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtBookQty.Location = new System.Drawing.Point(162, 9);
            this.TxtBookQty.MaxLength = 9;
            this.TxtBookQty.Name = "TxtBookQty";
            this.TxtBookQty.Size = new System.Drawing.Size(101, 27);
            this.TxtBookQty.TabIndex = 217;
            this.TxtBookQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblQty
            // 
            this.lblQty.AutoSize = true;
            this.lblQty.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblQty.Location = new System.Drawing.Point(40, 9);
            this.lblQty.Name = "lblQty";
            this.lblQty.Size = new System.Drawing.Size(119, 20);
            this.lblQty.TabIndex = 223;
            this.lblQty.Text = "Commited Qty :";
            // 
            // btnSubmit
            // 
            this.btnSubmit.Appearance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Appearance.Options.UseForeColor = true;
            this.btnSubmit.Location = new System.Drawing.Point(163, 77);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(100, 30);
            this.btnSubmit.TabIndex = 220;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // chkAutoGenCommitedQty
            // 
            this.chkAutoGenCommitedQty.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAutoGenCommitedQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.chkAutoGenCommitedQty.Location = new System.Drawing.Point(12, 77);
            this.chkAutoGenCommitedQty.Name = "chkAutoGenCommitedQty";
            this.chkAutoGenCommitedQty.Size = new System.Drawing.Size(129, 31);
            this.chkAutoGenCommitedQty.TabIndex = 225;
            this.chkAutoGenCommitedQty.Text = "Auto Generate Commited Qty";
            this.chkAutoGenCommitedQty.UseVisualStyleBackColor = true;
            this.chkAutoGenCommitedQty.CheckedChanged += new System.EventHandler(this.chkAutoGenCommitedQty_CheckedChanged);
            // 
            // FrmCustomerAdvBookQtyDays
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(272, 120);
            this.Controls.Add(this.chkAutoGenCommitedQty);
            this.Controls.Add(this.TxtDays);
            this.Controls.Add(this.lblDays);
            this.Controls.Add(this.TxtBookQty);
            this.Controls.Add(this.lblQty);
            this.Controls.Add(this.btnSubmit);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "FrmCustomerAdvBookQtyDays";
            this.Text = "Customer Advance Book Qty And Days";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox TxtDays;
        private System.Windows.Forms.Label lblDays;
        public System.Windows.Forms.TextBox TxtBookQty;
        private System.Windows.Forms.Label lblQty;
        public LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private System.Windows.Forms.CheckBox chkAutoGenCommitedQty;
    }
}