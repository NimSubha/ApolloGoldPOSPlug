namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class CustomerOrderDetails
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvCustomerOrder = new System.Windows.Forms.DataGridView();
            this.colLineNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConfigID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colColorID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSizeID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStyleID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPieces = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQuantity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMakingRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIngredientsAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colExtendedDetails = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerOrder)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvCustomerOrder
            // 
            this.dgvCustomerOrder.AllowUserToAddRows = false;
            this.dgvCustomerOrder.AllowUserToDeleteRows = false;
            this.dgvCustomerOrder.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Navy;
            this.dgvCustomerOrder.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvCustomerOrder.BackgroundColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomerOrder.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgvCustomerOrder.ColumnHeadersHeight = 40;
            this.dgvCustomerOrder.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colLineNum,
            this.colItemId,
            this.colConfigID,
            this.colColorID,
            this.colSizeID,
            this.colStyleID,
            this.colPieces,
            this.colQuantity,
            this.colRate,
            this.colMakingRate,
            this.colIngredientsAmount,
            this.colExtendedDetails});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.ButtonFace;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.DodgerBlue;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvCustomerOrder.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgvCustomerOrder.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvCustomerOrder.GridColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.dgvCustomerOrder.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.dgvCustomerOrder.Location = new System.Drawing.Point(5, 4);
            this.dgvCustomerOrder.MultiSelect = false;
            this.dgvCustomerOrder.Name = "dgvCustomerOrder";
            this.dgvCustomerOrder.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.MenuBar;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomerOrder.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dgvCustomerOrder.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.Navy;
            this.dgvCustomerOrder.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dgvCustomerOrder.RowTemplate.DividerHeight = 2;
            this.dgvCustomerOrder.RowTemplate.Height = 40;
            this.dgvCustomerOrder.RowTemplate.ReadOnly = true;
            this.dgvCustomerOrder.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvCustomerOrder.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCustomerOrder.Size = new System.Drawing.Size(1053, 440);
            this.dgvCustomerOrder.TabIndex = 1;
            this.dgvCustomerOrder.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomerOrder_CellClick);
            this.dgvCustomerOrder.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvCustomerOrder_CellDoubleClick);
            // 
            // colLineNum
            // 
            this.colLineNum.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colLineNum.DataPropertyName = "LINENUM";
            this.colLineNum.HeaderText = "No.";
            this.colLineNum.Name = "colLineNum";
            this.colLineNum.ReadOnly = true;
            this.colLineNum.Width = 55;
            // 
            // colItemId
            // 
            this.colItemId.DataPropertyName = "ITEMID";
            this.colItemId.HeaderText = "Item Id";
            this.colItemId.Name = "colItemId";
            this.colItemId.ReadOnly = true;
            // 
            // colConfigID
            // 
            this.colConfigID.DataPropertyName = "CONFIGID";
            this.colConfigID.HeaderText = "Configuration";
            this.colConfigID.Name = "colConfigID";
            this.colConfigID.ReadOnly = true;
            this.colConfigID.Width = 120;
            // 
            // colColorID
            // 
            this.colColorID.DataPropertyName = "CODE";
            this.colColorID.HeaderText = "Color";
            this.colColorID.Name = "colColorID";
            this.colColorID.ReadOnly = true;
            this.colColorID.Width = 70;
            // 
            // colSizeID
            // 
            this.colSizeID.DataPropertyName = "SIZEID";
            this.colSizeID.HeaderText = "Size";
            this.colSizeID.Name = "colSizeID";
            this.colSizeID.ReadOnly = true;
            this.colSizeID.Width = 50;
            // 
            // colStyleID
            // 
            this.colStyleID.DataPropertyName = "STYLE";
            this.colStyleID.HeaderText = "Style";
            this.colStyleID.Name = "colStyleID";
            this.colStyleID.ReadOnly = true;
            this.colStyleID.Width = 60;
            // 
            // colPieces
            // 
            this.colPieces.DataPropertyName = "PCS";
            this.colPieces.HeaderText = "Pieces";
            this.colPieces.Name = "colPieces";
            this.colPieces.ReadOnly = true;
            this.colPieces.Width = 70;
            // 
            // colQuantity
            // 
            this.colQuantity.DataPropertyName = "QTY";
            this.colQuantity.HeaderText = "Quantity";
            this.colQuantity.Name = "colQuantity";
            this.colQuantity.ReadOnly = true;
            this.colQuantity.Width = 90;
            // 
            // colRate
            // 
            this.colRate.DataPropertyName = "RATE";
            this.colRate.HeaderText = "Rate";
            this.colRate.Name = "colRate";
            this.colRate.ReadOnly = true;
            // 
            // colMakingRate
            // 
            this.colMakingRate.DataPropertyName = "MAKINGRATE";
            this.colMakingRate.HeaderText = "Making Rate";
            this.colMakingRate.Name = "colMakingRate";
            this.colMakingRate.ReadOnly = true;
            // 
            // colIngredientsAmount
            // 
            this.colIngredientsAmount.DataPropertyName = "EXTENDEDDETAILSAMOUNT";
            this.colIngredientsAmount.HeaderText = "Ingredients Amount";
            this.colIngredientsAmount.Name = "colIngredientsAmount";
            this.colIngredientsAmount.ReadOnly = true;
            // 
            // colExtendedDetails
            // 
            this.colExtendedDetails.HeaderText = "Click for Details";
            this.colExtendedDetails.Name = "colExtendedDetails";
            this.colExtendedDetails.ReadOnly = true;
            // 
            // CustomerOrderDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.ClientSize = new System.Drawing.Size(1063, 449);
            this.Controls.Add(this.dgvCustomerOrder);
            this.ForeColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CustomerOrderDetails";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Customer Order Details";
            this.Load += new System.EventHandler(this.CustomerOrderDetails_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCustomerOrder)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvCustomerOrder;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLineNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConfigID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColorID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSizeID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStyleID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPieces;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQuantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMakingRate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIngredientsAmount;
        private System.Windows.Forms.DataGridViewButtonColumn colExtendedDetails;
    }
}