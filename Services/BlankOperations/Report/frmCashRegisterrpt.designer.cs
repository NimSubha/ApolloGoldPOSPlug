namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class frmCashRegisterrpt
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
            this.dtTransactionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsCashRegister = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsCashRegister();
            this.dtCollectionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dtMOPBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dtTransactionTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsCashRegisterTableAdapters.dtTransactionTableAdapter();
            this.dtCollectionTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsCashRegisterTableAdapters.dtCollectionTableAdapter();
            this.dtMOPTableAdapter = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsCashRegisterTableAdapters.dtMOPTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCashRegister)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCollectionBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtMOPBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dtTransactionBindingSource
            // 
            this.dtTransactionBindingSource.DataMember = "dtTransaction";
            this.dtTransactionBindingSource.DataSource = this.dsCashRegister;
            // 
            // dsCashRegister
            // 
            this.dsCashRegister.DataSetName = "dsCashRegister";
            this.dsCashRegister.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dtCollectionBindingSource
            // 
            this.dtCollectionBindingSource.DataMember = "dtCollection";
            this.dtCollectionBindingSource.DataSource = this.dsCashRegister;
            // 
            // dtMOPBindingSource
            // 
            this.dtMOPBindingSource.DataMember = "dtMOP";
            this.dtMOPBindingSource.DataSource = this.dsCashRegister;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "dsCashRegister";
            reportDataSource1.Value = this.dtTransactionBindingSource;
            reportDataSource2.Name = "dsCollection";
            reportDataSource2.Value = this.dtCollectionBindingSource;
            reportDataSource3.Name = "dsMOP";
            reportDataSource3.Value = this.dtMOPBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.CashRegisterrpt .rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(792, 580);
            this.reportViewer1.TabIndex = 0;
            // 
            // dtTransactionTableAdapter
            // 
            this.dtTransactionTableAdapter.ClearBeforeFill = true;
            // 
            // dtCollectionTableAdapter
            // 
            this.dtCollectionTableAdapter.ClearBeforeFill = true;
            // 
            // dtMOPTableAdapter
            // 
            this.dtMOPTableAdapter.ClearBeforeFill = true;
            // 
            // frmCashRegisterrpt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 580);
            this.Controls.Add(this.reportViewer1);
            this.Name = "frmCashRegisterrpt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.frmCashRegisterrpt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtTransactionBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsCashRegister)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtCollectionBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtMOPBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource dtTransactionBindingSource;
        private dsCashRegister dsCashRegister;
        private System.Windows.Forms.BindingSource dtCollectionBindingSource;
        private System.Windows.Forms.BindingSource dtMOPBindingSource;
        private dsCashRegisterTableAdapters.dtTransactionTableAdapter dtTransactionTableAdapter;
        private dsCashRegisterTableAdapters.dtCollectionTableAdapter dtCollectionTableAdapter;
        private dsCashRegisterTableAdapters.dtMOPTableAdapter dtMOPTableAdapter;
    }
}