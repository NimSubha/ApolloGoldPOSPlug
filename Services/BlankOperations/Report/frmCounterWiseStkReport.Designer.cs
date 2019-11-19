namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmCounterWiseStkReport
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
            this.dtCWStockBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DSCounterWiseStock = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSCounterWiseStock();
            this.rptViewerStk = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dtCWStockTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.DSCounterWiseStockTableAdapters.dtCWStockTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dtCWStockBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DSCounterWiseStock)).BeginInit();
            this.SuspendLayout();
            // 
            // dtCWStockBindingSource
            // 
            this.dtCWStockBindingSource.DataMember = "dtCWStock";
            this.dtCWStockBindingSource.DataSource = this.DSCounterWiseStock;
            // 
            // DSCounterWiseStock
            // 
            this.DSCounterWiseStock.DataSetName = "DSCounterWiseStock";
            this.DSCounterWiseStock.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // rptViewerStk
            // 
            this.rptViewerStk.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "dsCWStk";
            reportDataSource1.Value = this.dtCWStockBindingSource;
            this.rptViewerStk.LocalReport.DataSources.Add(reportDataSource1);
            this.rptViewerStk.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptCounterWiseStock.rdlc";
            this.rptViewerStk.Location = new System.Drawing.Point(0, 0);
            this.rptViewerStk.Name = "rptViewerStk";
            this.rptViewerStk.Size = new System.Drawing.Size(744, 562);
            this.rptViewerStk.TabIndex = 0;
            // 
            // dtCWStockTableAdapter
            // 
            this.dtCWStockTableAdapter.ClearBeforeFill = true;
            // 
            // frmCounterWiseStkReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 562);
            this.Controls.Add(this.rptViewerStk);
            this.Name = "frmCounterWiseStkReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Counter Wise Stock Report";
            this.Load += new System.EventHandler(this.frmCounterWiseStkReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtCWStockBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DSCounterWiseStock)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer rptViewerStk;
        private System.Windows.Forms.BindingSource dtCWStockBindingSource;
        private DSCounterWiseStock DSCounterWiseStock;
        private DSCounterWiseStockTableAdapters.dtCWStockTableAdapter dtCWStockTableAdapter;
    }
}