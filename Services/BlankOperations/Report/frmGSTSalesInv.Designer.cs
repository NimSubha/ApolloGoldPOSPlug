namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmGSTSalesInv
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource7 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.DetailBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DSSalesInvoice = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoice();
            this.SubTotalBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TenderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PaymentInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.StdRateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TaxInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TaxInDetailsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.DetailTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.DetailTableAdapter();
            this.SubTotalTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.SubTotalTableAdapter();
            this.TenderTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.TenderTableAdapter();
            this.PaymentInfoTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.PaymentInfoTableAdapter();
            this.StdRateTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.StdRateTableAdapter();
            this.TaxInfoTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.TaxInfoTableAdapter();
            this.TaxInDetailsTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSSalesInvoiceTableAdapters.TaxInDetailsTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.DetailBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DSSalesInvoice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubTotalBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TenderBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaymentInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.StdRateBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInDetailsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // DetailBindingSource
            // 
            this.DetailBindingSource.DataMember = "Detail";
            this.DetailBindingSource.DataSource = this.DSSalesInvoice;
            // 
            // DSSalesInvoice
            // 
            this.DSSalesInvoice.DataSetName = "DSSalesInvoice";
            this.DSSalesInvoice.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // SubTotalBindingSource
            // 
            this.SubTotalBindingSource.DataMember = "SubTotal";
            this.SubTotalBindingSource.DataSource = this.DSSalesInvoice;
            // 
            // TenderBindingSource
            // 
            this.TenderBindingSource.DataMember = "Tender";
            this.TenderBindingSource.DataSource = this.DSSalesInvoice;
            // 
            // PaymentInfoBindingSource
            // 
            this.PaymentInfoBindingSource.DataMember = "PaymentInfo";
            this.PaymentInfoBindingSource.DataSource = this.DSSalesInvoice;
            // 
            // StdRateBindingSource
            // 
            this.StdRateBindingSource.DataMember = "StdRate";
            this.StdRateBindingSource.DataSource = this.DSSalesInvoice;
            // 
            // TaxInfoBindingSource
            // 
            this.TaxInfoBindingSource.DataMember = "TaxInfo";
            this.TaxInfoBindingSource.DataSource = this.DSSalesInvoice;
            // 
            // TaxInDetailsBindingSource
            // 
            this.TaxInDetailsBindingSource.DataMember = "TaxInDetails";
            this.TaxInDetailsBindingSource.DataSource = this.DSSalesInvoice;
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
            reportDataSource4.Name = "PaymentInfo";
            reportDataSource4.Value = this.PaymentInfoBindingSource;
            reportDataSource5.Name = "StdRate";
            reportDataSource5.Value = this.StdRateBindingSource;
            reportDataSource6.Name = "TaxInfo";
            reportDataSource6.Value = this.TaxInfoBindingSource;
            reportDataSource7.Name = "TaxInDetails";
            reportDataSource7.Value = this.TaxInDetailsBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource4);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource5);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource6);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource7);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.RptSaleInv.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(816, 558);
            this.reportViewer1.TabIndex = 1;
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
            // PaymentInfoTableAdapter
            // 
            this.PaymentInfoTableAdapter.ClearBeforeFill = true;
            // 
            // StdRateTableAdapter
            // 
            this.StdRateTableAdapter.ClearBeforeFill = true;
            // 
            // TaxInfoTableAdapter
            // 
            this.TaxInfoTableAdapter.ClearBeforeFill = true;
            // 
            // TaxInDetailsTableAdapter
            // 
            this.TaxInDetailsTableAdapter.ClearBeforeFill = true;
            // 
            // frmGSTSalesInv
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(816, 558);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmGSTSalesInv";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sales Invoice";
            this.Load += new System.EventHandler(this.frmGSTSalesInv_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DetailBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DSSalesInvoice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.SubTotalBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TenderBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaymentInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.StdRateBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInDetailsBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource DetailBindingSource;
        private DSSalesInvoice DSSalesInvoice;
        private System.Windows.Forms.BindingSource SubTotalBindingSource;
        private System.Windows.Forms.BindingSource TenderBindingSource;
        private System.Windows.Forms.BindingSource PaymentInfoBindingSource;
        private System.Windows.Forms.BindingSource StdRateBindingSource;
        private System.Windows.Forms.BindingSource TaxInfoBindingSource;
        private System.Windows.Forms.BindingSource TaxInDetailsBindingSource;
        private DSSalesInvoiceTableAdapters.DetailTableAdapter DetailTableAdapter;
        private DSSalesInvoiceTableAdapters.SubTotalTableAdapter SubTotalTableAdapter;
        private DSSalesInvoiceTableAdapters.TenderTableAdapter TenderTableAdapter;
        private DSSalesInvoiceTableAdapters.PaymentInfoTableAdapter PaymentInfoTableAdapter;
        private DSSalesInvoiceTableAdapters.StdRateTableAdapter StdRateTableAdapter;
        private DSSalesInvoiceTableAdapters.TaxInfoTableAdapter TaxInfoTableAdapter;
        private DSSalesInvoiceTableAdapters.TaxInDetailsTableAdapter TaxInDetailsTableAdapter;
    }
}