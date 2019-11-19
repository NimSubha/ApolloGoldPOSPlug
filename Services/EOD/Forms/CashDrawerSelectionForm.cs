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
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    sealed partial class CashDrawerSelectionForm : frmTouchBase
    {

        #region Properties

        private IList<IPosBatchStaging> openedShifts;

        /// <summary>
        /// Get FormResult
        /// </summary>
        public string SelectedCashDrawer { get; private set; }

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="CashDrawerSelectionForm"/> class.
        /// </summary>
        private CashDrawerSelectionForm()
            : base()
        {
            // Required for Windows Form Designer support
            InitializeComponent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CashDrawerSelectionForm"/> class.
        /// </summary>
        /// <param name="openedShifts">The opened shifts.</param>
        public CashDrawerSelectionForm(IList<IPosBatchStaging> openedShifts)
            :this()
        {
            this.openedShifts = openedShifts;
        }

        /// <summary>
        /// Overload the <see cref="E:Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();

                int counter = 0;
                foreach (Tuple<string, string> drawerInfo in EOD.InternalApplication.Services.Peripherals.CashDrawer.GetAvailableDrawers())
                {
                    ++counter;

                    // If cash drawer description is empty then use resource string.
                    string buttonText = string.IsNullOrWhiteSpace(drawerInfo.Item2)
                                        ? ApplicationLocalizer.Language.Translate(51318, counter)
                                        : drawerInfo.Item2;
                    string drawerName = drawerInfo.Item1;
                    SimpleButtonEx button = CreateDrawerButton(buttonText, drawerName);

                    if (openedShifts.Any(s => s.CashDrawer.Equals(drawerName, StringComparison.OrdinalIgnoreCase)))
                    {  // If drawer is already being used, then disable the button.
                        button.Enabled = false;
                    }

                    this.flowLayoutPanel.Controls.Add(button);
                }
            }

            base.OnLoad(e);
        }

        /// <summary>
        /// Translates the labels.
        /// </summary>
        private void TranslateLabels()
        {
            this.Text = ApplicationLocalizer.Language.Translate(51311); // Logon
            this.btnCancel.Text = ApplicationLocalizer.Language.Translate(51315); // Cancel
        }

        /// <summary>
        /// Creates the drawer button.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="tag">The tag.</param>
        /// <returns>A new button.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]
        private SimpleButtonEx CreateDrawerButton(string text, string tag)
        {
            SimpleButtonEx button = new SimpleButtonEx();

            button.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, FontStyle.Bold);
            button.Click += new EventHandler(button_Click);
            button.DialogResult = DialogResult.OK;
            button.Size = new Size(270, 50);
            button.Margin = new Padding(10);
            button.Text = text;
            button.Tag = tag;
            button.Name = tag;   // Set for automation purposes

            return button;
        }  

        #endregion

        #region Events

        private void button_Click(object sender, EventArgs e)
        {
            // Cash drawer name is tagged with button.
            SelectedCashDrawer = (string)((Control)sender).Tag;
        }

        #endregion

    }

}
