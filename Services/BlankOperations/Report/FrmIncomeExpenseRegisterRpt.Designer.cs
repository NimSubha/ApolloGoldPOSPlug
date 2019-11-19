namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    partial class FrmIncomeExpenseRegisterRpt
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
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.dsIncomeExpenseRegister = new Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.dsIncomeExpenseRegister();
            ((System.ComponentModel.ISupportInitialize)(this.dsIncomeExpenseRegister)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.Rpt_IncomeExpenseRegister.rd" +
    "lc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(993, 559);
            this.reportViewer1.TabIndex = 0;
            // 
            // dsIncomeExpenseRegister
            // 
            this.dsIncomeExpenseRegister.DataSetName = "dsIncomeExpenseRegister";
            this.dsIncomeExpenseRegister.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FrmIncomeExpenseRegisterRpt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(993, 559);
            this.Controls.Add(this.reportViewer1);
            this.Name = "FrmIncomeExpenseRegisterRpt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.FrmIncomeExpenseRegisterRpt_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsIncomeExpenseRegister)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Reporting.WinForms.ReportViewer reportViewer1;
        private dsIncomeExpenseRegister dsIncomeExpenseRegister;




    }
}