namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmSaleInv
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
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource3 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource4 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource5 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource6 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.DetailBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DSSaleInv = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInv();
            this.SubTotalBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TenderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.StdRateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TaxInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PaymentInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.DetailTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInvTableAdapters.DetailTableAdapter();
            this.SubTotalTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInvTableAdapters.SubTotalTableAdapter();
            this.TenderTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInvTableAdapters.TenderTableAdapter();
            this.StdRateTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInvTableAdapters.StdRateTableAdapter();
            this.TaxInfoTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInvTableAdapters.TaxInfoTableAdapter();
            this.PaymentInfoTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSaleInvTableAdapters.PaymentInfoTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.DetailBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DSSaleInv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubTotalBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TenderBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StdRateBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaymentInfoBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // DetailBindingSource
            // 
            this.DetailBindingSource.DataMember = "Detail";
            this.DetailBindingSource.DataSource = this.DSSaleInv;
            // 
            // DSSaleInv
            // 
            this.DSSaleInv.DataSetName = "DSSaleInv";
            this.DSSaleInv.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // SubTotalBindingSource
            // 
            this.SubTotalBindingSource.DataMember = "SubTotal";
            this.SubTotalBindingSource.DataSource = this.DSSaleInv;
            // 
            // TenderBindingSource
            // 
            this.TenderBindingSource.DataMember = "Tender";
            this.TenderBindingSource.DataSource = this.DSSaleInv;
            // 
            // StdRateBindingSource
            // 
            this.StdRateBindingSource.DataMember = "StdRate";
            this.StdRateBindingSource.DataSource = this.DSSaleInv;
            // 
            // TaxInfoBindingSource
            // 
            this.TaxInfoBindingSource.DataMember = "TaxInfo";
            this.TaxInfoBindingSource.DataSource = this.DSSaleInv;
            // 
            // PaymentInfoBindingSource
            // 
            this.PaymentInfoBindingSource.DataMember = "PaymentInfo";
            this.PaymentInfoBindingSource.DataSource = this.DSSaleInv;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "Detail";
            reportDataSource1.Value = this.DetailBindingSource;
            reportDataSource2.Name = "SubTotal";
            reportDataSource2.Value = this.SubTotalBindingSource;
            reportDataSource3.Name = "Tender";
            reportDataSource3.Value = this.TenderBindingSource;
            reportDataSource4.Name = "StdRate";
            reportDataSource4.Value = this.StdRateBindingSource;
            reportDataSource5.Name = "TaxInfo";
            reportDataSource5.Value = this.TaxInfoBindingSource;
            reportDataSource6.Name = "PaymentInfo";
            reportDataSource6.Value = this.PaymentInfoBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource4);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource5);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource6);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.RptSaleInv.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(730, 486);
            this.reportViewer1.TabIndex = 0;
            // 
            // DetailTableAdapter
            // 
            this.DetailTableAdapter.ClearBeforeFill = true;
            // 
            // SubTotalTableAdapter
            // 
            this.SubTotalTableAdapter.ClearBeforeFill = true;
            // 
            // TenderTableAdapter
            // 
            this.TenderTableAdapter.ClearBeforeFill = true;
            // 
            // StdRateTableAdapter
            // 
            this.StdRateTableAdapter.ClearBeforeFill = true;
            // 
            // TaxInfoTableAdapter
            // 
            this.TaxInfoTableAdapter.ClearBeforeFill = true;
            // 
            // PaymentInfoTableAdapter
            // 
            this.PaymentInfoTableAdapter.ClearBeforeFill = true;
            // 
            // frmSaleInv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(730, 486);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmSaleInv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmSaleInv_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DetailBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DSSaleInv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubTotalBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TenderBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StdRateBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaymentInfoBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private DSSaleInv DSSaleInv;
        private System.Windows.Forms.BindingSource DetailBindingSource;
        private DSSaleInvTableAdapters.DetailTableAdapter DetailTableAdapter;
        private System.Windows.Forms.BindingSource SubTotalBindingSource;
        private DSSaleInvTableAdapters.SubTotalTableAdapter SubTotalTableAdapter;
        private System.Windows.Forms.BindingSource TenderBindingSource;
        private DSSaleInvTableAdapters.TenderTableAdapter TenderTableAdapter;
        private System.Windows.Forms.BindingSource StdRateBindingSource;
        private DSSaleInvTableAdapters.StdRateTableAdapter StdRateTableAdapter;
        private System.Windows.Forms.BindingSource TaxInfoBindingSource;
        private DSSaleInvTableAdapters.TaxInfoTableAdapter TaxInfoTableAdapter;
        private System.Windows.Forms.BindingSource PaymentInfoBindingSource;
        private DSSaleInvTableAdapters.PaymentInfoTableAdapter PaymentInfoTableAdapter;
    }
}