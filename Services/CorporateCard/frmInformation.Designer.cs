/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

namespace Microsoft.Dynamics.Retail.Pos.CorporateCard
{
	partial class frmInformation
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
			this.panBackground = new DevExpress.XtraEditors.PanelControl();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.lblDriverId = new System.Windows.Forms.Label();
			this.txtDriverId = new DevExpress.XtraEditors.TextEdit();
			this.txtOdometer = new DevExpress.XtraEditors.TextEdit();
			this.lblVehicleId = new System.Windows.Forms.Label();
			this.lblOdometer = new System.Windows.Forms.Label();
			this.txtVehicleId = new DevExpress.XtraEditors.TextEdit();
			this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lblHeading = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.panBackground)).BeginInit();
			this.panBackground.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDriverId.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtOdometer.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtVehicleId.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
			this.SuspendLayout();
			// 
			// panBackground
			// 
			this.panBackground.Controls.Add(this.tableLayoutPanel2);
			this.panBackground.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panBackground.Location = new System.Drawing.Point(0, 0);
			this.panBackground.Margin = new System.Windows.Forms.Padding(0);
			this.panBackground.Name = "panBackground";
			this.panBackground.Size = new System.Drawing.Size(972, 711);
			this.panBackground.TabIndex = 0;
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 2;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel1, 0, 1);
			this.tableLayoutPanel2.Controls.Add(this.btnOK, 0, 2);
			this.tableLayoutPanel2.Controls.Add(this.btnCancel, 1, 2);
			this.tableLayoutPanel2.Controls.Add(this.lblHeading, 0, 0);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
			this.tableLayoutPanel2.RowCount = 3;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel2.Size = new System.Drawing.Size(968, 707);
			this.tableLayoutPanel2.TabIndex = 1;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel2.SetColumnSpan(this.tableLayoutPanel1, 2);
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.lblDriverId, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.txtDriverId, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.txtOdometer, 1, 4);
			this.tableLayoutPanel1.Controls.Add(this.lblVehicleId, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.lblOdometer, 0, 4);
			this.tableLayoutPanel1.Controls.Add(this.txtVehicleId, 1, 2);
			this.tableLayoutPanel1.Controls.Add(this.txtKeyboardInput, 1, 6);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(219, 241);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 7;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(530, 305);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// lblDriverId
			// 
			this.lblDriverId.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblDriverId.AutoSize = true;
			this.lblDriverId.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.lblDriverId.Location = new System.Drawing.Point(3, 6);
			this.lblDriverId.Name = "lblDriverId";
			this.lblDriverId.Size = new System.Drawing.Size(70, 21);
			this.lblDriverId.TabIndex = 1;
			this.lblDriverId.Tag = "H3";
			this.lblDriverId.Text = "Driver Id";
			this.lblDriverId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtDriverId
			// 
			this.txtDriverId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtDriverId.Location = new System.Drawing.Point(90, 3);
			this.txtDriverId.Name = "txtDriverId";
			this.txtDriverId.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.txtDriverId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtDriverId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.txtDriverId.Properties.Appearance.Options.UseBackColor = true;
			this.txtDriverId.Properties.Appearance.Options.UseFont = true;
			this.txtDriverId.Properties.Appearance.Options.UseForeColor = true;
			this.txtDriverId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.txtDriverId.Size = new System.Drawing.Size(437, 28);
			this.txtDriverId.TabIndex = 0;
			// 
			// txtOdometer
			// 
			this.txtOdometer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtOdometer.Location = new System.Drawing.Point(90, 71);
			this.txtOdometer.Name = "txtOdometer";
			this.txtOdometer.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.txtOdometer.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtOdometer.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.txtOdometer.Properties.Appearance.Options.UseBackColor = true;
			this.txtOdometer.Properties.Appearance.Options.UseFont = true;
			this.txtOdometer.Properties.Appearance.Options.UseForeColor = true;
			this.txtOdometer.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.txtOdometer.Size = new System.Drawing.Size(437, 28);
			this.txtOdometer.TabIndex = 5;
			// 
			// lblVehicleId
			// 
			this.lblVehicleId.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblVehicleId.AutoSize = true;
			this.lblVehicleId.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.lblVehicleId.Location = new System.Drawing.Point(3, 40);
			this.lblVehicleId.Name = "lblVehicleId";
			this.lblVehicleId.Size = new System.Drawing.Size(77, 21);
			this.lblVehicleId.TabIndex = 2;
			this.lblVehicleId.Text = "Vehicle Id";
			this.lblVehicleId.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblOdometer
			// 
			this.lblOdometer.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.lblOdometer.AutoSize = true;
			this.lblOdometer.Font = new System.Drawing.Font("Segoe UI", 12F);
			this.lblOdometer.Location = new System.Drawing.Point(3, 74);
			this.lblOdometer.Name = "lblOdometer";
			this.lblOdometer.Size = new System.Drawing.Size(81, 21);
			this.lblOdometer.TabIndex = 4;
			this.lblOdometer.Text = "Odometer";
			this.lblOdometer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtVehicleId
			// 
			this.txtVehicleId.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.txtVehicleId.Location = new System.Drawing.Point(90, 37);
			this.txtVehicleId.Name = "txtVehicleId";
			this.txtVehicleId.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.txtVehicleId.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtVehicleId.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.txtVehicleId.Properties.Appearance.Options.UseBackColor = true;
			this.txtVehicleId.Properties.Appearance.Options.UseFont = true;
			this.txtVehicleId.Properties.Appearance.Options.UseForeColor = true;
			this.txtVehicleId.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.txtVehicleId.Size = new System.Drawing.Size(437, 28);
			this.txtVehicleId.TabIndex = 3;
			// 
			// txtKeyboardInput
			// 
			this.txtKeyboardInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtKeyboardInput.Location = new System.Drawing.Point(90, 138);
			this.txtKeyboardInput.Name = "txtKeyboardInput";
			this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
			this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
			this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
			this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
			this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
			this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.txtKeyboardInput.Size = new System.Drawing.Size(437, 28);
			this.txtKeyboardInput.TabIndex = 6;
			this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyboardInput_KeyDown);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.btnOK.Appearance.Options.UseFont = true;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(353, 635);
			this.btnOK.Margin = new System.Windows.Forms.Padding(4);
			this.btnOK.Name = "btnOK";
			this.btnOK.Padding = new System.Windows.Forms.Padding(3);
			this.btnOK.Size = new System.Drawing.Size(127, 57);
			this.btnOK.TabIndex = 7;
			this.btnOK.Text = "OK";
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
			this.btnCancel.Appearance.Options.UseFont = true;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(488, 635);
			this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Padding = new System.Windows.Forms.Padding(3);
			this.btnCancel.Size = new System.Drawing.Size(127, 57);
			this.btnCancel.TabIndex = 6;
			this.btnCancel.Text = "Cancel";
			// 
			// lblHeading
			// 
			this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.lblHeading.AutoSize = true;
			this.tableLayoutPanel2.SetColumnSpan(this.lblHeading, 2);
			this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 48F);
			this.lblHeading.Location = new System.Drawing.Point(210, 40);
			this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
			this.lblHeading.Name = "lblHeading";
			this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
			this.lblHeading.Size = new System.Drawing.Size(547, 116);
			this.lblHeading.TabIndex = 8;
			this.lblHeading.Text = "Pay corporate card";
			this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// frmInformation
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(972, 711);
			this.Controls.Add(this.panBackground);
			this.HelpButton = true;
			this.Name = "frmInformation";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			((System.ComponentModel.ISupportInitialize)(this.panBackground)).EndInit();
			this.panBackground.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel2.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.txtDriverId.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtOdometer.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtVehicleId.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
			this.ResumeLayout(false);
		}

		#endregion

		private DevExpress.XtraEditors.PanelControl panBackground;
		private System.Windows.Forms.Label lblVehicleId;
		private System.Windows.Forms.Label lblOdometer;
		private System.Windows.Forms.Label lblDriverId;
		private DevExpress.XtraEditors.TextEdit txtOdometer;
		private DevExpress.XtraEditors.TextEdit txtVehicleId;
		private DevExpress.XtraEditors.TextEdit txtDriverId;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private DevExpress.XtraEditors.SimpleButton btnOK;
        private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label lblHeading;
	}
}
