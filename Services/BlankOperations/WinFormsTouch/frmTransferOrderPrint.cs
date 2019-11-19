using System;
using System.Data;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Settings;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmTransferOrderPrint : frmTouchBase
    {
        private IApplication application;

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
            }
        }

        public frmTransferOrderPrint()
        {
            InitializeComponent();
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransOrdrId.Text.Trim()))
            {
                PrintTransferOrder(txtTransOrdrId.Text.Trim(), 0);
            }
        }

        private void btnDetail_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtTransOrdrId.Text.Trim()))
            {
                PrintTransferOrder(txtTransOrdrId.Text.Trim(), 1);
            }
        }

        private void PrintTransferOrder(string sTransOrdrId, int iOption)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    DataSet dsHdr = new DataSet();
                    DataSet dsDtl = new DataSet();
                    ReadOnlyCollection<object> cTransReport;


                    cTransReport = PosApplication.Instance.TransactionServices.InvokeExtension("GetTransferVoucherInfo", sTransOrdrId);

                    StringReader srTransHdr = new StringReader(Convert.ToString(cTransReport[3]));

                    if (Convert.ToString(cTransReport[3]).Trim().Length > 38)
                    {
                        dsHdr.ReadXml(srTransHdr);
                    }

                    StringReader srTransDetail = new StringReader(Convert.ToString(cTransReport[4]));

                    if (Convert.ToString(cTransReport[4]).Trim().Length > 38)
                    {
                        dsDtl.ReadXml(srTransDetail);
                    }

                    Microsoft.Dynamics.Retail.Pos.BlankOperations.Report.frmTransOrderCreateRpt reportfrm
                        = new Report.frmTransOrderCreateRpt(dsHdr, dsDtl, iOption, "STOCK TRANSFER");

                    reportfrm.ShowDialog();
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DataRow drSelected = null;
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("reprintTransferDetails", sStoreId,Convert.ToDateTime(dtpShipDate.Value));

                    bool isAvailable = false;
                    isAvailable = Convert.ToBoolean(containerArray[1]);

                    if (isAvailable)
                    {
                        DataSet dsWH = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsWH.ReadXml(srTransDetail);
                        }
                        if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], drSelected, "Transfer Order list");
                            OSearch.ShowDialog();
                            drSelected = OSearch.SelectedDataRow;
                            if (drSelected != null)
                            {
                                string sTransferId = Convert.ToString(drSelected["TransferId"]);
                                txtTransOrdrId.Text = sTransferId;
                                txtTransOrdrId.Enabled = false;
                            }
                            else
                                txtTransOrdrId.Enabled = true;
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Transfer order found.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Transfer order found.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
