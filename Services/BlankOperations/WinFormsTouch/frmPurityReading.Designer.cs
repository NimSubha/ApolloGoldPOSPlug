namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmPurityReading
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
            this.btnSubmit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblPCS = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtR1 = new System.Windows.Forms.TextBox();
            this.txtR2 = new System.Windows.Forms.TextBox();
            this.txtR3 = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnFetch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.SuspendLayout();
            // 
            // btnSubmit
            // 
            this.btnSubmit.Location = new System.Drawing.Point(211, 9);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(61, 30);
            this.btnSubmit.TabIndex = 4;
            this.btnSubmit.Text = "SUBMIT";
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(211, 43);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblPCS
            // 
            this.lblPCS.AutoSize = true;
            this.lblPCS.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPCS.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblPCS.Location = new System.Drawing.Point(22, 9);
            this.lblPCS.Name = "lblPCS";
            this.lblPCS.Size = new System.Drawing.Size(62, 15);
            this.lblPCS.TabIndex = 203;
            this.lblPCS.Text = "Reading 1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(22, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 15);
            this.label1.TabIndex = 205;
            this.label1.Text = "Reading 2";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(22, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 15);
            this.label2.TabIndex = 207;
            this.label2.Text = "Reading 3";
            // 
            // txtR1
            // 
            this.txtR1.BackColor = System.Drawing.Color.White;
            this.txtR1.Location = new System.Drawing.Point(96, 9);
            this.txtR1.MaxLength = 6;
            this.txtR1.Name = "txtR1";
            this.txtR1.Size = new System.Drawing.Size(87, 20);
            this.txtR1.TabIndex = 0;
            this.txtR1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtR1.WordWrap = false;
            this.txtR1.TextChanged += new System.EventHandler(this.txtR1_TextChanged);
            this.txtR1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtR1_KeyPress);
            // 
            // txtR2
            // 
            this.txtR2.BackColor = System.Drawing.Color.White;
            this.txtR2.Location = new System.Drawing.Point(96, 33);
            this.txtR2.MaxLength = 6;
            this.txtR2.Name = "txtR2";
            this.txtR2.Size = new System.Drawing.Size(87, 20);
            this.txtR2.TabIndex = 1;
            this.txtR2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtR2.WordWrap = false;
            this.txtR2.TextChanged += new System.EventHandler(this.txtR2_TextChanged);
            this.txtR2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtR2_KeyPress);
            // 
            // txtR3
            // 
            this.txtR3.BackColor = System.Drawing.Color.White;
            this.txtR3.Location = new System.Drawing.Point(96, 57);
            this.txtR3.MaxLength = 6;
            this.txtR3.Name = "txtR3";
            this.txtR3.Size = new System.Drawing.Size(87, 20);
            this.txtR3.TabIndex = 2;
            this.txtR3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtR3.WordWrap = false;
            this.txtR3.TextChanged += new System.EventHandler(this.txtR3_TextChanged);
            this.txtR3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtR3_KeyPress);
            // 
            // txtName
            // 
            this.txtName.BackColor = System.Drawing.Color.White;
            this.txtName.Enabled = false;
            this.txtName.Location = new System.Drawing.Point(96, 107);
            this.txtName.MaxLength = 20;
            this.txtName.Name = "txtName";
            this.txtName.ReadOnly = true;
            this.txtName.Size = new System.Drawing.Size(87, 20);
            this.txtName.TabIndex = 210;
            this.txtName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtName.WordWrap = false;
            // 
            // txtCode
            // 
            this.txtCode.BackColor = System.Drawing.Color.White;
            this.txtCode.Enabled = false;
            this.txtCode.Location = new System.Drawing.Point(96, 83);
            this.txtCode.MaxLength = 20;
            this.txtCode.Name = "txtCode";
            this.txtCode.ReadOnly = true;
            this.txtCode.Size = new System.Drawing.Size(87, 20);
            this.txtCode.TabIndex = 100;
            this.txtCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCode.WordWrap = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label4.Location = new System.Drawing.Point(22, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 15);
            this.label4.TabIndex = 211;
            this.label4.Text = "Person";
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(210, 84);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(63, 30);
            this.btnFetch.TabIndex = 3;
            this.btnFetch.Text = "Fetch";
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // frmPurityReading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 137);
            this.Controls.Add(this.btnFetch);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtR3);
            this.Controls.Add(this.txtR2);
            this.Controls.Add(this.txtR1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblPCS);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnCancel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPurityReading";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Purity Reading";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSubmit;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.Label lblPCS;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtR1;
        public System.Windows.Forms.TextBox txtR2;
        public System.Windows.Forms.TextBox txtR3;
        public System.Windows.Forms.TextBox txtName;
        public System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Label label4;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnFetch;
    }
}