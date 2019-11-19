namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmOptionSelectionExchangeBuyback
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
        //private void InitializeComponent()
        //{
        //    this.components = new System.ComponentModel.Container();
        //    this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        //    this.Text = "frmOptionSelectionExchangeBuyback";
        //}
        private void InitializeComponent()
        {
            this.lblOption = new System.Windows.Forms.Label();
            this.btnExchange = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCovertToAdv = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnExchangeFull = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.SuspendLayout();
            // 
            // lblOption
            // 
            this.lblOption.AutoSize = true;
            this.lblOption.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblOption.ForeColor = System.Drawing.Color.Black;
            this.lblOption.Location = new System.Drawing.Point(10, 5);
            this.lblOption.Name = "lblOption";
            this.lblOption.Size = new System.Drawing.Size(524, 65);
            this.lblOption.TabIndex = 0;
            this.lblOption.Text = "Please select one Option";
            // 
            // btnExchange
            // 
            this.btnExchange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExchange.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExchange.Appearance.Options.UseFont = true;
            this.btnExchange.Location = new System.Drawing.Point(10, 148);
            this.btnExchange.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnExchange.Name = "btnExchange";
            this.btnExchange.Padding = new System.Windows.Forms.Padding(0);
            this.btnExchange.ShowToolTips = false;
            this.btnExchange.Size = new System.Drawing.Size(521, 60);
            this.btnExchange.TabIndex = 2;
            this.btnExchange.Tag = "btnExchange";
            this.btnExchange.Text = "Exchange";
            this.btnExchange.Click += new System.EventHandler(this.btnExchange_Click);
            // 
            // btnCovertToAdv
            // 
            this.btnCovertToAdv.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCovertToAdv.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCovertToAdv.Appearance.Options.UseFont = true;
            this.btnCovertToAdv.Location = new System.Drawing.Point(10, 218);
            this.btnCovertToAdv.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnCovertToAdv.Name = "btnCovertToAdv";
            this.btnCovertToAdv.Padding = new System.Windows.Forms.Padding(0);
            this.btnCovertToAdv.ShowToolTips = false;
            this.btnCovertToAdv.Size = new System.Drawing.Size(521, 60);
            this.btnCovertToAdv.TabIndex = 3;
            this.btnCovertToAdv.Tag = "btnCovertToAdv";
            this.btnCovertToAdv.Text = "Convert To Advance";
            this.btnCovertToAdv.Click += new System.EventHandler(this.btnCovertToAdv_Click);
            // 
            // btnExchangeFull
            // 
            this.btnExchangeFull.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExchangeFull.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExchangeFull.Appearance.Options.UseFont = true;
            this.btnExchangeFull.Location = new System.Drawing.Point(10, 79);
            this.btnExchangeFull.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnExchangeFull.Name = "btnExchangeFull";
            this.btnExchangeFull.Padding = new System.Windows.Forms.Padding(0);
            this.btnExchangeFull.ShowToolTips = false;
            this.btnExchangeFull.Size = new System.Drawing.Size(521, 60);
            this.btnExchangeFull.TabIndex = 1;
            this.btnExchangeFull.Tag = "btnExchangeFull";
            this.btnExchangeFull.Text = "Cash Back";
            this.btnExchangeFull.Click += new System.EventHandler(this.btnExchangeFull_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(10, 287);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(0);
            this.btnCancel.ShowToolTips = false;
            this.btnCancel.Size = new System.Drawing.Size(521, 60);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Tag = "btnCancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmOptionSelectionExchangeBuyback
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 360);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExchangeFull);
            this.Controls.Add(this.lblOption);
            this.Controls.Add(this.btnCovertToAdv);
            this.Controls.Add(this.btnExchange);
            this.HelpButton = false;
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmOptionSelectionExchangeBuyback";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Exchange / Buy Back";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblOption;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnExchange;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCovertToAdv;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnExchangeFull;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
    }
}