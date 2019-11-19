/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraGrid;
using LSRetailPosis.POSProcesses;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    /// <summary>
    /// Displays the denominations of a specific currency for counting purposes.
    /// </summary>
    internal partial class frmDenominations : frmTouchBase
    {
        public List<DenominationViewModel> GridSource
        {
            get;
            private set;
        }
        private int qtyPressedRowHandle = GridControl.InvalidRowHandle;
        private int qtyHighlightedRowHandle = GridControl.InvalidRowHandle;
        private int totalPressedRowHandle = GridControl.InvalidRowHandle;
        private int totalHighlightedRowHandle = GridControl.InvalidRowHandle;
        private const int BUTTONMARGIN = 3;
        private const string COLTOTAL = "Total";
        private const string COLQTY = "Quantity";
        private readonly string currencyCode;

        protected frmDenominations()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates the form and initializes it with a specific currency
        /// </summary>
        /// <param name="currencyCode">ISO 3-letter currency code</param>
        public frmDenominations(string currencyCode, List<DenominationViewModel> source)
            : this()
        {
            this.currencyCode = currencyCode;
            if (source != null)
                GridSource = source;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();

                LoadData();
            }

            this.gvDenom.Appearance.HeaderPanel.ForeColor = this.ForeColor;
            this.gvDenom.Appearance.Row.ForeColor = this.ForeColor;

            base.OnLoad(e);
        }

        /// <summary>
        /// Current total of all denominations
        /// </summary>
        public decimal Total
        {
            get;
            private set;
        }

        private void TranslateLabels()
        {
            this.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1964);                            // Denominations
            labelEnterDenom.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1960, this.currencyCode);         // Enter {0} denominations
            colDenom.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(1961);                            // Denomination
            colQty.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(1962);                            // Quantity
            colTotal.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(1963);                            // Total
            btnOK.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1261);                            // OK
            btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1260);                            // Cancel
        }

        private void LoadData()
        {
            if (GridSource == null)
            {
                // Load curreny info and create a datasource for the grid
                ICurrencyInfo currInfo = Dialog.InternalApplication.Services.Currency.DetailedCurrencyInfo(this.currencyCode);
                this.GridSource = new List<DenominationViewModel>(currInfo.CurrencyItems.Length);

                foreach (ICurrencyItemInfo currItem in currInfo.CurrencyItems)
                {
                    DenominationViewModel vm = new DenominationViewModel()
                    {
                        Denomination = currItem.CurrValue,
                        DenominationText = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(currItem.CurrValue, false, false)
                    };

                    this.GridSource.Add(vm);
                }
            }

            this.gridDenom.DataSource = this.GridSource;
            UpdateTotal();
        }

        private void OnQtyButtonClicked(int rowHandle)
        {
            using (frmInputNumpad inputDialog = new frmInputNumpad())
            {
                inputDialog.EntryTypes = NumpadEntryTypes.IntegerPositive;
                inputDialog.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(1944); // "Number of counted units"
                inputDialog.CurrencyCode = this.currencyCode;

                int qty;
                POSFormsManager.ShowPOSForm(inputDialog);

                if (inputDialog.DialogResult != DialogResult.Cancel)
                {
                    if (int.TryParse(inputDialog.InputText, out qty))
                    {
                        // Update the row source and repaint
                        this.GridSource[rowHandle].Quantity = qty;
                        this.gvDenom.RefreshRow(rowHandle);

                        UpdateTotal();
                    }
                }
            }
        }

        private void OnTotalButtonClicked(int rowHandle)
        {
            using (frmInputNumpad inputDialog = new frmInputNumpad())
            {
                inputDialog.EntryTypes = NumpadEntryTypes.Price;
                inputDialog.PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(1443); // "Enter amount"
                inputDialog.CurrencyCode = this.currencyCode;

                decimal total;
                POSFormsManager.ShowPOSForm(inputDialog);
                if (inputDialog.DialogResult != DialogResult.Cancel)
                {
                    if (decimal.TryParse(inputDialog.InputText, out total))
                    {
                        // Update the row source and repaint
                        this.GridSource[rowHandle].Total = total;
                        this.gvDenom.RefreshRow(rowHandle);

                        UpdateTotal();
                    }
                    else
                    {
                        using (frmMessage dialog = new frmMessage(LSRetailPosis.ApplicationLocalizer.Language.Translate(1949),
                            MessageBoxButtons.OK, MessageBoxIcon.Warning))
                        {
                            POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                }
            }
        }

        private void UpdateTotal()
        {
            this.Total = GridSource.Sum(denom => denom.Total);

            // Refresh the total area in the grid
            gvDenom.UpdateTotalSummary();
        }

        #region Custom grid button code

        private void gvDenom_CustomDrawCell(object sender, DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventArgs e)
        {
            string column = e.Column.FieldName;

            if (column == COLQTY || column == COLTOTAL)
            {
                e.Appearance.FillRectangle(e.Cache, e.Bounds);
                DrawButton(e.Cache, e.Bounds, gridDenom.LookAndFeel.ActiveLookAndFeel.ActiveStyle, e.Appearance, GetButtonState(e.RowHandle, column), e.DisplayText);

                e.Handled = true;
            }
        }

        private void DrawButton(GraphicsCache cache, Rectangle bounds, ActiveLookAndFeelStyle lookAndFeel, AppearanceObject appearance, ObjectState state, string caption)
        {
            EditorButtonObjectInfoArgs args = new EditorButtonObjectInfoArgs(cache, Button, appearance);
            BaseLookAndFeelPainters painters = LookAndFeelPainterHelper.GetPainter(lookAndFeel);

            // Create some margin
            bounds.Inflate(-BUTTONMARGIN, -BUTTONMARGIN);

            args.Bounds = bounds;
            args.State = state;

            painters.Button.DrawObject(args);

            // Draw the text
            using (Brush brush = gvDenom.Appearance.HeaderPanel.GetForeBrush(cache))
            {
                StringFormat sf = appearance.GetStringFormat(appearance.GetTextOptions());
                painters.Button.DrawCaption(args, caption, appearance.Font, brush, args.Bounds, sf);
            }

        }

        private ObjectState GetButtonState(int rowHandle, string column)
        {
            int pressedRowHandle = GridControl.InvalidRowHandle;
            int highlightedRowHandle = GridControl.InvalidRowHandle;

            switch (column)
            {
                case COLQTY:
                    pressedRowHandle = QtyPressedRowHandle;
                    highlightedRowHandle = QtyHighlightedRowHandle;
                    break;
                case COLTOTAL:
                    pressedRowHandle = TotalPressedRowHandle;
                    highlightedRowHandle = TotalHighlightedRowHandle;
                    break;
                default:
                    break;
            }

            if (rowHandle == pressedRowHandle)
            {
                return ObjectState.Pressed;
            }
            else
            {
                // Show hot if row is highlighted or cell is focused
                if (rowHandle == highlightedRowHandle
                    || (gridDenom.IsFocused && (gvDenom.FocusedColumn.FieldName == column)
                    && (gvDenom.FocusedRowHandle == rowHandle)))
                {
                    return ObjectState.Hot;
                }
                else
                {
                    return ObjectState.Normal;
                }
            }
        }

        private EditorButton button;

        private EditorButton Button
        {
            get
            {
                if (button == null)
                {
                    button = new EditorButton(ButtonPredefines.Glyph);
                }

                return button;
            }
        }

        /// <summary>
        /// Tracks the currently pressed row handle for the quantity column
        /// </summary>
        private int QtyPressedRowHandle
        {
            get { return qtyPressedRowHandle; }
            set
            {
                if (qtyPressedRowHandle != value)
                {
                    SetPressedRowHandle(ref qtyPressedRowHandle, COLQTY, value);

                    gvDenom.InvalidateRowCell(qtyPressedRowHandle, gvDenom.Columns[COLQTY]);
                }
            }
        }

        /// <summary>
        /// Tracks the currently highlighted row handle for the quantity column
        /// </summary>
        private int QtyHighlightedRowHandle
        {
            get { return qtyHighlightedRowHandle; }
            set
            {
                if (qtyHighlightedRowHandle != value)
                {
                    SetHighlightedRowHandle(ref qtyHighlightedRowHandle, COLQTY, value);

                    gvDenom.InvalidateRowCell(qtyHighlightedRowHandle, gvDenom.Columns[COLQTY]);
                }
            }
        }

        /// <summary>
        /// Tracks the currently pressed row handle for the total column
        /// </summary>
        private int TotalPressedRowHandle
        {
            get { return totalPressedRowHandle; }
            set
            {
                if (totalPressedRowHandle == value)
                {
                    return;
                }

                SetPressedRowHandle(ref totalPressedRowHandle, COLTOTAL, value);

                gvDenom.InvalidateRowCell(totalPressedRowHandle, gvDenom.Columns[COLTOTAL]);
            }
        }

        /// <summary>
        /// Tracks the currently highlighted row handle for the total column
        /// </summary>
        private int TotalHighlightedRowHandle
        {
            get { return totalHighlightedRowHandle; }
            set
            {
                if (totalHighlightedRowHandle == value)
                {
                    return;
                }

                SetHighlightedRowHandle(ref totalHighlightedRowHandle, COLTOTAL, value);

                gvDenom.InvalidateRowCell(totalHighlightedRowHandle, gvDenom.Columns[COLTOTAL]);
            }
        }

        private void SetPressedRowHandle(ref int rowHandle, string column, int value)
        {
            if (rowHandle != GridControl.InvalidRowHandle)
            {
                int tempRowHandle = rowHandle;
                rowHandle = GridControl.InvalidRowHandle;
                gvDenom.InvalidateRowCell(tempRowHandle, gvDenom.Columns[column]);
            }

            rowHandle = value;
        }

        private void SetHighlightedRowHandle(ref int rowHandle, string column, int value)
        {
            if (rowHandle == value)
            {
                return;
            }

            if (rowHandle != GridControl.InvalidRowHandle)
            {
                int tempRowHandle = rowHandle;
                rowHandle = GridControl.InvalidRowHandle;
                gvDenom.InvalidateRowCell(tempRowHandle, gvDenom.Columns[column]);
            }
            else
            {
                rowHandle = value;
                QtyPressedRowHandle = GridControl.InvalidRowHandle;
            }
        }

        /// <summary>
        /// Mouse down event handler for grid view
        /// </summary>        
        private void gvDenom_MouseDown(object sender, MouseEventArgs e)
        {
            if (QtyHighlightedRowHandle != GridControl.InvalidRowHandle)
            {
                QtyPressedRowHandle = QtyHighlightedRowHandle;
            }

            if (TotalHighlightedRowHandle != GridControl.InvalidRowHandle)
            {
                TotalPressedRowHandle = TotalHighlightedRowHandle;
            }
        }

        /// <summary>
        /// Mouse up event handler for grid view
        /// </summary>        
        private void gvDenom_MouseUp(object sender, MouseEventArgs e)
        {
            if (QtyPressedRowHandle != GridControl.InvalidRowHandle)
            {
                OnQtyButtonClicked(QtyPressedRowHandle);
                QtyPressedRowHandle = GridControl.InvalidRowHandle;
            }

            if (TotalPressedRowHandle != GridControl.InvalidRowHandle)
            {
                OnTotalButtonClicked(TotalPressedRowHandle);
                TotalPressedRowHandle = GridControl.InvalidRowHandle;
            }
        }

        /// <summary>
        /// Mouse move event handler for grid view
        /// </summary>
        private void gvDenom_MouseMove(object sender, MouseEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView view = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (view != null)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridHitInfo info = view.CalcHitInfo(e.X, e.Y);
                if (info.InRowCell && (info.Column.FieldName == COLQTY) && IsMouseOverButton(info.RowHandle, COLQTY, new Point(e.X, e.Y)))
                {
                    QtyHighlightedRowHandle = info.RowHandle;
                }
                else
                {
                    QtyHighlightedRowHandle = GridControl.InvalidRowHandle;
                }

                if (info.InRowCell && (info.Column.FieldName == COLTOTAL) && IsMouseOverButton(info.RowHandle, COLTOTAL, new Point(e.X, e.Y)))
                {
                    TotalHighlightedRowHandle = info.RowHandle;
                }
                else
                {
                    TotalHighlightedRowHandle = GridControl.InvalidRowHandle;
                }
            }
        }

        private bool IsMouseOverButton(int rowHandle, string column, Point point)
        {
            bool result = false;

            DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo viewInfo = gvDenom.GetViewInfo() as DevExpress.XtraGrid.Views.Grid.ViewInfo.GridViewInfo;
            if (viewInfo != null)
            {
                DevExpress.XtraGrid.Views.Grid.ViewInfo.GridCellInfo cellInfo = viewInfo.GetGridCellInfo(rowHandle, gvDenom.Columns[column]);
                result = cellInfo.Bounds.Contains(point);
            }

            return result;
        }

        /// <summary>
        /// Handles the display text for custom grid columns
        /// </summary>        
        private void gvDenom_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            switch (e.Column.FieldName)
            {
                case COLQTY:
                    e.DisplayText = GridSource[e.ListSourceRowIndex].Quantity.ToString();
                    break;
                case COLTOTAL:
                    // Get amount value
                    decimal amount = GridSource[e.ListSourceRowIndex].Total;
                    e.DisplayText = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(amount, this.currencyCode, false, false);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Calculates the display value for the custom total area
        /// </summary>        
        private void gvDenom_CustomSummaryCalculate(object sender, DevExpress.Data.CustomSummaryEventArgs e)
        {
            switch (e.SummaryProcess)
            {
                case DevExpress.Data.CustomSummaryProcess.Start:
                    e.TotalValueReady = true;
                    break;
                case DevExpress.Data.CustomSummaryProcess.Finalize:
                    e.TotalValue = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(this.Total, this.currencyCode, true, false);
                    break;
                default:
                    break;
            }
        }

        private void btnPageUp_Click(object sender, EventArgs e)
        {
            gvDenom.MovePrevPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gvDenom.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gvDenom.MoveNext();
        }

        private void btnPageDown_Click(object sender, EventArgs e)
        {
            gvDenom.MoveNextPage();
        }

        /// <summary>
        /// Key up handler for grid view
        /// </summary>
        /// <remarks>Invokes the  clicked handler for a column button when the space bar is pressed</remarks>
        private void gvDenom_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                switch (gvDenom.FocusedColumn.FieldName)
                {
                    case COLQTY:
                        OnQtyButtonClicked(gvDenom.FocusedRowHandle);
                        break;
                    case COLTOTAL:
                        OnTotalButtonClicked(gvDenom.FocusedRowHandle);
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// Model of denomination row for grid to bind to
    /// </summary>
    sealed internal class DenominationViewModel
    {
        private int quantity;
        private decimal total;

        public decimal Denomination
        {
            get;
            set;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "get_DenominationText is used by grid for binding")]
        public string DenominationText
        {
            get;
            set;
        }

        public int Quantity
        {
            get { return this.quantity; }
            set
            {
                if (this.quantity != value)
                {
                    this.quantity = value;
                    this.Total = value * this.Denomination;
                }
            }
        }

        public decimal Total
        {
            get { return this.total; }
            set
            {
                if (this.total != value)
                {
                    if (this.Denomination <= 0m)
                    {
                        throw new ArgumentOutOfRangeException("Denomination", "Denomination cannot be zero or negative");
                    }

                    // Validate that the total divides evenly
                    if (value % this.Denomination == 0)
                    {
                        this.total = value;

                        // Calculate quantity
                        this.quantity = (int)(value / this.Denomination);
                    }
                }
            }
        }
    }
}
