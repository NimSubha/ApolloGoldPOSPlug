namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmR_OldGoldPurchase
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
            this.rptviewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.SuspendLayout();
            // 
            // rptviewer
            // 
            this.rptviewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rptviewer.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmR_OldGoldPurchase.rdlc";
            this.rptviewer.Location = new System.Drawing.Point(0, 0);
            this.rptviewer.Name = "rptviewer";
            this.rptviewer.Size = new System.Drawing.Size(744, 262);
            this.rptviewer.TabIndex = 0;
            // 
            // frmR_OldGoldPurchase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 262);
            this.Controls.Add(this.rptviewer);
            this.Name = "frmR_OldGoldPurchase";
            this.Text = "frmR_OldGoldPurchase";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.frmR_OldGoldPurchase_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer rptviewer;
    }
}