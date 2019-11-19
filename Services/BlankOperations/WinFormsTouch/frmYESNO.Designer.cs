namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmYESNO
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
            this.btnNo = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnYes = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // btnNo
            // 
            this.btnNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNo.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNo.Appearance.Options.UseFont = true;
            this.btnNo.Location = new System.Drawing.Point(60, 68);
            this.btnNo.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnNo.Name = "btnNo";
            this.btnNo.Padding = new System.Windows.Forms.Padding(0);
            this.btnNo.ShowToolTips = false;
            this.btnNo.Size = new System.Drawing.Size(152, 48);
            this.btnNo.TabIndex = 9;
            this.btnNo.Tag = "btnCancel";
            this.btnNo.Text = "--";
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnYes
            // 
            this.btnYes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnYes.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnYes.Appearance.Options.UseFont = true;
            this.btnYes.Location = new System.Drawing.Point(60, 14);
            this.btnYes.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnYes.Name = "btnYes";
            this.btnYes.Padding = new System.Windows.Forms.Padding(0);
            this.btnYes.ShowToolTips = false;
            this.btnYes.Size = new System.Drawing.Size(152, 47);
            this.btnYes.TabIndex = 8;
            this.btnYes.Tag = "btnExchangeFull";
            this.btnYes.Text = "--";
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // frmYESNO
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 137);
            this.ControlBox = false;
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmYESNO";
            this.Text = "Select option";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnNo;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnYes;
    }
}