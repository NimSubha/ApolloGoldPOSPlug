/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using LSRetailPosis.POSProcesses;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
	public partial class frmPrintSelection : frmTouchBase
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Customer BalanceReport are reserved at 51060 - 51079
        // Used textid's 51060 - 51066

        private string customerId;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        public frmPrintSelection(string customerId)
        {
            // Required for Windows Form Designer support
            InitializeComponent();

            this.customerId = customerId;
        }

        private void frmPrintSelection_Load(object sender, EventArgs e)
        {
            this.Text = this.lblHeader.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51060);
            this.btnThisMonth.Text          = LSRetailPosis.ApplicationLocalizer.Language.Translate(51061);
            this.btnLastThreeMonths.Text    = LSRetailPosis.ApplicationLocalizer.Language.Translate(51062);
            this.btnLastSixMonths.Text      = LSRetailPosis.ApplicationLocalizer.Language.Translate(51063);
            this.btnLastYear.Text           = LSRetailPosis.ApplicationLocalizer.Language.Translate(51064);
            this.btnAllTransactions.Text    = LSRetailPosis.ApplicationLocalizer.Language.Translate(51065);
            this.btnCancel.Text             = LSRetailPosis.ApplicationLocalizer.Language.Translate(51066);
        }

        private void btnLastMonth_Click(object sender, EventArgs e)
        {
            PrintReport(1);
        }

        private void btnLastTwoMonths_Click(object sender, EventArgs e)
        {
            PrintReport(3);
        }

        private void btnLastSixMonths_Click(object sender, EventArgs e)
        {
            PrintReport(6);
        }

        private void btnLastYear_Click(object sender, EventArgs e)
        {
            PrintReport(12);
        }

        private void btnAllTransactions_Click(object sender, EventArgs e)
        {
            PrintReport(-1);
        }

        private void PrintReport(int numberOfMonths)
        {
            TransactionReport tranactionReport = new TransactionReport();
            tranactionReport.Print(customerId, numberOfMonths);
            Close();
        }
    }
}
