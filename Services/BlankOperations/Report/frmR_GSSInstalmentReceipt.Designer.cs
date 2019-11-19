namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmR_GSSInstalmentReceipt
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
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.dsGssInsRecBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsGssInsRec = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsGssInstalmentReceipt();
            this.rptGSSViewer = new Microsoft.Reporting.WinForms.ReportViewer();
            this.tenderTblAd = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsGssInstalmentReceiptTableAdapters.TenderTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dsGssInsRecBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsGssInsRec)).BeginInit();
            this.SuspendLayout();
            // 
            // dsGssInsRecBindingSource
            // 
            this.dsGssInsRecBindingSource.DataMember = "Tender";
            this.dsGssInsRecBindingSource.DataSource = this.dsGssInsRec;
            // 
            // dsGssInsRec
            // 
            this.dsGssInsRec.DataSetName = "dsGssInstalmentReceipt";
            this.dsGssInsRec.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // rptGSSViewer
            // 
            this.rptGSSViewer.AutoSize = true;
            this.rptGSSViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource2.Name = "DataSet1";
            reportDataSource2.Value = this.dsGssInsRecBindingSource;
            this.rptGSSViewer.LocalReport.DataSources.Add(reportDataSource2);
            this.rptGSSViewer.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptGSSInstalmentReceipt.rdlc" +
    "";
            this.rptGSSViewer.Location = new System.Drawing.Point(0, 0);
            this.rptGSSViewer.Name = "rptGSSViewer";
            this.rptGSSViewer.Size = new System.Drawing.Size(714, 448);
            this.rptGSSViewer.TabIndex = 0;
            // 
            // tenderTblAd
            // 
            this.tenderTblAd.ClearBeforeFill = true;
            // 
            // frmR_GSSInstalmentReceipt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 448);
            this.Controls.Add(this.rptGSSViewer);
            this.Name = "frmR_GSSInstalmentReceipt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purchase Advance Receipt";
            this.Load += new System.EventHandler(this.frmR_GSSInstalmentReceipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsGssInsRecBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsGssInsRec)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Reporting.WinForms.ReportViewer rptGSSViewer;
        private dsGssInstalmentReceipt dsGSSInstalmentReceipt;
        private DSSaleInvTableAdapters.TenderTableAdapter TenderTableAdapter;
        private System.Windows.Forms.BindingSource dsGssInsRecBindingSource;
        private dsGssInstalmentReceipt dsGssInsRec;
        private dsGssInstalmentReceiptTableAdapters.TenderTableAdapter tenderTblAd;
    }
}