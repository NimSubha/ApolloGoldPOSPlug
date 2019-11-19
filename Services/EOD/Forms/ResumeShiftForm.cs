/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    sealed partial class ResumeShiftForm : frmTouchBase
    {

        #region Properties

        /// <summary>
        /// Get selected shift.
        /// </summary>
        public IPosBatchStaging SelectedShift { get; private set; }

        private IList<IPosBatchStaging> shiftsData;

        private ShiftSelectionMode shiftSelectionMode;

        #endregion

        #region Construction

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="selectionMode">The selection mode.</param>
        /// <param name="shifts">The collection of shifts.</param>
        public ResumeShiftForm(ShiftSelectionMode selectionMode, IList<IPosBatchStaging> shifts)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            this.shiftsData = shifts;
            this.shiftSelectionMode = selectionMode;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                //Set the size of the form the same as the main form
                this.Bounds = new Rectangle(
                    ApplicationSettings.MainWindowLeft,
                    ApplicationSettings.MainWindowTop,
                    ApplicationSettings.MainWindowWidth,
                    ApplicationSettings.MainWindowHeight);

                this.grdShifts.DataSource = shiftsData.Cast<PosBatchStaging>().ToList();

                SetupAndTranslateLabels();
                UpdateControlButtons();
            }

            base.OnLoad(e);
        }

        private void SetupAndTranslateLabels()
        {
            switch (this.shiftSelectionMode)
            {
                case ShiftSelectionMode.Resume:
                    this.Text = ApplicationLocalizer.Language.Translate(51320); // Resume shift
                    this.colSuspendDateTime.Caption = ApplicationLocalizer.Language.Translate(51321); // Suspended
                    this.colStartDateTime.Visible = false;
                    break;

                case ShiftSelectionMode.UseOpened:
                    this.Text = ApplicationLocalizer.Language.Translate(51326); // Use shift
                    this.colStartDateTime.Caption = ApplicationLocalizer.Language.Translate(51327); // Started
                    this.colSuspendDateTime.Visible = false;
                    break;

                default:
                    throw new NotSupportedException("Unsupported shift selection mode");
            }

            this.colRegister.Caption = ApplicationLocalizer.Language.Translate(51322); // Register number
            this.colShift.Caption = ApplicationLocalizer.Language.Translate(51323); // Shift number
            this.colStaff.Caption = ApplicationLocalizer.Language.Translate(51324); // Staff
            this.btnResume.Text = ApplicationLocalizer.Language.Translate(51325); // Select
            this.btnCancel.Text = ApplicationLocalizer.Language.Translate(51315); // Cancel
        }

        #endregion

        #region Methods

        private void SelectShift()
        {
            if (grdView.SelectedRowsCount > 0)
            {
                SelectedShift = shiftsData[grdView.GetSelectedRows()[0]];
                DialogResult = DialogResult.OK;
            }
            else
            {
                SelectedShift = null;
                DialogResult = DialogResult.Cancel;
            }

            Close();
        }

        private void UpdateControlButtons()
        {
            btnPgDown.Enabled = btnDown.Enabled = !grdView.IsLastRow;
            btnUp.Enabled = btnPgUp.Enabled = !grdView.IsFirstRow;
            this.grdShifts.Focus();
        }

        #endregion

        #region Events

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();
            UpdateControlButtons();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrev();
            UpdateControlButtons();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNext();
            UpdateControlButtons();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
            UpdateControlButtons();
        }

        private void btnResume_Click(object sender, EventArgs e)
        {
            SelectShift();
        }

        private void grdView_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SelectShift();
            }
            else
            {
                UpdateControlButtons();
            }
        }

        private void grdView_Click(object sender, EventArgs e)
        {
            UpdateControlButtons();
        }

        private void grdView_DoubleClick(object sender, EventArgs e)
        {
            Point p = grdView.GridControl.PointToClient(MousePosition);
            GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest != GridHitTest.Column)
            {
                SelectShift();
            }
        }

        #endregion

    }

    internal enum ShiftSelectionMode
    {
        None = 0,
        Resume = 1,
        UseOpened = 2
    }

}
