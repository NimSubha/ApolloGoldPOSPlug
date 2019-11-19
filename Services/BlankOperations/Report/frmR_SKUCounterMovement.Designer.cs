namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmR_SKUCounterMovement
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
            this.CounterMovementReportBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsSKUCounterMovement = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsSKUCounterMovement();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.CounterMovementReportTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsSKUCounterMovementTableAdapters.CounterMovementReportTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.CounterMovementReportBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSKUCounterMovement)).BeginInit();
            this.SuspendLayout();
            // 
            // CounterMovementReportBindingSource
            // 
            this.CounterMovementReportBindingSource.DataMember = "CounterMovementReport";
            this.CounterMovementReportBindingSource.DataSource = this.dsSKUCounterMovement;
            // 
            // dsSKUCounterMovement
            // 
            this.dsSKUCounterMovement.DataSetName = "dsSKUCounterMovement";
            this.dsSKUCounterMovement.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "CounterMovementReport";
            reportDataSource1.Value = this.CounterMovementReportBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptCounterMovementReport.rdl" +
    "c";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(840, 516);
            this.reportViewer1.TabIndex = 0;
            // 
            // CounterMovementReportTableAdapter
            // 
            this.CounterMovementReportTableAdapter.ClearBeforeFill = true;
            // 
            // frmR_SKUCounterMovement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 516);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmR_SKUCounterMovement";
            this.Text = "SKU Counter Movement";
            this.Load += new System.EventHandler(this.frmR_SKUCounterMovement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.CounterMovementReportBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsSKUCounterMovement)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource CounterMovementReportBindingSource;
        private dsSKUCounterMovement dsSKUCounterMovement;
        private dsSKUCounterMovementTableAdapters.CounterMovementReportTableAdapter CounterMovementReportTableAdapter;
    }
}