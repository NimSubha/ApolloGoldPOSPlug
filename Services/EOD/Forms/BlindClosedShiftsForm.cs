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
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    sealed partial class BlindClosedShiftsForm : frmTouchBase
    {

        #region Fields

        private List<PosBatchStaging> shiftsData;

        #endregion

        #region Construction

        /// <summary>
        /// Ctor
        /// </summary>
        public BlindClosedShiftsForm(IList<IPosBatchStaging> shifts)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            this.shiftsData = shifts.Cast<PosBatchStaging>().ToList();
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

                // Associate operations with buttons.
                this.btnDeclareStartAmount.Tag = PosisOperations.DeclareStartAmount;
                this.btnTenderDeclaration.Tag = PosisOperations.TenderDeclaration;
                this.btnPrintX.Tag = PosisOperations.PrintX;
                this.btnCloseShift.Tag = PosisOperations.CloseShift;

                this.grdShifts.DataSource = shiftsData;

                TranslateLabels();
                UpdateControlButtons();
            }

            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            this.Text = ApplicationLocalizer.Language.Translate(51330); // Blind closed shifts
            this.lblHeading.Text = ApplicationLocalizer.Language.Translate(51330); //Blind closed shifts
            this.colRegister.Caption = ApplicationLocalizer.Language.Translate(51331); // Regiter number
            this.colShift.Caption = ApplicationLocalizer.Language.Translate(51332); // Shift number
            this.colOpenedDateTime.Caption = ApplicationLocalizer.Language.Translate(51333);
            this.colBlindClosedDateTime.Caption = ApplicationLocalizer.Language.Translate(51334);
            this.colStaff.Caption = ApplicationLocalizer.Language.Translate(51335);
            this.btnDeclareStartAmount.Text = ApplicationLocalizer.Language.Translate(51336);
            this.btnTenderDeclaration.Text = ApplicationLocalizer.Language.Translate(51337);
            this.btnPrintX.Text = ApplicationLocalizer.Language.Translate(51338);
            this.btnCloseShift.Text = ApplicationLocalizer.Language.Translate(51339);
            this.btnClose.Text = ApplicationLocalizer.Language.Translate(51340);
        }

        #endregion

        #region Methods

        private void UpdateControlButtons()
        {
            // Arrow buttons
            btnPgDown.Enabled = btnDown.Enabled = !grdView.IsLastRow;
            btnUp.Enabled = btnPgUp.Enabled = !grdView.IsFirstRow;

            // Operation buttons
            bool anySelectedRow = grdView.SelectedRowsCount > 0;
            btnDeclareStartAmount.Enabled = btnTenderDeclaration.Enabled = btnPrintX.Enabled = btnCloseShift.Enabled = anySelectedRow;

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

        private void PerformOperation(object sender, EventArgs e)
        {
            PosBatchStaging selectedShift = shiftsData[grdView.GetSelectedRows()[0]];
            PosisOperations operation = (PosisOperations)((Control)sender).Tag;

            EOD.InternalApplication.RunOperation(operation, selectedShift);

            // If operation changed the batch status (E.g. closed) then delete from list.
            if (selectedShift.Status != PosBatchStatus.BlindClosed)
            {
                shiftsData.Remove(selectedShift);
                grdView.RefreshData();
            }

            UpdateControlButtons();
        }

        private void grdView_KeyUp(object sender, KeyEventArgs e)
        {
            UpdateControlButtons();
        }

        private void grdView_Click(object sender, EventArgs e)
        {
            UpdateControlButtons();
        }

        #endregion

    }

}
