/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.CreditMemo.WinFormsTouch
{
    partial class frmPayCreditMemo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPayCreditMemo));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOk = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtCreditMemoId = new DevExpress.XtraEditors.TextEdit();
            this.lblCreditMemoAmountHeading = new DevExpress.XtraEditors.LabelControl();
            this.lblCreditMemoIdHeading = new DevExpress.XtraEditors.LabelControl();
            this.lblCreditMemoAmount = new DevExpress.XtraEditors.LabelControl();
            this.btnValidateCreditMemo = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreditMemoId.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.tableLayoutPanel3);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(887, 691);
            this.panelControl1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(883, 687);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeading.Location = new System.Drawing.Point(246, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(390, 116);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Text = "Credit memo";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnOk, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(33, 608);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(817, 65);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOk.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Location = new System.Drawing.Point(277, 4);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(127, 57);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(412, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtCreditMemoId, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCreditMemoAmountHeading, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblCreditMemoIdHeading, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblCreditMemoAmount, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnValidateCreditMemo, 1, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(251, 228);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(381, 305);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // txtCreditMemoId
            // 
            this.txtCreditMemoId.Location = new System.Drawing.Point(3, 30);
            this.txtCreditMemoId.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.txtCreditMemoId.Name = "txtCreditMemoId";
            this.txtCreditMemoId.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtCreditMemoId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtCreditMemoId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtCreditMemoId.Properties.Appearance.Options.UseBackColor = true;
            this.txtCreditMemoId.Properties.Appearance.Options.UseFont = true;
            this.txtCreditMemoId.Properties.Appearance.Options.UseForeColor = true;
            this.txtCreditMemoId.Properties.Appearance.Options.UseTextOptions = true;
            this.txtCreditMemoId.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.txtCreditMemoId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtCreditMemoId.Size = new System.Drawing.Size(252, 32);
            this.txtCreditMemoId.TabIndex = 2;
            this.txtCreditMemoId.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCreditMemoId_KeyDown);
            // 
            // lblCreditMemoAmountHeading
            // 
            this.lblCreditMemoAmountHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreditMemoAmountHeading.Location = new System.Drawing.Point(3, 85);
            this.lblCreditMemoAmountHeading.Name = "lblCreditMemoAmountHeading";
            this.lblCreditMemoAmountHeading.Size = new System.Drawing.Size(152, 21);
            this.lblCreditMemoAmountHeading.TabIndex = 5;
            this.lblCreditMemoAmountHeading.Text = "Credit memo amount:";
            // 
            // lblCreditMemoIdHeading
            // 
            this.lblCreditMemoIdHeading.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCreditMemoIdHeading.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreditMemoIdHeading.Location = new System.Drawing.Point(3, 3);
            this.lblCreditMemoIdHeading.Name = "lblCreditMemoIdHeading";
            this.lblCreditMemoIdHeading.Size = new System.Drawing.Size(153, 21);
            this.lblCreditMemoIdHeading.TabIndex = 1;
            this.lblCreditMemoIdHeading.Text = "Credit memo number:";
            // 
            // lblCreditMemoAmount
            // 
            this.lblCreditMemoAmount.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreditMemoAmount.Location = new System.Drawing.Point(261, 85);
            this.lblCreditMemoAmount.Name = "lblCreditMemoAmount";
            this.lblCreditMemoAmount.Size = new System.Drawing.Size(56, 21);
            this.lblCreditMemoAmount.TabIndex = 6;
            this.lblCreditMemoAmount.Text = "Amount";
            // 
            // btnValidateCreditMemo
            // 
            this.btnValidateCreditMemo.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnValidateCreditMemo.Appearance.Options.UseFont = true;
            this.btnValidateCreditMemo.Image = ((System.Drawing.Image)(resources.GetObject("btnValidateCreditMemo.Image")));
            this.btnValidateCreditMemo.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnValidateCreditMemo.Location = new System.Drawing.Point(261, 30);
            this.btnValidateCreditMemo.Margin = new System.Windows.Forms.Padding(3, 3, 3, 20);
            this.btnValidateCreditMemo.Name = "btnValidateCreditMemo";
            this.btnValidateCreditMemo.Padding = new System.Windows.Forms.Padding(0);
            this.btnValidateCreditMemo.Size = new System.Drawing.Size(57, 32);
            this.btnValidateCreditMemo.TabIndex = 4;
            this.btnValidateCreditMemo.Tag = "";
            this.btnValidateCreditMemo.Click += new System.EventHandler(this.btnValidateCreditMemo_Click);
            // 
            // frmPayCreditMemo
            // 
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(887, 691);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmPayCreditMemo";
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtCreditMemoId.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraEditors.TextEdit txtCreditMemoId;
        private DevExpress.XtraEditors.LabelControl lblCreditMemoAmountHeading;
        private DevExpress.XtraEditors.LabelControl lblCreditMemoIdHeading;
        private DevExpress.XtraEditors.LabelControl lblCreditMemoAmount;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnValidateCreditMemo;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOk;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;

    }
}
