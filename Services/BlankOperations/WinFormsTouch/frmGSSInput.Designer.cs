namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmGSSInput
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
            this.lblGss = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMonths = new System.Windows.Forms.Label();
            this.txtMonths = new System.Windows.Forms.TextBox();
            this.lblGSSNo = new System.Windows.Forms.Label();
            this.btnGSSNo = new System.Windows.Forms.Button();
            this.txtGSSNo = new System.Windows.Forms.TextBox();
            this.btnOK = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblGss
            // 
            this.lblGss.AutoSize = true;
            this.lblGss.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblGss.ForeColor = System.Drawing.Color.Black;
            this.lblGss.Location = new System.Drawing.Point(4, 3);
            this.lblGss.Name = "lblGss";
            this.lblGss.Size = new System.Drawing.Size(562, 65);
            this.lblGss.TabIndex = 0;
            this.lblGss.Text = "Gold Saving Scheme (GSS)";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtAmount);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblMonths);
            this.panel1.Controls.Add(this.txtMonths);
            this.panel1.Controls.Add(this.lblGSSNo);
            this.panel1.Controls.Add(this.btnGSSNo);
            this.panel1.Controls.Add(this.txtGSSNo);
            this.panel1.Location = new System.Drawing.Point(5, 71);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(551, 136);
            this.panel1.TabIndex = 1;
            // 
            // txtAmount
            // 
            this.txtAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtAmount.Location = new System.Drawing.Point(361, 80);
            this.txtAmount.MaxLength = 10;
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(135, 27);
            this.txtAmount.TabIndex = 160;
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAmount_KeyPress);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(284, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 21);
            this.label1.TabIndex = 159;
            this.label1.Text = "Amount";
            // 
            // lblMonths
            // 
            this.lblMonths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMonths.AutoSize = true;
            this.lblMonths.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.lblMonths.ForeColor = System.Drawing.Color.Black;
            this.lblMonths.Location = new System.Drawing.Point(6, 82);
            this.lblMonths.Name = "lblMonths";
            this.lblMonths.Size = new System.Drawing.Size(119, 21);
            this.lblMonths.TabIndex = 158;
            this.lblMonths.Text = "No. of Months";
            // 
            // txtMonths
            // 
            this.txtMonths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMonths.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtMonths.Location = new System.Drawing.Point(129, 80);
            this.txtMonths.MaxLength = 4;
            this.txtMonths.Name = "txtMonths";
            this.txtMonths.Size = new System.Drawing.Size(55, 27);
            this.txtMonths.TabIndex = 157;
            this.txtMonths.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtMonths.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.txtMonths.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // lblGSSNo
            // 
            this.lblGSSNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGSSNo.AutoSize = true;
            this.lblGSSNo.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.lblGSSNo.ForeColor = System.Drawing.Color.Black;
            this.lblGSSNo.Location = new System.Drawing.Point(6, 34);
            this.lblGSSNo.Name = "lblGSSNo";
            this.lblGSSNo.Size = new System.Drawing.Size(106, 21);
            this.lblGSSNo.TabIndex = 156;
            this.lblGSSNo.Text = "GSS Number";
            // 
            // btnGSSNo
            // 
            this.btnGSSNo.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnGSSNo.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnGSSNo.Location = new System.Drawing.Point(502, 29);
            this.btnGSSNo.Name = "btnGSSNo";
            this.btnGSSNo.Size = new System.Drawing.Size(42, 32);
            this.btnGSSNo.TabIndex = 4;
            this.btnGSSNo.UseVisualStyleBackColor = true;
            this.btnGSSNo.Click += new System.EventHandler(this.btnGSSNo_Click);
            // 
            // txtGSSNo
            // 
            this.txtGSSNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtGSSNo.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtGSSNo.Location = new System.Drawing.Point(129, 32);
            this.txtGSSNo.MaxLength = 50;
            this.txtGSSNo.Name = "txtGSSNo";
            this.txtGSSNo.Size = new System.Drawing.Size(367, 27);
            this.txtGSSNo.TabIndex = 2;
            this.txtGSSNo.Leave += new System.EventHandler(this.txtGSSNo_Leave);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Location = new System.Drawing.Point(190, 219);
            this.btnOK.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Padding = new System.Windows.Forms.Padding(0);
            this.btnOK.ShowToolTips = false;
            this.btnOK.Size = new System.Drawing.Size(74, 45);
            this.btnOK.TabIndex = 2;
            this.btnOK.Tag = "btnOK";
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(289, 219);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(0);
            this.btnCancel.ShowToolTips = false;
            this.btnCancel.Size = new System.Drawing.Size(74, 45);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Tag = "btnCancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmGSSInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(562, 273);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblGss);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmGSSInput";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "GOLD SAVING SCHEME (GSS)";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion


        private System.Windows.Forms.Label lblGss;
        private System.Windows.Forms.Panel panel1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOK;
        private System.Windows.Forms.TextBox txtGSSNo;
        private System.Windows.Forms.Button btnGSSNo;
        private System.Windows.Forms.Label lblGSSNo;
        private System.Windows.Forms.TextBox txtAmount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMonths;
        private System.Windows.Forms.TextBox txtMonths;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
    }
}