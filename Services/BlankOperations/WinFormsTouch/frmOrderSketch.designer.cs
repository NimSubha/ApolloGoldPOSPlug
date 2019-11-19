namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmOrderSketch
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
            this.label9 = new System.Windows.Forms.Label();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.openFileDiolog = new System.Windows.Forms.OpenFileDialog();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSubmit = new DevExpress.XtraEditors.SimpleButton();
            this.lblBreadCrumbs = new System.Windows.Forms.Label();
            this.picItem = new System.Windows.Forms.PictureBox();
            //((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).BeginInit();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI Light", 30.25F);
            this.label9.Location = new System.Drawing.Point(241, 9);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(270, 55);
            this.label9.TabIndex = 9;
            this.label9.Text = "Sketch Upload";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(199, 521);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Padding = new System.Windows.Forms.Padding(3);
            this.btnSearch.Size = new System.Drawing.Size(135, 60);
            this.btnSearch.TabIndex = 201;
            this.btnSearch.Text = "Upload";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // openFileDiolog
            // 
            this.openFileDiolog.FileName = "openFileDialog1";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(339, 521);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(3);
            this.btnClear.Size = new System.Drawing.Size(135, 60);
            this.btnClear.TabIndex = 202;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.Location = new System.Drawing.Point(480, 521);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(135, 60);
            this.btnClose.TabIndex = 203;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnSubmit
            // 
            this.btnSubmit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSubmit.Appearance.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.Appearance.Options.UseFont = true;
            this.btnSubmit.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSubmit.Location = new System.Drawing.Point(621, 521);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Padding = new System.Windows.Forms.Padding(3);
            this.btnSubmit.Size = new System.Drawing.Size(135, 60);
            this.btnSubmit.TabIndex = 204;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // lblBreadCrumbs
            // 
            this.lblBreadCrumbs.AutoSize = true;
            this.lblBreadCrumbs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblBreadCrumbs.Font = new System.Drawing.Font("Segoe UI Light", 10F);
            this.lblBreadCrumbs.ForeColor = System.Drawing.Color.Black;
            this.lblBreadCrumbs.Location = new System.Drawing.Point(6, 62);
            this.lblBreadCrumbs.Name = "lblBreadCrumbs";
            this.lblBreadCrumbs.Size = new System.Drawing.Size(15, 19);
            this.lblBreadCrumbs.TabIndex = 235;
            this.lblBreadCrumbs.Text = "..";
            // 
            // picItem
            // 
            this.picItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picItem.Location = new System.Drawing.Point(10, 84);
            this.picItem.Name = "picItem";
            this.picItem.Size = new System.Drawing.Size(749, 433);
            this.picItem.TabIndex = 199;
            this.picItem.TabStop = false;
            // 
            // frmOrderSketch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 589);
            this.Controls.Add(this.lblBreadCrumbs);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.picItem);
            this.Controls.Add(this.label9);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmOrderSketch";
            this.Text = "Custom order sketch";
            //((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label9;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        private System.Windows.Forms.PictureBox picItem;
        private System.Windows.Forms.OpenFileDialog openFileDiolog;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnClose;
        private DevExpress.XtraEditors.SimpleButton btnSubmit;
        private DevExpress.XtraEditors.StyleController styleController;
        public System.Windows.Forms.Label lblBreadCrumbs;
    }
}