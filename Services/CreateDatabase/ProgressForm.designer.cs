//Microsoft Dynamics AX for Retail POS Plug-ins
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.Windows.Forms;
namespace Microsoft.Dynamics.Retail.Pos.CreateDatabaseService
{
	partial class ProgressForm
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
            this.lboxStatusText = new ListBox();
            this.SuspendLayout();
            // 
            // lboxStatusText
            // 
            this.lboxStatusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lboxStatusText.Location = new System.Drawing.Point(0, 0);
            this.lboxStatusText.Name = "lboxStatusText";
            this.lboxStatusText.Size = new System.Drawing.Size(292, 266);
            this.lboxStatusText.TabIndex = 0;
            // 
            // ProgressForm
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.ControlBox = false;
            this.Controls.Add(this.lboxStatusText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Database creation process";
            this.ResumeLayout(false);

		}

		#endregion

		private ListBox lboxStatusText;
	}
}