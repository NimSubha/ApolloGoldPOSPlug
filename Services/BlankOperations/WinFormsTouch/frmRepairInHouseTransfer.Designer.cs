namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class frmRepairInHouseTransfer
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnEnter = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.lblRepairId = new System.Windows.Forms.Label();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtRepairId = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnWHSearch = new DevExpress.XtraEditors.SimpleButton();
            this.txtWarehouse = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblTitle.Location = new System.Drawing.Point(240, 30);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(221, 32);
            this.lblTitle.TabIndex = 15;
            this.lblTitle.Text = "HO Repair Transfer ";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.btnEnter);
            this.panel3.Controls.Add(this.btnClear);
            this.panel3.Controls.Add(this.lblRepairId);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.txtRepairId);
            this.panel3.Location = new System.Drawing.Point(10, 154);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(720, 63);
            this.panel3.TabIndex = 20;
            // 
            // btnEnter
            // 
            this.btnEnter.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.down;
            this.btnEnter.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnEnter.Location = new System.Drawing.Point(278, 12);
            this.btnEnter.Name = "btnEnter";
            this.btnEnter.Size = new System.Drawing.Size(57, 32);
            this.btnEnter.TabIndex = 179;
            this.btnEnter.Text = "Clear";
            this.btnEnter.Click += new System.EventHandler(this.btnEnter_Click);
            // 
            // btnClear
            // 
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(341, 12);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 179;
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblRepairId
            // 
            this.lblRepairId.AutoSize = true;
            this.lblRepairId.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.lblRepairId.Location = new System.Drawing.Point(7, 20);
            this.lblRepairId.Name = "lblRepairId";
            this.lblRepairId.Size = new System.Drawing.Size(51, 13);
            this.lblRepairId.TabIndex = 7;
            this.lblRepairId.Text = "Repair Id";
            // 
            // btnSearch
            // 
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(405, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 32);
            this.btnSearch.TabIndex = 9;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtRepairId
            // 
            this.txtRepairId.BackColor = System.Drawing.SystemColors.Control;
            this.txtRepairId.Enabled = false;
            this.txtRepairId.Location = new System.Drawing.Point(102, 18);
            this.txtRepairId.MaxLength = 60;
            this.txtRepairId.Name = "txtRepairId";
            this.txtRepairId.Size = new System.Drawing.Size(171, 21);
            this.txtRepairId.TabIndex = 8;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnWHSearch);
            this.panel2.Controls.Add(this.txtWarehouse);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(10, 83);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(720, 65);
            this.panel2.TabIndex = 19;
            // 
            // btnWHSearch
            // 
            this.btnWHSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.search;
            this.btnWHSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnWHSearch.Location = new System.Drawing.Point(278, 18);
            this.btnWHSearch.Name = "btnWHSearch";
            this.btnWHSearch.Size = new System.Drawing.Size(57, 32);
            this.btnWHSearch.TabIndex = 6;
            this.btnWHSearch.Text = "Search";
            this.btnWHSearch.Click += new System.EventHandler(this.btnWHSearch_Click);
            // 
            // txtWarehouse
            // 
            this.txtWarehouse.BackColor = System.Drawing.SystemColors.Control;
            this.txtWarehouse.Location = new System.Drawing.Point(102, 23);
            this.txtWarehouse.MaxLength = 20;
            this.txtWarehouse.Name = "txtWarehouse";
            this.txtWarehouse.ReadOnly = true;
            this.txtWarehouse.Size = new System.Drawing.Size(170, 21);
            this.txtWarehouse.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label2.Location = new System.Drawing.Point(6, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "To Warehouse:";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnClose.Location = new System.Drawing.Point(636, 435);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 43);
            this.btnClose.TabIndex = 23;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grItems);
            this.panel1.Location = new System.Drawing.Point(11, 223);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(718, 206);
            this.panel1.TabIndex = 22;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(0, 0);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(716, 204);
            this.grItems.TabIndex = 12;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.AutoCalcPreviewLineCount = true;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.OptionsView.ShowPreview = true;
            this.grdView.PreviewFieldName = "DIMENSIONS";
            this.grdView.PreviewIndent = 2;
            this.grdView.PreviewLineCount = 1;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // btnSubmit
            // 
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSubmit.Location = new System.Drawing.Point(533, 435);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(94, 43);
            this.btnSubmit.TabIndex = 21;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnDelete.Location = new System.Drawing.Point(427, 435);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(94, 43);
            this.btnDelete.TabIndex = 24;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // frmRepairInHouseTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 479);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblTitle);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmRepairInHouseTransfer";
            this.Text = "HO Repair Transfer";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel panel3;
        private DevExpress.XtraEditors.SimpleButton btnEnter;
        private DevExpress.XtraEditors.SimpleButton btnClear;
        private System.Windows.Forms.Label lblRepairId;
        private DevExpress.XtraEditors.SimpleButton btnSearch;
        public System.Windows.Forms.TextBox txtRepairId;
        private System.Windows.Forms.Panel panel2;
        private DevExpress.XtraEditors.SimpleButton btnWHSearch;
        public System.Windows.Forms.TextBox txtWarehouse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnDelete;
    }
}