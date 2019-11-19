/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    public partial class ViewAddressUserControl
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
            this.components = new System.ComponentModel.Container();
            this.lblFormattedAddress = new System.Windows.Forms.Label();
            this.viewAddressBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelAddress = new System.Windows.Forms.Label();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.viewAddressBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFormattedAddress
            // 
            this.lblFormattedAddress.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.viewAddressBindingSource, "FormattedAddress", true));
            this.lblFormattedAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFormattedAddress.Location = new System.Drawing.Point(3, 22);
            this.lblFormattedAddress.Margin = new System.Windows.Forms.Padding(3);
            this.lblFormattedAddress.MaximumSize = new System.Drawing.Size(228, 0);
            this.lblFormattedAddress.Name = "lblFormattedAddress";
            this.lblFormattedAddress.Size = new System.Drawing.Size(228, 123);
            this.lblFormattedAddress.TabIndex = 0;
            this.lblFormattedAddress.Text = "DUMMY TEXT";
            // 
            // viewAddressBindingSource
            // 
            this.viewAddressBindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.Customer.ViewModels.AddressViewModel);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.lblFormattedAddress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelAddress, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnEdit, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.MinimumSize = new System.Drawing.Size(200, 100);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(385, 148);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // labelAddress
            // 
            this.labelAddress.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelAddress.AutoSize = true;
            this.labelAddress.Location = new System.Drawing.Point(0, 0);
            this.labelAddress.Margin = new System.Windows.Forms.Padding(0);
            this.labelAddress.Name = "labelAddress";
            this.labelAddress.Size = new System.Drawing.Size(67, 19);
            this.labelAddress.TabIndex = 2;
            this.labelAddress.Text = "ADDRESS";
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.Edit;
            this.btnEdit.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnEdit.Location = new System.Drawing.Point(237, 22);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Padding = new System.Windows.Forms.Padding(3);
            this.btnEdit.Size = new System.Drawing.Size(57, 32);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // ViewAddressUserControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(285, 148);
            this.Name = "ViewAddressUserControl";
            this.Size = new System.Drawing.Size(385, 148);
            ((System.ComponentModel.ISupportInitialize)(this.viewAddressBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFormattedAddress;
        private System.Windows.Forms.BindingSource viewAddressBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelAddress;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnEdit;
    }
}
