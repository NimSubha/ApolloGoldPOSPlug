using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using Microsoft.Dynamics.Retail.Diagnostics;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class SelectSearchTypePopup : UserControl
    {
        public SelectSearchTypePopup()
        {
            InitializeComponent();
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
            btnLocalSearch.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(99996); // "Search this store"
            btnRemoteSearch.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(99997); // "Search everywhere"
        }

        private void OnButton_Click(object sender, EventArgs e)
        {
            SimpleButton btn = sender as SimpleButton;
            if (btn != null)
            {
                this.SelectedIndex = btn.TabIndex;
            }

            ToolStripDropDown parent = this.Parent as ToolStripDropDown;
            if (parent != null)
                parent.Close(ToolStripDropDownCloseReason.ItemClicked);
        }

        /// <summary>
        /// Index of selected button
        /// </summary>
        public int SelectedIndex
        {
            get;
            private set;
        }
    }
}
