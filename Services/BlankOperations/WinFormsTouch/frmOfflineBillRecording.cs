using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using System.IO;
using System.Data.OleDb;
using System.Globalization;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOfflineBillRecording : frmTouchBase
    {
        #region Variable
        SqlConnection conn = new SqlConnection();
        public IPosTransaction pos { get; set; }

        [Import]
        private IApplication application;
        DataTable skuItem = new DataTable();
        DataTable dtSku = new DataTable();

        Random randUnique = new Random();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataTable dtItemInfo = new DataTable("dtItemInfo");

        Random randUniqueMOP = new Random();
        bool IsEditMOP = false;
        int EditselectedIndexMOP = 0;
        DataTable dtMOPInfo = new DataTable("dtMOPInfo");



        /// <summary>
        /// </summary>
        enum OfflineTransactionType
        {
            None = 0,
            Sales = 1,
            Purchase = 2,
            Exchange = 3,
            PurchaseReturn = 4,
            ExchangeReturn = 5, 
            Advance = 6,
            FPP = 7,
            GMA = 8, 
        }
        #endregion

        enum MakingType
        {
            None = 0,
            Percent = 1,
            Tot = 2,
        }

        public frmOfflineBillRecording()
        {
            InitializeComponent();
        }

        public frmOfflineBillRecording(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            BindBillType();
            LoadMOPType();
            cmbMakingType.DataSource = Enum.GetValues(typeof(MakingType));
            dtORGDate.Enabled = false;
        }


        void BindBillType()
        {
            cmbIdType.DataSource = Enum.GetValues(typeof(OfflineTransactionType));
        }

        public string GetReceiptId()
        {
            string TransId = string.Empty;
            TransId = GetNextOfflineBillTransactionId();
            return TransId;
        }

        enum OfflineBillRecord
        {
            OfflineBillRecord = 21
        }

        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        public string GetNextOfflineBillTransactionId()
        {
            try
            {
                OfflineBillRecord transType = OfflineBillRecord.OfflineBillRecord;

                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                transactionNumber((int)transType, funcProfileId, out mask);
                if (string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }
        #endregion

        #region GetTransferTransactionId()  - CHANGED BY NIMBUS
        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 05-03-2019
        ///  Purpose :: to get mask of transaction id
        /// </summary>
        /// <param name="transType"></param>
        /// <param name="funcProfile"></param>
        /// <param name="mask"></param>
        private void transactionNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;
                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        #endregion

        #region GetSeedVal() - CHANGED BY NIMBUS
        /// <summary>
        ///  DEV BY RIPAN HOSSAIN ON 05/03/19
        ///  Purpose :: get max value(numeric) from HEADER table to generate new transaction id
        /// </summary>
        /// <returns></returns>
        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)OfflineBillRecord.OfflineBillRecord;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(RECEIPTID,@VAL,LEN(RECEIPTID)),0) AS INTEGER)) + 1 from CRWOfflineBillHeader";

                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                        conn.Open();
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                        Val = "1";


                    return Val;

                }
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbIdType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbIdType.SelectedIndex > 0)
                txtRefInvoiceNo.Text = GetReceiptId();
            else
                txtRefInvoiceNo.Text = "";

            if (cmbIdType.SelectedIndex == (int)OfflineTransactionType.GMA)
                dtORGDate.Enabled = true;
            else
                dtORGDate.Enabled = false;
        }

        private void LoadMOPType()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" select Name,TENDERTYPEID from RETAILSTORETENDERTYPETABLE WHERE CHANNEL = (SELECT top 1 RECID  from RETAILSTORETABLE  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "')");

            DataTable dtMOPType = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            using (SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dtMOPType.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (dtMOPType.Rows.Count > 0)
            {
                cmbMOPType.DataSource = dtMOPType;
                cmbMOPType.DisplayMember = "Name";
                cmbMOPType.ValueMember = "TENDERTYPEID";
            }
        }

        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if (isValiedItem())
            {
                //====================================== Subho check =============================
                calculateAmount();
                calTaxAmount();
                DataRow dr;
                if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("PARTICULARS", typeof(string));
                    dtItemInfo.Columns.Add("CERTIFICATIONNO", typeof(string));
                    dtItemInfo.Columns.Add("HSNCODE", typeof(string));
                    dtItemInfo.Columns.Add("TAGNO", typeof(string));
                    dtItemInfo.Columns.Add("UOM", typeof(string));
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("PURITY", typeof(string));
                    dtItemInfo.Columns.Add("QTY", typeof(decimal));
                    dtItemInfo.Columns.Add("GROSSWT", typeof(decimal));
                    dtItemInfo.Columns.Add("NETWT", typeof(decimal));
                    dtItemInfo.Columns.Add("RATEPERGM", typeof(decimal));
                    dtItemInfo.Columns.Add("MAKINGTYPE", typeof(int));
                    dtItemInfo.Columns.Add("MAKING", typeof(decimal));
                    dtItemInfo.Columns.Add("DISCOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("CGSTPCT", typeof(decimal));
                    dtItemInfo.Columns.Add("SGSTPCT", typeof(decimal));
                    dtItemInfo.Columns.Add("IGSTPCT", typeof(decimal));
                    dtItemInfo.Columns.Add("TAXAMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                    dtItemInfo.Columns.Add("SALESMAN", typeof(string));
                }
                if (IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();


                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());

                    dr["PARTICULARS"] = Convert.ToString(txtParticulars.Text.Trim());
                    dr["CERTIFICATIONNO"] = Convert.ToString(txtCertificationNo.Text.Trim());
                    dr["HSNCODE"] = Convert.ToString(txtHSNCode.Text.Trim());
                    dr["TAGNO"] = Convert.ToString(txtTAGNo.Text.Trim());
                    dr["UOM"] = Convert.ToString(txtUOM.Text.Trim());

                    if (!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = Convert.ToDecimal(txtPCS.Text.Trim());
                    else
                        dr["PCS"] = "0";

                    dr["PURITY"] = Convert.ToString(txtPurity.Text.Trim());

                    if (!string.IsNullOrEmpty(txtQty.Text.Trim()))
                        dr["QTY"] = decimal.Round(Convert.ToDecimal(txtQty.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["QTY"] = "0";

                    if (!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                        dr["GROSSWT"] = decimal.Round(Convert.ToDecimal(txtGrossWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["GROSSWT"] = "0";
                    if (!string.IsNullOrEmpty(txtNetWt.Text.Trim()))
                        dr["NETWT"] = decimal.Round(Convert.ToDecimal(txtNetWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["NETWT"] = "0";

                    if (!string.IsNullOrEmpty(txtRate.Text.Trim()))
                        dr["RATEPERGM"] = decimal.Round(Convert.ToDecimal(txtRate.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["RATEPERGM"] = "0";

                    dr["MAKINGTYPE"] = Convert.ToInt16(cmbMakingType.SelectedIndex);

                    if (!string.IsNullOrEmpty(txtMaking.Text.Trim()))
                        dr["MAKING"] = decimal.Round(Convert.ToDecimal(txtMaking.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["MAKING"] = "0";

                    if (!string.IsNullOrEmpty(txtDiscount.Text.Trim()))
                        dr["DISCOUNT"] = decimal.Round(Convert.ToDecimal(txtDiscount.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        dr["DISCOUNT"] = "0";

                    if (!string.IsNullOrEmpty(txtCGST.Text.Trim()))
                        dr["CGSTPCT"] = decimal.Round(Convert.ToDecimal(txtCGST.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["CGSTPCT"] = "0";

                    if (!string.IsNullOrEmpty(txtSGST.Text.Trim()))
                        dr["SGSTPCT"] = decimal.Round(Convert.ToDecimal(txtSGST.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["SGSTPCT"] = "0";

                    if (!string.IsNullOrEmpty(txtIGST.Text.Trim()))
                        dr["IGSTPCT"] = decimal.Round(Convert.ToDecimal(txtIGST.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["IGSTPCT"] = "0";

                    if (!string.IsNullOrEmpty(txtTaxAmt.Text.Trim()))
                        dr["TAXAMOUNT"] = decimal.Round(Convert.ToDecimal(txtTaxAmt.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["TAXAMOUNT"] = "0";

                    if (!string.IsNullOrEmpty(txtAmount.Text.Trim()))
                        dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["AMOUNT"] = "0";

                    dr["SALESMAN"] = txtSalesMan.Text.Trim();

                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                }

                if (IsEdit == true)
                {
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];

                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["PARTICULARS"] = txtParticulars.Text.Trim();
                    EditRow["CERTIFICATIONNO"] = txtCertificationNo.Text.Trim();
                    EditRow["HSNCODE"] = txtHSNCode.Text.Trim();
                    EditRow["TAGNO"] = txtTAGNo.Text.Trim();
                    EditRow["UOM"] = txtUOM.Text.Trim();
                    EditRow["PCS"] = string.IsNullOrEmpty(txtPCS.Text.Trim()) ? "0" : txtPCS.Text.Trim();
                    EditRow["PURITY"] = txtPurity.Text.Trim();
                    EditRow["QTY"] = string.IsNullOrEmpty(txtQty.Text.Trim()) ? "0" : txtQty.Text.Trim();
                    EditRow["GROSSWT"] = string.IsNullOrEmpty(txtGrossWt.Text.Trim()) ? "0" : txtGrossWt.Text.Trim();
                    EditRow["NETWT"] = string.IsNullOrEmpty(txtNetWt.Text.Trim()) ? "0" : txtNetWt.Text.Trim();
                    EditRow["RATEPERGM"] = string.IsNullOrEmpty(txtRate.Text.Trim()) ? "0" : txtRate.Text.Trim();
                    EditRow["MAKINGTYPE"] = Convert.ToInt16(cmbMakingType.SelectedIndex);
                    EditRow["MAKING"] = string.IsNullOrEmpty(txtMaking.Text.Trim()) ? "0" : txtMaking.Text.Trim();
                    EditRow["DISCOUNT"] = string.IsNullOrEmpty(txtDiscount.Text.Trim()) ? "0" : txtDiscount.Text.Trim();
                    EditRow["CGSTPCT"] = string.IsNullOrEmpty(txtCGST.Text.Trim()) ? "0" : txtCGST.Text.Trim();
                    EditRow["SGSTPCT"] = string.IsNullOrEmpty(txtSGST.Text.Trim()) ? "0" : txtSGST.Text.Trim();
                    EditRow["IGSTPCT"] = string.IsNullOrEmpty(txtIGST.Text.Trim()) ? "0" : txtIGST.Text.Trim();
                    EditRow["TAXAMOUNT"] = string.IsNullOrEmpty(txtTaxAmt.Text.Trim()) ? "0" : txtTaxAmt.Text.Trim();
                    EditRow["AMOUNT"] = string.IsNullOrEmpty(txtAmount.Text.Trim()) ? "0" : txtAmount.Text.Trim();
                    EditRow["SALESMAN"] = txtSalesMan.Text.Trim();

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                clearItemControl();
            }
            CalTotRecPay();
        }

        private void CalTotRecPay()
        {
            decimal dAmt = 0m;
            foreach (DataRow dr1 in dtItemInfo.Rows)
            {
                //Start : added on 26/05/2014                       
                dAmt = dAmt + Convert.ToDecimal(dr1["AMOUNT"]);
                //End : added on 26/05/2014
            }
            txtNetPayOrRec.Text = Convert.ToString(decimal.Round(dAmt, 2, MidpointRounding.AwayFromZero));
        }

        private bool isValiedItem()
        {
            if (string.IsNullOrEmpty(txtTAGNo.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("TAG no can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtTAGNo.Focus();
                    return false;
                }
            }

            //if (string.IsNullOrEmpty(txtHSNCode.Text.Trim()))
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("HSN code can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        txtHSNCode.Focus();
            //        return false;
            //    }
            //}

            //if (string.IsNullOrEmpty(txtQty.Text.Trim()))
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Qty can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        txtQty.Focus();
            //        return false;
            //    }
            //}
            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Rate can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtRate.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtAmount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Sales man can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtAmount.Focus();
                    return false;
                }
            }
            if (rdbCGST.Checked)
            {
                if (string.IsNullOrEmpty(txtCGST.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("CGST percentage can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtCGST.Focus();
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(txtSGST.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SGST percentage can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtSGST.Focus();
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(txtTaxAmt.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Tax amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtTaxAmt.Focus();
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }

            if (rdIGST.Checked)
            {
                if (string.IsNullOrEmpty(txtIGST.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("IGST percentage can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtIGST.Focus();
                        return false;
                    }
                }
                if (string.IsNullOrEmpty(txtTaxAmt.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Tax amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        txtTaxAmt.Focus();
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }

        private bool isValiedMOP()
        {
            if (string.IsNullOrEmpty(cmbMOPType.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("MOP can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    cmbMOPType.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtMOPAmount.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Payment amount can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtMOPAmount.Focus();
                    return false;
                }
            }
            else
            {
                return true;
            }

        }

        private bool isValied()
        {
            if (string.IsNullOrEmpty(txtRefInvoiceNo.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select a proper transaction type", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    cmbIdType.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtCustName.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtCustName.Focus();
                    return false;
                }
            }
            if (string.IsNullOrEmpty(txtCustMobile.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer mobile can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtCustMobile.Focus();
                    return false;
                }
            }
            if (dtItemInfo == null || dtItemInfo.Rows.Count == 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Items are there in sales line", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if (cmbIdType.SelectedIndex != (int)OfflineTransactionType.GMA)
            {
                if (dtMOPInfo == null || dtMOPInfo.Rows.Count == 0)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No payments are there in payments line", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }

                decimal dTotSaleItemAmt = 0m;
                decimal dTotMOPAmt = 0m;
                foreach (DataRow dr1 in dtItemInfo.Rows)
                {
                    dTotSaleItemAmt = dTotSaleItemAmt + Convert.ToDecimal(dr1["AMOUNT"]);
                }

                foreach (DataRow dr2 in dtMOPInfo.Rows)
                {
                    dTotMOPAmt = dTotMOPAmt + Convert.ToDecimal(dr2["AMOUNT"]);
                }

                if (dTotSaleItemAmt != dTotMOPAmt)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Payments amount and sales amount mismatch", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }

        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtItemInfo.Rows[EditselectedIndex];
                    txtParticulars.Text = Convert.ToString(theRowToSelect["PARTICULARS"]);
                    txtCertificationNo.Text = Convert.ToString(theRowToSelect["CERTIFICATIONNO"]);
                    txtHSNCode.Text = Convert.ToString(theRowToSelect["HSNCODE"]);
                    txtTAGNo.Text = Convert.ToString(theRowToSelect["TAGNO"]);
                    txtUOM.Text = Convert.ToString(theRowToSelect["UOM"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtPurity.Text = Convert.ToString(theRowToSelect["PURITY"]);
                    txtQty.Text = Convert.ToString(theRowToSelect["QTY"]);
                    txtGrossWt.Text = Convert.ToString(theRowToSelect["GROSSWT"]);
                    txtNetWt.Text = Convert.ToString(theRowToSelect["NETWT"]);
                    txtRate.Text = Convert.ToString(theRowToSelect["RATEPERGM"]);
                    txtMaking.Text = Convert.ToString(theRowToSelect["MAKING"]);
                    txtDiscount.Text = Convert.ToString(theRowToSelect["DISCOUNT"]);
                    txtCGST.Text = Convert.ToString(theRowToSelect["CGSTPCT"]);
                    txtSGST.Text = Convert.ToString(theRowToSelect["SGSTPCT"]);
                    txtIGST.Text = Convert.ToString(theRowToSelect["IGSTPCT"]);
                    txtTaxAmt.Text = Convert.ToString(theRowToSelect["TAXAMOUNT"]);
                    txtAmount.Text = Convert.ToString(theRowToSelect["AMOUNT"]);
                    txtSalesMan.Text = Convert.ToString(theRowToSelect["SALESMAN"]);
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if (grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];
                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }
            }
            if (DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;
            CalTotRecPay();
        }

        #region SaveFuction
        private void SaveOfflineBill()
        {
            int iHeader = 0;
            int iItemDetails = 0;
            DataTable dtBookedGroup = new DataTable();
            DataTable dtFromCounterGroup = new DataTable();

            SqlTransaction transaction = null;

            #region Offline Bill HEADER
            string commandText = " INSERT INTO [CRWOfflineBillHeader]([OffLineBillType],ReceiptId,TransDate," +
                                 " [STOREID],[TERMINALID],[DATAAREAID],[CustAccount]," +
                                 " [CustName],[CustPAN],[CustAadhar],[CustMobile], " +
                                 " [CustLoyaltyNo],[CustGSTIN],[CustAddress],[OperatorId]," +
                                 " [Remarks],RefOrdNo,ORGDATE)" +
                                 " VALUES(@OffLineBillType,@ReceiptId,@TransDate,@STOREID,@TERMINALID,@DATAAREAID," +
                                 " @CustAccount,@CustName,@CustPAN,@CustAadhar,@CustMobile,@CustLoyaltyNo,@CustGSTIN," +
                                 " @CustAddress,@OperatorId,@Remarks,@RefOrdNo,@ORGDATE)";
            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                command.Parameters.Add("@OffLineBillType", SqlDbType.Int).Value = Convert.ToInt16(cmbIdType.SelectedIndex);
                command.Parameters.Add("@ReceiptId", SqlDbType.NVarChar).Value = txtRefInvoiceNo.Text.Trim();
                
                if(dtORGDate.Enabled)//chenges done on 24th Apr-2019 on req of PNG @GMA Invoice
                    command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime("22-Feb-2019").ToShortDateString();
                else
                    command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now).ToShortDateString();

                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                if (application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@CustAccount", SqlDbType.NVarChar, 20).Value = txtCustAcc.Text.Trim();
                command.Parameters.Add("@CustName", SqlDbType.NVarChar, 60).Value = txtCustName.Text.Trim();
                command.Parameters.Add("@CustPAN", SqlDbType.NVarChar, 10).Value = txtCustPAN.Text.Trim();
                command.Parameters.Add("@CustAadhar", SqlDbType.NVarChar, 20).Value = txtCustAadhar.Text.Trim();
                command.Parameters.Add("@CustMobile", SqlDbType.NVarChar, 13).Value = txtCustMobile.Text.Trim();
                command.Parameters.Add("@CustLoyaltyNo", SqlDbType.NVarChar, 20).Value = txtCustLoyalty.Text.Trim();
                command.Parameters.Add("@CustGSTIN", SqlDbType.NVarChar, 20).Value = txtCustGSTIN.Text.Trim();
                command.Parameters.Add("@CustAddress", SqlDbType.NVarChar, 250).Value = txtAddress.Text.Trim();
                command.Parameters.Add("@OperatorId", SqlDbType.NVarChar, 10).Value = pos.OperatorId;
                command.Parameters.Add("@Remarks", SqlDbType.NVarChar, 250).Value = "";
                command.Parameters.Add("@RefOrdNo", SqlDbType.NVarChar, 20).Value = txtRefOrdNo.Text;
                command.Parameters.Add("@ORGDATE", SqlDbType.DateTime).Value = dtORGDate.Value.ToString("dd-MMM-yyyy");

            #endregion

                command.CommandTimeout = 0;
                iHeader = command.ExecuteNonQuery();

                if (iHeader == 1)
                {
                    if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                    {
                        #region Item
                        if (dtItemInfo != null && dtItemInfo.Rows.Count > 0)
                        {
                            int iL = 1;
                            foreach (DataRow dr in dtItemInfo.Rows)
                            {
                                string QRY = "INSERT INTO [CRWOfflineBillItem] " +
                                           "([ReceiptId],[ItemId],[LineNum],[Particulars]," +
                                           " [CertificationNo],[HSNCode],[UOM],PCS,Purity,Qty," +
                                           " GrossWt,NetWt,Rate,MakingType,Making,Discount,CGST,SGST," +
                                           " IGST,TaxAmount,Amount,SalesMan,STOREID,TERMINALID,DATAAREAID) " +
                                           " VALUES" +
                                           "(@ReceiptId,@ItemId,@LineNum,@Particulars,@CertificationNo,@HSNCode," +
                                           " @UOM,@PCS,@Purity,@Qty,@GrossWt,@NetWt,@Rate,@MakingType,@Making," +
                                           " @Discount,@CGST,@SGST,@IGST,@TaxAmount,@Amount,@SalesMan," +
                                           " @STOREID,@TERMINALID,@DATAAREAID)";

                                using (SqlCommand cmdItem = new SqlCommand(QRY, connection, transaction))
                                {
                                    cmdItem.CommandTimeout = 0;
                                    cmdItem.Parameters.Add("@ReceiptId", SqlDbType.VarChar, 20).Value = txtRefInvoiceNo.Text;
                                    cmdItem.Parameters.Add("@ItemId", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["TAGNO"].ToString());
                                    cmdItem.Parameters.Add("@LineNum", SqlDbType.Int).Value = iL;
                                    cmdItem.Parameters.Add("@Particulars", SqlDbType.VarChar, 60).Value = Convert.ToString(dr["PARTICULARS"].ToString());
                                    cmdItem.Parameters.Add("@CertificationNo", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["CERTIFICATIONNO"].ToString());
                                    cmdItem.Parameters.Add("@HSNCode", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["HSNCODE"].ToString());
                                    cmdItem.Parameters.Add("@UOM", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["UOM"].ToString());
                                    cmdItem.Parameters.Add("@PCS", SqlDbType.Int).Value = Convert.ToInt16(dr["PCS"].ToString());
                                    cmdItem.Parameters.Add("@Purity", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["PURITY"].ToString());
                                    cmdItem.Parameters.Add("@Qty", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["QTY"].ToString());
                                    cmdItem.Parameters.Add("@GrossWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["GROSSWT"].ToString());
                                    cmdItem.Parameters.Add("@NetWt", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["NETWT"].ToString());
                                    cmdItem.Parameters.Add("@Rate", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["RATEPERGM"].ToString());
                                    cmdItem.Parameters.Add("@MakingType", SqlDbType.Int).Value = Convert.ToInt16(dr["MAKINGTYPE"].ToString());
                                    cmdItem.Parameters.Add("@Making", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["MAKING"].ToString());
                                    cmdItem.Parameters.Add("@Discount", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["DISCOUNT"].ToString());
                                    cmdItem.Parameters.Add("@CGST", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["CGSTPCT"].ToString());
                                    cmdItem.Parameters.Add("@SGST", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["SGSTPCT"].ToString());
                                    cmdItem.Parameters.Add("@IGST", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["IGSTPCT"].ToString());
                                    cmdItem.Parameters.Add("@TaxAmount", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["TAXAMOUNT"].ToString());
                                    cmdItem.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["AMOUNT"].ToString());
                                    cmdItem.Parameters.Add("@SalesMan", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["SALESMAN"].ToString());
                                    cmdItem.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdItem.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdItem.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdItem.ExecuteNonQuery();
                                    cmdItem.Dispose();
                                    iL++;
                                }
                            }
                        }
                        #endregion

                        #region MOP Info
                        if (dtMOPInfo != null && dtMOPInfo.Rows.Count > 0)
                        {
                            int iL = 1;
                            foreach (DataRow dr in dtMOPInfo.Rows)
                            {
                                string chqQRY = "INSERT INTO [CRWOfflineBillMOP] " +
                                           "([ReceiptId],[TenderTypeId],[CardOrChqNo]," +
                                           " [Amount],STOREID,TERMINALID,DATAAREAID) " +
                                           " VALUES" +
                                           "(@ReceiptId,@TenderTypeId,@CardOrChqNo,@Amount,@STOREID,@TERMINALID,@DATAAREAID)";
                                using (SqlCommand cmdMOP = new SqlCommand(chqQRY, connection, transaction))
                                {
                                    cmdMOP.CommandTimeout = 0;
                                    cmdMOP.Parameters.Add("@ReceiptId", SqlDbType.VarChar, 20).Value = txtRefInvoiceNo.Text;
                                    cmdMOP.Parameters.Add("@TenderTypeId", SqlDbType.VarChar, 10).Value = Convert.ToString(dr["TENDERID"].ToString());
                                    cmdMOP.Parameters.Add("@CardOrChqNo", SqlDbType.VarChar, 20).Value = Convert.ToString(dr["CARDORCHQNO"].ToString());
                                    cmdMOP.Parameters.Add("@Amount", SqlDbType.Decimal).Value = Convert.ToDecimal(dr["AMOUNT"].ToString());
                                    cmdMOP.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                                    cmdMOP.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                                    cmdMOP.Parameters.Add("@DATAAREAID", SqlDbType.VarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                                    cmdMOP.ExecuteNonQuery();
                                    cmdMOP.Dispose();
                                    iL++;
                                }
                            }
                        }
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();


                if (iHeader == 1 || iItemDetails == 1)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Offline Bill record has been created successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        try
                        {
                            PrintVoucher();
                        }
                        catch { }

                        ClearControls();
                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }

            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void PrintVoucher()
        {
            frmOfflineBillPrint objVoucher = new frmOfflineBillPrint(pos, txtRefInvoiceNo.Text);
            objVoucher.ShowDialog();

            if (cmbIdType.SelectedIndex == (int)OfflineTransactionType.GMA)
            {
                frmOfflineBillPrint objVoucher1 = new frmOfflineBillPrint(pos, txtRefInvoiceNo.Text,1);
                objVoucher1.ShowDialog();
            }
        }

        private void ClearControls()
        {
            txtCustAcc.Text = "";
            txtCustAadhar.Text = "";
            txtCustName.Text = "";
            txtCustMobile.Text = "";
            txtCustGSTIN.Text = "";
            txtCustLoyalty.Text = "";
            txtCustPAN.Text = "";
            txtAddress.Text = "";
            grItems.DataSource = null;
            dtItemInfo.Clear();
            grMOP.DataSource = null;
            dtMOPInfo.Clear();
            dtItemInfo = new DataTable();
            dtMOPInfo = new DataTable();
            txtRefInvoiceNo.Text = GetNextOfflineBillTransactionId();
            txtNetPayOrRec.Text = "";

        }
        #endregion

        private void rdbCGST_CheckedChanged(object sender, EventArgs e)
        {
            radioButtanClick();
            calculateAmount();
        }

        private void radioButtanClick()
        {
            if (rdbCGST.Checked)
            {
                txtCGST.Enabled = true;
                txtSGST.Enabled = true;
                txtIGST.Enabled = false;
                txtIGST.Text = "";
                txtCGST.Text = "1.50";
                txtSGST.Text = "1.50";
            }
            else if (rdIGST.Checked)
            {
                txtCGST.Text = "";
                txtSGST.Text = "";
                txtCGST.Enabled = false;
                txtSGST.Enabled = false;
                txtIGST.Enabled = true;
                txtIGST.Text = "3.00";
            }
            else
            {
                txtCGST.Text = "";
                txtSGST.Text = "";
                txtIGST.Text = "";
                txtCGST.Enabled = false;
                txtSGST.Enabled = false;
                txtIGST.Enabled = false;
            }
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtQty.Text == string.Empty && e.KeyChar == '.')
            //{
            //    e.Handled = true;
            //}

            //else
            //{

            Regex reg = new Regex(@"^-?\d+[.]?\d*$");

            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(txtQty.Text.Insert(txtQty.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;

            //if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            //{
            //    e.Handled = true;
            //}
            //if (e.KeyChar == '-' && (sender as TextBox).Text.Length > 0)
            //{
            //    e.Handled = true;
            //}

            //if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //{
            //    e.Handled = true;
            //}

            //if (Regex.IsMatch(txtQty.Text, @"\.\d\d\d"))
            //{
            //    e.Handled = true;
            //}

            //if (e.KeyChar == (Char)Keys.Enter)
            //{
            //    e.Handled = true;
            //}
            //}
        }

        private void btnAddMOP_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if (isValiedMOP())
            {
                DataRow dr;
                if (IsEditMOP == false && dtMOPInfo != null && dtMOPInfo.Rows.Count == 0 && dtMOPInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    dtMOPInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtMOPInfo.Columns.Add("MOP", typeof(string));
                    dtMOPInfo.Columns.Add("TENDERID", typeof(string));
                    dtMOPInfo.Columns.Add("CARDORCHQNO", typeof(string));
                    dtMOPInfo.Columns.Add("AMOUNT", typeof(decimal));

                }
                if (IsEditMOP == false)
                {
                    dr = dtMOPInfo.NewRow();

                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUniqueMOP.Next());
                    dr["MOP"] = Convert.ToString(cmbMOPType.Text.Trim());
                    dr["TENDERID"] = Convert.ToString(cmbMOPType.SelectedValue);
                    dr["CARDORCHQNO"] = txtMOPType.Text.Trim();

                    if (!string.IsNullOrEmpty(txtMOPAmount.Text.Trim()))
                        dr["AMOUNT"] = decimal.Round(Convert.ToDecimal(txtMOPAmount.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        dr["AMOUNT"] = "0";

                    dtMOPInfo.Rows.Add(dr);
                    grMOP.DataSource = dtMOPInfo.DefaultView;
                }

                if (IsEditMOP == true)
                {
                    DataRow EditRow = dtMOPInfo.Rows[EditselectedIndexMOP];

                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["MOP"] = cmbMOPType.Text.Trim();
                    EditRow["TENDERID"] = Convert.ToString(cmbMOPType.SelectedValue);
                    EditRow["CARDORCHQNO"] = txtMOPType.Text.Trim();
                    EditRow["AMOUNT"] = string.IsNullOrEmpty(txtMOPAmount.Text.Trim()) ? "0" : txtAmount.Text.Trim();


                    dtMOPInfo.AcceptChanges();
                    grMOP.DataSource = dtMOPInfo.DefaultView;
                }

                clearMOPControl();
            }
        }

        private void clearMOPControl()
        {
            cmbMOPType.SelectedIndex = 0;
            txtMOPType.Text = "";
            txtMOPAmount.Text = "";
        }

        private void clearItemControl()
        {
            txtParticulars.Text = "";
            txtCertificationNo.Text = "";
            txtHSNCode.Text = "";
            txtTAGNo.Text = "";
            txtUOM.Text = "";
            txtPCS.Text = "";
            txtPurity.Text = "";
            txtQty.Text = "";
            txtGrossWt.Text = "";
            txtNetWt.Text = "";
            txtRate.Text = "";
            txtMaking.Text = "";
            txtDiscount.Text = "";
            txtCGST.Text = "";
            txtSGST.Text = "";
            txtIGST.Text = "";
            txtTaxAmt.Text = "";
            txtAmount.Text = "";
            txtSalesMan.Text = "";
            cmbMakingType.DataSource = Enum.GetValues(typeof(MakingType));
            rdbNoTax.Checked = true;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (isValied())
            {
                SaveOfflineBill();
            }
        }

        private void txtGrossWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtGrossWt.Text == string.Empty && e.KeyChar == '.')
            //{
            //    e.Handled = true;
            //}
            //else
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            //    {
            //        e.Handled = true;
            //    }

            //    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //    {
            //        e.Handled = true;
            //    }

            //    if (Regex.IsMatch(txtGrossWt.Text, @"\.\d\d\d"))
            //    {
            //        e.Handled = true;
            //    }

            //    if (e.KeyChar == (Char)Keys.Enter)
            //    {
            //        e.Handled = true;
            //    }
            //}

            Regex reg = new Regex(@"^-?\d+[.]?\d*$");

            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(txtGrossWt.Text.Insert(txtGrossWt.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;
        }

        private void txtNetWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtNetWt.Text == string.Empty && e.KeyChar == '.')
            //{
            //    e.Handled = true;
            //}
            //else
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            //    {
            //        e.Handled = true;
            //    }

            //    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //    {
            //        e.Handled = true;
            //    }

            //    if (Regex.IsMatch(txtNetWt.Text, @"\.\d\d\d"))
            //    {
            //        e.Handled = true;
            //    }

            //    if (e.KeyChar == (Char)Keys.Enter)
            //    {
            //        e.Handled = true;
            //    }
            //}

            Regex reg = new Regex(@"^-?\d+[.]?\d*$");

            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(txtNetWt.Text.Insert(txtNetWt.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtRate.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtRate.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtMaking_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtMaking.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtMaking.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtDiscount.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtDiscount.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtAmount.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtAmount.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtCGST_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtCGST.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtCGST.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtSGST_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtSGST.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtSGST.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtIGST_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtIGST.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtIGST.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtTaxAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtTaxAmt.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                if (Regex.IsMatch(txtTaxAmt.Text, @"\.\d\d"))
                {
                    e.Handled = true;
                }

                if (e.KeyChar == (Char)Keys.Enter)
                {
                    e.Handled = true;
                }
            }
        }

        private void txtMOPAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (txtMOPAmount.Text == string.Empty && e.KeyChar == '.')
            //{
            //    e.Handled = true;
            //}
            //else
            //{
            //    if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            //    {
            //        e.Handled = true;
            //    }

            //    if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //    {
            //        e.Handled = true;
            //    }

            //    if (Regex.IsMatch(txtMOPAmount.Text, @"\.\d\d"))
            //    {
            //        e.Handled = true;
            //    }

            //    if (e.KeyChar == (Char)Keys.Enter)
            //    {
            //        e.Handled = true;
            //    }
            //}
            Regex reg = new Regex(@"^-?\d+[.]?\d*$");

            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(txtMOPAmount.Text.Insert(txtMOPAmount.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;
        }

        private void cmbMOPType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txtCGST_TextChanged(object sender, EventArgs e)
        {
            //  calTaxAmount();
        }

        private void calTaxAmount()
        {
            decimal dAmt = 0m;
            decimal dCGSTPct = 0m;
            decimal dSGSTPct = 0m;
            decimal dIGSTPct = 0m;
            decimal dTotalTaxPct = 0m;
            decimal dTaxAmt = 0m;
            if (!string.IsNullOrEmpty(txtTaxAmt.Text))
            {
                dTaxAmt = Convert.ToDecimal(txtTaxAmt.Text);
            }

            if (!string.IsNullOrEmpty(txtAmount.Text))
            {
                dAmt = Convert.ToDecimal(txtAmount.Text);
            }

            if (rdbCGST.Checked)
            {
                if (!string.IsNullOrEmpty(txtCGST.Text))
                {
                    dCGSTPct = Convert.ToDecimal(txtCGST.Text);
                }
                if (!string.IsNullOrEmpty(txtSGST.Text))
                {
                    dSGSTPct = Convert.ToDecimal(txtSGST.Text);
                }
                dTotalTaxPct = dCGSTPct + dSGSTPct;
            }
            if (rdIGST.Checked)
            {
                if (!string.IsNullOrEmpty(txtIGST.Text))
                {
                    dIGSTPct = Convert.ToDecimal(txtIGST.Text);
                }

                dTotalTaxPct = dIGSTPct;
            }

            if (dTotalTaxPct > 0)
            {
                txtTaxAmt.Text = Convert.ToString(decimal.Round(((dAmt - dTaxAmt) * dTotalTaxPct) / 100, 2, MidpointRounding.AwayFromZero));
            }
        }

        private void txtSGST_TextChanged(object sender, EventArgs e)
        {
            calTaxAmount();
        }

        private void txtIGST_TextChanged(object sender, EventArgs e)
        {
            calTaxAmount();
        }

        private void rdIGST_CheckedChanged(object sender, EventArgs e)
        {
            radioButtanClick();
            calculateAmount();
        }

        private void txtQty_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQty.Text) && txtQty.Text != "-")
            {
                txtGrossWt.Text = txtQty.Text;
                txtNetWt.Text = txtQty.Text;

                calculateAmount();
            }
        }

        private void cmbMakingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculateAmount();
        }

        private void calculateAmount()
        {
            decimal dNetWt = 0m;
            decimal dRate = 0m;
            decimal dMk = 0m;
            decimal dAmt = 0m;
            decimal dDisc = 0m;
            decimal dTaxAmt = 0m;

            if (!string.IsNullOrEmpty(txtNetWt.Text) && txtNetWt.Text != "-")
            {
                dNetWt = Convert.ToDecimal(txtNetWt.Text);
            }

            if (!string.IsNullOrEmpty(txtRate.Text))
            {
                dRate = Convert.ToDecimal(txtRate.Text);
            }

            if (!string.IsNullOrEmpty(txtMaking.Text))
            {
                dMk = Convert.ToDecimal(txtMaking.Text);
            }

            if (!string.IsNullOrEmpty(txtDiscount.Text))
            {
                dDisc = Convert.ToDecimal(txtDiscount.Text);
            }
            if (!string.IsNullOrEmpty(txtTaxAmt.Text))
            {
                dTaxAmt = Convert.ToDecimal(txtTaxAmt.Text);
            }

            if (cmbMakingType.SelectedIndex == (int)MakingType.Percent)
            {
                dAmt = ((dNetWt * dRate) + ((dNetWt * dRate) * dMk / 100) - dDisc) + dTaxAmt;
            }
            else if (cmbMakingType.SelectedIndex == (int)MakingType.Tot)
            {
                dAmt = (((dNetWt * dRate) + dMk) - dDisc) + dTaxAmt;
            }

            txtAmount.Text = Convert.ToString(decimal.Round(dAmt, 2, MidpointRounding.AwayFromZero));

            //  radioButtanClick();
            calTaxAmount();
        }

        private void txtNetWt_TextChanged(object sender, EventArgs e)
        {
            calculateAmount();
        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            calculateAmount();
            calTaxAmount();
        }

        private void txtMaking_TextChanged(object sender, EventArgs e)
        {
            calculateAmount();
        }

        private void txtDiscount_TextChanged(object sender, EventArgs e)
        {
            calculateAmount();
        }

        private void btnMOPDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if (dtMOPInfo != null && dtMOPInfo.Rows.Count > 0)
            {
                if (grdMOPView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdMOPView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtMOPInfo.Rows[DeleteSelectedIndex];
                    dtMOPInfo.Rows.Remove(theRowToDelete);
                    grMOP.DataSource = dtMOPInfo.DefaultView;
                }
            }
            if (DeleteSelectedIndex == 0 && dtMOPInfo != null && dtMOPInfo.Rows.Count == 0)
            {
                grMOP.DataSource = null;
                dtMOPInfo.Clear();
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataTable dtGridItems = new DataTable();

            string commandText = "Select CASE WHEN OffLineBillType = 1   THEN 'Sales'"+  
	                             " WHEN OffLineBillType = 2   THEN 'Purchase'  "+
	                             " WHEN OffLineBillType = 3   THEN 'Exchange' "+ 
	                             " WHEN OffLineBillType = 4   THEN 'PurchaseReturn'"+
	                             " WHEN OffLineBillType = 5   THEN 'ExchangeReturn' "+ 
	                             " WHEN OffLineBillType = 6   THEN 'Advance'"+
                                 " WHEN OffLineBillType = 7   THEN 'FPP' " +
                                 " WHEN OffLineBillType = 8   THEN 'GMA' else 'None' END VoucherType" +
            ",ReceiptId,CONVERT(VARCHAR(15),TransDate,103) AS TransDate ,CustAccount, CustName ,CustMobile ,CustPAN";
            commandText += " FROM CRWOfflineBillHeader";
            commandText += " ORDER BY ReceiptId ";

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);

            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            dtGridItems = new DataTable();
            dtGridItems.Load(reader);
            if (dtGridItems != null && dtGridItems.Rows.Count > 0)
            {
                DataRow selRow = null;
                Dialog.Dialog objCustOrderSearch = new Dialog.Dialog();

                objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Offline Bill");
                if (selRow != null)
                {
                    string sOfflineBill = Convert.ToString(selRow["ReceiptId"]);
                    string sBillType = Convert.ToString(selRow["VoucherType"]);

                    if (sBillType == "GMA")
                    {
                        frmOfflineBillPrint objPrint = new frmOfflineBillPrint(pos, sOfflineBill);
                        objPrint.ShowDialog();
                        frmOfflineBillPrint objPrint1 = new frmOfflineBillPrint(pos, sOfflineBill,1);
                        objPrint1.ShowDialog();
                    }
                    else
                    {
                        frmOfflineBillPrint objPrint = new frmOfflineBillPrint(pos, sOfflineBill);
                        objPrint.ShowDialog();
                    }
                }
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No offline bill exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
                return;
            }

        }

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            Regex reg = new Regex(@"^-?\d+?\d*$");

            if (char.IsControl(e.KeyChar)) return;
            if (!reg.IsMatch(txtPCS.Text.Insert(txtPCS.SelectionStart, e.KeyChar.ToString()) + "1")) e.Handled = true;
        }

        private void rdbNoTax_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbNoTax.Checked)
            {
                txtTaxAmt.Text = "";
                txtSGST.Text = "";
                txtCGST.Text = "";
                txtIGST.Text = "";

                radioButtanClick();
                calculateAmount();
            }
        }
    }
}
