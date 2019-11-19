/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using LSRetailPosis;
using LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    sealed partial class ShiftActionForm : frmTouchBase
    {

        #region Properties

        /// <summary>
        /// Get FormResult
        /// </summary>
        public ShiftActionResult FormResult { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="message"></param>
        /// <param name="newButton"></param>
        /// <param name="resumeButton"></param>
        /// <param name="useButton"></param>
        public ShiftActionForm(string message, bool newButton, bool resumeButton, bool useButton)
            :base()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            lblMessage.Text = message;
            btnNewShift.Visible = newButton;
            btnResumeShift.Visible = resumeButton;
            btnUseShift.Visible = useButton;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();
            }

            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            this.Text = ApplicationLocalizer.Language.Translate(51311); // Logon
            this.lblQuestion.Text = ApplicationLocalizer.Language.Translate(51312); // What do you want to do?
            this.btnNewShift.Text = ApplicationLocalizer.Language.Translate(51313); // Open a new shift
            this.btnResumeShift.Text = ApplicationLocalizer.Language.Translate(51314); //Resume an existing shift
            this.btnUseShift.Text = ApplicationLocalizer.Language.Translate(51317); // Use this shift
            this.btnNonDrawerMode.Text = ApplicationLocalizer.Language.Translate(51316); //Perform a non-drawer operation
            this.btnCancel.Text = ApplicationLocalizer.Language.Translate(51315); // Cancel
        }

        #endregion

        #region Events

        private void btnNewShift_Click(object sender, EventArgs e)
        {
            FormResult = ShiftActionResult.New;
        }

        private void btnResumeShift_Click(object sender, EventArgs e)
        {
            FormResult = ShiftActionResult.Resume;
        }

        private void btnNonDrawerMode_Click(object sender, EventArgs e)
        {
            FormResult = ShiftActionResult.NonDrawer;
        }

        private void btnUseShift_Click(object sender, EventArgs e)
        {
            FormResult = ShiftActionResult.UseOpened;
        }

        #endregion

    }

    #region Enums

    internal enum ShiftActionResult
    {
        None = 0,
        New = 1,
        Resume = 2,
        NonDrawer = 3,
        UseOpened = 4,
    }

    #endregion

}
