namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmLanguageForInvoice
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
            this.btnArabic = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnEnglish = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblOption = new System.Windows.Forms.Label();
            this.btnBoth = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // btnArabic
            // 
            this.btnArabic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnArabic.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnArabic.Appearance.Options.UseFont = true;
            this.btnArabic.Location = new System.Drawing.Point(94, 135);
            this.btnArabic.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnArabic.Name = "btnArabic";
            this.btnArabic.Padding = new System.Windows.Forms.Padding(0);
            this.btnArabic.ShowToolTips = false;
            this.btnArabic.Size = new System.Drawing.Size(321, 60);
            this.btnArabic.TabIndex = 7;
            this.btnArabic.Tag = "btnCancel";
            this.btnArabic.Text = "Arabic (العربية)";
            this.btnArabic.Click += new System.EventHandler(this.btnArabic_Click);
            // 
            // btnEnglish
            // 
            this.btnEnglish.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnglish.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEnglish.Appearance.Options.UseFont = true;
            this.btnEnglish.Location = new System.Drawing.Point(94, 75);
            this.btnEnglish.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnEnglish.Name = "btnEnglish";
            this.btnEnglish.Padding = new System.Windows.Forms.Padding(0);
            this.btnEnglish.ShowToolTips = false;
            this.btnEnglish.Size = new System.Drawing.Size(321, 60);
            this.btnEnglish.TabIndex = 6;
            this.btnEnglish.Tag = "btnExchangeFull";
            this.btnEnglish.Text = "English";
            this.btnEnglish.Click += new System.EventHandler(this.btnEnglish_Click);
            // 
            // lblOption
            // 
            this.lblOption.AutoSize = true;
            this.lblOption.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblOption.ForeColor = System.Drawing.Color.Black;
            this.lblOption.Location = new System.Drawing.Point(12, -1);
            this.lblOption.Name = "lblOption";
            this.lblOption.Size = new System.Drawing.Size(524, 65);
            this.lblOption.TabIndex = 5;
            this.lblOption.Text = "Please select one Option";
            // 
            // btnBoth
            // 
            this.btnBoth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBoth.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBoth.Appearance.Options.UseFont = true;
            this.btnBoth.Location = new System.Drawing.Point(94, 195);
            this.btnBoth.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnBoth.Name = "btnBoth";
            this.btnBoth.Padding = new System.Windows.Forms.Padding(0);
            this.btnBoth.ShowToolTips = false;
            this.btnBoth.Size = new System.Drawing.Size(321, 60);
            this.btnBoth.TabIndex = 8;
            this.btnBoth.Tag = "btnCancel";
            this.btnBoth.Text = "Both";
            this.btnBoth.Click += new System.EventHandler(this.btnBoth_Click);
            // 
            // frmLanguageForInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 280);
            this.Controls.Add(this.btnBoth);
            this.Controls.Add(this.btnArabic);
            this.Controls.Add(this.btnEnglish);
            this.Controls.Add(this.lblOption);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmLanguageForInvoice";
            this.Text = "Language For Invoice";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnArabic;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEnglish;
        private System.Windows.Forms.Label lblOption;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnBoth;
    }
}