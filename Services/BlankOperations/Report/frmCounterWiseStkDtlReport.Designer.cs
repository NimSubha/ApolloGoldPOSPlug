namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmCounterWiseStkDtlReport
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.dtCWStockDtlBindingSource = new System.Windows.Forms.BindingSource();
            this.dsCounterWiseStockDtl = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsCounterWiseStockDtl();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dtCWStockDtlTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsCounterWiseStockDtlTableAdapters.dtCWStockDtlTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dtCWStockDtlBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCounterWiseStockDtl)).BeginInit();
            this.SuspendLayout();
            // 
            // dtCWStockDtlBindingSource
            // 
            this.dtCWStockDtlBindingSource.DataMember = "dtCWStockDtl";
            this.dtCWStockDtlBindingSource.DataSource = this.dsCounterWiseStockDtl;
            // 
            // dsCounterWiseStockDtl
            // 
            this.dsCounterWiseStockDtl.DataSetName = "dsCounterWiseStockDtl";
            this.dsCounterWiseStockDtl.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "dsCWStkDtl";
            reportDataSource1.Value = this.dtCWStockDtlBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptCounterWiseStockDtl.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(761, 464);
            this.reportViewer1.TabIndex = 0;
            // 
            // dtCWStockDtlTableAdapter
            // 
            this.dtCWStockDtlTableAdapter.ClearBeforeFill = true;
            // 
            // frmCounterWiseStkDtlReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(761, 464);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmCounterWiseStkDtlReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmCounterWiseStkDtlReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtCWStockDtlBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCounterWiseStockDtl)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource dtCWStockDtlBindingSource;
        private dsCounterWiseStockDtl dsCounterWiseStockDtl;
        private dsCounterWiseStockDtlTableAdapters.dtCWStockDtlTableAdapter dtCWStockDtlTableAdapter;
    }
}