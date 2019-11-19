/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.Card
{
	partial class frmSelectCardType
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
            this.panTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panCards = new System.Windows.Forms.FlowLayoutPanel();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panTableLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.panelControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panTableLayout
            // 
            this.panTableLayout.ColumnCount = 1;
            this.panTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panTableLayout.Controls.Add(this.btnCancel, 0, 2);
            this.panTableLayout.Controls.Add(this.lblTitle, 0, 0);
            this.panTableLayout.Controls.Add(this.panCards, 0, 1);
            this.panTableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panTableLayout.Location = new System.Drawing.Point(2, 2);
            this.panTableLayout.Margin = new System.Windows.Forms.Padding(0);
            this.panTableLayout.Name = "panTableLayout";
            this.panTableLayout.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.panTableLayout.RowCount = 3;
            this.panTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panTableLayout.Size = new System.Drawing.Size(1020, 764);
            this.panTableLayout.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(446, 692);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblTitle.Location = new System.Drawing.Point(251, 40);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(517, 65);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Please select a card type";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panCards
            // 
            this.panCards.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panCards.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.panCards.Location = new System.Drawing.Point(410, 361);
            this.panCards.Name = "panCards";
            this.panCards.Size = new System.Drawing.Size(200, 100);
            this.panCards.TabIndex = 1;
            this.panCards.Paint += new System.Windows.Forms.PaintEventHandler(this.panCards_Paint);
            // 
            // panelControl1
            // 
            this.panelControl1.Controls.Add(this.panTableLayout);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(0, 0);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Size = new System.Drawing.Size(1024, 768);
            this.panelControl1.TabIndex = 1;
            // 
            // frmSelectCardType
            // 
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmSelectCardType";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panTableLayout.ResumeLayout(false);
            this.panTableLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.panelControl1.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.TableLayoutPanel panTableLayout;
        private System.Windows.Forms.Label lblTitle;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.FlowLayoutPanel panCards;
	}
}
