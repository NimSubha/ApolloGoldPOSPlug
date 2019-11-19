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
using LSRetailPosis;

namespace Microsoft.Dynamics.Retail.Pos.SalesInvoice.WinFormsTouch
{
    public partial class frmGetSalesInvoice : LSRetailPosis.POSProcesses.frmTouchBase
    {

        #region Member variables

        private DataTable salesInvoices;
        private string selectedSalesInvoiceId;

        #endregion

        #region Properties
        /// <summary>
        /// Returns SalesInvoice as datatable.
        /// </summary>
        public DataTable SalesInvoices
        {
            set { salesInvoices = value; }
        }
        /// <summary>
        /// Returns SelectedSalesInvoiceId as string.
        /// </summary>
        public string SelectedSalesInvoiceId
        {
            get { return selectedSalesInvoiceId; }
        }

        #endregion

        /// <summary>
        /// Translates all the transaction function in the applicationlocalizer.
        /// </summary>
        public frmGetSalesInvoice()
        {
            InitializeComponent();
        }

        private void TranslateLabels()
        {
            //
            // Get all text through the Translation function in the ApplicationLocalizer
            //
            // TextID's for frmGetSalesInvoice are reserved at 57100 - 57199
            // The last id in use is: 57110
            //

            // Translate everything...
            this.Text                     = ApplicationLocalizer.Language.Translate(2401);      // Sales invoices
            this.lblHeading.Text          = ApplicationLocalizer.Language.Translate(2401);      // Sales invoices
            btnSelect.Text                = ApplicationLocalizer.Language.Translate(57104);     // Select
            btnClose.Text                 = ApplicationLocalizer.Language.Translate(57105);     // Close            
            colSalesInvoiceID.Caption     = ApplicationLocalizer.Language.Translate(57106);     // Sales invoice
            colCreationDate.Caption       = ApplicationLocalizer.Language.Translate(57107);     // Created
            colTotalPaidAmount.Caption    = ApplicationLocalizer.Language.Translate(57108);     // Paid
            colTotalInvoiceAmount.Caption = ApplicationLocalizer.Language.Translate(57109);     // Total
            colBalanceAmount.Caption      = ApplicationLocalizer.Language.Translate(57110);     // Balance
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                grSalesInvoices.DataSource = salesInvoices;

                TranslateLabels();
            }
            base.OnLoad(e);
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            gridView1.MovePrevPage();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            gridView1.MoveNextPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            gridView1.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            gridView1.MoveNext();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SetSelectedSalesInvoice();
        }

        private void grSalesOrders_DoubleClick(object sender, EventArgs e)
        {
            SetSelectedSalesInvoice();
        }

        private void SetSelectedSalesInvoice()
        {
            try
            {
                System.Data.DataRow Row = gridView1.GetDataRow(gridView1.GetSelectedRows()[0]);
                selectedSalesInvoiceId = (string)Row["INVOICEID"];
            }
            catch (Exception)
            {
                selectedSalesInvoiceId = null;
            }
            finally
            {
                Close();
            }
        }
    }
}
