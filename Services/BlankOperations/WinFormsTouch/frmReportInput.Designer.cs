namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmReportInput
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
            this.lblparam1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnAllCounter = new System.Windows.Forms.Button();
            this.cmbCounter = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // lblparam1
            // 
            this.lblparam1.AutoSize = true;
            this.lblparam1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblparam1.Location = new System.Drawing.Point(11, 15);
            this.lblparam1.Name = "lblparam1";
            this.lblparam1.Size = new System.Drawing.Size(50, 13);
            this.lblparam1.TabIndex = 96;
            this.lblparam1.Text = "Counter :";
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnOk.Location = new System.Drawing.Point(180, 38);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(94, 43);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnAllCounter
            // 
            this.btnAllCounter.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAllCounter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnAllCounter.Location = new System.Drawing.Point(80, 38);
            this.btnAllCounter.Name = "btnAllCounter";
            this.btnAllCounter.Size = new System.Drawing.Size(94, 43);
            this.btnAllCounter.TabIndex = 97;
            this.btnAllCounter.Text = "All Counter";
            this.btnAllCounter.UseVisualStyleBackColor = true;
            this.btnAllCounter.Click += new System.EventHandler(this.btnAllCounter_Click);
            // 
            // cmbCounter
            // 
            this.cmbCounter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCounter.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.cmbCounter.FormattingEnabled = true;
            this.cmbCounter.Location = new System.Drawing.Point(67, 12);
            this.cmbCounter.Name = "cmbCounter";
            this.cmbCounter.Size = new System.Drawing.Size(207, 21);
            this.cmbCounter.TabIndex = 98;
            // 
            // frmReportInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(297, 90);
            this.Controls.Add(this.cmbCounter);
            this.Controls.Add(this.btnAllCounter);
            this.Controls.Add(this.lblparam1);
            this.Controls.Add(this.btnOk);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmReportInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report parameter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblparam1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnAllCounter;
        private System.Windows.Forms.ComboBox cmbCounter;
    }
}