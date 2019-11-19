/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmGenericLookup.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "frm", Justification = "Grandfather")]
	public class frmGenericLookup : frmTouchBase
    {
        private DevExpress.XtraGrid.Blending.XtraGridBlending xtraGridBlending1;
        private DataRow selectedDataRow;
        private System.Reflection.PropertyInfo returnProperty;
        private PanelControl basePanel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private TableLayoutPanel tableLayoutPanel1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnSelect;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgDown;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnPgUp;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnUp;
		private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnDown;
		private Label labelHeading;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;

        protected frmGenericLookup()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }
        /// <summary>
        /// Locating the default value.
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="displayColumn"></param>
        /// <param name="sizeFactor"></param>
        /// <param name="defaultValue"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]        
        public frmGenericLookup(System.Data.DataTable dataTable, int displayColumn, string defaultValue) 
            : this()
        {            
            int index = 0;
			labelHeading.Text = defaultValue;
            foreach (DataColumn dataCol in dataTable.Columns)
            {
                AddGridColumn(index == displayColumn,
                                (dataCol.DataType.Name.IndexOf("STRING", StringComparison.OrdinalIgnoreCase) != -1),
                                dataCol.Caption,
                                dataCol.ColumnName);
                index += 1;
            }
                        
            grItems.DataSource = dataTable;
            
            SelectRowByColumnValue(0, defaultValue);            
        }        

        /// <summary>
        /// Displays a list of class instances and lets user choose one
        /// </summary>
        /// <param name="dataTable">List of classes with properties</param>
        /// <param name="displayPropertyName">Name of property which should be displayed</param>
        /// <param name="displayColumnCaption">Column title text for displayed property</param>
        /// <param name="returnPropertyName">Name of property whose value is saved in SelectedString property</param>
        /// <param name="sizeFactor"></param>
        /// <param name="defaultValue">Default focused instance when form opens</param>       
        public frmGenericLookup(IList dataTable, string displayPropertyName, string displayColumnCaption, string returnPropertyName, string defaultValue)
            : this()
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException();
            }

			labelHeading.Text = displayColumnCaption;
            var elementType = dataTable.AsQueryable().ElementType;
            var displayProperty = elementType.GetProperty(displayPropertyName) ?? elementType.GetProperties()[0];
            this.returnProperty = elementType.GetProperty(returnPropertyName) ?? elementType.GetProperties()[0];
            
            int index, displayColumnNum, returnColumnNum;
            index = displayColumnNum = returnColumnNum = 0;
            foreach (var property in elementType.GetProperties())
            {
                AddGridColumn(property == displayProperty, 
                                property.PropertyType == typeof(String), 
                                displayColumnCaption, 
                                property.Name);

                displayColumnNum = (property == displayProperty) ? index : displayColumnNum;
                returnColumnNum = (property == returnProperty) ? index : returnColumnNum;
                index += 1;
            }

            grItems.DataSource = dataTable;

            bool defaultRowFound = SelectRowByColumnValue(displayColumnNum, defaultValue);
            if (!defaultRowFound) { SelectRowByColumnValue(returnColumnNum, defaultValue); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "column objects are used by class and will be disposed")]
        private DevExpress.XtraGrid.Columns.GridColumn AddGridColumn(bool isDisplayProperty, bool isString, string columnCaption, string fieldName)
        {
            DevExpress.XtraGrid.Columns.GridColumn column = new DevExpress.XtraGrid.Columns.GridColumn();

            column.AppearanceCell.Options.UseTextOptions = true;
            if (isString)
            {
                column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
                column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.None;
            }
            else
            {
                column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
                column.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            }
            column.Caption = isDisplayProperty ? columnCaption : fieldName;
            column.FieldName = fieldName;
            column.Name = "datacol" + grdView.Columns.Count.ToString();
            column.Width = 130;
            column.Visible = true;
            column.VisibleIndex = isDisplayProperty ? 0 : -1;
            column.SortIndex = (column.VisibleIndex > 0) ? 0 : -1;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] { column });

            return column;
        }

        private bool SelectRowByColumnValue(int columnNumber, string value)
        {
            if (value == null)
                return false; 

            var column = grdView.Columns[columnNumber];
            for (int i = 0; i < grdView.RowCount; i++)
            {
                if (String.Equals(grdView.GetRowCellValue(i, column).ToString(), value, StringComparison.OrdinalIgnoreCase))
                {
                    grdView.FocusedRowHandle = grdView.GetRowHandle(i);

                    const int pageSize = 17;
                    // 17 is page size.
                    // If select item from very first half of the page then do not do anything.
                    // if selected item is on the last page bottom half of the page then put last item at the bottom of the grid to make it full.
                    // Show selected item in the middle of the grid in mid pages.
                    if (grdView.FocusedRowHandle > (pageSize / 2) && 
                        grdView.RowCount > pageSize)
                    {
                        // Is last page bottom half?
                        if ((grdView.RowCount - (pageSize - 1) / 2) < grdView.FocusedRowHandle)
                        {
                            // fill last page.
                            grdView.TopRowIndex = grdView.RowCount - pageSize;
                        }
                        else
                        {
                            // show selected item in the middle.
                            grdView.TopRowIndex = grdView.FocusedRowHandle - pageSize / 2;
                        }
                    }
                    return true;
                }
            }

            return false;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                //
                // Get all text through the Translation function in the ApplicationLocalizer
                //
                // TextID's for frmItemSearch are reserved at 1280 - 1299
                // In use now are ID's 1280 - 1290
                //

                btnSelect.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1283); // Select
                btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1281); // Cancel                    
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
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
            this.xtraGridBlending1 = new DevExpress.XtraGrid.Blending.XtraGridBlending();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelHeading = new System.Windows.Forms.Label();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraGridBlending1
            // 
            this.xtraGridBlending1.AlphaStyles.AddReplace("Row", 220);
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(869, 639);
            this.basePanel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.labelHeading, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnPgUp, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnUp, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnSelect, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnCancel, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnDown, 6, 4);
            this.tableLayoutPanel1.Controls.Add(this.btnPgDown, 7, 4);
            this.tableLayoutPanel1.Controls.Add(this.grItems, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 15);
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(865, 635);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // labelHeading
            // 
            this.labelHeading.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.labelHeading.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.labelHeading, 8);
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.labelHeading.Location = new System.Drawing.Point(333, 40);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Size = new System.Drawing.Size(198, 65);
            this.labelHeading.TabIndex = 7;
            this.labelHeading.Text = "Heading";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(33, 560);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Padding = new System.Windows.Forms.Padding(0);
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 1;
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.OnPageUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(96, 560);
            this.btnUp.Name = "btnUp";
            this.btnUp.Padding = new System.Windows.Forms.Padding(0);
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 2;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.OnUp_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(302, 560);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Padding = new System.Windows.Forms.Padding(0);
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 3;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.OnSelect_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancel.Location = new System.Drawing.Point(435, 560);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(0);
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(711, 560);
            this.btnDown.Name = "btnDown";
            this.btnDown.Padding = new System.Windows.Forms.Padding(0);
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 5;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.OnDown_Click);
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(774, 560);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Padding = new System.Windows.Forms.Padding(0);
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 6;
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.OnPageDown_Click);
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel1.SetColumnSpan(this.grItems, 8);
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(33, 138);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(799, 401);
            this.grItems.TabIndex = 0;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            this.grItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.AllowIncrementalSearch = true;
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsMenu.EnableColumnMenu = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 40;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.DoubleClick += new System.EventHandler(this.grdView_DoubleClick);
            // 
            // frmGenericLookup
            // 
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(869, 639);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmGenericLookup";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion
        /// <summary>
        /// Returns selected datarow as datarow.
        /// </summary>
        public DataRow SelectedDataRow
        {
            get { return selectedDataRow; }
        }

        /// <summary>
        /// Returns string representation of value in field specified in 'returnPropertyName'
        ///   property for selected instance (row) in grid
        /// </summary>
        public String SelectedString { get; private set; }

        private void SelectItem()
        {
            if (grdView.RowCount > 0)
            {
                int selectedIdx = grdView.GetSelectedRows()[0];
                if (this.returnProperty != null)
                {
                    object value = this.returnProperty.GetValue(grdView.GetRow(selectedIdx), null);
                    SelectedString = (value != null) ? value.ToString() : string.Empty;
                }
                
                selectedDataRow = grdView.GetDataRow(selectedIdx);
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                Close();
            }
        }

        private void OnPageUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();
        }

        private void OnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrev();
        }

        private void OnPageDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
        }

        private void OnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNext();
        }

        private void OnSelect_Click(object sender, EventArgs e)
        {
            SelectItem();
        }            

        private void grdView_DoubleClick(object sender, EventArgs e)
        {
            SelectItem();
        }

        private void grItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SelectItem();
        }

    }
}
