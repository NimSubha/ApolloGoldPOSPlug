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
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.POSProcesses.WinControls;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.DevUtilities;

namespace Microsoft.Dynamics.Retail.Pos.Card
{
    /// <summary>
    /// Prompt to select card type when an ambiguous card number format is detected.
    /// </summary>
    public partial class frmSelectCardType : LSRetailPosis.POSControls.Touch.frmTouchBase
    {
        private bool allowCancel = true;

        private frmSelectCardType()
        {
            InitializeComponent();
        }

        public frmSelectCardType(IEnumerable<ICardInfo> cardList, bool allowCancel)
            : this()
        {
            this.Cards = cardList;
            this.allowCancel = allowCancel;
        }

        /// <summary>
        /// List of cards to display to the user
        /// </summary>
        private IEnumerable<ICardInfo> Cards { get; set; }

        /// <summary>
        /// Card chosen by the user
        /// </summary>
        public ICardInfo SelectedCard { get; private set; }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "b objects are used by class and will be disposed")]
        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                if (!allowCancel)
                {   // Cancel is not allowed - card must be selected
                    btnCancel.Visible = false;
                }

                if (Cards != null)
                {
                    Size defaultSize = new Size(152, 52);
                    panCards.AutoSize = true;

                    // Add buttons for each card-type
                    foreach (ICardInfo c in Cards)
                    {
                        SimpleButtonEx b = new SimpleButtonEx()
                        {
                            Tag = c,
                            Text = string.IsNullOrWhiteSpace(c.CardName) ? c.CardTypeId : c.CardName,
                            Anchor = AnchorStyles.None,
                            Size = defaultSize
                        };

                        b.Click -= new EventHandler(OnCardTypeClick);
                        b.Click += new EventHandler(OnCardTypeClick);
                        this.panCards.Controls.Add(b);
                    }
                }

                TranslateLabels();
            }

            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmPayCard are reserved at 5180 - 5199
            // In use now are ID's: 5180 - 
            //

            this.btnCancel.Text = ApplicationLocalizer.Language.Translate(5183);    //Cancel
            this.Text = this.lblTitle.Text = ApplicationLocalizer.Language.Translate(5190);    //Please select a card type
        }

        void OnCardTypeClick(object sender, EventArgs e)
        {
            SimpleButtonEx b = sender as SimpleButtonEx;

            if (b != null)
            {
                this.SelectedCard = b.Tag as ICardInfo;
                this.DialogResult = DialogResult.OK;
            }
        }

        private void panCards_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
