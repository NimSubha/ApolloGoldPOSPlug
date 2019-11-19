/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class frmPrintSelection
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
            this.btnThisMonth = new DevExpress.XtraEditors.SimpleButton();
            this.btnLastThreeMonths = new DevExpress.XtraEditors.SimpleButton();
            this.btnLastYear = new DevExpress.XtraEditors.SimpleButton();
            this.btnLastSixMonths = new DevExpress.XtraEditors.SimpleButton();
            this.btnAllTransactions = new DevExpress.XtraEditors.SimpleButton();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.lblHeader = new System.Windows.Forms.Label();
            this.panelBase = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBase)).BeginInit();
            this.panelBase.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnThisMonth
            // 
            this.btnThisMonth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnThisMonth.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThisMonth.Appearance.Options.UseFont = true;
            this.btnThisMonth.Location = new System.Drawing.Point(60, 47);
            this.btnThisMonth.Margin = new System.Windows.Forms.Padding(10, 10, 30, 10);
            this.btnThisMonth.Name = "btnThisMonth";
            this.btnThisMonth.Size = new System.Drawing.Size(127, 57);
            this.btnThisMonth.TabIndex = 0;
            this.btnThisMonth.Text = "This month";
            this.btnThisMonth.Click += new System.EventHandler(this.btnLastMonth_Click);
            // 
            // btnLastThreeMonths
            // 
            this.btnLastThreeMonths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLastThreeMonths.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLastThreeMonths.Appearance.Options.UseFont = true;
            this.btnLastThreeMonths.Location = new System.Drawing.Point(247, 47);
            this.btnLastThreeMonths.Margin = new System.Windows.Forms.Padding(30, 10, 10, 10);
            this.btnLastThreeMonths.Name = "btnLastThreeMonths";
            this.btnLastThreeMonths.Size = new System.Drawing.Size(127, 57);
            this.btnLastThreeMonths.TabIndex = 1;
            this.btnLastThreeMonths.Text = "Three Months";
            this.btnLastThreeMonths.Click += new System.EventHandler(this.btnLastTwoMonths_Click);
            // 
            // btnLastYear
            // 
            this.btnLastYear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLastYear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLastYear.Appearance.Options.UseFont = true;
            this.btnLastYear.Location = new System.Drawing.Point(247, 124);
            this.btnLastYear.Margin = new System.Windows.Forms.Padding(30, 10, 10, 10);
            this.btnLastYear.Name = "btnLastYear";
            this.btnLastYear.Size = new System.Drawing.Size(127, 57);
            this.btnLastYear.TabIndex = 3;
            this.btnLastYear.Text = "One Year";
            this.btnLastYear.Click += new System.EventHandler(this.btnLastYear_Click);
            // 
            // btnLastSixMonths
            // 
            this.btnLastSixMonths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnLastSixMonths.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLastSixMonths.Appearance.Options.UseFont = true;
            this.btnLastSixMonths.Location = new System.Drawing.Point(60, 124);
            this.btnLastSixMonths.Margin = new System.Windows.Forms.Padding(10, 10, 30, 10);
            this.btnLastSixMonths.Name = "btnLastSixMonths";
            this.btnLastSixMonths.Size = new System.Drawing.Size(127, 57);
            this.btnLastSixMonths.TabIndex = 2;
            this.btnLastSixMonths.Text = "Six Months";
            this.btnLastSixMonths.Click += new System.EventHandler(this.btnLastSixMonths_Click);
            // 
            // btnAllTransactions
            // 
            this.btnAllTransactions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAllTransactions.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAllTransactions.Appearance.Options.UseFont = true;
            this.btnAllTransactions.Location = new System.Drawing.Point(60, 201);
            this.btnAllTransactions.Margin = new System.Windows.Forms.Padding(10, 10, 30, 10);
            this.btnAllTransactions.Name = "btnAllTransactions";
            this.btnAllTransactions.Size = new System.Drawing.Size(127, 57);
            this.btnAllTransactions.TabIndex = 5;
            this.btnAllTransactions.Text = "All";
            this.btnAllTransactions.Click += new System.EventHandler(this.btnAllTransactions_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(247, 201);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(30, 10, 10, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeader.AutoSize = true;
            this.lblHeader.BackColor = System.Drawing.Color.Transparent;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeader.Location = new System.Drawing.Point(278, 40);
            this.lblHeader.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeader.Size = new System.Drawing.Size(288, 95);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Tag = "";
            this.lblHeader.Text = "Select Period";
            this.lblHeader.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelBase
            // 
            this.panelBase.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelBase.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelBase.Controls.Add(this.tableLayoutPanel2);
            this.panelBase.Location = new System.Drawing.Point(205, 234);
            this.panelBase.Name = "panelBase";
            this.panelBase.Size = new System.Drawing.Size(434, 305);
            this.panelBase.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 2, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnThisMonth, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnAllTransactions, 1, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnLastThreeMonths, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnLastYear, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.btnLastSixMonths, 1, 2);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 5;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(434, 305);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelBase, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 15);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(845, 654);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // frmPrintSelection
            // 
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(845, 654);
            this.Controls.Add(this.tableLayoutPanel1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmPrintSelection";
            this.Load += new System.EventHandler(this.frmPrintSelection_Load);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelBase)).EndInit();
            this.panelBase.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.SimpleButton btnThisMonth;
        private DevExpress.XtraEditors.SimpleButton btnLastThreeMonths;
        private DevExpress.XtraEditors.SimpleButton btnLastYear;
        private DevExpress.XtraEditors.SimpleButton btnLastSixMonths;
        private DevExpress.XtraEditors.SimpleButton btnAllTransactions;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private System.Windows.Forms.Label lblHeader;
        private DevExpress.XtraEditors.PanelControl panelBase;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    }
}
