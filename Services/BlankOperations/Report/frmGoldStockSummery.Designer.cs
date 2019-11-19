namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmGoldStockSummery
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dsGoldStockSummary = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsGoldStockSummary();
            this.GetGoldStockBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.GetGoldStock = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsGoldStockSummaryTableAdapters.GetGoldStock();
            ((System.ComponentModel.ISupportInitialize)(this.dsGoldStockSummary)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GetGoldStockBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "GoldStockSummary";
            reportDataSource1.Value = this.GetGoldStockBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptGoldStockSummary.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(818, 518);
            this.reportViewer1.TabIndex = 1;
            // 
            // dsGoldStockSummary
            // 
            this.dsGoldStockSummary.DataSetName = "dsGoldStockSummary";
            this.dsGoldStockSummary.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // GetGoldStockBindingSource
            // 
            this.GetGoldStockBindingSource.DataMember = "GetGoldStock";
            this.GetGoldStockBindingSource.DataSource = this.dsGoldStockSummary;
            // 
            // GetGoldStock
            // 
            this.GetGoldStock.ClearBeforeFill = true;
            // 
            // frmGoldStockSummery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 518);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmGoldStockSummery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gold Stock Summery";
            this.Load += new System.EventHandler(this.frmGoldStockSummery_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsGoldStockSummary)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GetGoldStockBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource GetGoldStockBindingSource;
        private dsGoldStockSummary dsGoldStockSummary;
        private dsGoldStockSummaryTableAdapters.GetGoldStock GetGoldStock;
    }
}