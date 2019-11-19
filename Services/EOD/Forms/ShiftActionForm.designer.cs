/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    partial class ShiftActionForm
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
            if (disposing)
            {
                if (components != null)
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
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnUseShift = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnNewShift = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnResumeShift = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnNonDrawerMode = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblQuestion = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.AutoSize = true;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.panelControl1.Controls.Add(this.tableLayoutPanel2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(955, 715);
            this.panelControl1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.btnCancel, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.lblMessage, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(951, 711);
            this.tableLayoutPanel2.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(412, 639);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Tag = "";
            this.btnCancel.Text = "Cancel";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.btnUseShift, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnNewShift, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnResumeShift, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnNonDrawerMode, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.lblQuestion, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(330, 237);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(290, 353);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // btnUseShift
            // 
            this.btnUseShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUseShift.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnUseShift.Appearance.Options.UseFont = true;
            this.btnUseShift.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnUseShift.Location = new System.Drawing.Point(10, 55);
            this.btnUseShift.Margin = new System.Windows.Forms.Padding(10);
            this.btnUseShift.Name = "btnUseShift";
            this.btnUseShift.Size = new System.Drawing.Size(270, 57);
            this.btnUseShift.TabIndex = 2;
            this.btnUseShift.Tag = "";
            this.btnUseShift.Text = "Use this shift";
            this.btnUseShift.Visible = false;
            this.btnUseShift.Click += new System.EventHandler(this.btnUseShift_Click);
            // 
            // btnNewShift
            // 
            this.btnNewShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewShift.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNewShift.Appearance.Options.UseFont = true;
            this.btnNewShift.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNewShift.Location = new System.Drawing.Point(10, 132);
            this.btnNewShift.Margin = new System.Windows.Forms.Padding(10);
            this.btnNewShift.Name = "btnNewShift";
            this.btnNewShift.Size = new System.Drawing.Size(270, 57);
            this.btnNewShift.TabIndex = 3;
            this.btnNewShift.Tag = "BtnExtraLong";
            this.btnNewShift.Text = "Open a new shift";
            this.btnNewShift.Click += new System.EventHandler(this.btnNewShift_Click);
            // 
            // btnResumeShift
            // 
            this.btnResumeShift.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResumeShift.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnResumeShift.Appearance.Options.UseFont = true;
            this.btnResumeShift.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnResumeShift.Location = new System.Drawing.Point(10, 209);
            this.btnResumeShift.Margin = new System.Windows.Forms.Padding(10);
            this.btnResumeShift.Name = "btnResumeShift";
            this.btnResumeShift.Size = new System.Drawing.Size(270, 57);
            this.btnResumeShift.TabIndex = 4;
            this.btnResumeShift.Tag = "BtnExtraLong";
            this.btnResumeShift.Text = "Resume an existing shift";
            this.btnResumeShift.Click += new System.EventHandler(this.btnResumeShift_Click);
            // 
            // btnNonDrawerMode
            // 
            this.btnNonDrawerMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNonDrawerMode.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNonDrawerMode.Appearance.Options.UseFont = true;
            this.btnNonDrawerMode.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnNonDrawerMode.Location = new System.Drawing.Point(10, 286);
            this.btnNonDrawerMode.Margin = new System.Windows.Forms.Padding(10);
            this.btnNonDrawerMode.Name = "btnNonDrawerMode";
            this.btnNonDrawerMode.Size = new System.Drawing.Size(270, 57);
            this.btnNonDrawerMode.TabIndex = 5;
            this.btnNonDrawerMode.Tag = "BtnExtraLong";
            this.btnNonDrawerMode.Text = "Perform a non-drawer operation";
            this.btnNonDrawerMode.Click += new System.EventHandler(this.btnNonDrawerMode_Click);
            // 
            // lblQuestion
            // 
            this.lblQuestion.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblQuestion.AutoSize = true;
            this.lblQuestion.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuestion.Location = new System.Drawing.Point(26, 0);
            this.lblQuestion.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.lblQuestion.Name = "lblQuestion";
            this.lblQuestion.Size = new System.Drawing.Size(237, 25);
            this.lblQuestion.TabIndex = 1;
            this.lblQuestion.Tag = "";
            this.lblQuestion.Text = "What do you want to do?";
            this.lblQuestion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblMessage.Location = new System.Drawing.Point(71, 40);
            this.lblMessage.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(808, 130);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Tag = "";
            this.lblMessage.Text = "A shift is not currently not open on this register.";
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShiftActionForm
            // 
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(955, 715);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "ShiftActionForm";
            this.Text = "Logon";
            this.Controls.SetChildIndex(this.panelControl1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnResumeShift;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnNewShift;
        private System.Windows.Forms.Label lblMessage;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnNonDrawerMode;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUseShift;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblQuestion;
    }
}
