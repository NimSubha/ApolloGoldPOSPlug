using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class FrmCustomerAdvBookQtyDays : frmTouchBase
    {
        private IApplication application;
        public IPosTransaction pos { get; set; }
        string sCustomerId = string.Empty;
        RetailTransaction retailTrans;
        public bool bValid = false;
        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                InternalApplication = value;
            }
        }

        internal static IApplication InternalApplication { get; private set; }

       public FrmCustomerAdvBookQtyDays()
        {
            InitializeComponent();
        }

        public FrmCustomerAdvBookQtyDays(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;

            RetailTransaction _retailTrans = posTransaction as RetailTransaction;
            retailTrans = _retailTrans;

            if (!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
            {
                sCustomerId = retailTrans.Customer.CustomerId;
            }
        }

        #region iSEntryOk()
        public bool iSEntryOk()
        {
            if (string.IsNullOrEmpty(TxtBookQty.Text) && !chkAutoGenCommitedQty.Checked)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Booked qty must not be blank", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    TxtBookQty.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (string.IsNullOrEmpty(TxtDays.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("For Days must not be blank", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    TxtDays.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
           else
            {
                return true;
            }
        }
        #endregion

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (iSEntryOk())
            {
                decimal dBookQty = 0m;
                int iForDays = 0;
                decimal chk = 0m;
                chk = chk + 1;

                if (!string.IsNullOrEmpty(TxtBookQty.Text))
                {
                    dBookQty = Convert.ToDecimal(TxtBookQty.Text);
                    retailTrans.PartnerData.CustAdvCommitedQty = dBookQty;
                    this.Close();
                }
                else
                {
                    retailTrans.PartnerData.CustAdvCommitedQty = dBookQty;
                }

                if (!string.IsNullOrEmpty(TxtDays.Text))
                {
                    iForDays = Convert.ToInt32(TxtDays.Text);
                    retailTrans.PartnerData.CustCommitedForDays = iForDays;
                    this.Close();
                }

            }

        }

        private void chkAutoGenCommitedQty_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoGenCommitedQty.Checked)
            {
                TxtBookQty.Text = "";
                TxtBookQty.Enabled = false;
            }
            else
            {
                TxtBookQty.Text = "";
                TxtBookQty.Enabled = true;
            }
        }

    }
}
