/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    partial class frmDenominations
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
            this.gridDenom = new DevExpress.XtraGrid.GridControl();
            this.gvDenom = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colDenom = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colQty = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelEnterDenom = new System.Windows.Forms.Label();
            this.btnPageUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnOK = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPageDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDenom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDenom)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridDenom
            // 
            this.gridDenom.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel.SetColumnSpan(this.gridDenom, 8);
            this.gridDenom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridDenom.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.gridDenom.Location = new System.Drawing.Point(30, 139);
            this.gridDenom.MainView = this.gvDenom;
            this.gridDenom.Margin = new System.Windows.Forms.Padding(4);
            this.gridDenom.MinimumSize = new System.Drawing.Size(958, 500);
            this.gridDenom.Name = "gridDenom";
            this.gridDenom.Size = new System.Drawing.Size(958, 517);
            this.gridDenom.TabIndex = 1;
            this.gridDenom.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gvDenom});
            // 
            // gvDenom
            // 
            this.gvDenom.Appearance.FooterPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gvDenom.Appearance.FooterPanel.Options.UseFont = true;
            this.gvDenom.Appearance.FooterPanel.Options.UseTextOptions = true;
            this.gvDenom.Appearance.FooterPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.gvDenom.Appearance.FooterPanel.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.gvDenom.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.gvDenom.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.gvDenom.Appearance.HeaderPanel.Options.UseFont = true;
            this.gvDenom.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gvDenom.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.gvDenom.Appearance.Row.Options.UseFont = true;
            this.gvDenom.ColumnPanelRowHeight = 50;
            this.gvDenom.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colDenom,
            this.colQty,
            this.colTotal});
            this.gvDenom.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gvDenom.FooterPanelHeight = 60;
            this.gvDenom.GridControl = this.gridDenom;
            this.gvDenom.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gvDenom.Name = "gvDenom";
            this.gvDenom.OptionsBehavior.Editable = false;
            this.gvDenom.OptionsCustomization.AllowColumnMoving = false;
            this.gvDenom.OptionsCustomization.AllowColumnResizing = false;
            this.gvDenom.OptionsCustomization.AllowFilter = false;
            this.gvDenom.OptionsCustomization.AllowGroup = false;
            this.gvDenom.OptionsCustomization.AllowQuickHideColumns = false;
            this.gvDenom.OptionsCustomization.AllowSort = false;
            this.gvDenom.OptionsMenu.EnableColumnMenu = false;
            this.gvDenom.OptionsMenu.EnableFooterMenu = false;
            this.gvDenom.OptionsMenu.EnableGroupPanelMenu = false;
            this.gvDenom.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gvDenom.OptionsSelection.EnableAppearanceHideSelection = false;
            this.gvDenom.OptionsView.ShowFooter = true;
            this.gvDenom.OptionsView.ShowGroupPanel = false;
            this.gvDenom.OptionsView.ShowIndicator = false;
            this.gvDenom.RowHeight = 50;
            this.gvDenom.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.gvDenom.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gvDenom.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gvDenom_CustomDrawCell);
            this.gvDenom.CustomSummaryCalculate += new DevExpress.Data.CustomSummaryEventHandler(this.gvDenom_CustomSummaryCalculate);
            this.gvDenom.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvDenom_CustomColumnDisplayText);
            this.gvDenom.KeyUp += new System.Windows.Forms.KeyEventHandler(this.gvDenom_KeyUp);
            this.gvDenom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gvDenom_MouseDown);
            this.gvDenom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gvDenom_MouseUp);
            this.gvDenom.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gvDenom_MouseMove);
            // 
            // colDenom
            // 
            this.colDenom.AppearanceCell.Options.UseTextOptions = true;
            this.colDenom.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colDenom.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colDenom.AppearanceHeader.Options.UseTextOptions = true;
            this.colDenom.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colDenom.Caption = "Denomination";
            this.colDenom.FieldName = "DenominationText";
            this.colDenom.MaxWidth = 600;
            this.colDenom.Name = "colDenom";
            this.colDenom.OptionsColumn.AllowEdit = false;
            this.colDenom.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colDenom.OptionsColumn.ShowInCustomizationForm = false;
            this.colDenom.Visible = true;
            this.colDenom.VisibleIndex = 0;
            this.colDenom.Width = 583;
            // 
            // colQty
            // 
            this.colQty.AppearanceCell.Options.UseTextOptions = true;
            this.colQty.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colQty.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.colQty.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colQty.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            this.colQty.AppearanceHeader.Options.UseTextOptions = true;
            this.colQty.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colQty.Caption = "Quantity";
            this.colQty.FieldName = "Quantity";
            this.colQty.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colQty.MaxWidth = 200;
            this.colQty.MinWidth = 200;
            this.colQty.Name = "colQty";
            this.colQty.OptionsColumn.AllowEdit = false;
            this.colQty.OptionsColumn.AllowMove = false;
            this.colQty.OptionsColumn.AllowShowHide = false;
            this.colQty.OptionsColumn.AllowSize = false;
            this.colQty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colQty.OptionsFilter.AllowAutoFilter = false;
            this.colQty.OptionsFilter.AllowFilter = false;
            this.colQty.Visible = true;
            this.colQty.VisibleIndex = 1;
            this.colQty.Width = 200;
            // 
            // colTotal
            // 
            this.colTotal.AppearanceCell.Options.UseTextOptions = true;
            this.colTotal.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTotal.AppearanceCell.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
            this.colTotal.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
            this.colTotal.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
            this.colTotal.AppearanceHeader.Options.UseTextOptions = true;
            this.colTotal.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            this.colTotal.Caption = "Total";
            this.colTotal.FieldName = "Total";
            this.colTotal.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colTotal.MaxWidth = 200;
            this.colTotal.MinWidth = 200;
            this.colTotal.Name = "colTotal";
            this.colTotal.OptionsColumn.AllowEdit = false;
            this.colTotal.OptionsColumn.AllowMove = false;
            this.colTotal.OptionsColumn.AllowShowHide = false;
            this.colTotal.OptionsColumn.AllowSize = false;
            this.colTotal.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colTotal.OptionsFilter.AllowAutoFilter = false;
            this.colTotal.OptionsFilter.AllowFilter = false;
            this.colTotal.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[] {
            new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Custom)});
            this.colTotal.Visible = true;
            this.colTotal.VisibleIndex = 2;
            this.colTotal.Width = 200;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 8;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.btnUp, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.gridDenom, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.labelEnterDenom, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.btnPageUp, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.btnOK, 3, 2);
            this.tableLayoutPanel.Controls.Add(this.btnCancel, 4, 2);
            this.tableLayoutPanel.Controls.Add(this.btnPageDown, 7, 2);
            this.tableLayoutPanel.Controls.Add(this.btnDown, 6, 2);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1018, 743);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(95, 671);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 3;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // labelEnterDenom
            // 
            this.labelEnterDenom.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelEnterDenom.AutoSize = true;
            this.tableLayoutPanel.SetColumnSpan(this.labelEnterDenom, 8);
            this.labelEnterDenom.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.labelEnterDenom.Location = new System.Drawing.Point(237, 40);
            this.labelEnterDenom.Margin = new System.Windows.Forms.Padding(0);
            this.labelEnterDenom.Name = "labelEnterDenom";
            this.labelEnterDenom.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.labelEnterDenom.Size = new System.Drawing.Size(544, 95);
            this.labelEnterDenom.TabIndex = 0;
            this.labelEnterDenom.Text = "Enter USD denominations";
            this.labelEnterDenom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnPageUp
            // 
            this.btnPageUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPageUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPageUp.Appearance.Options.UseFont = true;
            this.btnPageUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPageUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPageUp.Location = new System.Drawing.Point(30, 671);
            this.btnPageUp.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnPageUp.Name = "btnPageUp";
            this.btnPageUp.Size = new System.Drawing.Size(57, 57);
            this.btnPageUp.TabIndex = 2;
            this.btnPageUp.Text = "Ç";
            this.btnPageUp.Click += new System.EventHandler(this.btnPageUp_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(378, 671);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(127, 57);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "OK";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(513, 671);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            // 
            // btnPageDown
            // 
            this.btnPageDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPageDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPageDown.Appearance.Options.UseFont = true;
            this.btnPageDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPageDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPageDown.Location = new System.Drawing.Point(931, 671);
            this.btnPageDown.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnPageDown.Name = "btnPageDown";
            this.btnPageDown.Size = new System.Drawing.Size(57, 57);
            this.btnPageDown.TabIndex = 7;
            this.btnPageDown.Text = "Ê";
            this.btnPageDown.Click += new System.EventHandler(this.btnPageDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(866, 671);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4, 11, 4, 4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // frmDenominations
            // 
            this.AcceptButton = this.btnOK;
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(1018, 743);
            this.Controls.Add(this.tableLayoutPanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmDenominations";
            this.Text = "Denominations";
            this.Controls.SetChildIndex(this.tableLayoutPanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridDenom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gvDenom)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl gridDenom;
        private DevExpress.XtraGrid.Views.Grid.GridView gvDenom;
        private DevExpress.XtraGrid.Columns.GridColumn colDenom;
        private DevExpress.XtraGrid.Columns.GridColumn colQty;
        private DevExpress.XtraGrid.Columns.GridColumn colTotal;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.Label labelEnterDenom;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOK;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPageUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPageDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
    }
}