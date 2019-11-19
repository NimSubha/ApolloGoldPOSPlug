namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmR_SalesAndSalesReturn
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.GetSalesReportDataBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsSalesAndSalesReturnData = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsSalesAndSalesReturnData();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.GetSalesReportDataTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsSalesAndSalesReturnDataTableAdapters.GetSalesReportDataTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.GetSalesReportDataBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSalesAndSalesReturnData)).BeginInit();
            this.SuspendLayout();
            // 
            // GetSalesReportDataBindingSource
            // 
            this.GetSalesReportDataBindingSource.DataMember = "GetSalesReportData";
            this.GetSalesReportDataBindingSource.DataSource = this.dsSalesAndSalesReturnData;
            // 
            // dsSalesAndSalesReturnData
            // 
            this.dsSalesAndSalesReturnData.DataSetName = "dsSalesAndSalesReturnData";
            this.dsSalesAndSalesReturnData.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource2.Name = "dsSalesAndSalesReturn";
            reportDataSource2.Value = this.GetSalesReportDataBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptSalesAndSalesReturn.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(953, 454);
            this.reportViewer1.TabIndex = 0;
            // 
            // GetSalesReportDataTableAdapter
            // 
            this.GetSalesReportDataTableAdapter.ClearBeforeFill = true;
            // 
            // frmR_SalesAndSalesReturn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(953, 454);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmR_SalesAndSalesReturn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Report Sales And Sales Return";
            this.Load += new System.EventHandler(this.frmR_SalesAndSalesReturn_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GetSalesReportDataBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSalesAndSalesReturnData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource GetSalesReportDataBindingSource;
        private dsSalesAndSalesReturnData dsSalesAndSalesReturnData;
        private dsSalesAndSalesReturnDataTableAdapters.GetSalesReportDataTableAdapter GetSalesReportDataTableAdapter;
    }
}