/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    public partial class frmReportList : frmTouchBase
    {
        protected frmReportList()
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            printWindow.Properties.ReadOnly = true;
            printWindow.BackColor           = Color.White;
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        public frmReportList(string reportLayout) : this()
        {        
            printWindow.Text = reportLayout;                        
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                btnPrint.Text  = LSRetailPosis.ApplicationLocalizer.Language.Translate(51304);
                btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51305);
                this.Text      = LSRetailPosis.ApplicationLocalizer.Language.Translate(51310);
                lblHeader.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51310);
            }

            base.OnLoad(e);
        }
    }
}
