namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class SelectSearchTypePopup
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnRemoteSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnLocalSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.btnRemoteSearch, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.btnLocalSearch, 0, 0);
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.Size = new System.Drawing.Size(186, 96);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // btnRemoteSearch
            // 
            this.btnRemoteSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnRemoteSearch.Appearance.Options.UseFont = true;
            this.btnRemoteSearch.Location = new System.Drawing.Point(4, 52);
            this.btnRemoteSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemoteSearch.Name = "btnRemoteSearch";
            this.btnRemoteSearch.Size = new System.Drawing.Size(178, 40);
            this.btnRemoteSearch.TabIndex = 1;
            this.btnRemoteSearch.Text = "Remote Search";
            this.btnRemoteSearch.Click += new System.EventHandler(this.OnButton_Click);
            // 
            // btnLocalSearch
            // 
            this.btnLocalSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.btnLocalSearch.Appearance.Options.UseFont = true;
            this.btnLocalSearch.Location = new System.Drawing.Point(4, 4);
            this.btnLocalSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnLocalSearch.Name = "btnLocalSearch";
            this.btnLocalSearch.Size = new System.Drawing.Size(178, 40);
            this.btnLocalSearch.TabIndex = 0;
            this.btnLocalSearch.Text = "Local Search";
            this.btnLocalSearch.Click += new System.EventHandler(this.OnButton_Click);
            // 
            // SelectSearchTypePopup
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "SelectSearchTypePopup";
            this.Size = new System.Drawing.Size(186, 96);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnLocalSearch;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnRemoteSearch;
    }
}
