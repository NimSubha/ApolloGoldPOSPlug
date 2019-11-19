namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmSETSKUScan
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
            this.txtItemId = new DevExpress.XtraEditors.TextEdit();
            this.btnClearProduct = new DevExpress.XtraEditors.SimpleButton();
            this.btnPOSItemSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnCommit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemId.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.labelControl1);
            this.panelControl1.Controls.Add(this.txtItemId);
            this.panelControl1.Controls.Add(this.btnClearProduct);
            this.panelControl1.Controls.Add(this.btnPOSItemSearch);
            this.panelControl1.Location = new System.Drawing.Point(9, 18);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(441, 51);
            this.panelControl1.TabIndex = 179;
            // 
            // labelControl1
            // 
            this.labelControl1.Appearance.Font = new System.Drawing.Font("Segoe UI Light", 11.25F, System.Drawing.FontStyle.Bold);
            this.labelControl1.Location = new System.Drawing.Point(5, 13);
            this.labelControl1.Name = "labelControl1";
            this.labelControl1.Size = new System.Drawing.Size(55, 20);
            this.labelControl1.TabIndex = 179;
            this.labelControl1.Text = "Product";
            // 
            // txtItemId
            // 
            this.txtItemId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtItemId.Location = new System.Drawing.Point(72, 9);
            this.txtItemId.Name = "txtItemId";
            this.txtItemId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtItemId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtItemId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtItemId.Properties.Appearance.Options.UseBackColor = true;
            this.txtItemId.Properties.Appearance.Options.UseFont = true;
            this.txtItemId.Properties.Appearance.Options.UseForeColor = true;
            this.txtItemId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtItemId.Properties.ValidateOnEnterKey = true;
            this.txtItemId.Size = new System.Drawing.Size(236, 32);
            this.txtItemId.TabIndex = 176;
            // 
            // btnClearProduct
            // 
            this.btnClearProduct.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnClearProduct.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClearProduct.Location = new System.Drawing.Point(375, 9);
            this.btnClearProduct.Name = "btnClearProduct";
            this.btnClearProduct.Size = new System.Drawing.Size(57, 32);
            this.btnClearProduct.TabIndex = 178;
            this.btnClearProduct.Text = "Clear";
            this.btnClearProduct.Click += new System.EventHandler(this.btnClearProduct_Click);
            // 
            // btnPOSItemSearch
            // 
            this.btnPOSItemSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnPOSItemSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPOSItemSearch.Location = new System.Drawing.Point(314, 9);
            this.btnPOSItemSearch.Name = "btnPOSItemSearch";
            this.btnPOSItemSearch.Size = new System.Drawing.Size(57, 32);
            this.btnPOSItemSearch.TabIndex = 177;
            this.btnPOSItemSearch.Text = "Search";
            this.btnPOSItemSearch.Click += new System.EventHandler(this.btnPOSItemSearch_Click);
            // 
            // btnCommit
            // 
            this.btnCommit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCommit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCommit.Appearance.Options.UseFont = true;
            this.btnCommit.Location = new System.Drawing.Point(327, 75);
            this.btnCommit.Name = "btnCommit";
            this.btnCommit.Size = new System.Drawing.Size(123, 42);
            this.btnCommit.TabIndex = 181;
            this.btnCommit.Text = "Submit";
            this.btnCommit.Click += new System.EventHandler(this.btnCommit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(199, 75);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(122, 42);
            this.btnCancel.TabIndex = 180;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmSETSKUScan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 119);
            this.ControlBox = false;
            this.Controls.Add(this.btnCommit);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.panelControl1);
            this.Name = "frmSETSKUScan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SET SKU ";
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.panelControl1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtItemId.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private DevExpress.XtraEditors.LabelControl labelControl1;
        private DevExpress.XtraEditors.TextEdit txtItemId;
        private DevExpress.XtraEditors.SimpleButton btnClearProduct;
        private DevExpress.XtraEditors.SimpleButton btnPOSItemSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCommit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
    }
}