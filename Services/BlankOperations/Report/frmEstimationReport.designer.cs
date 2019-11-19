namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmEstimationReport
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
            this.DetailBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsEstimation = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsEstimation();
            this.IngredientBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.DetailTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsEstimationTableAdapters.DetailTableAdapter();
            this.IngredientTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsEstimationTableAdapters.IngredientTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.DetailBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsEstimation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IngredientBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // DetailBindingSource
            // 
            this.DetailBindingSource.DataMember = "Detail";
            this.DetailBindingSource.DataSource = this.dsEstimation;
            // 
            // dsEstimation
            // 
            this.dsEstimation.DataSetName = "dsEstimation";
            this.dsEstimation.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // IngredientBindingSource
            // 
            this.IngredientBindingSource.DataMember = "Ingredient";
            this.IngredientBindingSource.DataSource = this.dsEstimation;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "Detail";
            reportDataSource1.Value = this.DetailBindingSource;
            reportDataSource2.Name = "Ingredient";
            reportDataSource2.Value = this.IngredientBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.rptEstimation.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(716, 464);
            this.reportViewer1.TabIndex = 0;
            // 
            // DetailTableAdapter
            // 
            this.DetailTableAdapter.ClearBeforeFill = true;
            // 
            // IngredientTableAdapter
            // 
            this.IngredientTableAdapter.ClearBeforeFill = true;
            // 
            // frmEstimationReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(716, 464);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmEstimationReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Estimation Report";
            this.Load += new System.EventHandler(this.frmEstimationReport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DetailBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsEstimation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IngredientBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource DetailBindingSource;
        private dsEstimation dsEstimation;
        private System.Windows.Forms.BindingSource IngredientBindingSource;
        private dsEstimationTableAdapters.DetailTableAdapter DetailTableAdapter;
        private dsEstimationTableAdapters.IngredientTableAdapter IngredientTableAdapter;
    }
}