using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmRetailStockCounting : Form
    {
        public IPosTransaction pos { get; set; }
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        [Import]
        private IApplication application;
        public frmRetailStockCounting()
        {
            InitializeComponent();
        }

        public frmRetailStockCounting(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            loadVoucher();
            LoadProductType();
        }


        private void btnCreatedBy_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtCreatedBY.Tag = dialog6.SelectedEmployeeId;
                txtCreatedBY.Text = dialog6.SelectEmployeeName;
            }
        }

        private void LoadProductType()
        {
            string sSQl = " select DISTINCT PRODUCTTYPECODE from INVENTTABLE where PRODUCTTYPECODE!=''";

            cmbProdType.DataSource = null;
            cmbProdType.Items.Add(" ");
            cmbProdType.DataSource = objBlank.NIM_LoadCombo("", "", "", sSQl);
            cmbProdType.DisplayMember = "PRODUCTTYPECODE";
            cmbProdType.ValueMember = "PRODUCTTYPECODE";
        }

        private void btnCountingClosedBy_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog6.ShowDialog();
            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
            {
                txtCountingClosedBy.Tag = dialog6.SelectedEmployeeId;
                txtCountingClosedBy.Text = dialog6.SelectEmployeeName;
            }
        }

        private void btnCreateVoucher_Click(object sender, EventArgs e)
        {
            if (!IsThereAnyNonClosedVoucher(Convert.ToString(cmbProdType.Text),Convert.ToString(txtArticle.Text)))
            {
                if (!string.IsNullOrEmpty(txtCreatedBY.Text))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Are you want to create a new voucher? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                        {
                            txtVoucherNo.Text = GetNextRetailStockCountingID();
                            if (!string.IsNullOrEmpty(txtVoucherNo.Text))
                            {
                                SaveCountingHeaderVoucher();
                                loadVoucher();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Please select a created by person.");
                    btnCreatedBy.Focus();
                }
            }
            else
            {
                MessageBox.Show("Open voucher are there in your system.");
                btnCreatedBy.Focus();
            }
        }
        #region - CHANGED BY NIMBUS TO GET THE ORDER ID

        enum ReceiptTransactionType
        {
            RetailStockCounting = 19
        }

        public string GetNextRetailStockCountingID()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.RetailStockCounting;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                GetMask((int)transType, funcProfileId, out mask);
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

        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.RetailStockCounting;

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(VOUCHERNUM,@VAL,LEN(VOUCHERNUM)),0) AS INTEGER)) + 1 from RETAILSTOCKCOUNTHEADER";
                using (SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    if (string.IsNullOrEmpty(Val))
                    {
                        Val = "1";
                    }

                    return Val;

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

        private void GetMask(int transType, string funcProfile, out string mask)
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


        private void SaveCountingHeaderVoucher()
        {
            string commandText = " INSERT INTO [RETAILSTOCKCOUNTHEADER] " +
                                   " ([VOUCHERNUM],[VOUCHERDATE] " +
                                   " ,[STOREID],[TERMINALID],[DATAAREAID],[CREATEDBY],PRODUCTTYPE,ARTICLE) " +
                                   "  VALUES " +
                                   " (@VOUCHERNUM,@VOUCHERDATE,@STOREID,@TERMINALID,@DATAAREAID,@CREATEDBY,@PRODUCTTYPE,@ARTICLE)";

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                command.Parameters.Add("@VOUCHERNUM", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtVoucherNo.Text);
                command.Parameters.Add("@VOUCHERDATE", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@CREATEDBY", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtCreatedBY.Tag);
                command.Parameters.Add("@PRODUCTTYPE", SqlDbType.NVarChar, 20).Value = Convert.ToString(cmbProdType.Text);
                command.Parameters.Add("@ARTICLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtArticle.Text);

                command.ExecuteNonQuery();
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();
            txtCreatedBY.Text = "";

        }

        private void UpdateCountingVoucher() 
        {
            string commandText = " Update RETAILSTOCKCOUNTDETAILS "+
                                   " set SYSPCS=PCS,"+
                                   " SYSQTY =QTY"+
                                   " where  VOUCHERNUM=@voucher	"+
                                   " and ITEMID in(select ST.SkuNumber from SKUTableTrans ST"+
                                   " left join INVENTTABLE I on St.SkuNumber=I.ITEMID "+
                                   " left join SKUTable_Posted sp on st.SkuNumber=sp.SkuNumber"+
                                   " where ST.isAvailable=1)";

            commandText += "Update [RETAILSTOCKCOUNTHEADER] " +
                            " set COUNTINGCLOSE=1," +
                            " ClosedBy =@ClosedBy," +
                            " COUNTINGCLOSEDDATE =GETDATE()" +
                            " where  VOUCHERNUM=@voucher and COUNTINGCLOSE=0";

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                command.Parameters.Add("@voucher", SqlDbType.NVarChar, 20).Value = Convert.ToString(cmbClosingVoucher.Text);
                command.Parameters.Add("@ClosedBy", SqlDbType.NVarChar, 20).Value = Convert.ToString(txtCountingClosedBy.Tag);
                command.ExecuteNonQuery();
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void CloseCounting()
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand("CloseSKUCounting", conn))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add("@voucher", SqlDbType.NVarChar).Value = cmbClosingVoucher.Text;
                command.Parameters.Add("@ClosedBy", SqlDbType.NVarChar).Value = Convert.ToString(txtCountingClosedBy.Tag);
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@Return_Message", SqlDbType.NVarChar, 1024);
                command.Parameters["@Return_Message"].Direction = ParameterDirection.Output;
                command.ExecuteNonQuery();

                if (conn.State == ConnectionState.Open)
                    conn.Close();
                MessageBox.Show(command.Parameters["@Return_Message"].Value.ToString());
            }

            //UpdateCountingVoucher();

            txtCountingClosedBy.Text = "";
        }

        private void loadVoucher()
        {
            string sSQl = "select VOUCHERNUM from RETAILSTOCKCOUNTHEADER where COUNTINGCLOSE=0";

            cmbClosingVoucher.DataSource = null;
            cmbClosingVoucher.Items.Add(" ");
            cmbClosingVoucher.DataSource = objBlank.NIM_LoadCombo("", "", "", sSQl);
            cmbClosingVoucher.DisplayMember = "VOUCHERNUM";
            cmbClosingVoucher.ValueMember = "VOUCHERNUM";
        }

        private void btnCloseCounting_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtCountingClosedBy.Text) && !string.IsNullOrEmpty(cmbClosingVoucher.Text))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Are you want to close counting for selected voucher? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                    {
                        if (!string.IsNullOrEmpty(cmbClosingVoucher.Text))
                        {
                            CloseCounting();
                            loadVoucher();
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a counting close by person and Closing voucher.");
                btnCountingClosedBy.Focus();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsThereAnyNonClosedVoucher(string sProdType, string sArtCode)
        {
            bool bResult = false;
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) VOUCHERNUM  from RETAILSTOCKCOUNTHEADER where COUNTINGCLOSE=0");
            if (!string.IsNullOrEmpty(sProdType))
                commandText.Append(" AND PRODUCTTYPE='" + sProdType + "'");
            if (!string.IsNullOrEmpty(sArtCode))
                commandText.Append(" AND  ARTICLE ='" + sArtCode + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (!string.IsNullOrEmpty(sResult))
                bResult = true;
            else
                bResult = false;

            return bResult;

        }

        private DataTable GetArticleList()  // SKU allow
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            DataTable dt = new DataTable();
            string commandText = "select DISTINCT ARTICLE_CODE   from INVENTTABLE where ARTICLE_CODE !=''";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;
        }

        private void btnArticleSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbProdType.Text))
            {
                DataTable dtArticle = new DataTable();
                dtArticle = GetArticleList();

                Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dtArticle, null, "Article Search");
                Osearch.ShowDialog();

                DataRow dr = Osearch.SelectedDataRow;

                if (dr != null)
                {
                    txtArticle.Text = Convert.ToString(dr["ARTICLE_CODE"]);
                }
            }
            else
            {
                MessageBox.Show("Please select a product type first.");
                cmbProdType.Focus();
            }
        }
    }
}
