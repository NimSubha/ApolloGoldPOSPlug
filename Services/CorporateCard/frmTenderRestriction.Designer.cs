/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.CorporateCard
{
	partial class frmTenderRestriction
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
            this.lblContinue = new System.Windows.Forms.Label();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.memMessage = new DevExpress.XtraEditors.MemoEdit();
            this.lstExcluded = new DevExpress.XtraEditors.ListBoxControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnYes = new DevExpress.XtraEditors.SimpleButton();
            this.btnNo = new DevExpress.XtraEditors.SimpleButton();
            this.lblExcluded = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memMessage.Properties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstExcluded)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblContinue
            // 
            this.lblContinue.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblContinue.AutoSize = true;
            this.lblContinue.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblContinue.Location = new System.Drawing.Point(269, 306);
            this.lblContinue.Name = "lblContinue";
            this.lblContinue.Size = new System.Drawing.Size(228, 25);
            this.lblContinue.TabIndex = 5;
            this.lblContinue.Tag = "";
            this.lblContinue.Text = "Do you want to continue?";
            this.lblContinue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panelControl1
            // 
            this.panelControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panelControl1.Controls.Add(this.tableLayoutPanel1);
            this.panelControl1.Location = new System.Drawing.Point(2, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(867, 670);
            this.panelControl1.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblExcluded, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(863, 666);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.lblContinue, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.memMessage, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lstExcluded, 0, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(48, 245);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(767, 331);
            this.tableLayoutPanel3.TabIndex = 8;
            // 
            // memMessage
            // 
            this.memMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memMessage.Location = new System.Drawing.Point(3, 231);
            this.memMessage.Name = "memMessage";
            this.memMessage.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.memMessage.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memMessage.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.memMessage.Properties.Appearance.Options.UseBackColor = true;
            this.memMessage.Properties.Appearance.Options.UseFont = true;
            this.memMessage.Properties.Appearance.Options.UseForeColor = true;
            this.memMessage.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.memMessage.Properties.ReadOnly = true;
            this.memMessage.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.memMessage.Size = new System.Drawing.Size(761, 72);
            this.memMessage.TabIndex = 4;
            this.memMessage.Tag = "";
            // 
            // lstExcluded
            // 
            this.lstExcluded.Appearance.BackColor = System.Drawing.Color.White;
            this.lstExcluded.Appearance.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lstExcluded.Appearance.ForeColor = System.Drawing.Color.Black;
            this.lstExcluded.Appearance.Options.UseBackColor = true;
            this.lstExcluded.Appearance.Options.UseFont = true;
            this.lstExcluded.Appearance.Options.UseForeColor = true;
            this.lstExcluded.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.lstExcluded.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.lstExcluded.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstExcluded.ItemHeight = 40;
            this.lstExcluded.Location = new System.Drawing.Point(3, 3);
            this.lstExcluded.Name = "lstExcluded";
            this.lstExcluded.Size = new System.Drawing.Size(761, 222);
            this.lstExcluded.TabIndex = 3;
            this.lstExcluded.Tag = "";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 4;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btnYes, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnNo, 2, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(30, 590);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(803, 65);
            this.tableLayoutPanel2.TabIndex = 8;
            // 
            // btnYes
            // 
            this.btnYes.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnYes.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnYes.Appearance.Options.UseFont = true;
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(270, 4);
            this.btnYes.Margin = new System.Windows.Forms.Padding(4);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(127, 57);
            this.btnYes.TabIndex = 1;
            this.btnYes.Tag = "";
            this.btnYes.Text = "Yes";
            // 
            // btnNo
            // 
            this.btnNo.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnNo.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNo.Appearance.Options.UseFont = true;
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.Location = new System.Drawing.Point(405, 4);
            this.btnNo.Margin = new System.Windows.Forms.Padding(4);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(127, 57);
            this.btnNo.TabIndex = 2;
            this.btnNo.Tag = "";
            this.btnNo.Text = "No";
            // 
            // lblExcluded
            // 
            this.lblExcluded.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblExcluded.AutoSize = true;
            this.lblExcluded.Font = new System.Drawing.Font("Segoe UI Light", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExcluded.Location = new System.Drawing.Point(131, 40);
            this.lblExcluded.Margin = new System.Windows.Forms.Padding(0);
            this.lblExcluded.Name = "lblExcluded";
            this.lblExcluded.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblExcluded.Size = new System.Drawing.Size(600, 202);
            this.lblExcluded.TabIndex = 6;
            this.lblExcluded.Text = "Items excluded from payment:";
            // 
            // frmTenderRestriction
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(870, 670);
            this.Controls.Add(this.panelControl1);
            this.HelpButton = true;
            this.Name = "frmTenderRestriction";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.memMessage.Properties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lstExcluded)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblContinue;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.MemoEdit memMessage;
		private DevExpress.XtraEditors.ListBoxControl lstExcluded;
		private System.Windows.Forms.Label lblExcluded;
		private DevExpress.XtraEditors.SimpleButton btnNo;
		private DevExpress.XtraEditors.SimpleButton btnYes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
	}
}
