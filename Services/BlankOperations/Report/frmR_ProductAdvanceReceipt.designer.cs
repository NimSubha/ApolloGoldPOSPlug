namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmR_ProductAdvanceReceipt
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource3 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.TenderBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TaxInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PaymentInfoBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsProductAdvanceReceipt = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsProductAdvanceReceipt();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.TenderTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsGssInstalmentReceiptTableAdapters.TenderTableAdapter();
            this.TaxInfoTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsGssInstalmentReceiptTableAdapters.TaxInfoTableAdapter();
            this.PaymentInfoTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsProductAdvanceReceiptTableAdapters.PaymentInfoTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.TenderBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaymentInfoBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProductAdvanceReceipt)).BeginInit();
            this.SuspendLayout();
            // 
            // PaymentInfoBindingSource
            // 
            this.PaymentInfoBindingSource.DataMember = "PaymentInfo";
            this.PaymentInfoBindingSource.DataSource = this.dsProductAdvanceReceipt;
            // 
            // dsProductAdvanceReceipt
            // 
            this.dsProductAdvanceReceipt.DataSetName = "dsProductAdvanceReceipt";
            this.dsProductAdvanceReceipt.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "DataSet1";
            reportDataSource1.Value = this.TenderBindingSource;
            reportDataSource2.Name = "DataSet2";
            reportDataSource2.Value = this.TaxInfoBindingSource;
            reportDataSource3.Name = "PaymentInfo";
            reportDataSource3.Value = this.PaymentInfoBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptProductAdvanceReceipt.rdl" +
    "c";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(820, 580);
            this.reportViewer1.TabIndex = 0;
            // 
            // TenderTableAdapter
            // 
            this.TenderTableAdapter.ClearBeforeFill = true;
            // 
            // TaxInfoTableAdapter
            // 
            this.TaxInfoTableAdapter.ClearBeforeFill = true;
            // 
            // PaymentInfoTableAdapter
            // 
            this.PaymentInfoTableAdapter.ClearBeforeFill = true;
            // 
            // frmR_ProductAdvanceReceipt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(820, 580);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmR_ProductAdvanceReceipt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Product Advance Receipt";
            this.Load += new System.EventHandler(this.frmR_ProductAdvanceReceipt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.TenderBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TaxInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PaymentInfoBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProductAdvanceReceipt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource TenderBindingSource;
        private System.Windows.Forms.BindingSource TaxInfoBindingSource;
        private dsGssInstalmentReceiptTableAdapters.TenderTableAdapter TenderTableAdapter;
        private dsGssInstalmentReceiptTableAdapters.TaxInfoTableAdapter TaxInfoTableAdapter;
        private dsProductAdvanceReceipt dsProductAdvanceReceipt;
        private System.Windows.Forms.BindingSource PaymentInfoBindingSource;
        private dsProductAdvanceReceiptTableAdapters.PaymentInfoTableAdapter PaymentInfoTableAdapter;
    }
}